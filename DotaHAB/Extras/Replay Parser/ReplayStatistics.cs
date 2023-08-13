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
using DotaHIT.DatabaseModel.Format;

namespace DotaHIT.Extras.Replay_Parser
{
    public partial class ReplayStatistics : Form
    {
        ReplayBrowserForm parser;                        
        HabPropertiesCollection hpcReplayParserCfg = null;
        List<IReplay> results = new List<IReplay>();

        public ReplayStatistics(ReplayBrowserForm parser, string initialPath)
        {
            InitializeComponent();            

            this.parser = parser;
            pathTextBox.Text = initialPath;            

            hpcReplayParserCfg = DHCFG.Items["Extra"].GetHpcValue("ReplayParser", true);            
        }       

        private void startB_Click(object sender, EventArgs e)
        {
            results.Clear();

            if (!Directory.Exists(pathTextBox.Text))
            {
                MessageBox.Show("Specified path does not exists");
                return;
            }
            string[] files = Directory.GetFiles(pathTextBox.Text, "*.w3g", subfoldersCB.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            startB.Enabled = false;            

            SplashScreen splashScreen = new SplashScreen();            
            splashScreen.Owner = this;            
            splashScreen.StopButtonVisible = true;
            splashScreen.Show();
            splashScreen.ShowText("Searching...");

            ParseSettings parseSettings = new ParseSettings();            
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
                    if (replayStats.IsDota)
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
                            results.Add(replay);

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
                    splashScreen.ShowText("Processing..." + index + "/" + files.Length + "   Relevant: " + results.Count);
                    Application.DoEvents();
                }

                if (splashScreen.StopButtonClicked)
                    break;
            }

            sw.Stop();            

            rpsLabel.Text = "Relevant replays: " + results.Count + "   Replays scanned per second: " + (float)(index / sw.Elapsed.TotalSeconds);            

            if (useReplayStatsCache)
                ReplayStatsCache.Save();

            DisplayResults();

            splashScreen.Close();
            startB.Enabled = true;
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
                    return;
                }
                else
                {
                    if (mapFilename.ToLower().Contains("dota"))
                    {
                        cacheOk = replay.MapCache.LoadFromFile(ReplayParserCore.CachePath + "\\dota.dha", mapPath);
                        if (cacheOk)
                        {
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

        public class HeroStatistics
        {
            public Image Icon { get; set; }
            public string Name { get; set; }
            public int GamesPlayed { get; set; }
            public int GamesFinished { get; set; }
            public int GamesWon { get; set; }
            public int GamesLost { get; set; }
            public float WinPercentage { get; set; }
            public float PickPercentage { get; set; }
            public int TotalKills { get; set; }
            public int TotalDeaths { get; set; }
            public int TotalAssists { get; set; }
            public float KillsPerGame { get; set; }
            public float DeathsPerGame { get; set; } 
            public float AssistsPerGame { get; set; }
        }

        public class PlayerStatistics
        {            
            public string Name { get; set; }
            public int GamesPlayed { get; set; }
            public int GamesFinished { get; set; }
            public int GamesWon { get; set; }
            public int GamesLost { get; set; }
            public float WinPercentage { get; set; }            
            public int TotalKills { get; set; }
            public int TotalDeaths { get; set; }
            public int TotalAssists { get; set; }
            public float KillsPerGame { get; set; }
            public float DeathsPerGame { get; set; }
            public float AssistsPerGame { get; set; }
        }


        void DisplayResults()
        {
            DisplayHeroStatistics();
            DisplayPlayerStatistics();
        }

        void DisplayHeroStatistics()
        {
            Dictionary<string, HeroStatistics> dcHeroCache = new Dictionary<string, HeroStatistics>();
            SortableBindingList<HeroStatistics> list = new SortableBindingList<HeroStatistics>();

            List<string> replayHeroes = new List<string>();
            foreach (IReplay replay in results)
            {
                replayHeroes.Clear();
                foreach (IPlayer player in replay.Players)
                {
                    if (string.IsNullOrEmpty(player.HeroID) || player.IsComputer || player.IsObserver)
                        continue;

                    // skip 'same-hero' mode games 
                    if (replayHeroes.Contains(player.HeroID))
                        continue;

                    replayHeroes.Add(player.HeroID);

                    HeroStatistics hero;
                    if (!dcHeroCache.TryGetValue(player.HeroID, out hero))
                    {
                        hero = new HeroStatistics();

                        string mapPath = ReplayParserCore.GetProperMapPath(replay.MapPath);
                        string mapFilename = Path.GetFileNameWithoutExtension(mapPath);

                        ReplayMapCache.Database database;
                        if (ReplayMapCache.TryGetDatabase(ReplayParserCore.CachePath + "\\" + mapFilename + ".dha", mapPath, out database))
                        {
                            HabProperties hpsHero = database.hpcUnitProfiles[player.HeroID];

                            string imagePath = hpsHero.GetStringValue("Art");
                            hero.Icon = (imagePath != null) ? database.Resources.GetImage(imagePath) : Properties.Resources.armor;

                            hero.Name = hpsHero.GetStringValue("Name").Trim('\"');
                        }
                        else
                        {
                            hero.Icon = Properties.Resources.armor;
                            hero.Name = player.HeroID;
                        }

                        dcHeroCache.Add(player.HeroID, hero);
                    }

                    if (replay.Winner != TeamType.Unknown)
                    {
                        hero.GamesFinished++;

                        if (player.TeamType == replay.Winner)
                            hero.GamesWon++;
                        else
                            hero.GamesLost++;
                    }

                    hero.GamesPlayed++;
                    hero.WinPercentage = 100 * ((float)hero.GamesWon / (float)hero.GamesFinished);                    
                    hero.TotalKills += (player.Kills == -1) ? 0 : player.Kills;
                    hero.TotalDeaths += (player.Deaths == -1) ? 0 : player.Deaths;
                    hero.TotalAssists += (player.Assists == -1) ? 0 : player.Assists;
                    hero.KillsPerGame = (float)hero.TotalKills / (float)hero.GamesPlayed;
                    hero.DeathsPerGame = (float)hero.TotalDeaths / (float)hero.GamesPlayed;
                    hero.AssistsPerGame = (float)hero.TotalAssists / (float)hero.GamesPlayed;
                }
            }

            foreach (HeroStatistics hero in dcHeroCache.Values)
            {
                hero.PickPercentage = 100 * ((float)hero.GamesPlayed / (float)results.Count);
                list.Add(hero);
            }

            heroBindingSource.DataSource = list;
        }

        void DisplayPlayerStatistics()
        {
            Dictionary<string, PlayerStatistics> dcPlayerCache = new Dictionary<string, PlayerStatistics>();
            SortableBindingList<PlayerStatistics> list = new SortableBindingList<PlayerStatistics>();

            List<string> replayPlayers = new List<string>();            
            foreach (IReplay replay in results)
            {
                replayPlayers.Clear();
                foreach (IPlayer player in replay.Players)
                {
                    if (string.IsNullOrEmpty(player.Name) || string.IsNullOrEmpty(player.HeroID) || player.IsComputer || player.IsObserver)
                        continue;

                    // skip players with same nicks in the game
                    if (replayPlayers.Contains(player.Name))
                        continue;

                    replayPlayers.Add(player.Name);

                    PlayerStatistics playerStats;
                    if (!dcPlayerCache.TryGetValue(player.Name, out playerStats))
                    {
                        playerStats = new PlayerStatistics{ Name = player.Name };
                        dcPlayerCache.Add(player.Name, playerStats);
                    }

                    if (replay.Winner != TeamType.Unknown)
                    {
                        playerStats.GamesFinished++;

                        if (player.TeamType == replay.Winner)
                            playerStats.GamesWon++;
                        else
                            playerStats.GamesLost++;
                    }
                    
                    playerStats.GamesPlayed++;
                    playerStats.WinPercentage = 100 * ((float)playerStats.GamesWon / (float)playerStats.GamesFinished);
                    playerStats.TotalKills += (player.Kills == -1) ? 0 : player.Kills;
                    playerStats.TotalDeaths += (player.Deaths == -1) ? 0 : player.Deaths;
                    playerStats.TotalAssists += (player.Assists == -1) ? 0 : player.Assists;
                    playerStats.KillsPerGame = (float)playerStats.TotalKills / (float)playerStats.GamesPlayed;
                    playerStats.DeathsPerGame = (float)playerStats.TotalDeaths / (float)playerStats.GamesPlayed;
                    playerStats.AssistsPerGame = (float)playerStats.TotalAssists / (float)playerStats.GamesPlayed;
                }
            }

            foreach (PlayerStatistics playerStats in dcPlayerCache.Values)
            {                
                list.Add(playerStats);
            }

            playerBindingSource.DataSource = list;
        }

        private void searchB_Click(object sender, EventArgs e)
        {
            string searchString = specificPlayerSearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchString))
                return;

            if (results.Count == 0)
            {
                MessageBox.Show("No relevant replay data found. Make sure you pressed 'Start processing' button before using this feature.");
                return;
            }

            searchB.Enabled = false;

            ReplayStatisticsSpecificPlayerView specificPlayerView = new ReplayStatisticsSpecificPlayerView(results, searchString);

            TabPage page = new TabPage(searchString);
            page.Controls.Add(specificPlayerView);
            specificPlayerView.Dock = DockStyle.Fill;

            specificPlayersTabControl.TabPages.Add(page);
            specificPlayersTabControl.SelectedTab = page;
            specificPlayerView.CloseButtonClicked += (s, args) => 
            {
                if (specificPlayersTabControl.SelectedIndex > 0)
                    specificPlayersTabControl.SelectedIndex--;

                specificPlayersTabControl.TabPages.Remove(page);                
                specificPlayerView.Dispose(); }
            ;
            Application.DoEvents();

            specificPlayerView.DisplayStatistics();

            searchB.Enabled = true;
        }
    }
}