using System;
using System.Collections.Generic;
using System.Text;

namespace DotaHIT
{
    namespace MpqPath
    {
        public class Script
        {            
            public static readonly string Common = "Scripts\\common.j";
            public static readonly string Blizzard = "Scripts\\Blizzard.j";

            public static readonly string Custom = "Scripts\\war3map.j";
            public static readonly string Custom2 = "Scripts\\war3map.j";
        }

        public class Unit
        {
            public static readonly string[] ProfileList = new string[] 
            {
                "Units\\CampaignUnitFunc.txt",
                "Units\\CampaignUnitStrings.txt",

                "Units\\HumanUnitFunc.txt",
                "Units\\HumanUnitStrings.txt",

                "Units\\OrcUnitFunc.txt",
                "Units\\OrcUnitStrings.txt",

                "Units\\NightElfUnitFunc.txt",
                "Units\\NightElfUnitStrings.txt",

                "Units\\UndeadUnitFunc.txt",
                "Units\\UndeadUnitStrings.txt",

                "Units\\NeutralUnitFunc.txt",
                "Units\\NeutralUnitStrings.txt"            
            };

            public static readonly string Data = "Units\\UnitData.slk";
            public static readonly string Balance = "Units\\UnitBalance.slk";
            public static readonly string Weapons = "Units\\UnitWeapons.slk";
            public static readonly string Abilities = "Units\\UnitAbilities.slk";            

            public static readonly string MetaData = "Units\\UnitMetaData.slk";
            public static readonly string CustomTable = "war3map.w3u";

            public static readonly string UI = "Units\\unitUI.slk";
            public static readonly string AckSounds = "UI\\SoundInfo\\UnitAckSounds.slk";
        }

        public class Item
        {
            public static readonly string[] ProfileList = new string[] 
            {
                "Units\\ItemStrings.txt",
                "Units\\ItemFunc.txt"
            };

            public static readonly string Data = "Units\\ItemData.slk";                   

            public static readonly string MetaData = "Units\\UnitMetaData.slk";
            public static readonly string CustomTable = "war3map.w3t";            
        }

        public class Ability
        {
            public static readonly string[] ProfileList = new string[] 
            {
                "Units\\CampaignAbilityFunc.txt",
                "Units\\CampaignAbilityStrings.txt",

                "Units\\CommonAbilityFunc.txt",
                "Units\\CommonAbilityStrings.txt",
            
                "Units\\HumanAbilityFunc.txt",
                "Units\\HumanAbilityStrings.txt",

                "Units\\NeutralAbilityFunc.txt",
                "Units\\NeutralAbilityStrings.txt",

                "Units\\NightElfAbilityFunc.txt",
                "Units\\NightElfAbilityStrings.txt",

                "Units\\OrcAbilityFunc.txt",
                "Units\\OrcAbilityStrings.txt",

                "Units\\UndeadAbilityFunc.txt",
                "Units\\UndeadAbilityStrings.txt",

                "Units\\ItemAbilityFunc.txt",
                "Units\\ItemAbilityStrings.txt"          
            };

            public static readonly string Data = "Units\\AbilityData.slk";

            public static readonly string MetaData = "Units\\AbilityMetaData.slk";
            public static readonly string CustomTable = "war3map.w3a";
        }

        public class Upgrade
        {
            public static readonly string[] ProfileList = new string[] 
            {
                "Units\\CampaignUpgradeFunc.txt",
                "Units\\CampaignUpgradeStrings.txt",

                "Units\\CommonUpgradeFunc.txt",
                "Units\\CommonUpgradeStrings.txt",
            
                "Units\\HumanUpgradeFunc.txt",
                "Units\\HumanUpgradeStrings.txt",

                "Units\\NeutralUpgradeFunc.txt",
                "Units\\NeutralUpgradeStrings.txt",

                "Units\\NightElfUpgradeFunc.txt",
                "Units\\NightElfUpgradeStrings.txt",

                "Units\\OrcUpgradeFunc.txt",
                "Units\\OrcUpgradeStrings.txt",

                "Units\\UndeadUpgradeFunc.txt",
                "Units\\UndeadUpgradeStrings.txt"                       
            };

            public static readonly string Data = "Units\\UpgradeData.slk";            
        }

        public class Editor
        {
            public static readonly string Data = "UI\\UnitEditorData.txt";
            public static readonly string Strings = "UI\\WorldEditStrings.txt";            
            public static readonly string Misc = "war3mapMisc.txt";

            public static readonly string TriggerStrings = "war3map.wts";
        }

        public class MpqArchive
        {
            public static readonly List<string> List = new List<string>( new string[] 
            { 
                "war3.mpq",                
                "war3x.mpq",
                "war3xlocal.mpq",
                "war3patch.mpq"
            });
        }
    }
}
