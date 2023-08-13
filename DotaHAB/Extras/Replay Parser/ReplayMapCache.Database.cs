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
        public class Database
        {
            static int version = 2;
            internal static Dictionary<string, FieldInfo> NameFieldPairs = null;            

            static Database()
            {
                NameFieldPairs = CollectNameFieldPairs();
            }
            public static Dictionary<string, FieldInfo> CollectNameFieldPairs()
            {
                Dictionary<string, FieldInfo> dcNameFields = new Dictionary<string, FieldInfo>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

                // field search criteria
                FieldInfo[] Fields = typeof(Database).GetFields(BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.DeclaredOnly);

                for (int i = 0; i < Fields.Length; i++)
                {
                    FieldInfo fi = Fields[i];

                    if (fi.FieldType == typeof(GZipArchive) || fi.FieldType == typeof(DHRC) || fi.FieldType == (typeof(Replay)))
                        continue;

                    dcNameFields.Add(fi.Name, fi);
                }

                return dcNameFields;
            }
            public static void WakeUp() { }

            GZipArchive cacheArchive = null;
            DHRC resources = DHRC.Default;            

            StringDictionary _dcHeroesTaverns = null;
            Dictionary<string, string> _dcAbilitiesHeroes = null;

            [SaveParamsAttribute(true, "heroAbilList", RequiredPropName = "heroAbilList")]
            HabPropertiesCollection _hpcUnitAbilities = null;

            [SaveParamsAttribute(true, "Name", "Propernames", "Art", RequiredPropName = "Art")]
            HabPropertiesCollection _hpcUnitProfiles = null;

            [SaveParamsAttribute(true, "reqLevel", "levelSkip", "code", "Art", "Hotkey", "Buttonpos", "Name", RequiredPropName = "Art")]
            HabPropertiesCollection _hpcAbilityData = null;

            [SaveParamsAttribute(false, "Tip", "Ubertip", "Description")]
            HabPropertiesCollection _hpcItemProfiles = null;

            [SaveParamsAttribute(true, "stockRegen", "uses", "goldcost")]
            HabPropertiesCollection _hpcItemData = null;

            [SaveParamsAttribute(true, "stockRegen", "goldcost")]
            HabPropertiesCollection _hpcUnitBalance = null;

            HabPropertiesCollection _hpcMapData = new HabPropertiesCollection();
            HabPropertiesCollection _hpcComplexItems = null;
            HabProperties _stackableItems = null;
            Bitmap _mapImage = null;

            public bool IsLoaded
            {
                get { return cacheArchive != null; }
            }
            public DHRC Resources
            {
                get
                {
                    return resources;
                }
            }

            public StringDictionary dcHeroesTaverns
            {
                get { return _dcHeroesTaverns; }
            }
            public Dictionary<string, string> dcAbilitiesHeroes
            {
                get { return _dcAbilitiesHeroes; }
            }
            public HabPropertiesCollection hpcUnitAbilities
            {
                get { return _hpcUnitAbilities; }
            }
            public HabPropertiesCollection hpcUnitProfiles
            {
                get { return _hpcUnitProfiles; }
            }
            public HabPropertiesCollection hpcAbilityData
            {
                get { return _hpcAbilityData; }
            }
            public HabPropertiesCollection hpcItemProfiles
            {
                get { return _hpcItemProfiles; }
            }
            public HabPropertiesCollection hpcItemData
            {
                get { return _hpcItemData; }
            }
            public HabPropertiesCollection hpcUnitBalance
            {
                get { return _hpcUnitBalance; }
            }
            public HabPropertiesCollection hpcMapData
            {
                get { return _hpcMapData; }
            }
            public HabPropertiesCollection hpcComplexItems
            {
                get { return _hpcComplexItems; }
            }
            public HabProperties StackableItems
            {
                get { return _stackableItems; }
            }
            public Bitmap mapImage
            {
                get { return _mapImage; }
                set { _mapImage = value; }
            }            

            public Database()
            {
            }            

            public bool LoadFromFile(string cachePath, string mapPath)
            {
                if (!File.Exists(cachePath)) return false;

                if (!File.Exists(mapPath)) return false;                

                cacheArchive = GZipArchive.FromFile(cachePath);
                if (cacheArchive.Version != version)
                {
                    cacheArchive = null;
                    return false;
                }

                resources = new DHRC(true);
                resources.OpenArchive(mapPath);

                foreach (FieldInfo fi in NameFieldPairs.Values)
                {
                    if (fi.FieldType == typeof(HabPropertiesCollection))
                        fi.SetValue(this, cacheArchive.GetHpc(fi.Name));
                    else
                        if (fi.FieldType == typeof(HabProperties))
                            fi.SetValue(this, cacheArchive.GetHps(fi.Name));
                        else
                            if (fi.FieldType == typeof(StringDictionary))
                                fi.SetValue(this, cacheArchive.GetStringDictionary(fi.Name));
                            else
                                if (fi.FieldType == typeof(Dictionary<string, string>))
                                    fi.SetValue(this, cacheArchive.GetGenericStringDictionary(fi.Name));
                                else
                                    if (fi.FieldType == typeof(Bitmap))
                                        fi.SetValue(this, cacheArchive.GetImage(fi.Name));
                }

                return true;
            }
            public bool SaveToFile(string path)
            {                
                // fill new-version-items collection
                HabPropertiesCollection hpcNewVersionItems = new HabPropertiesCollection(DHLOOKUP.shops.Count * 12);
                if (DHLOOKUP.shops.Count > 0 && DHHELPER.IsNewVersionItemShop(DHLOOKUP.shops[0]))
                    foreach (DotaHIT.Jass.Native.Types.unit shop in DHLOOKUP.shops)
                        foreach (string itemID in shop.sellunits)
                            hpcNewVersionItems[itemID] = new HabProperties(itemID);

                // hpcUnitBalance will only contain keys of new-version-items
                hpcNewVersionItems.Merge(_hpcUnitBalance, true);
                _hpcUnitBalance = hpcNewVersionItems;

                if (cacheArchive == null)
                    cacheArchive = new GZipArchive();

                // write values to cache archive
                foreach (FieldInfo fi in NameFieldPairs.Values)
                {
                    object value = fi.GetValue(this);
                    if (value == null) continue;

                    if (fi.FieldType == typeof(HabPropertiesCollection))
                    {
                        object[] attrs = fi.GetCustomAttributes(false);
                        if (attrs.Length > 0 && attrs[0] is SaveParamsAttribute)
                        {
                            SaveParamsAttribute spa = attrs[0] as SaveParamsAttribute;
                            cacheArchive.AddHpc(fi.Name, value as HabPropertiesCollection, spa.RequiredPropName, spa.SkipPropValues, spa.KeepParams, spa.PropNames);
                        }
                        else
                            cacheArchive.AddHpc(fi.Name, value as HabPropertiesCollection);
                    }
                    else
                        if (fi.FieldType == typeof(HabProperties))
                            cacheArchive.AddHps(fi.Name, value as HabProperties);
                        else
                            if (fi.FieldType == typeof(StringDictionary))
                                cacheArchive.AddStringDictionary(fi.Name, value as StringDictionary);
                            else
                                if (fi.FieldType == typeof(Dictionary<string, string>))
                                    cacheArchive.AddGenericStringDictionary(fi.Name, value as Dictionary<string, string>);
                                else
                                    if (fi.FieldType == typeof(Bitmap))
                                        cacheArchive.AddImage(fi.Name, value as Bitmap);
                }

                string directory = Path.GetDirectoryName(path);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                cacheArchive.SaveToFile(path, version);

                return true;
            }
        }
    }

    public class SaveParamsAttribute : Attribute
    {
        bool _keepParams;        
        string[] _propNames;
      
        public string RequiredPropName = null;
        public string[] SkipPropValues = null;

        public SaveParamsAttribute(bool keepParams, params string[] propNames)
        {
            _keepParams = keepParams;
            _propNames = propNames;
        }

        public bool KeepParams
        {
            get { return _keepParams; }            
        }

        public string[] PropNames
        {
            get { return _propNames; }            
        }
    }
}
