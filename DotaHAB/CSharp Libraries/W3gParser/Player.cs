using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Extras;
using System.Diagnostics;

namespace Deerchao.War3Share.W3gParser
{        
    public class Player : IPlayer
    {
        byte playerId;
        string name;
        GameType gameType;
        private LineUp lineUp;
        private location lineUpLocation;
        private int time;
        private int postPickTime = 0;
        private Race race;
        private byte slotNo;
        private PlayerColor color;
        private int handicap;
        private bool isComputer;
        private bool isObserver;
        private int teamNo;        
        private int actionsCount;
        private bool disconnected;        
        private readonly ReplayMapCache mapCache;
        private readonly Heroes heroes = null;
        private readonly Items buildings = new Items();
        private readonly Items items = new Items();
        private readonly Items abilities = new Items();
        private readonly Units units = new Units();
        private readonly Units upgrades = new Units();
        private readonly Units researches = new Units();
        private readonly Groups groups = new Groups();
        private Inventory orphanInventory = null;
        private readonly List<PlayerAction> actions = new List<PlayerAction>();
        private readonly List<Player> playersWithSharedControl = new List<Player>();
        private readonly ListDictionary possiblePlayersForSwap = new ListDictionary();
        private readonly Dictionary<string, KeyValuePair<Hero, int>> usedHeroes = new Dictionary<string, KeyValuePair<Hero, int>>();
        public Dictionary<string, int> gameCacheValues = new Dictionary<string, int>();

        public Player(ReplayMapCache cache)
        {
            this.mapCache = cache;
            heroes = new Heroes(cache);
        }

        public byte Id
        {
            get { return playerId; }
            set { playerId = value; }
        }

        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }

        public byte SlotNo
        {
            get { return slotNo; }
            internal set { slotNo = value; }
        }

        public Race Race
        {
            get { return race; }
            internal set { race = value; }
        }

        public PlayerColor Color
        {
            get { return color; }
            internal set { color = value; }
        }

        public int Handicap
        {
            get { return handicap; }
            internal set { handicap = value; }
        }

        public bool IsComputer
        {
            get { return isComputer; }
            internal set { isComputer = value; }
        }

        public bool IsObserver
        {
            get { return isObserver; }
            internal set { isObserver = value; }
        }

        public int TeamNo
        {
            get { return teamNo; }
            internal set { teamNo = value; }
        }

        public PlayerState State = new PlayerState();       

        public GameType GameType
        {
            get { return gameType; }
        }

        public TeamType TeamType
        {
            get
            {
                if (isObserver) return TeamType.Observers;
                if (slotNo < 5) return TeamType.Sentinel;
                else return TeamType.Scourge;
            }
        }

        public LineUp LineUp
        {
            get { return lineUp; }
            set { lineUp = value; }
        }

        public location LineUpLocation
        {
            get { return lineUpLocation; }
            set { lineUpLocation = value; }
        }

        public int ActionsCount
        {
            get { return actionsCount; }
            internal set { actionsCount = value; }
        }

        public List<PlayerAction> Actions
        {
            get { return actions; }
        }

        public List<Player> PlayersWithSharedControl
        {
            get { return playersWithSharedControl; }
        }       

        public bool Disconnected
        {
            get { return disconnected; }
            set { disconnected = value; }
        }       

        public Dictionary<string, KeyValuePair<Hero, int>> UsedHeroes
        {
            get { return usedHeroes; }
        }

        public int Time
        {
            get { return time; }
            internal set { time = value; }
        }

        public int PostPickTime
        {
            get { return postPickTime; }
            internal set { postPickTime = value; }
        }

        public TimeSpan GetTime()
        {
            return new TimeSpan(0, 0, 0, 0, time);
        }

        public float Apm
        {
            get
            {
                if (time == 0)
                    return 0;
                return actionsCount * 1000 * 60 / (time - (postPickTime == time ? 0 : postPickTime));
            }
        }

        public ReplayMapCache MapCache
        {
            get { return mapCache; }
        }

        public Heroes Heroes
        {
            get { return heroes; }
        }

        public Items Buildings
        {
            get
            {
                return buildings;
            }
        }

        public Items Items
        {
            get
            {
                return items;
            }
        }
        
        public Inventory Inventory
        {
            get
            {
                Hero hero = GetMostUsedHero();                
                return hero != null ? hero.Inventory : null;
            }
        }

        public Items Abilities
        {
            get
            {
                return abilities;
            }
        }

        public Units Units
        {
            get { return units; }
        }

        public Units Upgrades
        {
            get
            {
                return upgrades;
            }
        }

        public Units Researches
        {
            get
            {
                return researches;
            }
        }

        internal Groups Groups
        {
            get { return groups; }
        }

        public Inventory OrphanInventory
        {
            get
            {
                if (orphanInventory == null)                
                    orphanInventory = new Inventory(this.mapCache);                

                return orphanInventory;
            }
        }

        string IPlayer.HeroID
        {
            get
            {
                Hero hero = this.GetMostUsedHero();
                return (hero != null) ? hero.Name : null;
            }
        }

        ushort IPlayer.Apm
        {
            get
            {
                return (ushort)this.Apm;
            }
        }

        short IPlayer.Kills
        {
            get
            {
                return (short)this.getGCValue("kills", this.getGCValue("1", -1));
            }
        }

        short IPlayer.Deaths
        {
            get
            {
                return (short)this.getGCValue("deaths", this.getGCValue("2", -1));
            }
        }

        short IPlayer.CreepKills
        {
            get
            {
                return (short)this.getGCValue("creeps", this.getGCValue("3", -1));
            }
        }

        short IPlayer.CreepDenies
        {
            get
            {
                return (short)this.getGCValue("denies", this.getGCValue("4", -1));
            }
        }

        short IPlayer.Assists
        {
            get
            {
                return (short)this.getGCValue("5", -1);
            }
        }

        short IPlayer.NeutralKills
        {
            get
            {
                return (short)this.getGCValue("7", -1);
            }
        }

        // 4.1 [PlayerRecord]
        internal void Load(BinaryReader reader)
        {
            #region doc
            //offset | size/type | Description
            //-------+-----------+-----------------------------------------------------------
            //0x0000 |  1 byte   | RecordID:
            //       |           |  0x00 for game host
            //       |           |  0x16 for additional players (see 4.9)
            //0x0001 |  1 byte   | PlayerID
            //0x0002 |  n bytes  | PlayerName (null terminated string)
            //   n+2 |  1 byte   | size of additional data:
            //       |           |  0x01 = custom
            //       |           |  0x08 = ladder
            #endregion
            //RecordId
            reader.ReadByte();
            playerId = reader.ReadByte();
            name = ParserUtility.ReadString(reader);
            gameType = (GameType)reader.ReadByte();

            if (gameType == GameType.Custom)
                reader.ReadByte();
            else
            {
                //time
                reader.ReadInt32();
                Race = (Race)reader.ReadUInt32();
            }
        }

        public int getGCValue(string key)
        {
            int value;
            gameCacheValues.TryGetValue(key, out value);
            return value;
        }
        public int getGCValue(string key, int retOnFail)
        {
            int value;
            if (gameCacheValues.TryGetValue(key, out value))
                return value;
            else
                return retOnFail;
        }
        public string getGCVStringValue(string key, string retOnFail)
        {
            int value;
            if (gameCacheValues.TryGetValue(key, out value))
                return value.ToString();
            else
                return retOnFail;            
        }

        public void IncreaseHeroUseCount(Hero hero)
        {
            KeyValuePair<Hero, int> kvp;
            if (usedHeroes.TryGetValue(hero.Name, out kvp))
                usedHeroes[hero.Name] = new KeyValuePair<Hero, int>(kvp.Key, kvp.Value + 1);
            else
            {
                // if this is first controlled hero and orphanInventory is not null
                if (usedHeroes.Count == 0 && orphanInventory != null && hero.Inventory.Slots.Count == 0)
                {
                    // try to move items from orphanInventory to this hero inventory
                    hero.Inventory.Slots.AddRange(orphanInventory.Slots);

                    // dispose orphanInventory
                    orphanInventory = null;

                    Console.WriteLine(this.name + "(" + hero.GetClass() + ") has obtained orphanInventory's items");
                }

                usedHeroes.Add(hero.Name, new KeyValuePair<Hero, int>(hero, 1));
            }
        }

        public Hero GetMostRecentlyUsedOwnHero()
        {
            for (int i = heroes.BuildOrders.Count - 1; i >= 0; i--)
                if (usedHeroes.ContainsKey(heroes.BuildOrders[i].Value.Name))
                    return heroes.BuildOrders[i].Value;

            return null;            
        }

        public Hero GetMostRecentlyUsedHero()
        {
            int time = -1;
            KeyValuePair<Hero, int> kvpHero = new KeyValuePair<Hero, int>(null, -1);
            foreach (KeyValuePair<Hero, int> kvp in usedHeroes.Values)
                if (kvp.Key.Inventory.playerId == this.playerId && kvp.Key.LastIssuedOrder.time > time)
                {
                    if (kvp.Key.Level == 0 && kvpHero.Key != null && kvpHero.Key.Level > 1)
                        continue;

                    kvpHero = kvp;
                    time = kvp.Key.LastIssuedOrder.time;
                }

            return kvpHero.Key != null ? kvpHero.Key : null;//(heroes.Count > 0 ? heroes[0] : null);
        }
        public Hero GetMostUsedHero()
        {            
            KeyValuePair<Hero,int> kvpMax = new KeyValuePair<Hero,int>(null, -1);

            foreach (KeyValuePair<Hero, int> kvp in usedHeroes.Values)
                if (kvp.Value > kvpMax.Value)
                    kvpMax = kvp;

            return kvpMax.Key != null ? kvpMax.Key : (heroes.Count > 0 ? heroes[0] : null);
        }

        public string GetMostUsedHeroClass()
        {
            Hero hero = this.GetMostUsedHero();
            if (hero != null)
                return hero.GetClass();
            else
                return "";
        }       

        /// <summary>
        /// try to swap heroes with this player. can be done only if both players are trying to swap with eachother
        /// </summary>
        /// <param name="player"></param>
        public void TrySwap(Player player, Hero desiredHero)
        {
            // if player already tried to swap with this player           
            if (player.possiblePlayersForSwap.Contains(this.playerId))
            {
                // then the swap initiation is successful. swap heroes.                

                // get the hero that the player wants to control
                Hero heroToBeTaken = player.possiblePlayersForSwap[this.playerId] as Hero;
                
                // exchange inventory items between heroes
                Inventory temp = new Inventory(null);
                temp.Slots.AddRange(heroToBeTaken.Inventory.Slots);
                heroToBeTaken.Inventory.Slots.Clear(); heroToBeTaken.Inventory.Slots.AddRange(desiredHero.Inventory.Slots);
                desiredHero.Inventory.Slots.Clear(); desiredHero.Inventory.Slots.AddRange(temp.Slots);                

                // update item ownership
                desiredHero.Inventory.playerId = this.Id;
                heroToBeTaken.Inventory.playerId = player.Id;                

                // remove swap attempt record
                player.possiblePlayersForSwap.Remove(this.playerId);

                Console.WriteLine(this.name + "(" + heroToBeTaken.GetClass() + ") swapped with " + player.name + "(" + desiredHero.GetClass() + ")");
            }
            else
                // or this player doesnt control any hero other than 'desiredHero'
                if (this.usedHeroes.Count == 1 && player.usedHeroes.Count > 1)
                {
                    // then the swap initiation is successful. swap heroes. 

                    // update item ownership
                    desiredHero.Inventory.playerId = this.Id;

                    Console.WriteLine(this.name + "(???) swapped with " + player.name + "(" + desiredHero.GetClass() + ")");
                }
                else
                {
                    // check if other players attempted to swap with this player
                    foreach(Player p in mapCache.Replay.Players)
                        if (p.possiblePlayersForSwap.Contains(this.playerId))
                        {
                            // if someone else tried to swap with this player (controlled his hero),
                            // then assume that swap was successful and swap heroes.
                            this.TrySwap(p, p.GetMostRecentlyUsedOwnHero());
                            break;
                        }

                    // check if other players attempted to swap with 'player'
                    foreach (Player p in mapCache.Replay.Players)
                        if (player.possiblePlayersForSwap.Contains(p.playerId))
                        {
                            // if someone else tried to swap with 'player' (controlled his hero),
                            // then assume that swap was successful and swap heroes.
                            p.TrySwap(player, desiredHero);

                            // desiredHero owner has changed
                            player = p;
                            break;
                        }

                    // record this swap attempt and wait until that player will try to swap
                    this.possiblePlayersForSwap[player.Id] = desiredHero;
                }
        }

        public override string ToString()
        {
            return "["+this.Id+"]" + this.Name;
        }        
    }
}