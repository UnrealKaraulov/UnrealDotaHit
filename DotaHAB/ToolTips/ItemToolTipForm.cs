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
using DotaHIT.Jass.Native.Types;
using DotaHIT.DatabaseModel.Format;
using DotaHIT.Core.Resources;

namespace DotaHIT
{
    public partial class ItemToolTipForm : Form
    {
        enum ToolTipType
        {
            None=0,
            Item,
            Skill,
            Hero
        }      
        
        private ToolTipType toolTipType = ToolTipType.None;
        private object toolTipSubject = null;
        private int toggleState = 0;

        public ItemToolTipForm()
        {       
            InitializeComponent();
        }

        public ItemToolTipForm(Form Owner)
        {
            InitializeComponent();
            this.Owner = Owner;
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        internal string ExtractAbilityData(string text, ABILITYPROFILE profile)
        {
            // ...<AOcr,DataA1>...
            
            while (true)
            {
                int start = text.IndexOf('<');
                if (start == -1) break;

                int end = text.IndexOf('>', start);
                if (end == -1) break;

                int length = (end - start) - 1; // кол-во символов между скобками
                if (length == 0) break;

                int data = text.IndexOf(',', start+1, length);
                if (data == -1) break;

                string dataFullString = text.Substring(data + 1, (end - data) - 1);
                if (dataFullString.Length < 6) break;

                // DataA
                string dataName = dataFullString.Substring(0, 5);

                // 1
                string dataLevel = dataFullString.Substring(5);                

                // get ability data value for specified level
                object value = profile.Owner[dataLevel, dataName];

                text = text.Replace(text.Substring(start, length + 2), value + "");
            }

            return text;
        }

        internal void ShowPrices(string mana, string gold)
        {
            bool show = !(string.IsNullOrEmpty(gold) && string.IsNullOrEmpty(mana));
            itemCostPanel.Visible = show;

            if (!show)
                return;
            else
            {
                manacostTSMI.Visible = !string.IsNullOrEmpty(mana);
                manacostTSMI.Text = mana;

                goldcostTSMI.Visible = !string.IsNullOrEmpty(gold);
                goldcostTSMI.Text = gold;
            }
        }

        internal void ShowCopmlexItemPrice(string gold)
        {
            bool show = !(string.IsNullOrEmpty(gold));
            itemCostPanel.Visible = show;

            if (!show)
                return;
            else
            {
                Padding p = manacostTSMI.Padding; p.All = 0; manacostTSMI.Padding = p;
                manacostTSMI.Image = goldcostTSMI.Image;
                manacostTSMI.Visible = true;

                goldcostTSMI.Visible = !string.IsNullOrEmpty(gold);
                goldcostTSMI.Text = gold;
            }
        }

        public void showSkillToolTip(ABILITYPROFILE skill, bool isResearching)
        {
            if (toggleState == 0)
                this.Visible = false;
            else
                toggleState = 0;

            UIRichTextEx.Default.ClearText();

            if (skill == null) return;

            toolTipType = ToolTipType.Skill;
            toolTipSubject = skill;

            if (skill.Level > 0 && !isResearching)
            {
                UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(skill.Tip), UIFonts.boldArial8, Color.White);                
                tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

                ShowPrices(skill.Cost, null);

                UIRichTextEx.Default.ClearText();

                string ubertip = DHFormatter.ToString(skill.Ubertip);
                ubertip = ExtractAbilityData(ubertip, skill);

                UIRichTextEx.Default.AddTaggedText(ubertip, UIFonts.boldArial8, Color.White);
                ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();
            }
            else
            {
                if (isResearching && skill.Level == skill.Max_level)
                {
                    UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(skill.Tip), UIFonts.boldArial8, Color.White);
                    tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

                    ShowPrices(null, null);

                    UIRichTextEx.Default.ClearText();

                    UIRichTextEx.Default.AddText("You have completed researching this skill", UIFonts.boldArial8, Color.White);
                }
                else
                {
                    string researchTip = DHFormatter.ToString(skill.Researchtip, (skill.Level + 1));

                    UIRichTextEx.Default.AddTaggedText(researchTip, UIFonts.boldArial8, Color.White);
                    tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

                    ShowPrices(null, null);

                    UIRichTextEx.Default.ClearText();

                    string researchUbertip = DHFormatter.ToString(skill.Researchubertip);
                    researchUbertip = ExtractAbilityData(researchUbertip, skill);

                    UIRichTextEx.Default.AddTaggedText(researchUbertip, UIFonts.boldArial8, Color.White);
                }

                ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();
            }

            if (DHCFG.Items["UI"].GetIntValue("ShowDetailModeSwitchTip", 1) == 1)
            {
                switchLabel.Text = "details: ctrl+d";
                switchLabel.Visible = true;
            }
        }

        public void ToggleDetailedToolTip(bool isResearching)
        {
            switch (toolTipType)
            {
                case ToolTipType.Skill:                    
                    ABILITYPROFILE ap = toolTipSubject as ABILITYPROFILE;
                    if (ap == null) return;

                    if (toggleState != 0)
                    {
                        showSkillToolTip(ap, isResearching);
                        return;
                    }
                    else
                        toggleState = 1;

                    UIRichTextEx.Default.ClearText();

                    ShowPrices(null, null);

                    addAbilityProfileText(ap, isResearching, false);
                    
                    ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();
                    break;

                case ToolTipType.Item:                    
                    item item = toolTipSubject as item;
                    if (item == null) return;                    

                    if (toggleState != 0)
                    {                        
                        showItemToolTip(item, true);
                        return;
                    }
                    else
                        toggleState = 1;
                    
                    UIRichTextEx.Default.ClearText();

                    ShowPrices(null, null);

                    UIRichTextEx.Default.AddText("Item ID:  ", UIFonts.boldArial8, UIColors.toolTipData);
                    UIRichTextEx.Default.AddText("" + item.codeID, Color.White);
                    UIRichTextEx.Default.AddText("\n\n");

                    for (int i = 0; i < item.abilities.Count; i++)
                    {
                        addAbilityProfileText(item.abilities[i].Profile, false, true);
                        if (i + 1 < item.abilities.Count) UIRichTextEx.Default.AddText("\n\n");

                    }                    

                    ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();                    
                    break;
            }

            if (DHCFG.Items["UI"].GetIntValue("ShowDetailModeSwitchTip", 1) == 1)
            {
                switchLabel.Text = "back: ctrl+d";
                switchLabel.Visible = true;
            }
        }
        protected void addAbilityProfileText(ABILITYPROFILE ap, bool isResearching, bool isItemAbility)
        {
            UIRichTextEx.Default.AddText(isItemAbility ? "Ability Alias:  " : "Alias:  ", UIFonts.boldArial8, UIColors.toolTipData);
            UIRichTextEx.Default.AddText("" + ap.Owner.Alias, Color.White);

            UIRichTextEx.Default.AddText("\nBase Ability:  ", UIColors.toolTipData);
            UIRichTextEx.Default.AddText("" + ap.Owner.codeID, Color.White);

            object value = DHMpqDatabase.AbilitySlkDatabase["Profile"][ap.Owner.codeID, "Name"];
            string baseName = DHFormatter.ToString(value);
            if (!String.IsNullOrEmpty(baseName))
                UIRichTextEx.Default.AddText("  (" + baseName + ")", UIColors.activeEffect);

            if (ap.Level > 0 && !isResearching)
            {
                string text;

                // mana cost
                text = "" + ap[ap.Level, "Cost"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("Cost") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // casting time
                text = "" + ap[ap.Level, "Cast"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("Cast") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // cooldown
                text = "" + ap[ap.Level, "Cool"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("Cool") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // range
                text = "" + ap[ap.Level, "Rng"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("Rng") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // area
                text = "" + ap[ap.Level, "Area"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("Area") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // duration
                text = "" + ap[ap.Level, "Dur"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("Dur") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // hero duration
                text = "" + ap[ap.Level, "HeroDur"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("HeroDur") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                string abilityID = ap.Owner.codeID;

                // data a
                text = "" + ap[ap.Level, "DataA"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataA", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data b
                text = "" + ap[ap.Level, "DataB"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataB", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data c
                text = "" + ap[ap.Level, "DataC"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataC", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data d
                text = "" + ap[ap.Level, "DataD"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataD", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data e
                text = "" + ap[ap.Level, "DataE"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataE", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data f
                text = "" + ap[ap.Level, "DataF"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataF", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data g
                text = "" + ap[ap.Level, "DataG"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataG", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // data h
                text = "" + ap[ap.Level, "DataH"];
                if (!String.IsNullOrEmpty(text))
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityDataString(abilityID, "DataH", true) + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }

                // targets
                text = "" + ap[ap.Level, "targs"];
                if (!String.IsNullOrEmpty(text) && text != "_")
                {
                    if (!UIRichTextEx.Default.IsEmpty) UIRichTextEx.Default.AddText("\n");

                    UIRichTextEx.Default.AddNameValueText(
                        DHSTRINGS.GetWEAbilityStatsString("targs") + ":  ",
                        text,
                        UIFonts.boldArial8, UIColors.toolTipData, Color.White);
                }
            }
            else
            {
                UIRichTextEx.Default.AddText("\nLevels:  ", UIFonts.boldArial8, UIColors.toolTipData);
                UIRichTextEx.Default.AddText("" + ap.Max_level, Color.White);

                UIRichTextEx.Default.AddText("\nRequired Hero Level:  ", UIColors.toolTipData);
                UIRichTextEx.Default.AddText("" + ap[0, "reqLevel"], Color.White);
            }
        }

        public void showHeroToolTip(unit hero)
        {
            this.Visible = false;
            UIRichTextEx.Default.ClearText();

            if (hero == null) return;

            toolTipType = ToolTipType.Hero;
            toolTipSubject = hero;

            UIRichTextEx.Default.AddTaggedText(hero.tip + "", UIFonts.boldArial8, Color.White);
            tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

            ShowPrices(hero.goldCost, null);

            UIRichTextEx.Default.ClearText();
            UIRichTextEx.Default.AddTaggedText(hero.description + "", UIFonts.boldArial8, Color.White);
            ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }

        public void showHeroToolTip(HabProperties hpsHero)
        {
            this.Visible = false;
            UIRichTextEx.Default.ClearText();

            if (hpsHero == null) return;

            toolTipType = ToolTipType.Hero;
            toolTipSubject = hpsHero;

            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(hpsHero.GetValue("Tip")), UIFonts.boldArial8, Color.White);

            tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

            ShowPrices(null, hpsHero.GetValue("goldcost")+"");

            UIRichTextEx.Default.ClearText();
            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(hpsHero.GetStringListValue("Ubertip")), UIFonts.boldArial8, Color.White);
            ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();            
        }

        public void showItemToolTip(item item, bool inventory)
        {
            if (toggleState == 0)
                this.Visible = false;
            else
                toggleState = 0;

            UIRichTextEx.Default.ClearText();

            if (item == null) return;

            toolTipType = ToolTipType.Item;
            toolTipSubject = item;

            UIRichTextEx.Default.AddTaggedText(item.ID, UIFonts.boldArial8, Color.White);

            string gold = null;
            string mana = null;

            if ((inventory && !item.pawnable) == false)
                gold = inventory ? (item.goldCost / 2) + "" : item.goldCost.Text;

            if (inventory && item.usable)            
                foreach(DBABILITY ability in item.abilities)
                    if (ability.IsPassive == false)
                    {
                        mana = ability.Cost.Text;
                        break;
                    }

            ShowPrices(mana, gold);

            tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

            UIRichTextEx.Default.ClearText();

            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(item.description), UIFonts.boldArial8, Color.White);

            ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

            if (DHCFG.Items["UI"].GetIntValue("ShowDetailModeSwitchTip", 1) == 1)
            {
                switchLabel.Text = "details: ctrl+d";
                switchLabel.Visible = true;
            }
        }
        public void showItemToolTip(HabProperties hpsItem, bool inventory)
        {
            UIRichTextEx.Default.ClearText();

            if (hpsItem == null) return;

            toolTipType = ToolTipType.Item;
            toolTipSubject = hpsItem;

            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(hpsItem.GetValue("Tip")), UIFonts.boldArial8, Color.White);//item.ID, UIFonts.boldArial8, Color.White);

            string gold = null;
            string mana = null;

            HabProperties hpsItemData;
            if (DHHELPER.IsNewVersionItem(hpsItem.name))
                hpsItemData = DHMpqDatabase.UnitSlkDatabase["UnitBalance"][hpsItem.name];
            else
                hpsItemData = DHMpqDatabase.ItemSlkDatabase["ItemData"][hpsItem.name];

            bool pawnable = DHFormatter.ToBool(hpsItemData.GetValue("pawnable"));
            DBINT goldcost = new DBINT(hpsItemData.GetValue("goldcost"));
            //int goldcost = DHFormatter.ToInt(hpsItemData.GetValue("goldcost"));

            if ((inventory && !pawnable) == false)
                gold = inventory ? (goldcost / 2) + "" : goldcost.Text;

            ShowPrices(mana, gold);

            tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

            UIRichTextEx.Default.ClearText();

            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(hpsItem.GetValue("UberTip")), UIFonts.boldArial8, Color.White);//item.description.Text, UIFonts.boldArial8, Color.White);

            ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();
        }
        public void showComplexItemToolTip(HabProperties hpsItem, int goldCost)
        {
            UIRichTextEx.Default.ClearText();

            if (hpsItem == null) return;

            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(hpsItem.GetValue("Tip")), UIFonts.boldArial8, Color.White);//item.ID, UIFonts.boldArial8, Color.White);

            string gold = (goldCost == 0) ? "" : goldCost + "";

            HabProperties hpsItemData = DHMpqDatabase.ItemSlkDatabase["ItemData"][hpsItem.name];

            ShowCopmlexItemPrice(gold);

            tipRTB.Rtf = UIRichTextEx.Default.CloseRtf();

            UIRichTextEx.Default.ClearText();

            UIRichTextEx.Default.AddTaggedText(DHFormatter.ToString(hpsItem.GetValue("UberTip")), UIFonts.boldArial8, Color.White);//item.description.Text, UIFonts.boldArial8, Color.White);

            ubertipRTB.Rtf = UIRichTextEx.Default.CloseRtf();
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
        private void contentRTB_ContentsResized(object sender, ContentsResizedEventArgs e)
        {            
            (sender as RichTextBox).Size = e.NewRectangle.Size;
        }

        public string GetText()
        {
            return tipRTB.Text + "\n\n"+ ubertipRTB.Text;
        }
    }
}