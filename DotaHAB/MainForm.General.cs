using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Jass.Native.Types;
using System.Collections;
using System.CodeDom;
using DotaHIT.Core;
using DotaHIT.Core.Resources.Media;
using DotaHIT.Core.Resources;
using DotaHIT.DatabaseModel.Format;

namespace DotaHIT
{
    public partial class MainForm
    {
        internal DBINVENTORY fullInventory = new DBINVENTORY();
        internal List<Control> fullInventoryControls = new List<Control>();

        internal ArrayList abilitySlots = new ArrayList();
        Font smallFont = new Font("Arial", 2, FontStyle.Regular);

        UnitStatsCalculator usc = null;
        List<UnitStatsCalculator> uscList = new List<UnitStatsCalculator>();

        [Flags]
        public enum DisplayUnitFlags
        {
            None = 0,
            Portrait = 1,
            Stats = 2,
            Info = 4,
            Items = 8,
            Abilities = 16,
            All = Portrait | Stats | Info | Items | Abilities,
            Brief = Portrait | Items | Abilities
        }
      
        Timer selectionProviderTimer = null;
        HabProperties hpsProvidedHeroBuild = null;

        internal void PrepareTimers()
        {
            if (selectionProviderTimer == null)
            {
                selectionProviderTimer = new Timer();
                selectionProviderTimer.Interval = 100;
                selectionProviderTimer.Tick += new EventHandler(selectionProviderTimer_Tick);
                selectionProviderTimer.Start();
            }            
        }

        void selectionProviderTimer_Tick(object sender, EventArgs e)
        {            
            if (Current.player.selection.IsUpdating) return;

            switch(Current.player.selection.Count)
            {
                case 0:
                    if (Current.unit != null)
                    {
                        Current.unit = null;
                        uscList.Clear();

                        DisplayUnit();
                        fullInventory = new DBINVENTORY();                        
                    }
                    else
                        if (playerSelectionTabControl.SelectedTab != selectedUnitTabPage)
                            playerSelectionTabControl.SelectedTab = selectedUnitTabPage;
                    break;

                case 1:
                    if (Current.unit != Current.player.selection[0])
                    {
                        Current.unit = Current.player.selection[0];
                        Current.unit.refresh();                        

                        if (hpsProvidedHeroBuild != null)
                        {
                            load_hero_build(hpsProvidedHeroBuild);
                            Current.unit.Updated = true;
                        }
                    }

                    if (Current.unit.Updated)
                    {
                        Current.unit.Updated = false;

                        if (Current.unit.IsDisposed)
                        {
                            Current.unit = null;
                            Console.Beep(500, 100);
                        }

                        if (fullInventory.Owner != Current.unit) ResetBackpack();
                        DisplayUnit();
                    }
                    else
                        if (Current.unit.HasAbilitiesOnCooldown)
                        {                            
                            foreach (Control c in abilitySlots)
                                if (c.Tag is ABILITYPROFILE && (c.Tag as ABILITYPROFILE).IsOnCooldown)
                                    c.Refresh();
                        }
                        else
                            if (playerSelectionTabControl.SelectedTab != selectedUnitTabPage)
                                playerSelectionTabControl.SelectedTab = selectedUnitTabPage;
                    break;

                default:
                    if (Current.unit == null)
                        Current.unit = Current.player.selection[0];

                    if (fullInventory.Owner != Current.unit)
                    {
                        ResetBackpack();
                        DisplayUnit(DisplayUnitFlags.Brief);
                    }

                    if (Current.unit.HasAbilitiesOnCooldown)
                    {
                        foreach (Control c in abilitySlots)
                            if (c.Tag is ABILITYPROFILE && (c.Tag as ABILITYPROFILE).IsOnCooldown)
                                c.Refresh();
                    }

                    if (Current.player.selection.Updated)
                    {
                        Current.player.selection.Updated = false;
                        DisplaySelection();
                    }
                    else
                        foreach(unit u in Current.player.selection)
                            if (u.Updated)
                            {
                                u.Updated = false;

                                RefreshSelectionStatsCalculator();
                                DisplayUnit(DisplayUnitFlags.Brief);
                                DisplayInfo();
                                break;
                            }
                    break;
            }
        }          

        internal void LoadItem(item item)
        {
            if (Current.unit != null && item != null)
            {
                fullInventory.DisableRefresh();
                
                bool Ok = fullInventory.put_item(item);

                fullInventory.EnableRefresh();

                if (Ok) Current.unit.refresh();

                //DisplayUnit();
            }       
        }

        internal void DisplayUnit()
        {
            DisplayUnit(DisplayUnitFlags.All);
        }
        internal void DisplayUnit(DisplayUnitFlags flags)
        {
            if (Current.unit != null)
            {
                // info
                if ((flags & DisplayUnitFlags.Info) != 0)
                {
                    usc = new UnitStatsCalculator(Current.unit, (int)armorDamageNumUD.Value);
                    usc.PrepareInfo();
                }                

                // portrait
                if ((flags & DisplayUnitFlags.Portrait) != 0)
                {
                    heroImagePB.Image = Current.unit.iconImage;

                    if (Current.unit.IsInvulnerable)
                    {
                        hpTextBox.Text = "";
                        manaTextBox.Text = "";                    
                    }
                    else
                    {
                        hpTextBox.Text = Current.unit.hp.convert_to_int() + "/" + Current.unit.max_hp;
                        manaTextBox.Text = Current.unit.mana.convert_to_int() + "/" + Current.unit.max_mana;
                    }
                }

                // stats
                if ((flags & DisplayUnitFlags.Stats) != 0)
                {
                    heroNameLabel.Text = Current.unit.IsHero ? Current.unit.name : Current.unit.ID;

                    ////////////////
                    // normal damage
                    /////////////////
                    if (Current.unit.CanAttack)
                    {
                        damageRTB.Clear();
                        spellDamageRTB.Clear();

                        DamageCalcType damageCalcType = (DamageCalcType)((damageLL.Tag is int) ? (int)damageLL.Tag : 0);

                        usc.CalculateDamage(damageCalcType);

                        addText(spellDamageRTB, usc.spell_damage, usc.spell_damage > 0);

                        addText(damageRTB, "" + usc.damage, Color.White);
                        addText(damageRTB, " + " + usc.bonus_damage, UIColors.bonus_damage, usc.bonus_damage > 0);
                        addText(damageRTB, " " + usc.bonus_damage.ToString(true), UIColors.negative_damage, usc.bonus_damage < 0);

                        damagePB.Visible = true;
                        damageLL.Visible = true;
                        damageRTB.Visible = true;
                        spellDamageRTB.Visible = true;

                        // atack damage ugrades

                        DBUPGRADE upgrade = Current.unit.upgrades.GetByEffectOfType<DbAttackDamageBonus>();
                        if (upgrade != null)
                        {
                            attackUpgradeLabel.Text = upgrade.Level + "";

                            attackUpgradePanel.BackColor = Color.Black;
                            attackUpgradePanel.Visible = true;
                            attackUpgradePanel.BackColor = Color.Gold;
                        }
                        else
                            attackUpgradePanel.Visible = false;
                    }
                    else
                    {
                        damagePB.Visible = false;
                        damageLL.Visible = false;
                        damageRTB.Visible = false;
                        spellDamageRTB.Visible = false;
                    }

                    ////////////////
                    // armor
                    /////////////////
                    armorRTB.Clear();

                    if (Current.unit.IsInvulnerable)
                        addText(armorRTB, "Invulnerable", Color.Red);
                    else
                    {
                        double naked_armor = Current.unit.get_naked_armor();
                        double bonus_armor = Current.unit.armor - naked_armor;

                        addText(armorRTB, "" + naked_armor.ToString(DBDOUBLE.format, DBDOUBLE.provider), Color.White);
                        addText(armorRTB, " " + bonus_armor.ToString(DBDOUBLE.full_format, DBDOUBLE.provider), UIColors.bonus_damage, bonus_armor != 0);
                    }

                    armorPB.Visible = true;
                    armorLL.Visible = true;
                    armorRTB.Visible = true;

                    //////////////////
                    // hero info
                    //////////////////
                    if (Current.unit.IsHero)
                    {
                        heroLevelLL.Text = "Level " + Current.unit.Level + " " + Current.unit.ID;

                        switch (researchRestriction)
                        {
                            case 0:
                                heroLvlDownLink.Text = "<<";
                                heroLvlUpLink.Text = ">>";
                                break;
                            case 1:
                                heroLvlDownLink.Text = "[";
                                heroLvlUpLink.Text = "]";
                                break;
                            case 2:
                                heroLvlDownLink.Text = "{{";
                                heroLvlUpLink.Text = "}}";
                                break;
                        }                        

                        ////////////////
                        // strength
                        /////////////////
                        strengthRTB.Clear();

                        int naked_str = Current.unit.get_naked_attr(PrimAttrType.Str);
                        int bonus_str = Current.unit.strength.convert_to_int() - naked_str;

                        addText(strengthRTB, "" + naked_str, Color.White);
                        addText(strengthRTB, " + " + bonus_str, UIColors.bonus_damage, bonus_str != 0);
                        ////////////////
                        // agility
                        /////////////////
                        agilityRTB.Clear();

                        int naked_agi = Current.unit.get_naked_attr(PrimAttrType.Agi);
                        int bonus_agi = Current.unit.agility.convert_to_int() - naked_agi;

                        addText(agilityRTB, "" + naked_agi, Color.White);
                        addText(agilityRTB, " + " + bonus_agi, UIColors.bonus_damage, bonus_agi != 0);
                        ////////////////
                        // intelligence
                        /////////////////
                        intelligenceRTB.Clear();

                        int naked_int = Current.unit.get_naked_attr(PrimAttrType.Int);
                        int bonus_int = Current.unit.intelligence.convert_to_int() - naked_int;

                        addText(intelligenceRTB, "" + naked_int, Color.White);
                        addText(intelligenceRTB, " + " + bonus_int, UIColors.bonus_damage, bonus_int != 0);

                        ////////////////
                        // leveling
                        /////////////////
                        heroLvlUpLink.LinkColor = (Current.unit.Level < 25) ? Color.White : Color.Gray;
                        heroLvlDownLink.LinkColor = (Current.unit.Level > 1) ? Color.White : Color.Gray;

                        ///////////////////////
                        // primary attribute
                        //////////////////////
                        switch ((PrimAttrType)Current.unit.primary)
                        {
                            case PrimAttrType.Agi: primAttrPB.Image = global::DotaHIT.Properties.Resources.prim_attr_agi;
                                break;
                            case PrimAttrType.Str: primAttrPB.Image = global::DotaHIT.Properties.Resources.prim_attr_str;
                                break;
                            case PrimAttrType.Int: primAttrPB.Image = global::DotaHIT.Properties.Resources.prim_attr_int;
                                break;
                        }

                        expBarPanel.Visible = true;

                        primAttrPB.Visible = true;

                        strengthLabel.Visible = true;
                        strengthRTB.Visible = true;

                        agilityLabel.Visible = true;
                        agilityRTB.Visible = true;

                        intelligenceLabel.Visible = true;
                        intelligenceRTB.Visible = true;
                    }
                    else
                    {
                        expBarPanel.Visible = false;

                        primAttrPB.Visible = false;

                        strengthLabel.Visible = false;
                        strengthRTB.Visible = false;

                        agilityLabel.Visible = false;
                        agilityRTB.Visible = false;

                        intelligenceLabel.Visible = false;
                        intelligenceRTB.Visible = false;
                    }

                    //////////////////
                    // building info
                    //////////////////

                    if (Current.unit.IsBuilding)
                    {
                        statusLL.Visible = false;
                        statusTS.Visible = false;
                    }
                    else
                    {
                        statusLL.Visible = true;
                        statusTS.Visible = true;
                    }

                    ///////////////////////
                    // buffs
                    ///////////////////////
                    statusTS.Items.Clear();

                    int statusSwitch = (statusLL.Tag is int) ? (int)statusLL.Tag : 0;

                    TargetType[] targets = null;// = AbilityTargets.None;
                    AbilityMatchType matchType = AbilityMatchType.Intersects;                    

                    switch (statusSwitch)
                    {
                        case 0: // self
                            targets = new TargetType[] { TargetType.Self };
                            break;

                        case 1: // ally
                            targets = new TargetType[] { TargetType.Ally | TargetType.Friend };
                            break;

                        case 2: // foe
                            matchType = AbilityMatchType.NotIntersects;
                            targets = new TargetType[] { TargetType.None, TargetType.Self };
                            break;
                    }

                    DBABILITIES buffContainers = Current.unit.acquiredAbilities.GetSpecific(AbilitySpecs.HasBuff | AbilitySpecs.IsActivated,
                        matchType,
                        targets);

                    StackedAbilitiesDictionary stackedBuffContainers = new StackedAbilitiesDictionary(buffContainers);

                    if (statusSwitch == 0)
                    {
                        foreach (StackedAbilities sa in Current.unit.Buffs.Values)
                            foreach (DBABILITY a in sa)
                                stackedBuffContainers.Add(a, false);
                    }                        
                        


                    foreach (StackedAbilities stackedBuffContainer in stackedBuffContainers.Values)
                        foreach (DBABILITY ability in stackedBuffContainer)
                        {
                            DBBUFF buff = ability.Buff;
                            ToolStripButton tsb = new ToolStripButton((Image)buff.Profile.Image);
                            tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            tsb.MouseMove += new MouseEventHandler(statusTS_MouseMove);
                            tsb.MouseLeave += new EventHandler(toolTip_MouseLeave);
                            tsb.Tag = buff.Profile;

                            statusTS.Items.Add(tsb);
                        } 
                }

                // items
                if ((flags & DisplayUnitFlags.Items) != 0)
                {
                    foreach (Button c in fullInventoryControls)
                    {
                        DBITEMSLOT itemSlot = c.Tag as DBITEMSLOT;

                        c.BackgroundImage = itemSlot.Image;

                        if (!itemSlot.IsNull && itemSlot.Item.usable && itemSlot.Item.abilities.HasActivatedState(AbilityState.AllActivatedFlags))
                        {
                            c.FlatAppearance.BorderColor = UIColors.activeAbility;
                            c.FlatAppearance.BorderSize = 1;
                        }
                        else
                        {
                            c.FlatAppearance.BorderColor = Color.Black;
                            c.FlatAppearance.BorderSize = 0;
                        }
                    }
                }

                // abilities
                if ((flags & DisplayUnitFlags.Abilities) != 0)
                {
                    researchB.Visible = Current.unit.IsHero;
                    ArrangeAbilities(Current.unit.heroAbilities);
                }
                else
                {
                    researchB.Visible = false;
                    researchPointsPanel.Visible = false;
                }
            }
            else
            {
                heroImagePB.Image = null;
                heroNameLabel.Text = "";                
                heroLevelLL.Text = "";

                hpTextBox.Text = "";
                manaTextBox.Text = "";

                damageRTB.Text = "";
                spellDamageRTB.Text = "";
                armorRTB.Text = "";
                strengthRTB.Text = "";
                agilityRTB.Text = "";
                intelligenceRTB.Text = "";

                heroLvlUpLink.LinkColor = Color.Gray;
                heroLvlDownLink.LinkColor = Color.Gray;

                damagePB.Visible = false;
                damageLL.Visible = false;
                damageRTB.Visible = false;
                spellDamageRTB.Visible = false;

                armorPB.Visible = false;
                armorLL.Visible = false;
                armorRTB.Visible = false;

                researchB.Visible = false;
                researchPointsPanel.Visible = false;

                expBarPanel.Visible = false;

                primAttrPB.Visible = false;

                strengthLabel.Visible = false;
                strengthRTB.Visible = false;

                agilityLabel.Visible = false;
                agilityRTB.Visible = false;

                intelligenceLabel.Visible = false;
                intelligenceRTB.Visible = false;

                statusLL.Visible = false;
                statusTS.Visible = false;

                if ((flags & DisplayUnitFlags.Items) != 0)
                {
                    ArrayList al = new ArrayList();
                    al.AddRange(heroItemsPanel.Controls);
                    al.AddRange(backpackPanel.Controls);
                    foreach (DBITEMSLOT itemSlot in fullInventory)
                        foreach (Control c in al)
                            if (c.Name == itemSlot.Name + "B")
                            {
                                c.BackgroundImage = null;
                                c.Tag = null;
                            }
                }

                if ((flags & DisplayUnitFlags.Abilities) != 0)
                    foreach (Control c in abilitySlots)
                    {
                        c.Tag = null;
                        c.BackgroundImage = null;
                        c.Text = "";
                    }

                statusTS.Items.Clear();
            }
            
            cbForm.RefreshBuildCost();

            if ((flags & DisplayUnitFlags.Info) != 0)
                DisplayInfo();

            // ability order is available via inventory switch,
            // so if items are displayed, then ability order must be prepared too
            if ((flags & DisplayUnitFlags.Items) != 0)
                RefreshAbilityOrder();

            if ((flags & DisplayUnitFlags.Stats) != 0)
            {
                selectedUnitTabPage.Refresh();

                if (playerSelectionTabControl.SelectedTab != selectedUnitTabPage)
                    playerSelectionTabControl.SelectedTab = selectedUnitTabPage;
            }

            if (DHCFG.Items["UI"].GetIntValue("TextBoxBlurryFontFix", 0) == 1)
                this.Refresh();
        }

        internal void DisplaySelection()
        {
            ///////////////////////////
            // display selected units
            ///////////////////////////

            selectionTS.SuspendLayout();
            selectionTS.Items.Clear();            

            foreach (unit u in Current.player.selection)
            {
                ToolStripButton tsb = new ToolStripButton();

                tsb.BackColor = Current.unit == u ? Color.White : Color.Black;
                tsb.AutoSize = false;
                tsb.Size = selectionTS.ImageScalingSize;
                tsb.Margin = Padding.Empty;
                tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                tsb.Image = (Image)DHRC.Default.GetImage(u.iconName);
                tsb.TextImageRelation = TextImageRelation.Overlay;
                tsb.Tag = u;
                tsb.ToolTipText = u.ID;
                tsb.Text = u.ID;
                tsb.Padding = new Padding(2);

                #region MouseDown
                tsb.MouseDown += new MouseEventHandler(delegate(object sender, MouseEventArgs e)
                {
                    ToolStripButton tsButton = sender as ToolStripButton;

                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (Current.unit == tsButton.Tag)
                            {
                                // if its an already controlled unit,
                                // then make the selection contain just this unit
                                // (same as it would happen in the game)
                                Current.player.selection.BeginUpdate();
                                Current.player.selection.Clear();
                                Current.player.selection.EndUpdate();
                                Current.player.selection.Add(Current.unit);

                                DisplayUnit();
                            }
                            else
                            {
                                // make unit pointed by this button
                                // as currently controlled unit
                                Current.unit = tsButton.Tag as unit;

                                foreach (ToolStripButton tsbItem in selectionTS.Items)
                                    tsbItem.BackColor = tsbItem == tsButton ? Color.White : Color.Black;
                            }
                            break;
                    }
                });
                #endregion

                #region MouseEnter
                tsb.MouseEnter += new EventHandler(delegate(object sender, EventArgs e)
                {
                    ToolStripButton tsButton = sender as ToolStripButton;

                    if (Current.unit != tsButton.Tag)
                        tsButton.BackColor = Color.Gray;
                });
                #endregion

                #region MouseMove
                tsb.MouseMove += new MouseEventHandler(delegate(object sender, MouseEventArgs e)
                {
                    Point cp = selectionTS.PointToClient(MousePosition);
                    ToolStripItem tsItem = selectionTS.GetItemAt(cp.X, cp.Y);

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
                        toolTip.showUbertipToolTip(tsItem);
                        lastToolTipValue = tsItem.Tag;
                    }

                    toolTip.DisplayAtCursor(MousePosition);
                });
                #endregion

                #region MouseLeave
                tsb.MouseLeave += new EventHandler(delegate(object sender, EventArgs e)
                {
                    ToolStripButton tsButton = sender as ToolStripButton;

                    if (Current.unit != tsButton.Tag)
                        tsButton.BackColor = Color.Black;

                    toolTip_MouseLeave(sender, e);
                });
                #endregion

                selectionTS.Items.Add(tsb);
            }

            selectionTS.ResumeLayout();

            RefreshSelectionStatsCalculator();

            DisplayUnit(DisplayUnitFlags.Brief);

            if (playerSelectionTabControl.SelectedTab != selectedUnitsTabPage)
                playerSelectionTabControl.SelectedTab = selectedUnitsTabPage;

            DisplayInfo();
        }                

        void RefreshSelectionStatsCalculator()
        {
            uscList.Clear();

            foreach (unit u in Current.player.selection)
            {
                UnitStatsCalculator usc = new UnitStatsCalculator(u, 0);
                usc.PrepareInfo();
                uscList.Add(usc);
            }
        }     
        
        internal void ResetBackpack()
        {
            fullInventory = new DBINVENTORY(null, Current.unit, new FieldAttributeCollection());

            if (Current.unit != null)
            fullInventory.AddRange(Current.unit.Inventory);

            ///////////////////////
            // items
            ///////////////////////

            fullInventoryControls.Clear();
            
            foreach (Control c in heroItemsPanel.Controls)                    
            {
                c.Tag = null;
                c.BackgroundImage = DHRC.Default.GetImage("ReplaceableTextures\\CommandButtonsDisabled\\DIS" + Current.theme + "-inventory-slotfiller.blp");
            }    

            foreach (DBITEMSLOT itemSlot in fullInventory)
                foreach (Control c in heroItemsPanel.Controls)
                    if (c.Name == itemSlot.Name + "B")
                    {
                        c.Tag = itemSlot;
                        fullInventoryControls.Add(c);
                    }                    

            foreach (DBITEMSLOT itemSlot in fullInventory)
                foreach (Control c in backpackPanel.Controls)
                    if (c.Name == itemSlot.Name + "B")
                    {
                        c.Tag = itemSlot;
                        fullInventoryControls.Add(c);
                    }            
        }

        internal void GatherAbilitySlots()
        {
            abilitySlots.Clear();
            
            foreach (Control c in heroSkillsPanel.Controls)
                if (c.Name.StartsWith("skill"))
                    abilitySlots.Add(c);

            abilitySlots.Sort(new AbilitySlotsComparer());
        }

        public class AbilitySlotsComparer : IComparer
        {
            private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {                
                int priorityX;
                int priorityY;

                Int32.TryParse((x as Control).Name.Replace("skill_", "").Replace("B", ""), out priorityX);
                Int32.TryParse((y as Control).Name.Replace("skill_", "").Replace("B", ""), out priorityY);

                return priorityX - priorityY;
            }
        }

        internal void ArrangeAbilities(DBABILITIES abilities)
        {
            if (isResearching)
                ArrangeResearchAbilities(abilities);
            else
            if (fastResearch)
                ArrangeReadyAbilities(abilities);
            else                            
                ArrangeReadyAbilities(abilities.GetSpecific(AbilitySpecs.IsLearned));
        }        
        internal void ArrangeReadyAbilities(DBABILITIES abilities)
        {
            int researchPoints = Current.unit.IsHero ? Current.unit.Level - Current.unit.heroAbilities.GetTotalResearchPoints(Current.unit.BaseHeroAbilList) : 0;
            if (researchPoints != 0)
            {
                researchPointsLabel.Text = researchPoints + "";
                researchPointsLabel.Width = (researchPointsLabel.Text.Length > 2) ? 19 : 15;

                researchPointsPanel.BackColor = Color.Black;
                researchPointsPanel.Visible = true;
                researchPointsPanel.BringToFront();
                researchPointsPanel.BackColor = Color.Gold;
            }
            else
                researchPointsPanel.Visible = false;

            int matchesFound = 0;

            foreach (Button c in abilitySlots)
            {
                matchesFound = 0;

                // find ability for that button

                foreach (DBABILITY ability in abilities)
                    if (ability.IsAvailable && ability.IsVisible && c.Name == "skill_" + ability.Profile.SlotPriority + "B")
                    {
                        matchesFound++;
                        if (matchesFound > 1)
                        {
                            Button emptySlot = GetEmptyAbilitySlot();
                            if (emptySlot != null)
                                PlaceAbility(emptySlot, ability.Profile);
                            matchesFound--;
                        }
                        else
                            PlaceAbility(c, ability.Profile);
                    }

                if (matchesFound == 0)
                    PlaceAbility(c, null);
            }
        }
        internal void ArrangeResearchAbilities(DBABILITIES abilities)
        {
            researchPointsPanel.Visible = false;

            unit hero = abilities.Owner as unit;
            if (hero == null) return;

            List<DBABILITY> baseAbils = abilities.GetRange(hero.BaseHeroAbilList);
            List<DBABILITY> availableForResearch = (researchRestriction == 0) ? abilities.GetWithAvailableResearchPoints(hero.BaseHeroAbilList) : baseAbils;

            bool found;
            for (int i = 0; i < abilitySlots.Count; i++)
            {
                found = false;
                Button b = abilitySlots[i] as Button;

                foreach (DBABILITY ability in baseAbils)                
                    if (ability.Profile.ResearchSlotPriority == i)
                    {
                        PlaceResearchAbility(b, ability.Profile, availableForResearch.Contains(ability));
                        found = true;
                        break;
                    }

                if (!found)
                    PlaceResearchAbility(b, null, false);
            }
        }
        internal void PlaceAbility(Button c, ABILITYPROFILE ability)
        {
            if (ability != null)
            {
                if (!ability.IsPassive && ((ability.AbilityState & AbilityState.PermanentlyActivated)!=0))
                {
                    c.FlatAppearance.BorderColor = UIColors.activeAbility;
                    c.FlatAppearance.BorderSize = 2;
                }
                else
                {
                    c.FlatAppearance.BorderColor = Color.Black;
                    c.FlatAppearance.BorderSize = 0;
                }

                c.BackgroundImage = ability.IsOnCooldown ?  ability.DisabledResearchImage : ability.Image;
                c.Tag = ability;

                if (showHotKeys)
                {  
                    string hotkey;
                    if (!ckForm.dcAbilitiesHotkeys.TryGetValue(ability.codeID, out hotkey))
                        hotkey = ability.Hotkey;

                    c.Text = hotkey;
                }
                else
                    c.Text = (ability.Level != 0) ? ability.Level.ToString() : "";                
            }
            else
            {
                c.FlatAppearance.BorderColor = Color.Black;
                c.FlatAppearance.BorderSize = 0; 
                c.BackgroundImage = null;
                c.Tag = null;
                c.Text = "";
            }

        }
        internal void PlaceResearchAbility(Button c, ABILITYPROFILE ability, bool isAvailable)
        {
            if (ability != null)
            {
                c.FlatAppearance.BorderColor = Color.Black;
                c.FlatAppearance.BorderSize = 0;

                c.BackgroundImage = isAvailable ? ability.ResearchImage : ability.DisabledResearchImage;
                c.Tag = ability;

                if (showHotKeys)
                {
                    string hotkey;
                    if (!ckForm.dcAbilitiesHotkeys.TryGetValue(ability.codeID, out hotkey))
                        hotkey = ability.ResearchHotkey.Trim('"');

                    c.Text = hotkey;
                }
                else
                    c.Text = (ability.Level != 0) ? ability.Level.ToString() : "";
            }
            else
            {
                c.FlatAppearance.BorderColor = Color.Black;
                c.FlatAppearance.BorderSize = 0;
                c.BackgroundImage = null;
                c.Tag = null;
                c.Text = "";
            }

        }
        internal Button GetEmptyAbilitySlot()
        {
            foreach (Button c in abilitySlots)
                if (c.Tag == null)
                    return c;
            return null;
        }
        internal void RefreshAbilityOrder()
        {
            if (Current.unit != null)
            {
                ArrayList allAbilities = new ArrayList();

                foreach (DBABILITY ability in Current.unit.heroAbilities)
                    if (ability.IsAvailable && ability.LearnPoint != null)
                        allAbilities.Add(ability);

                foreach (DBITEMSLOT itemSlot in Current.unit.Inventory)
                    if (!(itemSlot is DBBACKPACKITEMSLOT) && !itemSlot.IsNull)
                        allAbilities.Add(itemSlot.Item);

                allAbilities.Sort(new AbilityOrItemLearnPointComparer());

                object lastValue = null;
                if (itemOrderLV.SelectedItems.Count != 0)
                    lastValue = itemOrderLV.SelectedItems[0].Tag;

                itemOrderLV.BeginUpdate();                
                itemOrderLV.Items.Clear();

                for(int i=0; i<allAbilities.Count; i++)
                {
                    object obj = allAbilities[i];
                    ListViewItem lvi_Item = new ListViewItem();

                    lvi_Item.Text = AbilityOrItemLearnPointComparer.GetName(obj);
                    lvi_Item.Tag = obj;                    

                    if (obj == lastValue)
                        lvi_Item.Selected = true;

                    itemOrderLV.Items.Add(lvi_Item);
                }                

                itemOrderLV.EndUpdate();
            }
        }        

        public class AbilityOrItemLearnPointComparer : IComparer
        {
            private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                DateTime leanPointX = GetLearnPoint(x);
                DateTime leanPointY = GetLearnPoint(y);

                return cic.Compare(leanPointX, leanPointY);
            }

            public static DateTime GetLearnPoint(object obj)
            {
                if (obj is item)
                {
                    DBABILITIES abils = (obj as item).abilities;
                    if (abils.Count == 0)
                        return DateTime.Now;
                    else
                        return abils[0].LearnPoint;
                }
                else
                    if (obj is DBABILITY)
                        return (obj as DBABILITY).LearnPoint;
                    else
                        return DateTime.Now;
            }
            public static void SetLearnPoint(object obj, DateTime LearnPoint)
            {
                if (obj is item)
                {
                    DBABILITIES abils = (obj as item).abilities;
                    foreach(DBABILITY ability in abils)
                        ability.LearnPoint = LearnPoint;                    
                }
                else
                    if (obj is DBABILITY)
                        (obj as DBABILITY).LearnPoint = LearnPoint;                    
            }

            public static string GetName(object aoi)
            {
                if (aoi is item)
                    return (aoi as item).ID.Text.Trim('"');
                else
                    if (aoi is DBABILITY)
                        return (aoi as DBABILITY).Name.Trim('"');
                    else
                        return "";
            }

            public static string GetCode(object aoi)
            {
                if (aoi is item)
                    return (aoi as item).codeID;
                else
                    if (aoi is DBABILITY)
                        return (aoi as DBABILITY).Alias;
                    else
                        return "";
            }
        }        

        internal void addText(RichTextBox rtb, string text)
        {
            rtb.AppendText(text);
        }
        internal void addText(RichTextBox rtb, string text, bool condition)
        {
            if (condition)
                rtb.AppendText(text);
        }        
        internal void addText(RichTextBox rtb, string text, Color color)
        {
            rtb.SelectionColor = color;
            rtb.AppendText(text);

        }
        internal void addText(RichTextBox rtb, string text, Color color, bool condition)
        {
            if (condition)
            {
                rtb.SelectionColor = color;
                rtb.AppendText(text);
            }
        }
        internal void addLineInterval(RichTextBox rtb)
        {
            Font oldFont = rtb.SelectionFont;
            rtb.SelectionFont = smallFont;
            rtb.SelectionBackColor = Color.DimGray;
            string str = "";
            for (int i = 0; i < 5; i++)
                str += "                                            ";
            rtb.AppendText("\n"+str+"\n");
            rtb.SelectionBackColor = Color.Black;
            rtb.SelectionFont = oldFont;
        }
        internal void addLineInterval(RichTextBox rtb, Color color)
        {
            Font oldFont = rtb.SelectionFont;
            rtb.SelectionFont = smallFont;
            rtb.SelectionBackColor = color;
            string str = "";
            for (int i = 0; i < 5; i++)
                str += "                                            ";
            rtb.AppendText("\n" + str + "\n");
            rtb.SelectionBackColor = Color.Black;
            rtb.SelectionFont = oldFont;
        }        

        internal void DisplayInfo()
        {
            int infoID = (heroInfoPanel.Tag is int) ? (int)heroInfoPanel.Tag : 0;

            switch (infoID)
            {
                case 0: DisplayGeneralInfo(); break;
                case 1: DisplayCustomInfo(); break;
                case 2: DisplayAttackEffectsInfo(); break;
            }
        }

        internal void DisplayGeneralInfo()
        {
            #region single unit

            if (Current.unit != null && Current.player.selection.Count == 1)
            {
                RichTextBox bufferRTB = new RichTextBox();

                bufferRTB.Font = heroInfoRTB.Font;
                bufferRTB.Clear();

                // get armor switch value
                ArmorCalcType armorCalcType = (ArmorCalcType) ((armorLL.Tag is int) ? (int)armorLL.Tag : 0);

                usc.CalculateGeneralInfo(armorCalcType);

                if (!Current.unit.IsInvulnerable)
                {
                    addText(bufferRTB, "HP: " + Current.unit.max_hp, Color.Lime);
                    addText(bufferRTB, "" + ((double)Current.unit.hpRegen).ToString(DBDOUBLE.full_format, DBDOUBLE.provider), Color.Gray);
                    addText(bufferRTB, "  MP: " + Current.unit.max_mana, Color.Lime);
                    addText(bufferRTB, "" + ((double)Current.unit.manaRegen).ToString(DBDOUBLE.full_format, DBDOUBLE.provider), Color.Gray);
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "EHP normal: " + (int)usc.total_ehp_normal, Color.Lime);
                    addText(bufferRTB, "\nEHP spell: " + (int)usc.total_ehp_spell);
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "Damage reduction: " + (int)usc.damage_reduction_normal + "%", Color.Lime);
                    addText(bufferRTB, "\nEHP bonus: " + (int)usc.ehp_increase_normal, Color.Lime);
                    addText(bufferRTB, " (" + (int)usc.ehp_increase_normal_items + ")", Color.Silver, usc.ehp_increase_normal_items != 0);
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "Spell Resistance: " + usc.spell_resistance.ToString(DBDOUBLE.format, DBDOUBLE.provider) + "%", Color.Lime);
                    addText(bufferRTB, "\nEHP bonus: " + usc.ehp_increase_spell.ToString(DBDOUBLE.format, DBDOUBLE.provider), Color.Lime);
                    addText(bufferRTB, " (" + (int)usc.ehp_increase_spell_items + ")", Color.Silver, usc.ehp_increase_spell_items != 0);
                    addLineInterval(bufferRTB);
                }
                else
                {
                    addText(bufferRTB, "Invulnerable", Color.Red);
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "EHP normal: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);

                    addText(bufferRTB, "\nEHP spell: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);
                    addLineInterval(bufferRTB);
                    
                    addText(bufferRTB, "Damage reduction: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);

                    addText(bufferRTB, "\nEHP bonus: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "Spell Resistance: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);

                    addText(bufferRTB, "\nEHP bonus: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);
                    addLineInterval(bufferRTB);
                }

                if (Current.unit.CanAttack)
                {
                    addText(bufferRTB, "Damage: " + usc.total_damage, Color.Lime);
                    addText(bufferRTB, "\nCooldown: " + Current.unit.cooldown);
                    addText(bufferRTB, "\nDPS normal: " + usc.dps_normal.ToString(DBDOUBLE.format, DBDOUBLE.provider));
                    addText(bufferRTB, "\nDPS spell: " + usc.dps_spell.ToString(DBDOUBLE.format, DBDOUBLE.provider));                    
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "Attack range: " + Current.unit.range, Color.Lime);
                }
                else
                {
                    addText(bufferRTB, "No attack\n \n \n ", Color.Red);                    
                    addLineInterval(bufferRTB);

                    addText(bufferRTB, "Attack range: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);
                }

                if (!Current.unit.IsBuilding)
                    addText(bufferRTB, "\nMove speed: " + Current.unit.moveSpeed, Color.Lime);
                else
                {
                    addText(bufferRTB, "\nMove speed: ", Color.Lime);
                    addText(bufferRTB, "n/a", Color.Red);
                }

                heroInfoRTB.Rtf = bufferRTB.Rtf;
            }

            #endregion

            #region multiple units

            else if (uscList.Count > 0)
            {
                RichTextBox bufferRTB = new RichTextBox();

                bufferRTB.Font = heroInfoRTB.Font;
                bufferRTB.Clear();

                addText(bufferRTB, "Units Selected: " + uscList.Count, Color.Lime);                
                addLineInterval(bufferRTB);

                double selection_dps_normal = 0.0;
                double selection_dps_spell = 0.0;

                foreach(UnitStatsCalculator usc in uscList)
                {
                    usc.CalculateDamage(DamageCalcType.DPA);
                    usc.CalculateGeneralInfo(false, true, ArmorCalcType.None);

                    selection_dps_normal += usc.dps_normal;
                    selection_dps_spell += usc.dps_spell;
                }

                addText(bufferRTB, "DPS normal: " + selection_dps_normal.ToString(DBDOUBLE.format, DBDOUBLE.provider), Color.Lime);
                addText(bufferRTB, "\nDPS spell: " + selection_dps_spell.ToString(DBDOUBLE.format, DBDOUBLE.provider));
                addLineInterval(bufferRTB);

                heroInfoRTB.Rtf = bufferRTB.Rtf;
            }

            #endregion

            else
            {
                heroInfoRTB.Text = "";
            }
        }

        internal void DisplayCustomInfo()
        {
            if (Current.unit != null)
            {
                RichTextBox bufferRTB = new RichTextBox();

                bufferRTB.Font = heroInfoRTB.Font;
                bufferRTB.Clear();                

                ////////////////////////
                //   E V A S I O N
                ////////////////////////

                DBEVASIONABILITY evasion = Current.unit.onDefenceAbilities.GetByType<DBEVASIONABILITY>();
                if (evasion != null && evasion.Chance != 0)
                {
                    addText(bufferRTB, "Evasion: " + evasion.Chance * 100 + "%", Color.Lime);
                    addLineInterval(bufferRTB);
                }

                ///////////////////////////////////////////
                //   D A M A G E    B L O C K 
                ///////////////////////////////////////////

                List<DBBLOCKDAMAGEABILITY> blocks = Current.unit.onDefenceAbilities.GetRangeByType<DBBLOCKDAMAGEABILITY>();
                if (blocks.Count != 0)
                {
                    DBDOUBLE chance = usc.defenseStacked.GetProbabilityOfType<DBBLOCKDAMAGEABILITY>();

                    addText(bufferRTB, "Damage Block sources: " + blocks.Count, Color.Lime);

                    if (blocks.Count > 1)
                    {
                        addText(bufferRTB, "\nChance to block: " + (int)(Math.Round(chance * 100.0)) + "%", Color.Lime);

                        DBDOUBLE maxDamageBlock = new DBDOUBLE(null);
                        DBDOUBLE maxBlockChance = new DBDOUBLE(null);

                        foreach (StackedAbilities sa in usc.defenseStacked)
                        {
                            DBBLOCKDAMAGEABILITY block = sa.GetFirstByType<DBBLOCKDAMAGEABILITY>();
                            if (block != null)
                            {
                                if (maxDamageBlock < block.BlockDamage)
                                {
                                    maxDamageBlock = block.BlockDamage;
                                    maxBlockChance = sa.probability;
                                }
                                else
                                    if (maxDamageBlock == block.BlockDamage)
                                        maxBlockChance += sa.probability;
                            }
                        }

                        addText(bufferRTB, "\nMax block/chance: " + maxDamageBlock + "/" + maxBlockChance, Color.Lime);
                    }
                    else
                        addText(bufferRTB, "\nBlock/chance: " + blocks[0].BlockDamage + "/" + chance, Color.Lime);

                    addLineInterval(bufferRTB);
                }

                ///////////////////////////////////////////
                //   C R I T I C A L    S T R I K E
                ///////////////////////////////////////////

                List<DBCRITICALSTRIKEABILITY> crits = Current.unit.onAttackAbilities.GetRangeByType<DBCRITICALSTRIKEABILITY>();
                if (crits.Count != 0)
                {
                    DBDOUBLE chance = usc.attackStacked.GetProbabilityOfType<DBCRITICALSTRIKEABILITY>();

                    addText(bufferRTB, "Critical Strike sources: " + crits.Count, Color.Lime);                    

                    if (crits.Count > 1)
                    {
                        addText(bufferRTB, "\nChance to crit: " + (int)(Math.Round(chance * 100.0)) + "%", Color.Lime);

                        DBDOUBLE maxCritMultiplier = new DBDOUBLE(null);
                        DBDOUBLE maxCritChance = new DBDOUBLE(null);

                        foreach (StackedAbilities sa in usc.attackStacked)
                        {
                            DBCRITICALSTRIKEABILITY crit = sa.GetFirstByType<DBCRITICALSTRIKEABILITY>();
                            if (crit != null)
                            {
                                if (maxCritMultiplier < crit.CritMultiplier)
                                {
                                    maxCritMultiplier = crit.CritMultiplier;
                                    maxCritChance = sa.probability;
                                }
                                else
                                    if (maxCritMultiplier == crit.CritMultiplier)
                                        maxCritChance += sa.probability;
                            }
                        }

                        addText(bufferRTB, "\nMax crit/chance: " + maxCritMultiplier + "/" + maxCritChance, Color.Lime);
                    }
                    else
                        addText(bufferRTB, "\nCrit/chance: " + crits[0].CritMultiplier + "/" + chance, Color.Lime);

                    addLineInterval(bufferRTB);
                }

                
                ///////////////////////////////////
                //   B A S H
                ///////////////////////////////////

                List<DBBASHABILITY> bashes = Current.unit.onAttackAbilities.GetRangeByType<DBBASHABILITY>();
                if (bashes.Count != 0)
                {
                    DBDOUBLE chance;
                    if (Current.unit.attackMethod == AttackMethod.Melee)
                        chance = usc.attackStacked.GetProbabilityOfType<DBBASHABILITY>();
                    else
                        chance = usc.onAttackStats.GetResultAbilityTypeProbability<DBBASHABILITY>("APPLIED_EFFECT");
                    
                    addText(bufferRTB, "Bash sources: " + bashes.Count, Color.Lime);                    

                    if (bashes.Count > 1)
                    {
                        addText(bufferRTB, "\nChance to bash: " + (int)(Math.Round(chance * 100.0)) + "%", Color.Lime);

                        DBDOUBLE maxStunTime = new DBDOUBLE(null);
                        DBDOUBLE maxStunChance = new DBDOUBLE(null);

                        bool changed;
                        foreach (StackedAbilities sa in usc.attackStacked)
                        {
                            changed = false;

                            foreach (DBABILITY a in sa)
                                if (a is DBBASHABILITY)
                                {
                                    if (maxStunTime < (a as DBBASHABILITY).StunTime)
                                    {
                                        maxStunTime = (a as DBBASHABILITY).StunTime;
                                        maxStunChance = sa.probability;
                                        changed = true;
                                    }
                                    else
                                        if (maxStunTime == (a as DBBASHABILITY).StunTime && !changed)
                                        {
                                            maxStunChance += sa.probability;
                                            changed = true;
                                        }
                                }
                        }

                        addText(bufferRTB, "\nMax stun/chance: " + maxStunTime + "/" + maxStunChance, Color.Lime);
                    }
                    else
                        addText(bufferRTB, "\nStun/chance: " + bashes[0].StunTime + "/" + chance, Color.Lime);

                    addLineInterval(bufferRTB);
                }

                heroInfoRTB.Rtf = bufferRTB.Rtf;
            }
            else
            {
                heroInfoRTB.Text = "";
            }
        }

        internal void DisplayAttackEffectsInfo()
        {
            if (Current.unit != null)
            {
                RichTextBox bufferRTB = new RichTextBox();

                bufferRTB.Font = heroInfoRTB.Font;
                bufferRTB.Clear();

                ///////////////////////////////////
                //   O R B    E F F E C T S
                ///////////////////////////////////

                if (usc.onAttackStats.Count != 0)
                {
                    List<string> allAttackEffects = new List<string>(Current.unit.onAttackAbilities.Count);

                    foreach (DBABILITY effect in Current.unit.onAttackAbilities)
                        if (effect is DBTRIGGER)
                        {
                            string effectName = (effect as DBTRIGGER).CommonName;

                            if (effectName != null && !allAttackEffects.Contains(effectName = effectName.Trim('"')))
                                allAttackEffects.Add(effectName);
                        }

                    if (allAttackEffects.Count == 0) { heroInfoRTB.Text = ""; return; }

                    addText(bufferRTB, "Attack Effects: " + allAttackEffects.Count, Color.Lime);
                    addText(bufferRTB, "\nCombinations: (", Color.Lime);
                    addText(bufferRTB, "active", Color.LightBlue);
                    addText(bufferRTB, "/", Color.Lime);
                    addText(bufferRTB, "inactive", Color.Gray);
                    addText(bufferRTB, ")", Color.Lime);

                    List<string> combinationsCache = new List<string>(usc.onAttackStats.Count);

                    foreach (AbilityResults ar in usc.onAttackStats)
                        if (ar.probability != 0)
                        {
                            List<string> currentResultEffects = new List<string>(ar.sources.Count);
                            foreach(DBABILITY source in ar.sources)
                                if (source is DBTRIGGER)
                                {
                                    string effectName = (source as DBTRIGGER).CommonName.Trim('"');

                                    if (!currentResultEffects.Contains(effectName))
                                        currentResultEffects.Add(effectName);
                                }

                            currentResultEffects.Sort();

                            string combination = DHFormatter.ToStringList(currentResultEffects);
                            if (!combinationsCache.Contains(combination))
                                combinationsCache.Add(combination);
                            else
                                continue;

                            addLineInterval(bufferRTB);

                            for (int i = 0; i < allAttackEffects.Count; i++)
                            {
                                if (currentResultEffects.Contains(allAttackEffects[i]))
                                    addText(bufferRTB, allAttackEffects[i], Color.LightBlue);
                                else
                                    addText(bufferRTB, allAttackEffects[i], Color.Gray);

                                if ((i + 1) < allAttackEffects.Count)
                                    addText(bufferRTB, ", ", Color.Lime);
                            }
                        }
                }

                heroInfoRTB.Rtf = bufferRTB.Rtf;
            }
            else
            {
                heroInfoRTB.Text = "";
            }
        }

        internal void UpdateHeroLeveling()
        {
            switch(researchRestriction)
            {
                case 0:
                    List<DBABILITY> baseAbilities = Current.unit.heroAbilities.GetRange(Current.unit.BaseHeroAbilList);
                    foreach (DBABILITY a in baseAbilities)
                        if (!a.IsResearchedProperly)
                            a.IsResearchedProperly = true;

                    int total_researched = 0;

                    foreach (DBABILITY a in baseAbilities)
                        total_researched += a.Level;

                    int difference = total_researched - Current.unit.Level;

                    if (difference > 0)
                        foreach (DBABILITY a in baseAbilities)
                            if (difference > a.Level)
                            {
                                difference -= a.Level;
                                a.Level = 0;
                            }
                            else
                            {
                                a.LevelShift(-difference);
                                break;
                            }    
                    break;

                case 1:
                    Current.unit.Level = Current.unit.heroAbilities.GetRequiredOwnerLevel(Current.unit.BaseHeroAbilList);
                    break;
            }            
        }
    }
}
