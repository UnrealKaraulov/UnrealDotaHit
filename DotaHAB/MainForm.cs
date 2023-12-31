using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Core;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using Microsoft.Win32;
using DotaHIT.Core.Resources.Media;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.MpqPath;
using DotaHIT.DatabaseModel.Format;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Extras;
using BitmapUtils;
using System.Drawing.Imaging;

namespace DotaHIT
{
    public partial class MainForm : Form
    {
        internal int mbX = -1;
        internal int mbY = -1;
        internal ToolTipForm toolTip = null;
        internal ItemToolTipForm itemToolTip = null;
        internal object lastToolTipValue = null;

        internal bool listsBound = true;

        internal Button dragTarget = null;
        internal Point dragPosition = Point.Empty;

        SplashScreen splashScreen;
        internal List<FloatingListForm> FloatingForms = new List<FloatingListForm>();
        internal List<ReplayBrowserForm> ReplayParserForms = new List<ReplayBrowserForm>();

        public delegate void PreMoveEvent(object sender, Point xy);
        public event PreMoveEvent PreMove = null;

        public HeroListForm hlForm;
        public ItemListForm ilForm;
        public ControlBarForm cbForm;

        public bool showHotKeys = false;
        public CustomKeysForm ckForm = null;
        public DataDumpForm ddForm = null;
        public ImageDumpForm idForm = null;

        public bool fastResearch = false;
        public bool isResearching = false;

        public bool isStrictResearch = true;
        public int researchRestriction = 0;

        private System.Windows.Forms.Timer wakeupTimer;
        private Thread wakeUpThread = null;

        public MainForm(string filename)
            : this(false)
        {
            this.Show();

            switch (Path.GetExtension(filename).ToLower())
            {
                case ".w3x":
                    DHMAIN.LoadMap(new MapLoadSettings(filename, this));
                    break;

                case ".dhb":
                    LoadHeroBuild(filename);
                    break;

                case ".w3g":
                    ReplayParserForms.Add(new ReplayBrowserForm(filename, true));
                    break;
            }
        }

        public MainForm(bool smoothWakeUp)
        {
            Current.mainForm = this;

            DHMAIN.War3MpqLoading += new MethodInvoker(DHMAIN_War3MpqLoading);
            DHMAIN.MapLoading += new MapLoadEventHandler(DHMAIN_MapLoading);
            DHMAIN.MapJassCompiled += new MethodInvoker(DHMAIN_MapJassCompiled);
            DHMAIN.MapLoaded += new MapLoadEventHandler(DHMAIN_MapLoaded);
            DHMAIN.MpqDatabaseClearing += new MethodInvoker(DHMAIN_MpqDatabaseClearing);

            this.SuspendLayout();

            hlForm = new HeroListForm();
            ilForm = new ItemListForm();
            cbForm = new ControlBarForm();

            splashScreen = new SplashScreen();
            splashScreen.Show();
            splashScreen.ShowText("Initializing...");

            DHTIMER.StartCount();
            Control.CheckForIllegalCrossThreadCalls = false;

            FloatingForms.AddRange(new FloatingListForm[] { hlForm, ilForm });

            splashScreen.ProgressAdd(30);

            InitializeComponent(); splashScreen.ProgressAdd(30);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);

            this.Icon = Properties.Resources.Icon;

            this.CenterToScreen();
            this.Top = this.Top + (hlForm.Height / 2);

            versionLabel.Text = "v" + DHRELEASE.CurrentVersion;

            GatherAbilitySlots(); splashScreen.ProgressAdd(10);

            heroInfoPanel.Visible = false;
            heroSkillsPanel.Visible = false;

            hlForm.SetParent(this);

            ilForm.SetParent(this);
            ilForm.ItemActivate += new ItemListForm.ItemActivateEvent(ilForm_ItemActivate);

            cbForm.SetParent(this, FloatingForms);

            splashScreen.ProgressAdd(29);

            splashScreen.Close();

            DisplayUnit();

            cbForm.ApplyConfig();

            int hlX = hlForm.Left;
            hlForm.Left = -this.Width; // hide it to avoid flickering

            int ilX = ilForm.Left;
            ilForm.Left = -this.Width;

            hlForm.Display();
            ilForm.Display();

            hlForm.Minimize(true);
            ilForm.Minimize(true);

            hlForm.Left = hlX;
            ilForm.Left = ilX;

            mapNameLabel.Text = "";
            statusTS.Renderer = UIRenderers.NoBorderRenderer;

            saveFileDialog.InitialDirectory = Application.StartupPath;

            ApplyConfig();

            this.themeToolStripMenuItem.Enabled = false;

            this.ResumeLayout(false);

            if (!File.Exists(DHCFG.FileName))
            {
                this.Show();

                List<string> fileTypes = new List<string>(new string[] { ".dhb" });
                DHCFG.Items["Settings"]["FileTypes"] = fileTypes;
                DHFILE.AssociateFileTypes(fileTypes);

                MessageBox.Show("Hi. Configuration file (dotahit.cfg) was not found so you're probably running this program for the first time." +
                    "\nTo start working you need to open a DotA map file (like 'Dota Allstars 6.43b.w3x') by using the 'DotA map'->'Open...' option." +
                    "\nAlternatively you can just drag a map file onto the main window and it will be opened automatically.", "Introduction");
            }

            Theme theme = (Theme)DHCFG.Items["UI"].GetIntValue("Theme", (int)Theme.None);
            if (IsThemeAvailableOnDisk(theme))
                SetUITheme(theme);

            if (smoothWakeUp)
            {
                wakeupTimer = new System.Windows.Forms.Timer();
                wakeupTimer.Interval = 100;
                wakeupTimer.Tick += new EventHandler(wakeupTimer_Tick);
                wakeupTimer.Start();
            }
            else
                wakeUpThreadProcedure();

            //DHRC.timeCounter = 0;
            GC.Collect();
            DHTIMER.EndCount();
            DHTIMER.PrintCount("Initialization");
            DHTIMER.ResetCount();
        }

        void wakeupTimer_Tick(object sender, EventArgs e)
        {
            wakeupTimer.Stop();
            wakeupTimer = null;

            wakeUpThread = new Thread(wakeUpThreadProcedure);
            wakeUpThread.Start();
        }

        void wakeUpThreadProcedure()
        {
            DHMAIN.LoadWar3Mpq();

            DHFormatter.WakeUp();
            DHRC.WakeUpKnow();

            SetUITheme((Theme)DHCFG.Items["UI"].GetIntValue("Theme", (int)Theme.None));

            this.themeToolStripMenuItem.Enabled = true;

            Plugins.PluginHandler.LoadPlugins();
            Plugins.PluginHandler.AttachPlugins();

            switch (DHCFG.Items["Update"].GetIntValue("CheckForUpdates", 2))
            {
                case 1:
                    update(DHCFG.Items["Update"].GetIntValue("ShowSplash", 1) == 1, false);
                    break;

                case 2:
                    int updateInterval = DHCFG.Items["Update"].GetIntValue("UpdateInterval", 7);
                    DateTime lastUpdateDate = new DateTime(DHCFG.Items["Update"].GetLongValue("LastUpdate", DateTime.MinValue.Ticks));
                    TimeSpan difference = new TimeSpan(DateTime.Now.Ticks - lastUpdateDate.Ticks);
                    if (difference.Days >= updateInterval)
                        update(DHCFG.Items["Update"].GetIntValue("ShowSplash", 1) == 1, false);
                    break;
            }
        }

        internal void ApplyConfig()
        {
            SetSoundState(DHCFG.Items["UI"].GetIntValue("MuteSound") == 0);
            SetUIDialogs(DHCFG.Items["UI"].GetIntValue("UseOwnDialogs") == 0);

            if (DHCFG.Items["Settings"].GetIntValue("RestoreOnStartup") == 1)
            {
                DHFILE.AssociateFileTypes(DHCFG.Items["Settings"].GetStringListValue("FileTypes"));
                DHFILE.RegisterContextMenus(DHCFG.Items["Settings"].GetStringListValue("ContextMenus"));
            }

            UpdateRecentMap(null);
            UpdateRecentBuild(null);
        }

        internal void WriteConfig()
        {
            foreach (FloatingListForm ffm in FloatingForms)
                ffm.WriteConfig();

            cbForm.WriteConfig();

            DHCFG.Items["UI"]["Theme"] = (int)Current.theme;
            DHCFG.Items["UI"]["MuteSound"] = Current.sessionAllowsSounds ? 0 : 1;
            DHCFG.Items["UI"]["UseOwnDialogs"] = UIDialogs.UseStandardDialogs ? 0 : 1;
        }

        void DHMAIN_War3MpqLoading()
        {
            if (Current.map != null)
            {
                hlForm.Reset();
                ilForm.Reset();

                hlForm.Minimize(true);
                ilForm.Minimize(true);
            }
        }

        void DHMAIN_MapLoading(MapLoadSettings s)
        {
            if (wakeUpThread != null && wakeUpThread.ThreadState == ThreadState.Running)
                wakeUpThread.Join();

            hlForm.Minimize(true);
            ilForm.Minimize(true);
            cbForm.Clear();
            Current.unit = null;
            DisplayUnit();

            propertiesToolStripMenuItem.Enabled = false;
        }

        void DHMAIN_MapJassCompiled()
        {
            cbForm.PrepareControls();
        }

        void DHMAIN_MapLoaded(MapLoadSettings s)
        {
            propertiesToolStripMenuItem.Enabled = true;

            if (s.MapOK)
            {
                DHLOOKUP.RefreshTaverns();
                DHLOOKUP.RefreshHeroesAbilities();
                DHLOOKUP.RefreshShops();

                s.SplashShowText("arranging heroes..."); s.SplashProgressAdd(10);

                hlForm.Init();
                hlForm.PrepareList();
                hlForm.SetListState(HeroListForm.ListSwitch.Sentinel);
                if (hlForm.ItemCount == 0) hlForm.SetListState(HeroListForm.ListSwitch.All);

                s.SplashShowText("arranging items..."); s.SplashProgressAdd(9);

                ilForm.Init();
                ilForm.PrepareList();
                ilForm.SetListState(0);
            }

            cbForm.PrepareGameUI();

            mapNameLabel.Text = Path.GetFileName(s.Filename);
            Current.filename = s.Filename;

            if (s.UIOpenListsOnFinish)
            {
                ilForm.Maximize();
                hlForm.Maximize();
            }

            DHCFG.Items["Path"]["MapLoad"] = Path.GetDirectoryName(s.Filename);

            if (s.MapOK)
            {
                PrepareTimers();
            }

            UpdateRecentMap(s.Filename);
        }

        void DHMAIN_MpqDatabaseClearing()
        {
            mapNameLabel.Text = "";
        }

        /*internal void LoadMap(string filename)
        {
            LoadMap(filename, true, this);
        }*/
        /*internal void LoadMap(string filename, bool openListsOnFinish, Form splashOwner)
        {
            if (wakeUpThread != null && wakeUpThread.ThreadState == ThreadState.Running)
                wakeUpThread.Join();

            hlForm.Minimize(true);
            ilForm.Minimize(true);
            cbForm.Clear();
            Current.unit = null;
            DisplayUnit();

            splashScreen = new SplashScreen();
            splashScreen.Owner = splashOwner;
                
            splashScreen.Show();
            splashScreen.ShowText("reading map file...");
            DHTIMER.StartCount();
            propertiesToolStripMenuItem.Enabled = false;
            bool mapOk = LoadMpq(filename); splashScreen.ProgressAdd(5);//10
            propertiesToolStripMenuItem.Enabled = true;

            if (mapOk)
            {
                DHLOOKUP.RefreshTaverns();
                DHLOOKUP.RefreshHeroesAbilities();
                DHLOOKUP.RefreshShops();

                splashScreen.ShowText("arranging heroes..."); splashScreen.ProgressAdd(10);

                hlForm.Init();
                hlForm.PrepareList();
                hlForm.SetListState(HeroListForm.ListSwitch.Sentinel);
                if (hlForm.ItemCount == 0) hlForm.SetListState(HeroListForm.ListSwitch.All);

                splashScreen.ShowText("arranging items..."); splashScreen.ProgressAdd(9);

                ilForm.Init();
                ilForm.PrepareList();
                ilForm.SetListState(0);
            }

            cbForm.PrepareGameUI();

            if (DHCFG.Items["Items"].GetIntValue("CollectCombiningData", 0) == 1)
                DHLOOKUP.CollectItemCombiningData();
            
            splashScreen.Close();
            DHTIMER.PrintEndResetCount();             

            mapNameLabel.Text = Path.GetFileName(filename);
            Current.filename = filename;

            //DisplayUnit();
            if (openListsOnFinish)
            {
                ilForm.Maximize();
                hlForm.Maximize();
            }
                        
            DHCFG.Items["Path"]["MapLoad"] = Path.GetDirectoryName(filename);

            if (mapOk)
            {
                PrepareTimers();
            }

            UpdateRecentMap(filename);

            GC.Collect();
        }*/

        public Point GetBindPoint(bool left)
        {
            if (left)
                return new Point(this.DesktopLocation.X + heroInfoPanel.Width,
                              this.DesktopLocation.Y + (this.Height - mainPanel.Height));
            else
                return new Point(this.DesktopLocation.X + (mainPanel.Width - heroSkillsPanel.Width),
                              this.DesktopLocation.Y + (this.Height - mainPanel.Height));
        }

        void ilForm_ItemActivate(object sender, IRecord item)
        {
            LoadItem(item as item);
        }

        private void OnPreMove(Point xy)
        {
            if (PreMove != null)
                PreMove(this, xy);
        }

        private void SetFormLocation(int x, int y)
        {
            OnPreMove(new Point(x, y));
            SetDesktopLocation(x, y);
        }

        private void showInfoB_Click(object sender, EventArgs e)
        {
            heroInfoPanel.Visible = !heroInfoPanel.Visible;
            this.Refresh();
        }

        private void showSkillsB_Click(object sender, EventArgs e)
        {
            heroSkillsPanel.Visible = !heroSkillsPanel.Visible;
            this.Refresh();
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mbX = MousePosition.X - this.Location.X;
                mbY = MousePosition.Y - this.Location.Y;
            }
            else
                mbX = mbY = -1;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbX != -1 && mbY != -1)
            {
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    SetFormLocation(MousePosition.X - mbX, MousePosition.Y - mbY);
            }
            else
                if (!itemOrderLV.Focused && listsBound) this.itemOrderLV.Focus();
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mbX = mbY = -1;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // force remove all handles from the memory
            DHJassHandleEngine.Reset();

            foreach (FloatingListForm flf in FloatingForms)
                flf.Terminate();

            // allow forms to properly close
            foreach (ReplayBrowserForm form in ReplayParserForms.ToArray())
                form.Close();

            WriteConfig();
        }

        private void closeB_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeB_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void heroLvlUpLink_MouseDown(object sender, MouseEventArgs e)
        {
            if (Current.unit != null)
            {
                Current.unit.LevelShift(1);
                heroLvlUpLink.LinkColor = (Current.unit.Level < 25) ? Color.White : Color.Gray;
                heroLvlDownLink.LinkColor = (Current.unit.Level > 1) ? Color.White : Color.Gray;
                UpdateHeroLeveling();
                //DisplayUnit();
            }
        }

        private void heroLvlDownLink_MouseDown(object sender, MouseEventArgs e)
        {
            if (Current.unit != null)
            {
                Current.unit.LevelShift(-1);
                heroLvlUpLink.LinkColor = (Current.unit.Level < 25) ? Color.White : Color.Gray;
                heroLvlDownLink.LinkColor = (Current.unit.Level > 1) ? Color.White : Color.Gray;
                UpdateHeroLeveling();
                //DisplayUnit();
            }
        }

        private void armorPB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbX != -1 && mbY != -1)
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    SetFormLocation(MousePosition.X - mbX, MousePosition.Y - mbY);

            if (toolTip == null)
            {
                toolTip = new ToolTipForm();
                toolTip.showArmorToolTip(Current.unit);
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private void toolTip_MouseLeave(object sender, EventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.Close();
                toolTip = null;
            }

            if (itemToolTip != null)
            {
                itemToolTip.Close();
                itemToolTip = null;
            }

            lastToolTipValue = null;
        }

        private void damagePB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbX != -1 && mbY != -1)
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    SetFormLocation(MousePosition.X - mbX, MousePosition.Y - mbY);

            if (toolTip == null)
            {
                toolTip = new ToolTipForm();
                toolTip.showDamageToolTip(usc);
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private void primAttrPB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbX != -1 && mbY != -1)
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    SetFormLocation(MousePosition.X - mbX, MousePosition.Y - mbY);

            if (toolTip == null)
            {
                toolTip = new ToolTipForm();
                toolTip.showPrimAttrToolTip(Current.unit);
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private void damageLL_MouseDown(object sender, MouseEventArgs e)
        {
            int damageSwitch = (damageLL.Tag is int) ? (int)damageLL.Tag : 0;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    damageSwitch++;
                    if (damageSwitch > 2/*3*/) damageSwitch = 0;
                    break;

                case MouseButtons.Right:
                    damageSwitch--;
                    if (damageSwitch < 0) damageSwitch = 2/*3*/;
                    break;
            }

            switch (damageSwitch)
            {
                case 0:
                    damageLL.Text = "Damage:";
                    break;

                case 1:
                    damageLL.Text = "Damage:stats";
                    break;

                case 2:
                    damageLL.Text = "Damage:dpa";
                    break;

                case 3:
                    damageLL.Text = "Damage:dpa*";
                    break;
            }

            damageLL.Tag = damageSwitch;
            DisplayUnit();
        }

        ArmorCalcType get_armor_switch(ArmorCalcType current, bool next)
        {
            switch (current)
            {
                case ArmorCalcType.Default:
                    return next ? ArmorCalcType.Evasion : ArmorCalcType.SurvivabilityMeasure | ArmorCalcType.DamageSpecified | ArmorCalcType.Evasion;

                case ArmorCalcType.Evasion:
                    return next ? ArmorCalcType.DamageSpecified : ArmorCalcType.Default;

                case ArmorCalcType.DamageSpecified:
                    return next ? ArmorCalcType.DamageSpecified | ArmorCalcType.Evasion : ArmorCalcType.Evasion;

                case ArmorCalcType.DamageSpecified | ArmorCalcType.Evasion:
                    return next ? ArmorCalcType.SurvivabilityMeasure | ArmorCalcType.DamageSpecified : ArmorCalcType.DamageSpecified;

                case ArmorCalcType.SurvivabilityMeasure | ArmorCalcType.DamageSpecified:
                    return next ? ArmorCalcType.SurvivabilityMeasure | ArmorCalcType.DamageSpecified | ArmorCalcType.Evasion : ArmorCalcType.DamageSpecified | ArmorCalcType.Evasion;

                case ArmorCalcType.SurvivabilityMeasure | ArmorCalcType.DamageSpecified | ArmorCalcType.Evasion:
                    return next ? ArmorCalcType.Default : ArmorCalcType.SurvivabilityMeasure | ArmorCalcType.DamageSpecified;

                default:
                    return ArmorCalcType.None;
            }
        }
        private void armorLL_MouseDown(object sender, MouseEventArgs e)
        {
            ArmorCalcType armorSwitch = (ArmorCalcType)((armorLL.Tag is int) ? (int)armorLL.Tag : 0);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    armorSwitch = get_armor_switch(armorSwitch, true);
                    break;

                case MouseButtons.Right:
                    armorSwitch = get_armor_switch(armorSwitch, false);
                    break;
            }

            armorDamageNumUD.Visible = ((armorSwitch & ArmorCalcType.DamageSpecified) != 0);
            if (!armorDamageNumUD.Visible) armorDamageNumUD.Value = 50;

            string info = "";

            if ((armorSwitch & ArmorCalcType.SurvivabilityMeasure) != 0)
                info += "[";

            if ((armorSwitch & ArmorCalcType.DamageSpecified) != 0)
                info += "dmg";

            if ((armorSwitch & ArmorCalcType.Evasion) != 0)
                info += "*";

            armorLL.Text = "Armor:" + info;

            armorLL.Tag = (int)armorSwitch;
            DisplayUnit();
        }

        internal void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DHMAIN.LoadMap(null, this);

            /*string path = DHCFG.Items["Path"].GetStringValue("MapLoad");
            if (string.IsNullOrEmpty(path)) openFileDialogWrapper.InitialDirectory = Application.StartupPath;
            else openFileDialogWrapper.InitialDirectory = path;

            this.openFileDialogWrapper.FileName = "";            
            this.openFileDialogWrapper.Filter = "w3x files|*.w3x|All files|*.*";

            switch (openFileDialogWrapper.ShowDialog())
            {
                case DialogResult.OK:
                    {
                        LoadMap(openFileDialogWrapper.FileName);
                    }
                    break;
            }*/
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PropertiesForm pf = new PropertiesForm();
            pf.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void loadRecentMap_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            string filename = tsmi.Tag as string;

            if (!File.Exists(filename))
            {
                MessageBox.Show("File does not exists");
                return;
            }

            DHMAIN.LoadMap(filename, this);
        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            SetUITheme((Theme)tsmi.MergeIndex);
        }

        private void aboutTSMI_Click(object sender, EventArgs e)
        {
            AboutForm af = new AboutForm();
            af.ShowDialog();
        }

        private void statusLL_MouseDown(object sender, MouseEventArgs e)
        {
            int statusSwitch = (statusLL.Tag is int) ? (int)statusLL.Tag : 0;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    statusSwitch++;
                    if (statusSwitch > 2) statusSwitch = 0;
                    break;

                case MouseButtons.Right:
                    statusSwitch--;
                    if (statusSwitch < 0) statusSwitch = 2;
                    break;
            }

            switch (statusSwitch)
            {
                case 0:
                    statusLL.Text = "Status:";
                    break;

                case 1:
                    statusLL.Text = "Ally Status:";
                    break;

                case 2:
                    statusLL.Text = "Foe Status:";
                    break;
            }

            statusLL.Tag = statusSwitch;
            DisplayUnit();
        }

        private void radarB_Click(object sender, EventArgs e)
        {
            heroInfoPanel.Tag = 0;
            DisplayInfo();
        }

        private void sightB_Click(object sender, EventArgs e)
        {
            heroInfoPanel.Tag = 1;
            DisplayInfo();
        }

        private void batlleB_Click(object sender, EventArgs e)
        {
            heroInfoPanel.Tag = 2;
            DisplayInfo();
        }

        internal void SetListsBoundState(int state)
        {
            bool bound = (state == 0) ? false : true;

            if (listsBound == bound)
                return;

            listsBound = bound;

            switch (bound)
            {
                case true:
                    boundToolStripMenuItem.Checked = true;
                    unboundToolStripMenuItem.Checked = false;
                    break;

                case false:
                    boundToolStripMenuItem.Checked = false;
                    unboundToolStripMenuItem.Checked = true;
                    break;
            }

            foreach (FloatingListForm ff in FloatingForms)
                ff.Bound = bound;
        }

        private void boundStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            SetListsBoundState(tsmi.MergeIndex);
        }

        private void swapUpB_Click(object sender, EventArgs e)
        {
            if (itemOrderLV.SelectedIndices.Count != 0)
            {
                int index = itemOrderLV.SelectedIndices[0];

                if (index < itemOrderLV.Items.Count && index > 0)
                {
                    itemOrderLV.BeginUpdate();

                    ListViewItem lvi_Selected = itemOrderLV.Items[index];
                    ListViewItem lvi_Swapped = itemOrderLV.Items[index - 1];

                    itemOrderLV.Items.RemoveAt(index);
                    itemOrderLV.Items.Insert(index - 1, lvi_Selected);

                    object swap1 = lvi_Selected.Tag;
                    object swap2 = lvi_Swapped.Tag;

                    string text1 = lvi_Selected.Text;
                    string text2 = lvi_Swapped.Text;

                    DateTime lp1 = AbilityOrItemLearnPointComparer.GetLearnPoint(swap1);
                    DateTime lp2 = AbilityOrItemLearnPointComparer.GetLearnPoint(swap2);

                    AbilityOrItemLearnPointComparer.SetLearnPoint(swap1, lp2);
                    AbilityOrItemLearnPointComparer.SetLearnPoint(swap2, lp1);

                    lvi_Selected.Text = text2;
                    lvi_Swapped.Text = text1;

                    itemOrderLV.EndUpdate();

                    Current.unit.refresh();
                    //DisplayUnit();                    
                }
                itemOrderLV.Focus();
            }
        }

        private void swapDownB_Click(object sender, EventArgs e)
        {
            if (itemOrderLV.SelectedIndices.Count != 0)
            {
                int index = itemOrderLV.SelectedIndices[0];

                if (index < itemOrderLV.Items.Count - 1)
                {
                    itemOrderLV.BeginUpdate();

                    ListViewItem lvi_Selected = itemOrderLV.Items[index];
                    ListViewItem lvi_Swapped = itemOrderLV.Items[index + 1];

                    itemOrderLV.Items.RemoveAt(index);
                    itemOrderLV.Items.Insert(index + 1, lvi_Selected);

                    object swap1 = lvi_Selected.Tag;
                    object swap2 = lvi_Swapped.Tag;

                    string text1 = lvi_Selected.Text;
                    string text2 = lvi_Swapped.Text;

                    DateTime lp1 = AbilityOrItemLearnPointComparer.GetLearnPoint(swap1);
                    DateTime lp2 = AbilityOrItemLearnPointComparer.GetLearnPoint(swap2);

                    AbilityOrItemLearnPointComparer.SetLearnPoint(swap1, lp2);
                    AbilityOrItemLearnPointComparer.SetLearnPoint(swap2, lp1);

                    lvi_Selected.Text = text2;
                    lvi_Swapped.Text = text1;

                    itemOrderLV.EndUpdate();

                    Current.unit.refresh();
                    //DisplayUnit();
                }
                itemOrderLV.Focus();
            }
        }

        private void inventoryLL_MouseDown(object sender, MouseEventArgs e)
        {
            int index = inventoryTabControl.SelectedIndex;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    index++;
                    if (index >= inventoryTabControl.TabPages.Count)
                        index = 0;
                    break;

                case MouseButtons.Right:
                    index--;
                    if (index < 0)
                        index = inventoryTabControl.TabPages.Count - 1;
                    break;
            }

            inventoryTabControl.SelectedIndex = index;
            inventoryLL.Text = inventoryTabControl.SelectedTab.Text;
        }

        private void skill_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            ABILITYPROFILE skill = b.Tag as ABILITYPROFILE;
            if (skill == null) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (showHotKeys)
                    {
                        if (string.IsNullOrEmpty(skill.Hotkey) && !(isResearching && !string.IsNullOrEmpty(skill.ResearchHotkey)))
                            break;

                        ToolTipForm ttf = new ToolTipForm(true);
                        string hotkey = ttf.GetHotkey();

                        if (!string.IsNullOrEmpty(hotkey))
                            ckForm.dcAbilitiesHotkeys[skill.codeID] = hotkey;

                        break;
                    }

                    Current.unit.ForceNoRefresh(true);

                    if (fastResearch)
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            if ((skill.AbilityState & AbilityState.AutoCast) != 0)
                            {
                                skill.AutoCast(false); // deactivate auto cast
                                skill.AbilityState = AbilityState.PermanentlyActivated; // activate manual cast
                            }
                            else
                                skill.AbilityState ^= AbilityState.PermanentlyActivated; // toggle manual cast                            
                        }
                        else
                        {
                            bool canLevelUp = (researchRestriction == 0) ? Current.unit.heroAbilities.GetWithAvailableResearchPoints(Current.unit.BaseHeroAbilList).Contains(skill.Owner) : true;
                            if (canLevelUp)
                            {
                                skill.level_up();
                                if (researchRestriction == 1) UpdateHeroLeveling();
                            }
                        }
                    }
                    else
                        if (isResearching)
                    {
                        bool canLevelUp = (researchRestriction == 0) ? Current.unit.heroAbilities.GetWithAvailableResearchPoints(Current.unit.BaseHeroAbilList).Contains(skill.Owner) : true;
                        if (canLevelUp)
                        {
                            skill.level_up();
                            if (researchRestriction == 1) UpdateHeroLeveling();
                        }
                    }
                    else
                    {
                        if ((skill.AbilityState & AbilityState.AutoCast) != 0)
                        {
                            skill.AutoCast(false); // deactivate auto cast
                            skill.AbilityState = AbilityState.PermanentlyActivated; // activate manual cast
                        }
                        else
                        {
                            if ((Control.ModifierKeys & Keys.Shift) != 0)
                                skill.AbilityState ^= AbilityState.PermanentlyActivated;
                            else
                            {
                                skill.AbilityState ^= AbilityState.Activated; // toggle manual cast
                                Current.unit.activate_ability(skill.Owner, false);
                            }
                        }
                    }

                    Current.unit.ForceNoRefresh(false);
                    Current.unit.refresh();
                    break;

                case MouseButtons.Right:
                    if (showHotKeys)
                        ckForm.dcAbilitiesHotkeys.Remove(skill.codeID);
                    else
                        if (fastResearch)
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            skill.AutoCast((skill.AbilityState & AbilityState.AutoCast) == 0);
                            Current.unit.refresh();
                        }
                        else
                        {
                            skill.level_down();
                            if (researchRestriction == 1) UpdateHeroLeveling();
                        }
                    }
                    else
                            if (isResearching)
                    {
                        skill.level_down();
                        if (researchRestriction == 1) UpdateHeroLeveling();
                    }
                    else
                    {
                        skill.AutoCast((skill.AbilityState & AbilityState.AutoCast) == 0);
                        Current.unit.refresh();
                    }
                    break;
            }

            //b.Text = (skill.Level != 0) ? skill.Level.ToString() : "";            
            DisplayUnit(DisplayUnitFlags.Abilities | DisplayUnitFlags.Items);
        }

        private void item_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            DBITEMSLOT itemSlot = b.Tag as DBITEMSLOT;
            if (itemSlot == null) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (!itemSlot.IsNull)
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != 0 && itemSlot.Item.usable)
                        {
                            itemSlot.Item.abilities.SetActivatedState(
                                itemSlot.Item.abilities.HasActivatedState(AbilityState.AllActivatedFlags) ?
                                AbilityState.None :
                                AbilityState.PermanentlyActivated);

                            Current.unit.refresh();
                        }
                        else
                        {
                            dragTarget = b;
                            dragPosition = e.Location;
                        }
                    }
                    break;

                case MouseButtons.Right:
                    itemSlot.drop_item(true);
                    if (itemToolTip != null)
                    {
                        itemToolTip.Close();
                        itemToolTip = null;
                    }
                    break;
            }

            //DisplayUnit();
        }

        private void bp_item_MouseDown(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            DBITEMSLOT itemSlot = b.Tag as DBITEMSLOT;
            if (itemSlot == null) return;

            switch (e.Button)
            {
                case MouseButtons.Right:
                    itemSlot.drop_item(true);
                    break;

                case MouseButtons.Left:
                    item item = itemSlot.grab_item();
                    fullInventory.put_item(item);
                    break;
            }

            DisplayUnit(DisplayUnitFlags.Abilities | DisplayUnitFlags.Items);
        }

        private void item_MouseMove(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;

            if (dragTarget == b)
            {
                if (dragPosition != e.Location)
                {
                    b.DoDragDrop(b, DragDropEffects.Copy | DragDropEffects.Move);
                    return;
                }
            }
            else
            {
                dragTarget = null;
                dragPosition = Point.Empty;
            }

            DBITEMSLOT itemSlot = b.Tag as DBITEMSLOT;
            if (itemSlot == null || itemSlot.IsNull) return;

            if (lastToolTipValue != itemSlot.Item.codeID)
            {
                if (itemToolTip == null)
                    itemToolTip = new ItemToolTipForm(this);
                itemToolTip.showItemToolTip(itemSlot.Item, true);
                lastToolTipValue = itemSlot.Item.codeID;
            }

            itemToolTip.DisplayAtCursor(MousePosition);
        }

        private void item_MouseUp(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            DBITEMSLOT itemSlot = b.Tag as DBITEMSLOT;

            if (itemSlot != null && !itemSlot.IsNull)
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        if ((Control.ModifierKeys & Keys.Shift) == 0 && itemSlot.Item.usable)
                        {
                            (itemSlot.Owner as unit).OnUseItem(itemSlot.Item);
                        }
                        break;
                }

            dragTarget = null;
        }

        private void skill_MouseMove(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            ABILITYPROFILE skill = b.Tag as ABILITYPROFILE;
            if (skill == null) return;

            if ((lastToolTipValue is int && ((int)lastToolTipValue == skill.Level)) == false)
            {
                if (itemToolTip == null)
                    itemToolTip = new ItemToolTipForm(this);
                itemToolTip.showSkillToolTip(skill, isResearching);
                lastToolTipValue = skill.Level;
            }

            itemToolTip.DisplayAtCursor(MousePosition);
        }

        private void itemOrderLV_MouseMove(object sender, MouseEventArgs e)
        {
            Point cp = itemOrderLV.PointToClient(MousePosition);
            ListViewItem lvItem = itemOrderLV.GetItemAt(cp.X, cp.Y);

            if (lvItem == null)
            {
                if (toolTip != null)
                {
                    toolTip.Close();
                    toolTip = null;
                    lastToolTipValue = null;
                }

                return;
            }

            if (lastToolTipValue != lvItem.Tag)
            {
                if (toolTip != null) toolTip.Close();

                toolTip = new ToolTipForm(this);
                toolTip.showAcquisitionToolTip(lvItem);
                lastToolTipValue = lvItem.Tag;
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private void statusTS_MouseMove(object sender, MouseEventArgs e)
        {
            Point cp = statusTS.PointToClient(MousePosition);
            ToolStripItem tsItem = statusTS.GetItemAt(cp.X, cp.Y);

            if (tsItem == null)
            {
                if (toolTip != null)
                {
                    toolTip.Close();
                    toolTip = null;
                    lastToolTipValue = null;
                }

                return;
            }

            if (lastToolTipValue != tsItem.Tag)
            {
                if (toolTip != null) toolTip.Close();

                toolTip = new ToolTipForm(this);
                toolTip.showStatusToolTip(tsItem);
                lastToolTipValue = tsItem.Tag;
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "dhhelp.chm");
        }

        private void anyRTB_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            RichTextBox rtb = (sender as RichTextBox);
            if (rtb.MinimumSize.Width < e.NewRectangle.Size.Width)
                rtb.Width = e.NewRectangle.Size.Width;
            else
                if (rtb.MinimumSize.Width > e.NewRectangle.Size.Width
                    && rtb.Width > rtb.MinimumSize.Width)
                rtb.Width = rtb.MinimumSize.Width;
        }

        private void heroImagePB_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Current.unit != null)
                        Current.unit.playSound(UnitAckSounds.What, true);
                    break;

                case MouseButtons.Right:
                    if (cbForm.Visible)
                        cbForm.Remove();
                    else
                        cbForm.Display();
                    break;
            }

            MainForm_MouseUp(sender, e);
        }

        private void item_Enter(object sender, EventArgs e)
        {
            statusLL.Focus();
        }

        private void dataDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.map == null)
            {
                MessageBox.Show("Load a map first");
                return;
            }

            if (ddForm == null)
            {
                ddForm = new DataDumpForm();
                ddForm.SetParent(this);
            }

            ddForm.Show();
        }

        private void extractImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.map == null)
            {
                MessageBox.Show("Load a map first");
                return;
            }

            if (idForm == null)
            {
                idForm = new ImageDumpForm();
                idForm.SetParent(this);
            }

            idForm.Show();
        }

        private void customKeysGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.map == null)
            {
                MessageBox.Show("Load a map first");
                return;
            }

            if (ckForm == null)
            {
                ckForm = new CustomKeysForm();
                ckForm.SetParent(this);
            }

            ckForm.Show();
        }

        internal void SetCustomKeysMode(bool enable, Color color)
        {
            foreach (Button b in abilitySlots)
            {
                b.Font = enable ? UIFonts.boldHugeVerdana : UIFonts.boldLargeVerdana;
                b.ForeColor = enable ? color : Color.DodgerBlue;
                b.TextAlign = enable ? ContentAlignment.TopLeft : ContentAlignment.BottomRight;
            }

            this.showHotKeys = enable;
            this.DisplayUnit();
        }

        private void armorDamageNumUD_ValueChanged(object sender, EventArgs e)
        {
            DisplayUnit();
        }

        private void skill_Paint(object sender, PaintEventArgs pe)
        {
            Button b = sender as Button;
            ABILITYPROFILE skill = b.Tag as ABILITYPROFILE;
            if (skill == null) return;

            if ((skill.AbilityState & AbilityState.AutoCast) != 0)
                UIGraphics.DrawAutoCastFrame(pe, Color.DodgerBlue);

            if (skill.Owner.IsOnCooldown)
                pe.Graphics.DrawString("" + (int)Math.Ceiling(skill.Owner.CooldownDuration), UIFonts.boldLargeVerdana, Brushes.LimeGreen, (int)pe.ClipRectangle.X, (int)pe.ClipRectangle.Y);
        }

        private void item_DragDrop(object sender, DragEventArgs e)
        {
            Button b = e.Data.GetData(typeof(Button)) as Button;
            if (b != null && b != sender)
            {
                DBITEMSLOT srcSlot = b.Tag as DBITEMSLOT;
                DBITEMSLOT dstSlot = (sender as Button).Tag as DBITEMSLOT;

                fullInventory.swap_items(srcSlot, dstSlot, true);

                //DisplayUnit();
            }

            dragTarget = null;
        }

        private void item_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void main_DragDrop(object sender, DragEventArgs e)
        {
            string filename = (e.Data.GetData("FileNameW") as string[])[0];
            switch (Path.GetExtension(filename))
            {
                case ".w3x":
                    DHMAIN.LoadMap(filename, this);
                    break;
                case ".dhb":
                    LoadHeroBuild(filename);
                    break;
            }
        }

        private void main_DragOver(object sender, DragEventArgs e)
        {
            string[] file = e.Data.GetData("FileNameW") as string[];
            if (file == null || file.Length == 0 || (!file[0].EndsWith("w3x") && !file[0].EndsWith("dhb"))) return;
            e.Effect = DragDropEffects.Copy;
        }

        public void CopyToolTipText()
        {
            if (itemToolTip != null)
                Clipboard.SetText(itemToolTip.GetText());
            else
                if (toolTip != null)
                Clipboard.SetText(toolTip.GetText());
            else
                    if (hlForm.toolTip != null)
                Clipboard.SetText(hlForm.toolTip.GetText());
            else
                        if (ilForm.toolTip != null)
                Clipboard.SetText(ilForm.toolTip.GetText());
        }

        public void ToggleDetailedToolTip()
        {
            if (itemToolTip != null)
                itemToolTip.ToggleDetailedToolTip(isResearching);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                CopyToolTipText();
            else
                if (e.Control && e.KeyCode == Keys.D)
                ToggleDetailedToolTip();
        }

        private void switch_MouseMove(object sender, MouseEventArgs e)
        {
            if (toolTip == null)
            {
                toolTip = new ToolTipForm();

                string text = "";

                int infoID;
                int.TryParse((sender as Button).Tag as string, out infoID);

                switch (infoID)
                {
                    case 0: text = "General Info"; break;
                    case 1: text = "Additional Info"; break;
                    case 2: text = "Attack Effects Interactions"; break;
                }
                toolTip.ShowText(text);
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private void researchB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                SetResearchMode(!fastResearch);
        }

        internal void SetResearchMode(bool fast)
        {
            fastResearch = fast;

            switch (fastResearch)
            {
                case false:
                    researchB.Text = "";
                    break;

                case true:
                    researchB.Text = "Fast";
                    break;
            }

            if (Current.unit != null)
                ArrangeAbilities(Current.unit.heroAbilities);
        }

        private void researchB_Click(object sender, EventArgs e)
        {
            if (fastResearch) return;

            SetResearchState(!isResearching);
        }

        internal Point GetInfoPanelPoint(int slot)
        {
            // x start = 6
            // x step = 56
            // y start = 47
            // y step = 57

            return new Point(
                6 + ((slot % 4) * 56),
                47 + ((slot / 4) * 57)
                );
        }

        internal void SetResearchState(bool active)
        {
            if (Current.map == null)
            {
                MessageBox.Show("Load a map first");
                return;
            }

            isResearching = active;
            backpackPanel.Visible = !active;

            if (isResearching)
            {
                for (int i = 0; i < abilitySlots.Count; i++)
                {
                    Button b = abilitySlots[i] as Button;
                    b.Location = GetInfoPanelPoint(i);
                }

                researchB.Location = GetInfoPanelPoint(11);
                researchB.BackgroundImage = DHRC.Default.GetImage(@"ReplaceableTextures\CommandButtons\BTNCancel.blp");
            }
            else
            {
                foreach (Button b in abilitySlots)
                {
                    int slot;
                    Int32.TryParse(b.Name.Replace("skill_", "").Replace("B", ""), out slot);
                    b.Location = GetInfoPanelPoint(slot);
                }

                researchB.Location = GetInfoPanelPoint(7);
                researchB.BackgroundImage = Properties.Resources.BTNSkillz;
            }

            if (Current.unit != null)
                ArrangeAbilities(Current.unit.heroAbilities);
        }

        private void armorLL_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == 0)
            {
                if (toolTip != null)
                {
                    toolTip.Close();
                    toolTip = null;
                    lastToolTipValue = null;
                }
                return;
            }

            ArmorCalcType armorSwitch = (ArmorCalcType)((armorLL.Tag is int) ? (int)armorLL.Tag : 0);

            if ((lastToolTipValue is ArmorCalcType && ((ArmorCalcType)lastToolTipValue == armorSwitch)) == false)
            {
                if (toolTip != null) toolTip.Close();
                toolTip = new ToolTipForm();

                string text = "";

                if ((armorSwitch & ArmorCalcType.DamageSpecified) != 0)
                {
                    text += "- The value specified in the spinbox is used in calculations as incoming damage";

                    if ((armorSwitch & ArmorCalcType.SurvivabilityMeasure) != 0)
                        text += "\n- Survivability measure for specified incoming damage is calculated\n  General formula: EHP = incoming_damage * hits_required_to_kill_you";
                    else
                        text += "\n- Survivability measure for specified incoming damage is not calculated";
                }
                else
                    text += "- 50 damage is used in calculations as incoming damage";

                if ((armorSwitch & ArmorCalcType.Evasion) != 0)
                    text += "\n- EHP from Evasion is calculated";
                else
                    text += "\n- EHP from Evasion is not calculated";

                toolTip.ShowText(text);
                lastToolTipValue = armorSwitch;
            }

            toolTip.DisplayAtCursor(MousePosition);
        }

        private bool IsCurrentProcessAdmin()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);

                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.SetParent(this);
            sf.ShowDialog();
            sf.Close();
        }

        private void heroLevelLL_MouseDown(object sender, MouseEventArgs e)
        {
            if (Current.unit != null)
            {
                researchRestriction += (e.Button == MouseButtons.Left) ? 1 : -1;

                if (researchRestriction < 0) researchRestriction = 2;
                else
                if (researchRestriction > 2) researchRestriction = 0;

                UpdateHeroLeveling();
                DisplayUnit(DisplayUnitFlags.Stats | DisplayUnitFlags.Abilities);
            }
        }

        private void openBuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIDialogs.OpenFileDialogBoxWrapper.Default.FileName = "";
            UIDialogs.OpenFileDialogBoxWrapper.Default.InitialDirectory = Application.StartupPath;

            UIDialogs.OpenFileDialogBoxWrapper.Default.Filter = "DotA H.I.T. Hero Build|*.dhb|All files|*.*";

            switch (UIDialogs.OpenFileDialogBoxWrapper.Default.ShowDialog())
            {
                case DialogResult.OK:
                    {
                        LoadHeroBuild(UIDialogs.OpenFileDialogBoxWrapper.Default.FileName);
                    }
                    break;
            }
        }

        private void saveBuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.map == null)
            {
                MessageBox.Show("Load map first, then pick a hero which build is to be saved");
                return;
            }

            if (Current.unit == null)
            {
                MessageBox.Show("Pick a hero which build is to be saved");
                return;
            }

            saveFileDialog.FileName = Current.unit.ID + ".dhb";
            saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            SaveHeroBuild(saveFileDialog.FileName);
        }

        private void menuStrip_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Current.unit != null)
            {
                Current.unit.reset();
                ResetBackpack();
            }
        }

        internal void UpdateRecentBuild(string filename)
        {
            HabProperties hps = DHCFG.Items["RecentBuilds"];

            if (filename != null && !hps.ContainsValue(Path.GetFullPath(filename)))
            {
                int length = (hps.Count >= 10) ? hps.Count : (hps.Count + 1);

                for (int i = length; i > 1; i--)
                    hps[i + ""] = hps[(i - 1) + ""];

                hps["1"] = Path.GetFullPath(filename);
            }

            recentBuildsToolStripMenuItem.DropDownItems.Clear();
            recentBuildsToolStripMenuItem.Enabled = (hps.Count != 0);

            foreach (KeyValuePair<string, object> kvp in hps)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                tsmi.Name = kvp.Key;
                tsmi.Text = Path.GetFileName(kvp.Value as string);
                tsmi.Tag = kvp.Value;
                tsmi.Click += new EventHandler(loadRecentBuild_Click);

                recentBuildsToolStripMenuItem.DropDownItems.Add(tsmi);
            }
        }
        void loadRecentBuild_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            string filename = tsmi.Tag as string;

            if (!File.Exists(filename))
            {
                MessageBox.Show("File does not exists");
                return;
            }

            LoadHeroBuild(filename);
        }



        private void replayParserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Replay rp = new Replay(@"D:\Bob\Programming\WorkSpace\DotaHIT\DotaHAB\bin\Debug\EqVSvp.w3g");
            string path = DHCFG.Items["Path"].GetStringValue("ReplayLoad");
            if (string.IsNullOrEmpty(path)) path = Application.StartupPath;

            ReplayParserForms.Add(new ReplayBrowserForm(path));
        }

        void SetSoundState(bool flag)
        {
            Current.sessionAllowsSounds = flag;

            if (flag)
                soundSwitchB.Image = Properties.Resources.Control_Sound;
            else
            {
                Bitmap grayscale = new Bitmap(
                    Properties.Resources.Control_Sound.Width,
                    Properties.Resources.Control_Sound.Height,
                    PixelFormat.Format24bppRgb);

                Graphics gr = Graphics.FromImage(grayscale);

                gr.DrawImageUnscaled(Properties.Resources.Control_Sound, 0, 0);

                BitmapFilter.GrayScale(grayscale);
                BitmapFilter.Brightness(grayscale, -90);

                soundSwitchB.Image = grayscale;
            }
        }

        private void soundSwitchB_Click(object sender, EventArgs e)
        {
            SetSoundState(!Current.sessionAllowsSounds);
        }

        void update(bool showSplash, bool notifyOnLatest)
        {
            try
            {
                string sfxPackageName;
                if (DHRELEASE.TryGetUpdate(showSplash, notifyOnLatest, out sfxPackageName))
                {
                    string batName = "update.bat";
                    DHRELEASE.CreateBatchScript("update.bat", sfxPackageName);

                    this.Close();

                    System.Diagnostics.Process.Start(batName);
                    Application.Exit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Update error: " + e.Message);
            }
        }
        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            update(true, true);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}