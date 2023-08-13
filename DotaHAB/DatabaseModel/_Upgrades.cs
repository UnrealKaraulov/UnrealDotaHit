using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;

namespace DotaHIT.DatabaseModel
{
    using Data;
    using DataTypes;

    namespace Upgrades
    {
        using Core;
        using System.Collections.Generic;        
        using DataTypes;        
        using DotaHIT.Core.Resources;
        using DotaHIT.Jass.Native.Types;
        using System.Collections.Specialized;

        public class DbEffectsKnowledge
        {
            //public static DBABILITY[] abilities = new DBABILITY[] { };
            public static Dictionary<string, DBEFFECT> NameEffectPairs;

            static DbEffectsKnowledge()
            {
                CollectDbEffects();
            }
            public static void WakeUp() { }

            public static void CollectDbEffects()
            {
                Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

                Type[] types = m.FindTypes(new TypeFilter(SearchCriteria), "DotaHIT.DatabaseModel.Upgrades");

                NameEffectPairs = new Dictionary<string, DBEFFECT>(types.Length,
                            (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

                foreach (Type type in types)
                {
                    DBEFFECT e = (DBEFFECT)type.InvokeMember(null,
                                BindingFlags.Public | BindingFlags.DeclaredOnly |
                                BindingFlags.Instance | BindingFlags.CreateInstance,
                                null, null, null);

                    if (!string.IsNullOrEmpty(e.codeID))
                        NameEffectPairs.Add(e.codeID, e);
                }
            }
            private static bool SearchCriteria(Type t, object filter)
            {
                if ((t.Namespace == (string)filter) && t.IsPublic
                    && t.IsSubclassOf(typeof(DBEFFECT))                                        
                    && !t.IsAbstract)
                    return true;
                return false;
            }

            public static DBEFFECT CreateKnownEffect(string codeID, params object[] args)
            {
                DBEFFECT effect;
                if (NameEffectPairs.TryGetValue(codeID, out effect))
                    return effect.GetNew(args);               

                return null;
            }
        }

        public class UPGRADEPROFILE
        {
            protected HabProperties hpsInitialData;
            protected string m_codeID;

            protected List<string> name = null;
            protected DBUPGRADE owner;                     

            protected List<string> tip = null;
            protected List<string> ubertip = null;
            protected List<string> art = null;
            private string hotkey = null;

            protected int slotPriority = 0;            

            protected UPGRADEPROFILE() { }
            internal UPGRADEPROFILE(DBUPGRADE u)
            {
                owner = u;

                hpsInitialData = DHMpqDatabase.UpgradeSlkDatabase["Profile"][u.codeID];

                ////////////////////////
                //  get codeID
                ////////////////////////

                this.m_codeID = hpsInitialData.name;

                ////////////////////////
                //  get name
                ////////////////////////

                this.name = hpsInitialData.GetStringListValue("Name");

                ////////////////////////
                //  get tip
                ////////////////////////                

                tip = hpsInitialData.GetStringListValue("Tip");

                ////////////////////////
                //  get ubertip
                ////////////////////////

                ubertip = hpsInitialData.GetStringListValue("Ubertip");
                                
                ////////////////////////
                //  get hotkey
                ////////////////////////

                this.hotkey = hpsInitialData.GetStringValue("Hotkey");                

                ////////////////////////
                //  get slot priority
                ////////////////////////

                List<string> buttonpos = hpsInitialData.GetStringListValue("Buttonpos");
                slotPriority = RecordSlotComparer.get_slot(Convert.ToInt32(buttonpos[0]), Convert.ToInt32(buttonpos[1]));

                ////////////////////////
                //  get art
                ////////////////////////

                this.art = hpsInitialData.GetStringListValue("Art");
            }

            public string Name
            {
                get
                {
                    try 
                    { 
                        return (name.Count != 0) ? 
                        (Level == 0 ? name[0] : name[Level - 1]) 
                        : ""; 
                    }
                    catch { return ""; }                    
                }                
            }
            public DBUPGRADE Owner
            {
                get
                {
                    return owner;
                }
            }
            public string codeID
            {
                get
                {
                    return m_codeID;
                }
                set
                {
                    m_codeID = value;
                }
            }

            public virtual string Tip
            {
                get
                {
                    try { return (tip.Count != 0) ? tip[Level - 1] : ""; }
                    catch { return ""; }
                }
            }
            public virtual string Ubertip
            {
                get
                {
                    try { return (ubertip.Count != 0) ? ubertip[Level - 1] : ""; }
                    catch { return ""; }
                }
            }            
            public string Hotkey
            {
                get { return hotkey; }
            }            

            public int Level
            {
                get { return owner.Level; }
                set { owner.Level = value; }
            }
            public int Max_level
            {
                get { return owner.Max_level; }
            }

            public object this[string name]
            {
                get
                {
                    object obj;
                    hpsInitialData.TryGetValue(name, out obj);
                    //return hpcLevels["level" + level].GetValue(name);
                    return obj;
                }
                set
                {
                    //hpcLevels["level" + level][name] = value;
                }
            }            
            protected object this[DbUnit unit]
            {
                get
                {
                    return unit;
                }
                set
                {
                    unit.Value = value;
                }
            }

            public int level_up()
            {
                return owner.level_up();
            }
            public int level_down()
            {
                return owner.level_down();
            }

            public Bitmap Image
            {
                get 
                {
                    try 
                    { 
                        string imageName = (art.Count != 0) ? art[Level - 1] : "";
                        if (String.IsNullOrEmpty(imageName) == false)
                            return DHRC.Default.GetImage(imageName);

                    }
                    catch { }

                    return null;
                }
            }            

            public int SlotPriority
            {
                get { return slotPriority; }
            }

            public override string ToString()
            {
                return "[" + codeID + "] " + name;
            }
        }

        public class DBUPGRADE
        {         
            protected HabProperties hpsInitialData;
            protected List<DBEFFECT> effects;

            protected string name;
            protected IRecord owner;            

            protected int level = 0;
            protected int max_level = 1;

            protected UPGRADEPROFILE profile;

            internal DBUPGRADE()
            {                
                hpsInitialData = new HabProperties();
                effects = new List<DBEFFECT>();
            }
            internal DBUPGRADE(HabProperties hps)
            {
                hpsInitialData = hps.GetCopy();

                ////////////////////////
                //  get alias
                ////////////////////////

                this.name = hps.name;                

                ////////////////////////
                //  get max level
                ////////////////////////

                this.max_level = hpsInitialData.GetIntValue("maxlevel");

                ////////////////////////
                //  get effects
                ////////////////////////

                effects = new List<DBEFFECT>(4);

                string effectID;
                for(int i=1; i<=4; i++)
                    if ((effectID = hps.GetStringValue("effect" + i)) != "_")
                    {
                        DBEFFECT e = DBEFFECT.InitProperEffect(effectID,
                            hps.GetDoubleValue("base" + i),
                            hps.GetDoubleValue("mod" + i));

                        if (e != null) effects.Add(e);
                    }

            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }
            public IRecord Owner
            {
                get
                {
                    return owner;
                }
            }
            
            public string codeID
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }                        

            public bool CodeIDEquals(string codeID)
            {
                if (String.IsNullOrEmpty(codeID)) return false;

                return (String.Compare(this.codeID, codeID, true) == 0);
            }            

            public int Level
            {
                get { return level; }
                set { LevelShift(value - level); }
            }
            public int Max_level
            {
                get { return max_level; }
            }
            public int LevelShift(int amount)
            {
                int old_level = level;

                level = Math.Max(0, Math.Min(max_level, level + amount));                

                if (level != old_level)                    
                    this.refresh();                

                return level;
            }
            public int LevelShift(int amount, bool refreshOwner)
            {
                int old_level = level;

                level = Math.Max(0, Math.Min(max_level, level + amount));
                
                if (level != old_level)                
                    this.refresh(refreshOwner);

                return level;
            }

            public virtual UPGRADEPROFILE Profile
            {
                get
                {
                    if (profile == null)
                        profile = new UPGRADEPROFILE(this);
                    return profile;
                }
            }
           
            protected object this[IField unit]
            {
                get
                {
                    return unit;
                }
                set
                {
                    unit.Value = value;
                }
            }

            public int level_up()
            {
                return LevelShift(1);
            }
            public int level_down()
            {
                return LevelShift(-1);
            }

            public virtual void refresh()
            {
                //if (Owner != null)
                //    Owner.refresh();
            }

            public void refresh(bool refreshOwner)
            {
                //if (refreshOwner && Owner != null)
                //    Owner.refresh();
            }
            public virtual void refresh(HabProperties hps)
            {               
            }

            public bool ContainsEffectOfType<T>() where T : DBEFFECT
            {
                foreach (DBEFFECT effect in effects)
                    if (effect is T)
                        return true;

                return false;
            }
            public T GetEffectByType<T>() where T : DBEFFECT
            {
                foreach (DBEFFECT effect in effects)
                    if (effect is T)
                        return effect as T;

                return null;
            }

            public void SetOwner(IRecord Owner)
            {
                this.owner = Owner;
            }                        
            
            public void Apply()
            {
                if (this.level == 0) return;

                foreach (DBEFFECT effect in this.effects)
                    effect.Apply(this.Owner as unit, this.level);
            }

            public virtual object GetCopy()
            {
                DBUPGRADE copy = new DBUPGRADE(hpsInitialData);
                copy.level = this.level;
                copy.owner = this.owner;
                return copy;
            }

            public override string ToString()
            {
                if (Name != null)
                    return "[" + codeID + "] " + Name;
                else
                    return "[" + codeID + "] " + this.GetType().Name;
            }            
        }

        public class DBUPGRADES : List<DBUPGRADE>, IField //DbUnit, IEnumerable, IDbCollection
        {
            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBUPGRADES()
            {
            }
            public DBUPGRADES(string upgradeList)
            {
                base.AddRange(getUpgrades(upgradeList));
                RefreshOwner();
            }
            public DBUPGRADES(params DBUPGRADE[] upgrades)
            {
                base.AddRange(upgrades);
                RefreshOwner();
            }
            internal DBUPGRADES(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                this.name = Name;
                this.owner = Owner;
                this.attributes = attributes;

                substituteName = this.attributes.Get(typeof(SubstituteAttribute)) as SubstituteAttribute;
                if (substituteName == null)
                {
                    substituteName = new SubstituteAttribute(name);
                    this.attributes.Add(substituteName);
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }
            public string SubstituteName
            {
                get
                {
                    return substituteName.Name;
                }
            }
            public bool IsSubstituteNameMatch(string value)
            {
                return substituteName.IsMatch(value);
            }
            public FieldAttributeCollection Attributes
            {
                get
                {
                    return attributes;
                }
            }
            public IRecord Owner
            {
                get
                {
                    return owner;
                }
            }
            public object AutoValue
            {
                get
                {
                    return null;
                }
            }
            public bool IsNull
            {
                get
                {
                    return (this.Count == 0);
                }
            }
            public bool ValueEquals(object obj)
            {
                if (obj is DBUPGRADE)
                {
                    if (this.Count != 0)
                        return (this[0] == (obj as DBUPGRADE));
                    else
                        return false;
                }
                else
                    return DbUnit.ValueEquals(this, obj);
            }
            public string Text
            {
                get
                {
                    return this.ToString();
                }
            }

            public virtual int Size
            {
                get
                {
                    return this.Count;
                }
            }
            public virtual DbValueType ValueType
            {
                get { return DbValueType.Array; }
            }
            public virtual object Value
            {
                get
                {
                    return this;
                }
                set
                {
                    this.Clear();
                    bool isNull;
                    this.AddRange((DBUPGRADE[])ToValue(value, out isNull));
                    RefreshOwner();
                }
            }
            public virtual object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is DBUPGRADE[])
                    return value;
                else
                    if (value is string && value as string != "_")
                        return getUpgrades(value as string);
                    else
                        if (value is IList<string>)
                            return getUpgrades(value as IList<string>);
                        else
                            if (value is List<DBUPGRADE>)
                                return (value as List<DBUPGRADE>).ToArray();
                            else
                            {
                                IsNull = true;
                                return new DBUPGRADE[] { };
                            }
            }

            public bool Add(string upgradeID)
            {
                DBUPGRADE u = getUpgrade(upgradeID);
                if (u != null)
                {
                    base.Add(u);
                    u.SetOwner(this.owner);
                    return true;
                }
                return false;
            }
            public bool Add(string upgradeID, int level)
            {
                DBUPGRADE u = getUpgrade(upgradeID);
                if (u != null)
                {
                    base.Add(u);
                    u.Level = level;
                    u.SetOwner(this.owner);
                    return true;
                }
                return false;
            }
            public void AddRange(params DBUPGRADE[] upgrades)
            {
                base.AddRange(upgrades);
            }
            public bool Contains(string codeID)
            {
                foreach (DBUPGRADE upgrade in this)
                    if (upgrade.codeID == codeID)                    
                        return true;
                
                return false;
            }            
            public bool Contains(DBSTRINGCOLLECTION codeIDs)
            {
                foreach (string codeID in codeIDs)
                {
                    bool found = false;

                    foreach (DBUPGRADE upgrade in this)
                        if (upgrade.codeID == codeID)
                        {
                            found = true;
                            break;
                        }

                    if (found == false)
                        return false;
                }

                return true;
            }
            public DBUPGRADE GetByEffectOfType<T>() where T : DBEFFECT
            {                
                foreach (DBUPGRADE upgrade in this)
                    if (upgrade.ContainsEffectOfType<T>())
                        return upgrade;

                return null;
            }
            public bool Remove(string upgradeID)
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i].codeID == upgradeID)
                    {
                        this[i].SetOwner(null);
                        this.RemoveAt(i);
                        return true;
                    }
                return false;
            }
            public new bool Remove(DBUPGRADE upgrade)
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i] == upgrade)
                    {
                        this[i].SetOwner(null);
                        this.RemoveAt(i);
                        return true;
                    }
                return false;
            }

            public object getAt(int index)
            {
                if (index < 0 || index >= this.Count)
                    return null;
                else
                    return this[index];
            }
            public DBUPGRADE GetByCodeID(string codeID)
            {
                foreach (DBUPGRADE u in this)
                    if (codeID == u.codeID)
                        return u;

                return null;
            }
            public T GetByType<T>() where T : DBUPGRADE
            {
                foreach (DBUPGRADE upgrade in this)
                    if (upgrade is T)
                        return (upgrade as T);

                return null;
            }
            public List<T> GetRangeByType<T>() where T : DBUPGRADE
            {
                List<T> result = new List<T>();

                foreach (DBUPGRADE upgrade in this)
                    if (upgrade is T)
                        result.Add(upgrade as T);

                return result;
            }
            public List<DBUPGRADE> GetRange(DBSTRINGCOLLECTION abilIDs)
            {
                List<DBUPGRADE> list = new List<DBUPGRADE>(this.Count);

                foreach (DBUPGRADE u in this)
                    if (abilIDs.Contains(u.codeID))
                        list.Add(u);

                return list;
            }

            public void ResetLevels()
            {
                foreach (DBUPGRADE upgrade in this)
                    upgrade.Level = 0;
            }
            public void LevelsShift(int amount)
            {
                foreach (DBUPGRADE upgrade in this)
                    upgrade.LevelShift(amount);
            }

            public void SetLevels(int level)
            {
                foreach (DBUPGRADE upgrade in this)
                    upgrade.Level = level;
            }            

            protected void RefreshOwner()
            {
                foreach (DBUPGRADE upgrade in this)
                    upgrade.SetOwner(this.owner);
            }

            public void SetOwner(IRecord Owner)
            {
                this.owner = Owner;
                RefreshOwner();
            }                                    

            public void Apply()
            {
                foreach (DBUPGRADE upgrade in this)
                    upgrade.Apply();
            }

            public virtual object GetCopy()
            {
                DBUPGRADES upgradesCopy = new DBUPGRADES();
                foreach (DBUPGRADE upgrade in this)
                    upgradesCopy.Add(upgrade.GetCopy() as DBUPGRADE);
                return upgradesCopy;
            }

            public override string ToString()
            {
                string str = "";

                foreach (DBUPGRADE upgrade in this)
                    str += upgrade.codeID + ",";

                return str.TrimEnd(DBSTRINGCOLLECTION.comma_separator);
            }            

            public static DBUPGRADE getUpgrade(string upgradeID)
            {
                HabProperties hps = DHLOOKUP.hpcUpgradeData[upgradeID];

                if (hps == null)
                    return null;
                else
                    return new DBUPGRADE(hps);
            }

            public static DBUPGRADE[] getUpgrades(string upgrades_enumeration)
            {
                DBSTRINGCOLLECTION dbc = new DBSTRINGCOLLECTION(upgrades_enumeration);

                ArrayList al = new ArrayList();

                foreach (string upgradeID in dbc)
                {
                    HabProperties hps = DHLOOKUP.hpcUpgradeData[upgradeID];

                    if (hps == null) continue;

                    DBUPGRADE upgrade = new DBUPGRADE(hps);
                    al.Add(upgrade);
                }

                return (DBUPGRADE[])al.ToArray(typeof(DBUPGRADE));
            }
            public static DBUPGRADE[] getUpgrades(IList<string> upgradeList)
            {
                DBSTRINGCOLLECTION dbc = new DBSTRINGCOLLECTION(upgradeList);

                ArrayList al = new ArrayList();

                foreach (string upgradeID in dbc)
                {
                    HabProperties hps = DHLOOKUP.hpcUpgradeData[upgradeID];

                    if (hps == null) continue;

                    DBUPGRADE upgrade = new DBUPGRADE(hps);
                    al.Add(upgrade);
                }

                return (DBUPGRADE[])al.ToArray(typeof(DBUPGRADE));
            }                                 

            public virtual IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBUPGRADES(Name, Owner, attributes);
            }
            public virtual IField NewCopy(IRecord Owner, bool setValue)
            {
                DBUPGRADES dbu = new DBUPGRADES(this.Name, Owner, this.attributes);
                if (setValue) dbu.AddRange(this);
                return dbu;
            }
        }

        public abstract class DBEFFECT
        {
            public double Base;
            public double Mod;

            internal DBEFFECT(){}
            internal DBEFFECT(double Base, double Mod)
            {
                this.Base = Base;
                this.Mod = Mod;
            }

            public abstract string codeID
            {
                get;
            }

            protected object this[IField unit]
            {
                get
                {
                    return unit;
                }
                set
                {
                    unit.Value = value;
                }
            }

            public abstract void Apply(unit u, int level);

            public DBEFFECT GetNew(params object[] args)
            {
                DBEFFECT dbe = (DBEFFECT)this.GetType().InvokeMember(null,
                        BindingFlags.Public | BindingFlags.DeclaredOnly |
                        BindingFlags.Instance | BindingFlags.CreateInstance,
                        null, null, args);

                return dbe;
            }

            public static DBEFFECT InitProperEffect(string codeID, double Base, double Mod)
            {
                if (String.IsNullOrEmpty(codeID) == false)
                    return DbEffectsKnowledge.CreateKnownEffect(codeID, Base, Mod);
                return null;
            }
        }

        public class DbAttackRangeBonus : DBEFFECT
        {
            public DbAttackRangeBonus() { }
            public DbAttackRangeBonus(double Base, double Mod) : base(Base, Mod) { }

            public override string codeID
            {
                get { return "ratr"; }
            }

            public override void Apply(unit u, int level)
            {
                this[u.range] = (int)(Base + Mod * (level - 1));
            }
        }
        public class DbAttackDamageBonus : DBEFFECT
        {
            public DbAttackDamageBonus() { }
            public DbAttackDamageBonus(double Base, double Mod) : base(Base, Mod) { }

            public override string codeID
            {
                get { return "ratx"; }
            }

            public override void Apply(unit u, int level)
            {
                this[u.baseDamageBonus] = (int)(Base + Mod * (level - 1));
            }
        }
        public class DbMovementSpeedBonus : DBEFFECT
        {
            public DbMovementSpeedBonus() { }
            public DbMovementSpeedBonus(double Base, double Mod) : base(Base, Mod) { }

            public override string codeID
            {
                get { return "rmvx"; }
            }

            public override void Apply(unit u, int level)
            {
                this[u.baseMoveSpeedBonus] = (int)(Base + Mod * (level - 1));
            }
        }
        public class DbArmorBonus : DBEFFECT
        {
            public DbArmorBonus() { }
            public DbArmorBonus(double Base, double Mod) : base(Base, Mod) { }

            public override string codeID
            {
                get { return "rarm"; }
            }

            public override void Apply(unit u, int level)
            {                
                this[u.baseArmorBonus] = u.defUp * level;
            }
        }
    }    
}


