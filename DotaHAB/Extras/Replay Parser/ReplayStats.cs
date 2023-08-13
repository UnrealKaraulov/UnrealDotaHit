using System;
using System.Collections.Generic;
using System.Text;
using DotaHIT.Core;
using DotaHIT.Core.Compression;
using DotaHIT.Core.Resources;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.Drawing;
using Deerchao.War3Share.W3gParser;

namespace DotaHIT.Extras
{
    public class ReplayStats : IReplay
    {
        public class PlayerStats : IPlayer
        {
            public string Name { get; set; }
            public bool IsComputer { get; set; }
            public bool IsObserver { get; set; }
            public string HeroID { get; set; }
            public TeamType TeamType { get; set; }
            public LineUp LineUp { get; set; }
            public ushort Apm { get; set; }
            public short Kills { get; set; }
            public short Deaths { get; set; }
            public short Assists { get; set; }
            public short CreepKills { get; set; }
            public short CreepDenies { get; set; }
            public short NeutralKills { get; set; }
        }
        
        public string ReplayPath { get; set; }
        public string MapPath { get; set; }
        public bool IsDota { get; set; }

        public string GameMode { get; set; }
        public TimeSpan GameLength { get; set; }
        public List<IPlayer> PlayersStats { get; set; }
        public TeamType Winner { get; set; }

        IList<IPlayer> IReplay.Players
        {
            get
            {
                return this.PlayersStats;
            }            
        }
        int IReplay.PlayerCount
        {
            get
            {
                return this.PlayersStats.Count;
            }
        }

        public void ToData(out string replayPath, out byte[] bytes)
        {
            replayPath = this.ReplayPath;

            MemoryStream ms = new MemoryStream(400);
            using (BinaryWriter bw = new BinaryWriter(ms, UTF8Encoding.UTF8))
            {
                bw.Write(this.MapPath);
                bw.Write(this.IsDota);
                bw.Write(this.GameMode);

                bw.Write((byte)this.GameLength.Hours);
                bw.Write((byte)this.GameLength.Minutes);
                bw.Write((byte)this.GameLength.Seconds);

                bw.Write((byte)this.Winner);

                bw.Write((byte)this.PlayersStats.Count);

                foreach (PlayerStats ps in this.PlayersStats)
                {
                    bw.Write(ps.Name);
                    bw.Write(ps.IsComputer);
                    bw.Write(ps.IsObserver);
                    bw.Write(ps.HeroID ?? "");
                    bw.Write((byte)ps.TeamType);
                    bw.Write((byte)ps.LineUp);
                    bw.Write(ps.Apm);
                    bw.Write(ps.Kills);
                    bw.Write(ps.Deaths);
                    bw.Write(ps.Assists);
                    bw.Write(ps.CreepKills);
                    bw.Write(ps.CreepDenies);
                    bw.Write(ps.NeutralKills);
                }
            }

            bytes = ms.ToArray();
        }

        public static ReplayStats FromReplay(IReplay replay)
        {
            ReplayStats replayStats = new ReplayStats
            {                
                ReplayPath = replay.ReplayPath,
                MapPath = replay.MapPath,
                IsDota = replay.IsDota,
                GameMode = replay.GameMode,
                GameLength = replay.GameLength,  
                Winner = replay.Winner,
            };

            List<IPlayer> PlayersStats = new List<IPlayer>(replay.Players.Count);
            foreach (IPlayer p in replay.Players)
            {
                PlayerStats pStats = new PlayerStats
                {
                    Name = p.Name,
                    IsComputer = p.IsComputer,
                    IsObserver = p.IsObserver,
                    HeroID = p.HeroID,
                    TeamType = p.TeamType,
                    LineUp = p.LineUp,
                    Apm = p.Apm,
                    Kills = p.Kills,
                    Deaths = p.Deaths,
                    Assists = p.Assists,
                    CreepKills = p.CreepKills,
                    CreepDenies = p.CreepDenies,
                    NeutralKills = p.NeutralKills,
                };                

                PlayersStats.Add(pStats);
            }

            replayStats.PlayersStats = PlayersStats;

            return replayStats;
        }
        public static ReplayStats FromData(string replayPath, byte[] bytes)
        {
            ReplayStats replayStats = new ReplayStats { ReplayPath = replayPath };

            using (BinaryReader br = new BinaryReader(new MemoryStream(bytes), UTF8Encoding.UTF8))
            {
                replayStats.MapPath = br.ReadString();
                replayStats.IsDota = br.ReadBoolean();
                replayStats.GameMode = br.ReadString();
                replayStats.GameLength = new TimeSpan(br.ReadByte(), br.ReadByte(), br.ReadByte());
                replayStats.Winner = (TeamType)br.ReadByte();

                int playerCount = br.ReadByte();
                List<IPlayer> playersStats = new List<IPlayer>(playerCount);
                for (int i = 0; i < playerCount; i++)                
                    playersStats.Add(new PlayerStats
                    {
                        Name = br.ReadString(),
                        IsComputer = br.ReadBoolean(),
                        IsObserver = br.ReadBoolean(),
                        HeroID = br.ReadString(),
                        TeamType = (TeamType)br.ReadByte(),
                        LineUp = (LineUp)br.ReadByte(),
                        Apm = br.ReadUInt16(),
                        Kills = br.ReadInt16(),
                        Deaths = br.ReadInt16(),
                        Assists = br.ReadInt16(),
                        CreepKills = br.ReadInt16(),
                        CreepDenies = br.ReadInt16(),
                        NeutralKills = br.ReadInt16(),
                    });

                replayStats.PlayersStats = playersStats;
            }

            return replayStats;
        }        
    }    
}
