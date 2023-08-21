using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.IO;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using ExpTreeLib;
using System.Collections.Specialized;
using System.Diagnostics;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Format;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Jass.Native.Types;
using BitmapUtils;
using DotaHIT.Extras.Replay_Parser;
using DotaHIT.Extras;

namespace DotaHIT
{
    public static class Program
    {
        public static bool SKIP_HEADER_CHECK = false;
        static void MapRequired(object sender, EventArgs e)
        {
            Replay replay = sender as Replay;
            string mapPath = ReplayParserCore.GetProperMapPath(replay.Map.Path);
            string mapFilename = Path.GetFileNameWithoutExtension(mapPath);

            Stopwatch sw = new Stopwatch(); sw.Start();


            bool cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha", mapPath);

            if (cacheOk)
            {
                sw.Stop(); Console.WriteLine("MapCacheLoad: " + ((float)sw.ElapsedMilliseconds / (float)1000.0));

                return;
            }
            else
            {
                if (mapFilename.ToLower().Contains("v6.83s") || mapFilename.ToLower().Contains("iccup"))
                {
                    cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\dota.dha", ReplayParserCore.CachePath + "\\dota.w3x");
                    if (cacheOk)
                    {
                        sw.Stop(); Console.WriteLine("MapCacheLoad: " + ((float)sw.ElapsedMilliseconds / (float)1000.0));

                        replay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha");
                        return;
                    }
                }
            }

            sw.Stop(); Console.WriteLine("MapCacheBadLoad: " + ((float)sw.ElapsedMilliseconds / (float)1000.0));



            // return if this map is already opened
            if (Current.map != null && string.Compare(Path.GetFileName(Current.map.Name), Path.GetFileName(mapPath), true) == 0)
            {
                if (replay.ParseSettings.EmulateInventory)
                {
                    DHLOOKUP.CollectItemCombiningData();
                    DHLOOKUP.DetectStackableItems();
                }
                return;
            }

            // turn off all features that increase map loading time
            Current.mainForm.MinimizeMapLoadTime();

            MapLoadSettings mlSettings = new MapLoadSettings(mapPath, false, new SplashScreen());
            mlSettings.RefreshItemCombiningDataLookups = replay.ParseSettings.EmulateInventory;

            if (!File.Exists(mapPath))
            {
                cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha", mapPath);

                if (cacheOk)
                {
                    return;
                }
                else
                {
                    if (mapFilename.ToLower().Contains("v6.83s") || mapFilename.ToLower().Contains("iccup"))
                    {
                        cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\dota.dha", ReplayParserCore.CachePath + "\\dota.w3x");
                        if (cacheOk)
                        {
                            replay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha");
                            return;
                        }
                        else
                        {
                            DialogResult dr = MessageBox.Show("The map for this replay ('" + mapPath + "') was not found 4." +
                                "\nIf this map is located somewhere else, you can open it manually if you press 'Yes'." +
                                "\nNote that if you don't have the required map you can open other DotA map which version is the closest to required (to avoid bugs)." +
                                "\nPressing 'No' will not stop the parsing process, but the information on heroes and items will not be present (only player names and chatlog)." +
                                "\nDo you want to manually specify the map file?", "Map file was not found", MessageBoxButtons.YesNo);

                            if (dr == DialogResult.Yes)
                                mlSettings.Filename = null;
                            else
                                return;
                        }
                    }
                }
            }

            DHMAIN.LoadMap(mlSettings);
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new DotaHIT.Extras.ReplayParserForm());
            //return;
            //args = new string[1];
            //args[0] = @"D:\Bob\Programming\WorkSpace\DotaHIT\DotaHAB\bin\Release\Anti-Mage test.dhb";    
            if (args.Length > 0)
            {
                string filename = args[0];

                if (!File.Exists(filename))
                    MessageBox.Show("File '" + filename + "' does not exists", "DotA H.I.T.");
                else
                {
                    switch (Path.GetExtension(filename).ToLower())
                    {
                        case ".w3x":
                            if (args.Length > 3 && args[1] == "RunPlugin1")
                            {
                                MessageBox.Show("1");
                                MainForm mf = new MainForm(filename);

                                object plugin = null;
                                foreach (DotaHIT.Core.HabProperties hpsPluginType in Current.plugins)
                                    if (hpsPluginType.TryGetValue(args[2], out plugin))
                                    {
                                        (plugin as DotaHIT.Plugins.IDotaHITPlugin1).Click(args[3], EventArgs.Empty);
                                        break;
                                    }

                                if (plugin == null)
                                    MessageBox.Show("Failed to find plugin '" + args[2] + "'");
                            }
                            else if (args.Length == 1)
                            {
                                MessageBox.Show("2");
                                Application.Run(new MainForm(filename));
                            }

                            break;
                        case ".dhb":
                        case ".w3g":
                            if (args.Length == 2)
                            {
                                if (args[1] == "scan" || args[1] == "scanner")
                                {
                                    SKIP_HEADER_CHECK = true;
                                    ReplayRawDataForm rawDataForm = new DotaHIT.Extras.Replay_Parser.ReplayRawDataForm();
                                    ParseSettings parseSettings = new ParseSettings();
                                    parseSettings.EmulateInventory = true;
                                    Replay currentReplay = new Replay(filename, MapRequired, parseSettings);
                                    rawDataForm.ForceScanner = true;
                                    rawDataForm.StartPosition = FormStartPosition.CenterScreen;
                                    rawDataForm.Init(currentReplay);
                                    Application.Run(rawDataForm);
                                }
                            }
                            else
                                Application.Run(new MainForm(filename));
                            break;
                        default:
                            MessageBox.Show("Unknown file extenstion", "DotA H.I.T.");
                            Application.Run(new MainForm(true));
                            break;
                    }
                    return;
                }
            }

            try
            {
                Application.Run(new MainForm(true));
            }
            catch (Exception e)
            {
                Application.Run(GetErrorReportForm("Error report", e));
            }
        }

        public static Form GetErrorReportForm(string caption, Exception e)
        {
            Form f = new Form();

            f.Text = caption;
            f.Width = 400;
            f.StartPosition = FormStartPosition.CenterScreen;
            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.Text = "Error message: " + e.Message + "\r\n\r\nStackTrace:\r\n" + e.StackTrace;
            f.Controls.Add(tb);
            tb.Dock = DockStyle.Fill;

            return f;
        }
    }
}