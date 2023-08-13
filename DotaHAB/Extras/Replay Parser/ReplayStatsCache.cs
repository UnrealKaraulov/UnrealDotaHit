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
    public class ReplayStatsCache
    {
        static int version = 2;
        static string filename = "ReplayStats.dha";
        static GZipArchive statsArchive = null;
        static Dictionary<string, ReplayStats> dcReplayStats = null;

        public static bool TryAddReplayStats(Replay replay)
        {
            if (dcReplayStats == null)
                Load();

            if (dcReplayStats.ContainsKey(replay.FileName))
                return false;

            ReplayStats replayStats = ReplayStats.FromReplay(replay);            

            string replayPath;
            byte[] bytes;
            replayStats.ToData(out replayPath, out bytes);

            dcReplayStats.Add(replayPath, replayStats);
            statsArchive.AddFile(replayPath, bytes);

            return true;
        }

        public static bool TryGetReplayStats(string filename, out ReplayStats replayStats)
        {
            if (dcReplayStats == null)
                Load();

            if (dcReplayStats.TryGetValue(filename, out replayStats))
                return true;

            byte[] bytes = statsArchive[filename];
            if (bytes != null)
            {
                try
                {
                    replayStats = ReplayStats.FromData(filename, bytes);
                    return true;
                }
                catch
                {                    
                }
            }            

            replayStats = null;
            return false;
        }

        public static void Load()
        {
            dcReplayStats = new Dictionary<string,ReplayStats>();

            string path = ReplayParserCore.CachePath + Path.DirectorySeparatorChar + filename;

            if (!File.Exists(path))
            {
                statsArchive = new GZipArchive(); 
                return;
            }

            statsArchive = GZipArchive.FromFile(path);

            if (statsArchive.Version != version)
            {
                statsArchive = new GZipArchive();
                return;
            }
        }

        public static void Save()
        {
            if (statsArchive == null)
                return;

            string path = ReplayParserCore.CachePath + Path.DirectorySeparatorChar + filename;

            statsArchive.SaveToFile(path, version);
        }
    }    
}
