using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Types;
using System.IO;
using System.Text.RegularExpressions;
using Deerchao.War3Share.W3gParser;
using System.Diagnostics;
using Be.Timvw.Framework.ComponentModel;

namespace DotaHIT.Extras.Replay_Parser
{
    public partial class ReplayFinder : Form
    {
        ReplayBrowserForm parser;
        List<unit> heroes = new List<unit>();
        Dictionary<string, string> heroNicksCache = new Dictionary<string, string>();
        List<string> previewNicknames = new List<string>();
        HabPropertiesCollection hpcReplayParserCfg = null;

        public ReplayFinder(ReplayBrowserForm parser, string initialPath)
        {
            InitializeComponent();

            if (Current.map == null)
            {
                heroesGridView.Enabled = false;
                heroesGridView.BackgroundColor = Color.Gainsboro;
            }            

            this.parser = parser;
            pathTextBox.Text = initialPath;            

            hpcReplayParserCfg = DHCFG.Items["Extra"].GetHpcValue("ReplayParser", true);
            
            unitsCheckTimer.Start();
        }

        private void unitsCheckTimer_Tick(object sender, EventArgs e)
        {
            if (Current.map != null && !heroesGridView.Enabled)
            {
                heroesGridView.Enabled = true;
                heroesGridView.BackgroundColor = Color.GhostWhite;
            }
            
            if (Current.player == null) return;

            List<unit> list = new List<unit>(Current.player.units.Count);

            foreach (unit u in Current.player.units.Values)
                if (u.IsHero)
                {
                    list.Add(u);

                    if (!heroNicksCache.ContainsKey(u.codeID))
                        heroNicksCache.Add(u.codeID, "");
                }

            foreach (unit u in heroes)
                if (u.codeID == "AnyHero")
                    list.Add(u);
                else
                    if (!list.Contains(u))
                        heroNicksCache.Remove(u.codeID); // clear unused hero-nicks from the cache

            heroes = list;
            heroesGridView.RowCount = heroes.Count;
        }

        private void heroesGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= heroes.Count) return;

            unit hero = heroes[e.RowIndex];
            if (hero.codeID == "AnyHero")
            {
                switch (e.ColumnIndex)
                {
                    case 0: // image
                        e.Value = null;
                        break;

                    case 1: // hero name
                        e.Value = "Any Hero (Nickname-only search)";
                        break;

                    case 2: // player
                        e.Value = hero.name;
                        break;
                }
            }
            else
            {
                HabProperties hpsHero = DHLOOKUP.hpcUnitProfiles[hero.codeID];
                switch (e.ColumnIndex)
                {
                    case 0: // image
                        e.Value = DHRC.Default.GetImage(hpsHero.GetStringValue("Art"));
                        break;

                    case 1: // hero name
                        e.Value = hero.ID;
                        break;

                    case 2: // player
                        string nick;
                        heroNicksCache.TryGetValue(hero.codeID, out nick);
                        e.Value = nick + "";
                        break;
                }
            }
        }

        private void heroesGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                unit hero = heroes[e.RowIndex];
                if (hero.codeID == "AnyHero")
                    hero.name = e.Value + "";
                else
                    heroNicksCache[hero.codeID] = e.Value + "";
            }
        }

        private void heroesGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                switch (e.KeyCode)
                {
                    case Keys.A:
                        heroes.Add(new unit("AnyHero"));
                        heroesGridView.RowCount = heroes.Count;
                        break;

                    case Keys.D:
                        if (heroesGridView.SelectedRows.Count > 0)
                        {
                            unit hero = heroes[heroesGridView.SelectedRows[0].Index];
                            heroes.Remove(hero);

                            if (hero.codeID != "AnyHero")
                                heroNicksCache.Remove(hero.codeID);
                            
                            hero.destroy();
                            heroesGridView.RowCount = heroes.Count;
                        }
                        break;
                }
        }

        private void startB_Click(object sender, EventArgs e)
        {
            previewNicknames.Clear();            

            foreach (unit hero in heroes)
                if (hero.codeID == "AnyHero" && !string.IsNullOrEmpty(hero.name))
                    previewNicknames.Add(hero.name);

            foreach (KeyValuePair<string, string> kvp in heroNicksCache)
                previewNicknames.Add(kvp.Value);

            iReplayBindingSource.Clear();
            SortableBindingList<IReplay> results = new SortableBindingList<IReplay>();

            if (!Directory.Exists(pathTextBox.Text))
            {
                MessageBox.Show("Specified path does not exists");
                return;
            }
            string[] files = Directory.GetFiles(pathTextBox.Text, "*.w3g", subfoldersCB.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            startB.Enabled = false;            
            resultsDataGridView.BackgroundColor = Color.WhiteSmoke;            

            SplashScreen splashScreen = new SplashScreen();            
            splashScreen.Owner = this;            
            splashScreen.StopButtonVisible = true;
            splashScreen.Show();
            splashScreen.ShowText("Searching...");

            ParseSettings parseSettings = new ParseSettings();
            parseSettings.Preview += replayPreview;
            parseSettings.MapPreview += replayMapPreview;
            parseSettings.EmulateInventory = true;

            bool useReplayStatsCache = hpcReplayParserCfg.GetIntValue("Replay", "UseStatsCache", 1) == 1;
            bool useMapCache = hpcReplayParserCfg.GetIntValue("Map", "UseCache", 1) == 1;

            // turn off all features that increase map loading time
            Current.mainForm.MinimizeMapLoadTime();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int index = 0;
            Replay replay;            
            foreach (string file in files)
            {
                index++;

                ReplayStats replayStats;
                if (useReplayStatsCache && ReplayStatsCache.TryGetReplayStats(file, out replayStats))
                {
                    if (replayStats.IsDota && IsReplayMatchesCriteria(replayStats))
                    {
                        results.Add(replayStats);
                    }
                }
                else
                {
                    try
                    {
                        replay = new Replay(file, MapRequired, parseSettings);

                        // if replay parsing was not cancelled
                        if (replay.IsPreview == false)
                        {
                            // continue checking replay 
                            if (IsReplayMatchesCriteria(replay))
                            {
                                results.Add(replay);
                            }

                            if (useMapCache || useReplayStatsCache)
                                ReplayParserCore.ParseLineups(replay);

                            if (useMapCache && Current.map != null && replay.MapCache.IsLoaded == false)
                                replay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + Path.GetFileNameWithoutExtension(replay.Map.Path) + ".dha");

                            if (useReplayStatsCache)
                                ReplayStatsCache.TryAddReplayStats(replay);
                        }
                        else
                            // if this replay was cancelled because it's a non-dota replay
                            if (replay.Tag == this)
                            {
                                // then save it's map cache and stats (if requested)
                                // so that this is the last time this replay is analyzed

                                if (useMapCache && Current.map != null && replay.MapCache.IsLoaded == false)
                                    replay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + Path.GetFileNameWithoutExtension(replay.Map.Path) + ".dha");

                                if (useReplayStatsCache)
                                    ReplayStatsCache.TryAddReplayStats(replay);
                            }

                        replay = null;
                    }
                    catch { }
                }

                if (index % 10 == 0)
                {
                    splashScreen.ShowProgress((double)index, (double)files.Length);
                    splashScreen.ShowText("Searching..." + index + "/" + files.Length + "   Found: " + results.Count);
                    Application.DoEvents();
                }

                if (splashScreen.StopButtonClicked)
                    break;
            }

            sw.Stop();            

            iReplayBindingSource.DataSource = results;
            resultsDataGridView.BackgroundColor = Color.AliceBlue;
            rpsLabel.Text = "Replays found: " + results.Count + "   Replays scanned per second: " + (float)(index / sw.Elapsed.TotalSeconds);

            if (useReplayStatsCache)
                ReplayStatsCache.Save();

            splashScreen.Close();
            startB.Enabled = true;
        }

        void replayPreview(Replay replay, ReplayPreviewEventArgs e)
        {
            e.CancelParsing = IsReplayPreviewMatchesCriteria(replay, previewNicknames) == false;
        }

        void replayMapPreview(Replay replay, ReplayPreviewEventArgs e)
        {
            e.CancelParsing = DHHELPER.IsDotaMapResources(replay.MapCache.Resources) == false;
            replay.Tag = e.CancelParsing ? this : null;
        }

        void MapRequired(object sender, EventArgs e)
        {
            Replay replay = sender as Replay;
            string mapPath = ReplayParserCore.GetProperMapPath(replay.Map.Path);
            string mapFilename = Path.GetFileNameWithoutExtension(mapPath);

            if (hpcReplayParserCfg.GetIntValue("Map", "UseCache", 1) == 1)
            {
                bool cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha", mapPath);

                if (cacheOk)
                {
                    MessageBox.Show("Found 1");
                    return;
                }
                else
                {
                    if (mapFilename.ToLower().Contains("dota"))
                    {
                        MessageBox.Show("HasDota!");
                        cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\dota.dha", mapPath);
                        if (cacheOk)
                        {
                            MessageBox.Show("HasDota! OKOKOKOKOKOKOK");
                            replay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha");
                            return;
                        }
                    }
                }

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

            MapLoadSettings mlSettings = new MapLoadSettings(mapPath, false, new SplashScreen(this));
            mlSettings.RefreshItemCombiningDataLookups = replay.ParseSettings.EmulateInventory;
            mlSettings.SupressErrorMessages = true;
            mlSettings.DisableNonDotaJass = true;

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

        bool IsReplayPreviewMatchesCriteria(Replay replay, List<string> nicknames)
        {
            // all specified nicknames must be present in the replay
            bool Ok;
            foreach (string nickname in nicknames)
            {
                Ok = false;
                string regexNickname = string.IsNullOrEmpty(nickname) ? ".*" : nickname.Replace("*", ".*");

                foreach (Player p in replay.Players)
                    if (Regex.IsMatch(p.Name, regexNickname, RegexOptions.IgnoreCase))
                    {
                        Ok = true;
                        break;
                    }

                if (!Ok) return false;
            }
            
            return true;
        }

        bool IsReplayMatchesCriteria(Replay replay)
        {                                    
            // all specified hero-nickname pairs must be present in the replay
            bool Ok;
            foreach (KeyValuePair<string,string> kvp in heroNicksCache)
            {
                Ok = false;
                string regexNickname = string.IsNullOrEmpty(kvp.Value) ? ".*" : kvp.Value.Replace("*", ".*");

                foreach (Player p in replay.Players)
                {
                    Hero hero = p.GetMostUsedHero();
                    if (hero != null && kvp.Key == hero.Name && Regex.IsMatch(p.Name, regexNickname, RegexOptions.IgnoreCase))
                    {
                        Ok = true;
                        break;
                    }
                }

                if (!Ok) return false;
            }

            return true;
        }

        bool IsReplayMatchesCriteria(ReplayStats replayStats)
        {
            // all specified nicknames must be present in the replay
            bool Ok;
            foreach (string nickname in previewNicknames)
            {
                Ok = false;
                string regexNickname = string.IsNullOrEmpty(nickname) ? ".*" : nickname.Replace("*", ".*");

                foreach (ReplayStats.PlayerStats p in replayStats.PlayersStats)
                    if (Regex.IsMatch(p.Name, regexNickname, RegexOptions.IgnoreCase))
                    {
                        Ok = true;
                        break;
                    }

                if (!Ok) return false;
            }

            // all specified hero-nickname pairs must be present in the replay            
            foreach (KeyValuePair<string, string> kvp in heroNicksCache)
            {
                Ok = false;
                string regexNickname = string.IsNullOrEmpty(kvp.Value) ? ".*" : kvp.Value.Replace("*", ".*");

                foreach (ReplayStats.PlayerStats p in replayStats.PlayersStats)
                {                    
                    if (p.HeroID != null && kvp.Key == p.HeroID && Regex.IsMatch(p.Name, regexNickname, RegexOptions.IgnoreCase))
                    {
                        Ok = true;
                        break;
                    }
                }

                if (!Ok) return false;
            }

            return true;
        }

        private void resultsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            parser.DisplaySourceItem(iReplayBindingSource);            
        }

        private void resultsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == gameLengthDataGridViewTextBoxColumn.DisplayIndex)
            {
                TimeSpan ts = (TimeSpan)e.Value;
                e.Value = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
                e.FormattingApplied = true;
            }
        }          
    }
}