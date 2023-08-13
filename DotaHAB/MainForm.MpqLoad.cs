using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Diagnostics;
using Microsoft.Win32;
using Utils;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.MpqPath;

namespace DotaHIT
{
    public partial class MainForm
    {
        /*internal bool LoadMpq(string filename)
        {
            ResetMpqDatabase(); // reset everything that was loaded with previous map

            Current.map = DHRC.OpenArchive(filename, 11);
            
            // heroes

            splashScreen.ShowText("loading units...");
            DHMpqDatabase.LoadUnits(); splashScreen.ProgressAdd(8);
            
            // items

            splashScreen.ShowText("loading items...");
            DHMpqDatabase.LoadItems(); splashScreen.ProgressAdd(8);
            
            // abilites

            splashScreen.ShowText("loading abilities...");
            DHMpqDatabase.LoadAbilities(splashScreen);            

            // upgrades

            splashScreen.ShowText("loading upgrades...");
            DHMpqDatabase.LoadUpgrades(splashScreen);            

            DHLOOKUP.RefreshHotEntries();
            
            // jass

            try
            {
                splashScreen.ShowText("compiling jass...");
                DHJass.ReadCustom(DHRC.Default.GetFile(Script.Custom).GetStream(), false); splashScreen.ProgressAdd(14);

                cbForm.PrepareControls();

                splashScreen.ShowText("executing jass...");
                DHJass.Config();
                DHJass.Go(); splashScreen.ProgressAdd(15);
            }
            catch(Exception e)
            {
                MessageBox.Show("Failed to load the jass script. This heavely reduces the functionality of DotaHIT, but you will still be able to use simple features like DataDump."
                    + Environment.NewLine + "Error Message: " + e.Message);
                return false;
            }

            return true;
        }      */

        /*internal void LoadWar3Mpq()
        {
            if (Current.map != null)
            {
                hlForm.Reset();                
                DHMpqDatabase.UnitSlkDatabase.Clear();

                ilForm.Reset();
                DHMpqDatabase.ItemSlkDatabase.Clear();

                hlForm.Minimize(true);
                ilForm.Minimize(true);
            }

            ClearMpqDatabase();

            string w3path = DHCFG.Items["Path"].GetStringValue("War3");

            if (string.IsNullOrEmpty(w3path))
                w3path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Blizzard Entertainment\Warcraft III", "InstallPath", null) as string;

            string war3_not_located = "Unable to locate Warcraft III installation path. For this tool to work correctly, it is required that you have all Warcraft III .mpq files in one directory." +
                "Would you like to specify that directory? Example: 'C:\\Games\\WarCraft III'";

            string error_no_mpqs = "The specified directory for WarCraft III files does not contain any .mpq files, which will cause maps not to load correctly." +
                "\nWould you like to specify another directory? Example: 'C:\\Games\\WarCraft III'";

            while (String.IsNullOrEmpty(w3path) || !Directory.Exists(w3path))
                switch (MessageBox.Show(war3_not_located, UIStrings.Warning, MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        if (BrowseForFolder())
                            w3path = folderBrowserDialogWrapper.SelectedPath;
                        break;

                    case DialogResult.No:
                        return;
                }

            string[] files;
            while ((files = Directory.GetFiles(Path.GetFullPath(w3path), "*.mpq")).Length == 0)
                switch (MessageBox.Show(error_no_mpqs, UIStrings.Warning, MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        if (BrowseForFolder())
                            w3path = folderBrowserDialogWrapper.SelectedPath;
                        break;

                    case DialogResult.No:
                        return;
                }

            DHCFG.Items["Path"]["War3"] = w3path; // save the correct war3 path

            foreach (string file in files)
            {
                int index = MpqArchive.List.IndexOf(Path.GetFileName(file.ToLower()));
                if (index != -1)
                    DHRC.OpenArchive(file, (uint)index);
                else
                    DHRC.OpenArchive(file, 0);//10);
            }

            ///////// editor data //////////
            DHMpqDatabase.LoadEditorData();

            //////// jass //////////////////                
            MemoryStream jassStream; DHJassExecutor.ShowMissingFunctions = true;

            jassStream = DHRC.Default.GetFile(Script.Common).GetStream();
            DHJass.ReadWar3Ex(jassStream, true);

            jassStream = DHRC.Default.GetFile(Script.Blizzard).GetStream();
            DHJass.ReadWar3(jassStream, false);

            DHJass.CompleteWar3Init();

            Current.sessionHasWar3DB = true;
        }*/        

        internal void LoadHeroBuild(string filename)
        {
            HabPropertiesCollection hpcBuild = new HabPropertiesCollection();
            hpcBuild.ReadFromFile(filename);

            try
            {
                HabProperties hpsHero = hpcBuild.GetByOrder(0);

                // load specified map if current map is empty
                if (Current.map == null)
                {
                    string mapfile = hpsHero.GetStringValue("Map");
                    if (File.Exists(mapfile))
                        DHMAIN.LoadMap(mapfile, this);
                    else
                    {
                        MessageBox.Show(
                            "The map file '" + mapfile + "' specified in this hero build cannot be found." +
                            "\nYou can open this map manually (DotA map->Open...) and then try to load this hero build again");
                        return;
                    }
                }                

                // pick specified hero
                hlForm.PickNewHero(hpsHero.name);

                if (!Current.unit.IsDisposed) load_hero_build(hpsHero);
                else
                    // delay this build until the hero
                    // will be found by hero provider
                    hpsProvidedHeroBuild = hpsHero;

                UpdateRecentBuild(filename);
            }
            catch
            {
                MessageBox.Show("Invalid Hero Build file");
                return;
            }            
        }
        internal void SaveHeroBuild(string filename)
        {
            try
            {
                HabPropertiesCollection hpcBuild = new HabPropertiesCollection();
                HabProperties hpsHero = new HabProperties();

                hpcBuild.Add(Current.unit.codeID, hpsHero);

                hpsHero["Map"] = Current.map.Name;
                hpsHero["Level"] = Current.unit.Level;

                hpsHero["Str"] = Current.unit.get_naked_attr(PrimAttrType.Str);
                hpsHero["Agi"] = Current.unit.get_naked_attr(PrimAttrType.Agi);
                hpsHero["Int"] = Current.unit.get_naked_attr(PrimAttrType.Int);

                ArrayList allAbilities = new ArrayList();

                foreach (DBABILITY ability in Current.unit.heroAbilities)
                    if (ability.LearnPoint != null)
                        allAbilities.Add(ability);

                foreach (DBITEMSLOT itemSlot in Current.unit.Inventory)
                    if (!itemSlot.IsNull)
                        allAbilities.Add(itemSlot.Item);

                allAbilities.Sort(new AbilityOrItemLearnPointComparer());

                HabPropertiesCollection acquisition = new HabPropertiesCollection(allAbilities.Count);
                for(int i=0; i<allAbilities.Count; i++)
                {
                    object obj = allAbilities[i];

                    HabProperties hpsAbilOrItem = new HabProperties();
                    hpsAbilOrItem.name = i + "";

                    if (obj is item)
                    {
                        hpsAbilOrItem["codeID"] = (obj as item).codeID;
                        hpsAbilOrItem["slot"] = Current.unit.Inventory.IndexOf(obj as item);
                    }
                    else
                        if (obj is DBABILITY)
                        {
                            hpsAbilOrItem["codeID"] = (obj as DBABILITY).Alias;
                            hpsAbilOrItem["level"] = (obj as DBABILITY).Level;
                        }

                    acquisition.Add(hpsAbilOrItem);                    
                }

                hpsHero["acquisition"] = acquisition;
                hpsHero["items"] = Current.unit.Inventory.ToString();
                hpsHero["abilities"] = Current.unit.heroAbilities.ToString(true);                

                hpcBuild.SaveToFile(filename);

            }
            catch
            {
                MessageBox.Show("Failed to save Hero Build file");
                return;
            }
        }

        private void load_hero_build(HabProperties hpsHero)
        {
            // set level
            Current.unit.Level = hpsHero.GetIntValue("Level");

            // set stats
            Current.unit.set_naked_attr(PrimAttrType.Str, hpsHero.GetIntValue("Str"));
            Current.unit.set_naked_attr(PrimAttrType.Agi, hpsHero.GetIntValue("Agi"));
            Current.unit.set_naked_attr(PrimAttrType.Int, hpsHero.GetIntValue("Int"));

            // get aquisition order
            HabPropertiesCollection hpcAcquisition = hpsHero.GetHpcValue("acquisition");

            // items
            List<string> items = hpsHero.GetStringListValue("items");

            // abilities
            List<string> abilities = hpsHero.GetStringListValue("abilities");

            // process acquisition order            

            // optimize item handling process by detecting instant timers (interval == 0)
            timer.EnableHistory = true;
            timer.History.Clear();
            timer.maxTimerIntervalForHistory = 1;

            try
            {
                for (int i = 0; i < hpcAcquisition.Count; i++)
                {
                    HabProperties hpsAcquired = hpcAcquisition[i + ""];

                    string codeID = hpsAcquired.GetStringValue("codeID");

                    if (items.Contains(codeID))
                    {
                        item item = new item(codeID);
                        item.set_owningPlayer(Current.player);

                        Current.unit.Inventory.put_item(item, hpsAcquired.GetIntValue("slot"));
                    }
                    else
                        if (abilities.Contains(codeID))
                        {
                            DBABILITY ability = Current.unit.heroAbilities.GetByAlias(codeID);

                            // if it's not a hero ability, then it must have been added via triggers
                            if (ability == null)
                                ability = DBABILITY.InitProperAbility(DHLOOKUP.hpcAbilityData.GetValue(codeID, new HabProperties(codeID)));

                            int level = hpsAcquired.GetIntValue("level");
                            for (int j = 1; j <= level; j++)
                                ability.Level = j;
                        }
                }
            }
            finally
            {
                timer.maxTimerIntervalForHistory = -1;
                foreach (timer t in timer.History)
                    t.forceCallback(); // to pass control to item handling script thread 

                // disable timer history (restore original state)
                timer.EnableHistory = false;   
            }                                 

            UpdateHeroLeveling();
        }

        public void MinimizeMapLoadTime()
        {
            cbForm.ShowBuildCost(false);            
            DHCFG.Items["Items"]["CombineMode"] = (ItemListForm.CombineMode.Normal);
        }
    }
}
