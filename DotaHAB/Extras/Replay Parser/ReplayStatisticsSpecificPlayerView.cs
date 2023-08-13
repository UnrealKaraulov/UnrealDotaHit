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
    public partial class ReplayStatisticsSpecificPlayerView : UserControl
    {
        string searchString;
        List<IReplay> results;

        public ReplayStatisticsSpecificPlayerView(List<IReplay> results, string searchString)
        {
            InitializeComponent();

            this.results = results;
            this.searchString = searchString;
        }

        public void DisplayStatistics()
        {
            string regexNickname = searchString.Replace("*", ".*");

            Dictionary<string, ReplayStatistics.PlayerStatistics> dcPlayerCache = new Dictionary<string, ReplayStatistics.PlayerStatistics>();
            Dictionary<string, ReplayStatistics.HeroStatistics> dcHeroCache = new Dictionary<string, ReplayStatistics.HeroStatistics>();
            SortableBindingList<ReplayStatistics.HeroStatistics> heroes = new SortableBindingList<ReplayStatistics.HeroStatistics>();

            int relevantReplays = 0;
            foreach (IReplay replay in results)
            {                
                foreach (IPlayer player in replay.Players)
                {
                    if (string.IsNullOrEmpty(player.Name) || string.IsNullOrEmpty(player.HeroID) || player.IsComputer || player.IsObserver)
                        continue;

                    if (Regex.IsMatch(player.Name, regexNickname, RegexOptions.IgnoreCase))
                    {
                        relevantReplays++;

                        ReplayStatistics.PlayerStatistics playerStats;
                        if (!dcPlayerCache.TryGetValue(player.Name, out playerStats))
                        {
                            playerStats = new ReplayStatistics.PlayerStatistics { Name = player.Name };
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

                        ReplayStatistics.HeroStatistics hero;
                        if (!dcHeroCache.TryGetValue(player.HeroID, out hero))
                        {
                            hero = new ReplayStatistics.HeroStatistics();

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

                        // go to next replay
                        break;
                    }
                }                
            }

            ReplayStatistics.PlayerStatistics totalPlayerStats = new ReplayStatistics.PlayerStatistics{ Name = searchString };

            string foundPlayers = "";
            foreach (ReplayStatistics.PlayerStatistics playerStats in dcPlayerCache.Values)
            {
                foundPlayers += playerStats.Name + ", ";
                totalPlayerStats.GamesFinished += playerStats.GamesFinished;
                totalPlayerStats.GamesWon += playerStats.GamesWon;
                totalPlayerStats.GamesLost += playerStats.GamesLost;
                totalPlayerStats.GamesPlayed += playerStats.GamesPlayed;
                totalPlayerStats.TotalKills += playerStats.TotalKills;
                totalPlayerStats.TotalDeaths += playerStats.TotalDeaths;
                totalPlayerStats.TotalAssists += playerStats.TotalAssists;
            }            

            totalPlayerStats.WinPercentage = 100 * ((float)totalPlayerStats.GamesWon / (float)totalPlayerStats.GamesFinished);
            totalPlayerStats.KillsPerGame = (float)totalPlayerStats.TotalKills / (float)totalPlayerStats.GamesPlayed;
            totalPlayerStats.DeathsPerGame = (float)totalPlayerStats.TotalDeaths / (float)totalPlayerStats.GamesPlayed;
            totalPlayerStats.AssistsPerGame = (float)totalPlayerStats.TotalAssists / (float)totalPlayerStats.GamesPlayed;

            playersTextBox.Text = foundPlayers.TrimEnd(',', ' ');

            foreach (ReplayStatistics.HeroStatistics hero in dcHeroCache.Values)
            {
                hero.PickPercentage = 100 * ((float)hero.GamesPlayed / (float)relevantReplays);
                heroes.Add(hero);
            }

            playerBindingSource.DataSource = (foundPlayers != "") ? totalPlayerStats : (object)typeof(ReplayStatistics.PlayerStatistics);
            playerHeroesBindingSource.DataSource = heroes;
        }

        public event EventHandler CloseButtonClicked;

        private void closeB_Click(object sender, EventArgs e)
        {
            if (CloseButtonClicked != null)
                CloseButtonClicked(this, e);
        }
    }
}
