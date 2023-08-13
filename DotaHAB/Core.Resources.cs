using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using DotaHIT.DatabaseModel.Format;
using DotaHIT;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Abilities;
using BlpLib;
using Utils;
using System.CodeDom;
using System.Media;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass.Types;
using DotaHIT.Jass;
using SoundPlayerEx;
using FileExtensionWrapper;
using System.Drawing.Imaging;
using DotaHIT.MpqPath;
using Microsoft.Win32;

namespace DotaHIT.Core.Resources
{
    using Media;

    namespace Media
    {
        public enum UnitAckSounds
        {
            Ready,
            What,
            Yes,
            Pissed
        }
        public enum AnimSounds
        {
            Birth,
            Death
        }
    }

    public class DHTIMER
    {
        public static System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        public static void StartCount()
        {
            Stopwatch.Start();
        }
        /// <summary>
        /// resets time counter to zero and starts timer
        /// </summary>
        public static void StartNewCount()
        {
            ResetCount();
            StartCount();
        }
        public static void EndCount()
        {
            Stopwatch.Stop();
        }
        public static float Elapsed
        {
            get
            {
                return (float)Stopwatch.ElapsedMilliseconds / (float)1000.0;
            }
        }
        public static void PrintCount()
        {
            Console.WriteLine("Time: " + Elapsed);
        }
        public static void PrintCount(string param)
        {
            Console.WriteLine(" Time: " + Elapsed + " " + param);
        }
        /// <summary>
        /// sets time counter to zero
        /// </summary>
        public static void ResetCount()
        {
            Stopwatch.Reset();
        }

        /// <summary>
        /// stops timer and prints results
        /// </summary>
        public static void PrintEndCount()
        {
            EndCount();
            PrintCount();
        }

        /// <summary>
        /// stops timer, prints results, and then resets timer
        /// </summary>
        public static void PrintEndResetCount()
        {
            EndCount();
            PrintCount();
            ResetCount();
        }
        /// <summary>
        /// stops timer and prints results
        /// </summary>
        public static void PrintEndCount(string param)
        {
            EndCount();
            PrintCount(param);
        }

        /// <summary>
        /// stops timer, prints results, sets time counter to zero and starts timer
        /// </summary>
        public static void PrintRefreshCount()
        {
            EndCount();
            PrintCount();
            ResetCount();
            StartCount();
        }
        /// <summary>
        /// stops timer, prints results, sets time counter to zero and starts timer
        /// </summary>
        public static void PrintRefreshCount(string param)
        {
            EndCount();
            PrintCount(param);
            ResetCount();
            StartCount();
        }
    }

    public struct MapLoadSettings
    {
        public string Filename;
        public bool UIOpenListsOnFinish;

        public bool SupressBaseLookupsRefresh;
        public bool RefreshItemCombiningDataLookups;
        public bool SupressErrorMessages;
        public bool DisableNonDotaJass;

        public SplashScreen Splash;

        public bool MapOK;

        public MapLoadSettings(string filename, Form splashOwner)
            : this(filename, true, new SplashScreen(splashOwner))
        {
        }
        public MapLoadSettings(string filename, bool openListsOnFinish, SplashScreen splash)
        {
            Filename = filename;
            UIOpenListsOnFinish = openListsOnFinish;

            SupressBaseLookupsRefresh = false;
            RefreshItemCombiningDataLookups = false;
            SupressErrorMessages = false;
            DisableNonDotaJass = false;

            Splash = splash;

            MapOK = false;
        }
        public MapLoadSettings(string filename, bool openListsOnFinish, bool refreshItemCombiningDataLookups, SplashScreen splash)
        {
            Filename = filename;
            UIOpenListsOnFinish = openListsOnFinish;

            SupressBaseLookupsRefresh = false;
            RefreshItemCombiningDataLookups = refreshItemCombiningDataLookups;
            SupressErrorMessages = false;
            DisableNonDotaJass = false;

            Splash = splash;

            MapOK = false;
        }

        public void SplashShowText(string text)
        {
            if (Splash != null) Splash.ShowText(text);
        }
        public void SplashProgressAdd(int amount)
        {
            if (Splash != null) Splash.ProgressAdd(amount);
        }
    }
    public delegate void MapLoadEventHandler(MapLoadSettings s);
    public class DHMAIN
    {
        public static event MapLoadEventHandler MapLoading;
        public static event MapLoadEventHandler MapLoaded;
        public static event MethodInvoker MapJassCompiled;
        public static event MethodInvoker MpqDatabaseClearing;
        public static event MethodInvoker War3MpqLoading;

        public static bool LoadMap(string filename, Form splashOwner)
        {
            return LoadMap(new MapLoadSettings(filename, splashOwner));
        }
        public static bool LoadMap(MapLoadSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Filename))
            {
                settings.Filename = LoadMapDialog();
                if (settings.Filename == null) return false;
            }

            if (MapLoading != null) MapLoading(settings);

            if (settings.Splash != null)
            {
                settings.Splash.Show();
                settings.Splash.ShowText("reading map file...");
            }

            DHTIMER.StartCount();

            settings.MapOK = LoadMpq(settings); //settings.SplashProgressAdd(5);//10

            if (MapLoaded != null) MapLoaded(settings);

            if (settings.RefreshItemCombiningDataLookups)
            {
                DHLOOKUP.CollectItemCombiningData(settings.Splash, settings.Splash != null, false);
                DHLOOKUP.DetectStackableItems();
            }

            settings.SplashProgressAdd(5);//10

            DHTIMER.PrintEndResetCount();

            if (settings.Splash != null)
                settings.Splash.Close();           

            return true;
        }

        static string LoadMapDialog()
        {
            UIDialogs.OpenFileDialogBoxWrapper openFileDialog = UIDialogs.OpenFileDialogBoxWrapper.Default;

            string path = DHCFG.Items["Path"].GetStringValue("MapLoad");
            if (string.IsNullOrEmpty(path)) openFileDialog.InitialDirectory = Application.StartupPath;
            else openFileDialog.InitialDirectory = path;

            openFileDialog.FileName = "";
            openFileDialog.Filter = "w3x files|*.w3x|All files|*.*";

            switch (openFileDialog.ShowDialog())
            {
                case DialogResult.OK:
                    {
                        return openFileDialog.FileName;
                    }
            }

            return null;
        }

        static bool LoadMpq(MapLoadSettings settings)
        {
            ResetMpqDatabase(); // reset everything that was loaded with previous map

            Current.map = DHRC.Default.OpenArchive(settings.Filename, 11);

            // heroes

            settings.SplashShowText("loading units...");
            DHMpqDatabase.LoadUnits(); settings.SplashProgressAdd(8);

            // items

            settings.SplashShowText("loading items...");
            DHMpqDatabase.LoadItems(); settings.SplashProgressAdd(8);

            // abilites

            settings.SplashShowText("loading abilities...");
            DHMpqDatabase.LoadAbilities(settings.Splash);

            // upgrades

            settings.SplashShowText("loading upgrades...");
            DHMpqDatabase.LoadUpgrades(settings.Splash);

            DHLOOKUP.RefreshHotEntries();

            // jass

            if (settings.DisableNonDotaJass == false
                || DHHELPER.IsDotaMap(Current.map))
            {
                try
                {
                    settings.SplashShowText("compiling jass...");
                    DHJass.ReadCustom(DHRC.Default.GetFile(Script.Custom2).GetStream(), true);
                    DHJass.ReadCustom(DHRC.Default.GetFile(Script.Custom).GetStream(), true);/* false);*/ settings.SplashProgressAdd(14);

                    if (MapJassCompiled != null) MapJassCompiled();

                    settings.SplashShowText("executing jass...");
                    DHJass.Config();                    
                    DHJass.Go(); settings.SplashProgressAdd(15);
                }
                catch (Exception e)
                {
                    if (!settings.SupressErrorMessages)
                        MessageBox.Show("Failed to load the jass script. This heavely reduces the functionality of DotaHIT, but you will still be able to use simple features like DataDump."
                        + Environment.NewLine + "Error Message: " + e.Message);
                    return false;
                }
            }
            else
            {
                Current.player = new player(0, false) { name = null };
            }

            return true;
        }

        public static void ResetMpqDatabase()
        {
            if (Current.map != null)
            {
                DHJass.Reset();

                DHRC.Default.CloseArchive(Current.map);                
                Current.player = null;
                Current.unit = null;
                DHRC.Default.Reset();                
                GC.Collect();
            }
        }
        public static void ClearMpqDatabase()
        {
            if (MpqDatabaseClearing != null) MpqDatabaseClearing();

            DHJass.Clear();

            Current.map = null;
            Current.player = null;
            Current.unit = null;
            DHRC.Default.Clear();
            DHRC.Base.Clear();                        
            GC.Collect();

            Current.sessionHasWar3DB = false;
        }

        public static void LoadWar3Mpq()
        {
            if (War3MpqLoading != null) War3MpqLoading();

            //DHMpqDatabase.UnitSlkDatabase.Clear();
            //DHMpqDatabase.ItemSlkDatabase.Clear();

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
                        if (UIDialogs.FolderBrowserDialogBoxWrapper.Default.ShowDialog(Current.mainForm) == DialogResult.OK)
                            w3path = UIDialogs.FolderBrowserDialogBoxWrapper.Default.SelectedPath;
                        break;

                    case DialogResult.No:
                        return;
                }

            string[] files;
            while ((files = Directory.GetFiles(Path.GetFullPath(w3path), "*.mpq")).Length == 0)
                switch (MessageBox.Show(error_no_mpqs, UIStrings.Warning, MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        if (UIDialogs.FolderBrowserDialogBoxWrapper.Default.ShowDialog(Current.mainForm) == DialogResult.OK)
                            w3path = UIDialogs.FolderBrowserDialogBoxWrapper.Default.SelectedPath;
                        break;

                    case DialogResult.No:
                        return;
                }

            DHCFG.Items["Path"]["War3"] = w3path; // save the correct war3 path

            foreach (string file in files)
            {
                int index = MpqArchive.List.IndexOf(Path.GetFileName(file.ToLower()));
                if (index != -1)
                    DHRC.Base.OpenArchive(file, (uint)index);
                else
                    DHRC.Base.OpenArchive(file, 0);//10);
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
        }
    }

    public class DHRC
    {
        public static Dictionary<string, Regex> RegexCache = new Dictionary<string, Regex>();

        public static readonly DHRC Base = new DHRC(false);
        public static readonly DHRC Default = new DHRC();

        static DHRC()
        {
            DHCFG.WakeUp();
        }

        public static void WakeUpKnow()
        {
            FieldsKnowledge.WakeUp();
            RecordsKnowledge.WakeUp();
            DbAbilitiesKnowledge.WakeUp();
        }

        public DHMpqArchiveCollection mpqCollection = new DHMpqArchiveCollection();
        public Dictionary<string, Bitmap> dhImages = new Dictionary<string, Bitmap>();
        public Dictionary<string, DHMpqFile> dhFiles = new Dictionary<string, DHMpqFile>();
        public Dictionary<string, HabPropertiesCollection> dhMergedFiles = new Dictionary<string, HabPropertiesCollection>();

        protected bool useBase = true;

        public DHRC()
        {            
        }
        public DHRC(bool useBase)
        {
            this.useBase = useBase;
        }        

        /// <summary>
        /// clears all cached files and closes all opened mpq archives
        /// </summary>
        public void Clear()
        {
            foreach (DHMpqArchive mpq in mpqCollection) mpq.Close();
            mpqCollection.Clear();

            Reset();
        }
        /// <summary>
        /// clears all cached files that were read from mpq archives
        /// </summary>
        public void Reset()
        {
            dhImages.Clear();
            dhFiles.Clear();
            dhMergedFiles.Clear();
        }

        public DHMpqArchive OpenArchive(string filename)
        {
            DHMpqArchive mpq = mpqCollection[filename];

            if (mpq == null)
            {
                mpq = new DHMpqArchive(filename);
                if (!mpq.IsNull) mpqCollection.Add(mpq);
            }

            return mpq;
        }
        public DHMpqArchive OpenArchive(string filename, uint priority)
        {
            DHMpqArchive mpq = mpqCollection[filename];

            if (mpq == null)
            {
                mpq = new DHMpqArchive(filename, priority);
                if (!mpq.IsNull) mpqCollection.Add(mpq);
            }

            return mpq;
        }
        public void CloseArchive(string filename)
        {
            DHMpqArchive mpq = mpqCollection[filename];
            if (mpq != null)
            {
                mpq.Close();
                mpqCollection.Remove(mpq);
            }
        }
        public void CloseArchive(DHMpqArchive mpq)
        {
            if (mpq != null)
            {
                mpq.Close();
                mpqCollection.Remove(mpq);
            }
        }

        public Bitmap GetTgaImage(string filename)
        {
            DHMpqFile file = GetFile(filename);

            if (!file.IsNull)
                return Tga.TgaLoader.BitmapFromTga(file.GetStream());
            else
                return null;
        }
        public Bitmap GetImage(string filename)
        {
            return GetImage(filename, PixelFormat.DontCare);
        }
        public Bitmap GetImage(string filename, PixelFormat pf)
        {
            if (string.IsNullOrEmpty(filename))
                return null;

            Bitmap image;
            if (dhImages.TryGetValue(filename, out image))
                return image;            

            DHMpqFile file = GetFile(filename, false);

            if (file.IsNull == false)
            {
                image = BlpLibWrapper.BlpToBitmap(file.GetStream(), pf);
                dhImages.Add(filename, image);
                return image;
            }
            else
                switch (Path.GetExtension(filename))
                {
                    case ".tga":
                    case "":
                        image = GetImage(Path.ChangeExtension(filename, ".blp"), pf);
                        if (image != null) return image;
                        break;
                }

            if (useBase) 
                image = Base.GetImage(filename, pf);

            dhImages.Add(filename, image);
            return image;
        }

        public DHMpqFile GetImageFile(string filename)
        {
            DHMpqFile file = GetFile(filename, false);
            
            if (file.IsNull == false)
                return file;
            else
                switch (Path.GetExtension(filename))
                {
                    case ".tga":
                    case "":
                        return GetImageFile(Path.ChangeExtension(filename, ".blp"));
                }

            return file;
        }

        public DHMpqFile GetFile(string filename)
        {
            return GetFile(filename, true);
        }
        public DHMpqFile GetFile(string filename, bool canCache)
        {
            // check cache

            DHMpqFile file;
            if (dhFiles.TryGetValue(filename, out file))
                return file;           

            // now check mpq collection

            MpqReader.MpqHash hash;
            foreach (DHMpqArchive mpq in mpqCollection)
                if ((hash = mpq.MpqHandle.GetHashEntry(filename)).IsValid)
                {
                    file = new DHMpqFile(mpq, hash, filename);

                    if (canCache) 
                        dhFiles.Add(filename, file);

                    return file;
                }

            if (useBase)
                file = Base.GetFile(filename, canCache);
            else
                // create empty file
                file = new DHMpqFile(null, filename);

            if (canCache) 
                dhFiles.Add(filename, file);

            return file;
        }
        public List<DHMpqFile> GetFiles(string filename)
        {
            List<DHMpqFile> files = new List<DHMpqFile>();

            List<DHMpqArchive> fullMpqCollection = null;

            if (useBase)
            {
                fullMpqCollection = new List<DHMpqArchive>(mpqCollection);
                fullMpqCollection.AddRange(Base.mpqCollection);
            }
            else
                fullMpqCollection = mpqCollection;

            foreach (DHMpqArchive mpq in fullMpqCollection)
            {
                DHMpqFile file = new DHMpqFile(mpq, filename);
                if (file.IsNull == false)
                    files.Add(file);
            }
            return files;
        }
        /// <summary>
        /// gathers all instances of this file in mpq-archives
        /// and then stores all data read from those files in one HabPropertiesCollection
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public HabPropertiesCollection GetMergedFiles(string filename)
        {
            HabPropertiesCollection hpc;
            if (dhMergedFiles.TryGetValue(filename, out hpc))
                return hpc;

            List<DHMpqFile> files = DHRC.Default.GetFiles(filename);
            hpc = new HabPropertiesCollection();
            foreach (DHMpqFile file in files)
            {
                HabPropertiesCollection file_hpc = file.read_hpc();
                hpc.Merge(file_hpc);
            }

            dhMergedFiles.Add(filename, hpc);
            return hpc;
        }

        public static Regex GetRegex(string pattern, bool caseSensitive, bool compiled)
        {
            Regex rg;
            if (!RegexCache.TryGetValue(pattern, out rg))
            {
                rg = new Regex(pattern, (caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase) |
                                        (compiled ? RegexOptions.Compiled : RegexOptions.None));
                RegexCache.Add(pattern, rg);
            }
            return rg;
        }
        public static Regex GetRegex(string pattern)
        {
            Regex rg;
            if (!RegexCache.TryGetValue(pattern, out rg))
            {
                rg = new Regex(pattern);
                RegexCache.Add(pattern, rg);
            }
            return rg;
        }
    }

    public class DHMULTIMEDIA
    {
        static HabPropertiesCollection hpcSoundInfo = null;
        static SoundPlayerAsync soundPlayer = new SoundPlayerAsync();
        static int lastRandomSound = -1;

        static DHMULTIMEDIA()
        {
            try
            {
                hpcSoundInfo = DHRC.Default.GetMergedFiles("UI\\SoundInfo\\UnitAckSounds.slk");
                hpcSoundInfo.Merge(DHRC.Default.GetMergedFiles("UI\\SoundInfo\\AnimSounds.slk"));
            }
            catch
            {
                Console.WriteLine("Error: Couldn't initialize DHMULTIMEDIA");
            }
        }
        public static void PlayUnitSound(string unitID, UnitAckSounds sound)
        {
            if (!Current.sessionAllowsSounds) return;

            HabProperties hpsUnitData;
            if (DHMpqDatabase.UnitSlkDatabase["UnitUI"].TryGetValue(unitID, out hpsUnitData))
            {
                string unitSound = hpsUnitData.GetValue("unitSound") as string;
                if (string.IsNullOrEmpty(unitSound))
                    return;
                try
                {
                    HabProperties hps = hpcSoundInfo[unitSound + sound.ToString()];
                    if (hps != null)
                    {
                        DBSTRINGCOLLECTION filenames = new DBSTRINGCOLLECTION(hps.GetValue("FileNames") as string);

                        string soundFile = Path.Combine(hps.GetValue("DirectoryBase") as string, filenames[0]);

                        DHMpqFile waveFile = DHRC.Default.GetFile(soundFile);

                        soundPlayer.PlaySound(waveFile.GetStream());
                    }
                }
                catch { }
            }
        }
        public static void PlayUnitSound(string unitID, UnitAckSounds sound, bool random)
        {
            if (!Current.sessionAllowsSounds) return;

            HabProperties hpsUnitData;
            if (DHMpqDatabase.UnitSlkDatabase["UnitUI"].TryGetValue(unitID, out hpsUnitData))
            {
                string unitSound = hpsUnitData.GetValue("unitSound") as string;
                if (string.IsNullOrEmpty(unitSound))
                    return;
                try
                {
                    HabProperties hps = hpcSoundInfo[unitSound + sound.ToString()];
                    if (hps != null)
                    {
                        DBSTRINGCOLLECTION filenames = new DBSTRINGCOLLECTION(hps.GetValue("FileNames") as string);

                        int index = 0;
                        if (random)
                        {
                            Random r = new Random();

                            if (filenames.Size > 1)
                                while ((index = r.Next(filenames.Size)) == lastRandomSound) ;
                            else
                                index = r.Next(filenames.Size);

                            lastRandomSound = index;
                        }

                        string soundFile = Path.Combine(hps.GetValue("DirectoryBase") as string, filenames[index]);

                        DHMpqFile waveFile = DHRC.Default.GetFile(soundFile);

                        soundPlayer.PlaySound(waveFile.GetStream());
                    }
                }
                catch
                {
                }
            }
        }
        public static void PlayUnitSound(string unitID, AnimSounds sound)
        {
            if (!Current.sessionAllowsSounds) return;

            HabProperties hpsUnitData;
            if (DHMpqDatabase.UnitSlkDatabase["UnitUI"].TryGetValue(unitID, out hpsUnitData))
            {
                string file = hpsUnitData.GetValue("file") as string;
                if (string.IsNullOrEmpty(file))
                    return;
                try
                {
                    HabProperties hps = hpcSoundInfo[Path.GetFileName(file) + sound.ToString()];
                    if (hps != null)
                    {
                        DBSTRINGCOLLECTION filenames = new DBSTRINGCOLLECTION(hps.GetValue("FileNames") as string);

                        string soundFile = Path.Combine(hps.GetValue("DirectoryBase") as string, filenames[0]);

                        DHMpqFile waveFile = DHRC.Default.GetFile(soundFile);

                        soundPlayer.PlaySound(waveFile.GetStream());
                    }
                }
                catch { }
            }
        }
    }

    public class DHCFG
    {
        static string CfgFileName = "dotahit.cfg";

        static HabPropertiesCollection hpcConfig = new HabPropertiesCollection();
        static DHCFG()
        {
            CfgFileName = Application.StartupPath + "\\" + CfgFileName;
            hpcConfig.ReadFromFile(CfgFileName);
        }

        public static string FileName
        {
            get
            {
                return DHCFG.CfgFileName;
            }
        }

        public static void WakeUp() { }

        public static DHCFG Items = new DHCFG();

        public static void Save()
        {
            DHFormatter.RefreshFormatForCurrentThread();

            hpcConfig.SaveToFile(CfgFileName);
        }

        public HabProperties this[string name]
        {
            get
            {
                HabProperties hps;
                if (hpcConfig.TryGetValue(name, out hps))
                    return hps;
                else
                {
                    hps = new HabProperties();
                    hpcConfig.Add(name, hps);
                    return hps;
                }
            }
            set
            {
                hpcConfig[name] = value;
            }
        }

        ~DHCFG()
        {
            Save();
        }
    }

    public class DHSTRINGS
    {
        static HabProperties hpsTriggerStrings = null;
        static HabProperties LoadTriggerStrings()
        {
            HabProperties hps = new HabProperties();

            DHMpqFile file = DHRC.Default.GetFile(MpqPath.Editor.TriggerStrings);
            MemoryStream ms = file.GetStream();

            string line;
            using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    // STRING 3008
                    if (line.StartsWith("STRING"))
                    {
                        string id = line.Substring(7);

                        // {
                        while (sr.ReadLine() != "{") ;

                        string value = "";

                        // ...
                        // ...
                        // }                                
                        while ((line = sr.ReadLine()) != "}")
                            value += (value.Length > 0) ? (Environment.NewLine + line) : line;

                        // key = 'TRIGSTR_3008'
                        hps.Add("TRIGSTR_" + id, value);
                    }
                }
            }

            return hps;
        }
        public static string GetTriggerString(string id)
        {
            if (hpsTriggerStrings == null)
                hpsTriggerStrings = LoadTriggerStrings();

            object value;
            if (hpsTriggerStrings.TryGetValue(id, out value))
                return value as string;

            return id;
        }
        public static string GetWEAbilityStatsString(string fieldname)
        {
            HabPropertiesCollection hpcMetaData = DHMpqDatabase.AbilitySlkDatabase["MetaData"];
            HabProperties hpsWEStrings = DHMpqDatabase.EditorDatabase["Data"]["WorldEditStrings"];

            foreach (HabProperties hpsMetaData in hpcMetaData)
                if (hpsMetaData.GetStringValue("field") == fieldname)
                {
                    string westring = hpsMetaData.GetStringValue("displayName");
                    return DHMpqDatabase.EditorDatabase["Data"]["WorldEditStrings", westring] + "";
                }

            return null;
        }
        public static string GetWEAbilityDataString(string abilityID, string datafield)
        {
            HabPropertiesCollection hpcMetaData = DHMpqDatabase.AbilitySlkDatabase["MetaData"];

            string keyName = abilityID + "," + datafield;

            string metaName;
            if (DHLOOKUP.dcAbilityDataMetaNames.TryGetValue(keyName, out metaName))
            {
                HabProperties hpsMetaData;
                if (!hpcMetaData.TryGetValue(metaName, out hpsMetaData)) return datafield;

                string westring = hpsMetaData.GetStringValue("displayName");
                return DHMpqDatabase.EditorDatabase["Data"]["WorldEditStrings", westring] + "";
            }
            else
                return datafield;
        }
        public static string GetWEAbilityDataString(string abilityID, string datafield, bool tryAliasOnFail)
        {
            HabPropertiesCollection hpcMetaData = DHMpqDatabase.AbilitySlkDatabase["MetaData"];

            string keyName = null;
            bool triedID = false;
            bool triedAlias = false;

            do
            {
                keyName = abilityID + "," + datafield;

                string metaName;
                if (DHLOOKUP.dcAbilityDataMetaNames.TryGetValue(keyName, out metaName))
                {
                    HabProperties hpsMetaData;
                    if (!hpcMetaData.TryGetValue(metaName, out hpsMetaData)) return datafield;

                    string westring = hpsMetaData.GetStringValue("displayName");
                    return DHMpqDatabase.EditorDatabase["Data"]["WorldEditStrings", westring] + "";
                }
                else
                    if (triedID == false && tryAliasOnFail)
                    {
                        triedID = true;

                        foreach (HabProperties hps in DHLOOKUP.hpcAbilityData)
                            if (hps["code"] as string == abilityID)
                            {
                                abilityID = hps.name;
                                break;
                            }
                    }
                    else
                        triedAlias = true;
            }
            while ((triedID && triedAlias) == false);

            return datafield;
        }
        public static string GetDisabledTextureString(string enabledTextureString)
        {
            //    ReplaceableTextures\CommandButtons\BTNDemolish.blp
            // -> ReplaceableTextures\CommandButtonsDisabled\DISBTNDemolish.blp

            //    ReplaceableTextures\PassiveButtons\PASSBTNMagicImmunity.blp
            // -> ReplaceableTextures\CommandButtonsDisabled\DISBTNMagicImmunity.blp

            string directory = Path.GetDirectoryName(Path.GetDirectoryName(enabledTextureString));
            string filename = Path.GetFileName(enabledTextureString);

            return directory + "\\" + "CommandButtonsDisabled" + "\\" + "DIS" + filename.Replace("PASBTN", "BTN");
        }
        public static string GetUntaggedString(string taggedString)
        {
            int index;
            while ((index = taggedString.IndexOf("|c")) != -1)
                taggedString = taggedString.Remove(index, 2 + 8);

            return taggedString.Replace("|r", "");
        }
    }

    public class DHFILE
    {
        public static string[] KnownFileTypes = new string[] { ".w3x", ".dhb", ".w3g" };

        public static void AssociateFileTypes(List<string> fileTypes)
        {
            try
            {
                foreach (string filetype in KnownFileTypes)
                {
                    FileExtension ext = new FileExtension(filetype);

                    if (fileTypes.Contains(filetype))
                    {
                        ext.Starter.Shell.Open.SetApplication(Application.ExecutablePath);

                        if (filetype == ".dhb")
                        {
                            string iconName = Application.LocalUserAppDataPath + "\\" + "herobuild.ico";
                            if (!File.Exists(iconName))
                            {
                                Stream stream = File.Create(iconName);
                                Properties.Resources.HeroBuild.Save(stream);
                                stream.Close();
                            }
                            ext.Starter.IconFileName = iconName;
                        }
                    }
                    else
                    {
                        string filename = ext.Starter.Shell.Open.Command + "";
                        filename = Path.GetFileNameWithoutExtension(filename.Split('\"')[0]);

                        if (filename == Path.GetFileNameWithoutExtension(Application.ExecutablePath))
                        {
                            ext.Starter.Shell.Writable = true;
                            ext.Starter.Shell.RegistryKey.DeleteSubKeyTree("open");
                        }

                        if (filetype == ".dhb")
                        {
                            string iconName = Application.LocalUserAppDataPath + "\\" + "herobuild.ico";
                            if (File.Exists(iconName)) File.Delete(iconName);
                            ext.Starter.IconFileName = "";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to register file type associations.\n The error message is: " + e.Message);
            }
        }
        public static void RegisterContextMenus(List<string> fileTypes)
        {
            try
            {
                foreach (string filetype in KnownFileTypes)
                {
                    FileExtension ext = new FileExtension(filetype);

                    if (fileTypes.Contains(filetype))
                        ext.Starter.Shell["Open with DotA H.I.T."].SetApplication(Application.ExecutablePath);
                    else
                    {
                        if (ext.Starter.Shell.RegistryKey != null && new List<string>(ext.Starter.Shell.RegistryKey.GetSubKeyNames()).Contains("Open with DotA H.I.T."))
                        {
                            ext.Starter.Shell.Writable = true;
                            ext.Starter.Shell.RegistryKey.DeleteSubKeyTree("Open with DotA H.I.T.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to register context menus.\n The error message is: " + e.Message);
            }
        }
    }

    public class DHLOOKUP
    {
        public static List<unit> taverns = new List<unit>();
        public static List<unit> shops = new List<unit>();

        public static StringDictionary dcHeroesTaverns = new StringDictionary();
        public static Dictionary<string, string> dcAbilitiesHeroes = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

        // hot entries
        public static HabPropertiesCollection hpcUnitAbilities = null;
        public static HabPropertiesCollection hpcUnitProfiles = null;
        public static HabPropertiesCollection hpcAbilityData = null;
        public static HabPropertiesCollection hpcAbilityProfiles = null;
        public static HabPropertiesCollection hpcItemProfiles = null;
        public static HabPropertiesCollection hpcItemData = null;
        public static HabPropertiesCollection hpcUpgradeData = null;
        public static HabPropertiesCollection hpcUpgradeProfiles = null;
        public static Dictionary<string, string> dcAbilityDataMetaNames = new Dictionary<string, string>();

        // item combining data        
        public static Dictionary<string, string> MorphingItems = null;
        /// <summary>
        /// key - shop item, value - real item
        /// </summary>
        public static Dictionary<string, string> MorphingShopItems = null;
        public static HabPropertiesCollection hpcComplexItems = null;
        public static HabProperties MutedItems = null;
        public static HabProperties StackableItems = null;

        public static void RefreshHotEntries()
        {
            hpcUnitAbilities = DHMpqDatabase.UnitSlkDatabase["UnitAbilities"];
            hpcUnitProfiles = DHMpqDatabase.UnitSlkDatabase["Profile"];

            hpcAbilityProfiles = DHMpqDatabase.AbilitySlkDatabase["Profile"];

            hpcAbilityProfiles = DHMpqDatabase.AbilitySlkDatabase["Profile"];
            hpcAbilityData = new HabPropertiesCollection(hpcAbilityProfiles.Count);
            hpcAbilityData.Merge(DHMpqDatabase.AbilitySlkDatabase["AbilityData"]);
            hpcAbilityData.Merge(hpcAbilityProfiles);
            hpcAbilityProfiles = hpcAbilityData;

            hpcItemProfiles = DHMpqDatabase.ItemSlkDatabase["Profile"];
            hpcItemData = DHMpqDatabase.ItemSlkDatabase["ItemData"];

            hpcUpgradeProfiles = DHMpqDatabase.UpgradeSlkDatabase["Profile"];
            hpcUpgradeData = new HabPropertiesCollection(hpcUpgradeProfiles.Count);
            hpcUpgradeData.Merge(DHMpqDatabase.UpgradeSlkDatabase["UpgradeData"]);
            hpcUpgradeData.Merge(hpcUpgradeProfiles);
            hpcUpgradeProfiles = hpcUpgradeData;

            RefreshAbilityDataMetaNames();
        }

        public static void RefreshTaverns()
        {
            dcHeroesTaverns.Clear();

            taverns = new List<unit>();

            foreach (player p in player.players)
                foreach (unit u in p.units.Values)
                    if (!u.sellunits.IsNull && !DHHELPER.IsNewVersionItemShop(u))
                        taverns.Add(u);

            foreach (unit tavern in taverns)
            {
                tavern.sellunits.Sort(new UnitSlotComparer());

                foreach (string unitID in tavern.sellunits)
                    if (!dcHeroesTaverns.ContainsKey(unitID))
                        dcHeroesTaverns.Add(unitID, tavern.ID);
            }
        }
        public static void RefreshHeroesAbilities()
        {
            dcAbilitiesHeroes.Clear();

            foreach (string heroID in dcHeroesTaverns.Keys)
            {
                HabProperties hpsHeroAbilities = hpcUnitAbilities[heroID];
                List<string> abilList = hpsHeroAbilities.GetStringListValue("heroAbilList");

                foreach (string abilID in abilList)
                    dcAbilitiesHeroes[abilID] = heroID;
            }
        }

        public static void RefreshShops()
        {
            shops = new List<unit>();

            List<string> shopsItems = new List<string>();

            foreach (player p in player.players)
                foreach (unit u in p.units.Values)
                    if (!u.sellitems.IsNull)
                    {
                        string items = u.sellitems.ToString();
                        if (!shopsItems.Contains(items))
                        {
                            shops.Add(u);
                            shopsItems.Add(items);
                        }
                    }
                    else
                        if (DHHELPER.IsNewVersionItemShop(u))
                        {
                            string items = u.sellunits.ToString();
                            if (!shopsItems.Contains(items))
                            {
                                shops.Add(u);
                                shopsItems.Add(items);
                            }
                        }

            foreach (unit shop in shops)
                SortItemsBySlots(shop);
        }

        private static void RefreshAbilityDataMetaNames()
        {
            HabPropertiesCollection hpcMetaData = DHMpqDatabase.AbilitySlkDatabase["MetaData"];

            object obj;
            foreach (HabProperties hpsMetaData in hpcMetaData)
                if (hpsMetaData.TryGetValue("useSpecific", out obj))
                {
                    string ID = hpsMetaData.name;
                    string field = hpsMetaData.GetStringValue("field");
                    string data = hpsMetaData.GetStringValue("data");

                    string key;

                    if (data[0] != '0')
                        key = field + (char)(data[0] + ('A' - '1'));
                    else
                        key = field;

                    DBSTRINGCOLLECTION sc = new DBSTRINGCOLLECTION(obj as string);

                    foreach (string abilityID in sc)
                    {
                        // add ability's data meta name
                        dcAbilityDataMetaNames[abilityID + "," + key] = ID;

                        // add ability meta name
                        dcAbilityDataMetaNames[abilityID] = sc[0];
                    }
                }
        }

        public static void CollectItemCombiningData()
        {
            CollectItemCombiningData(null, true, true);
        }
        public static void CollectItemCombiningData(SplashScreen splashScreen, bool showSplash, bool addProgress)
        {
            if (MorphingItems != null) return;

            // if no shops exists in current database, then leave
            if (DHLOOKUP.shops.Count == 0)
            {
                MorphingItems = new Dictionary<string, string>(0);
                MorphingShopItems = new Dictionary<string, string>(0);
                return;
            }

            if (splashScreen == null && showSplash)
            {
                splashScreen = new SplashScreen();
                splashScreen.Show();
            }
            if (splashScreen != null) splashScreen.ShowText("Detecting morphing items...");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); sw.Start();

            Current.player.AcceptMessages = false;

            int itemCountMax = DHHELPER.IsNewVersionItemShop(DHLOOKUP.shops[0]) ? 6 : -1; // estimated item count that should be enough for tests

            Dictionary<string, widget> dcItems = new Dictionary<string, widget>(itemCountMax == -1 ? DHLOOKUP.shops.Count * 12 : itemCountMax);
            Dictionary<string, unit> dcItemsShops = new Dictionary<string, unit>(itemCountMax == -1 ? DHLOOKUP.shops.Count * 12 : itemCountMax);

            foreach (unit shop in DHLOOKUP.shops)
            {
                if (DHHELPER.IsNewVersionItemShop(shop))
                {
                    foreach (string unitID in shop.sellunits)
                        if (!dcItems.ContainsKey(unitID))
                        {
                            unit u = new unit(unitID);
                            u.DoSummon = true;
                            u.set_owningPlayer(Current.player);

                            dcItems.Add(unitID, u);
                            dcItemsShops.Add(unitID, shop);
                        }
                }
                else
                    foreach (string itemID in shop.sellitems)
                        if (!dcItems.ContainsKey(itemID))
                            dcItems.Add(itemID, new item(itemID));

                if (itemCountMax != -1 && dcItems.Count >= itemCountMax) break;
            }

            if (addProgress) splashScreen.ShowProgress((double)10, (double)100);

            Current.player.selection.BeginUpdate();
            List<unit> selectionBackup = new List<unit>(Current.player.selection);
            unit test_unit = new unit();
            test_unit.ForceNoRefresh(true);
            test_unit.codeID = "test";
            test_unit.Inventory.init(6, 0); Jass.Native.Functions.UnitInventorySize.ForceReturnValue = 6;
            test_unit.primary = PrimAttrType.Str;
            //Current.unit = test_unit;            

            test_unit.set_owningPlayer(Current.player);
            test_unit.Inventory.EnableHistory = true;

            timer.EnableHistory = true;
            timer.maxTimerIntervalForHistory = -1;

            DBINVENTORY inventory = test_unit.Inventory;

            MorphingItems = new Dictionary<string, string>(DHLOOKUP.shops.Count * 10); // estimated capacity
            MorphingShopItems = new Dictionary<string, string>(DHLOOKUP.shops.Count * 10);

            bool isQuickApproachValid = itemCountMax != -1;
            List<DHJassArray> arrayList = null;
            foreach (widget item in dcItems.Values)
            {
                test_unit.Inventory.History.Clear();

                if (isQuickApproachValid)
                {
                    DHJassExecutor.CaughtReferences.Clear();
                    DHJassExecutor.CatchArrayReference = true;
                }

                if (DHHELPER.IsNewVersionItem(item.codeID))
                {
                    unit shop = dcItemsShops[item.codeID];
                    test_unit.x = shop.x;
                    test_unit.y = shop.y;

                    timer.History.Clear();
                    timer.maxTimerIntervalForHistory = 1;
                    test_unit.OnSell(shop, item as unit);
                    timer.maxTimerIntervalForHistory = -1;

                    if (test_unit.Inventory.History.Count > 0 && test_unit.Inventory.History[0].powerup)
                        foreach (timer t in timer.History)
                            t.forceCallback(); // to pass control to item handling script thread                      
                }
                else
                {
                    test_unit.OnSellItem(item as item, test_unit);
                    inventory.put_item(item as item);
                }

                if (isQuickApproachValid)
                {
                    DHJassExecutor.CatchArrayReference = false;
                    arrayList = new List<DHJassArray>(DHJassExecutor.CaughtReferences.Count);
                    foreach (string arrayName in DHJassExecutor.CaughtReferences)
                    {
                        DHJassValue array;
                        if (DHJassExecutor.Globals.TryGetValue(arrayName, out array) && array is DHJassArray)
                            arrayList.Add(array as DHJassArray);
                    }

                    if (arrayList.Count > 1)
                    {
                        DHJassArray shopItemArray = null;
                        DHJassArray realItemArray = null;
                        foreach (DHJassArray array in arrayList)
                        {
                            if (shopItemArray == null)
                            {
                                foreach (DHJassValue value in array.Array.Values)
                                    if (value is DHJassInt)
                                    {
                                        if (DHLOOKUP.hpcUnitProfiles.ContainsKey((value as DHJassInt).IDValue))
                                        {
                                            shopItemArray = array;
                                            break;
                                        }
                                        else
                                            if (value.IntValue != 0) // if this number is not an IDValue, then this is the wrong array
                                                break;
                                    }
                                    else
                                        break;
                            }
                            else
                                if (realItemArray == null)
                                {
                                    foreach (DHJassValue value in array.Array.Values)
                                        if (value is DHJassInt)
                                        {
                                            if (DHLOOKUP.hpcItemProfiles.ContainsKey((value as DHJassInt).IDValue))
                                            {
                                                realItemArray = array;
                                                break;
                                            }
                                            else
                                                if (value.IntValue != 0) // if this number is not an IDValue, then this is the wrong array
                                                    break;
                                        }
                                        else
                                            break;
                                }
                                else
                                    break;
                        }

                        if (shopItemArray != null && realItemArray != null)
                        {
                            MorphingItems.Clear();
                            MorphingShopItems.Clear();

                            foreach (KeyValuePair<int, DHJassValue> kvp in shopItemArray.Array)
                            {
                                DHJassValue value;
                                if (kvp.Value.IntValue != 0 && realItemArray.Array.TryGetValue(kvp.Key, out value) && value.IntValue != 0)
                                {
                                    string shopItemCodeID = (kvp.Value as DHJassInt).IDValue;
                                    string resultItemCodeID = (value as DHJassInt).IDValue;

                                    if (resultItemCodeID != shopItemCodeID)
                                    {
                                        if (!MorphingItems.ContainsKey(resultItemCodeID))
                                            MorphingItems.Add(resultItemCodeID, shopItemCodeID);

                                        if (!MorphingShopItems.ContainsKey(shopItemCodeID))
                                            MorphingShopItems.Add(shopItemCodeID, resultItemCodeID);
                                    }
                                }
                            }

                            inventory[0].drop_item(false,true);

                            break;
                        }
                    }
                }

                item result = inventory[0].Item;
                if (result != null)
                {
                    if (result.codeID != item.codeID)
                    {
                        if (!MorphingItems.ContainsKey(result.codeID))
                            MorphingItems.Add(result.codeID, item.codeID);

                        if (!MorphingShopItems.ContainsKey(item.codeID))
                            MorphingShopItems.Add(item.codeID, result.codeID);
                    }
                    inventory[0].drop_item(false, true);
                }
            }

            if (addProgress) splashScreen.ShowProgress((double)60, (double)100);

            test_unit.Inventory.EnableHistory = false;

            // find arrays with item combining data         

            DHJassExecutor.CaughtReferences.Clear();
            DHJassExecutor.CatchArrayReference = true;

            inventory.DisableRefresh();

            timer.History.Clear();
            timer.maxTimerIntervalForHistory = 1;
            int itemCount = 1;
            foreach (string morphingItemID in MorphingItems.Keys)
            {
                item testItem = new item(morphingItemID); testItem.set_owningPlayer(Current.player);
                test_unit.Inventory.put_item(testItem);
                if (itemCount++ >= 6) break;
            }
            timer.maxTimerIntervalForHistory = -1;

            foreach (timer t in timer.History)
                t.forceCallback(); // to pass control to item handling script thread

            for (int i = 0; i < itemCount - 1; i++)
                inventory[i].drop_item(false, true);

            if (addProgress) splashScreen.ShowProgress((double)80, (double)100);

            List<string> arrays = DHJassExecutor.CaughtReferences;
            arrayList = new List<DHJassArray>(arrays.Count);
            foreach (string arrayName in arrays)
            {
                DHJassValue array;
                if (DHJassExecutor.Globals.TryGetValue(arrayName, out array) && array is DHJassArray && (array as DHJassArray).Array.Count > 0)
                    arrayList.Add(array as DHJassArray);
            }

            try
            {
                switch (arrayList.Count)
                {
                    default:                    
                        // [0] or [1] - itemID lookup by index
                        // [1] or [0] - dummy itemIDs lookup by index for items in [0]
                        // [2] - muted itemIDs lookup by index for items in [0]
                        // [3]--[7] component item indexes
                        // [8] - result item index (to be found in this codeblock)

                        DHJassArray idLookup = arrayList[0];

                        // check if its dummy
                        if (DHLOOKUP.hpcItemData.GetStringValue((idLookup[5] as DHJassInt).IDValue, "abilList").Length < 4)
                            idLookup = arrayList[1];

                        List<item> testItemList = new List<item>(6);
                        int itemKey = 1; // some sample item key
                        DHJassValue value;
                        int first = 3;
                        int last = 7;                        

                        if (!arrayList[first].Array.TryGetValue(itemKey, out value))
                        {
                            first++;
                            last++;
                        }

                        last = Math.Min(last, arrayList.Count-1);

                        for (int i = first; i <= last; i++)
                        {
                            DHJassArray componentArray = arrayList[i];
                            if (!componentArray.Array.TryGetValue(itemKey, out value))
                                break;

                            DHJassValue jvItemID;
                            if (!idLookup.Array.TryGetValue(value.IntValue, out jvItemID))
                                throw new Exception("idLookup error");

                            testItemList.Add(new item((jvItemID as DHJassInt).IDValue));
                        }

                        if (testItemList.Count == 0)
                            throw new Exception("no item components found");

                        int caughtReferencesBeforeTest = DHJassExecutor.CaughtReferences.Count;

                        timer.History.Clear();
                        timer.maxTimerIntervalForHistory = 1;
                        foreach (item testItem in testItemList)
                        {
                            testItem.set_owningPlayer(Current.player);
                            test_unit.Inventory.put_item(testItem);
                        }
                        timer.maxTimerIntervalForHistory = -1;

                        foreach (timer t in timer.History)
                            t.forceCallback(); // to pass control to item handling script thread                        
                        
                        DHJassValue resultArray;
                        switch (DHJassExecutor.CaughtReferences.Count - caughtReferencesBeforeTest)
                        {
                            // if result array was found
                            case 1:
                                if (DHJassExecutor.Globals.TryGetValue(DHJassExecutor.CaughtReferences[DHJassExecutor.CaughtReferences.Count - 1], out resultArray) && resultArray is DHJassArray)
                                    arrayList.Add(resultArray as DHJassArray);
                                else
                                    throw new Exception("result array not found");
                                break;

                            // if result array was already in the list
                            case 0:
                                resultArray = arrayList[arrayList.Count - 1];
                                if (last == arrayList.Count - 1) last--;
                                break;

                            default:
                                throw new Exception("result array not found");                                
                        }

                        if ((resultArray as DHJassArray).Array.TryGetValue(itemKey, out value) && (idLookup.Array[value.IntValue] as DHJassInt).IDValue == inventory[0].Item.codeID)
                        {
                            // all arrays found
                            ExtractComplexItems(arrayList,
                                idLookup == arrayList[0] ? 0 : 1,
                                2, first, last, arrayList.Count - 1);
                        }    
                        break;                    

                    case 6:
                        testItemList = new List<item>(6);
                        itemKey = 1; // some sample item key
                        for (int i = 0; i <= 3; i++)
                        {
                            DHJassArray componentArray = arrayList[i];
                            if (componentArray.Array.TryGetValue(itemKey, out value) && value.IntValue != 0)
                                testItemList.Add(new item((value as DHJassInt).IDValue));
                        }

                        if (testItemList.Count == 0)
                            throw new Exception("no item components found");

                        timer.History.Clear();
                        timer.maxTimerIntervalForHistory = 1;
                        foreach (item testItem in testItemList)
                        {
                            testItem.set_owningPlayer(Current.player);
                            test_unit.Inventory.put_item(testItem);
                        }
                        timer.maxTimerIntervalForHistory = -1;

                        foreach (timer t in timer.History)
                            t.forceCallback(); // to pass control to item handling script thread

                        DHJassArray resultItemArray = arrayList[4];

                        if (resultItemArray.Array.TryGetValue(itemKey, out value) && (value as DHJassInt).IDValue == inventory[0].Item.codeID)
                        {
                            // all arrays found
                            ExtractComplexItems(arrayList, -1, -1, 0, 3, 4);
                        }
                        break;
                }
            }
            catch
            {
            }

            if (addProgress) splashScreen.ShowProgress((double)99, (double)100);

            Jass.Native.Functions.UnitInventorySize.ForceReturnValue = -1;
            DHJassExecutor.CatchArrayReference = false;
            timer.EnableHistory = false;

            foreach (widget w in dcItems.Values)
                w.destroy();

            test_unit.destroy();

            Current.player.selection.Clear();
            Current.player.selection.AddRange(selectionBackup);
            Current.player.selection.EndUpdate();

            sw.Stop();
            if (splashScreen != null) splashScreen.Close();

            Current.player.AcceptMessages = true;

            Console.WriteLine("CollectItemCombiningData: " + sw.ElapsedMilliseconds);
        }
        public static void ResetItemCombiningData()
        {
            MorphingItems = null;
            MorphingShopItems = null;
            hpcComplexItems = null;
            MutedItems = null;
            StackableItems = null;
        }

        private static void ExtractComplexItems(List<DHJassArray> arrayList, int realIndex, int mutedIndex, int componentsFromIndex, int componentsToIndex, int resultIndex)
        {
            hpcComplexItems = new HabPropertiesCollection(arrayList[resultIndex].Array.Count);

            Dictionary<int, DHJassValue> dcItemIdLookup = realIndex != -1 ? arrayList[realIndex].Array : null;
            Dictionary<int, DHJassValue> dcMutedItems = mutedIndex != -1 ? arrayList[mutedIndex].Array : null;

            List<Dictionary<int, DHJassValue>> componentList = new List<Dictionary<int, DHJassValue>>(componentsToIndex - componentsFromIndex + 1);
            for (int i = componentsFromIndex; i <= componentsToIndex; i++)
                componentList.Add(arrayList[i].Array);

            Dictionary<int, DHJassValue> dcResults = arrayList[resultIndex].Array;

            foreach (int key in dcResults.Keys)
            {
                List<string> components = new List<string>(4);

                string itemID = null;
                DHJassValue value;
                foreach (Dictionary<int, DHJassValue> dcComponents in componentList)
                    if (dcComponents.TryGetValue(key, out value) && value.IntValue != 0)
                    {
                        itemID = (dcItemIdLookup != null) ? (dcItemIdLookup[value.IntValue] as DHJassInt).IDValue : (value as DHJassInt).IDValue;
                        components.Add(itemID);
                    }

                itemID = (dcItemIdLookup != null) ? (dcItemIdLookup[dcResults[key].IntValue] as DHJassInt).IDValue : (dcResults[key] as DHJassInt).IDValue;

                HabProperties componentsList;
                if (!hpcComplexItems.TryGetValue(itemID, out componentsList))
                {
                    componentsList = new HabProperties(1);                    
                    hpcComplexItems.AddUnchecked(itemID, componentsList);
                }

                componentsList.Add(componentsList.Count.ToString(), components);
            }

            // muted items
            MutedItems = null;
            if (dcMutedItems != null)
            {
                MutedItems = new HabProperties();
                foreach (KeyValuePair<int, DHJassValue> kvp in dcMutedItems)
                {
                    string original = (dcItemIdLookup[kvp.Key] as DHJassInt).IDValue;
                    string muted = (kvp.Value as DHJassInt).IDValue;

                    if (string.IsNullOrEmpty(muted) || muted[0] == 0)
                        continue;

                    MutedItems[original] = muted;
                }
            }
            
            //hpcComplexItems.AddUnchecked("Muted", MutedItems);
        }

        public static void DetectStackableItems()
        {
            if (StackableItems != null) return;

            if (MorphingItems == null) CollectItemCombiningData();            
            
            StackableItems = new HabProperties();

            if (Current.player == null || Current.player.name == null) 
                return;

            Current.player.AcceptMessages = false;
            Current.player.selection.BeginUpdate();
            List<unit> selectionBackup = new List<unit>(Current.player.selection);

            unit test_unit = new unit();
            test_unit.ForceNoRefresh(true);
            test_unit.codeID = "test";
            test_unit.Inventory.init(6, 0); Jass.Native.Functions.UnitInventorySize.ForceReturnValue = 6;
            test_unit.primary = PrimAttrType.Str;            

            test_unit.set_owningPlayer(Current.player);            

            timer.EnableHistory = true;
            timer.maxTimerIntervalForHistory = -1;

            DBINVENTORY inventory = test_unit.Inventory;

            foreach (string itemID in MorphingItems.Keys)
            {
                // skip items with no charges
                if (DHLOOKUP.hpcItemData.GetIntValue(itemID, "uses", 0) == 0)
                    continue;

                item itemA = new item(itemID);
                itemA.set_owningPlayer(Current.player);

                item itemB = new item(itemID);
                itemB.set_owningPlayer(Current.player);

                timer.History.Clear();
                timer.maxTimerIntervalForHistory = 1;
                inventory.put_item(itemA);
                inventory.put_item(itemB);
                timer.maxTimerIntervalForHistory = -1;

                foreach (timer t in timer.History)
                    t.forceCallback(); // to pass control to item handling script thread 

                // if one item stacked with another item
                if ((itemA.IsDisposed && !inventory[1].IsNull && inventory[1].Item.codeID == itemID) || (itemB.IsDisposed && !inventory[0].IsNull && inventory[0].Item.codeID == itemID))                                    
                    StackableItems.Add(itemID, null);                

                inventory[0].drop_item(false, true);
                inventory[1].drop_item(false, true);

                itemA.destroy();
                itemB.destroy();
            }

            Jass.Native.Functions.UnitInventorySize.ForceReturnValue = -1;
            timer.EnableHistory = false;

            test_unit.destroy();

            Current.player.selection.Clear();
            Current.player.selection.AddRange(selectionBackup);
            Current.player.selection.EndUpdate();
            Current.player.AcceptMessages = true;            
        }

        private static void SortItemsBySlots(unit shop)
        {
            if (DHHELPER.IsNewVersionItemShop(shop))
                SortItemsBySlotsNewVersion(shop);
            else
                SortItemsBySlotsOldVersion(shop);

        }
        private static void SortItemsBySlotsOldVersion(unit shop)
        {
            Dictionary<string, int> ItemSlotPairs = new Dictionary<string, int>();
            for (int i = 0; i < shop.sellitems.Count; i++)
            {
                string itemID = shop.sellitems[i];
                HabProperties hpsItem = DHLOOKUP.hpcItemProfiles[itemID];

                if (hpsItem != null)
                    ItemSlotPairs.Add(itemID, RecordSlotComparer.get_slot(hpsItem.GetValue("Buttonpos") + ""));
                else
                {
                    shop.sellitems.RemoveAt(i);
                    i--;
                }
            }

            List<string> unsortedItems = new List<string>(shop.sellitems);
            List<string> sortedItems = new List<string>();

            for (int i = 0; i < 12 && sortedItems.Count < 12; i++)
            {
                for (int j = 0; j < unsortedItems.Count; j++)
                {
                    string itemID = unsortedItems[j];
                    int slot = ItemSlotPairs[itemID];

                    if (slot == -1 || i >= slot)
                    {
                        sortedItems.Add(itemID);
                        unsortedItems.Remove(itemID);
                        break;
                    }
                }
            }

            shop.sellitems.Clear();
            shop.sellitems.AddRange(sortedItems);
        }
        private static void SortItemsBySlotsNewVersion(unit shop)
        {
            Dictionary<string, int> UnitSlotPairs = new Dictionary<string, int>();
            for (int i = 0; i < shop.sellunits.Count; i++)
            {
                string unitID = shop.sellunits[i];
                HabProperties hpsUnit = DHLOOKUP.hpcUnitProfiles[unitID];

                if (hpsUnit != null)
                    UnitSlotPairs.Add(unitID, RecordSlotComparer.get_slot(hpsUnit.GetValue("Buttonpos") + ""));
                else
                {
                    shop.sellunits.RemoveAt(i);
                    i--;
                }
            }

            List<string> unsortedItems = new List<string>(shop.sellunits);
            List<string> sortedItems = new List<string>();

            for (int i = 0; i < 12 && sortedItems.Count < 12; i++)
            {
                for (int j = 0; j < unsortedItems.Count; j++)
                {
                    string itemID = unsortedItems[j];
                    int slot = UnitSlotPairs[itemID];

                    if (slot == -1 || i >= slot)
                    {
                        sortedItems.Add(itemID);
                        unsortedItems.Remove(itemID);
                        break;
                    }
                }
            }

            if (unsortedItems.Count > 0 && sortedItems.Count < 12)
            {
                // search for empty slot 
                // to put unsorted item to

                int lastSlot = -1;
                for (int i = 0; i < sortedItems.Count; i++)
                {
                    string itemID = sortedItems[i];
                    int slot = UnitSlotPairs[itemID];

                    if (slot == lastSlot || slot == lastSlot + 1)
                        lastSlot = slot;
                    else
                    {
                        sortedItems.Insert(i, unsortedItems[0]);
                        unsortedItems.RemoveAt(0);

                        if (unsortedItems.Count == 0)
                            break;

                        lastSlot++;
                    }
                }
            }

            shop.sellunits.Clear();
            shop.sellunits.AddRange(sortedItems);
        }        
    }

    public class DHHELPER
    {
        public static string GetSimilarAbilityFromHero(string heroId, string abilityId)
        {
            return GetSimilarAbilityFromHero(DHLOOKUP.hpcAbilityProfiles, DHLOOKUP.hpcUnitAbilities, heroId, abilityId);
        }
        public static string GetSimilarAbilityFromHero(HabPropertiesCollection hpcAbilityProfiles, HabPropertiesCollection hpcUnitAbilities, string heroId, string abilityId)
        {
            HabProperties hpsAbility;
            if (!hpcAbilityProfiles.TryGetValue(abilityId, out hpsAbility))
                return null;

            List<string> abilList = hpcUnitAbilities[heroId].GetStringListValue("heroAbilList");
            if (abilList.Count == 0)
                return null;

            string art = hpsAbility.GetStringValue("Art");
            string hotkey = hpsAbility.GetStringValue("Hotkey");
            string buttonPos = hpsAbility.GetStringValue("Buttonpos");

            HabProperties hpsHeroAbility;
            foreach (string abilID in abilList)
            {
                hpsHeroAbility = hpcAbilityProfiles[abilID];
                if (hpsHeroAbility == null) continue;

                if (hpsHeroAbility.GetStringValue("Art") == art
                    && hpsHeroAbility.GetStringValue("Hotkey") == hotkey
                    && hpsHeroAbility.GetStringValue("Buttonpos") == buttonPos)
                    return abilID;
            }

            return null;
        }
        public static int GetRequiredHeroLevelForAbility(string abilityID, int abilityLevel)
        {
            return GetRequiredHeroLevelForAbility(DHLOOKUP.hpcAbilityData[abilityID], abilityLevel);            
        }
        public static int GetRequiredHeroLevelForAbility(HabProperties hpsAbilityData, int abilityLevel)
        {
            int reqLevel = hpsAbilityData.GetIntValue("reqLevel");
            int levelSkip = hpsAbilityData.GetIntValue("levelSkip", 2);

            return reqLevel + (levelSkip * (abilityLevel - 1));
        }

        public static string GetMorphedItemID(string originalCodeID)
        {
            if (DHLOOKUP.MorphingShopItems == null)
                return null;

            string itemID;
            if (!DHLOOKUP.MorphingShopItems.TryGetValue(originalCodeID, out itemID))
                itemID = originalCodeID;

            return itemID;
        }

        public static bool IsNewVersionItemShop(unit u)
        {
            if (u.sellitems.IsNull && !u.sellunits.IsNull)
                return IsNewVersionItem(u.sellunits[0]);

            return false;
        }
        public static bool IsNewVersionItem(string unitID)
        {
            return IsNewVersionItem(DHLOOKUP.hpcUnitAbilities, DHLOOKUP.hpcAbilityData, unitID);
        }
        public static bool IsNewVersionItem(HabPropertiesCollection hpcUnitAbilities, HabPropertiesCollection hpcAbilityData, string unitID)
        {
            HabProperties hpsUnit;
            if (hpcUnitAbilities.TryGetValue(unitID, out hpsUnit))
            {
                string abilList = hpsUnit.GetStringValue("abilList");
                string heroAbilList = hpsUnit.GetStringValue("heroAbilList");

                // if both abilList and heroAbilList do not have any abilities
                // then this unit could be a new-version-item.
                // 6.53 UPDATE: now abilList for new-version-item contains ability based on 'Asph'
                return ((abilList.Length < 4 || (abilList.Length == 4 && hpcAbilityData[abilList, "code"] as string == "Asph")) && heroAbilList.Length < 4);
            }

            // unit without any abilities has a chance to be a new-version-item
            return false;
        }

        public static bool IsDotaMap(DHMpqArchive map)
        {
            bool result = false;
            MpqReader.MpqHash hash;
            if ((hash = map.MpqHandle.GetHashEntry(MpqPath.Editor.TriggerStrings)).IsValid)
            {
                DHMpqFile tsFile = new DHMpqFile(map, hash, MpqPath.Editor.TriggerStrings);
                string text = tsFile.read_text();

                result = text.Contains("getdota.com");
            }

            return result;
        }

        public static bool IsDotaMapResources(DHRC resources)
        {
            string text = resources.GetFile(MpqPath.Editor.TriggerStrings).read_text();
            return text.Contains("getdota.com");
        }
    }

    public class UnitSlotComparer : IComparer<string>
    {
        public UnitSlotComparer()
        {
        }
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<string>.Compare(string x, string y)
        {
            HabProperties hpsX = DHMpqDatabase.UnitSlkDatabase["Profile"][x];//DHJassDatabase.hpcUnits[x];
            HabProperties hpsY = DHMpqDatabase.UnitSlkDatabase["Profile"][y];//DHJassDatabase.hpcUnits[y];

            int slotX = RecordSlotComparer.get_slot(hpsX.GetValue("Buttonpos") + "");
            int slotY = RecordSlotComparer.get_slot(hpsY.GetValue("Buttonpos") + "");

            return slotX - slotY;
        }
    }
    public class NoBorderRenderer : ToolStripSystemRenderer
    {
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {// empty. no borders are drawn        
        }
    }           
}
