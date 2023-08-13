using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ExpTreeLib;
using System.Collections;
using System.Collections.Specialized;
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
        public static readonly string ReplayExportCfgFileName = Application.StartupPath + "\\" + "dhrexport.cfg";

        public static event MethodInvoker ReplayDisplayed;

        private bool openBuildsInForms = false;
        private string originalExport = string.Empty;
        Dictionary<string, string> dcUsedHeroTags = null;
        private string currentExportCfgFileName = null;
        private int currentExportLayout = -1;
        private string[] defaultHeroFormatList = new string[] { "%tag", "%cl", "%n The %cl" };
        private string[] bbCodeItems = new string[]
            {
                "Bans", "Picks", "HeroString",
                "Sentinel", "Scourge",
                "Player1", "Player2", "Player3", "Player4", "Player5",
                "Player6", "Player7", "Player8", "Player9", "Player10",
            };

        HabPropertiesCollection hpcExportCfg = null;
        BindingSource replaySource = null;

        public static Color playerColorToColorChat(PlayerColor pc)
        {
            switch (pc)
            {
                case PlayerColor.Blue: return Color.FromArgb(30, 130, 255);
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
                case PlayerColor.Yellow: return Color.FromArgb(255, 255, 102);//Color.Yellow;
                default: return Color.Black;
            }
        }
        public static Color playerColorToColorBG(PlayerColor pc)
        {
            switch (pc)
            {
                case PlayerColor.Red: return Color.Red;
                case PlayerColor.Blue: return Color.FromArgb(0, 8, 160);
                case PlayerColor.Cyan: return Color.FromArgb(0, 164, 120);
                case PlayerColor.Purple: return Color.FromArgb(112, 8, 160);
                case PlayerColor.Yellow: return Color.FromArgb(160, 164, 0);//Color.Yellow;
                case PlayerColor.Orange: return Color.FromArgb(160, 112, 0);
                case PlayerColor.Green: return Color.Green;
                case PlayerColor.Pink: return Color.FromArgb(152, 84, 136);
                case PlayerColor.Gray: return Color.FromArgb(112, 120, 112);
                case PlayerColor.LightBlue: return Color.FromArgb(112, 140, 160);//Color.LightSkyBlue;                
                case PlayerColor.DarkGreen: return Color.FromArgb(40, 108, 72);
                case PlayerColor.Brown: return Color.FromArgb(80, 56, 32);
                case PlayerColor.Observer: return Color.White;
                default: return Color.Black;
            }
        }

        public void DisplayReplay(Replay replay)
        {
            this.Text = "DotA H.I.T. Replay Browser/Parser: " + Path.GetFileName(replay.FileName);
            tabControl.SelectedTab = parseTabPage;
            tabControl.SuspendLayout();

            DHTIMER.ResetCount();
            DHTIMER.StartCount();

            ClearPreviousData();

            PrepareMapImage(); DHTIMER.PrintRefreshCount("PrepareMapImage");
            ReplayParserCore.ParseLineups(replay); DHTIMER.PrintRefreshCount("ParseLineups");
            DisplayTeams(replay); DHTIMER.PrintRefreshCount("DisplayTeams");
            PlacePlayersOnTheMap(); DHTIMER.PrintRefreshCount("PlacePlayersOnTheMap");
            DisplayDescription(replay); DHTIMER.PrintRefreshCount("DisplayDescription");
            DisplayBansAndPicks(replay); DHTIMER.PrintRefreshCount("DisplayBansAndPicks");
            DisplayChat(replay); DHTIMER.PrintRefreshCount("DisplayChat");
            DisplayStatistics(replay); DHTIMER.PrintRefreshCount("DisplayStatistics");
            DisplayKillLog(replay); DHTIMER.PrintRefreshCount("DisplayKillLog");

            InitPlugins(); DHTIMER.PrintRefreshCount("InitPlugins");

            if (Current.map != null && currentReplay.MapCache.IsLoaded == false && hpcReplayParserCfg.GetIntValue("Map", "UseCache", 1) == 1)
            {
                bool ok = currentReplay.MapCache.SaveToFile(ReplayParserCore.CachePath + "\\" + Path.GetFileNameWithoutExtension(currentReplay.Map.Path) + ".dha");
                if (!ok) MessageBox.Show("Failed to save the map cache file."); DHTIMER.PrintRefreshCount("SaveMapCache");
            }

            DHTIMER.EndCount();
            DHTIMER.ResetCount();

            tabControl.ResumeLayout();

            if (ReplayDisplayed != null) ReplayDisplayed();
        }

        internal bool DisplaySourceItem(BindingSource replaySource)
        {
            IReplay replay = replaySource.Current as IReplay;
            if (replay == null)
                return false;

            this.replaySource = replaySource;
            if (ParseReplay(replay.ReplayPath))
            {
                DisplayReplay(CurrentReplay);

                Application.DoEvents();

                browser.SelectedPath = Path.GetDirectoryName(replay.ReplayPath);
                browser.SelectedFile = replay.ReplayPath;

                return true;
            }

            return false;
        }

        void closeReplayB_Click(object sender, EventArgs e)
        {
            string file = browser.SelectedFile;

            replaySource = null;
            tabControl.SelectedTab = browseTabPage;
            this.Text = "DotA H.I.T. Replay Browser/Parser";

            browser.SelectedFile = file;
            browser.explorerLV.Select();
        }

        private void nextReplayB_Click(object sender, EventArgs e)
        {
            if (replaySource != null)
            {
                replaySource.MoveNext();
                DisplaySourceItem(replaySource);
                return;
            }

            if (browser.MoveNext(true))
                parseB_Click(sender, e);
        }

        private void previousReplayB_Click(object sender, EventArgs e)
        {
            if (replaySource != null)
            {
                replaySource.MovePrevious();
                DisplaySourceItem(replaySource);
                return;
            }

            if (browser.MovePrev(true))
                parseB_Click(sender, e);
        }

        private void viewReplayB_Click(object sender, EventArgs e)
        {
            ViewReplayInWarCraft(browser.SelectedFile);
        }

        protected void ClearPreviousData()
        {
            for (int i = 0; i < replayTabControl.TabPages.Count; i++)
                if (replayTabControl.TabPages[i].Tag is Player)
                {
                    replayTabControl.TabPages.RemoveAt(i);
                    i--;
                }

            PrepareReplayExport();
        }

        protected void PrepareMapImage()
        {
            if (Current.map == null && currentReplay.MapCache.IsLoaded == false)
            {
                mapPanel.Visible = false;
                return;
            }
            else
                mapPanel.Visible = true;

            if (currentReplay.MapCache.mapImage != null)
                mapPanel.BackgroundImage = currentReplay.MapCache.mapImage;
            else  if (mapPanel.BackgroundImage == null || hpcReplayParserCfg.GetIntValue("Map", "UseCache", 1) == 1)
            {
                try
                {
                    Bitmap bmp = (Bitmap)(Image)currentReplay.MapCache.Resources.GetImage("war3mapMap.blp").Clone();
                    BitmapFilter.Gamma(bmp, 2.5, 2.5, 2.5);
                    mapPanel.BackgroundImage = bmp;

                    currentReplay.MapCache.mapImage = bmp;
                }
                catch
                {
                    try
                    {
                        Bitmap bmp = (Bitmap)(Image)currentReplay.MapCache.Resources.GetTgaImage("war3mapMap.tga").Clone();
                        BitmapFilter.Gamma(bmp, 2.5, 2.5, 2.5);
                        mapPanel.BackgroundImage = bmp;

                        currentReplay.MapCache.mapImage = bmp;
                    }
                    catch
                    {
                        currentReplay.MapCache.mapImage = (Bitmap)mapPanel.BackgroundImage;
                    }
                }
            }
            else
                currentReplay.MapCache.mapImage = (Bitmap)mapPanel.BackgroundImage;
        }

        protected void DisplayTeams(Replay replay)
        {
            sentinelTeamToolStrip.Items.Clear();
            scourgeTeamToolStrip.Items.Clear();

            foreach (Team t in replay.Teams)
                switch (t.Type)
                {
                    case TeamType.Sentinel:
                        displayTeam(sentinelTeamToolStrip, t);
                        break;

                    case TeamType.Scourge:
                        displayTeam(scourgeTeamToolStrip, t);
                        break;
                }

            versusLabel.Height = Math.Min(sentinelTeamToolStrip.PreferredSize.Height, scourgeTeamToolStrip.PreferredSize.Height);
        }
        void displayTeam(ToolStrip ts, Team team)
        {
            if (Current.map == null && currentReplay.MapCache.IsLoaded == false)
            {
                displayPlayersByLineUp(ts, team, LineUp.Unknown);
                return;
            }

            displayPlayersByLineUp(ts, team, LineUp.Top);
            displayPlayersByLineUp(ts, team, LineUp.Middle);
            displayPlayersByLineUp(ts, team, LineUp.Bottom);
            displayPlayersByLineUp(ts, team, LineUp.JungleOrRoaming);
        }
        void displayPlayersByLineUp(ToolStrip ts, Team team, LineUp lineUp)
        {
            bool separate = (ts.Items.Count != 0 && !(ts.Items[ts.Items.Count - 1] is ToolStripSeparator));

            foreach (Player p in team.Players)
                if (p.LineUp == lineUp)
                {
                    if (separate)
                    {
                        ToolStripSeparator tss = new ToolStripSeparator();
                        tss.Margin = new Padding(0, 5, 0, 5);
                        ts.Items.Add(tss);
                        separate = false;
                    }

                    ToolStripButton tsb = new ToolStripButton();
                    tsb.Text = p.Name;
                    switch (p.LineUp)
                    {
                        case LineUp.Top:
                        case LineUp.Middle:
                        case LineUp.Bottom:
                            tsb.Text += " (" + p.LineUp.ToString() + ")";
                            break;
                        case LineUp.JungleOrRoaming:
                            tsb.Text += " (" + "Jungle/Roaming" + ")";
                            break;
                    }
                    tsb.Font = UIFonts.tahoma9_75Bold;
                    tsb.BackColor = playerColorToColorBG(p.Color);
                    tsb.ForeColor = Color.White;
                    tsb.AutoToolTip = false;
                    tsb.Click += new EventHandler(playerToolStripButton_Click);
                    tsb.MouseDown += new MouseEventHandler(playerToolStripButton_MouseDown);
                    tsb.Tag = p;

                    if (p.Heroes.Count != 0)
                    {
                        string imagePath = currentReplay.MapCache.hpcUnitProfiles[p.GetMostUsedHero().Name, "Art"] as string;
                        tsb.Image = (imagePath != null) ? currentReplay.MapCache.Resources.GetImage(imagePath) : Properties.Resources.armor;
                    }
                    else
                        tsb.Image = Properties.Resources.armor;

                    tsb.ImageAlign = ContentAlignment.MiddleLeft;

                    ts.Items.Add(tsb);
                }
        }

        protected void PlacePlayersOnTheMap()
        {
            mapPanel.Visible = false;
            mapPanel.SuspendLayout();

            sentinelTopLineUpTS.SuspendLayout();
            sentinelMiddleLineUpTS.SuspendLayout();
            sentinelBottomLineUpTS.SuspendLayout();

            scourgeTopLineUpTS.SuspendLayout();
            scourgeMiddleLineUpTS.SuspendLayout();
            scourgeBottomLineUpTS.SuspendLayout();

            sentinelTopLineUpTS.Items.Clear();
            sentinelMiddleLineUpTS.Items.Clear();
            sentinelBottomLineUpTS.Items.Clear();

            scourgeTopLineUpTS.Items.Clear();
            scourgeMiddleLineUpTS.Items.Clear();
            scourgeBottomLineUpTS.Items.Clear();

            for (int i = 0; i < mapPanel.Controls.Count; i++)
                if (string.IsNullOrEmpty(mapPanel.Controls[i].Name))
                {
                    mapPanel.Controls.RemoveAt(i);
                    i--;
                }

            foreach (ToolStripItem tsi in sentinelTeamToolStrip.Items)
                if (tsi.Image != null)
                    placePlayerOnTheMap(tsi.Tag as Player, tsi.Image);

            foreach (ToolStripItem tsi in scourgeTeamToolStrip.Items)
                if (tsi.Image != null)
                    placePlayerOnTheMap(tsi.Tag as Player, tsi.Image);

            sentinelTopLineUpTS.ResumeLayout();
            sentinelMiddleLineUpTS.ResumeLayout();
            sentinelBottomLineUpTS.ResumeLayout();

            scourgeTopLineUpTS.ResumeLayout();
            scourgeMiddleLineUpTS.ResumeLayout();
            scourgeBottomLineUpTS.ResumeLayout();

            mapPanel.ResumeLayout();
            mapPanel.Visible = true;
        }
        void placePlayerOnTheMap(Player player, Image img)
        {
            if (player.LineUp == LineUp.JungleOrRoaming)
            {
                double realX = player.LineUpLocation.x - Jass.Native.Constants.map.minX;
                double realY = Jass.Native.Constants.map.maxY - player.LineUpLocation.y;
                double mapWidth = Jass.Native.Constants.map.maxX - Jass.Native.Constants.map.minX;
                double mapHeight = Jass.Native.Constants.map.maxY - Jass.Native.Constants.map.minY;

                double scaledX = ((double)mapPanel.Width) * (realX / mapWidth);
                double scaledY = ((double)mapPanel.Height) * (realY / mapHeight);

                ToolStrip ts = new ToolStrip();
                ts.BackColor = Color.Transparent;
                ts.GripStyle = ToolStripGripStyle.Hidden;
                ts.ImageScalingSize = sentinelBottomLineUpTS.ImageScalingSize;
                ts.Renderer = UIRenderers.NoBorderRenderer;
                ts.CanOverflow = false;
                ts.Dock = DockStyle.None;
                ts.LayoutStyle = ToolStripLayoutStyle.Flow;
                ts.Padding = new Padding(0);

                ToolStripButton tsb = new ToolStripButton();
                tsb.Image = img;
                tsb.ToolTipText = player.Name;
                tsb.Margin = new Padding(0, 0, 0, 0);
                tsb.Padding = new Padding(1, 1, 0, 0);
                tsb.BackColor = player.TeamType == TeamType.Sentinel ? Color.Red : Color.Lime;
                tsb.Tag = player;
                tsb.Click += new EventHandler(playerToolStripButton_Click);
                tsb.MouseDown += new MouseEventHandler(playerToolStripButton_MouseDown);

                ts.Items.Add(tsb);

                mapPanel.Controls.Add(ts);

                ts.Left = (int)scaledX - (tsb.Width / 2);
                ts.Top = (int)scaledY - (tsb.Height / 2);
            }
            else
            {
                switch (player.LineUp)
                {
                    case LineUp.Top:
                        addPlayerToMapLineUp(player, img,
                            (player.TeamType == TeamType.Sentinel) ? sentinelTopLineUpTS : scourgeTopLineUpTS,
                            false,
                            (player.TeamType == TeamType.Scourge));
                        break;

                    case LineUp.Middle:
                        addPlayerToMapLineUp(player, img,
                            (player.TeamType == TeamType.Sentinel) ? sentinelMiddleLineUpTS : scourgeMiddleLineUpTS,
                            true,
                            (player.TeamType == TeamType.Scourge));
                        break;

                    case LineUp.Bottom:
                        addPlayerToMapLineUp(player, img,
                            (player.TeamType == TeamType.Sentinel) ? sentinelBottomLineUpTS : scourgeBottomLineUpTS,
                            false,
                            (player.TeamType == TeamType.Scourge));
                        break;
                }
            }
        }
        void addPlayerToMapLineUp(Player p, Image img, ToolStrip ts, bool useAlternativeLayout, bool downToUpDirection)
        {
            ToolStripButton tsb = new ToolStripButton();
            tsb.Image = img;
            tsb.ToolTipText = p.Name;
            tsb.Margin = new Padding(1, 0, 0, 0);
            tsb.Padding = new Padding(1, 1, 0, 0);
            tsb.BackColor = p.TeamType == TeamType.Sentinel ? Color.Red : Color.Lime;
            tsb.Tag = p;
            tsb.Click += new EventHandler(playerToolStripButton_Click);
            tsb.MouseDown += new MouseEventHandler(playerToolStripButton_MouseDown);

            if (useAlternativeLayout)
            {
                if (downToUpDirection)
                {
                    switch (ts.Items.Count)
                    {
                        case 0:
                            ts.Items.AddRange(new ToolStripItem[]{
                                GetLabel(tsb, true),
                                GetLabel(tsb, true),
                                GetLabel(tsb, false, 4),
                                tsb,
                            });
                            break;

                        case 4:
                            if (ts.Items[0].Image == null)
                            {
                                ts.Items.RemoveAt(0);
                                ts.Items.Insert(0, tsb);
                                break;
                            }
                            else if (ts.Items[1].Image == null)
                            {
                                ts.Items.RemoveAt(1);
                                ts.Items.Insert(1, tsb);
                                break;
                            }
                            else
                                if (ts.Items[2] is ToolStripLabel)
                            {
                                ts.Items.RemoveAt(2);
                                ts.Items.Insert(2, tsb);
                                break;
                            }

                            ts.Items.Add(tsb);
                            break;
                    }
                }
                else
                    switch (ts.Items.Count)
                    {
                        case 0:
                            ts.Items.AddRange(new ToolStripItem[]{
                                GetLabel(tsb, true),
                                tsb
                            });
                            break;

                        case 2:
                            ts.Items.RemoveAt(0);
                            ts.Items.Insert(0, GetLabel(tsb, false));

                            ts.Items.AddRange(new ToolStripItem[]{
                                GetLabel(tsb, true),
                                tsb
                            });
                            break;

                        case 4:
                            if (ts.Items[2].Image == null)
                                ts.Items.RemoveAt(2);
                            else if (ts.Items[0].Image == null)
                                ts.Items.RemoveAt(0);
                            ts.Items.Add(tsb);
                            break;
                    }
            }
            else
                if (downToUpDirection)
            {
                switch (ts.Items.Count)
                {
                    case 0:
                        ts.Items.AddRange(new ToolStripItem[]{
                                GetLabel(tsb, true),
                                GetLabel(tsb, true),
                                GetLabel(tsb, false, 4),
                                tsb,
                            });
                        break;

                    case 4:
                        if (ts.Items[2].Image == null)
                        {
                            ts.Items.RemoveAt(2);
                            ts.Items.Add(tsb);
                            break;
                        }
                        else if (ts.Items[1].Image == null)
                        {
                            ts.Items.RemoveAt(0);
                            ts.Items.Insert(0, GetLabel(tsb, false, 4));

                            ts.Items.RemoveAt(1);
                            ts.Items.Insert(1, tsb);
                            break;
                        }
                        else
                            if (ts.Items[0].Image == null)
                        {
                            ts.Items.RemoveAt(0);
                            ts.Items.Insert(0, tsb);
                            break;
                        }

                        ts.Items.Add(tsb);
                        break;
                }
            }
            else
                switch (ts.Items.Count)
                {
                    case 0:
                        ts.Items.AddRange(new ToolStripItem[]{
                                GetLabel(tsb, false, 4),
                                tsb
                            });
                        break;

                    case 2:
                        if (ts.Items[0] is ToolStripLabel)
                            ts.Items.RemoveAt(0);
                        else
                            ts.Items.Insert(0, GetLabel(tsb, false, 4));

                        ts.Items.Add(tsb);
                        break;

                    case 4:
                        if (ts.Items[0] is ToolStripLabel)
                            ts.Items.RemoveAt(0);

                        ts.Items.Add(tsb);
                        break;
                }

        }

        ToolStripLabel GetLabel(ToolStripButton tsb, bool buttonWidth)
        {
            return GetLabel(tsb, buttonWidth, 0);
        }
        ToolStripLabel GetLabel(ToolStripButton tsb, bool buttonWidth, int extraSize)
        {
            ToolStripLabel tsl = new ToolStripLabel();
            tsl.AutoSize = false;
            tsl.Height = tsb.Height;
            tsl.Width = (buttonWidth) ? tsb.Width : (10 + extraSize);
            tsl.Margin = new Padding(1, 0, 0, 0);
            return tsl;
        }

        void playerToolStripButton_Click(object sender, EventArgs e)
        {
            Player p = (sender as ToolStripItem).Tag as Player;
            if (p == null) return;

            displayHeroBuildOrder(p);
        }
        void playerToolStripButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                playerContextMenuStrip.Tag = sender;
                playerContextMenuStrip.Show(MousePosition);
            }
        }

        private void bringHeroIconToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Player p = (playerContextMenuStrip.Tag as ToolStripItem).Tag as Player;
            if (p == null) return;

            foreach (ToolStrip ts in mapPanel.Controls)
            {
                foreach (ToolStripItem tsi in ts.Items)
                    if (tsi.Tag == p)
                    {
                        ts.BringToFront();
                        return;
                    }
            }
        }

        private void copyPlayerNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Player p = (playerContextMenuStrip.Tag as ToolStripItem).Tag as Player;
            if (p == null) return;

            Clipboard.SetText(p.Name);
        }

        private void copyHeroNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Player p = (playerContextMenuStrip.Tag as ToolStripItem).Tag as Player;
            if (p == null) return;

            Clipboard.SetText(DHFormatter.ToString(currentReplay.MapCache.hpcUnitProfiles[p.GetMostUsedHero().Name, "Name"]));
        }

        void openBuildsInFormLL_MouseDown(object sender, MouseEventArgs e)
        {
            openBuildsInForms = !openBuildsInForms;
            openBuildsInFormLL.Text = "Open build orders in new window: " + (openBuildsInForms ? "On" : "Off");
        }
        void displayHeroBuildOrder(Player p)
        {
            if (!openBuildsInForms)
            {
                foreach (TabPage page in replayTabControl.TabPages)
                    if (page.Tag == p)
                    {
                        replayTabControl.SelectedTab = page;
                        return;
                    }

                replayTabControl.SuspendLayout();

                TabPage tp = new TabPage();
                HeroBuildView hbv = new HeroBuildView(p);
                tp.Controls.Add(hbv);
                tp.Text = p.Name;
                tp.Tag = p;
                tp.ToolTipText = p.Name + "'s Hero Build Order (Righ-Click to close)";

                hbv.Dock = DockStyle.Fill;

                replayTabControl.TabPages.Add(tp);
                replayTabControl.SelectedTab = tp;

                replayTabControl.ResumeLayout(false);
            }
            else
            {
                Form buildForm = new Form();
                buildForm.StartPosition = FormStartPosition.CenterScreen;

                buildForm.SuspendLayout();

                HeroBuildView hbv = new HeroBuildView(p);

                buildForm.Controls.Add(hbv);
                buildForm.Text = p.Name + " - " + hbv.heroNameLabel.Text;
                buildForm.Tag = p;

                hbv.Dock = DockStyle.Fill;

                buildForm.Icon = Icon.FromHandle(((Bitmap)hbv.heroImagePanel.BackgroundImage).GetHicon());
                buildForm.ClientSize = hbv.PreferredSize;

                buildForm.ResumeLayout(false);

                buildForm.Show();
            }
        }

        void replayTabControl_MouseUp(object sender, MouseEventArgs e)
        {
            // check if the right mouse button was pressed
            if (e.Button == MouseButtons.Right)
            {
                // iterate through all the tab pages
                for (int i = 0; i < replayTabControl.TabCount; i++)
                {
                    // get their rectangle area and check if it contains the mouse cursor
                    Rectangle r = replayTabControl.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        if (replayTabControl.TabPages[i].Tag is Player)
                            replayTabControl.TabPages.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        protected void DisplayDescription(Replay rp)
        {
            // replay version
            infoLV.Items[0].SubItems[1].Text = "1." + rp.Version;
            // map name
            infoLV.Items[1].SubItems[1].Text = Path.GetFileName(rp.Map.Path);
            // map location
            infoLV.Items[2].SubItems[1].Text = Path.GetDirectoryName(rp.Map.Path);
            // host name
            infoLV.Items[3].SubItems[1].Text = rp.HostName;
            // saved by
            infoLV.Items[4].SubItems[1].Text = rp.SaverName;
            // mode
            infoLV.Items[5].SubItems[1].Text = rp.GameMode;
            // game length
            infoLV.Items[6].SubItems[1].Text = DHFormatter.ToString(rp.GameLength);
            // sentinel players
            infoLV.Items[7].SubItems[1].Text = rp.GetTeamByType(TeamType.Sentinel).Players.Count.ToString();
            // scourge players
            infoLV.Items[8].SubItems[1].Text = rp.GetTeamByType(TeamType.Scourge).Players.Count.ToString();
            // winner
            string winner = "Winner info not found";
            infoLV.Items[9].SubItems[1].ForeColor = Color.Black;
            foreach (Team t in rp.Teams)
                if (t.IsWinner)
                {
                    winner = t.Type.ToString();
                    infoLV.Items[9].SubItems[1].ForeColor = (t.Type == TeamType.Sentinel) ? Color.Red : Color.Green;
                    break;
                }
            infoLV.Items[9].SubItems[1].Text = winner;
        }

        protected void DisplayBansAndPicks(Replay rp)
        {
            picksPanel.Visible = rp.Bans.Count != 0;

            if (rp.Bans.Count == 0)
            {
                bansTS.Items.Clear();
                picksTS.Items.Clear();
                return;
            }

            bansTS.SuspendLayout();
            bansTS.Items.Clear();

            picksTS.SuspendLayout();
            picksTS.Items.Clear();

            foreach (PickInfo ban in rp.Bans)
                bansTS.Items.Add(getPickItem(ban));

            PickInfo lastPick = default(PickInfo);
            foreach (PickInfo pick in rp.Picks)
            {
                if (lastPick.Hero != null && lastPick.TeamType != pick.TeamType)
                    picksTS.Items.Add(getPickSeparator());

                lastPick = pick;

                picksTS.Items.Add(getPickItem(pick));
            }

            bansTS.ResumeLayout();
            picksTS.ResumeLayout();

        }
        ToolStripButton getPickItem(PickInfo pick)
        {
            ToolStripButton tsb = new ToolStripButton();

            string imagePath = currentReplay.MapCache.hpcUnitProfiles[pick.Hero.Name, "Art"] as string;

            tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsb.Image = (imagePath != null) ? currentReplay.MapCache.Resources.GetImage(imagePath) : Properties.Resources.armor;
            tsb.ToolTipText = DHFormatter.ToString(currentReplay.MapCache.hpcUnitProfiles[pick.Hero.Name, "Name"]);
            tsb.Margin = new System.Windows.Forms.Padding(0);
            tsb.Padding = new Padding(1, 1, 0, 0);
            tsb.Size = new System.Drawing.Size(25, 25);
            tsb.BackColor = pick.TeamType == TeamType.Sentinel ? Color.OrangeRed : Color.FromArgb(43, 172, 43);
            tsb.Tag = pick.Hero;

            tsb.ImageAlign = ContentAlignment.MiddleLeft;

            return tsb;
        }
        ToolStripButton getPickSeparator()
        {
            ToolStripButton tsb = new ToolStripButton();

            tsb.AutoSize = false;
            tsb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            tsb.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsb.Size = new System.Drawing.Size(4, 4);

            return tsb;
        }

        protected void DisplayChat(Replay replay)
        {
            chatlogRTB.Clear();
            UIRichTextEx.Default.ClearText();

            bool isPaused = false;
            foreach (ChatInfo ci in replay.Chats)
                if (ci.To != TalkTo.System)
                {
                    switch (ci.To)
                    {
                        case TalkTo.Allies:
                        case TalkTo.All:
                        case TalkTo.Observers:
                            UIRichTextEx.Default.AddText(DHFormatter.ToString(ci.Time) + " ", UIFonts.boldArial8, isPaused ? Color.Silver : UIColors.toolTipData);
                            UIRichTextEx.Default.AddText("[" + ci.To.ToString() + "] ", Color.White);
                            break;
                        default:
                            continue;
                    }

                    UIRichTextEx.Default.AddText(ci.From.Name + ": ", playerColorToColorChat(ci.From.Color));
                    UIRichTextEx.Default.AddText(ci.Message.Replace("}", ""), Color.White);
                    UIRichTextEx.Default.AddNewLine();
                }
                else
                {
                    switch (ci.Message)
                    {
                        case "pause":
                            if (!isPaused)
                            {
                                isPaused = true;
                                UIRichTextEx.Default.AddText(DHFormatter.ToString(ci.Time) + " ", UIFonts.boldArial8, Color.White);
                                UIRichTextEx.Default.AddText(ci.From.Name + " ", playerColorToColorChat(ci.From.Color));
                                UIRichTextEx.Default.AddText("paused the game.", Color.White);
                                UIRichTextEx.Default.AddNewLine();
                            }
                            break;

                        case "resume":
                            if (isPaused)
                            {
                                isPaused = false;
                                UIRichTextEx.Default.AddText(DHFormatter.ToString(ci.Time) + " ", UIFonts.boldArial8, Color.White);
                                UIRichTextEx.Default.AddText(ci.From.Name + " ", playerColorToColorChat(ci.From.Color));
                                UIRichTextEx.Default.AddText("has resumed the game.", Color.White);
                                UIRichTextEx.Default.AddNewLine();
                            }
                            break;

                        case "save":
                            UIRichTextEx.Default.AddText(DHFormatter.ToString(ci.Time) + " ", UIFonts.boldArial8, Color.LightGreen);
                            UIRichTextEx.Default.AddText("Game was saved by ", Color.White);
                            UIRichTextEx.Default.AddText(ci.From.Name, playerColorToColorChat(ci.From.Color));
                            UIRichTextEx.Default.AddNewLine();
                            break;

                        case "leave":
                            UIRichTextEx.Default.AddText(DHFormatter.ToString(ci.Time) + " ", UIFonts.boldArial8, Color.White);
                            UIRichTextEx.Default.AddText(ci.From.Name + " ", playerColorToColorChat(ci.From.Color));
                            UIRichTextEx.Default.AddText("has left the game.", Color.White);
                            UIRichTextEx.Default.AddNewLine();
                            break;
                    }
                }

            chatlogRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        protected void DisplayStatistics(Replay rp)
        {
            statisticsLV.Items.Clear();
            statisticsLV.BeginUpdate();

            foreach (Player p in rp.Players)
                if (!p.IsComputer && !p.IsObserver)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = (p.SlotNo + 1) + "";
                    lvi.BackColor = playerColorToColorBG(p.Color);
                    lvi.UseItemStyleForSubItems = false;
                    lvi.Tag = p;

                    Color bgColor = p.TeamType == TeamType.Sentinel ? Color.FromArgb(255, 250, 240) : Color.FromArgb(245, 255, 240);

                    ListViewItem.ListViewSubItem lvi_Player = new ListViewItem.ListViewSubItem();
                    lvi_Player.Text = p.Name;
                    lvi_Player.BackColor = bgColor;

                    ListViewItem.ListViewSubItem lvi_Hero = new ListViewItem.ListViewSubItem();
                    if (p.Heroes.Count != 0)
                        lvi_Hero.Text = DHFormatter.ToString(p.GetMostUsedHeroClass());
                    lvi_Hero.BackColor = bgColor;

                    ListViewItem.ListViewSubItem lvi_APM = new ListViewItem.ListViewSubItem();
                    lvi_APM.Text = "" + (int)p.Apm;
                    lvi_APM.BackColor = bgColor;

                    ListViewItem.ListViewSubItem lvi_Kills = new ListViewItem.ListViewSubItem();
                    lvi_Kills.Text = p.getGCVStringValue("kills", p.getGCVStringValue("1", ""));
                    if (lvi_Kills.Text.Length != 0)
                        lvi_Kills.Text += " / " + p.getGCVStringValue("deaths", p.getGCVStringValue("2", "")) + " / " + p.getGCVStringValue("5", "");
                    lvi_Kills.BackColor = bgColor;

                    ListViewItem.ListViewSubItem lvi_creepKD = new ListViewItem.ListViewSubItem();
                    lvi_creepKD.Text = p.getGCVStringValue("creeps", p.getGCVStringValue("3", ""));
                    if (lvi_creepKD.Text.Length != 0)
                        lvi_creepKD.Text += " / " + p.getGCVStringValue("denies", p.getGCVStringValue("4", ""))
                            + (p.gameCacheValues.ContainsKey("7") ? (" / " + p.getGCVStringValue("7", "")) : "");
                    lvi_creepKD.BackColor = bgColor;

                    ListViewItem.ListViewSubItem lvi_wards = new ListViewItem.ListViewSubItem();

                    int[] ward_stats = getWardStatistics(p);

                    if (ward_stats[0] != 0)
                        lvi_wards.Text = twoChars(ward_stats[0]) + " { " + ward_stats[1] + " + " + ward_stats[2] + " }";

                    lvi_wards.Tag = ward_stats;
                    lvi_wards.BackColor = bgColor;

                    lvi.SubItems.AddRange(new ListViewItem.ListViewSubItem[]{
                        lvi_Player,
                        lvi_Hero,
                        lvi_APM,
                        lvi_Kills,
                        lvi_creepKD,
                        lvi_wards
                    });

                    statisticsLV.Items.Add(lvi);
                }

            statisticsLV.EndUpdate();
        }
        protected int[] getWardStatistics(Player p)
        {
            if (Current.map == null && currentReplay.MapCache.IsLoaded == false) return new int[] { 0 };
            string result = string.Empty;

            int total = 0;
            int observer = 0;
            int sentry = 0;

            // get proper observer ward codeID
            string obsCodeID;
            if (currentReplay.MapCache.hpcItemProfiles.GetStringValue("sor7", "Name").Contains("Observer"))
                obsCodeID = "sor7";
            else
                obsCodeID = "I05G"; // new version for inventory

            // get proper sentry ward codeID
            string sentryCodeID;
            if (currentReplay.MapCache.hpcItemProfiles.GetStringValue("tgrh", "Name").Contains("Sentry"))
                sentryCodeID = "tgrh";
            else
                sentryCodeID = "I05H"; // new version for inventory

            int observerUses = DHFormatter.ToInt(currentReplay.MapCache.hpcItemData[obsCodeID, "uses"]);
            int sentryUses = DHFormatter.ToInt(currentReplay.MapCache.hpcItemData[sentryCodeID, "uses"]);

            foreach (OrderItem item in p.Items.BuildOrders)
            {
                switch (item.Name)
                {
                    // observer ward
                    case "sor7":
                    case "h02C": // new version obs ward for shop
                        total += observerUses;
                        observer += observerUses;
                        break;

                    case "tgrh":
                    case "h02D": // new version sentry ward for shop
                        total += sentryUses;
                        sentry += sentryUses;
                        break;
                }
            }

            return new int[3] { total, observer, sentry };
        }
        protected string twoChars(int value)
        {
            string result = value + "";
            if (result.Length == 1) return " " + result;
            else
                return result;
        }
        void statisticsLV_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Set the ListViewItemSorter property to a new ListViewItemComparer 
            // object. Setting this property immediately sorts the 
            // ListView using the ListViewItemComparer object.
            this.statisticsLV.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }
        class ListViewItemComparer : IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                switch (col)
                {
                    case 0:
                        return CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)x).Tag as Player).SlotNo, (((ListViewItem)y).Tag as Player).SlotNo);
                    case 1:
                    case 2:
                        return CaseInsensitiveComparer.DefaultInvariant.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                    case 3:
                        return CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)y).Tag as Player).Apm, (((ListViewItem)x).Tag as Player).Apm);
                    case 4:
                        return CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)y).Tag as Player).getGCValue("1"), (((ListViewItem)x).Tag as Player).getGCValue("1"));
                    case 5:
                        return CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)y).Tag as Player).getGCValue("2"), (((ListViewItem)x).Tag as Player).getGCValue("2"));
                    case 6:
                        int result = CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)y).Tag as Player).getGCValue("3"), (((ListViewItem)x).Tag as Player).getGCValue("3"));
                        if (result == 0)
                            return CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)y).Tag as Player).getGCValue("4"), (((ListViewItem)x).Tag as Player).getGCValue("4"));
                        else
                            return result;
                    case 7:
                        return CaseInsensitiveComparer.DefaultInvariant.Compare((((ListViewItem)y).SubItems[7].Tag as int[])[0], (((ListViewItem)x).SubItems[7].Tag as int[])[0]);
                    default:
                        return 0;
                }
            }
        }

        protected void DisplayKillLog(Replay rp)
        {
            TimeSpan lastKillTime = TimeSpan.MinValue;

            killLogRTB.Clear();
            UIRichTextEx.Default.ClearText();

            if (rp.Kills.Count == 0)
                UIRichTextEx.Default.AddText("Not available for this replay", Color.White);
            else
                foreach (KillInfo ki in rp.Kills)
                {
                    UIRichTextEx.Default.AddText(DHFormatter.ToString(ki.Time) + "  ", UIFonts.boldArial8, (ki.Time.TotalSeconds - lastKillTime.TotalSeconds < 8) ? Color.Cyan : Color.LightSkyBlue);

                    if (ki.Killer != null)
                    {
                        UIRichTextEx.Default.AddText(ki.Killer.Name, playerColorToColorChat(ki.Killer.Color));
                        UIRichTextEx.Default.AddText(" (" + ki.Killer.GetMostUsedHeroClass() + ")", Color.White);
                    }
                    else
                        UIRichTextEx.Default.AddText("Creeps", Color.White);

                    if (ki.Victim != null)
                    {
                        UIRichTextEx.Default.AddText("  killed  ", Color.White);
                        UIRichTextEx.Default.AddText(ki.Victim.Name, playerColorToColorChat(ki.Victim.Color));
                        UIRichTextEx.Default.AddText(" (" + ki.Victim.GetMostUsedHeroClass() + ")", Color.White);
                    }
                    else
                    {
                        UIRichTextEx.Default.AddText((ki.Killer != null && ki.Killer.TeamType == ki.VictimInfo.TeamType) ? "  denied  " : "  destroyed  ", Color.White);
                        UIRichTextEx.Default.AddText(ki.VictimInfo.Description, ki.VictimInfo.TeamType == TeamType.Sentinel ? sentinelLabel.ForeColor : scourgeLabel.ForeColor);
                    }

                    UIRichTextEx.Default.AddNewLine();

                    lastKillTime = ki.Time;
                }

            killLogRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        void InitPlugins()
        {
            object[] parameters = new object[2] { currentReplay, hpcExportCfg };

            HabProperties hpsPlugins;
            if (Current.plugins.TryGetValue("ReplayParser", out hpsPlugins))
                foreach (Plugins.IDotaHITPlugin plugin in hpsPlugins.Values)
                    plugin.Tag = parameters;
        }

        private void PrepareReplayExport()
        {
            List<string> files = new List<string>(Directory.GetFiles(Application.StartupPath, "dhrexport*.cfg"));

            // add default export configuration
            if (!files.Contains(ReplayExportCfgFileName))
                files.Insert(0, ReplayExportCfgFileName);

            object selectedItem = configurationCmbB.SelectedItem;

            configurationCmbB.SuspendLayout();
            configurationCmbB.Items.Clear();
            configurationCmbB.Items.AddRange(files.ToArray());
            configurationCmbB.ResumeLayout();

            configurationCmbB.SelectedItem = selectedItem != null ? selectedItem : ReplayExportCfgFileName;
        }
        private void LoadReplayExportConfig(string filename)
        {
            hpcExportCfg = new HabPropertiesCollection();
            hpcExportCfg.ReadFromFile(filename);

            HabProperties hpsExportUI;
            if (!hpcExportCfg.TryGetValue("UI", out hpsExportUI))
            {
                hpsExportUI = new HabProperties();
                hpcExportCfg.Add("UI", hpsExportUI);
            }

            originalExport = string.Empty;

            exportPreviewB.Checked = false;

            int layout = hpsExportUI.GetIntValue("Layout", -1);

            if (currentExportLayout == layout)
                ShowReplayExport(layout);
            else
                layoutCmbB.SelectedIndex = layout;

            replayExportRTB.Font = new Font(
                hpsExportUI.GetStringValue("FontFamily", "Verdana"),
                (float)hpsExportUI.GetDoubleValue("FontSize", 10));

            fontTextBox.Text = replayExportRTB.Font.FontFamily.Name + " " + replayExportRTB.Font.Size;

            shortLaneNamesCB.Checked = hpsExportUI.GetIntValue("ShortLaneNames", 0) == 1;
            exportIconWidthNumUD.Value = (decimal)hpsExportUI.GetIntValue("ImageWidth", 18);

            heroFormatCmbB.SuspendLayout();
            heroFormatCmbB.Items.Clear();

            List<String> formats = hpsExportUI.GetStringListValue("HeroFormats");
            heroFormatCmbB.Items.AddRange(formats.ToArray());

            // add default format strings to the end of list
            heroFormatCmbB.Items.AddRange(defaultHeroFormatList);

            heroFormatCmbB.SelectedIndex = hpsExportUI.GetIntValue("HeroFormatIndex", 0);

            bbCodeCB.Checked = hpsExportUI.GetIntValue("UseBBCodes", 0) == 1;
        }
        private void SaveReplayExportConfig(string filename)
        {
            if (hpcExportCfg != null && hpcExportCfg.Count != 0)
            {
                hpcExportCfg["UI"]["FontFamily"] = replayExportRTB.Font.FontFamily.Name;
                hpcExportCfg["UI"]["FontSize"] = (double)replayExportRTB.Font.Size;
                hpcExportCfg["UI"]["ShortLaneNames"] = shortLaneNamesCB.Checked ? 1 : 0;
                hpcExportCfg["UI"]["ImageWidth"] = (int)exportIconWidthNumUD.Value;

                int heroFormatStringCount = heroFormatCmbB.Items.Count - defaultHeroFormatList.Length;
                List<string> heroFormatStringList = new List<string>(heroFormatStringCount);
                for (int i = 0; i < (heroFormatCmbB.Items.Count - defaultHeroFormatList.Length); i++)
                    heroFormatStringList.Add(heroFormatCmbB.Items[i] as string);

                hpcExportCfg["UI"]["HeroFormats"] = heroFormatStringList;
                hpcExportCfg["UI"]["HeroFormatIndex"] = heroFormatCmbB.SelectedIndex;

                hpcExportCfg["UI"]["UseBBCodes"] = bbCodeCB.Checked ? 1 : 0;

                DHFormatter.RefreshFormatForCurrentThread();

                hpcExportCfg.SaveToFile(filename);
            }
        }

        private void configurationCmbB_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = Path.GetFileNameWithoutExtension(e.ListItem as string);
        }

        private void configurationCmbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            // save changes made to previously selected configuration
            if (currentExportCfgFileName != null)
                SaveReplayExportConfig(currentExportCfgFileName);

            currentExportCfgFileName = configurationCmbB.SelectedItem as string;

            int layout = currentExportLayout;

            LoadReplayExportConfig(currentExportCfgFileName);
            RefreshExportPreview();

            layoutCmbB.SelectedIndex = layout;
        }

        private void configurationInfoLL_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This allows you to select replay export configuration file to be used for replay export." +
                "\nEach configuration file holds its own set of hero tags, classes, names, format strings and UI settings." +
                "\nYou can't create new configuration file via DotaHIT interface though. You have to manually create a copy" +
                "\nof 'dhrexport.cfg' file, located in DotaHIT folder, and then rename it to anything that begins with 'dhrexport'." +
                "\nExample: 'dhrexport_playdota.cfg'" +
                "\nChanges made in the replay export user interface will be saved automatically to the cfg file specified once you" +
                "\nselect different configuration file or close Replay Parser window.");
        }

        private void layoutCmbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowReplayExport(layoutCmbB.SelectedIndex);

            if (exportPreviewB.Checked)
                exportPreviewB_CheckedChanged(null, EventArgs.Empty);
        }

        void ShowReplayExport(int layoutType)
        {
            currentExportLayout = layoutType;

            Team t1;
            Team t2;

            dcUsedHeroTags = new Dictionary<string, string>();

            UIRichText.Default.ClearText();
            UIRichText.Default.buffer.Font = replayExportRTB.Font;
            UIRichText.Default.buffer.SelectionFont = replayExportRTB.Font;

            if (layoutType != -1 && currentReplay.Bans.Count != 0)
            {
                appendBansToExport(bbCodeCB.Checked); UIRichText.Default.AddText("\n");
            }

            if (layoutType != -1 && currentReplay.Picks.Count != 0)
            {
                appendPicksToExport(bbCodeCB.Checked); UIRichText.Default.AddText("\n");
            }

            switch (layoutType)
            {
                case 0:
                    if (bbCodeCB.Checked) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Sentinel", 0));
                    UIRichText.Default.AddText("The Sentinel");
                    if (bbCodeCB.Checked) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Sentinel", 1));
                    UIRichText.Default.AddText("\n");

                    t1 = currentReplay.GetTeamByType(TeamType.Sentinel);
                    appendLineUpToExport(t1, LineUp.Top, true); UIRichText.Default.AddText("\n");
                    appendLineUpToExport(t1, LineUp.Middle, true); UIRichText.Default.AddText("\n");
                    appendLineUpToExport(t1, LineUp.Bottom, true); UIRichText.Default.AddText("\n");
                    appendLineUpToExport(t1, LineUp.JungleOrRoaming, true); UIRichText.Default.AddText("\n");

                    if (bbCodeCB.Checked) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Scourge", 0));
                    UIRichText.Default.AddText("The Scourge");
                    if (bbCodeCB.Checked) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Scourge", 1));
                    UIRichText.Default.AddText("\n");

                    t2 = currentReplay.GetTeamByType(TeamType.Scourge);
                    appendLineUpToExport(t2, LineUp.Top, true); UIRichText.Default.AddText("\n");
                    appendLineUpToExport(t2, LineUp.Middle, true); UIRichText.Default.AddText("\n");
                    appendLineUpToExport(t2, LineUp.Bottom, true); UIRichText.Default.AddText("\n");
                    appendLineUpToExport(t2, LineUp.JungleOrRoaming, true); UIRichText.Default.AddText("\n");
                    break;

                case 1:
                    int lineCount = UIRichText.Default.buffer.Lines.Length - 1;
                    t1 = currentReplay.GetTeamByType(TeamType.Sentinel);

                    UIRichText.Default.AddText(getLaneName(LineUp.Top) + ":  "); appendLineUpToExport(t1, LineUp.Top, false); UIRichText.Default.AddText("\n");
                    UIRichText.Default.AddText(getLaneName(LineUp.Middle) + ":  "); appendLineUpToExport(t1, LineUp.Middle, false); UIRichText.Default.AddText("\n");
                    UIRichText.Default.AddText(getLaneName(LineUp.Bottom) + ":  "); appendLineUpToExport(t1, LineUp.Bottom, false); UIRichText.Default.AddText("\n");
                    UIRichText.Default.AddText(getLaneName(LineUp.JungleOrRoaming) + ":  "); appendLineUpToExport(t1, LineUp.JungleOrRoaming, false);

                    rightAlignRtfLines(UIRichText.Default.buffer);

                    t2 = currentReplay.GetTeamByType(TeamType.Scourge);

                    string[] lines = UIRichText.Default.buffer.Lines;
                    lines[lineCount] += "   vs   " + getHorzLineUp(t2, LineUp.Top);
                    lines[lineCount + 1] += "   vs   " + getHorzLineUp(t2, LineUp.Middle);
                    lines[lineCount + 2] += "   vs   " + getHorzLineUp(t2, LineUp.Bottom);
                    lines[lineCount + 3] += "        " + getHorzLineUp(t2, LineUp.JungleOrRoaming);

                    UIRichText.Default.buffer.Lines = lines;
                    break;

                case -1:
                    UIRichText.Default.AddText("Select layout first");
                    break;
            }

            UIRichText.Default.buffer.SelectAll();
            UIRichText.Default.buffer.SelectionFont = replayExportRTB.Font;
            replayExportRTB.Rtf = UIRichText.Default.buffer.Rtf;

            originalExport = replayExportRTB.Rtf;

            UIRichText.Default.ClearText();
        }
        string getHeroTag(string heroID)
        {
            string tag = hpcExportCfg["HeroTags", heroID] as string;

            return (string.IsNullOrEmpty(tag)) ? ":" + heroID + ":" : tag;
        }
        string getHeroClass(string heroID)
        {
            string heroClass = hpcExportCfg["HeroClasses", heroID] as string;

            return (string.IsNullOrEmpty(heroClass)) ? currentReplay.MapCache.hpcUnitProfiles.GetStringValue(heroID, "Name").Trim('\"') : heroClass;
        }
        string getHeroName(string heroID)
        {
            string heroName = hpcExportCfg["HeroNames", heroID] as string;

            return (string.IsNullOrEmpty(heroName)) ? currentReplay.MapCache.hpcUnitProfiles[heroID].GetStringListValue("Propernames")[0] : heroName;
        }
        string getPlayerName(Player p, bool bbCode)
        {
            if (bbCode)
            {
                string bbCodeItem = "Player" + ((int)p.Color - (int)p.Color / (int)PlayerColor.Green);
                return hpcExportCfg.GetStringListItemValue("bbCode", bbCodeItem, 0) + p.Name + hpcExportCfg.GetStringListItemValue("bbCode", bbCodeItem, 1);
            }
            else
                return p.Name;
        }
        string getLaneName(LineUp lane)
        {
            switch (lane)
            {
                case LineUp.Top:
                    return lane.ToString();

                case LineUp.Middle:
                    return shortLaneNamesCB.Checked ? "Mid" : lane.ToString();

                case LineUp.Bottom:
                    return shortLaneNamesCB.Checked ? "Bot" : lane.ToString();

                case LineUp.JungleOrRoaming:
                    return shortLaneNamesCB.Checked ? "Jungle" : "Jungle/Roaming";

                default:
                    return string.Empty;
            }
        }
        void rightAlignRtfLines(RichTextBox rtb)
        {
            int maxLine = -1;
            float maxWidth = 0;
            Graphics g = this.CreateGraphics();
            for (int i = 0; i < rtb.Lines.Length; i++)
            {
                float width = (float)UIRichText.MeasureString(g, rtb.Lines[i], rtb.Font).Width;
                width += getExtraWidthForImages(g, rtb.Font, rtb.Lines[i]);

                if (maxWidth < width)
                {
                    maxWidth = width;
                    maxLine = i;
                }
            }
            float wsWidth = (float)UIRichText.MeasureString(g, " ", rtb.Font).Width;
            for (int i = 0; i < rtb.Lines.Length; i++)
            {
                float width = (float)UIRichText.MeasureString(g, rtb.Lines[i], rtb.Font).Width;
                width += getExtraWidthForImages(g, rtb.Font, rtb.Lines[i]);

                //Console.WriteLine("line: "+i+ "="+ width);

                if (width < maxWidth)
                {
                    float countF = (maxWidth - width) / wsWidth;
                    int count = (int)((maxWidth - width) / wsWidth);
                    string newLine = rtb.Lines[i].PadRight(rtb.Lines[i].Length + count, ' ');
                    string[] lines = rtb.Lines;
                    lines[i] = newLine;
                    rtb.Lines = lines;
                }
            }
        }
        float getExtraWidthForImages(Graphics g, Font font, string line)
        {
            float width = 0;
            float imageWidth = (float)exportIconWidthNumUD.Value;

            foreach (string tag in dcUsedHeroTags.Keys)
            {
                int index;
                while ((index = line.IndexOf(tag)) != -1)
                {
                    width += imageWidth - (float)UIRichText.MeasureString(g, tag, font).Width;
                    line = line.Remove(index, tag.Length);
                }
            }

            return width;
        }
        void appendBansToExport(bool bbCode)
        {
            int enumType = hpcExportCfg.GetIntValue("BanPick", "BanEnumType", 0);
            int showFirst = hpcExportCfg.GetIntValue("BanPick", "FirstBanner", 1);

            switch (enumType)
            {
                case 0:
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Bans", 0));

                    string bansTitle = "Bans: " + ((showFirst == 1) ? "(" + currentReplay.Bans[0].TeamType.ToString() + " First) " : "");
                    UIRichText.Default.AddText(bansTitle);

                    for (int i = 0; i < currentReplay.Bans.Count; i++)
                    {
                        PickInfo ban = currentReplay.Bans[i];

                        if (i > 0) UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "BanSeparator", ", "));

                        UIRichText.Default.AddText(getFormattedHeroString(ban.Hero, heroFormatCmbB.Text, bbCodeCB.Checked));
                    }
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Bans", 1));
                    break;

                case 1:
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Bans", 0));

                    bansTitle = "Bans: " + ((showFirst == 1) ? "(" + currentReplay.Picks[0].TeamType.ToString() + " First) " : "");
                    UIRichText.Default.AddText(bansTitle);

                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Bans", 1));
                    UIRichText.Default.AddText("\n\n");

                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Sentinel", 0));
                    UIRichText.Default.AddText("Sentinel: ");
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Sentinel", 1));
                    bool firstItem = true;
                    for (int i = 0; i < currentReplay.Bans.Count; i++)
                    {
                        PickInfo ban = currentReplay.Bans[i];

                        if (ban.TeamType != TeamType.Sentinel)
                            continue;

                        if (!firstItem)
                            UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "BanSeparator", ", "));
                        else
                            firstItem = false;

                        UIRichText.Default.AddText(getFormattedHeroString(ban.Hero, heroFormatCmbB.Text, bbCodeCB.Checked));
                    }

                    UIRichText.Default.AddText("\n");
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Scourge", 0));
                    UIRichText.Default.AddText("Scourge: ");
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Scourge", 1));
                    firstItem = true;
                    for (int i = 0; i < currentReplay.Bans.Count; i++)
                    {
                        PickInfo ban = currentReplay.Bans[i];

                        if (ban.TeamType != TeamType.Scourge)
                            continue;

                        if (!firstItem)
                            UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "BanSeparator", ", "));
                        else
                            firstItem = false;

                        UIRichText.Default.AddText(getFormattedHeroString(ban.Hero, heroFormatCmbB.Text, bbCodeCB.Checked));
                    }
                    UIRichText.Default.AddText("\n");
                    break;
            }

            UIRichText.Default.AddText("\n");
        }
        void appendPicksToExport(bool bbCode)
        {
            int enumType = hpcExportCfg.GetIntValue("BanPick", "PickEnumType", 0);
            int showFirst = hpcExportCfg.GetIntValue("BanPick", "FirstPicker", 1);

            switch (enumType)
            {
                case 0:
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Picks", 0));

                    string picksTitle = "Picks: " + ((showFirst == 1) ? "(" + currentReplay.Picks[0].TeamType.ToString() + " First) " : "");
                    UIRichText.Default.AddText(picksTitle);

                    PickInfo lastPick = default(PickInfo);
                    for (int i = 0; i < currentReplay.Picks.Count; i++)
                    {
                        PickInfo pick = currentReplay.Picks[i];

                        if (i > 0)
                        {
                            if (lastPick.Hero != null && lastPick.TeamType == pick.TeamType)
                                UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "PickPairSeparator", " + "));
                            else
                                UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "PickSeparator", ", "));
                        }

                        UIRichText.Default.AddText(getFormattedHeroString(pick.Hero, heroFormatCmbB.Text, bbCodeCB.Checked));

                        lastPick = pick;
                    }
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Picks", 1));
                    break;

                case 1:
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Picks", 0));

                    picksTitle = "Picks: " + ((showFirst == 1) ? "(" + currentReplay.Picks[0].TeamType.ToString() + " First) " : "");
                    UIRichText.Default.AddText(picksTitle);

                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Picks", 1));
                    UIRichText.Default.AddText("\n\n");

                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Sentinel", 0));
                    UIRichText.Default.AddText("Sentinel: ");
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Sentinel", 1));
                    bool firstItem = true;
                    lastPick = default(PickInfo);
                    for (int i = 0; i < currentReplay.Picks.Count; i++)
                    {
                        PickInfo pick = currentReplay.Picks[i];

                        if (pick.TeamType != TeamType.Sentinel)
                        {
                            lastPick = pick;
                            continue;
                        }

                        if (!firstItem)
                        {
                            if (lastPick.Hero != null && lastPick.TeamType == pick.TeamType)
                                UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "PickPairSeparator", " + "));
                            else
                                UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "PickSeparator", ", "));
                        }
                        else
                            firstItem = false;

                        UIRichText.Default.AddText(getFormattedHeroString(pick.Hero, heroFormatCmbB.Text, bbCodeCB.Checked));

                        lastPick = pick;
                    }

                    UIRichText.Default.AddText("\n");
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Scourge", 0));
                    UIRichText.Default.AddText("Scourge: ");
                    if (bbCode) UIRichText.Default.AddText(hpcExportCfg.GetStringListItemValue("bbCode", "Scourge", 1));
                    firstItem = true;
                    lastPick = default(PickInfo);
                    for (int i = 0; i < currentReplay.Picks.Count; i++)
                    {
                        PickInfo pick = currentReplay.Picks[i];

                        if (pick.TeamType != TeamType.Scourge)
                        {
                            lastPick = pick;
                            continue;
                        }

                        if (!firstItem)
                        {
                            if (lastPick.Hero != null && lastPick.TeamType == pick.TeamType)
                                UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "PickPairSeparator", " + "));
                            else
                                UIRichText.Default.AddText(hpcExportCfg.GetStringValue("BanPick", "PickSeparator", ", "));
                        }
                        else
                            firstItem = false;

                        UIRichText.Default.AddText(getFormattedHeroString(pick.Hero, heroFormatCmbB.Text, bbCodeCB.Checked));

                        lastPick = pick;
                    }
                    UIRichText.Default.AddText("\n");
                    break;
            }

            UIRichText.Default.AddText("\n");
        }
        void appendLineUpToExport(Team t, LineUp lineUp, bool vertical)
        {
            int count = 0;
            foreach (Player p in t.Players)
                if (p.LineUp == lineUp)
                {
                    count++;
                    if (count > 1)
                    {
                        if (vertical)
                            UIRichText.Default.AddText("\n");
                        else
                            UIRichText.Default.AddText(" + ");
                    }

                    Hero h = p.GetMostUsedHero();

                    UIRichText.Default.AddText(getFormattedHeroString(h, heroFormatCmbB.Text, bbCodeCB.Checked));
                    UIRichText.Default.AddText(" " + getPlayerName(p, bbCodeCB.Checked));
                    if (vertical) UIRichText.Default.AddText("(" + getLaneName(lineUp) + ")");
                }
        }
        string getFormattedHeroString(Hero h, string format, bool bbCode)
        {
            string output = format;

            if (output.Contains("%tag"))
            {
                string tag = getHeroTag(h.Name);
                dcUsedHeroTags[tag] = h.Name;

                output = output.Replace("%tag", tag);
            }

            output = output.Replace("%cl", getHeroClass(h.Name));
            output = output.Replace("%n", getHeroName(h.Name));

            if (bbCode && (format.Contains("%cl") || format.Contains("%n")))
                return hpcExportCfg.GetStringListItemValue("bbCode", "HeroString", 0) + output + hpcExportCfg.GetStringListItemValue("bbCode", "HeroString", 1);
            else
                return output;
        }
        string getHorzLineUp(Team t, LineUp lineUp)
        {
            string result = string.Empty;
            int count = 0;
            foreach (Player p in t.Players)
                if (p.LineUp == lineUp)
                {
                    count++;
                    if (count > 1) result += " + ";

                    Hero h = p.GetMostUsedHero();

                    result += getFormattedHeroString(h, heroFormatCmbB.Text, bbCodeCB.Checked);
                    result += " " + getPlayerName(p, bbCodeCB.Checked);
                }

            return result;
        }

        private void exportPreviewB_CheckedChanged(object sender, EventArgs e)
        {
            if (exportPreviewB.Checked)
            {
                UIRichText.Default.buffer.Font = replayExportRTB.Font;
                UIRichText.Default.buffer.Rtf = replayExportRTB.Rtf;

                foreach (string tag in dcUsedHeroTags.Keys)
                {
                    int index;
                    while ((index = UIRichText.Default.buffer.Text.IndexOf(tag)) != -1)
                    {
                        UIRichText.Default.buffer.Select(index, tag.Length);
                        Bitmap image = currentReplay.MapCache.Resources.GetImage(DHFormatter.ToString(currentReplay.MapCache.hpcUnitProfiles[dcUsedHeroTags[tag], "Art"]));
                        Bitmap thumbImage = new Bitmap(image, (int)exportIconWidthNumUD.Value, (int)exportIconWidthNumUD.Value);
                        UIRichText.Default.PasteImage(thumbImage);
                    }
                }

                replayExportRTB.Rtf = UIRichText.Default.buffer.Rtf;
                UIRichText.Default.ClearText();
            }
            else
                replayExportRTB.Rtf = originalExport;
        }

        void RefreshExportPreview()
        {
            ShowReplayExport(currentExportLayout);

            if (exportPreviewB.Checked)
                exportPreviewB_CheckedChanged(null, EventArgs.Empty);
        }

        private void shortLaneNamesCB_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExportPreview();
        }

        private void bansPicksB_Click(object sender, EventArgs e)
        {
            BansPicksForm bp = new BansPicksForm();
            bp.StartPosition = FormStartPosition.CenterScreen;
            bp.ShowDialog(currentExportCfgFileName, hpcExportCfg);

            RefreshExportPreview();
        }

        private void heroTagB_Click(object sender, EventArgs e)
        {
            HeroTagListForm htl = new HeroTagListForm(currentReplay.MapCache);
            htl.StartPosition = FormStartPosition.CenterScreen;
            htl.ShowDialog(currentExportCfgFileName, hpcExportCfg);

            RefreshExportPreview();
        }

        private void bbCodeCB_CheckedChanged(object sender, EventArgs e)
        {
            RefreshExportPreview();
        }

        private void bbCodeB_Click(object sender, EventArgs e)
        {
            BBCodeSettingsForm bbCodeSettings = new BBCodeSettingsForm();
            bbCodeSettings.StartPosition = FormStartPosition.CenterScreen;
            bbCodeSettings.ShowDialog(bbCodeItems, currentExportCfgFileName, hpcExportCfg);

            if (bbCodeCB.Checked)
                RefreshExportPreview();
        }

        private void chooseFontB_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = replayExportRTB.Font;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                replayExportRTB.Font = fd.Font;
                fontTextBox.Text = fd.Font.FontFamily.Name + " " + fd.Font.Size;

                RefreshExportPreview();
            }
        }

        private void heroFormatCmbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            heroFormatRemoveLL.Enabled = heroFormatCmbB.SelectedIndex < heroFormatCmbB.Items.Count - defaultHeroFormatList.Length;
        }

        private void heroFormatCmbB_TextChanged(object sender, EventArgs e)
        {
            heroFormatAddLL.Enabled = !string.IsNullOrEmpty(heroFormatCmbB.Text) && !heroFormatCmbB.Items.Contains(heroFormatCmbB.Text);

            RefreshExportPreview();
        }

        private void heroFormatAddLL_Click(object sender, EventArgs e)
        {
            if (!heroFormatCmbB.Items.Contains(heroFormatCmbB.Text))
            {
                heroFormatCmbB.Items.Insert(0, heroFormatCmbB.Text);
                heroFormatAddLL.Enabled = false;

                heroFormatCmbB.SelectedIndex = 0;
            }
        }

        private void heroFormatRemoveLL_Click(object sender, EventArgs e)
        {
            heroFormatCmbB.Items.Remove(heroFormatCmbB.Text);
        }

        private void heroFormatInfoLL_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Use 'add'/'rem' to add/remove items (hero format strings) to the list. Default items can't be removed." +
                "\nContent of the list is saved in 'dhrexport.cfg' file and is auto-loaded each time you run Replay Parser." +
                "\nTo add an item, type it in combobox and click on 'add'. To remove an item, select item to be removed and click on 'rem'" +
                "\nThe hero format string consists of the following variables, which values are inserted from the 'Hero<->Tag' table: " +
                "\n\n %tag  - hero tag. Example: \":Edem:\", or whatever you enter in the 'Hero<->Tag' table" +
                "\n %cl - hero class. Example: \"Anti-Mage\"" +
                "\n %n - hero name. Example: \"Magina\"" +
                "\n\nYou can use the default format string or enter any other combination you like, for example, " +
                "\nentering \"%tag %n The %cl\" will result in \":Edem: Magina The Anti-Mage\" hero string to be used for Antimage");
        }

        private void viewRawDataB_Click(object sender, EventArgs e)
        {
            Replay_Parser.ReplayRawDataForm rawDataForm = new DotaHIT.Extras.Replay_Parser.ReplayRawDataForm();

            rawDataForm.Init(currentReplay);

            rawDataForm.StartPosition = FormStartPosition.CenterScreen;
            rawDataForm.Show();
        }
    }
}
