#define SUPRESS_HASH_CALCULATION

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using DotaHIT.Jass.Types;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Constants;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Extras;
using DotaHIT.Core;

namespace Deerchao.War3Share.W3gParser
{
    public class Replay : IReplay
    {
        static readonly string HeaderString = "Warcraft III recorded game\u001A\0";
        static readonly int DebugInventoryPlayerSlot = 0;

        private bool isPreview = false;
#if !SUPRESS_HASH_CALCULATION 
        private long hash;
#endif
        private string filename;
        private ParseSettings parseSettings;
        private MapInfo map;
        private ReplayMapCache mapCache;
        private int length;
        private int version;
        private short buildNo;
        private GameType type;
        private string name;
        private Player host;
        private string hostName;
        private string saverName;
        private GameSettings settings;
        private string gameMode = string.Empty;
        private List<PickInfo> bans = new List<PickInfo>();
        private List<PickInfo> picks = new List<PickInfo>();
        private readonly List<Team> teams = new List<Team>();
        private readonly List<IPlayer> players = new List<IPlayer>();
        private readonly List<ChatInfo> chats = new List<ChatInfo>();
        private readonly List<KillInfo> kills = new List<KillInfo>();
        Dictionary<int, Dictionary<string, int>> dcShopItemsStockRegen = new Dictionary<int, Dictionary<string, int>>();
        internal Dictionary<int, string> dcObjectsCodeIDs = new Dictionary<int, string>();
        internal Dictionary<int, Hero> dcHeroCache = new Dictionary<int, Hero>();
        internal int[] playerSelectionUnignoreTime = new int[16];

        private long actionListOffset;
        internal long ActionListOffset
        {
            get { return actionListOffset; }
        }

        private readonly long size;
        public long Size
        {
            get { return size; }
        }

        public string FileName
        {
            get { return filename; }
        }

        public ParseSettings ParseSettings
        {
            get { return parseSettings; }
        }

        public Player Host
        {
            get
            {
                return host;
            }
        }

        public string HostName
        {
            get
            {
                return hostName;
            }
        }
        public string SaverName
        {
            get
            {
                return saverName;
            }
        }
        public MapInfo Map
        {
            get { return map; }
        }
        public ReplayMapCache MapCache
        {
            get { return mapCache; }
            internal set { mapCache = value; }
        }
        public string GameName
        {
            get { return name; }
        }
        public GameType GameType
        {
            get { return type; }
        }
        public TimeSpan GameLength
        {
            get { return new TimeSpan(0, 0, 0, 0, length); }
        }
        public int Version
        {
            get { return version; }
        }
        public int BuildNo
        {
            get { return buildNo; }
        }
        public string GameMode
        {
            get { return gameMode; }
        }
        public List<PickInfo> Bans
        {
            get { return bans; }
        }
        public List<PickInfo> Picks
        {
            get { return picks; }
        }
        public List<IPlayer> Players
        {
            get
            {
                return players;
            }
        }
        public List<Team> Teams
        {
            get { return teams; }
        }
        public List<ChatInfo> Chats
        {
            get { return chats; }
        }
        public List<KillInfo> Kills
        {
            get { return kills; }
        }

#if !SUPRESS_HASH_CALCULATION 
        public long Hash
        {
            get { return hash; }
        }
#endif

        public bool IsPreview
        {
            get { return isPreview; }
        }

        public object Tag
        {
            get;
            set;
        }

        string IReplay.ReplayPath
        {
            get { return this.FileName; }
        }

        string IReplay.MapPath
        {
            get { return this.Map.Path; }
        }

        IList<IPlayer> IReplay.Players
        {
            get { return this.players; }
        }

        int IReplay.PlayerCount
        {
            get
            {
                return this.players.Count;
            }
        }

        TeamType IReplay.Winner
        {
            get
            {
                foreach (Team t in this.Teams)
                    if (t.IsWinner)
                        return t.Type;

                return TeamType.Unknown;
            }
        }

        bool IReplay.IsDota
        {
            get
            {
                return DHHELPER.IsDotaMapResources(this.mapCache.Resources);
            }
        }

#if !SUPRESS_HASH_CALCULATION 
        private BinaryWriter hashWriter;        
#endif

        public KeyValuePair<int, long> GetMapKey()
        {
            return new KeyValuePair<int, long>(version, Map.Hash);
        }

        private event MapRequiredEventHandler mapRequired;
        private void OnMapRequired()
        {
            if (mapRequired != null) mapRequired(this, EventArgs.Empty);
        }

        public Replay(string fileName)
            : this(File.OpenRead(fileName))
        {
            this.filename = fileName;
        }

        public Replay(string fileName, MapRequiredEventHandler mapRequiredEvent)
            : this(File.OpenRead(fileName), mapRequiredEvent)
        {
            this.filename = fileName;
        }

        public Replay(string fileName, ParseSettings parseSettings)
            : this(File.OpenRead(fileName), parseSettings)
        {
            this.filename = fileName;
        }

        public Replay(string fileName, MapRequiredEventHandler mapRequiredEvent, ParseSettings parseSettings)
            : this(new MemoryStream(File.ReadAllBytes(fileName)), mapRequiredEvent, parseSettings) //File.OpenRead(fileName), mapRequiredEvent, parseSettings)
        {
            this.filename = fileName;
        }

        public Replay(string fileName, bool preview)
            : this(File.OpenRead(fileName), preview)
        {
            this.filename = fileName;
        }

        public Replay(Stream stream)
        {
            mapCache = new ReplayMapCache(this);

            using (stream)
            {
                try { size = stream.Length; }
                catch (NotSupportedException)
                { }
                Load(stream);
            }
        }

        public Replay(Stream stream, MapRequiredEventHandler mapRequiredEvent)
        {
            mapCache = new ReplayMapCache(this);

            using (stream)
            {
                try { size = stream.Length; }
                catch (NotSupportedException)
                { }
                this.mapRequired += mapRequiredEvent;
                Load(stream);
            }
        }

        public Replay(Stream stream, ParseSettings parseSettings)
        {
            mapCache = new ReplayMapCache(this);

            using (stream)
            {
                try { size = stream.Length; }
                catch (NotSupportedException)
                { }
                this.parseSettings = parseSettings;
                Load(stream);
            }
        }

        public Replay(Stream stream, MapRequiredEventHandler mapRequiredEvent, ParseSettings parseSettings)
        {
            mapCache = new ReplayMapCache(this);

            using (stream)
            {
                try { size = stream.Length; }
                catch (NotSupportedException)
                { }
                this.mapRequired += mapRequiredEvent;
                this.parseSettings = parseSettings;
                Load(stream);
            }
        }

        public Replay(Stream stream, bool preview)
        {
            mapCache = new ReplayMapCache(this);

            this.isPreview = preview;

            using (stream)
            {
                try { size = stream.Length; }
                catch (NotSupportedException)
                { }

                if (preview)
                    LoadPreview(stream);
                else
                    Load(stream);
            }
        }

        private void Load(Stream stream)
        {
            MemoryStream blocksData = LoadHeader(stream);
#if !SUPRESS_HASH_CALCULATION 
            hashWriter = new BinaryWriter(new MemoryStream());
#endif
            using (BinaryReader reader = new BinaryReader(blocksData))
            {
                LoadPlayers(reader);

                // replay preview
                if (this.parseSettings.Preview != null)
                {
                    ReplayPreviewEventArgs args = new ReplayPreviewEventArgs();
                    this.parseSettings.Preview(this, args);

                    if (args.CancelParsing)
                    {
                        isPreview = true;
                        return;
                    }
                }

                this.OnMapRequired(); // load map database event

                // replay map preview
                if (this.parseSettings.MapPreview != null)
                {
                    ReplayPreviewEventArgs args = new ReplayPreviewEventArgs();
                    this.parseSettings.MapPreview(this, args);

                    if (args.CancelParsing)
                    {
                        isPreview = true;
                        return;
                    }
                }

                if (parseSettings.EmulateInventory && mapCache.hpcComplexItems != null)
                {
                    // convert all string values to List<string>
                    foreach (HabProperties hps in mapCache.hpcComplexItems)
                        for (int i = 0; i < hps.Count; i++)
                            hps.GetStringListValue(i.ToString(), true);
                }

                this.actionListOffset = reader.BaseStream.Position;

                LoadActions(reader);
            }
            //strangely, I found a ladder replay with -49 seconds length..
            if (length < 0)
            {
                foreach (Player player in players)
                {
                    if (player.Time > length)
                        length = player.Time;
                }
            }

            // end research sessions for all players
            foreach (Player player in players)
            {
                player.State.EndResearch();
                /*Console.WriteLine("Player: " + player.Name);
                string outputA = "";
                for(int i = 0; i <6; i++)
                    outputA += DHJassInt.int2id(player.getGCValue("8_" + i)) + ", ";
                Console.WriteLine(outputA); 
                string outputB = "";
                for (int i = 0; i < player.Inventory.Slots.Count; i++)
                    outputB += player.Inventory.Slots[i]+", ";
                Console.WriteLine(outputB);*/
            }

            // fix player slots for -sp mode
            this.SpModeFix();

            this.DetectBansAndPicks();

#if !SUPRESS_HASH_CALCULATION 
            hashWriter.Write(version);
            hashWriter.Write(map.Hash);
            hashWriter.Flush();

            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hashedData = provider.ComputeHash(((MemoryStream)hashWriter.BaseStream).ToArray());
            hashWriter.Close();
            hash = 0;
            for (int i = 4; i < 12; i++)
            {
                hash = hash << 8;
                hash |= hashedData[i];
            }
#endif
        }

        private void LoadPreview(Stream stream)
        {
            MemoryStream blocksData = LoadHeader(stream);
            using (BinaryReader reader = new BinaryReader(blocksData, System.Text.Encoding.Default))
            {
                LoadPlayers(reader);
            }
        }

        private void LoadActions(BinaryReader reader)
        {
            int time = 0;
            bool isPaused = false;
            while (reader.BaseStream.Length - reader.BaseStream.Position > 0)
            {
                try
                {
                    byte blockId = reader.ReadByte();
                    switch (blockId)
                    {
                        case 0x1A:
                        case 0x1B:
                        case 0x1C:
                            reader.BaseStream.Seek(reader.BaseStream.Position + 4, SeekOrigin.Begin);
                            break;
                        case 0x22:
                            reader.BaseStream.Seek(reader.BaseStream.Position + 5, SeekOrigin.Begin);
                            break;
                        case 0x23:
                            reader.BaseStream.Seek(reader.BaseStream.Position + 10, SeekOrigin.Begin);
                            break;
                        case 0x2F:
                            reader.BaseStream.Seek(reader.BaseStream.Position + 8, SeekOrigin.Begin);
                            break;
                        case 0x30: // 3 bytes (unknown & undocumented)
                            reader.BaseStream.Seek(reader.BaseStream.Position + 3, SeekOrigin.Begin);
                            break;
                        //leave game
                        case 0x17:
                            int reason = reader.ReadInt32();
                            byte playerId = reader.ReadByte();
                            Player p = GetPlayerById(playerId);
                            p.Time = time;
                            if (reason == 0x0C) saverName = p.Name;
                            reader.ReadInt64();
                            chats.Add(new ChatInfo(time, p, TalkTo.System, null, "leave"));
                            p.Disconnected = true;
                            break;
                        //chat
                        case 0x20:
                            byte fromId = reader.ReadByte();
                            reader.ReadBytes(2);
                            byte chatType = reader.ReadByte();
                            TalkTo to = TalkTo.All;
                            if (chatType != 0x10)
                            {
                                to = (TalkTo)reader.ReadInt32();
                            }
                            string message = ParserUtility.ReadString(reader);
                            if (chatType != 0x10)
                            {
                                ChatInfo chat = new ChatInfo(time, GetPlayerById(fromId), to, GetPlayerById((byte)(to - 3)), message);
                                chats.Add(chat);
                            }
                            break;
                        //time slot
                        case 0x1E:
                        case 0x1F:
                            short rest = reader.ReadInt16();
                            short increasedTime = reader.ReadInt16();
                            if (!isPaused)
                                time += increasedTime;
                            rest -= 2;
                            LoadTimeSlot(reader, rest, time, ref isPaused);
                            break;
                        case 0:
                            return;
                        case 0x01: // 0 bytes (unknown & undocumented)                       
                            break;
                        default:
                            continue;//bypass error
                                     //throw new W3gParserException("Unknown Block ID:" + blockId);
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private void LoadTimeSlot(BinaryReader reader, short rest, int time, ref bool isPaused)
        {
            bool wasDeselect;

            //#pragma warning disable TooWideLocalVariableScope
            short flag;
            uint itemId;
            float x;
            float y;
            int objectId1;
            int objectId2;
            uint itemId1;
            uint itemId2;
            float x2;
            float y2;
            short unitCount;
            byte groupNo;
            byte slotNo;
            int len;
            string gamecache;
            string missonKey;
            string key;
            int value;
            List<int> units;

            //#pragma warning restore TooWideLocalVariableScope

            while (rest > 0)
            {
                // reset deselect state
                wasDeselect = false;

                byte playerId = reader.ReadByte();
                Player player = GetPlayerById(playerId);
                if (player == null)
                    continue;
                player.Time = time;
                short playerBlockRest = reader.ReadInt16();
                rest -= 3;
                short prest = playerBlockRest;

                while (prest > 0)
                {
                    try
                    {
                        #region
                        byte actionId = reader.ReadByte();
                        switch (actionId)
                        {
                            //pause game
                            case 0x01:
                                isPaused = true;
                                chats.Add(new ChatInfo(time, player, TalkTo.System, null, "pause"));
                                prest--;
                                break;
                            //resume game
                            case 0x02:
                                isPaused = false;
                                chats.Add(new ChatInfo(time, player, TalkTo.System, null, "resume"));
                                prest--;
                                break;
                            //set game speed
                            case 0x03:
                                prest -= 2;
                                break;
                            //icrease, decrease game speed
                            case 0x04:
                            case 0x05:
                                prest--;
                                break;
                            //save game
                            case 0x06:
                                len = 0;
                                while (reader.ReadByte() != 0)
                                    len++;
                                chats.Add(new ChatInfo(time, player, TalkTo.System, null, "save"));
                                prest -= (short)(len + 2);
                                break;
                            //game saved
                            case 0x07:
                                reader.ReadInt32();
                                prest -= 5;
                                break;
                            //unit ability without target
                            case 0x10:
                                flag = reader.ReadInt16();
                                itemId = reader.ReadUInt32();
                                //unknownA, unknownB
                                reader.ReadInt64();

                                player.ActionsCount++;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(flag);
                            hashWriter.Write(itemId);
#endif

                                OrderItem(player, itemId, time, 0, 0, -1, -1);
                                prest -= 15;
                                break;
                            //unit ability with target position
                            case 0x11:
                                flag = reader.ReadInt16();
                                itemId = reader.ReadUInt32();
                                //unknownA, unknownB
                                reader.ReadInt64();
                                x = reader.ReadSingle();
                                y = reader.ReadSingle();

                                player.ActionsCount++;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(flag);
                            hashWriter.Write(itemId);
                            hashWriter.Write(x);
                            hashWriter.Write(y);
#endif

                                OrderItem(player, itemId, time, x, y, -1, -1);
                                prest -= 23;
                                break;
                            //unit ability with target position and target object
                            case 0x12:
                                flag = reader.ReadInt16();
                                itemId = reader.ReadUInt32();
                                //unknownA, unknownB
                                reader.ReadInt64();
                                x = reader.ReadSingle();
                                y = reader.ReadSingle();
                                objectId1 = reader.ReadInt32();
                                objectId2 = reader.ReadInt32();

                                player.ActionsCount++;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(flag);
                            hashWriter.Write(itemId);
                            hashWriter.Write(x);
                            hashWriter.Write(y);
                            hashWriter.Write(objectId1);
                            hashWriter.Write(objectId2);
#endif
                                OrderItem(player, itemId, time, x, y, objectId1, objectId2);
                                prest -= 31;
                                break;
                            //unit ability with target position, target object, and target item (give item action)
                            case 0x13:
                                flag = reader.ReadInt16();
                                itemId = reader.ReadUInt32();
                                //unknownA, unknownB
                                reader.ReadInt64();
                                x = reader.ReadSingle();
                                y = reader.ReadSingle();
                                objectId1 = reader.ReadInt32();
                                objectId2 = reader.ReadInt32();
                                itemId1 = reader.ReadUInt32();
                                itemId2 = reader.ReadUInt32();

                                player.ActionsCount++;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(flag);
                            hashWriter.Write(itemId);
                            hashWriter.Write(x);
                            hashWriter.Write(y);
                            hashWriter.Write(objectId1);
                            hashWriter.Write(objectId2);
                            hashWriter.Write(itemId1);
                            hashWriter.Write(itemId2);
#endif
                                prest -= 39;
                                break;
                            //unit ability with two target positions and two item IDs
                            case 0x14:
                                flag = reader.ReadInt16();
                                itemId = reader.ReadUInt32();
                                //unknownA, unknownB
                                reader.ReadInt64();
                                x = reader.ReadSingle();
                                y = reader.ReadSingle();
                                itemId1 = reader.ReadUInt32();
                                reader.ReadBytes(9);
                                x2 = reader.ReadSingle();
                                y2 = reader.ReadSingle();

                                player.ActionsCount++;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(flag);
                            hashWriter.Write(itemId);
                            hashWriter.Write(x);
                            hashWriter.Write(y);
                            hashWriter.Write(itemId1);
                            hashWriter.Write(x2);
                            hashWriter.Write(y2);
#endif
                                Order(player, itemId, time, x, y, itemId1, x2, y2);
                                prest -= 44;
                                break;
                            //change selection
                            case 0x16:
                                byte selectMode = reader.ReadByte();
                                unitCount = reader.ReadInt16();
                                //object ids
                                //reader.ReadBytes(unitCount * 8);
                                for (int i = 0; i < unitCount; i++)
                                {
                                    objectId1 = reader.ReadInt32();
                                    objectId2 = reader.ReadInt32();

                                    if (selectMode != 2)
                                    {
                                        if (playerSelectionUnignoreTime[player.Id] <= time || !dcHeroCache.ContainsKey(objectId1))
                                            player.State.CurrentSelection.Add(objectId1);
                                    }
                                    else
                                        player.State.CurrentSelection.Remove(objectId1);
                                }

                                //if is deselect
                                if (selectMode == 2)
                                {
                                    wasDeselect = true;
                                    player.ActionsCount++;
                                }
                                else
                                {
                                    if (!wasDeselect)
                                        player.ActionsCount++;
                                    wasDeselect = false;
                                }
                                player.Units.Multiplier = unitCount;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(selectMode);
                            hashWriter.Write(unitCount);
#endif
                                prest -= (short)(unitCount * 8 + 4);
                                break;
                            //create group
                            case 0x17:
                                groupNo = reader.ReadByte();
                                unitCount = reader.ReadInt16();
                                //unit ids
                                //reader.ReadBytes(unitCount * 8);
                                units = new List<int>(unitCount);
                                for (int i = 0; i < unitCount; i++)
                                {
                                    objectId1 = reader.ReadInt32();
                                    objectId2 = reader.ReadInt32();

                                    RefreshHeroOwner(player, time, objectId1);

                                    units.Add(objectId1);
                                }

                                player.ActionsCount++;
                                player.Groups.SetGroup(groupNo, units);
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(groupNo);
                            hashWriter.Write(unitCount);
#endif
                                prest -= (short)(unitCount * 8 + 4);
                                break;
                            //select group
                            case 0x18:
                                groupNo = reader.ReadByte();
                                //unknown
                                reader.ReadByte();

                                player.State.CurrentSelection = new List<int>(player.Groups[groupNo]);

                                player.ActionsCount++;
                                player.Units.Multiplier = (short)player.Groups[groupNo].Count;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(groupNo);
#endif
                                prest -= 3;
                                break;
                            //select sub group
                            case 0x19:
                                //itemId, objectId1, objectId2
                                itemId = reader.ReadUInt32();
                                objectId1 = reader.ReadInt32();
                                reader.ReadInt32();

                                string codeID = DHJassInt.int2id(itemId);

                                // write this objectID-codeID pair to cache
                                if (!dcObjectsCodeIDs.ContainsKey(objectId1) || dcObjectsCodeIDs[objectId1] != codeID)
                                {
                                    dcObjectsCodeIDs[objectId1] = codeID;

                                    if (this.mapCache.dcHeroesTaverns.ContainsKey(codeID))
                                    {
                                        Hero cachedHero;
                                        if (!dcHeroCache.TryGetValue(objectId1, out cachedHero) || cachedHero.Name != codeID)
                                            dcHeroCache[objectId1] = new Hero(this.mapCache, codeID);
                                    }
                                }

                                // update current selection for player
                                //player.State.CurrentSelection = objectId1;

                                //no way to find how many buildings is in a subgroup..
                                player.Units.Multiplier = 1;
                                prest -= 13;
                                break;
                            //pre select sub group
                            case 0x1A:
                                prest--;
                                break;
                            //unknown
                            case 0x1B:
                                //unknown, objectid1, objectid2
                                reader.ReadByte();
                                reader.ReadInt64();
                                prest -= 10;
                                break;
                            //select ground item
                            case 0x1C:
                                //unknown, objectid1, objectid2
                                reader.ReadByte();
                                reader.ReadInt64();

                                player.ActionsCount++;
                                prest -= 10;
                                break;
                            //cancel hero revival
                            case 0x1D:
                                reader.ReadInt64();
                                player.ActionsCount++;
                                prest -= 9;
                                break;
                            //remove unit from order queue
                            case 0x1E:
                                slotNo = reader.ReadByte();
                                itemId = reader.ReadUInt32();

                                player.ActionsCount++;
#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(slotNo);
                            hashWriter.Write(itemId);
#endif
                                CancelItem(player, itemId, time);
                                prest -= 6;
                                break;
                            //unknown
                            case 0x21:
                                reader.ReadInt64();
                                prest -= 9;
                                break;
                            //cheats
                            case 0x20:
                            case 0x22:
                            case 0x23:
                            case 0x24:
                            case 0x25:
                            case 0x26:
                            case 0x29:
                            case 0x2A:
                            case 0x2B:
                            case 0x2C:
                            case 0x2F:
                            case 0x30:
                            case 0x31:
                            case 0x32:
                                prest--;
                                break;
                            //cheats
                            case 0x27:
                            case 0x28:
                            case 0x2D:
                                reader.ReadByte();
                                reader.ReadInt32();
                                prest -= 6;
                                break;
                            //cheats
                            case 0x2E:
                                reader.ReadInt32();
                                prest -= 5;
                                break;
                            //change ally option
                            case 0x50:
                                slotNo = reader.ReadByte();
                                int flags = reader.ReadInt32();
                                prest -= 6;

                                if ((flags & 0x40) != 0) // if shared unit control is enabled                                                            
                                    player.PlayersWithSharedControl.Add(GetPlayerBySpecialSlot(slotNo));
                                else
                                    player.PlayersWithSharedControl.Remove(GetPlayerBySpecialSlot(slotNo));
                                break;
                            //transfer resource
                            case 0x51:
                                slotNo = reader.ReadByte();
                                int gold = reader.ReadInt32();
                                int lumber = reader.ReadInt32();

#if !SUPRESS_HASH_CALCULATION
                            hashWriter.Write(time);
                            hashWriter.Write(actionId);
                            hashWriter.Write(slotNo);
                            hashWriter.Write(gold);
                            hashWriter.Write(lumber);
#endif
                                prest -= 10;
                                break;
                            //trigger chat
                            case 0x60:
                                //unknownA, unknownB                            
                                //byte[] array = reader.ReadBytes(8);
                                reader.ReadInt64();
                                key = ParserUtility.ReadString(reader, out len);

                                if (player == host || player.Color == PlayerColor.Blue)
                                    DetectGameMode(key, time);

                                // try to handle most cases with duplicate chat messages
                                switch (chats.Count)
                                {
                                    case 0:
                                        chats.Add(new ChatInfo(time, player, TalkTo.Allies, null, key));
                                        break;

                                    case 1:
                                        if (!chats[0].IsClone(time, player, key))
                                            chats.Add(new ChatInfo(time, player, TalkTo.Allies, null, key));
                                        break;

                                    case 2:
                                        if (!chats[1].IsClone(time, player, key) && !chats[0].IsClone(time, player, key))
                                            chats.Add(new ChatInfo(time, player, TalkTo.Allies, null, key));
                                        break;

                                    default:
                                        if (!chats[chats.Count - 1].IsClone(time, player, key) && !chats[chats.Count - 2].IsClone(time, player, key) && !chats[chats.Count - 3].IsClone(time, player, key))
                                            chats.Add(new ChatInfo(time, player, TalkTo.Allies, null, key));
                                        break;
                                }

                                prest -= (short)(9 + len);
                                break;
                            //esc pressed
                            case 0x61:
                                // exit research mode for this player
                                if (player.State.IsResearching)
                                    player.State.EndResearch();
                                player.ActionsCount++;
                                prest--;
                                break;
                            //Scenario Trigger
                            case 0x62:
                                //unknownABC
                                reader.ReadInt32();
                                reader.ReadInt64();

                                prest -= 13;
                                break;
                            //begin choose hero skill
                            case 0x66:
                                // end any previous research session
                                player.State.EndResearch();
                                // make sure that there is a hero in player's current selection
                                Hero hero;
                                if (!MakeSureHeroExists(player, time, out hero))
                                    Console.WriteLine("No hero selected for choose skill action!");
                                else
                                    player.State.BeginResearch(hero);
                                player.ActionsCount++;
                                prest--;
                                break;
                            //begin choose building
                            case 0x67:
                                player.ActionsCount++;
                                prest--;
                                break;
                            //ping mini map
                            case 0x68:
                                //x, y
                                reader.ReadInt64();
                                //unknown
                                reader.ReadInt32();

                                prest -= 13;
                                break;
                            //continue game
                            case 0x69:
                            case 0x6A:
                                reader.ReadInt64();
                                reader.ReadInt64();
                                prest -= 17;
                                break;
                            // SyncStoredInteger actions
                            case 0x6B:
                                gamecache = ParserUtility.ReadString(reader);
                                missonKey = ParserUtility.ReadString(reader);
                                key = ParserUtility.ReadString(reader);
                                value = reader.ReadInt32();
                                //Console.WriteLine("Sync: [" + gamecache + "][" + missonKey + "][" + key + "]=" + value);
                                prest -= (short)((gamecache.Length + 1) + (missonKey.Length + 1) + (key.Length + 1) + 4 + 1);
                                try
                                {
                                    switch (missonKey)
                                    {
                                        //case "0":
                                        case "1":
                                        case "2":
                                        case "3":
                                        case "4":
                                        case "5":
                                            int slot = int.Parse(missonKey);
                                            Player p = GetPlayerBySlot(slot - 1);
                                            if (p != null)
                                            {
                                                p.gameCacheValues[key] = value;
                                                if (key == "9")
                                                {
                                                    if (p.PostPickTime == 0) p.PostPickTime = time;

                                                    // AI_MAP // order hero for computer player
                                                    if (p.IsComputer)
                                                    {
                                                        OrderItem(p, (uint)value, time, 0, 0, 0, 0);
                                                        if (p.UsedHeroes.Count > 0)
                                                            p.IncreaseHeroUseCount(p.GetMostUsedHero());
                                                    }
                                                }
                                            }
                                            break;
                                        case "7":
                                        case "8":
                                        case "9":
                                        case "10":
                                        case "11":
                                            slot = int.Parse(missonKey);
                                            p = GetPlayerBySlot(slot - 2);
                                            if (p != null)
                                            {
                                                p.gameCacheValues[key] = value;
                                                if (key == "9")
                                                {
                                                    if (p.PostPickTime == 0) p.PostPickTime = time;

                                                    // AI_MAP // order hero for computer player
                                                    if (p.IsComputer)
                                                    {
                                                        OrderItem(p, (uint)value, time, 0, 0, 0, 0);
                                                        if (p.UsedHeroes.Count > 0)
                                                            p.IncreaseHeroUseCount(p.GetMostUsedHero());
                                                    }
                                                }
                                            }
                                            break;
                                        case "Global":
                                            if (key == "Winner")
                                                this.GetTeamByType((TeamType)value).IsWinner = true;
                                            break;
                                        case "Data":
                                            // hero death
                                            if (key.StartsWith("Hero"))
                                            {
                                                int deadId = int.Parse(key.Substring(4));
                                                Player victim = GetPlayerBySpecialSlot(deadId);
                                                Player killer = GetPlayerBySpecialSlot(value);

                                                int gcValue;
                                                victim.gameCacheValues.TryGetValue("deaths", out gcValue);
                                                victim.gameCacheValues["deaths"] = gcValue + 1;

                                                // if killer is a player
                                                if (killer != null)
                                                {
                                                    killer.gameCacheValues.TryGetValue("kills", out gcValue);
                                                    killer.gameCacheValues["kills"] = gcValue + 1;
                                                }

                                                kills.Add(new KillInfo(time, killer, victim));
                                            }
                                            else
                                                // player disconnected
                                                if (key.StartsWith("CK"))
                                            {
                                                int deniesIndex = key.IndexOf('D');
                                                int neutralsIndex = key.IndexOf('N');

                                                int creepKills = int.Parse(key.Substring(2, deniesIndex - 2));
                                                int creepDenies = int.Parse(key.Substring(deniesIndex + 1, neutralsIndex - deniesIndex - 1));

                                                Player leavingPlayer = GetPlayerBySpecialSlot(value);
                                                if (leavingPlayer != null)
                                                {
                                                    leavingPlayer.gameCacheValues["creeps"] = creepKills;
                                                    leavingPlayer.gameCacheValues["denies"] = creepDenies;
                                                }
                                            }
                                            else
                                                    if (key.StartsWith("Mode"))
                                                this.gameMode = key.Substring(4);
                                            else
                                                        // pickup item
                                                        if (key.StartsWith("PUI_"))
                                            {
                                                if (parseSettings.EmulateInventory == false || value == 0) break;

                                                int owningPlayerId = int.Parse(key.Substring(4));
                                                Player owningPlayer = GetPlayerBySpecialSlot(owningPlayerId);

                                                string itemID = DHJassInt.int2id(value);

#if DEBUG
                                                if (owningPlayerId == DebugInventoryPlayerSlot)
                                                {
                                                    Console.WriteLine(key + " " + mapCache.hpcItemProfiles.GetStringValue(itemID, "Name"));
                                                }
#endif

                                                Hero recentlyUsedHero = owningPlayer.GetMostRecentlyUsedHero();
                                                if (recentlyUsedHero != null) recentlyUsedHero.Inventory.PutItem(itemID);
                                                else owningPlayer.OrphanInventory.PutItem(itemID);
                                            }
                                            else
                                                            // drop item
                                                            if (key.StartsWith("DRI_"))
                                            {
                                                if (parseSettings.EmulateInventory == false || value == 0) break;

                                                int owningPlayerId = int.Parse(key.Substring(4));
                                                Player owningPlayer = GetPlayerBySpecialSlot(owningPlayerId);

                                                string itemID = DHJassInt.int2id(value);

#if DEBUG
                                                if (owningPlayerId == DebugInventoryPlayerSlot)
                                                {
                                                    Console.WriteLine(key + " " + mapCache.hpcItemProfiles.GetStringValue(itemID, "Name"));
                                                }
#endif

                                                Hero recentlyUsedHero = owningPlayer.GetMostRecentlyUsedHero();
                                                if (recentlyUsedHero != null) recentlyUsedHero.Inventory.DropItem(itemID);
                                                else owningPlayer.OrphanInventory.DropItem(itemID);
                                            }
                                            else
                                                                // level up
                                                                if (key.StartsWith("Level"))
                                            {
                                                int newLevel = int.Parse(key.Substring(5));

                                                Player owningPlayer = GetPlayerBySpecialSlot(value);
                                                owningPlayer.gameCacheValues["Level"] = newLevel;
                                            }
                                            else
                                                                    if (key.StartsWith("RuneUse"))
                                            {
                                                Player runeUser = GetPlayerBySpecialSlot(value);
                                                runeUser.gameCacheValues[key] = runeUser.getGCValue(key) + 1;
                                            }
                                            else
                                                                        // bans
                                                                        if (key.StartsWith("Ban"))
                                            {
                                                // [6.58]Doc: - "Ban",playerid,heroid: is sent for each time a hero is banned in cm/xl 
                                                bans.Add(new PickInfo(time, key.Contains("1") ? TeamType.Sentinel : TeamType.Scourge, new Hero(mapCache, DHJassInt.int2id(value))));
                                                /*for (char c = 'A'; c <= 'Z'; c++)
                                                {
                                                    string keyHeader = "Ban"+c;
                                                    bool exists = false;
                                                    foreach(string gcKey in player.gameCacheValues.Keys)
                                                        if (gcKey.StartsWith(keyHeader))
                                                        {
                                                            exists = true;
                                                            break;
                                                        }

                                                    if (!exists)
                                                    {
                                                        player.gameCacheValues[key.Replace("Ban", keyHeader)] = value;
                                                        break;
                                                    }
                                                }*/
                                            }
                                            else // picks
                                                                            if (key.StartsWith("Pick"))
                                            {
                                                picks.Add(new PickInfo(time, key.Contains("1") ? TeamType.Sentinel : TeamType.Scourge, new Hero(mapCache, DHJassInt.int2id(value))));
                                                /*for (char c = 'A'; c <= 'Z'; c++)
                                                {
                                                    string keyHeader = "Pick" + c;
                                                    bool exists = false;
                                                    foreach (string gcKey in player.gameCacheValues.Keys)
                                                        if (gcKey.StartsWith(keyHeader))
                                                        {
                                                            exists = true;
                                                            break;
                                                        }

                                                    if (!exists)
                                                    {
                                                        player.gameCacheValues[key.Replace("Pick", keyHeader)] = value;
                                                        break;
                                                    }
                                                }*/
                                            }
                                            else
                                                                                // towers
                                                                                if (key.StartsWith("Tower"))
                                            {
                                                char towerTeam = key[5];
                                                char towerLevel = key[6];
                                                char towerLane = key[7];

                                                Player killer = GetPlayerBySpecialSlot(value);
                                                if (killer != null)
                                                {
                                                    string gcKey = towerTeam == '0' ? "TowerKill0" : "TowerKill1";
                                                    int gcValue;

                                                    killer.gameCacheValues.TryGetValue(gcKey, out gcValue);
                                                    killer.gameCacheValues[gcKey] = gcValue + 1;

                                                    string description = towerTeam == '0' ? "Sentinel Tower" : "Scourge Tower";
                                                    description += " " + towerLevel;

                                                    switch (towerLane)
                                                    {
                                                        case '0':
                                                            description += " (Top) ";
                                                            break;

                                                        case '1':
                                                            if (towerLevel != '4')
                                                                description += " (Middle) ";
                                                            break;

                                                        case '2':
                                                            description += " (Bottom) ";
                                                            break;
                                                    }

                                                    this.kills.Add(new KillInfo(
                                                        time,
                                                        killer,
                                                        new UnitInfo(
                                                            towerTeam == '0' ? TeamType.Sentinel : TeamType.Scourge,
                                                            description)));
                                                }
                                            }
                                            else
                                                                                    // rax
                                                                                    if (key.StartsWith("Rax"))
                                            {
                                                char raxTeam = key[3];
                                                char raxLane = key[4];
                                                char raxType = key[5];

                                                Player killer = GetPlayerBySpecialSlot(value);
                                                if (killer != null)
                                                {
                                                    string description = raxTeam == '0' ? "Sentinel Barrack" : "Scourge Barrack";

                                                    switch (raxLane)
                                                    {
                                                        case '0':
                                                            description += " (Top) ";
                                                            break;

                                                        case '1':
                                                            description += " (Middle) ";
                                                            break;

                                                        case '2':
                                                            description += " (Bottom) ";
                                                            break;
                                                    }

                                                    description += (raxType == '0' ? "(Melee)" : "(Range)");

                                                    this.kills.Add(new KillInfo(
                                                        time,
                                                        killer,
                                                        new UnitInfo(
                                                            raxTeam == '0' ? TeamType.Sentinel : TeamType.Scourge,
                                                            description)));
                                                }
                                            }
                                            else
                                                                                        // [6.65]Doc: Added hero pool information to replays for -CD mode (Format: Pool+index,id)
                                                                                        if (key.StartsWith("Pool"))
                                                player.gameCacheValues[key] = value;
                                            else
                                                                                            // AI_MAP
                                                                                            if (key.StartsWith("Learn"))
                                            {
                                                slot = int.Parse(key.Substring(5));
                                                p = GetPlayerBySlot(slot > 5 ? (slot - 2) : (slot - 1));
                                                if (p != null && p.IsComputer)
                                                {
                                                    hero = p.GetMostUsedHero();
                                                    if (hero != null)
                                                    {
                                                        p.State.BeginResearch(hero);
                                                        OrderItem(p, (uint)value, time, 0, 0, 0, 0);
                                                        p.State.EndResearch();
                                                    }
                                                }
                                            }

                                            break;
                                    }
                                }
                                catch
                                {

                                }
                                break;
                            case 0x6C:
                                gamecache = ParserUtility.ReadString(reader);
                                missonKey = ParserUtility.ReadString(reader);
                                key = ParserUtility.ReadString(reader);
                                value = reader.ReadInt32();
                                prest -= (short)((gamecache.Length + 1) + (missonKey.Length + 1) + (key.Length + 1) + 4 + 1);
                                break;
                            case 0x70:
                                gamecache = ParserUtility.ReadString(reader);
                                missonKey = ParserUtility.ReadString(reader);
                                key = ParserUtility.ReadString(reader);
                                prest -= (short)((gamecache.Length + 1) + (missonKey.Length + 1) + (key.Length + 1) + 1);
                                break;
                            case 0x75:
                                reader.ReadByte();
                                prest -= 2;
                                break;
                            default:
                                if (actionId == 0)
                                {
                                    prest--;
                                    byte[] bskipped = reader.ReadBytes(prest);
                                    prest = 0;
                                    Console.WriteLine("Warning: ActionID==0 found. Skipping remaining " + bskipped.Length + " bytes in the timeslot...");
                                }
                                else
                                    continue; // bypasserror
                                              //throw new W3gParserException("Unknown ActionID: " + actionId + " (0x"+actionId.ToString("X")+")");
                                break;
                        }
                        if (actionId != 0x16) wasDeselect = false; // reset deselect state
                        #endregion
                    }
                    catch
                    {
                        break;
                    }
                }
                rest -= playerBlockRest;
            }
        }

        private void CancelItem(Player player, uint itemId, int time)
        {
            if (itemId < 0x41000000 || itemId > 0x7a000000)
                return;
            string stringId = DHJassInt.int2id(itemId);  //ParserUtility.StringFromUInt(itemId);
            switch (ParserUtility.ItemTypeFromId(this.mapCache, stringId))
            {
                case ItemType.None:
                    break;
                case ItemType.Hero:
                    player.Heroes.Cancel(stringId, time);
                    break;
                case ItemType.Building:
                    player.Buildings.Cancel(new OrderItem(stringId, time, true)); ;
                    break;
                case ItemType.Research:
                    player.Researches.Cancel(new OrderItem(stringId, time, true)); ;
                    break;
                case ItemType.Unit:
                    player.Units.Cancel(new OrderItem(stringId, time, true)); ;
                    break;
                case ItemType.Upgrade:
                    player.Upgrades.Cancel(new OrderItem(stringId, time, true)); ;
                    break;
            }
        }

        private void OrderItem(Player player, uint itemId, int time, float x, float y, int objectID1, int objectID2)
        {
            // if this is orderID (not an objectID)
            if ((itemId >> 16) == 0x000D)
            {
                OrderID orderId = (OrderID)itemId;

                switch (orderId)
                {
                    case (OrderID)0x000D0003: // rightclick
                        player.Actions.Add(new PlayerAction(
                            PlayerActionType.RightClick,
                            x, y,
                            time,
                            objectID1, objectID2));
                        break;

                    case OrderID.attack: // attack
                        player.Actions.Add(new PlayerAction(
                            PlayerActionType.Attack,
                            x, y,
                            time,
                            objectID1, objectID2));
                        break;

                    case OrderID.dropitem: // give away item to unit (or drop on ground)
                        break;

                    case OrderID.elementalfury: // pandaren brewmaster ulti
                        playerSelectionUnignoreTime[player.Id] = time + 1500; // 1.5 sec 
                        break;
                }

                // if this action is a unit-command
                if (orderId <= OrderID.useslot6)
                {
                    // then it can be used to derive controlled units
                    Hero hero;
                    if (MakeSureHeroExists(player, time, out hero))
                    {
                        // if this command has target location
                        if (x != 0 && y != 0)
                            hero.IssueOrder(time, player, x, y);
                    }
                    else
                        if (player.State.CurrentSelection.Count == 0)
                        Console.WriteLine("No unit selected for unit-command action!");
                }

                return;
            }

            string stringId = DHJassInt.int2id(itemId);//ParserUtility.StringFromUInt(itemId);
            ItemType itemType = ParserUtility.ItemTypeFromId(this.mapCache, stringId);
            switch (itemType)
            {
                case ItemType.None:
                    if ((itemId >> 16) == 0x00000233)
                        player.Units.Order(new OrderItem("ubsp", time));
                    break;
                case ItemType.Hero:
                    player.Heroes.Order(stringId, time);
                    if (parseSettings.EmulateInventory)
                        player.Heroes[player.Heroes.Count - 1].Inventory.RefreshOwner(player); // refresh items owner
                    break;
                case ItemType.HeroAbility:
                    Hero hero;
                    if (player.State.IsResearching
                        && (TryFindHeroByCache(player.State.CurrentSelection, out hero) || (hero = player.GetMostUsedHero()) != null)
                        && hero.Train(stringId, time, player.State.MaxAllowedLevelForResearch))// player.Heroes.Train(stringId, time))
                    {
                        player.State.CompleteResearch();
                    }
                    break;
                case ItemType.Ability:
                    if (player.State.IsResearching
                        && (TryFindHeroByCache(player.State.CurrentSelection, out hero) || (hero = player.GetMostUsedHero()) != null)
                        && hero.Train(ParserUtility.GetBestMatchAbilityForHero(this.mapCache, hero, stringId, ItemType.Ability), time, player.State.MaxAllowedLevelForResearch))
                    {
                        player.State.CompleteResearch();
                    }
                    break;
                case ItemType.Building:
                    player.Buildings.Order(new OrderItem(stringId, time));
                    break;
                case ItemType.Item:
                case ItemType.NewVersionItem:
                    if (PlayerCanBuyItem(player, stringId, time))
                    {
                        BuyItemForPlayer(player, stringId, time, itemType == ItemType.NewVersionItem);

                        player.Items.Order(new OrderItem(stringId, time));
                    }

                    if (stringId == "tret")
                        player.Heroes.PossibleRetrained(time);
                    break;
                case ItemType.Unit:
                    player.Units.Order(new OrderItem(stringId, time));
                    break;
                case ItemType.Upgrade:
                    player.Upgrades.Order(new OrderItem(stringId, time)); ;
                    break;
                case ItemType.Research:
                    player.Researches.Order(new OrderItem(stringId, time)); ;
                    break;
            }
        }
        private void Order(Player player, uint itemId, int time, float x, float y, uint itemId2, float x2, float y2)
        {
            // if this is orderID (not an objectID)
            if ((itemId >> 16) == 0x000D)
            {
                Hero hero;
                if (!MakeSureHeroExists(player, time, out hero))
                {
                    if (player.State.CurrentSelection.Count == 0)
                        Console.WriteLine("No unit selected for order!");
                    return;
                }

                OrderID orderId = (OrderID)itemId;

                switch (orderId)
                {
                    case OrderID.smart: // right-click on building
                    case OrderID.attack: // attack building                        
                        hero.IssueOrder(time, player, x, y);
                        break;

                    case OrderID.useslot1:
                    case OrderID.useslot2:
                    case OrderID.useslot3:
                    case OrderID.useslot4:
                    case OrderID.useslot5:
                    case OrderID.useslot6:
                        break;

                    default:
                        break;
                }
            }
        }

        bool PlayerCanBuyItem(Player p, string itemID, int time)
        {
            List<int> currentSelection = p.State.CurrentSelection;

            // at least one shop must be selected
            if (currentSelection.Count < 1) return false;

            Dictionary<string, int> dcItemsStockRegen;
            if (!dcShopItemsStockRegen.TryGetValue(currentSelection[currentSelection.Count - 1], out dcItemsStockRegen)) return true;

            int readyTime;
            if (!dcItemsStockRegen.TryGetValue(itemID, out readyTime)) return true;

            return time > readyTime;
        }

        bool BuyItemForPlayer(Player p, string itemID, int time, bool isNewVersionItem)
        {
            List<int> currentSelection = p.State.CurrentSelection;

            // only one shop can be selected at a time
            if (currentSelection.Count < 1) return false;

            Dictionary<string, int> dcItemsStockRegen;
            if (!dcShopItemsStockRegen.TryGetValue(currentSelection[currentSelection.Count - 1], out dcItemsStockRegen))
            {
                dcItemsStockRegen = new Dictionary<string, int>();
                dcShopItemsStockRegen.Add(currentSelection[currentSelection.Count - 1], dcItemsStockRegen);
            }

            // ready_time = time + stock_regen + 350 (buy delay)
            dcItemsStockRegen[itemID] = time + ParserUtility.GetItemStockRegen(this.mapCache, itemID, isNewVersionItem) + (isNewVersionItem ? 100 : 350);

            return true;
        }

        bool TryFindHeroByCache(List<int> unitList, out Hero hero)
        {
            foreach (int objectID in unitList)
                if (dcHeroCache.TryGetValue(objectID, out hero))
                    return true;

            hero = null;
            return false;
        }
        bool TryFindHeroByCache(List<int> unitList, out Hero hero, out int heroObjectID)
        {
            foreach (int objectID in unitList)
                if (dcHeroCache.TryGetValue(objectID, out hero))
                {
                    heroObjectID = objectID;
                    return true;
                }

            heroObjectID = 0;
            hero = null;
            return false;
        }

        bool MakeSureHeroExists(Player player, int time, out Hero hero)
        {
            // check if the list of currently selected units contains objectId
            // that references hero in dcHeroCache

            int objectId1;

            if (TryFindHeroByCache(player.State.CurrentSelection, out hero, out objectId1))
            {
                // if the hero object value is empty (which means this hero hasn't been used by anyone yet)
                if (hero.ObjectId == -1)
                {
                    // get name of currently selected hero
                    string heroName = hero.Name;

                    // find hero that is owned by this player
                    // and has same name as the hero currently selected
                    Hero playerHero = player.Heroes[heroName];

                    // if this player does not own hero with specified name
                    if (playerHero == null)
                    {
                        // then use the selected hero
                        playerHero = hero;

                        // add this hero to current player
                        // (the player will own this hero from now on)
                        player.Heroes.Order(playerHero, time);

                        if (parseSettings.EmulateInventory)
                            playerHero.Inventory.RefreshOwner(player); // refresh items owner
                    }
                    else
                    {   // if this player does have a hero with same name as the selected hero,
                        // then use the player's own hero
                        hero = playerHero;

                        // record this action as hero re-order
                        hero.Order(time);
                    }

                    // assign object id to this hero
                    hero.ObjectId = objectId1;

                    // update hero-cache
                    // this ensures that objectId contained in current selection
                    // will reference the hero object that this player owns.
                    dcHeroCache[objectId1] = hero;
                }
                else
                    // if this hero has been already used by someone,
                    // check if this player doesnt have any hero,
                    // in which case add this hero to him (players probably used -swap command)
                    if (player.Heroes.Count == 0)
                {
                    player.Heroes.Order(hero, time);

                    if (parseSettings.EmulateInventory)
                        hero.Inventory.RefreshOwner(player); // refresh items owner
                }

                player.IncreaseHeroUseCount(hero);

                return true;
            }
            else
            {
                // if hero cannot be found, just try to get most used hero
                //hero = player.GetMostUsedHero();
                //return hero != null;
                hero = null;
                return false;
            }
        }
        bool RefreshHeroOwner(Player player, int time, int objectId1)
        {
            Hero hero;
            if (!dcHeroCache.TryGetValue(objectId1, out hero))
                return false;

            if (hero.ObjectId == -1)
            {
                // get name of hero to refresh owner of
                string heroName = hero.Name;

                // check if player already owns hero with that name
                Hero playerHero = player.Heroes[heroName];

                // if this player does not own hero with specified name
                if (playerHero == null)
                {
                    // then use this hero
                    playerHero = hero;

                    // add this hero to current player
                    // (the player will own this hero from now on)
                    player.Heroes.Order(playerHero, time);

                    if (parseSettings.EmulateInventory)
                        playerHero.Inventory.RefreshOwner(player); // refresh items owner
                }
                else
                {   // if this player does have a hero with same name as the this hero,
                    // then use the player's own hero
                    hero = playerHero;

                    // record this action as hero re-order
                    hero.Order(time);
                }

                // assign object id to this hero
                hero.ObjectId = objectId1;

                // update hero-cache
                // this ensures that objectId contained in current selection
                // will reference the hero object that this player owns.
                dcHeroCache[objectId1] = hero;
            }
            //else
            // if this hero has been already used by someone,                
            // ignore that and just add this hero to this player (players probably used -swap command)                
            //  if (player != hero.LastIssuedOrder.player)
            //    player.Heroes.Order(hero, time);                

            player.IncreaseHeroUseCount(hero);

            return true;
        }

        void SpModeFix()
        {
            // make sure both teams are present and have players 
            // that have the "id" gamecache value

            Team t1 = GetTeamByType(TeamType.Sentinel);
            Team t2 = GetTeamByType(TeamType.Scourge);

            if (t1.TeamNo == -1 || t1.Players.Count == 0 || !t1.Players[0].gameCacheValues.ContainsKey("id")) return;
            if (t2.TeamNo == -1 || t2.Players.Count == 0 || !t2.Players[0].gameCacheValues.ContainsKey("id")) return;

            // now collect all "id" values for each player

            Dictionary<int, Player> dcColorPlayer = new Dictionary<int, Player>();

            int colorId;
            foreach (Player p in players)
                if (p.gameCacheValues.TryGetValue("id", out colorId))
                {
                    try
                    {
                        dcColorPlayer.Add(colorId, p);
                    }
                    catch
                    {

                    }
                }
                else
                    if (p.TeamType != TeamType.Observers)
                    return; // "id" not found, then not "-sp" mode maybe

            // ... and recreate teams

            Player player;
            t1.Players.Clear();
            for (int i = 1; i <= 5; i++)
                if (dcColorPlayer.TryGetValue(i, out player))
                {
                    player.Color = (PlayerColor)i;
                    player.SlotNo = (byte)(i - 1);

                    t1.Add(player);
                }

            t2.Players.Clear();
            for (int i = 6; i <= 10; i++)
                if (dcColorPlayer.TryGetValue(i, out player))
                {
                    player.Color = (PlayerColor)(i + 1);
                    player.SlotNo = (byte)(i - 1);

                    t2.Add(player);
                }
        }

        void DetectBansAndPicks()
        {
            System.Collections.Specialized.ListDictionary usedHeroes = new System.Collections.Specialized.ListDictionary();

            // collect used heroes
            foreach (Player player in this.players)
                foreach (KeyValuePair<string, KeyValuePair<Hero, int>> kvp in player.UsedHeroes)
                    if (!usedHeroes.Contains(kvp.Key))
                        usedHeroes.Add(kvp.Key, null);

            // now get team captains
            Player sentinelCaptain = GetPlayerBySlot(0);
            Player scourgeCaptain = GetPlayerBySlot(5);

            if (sentinelCaptain == null || scourgeCaptain == null) return;

            if (sentinelCaptain.Heroes.BuildOrders.Count <= 1 || scourgeCaptain.Heroes.BuildOrders.Count <= 1)
            {
                if (sentinelCaptain.gameCacheValues.ContainsKey("Ban1") == false)
                    return;
            }
            else
            {
                bans.Clear();
                picks.Clear();

                foreach (KeyValuePair<int, Hero> pick in sentinelCaptain.Heroes.BuildOrders)
                    if (usedHeroes.Contains(pick.Value.Name))
                    {
                        // if this hero has no owner OR captain owned more than one instance of this hero, then its a pick                    
                        if (pick.Value.ObjectId == -1 || pick.Value.ReviveTimes.Count > 0)
                            this.picks.Add(new PickInfo(pick.Key, TeamType.Sentinel, pick.Value));
                    }
                    else
                        this.bans.Add(new PickInfo(pick.Key, TeamType.Sentinel, pick.Value));

                foreach (KeyValuePair<int, Hero> pick in scourgeCaptain.Heroes.BuildOrders)
                    if (usedHeroes.Contains(pick.Value.Name))
                    {
                        // if this hero has no owner OR captain owned more than one instance of this hero, then its a pick
                        if (pick.Value.ObjectId == -1 || pick.Value.ReviveTimes.Count > 0)
                            this.picks.Add(new PickInfo(pick.Key, TeamType.Scourge, pick.Value));
                    }
                    else
                        this.bans.Add(new PickInfo(pick.Key, TeamType.Scourge, pick.Value));
            }

            this.bans.Sort((Comparison<PickInfo>)delegate (PickInfo a, PickInfo b)
            {
                return a.Time.CompareTo(b.Time);
            });

            this.picks.Sort((Comparison<PickInfo>)delegate (PickInfo a, PickInfo b)
            {
                return a.Time.CompareTo(b.Time);
            });
        }

        private MemoryStream LoadHeader(Stream stream)
        {
            MemoryStream blocksData = new MemoryStream();
            using (BinaryReader reader = new BinaryReader(stream))
            {
                #region 2.0 [Header]

                #region doc

                //offset | size/type | Description
                //-------+-----------+-----------------------------------------------------------
                //0x0000 | 28 chars  | zero terminated string "Warcraft III recorded game\0x1A\0"
                //0x001c |  1 dword  | fileoffset of first compressed data block (header size)
                //       |           |  0x40 for WarCraft III with patch <= v1.06
                //       |           |  0x44 for WarCraft III patch >= 1.07 and TFT replays
                //0x0020 |  1 dword  | overall size of compressed file
                //0x0024 |  1 dword  | replay header version:
                //       |           |  0x00 for WarCraft III with patch <= 1.06
                //       |           |  0x01 for WarCraft III patch >= 1.07 and TFT replays
                //0x0028 |  1 dword  | overall size of decompressed data (excluding header)
                //0x002c |  1 dword  | number of compressed data blocks in file
                //0x0030 |  n bytes  | SubHeader (see section 2.1 and 2.2)

                #endregion

                ValidateHeaderString(reader.ReadBytes(28));

                int headerSize = reader.ReadInt32();
                //overall size of compressed file
                reader.ReadInt32();
                int versionFlag = reader.ReadInt32();
                //overall size of decompressed data (excluding header)
                reader.ReadInt32();
                int nBlocks = reader.ReadInt32();

                #endregion

                #region SubHeader

                if (versionFlag == 0)
                {
                    throw new W3gParserException("本软件不支持1.06及更低版本录像.");
                }
                else if (versionFlag == 1)
                {
                    #region 2.2 [SubHeader] for header version 1

                    #region doc

                    //offset | size/type | Description
                    //-------+-----------+-----------------------------------------------------------
                    //0x0000 |  1 dword  | version identifier string reading:
                    //       |           |  'WAR3' for WarCraft III Classic
                    //       |           |  'W3XP' for WarCraft III Expansion Set 'The Frozen Throne'
                    //0x0004 |  1 dword  | version number (corresponds to patch 1.xx so far)
                    //0x0008 |  1  word  | build number (see section 2.3)
                    //0x000A |  1  word  | flags
                    //       |           |   0x0000 for single player games
                    //       |           |   0x8000 for multiplayer games (LAN or Battle.net)
                    //0x000C |  1 dword  | replay length in msec
                    //0x0010 |  1 dword  | CRC32 checksum for the header
                    //       |           | (the checksum is calculated for the complete header
                    //       |           |  including this field which is set to zero)

                    #endregion

                    string war3string = DHJassInt.int2id(reader.ReadUInt32()); //ParserUtility.StringFromUInt(reader.ReadUInt32());
                    if (war3string != "W3XP")
                        throw new W3gParserException("本软件只支持冰封王座录像,不支持混乱之治录像.");

                    version = reader.ReadInt32();
                    buildNo = reader.ReadInt16();
                    //flags
                    reader.ReadInt16();
                    //length = reader.ReadInt32();
                    reader.ReadInt32(); length = -1;// will be calculated manually
                    //CRC32 checksum for the header
                    reader.ReadInt32();

                    #endregion
                }

                #endregion

                reader.BaseStream.Seek(headerSize, SeekOrigin.Begin);
                for (int i = 0; i < nBlocks; i++)
                {
                    #region [Data block header]

                    #region doc

                    //offset | size/type | Description
                    //-------+-----------+-----------------------------------------------------------
                    //0x0000 |  1  word  | size n of compressed data block (excluding header)
                    //0x0002 |  1  word  | size of decompressed data block (currently 8k)
                    //0x0004 |  1 dword  | unknown (probably checksum)
                    //0x0008 |  n bytes  | compressed data (decompress using zlib)

                    #endregion

                    ushort compressedSize = reader.ReadUInt16();
                    ushort decompressedSize = reader.ReadUInt16();

                    //unknown (probably checksum)
                    reader.ReadInt32();

                    byte[] decompressed = new byte[decompressedSize];
                    byte[] compressed = reader.ReadBytes(compressedSize);

                    using (InflaterInputStream zipStream = new InflaterInputStream(new MemoryStream(compressed)))
                    {
                        zipStream.Read(decompressed, 0, decompressedSize);
                    }
                    blocksData.Write(decompressed, 0, decompressedSize);

                    #endregion
                }
            }
            blocksData.Seek(0, SeekOrigin.Begin);

            //Stream s= File.Create("replayDecoded.dta");
            //s.Write(blocksData.GetBuffer(), 0, blocksData.GetBuffer().Length);
            //s.Close();
            return blocksData;
        }

        private void LoadPlayers(BinaryReader reader)
        {
            #region 4.0 [Decompressed data]

            #region doc

            // # |   Size   | Name
            //---+----------+--------------------------
            // 1 |   4 byte | Unknown (0x00000110 - another record id?)
            // 2 | variable | PlayerRecord (see 4.1)
            // 3 | variable | GameName (null terminated string) (see 4.2)
            // 4 |   1 byte | Nullbyte
            // 5 | variable | Encoded String (null terminated) (see 4.3)
            //   |          |  - GameSettings (see 4.4)
            //   |          |  - Map&CreatorName (see 4.5)
            // 6 |   4 byte | PlayerCount (see 4.6)
            // 7 |   4 byte | GameType (see 4.7)
            // 8 |   4 byte | LanguageID (see 4.8)
            // 9 | variable | PlayerList (see 4.9)
            //10 | variable | GameStartRecord (see 4.11)

            #endregion
            //Unknown
            reader.ReadInt32();

            host = new Player(this.mapCache);
            host.Load(reader);
            players.Add(host);
            type = host.GameType;

            name = ParserUtility.ReadString(reader);
            //nullbyte
            reader.ReadByte();

            byte[] decoded = ParserUtility.GetDecodedBytes(reader);

            settings = new GameSettings(decoded);
            hostName = ParserUtility.GetString(decoded, 13);
            map = new MapInfo(ParserUtility.GetUInt(decoded, 9), hostName);
            hostName = ParserUtility.GetString(decoded, 13 + hostName.Length + 1);
            #endregion

            //playerCount, gameType, langId            
            reader.ReadBytes(12);

            #region Player List
            while (reader.PeekChar() == 0x16)
            {
                #region doc
                //offset | size/type | Description
                //-------+-----------+-----------------------------------------------------------
                //0x0000 | 4/11 byte | PlayerRecord (see 4.1)
                //0x000? |    4 byte | unknown
                //       |           |  (always 0x00000000 for patch version >= 1.07
                //       |           |   always 0x00000001 for patch version <= 1.06)
                #endregion

                Player p = new Player(this.mapCache);
                p.Load(reader);
                players.Add(p);
                reader.ReadBytes(4);

                if (p.Name == hostName)
                    host = p;
            }
            #endregion

            #region 4.10 [GameStartRecord]
            #region doc
            //offset | size/type | Description
            //-------+-----------+-----------------------------------------------------------
            //0x0000 |  1 byte   | RecordID - always 0x19
            //0x0001 |  1 word   | number of data bytes following
            //0x0003 |  1 byte   | nr of SlotRecords following (== nr of slots on startscreen)
            //0x0004 |  n bytes  | nr * SlotRecord (see 4.11)
            //   n+4 |  1 dword  | RandomSeed (see 4.12)
            //   n+8 |  1 byte   | SelectMode
            //       |           |   0x00 - team & race selectable (for standard custom games)
            //       |           |   0x01 - team not selectable
            //       |           |          (map setting: fixed alliances in WorldEditor)
            //       |           |   0x03 - team & race not selectable
            //       |           |          (map setting: fixed player properties in WorldEditor)
            //       |           |   0x04 - race fixed to random
            //       |           |          (extended map options: random races selected)
            //       |           |   0xcc - Automated Match Making (ladder)
            //   n+9 |  1 byte   | StartSpotCount (nr. of start positions in map)
            #endregion

            //RecordId, number of data bytes following
            reader.ReadBytes(3);
            byte nSlots = reader.ReadByte();
            for (byte i = 0; i < nSlots; i++)
            {
                #region 4.11 [SlotRecord]

                #region doc
                //offset | size/type | Description
                //-------+-----------+-----------------------------------------------------------
                //0x0000 |  1 byte   | player id (0x00 for computer players)
                //0x0001 |  1 byte   | map download percent: 0x64 in custom, 0xff in ladder
                //0x0002 |  1 byte   | slotstatus:
                //       |           |   0x00 empty slot
                //       |           |   0x01 closed slot
                //       |           |   0x02 used slot
                //0x0003 |  1 byte   | computer player flag:
                //       |           |   0x00 for human player
                //       |           |   0x01 for computer player
                //0x0004 |  1 byte   | team number:0 - 11
                //       |           | (team 12 == observer or referee)
                //0x0005 |  1 byte   | color (0-11):
                //       |           |   value+1 matches player colors in world editor:
                //       |           |   (red, blue, cyan, purple, yellow, orange, green,
                //       |           |    pink, gray, light blue, dark green, brown)
                //       |           |   color 12 == observer or referee
                //0x0006 |  1 byte   | player race flags (as selected on map screen):
                //       |           |   0x01=human
                //       |           |   0x02=orc
                //       |           |   0x04=nightelf
                //       |           |   0x08=undead
                //       |           |   0x20=random
                //       |           |   0x40=race selectable/fixed (see notes below)
                //0x0007 |  1 byte   | computer AI strength: (only present in v1.03 or higher)
                //       |           |   0x00 for easy
                //       |           |   0x01 for normal
                //       |           |   0x02 for insane
                //       |           | for non-AI players this seems to be always 0x01
                //0x0008 |  1 byte   | player handicap in percent (as displayed on startscreen)
                //       |           | valid values: 0x32, 0x3C, 0x46, 0x50, 0x5A, 0x64
                //       |           | (field only present in v1.07 or higher)
                #endregion

                #region
                byte playerId = reader.ReadByte();
                reader.ReadByte();
                byte slotStatus = reader.ReadByte();
                byte computerFlag = reader.ReadByte();
                byte teamNo = reader.ReadByte();
                PlayerColor color = (PlayerColor)reader.ReadByte();
                Race race = (Race)reader.ReadByte();
                if ((uint)race > 0x40)
                    race -= 0x40;
                AIStrength strength = (AIStrength)reader.ReadByte();
                int handicap = reader.ReadByte();
                #endregion

                #region
                if (slotStatus == 0x02)
                {
                    Player player = GetPlayerById(playerId);
                    if (playerId == 0 && /*player == null &&*/ computerFlag == 0x01)
                    {
                        player = new Player(this.mapCache);
                        player.Race = race;
                        player.Id = i; playerId = i; // DANAT experimental
                        if (strength == AIStrength.Easy)
                            player.Name = "Computer(Easy)";
                        else if (strength == AIStrength.Normal)
                            player.Name = "Computer(Normal)";
                        else
                            player.Name = "Computer(Insane)";
                        players.Add(player);
                    }
                    else if (player == null)
                    {
                        continue;
                    }
                    player.SlotNo = i;
                    player.Color = color;
                    player.Handicap = handicap;
                    player.Id = playerId;
                    player.IsComputer = computerFlag == 0x01;
                    player.IsObserver = teamNo == 12;
                    player.Race = race;
                    player.TeamNo = teamNo; //+ 1;
                }
                #endregion
                #endregion
            }


            if (buildNo == 0 && nSlots == 0)
            {
                #region 6.1 Notes on official Blizzard Replays
                #region doc
                //o Since the lack of all slot records, one has to generate these data:
                //iterate slotNumber from 1 to number of PlayerRecords (see 4.1)
                //  player id    = slotNumber
                //  slotstatus   = 0x02                   (used)
                //  computerflag = 0x00                   (human player)
                //  team number  = (slotNumber -1) mod 2  (team membership alternates in
                //                                                          PlayerRecord)
                //  color        = unknown                (player gets random colors)
                //  race         = as in PlayerRecord
                //  computerAI   = 0x01                   (non computer player)
                //  handicap     = 0x64                   (100%)
                #endregion
                foreach (Player player in players)
                {
                    player.SlotNo = player.Id;
                    player.TeamNo = (player.SlotNo - 1) % 2;
                    player.Handicap = 100;
                }
                #endregion
            }

            // sort players by their teams
            foreach (Player player in players)
            {
                // mark uninitialized players as Observers to avoid errors
                if (player.Race == 0 && player.TeamNo == 0)
                {
                    Console.WriteLine("Warning: Player '" + player.Name + "' was not initialized and thus marked as Observer");

                    player.TeamNo = 12;
                    player.IsObserver = true;
                }

                Team team = GetTeamByNo(player.TeamNo);
                team.AddSortedBySlot(player);
            }
            //random seed, select mode, startspotcount
            reader.ReadBytes(6);
            #endregion
        }

        void DetectGameMode(string chatMessage, int currentTime)
        {
            if (this.gameMode != string.Empty) return;

            if (currentTime >= 16000)
            {
                this.gameMode = "Normal mode";
                return;
            }

            if (!chatMessage.Contains("-")) return;


            // read only letters
            string modeString = "";
            foreach (char c in chatMessage)
                if (char.IsLetter(c))
                    modeString += char.ToLower(c);

            // skip this chat string if it's less than 2 characters
            if (modeString.Length < 2) return;

            // scan modeString for modes.
            // if a mode is found its letters are removed from the modeString
            int modeTypes = 0;
            int rejectedModes = 0;
            int acceptedModes = 0;
            for (int i = 0; i < modeString.Length / 2; i++)
            {
                int modeType = GetModeType(modeString.Substring(i * 2, 2));

                if (modeType == -1) // if this mode string was rejected						
                    rejectedModes++; // increase rejected modes counter
                else
                    if ((modeType & modeTypes) != 0) // if this mode type conflicts with previously found mode types
                    return; // then this chat string is not a mode						
                else
                {
                    modeTypes |= modeType; // otherwise add this mode type flags to the modeTypes
                    acceptedModes++; // increase accepted modes counter
                }
            }

            // in case some new unknown modes were added to the map, 
            // it is okay to have some of them rejected, but not unless no valid modes are found in the string
            if (rejectedModes > 0 && acceptedModes == 0)
                return;

            this.gameMode = modeString;
        }

        private int GetModeType(string mode)
        {
            switch (mode)
            {
                case "ap":
                    return 1; //00000000 00000000 00000001

                //  if VR and (... AR)
                case "ar": //ar
                    return 512 | 1;
                case "lm": //lm
                    return 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 1024 | 2048 | 4096 | 8192 | 32768;
                case "mm": //mm
                    return 2; //00000000 00000000 00000010							
                case "tr": //tr
                    return 4; //00000000 00000000 00000100

                //  if (DM and (CM or TR or LM or MR or SH or RV or RD or CD or SD)) or (MM and DM) or (SH and DM) then
                case "dm": //dm
                    return 32768 | 8 | 4 | 2;

                // if MR and (CM or AP or LM or AR or SD  or MM or TR or DM or AA or AI or AS or SH or RV or RO or MO) then
                case "mr": //mr
                    return 1 | 2 | 4;

                case "sp": //sp
                    return 16384; //00000000 01000000 00000000

                // if (AA and (AI or AS or DM)) or (AS and (AI or DM)) or (AI and DM) then
                case "aa": //aa						
                case "ai": //ai					
                case "as": //as
                    return 8; //00000000 00000000 00001000

                case "id": //id
                    return 16; //00000000 00000000 00010000						
                case "np": //np
                    return 32; //00000000 00000000 00100000
                case "sc": //sc
                    return 64; //00000000 00000000 01000000							
                case "em": //em
                    return 128;//00000000 00000000 10000000
                case "du": //du
                    return 256;//00000000 00000001 00000000

                // if ... (MM and SH)then
                case "sh": //sh
                    return 2;

                //  if VR and (...MM)
                case "vr": //vr
                    return 512 | 2;
                case "rv": //rv
                    return 512; //00000000 00000010 00000000
                case "rd": //rd
                    return 512 | 1 | 2 | 4 | 8;
                case "om": //om
                    return 1024; //00000000 00000100 00000000
                case "xl": //xl
                    return 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 1024 | 2048 | 4096 | 8192 | 32768;
                case "nm": //nm
                    return 2048; //00000000 00001000 00000000
                case "nt": //nt
                    return 4096; //00000000 00010000 00000000
                case "nb": //nb
                    return 8192; //00000000 00100000 00000000
                case "ns": //ns
                    return 0;
                case "nr": //nr
                    return 0;
                case "ts": //ts
                    return 0;
                case "cd": //cd (captains draft)
                    return 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 32768;
                case "sd": //sd
                    return 1 | 4 | 8 | 512;
                case "pm": //pm
                    return 0;
                case "oi": //oi
                    return 0;
                case "mi": //mi
                    return 0;
                case "cm": //cm (captains mode)
                    return 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 1024 | 2048 | 4096 | 8192 | 16384 | 32768;
                case "fr": //fr
                    return 0;

                //  if (RO and (MO or DM)) or (MO and DM) then
                case "mo": //mo
                case "ro": //ro
                    return 32768; //00000000 10000000 00000000

                case "er": // er (experimental runes)
                    return 0;
                case "rs": // rs (random side)
                    return 0;
                case "so": // so (switch on)
                    return 0;

                default:
                    Console.WriteLine("Rejected mode string: " + mode);
                    return -1; // invalid mode
            }
        }

        private Team GetTeamByNo(int teamNo)
        {
            foreach (Team team in teams)
                if (team.TeamNo == teamNo)
                    return team;
            Team t = new Team(teamNo);
            teams.Add(t);
            return t;
        }

        public Team GetTeamByType(TeamType teamType)
        {
            foreach (Team team in teams)
                if (team.Type == teamType)
                    return team;
            return new Team(-1);
        }
        public TeamType GetTeamTypeBySpecialSlot(int slot)
        {
            if (slot > 0 && slot < 6)
                return TeamType.Sentinel;

            if (slot > 6 && slot < 12)
                return TeamType.Scourge;

            return TeamType.Observers;
        }

        public Player GetPlayerById(byte playerId)
        {
            foreach (Player player in players)
            {
                if (player.Id == playerId)
                    return player;
            }
            return null;
        }
        public Player GetPlayerBySpecialSlot(int slot)
        {
            if (slot > 0 && slot < 6)
                return GetPlayerBySlot(slot - 1);

            if (slot > 6 && slot < 12)
                return GetPlayerBySlot(slot - 2);

            return GetPlayerBySlot(slot);
        }
        public Player GetPlayerBySlot(int slot)
        {
            foreach (Player player in players)
            {
                if (player.SlotNo == slot)
                    return player;
            }
            return null;
        }

        private void ValidateHeaderString(byte[] header)
        {
            for (int i = 0; i < 28; i++)
            {
                if (HeaderString[i] != (char)header[i])
                    throw new W3gParserException("指定的文件不是合法的Warcraft III Replay文件。");
            }
        }
    }

    public delegate void MapRequiredEventHandler(object sender, EventArgs e);

    public class ReplayPreviewEventArgs : EventArgs
    {
        bool cancelParsing;

        public bool CancelParsing
        {
            get { return cancelParsing; }
            set { cancelParsing = value; }
        }
    }
    public delegate void ReplayPreviewEventHandler(Replay replay, ReplayPreviewEventArgs e);
}