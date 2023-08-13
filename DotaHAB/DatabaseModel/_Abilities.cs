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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotaHIT.DatabaseModel.Abilities
{
    using Core;
    using Core.Resources;
    using Data;
    using DataTypes;    
    using Jass;
    using Jass.Native.Types;
    using Jass.Native.Constants;    

    #region pragma enable
#pragma warning disable 660
    #endregion

    [Flags]
    public enum AbilityState
    {
        None = 0,
        Deactivated = None,
        Activated = 1,
        PermanentlyActivated = 2,
        AutoCast = 4,
        AllActivatedFlags = Activated | PermanentlyActivated,
        Expired = 8
    }

    [Flags]
    public enum AbilitySpecs
    {
        None = 0,
        Any = None,
        HasBuff = 1,
        IsBuff = 2,
        IsAura = 4,
        IsPassive = 8,
        IsNotPassive = 16,
        IsLearned = 32,
        IsActivated = 64,
        IsApplied = 128,
        TriggersOnAttack = 256,
        TriggersOnDefense = 512,
        TriggersAlways = 1024
    }

    [Flags]
    public enum AbilityTriggerType
    {
        Never = 0,
        Always = 1,
        Default = Always,
        OnAttack = 2,
        OnDefense = 4,
        OnCast = 8
    }

    [Flags]
    public enum AbilityTriggerSpecs
    {
        None = 0,
        Default = None,
        AttacksOnly = 1,
        Dominating = 2,
        InventorySubject = 4,
        EffectApplier = 8,
    }

    [Flags]
    public enum AbilityInfo
    {
        CodeID = 1,
        Level = 2,
        LevelingData = 4,
        InitialData = 8,
        Buff = 16,
        Image = 32,
        Tip = 64,
        Targets = 128,
        Cooldown = 256,
        Area = 512,
        TriggerType = 1024,
        StackData = 2048,
        Activity = 4096,
        Owner = 8192,
        stateInfo = Level | TriggerType | Activity | Owner,
        fullInfo = ((Owner * 2) - 1),
        buffInfo = Level | TriggerType | Targets | Area | Owner,
        snapInfo = buffInfo | CodeID,
        stackInfo = Buff | StackData | Owner | Targets
    }

    public enum AbilityMatchType
    {
        Equals,
        Intersects,
        Subset,
        NotEquals,
        NotIntersects
    }

    [Flags]
    public enum AbilityBenefitType
    {
        None,
        NormalDamage,
        UnstableDamage,
        AdditionalDamage,
        SpellDamage
    }

    public class DbAbilitiesKnowledge
    {
        //public static DBABILITY[] abilities = new DBABILITY[] { };
        public static Dictionary<string, DBABILITY> NameAbilityPairs;

        static DbAbilitiesKnowledge()
        {
            CollectDbAbilities();
        }
        public static void WakeUp() { }

        public static void CollectDbAbilities()
        {
            Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

            Type[] types = m.FindTypes(new TypeFilter(SearchCriteria), "DotaHIT.DatabaseModel.Abilities");

            NameAbilityPairs = new Dictionary<string, DBABILITY>(types.Length,
                        (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

            foreach (Type type in types)
            {
                DBABILITY a = (DBABILITY)type.InvokeMember(null,
                            BindingFlags.Public | BindingFlags.DeclaredOnly |
                            BindingFlags.Instance | BindingFlags.CreateInstance,
                            null, null, null);

                if (!string.IsNullOrEmpty(a.codeID))
                    NameAbilityPairs.Add(a.codeID, a);
            }
        }
        private static bool SearchCriteria(Type t, object filter)
        {
            if ((t.Namespace == (string)filter) && t.IsPublic
                && t.IsSubclassOf(typeof(DBABILITY))
                && !t.IsAbstract)
                return true;
            return false;
        }

        public static DBABILITY CreateKnownAbility(string codeID, params object[] args)
        {
            DBABILITY ability;
            if (NameAbilityPairs.TryGetValue(codeID, out ability))
                return ability.GetNew(args);

            string metaCodeID = DBABILITIES.GetMetaName(codeID);
            if (metaCodeID != null) codeID = metaCodeID;

            if (NameAbilityPairs.TryGetValue(codeID, out ability))
                return ability.GetNew(args);

            return null;
        }
    }

    public class ABILITYPROFILE
    {
        protected HabProperties hpsInitialData;
        protected string m_codeID;

        protected string name;
        protected DBABILITY owner;

        protected DBIMAGE image = null;
        protected DBIMAGE researchImage = null;
        protected DBIMAGE disabledResearchImage = null;

        protected List<string> tip = null;
        protected List<string> ubertip = null;
        protected string researchtip = null;
        protected string researchubertip = null;
        private string hotkey = null;
        private string researchHotkey = null;

        protected int slotPriority = 0;
        protected int researchSlotPriority = 0;

        protected ABILITYPROFILE() { }
        internal ABILITYPROFILE(DBABILITY a)
        {
            owner = a;

            hpsInitialData = DHMpqDatabase.AbilitySlkDatabase["Profile"][a.Alias];

            if (hpsInitialData == null)
            {
                Console.WriteLine("Ability alias '" + a.Alias + "' not found in database");
                hpsInitialData = DHMpqDatabase.AbilitySlkDatabase["Profile"][a.codeID];
            }

            ////////////////////////
            //  get codeID
            ////////////////////////

            this.m_codeID = hpsInitialData.name;

            ////////////////////////
            //  get name
            ////////////////////////

            this.name = hpsInitialData.GetStringValue("Name");

            ////////////////////////
            //  get tip
            ////////////////////////                

            tip = hpsInitialData.GetStringListValue("Tip");

            ////////////////////////
            //  get ubertip
            ////////////////////////

            ubertip = hpsInitialData.GetStringListValue("Ubertip");

            //////////////////////////////////////
            //  check for unit tips and ubertips
            //////////////////////////////////////

            if (tip.Count == 0 && ubertip.Count == 0)
                for (int i = 1; i <= a.Max_level; i++)
                {
                    DBABILITY unit = a[i, "Unit"] as DBABILITY;
                    if (unit != null)
                    {
                        tip.Add(unit.Profile.tip[0]);
                        ubertip.Add(unit.Profile.ubertip[0]);
                    }
                }

            ////////////////////////
            //  get researchtip
            ////////////////////////

            this.researchtip = hpsInitialData.GetStringValue("Researchtip");            

            ////////////////////////
            //  get researchubertip
            ////////////////////////

            this.researchubertip = hpsInitialData.GetStringValue("Researchubertip");

            ////////////////////////
            //  get hotkey
            ////////////////////////

            this.hotkey = hpsInitialData.GetStringValue("Hotkey");
            this.researchHotkey = hpsInitialData.GetStringValue("Researchhotkey");

            ////////////////////////
            //  get slot priority
            ////////////////////////

            List<string> buttonpos = hpsInitialData.GetStringListValue("Buttonpos");
            if (buttonpos.Count > 0)
                slotPriority = RecordSlotComparer.get_slot(Convert.ToInt32(buttonpos[0]), Convert.ToInt32(buttonpos[1]));

            buttonpos = hpsInitialData.GetStringListValue("Researchbuttonpos");
            if (buttonpos.Count > 0)
                researchSlotPriority = RecordSlotComparer.get_slot(Convert.ToInt32(buttonpos[0]), (buttonpos.Count > 1) ? Convert.ToInt32(buttonpos[1]) : 0);

            ////////////////////////
            // get image
            ////////////////////////

            string imageName = hpsInitialData.GetStringValue("Art");

            if (String.IsNullOrEmpty(imageName) == false)
                this.image = DHRC.Default.GetImage(imageName);
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
        public DBABILITY Owner
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

        public bool IsOnCooldown
        {
            get { return owner.IsOnCooldown; }
            set { owner.IsOnCooldown = value; }
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
        public string Researchtip
        {
            get { return researchtip; }
        }
        public string Researchubertip
        {
            get { return researchubertip; }
        }
        public string Hotkey
        {
            get { return hotkey; }
        }
        public string ResearchHotkey
        {
            get { return researchHotkey; }
        }

        public DBINT Cost
        {
            get
            {
                return owner.Cost;
            }
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
        public object this[int level, string name]
        {
            get
            {
                return owner[level, name];
            }
            set
            {
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

        public DBIMAGE Image
        {
            get { return image; }
        }
        public DBIMAGE ResearchImage
        {
            get
            {
                if (researchImage == null)
                {
                    string imageName = hpsInitialData.GetStringValue("Researchart");

                    if (String.IsNullOrEmpty(imageName) == false)
                        this.researchImage = DHRC.Default.GetImage(imageName);
                }

                return researchImage;
            }
        }
        public DBIMAGE DisabledResearchImage
        {
            get
            {
                if (disabledResearchImage == null)
                {
                    string imageName = hpsInitialData.GetStringValue("Researchart");

                    if (String.IsNullOrEmpty(imageName) == false)
                    {
                        imageName = DHSTRINGS.GetDisabledTextureString(imageName);
                        this.disabledResearchImage = DHRC.Default.GetImage(imageName);
                    }
                }

                return disabledResearchImage;
            }
        }
        public DBDATETIME LearnPoint
        {
            get { return owner.LearnPoint; }
            set { owner.LearnPoint = value; }
        }

        public bool IsPassive
        {
            get
            {
                return owner.IsPassive;
            }
        }
        public AbilityState AbilityState
        {
            get
            {
                return owner.AbilityState;
            }
            set
            {
                owner.AbilityState = value;
            }
        }
        public void AutoCast(bool flag)
        {
            owner.AutoCast(flag);
        }

        public int SlotPriority
        {
            get { return slotPriority; }
        }

        public int ResearchSlotPriority
        {
            get { return researchSlotPriority; }
        }

        public bool TryConfig(out HabProperties hpsConfig)
        {
            return owner.TryConfig(out hpsConfig);
        }
        public void AcceptConfig(HabProperties hpsConfig)
        {
            owner.AcceptConfig(hpsConfig);
        }

        public override string ToString()
        {
            return "[" + codeID + "] " + name;
        }
    }
    public class BUFFPROFILE : ABILITYPROFILE
    {
        internal BUFFPROFILE(DBABILITY a)
        {
            owner = a;

            hpsInitialData = DHMpqDatabase.AbilitySlkDatabase["Profile"][a.codeID];

            ////////////////////////
            //  get codeID
            ////////////////////////

            this.m_codeID = hpsInitialData.name;

            ////////////////////////
            //  get tip
            ////////////////////////                

            tip = hpsInitialData.GetStringListValue("Bufftip");

            ////////////////////////
            //  get ubertip
            ////////////////////////

            ubertip = hpsInitialData.GetStringListValue("Buffubertip");

            ////////////////////////
            //  get researchubertip
            ////////////////////////

            this.researchubertip = hpsInitialData.GetStringValue("Researchubertip");

            ////////////////////////
            // get image
            ////////////////////////

            string imageName = hpsInitialData.GetStringValue("Buffart");

            if (String.IsNullOrEmpty(imageName) == false)
                this.image = DHRC.Default.GetImage(imageName);
        }

        public override string Tip
        {
            get { return tip[0]; }
        }
        public override string Ubertip
        {
            get { return ubertip[0]; }
        }
    }
    public class ABILITYPROFILECOLLECTION : List<ABILITYPROFILE>
    {
    }

    public abstract class DBABILITY : IComparable<DBABILITY>
    {
        protected HabPropertiesCollection hpcLevels;
        protected HabProperties hpsInitialData;
        protected HabProperties hpsLevelMeta;

        protected string name;
        protected IRecord owner;
        protected DBABILITY parent;

        protected string alias = null;

        protected double slot = 0;

        protected double cooldownDuration;
        protected double activatedDuration;

        protected int level = 0;
        protected int max_level = 1;

        protected ABILITYPROFILE profile;

        protected DBDATETIME learnPoint = null;

        protected List<DBBUFF> buffs = new List<DBBUFF>();
        protected DBTARGETTYPE targets = TargetType.Default;
        protected DBDOUBLE cooldown = new DBDOUBLE(null);
        protected DBDOUBLE heroDuration = new DBDOUBLE(null);
        protected DBDOUBLE duration = new DBDOUBLE(null);
        protected DBINT cost = new DBINT(null);
        protected DBINT area = new DBINT(null);
        protected AbilityTriggerType triggerType = AbilityTriggerType.Default;
        protected AbilityBenefitType benefitType = AbilityBenefitType.None;

        protected string order;
        protected string orderon;
        protected string orderoff;

        internal DBABILITY()
        {
            hpcLevels = new HabPropertiesCollection();
            hpsInitialData = new HabProperties();
            hpsLevelMeta = new HabProperties();
        }
        internal DBABILITY(HabProperties hps)
        {
            hpsInitialData = hps.GetCopy();

            ////////////////////////
            //  get alias
            ////////////////////////

            this.alias = hps.name;

            ////////////////////////
            //  get name
            ////////////////////////

            this.name = hps.GetStringValue("Name");

            ////////////////////////
            // get order strings
            ////////////////////////

            order = hps.GetStringValue("Order");
            orderon = hps.GetStringValue("Orderon");
            orderoff = hps.GetStringValue("Orderoff");

            ////////////////////////
            //  get max level
            ////////////////////////

            this.max_level = hpsInitialData.GetIntValue("levels");

            ////////////////////////
            //  get all levels
            ////////////////////////

            HabPropertiesCollection hpcLevelsTry = hpsInitialData.GetValue("hpcLevels") as HabPropertiesCollection;
            if (hpcLevelsTry != null)
                hpcLevels = hpcLevelsTry;
            else
                hpcLevels = new HabPropertiesCollection();

            hpsLevelMeta = new HabProperties();

            ////////////////////////
            // get buffs
            ////////////////////////

            string buffCodeIDs = hpcLevels.GetStringValue("1", "BuffID");
            buffs.AddRange(DBABILITIES.getBuffs(buffCodeIDs, this));

            ////////////////////////
            // get units
            ////////////////////////

            for (int i = 1; i <= max_level; i++)
            {
                HabProperties hpsLevel = hpcLevels[i + ""];
                if (hpsLevel == null) continue; // no data for this level

                string unitID = hpsLevel.GetStringValue("UnitID");
                if (!string.IsNullOrEmpty(unitID))
                    hpsLevel["Unit"] = DBABILITIES.getAbility(unitID);
            }
        }
        internal DBABILITY(DBABILITY parent)
        {
            this.parent = parent;
        }

        public string Name
        {
            get
            {
                return (parent == null) ? name : parent.name;
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
                return (parent == null) ? owner : parent.owner;
            }
        }

        public virtual string Alias
        {
            get { return alias; }//return (parent == null) ? alias : parent.alias; }
            set { alias = value; }
        }
        public abstract string codeID
        {
            get;
            set;
        }

        public double Slot
        {
            get { return slot; }
            set { slot = value; }
        }
        public virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>if false, then this ability 
        /// will not be shown in hero's skill table
        /// when learned</summary>
        public virtual bool IsVisible
        {
            get { return true; }
        }
        public bool IsAvailable = true;        

        public bool CodeIDEquals(string codeID)
        {
            if (String.IsNullOrEmpty(codeID)) return false;

            return (String.Compare(this.codeID, codeID, true) == 0);
        }
        public bool TargetsMatch(AbilityMatchType targMatchType, params TargetType[] targs)
        {
            switch (targMatchType)
            {
                case AbilityMatchType.Equals:
                    foreach (TargetType atarget in targs)
                        if (this.Targets == atarget)
                            return true;
                    break;

                case AbilityMatchType.Intersects:
                    foreach (TargetType atarget in targs)
                        if ((this.Targets & atarget) != 0 || this.Targets == atarget)
                            return true;
                    break;

                case AbilityMatchType.Subset:
                    foreach (TargetType atarget in targs)
                        if ((this.Targets & atarget) == atarget)
                            return true;
                    break;

                case AbilityMatchType.NotIntersects:
                    foreach (TargetType atarget in targs)
                        if ((this.Targets & atarget) != 0 || this.Targets == atarget)
                            return false;
                    return true;

                case AbilityMatchType.NotEquals:
                    foreach (TargetType atarget in targs)
                        if (this.Targets == atarget)
                            return false;
                    return true;
            }

            return false;
        }
        public bool TargetsMatch(unit u)
        {
            if ((this.Targets & TargetType.Self) != 0 && this.owner == u)
                return true;

            if ((this.Targets & TargetType.Friend) != 0 && u.IsFriend(this.owner as unit))
                return true;

            if ((this.Targets & (TargetType.Allies | TargetType.Ally)) != 0 && u.IsAlly(this.owner as unit))
                return true;

            if ((this.Targets & (TargetType.Enemies | TargetType.Enemy)) != 0 && u.IsEnemy(this.owner as unit))
                return true;

            return false;
        }
        public bool SpecsMatch(AbilitySpecs specs)
        {
            if ((specs & AbilitySpecs.IsLearned) != 0 && (this.Level == 0))
                return false;

            if ((specs & AbilitySpecs.IsActivated) != 0 && (this.AbilityState & AbilityState.AllActivatedFlags)==0)
                return false;

            if ((specs & AbilitySpecs.HasBuff) != 0 && (this.HasBuff == false))
                return false;

            if ((specs & AbilitySpecs.IsBuff) != 0 && (this.IsBuff == false))
                return false;

            if ((specs & AbilitySpecs.IsNotPassive) != 0 && this.IsPassive)
                return false;

            if ((specs & AbilitySpecs.IsPassive) != 0 && (this.IsPassive == false))
                return false;

            if ((specs & AbilitySpecs.IsAura) != 0 && (this.IsAura == false))
                return false;

            return true;
        }

        public int Level
        {
            get { return (parent == null) ? level : parent.level; }
            set { LevelShift(value - level); }
        }
        public int Max_level
        {
            get { return (parent == null) ? max_level : parent.max_level; }
        }
        public int LevelShift(int amount)
        {
            int old_level = level;

            level = Math.Max(0, Math.Min(max_level, level + amount));

            if (level > 0)
            {
                if (old_level == 0)
                    learnPoint = GetLearnPoint();
            }
            else
                learnPoint = null;

            if (level != old_level)
            {
                if (owner is unit) (owner as unit).OnHeroSkill(this);
                this.refresh();
            }

            return level;
        }
        public int LevelShift(int amount, bool refreshOwner)
        {
            int old_level = level;

            level = Math.Max(0, Math.Min(max_level, level + amount));

            if (level > 0)
            {
                if (old_level == 0)
                    learnPoint = GetLearnPoint();
            }
            else
                learnPoint = null;

            if (level != old_level)
            {
                if (owner is unit) (owner as unit).OnHeroSkill(this);
                this.refresh(refreshOwner);
            }

            return level;
        }

        public virtual ABILITYPROFILE Profile
        {
            get
            {
                if (parent != null)
                    return parent.Profile;

                if (profile == null)
                    profile = new ABILITYPROFILE(this);
                return profile;
            }
        }

        public object this[int level, string name]
        {
            get
            {
                object obj;

                if (level == 0) hpsInitialData.TryGetValue(name, out obj);
                else obj = hpcLevels[level + "", name];

                return obj;
            }
            set
            {
                //hpcLevels["level" + level][name] = value;
            }
        }
        public object this[string level, string name]
        {
            get
            {
                return hpcLevels[level, name];
            }
            set
            {
                //hpcLevels["level" + level][name] = value;
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
            hpsLevelMeta = GetProps(true);

            this.refresh(hpsLevelMeta);

            if (Owner != null)
                Owner.refresh();
        }

        public void refresh(bool refreshOwner)
        {
            hpsLevelMeta = GetProps(true);//false);

            this.refresh(hpsLevelMeta);

            if (refreshOwner && Owner != null)
                Owner.refresh();
        }
        public virtual void refresh(HabProperties hps)
        {
            ////////////////////////
            // get targets
            ////////////////////////

            this[targets] = hps.GetValue("targs");

            ////////////////////////
            // get cooldown
            ////////////////////////

            this[cooldown] = hps.GetValue("Cool");

            ////////////////////////
            // get hero duration
            ////////////////////////

            this[heroDuration] = hps.GetValue("HeroDur");

            ////////////////////////
            // get duration
            ////////////////////////

            this[duration] = hps.GetValue("Dur");

            ////////////////////////
            // get cost
            ////////////////////////

            this[cost] = hps.GetValue("Cost");

            ////////////////////////
            // get area
            ////////////////////////

            this[area] = hps.GetValue("Area");
        }

        public DBDATETIME LearnPoint
        {
            get { return (parent == null) ? learnPoint : parent.learnPoint; }
            set { learnPoint = value; }
        }

        public virtual bool IsAura
        {
            get
            {
                return false;
            }
        }
        public virtual bool HasBuff
        {
            get
            {
                return (parent == null) ? (buffs.Count != 0) : parent.HasBuff;
            }
        }
        public bool IsBuff
        {
            get
            {
                return (this is DBBUFF);
            }
        }
        public virtual bool IsPassive
        {
            get
            {
                return true;
            }
        }
        public virtual AbilityState AbilityState
        {
            get
            {
                return (parent == null) ?
                         ((IsPassive) ? AbilityState.PermanentlyActivated : AbilityState.Deactivated)
                         :
                         parent.AbilityState;
            }
            set
            {
            }
        }        

        public bool IsOnCooldown
        {
            get 
            { 
                return cooldownDuration > 0; 
            }
            set 
            {
                if (!(this.owner is widget)) return;

                if (value)
                {
                    // just in case
                    DHJassGlobalClock.Tick -= CooldownTick;

                    cooldownDuration = this.cooldown;
                    (this.owner as widget).UpdateAbilitiesOnCooldownCounter(true);

                    DHJassGlobalClock.Tick += CooldownTick;
                }
                else
                {
                    if (cooldownDuration != 0)
                    {
                        (this.owner as widget).UpdateAbilitiesOnCooldownCounter(false);
                        cooldownDuration = 0;
                        DHJassGlobalClock.Tick -= CooldownTick;
                        if (this.owner is unit) (owner as unit).Updated = true;
                    }
                }
            }
        }        
        private void CooldownTick()
        {
            if (this.cooldownDuration <= DHJassGlobalClock.TickInterval)            
                IsOnCooldown = false;
            else
                this.cooldownDuration -= DHJassGlobalClock.TickInterval;            
        }

        public double CooldownDuration
        {
            get { return cooldownDuration; }
        }

        public virtual DBTARGETTYPE Targets
        {
            get { return targets; }
        }
        public virtual DBDOUBLE Cooldown
        {
            get { return cooldown; }
        }
        public virtual DBINT Cost
        {
            get { return cost; }
        }
        public virtual AbilityTriggerType TriggerType
        {
            get { return triggerType; }
        }

        public virtual DBBUFF Buff
        {
            get
            {
                if (parent != null)
                    return parent.Buff;

                if (buffs.Count != 0)
                    return buffs[0];
                else
                    return null;
            }
        }
        public DBBUFF GetBuffByID(string codeID)
        {
            foreach (DBBUFF b in buffs)
                if (b.CodeIDEquals(codeID)) return b;
            return null;
        }

        public virtual void Activate()
        {
            if (!(this.owner is widget)) return;

            if (IsPassive) return;

            // just in case
            DHJassGlobalClock.Tick -= ActivatedTick;

            unit ownerUnit = (owner is item) ? (owner as item).get_owner() : owner as unit;

            activatedDuration = ownerUnit.IsHero ? heroDuration : duration;
            (this.owner as widget).UpdateActivatedAbilitiesCounter(true);            

            DHJassGlobalClock.Tick += ActivatedTick;
            
            ownerUnit.OnSpellCast(this);
            ownerUnit.OnSpellEffect(this);                       
        }
        public virtual void Deactivate(bool refreshOwner)
        {
            if (activatedDuration >=0)
            {
                (this.owner as widget).UpdateActivatedAbilitiesCounter(false);
                activatedDuration = 0;
                DHJassGlobalClock.Tick -= ActivatedTick;

                this.AbilityState = AbilityState.Expired;

                if (refreshOwner) this.owner.refresh();
            }
        }
        private void ActivatedTick()
        {
            if (this.activatedDuration <= DHJassGlobalClock.TickInterval)
            {
                // give it a chance to raise duration
                //this.owner.refresh();

                // if nothing changed, deactivate it
                //if (this.activatedDuration <= DHJassGlobalClock.TickInterval)
                    Deactivate(true);
            }
            else
                this.activatedDuration -= DHJassGlobalClock.TickInterval;
        }

        public virtual void AutoCast(bool flag)
        {            
        }

        public void SetOwner(IRecord Owner)
        {
            this.owner = Owner;

            if (owner is unit && !(owner as unit).IsDisposed)
                this.IsAvailable = (owner as unit).get_owningPlayer().getAbilityAvaiable(this.alias);
        }
        public void IssueOrder(string order)
        {
            if (owner is unit)
            {
                OrderID orderID = orderid.Parse(order);
                (owner as unit).OnIssuedOrder(orderID);
            }
        }

        public bool IsResearchedProperly
        {
            get
            {
                if (!(owner is unit)) return true;

                int reqLevel = hpsInitialData.GetIntValue("reqLevel");
                int levelSkip = hpsInitialData.GetIntValue("levelSkip", 2);

                return (owner as unit).Level >= (reqLevel + (levelSkip * (this.level - 1)));
            }
            set
            {
                if (!(owner is unit)) return;

                if (value)
                {
                    int reqLevel = hpsInitialData.GetIntValue("reqLevel");
                    int levelSkip = hpsInitialData.GetIntValue("levelSkip", 2);

                    if (reqLevel > (owner as unit).Level)
                        this.Level = 0;
                    else
                        this.Level = 1 + (((owner as unit).Level - reqLevel) / levelSkip);
                }
            }
        }
        public int GetCurrentRequiredOwnerLevel()
        {
            if (this.level == 0) return 0;

            int reqLevel = hpsInitialData.GetIntValue("reqLevel");
            int levelSkip = hpsInitialData.GetIntValue("levelSkip", 2);

            return reqLevel + (levelSkip * (this.level - 1));
        }
        public int GetNextRequiredOwnerLevel()
        {
            if (!(owner is unit)) return 0;

            if (this.level == this.max_level) return 0;

            int reqLevel = hpsInitialData.GetIntValue("reqLevel");
            int levelSkip = hpsInitialData.GetIntValue("levelSkip", 2);

            return reqLevel + (levelSkip * this.level);
        }

        public virtual void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
            this.StackDataB = Math.Max(this.StackDataB, a.StackDataB);
            this.StackDataC = Math.Max(this.StackDataC, a.StackDataC);
            this.StackDuration = Math.Max(this.StackDuration, a.StackDuration);
        }

        public virtual HabProperties MetaProps
        {
            get
            {
                return hpsLevelMeta;
            }
        }
        public virtual HabProperties GetProps(bool metaNames)
        {
            HabProperties hpsLevel = hpcLevels[level + ""];
            if (hpsLevel == null) return new HabProperties();

            if (metaNames)
            {
                HabProperties hpsMeta = new HabProperties(hpsLevel.Count);

                foreach (KeyValuePair<string, object> kvp in hpsLevel)
                {
                    string metaKey = this.codeID + "," + kvp.Key;
                    string metaName = DBABILITIES.GetMetaName(metaKey);

                    if (metaName != null)
                        hpsMeta.Add(metaName, kvp.Value);
                    else
                        hpsMeta.Add(kvp);
                }

                return hpsMeta;
            }
            else
                return hpsLevel;
        }
        public virtual DBABILITIES GetComponents()
        {
            DBABILITIES comps = new DBABILITIES();

            if (level != 0) comps.Add(this);

            return comps;
        }

        public virtual bool TryConfig(out HabProperties hpsConfig)
        {
            hpsConfig = null;
            return false;
        }
        public virtual void AcceptConfig(HabProperties hpsConfig)
        {
        }

        public virtual bool GetBenefit(AbilityTriggerType triggerType, AbilityBenefitType benefitType, out double amount)
        {
            amount = 0;
            return false;
        }
        public virtual bool Apply()
        {
            return false;
        }
        public virtual void Apply(AbilityResults ar)
        {
        }
        public virtual bool ApplyTo()
        {
            return false;
        }

        public virtual object Data
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
        public virtual double StackDataA
        {
            get
            {
                return 0.0;
            }
            set
            {
            }
        }
        public virtual double StackDataB
        {
            get
            {
                return 0.0;
            }
            set
            {
            }
        }
        public virtual double StackDataC
        {
            get
            {
                return 0.0;
            }
            set
            {
            }
        }
        public virtual double StackChance
        {
            get
            {
                return 1.0;
            }
        }
        public virtual double StackDuration
        {
            get
            {
                return activatedDuration;
            }
            set 
            {
                activatedDuration = value;
            }
        }

        /// <summary>
        /// determines if this ability will try to stack 
        /// with other abilities that have the same base
        /// </summary>
        public virtual bool Stackable
        {
            get
            {
                return false;
            }
        }
        public virtual DBABILITY GetStackCopy()
        {
            DBABILITY stackCopy = this.GetNew();

            this.CopyAttributesTo(stackCopy, AbilityInfo.stackInfo);

            return stackCopy;
        }

        public virtual DBABILITY GetNew(params object[] args)
        {
            DBABILITY dba = (DBABILITY)this.GetType().InvokeMember(null,
                    BindingFlags.Public | BindingFlags.DeclaredOnly |
                    BindingFlags.Instance | BindingFlags.CreateInstance,
                    null, null, args);

            return dba;
        }

        protected virtual DBABILITY GetAbilityCopy()
        {
            return InitProperAbility(hpsInitialData);
        }
        public virtual object GetCopy()
        {
            DBABILITY copy = GetAbilityCopy();
            this.CopyAttributesTo(copy, AbilityInfo.stateInfo | AbilityInfo.CodeID);
            return copy;
        }
        public virtual DBABILITY GetCopy(AbilityInfo copyInfo)
        {
            DBABILITY copy = GetAbilityCopy();
            this.CopyAttributesTo(copy, copyInfo);
            return copy;
        }
        public virtual void CopyAttributesTo(DBABILITY ability, AbilityInfo copyInfo)
        {
            CopyAttribues(this, ability, copyInfo);
        }
        public static void CopyAttribues(DBABILITY from, DBABILITY to, AbilityInfo copyInfo)
        {
            if ((copyInfo & AbilityInfo.Activity) != 0)
            {
                //to.AutoCast = (from != null) ? from.AutoCast : false;
                to.AbilityState = (from != null) ? from.AbilityState : AbilityState.None;
            }

            if ((copyInfo & AbilityInfo.Area) != 0)
                to.area = (from != null) ? from.area : new DBINT(null);

            if ((copyInfo & AbilityInfo.Buff) != 0)
                to.buffs = (from != null) ? from.buffs : new List<DBBUFF>();

            if ((copyInfo & AbilityInfo.CodeID) != 0)
                to.codeID = (from != null) ? from.codeID : null;

            if ((copyInfo & AbilityInfo.Cooldown) != 0)
                to.cooldown = (from != null) ? from.cooldown : new DBDOUBLE(null);

            if ((copyInfo & AbilityInfo.InitialData) != 0)
                to.hpsInitialData = (from != null) ? from.hpsInitialData : new HabProperties();

            if ((copyInfo & AbilityInfo.LevelingData) != 0)
                to.hpcLevels = (from != null) ? from.hpcLevels : new HabPropertiesCollection();

            if ((copyInfo & AbilityInfo.Level) != 0)
            {
                to.max_level = (from != null) ? from.max_level : 0;
                to.LevelShift(((from != null) ? from.level : 0) - to.level, false);
                to.learnPoint = (from != null) ? from.learnPoint : null;
                to.slot = from.slot;
            }

            if ((copyInfo & AbilityInfo.Owner) != 0)
                to.owner = (from != null) ? from.owner : null;

            if ((copyInfo & AbilityInfo.StackData) != 0)
            {
                to.StackDataA = (from != null) ? from.StackDataA : 0.0;
                to.StackDataB = (from != null) ? from.StackDataB : 0.0;
                to.StackDataC = (from != null) ? from.StackDataC : 0.0;
            }

            if ((copyInfo & AbilityInfo.Targets) != 0)
                to.targets = (from != null) ? (TargetType)from.targets : TargetType.Default;

            if ((copyInfo & AbilityInfo.TriggerType) != 0)
                to.triggerType = (from != null) ? from.triggerType : AbilityTriggerType.Default;
        }

        public static DBABILITY InitProperAbility(HabProperties hpsAbilityData)
        {
            string codeID = hpsAbilityData.GetValue("code") as string;

            if (String.IsNullOrEmpty(codeID))
            {
                if (String.IsNullOrEmpty(hpsAbilityData.name))
                    return new DBSIMPLEABILITY(hpsAbilityData);

                if (hpsAbilityData.name.StartsWith("B"))
                    return new DBBUFF(null, hpsAbilityData);
                else
                    codeID = hpsAbilityData.name; // probably custom ability                        
            }

            DBABILITY dba = DbAbilitiesKnowledge.CreateKnownAbility(codeID, hpsAbilityData);
            if (dba != null)
                return dba;

            dba = DbAbilitiesKnowledge.CreateKnownAbility(hpsAbilityData.name, hpsAbilityData);
            if (dba != null)
                return dba;

            return new DBSIMPLEABILITY(hpsAbilityData);
        }

        protected static DateTime previousLearnPoint = DateTime.Now.Date;
        public static DateTime GetLearnPoint()
        {
            DateTime learnPoint = DateTime.Now;
            if (learnPoint <= previousLearnPoint)
                learnPoint = new DateTime(previousLearnPoint.Ticks + 10);

            previousLearnPoint = learnPoint;
            return learnPoint;
        }

        public override string ToString()
        {
            if (Name != null)
                return "[" + codeID + "] " + Name;
            else
                return "[" + codeID + "] " + this.GetType().Name;
        }

        public void SendToApply(Dictionary<string, DBABILITIES> queue)
        {
            DBABILITIES dba;
            if (!queue.TryGetValue(this.codeID, out dba))
            {
                dba = new DBABILITIES();
                queue.Add(this.codeID, dba);
            }

            dba.Add(this);
        }

        public virtual int CompareTo(DBABILITY a)
        {
            int result = CaseInsensitiveComparer.DefaultInvariant.Compare(this.Priority, a.Priority);
            if (result != 0) return result;

            return CaseInsensitiveComparer.DefaultInvariant.Compare((DateTime)a.LearnPoint, (DateTime)this.LearnPoint);
        }
    }

    public class StackedAbilities : LinkedList<DBABILITY>
    {
        public double probability;

        public StackedAbilities() { }
        public StackedAbilities(DBABILITIES abilities)
        {
            foreach (DBABILITY a in abilities)
                Stack(a);
        }
        public StackedAbilities(StackedAbilities abilities)
            : base(abilities)
        {
            this.probability = abilities.probability;
        }
        public StackedAbilities(List<DBABILITY> selected, DBABILITIES collection)
        {
            probability = 1;

            foreach (DBABILITY a in collection)
                if (selected.Contains(a))
                    probability *= a.StackChance;
                else
                    probability *= (1 - a.StackChance);

            foreach (DBABILITY a in selected)
                Stack(a);
        }
        protected void Init(List<DBABILITY> selected, DBABILITIES collection)
        {
            probability = 1;

            foreach (DBABILITY a in collection)
                if (selected.Contains(a))
                    probability *= a.StackChance;
                else
                    probability *= (1 - a.StackChance);

            foreach (DBABILITY a in selected)
                Stack(a);
        }
        public void Add(DBABILITY a)
        {
            base.AddLast(a);
        }
        public virtual void Stack(DBABILITY a)
        {
            foreach (DBABILITY stackedAbility in this)
                stackedAbility.Stack(a);
        }
        public void RemoveByID(string abilID)
        {
            LinkedListNode<DBABILITY> head = this.First;

            if (head != null)
            {
                do
                {
                    if (head.Value.CodeIDEquals(abilID))
                    {
                        this.Remove(head);
                        return;
                    }
                    head = head.Next;
                }
                while (head != null);
            }
        }
        public void RemoveByState(AbilityState abilityState)
        {
            LinkedListNode<DBABILITY> head = this.First;

            if (head != null)
            {
                do
                {
                    if ((head.Value.AbilityState & abilityState) != 0)
                    {
                        this.Remove(head);
                        return;
                    }
                    head = head.Next;
                }
                while (head != null);
            }
        }

        public bool ContainsID(string abilID)
        {
            foreach (DBABILITY ability in this)
                if (ability.CodeIDEquals(abilID))
                    return true;

            return false;
        }
        public virtual StackedAbilities GetCopy()
        {
            return new StackedAbilities(this);
        }
        public AbilityResults GetResults()
        {
            AbilityResults ar = new AbilityResults(this.probability);

            foreach (DBABILITY a in this)
                a.Apply(ar);

            return ar;
        }
        public AbilityResults GetResults(AbilityResults initial_results)
        {
            AbilityResults ar = initial_results.GetCopy(this.probability);

            foreach (DBABILITY a in this)
                a.Apply(ar);

            return ar;
        }

        public bool ContainsType<T>() where T : DBABILITY
        {
            foreach (DBABILITY ability in this)
                if (ability is T)
                    return true;

            return false;
        }
        public T GetFirstByType<T>() where T : DBABILITY
        {
            LinkedListNode<DBABILITY> head = this.First;

            if (head != null)
            {
                do
                {
                    if (head.Value is T)
                        return head.Value as T;

                    head = head.Next;
                }
                while (head != null && head != this.First);
            }

            return null;
        }
        public T GetLastByType<T>() where T : DBABILITY
        {
            LinkedListNode<DBABILITY> tail = this.Last;

            if (tail != null)
            {
                do
                {
                    if (tail.Value is T)
                        return tail.Value as T;

                    tail = tail.Previous;
                }
                while (tail != null && tail != this.Last);
            }

            return null;
        }

        public static StackedAbilitiesCollection GetPossibleCombinations<T>(DBABILITIES abilities) where T : StackedAbilities, new()
        {
            if (abilities.Count == 0) return new StackedAbilitiesCollection();

            int capacity = 1 + (int)DHMATH.LC(abilities.Count, abilities.Count);
            StackedAbilitiesCollection combinations = new StackedAbilitiesCollection(capacity);

            ///////////////////////////////////////////
            // collecting stack chances (probabilities)
            ///////////////////////////////////////////

            double[] probs = new double[abilities.Count];
            for (int i = 0; i < abilities.Count; i++)
                probs[i] = abilities[i].StackChance;

            ////////////////////////////////////////////
            // collecting all possible combinations 
            ////////////////////////////////////////////

            for (int i = 0; i < abilities.Count; i++)
                addPossibleCombinations<T>(abilities, i, new List<DBABILITY>(), combinations);

            ////////////////////////////////////////////
            // adding empty combination
            ////////////////////////////////////////////

            T stacked = new T();
            stacked.probability = 1.0;
            foreach (double p in probs)
                stacked.probability *= (1 - p);

            combinations.Add(stacked);

            return combinations;
        }
        static void addPossibleCombinations<T>(DBABILITIES abilities, int index, List<DBABILITY> gathered, StackedAbilitiesCollection combinations) where T : StackedAbilities, new()
        {
            if (index >= abilities.Count) return;

            gathered.Add(abilities[index]);

            T stacked = new T();
            stacked.Init(gathered, abilities);

            combinations.Add(stacked);

            for (int i = index + 1; i < abilities.Count; i++)
                addPossibleCombinations<T>(abilities, i, new List<DBABILITY>(gathered), combinations);
        }
    }
    public class StackedAbilitiesDictionary : ListDictionary
    {
        public StackedAbilitiesDictionary() { }
        public StackedAbilitiesDictionary(DBABILITIES abilities)
        {
            AddRange(abilities);
        }
        public bool Add(DBABILITY a)
        {
            StackedAbilities sa;

            // if it's an ability with buff
            if (a.HasBuff)
            {
                sa = this[a.Buff.codeID] as StackedAbilities;

                if (sa == null)
                {
                    sa = new StackedAbilities();
                    this.Add(a.Buff.codeID, sa);                    

                    sa.Add(a.GetStackCopy());
                    return true;
                }
                else
                    sa.Stack(a);
            }
            else
                if (a.Stackable)
                {
                    sa = this[a.codeID] as StackedAbilities;

                    if (sa == null)
                    {
                        sa = new StackedAbilities();
                        this.Add(a.codeID, sa);

                        sa.Add(a.GetStackCopy());
                        return true;
                    }
                    else
                        sa.Stack(a);
                }
                else
                {
                    sa = this["notstacked"] as StackedAbilities;

                    if (sa == null)
                    {
                        sa = new StackedAbilities();
                        this.Add("notstacked", sa);                        
                    }

                    sa.Add(a);

                    return true;
                }

            return false;
        }
        public bool Add(DBABILITY a, bool asCopy)
        {
            StackedAbilities sa;

            // if it's an ability with buff
            if (a.HasBuff)
            {
                sa = this[a.Buff.codeID] as StackedAbilities;

                if (sa == null)
                {
                    sa = new StackedAbilities();
                    this.Add(a.Buff.codeID, sa);                                        

                    sa.Add(asCopy ? a.GetStackCopy() : a);
                    return true;
                }
                else
                    sa.Stack(a);
            }
            else
                if (a.Stackable)
                {
                    sa = this[a.codeID] as StackedAbilities;

                    if (sa == null)
                    {
                        sa = new StackedAbilities();
                        this.Add(a.codeID, sa);

                        sa.Add(asCopy ? a.GetStackCopy() : a);
                        return true;
                    }
                    else
                        sa.Stack(a);
                }
                else
                {
                    sa = this["notstacked"] as StackedAbilities;

                    if (sa == null)
                    {
                        sa = new StackedAbilities();
                        this.Add("notstacked", sa);
                    }

                    sa.Add(a);
                    return true;
                }

            return false;
        }
        public void AddRange(DBABILITIES abilities)
        {
            foreach (DBABILITY a in abilities)
                Add(a);
        }
        public DBABILITIES GetAbilities()
        {
            DBABILITIES abilities = new DBABILITIES();
            foreach (StackedAbilities sa in this.Values)
                abilities.AddRange(sa);
            return abilities;
        }
        public void RemoveByState(AbilityState abilityState)
        {
            object[] keys = new object[this.Keys.Count];

            this.Keys.CopyTo(keys, 0);
            
            for(int i=0; i< keys.Length; i++)
            {
                StackedAbilities sa = this[keys[i]] as StackedAbilities;

                sa.RemoveByState(abilityState);

                if (sa.Count == 0) this.Remove(keys[i]);
            }
        }
    }
    public class StackedAbilitiesCollection : List<StackedAbilities>
    {
        public StackedAbilitiesCollection() { }
        public StackedAbilitiesCollection(int capacity) : base(capacity) { }
        public double GetProbabilityOfType<T>() where T : DBABILITY
        {
            double probability = 0;
            foreach (StackedAbilities sa in this)
                if (sa.ContainsType<T>())
                    probability += sa.probability;
            return probability;
        }
    }

    public class FreeStackedAbilities : StackedAbilities
    {
        public FreeStackedAbilities() { }
        public FreeStackedAbilities(DBABILITIES abilities)
            : base(abilities)
        {
        }
        public FreeStackedAbilities(StackedAbilities abilities)
            : base(abilities)
        {
        }
        public FreeStackedAbilities(List<DBABILITY> selected, DBABILITIES collection)
            : base(selected, collection)
        {
        }
        public override void Stack(DBABILITY a)
        {
            base.AddLast(a);
        }
        public override StackedAbilities GetCopy()
        {
            return new FreeStackedAbilities(this);
        }
    }

    public class AbilityResults : ListDictionary
    {
        public double probability;
        public List<DBABILITY> sources = new List<DBABILITY>();

        public AbilityResults()
        {
            this.probability = 1.0;
        }
        public AbilityResults(double probability)
        {
            this.probability = probability;
        }
        public void Add(string key, object value)
        {
            base.Add(key, value);
        }

        public void SetDouble(string key, double value)
        {
            base[key] = value;
        }
        public void SetDoubleEx(string key, double value)
        {
            if (!this.Contains(key))
                base[key] = value;
        }
        public double GetDouble(string key)
        {
            object result = base[key];

            if (result != null) return (double)result;
            else return 0.0;
        }

        public void SetInt(string key, int value)
        {
            base[key] = value;
        }
        public int GetInt(string key)
        {
            object result = base[key];

            if (result != null) return (int)result;
            else return 0;
        }

        public void SetDamage(string key, Damage value)
        {
            base[key] = value;
        }
        public void SetDamageEx(string key, Damage value)
        {
            if (!this.Contains(key))
                base[key] = value;
        }
        public Damage GetDamage(string key)
        {
            object result = base[key];

            if (result != null) return (Damage)result;
            else return new Damage();
        }

        public void SetDbDamage(string key, DBDAMAGE value)
        {
            base[key] = value;
        }
        public DBDAMAGE GetDbDamage(string key)
        {
            object result = base[key];

            if (result != null) return (DBDAMAGE)result;
            else return new DBDAMAGE();
        }

        public void SetAbility(string key, DBABILITY value)
        {
            base[key] = value;
        }
        public void SetAbilityEx(string key, DBABILITY value)
        {
            if (!this.Contains(key))
                base[key] = value;
        }
        public DBABILITY GetAbility(string key)
        {
            return base[key] as DBABILITY;
        }

        public void Rename(string oldKey, string newKey)
        {
            object value = this[oldKey];
            if (value != null)
            {
                this.Remove(oldKey);
                this.Add(newKey, value);
            }
        }

        public AbilityResults GetCopy()
        {
            AbilityResults copy = new AbilityResults(this.probability);
            foreach (DictionaryEntry de in this)
                copy.Add(de.Key, de.Value);
            return copy;
        }
        public AbilityResults GetCopy(double probability)
        {
            AbilityResults copy = new AbilityResults(probability);
            foreach (DictionaryEntry de in this)
                copy.Add(de.Key, de.Value);
            return copy;
        }
    }
    public class StackedAbilityResults : List<AbilityResults>
    {
        ListDictionary cache = new ListDictionary();

        public StackedAbilityResults() { }
        public StackedAbilityResults(IEnumerable<StackedAbilities> stackedAbilities)
        {
            foreach (StackedAbilities sa in stackedAbilities)
                this.Add(sa.GetResults());
        }
        public StackedAbilityResults(IEnumerable<StackedAbilities> stackedAbilities, AbilityResults initial_results)
        {
            foreach (StackedAbilities sa in stackedAbilities)
                this.Add(sa.GetResults(initial_results));
        }
        public StackedAbilityResults(DBABILITIES abilities)
        {
            foreach (DBABILITY a in abilities)
            {
                AbilityResults ar = new AbilityResults();

                a.Apply(ar);

                this.Add(ar);
            }
        }
        public Damage GetDamage(string key)
        {
            object cacheValue = cache[key];
            if (cacheValue != null) return (Damage)cacheValue;

            Damage result = new Damage();

            foreach (AbilityResults ar in this)
            {
                Damage dmg = ar.GetDamage(key);

                if (ar.probability == 1.0) result += dmg;
                else
                    if (ar.probability != 0.0) result += dmg * ar.probability;
            }

            cache[key] = result;

            return result;
        }
        public DBDAMAGE GetDbDamage(string key)
        {
            object cacheValue = cache[key];
            if (cacheValue != null) return (DBDAMAGE)cacheValue;

            DBDAMAGE result = new DBDAMAGE();

            foreach (AbilityResults ar in this)
            {
                DBDAMAGE dmg = ar.GetDbDamage(key);

                if (ar.probability == 1.0) result += dmg;
                else
                    if (ar.probability != 0.0) result += dmg * ar.probability;
            }

            cache[key] = result;

            return result;
        }
        public double GetDouble(string key)
        {
            object cacheValue = cache[key];
            if (cacheValue != null) return (double)cacheValue;

            double result = 0;

            foreach (AbilityResults ar in this)
            {
                double value = ar.GetDouble(key);

                if (ar.probability == 1.0) result += value;
                else
                    if (ar.probability != 0.0) result += value * ar.probability;
            }

            cache[key] = result;

            return result;
        }
        public double GetResultAbilityTypeProbability<T>(string key) where T : DBABILITY
        {
            double result = 0;
            foreach (AbilityResults ar in this)
                if (ar.GetAbility(key) is T)
                    result += ar.probability;
            return result;
        }
        public double GetKeyProbability(string key)
        {
            double result = 0.0;
            foreach (AbilityResults ar in this)
                if (ar.Contains(key))
                    result += ar.probability;
            return result;
        }
        public List<DBABILITY> GetPossibleSources()
        {
            List<DBABILITY> sources = new List<DBABILITY>();

            foreach (AbilityResults ar in this)
                if (ar.probability != 0)
                    foreach (DBABILITY ability in ar.sources)
                        if (!sources.Contains(ability))
                            sources.Add(ability);

            return sources;
        }
    }

    public class DBABILITIES : List<DBABILITY>, IField //DbUnit, IEnumerable, IDbCollection
    {
        protected string name;
        protected IRecord owner;
        protected SubstituteAttribute substituteName = null;
        protected FieldAttributeCollection attributes = new FieldAttributeCollection();

        public DBABILITIES()
        {
        }
        public DBABILITIES(string abilityList)
        {
            base.AddRange(getAbilities(abilityList));
            RefreshOwner();
        }
        public DBABILITIES(params DBABILITY[] abilities)
        {
            base.AddRange(abilities);
            RefreshOwner();
        }
        internal DBABILITIES(string Name, IRecord Owner, FieldAttributeCollection attributes)
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
            if (obj is DBABILITY)
            {
                if (this.Count != 0)
                    return (this[0] == (obj as DBABILITY));
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
                this.AddRange((DBABILITY[])ToValue(value, out isNull));
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

            if (value is DBABILITY[])
                return value;
            else
                if (value is string)
                    return getAbilities(value as string);
                else
                    if (value is IList<string>)
                        return getAbilities(value as IList<string>);
                    else
                        if (value is List<DBABILITY>)
                            return (value as List<DBABILITY>).ToArray();
                        else
                        {
                            IsNull = true;
                            return new DBABILITY[] { };
                        }
        }

        public bool Add(string abilityID)
        {
            DBABILITY a = getAbility(abilityID);
            if (a != null)
            {
                base.Add(a);
                a.SetOwner(this.owner);
                return true;
            }
            return false;
        }
        public bool Add(string abilityID, int level)
        {
            DBABILITY a = getAbility(abilityID);
            if (a != null)
            {
                base.Add(a);
                a.Level = level;
                a.SetOwner(this.owner);
                return true;
            }
            return false;
        }
        public bool AddEx(string abilityID, int level)
        {
            if (this.GetByAlias(abilityID) != null)
                return false;
            else
                return Add(abilityID, level);
        }
        public void AddRange(params DBABILITY[] abilities)
        {
            base.AddRange(abilities);
        }
        public bool Contains(string codeID)
        {
            foreach (DBABILITY ability in this)
                if (ability.codeID == codeID)
                    return true;

            return false;
        }
        public bool Contains(DBSTRINGCOLLECTION codeIDs)
        {
            foreach (string codeID in codeIDs)
            {
                bool found = false;

                foreach (DBABILITY ability in this)
                    if (ability.codeID == codeID)
                    {
                        found = true;
                        break;
                    }

                if (found == false)
                    return false;
            }

            return true;
        }
        public bool ContainsLearnPoint(DBABILITY ability)
        {
            foreach (DBABILITY abil in this)
                if (abil.LearnPoint == ability.LearnPoint)
                    return true;
            return false;
        }
        public bool Remove(string abilityID)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i].Alias == abilityID)
                {
                    this[i].SetOwner(null);
                    this.RemoveAt(i);
                    return true;
                }
            return false;
        }
        public new bool Remove(DBABILITY ability)
        {
            for (int i = 0; i < this.Count; i++)
                if (this[i] == ability)
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
        public DBABILITY GetByAlias(string alias)
        {
            foreach (DBABILITY a in this)
                if (alias == a.Alias)
                    return a;

            return null;
        }
        public T GetByType<T>() where T : DBABILITY
        {
            foreach (DBABILITY ability in this)
                if (ability is T)
                    return (ability as T);

            return null;
        }
        public List<T> GetRangeByType<T>() where T : DBABILITY
        {
            List<T> result = new List<T>();

            foreach (DBABILITY ability in this)
                if (ability is T)
                    result.Add(ability as T);

            return result;
        }
        public List<DBABILITY> GetRange(DBSTRINGCOLLECTION abilIDs)
        {
            List<DBABILITY> list = new List<DBABILITY>(this.Count);

            foreach (DBABILITY a in this)
                if (abilIDs.Contains(a.Alias))
                    list.Add(a);

            return list;
        }

        public void SetAbilityAvailable(string alias, bool available)
        {
            bool updated = false;

            foreach (DBABILITY a in this)
                if (alias == a.Alias && a.IsAvailable != available)
                {
                    a.IsAvailable = available;
                    updated = true;
                }

            if (updated && this.owner is unit)
                this.owner.refresh();
        }

        public void ResetLevels()
        {
            foreach (DBABILITY ability in this)
                ability.Level = 0;
        }
        public void LevelsShift(int amount)
        {
            foreach (DBABILITY ability in this)
                ability.LevelShift(amount);
        }

        public void SetLevels(int level)
        {
            foreach (DBABILITY ability in this)
                ability.Level = level;
        }
        public void SetSlot(double slot)
        {
            foreach (DBABILITY ability in this)
                ability.Slot = slot;
        }

        public List<DBABILITY> GetWithAvailableResearchPoints()
        {
            List<DBABILITY> result = new List<DBABILITY>(this.Count);

            if (!(owner is unit)) return result;

            int total_researched = 0;

            foreach (DBABILITY a in this)
                total_researched += a.Level;

            if ((owner as unit).Level - total_researched <= 0) return result;

            foreach (DBABILITY a in this)
                if (a.GetNextRequiredOwnerLevel() <= (owner as unit).Level)
                    result.Add(a);

            return result;
        }
        public List<DBABILITY> GetWithAvailableResearchPoints(DBSTRINGCOLLECTION abilIDs)
        {
            List<DBABILITY> result = new List<DBABILITY>(this.Count);

            if (!(owner is unit)) return result;

            int total_researched = 0;

            foreach (DBABILITY a in this)
                if (abilIDs.Contains(a.Alias))
                    total_researched += a.Level;

            if ((owner as unit).Level - total_researched <= 0) return result;

            foreach (DBABILITY a in this)
                if (abilIDs.Contains(a.Alias) && a.GetNextRequiredOwnerLevel() <= (owner as unit).Level)
                    result.Add(a);

            return result;
        }
        public int GetTotalResearchPoints()
        {
            int total_researched = 0;

            foreach (DBABILITY a in this)
                total_researched += a.Level;

            return total_researched;
        }
        public int GetTotalResearchPoints(DBSTRINGCOLLECTION abilIDs)
        {
            int total_researched = 0;

            foreach (DBABILITY a in this)
                if (abilIDs.Contains(a.Alias))
                    total_researched += a.Level;

            return total_researched;
        }
        public int GetRequiredOwnerLevel()
        {
            int total_researched = 0;
            int required_level = 0;

            foreach (DBABILITY a in this)
            {
                total_researched += a.Level;
                required_level = Math.Max(required_level, a.GetCurrentRequiredOwnerLevel());
            }

            return Math.Max(total_researched, required_level);
        }
        public int GetRequiredOwnerLevel(DBSTRINGCOLLECTION abilIDs)
        {
            int total_researched = 0;
            int required_level = 0;

            foreach (DBABILITY a in this)
                if (abilIDs.Contains(a.Alias))
                {
                    total_researched += a.Level;
                    required_level = Math.Max(required_level, a.GetCurrentRequiredOwnerLevel());
                }

            return Math.Max(total_researched, required_level);
        }

        protected void RefreshOwner()
        {
            foreach (DBABILITY ability in this)
                ability.SetOwner(this.owner);
        }

        public void SetOwner(IRecord Owner)
        {
            this.owner = Owner;
            RefreshOwner();
        }

        public HabPropertiesCollection GetProps()
        {
            HabPropertiesCollection hpc = new HabPropertiesCollection();
            foreach (DBABILITY ability in this)
                hpc.Add(ability.GetProps(true));
            return hpc;
        }

        public DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();
            for (int i = 0; i < this.Count; i++)
                components.AddRange(this[i].GetComponents());
            return components;
        }

        public DBABILITIES GetSpecific(AbilitySpecs specs)
        {
            DBABILITIES specAbils = new DBABILITIES();

            foreach (DBABILITY ability in this)
            {
                if (!ability.SpecsMatch(specs))
                    continue;

                specAbils.Add(ability);
            }

            return specAbils;
        }
        public DBABILITIES GetSpecific(AbilityTriggerType triggerType)
        {
            DBABILITIES specAbils = new DBABILITIES();

            foreach (DBABILITY ability in this)
            {
                if ((ability.TriggerType & triggerType) == 0)
                    continue;

                specAbils.Add(ability);
            }

            return specAbils;
        }
        public DBABILITIES GetSpecific(AbilitySpecs specs, AbilityTriggerType triggerType)
        {
            DBABILITIES specAbils = new DBABILITIES();

            foreach (DBABILITY ability in this)
            {
                if (!ability.SpecsMatch(specs))
                    continue;

                if ((ability.TriggerType & triggerType) == 0)
                    continue;

                specAbils.Add(ability);
            }

            return specAbils;
        }
        public DBABILITIES GetSpecific(AbilitySpecs specs, AbilityMatchType targMatchType, params TargetType[] targs)
        {
            DBABILITIES specAbils = new DBABILITIES();

            foreach (DBABILITY ability in this)
            {
                if (!ability.SpecsMatch(specs))
                    continue;

                if (!ability.TargetsMatch(targMatchType, targs))
                    continue;

                specAbils.Add(ability);
            }

            return specAbils;
        }
        public DBABILITIES GetSpecific(AbilitySpecs specs, AbilityTriggerType triggerType, AbilityMatchType targMatchType, params TargetType[] targs)
        {
            DBABILITIES specAbils = new DBABILITIES();

            foreach (DBABILITY ability in this)
            {
                if ((ability.TriggerType & triggerType) == 0)
                    continue;

                if (!ability.SpecsMatch(specs))
                    continue;

                if (!ability.TargetsMatch(targMatchType, targs))
                    continue;

                specAbils.Add(ability);
            }

            return specAbils;
        }       

        public void SetActivatedState(AbilityState abilityState)
        {
            foreach (DBABILITY a in this)
                if (!a.IsPassive)
                    a.AbilityState = abilityState;
        }

        public bool HasActivatedState(AbilityState abilityState)
        {
            foreach (DBABILITY a in this)
                if (!a.IsPassive && (a.AbilityState & abilityState) != 0)
                    return true;
            return false;
        }

        public void Apply()
        {
            foreach (DBABILITY ability in this)
                ability.Apply();
        }

        public virtual object GetCopy()
        {
            DBABILITIES abilsCopy = new DBABILITIES();
            foreach (DBABILITY ability in this)
                abilsCopy.Add(ability.GetCopy() as DBABILITY);
            return abilsCopy;
        }

        public override string ToString()
        {
            string str = "";

            foreach (DBABILITY ability in this)
                str += ability.codeID + ",";

            return str.TrimEnd(DBSTRINGCOLLECTION.comma_separator);
        }
        public string ToString(bool alias)
        {
            string str = "";

            if (alias)
                foreach (DBABILITY ability in this)
                    str += ability.Alias + ",";
            else
                foreach (DBABILITY ability in this)
                    str += ability.codeID + ",";

            return str.TrimEnd(DBSTRINGCOLLECTION.comma_separator);
        }

        public void SortByAcquisition(bool reverse)
        {
            this.Sort(new DbAbilitiesAcquisitionOrderComparer(reverse));
        }

        public static DBABILITY getAbility(string abilitiyID)
        {
            HabProperties hps = DHLOOKUP.hpcAbilityData[abilitiyID];

            if (hps == null)
                return null;
            else
                return DBABILITY.InitProperAbility(hps);
        }

        public static DBABILITY[] getAbilities(string abilities_enumeration)
        {
            DBSTRINGCOLLECTION dbc = new DBSTRINGCOLLECTION(abilities_enumeration);

            ArrayList al = new ArrayList();

            foreach (string abilityID in dbc)
            {
                HabProperties hps = DHLOOKUP.hpcAbilityData[abilityID];

                if (hps == null) continue;

                DBABILITY ability = DBABILITY.InitProperAbility(hps);

                if (ability != null)
                    al.Add(ability);
            }

            return (DBABILITY[])al.ToArray(typeof(DBABILITY));
        }
        public static DBABILITY[] getAbilities(IList<string> abilityList)
        {
            DBSTRINGCOLLECTION dbc = new DBSTRINGCOLLECTION(abilityList);

            ArrayList al = new ArrayList();

            foreach (string abilityID in dbc)
            {
                HabProperties hps = DHLOOKUP.hpcAbilityData[abilityID];

                if (hps == null) continue;

                DBABILITY ability = DBABILITY.InitProperAbility(hps);

                if (ability != null)
                    al.Add(ability);
            }

            return (DBABILITY[])al.ToArray(typeof(DBABILITY));
        }

        public static List<DBBUFF> getBuffs(string buffs_enumeration, DBABILITY parent)
        {
            DBSTRINGCOLLECTION dbc = new DBSTRINGCOLLECTION(buffs_enumeration);

            List<DBBUFF> buffs = new List<DBBUFF>();

            foreach (string abilityID in dbc)
            {
                HabProperties hps = DHLOOKUP.hpcAbilityData[abilityID];

                if (hps == null) continue;

                DBBUFF ability = new DBBUFF(parent, hps);
                buffs.Add(ability);
            }

            return buffs;
        }

        public static string GetMetaName(string keyName)
        {
            string metaName;
            DHLOOKUP.dcAbilityDataMetaNames.TryGetValue(keyName, out metaName);
            return metaName;
        }

        public virtual IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
        {
            return new DBABILITIES(Name, Owner, attributes);
        }
        public virtual IField NewCopy(IRecord Owner, bool setValue)
        {
            DBABILITIES dbu = new DBABILITIES(this.Name, Owner, this.attributes);
            if (setValue) dbu.AddRange(this);
            return dbu;
        }
    }

    public class DbAbilitiesAcquisitionOrderComparer : IComparer<DBABILITY>
    {
        private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();
        private bool reverse = false;

        public DbAbilitiesAcquisitionOrderComparer(bool reverse)
        {
            this.reverse = reverse;
        }
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<DBABILITY>.Compare(DBABILITY x, DBABILITY y)
        {
            if (reverse)
                return cic.Compare((DateTime)y.LearnPoint, (DateTime)x.LearnPoint);
            else
                return cic.Compare((DateTime)x.LearnPoint, (DateTime)y.LearnPoint);
        }
    }

    public class DBSIMPLEABILITY : DBABILITY
    {
        protected string m_codeID;

        public DBSIMPLEABILITY() { m_codeID = ""; }
        public DBSIMPLEABILITY(HabProperties hps)
            : base(hps)
        {
            this.m_codeID = hps.GetValue("code") as string;
        }
        public DBSIMPLEABILITY(string codeID)
        {
            this.m_codeID = codeID;
        }
        public DBSIMPLEABILITY(string codeID, string parentCodeID)
        {
            this.m_codeID = codeID;
        }
        public DBSIMPLEABILITY(string codeID, HabProperties hps)
            : base(hps)
        {
            this.m_codeID = codeID;
        }

        public DBSIMPLEABILITY(string codeID, int level, HabProperties hps)
            : base(hps)
        {
            this.m_codeID = codeID;
            this.level = level;
        }

        public override string codeID
        {
            get { return m_codeID; }
            set { m_codeID = value; }
        }

        public override object Data
        {
            get
            {
                HabProperties hps = GetProps(false);
                return hps.GetValue("Data", false);
            }
            set
            {
                HabProperties hps = GetProps(false);
                hps.SetValueToFirstMatchedKey("Data", value);
            }
        }

        protected override DBABILITY GetAbilityCopy()
        {
            return new DBSIMPLEABILITY(this.codeID, hpsInitialData);
        }

        public override bool IsAura
        {
            get { return !area.IsNull; }
        }
        public override bool IsPassive
        {
            get { return cooldown.IsNull; }
        }
    }
    public class DBBUFF : DBSIMPLEABILITY
    {
        public DBBUFF() { }
        public DBBUFF(DBABILITY parent, HabProperties hps)
        {
            this.parent = parent;

            alias = hps.name;
            m_codeID = hps.name;
        }
        public DBBUFF(string codeID) : base(codeID) { }
        public DBBUFF(string codeID, string parentCodeID) : base(codeID, parentCodeID) { }

        public override bool HasBuff
        {
            get
            {
                return false;
            }
        }
        public override DBBUFF Buff
        {
            get
            {
                return null;
            }
        }

        public override string codeID
        {
            get { return m_codeID; }
        }

        public override ABILITYPROFILE Profile
        {
            get
            {
                if (profile == null)
                    profile = new BUFFPROFILE(this);
                return profile;
            }
        }

        public override HabProperties GetProps(bool addHeader)
        {
            return new HabProperties();
        }

        public override void refresh(HabProperties hps)
        {
        }
    }

    public class DBHPREGENABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn = "Ihpr";

        public DBHPREGENABILITY() { }
        public DBHPREGENABILITY(HabProperties hps)
            : base(hps)
        {
        }
        public DBHPREGENABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
        }

        public override string codeID
        {
            get { return "Arel"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.hpRegen] = value;
                return true;
            }

            return false;
        }
    }
    public class DBIMANAREGENABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn = "Imrp";

        public DBIMANAREGENABILITY() { }
        public DBIMANAREGENABILITY(HabProperties hps)
            : base(hps)
        {
        }
        public DBIMANAREGENABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
        }

        public override string codeID
        {
            get { return "AIrm"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.imanaRegen] = value;
                return true;
            }

            return false;
        }
    }

    public class DBAURAUNHOLY : DBABILITY
    {
        private DBDOUBLE ims = new DBDOUBLE(null);
        private DBDOUBLE hpRegen = new DBDOUBLE(null);

        private string imsColumn = "Uau1";
        private string hpRegenColumn = "Uau2";

        public DBAURAUNHOLY() { }
        public DBAURAUNHOLY(HabProperties hps)
            : base(hps)
        {
        }
        public DBAURAUNHOLY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            hpRegenColumn = column;
        }

        public override string codeID
        {
            get { return "AUau"; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get values
            /////////////////////////////

            this[ims] = hps.GetValue(imsColumn);
            this[hpRegen] = hps.GetValue(hpRegenColumn);
        }

        public override double StackDataA
        {
            get
            {
                return ims;
            }
            set
            {
                this.ims.Value = value;
            }
        }
        public override double StackDataB
        {
            get
            {
                return hpRegen;
            }
            set
            {
                this.hpRegen.Value = value;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
            this.StackDataB = Math.Max(this.StackDataB, a.StackDataB);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                if ((targets & TargetType.Self) != 0)
                {
                    this[hero.ims] = ims;
                    this[hero.hpRegen] = hpRegen;
                }

                return true;
            }

            return false;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }
    }

    public class DBLIFEBONUSABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn = "Ilif";

        public DBLIFEBONUSABILITY() { }
        public DBLIFEBONUSABILITY(HabProperties hps)
            : base(hps)
        {
        }
        public DBLIFEBONUSABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
        }

        public override string codeID
        {
            get { return "AIml"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.max_hp] = value;
                return true;
            }

            return false;
        }
    }
    public class DBMANABONUSABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn = "Iman";

        public DBMANABONUSABILITY() { }
        public DBMANABONUSABILITY(HabProperties hps)
            : base(hps)
        {
        }
        public DBMANABONUSABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
        }

        public override string codeID
        {
            get { return "AImm"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.max_mana] = value;
                return true;
            }

            return false;
        }
    }

    public class DBREPLENISHLIFEABILITY : DBABILITY
    {
        protected AbilityState abilityState;        

        //private DBDOUBLE manaPointsGained = new DBDOUBLE(null);
        //private DBDOUBLE minimumManaRequired = new DBDOUBLE(null);
        //private DBINT maximumUnitsChargedToCaster = new DBINT(null);

        //private string manaPointsGainedColumn = "Rej2";
        //private string minimumManaRequiredColumn = "Rpb4";
        //private string maximumUnitsChargedToCasterColumn = "Rpb5";

        public DBREPLENISHLIFEABILITY() { }
        public DBREPLENISHLIFEABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get { return "Arpl"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void AutoCast(bool flag)
        {
            abilityState = flag ? AbilityState.PermanentlyActivated | AbilityState.AutoCast : AbilityState.None;            
            IssueOrder(flag ? orderon : orderoff);
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            //this[manaPointsGained] = hps.GetValue(manaPointsGainedColumn);
            //this[minimumManaRequired] = hps.GetValue(minimumManaRequiredColumn);
            //this[maximumUnitsChargedToCaster] = hps.GetValue(maximumUnitsChargedToCasterColumn);
        }

        public override bool Apply()
        {
            return false;
        }
    }
    public class DBREPLENISHMANAABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE manaPointsGained = new DBDOUBLE(null);
        private DBDOUBLE minimumManaRequired = new DBDOUBLE(null);
        private DBINT maximumUnitsChargedToCaster = new DBINT(null);

        private string manaPointsGainedColumn = "Rej2";
        private string minimumManaRequiredColumn = "Rpb4";
        private string maximumUnitsChargedToCasterColumn = "Rpb5";

        public DBREPLENISHMANAABILITY() { }
        public DBREPLENISHMANAABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get { return "Arpm"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void AutoCast(bool flag)
        {
            abilityState = flag ? AbilityState.PermanentlyActivated | AbilityState.AutoCast : AbilityState.None;
            IssueOrder(flag ? orderon : orderoff);
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }        

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[manaPointsGained] = hps.GetValue(manaPointsGainedColumn);
            this[minimumManaRequired] = hps.GetValue(minimumManaRequiredColumn);
            this[maximumUnitsChargedToCaster] = hps.GetValue(maximumUnitsChargedToCasterColumn);
        }

        public override bool Apply()
        {
            return false;
        }
    }

    public class DBACTIVELIFEBONUSABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn = "Ilif";

        public DBACTIVELIFEBONUSABILITY() { }
        public DBACTIVELIFEBONUSABILITY(HabProperties hps)
            : base(hps)
        {
        }
        public DBACTIVELIFEBONUSABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
        }

        public override string codeID
        {
            get { return "AImi"; }
            set { }
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.baseHp] = hero.baseHp + value;
                return true;
            }

            return false;
        }
    }

    public class DBDEFENSEABILITY : DBABILITY
    {
        private DBDOUBLE spellDefense = new DBDOUBLE(null);
        private readonly string spellDefenseColumn = "Def5";

        public DBDEFENSEABILITY()
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }
        public DBDEFENSEABILITY(HabProperties hps)
            : base(hps)
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }

        public override string codeID
        {
            get { return "AIdd"; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get spell defense
            /////////////////////////////

            this[spellDefense] = hps.GetValue(spellDefenseColumn);
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit)
            {
                DBDAMAGE dmg = ar["DAMAGE"] as DBDAMAGE;

                if (dmg != null)
                    switch (dmg.AttackType)
                    {
                        case AttackType.Spell:
                            if (!spellDefense.IsNull) ar["DAMAGE"] = dmg * spellDefense;
                            break;
                    }
            }
        }
    }
    public class DBACTIVEDEFENSEABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE damageTaken = new DBDOUBLE(null);
        private DBDOUBLE damageDealt = new DBDOUBLE(null);
        private DBDOUBLE msFactor = new DBDOUBLE(null);
        private DBDOUBLE asFactor = new DBDOUBLE(null);
        private DBDOUBLE spellDefense = new DBDOUBLE(null);
        private readonly string damageTakenColumn = "Def1";
        private readonly string damageDealtColumn = "Def2";
        private readonly string msFactorColumn = "Def3";
        private readonly string asFactorColumn = "Def4";
        private readonly string spellDefenseColumn = "Def5";

        public DBACTIVEDEFENSEABILITY()
        {
            this.triggerType = AbilityTriggerType.Never;
        }
        public DBACTIVEDEFENSEABILITY(HabProperties hps)
            : base(hps)
        {
            this.triggerType = AbilityTriggerType.Never;
        }

        public override string codeID
        {
            get { return "Adef"; }
            set { }
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override DBABILITIES GetComponents()
        {
            if ((abilityState & AbilityState.AllActivatedFlags)!=0)
            {
                DBACTIVEDEFENSE_DEFENSE_ABILITY defense = new DBACTIVEDEFENSE_DEFENSE_ABILITY(this);
                DBACTIVEDEFENSE_STATE_ABILITY state = new DBACTIVEDEFENSE_STATE_ABILITY(this);
                DBACTIVEDEFENSE_ATTACK_ABILITY attack = new DBACTIVEDEFENSE_ATTACK_ABILITY(this);

                HabProperties hps = GetProps(true);

                defense.refresh(hps);
                state.refresh(hps);
                attack.refresh(hps);

                return new DBABILITIES(
                    defense,
                    state,
                    attack);
            }
            else
                return new DBABILITIES();
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[damageTaken] = hps.GetValue(damageTakenColumn);
            this[damageDealt] = hps.GetValue(damageDealtColumn);
            this[msFactor] = hps.GetValue(msFactorColumn);
            this[asFactor] = hps.GetValue(asFactorColumn);
            this[spellDefense] = hps.GetValue(spellDefenseColumn);
        }
    }
    public class DBACTIVEDEFENSE_STATE_ABILITY : DBABILITY
    {
        private DBDOUBLE msFactor = new DBDOUBLE(null);
        private DBDOUBLE asFactor = new DBDOUBLE(null);
        private readonly string msFactorColumn = "Def3";
        private readonly string asFactorColumn = "Def4";

        public DBACTIVEDEFENSE_STATE_ABILITY()
        {
            this.triggerType = AbilityTriggerType.Always;
        }
        public DBACTIVEDEFENSE_STATE_ABILITY(DBABILITY parent)
            : base(parent)
        {
            this.triggerType = AbilityTriggerType.Always;
        }

        public override string codeID
        {
            get { return ""; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            this[msFactor] = hps.GetValue(msFactorColumn);
            this[asFactor] = hps.GetValue(asFactorColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                if (!asFactor.IsNull) this[hero.ias] = asFactor - 1;
                if (!msFactor.IsNull) this[hero.ims] = msFactor - 1;
                return true;
            }

            return false;
        }
    }
    public class DBACTIVEDEFENSE_DEFENSE_ABILITY : DBABILITY
    {
        private DBDOUBLE damageTaken = new DBDOUBLE(null);
        private DBDOUBLE spellDefense = new DBDOUBLE(null);
        private readonly string damageTakenColumn = "Def1";
        private readonly string spellDefenseColumn = "Def5";

        public DBACTIVEDEFENSE_DEFENSE_ABILITY()
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }
        public DBACTIVEDEFENSE_DEFENSE_ABILITY(DBABILITY parent)
            : base(parent)
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }

        public override string codeID
        {
            get { return ""; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            this[damageTaken] = hps.GetValue(damageTakenColumn);
            this[spellDefense] = hps.GetValue(spellDefenseColumn);
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit)
            {
                DBDAMAGE dmg = ar["DAMAGE"] as DBDAMAGE;

                if (dmg != null)
                    switch (dmg.AttackType)
                    {
                        case AttackType.Spell:
                            if (!spellDefense.IsNull) ar["DAMAGE"] = dmg * spellDefense;
                            break;
                    }
            }
        }
    }
    public class DBACTIVEDEFENSE_ATTACK_ABILITY : DBABILITY
    {
        private DBDOUBLE damageDealt = new DBDOUBLE(null);
        private readonly string damageDealtColumn = "Def2";

        public DBACTIVEDEFENSE_ATTACK_ABILITY()
        {
            this.triggerType = AbilityTriggerType.OnAttack;
        }
        public DBACTIVEDEFENSE_ATTACK_ABILITY(DBABILITY parent)
            : base(parent)
        {
            this.triggerType = AbilityTriggerType.OnAttack;
        }

        public override string codeID
        {
            get { return ""; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            this[damageDealt] = hps.GetValue(damageDealtColumn);
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit)
            {
                // a trick to skip this if autocast is enabled
                if (ar.GetAbility("DOMINATING_ABILITY") != null)
                    return;

                ar.SetDamageEx("ATTACK_DAMAGE", (owner as unit).damage);
                ar.SetDamage("ATTACK_DAMAGE", ar.GetDamage("ATTACK_DAMAGE") * damageDealt);

                ar.sources.Add(this);
            }
        }
    }

    public class DBISPELLRESISTANCEABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn = "isr2";

        public DBISPELLRESISTANCEABILITY() { this.triggerType = AbilityTriggerType.OnDefense; }
        public DBISPELLRESISTANCEABILITY(HabProperties hps)
            : base(hps)
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }
        public DBISPELLRESISTANCEABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
            this.triggerType = AbilityTriggerType.OnDefense;
        }

        public override string codeID
        {
            get { return "AIsr"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit)
            {
                DBDAMAGE dmg = ar["DAMAGE"] as DBDAMAGE;

                if (dmg != null)
                    switch (dmg.AttackType)
                    {
                        case AttackType.Spell:
                            ar["RESISTED_DAMAGE"] = dmg * (1.0 - value);
                            ar.Remove("DAMAGE");
                            break;
                    }
            }
        }
    }

    public class DBBONUSMSABILITY : DBABILITY
    {
        private string m_codeID = "AIms";

        private DBDOUBLE bms = new DBDOUBLE(null);
        private string bmsColumn;

        public DBBONUSMSABILITY() { }
        public DBBONUSMSABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            m_codeID = bmsColumn = column;
            alias = parent.Alias + column;
        }
        public DBBONUSMSABILITY(HabProperties hps)
            : base(hps)
        {
            bmsColumn = "Imvb";
        }

        public override string codeID
        {
            get { return m_codeID; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get bonus move speed
            /////////////////////////////

            this[bms] = hps.GetValue(bmsColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.bonus_ms] = bms;
                return true;
            }

            return false;
        }
    }

    public class DBIMSABILITY : DBABILITY
    {
        private DBDOUBLE ims = new DBDOUBLE(null);
        private string imsColumn;

        public DBIMSABILITY() { }
        public DBIMSABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            imsColumn = column;
            alias = parent.Alias + column;
        }

        public override string codeID
        {
            get { return imsColumn; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get increased move speed
            /////////////////////////////

            this[ims] = hps.GetValue(imsColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.ims] = ims;
                return true;
            }

            return false;
        }
    }
    public class DBIASABILITY : DBABILITY
    {
        private string m_codeID = "AIas";

        private DBDOUBLE ias = new DBDOUBLE(null);
        private string iasColumn;

        public DBIASABILITY() { }
        public DBIASABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            m_codeID = iasColumn = column;
            alias = parent.Alias + column;
        }
        public DBIASABILITY(HabProperties hps)
            : base(hps)
        {
            iasColumn = "Isx1";
        }

        public override string codeID
        {
            get { return m_codeID; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get increased attack speed
            /////////////////////////////

            this[ias] = hps.GetValue(iasColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.ias] = ias;
                return true;
            }

            return false;
        }
    }
    public class DBAURAENDURANCEABILITY : DBABILITY
    {
        DBDOUBLE ims = new DBDOUBLE();
        DBDOUBLE ias = new DBDOUBLE();
        string imsColumn = "Oae1";
        string iasColumn = "Oae2";

        public DBAURAENDURANCEABILITY() { }
        public DBAURAENDURANCEABILITY(HabProperties hps)
            : base(hps)
        {
            //ims = new DBIMSABILITY("Oae1",this);
            //ias = new DBIASABILITY("Oae2", this);
        }

        public override string codeID
        {
            get { return "AOae"; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[ims] = hps.GetValue(imsColumn);
            this[ias] = hps.GetValue(iasColumn);
        }

        public override double StackDataA
        {
            get
            {
                return ims;
            }
            set
            {
                ims.Value = value;
            }
        }
        public override double StackDataB
        {
            get
            {
                return ias;
            }
            set
            {
                ias.Value = value;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
            this.StackDataB = Math.Max(this.StackDataB, a.StackDataB);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.ias] = ias;
                this[hero.ims] = ims;
                return true;
            }

            return false;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }
    }

    public class DBSPELLBOOKABILITY : DBABILITY
    {
        DBABILITIES abilList;
        DBBIT someBool;
        DBINT someInt1;
        DBINT someInt2;

        public DBSPELLBOOKABILITY() { }
        public DBSPELLBOOKABILITY(HabProperties hps)
            : base(hps)
        {
            abilList = new DBABILITIES();
            someBool = new DBBIT();
            someInt1 = new DBINT();
            someInt2 = new DBINT();
        }

        public override string codeID
        {
            get { return "Aspb"; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
            {
                components.AddRange(abilList.GetComponents());
            }

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[abilList] = hps.GetValue("spb1");
            this[someBool] = hps.GetValue("spb2");
            this[someInt1] = hps.GetValue("spb3");
            this[someInt2] = hps.GetValue("spb4");

            foreach (DBABILITY a in abilList)
                a.Level = this.Level;
        }
    }

    public class DBATTRIBUTEBONUSABILITY : DBABILITY
    {
        private DBINT agiBonus = new DBINT(null);
        private DBINT intBonus = new DBINT(null);
        private DBINT strBonus = new DBINT(null);
        private DBBIT isHidden = new DBBIT(0);
        private readonly string agiColumn = "Iagi";
        private readonly string intColumn = "Iint";
        private readonly string strColumn = "Istr";
        private readonly string isHiddenColumn = "Ihid";

        public DBATTRIBUTEBONUSABILITY()
        {
        }
        public DBATTRIBUTEBONUSABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get { return "Aamk"; }
            set { }
        }

        public override bool IsVisible
        {
            get
            {
                return !isHidden;
            }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get strength bonus
            /////////////////////////////

            this[strBonus] = hps.GetValue(strColumn);

            /////////////////////////////
            // get agility bonus
            /////////////////////////////

            this[agiBonus] = hps.GetValue(agiColumn);

            /////////////////////////////
            // get intelligence bonus
            /////////////////////////////

            this[intBonus] = hps.GetValue(intColumn);

            /////////////////////////////
            // get hidden state
            /////////////////////////////

            this[isHidden] = hps.GetValue(isHiddenColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.agility] = agiBonus;
                this[hero.intelligence] = intBonus;
                this[hero.strength] = strBonus;

                return true;
            }

            return false;
        }
    }
    public class DBINVENTORYABILITY : DBABILITY
    {
        private DBINT slots = new DBINT(null);
        private readonly string slotsColumn = "inv1";

        public DBINVENTORYABILITY()
        {
        }
        public DBINVENTORYABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get
            {
                return "AInv";
            }
            set
            {
            }
        }

        public int Slots
        {
            get { return slots; }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get number of slots
            /////////////////////////////

            this[slots] = hps.GetValue(slotsColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                hero.Inventory.init(slots, 4); // 4 additional slots for backpack
                return true;
            }

            return false;
        }
    }

    public class DBCHANNELABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        [Flags]
        enum ChannelFlags
        {
            None = 0,
            Visible = 1,
            TargetingImage = 2,
            PhysicalSpell = 4,
            UniversalSpell = 8,
            UniqueCast = 16
        }        

        private ChannelFlags channelFlags = ChannelFlags.None;
        private readonly string channelFlagsColumn = "Ncl3";

        public DBCHANNELABILITY()
        {
        }
        public DBCHANNELABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get
            {
                return "ANcl";
            }
            set
            {
            }
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override bool IsVisible
        {
            get
            {
                return (channelFlags & ChannelFlags.Visible) != 0;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get channel flags
            /////////////////////////////

            this.channelFlags = (ChannelFlags)hps.GetIntValue(channelFlagsColumn, 0);
        }
    }

    /*
    public class DBNECROMASTERYABILITY : DBCUSTOMABILITY
    {            
        private DBINT soulLimit = new DBINT(null);
        private string soulLimitColumn = "DataA";

        private string soulsPropName = "Souls stored";

        public DBNECROMASTERYABILITY() { this.alias = "A0BR"; }
        public DBNECROMASTERYABILITY(HabProperties hps)
            : base(hps)
        {                
        }            

        public override HabProperties GetProps(bool addHeader)
        {
            HabProperties hps = hpcLevels["level" + level];
            hps = (hps != null) ? hps : new HabProperties();
            if (addHeader)
                return hps.AddHeader("A0BR");
            else
                return hps;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get soul limit
            /////////////////////////////

            this[soulLimit] = hps.GetValue(soulLimitColumn, false);                
        }

        public override bool TryConfig(out HabProperties hpsConfig)
        {
            if (this.owner is unit)
            {
                unit hero = owner as unit;

                DBINT soulsStored = new DBINT(hero.hpsCustomData.GetValue(soulsPropName));

                hpsConfig = new HabProperties();
                hpsConfig.Add(soulsPropName, soulsStored);

                return true;
            }
            else
                return base.TryConfig(out hpsConfig);
        }
        public override void AcceptConfig(HabProperties hpsConfig)
        {
            unit hero = owner as unit;
            DBINT soulsStored = hpsConfig.GetValue(soulsPropName) as DBINT;

            int limitedSoulsStored = soulsStored;
            limitedSoulsStored = Math.Min((int)soulLimit, Math.Max(0, limitedSoulsStored));

            this[soulsStored] = limitedSoulsStored;

            hero.hpsCustomData.Add(soulsPropName, hpsConfig[soulsPropName]);
        }

        public override bool Apply()
        {
            if (base.Apply())
            {
                unit hero = owner as unit;

                DBINT soulsStored = new DBINT(hero.hpsCustomData.GetValue(soulsPropName));

                if (!soulsStored.IsNull)
                {
                    this[hero.bonus_damage] = (soulsStored*2);
                    hero.udpate_dmg();
                }
                return true;
            }
            else
                return false;
        }            
    }
    */

    public class DBDAMAGEINCREASEABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn;

        public DBDAMAGEINCREASEABILITY() { }
        public DBDAMAGEINCREASEABILITY(HabProperties hps)
            : base(hps)
        {
            valueColumn = "Iatt";
        }
        public DBDAMAGEINCREASEABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            alias = codeID;
            valueColumn = column;
        }

        public override string codeID
        {
            get { return "AIat"; }
            set { }
        }

        public override object Data
        {
            get
            {
                return value;
            }
            set
            {
                this.value.Value = value;
            }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.bonus_damage] = value;
                return true;
            }

            return false;
        }
    }
    public class DBENGINEERINGUPGRADEABILITY : DBABILITY
    {
        private DBDOUBLE bonusMovespeed = new DBDOUBLE(null);
        private DBDOUBLE bonusDamage = new DBDOUBLE(null);
        private List<string> upgrade1 = new List<string>();
        private List<string> upgrade2 = new List<string>();
        private List<string> upgrade3 = new List<string>();
        private string bonusMovespeedColumn = "Neg1";
        private string bonusDamageColumn = "Neg2";
        private string upgrade1Column = "Neg3";
        private string upgrade2Column = "Neg4";
        private string upgrade3Column = "Neg5";

        public DBENGINEERINGUPGRADEABILITY() { }
        public DBENGINEERINGUPGRADEABILITY(HabProperties hps)
            : base(hps)
        {
        }
        public DBENGINEERINGUPGRADEABILITY(string column, DBABILITY parent)
            : base(parent)
        {
        }

        public override string codeID
        {
            get { return "ANeg"; }
            set { }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override bool HasBuff
        {
            get
            {
                return false;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[bonusMovespeed] = hps.GetValue(bonusMovespeedColumn);
            this[bonusDamage] = hps.GetValue(bonusDamageColumn);

            upgrade1 = hps.GetStringListValue(upgrade1Column);
            upgrade2 = hps.GetStringListValue(upgrade2Column);
            upgrade3 = hps.GetStringListValue(upgrade3Column);
        }

        private void ApplyUpgrade(unit hero, List<string> upgrade)
        {
            if (upgrade.Count < 2) return;

            string oldAbilityID = upgrade[0];
            string newAbilityID = upgrade[1];

            if (hero.BaseHeroAbilList.Remove(oldAbilityID))
            {
                DBABILITY oldAbility = hero.heroAbilities.GetByAlias(oldAbilityID);

                hero.heroAbilities.Remove(oldAbility);

                hero.heroAbilities.Add(newAbilityID, oldAbility.Level);
                hero.BaseHeroAbilList.Add(newAbilityID);
            }
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                if (!bonusMovespeed.IsNull) this[hero.ims] = bonusMovespeed;
                if (!bonusDamage.IsNull) this[hero.bonus_damage] = bonusDamage;

                ApplyUpgrade(hero, upgrade1);
                ApplyUpgrade(hero, upgrade2);
                ApplyUpgrade(hero, upgrade3);

                return true;
            }

            return false;
        }
    }

    public abstract class DBATTACKMETHODABILITY : DBABILITY
    {
        private AttackMethod attackMethod = AttackMethod.Default;
        protected string meleeColumn = "Ear2";
        protected string rangeColumn = "Ear3";

        public DBATTACKMETHODABILITY() { }

        public DBATTACKMETHODABILITY(HabProperties hps)
            : base(hps)
        {
        }

        protected AttackMethod AttackMethod
        {
            get { return attackMethod; }
            set { attackMethod = value; }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            attackMethod = AttackMethod.None;

            /////////////////////////////////////////
            // check if melee attacks are supported
            /////////////////////////////////////////

            DBBIT meleeAttacks = new DBBIT();
            meleeAttacks.Value = hps.GetValue(meleeColumn, false);
            if (meleeAttacks) attackMethod |= AttackMethod.Melee;

            ////////////////////////////////////////
            // check if range attacks are supported
            ////////////////////////////////////////

            DBBIT rangeAttacks = new DBBIT();
            rangeAttacks.Value = hps.GetValue(rangeColumn, false);
            if (rangeAttacks) attackMethod |= AttackMethod.Range;

            if (attackMethod == AttackMethod.None)
                attackMethod = AttackMethod.Default;
        }

        public override bool Apply()
        {
            if (owner is unit && ((owner as unit).attackMethod & attackMethod) != 0)
                return true;
            else
                return false;
        }
    }
    public abstract class DBATTACKDAMAGEINCREASEABILITY : DBATTACKMETHODABILITY
    {
        protected DBDOUBLE damageIncrease = new DBDOUBLE(null);
        protected string damageIncreaseColumn = "DataA";

        public DBATTACKDAMAGEINCREASEABILITY()
        {

        }
        public DBATTACKDAMAGEINCREASEABILITY(HabProperties hps)
            : base(hps)
        {
            meleeColumn = "Ear2";
            rangeColumn = "Ear3";
        }

        public override object Data
        {
            get
            {
                return damageIncrease;
            }
            set
            {
                damageIncrease.Value = value;
            }
        }
        public override double StackDataA
        {
            get
            {
                return damageIncrease;
            }
            set
            {
                damageIncrease.Value = value;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get damage
            /////////////////////////////

            this[damageIncrease] = hps.GetValue(damageIncreaseColumn, false);
        }

        public override bool Apply()
        {
            if (base.Apply())
            {
                unit hero = owner as unit;

                double result = hero.statsDamage.convert_to_double() * this.damageIncrease;

                result = Math.Round(result);

                this[hero.bonus_damage] = (int)result;
                //hero.udpate_dmg();
                return true;
            }
            else
                return false;
        }

        public override int Priority
        {
            get
            {
                return 1;
            }
        }
    }

    public class DBGENERALAURACOMMANDABILITY : DBABILITY
    {
        protected DBDOUBLE damageIncrease = new DBDOUBLE(null);
        protected AttackMethod attackMethod = AttackMethod.Default;
        protected DBBIT isConstantIncrease = new DBBIT();

        protected string damageIncreaseColumn = "DataA";
        protected string meleeColumn = "DataB";
        protected string rangeColumn = "DataC";
        protected string isConstantIncreaseColumn = "DataD";

        public DBGENERALAURACOMMANDABILITY()
        {

        }
        public DBGENERALAURACOMMANDABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get
            {
                return "AOac";
            }
            set
            {
            }
        }

        public override object Data
        {
            get
            {
                return damageIncrease;
            }
            set
            {
                damageIncrease.Value = value;
            }
        }
        public override double StackDataA
        {
            get
            {
                return damageIncrease;
            }
            set
            {
                damageIncrease.Value = value;
            }
        }

        public override DBABILITY GetStackCopy()
        {
            DBGENERALAURACOMMANDABILITY stackCopy = new DBGENERALAURACOMMANDABILITY();

            stackCopy.attackMethod = this.attackMethod;
            this[stackCopy.isConstantIncrease] = this.isConstantIncrease;

            this.CopyAttributesTo(stackCopy, AbilityInfo.stackInfo);

            return stackCopy;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get damage
            /////////////////////////////

            this[damageIncrease] = hps.GetDoubleValue(damageIncreaseColumn);

            // attack method

            attackMethod = AttackMethod.None;

            /////////////////////////////////////////
            // check if melee attacks are supported
            /////////////////////////////////////////

            DBBIT meleeAttacks = new DBBIT();
            meleeAttacks.Value = hps.GetValue(meleeColumn, false);
            if (meleeAttacks) attackMethod |= AttackMethod.Melee;

            ////////////////////////////////////////
            // check if range attacks are supported
            ////////////////////////////////////////

            DBBIT rangeAttacks = new DBBIT();
            rangeAttacks.Value = hps.GetValue(rangeColumn, false);
            if (rangeAttacks) attackMethod |= AttackMethod.Range;

            if (attackMethod == AttackMethod.None)
                attackMethod = AttackMethod.Default;

            /////////////////////////////////
            // get constant/percentage bool
            //////////////////////////////////

            this[isConstantIncrease] = hps.GetIntValue(isConstantIncreaseColumn);
        }

        public override bool Apply()
        {
            if (owner is unit && ((owner as unit).attackMethod & attackMethod) != 0)
            {
                unit hero = owner as unit;

                double result;

                if (isConstantIncrease)
                    result = damageIncrease;
                else
                {
                    result = hero.statsDamage.convert_to_double() * this.damageIncrease;
                    result = Math.Round(result);
                }

                this[hero.bonus_damage] = (int)result;
                //hero.udpate_dmg();
                return true;
            }
            else
                return false;
        }

        public override int Priority
        {
            get
            {
                return 1;
            }
        }
    }
    public class DBAURACOMMANDABILITY : DBATTACKDAMAGEINCREASEABILITY
    {
        public DBAURACOMMANDABILITY()
        {

        }
        public DBAURACOMMANDABILITY(HabProperties hps)
            : base(hps)
        {
            damageIncreaseColumn = "Cac1";
        }

        public override string codeID
        {
            get
            {
                return "ACac";
            }
            set
            {
            }
        }

        public override DBABILITY GetStackCopy()
        {
            DBAURACOMMANDABILITY stackCopy = new DBAURACOMMANDABILITY();

            stackCopy.AttackMethod = this.AttackMethod;

            this.CopyAttributesTo(stackCopy, AbilityInfo.stackInfo);

            return stackCopy;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }
    }
    public class DBAURATRUESHOTABILITY : DBATTACKDAMAGEINCREASEABILITY
    {
        public DBAURATRUESHOTABILITY()
        {

        }
        public DBAURATRUESHOTABILITY(HabProperties hps)
            : base(hps)
        {
            damageIncreaseColumn = "Ear1";
        }

        public override string codeID
        {
            get
            {
                return "AEar";
            }
            set
            {
            }
        }

        public override DBABILITY GetStackCopy()
        {
            DBAURATRUESHOTABILITY stackCopy = new DBAURATRUESHOTABILITY();

            stackCopy.AttackMethod = this.AttackMethod;

            this.CopyAttributesTo(stackCopy, AbilityInfo.stackInfo);

            return stackCopy;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }
    }

    public class DBIARMORABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn;

        public DBIARMORABILITY() { }
        public DBIARMORABILITY(HabProperties hps)
            : base(hps)
        {
            valueColumn = "Idef";
        }
        public DBIARMORABILITY(string column, DBABILITY parent)
            : base(parent)
        {
            valueColumn = column;
            alias = parent.Alias + column;
        }

        public override string codeID
        {
            get { return "AIde"; }
            set { }
        }

        public override bool IsVisible
        {
            get
            {
                return false;
            }
        }

        public override double StackDataA
        {
            get
            {
                return value;
            }
            set
            {
                this[this.value] = value;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.armor] = value;
                return true;
            }

            return false;
        }
    }

    public class DBACTIVEIARMORABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn;

        public DBACTIVEIARMORABILITY() { }
        public DBACTIVEIARMORABILITY(HabProperties hps)
            : base(hps)
        {
            valueColumn = "Idef";
        }

        public override string codeID
        {
            get { return "AIda"; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.armor] = value;
                return true;
            }

            return false;
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override double StackDataA
        {
            get
            {
                return value;
            }
            set
            {
                this.value.Value = value;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
        }
    }

    public class DBSPIKEDCARAPACEABILITY : DBABILITY
    {
        private string defenseBonusColumn = "Uts3";

        public DBSPIKEDCARAPACEABILITY() { }
        public DBSPIKEDCARAPACEABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get { return "AUts"; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            DBIARMORABILITY armor = new DBIARMORABILITY(defenseBonusColumn, this);
            DBSPIKEDCARAPACE_DEFENSE_ABILITY defense = new DBSPIKEDCARAPACE_DEFENSE_ABILITY(this);

            HabProperties hps = GetProps(true);

            armor.refresh(hps);
            defense.refresh(hps);

            return new DBABILITIES(
                armor,
                defense);
        }
    }
    public class DBSPIKEDCARAPACE_DEFENSE_ABILITY : DBABILITY
    {
        private DBDOUBLE returnedDamageFactor = new DBDOUBLE(null);
        private DBDOUBLE receivedDamageFactor = new DBDOUBLE(null);

        private string returnedDamageFactorColumn = "Uts1";
        private string receivedDamageFactorColumn = "Uts2";

        public DBSPIKEDCARAPACE_DEFENSE_ABILITY()
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }
        public DBSPIKEDCARAPACE_DEFENSE_ABILITY(DBABILITY parent)
            : base(parent)
        {
            this.triggerType = AbilityTriggerType.OnDefense;
        }

        public override string codeID
        {
            get { return ""; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            this[returnedDamageFactor] = hps.GetValue(returnedDamageFactorColumn);
            this[receivedDamageFactor] = hps.GetValue(receivedDamageFactorColumn);
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit)
            {
                DBDAMAGE dmg = ar["DAMAGE"] as DBDAMAGE;

                if (dmg != null)
                    if (dmg.AttackType == AttackType.Hero && dmg.DamageType == DamageType.Normal)
                    {
                        DBDAMAGE dmgReturned;

                        if (ar.Contains("DAMAGE_RETURNED"))
                            dmgReturned = ar.GetDbDamage("DAMAGE_RETURNED");
                        else
                            dmgReturned = new DBDAMAGE();

                        ar.SetDbDamage("DAMAGE_RETURNED", dmgReturned + (dmg * returnedDamageFactor));
                    }
            }
        }
    }

    public class DBACTIVEBERSERKABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE ims = new DBDOUBLE(null);
        private string imsColumn;

        private DBDOUBLE ias = new DBDOUBLE(null);
        private string iasColumn;

        private DBDOUBLE hpFactor = new DBDOUBLE(null);
        private string hpFactorColumn;

        public DBACTIVEBERSERKABILITY() { }
        public DBACTIVEBERSERKABILITY(HabProperties hps)
            : base(hps)
        {
            imsColumn = "bsk1";
            iasColumn = "bsk2";
            hpFactorColumn = "bsk3";
        }

        public override string codeID
        {
            get { return "Absk"; }
            set { }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get ias, ims and hpFactor
            /////////////////////////////

            this[ims] = hps.GetValue(imsColumn);
            this[ias] = hps.GetValue(iasColumn);
            this[hpFactor] = hps.GetValue(hpFactorColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.ims] = ims;
                this[hero.ias] = ias;
                this[hero.hpFactor] = hpFactor;

                return true;
            }

            return false;
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override double StackDataA
        {
            get
            {
                return ims;
            }
            set
            {
                ims.Value = value;
            }
        }
        public override double StackDataB
        {
            get
            {
                return ias;
            }
            set
            {
                ias.Value = value;
            }
        }
        public override double StackDataC
        {
            get
            {
                return hpFactor;
            }
            set
            {
                hpFactor.Value = value;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
            this.StackDataB = Math.Max(this.StackDataB, a.StackDataB);
            this.StackDataC = Math.Max(this.StackDataC, a.StackDataC);
        }
    }
    public class DBACTIVEROARABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE damageIncrease = new DBDOUBLE(null);
        private DBINT defenseIncrease = new DBINT(null);
        private DBBIT preferFriendlies = new DBBIT(0);
        private DBBIT preferHostiles = new DBBIT(0);
        private readonly string damageIncreaseColumn = "Roa1";
        private readonly string defenseIncreaseColumn = "Roa2";
        private readonly string preferFriendliesColumn = "Roa6";
        private readonly string preferHostilesColumn = "Roa5";

        public DBACTIVEROARABILITY()
        {
        }
        public DBACTIVEROARABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get { return "Aroa"; }
            set { }
        }

        public override int Priority
        {
            get
            {
                return 1;
            }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[damageIncrease] = hps.GetValue(damageIncreaseColumn);
            this[defenseIncrease] = hps.GetValue(defenseIncreaseColumn);

            this[preferHostiles] = hps.GetValue(preferHostilesColumn);
            this[preferFriendlies] = hps.GetValue(preferFriendliesColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                if (!damageIncrease.IsNull)
                {
                    double result = hero.statsDamage.convert_to_double() * this.damageIncrease;

                    result = Math.Round(result);

                    this[hero.bonus_damage] = (int)result;
                    //hero.udpate_dmg();
                }


                if (!defenseIncrease.IsNull)
                {
                    this[hero.bonus_armor] = defenseIncrease;
                    //hero.update_armor();
                }

                return true;
            }

            return false;
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override double StackDataA
        {
            get
            {
                return damageIncrease;
            }
            set
            {
                damageIncrease.Value = value;
            }
        }
        public override double StackDataB
        {
            get
            {
                return defenseIncrease;
            }
            set
            {
                defenseIncrease.Value = value;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
            this.StackDataB = Math.Max(this.StackDataB, a.StackDataB);
        }
    }
    public class DBACTIVESOULBURNABILITY : DBABILITY
    {
        protected AbilityState abilityState;

        private DBDOUBLE damageAmount = new DBDOUBLE(null);
        private DBDOUBLE damagePeriod = new DBDOUBLE(null);
        private DBDOUBLE damagePenalty = new DBDOUBLE(null);
        private DBDOUBLE moveSpeedReduction = new DBDOUBLE(null);
        private DBDOUBLE attackSpeedReduction = new DBDOUBLE(null);

        private readonly string damageAmountColumn = "Nso1";
        private readonly string damagePeriodColumn = "Nso2";
        private readonly string damagePenaltyColumn = "Nso3";
        private readonly string moveSpeedReductionColumn = "Nso4";
        private readonly string attackSpeedReductionColumn = "Nso5";

        public DBACTIVESOULBURNABILITY()
        {
        }
        public DBACTIVESOULBURNABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override string codeID
        {
            get { return "ANso"; }
            set { }
        }

        public override int Priority
        {
            get
            {
                return 1;
            }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                if ((abilityState & AbilityState.AllActivatedFlags) != 0) return TargetType.Self;
                return base.Targets;
            }
        }

        public override DBABILITIES GetComponents()
        {
            return new DBABILITIES(this);
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[damageAmount] = hps.GetValue(damageAmountColumn);
            this[damagePeriod] = hps.GetValue(damagePeriodColumn);
            this[damagePenalty] = hps.GetValue(damagePenaltyColumn);
            this[moveSpeedReduction] = hps.GetValue(moveSpeedReductionColumn);
            this[attackSpeedReduction] = hps.GetValue(attackSpeedReductionColumn);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                if (!damagePenalty.IsNull)
                {
                    double result = hero.statsDamage.convert_to_double() * this.damagePenalty;

                    result = Math.Round(result);

                    this[hero.bonus_damage] = -(int)result;
                    //hero.udpate_dmg();
                }

                return true;
            }

            return false;
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override double StackDataA
        {
            get
            {
                return damagePenalty;
            }
            set
            {
                damagePenalty.Value = value;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
        }
    }

    public class DBAURADEVOTIONABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn;

        public DBAURADEVOTIONABILITY() { }
        public DBAURADEVOTIONABILITY(HabProperties hps)
            : base(hps)
        {
            valueColumn = "Had1";
        }

        public override string codeID
        {
            get { return "AHad"; }
            set { }
        }
        public override double StackDataA
        {
            get
            {
                return value;
            }
            set
            {
                this.value.Value = value;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);            
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.armor] = value;
                return true;
            }

            return false;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }
    }

    public class DBAURABRILLIANCEABILITY : DBABILITY
    {
        private DBDOUBLE value = new DBDOUBLE(null);
        private string valueColumn;

        public DBAURABRILLIANCEABILITY() { }
        public DBAURABRILLIANCEABILITY(HabProperties hps)
            : base(hps)
        {
            valueColumn = "Hab1";
        }

        public override string codeID
        {
            get { return "AHab"; }
            set { }
        }
        public override double StackDataA
        {
            get
            {
                return value;
            }
            set
            {
                this.value.Value = value;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get value
            /////////////////////////////

            this[value] = hps.GetValue(valueColumn);
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
        }

        public override bool Apply()
        {
            if (owner is unit)
            {
                unit hero = owner as unit;

                this[hero.bmanaRegen] = value;
                return true;
            }

            return false;
        }

        public override bool IsAura
        {
            get
            {
                return true;
            }
        }
    }

    public abstract class DBTRIGGER : DBABILITY
    {
        protected DBDOUBLE chance = new DBDOUBLE(null);

        public DBTRIGGER()
        {
        }
        public DBTRIGGER(HabProperties hps)
            : base(hps)
        {
        }
        public DBTRIGGER(AbilityTriggerType triggerType)
        {
            this.triggerType = triggerType;
        }
        public DBTRIGGER(AbilityTriggerType triggerType, DBABILITY parent)
            : base(parent)
        {
            this.triggerType = triggerType;
        }
        public DBTRIGGER(AbilityTriggerType triggerType, HabProperties hps)
            : base(hps)
        {
            this.triggerType = triggerType;
        }

        public abstract AbilityTriggerSpecs TriggerSpecs
        {
            get;
        }

        public DBDOUBLE Chance
        {
            get { return chance; }
            set { chance.Value = value; }
        }
        public override double StackChance
        {
            get
            {
                return chance;
            }
        }

        public abstract string CommonName
        {
            get;
        }

        public override object GetCopy()
        {
            DBTRIGGER trigger = base.GetCopy() as DBTRIGGER;
            trigger.Chance = this.chance;
            return trigger;
        }

        public bool IsTriggerSpecsOk(AbilityResults ar)
        {
            if (owner is unit)
            {
                if ((TriggerSpecs & AbilityTriggerSpecs.AttacksOnly) != 0)
                {
                    if (ar.GetAbility("NON_ATTACK") != null)
                        return false;
                }

                if ((TriggerSpecs & AbilityTriggerSpecs.Dominating) != 0)
                {
                    if (ar.GetAbility("DOMINATING_ABILITY") == null)
                        ar.SetAbility("DOMINATING_ABILITY", this);
                    else
                        return false;
                }

                return true;
            }

            return false;
        }

        public override int CompareTo(DBABILITY a)
        {
            int result = CaseInsensitiveComparer.DefaultInvariant.Compare(this.Priority, a.Priority);
            if (result != 0) return result;

            if (a is DBTRIGGER && (TriggerSpecs & AbilityTriggerSpecs.InventorySubject) != 0)
            {
                result = CaseInsensitiveComparer.DefaultInvariant.Compare(this.Slot, a.Slot);
                if (result != 0) return result;
            }

            return CaseInsensitiveComparer.DefaultInvariant.Compare((DateTime)a.LearnPoint, (DateTime)this.LearnPoint);
        }
    }

    #region On attack triggers

    public class DBNONATTACKMARK : DBTRIGGER
    {
        public DBNONATTACKMARK()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBNONATTACKMARK(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "None"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.None; }
        }

        public override int Priority
        {
            get
            {
                return -1;
            }
        }

        public override string CommonName
        {
            get
            {
                return null;
            }
        }

        public override object Data
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            components.Add(this);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
        }

        public override object GetCopy()
        {
            return new DBNONATTACKMARK();
        }

        public override void Apply(AbilityResults ar)
        {
            ar.SetAbilityEx("NON_ATTACK", this);
        }
    }

    public class DBBASHABILITY : DBTRIGGER
    {
        private DBDOUBLE bashDamage = new DBDOUBLE(null);
        private DBDOUBLE stunTime = new DBDOUBLE(null);
        private readonly string chanceColumn = "Hbh1";
        private readonly string bashDamageColumn = "Hbh3";
        private readonly string stunTimeColumn = "HeroDur";

        public DBBASHABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
        }
        public DBBASHABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
        }

        public override string codeID
        {
            get { return "AHbh"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Bash";
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE BashDamage
        {
            get { return bashDamage; }
            set { bashDamage.Value = value; }
        }

        public DBDOUBLE StunTime
        {
            get { return stunTime; }
            set { stunTime.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get bash chance
            /////////////////////////////

            this[chance] = hps.GetDoubleValue(chanceColumn) * 0.01;

            /////////////////////////////
            // get bash damage
            /////////////////////////////

            this[bashDamage] = hps.GetDoubleValue(bashDamageColumn);

            /////////////////////////////
            // get stun time
            /////////////////////////////

            this[stunTime] = hps.GetDoubleValue(stunTimeColumn);
        }

        public override object GetCopy()
        {
            DBBASHABILITY bash = base.GetCopy() as DBBASHABILITY;
            bash.BashDamage = this.bashDamage;
            bash.StunTime = this.stunTime;
            return bash;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                switch ((AttackMethod)(owner as unit).attackMethod)
                {
                    case AttackMethod.Melee:
                        ar.SetDamage("DAMAGE_SPELL", ar.GetDamage("DAMAGE_SPELL") + this.bashDamage);
                        ar.sources.Add(this);
                        break;

                    default:
                        if (ar.GetAbility("APPLIED_EFFECT") == null)
                        {
                            ar.SetDamage("ATTACK_BONUS_DAMAGE", ar.GetDamage("ATTACK_BONUS_DAMAGE") + this.bashDamage);
                            ar.SetAbility("APPLIED_EFFECT", this);
                            ar.sources.Add(this);
                        }
                        break;
                }
            }
        }
    }
    public class DBCRITICALSTRIKEABILITY : DBTRIGGER
    {
        private DBDOUBLE critMultiplier = new DBDOUBLE(null);
        private readonly string chanceColumn;
        private readonly string critMultiplierColumn;

        public DBCRITICALSTRIKEABILITY() { }
        public DBCRITICALSTRIKEABILITY(string chanceColumn, string critMultiplierColumn, DBABILITY parent)
            : base(AbilityTriggerType.OnAttack, parent)
        {
            this.chanceColumn = chanceColumn;
            this.critMultiplierColumn = critMultiplierColumn;

            alias = parent.Alias + critMultiplierColumn;
        }
        public DBCRITICALSTRIKEABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            chanceColumn = "Ocr1";
            critMultiplierColumn = "Ocr2";
        }

        public override string codeID
        {
            get { return critMultiplierColumn; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Crit";
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE CritMultiplier
        {
            get { return critMultiplier; }
            set { critMultiplier.Value = value; }
        }

        public Damage GetFullDamage()
        {
            if (this.owner is unit)
            {
                Damage source_damage = (owner as unit).damage;

                double min = source_damage.Min * this.critMultiplier;
                double max = source_damage.Max * this.critMultiplier;

                return new Damage(min, max);
            }
            else
                return new Damage();
        }
        public Damage GetBonusDamage()
        {
            if (this.owner is unit)
            {
                Damage source_damage = (owner as unit).damage;

                double min = source_damage.Min * (this.critMultiplier - 1);
                double max = source_damage.Max * (this.critMultiplier - 1);

                return new Damage(min, max);
            }
            else
                return new Damage();
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get critical strike chance
            // the value must be rounded to nearest 5
            /////////////////////////////

            this[chance] = (Math.Round(hps.GetDoubleValue(chanceColumn) / 5.0) * 5.0) * 0.01;

            /////////////////////////////
            // get critical multiplier
            /////////////////////////////

            this[critMultiplier] = hps.GetDoubleValue(critMultiplierColumn);
        }

        public override object GetCopy()
        {
            DBCRITICALSTRIKEABILITY crit = base.GetCopy() as DBCRITICALSTRIKEABILITY;
            crit.CritMultiplier = this.critMultiplier;
            return crit;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                // a trick to skip this if autocast is enabled
                if (ar.GetAbility("DOMINATING_ABILITY") != null)
                    return;

                // double attackDamage = damage;
                ar.SetDamageEx("ATTACK_DAMAGE", (owner as unit).damage);

                // double currentCrit = 1.0;
                ar.SetDoubleEx("CURRENT_CRIT_MULTIPLIER", 1.0);
                double currentCrit = ar.GetDouble("CURRENT_CRIT_MULTIPLIER");

                // double cm = 1.0;
                ar.SetDoubleEx("CUMULATIVE_CRIT", 1.0);

                if (currentCrit > 1.0) return;

                if (currentCrit != this.critMultiplier)
                {
                    // currentCrit = crit;
                    currentCrit = this.critMultiplier;
                    ar.SetDouble("CURRENT_CRIT_MULTIPLIER", currentCrit);

                    // bonusDamageCalc = attackDamage;
                    ar.SetDamage("ATTACK_DAMAGE_FOR_CRIT", ar.GetDamage("ATTACK_DAMAGE"));

                    // attackDamage = damage;
                    ar.SetDamage("ATTACK_DAMAGE", (owner as unit).damage);

                    // cm = 1.0;
                    ar.SetDouble("CUMULATIVE_CRIT", 1.0);
                }

                // cm *= (currentCrit - 1);
                double cumulativeCrit = ar.GetDouble("CUMULATIVE_CRIT") * (currentCrit - 1);
                ar.SetDouble("CUMULATIVE_CRIT", cumulativeCrit);

                // attackDamage += bonusDamageCalc * cm;
                ar.SetDamage("ATTACK_DAMAGE", ar.GetDamage("ATTACK_DAMAGE") + ar.GetDamage("ATTACK_DAMAGE_FOR_CRIT") * cumulativeCrit);

                // ability effect (bash/crit number/etc...)
                if ((AttackMethod)(owner as unit).attackMethod != AttackMethod.Melee)
                    ar.SetAbilityEx("APPLIED_EFFECT", this);

                ar.sources.Add(this);
            }
        }
    }
    public class DBSEARINGARROWSABILITY : DBTRIGGER
    {
        protected AbilityState abilityState;

        private DBDOUBLE bonusDamage = new DBDOUBLE(null);
        private readonly string bonusDamageColumn = "Hfa1";

        public DBSEARINGARROWSABILITY()
        {
            this[chance] = 1.0;
        }
        public DBSEARINGARROWSABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AHfa"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ?
                    AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.Dominating
                    :
                    AbilityTriggerSpecs.Dominating;
            }
        }

        public override int Priority
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ? 0 : 7;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name + (((abilityState & AbilityState.AutoCast) != 0) ? "(AutoCast)" : "(Cast)");
            }
        }

        public override void AutoCast(bool flag)
        {
            abilityState = flag ? AbilityState.PermanentlyActivated | AbilityState.AutoCast : AbilityState.None;
            IssueOrder(flag ? orderon : orderoff);
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE BonusDamage
        {
            get { return bonusDamage; }
            set { bonusDamage.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
            {
                components.Add(this.GetCopy() as DBABILITY);

                if ((abilityState & AbilityState.AllActivatedFlags) != 0 && (abilityState & AbilityState.AutoCast) == 0)//(this.isActivated && !autocast)
                {
                    DBNONATTACKMARK nam = new DBNONATTACKMARK();
                    this.CopyAttributesTo(nam, AbilityInfo.fullInfo);

                    components.Add(nam);
                }
            }

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[bonusDamage] = hps.GetDoubleValue(bonusDamageColumn);
        }

        public override object GetCopy()
        {
            DBSEARINGARROWSABILITY searingArrow = base.GetCopy() as DBSEARINGARROWSABILITY;
            searingArrow.BonusDamage = this.bonusDamage;
            return searingArrow;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetDouble("CURRENT_CRIT_MULTIPLIER") > 1.0) return;

                ar.SetDamage("ATTACK_DAMAGE", (owner as unit).damage + bonusDamage);

                ar.sources.Add(this);
            }
        }
    }
    public class DBBLACKARROWABILITY : DBTRIGGER
    {
        protected AbilityState abilityState;

        private DBDOUBLE bonusDamage = new DBDOUBLE(null);
        private readonly string bonusDamageColumn = "Nba1";

        public DBBLACKARROWABILITY()
        {
            this[chance] = 1.0;
        }
        public DBBLACKARROWABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "ANba"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ?
                    AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier
                    :
                    AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier;
            }
        }

        public override int Priority
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ? 0 : 7;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name + (((abilityState & AbilityState.AutoCast) != 0) ? "(AutoCast)" : "(Cast)");
            }
        }

        public override void AutoCast(bool flag)
        {
            abilityState = flag ? AbilityState.PermanentlyActivated | AbilityState.AutoCast : AbilityState.None;
            IssueOrder(flag ? orderon : orderoff);
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE BonusDamage
        {
            get { return bonusDamage; }
            set { bonusDamage.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
            {
                components.Add(this.GetCopy() as DBABILITY);

                if ((abilityState & AbilityState.AllActivatedFlags) != 0 && (abilityState & AbilityState.AutoCast) == 0)//(this.isActivated && !autocast)
                {
                    DBNONATTACKMARK nam = new DBNONATTACKMARK();
                    this.CopyAttributesTo(nam, AbilityInfo.fullInfo);

                    components.Add(nam);
                }
            }

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[bonusDamage] = hps.GetDoubleValue(bonusDamageColumn);
        }

        public override object GetCopy()
        {
            DBBLACKARROWABILITY blackArrow = base.GetCopy() as DBBLACKARROWABILITY;
            blackArrow.BonusDamage = this.bonusDamage;
            return blackArrow;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetDouble("CURRENT_CRIT_MULTIPLIER") > 1.0) return;

                ar.SetDamage("ATTACK_DAMAGE", (owner as unit).damage + bonusDamage);

                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }
    public class DBCOLDARROWABILITY : DBTRIGGER
    {
        protected AbilityState abilityState;

        private DBDOUBLE bonusDamage = new DBDOUBLE(null);
        private DBDOUBLE msFactor = new DBDOUBLE(null);
        private DBDOUBLE asFactor = new DBDOUBLE(null);
        private readonly string bonusDamageColumn = "Hca1";
        private readonly string msFactorColumn = "Hca2";
        private readonly string asFactorColumn = "Hca3";

        public DBCOLDARROWABILITY()
        {
            this[chance] = 1.0;
        }
        public DBCOLDARROWABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AHca"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ?
                    AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier
                    :
                    AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier;
            }
        }

        public override int Priority
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ? 0 : 7;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name + (((abilityState & AbilityState.AutoCast) != 0) ? "(AutoCast)" : "(Cast)");
            }
        }

        public override void AutoCast(bool flag)
        {
            abilityState = flag ? AbilityState.PermanentlyActivated | AbilityState.AutoCast : AbilityState.None;
            IssueOrder(flag ? orderon : orderoff);
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE BonusDamage
        {
            get { return bonusDamage; }
            set { bonusDamage.Value = value; }
        }

        public DBDOUBLE MsFactor
        {
            get { return msFactor; }
            set { msFactor.Value = value; }
        }

        public DBDOUBLE AsFactor
        {
            get { return asFactor; }
            set { asFactor.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
            {
                components.Add(this.GetCopy() as DBABILITY);

                if ((abilityState & AbilityState.AllActivatedFlags) != 0 && (abilityState & AbilityState.AutoCast) == 0)//(this.isActivated && !autocast)
                {
                    DBNONATTACKMARK nam = new DBNONATTACKMARK();
                    this.CopyAttributesTo(nam, AbilityInfo.fullInfo);

                    components.Add(nam);
                }
            }

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[bonusDamage] = hps.GetDoubleValue(bonusDamageColumn);
            this[msFactor] = hps.GetDoubleValue(msFactorColumn);
            this[asFactor] = hps.GetDoubleValue(asFactorColumn);
        }

        public override object GetCopy()
        {
            DBCOLDARROWABILITY coldArrow = base.GetCopy() as DBCOLDARROWABILITY;
            coldArrow.BonusDamage = this.bonusDamage;
            coldArrow.MsFactor = this.msFactor;
            coldArrow.AsFactor = this.asFactor;
            return coldArrow;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetDouble("CURRENT_CRIT_MULTIPLIER") > 1.0) return;

                ar.SetDamage("ATTACK_DAMAGE", (owner as unit).damage + bonusDamage);

                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }
    public class DBPOISONARROWABILITY : DBTRIGGER
    {
        protected AbilityState abilityState;

        private DBDOUBLE bonusDamage = new DBDOUBLE(null);
        private DBDOUBLE damagePerSecond = new DBDOUBLE(null);
        private DBDOUBLE asFactor = new DBDOUBLE(null);
        private DBDOUBLE msFactor = new DBDOUBLE(null);
        private readonly string bonusDamageColumn = "Poa1";
        private readonly string damagePerSecondColumn = "Poa2";
        private readonly string asFactorColumn = "Poa3";
        private readonly string msFactorColumn = "Poa4";

        public DBPOISONARROWABILITY()
        {
            this[chance] = 1.0;
        }
        public DBPOISONARROWABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AEpa"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ?
                    AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier
                    :
                    AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier;
            }
        }

        public override int Priority
        {
            get
            {
                return ((abilityState & AbilityState.AutoCast) != 0) ? 0 : 7;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name + (((abilityState & AbilityState.AutoCast) != 0) ? "(AutoCast)" : "(Cast)");
            }
        }

        public override void AutoCast(bool flag)
        {
            abilityState = flag ? AbilityState.PermanentlyActivated | AbilityState.AutoCast : AbilityState.None;
            IssueOrder(flag ? orderon : orderoff);
        }

        public override AbilityState AbilityState
        {
            get
            {
                return abilityState;
            }
            set
            {
                abilityState = value;
            }
        }

        public override bool IsPassive
        {
            get
            {
                return false;
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE BonusDamage
        {
            get { return bonusDamage; }
            set { bonusDamage.Value = value; }
        }

        public DBDOUBLE DamagePerSecond
        {
            get { return damagePerSecond; }
            set { damagePerSecond.Value = value; }
        }

        public DBDOUBLE AsFactor
        {
            get { return asFactor; }
            set { asFactor.Value = value; }
        }

        public DBDOUBLE MsFactor
        {
            get { return msFactor; }
            set { msFactor.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
            {
                components.Add(this.GetCopy() as DBABILITY);

                if ((abilityState & AbilityState.AllActivatedFlags) != 0 && (abilityState & AbilityState.AutoCast) == 0)//(this.isActivated && !autocast)
                {
                    DBNONATTACKMARK nam = new DBNONATTACKMARK();
                    this.CopyAttributesTo(nam, AbilityInfo.fullInfo);

                    components.Add(nam);
                }
            }

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[bonusDamage] = hps.GetDoubleValue(bonusDamageColumn);
            this[damagePerSecond] = hps.GetDoubleValue(damagePerSecondColumn);
            this[asFactor] = hps.GetDoubleValue(asFactorColumn);
            this[msFactor] = hps.GetDoubleValue(msFactorColumn);
        }

        public override object GetCopy()
        {
            DBPOISONARROWABILITY poisonArrow = base.GetCopy() as DBPOISONARROWABILITY;
            poisonArrow.BonusDamage = this.bonusDamage;
            poisonArrow.DamagePerSecond = this.damagePerSecond;
            poisonArrow.AsFactor = this.asFactor;
            poisonArrow.MsFactor = this.msFactor;
            return poisonArrow;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetDouble("CURRENT_CRIT_MULTIPLIER") > 1.0) return;

                ar.SetDamage("ATTACK_DAMAGE", (owner as unit).damage + bonusDamage);

                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }

    public class DBLIFESTEALABILITY : DBTRIGGER
    {
        private DBDOUBLE lifeStealAmount = new DBDOUBLE(null);
        private readonly string lifeStealAmountColumn = "Ivam";

        public DBLIFESTEALABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBLIFESTEALABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AIva"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.InventorySubject; }
        }

        public override int Priority
        {
            get
            {
                return 3;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Lifesteal";
            }
        }

        public override object Data
        {
            get
            {
                return lifeStealAmount;
            }
            set
            {
                lifeStealAmount.Value = value;
            }
        }

        public DBDOUBLE LifeStealAmount
        {
            get { return lifeStealAmount; }
            set { lifeStealAmount.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[lifeStealAmount] = hps.GetDoubleValue(lifeStealAmountColumn);
        }

        public override object GetCopy()
        {
            DBLIFESTEALABILITY lifesteal = base.GetCopy() as DBLIFESTEALABILITY;
            lifesteal.LifeStealAmount = this.lifeStealAmount;
            return lifesteal;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                ar.sources.Add(this);
            }
        }
    }

    public class DBORBOFSLOWABILITY : DBTRIGGER
    {
        private DBDOUBLE damageBonus = new DBDOUBLE(null);
        private DBDOUBLE effectChance = new DBDOUBLE(null);
        private DBABILITY effectAbility = null;
        private readonly string damageBonusColumn = "Idam";
        private readonly string effectChanceColumn = "Iob3";
        private readonly string effectAbilityColumn = "Iobu";

        private AbilityTriggerSpecs orbSpecs = AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier | AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.InventorySubject;

        public DBORBOFSLOWABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBORBOFSLOWABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AIdf"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return orbSpecs; }
        }

        public override int Priority
        {
            get
            {
                return 4;
            }
        }

        public override string CommonName
        {
            get
            {
                return ((TriggerSpecs & AbilityTriggerSpecs.Dominating) == 0) ? effectAbility.Name : null;
            }
        }

        public override object Data
        {
            get
            {
                return effectAbility;
            }
            set
            {
                effectAbility = value as DBABILITY;
            }
        }

        public DBDOUBLE DamageBonus
        {
            get { return damageBonus; }
            set { damageBonus.Value = value; }
        }

        public DBDOUBLE EffectChance
        {
            get { return effectChance; }
            set { effectChance.Value = value; }
        }

        public DBABILITY EffectAbility
        {
            get { return effectAbility; }
            set { effectAbility = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
            {
                DBORBOFSLOWABILITY effectSource = this.GetCopy() as DBORBOFSLOWABILITY;
                effectSource.orbSpecs = AbilityTriggerSpecs.EffectApplier | AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.InventorySubject;

                DBORBOFSLOWABILITY dominatingMark = this.GetCopy() as DBORBOFSLOWABILITY;
                dominatingMark.orbSpecs = AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.InventorySubject;
                dominatingMark.Data = effectSource;
                dominatingMark.chance = 1.0;

                HabProperties hps = GetProps(true);
                DBDAMAGEINCREASEABILITY damageBonusAbility = new DBDAMAGEINCREASEABILITY(damageBonusColumn, this);
                damageBonusAbility.refresh(hps);

                components.AddRange(dominatingMark, effectSource, damageBonusAbility);
            }

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[damageBonus] = hps.GetDoubleValue(damageBonusColumn);

            this[effectChance] = hps.GetDoubleValue(effectChanceColumn) * 0.01;

            this[chance] = this.effectChance;

            effectAbility = DBABILITIES.getAbility(hps.GetStringValue(effectAbilityColumn));
        }

        public override object GetCopy()
        {
            DBORBOFSLOWABILITY orbOfSlow = base.GetCopy() as DBORBOFSLOWABILITY;
            orbOfSlow.DamageBonus = this.damageBonus;
            orbOfSlow.EffectChance = this.effectChance;
            orbOfSlow.EffectAbility = this.effectAbility;
            return orbOfSlow;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if ((TriggerSpecs & AbilityTriggerSpecs.Dominating) == 0)
                {
                    DBABILITY a = ar.GetAbility("DOMINATING_ABILITY");

                    if (a != null && a.Data == this)
                        if (ar.GetAbility("APPLIED_EFFECT") == null)
                        {
                            ar.SetAbility("APPLIED_EFFECT", effectAbility);
                            ar.sources.Add(this);
                        }
                }
            }
        }

        public override int CompareTo(DBABILITY a)
        {
            int result = base.CompareTo(a);
            if (result != 0) return result;
            else
            {
                if (this == a) return 0;
                else
                    return ((TriggerSpecs & AbilityTriggerSpecs.Dominating) != 0) ? -1 : 1;
            }
        }
    }
    public class DBORBOFCORRUPTIONABILITY : DBTRIGGER
    {
        private DBDOUBLE armorPenalty = new DBDOUBLE(null);
        private readonly string armorPenaltyColumn = "Iarp";

        public DBORBOFCORRUPTIONABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBORBOFCORRUPTIONABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AIcb"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier | AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.InventorySubject; }
        }

        public override int Priority
        {
            get
            {
                return 4;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Corruption";
            }
        }

        public override object Data
        {
            get
            {
                return armorPenalty;
            }
            set
            {
                armorPenalty.Value = value;
            }
        }

        public DBDOUBLE ArmorPenalty
        {
            get { return armorPenalty; }
            set { armorPenalty.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[armorPenalty] = hps.GetDoubleValue(armorPenaltyColumn);
        }

        public override object GetCopy()
        {
            DBORBOFCORRUPTIONABILITY orbOfCorruption = base.GetCopy() as DBORBOFCORRUPTIONABILITY;
            orbOfCorruption.ArmorPenalty = this.armorPenalty;
            return orbOfCorruption;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }
    public class DBORBOFFROSTABILITY : DBTRIGGER
    {
        //FrostAttackSpeedDecrease
        //FrostMoveSpeedDecrease            

        public DBORBOFFROSTABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBORBOFFROSTABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "AIob"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier | AbilityTriggerSpecs.AttacksOnly | AbilityTriggerSpecs.InventorySubject; }
        }

        public override int Priority
        {
            get
            {
                return 4;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Frost(melee)";
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);
        }

        public override object GetCopy()
        {
            DBORBOFFROSTABILITY orbOfFrost = base.GetCopy() as DBORBOFFROSTABILITY;
            return orbOfFrost;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }

    public class DBFEEDBACKABILITY : DBTRIGGER
    {
        private DBDOUBLE maxManaDrained = new DBDOUBLE(null);
        private DBDOUBLE damageRatio = new DBDOUBLE(null);
        private readonly string maxManaDrainedColumn = "fbk1";
        private readonly string damageRatioColumn = "fbk4";

        public DBFEEDBACKABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBFEEDBACKABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "Afbk"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.Dominating | AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 6;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Feedback";
            }
        }

        public override object Data
        {
            get
            {
                return damageRatio;
            }
            set
            {
                damageRatio.Value = value;
            }
        }

        public DBDOUBLE MaxManaDrained
        {
            get { return maxManaDrained; }
            set { maxManaDrained.Value = value; }
        }

        public DBDOUBLE DamageRatio
        {
            get { return damageRatio; }
            set { damageRatio.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[maxManaDrained] = hps.GetDoubleValue(maxManaDrainedColumn);
            this[damageRatio] = hps.GetDoubleValue(damageRatioColumn);
        }

        public override object GetCopy()
        {
            DBFEEDBACKABILITY feedback = base.GetCopy() as DBFEEDBACKABILITY;
            feedback.MaxManaDrained = this.maxManaDrained;
            feedback.DamageRatio = this.damageRatio;
            return feedback;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.Contains("APPLIED_EFFECT") == false)
                {
                    if ((AttackMethod)(owner as unit).attackMethod != AttackMethod.Melee)
                        ar.SetAbility("APPLIED_EFFECT", this);

                    ar.SetDamage("ATTACK_BONUS_DAMAGE", ar.GetDamage("ATTACK_BONUS_DAMAGE") + (maxManaDrained * damageRatio));
                    ar.sources.Add(this);
                }
            }
        }
    }
    public class DBBARRAGEABILITY : DBTRIGGER
    {
        private DBDOUBLE numberOfTargets = new DBDOUBLE(null);
        private DBDOUBLE maxTotalDamage = new DBDOUBLE(null);
        private readonly string numberOfTargetsColumn = "Efk3";
        private readonly string maxTotalDamageColumn = "Efk2";

        public DBBARRAGEABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBBARRAGEABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "Aroc"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 6;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name;
            }
        }

        public override object Data
        {
            get
            {
                return numberOfTargets;
            }
            set
            {
                numberOfTargets.Value = value;
            }
        }

        public DBDOUBLE NumberOfTargets
        {
            get { return numberOfTargets; }
            set { numberOfTargets.Value = value; }
        }

        public DBDOUBLE MaxTotalDamage
        {
            get { return maxTotalDamage; }
            set { maxTotalDamage.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[numberOfTargets] = hps.GetDoubleValue(numberOfTargetsColumn);
            this[maxTotalDamage] = hps.GetDoubleValue(maxTotalDamageColumn);
        }

        public override object GetCopy()
        {
            DBBARRAGEABILITY barrage = base.GetCopy() as DBBARRAGEABILITY;
            barrage.NumberOfTargets = this.numberOfTargets;
            barrage.MaxTotalDamage = this.maxTotalDamage;
            return barrage;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.Contains("APPLIED_EFFECT") == false)
                {
                    if ((AttackMethod)(owner as unit).attackMethod != AttackMethod.Melee)
                    {
                        ar.SetAbility("APPLIED_EFFECT", null);
                        ar.sources.Add(this);
                    }
                }
            }
        }
    }
    public class DBPOISONSTINGABILITY : DBTRIGGER
    {
        private DBDOUBLE damagePerSecond = new DBDOUBLE(null);
        private DBDOUBLE attackSpeedFactor = new DBDOUBLE(null);
        private DBDOUBLE moveSpeedFactor = new DBDOUBLE(null);
        private readonly string damagePerSecondColumn = "Poi1";
        private readonly string attackSpeedFactorColumn = "Poi2";
        private readonly string moveSpeedFactorColumn = "Poi3";

        public DBPOISONSTINGABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBPOISONSTINGABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "Apoi"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 6;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name;
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[damagePerSecond] = hps.GetValue(damagePerSecondColumn);
            this[attackSpeedFactor] = hps.GetValue(attackSpeedFactorColumn);
            this[moveSpeedFactor] = hps.GetValue(moveSpeedFactorColumn);
        }

        public override object GetCopy()
        {
            DBPOISONSTINGABILITY poisonSting = base.GetCopy() as DBPOISONSTINGABILITY;
            return poisonSting;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }
    public class DBSLOWPOISONABILITY : DBTRIGGER
    {
        private DBDOUBLE damagePerSecond = new DBDOUBLE(null);
        private DBDOUBLE moveSpeedFactor = new DBDOUBLE(null);
        private DBDOUBLE attackSpeedFactor = new DBDOUBLE(null);
        private readonly string damagePerSecondColumn = "Spo1";
        private readonly string moveSpeedFactorColumn = "Spo2";
        private readonly string attackSpeedFactorColumn = "Spo3";

        public DBSLOWPOISONABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBSLOWPOISONABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "Aspo"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 6;
            }
        }

        public override string CommonName
        {
            get
            {
                return this.Name;
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            this[damagePerSecond] = hps.GetValue(damagePerSecondColumn);
            this[attackSpeedFactor] = hps.GetValue(attackSpeedFactorColumn);
            this[moveSpeedFactor] = hps.GetValue(moveSpeedFactorColumn);
        }

        public override object GetCopy()
        {
            DBSLOWPOISONABILITY poisonSting = base.GetCopy() as DBSLOWPOISONABILITY;
            return poisonSting;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }

    public class DBFROSTATTACKABILITY : DBTRIGGER
    {
        //FrostAttackSpeedDecrease
        //FrostMoveSpeedDecrease            

        public DBFROSTATTACKABILITY()
            : base(AbilityTriggerType.OnAttack)
        {
            this[chance] = 1.0;
        }
        public DBFROSTATTACKABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnAttack, hps)
        {
            this[chance] = 1.0;
        }

        public override string codeID
        {
            get { return "Afra"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.EffectApplier; }
        }

        public override int Priority
        {
            get
            {
                return 8;
            }
        }

        public override string CommonName
        {
            get
            {
                return "Frost(range)";
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);
        }

        public override object GetCopy()
        {
            DBFROSTATTACKABILITY frostAttack = base.GetCopy() as DBFROSTATTACKABILITY;
            return frostAttack;
        }

        public override void Apply(AbilityResults ar)
        {
            if (IsTriggerSpecsOk(ar))
            {
                if (ar.GetAbility("APPLIED_EFFECT") == null)
                {
                    ar.SetAbility("APPLIED_EFFECT", this);
                    ar.sources.Add(this);
                }
            }
        }
    }

    #endregion

    public class DBEVASIONABILITY : DBTRIGGER
    {
        private string chanceColumn;

        public DBEVASIONABILITY() { }
        public DBEVASIONABILITY(string column, DBABILITY parent)
            : base(AbilityTriggerType.OnDefense, parent)
        {
            chanceColumn = column;
            alias = parent.Alias + column;
        }
        public DBEVASIONABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnDefense, hps)
        {
            chanceColumn = "Eev1";
        }

        public override string codeID
        {
            get { return "AEev"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.None; }
        }

        public override DBTARGETTYPE Targets
        {
            get
            {
                return TargetType.None;
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get evasion
            /////////////////////////////

            this[chance] = hps.GetDoubleValue(chanceColumn);
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public override string CommonName
        {
            get
            {
                return "evasion";
            }
        }

        public override double StackDataA
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public override bool Stackable
        {
            get
            {
                return true;
            }
        }

        public override void Stack(DBABILITY a)
        {
            this.StackDataA = Math.Max(this.StackDataA, a.StackDataA);
        }

        public override DBABILITY GetStackCopy()
        {
            DBEVASIONABILITY stackCopy = new DBEVASIONABILITY();

            this.CopyAttributesTo(stackCopy, AbilityInfo.stackInfo | AbilityInfo.TriggerType);

            return stackCopy;
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit && ar.GetInt("IS_ATTACK") == 1)
            {
                DBDAMAGE dmg = ar["DAMAGE"] as DBDAMAGE;

                if (dmg != null)
                    ar.SetDbDamage("DAMAGE", 0);
            }
        }
    }

    public class DBBLOCKDAMAGEABILITY : DBTRIGGER
    {
        private DBDOUBLE blockDamage = new DBDOUBLE(null);
        private readonly string chanceColumn = "Ssk1";
        private readonly string blockDamageColumn = "Ssk3";
        //private readonly string meleeAttacksColumn = "Ssk4"; // not implemented
        //private readonly string rangeAttacksColumn = "Ssk5"; // not implemented

        public DBBLOCKDAMAGEABILITY() : base(AbilityTriggerType.OnDefense) { }
        public DBBLOCKDAMAGEABILITY(HabProperties hps)
            : base(AbilityTriggerType.OnDefense, hps)
        {
        }

        public override string codeID
        {
            get { return "Assk"; }
            set { }
        }

        public override AbilityTriggerSpecs TriggerSpecs
        {
            get { return AbilityTriggerSpecs.None; }
        }

        public override string CommonName
        {
            get
            {
                return "block";
            }
        }

        public override object Data
        {
            get
            {
                return chance;
            }
            set
            {
                chance.Value = value;
            }
        }

        public DBDOUBLE BlockDamage
        {
            get { return blockDamage; }
            set { blockDamage.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this.GetCopy() as DBABILITY);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get chance                
            /////////////////////////////

            this[chance] = hps.GetDoubleValue(chanceColumn) * 0.01;

            /////////////////////////////
            // get block damage
            /////////////////////////////

            this[blockDamage] = hps.GetDoubleValue(blockDamageColumn);
        }

        public override int Priority
        {
            get
            {
                return 1;
            }
        }

        public override object GetCopy()
        {
            DBBLOCKDAMAGEABILITY block = base.GetCopy() as DBBLOCKDAMAGEABILITY;
            block.BlockDamage = this.blockDamage;
            return block;
        }

        public override void Apply(AbilityResults ar)
        {
            if (owner is unit)
            {
                DBDAMAGE dmg = ar["DAMAGE"] as DBDAMAGE;

                if (dmg != null && dmg != 0)
                    switch (dmg.DamageType)
                    {
                        case DamageType.Normal:
                            if (ar.Contains("BLOCKED"))
                                break;
                            ar["DAMAGE"] = dmg - Math.Min((double)dmg, (double)blockDamage);
                            ar["BLOCKED"] = true;
                            break;
                    }
            }
        }
    }

    public class DBCRITANDEVASIONABILITY : DBABILITY
    {
        DBCRITICALSTRIKEABILITY crit;
        DBEVASIONABILITY evasion;

        public DBCRITANDEVASIONABILITY()
        {
        }
        public DBCRITANDEVASIONABILITY(HabProperties hps)
            : base(hps)
        {
            crit = new DBCRITICALSTRIKEABILITY("Ocr1", "Ocr2", this);
            evasion = new DBEVASIONABILITY("Ocr4", this);
        }

        public override string codeID
        {
            get { return "AOcr"; }
            set { }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0) components.AddRange(crit, evasion);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            crit.refresh(hps);
            evasion.refresh(hps);
        }
    }

    public abstract class DBOVERTIMEABILITY : DBABILITY
    {
        private DBDOUBLE timeInterval = new DBDOUBLE(null);
        protected string timeIntervalColumn = "HeroDur";
        protected bool canUseDefaultInterval = true;

        public DBOVERTIMEABILITY()
        {
        }

        public DBOVERTIMEABILITY(HabProperties hps)
            : base(hps)
        {
        }

        public override object Data
        {
            get
            {
                return timeInterval;
            }
            set
            {
                timeInterval.Value = value;
            }
        }

        public DBDOUBLE TimeInterval
        {
            get { return timeInterval; }
            set { timeInterval.Value = value; }
        }

        public override DBABILITIES GetComponents()
        {
            DBABILITIES components = new DBABILITIES();

            if (level != 0)
                components.Add(this);

            return components;
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get time interval
            /////////////////////////////

            this[timeInterval] = hps.GetValue(timeIntervalColumn, false);

            if (timeInterval.IsNull && canUseDefaultInterval)
                this[timeInterval] = 1.0;
        }

        public override object GetCopy()
        {
            DBOVERTIMEABILITY overtimeAbility = base.GetCopy() as DBOVERTIMEABILITY;
            overtimeAbility.TimeInterval = this.TimeInterval;
            return overtimeAbility;
        }
    }
    public class DBIMMOLIATIONABILITY : DBOVERTIMEABILITY
    {
        protected DBDOUBLE damageAmount = new DBDOUBLE(null);
        protected string damageAmountColumn = "Eim1";

        public DBIMMOLIATIONABILITY()
        {
            this.benefitType = AbilityBenefitType.SpellDamage;
        }
        public DBIMMOLIATIONABILITY(HabProperties hps)
            : base(hps)
        {
            this.benefitType = AbilityBenefitType.SpellDamage;
        }

        public override string codeID
        {
            get
            {
                return "AEim";
            }
            set
            {
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get damage
            /////////////////////////////

            this[damageAmount] = hps.GetValue(damageAmountColumn, false);
        }

        public override void Apply(AbilityResults ar)
        {
            ar.SetDouble("DPS_SPELL", damageAmount / TimeInterval);
        }
    }
    public class DBTORNADODAMAGEABILITY : DBOVERTIMEABILITY
    {
        protected DBDOUBLE damageAmount = new DBDOUBLE(null);
        protected string damageAmountColumn = "Tdg1";

        public DBTORNADODAMAGEABILITY()
        {
            this.benefitType = AbilityBenefitType.SpellDamage;
        }
        public DBTORNADODAMAGEABILITY(HabProperties hps)
            : base(hps)
        {
            this.benefitType = AbilityBenefitType.SpellDamage;
        }

        public override string codeID
        {
            get
            {
                return "Atdg";
            }
            set
            {
            }
        }

        public override void refresh(HabProperties hps)
        {
            base.refresh(hps);

            /////////////////////////////
            // get damage
            /////////////////////////////

            this[damageAmount] = hps.GetValue(damageAmountColumn, false);
        }

        public override void Apply(AbilityResults ar)
        {
            ar.SetDouble("DPS_SPELL", damageAmount / TimeInterval);
        }
    }

    #region pragma disable
#pragma warning restore 660
    #endregion
}


