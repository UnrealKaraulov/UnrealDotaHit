using System;
using System.Collections.Generic;
using System.Text;
using DotaHIT.Core;
using DotaHIT.Core.Compression;
using DotaHIT.Core.Resources;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.Drawing;
using Deerchao.War3Share.W3gParser;

namespace DotaHIT.Extras
{
    public partial class ReplayMapCache
    {
        static Dictionary<string, Database> dcDatabaseCache = new Dictionary<string, Database>();

        public static void WakeUp() { Database.WakeUp(); }
        public static bool TryGetDatabase(string cachePath, string mapPath, out Database database)
        {            
            if (!dcDatabaseCache.TryGetValue(cachePath, out database))
            {
                database = new Database();
                if (database.LoadFromFile(cachePath, mapPath) == true)
                {
                    dcDatabaseCache.Add(cachePath, database);
                    return true;
                }
                else
                {
                    database = null;
                    return false;
                }
            }

            return true;
        }
             
        Replay replay = null;
        Database database = null;

        public bool IsLoaded
        {
            get { return database != null && database.IsLoaded; }
        }
        public DHRC Resources
        {
            get
            {
                return database != null ? database.Resources : DHRC.Default;
            }
        }

        public StringDictionary dcHeroesTaverns
        {
            get { return this.IsLoaded ? database.dcHeroesTaverns : DHLOOKUP.dcHeroesTaverns; }
        }
        public Dictionary<string, string> dcAbilitiesHeroes
        {
            get { return this.IsLoaded ? database.dcAbilitiesHeroes : DHLOOKUP.dcAbilitiesHeroes; }            
        }
        public HabPropertiesCollection hpcUnitAbilities
        {
            get { return this.IsLoaded ? database.hpcUnitAbilities : DHLOOKUP.hpcUnitAbilities; }
        }
        public HabPropertiesCollection hpcUnitProfiles
        {
            get { return this.IsLoaded ? database.hpcUnitProfiles : DHLOOKUP.hpcUnitProfiles; }
        }
        public HabPropertiesCollection hpcAbilityData
        {
            get { return this.IsLoaded ? database.hpcAbilityData : DHLOOKUP.hpcAbilityData; }
        }
        public HabPropertiesCollection hpcItemProfiles
        {
            get { return this.IsLoaded ? database.hpcItemProfiles : DHLOOKUP.hpcItemProfiles; }
        }
        public HabPropertiesCollection hpcItemData
        {
            get { return this.IsLoaded ? database.hpcItemData : DHLOOKUP.hpcItemData; }
        }
        public HabPropertiesCollection hpcUnitBalance
        {
            get { return this.IsLoaded ? database.hpcUnitBalance : DHMpqDatabase.UnitSlkDatabase["UnitBalance"]; }
        }
        public HabPropertiesCollection hpcMapData
        {
            get { return database != null ? database.hpcMapData : null; }
        }
        public HabPropertiesCollection hpcComplexItems
        {
            get { return this.IsLoaded ? database.hpcComplexItems : DHLOOKUP.hpcComplexItems; }
        }
        public HabProperties StackableItems
        {
            get { return this.IsLoaded ? database.StackableItems : DHLOOKUP.StackableItems; }
        }
        public Bitmap mapImage
        {
            get { return database.mapImage; }
            internal set { database.mapImage = value; }
        }

        public Replay Replay
        {
            get { return replay; }
        }

        public bool IsNewVersionItem(string itemID)
        {
            return this.IsLoaded ? database.hpcUnitBalance.ContainsKey(itemID) : DHHELPER.IsNewVersionItem(itemID);
        }

        public ReplayMapCache()
        {            
        }

        public ReplayMapCache(Replay replay)
        {
            this.replay = replay;
        }

        public bool LoadFromFile(string cachePath, string mapPath)
        {
            if (!dcDatabaseCache.TryGetValue(cachePath, out database))
            {
                database = new Database();

                if (database.LoadFromFile(cachePath, mapPath) == true)
                    dcDatabaseCache.Add(cachePath, database);
                else
                    return false;                
            }            

            return true;
        }
        public bool SaveToFile(string path)
        {
            // find declared properties
            PropertyInfo[] Props = typeof(ReplayMapCache).GetProperties(BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.DeclaredOnly);

            // make sure item combining data will be saved to cache
            DHLOOKUP.CollectItemCombiningData(null, false, false);

            // detect stackable items for use in inventory emulation
            DHLOOKUP.DetectStackableItems();

            // set property values to fields (using default values)
            FieldInfo field;
            foreach (PropertyInfo pi in Props)
                if (Database.NameFieldPairs.TryGetValue("_" + pi.Name, out field))
                    field.SetValue(database, pi.GetValue(this, null));

            bool success = database.SaveToFile(path);

            if (success)
                dcDatabaseCache[path] = database;

            return success;
        }
    }    
}
