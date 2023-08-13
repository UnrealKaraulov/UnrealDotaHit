using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.CodeDom;
using System.Reflection;
using DotaHIT.Jass.Operations;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;
using DotaHIT.Jass.Commands;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Constants;
using System.Collections.Specialized;
using System.Collections;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Core.Resources.Media;
using System.Windows.Forms;
using System.Threading;

namespace DotaHIT.Jass.Native.Types
{
    public class RecordsKnowledge
    {
        public static Dictionary<Type, Dictionary<string, IField>> TypeFieldCollectionPairs = new Dictionary<Type, Dictionary<string, IField>>();
        public static Dictionary<Type, Dictionary<string, string>> TypeSubstCacheCollectionPairs = new Dictionary<Type, Dictionary<string, string>>();

        static RecordsKnowledge()
        {
            CollectRecordFields();
        }
        public static void WakeUp() { }

        public static void CollectRecordFields()
        {
            Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

            StringCollection sc = new StringCollection();

            sc.Add("DotaHIT.Jass.Native.Types");

            Type[] types = m.FindTypes(new TypeFilter(RecordFieldsSearchCriteria), sc);

            // make ITEM first in the list for faster processing
            for (int i = 0; i < types.Length; i++)
                if (types[i].Name == "item")
                {
                    Type tmp = types[0];
                    types[0] = types[1];
                    types[1] = tmp;
                    break;
                }

            TypeFieldCollectionPairs = new Dictionary<Type, Dictionary<string, IField>>();

            object[] args = new object[] { true };

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    IRecord rsr = types[i].InvokeMember(null,
                            BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
                            BindingFlags.Instance | BindingFlags.CreateInstance,
                            null, null, args) as IRecord;

                    Dictionary<string, IField> units = new Dictionary<string, IField>(rsr.Fields.Count);
                    foreach (IField dbu in rsr.Fields)
                        units.Add(dbu.Name, dbu);

                    TypeFieldCollectionPairs.Add(types[i], units);
                    TypeSubstCacheCollectionPairs.Add(types[i], new Dictionary<string, string>());
                }
                catch { }
            }
        }
        private static bool RecordFieldsSearchCriteria(Type t, object filter)
        {
            if (((StringCollection)filter).Contains(t.Namespace) && t.IsPublic
                && t.IsSubclassOf(typeof(widget)) && !t.IsAbstract)
                return true;
            return false;
        }
    }

    public class MonitoredList<T> : List<T>
    {
        bool updated = false;
        byte updateCounter = 0;

        public MonitoredList() { }
        public MonitoredList(int capacity) : base(capacity) { }
        public MonitoredList(IEnumerable<T> collection) : base(collection) { }

        public new void Add(T item)
        {
            base.Add(item);
            OnListChanged();
        }

        public void AddEx(T item)
        {
            if (!base.Contains(item))
            {
                base.Add(item);
                OnListChanged();
            }
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            OnListChanged();
        }

        public new void Clear()
        {
            base.Clear();
            OnListChanged();
        }

        public new bool Remove(T item)
        {
            if (base.Remove(item))
            {
                OnListChanged();
                return true;
            }
            else
                return false;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnListChanged();
        }

        public event MethodInvoker ListChanged = null;
        void OnListChanged()
        {
            updated = true;

            if (ListChanged != null && updateCounter == 0)
                ListChanged();
        }

        public void BeginUpdate() { updateCounter++; }
        public void EndUpdate() { updateCounter--; }
        public bool IsUpdating
        {
            get { return updateCounter != 0; }
        }

        public bool Updated
        {
            get { return updated; }
            set { updated = value; }
        }
    }

    public class tech
    {
        public readonly string id;
        public int max;
        public int count;
        public tech(string id) { this.id = id; }
    }

    public abstract class handlevalue
    {
        object _syncRoot;
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    _syncRoot = new object();

                return _syncRoot;
            }
        }

        public int handle;        
        public handlevalue()
        {
            handle = DHJassHandleEngine.AddNewHandle(this);
        }
        public virtual void destroy()
        {
            if (handle != 0)
            {
                DHJassHandleEngine.RemoveHandle(handle);
                this.handle = 0;
            }
        }
        public bool IsDisposed
        {
            get
            {
                return handle == 0;
            }
        }
        ~handlevalue()
        {
            if (this.handle != 0)
            {
                this.handle = 0;
                this.destroy();
            }
        }        
    }
    public class player : handlevalue
    {
        public static player[] players = null;
        public static player defaultPlayer = null;
        static player()
        {
            //Reset();
        }
        public static void Reset()
        {
            DHJassValue jMaxPlayers;
            if (DHJassExecutor.War3Globals.TryGetValue("bj_MAX_PLAYER_SLOTS", out jMaxPlayers))
            {
                int max_players = jMaxPlayers.IntValue;
                players = new player[max_players];
                for (int i = 0; i < max_players; i++)
                    players[i] = new player(i);//, i==1);                            

                defaultPlayer = players[players.Length - 1];
            }
            else
                players = new player[0];
        }

        int number; // number of player (0-15)
        public int team = -1;
        public string name;
        protected int gold;
        public int goldGathered = 0;
        public int lumber;
        public int foodUsed;
        public bool playing = true;
        public int pController = 5; // MAP_CONTROL_NONE (common.j)
        public int observer = 0;
        public int givesBounty = 0;
        protected bool acceptMessages = true;

        bool[,] alliance = new bool[players.Length, 10];

        public Dictionary<int, unit> units = new Dictionary<int, unit>();
        public Dictionary<string, tech> techs = new Dictionary<string, tech>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, bool> abilsAvail = new Dictionary<string, bool>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

        public MonitoredList<unit> selection = new MonitoredList<unit>();

        public player(int number)
        {
            this.number = number;
        }
        public player(int number, bool playing)
        {
            this.number = number;
            this.playing = playing;
        }

        public int get_id()
        {
            return number;
        }

        public void add_unit(unit u)
        {
            if (!units.ContainsKey(u.handle))
                add_registered_unit(u);
        }
        public unit add_unit(int unitID)
        {
            unit u = new unit(unitID);
            u.set_owningPlayer(this);
            //add_registered_unit(u);
            return u;
        }
        public unit add_unit(string unitID)
        {
            unit u = new unit(unitID);
            u.set_owningPlayer(this);
            //add_registered_unit(u);
            return u;
        }

        protected void add_registered_unit(unit u)
        {
            lock ((units as ICollection).SyncRoot)
                units.Add(u.handle, u);

            u.death += OnUnitDeath;
            u.sell += OnUnitSell;
            u.sell_item += OnUnitSellItem;
            u.pickup_item += OnUnitPickupItem;
            u.drop_item += OnUnitDropItem;
            u.use_item += OnUnitUseItem;
            u.summon += OnUnitSummon;
            u.hero_skill += OnHeroSkill;
            u.spell_cast += OnUnitSpellCast;
            u.spell_effect += OnUnitSpellEffect;
            u.issued_order += OnUnitIssuedOrder;

            if (u.DoSummon) { u.OnSummon(); u.DoSummon = false; }
        }
        public void remove_unit(unit u)
        {
            selection.Remove(u);

            lock ((units as ICollection).SyncRoot)
                units.Remove(u.handle);

            u.death -= OnUnitDeath;
            u.sell -= OnUnitSell;
            u.sell_item -= OnUnitSellItem;
            u.pickup_item -= OnUnitPickupItem;
            u.drop_item -= OnUnitDropItem;
            u.use_item -= OnUnitUseItem;
            u.summon -= OnUnitSummon;
            u.hero_skill -= OnHeroSkill;
            u.spell_cast -= OnUnitSpellCast;
            u.spell_effect -= OnUnitSpellEffect;
            u.issued_order -= OnUnitIssuedOrder;
        }

        public void setTechResearched(int techid, int setToLevel)
        {
            string id = DHJassInt.int2id(techid);

            tech t;
            if (!techs.TryGetValue(id, out t))
                techs.Add(id, t = new tech(id));

            t.count = setToLevel;

            refreshUnitsWithTech(t);
        }
        public void setTechResearched(string techid, int setToLevel)
        {
            tech t;
            if (!techs.TryGetValue(techid, out t))
                techs.Add(techid, t = new tech(techid));

            t.count = setToLevel;

            refreshUnitsWithTech(t);
        }
        private void refreshUnitsWithTech(tech tech)
        {
            DBUPGRADE upgrade;

            lock ((units as ICollection).SyncRoot)
                foreach (unit u in this.units.Values)
                    if ((upgrade = u.upgrades.GetByCodeID(tech.id)) != null)
                        upgrade.Level = tech.count;
        }

        public void setTechMaxAllowed(int techid, int maximum)
        {
            string id = DHJassInt.int2id(techid);

            tech t;
            if (!techs.TryGetValue(id, out t))
                techs.Add(id, t = new tech(id));

            t.max = maximum;
        }
        public void setTechMaxAllowed(string techid, int maximum)
        {
            tech t;
            if (!techs.TryGetValue(techid, out t))
                techs.Add(techid, t = new tech(techid));

            t.max = maximum;
        }

        public int getTechMaxAllowed(int techid)
        {
            string id = DHJassInt.int2id(techid);

            tech t;
            if (techs.TryGetValue(id, out t))
                return t.max;

            return 0;
        }
        public int getTechMaxAllowed(string techid)
        {
            tech t;
            if (techs.TryGetValue(techid, out t))
                return t.max;

            return 0;
        }

        public int getTechCount(int techid)
        {
            string id = DHJassInt.int2id(techid);

            tech t;
            if (techs.TryGetValue(id, out t))
                return t.count;

            return 0;
        }
        public int getTechCount(string techid)
        {
            tech t;
            if (techs.TryGetValue(techid, out t))
                return t.count;

            return 0;
        }

        public void setAbilityAvaiable(int abilid, bool avail)
        {
            setAbilityAvaiable(DHJassInt.int2id(abilid), avail);
        }
        public void setAbilityAvaiable(string abilid, bool avail)
        {
            abilsAvail[abilid] = avail;

            lock ((units as ICollection).SyncRoot)
                foreach (unit u in this.units.Values)
                {
                    u.abilities.SetAbilityAvailable(abilid, avail);
                    u.heroAbilities.SetAbilityAvailable(abilid, avail);
                }
        }

        public bool getAbilityAvaiable(string abilid)
        {
            if (abilid == null) 
                return false;

            bool value;
            if (abilsAvail.TryGetValue(abilid, out value))
                return value;

            return true;
        }

        public void setAlliance(player p, int alliancetype, bool value)
        {
            alliance[p.get_id(), alliancetype] = value;
        }
        public bool getAlliance(player p, int alliancetype)
        {
            return alliance[p.get_id(), alliancetype];
        }

        public int Gold
        {
            get
            {
                return gold;
            }

            set
            {
                gold = value;
                OnGoldChanged(null, new DHJassEventArgs());
            }
        }

        public event DHJassEventHandler unit_death;
        protected void OnUnitDeath(object sender, DHJassEventArgs e)
        {
            if (unit_death != null)
            {
                e.args["player"] = this;
                unit_death(this, e);
            }
        }

        public event DHJassEventHandler chat;
        public void OnChat(string s)
        {
            if (chat != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["player"] = this;
                e.args["chatString"] = s;

                chat(this, e);
            }
        }

        public event DHJassEventHandler message;
        public void OnMessage(string msg)
        {
            if (message != null && acceptMessages)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["messageString"] = msg;

                message(this, e);
            }
        }
        public bool AcceptMessages
        {
            get { return acceptMessages; }
            set { acceptMessages = value; }
        }

        public event DHJassEventHandler unit_sell;
        protected void OnUnitSell(object sender, DHJassEventArgs e)
        {
            if (unit_sell != null)
            {
                e.args["player"] = this;
                unit_sell(this, e);
            }
        }

        public event DHJassEventHandler unit_sell_item;
        protected void OnUnitSellItem(object sender, DHJassEventArgs e)
        {
            if (unit_sell_item != null)
            {
                e.args["player"] = this;
                unit_sell_item(this, e);
            }
        }

        public event DHJassEventHandler unit_pickup_item;
        protected void OnUnitPickupItem(object sender, DHJassEventArgs e)
        {
            if (unit_pickup_item != null)
            {
                e.args["player"] = this;
                unit_pickup_item(this, e);
            }
        }

        public event DHJassEventHandler unit_drop_item;
        protected void OnUnitDropItem(object sender, DHJassEventArgs e)
        {
            if (unit_drop_item != null)
            {
                e.args["player"] = this;
                unit_drop_item(this, e);
            }
        }

        public event DHJassEventHandler unit_use_item;
        protected void OnUnitUseItem(object sender, DHJassEventArgs e)
        {
            if (unit_use_item != null)
            {
                e.args["player"] = this;
                unit_use_item(this, e);
            }
        }

        public event DHJassEventHandler unit_summon;
        protected void OnUnitSummon(object sender, DHJassEventArgs e)
        {
            if (unit_summon != null)
            {
                e.args["player"] = this;
                unit_summon(this, e);
            }
        }

        public event DHJassEventHandler gold_changed;
        protected void OnGoldChanged(object sender, DHJassEventArgs e)
        {
            if (gold_changed != null)
            {
                e.args["player"] = this;
                gold_changed(this, e);
            }
        }

        public event DHJassEventHandler hero_skill;
        protected void OnHeroSkill(object sender, DHJassEventArgs e)
        {
            if (hero_skill != null)
            {
                e.args["player"] = this;
                hero_skill(this, e);
            }
        }

        public event DHJassEventHandler unit_spell_cast;
        protected void OnUnitSpellCast(object sender, DHJassEventArgs e)
        {
            if (unit_spell_cast != null)
            {
                e.args["player"] = this;
                unit_spell_cast(this, e);
            }
        }

        public event DHJassEventHandler unit_spell_effect;
        protected void OnUnitSpellEffect(object sender, DHJassEventArgs e)
        {
            if (unit_spell_cast != null)
            {
                e.args["player"] = this;
                unit_spell_effect(this, e);
            }
        }

        public event DHJassEventHandler unit_issued_order;
        protected void OnUnitIssuedOrder(object sender, DHJassEventArgs e)
        {
            if (unit_issued_order != null)
            {
                e.args["player"] = this;
                unit_issued_order(this, e);
            }
        }
    }
    public abstract class widget : handlevalue, IRecord
    {
        #region variables
        [Substitute("Name")]
        [Trim('"')]
        public DBCHAR ID = null;

        public DBCHAR codeID = null;

        [Substitute("tip")]
        public DBCHAR tip = null;

        [Substitute("Ubertip")]
        public DBCHAR description = null;

        [Substitute("Art")]
        public DBCHAR iconName = null;
        public DBIMAGE iconImage = null;

        [Substitute("Buttonpos")]
        public DBINTCOLLECTON buttonPos = null;

        [Substitute("goldcost")]
        public DBINT goldCost = null;

        [Substitute("lumbercost")]
        public DBINT lumberCost = null;

        [Substitute("abilList")]
        public DBABILITIES abilities = null;

        public DBDOUBLE hp = null;
        public DBDOUBLE mana = null;

        [Substitute("AIml,DataA")]
        public DBSMARTINT max_hp = null;
        [Substitute("AImm,DataA")]
        public DBSMARTINT max_mana = null;

        [Substitute("targType")]
        public DBTARGETTYPE targetType = null;

        protected int abilitiesOnCooldown;
        protected int activatedAbilities;

        public double x = map.minX - 100; // initial x is outside the map
        public double y = map.minY - 100; // initial y is outside the map
        public double face;
        #endregion

        protected player owningPlayer = player.defaultPlayer;
        public int userData;

        protected HabProperties hpsInitialData = new HabProperties();
        public HabProperties hpsCustomData = new HabProperties();

        protected FieldCollection fields;

        /// <summary>
        /// определяется какие поля были объявлены в структуре
        /// и заносит их в массив units, предварительно указав их имена и атрибуты (через конструктор DbUnit).
        /// предполагается что все объявленные поля (критерий поиска полей указан в коде) типа DbUnit
        /// </summary>
        public widget()
        {
            Dictionary<string, IField> dbuSamples;
            if (RecordsKnowledge.TypeFieldCollectionPairs.TryGetValue(this.GetType(), out dbuSamples))
            {
                fields = new FieldCollection(RecordsKnowledge.TypeSubstCacheCollectionPairs[this.GetType()]);

                FieldInfo[] Fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance);

                foreach (FieldInfo fi in Fields)
                {
                    IField dbu;
                    if (dbuSamples.TryGetValue(fi.Name, out dbu))
                    {
                        IField newUnit = dbu.NewCopy(this, !dbu.IsNull);
                        fi.SetValue(this, newUnit);
                        fields.Add(newUnit);
                    }
                }
            }
            else
                this.fields = GetFieldSamples();

            Init();
        }
        public FieldCollection GetFieldSamples()
        {
            // критерии поиска полей
            FieldInfo[] Fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance);

            FieldCollection unitSamples = new FieldCollection();

            for (int i = 0; i < Fields.Length; i++)
            {
                FieldInfo fi = Fields[i];

                if (fi.FieldType.GetInterface(typeof(IField).Name) == null)
                    continue;

                IField oldUnit = fi.GetValue(this) as IField;

                // вызываю конструктор DbUnit в котором передаю имя свойства и его атрибуты
                IField newUnit;
                FieldsKnowledge.TypeUnitPairs.TryGetValue(fi.FieldType, out newUnit);
                newUnit = newUnit.New(fi.Name, this, new FieldAttributeCollection(fi.GetCustomAttributes(true)));

                if (oldUnit != null && (oldUnit.IsNull == false))
                    newUnit.Value = oldUnit.Value;

                fi.SetValue(this, newUnit);

                // добавляю проинициализарованное поле в коллекцию
                unitSamples.Add(newUnit);
            }

            return unitSamples;
        }

        public FieldCollection Fields
        {
            get { return fields; }
        }        

        public abstract void Init();

        public override string ToString()
        {
            return this.ID + (IsDisposed ? "[Disposed]" : "");
        }

        public IRecord Clone()
        {
            try
            {
                widget clone = (widget)this.GetType().InvokeMember(null,
                            BindingFlags.Public | BindingFlags.DeclaredOnly |
                            BindingFlags.Instance | BindingFlags.CreateInstance,
                            null, null, null);

                this.CopyTo(clone);
                return clone;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// копирует значения полей этой структуры в Record
        /// </summary>
        /// <param name="Record"></param>
        public virtual void CopyTo(IRecord Record)
        {
            FieldCollection SrcUnits = this.Fields;
            FieldCollection DstUnits = Record.Fields;

            for (int i = 0; i < SrcUnits.Count; i++)
                DstUnits[i].Value = SrcUnits[i].GetCopy();
        }

        public object this[IField unit]
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

        public abstract void refresh();

        public abstract void reset_variables();

        public string Title
        {
            get
            {
                return ID;
            }
        }

        public bool HasAbilitiesOnCooldown
        {
            get { return abilitiesOnCooldown > 0; }
        }
        public void UpdateAbilitiesOnCooldownCounter(bool increase)
        {
            abilitiesOnCooldown += increase ? 1 : -1;
        }

        public bool HasActivatedAbilities
        {
            get { return activatedAbilities > 0; }
        }
        public void UpdateActivatedAbilitiesCounter(bool increase)
        {
            activatedAbilities += increase ? 1 : -1;
        }

        public virtual void ApplyProps(HabProperties hps)
        {
            foreach (KeyValuePair<string, object> hp in hps)
            {
                IField dbu;
                if (fields.TryGetByName(hp.Key, out dbu))
                    dbu.Value = hp.Value;
            }
        }

        public virtual void ApplyPropsBySubstitute(HabProperties hps)
        {
            if (hps == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> hp in hps)
            {
                IField dbu;
                if (fields.TryGetBySubstituteName(hp.Key, out dbu))
                    dbu.Value = hp.Value;
            }
        }

        public virtual void ApplyPropsExBySubstitute(HabProperties hps)
        {
            string old_icon_name = iconName;

            this.ApplyPropsBySubstitute(hps);

            if (iconName != old_icon_name)
                LoadIcons();
        }

        protected abstract void LoadIcons();

        public string ToString(params string[] unitNames)
        {
            string str = "";

            foreach (string name in unitNames)
            {
                IField dbu;
                if (this.Fields.TryGetByName(name, out dbu))
                    str += "'" + dbu.Text + "' ";
            }

            return str;
        }

        public double GetLife()
        {
            return hp;
        }
        public void SetLife(double newLife)
        {
            this[hp] = Math.Min(newLife, max_hp);
        }

        public location get_location()
        {
            return new location(this.x, this.y);
        }
        public virtual void set_location(location l)
        {
            this.x = l.x;
            this.y = l.y;
        }
        public virtual void set_location(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public virtual void set_owningPlayer(player p)
        {
            owningPlayer = p;
        }
        public virtual player get_owningPlayer()
        {
            return owningPlayer;
        }
    }
    public class unit : widget, IRecord
    {
        //public static Dictionary<string, unit> units = new Dictionary<string, unit>((IEqualityComparer<string>)StringComparer.CurrentCultureIgnoreCase);

        #region base_variables
        [Substitute("DHAIms,DataA")]
        public DBSMARTDOUBLE ims = null;

        [Substitute("AIms,DataA")]
        public DBSMARTINT bonus_ms = null;
        [Substitute("AIas,DataA")]
        public DBSMARTDOUBLE ias = null;

        [Substitute("")]
        public DBSMARTDOUBLE armor = null;

        [Substitute("(AHad|AIde),DataA\\d+", IsPattern = true)]
        public DBSMARTDOUBLE bonus_armor = null;

        [Substitute("AIsr,DataB1")]
        public DBSMARTDOUBLE spell_resistance = null;

        [Substitute("Arel,DataA1")]
        public DBSMARTDOUBLE hpRegen = null;

        public DBDOUBLE manaRegen = null;
        [Substitute("AHab,DataA\\d+", IsPattern = true)]
        public DBSMARTDOUBLE bmanaRegen = null;
        [Substitute("AIrm,DataA1")]
        public DBSMARTDOUBLE imanaRegen = null;

        public DBDAMAGE damage = null;
        // bonus damage
        [Substitute("AIat,DataA1")]
        public DBSMARTDAMAGE bonus_damage = null;

        [Substitute("DHAmdm,DataA1")]
        public DBDAMAGE magic_damage = null;

        [Substitute("rangeN1")]
        public DBINT initRange = null;
        public DBSMARTINT range = null;

        [Substitute("DHAstr,DataA", IsPattern = true)]
        public DBSMARTDOUBLE strength = null;

        [Substitute("DHAagi,DataA\\d+", IsPattern = true)]
        public DBSMARTDOUBLE agility = null;

        [Substitute("DHAint,DataA\\d+", IsPattern = true)]
        public DBSMARTDOUBLE intelligence = null;

        [Substitute("isbldg")]
        public DBINT isbuilding = null;

        public bool isIllusion = false;
        public bool isVisible = true;
        public bool pathing = true;
        public int xp = 0;

        string timedLifeBuffId;
        double timedLifeDuration;
        #endregion
        #region custom_variables
        [Substitute("Propernames"), Enumeration(false)]
        public DBCHAR name = null;

        [Substitute("chosenSide")]
        public DBINT chosenSide = null;

        [Substitute("primary")]
        public DBPRIMATTR primary = null;

        [Substitute("weapTp1")]
        public DBATTACKMETHOD attackMethod = AttackMethod.Melee;

        [Substitute("cool1")]
        public DBDOUBLE BAT = null;

        public DBINT moveSpeed = null;

        [Substitute("spd")]
        public DBINT baseMoveSpeed = null;
        public DBSMARTINT baseMoveSpeedBonus = null;

        [Substitute("HP")]
        public DBDOUBLE baseHp = null;//150
        public DBDOUBLE baseArmor = null;
        public DBSMARTDOUBLE baseArmorBonus = null;

        public DBDOUBLE baseSpellResistance = 0.25;

        [Substitute("regenHP")]
        public DBDOUBLE baseHpRegen = null;
        [Substitute("regenMana")]
        public DBDOUBLE baseManaRegen = null;

        // this defines hero's base damage min/max

        [Substitute("dice1")]
        public DBINT dice = null;
        [Substitute("sides1")]
        public DBINT sides = null;
        [Substitute("dmgplus1")]
        public DBINT dmgplus = null;

        [Substitute("bountydice")]
        public DBINT bountydice = null;
        [Substitute("bountysides")]
        public DBINT bountysides = null;
        [Substitute("bountyplus")]
        public DBINT bountyplus = null;

        ////////////////////////////

        public DBINT def = null;
        public DBINT defUp = null;

        public DBDAMAGE baseDamage = null;
        public DBDAMAGE statsDamage = null;

        public DBSMARTINT baseDamageBonus = null;

        public DBDOUBLE cooldown = null;

        [Substitute("STR")]
        public DBDOUBLE initStrength = 0;
        [Substitute("AGI")]
        public DBDOUBLE initAgility = 0;
        [Substitute("INT")]
        public DBDOUBLE initIntelligence = 0;

        private double internalBonusStr = 0;
        private double internalBonusAgi = 0;
        private double internalBonusInt = 0;

        [Substitute("STRplus")]
        public DBDOUBLE strPerLvl = null;
        [Substitute("AGIplus")]
        public DBDOUBLE agiPerLvl = null;
        [Substitute("INTplus")]
        public DBDOUBLE intPerLvl = null;

        public DBDOUBLE hpFactor = null;

        [Substitute("AInv,DataA")]
        protected DBINVENTORY inventory;
        public DBINVENTORY Inventory { get { return inventory; } }

        public DBSTRINGCOLLECTION sellunits = null;
        public DBSTRINGCOLLECTION sellitems = null;

        [Substitute("heroAbilList")]
        public DBABILITIES heroAbilities = null;
        protected DBSTRINGCOLLECTION baseHeroAbilList = null;

        [Substitute("upgrades")]
        public DBUPGRADES upgrades = null;

        public DBABILITIES acquiredAbilities = null;
        public DBABILITIES onAttackAbilities = null;
        public DBABILITIES onDefenceAbilities = null;
        public StackedAbilitiesDictionary selfAppliedStackedAbilities = null;

        protected StackedAbilitiesDictionary buffs = null;

        [Substitute("points")]
        public DBINT points = null;

        //public DBSMARTABILITIES selfAppliedAbilities = null;

        protected bool updated = false;
        protected bool doSummon = false;
        protected int forceNoRefresh = 0;

        protected int level = 0;

        protected int hpPerStr = 19;
        protected int manaPerInt = 13;
        protected double armorPerAgi = 0.14;//(double)((double)1 / (double)7);                    
        #endregion

        protected unit(bool thisIsForDbUnitsKnowledge) { }
        public unit()
        {
            /*this[range] = 100;

            DBITEMSLOT[] items = new DBITEMSLOT[6]
            {   item_1,
                item_2,
                item_3,
                item_4,
                item_5,
                item_6
            };

            this[inventory] = items;*/
        }

        public unit(string codeID)
        {
            this[this.codeID] = codeID;
            ApplyPropsByID(codeID);

            Prepare();
        }

        public unit(int unitID)
        {
            string codeID = DHJassInt.int2id(unitID);

            this[this.codeID] = codeID;

            ApplyPropsByID(codeID);

            Prepare();
        }

        public static void Reset()
        {
            unit.move = null;
        }

        protected void ApplyPropsByID(string codeID)
        {
            if (codeID == "n00W")
            {
            }

            HabProperties hpsUnit = DHMpqDatabase.UnitSlkDatabase["Profile"][codeID];

            // if no data is found for this codeID
            // then this is probably not a unit's ID,
            // so just return from this function
            if (hpsUnit == null)
            {
                Console.WriteLine("CodeID '" + codeID + "' is not found in the unit database");
                return;
            }

            this.ApplyPropsExBySubstitute(hpsUnit);

            hpsUnit = DHMpqDatabase.UnitSlkDatabase["UnitData"][codeID];
            this.ApplyPropsExBySubstitute(hpsUnit);

            hpsUnit = DHMpqDatabase.UnitSlkDatabase["UnitWeapons"][codeID];
            this.ApplyPropsExBySubstitute(hpsUnit);

            hpsUnit = DHMpqDatabase.UnitSlkDatabase["UnitBalance"][codeID];
            this.ApplyPropsExBySubstitute(hpsUnit);

            hpsUnit = DHMpqDatabase.UnitSlkDatabase["UnitAbilities"][codeID];
            this.ApplyPropsExBySubstitute(hpsUnit);
        }

        public override void Init()
        {
            bonus_ms.SmartMethod = SmartMethodType.Max;
            spell_resistance.SmartMethod = SmartMethodType.Factor;
        }

        protected void Prepare()
        {
            foreach (DBABILITY a in abilities)
                if (String.IsNullOrEmpty(a[0, "reqLevel"] as string))
                {
                    a.Level = 1;
                    a.Apply();
                    //HabProperties hps = a.GetProps(true);
                    //this.ApplyPropsBySubstitute(hps);
                }

            baseHeroAbilList.Clear();
            foreach (DBABILITY a in heroAbilities)
                baseHeroAbilList.Add(a.Alias);
        }

        public override void ApplyPropsExBySubstitute(HabProperties hps)
        {
            base.ApplyPropsExBySubstitute(hps);

            this[baseDamage] = (new Damage(dice, dice * sides)) + dmgplus;
        }

        protected override void LoadIcons()
        {
            //string imageName = Application.StartupPath + "\\" + "Images" + "\\" + iconName;              

            if (String.IsNullOrEmpty(iconName) == false)
            {
                this[iconImage] = DHRC.Default.GetImage(iconName);
                if (iconImage.IsNull)
                    Console.WriteLine("hero icon fail:" + iconName);
            }

            /*for (int i = 0; i < Skills.Length; i++)
                Skills[i].skill_image = DBIMAGE.FromImage(imageName + (i + 1) + ".gif");*/
        }

        public DBINT Level
        {
            get
            {
                return level + 1;
            }
            set
            {
                LevelShift(value - (level + 1));
            }
        }

        public void LevelShift(int amount)
        {
            level = Math.Max(0, Math.Min(24, level + amount));

            refresh();
        }

        public DBSTRINGCOLLECTION BaseHeroAbilList
        {
            get
            {
                return baseHeroAbilList;
            }
        }

        public StackedAbilitiesDictionary Buffs
        {
            get
            {
                if (buffs == null)
                    buffs = new StackedAbilitiesDictionary();

                return buffs;
            }
        }

        /// <summary>
        /// setting this value to true will cause the hero to be redrawn on the user interface
        /// </summary>
        public bool Updated
        {
            get
            {
                return updated;
            }
            set
            {
                updated = value;
            }
        }

        public bool DoSummon
        {
            get
            {
                return doSummon;
            }
            set
            {
                doSummon = value;
            }
        }

        public bool IsHero
        {
            get
            {
                return (PrimAttrType)primary != PrimAttrType.None;
            }
        }

        public bool IsBuilding
        {
            get
            {
                return isbuilding == 1;
            }
        }

        public bool IsInvulnerable
        {
            get { return abilities.Contains("Avul"); }
        }

        public bool CanAttack
        {
            get
            {
                return attackMethod != AttackMethod.None;
            }
        }
        
        public void ForceNoRefresh(bool increase)
        {
            forceNoRefresh += increase ? 1 : -1;
        }

        public void activate_ability(DBABILITY ability, bool refresh)
        {
            if (!refresh) forceNoRefresh++;

            if ((ability.AbilityState & AbilityState.Activated) != 0)
            {
                // remove activated state so it wouldnt trigger twice
                ability.AbilityState ^= AbilityState.Activated;

                // set it on cooldown
                ability.IsOnCooldown = true;

                // add this ability to buff list
                if (ability.HasBuff)
                {
                    // make a copy to avoid changing original ability's state
                    DBABILITY copy = ability.GetCopy(AbilityInfo.fullInfo);

                    // activate this ability                        
                    copy.Activate();

                    // add it to buff list
                    if (!this.Buffs.Add(copy, false))
                        copy.Deactivate(false); // deactivate it if it was not accepted
                }
                else
                    ability.Activate();
            }

            if (!refresh) forceNoRefresh--;           
        }

        public void reset()
        {
            this.abilities.Clear();
            this.acquiredAbilities.Clear();
            this.heroAbilities.Clear();
            this.inventory.Clear();
            this.level = 0;

            ApplyPropsByID(codeID);

            Prepare();

            refresh();
        }

        /// <summary>
        /// recalculates all unit's stats and bonuses and sets 'updated' status to true
        /// </summary>
        public override void refresh()
        {
            if (forceNoRefresh != 0) return;
            //inventory.refresh();

            reset_variables();

            update_benefits();
            update_upgrades();

            apply_abilities(0);
            apply_buffs(0);

            update_stats();
            udpate_dmg(false);
            update_armor(false);
            update_hpmana();
            update_ias();
            update_ims();            

            apply_abilities(1);
            apply_buffs(1);

            udpate_dmg(true);
            update_armor(true);

            updated = true;
        }

        public override void reset_variables()
        {
            this[cooldown] = 0;

            ims.Clear();
            bonus_ms.Clear();
            baseMoveSpeedBonus.Clear();
            ias.Clear();
            //hp.Clear();
            armor.Clear();
            bonus_armor.Clear();
            baseArmorBonus.Clear();
            spell_resistance.Clear();

            hpRegen.Clear();
            this[manaRegen] = 0;
            bmanaRegen.Clear();
            imanaRegen.Clear();

            max_hp.Clear();
            max_mana.Clear();

            this[damage] = 0;
            baseDamageBonus.Clear();
            bonus_damage.Clear();

            range.Clear();

            strength.Clear();
            agility.Clear();
            intelligence.Clear();

            this[hpFactor] = 0.0;
        }

        public void update_upgrades()
        {
            if (owningPlayer == null) return;

            tech t;
            foreach (DBUPGRADE upgrade in this.upgrades)
                if (this.owningPlayer.techs.TryGetValue(upgrade.codeID, out t))
                {
                    upgrade.Level = t.count;
                    upgrade.Apply();
                }
        }

        public void update_benefits()
        {
            DBABILITIES allAbilities = new DBABILITIES();

            allAbilities.AddRange(this.heroAbilities);

            foreach (DBITEMSLOT itemSlot in Inventory)
                allAbilities.AddRange(itemSlot.getAbilities());

            /*foreach(DBABILITY ability in allAbilities)
                if ((ability.AbilityState & AbilityState.Activated)!=0)
                {
                    // remove activated state so it wouldnt trigger twice
                    ability.AbilityState ^= AbilityState.Activated;

                    // set it on cooldown
                    ability.IsOnCooldown = true;

                    // add this ability to buff list
                    if (ability.HasBuff)
                    {
                        // make a copy to avoid changing original ability's state
                        DBABILITY copy = ability.GetCopy(AbilityInfo.fullInfo);

                        // activate this ability                        
                        copy.Activate();

                        // add it to buff list
                        if (!this.Buffs.Add(copy, false))
                            copy.Deactivate(false); // deactivate it if it was not accepted
                    }
                    else
                        ability.Activate();
                }*/

            // remove expired buffs
            Buffs.RemoveByState(AbilityState.Expired);            

            allAbilities = allAbilities.GetComponents();

            allAbilities.SetOwner(this);

            acquiredAbilities = allAbilities.GetSpecific(AbilitySpecs.IsLearned);
            acquiredAbilities.SortByAcquisition(true);

            DBABILITIES selfAppliedAcquiredAbilities = acquiredAbilities.GetSpecific(AbilitySpecs.IsActivated,
                AbilityTriggerType.Default,
                AbilityMatchType.Intersects,
                TargetType.Self, TargetType.None);

            selfAppliedStackedAbilities = new StackedAbilitiesDictionary(selfAppliedAcquiredAbilities);

            ////////////////////////
            // on attack abilities
            ////////////////////////

            onAttackAbilities = acquiredAbilities.GetSpecific(AbilitySpecs.IsActivated, AbilityTriggerType.OnAttack);
            onAttackAbilities.Sort();

            //////////////////////////
            // on defense abilities 
            //////////////////////////

            onDefenceAbilities = acquiredAbilities.GetSpecific(AbilityTriggerType.OnDefense);
            StackedAbilitiesDictionary sad = new StackedAbilitiesDictionary(onDefenceAbilities);
            onDefenceAbilities = sad.GetAbilities();
        }

        public void update_stats()
        {
            this[strength] = (int)(initStrength + internalBonusStr + strPerLvl * level);
            this[agility] = (int)(initAgility + internalBonusAgi + agiPerLvl * level);
            this[intelligence] = (int)(initIntelligence + internalBonusInt + intPerLvl * level);

            this[range] = initRange;
        }

        public void update_hpmana()
        {
            this[max_hp] = (int)(baseHp + (strength * hpPerStr));
            this[max_mana] = (int)(intelligence * manaPerInt);

            //if (hp == 0) // if refreshing newly created hero, or he was dead
            {
                this[hp] = (double)max_hp;
                this[mana] = (double)max_mana;
            }

            double raw_hpRegen = baseHpRegen + 0.03 * strength;
            this[hpRegen] = Math.Floor(raw_hpRegen * 100.0) / 100.0;

            double raw_manaRegen = baseManaRegen + 0.04 * intelligence;

            this[manaRegen] = bmanaRegen + ((1.0 + imanaRegen) * Math.Floor(raw_manaRegen * 100.0) / 100.0);
        }

        public void udpate_dmg(bool postUpdate)
        {
            if (!postUpdate)
            {
                switch ((PrimAttrType)this.primary)
                {
                    case PrimAttrType.Agi:
                        this[damage] = baseDamage + baseDamageBonus + (int)agility;
                        break;

                    case PrimAttrType.Int:
                        this[damage] = baseDamage + baseDamageBonus + (int)intelligence;
                        break;

                    case PrimAttrType.Str:
                        this[damage] = baseDamage + baseDamageBonus + (int)strength;
                        break;
                }

                this[statsDamage] = damage;
            }
            else
                this[damage] = damage + bonus_damage;
        }

        public void update_ias()
        {
            this[ias] = (agility * 0.01);

            double raw_cooldown = (BAT / (1 + ias));
            this[cooldown] = (Math.Round(raw_cooldown * 100.0) + 0.0) / 100.0;
        }

        public void update_ims()
        {
            int simpleMoveSpeed = baseMoveSpeed + baseMoveSpeedBonus + (int)bonus_ms;
            moveSpeed = simpleMoveSpeed + (int)(simpleMoveSpeed * ims);
        }

        public double get_dps()
        {
            int total_damage = this.damage;

            //total_damage += bonus_damage;
            //total_damage += unstable_bonus_damage;

            double dps = total_damage / cooldown;
            return Math.Round(dps * 10) / 10;
        }

        public double get_dps(double additional_damage)
        {
            double total_damage = this.damage;
            total_damage += additional_damage;

            //total_damage += bonus_damage;
            //total_damage += unstable_bonus_damage;

            double dps = total_damage / cooldown;
            return Math.Round(dps * 10) / 10;
        }

        public int get_naked_attr(PrimAttrType attr_type)
        {
            switch (attr_type)
            {
                case PrimAttrType.Agi:
                    return (int)(initAgility + internalBonusAgi + agiPerLvl * level);
                case PrimAttrType.Int:
                    return (int)(initIntelligence + internalBonusInt + intPerLvl * level);
                case PrimAttrType.Str:
                    return (int)(initStrength + internalBonusStr + strPerLvl * level);
            }

            return 0;
        }

        public void set_naked_attr(PrimAttrType attr_type, int value)
        {
            int attrValue;
            switch (attr_type)
            {
                case PrimAttrType.Agi:
                    attrValue = (int)(initAgility + internalBonusAgi + agiPerLvl * level);
                    internalBonusAgi += value - attrValue;
                    break;
                case PrimAttrType.Int:
                    attrValue = (int)(initIntelligence + internalBonusInt + intPerLvl * level);
                    internalBonusInt += value - attrValue;
                    break;
                case PrimAttrType.Str:
                    attrValue = (int)(initStrength + internalBonusStr + strPerLvl * level);
                    internalBonusStr += value - attrValue;
                    break;
            }
        }

        public Damage get_naked_damage()
        {
            switch ((PrimAttrType)this.primary)
            {
                case PrimAttrType.Agi:
                    return baseDamage + baseDamageBonus + (int)(initAgility + internalBonusAgi + agiPerLvl * level);
                case PrimAttrType.Int:
                    return baseDamage + baseDamageBonus + (int)(initIntelligence + internalBonusInt + intPerLvl * level);
                case PrimAttrType.Str:
                    return baseDamage + baseDamageBonus + (int)(initStrength + internalBonusStr + strPerLvl * level);
            }

            return new Damage();
        }

        public void update_armor(bool postUpdate)
        {
            if (!postUpdate)
            {
                if (baseArmor.IsNull)
                    this[baseArmor] = def - 1;

                this[spell_resistance] = baseSpellResistance;
            }
            else
            {
                this[armor] = get_naked_armor();

                double bonus_agi = agility - get_naked_attr(PrimAttrType.Agi);

                this[armor] = (Math.Floor((bonus_armor + (bonus_agi * 0.14)) * 10) + 0.0) / 10.0;
            }
        }

        public double get_naked_armor()
        {
            return (Math.Floor((baseArmor + baseArmorBonus + (get_naked_attr(PrimAttrType.Agi) * 0.14)) * 10) + 0.0) / 10.0;
        }

        public int get_damage_reduction()
        {
            return (int)Math.Floor((6 * armor) / (1 + (0.06 * armor)));
        }

        private void apply_abilities(int priority)
        {
            foreach (StackedAbilities sa in selfAppliedStackedAbilities.Values)
                foreach (DBABILITY a in sa)
                    if (a.Priority == priority)
                        a.Apply();
        }
        private void apply_buffs(int priority)
        {
            foreach (StackedAbilities sa in Buffs.Values)
                foreach (DBABILITY a in sa)
                    if (a.Priority == priority)
                        a.Apply();
        }

        public void playSound(UnitAckSounds sound)
        {
            DHMULTIMEDIA.PlayUnitSound(this.codeID, sound);
        }
        public void playSound(UnitAckSounds sound, bool random)
        {
            DHMULTIMEDIA.PlayUnitSound(this.codeID, sound, random);
        }
        public void playSound(AnimSounds sound)
        {
            DHMULTIMEDIA.PlayUnitSound(this.codeID, sound);
        }

        public int getInventorySize()
        {            
            int inventorySize = 0;
            foreach (DBABILITY ability in this.abilities)
                if (ability is DBINVENTORYABILITY)
                {
                    inventorySize = (ability as DBINVENTORYABILITY).Slots;
                    break;
                }

            return inventorySize;
        }

        public int getAbilityLevel(string codeID)
        {
            foreach (DBABILITY a in abilities)
                if (a.Alias == codeID) return a.Level;
                else
                {
                    DBBUFF b = a.GetBuffByID(codeID);
                    if (b != null) return b.Level;
                }

            foreach (DBABILITY a in heroAbilities)
                if (a.Alias == codeID) return a.Level;
                else
                {
                    DBBUFF b = a.GetBuffByID(codeID);
                    if (b != null) return b.Level;
                }

            return 0;
        }
        public int setAbilityLevel(string codeID, int level)
        {
            foreach (DBABILITY a in abilities)
                if (a.Alias == codeID)
                {
                    a.Level = level;
                    return level;
                }
                else
                {
                    DBBUFF b = a.GetBuffByID(codeID);
                    if (b != null)
                    {
                        b.Level = level;
                        return level;
                    }
                }

            foreach (DBABILITY a in heroAbilities)
                if (a.Alias == codeID)
                {
                    a.Level = level;
                    return level;
                }
                else
                {
                    DBBUFF b = a.GetBuffByID(codeID);
                    if (b != null)
                    {
                        b.Level = level;
                        return level;
                    }
                }

            return 0;
        }

        public override void set_owningPlayer(player p)
        {
            set_owningPlayer(p, 0, 0);
        }
        public void set_owningPlayer(player p, double newx, double newy)
        {
            if (owningPlayer != null) owningPlayer.remove_unit(this);

            p.add_unit(this);

            owningPlayer = p;

            OnMove(newx, newy);// like it was outside of the map before
        }

        public override void destroy()
        {
            if (owningPlayer != null)
            {
                owningPlayer.remove_unit(this);
                owningPlayer = null;
            }

            if (this.abilitiesOnCooldown > 0)
            {
                DBABILITIES allAbilities = new DBABILITIES();

                allAbilities.AddRange(this.heroAbilities);

                foreach (DBITEMSLOT itemSlot in Inventory)
                    allAbilities.AddRange(itemSlot.getAbilities());

                foreach (DBABILITY ability in allAbilities)
                    ability.IsOnCooldown = false;
            }

            if (this.activatedAbilities > 0)
            {
                foreach (StackedAbilities sa in Buffs.Values)
                    foreach (DBABILITY a in sa)
                        a.Deactivate(false);
            }

            base.destroy();
        }

        public static event DHJassEventHandler move;
        protected void OnMove(double newx, double newy)
        {
            if (move != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["oldx"] = this.x;
                e.args["oldy"] = this.y;

                base.set_location(newx, newy);

                move(this, e);
            }
            else
                base.set_location(newx, newy);
        }
        protected void OnMove(location l)
        {
            if (move != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["oldx"] = this.x;
                e.args["oldy"] = this.y;

                base.set_location(l);

                move(this, e);
            }
            else
                base.set_location(l);
        }
        public override void set_location(location l)
        {
            OnMove(l);
        }
        public override void set_location(double x, double y)
        {
            OnMove(x, y);
        }

        protected int GetBounty()
        {
            Random r = new Random();
            return r.Next(bountydice + bountyplus, (bountydice * bountysides) + bountyplus);
        }

        public bool IsAlly(unit u)
        {
            return u != null ? IsAlly(u.owningPlayer) : false;
        }
        public bool IsAlly(player p)
        {
            int id = p.get_id();
            int ownerId = this.owningPlayer.get_id();

            if (ownerId == id) return false;

            if (id >= 0 && id <= 5)
                return (ownerId >= 0 && ownerId <= 5);
            else
                if (id >= 6 && id <= 11)
                    return (ownerId >= 6 && ownerId <= 11);
                else
                    return false;
        }

        public bool IsFriend(unit u)
        {
            return u != null ? IsFriend(u.owningPlayer) : false;
        }
        public bool IsFriend(player p)
        {
            int id = p.get_id();
            int ownerId = this.owningPlayer.get_id();

            if (id >= 0 && id <= 5)
                return (ownerId >= 0 && ownerId <= 5);
            else
                if (id >= 6 && id <= 11)
                    return (ownerId >= 6 && ownerId <= 11);
                else
                    return ownerId == id;
        }

        public bool IsEnemy(unit u)
        {
            return u != null ? IsEnemy(u.owningPlayer) : false;
        }
        public bool IsEnemy(player p)
        {
            int id = p.get_id();
            int ownerId = this.owningPlayer.get_id();

            if (id >= 0 && id <= 5)
                return !(ownerId >= 0 && ownerId <= 5);
            else
                if (id >= 6 && id <= 11)
                    return !(ownerId >= 6 && ownerId <= 11);
                else
                    return ownerId != id;
        }

        public void ApplyTimedLife(string buffId, double duration)
        {
            // just in case
            DHJassGlobalClock.Tick -= TimedLifeTick;

            timedLifeBuffId = buffId;
            timedLifeDuration = duration;

            DHJassGlobalClock.Tick += TimedLifeTick;

        }

        private void OnTimedLifeExpire()
        {
            DHJassGlobalClock.Tick -= TimedLifeTick;           
            this.destroy();
        }

        private void TimedLifeTick()
        {
            this.timedLifeDuration -= DHJassGlobalClock.TickInterval;

            if (timedLifeDuration <= 0)
                OnTimedLifeExpire();            
        }

        public event DHJassEventHandler death;
        public void OnDeath(unit killer)
        {
            if (death != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["killing"] = killer;

                death(this, e);
            }

            if (killer != null && killer.owningPlayer is player)
                killer.owningPlayer.Gold = killer.owningPlayer.Gold + this.GetBounty();
        }

        public event DHJassEventHandler sell_item;
        public void OnSellItem(item solditem, unit buyingunit)
        {
            if (sell_item != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["selling"] = this;
                e.args["item"] = solditem;
                e.args["buying"] = buyingunit;

                sell_item(this, e);
            }
        }

        public event DHJassEventHandler pickup_item;
        public void OnPickupItem(item item)
        {
            if (pickup_item != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["item"] = item;

                pickup_item(this, e);
            }
        }

        public event DHJassEventHandler drop_item;
        public void OnDropItem(item item)
        {
            if (drop_item != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["item"] = item;

                drop_item(this, e);
            }
        }

        public event DHJassEventHandler use_item;
        public void OnUseItem(item item)
        {
            if (use_item != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["item"] = item;

                use_item(this, e);
            }
        }

        public event DHJassEventHandler sell;
        public void OnSell(unit soldunit)
        {
            if (sell != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["selling"] = this;
                e.args["unit"] = soldunit;

                sell(this, e);
            }
        }
        public void OnSell(unit seller, unit soldunit)
        {
            if (sell != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["selling"] = seller;
                e.args["unit"] = soldunit;

                sell(this, e);
            }
        }

        public event DHJassEventHandler summon;
        public void OnSummon()
        {
            if (summon != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;

                summon(this, e);
            }
        }

        public event DHJassEventHandler hero_skill;
        public void OnHeroSkill(DBABILITY skill)
        {
            if (hero_skill != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["skill"] = skill;

                hero_skill(this, e);
            }
        }

        public event DHJassEventHandler spell_cast;
        public void OnSpellCast(DBABILITY spell)
        {
            if (spell_cast != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["spell"] = spell.Alias;

                spell_cast(this, e);
            }
        }

        public event DHJassEventHandler spell_effect;
        public void OnSpellEffect(DBABILITY spell)
        {
            if (spell_cast != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["spell"] = spell.Alias;

                spell_effect(this, e);
            }
        }

        public event DHJassEventHandler issued_order;
        public void OnIssuedOrder(OrderID order)
        {
            if (issued_order != null)
            {
                DHJassEventArgs e = new DHJassEventArgs();
                e.args["unit"] = this;
                e.args["order"] = order;

                issued_order(this, e);
            }
        }        
    }
    public class item : widget
    {
        #region variables
        [Substitute("class")]
        public DBCHAR type = null;

        /// <summary>
        /// drop - Dropped When Carrier Dies
        /// </summary>
        public DBBIT drop = null;

        /// <summary>
        /// droppable - Can Be Dropped
        /// </summary>
        public DBBIT droppable = null;

        /// <summary>
        /// pawnable - Can Be Sold To Merchants
        /// </summary>
        public DBBIT pawnable = null;

        /// <summary>
        /// sellable - Can Be Sold By Merchants
        /// </summary>
        public DBBIT sellable = null;

        /// <summary>
        /// powerup - Use Automatically When Acquired
        /// </summary>
        public DBBIT powerup = null;

        /// <summary>
        /// uses - Number Of Charges
        /// </summary>
        public DBINT uses = null;

        /// <summary>
        /// usable - Actively Used
        /// <para>True: one of the abilities of the item is castable.</para>
        /// <para>False: the abilities of the item are all passive or the item has no abilities</para>                    
        /// </summary>
        public DBBIT usable = null;

        /// <summary>
        /// perishable - Perishable 
        /// If False: the item will not be destroyed after all charges have been used
        /// </summary>
        public DBBIT perishable = null;
        #endregion

        unit owner;

        protected item(bool thisIsForDbUnitsKnowledge) { }

        public item()
        {
        }
        public item(int itemID)
        {
            string codeID = DHJassInt.int2id(itemID);

            this[this.codeID] = codeID;

            ApplyPropsByID(codeID);
        }
        public item(string codeID)
        {
            this[this.codeID] = codeID;
            ApplyPropsByID(codeID);
        }

        protected void ApplyPropsByID(string codeID)
        {
            HabProperties hpsItem = DHMpqDatabase.ItemSlkDatabase["Profile"][codeID];
            this.ApplyPropsExBySubstitute(hpsItem);

            hpsItem = DHMpqDatabase.ItemSlkDatabase["ItemData"][codeID];
            this.ApplyPropsExBySubstitute(hpsItem);
        }

        public static RecordCollection GetMerged(RecordCollection Items, HabPropertiesCollection Props)
        {
            RecordCollection merged = new RecordCollection();

            foreach (HabProperties hps in Props)
            {
                IRecord item = Items.GetByUnit("codeID", hps.name);

                if (item != null) // если item с таким именем найден, добавляем его в массив
                    merged.Add(item);
            }

            return merged;
        }

        protected override void LoadIcons()
        {
            if (String.IsNullOrEmpty(iconName) == false)
            {
                this[iconImage] = DHRC.Default.GetImage(iconName);
                if (iconImage.IsNull)
                    Console.WriteLine("fail:" + iconName);
            }
        }

        public override void refresh()
        {
        }

        public override void Init()
        { }

        public override void reset_variables()
        {
        }

        public void set_owner(unit u)
        {
            owner = u;

            if (u != null)
            {
                this.x = u.x;
                this.y = u.y;
            }
        }
        /// <summary>
        /// drops this item from old owner before changing to new owner
        /// </summary>
        /// <param name="u"></param>
        public void set_owner_ex(unit u)
        {
            if (owner != null) owner.Inventory.remove_item(this);
            owner = u;
        }
        public unit get_owner()
        {
            return owner;
        }

        public void set_charges(int charges)
        {
            this.uses = charges;
            if (this.uses == 0 && this.perishable)
                destroy();
        }       

        public override void destroy()
        {
            set_owner_ex(null);
            base.destroy();
        }
    }

    public class boolexpr : handlevalue
    {
        public DHJassFunction function;

        public boolexpr(DHJassFunction function)
        {
            this.function = function;
        }
        public bool GetResult()
        {
            if (function == null) return true;
            else
            {
                if (DHJassExecutor.IsTracing && function.Name.StartsWith(DHJassExecutor.TraceName))
                {
                    Console.Write("Called:" + function.Name);
                    Console.Write("\n");
                }
                return function.Execute().BoolValue;
            }
        }
    }

    public class trigger : handlevalue
    {
        public string debugInfo;
        protected bool enabled = true;
        triggeraction action = null;
        triggercondition condition = null;

        public void addAction(triggeraction action)
        {
            this.action = action;
            /*if (this.action == null)
                actions = new List<triggeraction>();
                        
            actions.Add(action);
            if (actions.Count > 1)
                Console.Beep(300, 100);*/
        }
        public void addCondition(triggercondition condition)
        {
            this.condition = condition;
            /*if (conditions == null)
                conditions = new List<triggercondition>();

            conditions.Add(condition);

            if (conditions.Count > 1)
                Console.Beep(300, 100);*/
        }
        public void Execute()
        {
            if (!enabled) return;
            /*Thread t = new Thread((ThreadStart)this.Run);
            t.Start();*/
            this.Run();
        }
        public void Execute(DHJassEventArgs e)
        {
            if (!enabled) return;
            /*Thread t = new Thread((ParameterizedThreadStart)this.Run);
            t.Start(e);*/
            this.Run(e);
        }
        public void ExecuteAction()
        {
            if (action != null) action.Execute();
        }
        protected void Run()
        {
            //Console.WriteLine("trigger.Run() not ready!");
            Run(new DHJassEventArgs());
        }
        protected void Run(object obj)
        {
            DHJassExecutor.TriggerStack.Push((obj as DHJassEventArgs).args);
            if (condition == null || condition.GetResult())
                if (action != null) action.Execute();

            DHJassExecutor.TriggerStack.Pop();
        }
        public string debugActionInfo
        {
            get
            {
                if (action != null)
                    return action.debugInfo;
                else
                    return null;
            }
        }
        public int debugActionHandle
        {
            get
            {
                if (action != null)
                    return action.handle;
                else
                    return 0;
            }
        }
        public void enable()
        {
            enabled = true;
        }
        public void disable()
        {
            enabled = false;
        }
        public bool Enabled
        {
            get
            {
                return enabled;
            }
        }
        public int ExecCount
        {
            get { return action.ExecCount; }
        }
        public int EvalCount
        {
            get { return condition.EvalCount; }
        }
    }
    public class group : handlevalue
    {
        public List<unit> units = new List<unit>();
        public void addUnit(unit u)
        {
            units.Add(u);            
        }
        public void removeUnit(unit u)
        {
            units.Remove(u);
        }
        public void runForeach(DHJassFunction callback)
        {
            List<unit> safeCopy = new List<unit>(units);
            foreach (unit u in safeCopy)
            {
                DHJassExecutor.GroupStack.Push(u);
                callback.Execute();
                DHJassExecutor.GroupStack.Pop();
            }
        }
        public void enumUnitsOfPlayer(player p, boolexpr be)
        {
            // to avoid cross-thread modifications in collection
            lock ((p.units as ICollection).SyncRoot)
                foreach (unit u in p.units.Values)
                {
                    DHJassExecutor.GroupStack.Push(u);
                    if (be == null || be.GetResult()) addUnit(u);
                    DHJassExecutor.GroupStack.Pop();
                }
        }
        public void enumUnitsInRange(double x, double y, double radius, boolexpr be)
        {
            foreach (player p in player.players)
            {
                // to avoid cross-thread modifications in collection                                                
                lock ((p.units as ICollection).SyncRoot)
                    foreach (unit u in p.units.Values)
                    {
                        if (!region.IsUnitInRange(u, x, y, radius))
                            continue;

                        DHJassExecutor.GroupStack.Push(u);
                        if (be == null || be.GetResult()) addUnit(u);
                        DHJassExecutor.GroupStack.Pop();
                    }
            }
        }
        public void enumUnitsInRect(rect r, boolexpr be)
        {
            foreach (player p in player.players)
            {
                // to avoid cross-thread modifications in collection
                lock ((p.units as ICollection).SyncRoot)
                    foreach (unit u in p.units.Values)
                    {
                        if (!r.ContainsXY(u.x, u.y))
                            continue;

                        DHJassExecutor.GroupStack.Push(u);
                        if (be == null || be.GetResult()) addUnit(u);
                        DHJassExecutor.GroupStack.Pop();
                    }
            }
        }
        public void clear()
        {
            units.Clear();
        }
        public unit getFirst()
        {
            if (units.Count > 0) return units[0];
            else
                return null;
        }
    }
    public class force : handlevalue
    {
        public List<player> players = new List<player>();
        public void addPlayer(player p)
        {
            players.Add(p);
        }
        public void runForeach(DHJassFunction callback)
        {
            foreach (player p in players)
            {
                DHJassExecutor.ForceStack.Push(p);
                callback.Execute();
                DHJassExecutor.ForceStack.Pop();
            }
        }
        public void enumPlayers(boolexpr be)
        {
            foreach (player p in player.players)
            {
                DHJassExecutor.ForceStack.Push(p);
                if (be == null || be.GetResult()) addPlayer(p);
                DHJassExecutor.ForceStack.Pop();
            }
        }
        public void enumEnemies(player player, boolexpr be)
        {
            int minID = -1;
            int maxID = -1;

            int id = player.get_id();

            if (id >= 1 && id <= 5)
            {
                minID = 7;
                maxID = 11;
            }
            else
                if (id >= 7 && id <= 11)
                {
                    minID = 1;
                    maxID = 5;
                }

            foreach (player p in player.players)
            {
                id = p.get_id();
                if (!(id >= minID && id <= maxID)) continue;

                DHJassExecutor.ForceStack.Push(p);
                if (be == null || be.GetResult()) addPlayer(p);
                DHJassExecutor.ForceStack.Pop();
            }
        }
    }

    public class triggerevent : handlevalue
    {
        trigger t;
        int eventId;
        object data;

        public triggerevent(trigger t)
        {
            this.t = t;
            this.eventId = -1;
        }
        public triggerevent(trigger t, int eventId)
        {
            this.t = t;
            this.eventId = eventId;
        }
        public triggerevent(trigger t, object data)
        {
            this.t = t;
            this.data = data;
            this.eventId = -1;
        }
        public void OnEvent(object sender, DHJassEventArgs e)
        {
            e.args["trigger"] = t;
            e.args["eventId"] = eventId;

            t.Execute(e);
        }
        public string triggerDebugInfo
        {
            get
            {
                return t.debugActionInfo;
            }
        }
        public override void destroy()
        {
            data = null;
            base.destroy();
        }
    }
    public class triggercommandevent : handlevalue
    {
        trigger t;
        DHJassCommand cmd;

        public triggercommandevent(trigger t, DHJassCommand cmd)
        {
            this.t = t;
            this.cmd = cmd;
        }
        public void OnEvent(object sender, DHJassEventArgs e)
        {
            if (cmd.GetResult().BoolValue)
                t.Execute();
        }
    }
    public class triggerregionevent : handlevalue
    {
        trigger t;
        region r;

        public triggerregionevent(trigger t, region r)
        {
            this.t = t;
            this.r = r;
        }
        public void OnEnterEvent(object sender, DHJassEventArgs e)
        {
            unit u = e.args["unit"] as unit;

            double oldx = (double)e.args["oldx"];
            double oldy = (double)e.args["oldy"];

            if (r.EnterRegion(u, oldx, oldy))
                t.Execute(e);
        }
        public void OnLeaveEvent(object sender, DHJassEventArgs e)
        {
            unit u = e.args["unit"] as unit;

            double oldx = (double)e.args["oldx"];
            double oldy = (double)e.args["oldy"];

            if (r.LeaveRegion(u, oldx, oldy))
                t.Execute(e);
        }
    }
    public class triggerunitinrangeevent : handlevalue
    {
        trigger t;
        range r;        

        public triggerunitinrangeevent(trigger t, range r)
        {
            this.t = t;
            this.r = r;            
        }
        public void OnUnitInRangeEvent(object sender, DHJassEventArgs e)
        {
            unit u = e.args["unit"] as unit;

            double oldx = (double)e.args["oldx"];
            double oldy = (double)e.args["oldy"];

            if (r.EnterRange(u, oldx, oldy))
                t.Execute(e);
        }
        public void OnUnitOutOfRangeEvent(object sender, DHJassEventArgs e)
        {
            unit u = e.args["unit"] as unit;

            double oldx = (double)e.args["oldx"];
            double oldy = (double)e.args["oldy"];

            if (r.LeaveRange(u, oldx, oldy))
                t.Execute(e);
        }
    }

    public class triggeraction : handlevalue
    {
        int execCount = 0;
        DHJassFunction function;
        public triggeraction(DHJassFunction function)
        {
            this.function = function;
        }
        public string debugInfo
        {
            get
            {
                return function.Name;
            }
        }
        public void Execute()
        {
            if (DHJassExecutor.ShowTriggerActions)
                Console.WriteLine("Executing TriggerAction: " + function.Name);
            function.Execute();
            execCount++;
        }
        public int ExecCount
        {
            get { return execCount; }
        }
        public override string ToString()
        {
            return "triggeraction: '" + debugInfo + "'";
        }
    }
    public class triggercondition : handlevalue
    {
        //DHJassHandle pCondition;
        int evalCount = 0;
        boolexpr condition;
        public triggercondition(boolexpr condition)
        {
            this.condition = condition;
        }
        public bool GetResult()
        {
            bool result = condition.GetResult();

            evalCount++;

            return result;
        }
        public int EvalCount
        {
            get { return evalCount; }
        }
    }

    public class timer : handlevalue
    {
        public static double timeFactor = 1.0;
        public static bool allowAlternateInstantTimers = false;        
        public static List<timer> History = null;
        public static int maxTimerIntervalForHistory = -1;
        public static int activeTimerCount = 0;
        //System.Threading.Timer t;
        //System.Timers.Timer t;
        System.Windows.Forms.Timer t;
        System.Timers.Timer instantTimer;
        bool periodic = false;
        event DHJassEventHandler tick;
        int lastTickTime;        

        private static bool enableHistory = false;
        public static bool EnableHistory
        {
            get
            {
                return enableHistory;
            }
            set
            {
                enableHistory = value;
                if (enableHistory)
                    History = new List<timer>();
                else
                    History = null;
            }
        }

        public void start(double timeout, bool periodic)
        {
            //TimerCallback tc = new TimerCallback(callback);
            // 500 is for faster debugging
            int timeout_ms = (int)(timeout * (timeFactor * 1000));//1000);

            if (timeout_ms == 0 && allowAlternateInstantTimers)
            {
                instantTimer = new System.Timers.Timer(1); // 1 ms
                instantTimer.Elapsed += new System.Timers.ElapsedEventHandler(callback);
                instantTimer.AutoReset = false;//periodic;
                instantTimer.Enabled = true;                
                return;
            }

            if (timeout_ms == 0) timeout_ms = 1;

            this.periodic = periodic;
            //t = new System.Threading.Timer(tc,null,timeout_ms,(periodic ? timeout_ms : 0));

            /*t = new System.Timers.Timer(timeout_ms);
            t.Elapsed += new System.Timers.ElapsedEventHandler(callback);
            t.AutoReset = false;//periodic;
            t.Enabled = true;*/

            t = new System.Windows.Forms.Timer();
            t.Tick += new EventHandler(t_Tick);
            t.Interval = timeout_ms;

            lastTickTime = Environment.TickCount;

            t.Start(); activeTimerCount++;
            if (enableHistory && timeout_ms <= maxTimerIntervalForHistory)
                History.Add(this);

            //callback(null, System.Timers.ElapsedEventArgs.Empty as System.Timers.ElapsedEventArgs);
        }       

        void t_Tick(object sender, EventArgs e)
        {
            if (t != null)
            {
                try
                {
                    callback(null, System.Timers.ElapsedEventArgs.Empty as System.Timers.ElapsedEventArgs);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Timer " + this.handle + " callback exception: " + ex.Message);
                }
            }

            if (t != null && !periodic) { t.Stop(); activeTimerCount--; }

            lastTickTime = Environment.TickCount;
        }

        public void SetCallback(DHJassEventHandler callback)
        {
            tick = null;
            tick += callback;
        }

        void callback(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (tick != null)
                tick(null, new DHJassEventArgs(new KeyValuePair<string, object>("timer", this)));
        }

        public void forceCallback()
        {
            t_Tick(null, EventArgs.Empty);
        }

        /*protected void callback(object obj)
        {
            if (tick != null)
                tick(null, new DHJassEventArgs());
        }*/

        public override void destroy()
        {
            if (t != null)
            {
                if (t.Enabled)
                {
                    t.Stop();
                    activeTimerCount--;
                }
                t = null;
                tick = null;
            }
            //t.Dispose();
            base.destroy();
        }

        public double Elapsed
        {
            get
            {
                if (t == null) return 0;

                int elapsed = Environment.TickCount - lastTickTime;
                return ((double)elapsed) / (timeFactor * 1000);
            }
        }
        public double Remaining
        {
            get
            {
                if (t == null) return 0;

                int remaining = t.Interval - (Environment.TickCount - lastTickTime);
                return ((double)remaining) / (timeFactor * 1000);
            }
        }        
    }
    public interface IGeometry
    {
        bool ContainsXY(double x, double y);
    }
    public class rect : handlevalue, IGeometry
    {
        double minx, miny;
        double maxx, maxy;
        public rect(double minx, double miny, double maxx, double maxy)
        {
            this.minx = minx;
            this.miny = miny;
            this.maxx = maxx;
            this.maxy = maxy;
        }

        public bool ContainsXY(double x, double y)
        {
            return (x > minx && x < maxx &&
                y > miny && y < maxy);
        }
        public bool ContainsXY(double x, double y, double errorValue)
        {
            return ((x + errorValue) > minx && (x - errorValue) < maxx &&
                (y + errorValue) > miny && (y - errorValue) < maxy);
        }
        public double MinX { get { return minx; } }
        public double MaxX { get { return maxx; } }
        public double MinY { get { return miny; } }
        public double MaxY { get { return maxy; } }
        public double GetCenterX()
        {
            return (minx + maxx) / 2;
        }
        public double GetCenterY()
        {
            return (miny + maxy) / 2;
        }        
    }
    public class region : handlevalue, IGeometry
    {
        List<IGeometry> geometries = new List<IGeometry>();
        List<unit> units = new List<unit>();
        public region()
        {
        }
        public void addRect(rect r)
        {
            geometries.Add(r);
        }
        public bool ContainsXY(double x, double y)
        {
            foreach (IGeometry g in geometries)
                if (g.ContainsXY(x, y)) return true;
            return false;
        }
        public bool EnterRegion(unit u, double oldx, double oldy)
        {
            return !ContainsXY(oldx, oldy) && ContainsXY(u.x, u.y);
        }
        public bool LeaveRegion(unit u, double oldx, double oldy)
        {
            return ContainsXY(oldx, oldy) && !ContainsXY(u.x, u.y);
        }
        /// <summary>
        /// checks if unit's location is inside the circle with center at x,y and specified radius
        /// </summary>
        /// <param name="u"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <returns></returns> 
        public static bool IsUnitInRange(unit u, double x, double y, double radius)
        {
            // a^2 + b^2 = c^2
            return ((u.x - x) * (u.x - x) + (u.y - y) * (u.y - y)) <= (radius * radius);
        }
        public static bool IsPointInRange(double pointX, double pointY, double x, double y, double radius)
        {
            // a^2 + b^2 = c^2
            return ((pointX - x) * (pointX - x) + (pointY - y) * (pointY - y)) <= (radius * radius);
        }
    }
    public class gamecache : handlevalue
    {
        static Dictionary<string, HabPropertiesCollection> gcDatabase = new Dictionary<string, HabPropertiesCollection>();
        public static void Reset()
        {
            gcDatabase.Clear();
        }

        HabPropertiesCollection hpcCache;
        public gamecache(string name)
        {
            if (!gcDatabase.TryGetValue(name, out hpcCache))
            {
                hpcCache = new HabPropertiesCollection();
                gcDatabase.Add(name, hpcCache);
            }
        }
        public void flush()
        {
            hpcCache.Clear();
            this.destroy();
        }
        public void FlushStoredMission(string missionKey)
        {
            if (missionKey != null)
            hpcCache.Remove(missionKey);
        }
        public void StoreValue(string missionKey, string key, object value)
        {
            HabProperties hps;
            if (!hpcCache.TryGetValue(missionKey, out hps))
            {
                hps = new HabProperties();
                hpcCache.AddUnchecked(missionKey, hps);
            }

            hps[key] = value;
        }
        public object GetStoredValue(string missionKey, string key)
        {
            HabProperties hps;
            if (hpcCache.TryGetValue(missionKey, out hps))
                return hps.GetValue(key);
            else
                return null;
        }
        public bool HaveStoredValue(string missionKey, string key)
        {
            HabProperties hps;
            return hpcCache.TryGetValue(missionKey, out hps) && hps.ContainsKey(key);
        }
    }
    public class hashtable : handlevalue
    {
        public class subhashtable<T> : Dictionary<int, Dictionary<int, T>>
        {
            public T this[int parentKey, int childKey]
            {
                get
                {
                    Dictionary<int, T> child;
                    if (!this.TryGetValue(parentKey, out child))
                        return default(T);

                    T value;
                    child.TryGetValue(childKey, out value);
                    return value;
                }
                set
                {
                    Dictionary<int, T> child;
                    if (!this.TryGetValue(parentKey, out child))
                    {
                        child = new Dictionary<int, T>();
                        this.Add(parentKey, child);
                    }

                    child[childKey] = value;                    
                }
            }
            public bool Contains(int parentKey, int childKey)
            {
                Dictionary<int, T> child;
                return (this.TryGetValue(parentKey, out child) && child.ContainsKey(childKey));
            }
            public bool Remove(int parentKey, int childKey)
            {
                Dictionary<int, T> child;
                return (this.TryGetValue(parentKey, out child) && child.Remove(childKey));
            }
        }

        subhashtable<int> dcIntHashtable = new subhashtable<int>();
        subhashtable<double> dcRealHashtable = new subhashtable<double>();
        subhashtable<bool> dcBoolHashtable = new subhashtable<bool>();
        subhashtable<string> dcStrHashtable = new subhashtable<string>();
        subhashtable<handlevalue> dcHandleHashtable = new subhashtable<handlevalue>();

        public hashtable()
        {           
        }
        public void flush()
        {
            dcIntHashtable.Clear();
            dcRealHashtable.Clear();
            dcBoolHashtable.Clear();
            dcStrHashtable.Clear();
            dcHandleHashtable.Clear();            
            this.destroy();
        }        

        public void SaveInteger(int parentKey, int childKey, int value)
        {
            dcIntHashtable[parentKey, childKey] = value;
        }
        public void SaveReal(int parentKey, int childKey, double value)
        {
            dcRealHashtable[parentKey, childKey] = value;
        }
        public void SaveBoolean(int parentKey, int childKey, bool value)
        {
            dcBoolHashtable[parentKey, childKey] = value;
        }
        public void SaveString(int parentKey, int childKey, string value)
        {
            dcStrHashtable[parentKey, childKey] = value;
        }
        public void SaveHandle(int parentKey, int childKey, handlevalue value)
        {
            dcHandleHashtable[parentKey, childKey] = value;
        }

        public int LoadInteger(int parentKey, int childKey)
        {
            return dcIntHashtable[parentKey, childKey];
        }
        public double LoadReal(int parentKey, int childKey)
        {
            return dcRealHashtable[parentKey, childKey];
        }
        public bool LoadBoolean(int parentKey, int childKey)
        {
            return dcBoolHashtable[parentKey, childKey];
        }
        public string LoadString(int parentKey, int childKey)
        {
            return dcStrHashtable[parentKey, childKey];
        }
        public handlevalue LoadHandle(int parentKey, int childKey)
        {
            return dcHandleHashtable[parentKey, childKey];
        }
        public handlevalue LoadHandle<T>(int parentKey, int childKey) where T : handlevalue
        {
            return dcHandleHashtable[parentKey, childKey] as T;
        }

        public bool HaveSavedInteger(int parentKey, int childKey)
        {
            return dcIntHashtable.Contains(parentKey, childKey);
        }
        public bool HaveSavedReal(int parentKey, int childKey)
        {
            return dcRealHashtable.Contains(parentKey, childKey);
        }
        public bool HaveSavedBoolean(int parentKey, int childKey)
        {
            return dcBoolHashtable.Contains(parentKey, childKey);
        }
        public bool HaveSavedString(int parentKey, int childKey)
        {
            return dcStrHashtable.Contains(parentKey, childKey);
        }
        public bool HaveSavedHandle(int parentKey, int childKey)
        {
            return dcHandleHashtable.Contains(parentKey, childKey);
        }

        public void RemoveSavedInteger(int parentKey, int childKey)
        {
            dcIntHashtable.Remove(parentKey, childKey);
        }
        public void RemoveSavedReal(int parentKey, int childKey)
        {
            dcRealHashtable.Remove(parentKey, childKey);
        }
        public void RemoveSavedBoolean(int parentKey, int childKey)
        {
            dcBoolHashtable.Remove(parentKey, childKey);
        }
        public void RemoveSavedString(int parentKey, int childKey)
        {
            dcStrHashtable.Remove(parentKey, childKey);
        }
        public void RemoveSavedHandle(int parentKey, int childKey)
        {
            dcHandleHashtable.Remove(parentKey, childKey);
        }

        public void FlushChildHashtable(int parentKey)
        {
            dcIntHashtable.Remove(parentKey);
            dcRealHashtable.Remove(parentKey);
            dcBoolHashtable.Remove(parentKey);
            dcStrHashtable.Remove(parentKey);
            dcHandleHashtable.Remove(parentKey);                         
        }       
    }
    public class location : handlevalue, IGeometry
    {
        public double x;
        public double y;
        public location(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public bool ContainsXY(double x, double y)
        {
            return this.x == x && this.y == y;
        }
    }
    public class range : IGeometry
    {
        unit center;
        double radius;
        public range(unit center, double radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool ContainsXY(double x, double y)
        {
            return (x - center.x) * (x - center.x) + (y - center.y) * (y - center.y) <= radius * radius;            
        }
        public bool EnterRange(unit u, double oldx, double oldy)
        {
            return (u != center) && !ContainsXY(oldx, oldy) && ContainsXY(u.x, u.y);
        }
        public bool LeaveRange(unit u, double oldx, double oldy)
        {
            return (u != center) && ContainsXY(oldx, oldy) && !ContainsXY(u.x, u.y);
        }
    }
}
