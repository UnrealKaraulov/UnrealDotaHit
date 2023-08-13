using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ExpTreeLib;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Format;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Jass.Native.Types;
using BitmapUtils;

namespace DotaHIT.Extras
{
    public partial class ReplayBrowserForm : Form
    {
        //private ManualResetEvent Event1 = new ManualResetEvent(true);
        private bool showPlayerColors = false;
        private Replay currentReplay = null;
        private RichTextBox buffer = new RichTextBox();
        private Replay_Parser.ReplayFinder replayFinder = null;
        private Replay_Parser.ReplayStatistics replayStatistics = null;
        HabPropertiesCollection hpcReplayParserCfg = null;        

        Dictionary<string, Replay> dcReplayCache = new Dictionary<string, Replay>();

        ToolStripMenuItem watchReplayItem;

        public ReplayBrowserForm()
        {
            InitializeComponent();
            sentinelBottomLineUpTS.Renderer = UIRenderers.NoBorderRenderer;
            sentinelMiddleLineUpTS.Renderer = UIRenderers.NoBorderRenderer;
            sentinelTopLineUpTS.Renderer = UIRenderers.NoBorderRenderer;
            scourgeBottomLineUpTS.Renderer = UIRenderers.NoBorderRenderer;
            scourgeMiddleLineUpTS.Renderer = UIRenderers.NoBorderRenderer;
            scourgeTopLineUpTS.Renderer = UIRenderers.NoBorderRenderer;
            bansTS.Renderer = UIRenderers.NoBorderRenderer;
            picksTS.Renderer = UIRenderers.NoBorderRenderer;            

            this.Icon = Properties.Resources.Icon;

            buffer.Font = sentinelRTB.Font;
            buffer.SelectionColor = sentinelRTB.SelectionColor;

            watchReplayItem = new ToolStripMenuItem("View in WarCraft III");
            watchReplayItem.Click += new EventHandler(watchReplayItem_Click);
            browser.FilesContextMenuStrip.Items.Insert(1, watchReplayItem);
            browser.ContextMenuShowing += new EventHandler(browser_ContextMenuShowing);
            
            PreviewReplay(null);

            Control.CheckForIllegalCrossThreadCalls = false;

            AttachPlugins();

            hpcReplayParserCfg = DHCFG.Items["Extra"].GetHpcValue("ReplayParser", true);

            this.WindowState = (hpcReplayParserCfg.GetIntValue("UI", "Maximized", 0) == 1) ? FormWindowState.Maximized : FormWindowState.Normal;
            this.cacheMapsTSMI.Checked = hpcReplayParserCfg.GetIntValue("Map", "UseCache", 1) == 1;
            this.cacheReplayStatsTSMI.Checked = hpcReplayParserCfg.GetIntValue("Replay", "UseStatsCache", 1) == 1;
        }             

        public ReplayBrowserForm(string path)
            : this()
        {            
            browser.SelectedPath = path;

            this.Show();
        }

        public ReplayBrowserForm(string file, bool open)
            : this()
        {
            if (open)
            {
                if (ParseReplay(file))
                {
                    DisplayReplay(currentReplay);                    
                }
            }            

            this.Show();

            Application.DoEvents();

            browser.EnsureExplorerLoaded();
            browser.SelectedPath = Path.GetDirectoryName(file);
            browser.SelectedFile = file;            
        }

        private void ReplayParserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReplayStatsCache.Save();

            SaveReplayExportConfig(configurationCmbB.SelectedItem as string);

            hpcReplayParserCfg["UI", "Maximized"] = this.WindowState == FormWindowState.Maximized ? 1 : 0;

            Current.mainForm.ReplayParserForms.Remove(this);

            // remove plugin controls from tabpages
            // to avoid them being disposed
            foreach (TabPage page in replayTabControl.TabPages)
                page.Controls.Clear();
        }

        void AttachPlugins()
        {            
            HabProperties hpsPlugins;
            if (Current.plugins.TryGetValue("ReplayParser", out hpsPlugins))            
                foreach(Plugins.IDotaHITPlugin plugin in hpsPlugins.Values)
                {
                    TabPage tbPlugin = new TabPage(plugin.Name);
                    replayTabControl.TabPages.Add(tbPlugin);

                    tbPlugin.Controls.Add(plugin.Panel);
                }
        }

        public Replay CurrentReplay
        {
            get
            {
                return currentReplay;
            }
        }

        private void browser_FileOk(object sender, CancelEventArgs e)
        {
            string file = browser.SelectedFile;

            if (string.IsNullOrEmpty(file)) return;

            if (Path.GetExtension(file).ToLower() == ".w3g")
            {
                e.Cancel = true; // suppress file execution

                if (ParseReplay(file))                
                    DisplayReplay(currentReplay);                
            }
        }

        private void browser_SelectedItemsChanged(object sender, FileBrowser.SelectedItemsChangedEventArgs e)
        {
            if (tabControl.SelectedTab == parseTabPage)
                return;

            if (e.SelectedItems.Count > 0)
            {
                CShItem item = e.SelectedItems[0];

                if (item.IsFileSystem && !item.IsFolder)
                {
                    string ext = Path.GetExtension(item.Path);
                    if (ext == ".w3g")
                    {
                        //lv1.Cursor = Cursors.AppStarting;
                        PreviewReplay(item.Path);
                        //lv1.Cursor = Cursors.Default;
                        return;
                    }
                }
            }

            PreviewReplay(null);
        }

        private void browser_SelectedPathChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == parseTabPage)
                return;

            PreviewReplay(null);
        }

        public static Color playerColorToColor(PlayerColor pc)
        {
            switch (pc)
            {
                case PlayerColor.Blue: return Color.FromArgb(0, 64, 248);//Color.Blue;
                case PlayerColor.Brown: return Color.FromArgb(153, 102, 102);
                case PlayerColor.Cyan: return Color.FromArgb(0, 200, 200);//Color.Cyan;
                case PlayerColor.DarkGreen: return Color.MediumSeaGreen;
                case PlayerColor.Gray: return Color.FromArgb(180, 180, 180);
                case PlayerColor.Green: return Color.Green;
                case PlayerColor.LightBlue: return Color.FromArgb(87, 188, 249);//Color.LightSkyBlue;
                case PlayerColor.Observer: return Color.White;
                case PlayerColor.Orange: return Color.FromArgb(255, 165, 0);
                case PlayerColor.Pink: return Color.FromArgb(255, 153, 204);//return Color.HotPink;
                case PlayerColor.Purple: return Color.FromArgb(153, 102, 204);
                case PlayerColor.Red: return Color.Red;
                case PlayerColor.Yellow: return Color.FromArgb(200, 200, 70);//Color.Yellow;
                default: return Color.Black;
            }
        }        

        private void FillPlayerList(RichTextBox rtb, List<Player> players)
        {            
            buffer.Clear();
            for (int i = 0; i < players.Count; i++)
            {                
                if (showPlayerColors)                
                    buffer.SelectionColor = playerColorToColor(players[i].Color);

                buffer.AppendText(players[i].Name);

                if (i + 1 < players.Count)
                {
                    buffer.SelectionColor = Color.Black;
                    buffer.AppendText(", ");
                }
            }

            rtb.Rtf = buffer.Rtf;
        }
      
        internal void DisplayReplayInfo(Replay replay)
        {
            currentReplay = replay;

            if (replay != null)
            {
                mapTextBox.Text = replay.Map.Path;
                hostTextBox.Text = replay.Host.Name;

                foreach (Team t in replay.Teams)
                    switch (t.Type)
                    {
                        case TeamType.Sentinel:
                            FillPlayerList(sentinelRTB, t.Players);
                            break;

                        case TeamType.Scourge:
                            FillPlayerList(scourgeRTB, t.Players);
                            break;
                    }

                string path = ReplayParserCore.GetProperMapPath(replay.Map.Path);
                if (File.Exists(path))
                {
                    DisplayUserInfo(Color.Green, "Ready", "for parsing",true);
                }
                else
                    DisplayUserInfo(Color.Red, "Map file", "not found", false, true);
            }
            else
            {
                mapTextBox.Text = "";
                hostTextBox.Text = "";
                sentinelRTB.Clear();
                scourgeRTB.Clear();
            }
        }

        internal void DisplayUserInfo(Color color, string strA, string strB, bool canParse)
        {
            DisplayUserInfo(color, strA, strB, canParse, canParse);
        }
        internal void DisplayUserInfo(Color color, string strA, string strB, bool canParse, bool canExtract)
        {
            userInfoPanel.BackColor = color;
            userInfoALabel.Text = strA;
            userInfoBLabel.Text = strB;
            parseB.Enabled = canParse;
            parseToolStripMenuItem.Enabled = canParse;
            extractToolStripMenuItem.Enabled = canExtract;
        }

        private void PreviewReplay(string filename)
        {
            if (filename != null)
            {
                if (!dcReplayCache.TryGetValue(filename, out currentReplay))
                {
                    try
                    {
                        currentReplay = new Replay(filename, true);
                    }
                    catch
                    {
                        currentReplay = null;
                        DisplayUserInfo(Color.Red, "The file", "is corrupt", false);
                    }
                    dcReplayCache[filename] = currentReplay;
                }

                DisplayReplayInfo(currentReplay);                            
            }
            else
            {
                currentReplay = null;
                DisplayReplayInfo(currentReplay);
                DisplayUserInfo(Color.FromArgb(33,136,235), "Select", "Replay File", false);
            }
        }      

        private void playerColorsLL_MouseDown(object sender, MouseEventArgs e)
        {
            showPlayerColors = !showPlayerColors;
            playerColorsLL.Text = "Player Colors: " + (showPlayerColors ? "On" : "Off");
            DisplayReplayInfo(currentReplay);
        }

        private void parseB_Click(object sender, EventArgs e)
        {
            if (ParseReplay(browser.SelectedFile))
                DisplayReplay(currentReplay);
        }

        public bool ParseReplay(string filename)
        {
            if (filename == null) return false;

            DHTIMER.StartNewCount();
            DHCFG.Items["Path"]["ReplayLoad"] = Path.GetDirectoryName(filename);

            if (!dcReplayCache.TryGetValue(filename, out currentReplay) || currentReplay.IsPreview)
            {
                try
                {
                    ParseSettings parseSettings = new ParseSettings();
                    parseSettings.EmulateInventory = true;                    

                    currentReplay = new Replay(filename, MapRequired, parseSettings);
                    dcReplayCache[filename] = currentReplay;
                }
                catch(Exception e)
                {
                    if (MessageBox.Show("An error occured while parsing this replay. Do you want to see the error message?", "Parse error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {                        
                        Form f = Program.GetErrorReportForm("Parse Error report", e);               
                        f.Show();
                    }
                    currentReplay = null;
                    return false;
                }                
            }

            DHTIMER.PrintEndCount("ParseReplay");
            DHTIMER.ResetCount();

            return true;            
        }

        public void MapRequired(object sender, EventArgs e)
        {
            Replay replay = sender as Replay;
            string mapPath = ReplayParserCore.GetProperMapPath(replay.Map.Path);
            string mapFilename = Path.GetFileNameWithoutExtension(mapPath);

            if (hpcReplayParserCfg.GetIntValue("Map", "UseCache", 1) == 1)
            {
                Stopwatch sw = new Stopwatch(); sw.Start();


                bool cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha", mapPath);

             
                if (cacheOk)
                {
                    sw.Stop(); Console.WriteLine("MapCacheLoad: " + ((float)sw.ElapsedMilliseconds / (float)1000.0));

                    return;
                }
                else
                {
                    if (mapFilename.ToLower().Contains("dota"))
                    {
                        cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\dota.dha", mapPath);
                        if (cacheOk)
                        {
                            sw.Stop(); Console.WriteLine("MapCacheLoad: " + ((float)sw.ElapsedMilliseconds / (float)1000.0));

                            replay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha");
                            return;
                        }
                    }
                }

                sw.Stop(); Console.WriteLine("MapCacheBadLoad: " + ((float)sw.ElapsedMilliseconds / (float)1000.0));

            }

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

            MapLoadSettings mlSettings = new MapLoadSettings(mapPath, false, new SplashScreen(this));
            mlSettings.RefreshItemCombiningDataLookups = replay.ParseSettings.EmulateInventory;

            if (!File.Exists(mapPath))
            {
                DialogResult dr = MessageBox.Show("The map for this replay ('" + mapPath + "') was not found." +
                    "\nIf this map is located somewhere else, you can open it manually if you press 'Yes'." +
                    "\nNote that if you don't have the required map you can open other DotA map which version is the closest to required (to avoid bugs)." +
                    "\nPressing 'No' will not stop the parsing process, but the information on heroes and items will not be present (only player names and chatlog)." +
                    "\nDo you want to manually specify the map file?", "Map file was not found", MessageBoxButtons.YesNo);

                if (dr == DialogResult.Yes)
                    mlSettings.Filename = null;
                else
                    return;
            }
            
            DHMAIN.LoadMap(mlSettings);            

            this.BringToFront();
        }

        void ViewReplayInWarCraft(string filename)
        {
            string war3exePath = DHCFG.Items["Path"].GetStringValue("War3") + Path.DirectorySeparatorChar + "war3.exe";
            string war3exeArgs = "-loadfile " + "\"" + filename + "\"";

            Process.Start(war3exePath, war3exeArgs);
        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parseB.PerformClick();
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Replay_Parser.ReplayDataExtractForm extractDataForm = new DotaHIT.Extras.Replay_Parser.ReplayDataExtractForm(browser.SelectedFile, MapRequired);
            extractDataForm.ShowDialog();
        }

        private void replayFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (replayFinder != null && !replayFinder.IsDisposed)
                replayFinder.BringToFront();
            else
            {
                replayFinder = new DotaHIT.Extras.Replay_Parser.ReplayFinder(this, browser.SelectedPath);
                replayFinder.Show();
            }
        }

        private void replayStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (replayStatistics != null && !replayStatistics.IsDisposed)
                replayStatistics.BringToFront();
            else
            {
                replayStatistics = new DotaHIT.Extras.Replay_Parser.ReplayStatistics(this, browser.SelectedPath);
                replayStatistics.Show();
            }
        }

        private void cacheMapsTSMI_Click(object sender, EventArgs e)
        {
            hpcReplayParserCfg["Map", "UseCache"] = cacheMapsTSMI.Checked ? 1 : 0;
        }

        private void cacheReplayStatsTSMI_Click(object sender, EventArgs e)
        {
            hpcReplayParserCfg["Replay", "UseStatsCache"] = cacheReplayStatsTSMI.Checked ? 1 : 0;
        }

        void browser_ContextMenuShowing(object sender, EventArgs e)
        {
            watchReplayItem.Visible = (Path.GetExtension(browser.SelectedFile) == ".w3g");
        }   

        void watchReplayItem_Click(object sender, EventArgs e)
        {
            ViewReplayInWarCraft(browser.SelectedFile);
        }              
    }
}