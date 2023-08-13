using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Core.Resources;

namespace DotaHIT
{
    public partial class ToolTipForm : Form
    {        
        private bool activate = false;

        public ToolTipForm()
        {       
            InitializeComponent();
        }

        public ToolTipForm(bool activate)
        {
            this.activate = activate;

            InitializeComponent();
        }

        public ToolTipForm(Form Owner)
        {
            InitializeComponent();
            this.Owner = Owner;
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return !activate;
            }
        }     
     
        public void showArmorToolTip(unit hero)
        {            
            UIRichTextEx.Default.ClearText();

            if (hero == null) return;

            UIRichTextEx.Default.AddText("Armor: " + hero.armor, UIFonts.boldArial8, Color.White);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Type: ", Color.Gray); UIRichTextEx.Default.AddText("Hero", Color.Yellow);
            UIRichTextEx.Default.AddText("\nDamage reduction: " + hero.get_damage_reduction() + "%", Color.Gray);
            UIRichTextEx.Default.AddText("\nMove speed: " + hero.moveSpeed);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Heroes take reduced damage from Piercing, Magic,\nSiege attacks, and Spells", Color.White);

            contentRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        public void showDamageToolTip(UnitStatsCalculator usc)
        {
            UIRichTextEx.Default.ClearText();

            if (usc == null) return;

            //Damage totalDamage = (Damage)hero.damage + additional_damage;
            unit hero = usc.unit;

            UIRichTextEx.Default.AddText("Damage: " + usc.total_damage, UIFonts.boldArial8, Color.White);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Type: ", Color.Gray); UIRichTextEx.Default.AddText("Hero", Color.Yellow);
            UIRichTextEx.Default.AddText("\nRange: " + hero.range + " (" + hero.attackMethod + ")", Color.Gray);
            UIRichTextEx.Default.AddText("\nSpeed (cooldown): " + hero.cooldown + " sec (" + ((int)(hero.ias * 100)).ToString(DBINT.full_format, DBDOUBLE.provider) + "% IAS)");
            UIRichTextEx.Default.AddText("\nAvg. damage per second: " + usc.dps_normal.ToString(DBDOUBLE.format, DBDOUBLE.provider));

            // atack damage ugrades

            DBUPGRADE upgrade = hero.upgrades.GetByEffectOfType<DbAttackDamageBonus>();
            if (upgrade != null)
                UIRichTextEx.Default.AddText("\nUpgrade: " + upgrade.Profile.Name + " - Level " + upgrade.Level);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Hero attacks do reduced damage to fortified armor", Color.White);

            contentRTB.Rtf = UIRichTextEx.Default.CloseRtf();       
        }

        public void showPrimAttrToolTip(unit hero)
        {
            UIRichTextEx.Default.ClearText();

            if (hero == null) return;

            UIRichTextEx.Default.AddText("Hero Attributes:", UIFonts.boldArial8, Color.White);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Strength", Color.White);
            if (hero.primary == PrimAttrType.Str)
            {
                UIRichTextEx.Default.AddText("\n - Primary Attribute", Color.Yellow);
                UIRichTextEx.Default.AddText("\n-Each point increases damage by 1", Color.White);
            }
            UIRichTextEx.Default.AddText("\n-Each point increases hit points by 19",Color.White);
            UIRichTextEx.Default.AddText("\n-Each point increases hit point regen");
            UIRichTextEx.Default.AddText("\nStrength per level: "+hero.strPerLvl, Color.Gray);
            UIRichTextEx.Default.AddText("\nHit point regen: "+hero.hpRegen);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Agility", Color.White);
            if (hero.primary == PrimAttrType.Agi)
            {
                UIRichTextEx.Default.AddText("\n - Primary Attribute", Color.Yellow);
                UIRichTextEx.Default.AddText("\n-Each point increases damage by 1", Color.White);
            }
            UIRichTextEx.Default.AddText("\n-Every 7 points increase armor by 1");
            UIRichTextEx.Default.AddText("\n-Each point increases attack speed by 1%");
            UIRichTextEx.Default.AddText("\nAgility per level: " + hero.agiPerLvl, Color.Gray);

            UIRichTextEx.Default.AddLineInterval();

            UIRichTextEx.Default.AddText("Intelligence", Color.White);
            if (hero.primary == PrimAttrType.Int)
            {
                UIRichTextEx.Default.AddText("\n - Primary Attribute", Color.Yellow);
                UIRichTextEx.Default.AddText("\n-Each point increases damage by 1", Color.White);
            }
            UIRichTextEx.Default.AddText("\n-Each point increases mana by 13");
            UIRichTextEx.Default.AddText("\n-Each point increases mana regen");
            UIRichTextEx.Default.AddText("\nIntelligence per level: " + hero.intPerLvl, Color.Gray);            
            UIRichTextEx.Default.AddText("\nMana regen: " + hero.manaRegen + " mana/sec");

            contentRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }      

        public void showAcquisitionToolTip(ListViewItem lvItem)
        {
            Size minSize = contentTLP.MinimumSize;
            minSize.Width = 150;            
            contentTLP.MinimumSize = minSize;
            botPanel.Size = minSize;
            topPanel.Size = minSize;

            minSize = contentRTB.MinimumSize;
            minSize.Width = 150; 
            contentRTB.MinimumSize = minSize;

            UIRichTextEx.Default.ClearText();

            if (lvItem == null) return;

            item item = lvItem.Tag as item;
            string name = MainForm.AbilityOrItemLearnPointComparer.GetName(lvItem.Tag);

            UIRichTextEx.Default.AddText(name, UIFonts.boldArial8, Color.White);
            UIRichTextEx.Default.AddLineInterval();

            string order = "";
            int iOrder = lvItem.Index + 1;
            switch (iOrder)
            {
                case 1:
                    order = "1-st";
                    break;
                case 2:
                    order = "2-nd";
                    break;
                case 3:
                    order = "3-rd";
                    break;
                default:
                    order = iOrder + "-th";
                    break;
            }

            UIRichTextEx.Default.AddText("Acquired: ", UIFonts.boldArial8, Color.LightSkyBlue);
            UIRichTextEx.Default.AddText(order, UIFonts.boldArial8, Color.White);

            contentRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        public void showStatusToolTip(ToolStripItem tsItem)
        {
            UIRichTextEx.Default.ClearText();

            if (tsItem == null) return;

            ABILITYPROFILE ability = tsItem.Tag as ABILITYPROFILE;

            if (ability == null) return;

            UIRichTextEx.Default.AddTaggedText(ability.Tip, UIFonts.boldArial8, Color.Lime);
            UIRichTextEx.Default.AddLineInterval();
            if (ability.Max_level != 1)
            {
                UIRichTextEx.Default.AddText("Level: ", UIFonts.boldArial8, Color.Gray);
                UIRichTextEx.Default.AddText(ability.Level + "", UIFonts.boldArial8, Color.White);
                UIRichTextEx.Default.AddLineInterval();
            }
            string ubertip = ability.Ubertip + "";
            ubertip = ubertip.Replace(";", ";\n");
            UIRichTextEx.Default.AddText(ubertip, UIFonts.boldArial8, Color.White);

            contentRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        public void showUbertipToolTip(ToolStripItem tsItem)
        {
            Size minSize = contentTLP.MinimumSize;
            minSize.Width = 150;
            minSize.Height = 10;
            contentTLP.MinimumSize = minSize;
            contentRTB.MinimumSize = minSize;
            contentRTB.Size = minSize;

            UIRichTextEx.Default.ClearText();

            if (tsItem == null) return;

            unit unit = tsItem.Tag as unit;

            if (unit == null) return;

            if (unit.IsHero)
            {
                UIRichTextEx.Default.AddTaggedText(unit.name==null? unit.ID : unit.name, UIFonts.boldArial8, Color.White);
                UIRichTextEx.Default.AddLineInterval();

                UIRichTextEx.Default.AddText("Level " + unit.Level, UIFonts.boldArial8, Color.White);
            }
            else
            {
                UIRichTextEx.Default.AddTaggedText(unit.ID, UIFonts.boldArial8, Color.White);
                UIRichTextEx.Default.AddLineInterval();

                UIRichTextEx.Default.AddTaggedText(DHLOOKUP.hpcUnitProfiles[unit.codeID].GetStringValue("Ubertip"), UIFonts.boldArial8, Color.White);
            }

            contentRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        public void Display()
        {
            if (this.Visible == false)
                this.Visible = true;
        }

        public void DisplayAtCursor(Point position)
        {
            // [experimental]
            // a fix to stop the tooltip flickering 
            // on some machines
            position.Offset(1, 1);

            Rectangle rect = this.Bounds;
            rect.X = position.X + Cursor.HotSpot.X;
            rect.Y = position.Y + Cursor.HotSpot.Y;

            if (Screen.PrimaryScreen.WorkingArea.Contains(rect) == false)
            {
                if ((rect.X + rect.Width) > Screen.PrimaryScreen.WorkingArea.Width)
                    rect.X -= Cursor.HotSpot.X + rect.Width;

                if ((rect.Y + rect.Height) > Screen.PrimaryScreen.WorkingArea.Height)
                    rect.Y -= Cursor.HotSpot.Y + rect.Height;

                if (rect.Y < 0)
                    rect.Y = Screen.PrimaryScreen.WorkingArea.Height - rect.Height;

                if (rect.X < 0)
                    rect.X = Screen.PrimaryScreen.WorkingArea.Width - rect.Width;
            }

            this.Location = rect.Location;

            if (this.Visible == false)
                this.Visible = true;
        }

        public string GetHotkey()
        {
            UIRichText.Default.ClearText();
            UIRichText.Default.AddText("\n   Enter the hotkey, or press Esc to cancel...", UIFonts.boldArial8, Color.White);
            contentRTB.Rtf = UIRichText.Default.buffer.Rtf;
                    
            this.CenterToScreen();
            this.ShowDialog();
            return this.Text;
        }        

        public void ShowText(string text)
        {
            Size minSize = contentTLP.MinimumSize;
            minSize.Width = 150;
            minSize.Height = 10;
            contentTLP.MinimumSize = minSize;
            contentRTB.MinimumSize = minSize;
            contentRTB.Size = minSize;

            UIRichText.Default.ClearText();
            UIRichText.Default.AddText(text, UIFonts.boldArial8, Color.White);
            contentRTB.Rtf = UIRichText.Default.buffer.Rtf;
        }

        public void ShowText(string text, int timeout)
        {            
            ShowText(text);
            Timer toolTipTimer = new Timer();
            toolTipTimer.Interval = timeout;
            toolTipTimer.Tick += new EventHandler(toolTipTimer_Tick);
            toolTipTimer.Start();
        }

        void toolTipTimer_Tick(object sender, EventArgs e)
        {
            (sender as Timer).Stop();
            this.Close();
        }

        private void contentRTB_ContentsResized(object sender, ContentsResizedEventArgs e)
        {            
            contentRTB.Size = e.NewRectangle.Size;
        }

        private void OnKeyDown(Keys KeyCode)
        {
            switch (KeyCode)
            {
                case Keys.Escape:
                    this.Text = null;
                    break;

                default:
                    string hotkey = KeyCode.ToString();
                    this.Text = (hotkey.Length > 1) ? null : hotkey;
                    break;
            }

            this.Close();
        }

        private void contentRTB_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyCode);            
        }

        private void rightPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            OnKeyDown(e.KeyCode);
        }

        private void ToolTipForm_Shown(object sender, EventArgs e)
        {
            if (activate) rightPanel.Focus();            
        }

        public string GetText()
        {
            return contentRTB.Text;
        }
    }
}