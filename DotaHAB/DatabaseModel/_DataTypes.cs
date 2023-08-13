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

    // типы данных
    namespace DataTypes
    {
        using Data;        
        using System.Drawing.Imaging;
        using Abilities;
        using System.Collections.Generic;
        using Jass.Native.Types;

        #region pragma enable
#pragma warning disable 660
        #endregion

        public class FieldsKnowledge
        {
            public static Dictionary<Type, IField> TypeUnitPairs = new Dictionary<Type, IField>();            

            static FieldsKnowledge()
            {                
                TypeUnitPairs = CollectFields();                                               
            }
            public static void WakeUp(){}

            public static Dictionary<Type, IField> CollectFields()
            {
                Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

                StringCollection sc = new StringCollection();

                sc.Add("DotaHIT.DatabaseModel.DataTypes");
                sc.Add("DotaHIT.DatabaseModel.Abilities");
                sc.Add("DotaHIT.DatabaseModel.Upgrades");

                Type[] types = m.FindTypes(new TypeFilter(SearchCriteria), sc);

                Dictionary<Type, IField> units = new Dictionary<Type, IField>();

                object[] args = new object[3]{"name",null,new object[]{}};
                Type[] arg_types = new Type[3]{typeof(string),typeof(IRecord),typeof(object[])};

                object[] arg = new object[1] { null };
                Type[] arg_type = new Type[1] { typeof(object) };

                BindingFlags invokeAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly |
                                    BindingFlags.Instance | BindingFlags.CreateInstance;

                for (int i = 0; i < types.Length; i++)
                {
                    ConstructorInfo[] constructors = types[i].GetConstructors(invokeAttr);

                    try
                    {
                        bool Ok = false;

                        foreach (ConstructorInfo ci in constructors)
                            if (ci.GetParameters().Length == 0)
                            {
                                IField dbu = (IField)types[i].InvokeMember(null, invokeAttr, null, null, null);
                                units.Add(types[i], dbu);
                                Ok = true;
                                break;
                            }

                        if (!Ok)
                        foreach (ConstructorInfo ci in constructors)
                        {
                            ParameterInfo[] pi = ci.GetParameters();
                            if (pi.Length == 1)
                            {
                                IField dbu = (IField)types[i].InvokeMember(null, invokeAttr, null, null, arg);
                                units.Add(types[i], dbu);
                                Ok = true;
                                break;                                
                            }
                        }

                        if (!Ok)
                            foreach (ConstructorInfo ci in constructors)
                            {
                                ParameterInfo[] pi = ci.GetParameters();
                                if (pi.Length == 3 
                                    && pi[0].ParameterType == typeof(string)
                                    && pi[1].ParameterType == typeof(IRecord)
                                    && pi[2].ParameterType == typeof(object[]))
                                {
                                    IField dbu = (IField)types[i].InvokeMember(null, invokeAttr, null, null, args);
                                    units.Add(types[i], dbu);
                                    Ok = true;
                                    break;
                                }
                            }                                                                              
                    }
                    catch
                    {
                    }
                }

                return units;
            }            
            private static bool SearchCriteria(Type t, object filter)
            {
                if (((StringCollection)filter).Contains(t.Namespace) && t.IsPublic
                    && (t.GetInterface(typeof(IField).Name) != null) && !t.IsAbstract)                    
                    return true;
                return false;
            }            
        }        

        public enum PrimAttrType
        {
            None = 0,
            _ = None,
            Str,
            Agi,
            Int
        }

        [Flags]
        public enum TargetType
        {
            None = 0, Default = None, _ = None,
            Air = 1,
            Ground = 2,
            Organic = 4,
            Self = 8,
            Notself = 16,
            Invu = 32, Invulnerable = Invu,
            Vuln = 64, Vulnerable = Vuln,
            Nonancient = 128,
            Friend = 256,
            Enemy = 512,
            Enemies = 1024,
            Debris = 2048,
            Structure = 4096,
            Ally = 8192,
            Allies = 16384,
            Neutral = 32768,
            Nonhero = 65536,
            Hero = 131072,
            Tree = 262144,
            Ward = 524288,
            Item = 1048576,
            Player = 2097152,
            Dead = 4194304,
            Alive = 8388608,
            Nonsapper = 16777216
        }

        [Flags]
        public enum AttackMethod
        {
            None = 0,
            Normal = 1, Default = None,
            Range = 2,            
            Melee = Normal,
            Missile = Range,
            Artillery = Range,
            Msplash = 4,
            Mbounce = Range
        }
              
        public enum SmartMethodType
        {
            Factor,
            Summ,
            Max,
            AnyOne,
            Last
        }

        public enum AttackType
        {
            Hero = 0,
            Spell         
        }
        public enum DamageType
        {
            Normal=0,
            Magic         
        }

        public struct Damage
        {
            double min;            
            double max;            

            public Damage(int damage)
            {
                this.min = (double)damage;
                this.max = damage;
            }

            public Damage(double damage)
            {
                this.min = damage;
                this.max = damage;
            }

            public Damage(string text)
            {
                if (text.Contains("-"))
                {
                    string[] dmg_parts = text.Split(new char[] { '-' }, StringSplitOptions.None);

                    try
                    {
                        min = Convert.ToInt32(dmg_parts[0]);
                        max = Convert.ToInt32(dmg_parts[1]);
                    }
                    catch
                    {
                        min = max = 0;
                    }
                }
                else
                {
                    try
                    {
                        min = max = Convert.ToInt32(text);
                    }
                    catch
                    {
                        min = max = 0;
                    }
                }
            }

            public Damage(int min, int max)
            {
                this.min = min;
                this.max = max;
            }

            public Damage(double min, double max)
            {
                this.min = min;
                this.max = max;
            }

            public override string ToString()
            {
                if (min == max)
                    return min.ToString(DBDOUBLE.format, DBDOUBLE.provider);
                else
                    return min.ToString(DBDOUBLE.format, DBDOUBLE.provider) + " - " + max.ToString(DBDOUBLE.format, DBDOUBLE.provider);
            }

            public int get_damage_int()
            {
                if (min == max) return (int)min;
                else return (int) ((min + max) / 2);
            }
            public double get_damage_double()
            {
                if (min == max) return min;
                else return ((min + max) / 2);
            }

            public void set_damage(int value)
            {
                min = max = value;
            }
            public void set_damage(double value)
            {
                min = max = value;
            }
            public void set_bonus(int amount)
            {
                min += amount;
                max += amount;
            }
            public void set_bonus(double amount)
            {
                min += amount;
                max += amount;
            }

            public double Min
            {
                get { return min; }
                set { min = value; }
            }
            public double Max
            {
                get { return max; }
                set { max = value; }
            }

            public static implicit operator Damage(int value)
            {
                return new Damage(value);
            }
            public static implicit operator int(Damage value)
            {
                return value.get_damage_int();
            }
            public static implicit operator double(Damage value)
            {
                return value.get_damage_double();
            }
            public static bool operator >(Damage a, int b)
            {
                return a.get_damage_int() > b;
            }
            public static bool operator <(Damage a, int b)
            {
                return a.get_damage_int() < b;
            }
            public static bool operator !=(Damage a, int b)
            {
                return a.get_damage_int() != b;
            }
            public static bool operator ==(Damage a, int b)
            {
                return a.get_damage_int() == b;
            }
            public static Damage operator +(Damage a, int b)
            {
                return new Damage(a.min + b, a.max + b);
            }
            public static Damage operator -(Damage a, int b)
            {
                return new Damage(a.min - b, a.max - b);
            }
            public static Damage operator +(Damage a, double b)
            {
                return new Damage(a.min + b, a.max + b);
            }
            public static Damage operator -(Damage a, double b)
            {
                return new Damage(a.min - b, a.max - b);
            }
            public static Damage operator +(Damage a, Damage b)
            {
                return new Damage(a.min + b.min, a.max + b.max);
            }
            public static Damage operator -(Damage a, Damage b)
            {
                return new Damage(a.min - b.min, a.max - b.max);
            }
            public static Damage operator *(Damage a, double b)
            {
                return new Damage(a.min * b, a.max * b);
            }
            public static Damage operator /(Damage a, double b)
            {
                return new Damage(a.min / b, a.max / b);
            }
            public static bool operator >=(Damage a, Damage b)
            {
                return a.get_damage_double() >= b.get_damage_double();
            }
            public static bool operator <=(Damage a, Damage b)
            {
                return a.get_damage_double() <= b.get_damage_double();
            }
            public static Damage GetMax(Damage a, Damage b)
            {
                if (a.get_damage_double() >= b.get_damage_double())
                    return a;
                else
                    return b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }             

        public abstract class DBBINARY : DbUnit
        {
            protected byte[] byteArray;

            public virtual int this[int index]
            {
                get
                {
                    return byteArray[index];
                }
                set
                {
                    byteArray[index] = (byte)value;
                }
            }

            internal DBBINARY()
            {
                byteArray = null;
                isNull = true;
            }
            internal DBBINARY(byte[] value)
            {
                byteArray = value;
            }
            internal DBBINARY(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }

            public override int Size
            {
                get
                {
                    return byteArray.Length;
                }
            }
            public override object Value
            {
                get
                {
                    return byteArray;
                }
                set
                {
                    byteArray = (byte[])ToValue(value, out isNull);
                }
            }
        }                

        public abstract class DBSMARTVALUE<T> : List<T>, IField where T:struct
        {
            private SmartMethodType smartMethod = SmartMethodType.Summ;

            protected bool changed = false;
            protected T smartValue = new T();

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            internal DBSMARTVALUE() { }
            internal DBSMARTVALUE(string Name, IRecord Owner, FieldAttributeCollection attributes)
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

            public T GetSmartValue()
            {
                if (changed)
                {
                    CalculateSmartValue();
                    changed = false;
                }

                return smartValue;
            }
            protected abstract T CalculateSmartValue();
            
            public virtual object Value
            {
                get
                {
                    return this.GetSmartValue();
                }
                set
                {
                    bool isNull;

                    object result = ToValue(value, out isNull);

                    if (result is T) this.Add((T)result);
                }
            }
            public abstract object ToValue(object value, out bool IsNull);

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
                    return Value;
                }
            }
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return (this.Count == 0);
                }
            }

            public SmartMethodType SmartMethod
            {
                get { return smartMethod; }
                set { smartMethod = value; }
            }

            public new T this[int index]
            {
                get
                {
                    return base[index];
                }
                set
                {
                    base[index] = value;
                    changed = true;
                }
            }
            public new void Add(T item)
            {
                base.Add(item);
                changed = true;               
            }
            public new void Remove(T item)
            {
                base.Remove(item);
                changed = true;                
            }
            public new void Clear()
            {
                base.Clear();
                changed = true;
            }

            public virtual int convert_to_int()
            {
                try
                {
                    return Convert.ToInt32(AutoValue);
                }
                catch (System.FormatException) { return 0; }
            }

            public virtual double convert_to_double()
            {
                try
                {
                    return Convert.ToDouble(AutoValue);
                }
                catch (System.FormatException) { return 0.0; }
            }

            public virtual string Text
            {
                get
                {
                    return Value.ToString();
                }
            }
            
            public override string ToString()
            {
                return Text;
            }
            public bool ValueEquals(object obj)
            {
                return DbUnit.ValueEquals(this, obj);
            }

            public abstract IField New(string Name, IRecord Owner, FieldAttributeCollection attributes);
            public abstract IField NewCopy(IRecord Owner, bool setValue);            
        }

        public class DBSMARTDOUBLE : DBSMARTVALUE<double>
        {  
            internal DBSMARTDOUBLE() { }
            internal DBSMARTDOUBLE(string Name, IRecord Owner, FieldAttributeCollection attributes)
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

            protected override double CalculateSmartValue()
            {
                smartValue = 0.0;

                switch (SmartMethod)
                {
                    case SmartMethodType.Max:
                        foreach (double value in this)
                            smartValue = Math.Max(smartValue, value);
                        break;

                    case SmartMethodType.Summ:
                        foreach (double value in this)
                            smartValue += value;
                        break;

                    case SmartMethodType.Factor:
                        smartValue = 1;
                        foreach (double value in this)
                            smartValue *= (1 - value);
                        smartValue = 1 - smartValue;
                        break;
                }

                return smartValue;
            }            
           
            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is DbUnit)
                {
                    DbUnit dbu = value as DbUnit;

                    IsNull = dbu.IsNull;

                    return dbu.convert_to_double();                   
                }

                if (value is double)
                    return value;
                else
                    if (value is int)
                        return (double)(int)value;
                    else
                        if (value is string)
                        {
                            double strValue;

                            if (!double.TryParse(value as string, NumberStyles.Float, DBDOUBLE.provider, out strValue))
                            {
                                IsNull = true;
                                return null;
                            }

                            return strValue;
                        }
                        else
                        {
                            IsNull = true;
                            return null;
                        }
            }                                      

            public override string Text
            {
                get
                {
                    return GetSmartValue().ToString(DBDOUBLE.format, DBDOUBLE.provider);
                }
            }

            public static implicit operator double(DBSMARTDOUBLE value)
            {
                return value.GetSmartValue();
            }                        

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBSMARTDOUBLE(Name, Owner, attributes);
            }
            public override IField NewCopy(IRecord Owner, bool setValue)
            {
                DBSMARTDOUBLE dbu = new DBSMARTDOUBLE(this.Name, Owner, this.attributes);
                if (setValue)
                    dbu.AddRange(this);
                return dbu;
            }
        }
        public class DBSMARTINT: DBSMARTVALUE<int>
        {
            internal DBSMARTINT() { }
            internal DBSMARTINT(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
            
            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is DbUnit)
                {
                    DbUnit dbu = value as DbUnit;

                    IsNull = dbu.IsNull;

                    return dbu.convert_to_int();
                }

                if (value is int)
                    return value;
                else
                    if (value is double)
                        return (int)(double)value;
                    else
                        if (value is string)
                        {
                            int strValue;

                            if (!int.TryParse(value as string, out strValue))
                            {
                                IsNull = true;
                                return null;
                            }

                            return strValue;
                        }
                        else
                        {
                            IsNull = true;
                            return null;
                        }                            
            }

            protected override int CalculateSmartValue()
            {
                smartValue = 0;

                switch (SmartMethod)
                {
                    case SmartMethodType.Max:
                        foreach (int value in this)
                            smartValue = Math.Max(smartValue, value);
                        break;

                    case SmartMethodType.Summ:
                        foreach (int value in this)
                            smartValue += value;
                        break;
                }

                return smartValue;
            }            

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBSMARTINT(Name, Owner, attributes);
            }
            public override IField NewCopy(IRecord Owner, bool setValue)
            {
                DBSMARTINT dbu = new DBSMARTINT(this.Name, Owner, this.attributes);
                if (setValue)
                    dbu.AddRange(this);
                return dbu;
            }

            public static implicit operator int(DBSMARTINT value)
            {
                return value.GetSmartValue();
            }
        }
        public class DBSMARTDAMAGE : DBSMARTVALUE<Damage>
        {
            internal DBSMARTDAMAGE() { }
            internal DBSMARTDAMAGE(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
           
            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)                
                    value = (value as IField).Value;

                if (value is Damage)
                    return value;
                else
                    if (value is double)
                        return new Damage((double)value);
                    else
                        if (value is int)
                            return new Damage((int)value);
                        else                            
                            if (value is string)                            
                                return new Damage(value as string);                            
                            else
                                return null;
            }

            protected override Damage CalculateSmartValue()
            {
                smartValue = 0;

                switch (SmartMethod)
                {
                    case SmartMethodType.Max:
                        foreach (Damage value in this)
                            smartValue = Damage.GetMax(smartValue, value);
                        break;

                    case SmartMethodType.Summ:
                        foreach (Damage value in this)
                            smartValue += value;
                        break;
                }

                return smartValue;
            }

            public static implicit operator Damage(DBSMARTDAMAGE value)
            {
                return value.GetSmartValue();
            }
            public static implicit operator int(DBSMARTDAMAGE value)
            {
                return (int)value.GetSmartValue();
            }

            public override int convert_to_int()
            {
                return (int)this.GetSmartValue();
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBSMARTDAMAGE(Name, Owner, attributes);
            }
            public override IField NewCopy(IRecord Owner, bool setValue)
            {
                DBSMARTDAMAGE dbu = new DBSMARTDAMAGE(this.Name, Owner, this.attributes);
                if (setValue)
                    dbu.AddRange(this);
                return dbu;
            }
        }

        public class DBCHAR : DbUnit
        {
            protected string strValue;

            public DBCHAR() 
            {
                strValue = null;
                isNull = true;
            }
            public DBCHAR(string value)
            {
                strValue = value;
            }            
            internal DBCHAR(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }

            public override int Size
            {
                get
                {
                    return strValue.Length;
                }
            }

            public override object Value
            {
                get
                {
                    return strValue;
                }
                set
                {
                    strValue = (string)ToValue(value, out isNull);
                }
            }

            public override object AutoValue
            {
                get
                {
                    return (strValue == null) ? "" : strValue;
                }
            }

            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is IList<string>)
                    if ((value as IList<string>).Count != 0)
                        value = (value as IList<string>)[0];

                if (value is string)                
                    return attributes.Process(value as string);                

                IsNull = true;
                return null;
            }

            public override DbValueType ValueType
            {
                get { return DbValueType.String; }
            }

            public static implicit operator DBCHAR(string value)
            {
                return new DBCHAR(value);
            }
            public static implicit operator string(DBCHAR value)
            {
                return (value==null)? null : value.strValue;
            }

            protected override DbUnit add(DbUnit b)
            {                
                return new DBCHAR(this.strValue + (b.Value as string));
            }
            protected override DbUnit subtract(DbUnit b)
            {                
                return new DBCHAR(this.strValue);
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBCHAR(Name, Owner, attributes);
            }
        };
        public class DBBIT : DbUnit
        {
            private byte bitValue;

            public DBBIT()
            {
                bitValue = 0;
                isNull = true;
            }
            public DBBIT(byte value)
            {
                bitValue = value;
            }
            internal DBBIT(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
            public override int Size
            {
                get
                {
                    return Marshal.SizeOf(bitValue);
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Number; }
            }
            public override object Value
            {
                get
                {
                    return bitValue;
                }
                set
                {
                    bitValue = (byte)ToValue(value, out isNull);
                }
            }
            public int IntValue
            {
                get
                {
                    return (int)bitValue;
                }
            }
            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is byte)
                {
                    return (byte)value;
                }
                else
                    if (value is bool)
                    {
                        return (byte)((bool)value ? 1 : 0);
                    }
                    else
                        if (value is int)
                            return (byte)(int)value;
                        else
                            if (value is string)
                            {
                                byte result;
                                if (byte.TryParse(value as string, out result))
                                    return result;
                                else
                                {
                                    IsNull = true;
                                    return (byte)0;
                                }
                            }

                IsNull = true;
                return (byte)0;
            }
            public override bool ValueEquals(object obj)
            {
                if (obj is bool)
                    obj = ((bool)obj) ? (byte)1 : (byte)0;
                return base.ValueEquals(obj);
            }

            public static implicit operator DBBIT(int value)
            {
                return new DBBIT((byte)value);
            }
            public static implicit operator int(DBBIT value)
            {
                return (int)value.bitValue;
            }
            public static implicit operator DBBIT(bool value)
            {
                return new DBBIT((byte)((value) ? 1 : 0));
            }
            public static implicit operator bool(DBBIT value)
            {
                return value.bitValue != 0;
            }
            public static bool operator ==(DBBIT a, bool b)
            {
                return (a.bitValue != 0) == b;
            }
            public static bool operator !=(DBBIT a, bool b)
            {
                return (a.bitValue != 0) != b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            protected override DbUnit add(DbUnit b)
            {
                return new DBBIT((byte)(this.bitValue + (byte)(b.Value)));
            }
            protected override DbUnit subtract(DbUnit b)
            {
                return new DBBIT((byte)(this.bitValue - (byte)(b.Value)));
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBBIT(Name, Owner, attributes);
            }
        };     
        public class DBINT : DbUnit
        {
            private int intValue;
            public static readonly string full_format = "+ 0;- 0;0";

            public DBINT()
            {
                intValue = 0;
            }
            public DBINT(int value)
            {
                intValue = value;
            }
            public DBINT(object value) : base(value) { }

            internal DBINT(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
            public override int Size
            {
                get
                {
                    return Marshal.SizeOf(intValue);
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Number; }
            }
            public override object Value
            {
                get
                {
                    return intValue;
                }
                set
                {
                    intValue = (int)ToValue(value, out isNull);
                }
            }

            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is int)                
                    return (int)value;                
                else
                    if (value is double)
                        return (int)(double)value; 
                    else
                        if (value is string)
                        {
                            int resultValue;
                            int.TryParse(value as string, out resultValue);
                            return resultValue;
                        }
                        else
                        {
                            IsNull = true;
                            return 0;
                        }    
            }

            public override int convert_to_int()
            {
                return intValue;
            }

            public static implicit operator DBINT(int value)
            {
                return new DBINT(value);
            }
            public static implicit operator int(DBINT value)
            {
                return value.intValue;
            }
            public static bool operator ==(DBINT a, int b)
            {
                return a.intValue == b;
            }
            public static bool operator !=(DBINT a, int b)
            {
                return a.intValue != b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static int operator+(DBINT a, DBINT b)
            {
                return a.intValue + b.intValue;
            }
            public static int operator-(DBINT a, DBINT b)
            {
                return a.intValue - b.intValue;
            }

            protected override DbUnit add(DbUnit b)
            {
                return new DBINT(this.convert_to_double() + b.convert_to_double());
            }
            protected override DbUnit subtract(DbUnit b)
            {
                return new DBINT(this.convert_to_double() - b.convert_to_double());
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBINT(Name, Owner, attributes);
            }
        };
        public class DBBIGINT : DbUnit
        {
            private long longValue;

            public DBBIGINT()
            {
                longValue = 0;
            }
            public DBBIGINT(long value)
            {
                longValue = value;
            }
            public DBBIGINT(object value) : base(value) { }

            internal DBBIGINT(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
            public override int Size
            {
                get
                {
                    return Marshal.SizeOf(longValue);
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Number; }
            }
            public override object Value
            {
                get
                {
                    return longValue;
                }
                set
                {
                    longValue = (long)ToValue(value, out isNull);
                }
            }

            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is long || value is int)
                {
                    return (long)value;
                }
                else
                if (value is string || value is double)
                {
                    long resultValue;

                    try { resultValue = Convert.ToInt64(value); }
                    catch { IsNull = true; resultValue = 0; }

                    return resultValue;
                }
                else
                {
                    IsNull = true;
                    return 0;
                }
            }

            public static implicit operator DBBIGINT(long value)
            {
                return new DBBIGINT(value);
            }
            public static implicit operator long(DBBIGINT value)
            {
                return value.longValue;
            }
            public static bool operator ==(DBBIGINT a, int b)
            {
                return a.longValue == b;
            }
            public static bool operator !=(DBBIGINT a, int b)
            {
                return a.longValue != b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            protected override DbUnit add(DbUnit b)
            {
                return new DBBIGINT(this.convert_to_double() + b.convert_to_double());
            }
            protected override DbUnit subtract(DbUnit b)
            {
                return new DBBIGINT(this.convert_to_double() - b.convert_to_double());
            }
        };
        public class DBIMAGE : DbUnit
        {
            protected Bitmap image;

            public DBIMAGE() { image = null; isNull = true; }
            public DBIMAGE(Bitmap value) { this.image = value; }
            internal DBIMAGE(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }

            public override int Size
            {
                get
                {
                    return 0;
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Image; }
            }

            public override object Value
            {
                get
                {
                    return image;
                }
                set
                {
                    image = (Bitmap)ToValue(value, out isNull);
                }
            }
            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is Bitmap)                
                    return value;                

                IsNull = true;
                return null;
            }
            
            public bool LoadImage(string filePath)
            {
                if (new FileInfo(filePath).Exists == false)
                    return false;

                try
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);

                    byte[] byteArray = br.ReadBytes((int)fs.Length);
                    Value = byteArray;

                    br.Close();
                    fs.Close();
                }
                catch { return false; }

                return true;
            }
            public void SaveImage(string filePath)
            {
                if (image == null) return;
                image.Save(filePath);
            }          
            public static implicit operator DBIMAGE(Bitmap value)
            {
                return FromBitmap(value);
            }
            public static implicit operator Bitmap(DBIMAGE value)
            {
                return ToBitmap(value);
            }
            public static Bitmap ToBitmap(DBIMAGE value)
            {
                if (value != null && value.image != null)
                    return value.image;

                return null;
            }
            public static DBIMAGE FromBitmap(Bitmap value)
            {
                if (value == null) return new DBIMAGE();

                return new DBIMAGE(value);
            }
            public static DBIMAGE FromImage(string filePath)
            {
                Bitmap bmp = (Bitmap)Bitmap.FromFile(filePath);
                return new DBIMAGE(bmp);
            }
            protected override DbUnit add(DbUnit b)
            {
                return null;
            }
            protected override DbUnit subtract(DbUnit b)
            {
                return null;
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBIMAGE(Name, Owner, attributes);
            }
        };
        public class DBDATETIME : DbUnit
        {
            private DateTime datetimeValue;

            public DBDATETIME()
            {
                datetimeValue = new DateTime();
            }
            public DBDATETIME(DateTime value)
            {
                datetimeValue = value;
            }
            internal DBDATETIME(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
            public override int Size
            {
                get
                {
                    return Marshal.SizeOf(datetimeValue.Ticks);
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Struct; }
            }
            public override object Value
            {
                get
                {
                    return datetimeValue;
                }
                set
                {
                    datetimeValue = (DateTime)ToValue(value, out isNull);
                }
            }            
            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is DateTime)
                {
                    return (DateTime)value;
                }
                else
                    if (value is string)
                    {
                        DateTime dt;

                        try
                        {
                            dt = Convert.ToDateTime((string)value);
                        }
                        catch (Exception)
                        {
                            return DateTime.MinValue;
                        }
                        return dt;
                    }

                IsNull = true;
                return DateTime.MinValue;
            }
            public string DateText
            {
                get
                {
                    return (isNull)? "" : datetimeValue.ToShortDateString();
                }
            }
            public string TimeText
            {
                get
                {
                    return (isNull) ? "" : datetimeValue.ToShortTimeString();
                }
            }
            public string ToText(string format)
            {
                return (isNull) ? "" : datetimeValue.ToString(format);
            }
            internal DateTime InternalValue
            {
                get
                {
                    return datetimeValue;
                }
            }
            public DateTime AddTime(DateTime Time)
            {
                return datetimeValue.AddHours(Time.Hour).AddMinutes(Time.Minute).AddSeconds(Time.Second);
            }
            public DateTime SubtractTime(DateTime Time)
            {
                return datetimeValue.AddHours(-Time.Hour).AddMinutes(-Time.Minute).AddSeconds(-Time.Second);
            }
            public long DifferenceInDays(DateTime DateTo)
            {
                return DifferenceInDays(datetimeValue, DateTo);
            }
            /// <summary>
            /// устанавливает дату (время остается без изменений)
            /// </summary>
            /// <param name="Date"></param>
            public void DateAlign(DateTime Date)
            {
                datetimeValue = new DateTime(Date.Year, Date.Month, Date.Day,
                                            datetimeValue.Hour, datetimeValue.Minute, datetimeValue.Second,
                                            datetimeValue.Millisecond);
            }
            /// <summary>
            /// возвращает время представленное в количестве секунд 
            /// </summary>
            /// <returns></returns>
            public long TimeToSeconds()
            {
                if (isNull)
                    return 0;
                else                
                    return (datetimeValue.Hour * 3600) + (datetimeValue.Minute * 60) + datetimeValue.Second;                
            }

            /// <summary>
            /// возвращает разницу в днях между двумя DateFrom и DateTo
            /// </summary>
            /// <param name="DateFrom"></param>
            /// <param name="DateTo"></param>
            /// <returns></returns>
            public static long DifferenceInDays(DateTime DateFrom,DateTime DateTo)
            {
                if (DateFrom.Date == DateTo.Date) return 0;

                long seconds = (DateTo.Date.Ticks - DateFrom.Date.Ticks) / 10000000;
                long minutes = seconds / 60;
                long hours = minutes / 60;
                long days = hours / 24;

                return days;
            }
            /// <summary>
            /// возвращает разницу в секундах между двумя DateFrom и DateTo
            /// </summary>
            /// <param name="DateFrom"></param>
            /// <param name="DateTo"></param>
            /// <returns></returns>
            public static long DifferenceInMilliseconds(DateTime DateFrom, DateTime DateTo)
            {
                if (DateFrom == DateTo) return 0;

                long ms = (DateTo.Ticks - DateFrom.Ticks) / 10000;

                return ms;
            }
            /// <summary>
            /// возвращает разницу в секундах между двумя DateFrom и DateTo
            /// </summary>
            /// <param name="DateFrom"></param>
            /// <param name="DateTo"></param>
            /// <returns></returns>
            public static long DifferenceInSeconds(DateTime DateFrom, DateTime DateTo)
            {
                if (DateFrom == DateTo) return 0;

                long seconds = (DateTo.Ticks - DateFrom.Ticks) / 10000000;

                return seconds;
            }
            /// <summary>
            /// возвращает разницу в минутах между двумя DateFrom и DateTo
            /// </summary>
            /// <param name="DateFrom"></param>
            /// <param name="DateTo"></param>
            /// <returns></returns>
            public static long DifferenceInMinutes(DateTime DateFrom, DateTime DateTo)
            {
                if (DateFrom == DateTo) return 0;

                long seconds = (DateTo.Ticks - DateFrom.Ticks) / 10000000;
                long minutes = seconds / 60;

                return minutes;
            }
            /// <summary>
            /// возвращает разницу в часах между двумя DateFrom и DateTo
            /// </summary>
            /// <param name="DateFrom"></param>
            /// <param name="DateTo"></param>
            /// <returns></returns>
            public static long DifferenceInHours(DateTime DateFrom, DateTime DateTo)
            {
                if (DateFrom.Date == DateTo.Date) return 0;

                long seconds = (DateTo.Date.Ticks - DateFrom.Date.Ticks) / 10000000;
                long minutes = seconds / 60;
                long hours = minutes / 60;

                return hours;
            }

            public static implicit operator DBDATETIME(DateTime value)
            {
                return new DBDATETIME(value);
            }
            public static implicit operator DateTime(DBDATETIME value)
            {
                return value.datetimeValue;
            }
            public static bool operator <(DBDATETIME a, DBDATETIME b)
            {
                return a.datetimeValue < b.datetimeValue;
            }
            public static bool operator >(DBDATETIME a, DBDATETIME b)
            {
                return a.datetimeValue > b.datetimeValue;
            }

            protected override DbUnit add(DbUnit b)
            {
                return null;
            }
            protected override DbUnit subtract(DbUnit b)
            {
                return null;
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBDATETIME(Name, Owner, attributes);
            }
        };
        public class DBDOUBLE : DbUnit
        {
            private double doubleValue;
            public static readonly NumberFormatInfo provider = new NumberFormatInfo();
            public static readonly string format = "0.##";
            public static readonly string full_format = "+ 0.##;- 0.##;0.0";
      
            public DBDOUBLE()
            {                
                doubleValue = 0;                
            }
            public DBDOUBLE(double value)
            {
                doubleValue = value;                
            }
            public DBDOUBLE(object value):base(value) { }

            internal DBDOUBLE(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }            
            public override int Size
            {
                get
                {
                    return Marshal.SizeOf(doubleValue);
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Number; }
            }
            public override object Value
            {
                get
                {
                    return doubleValue;
                }
                set
                {
                    doubleValue = (double)ToValue(value, out isNull);
                }
            }
            public override string Text
            {
                get
                {
                    if (isNull) return "";
                    return doubleValue.ToString(format, provider);
                }
            }

            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is double)
                {
                    return (double)value;
                }
                else
                    if (value is float)
                        return (double)(float)value;
                    else
                    if (value is int)
                    {
                        return Convert.ToDouble((int)value);
                    }
                    else
                    if (value is string)
                    {
                        double strValue;

                        if (double.TryParse(value as string, NumberStyles.Float, DBDOUBLE.provider, out strValue) == false)
                        {
                            IsNull = true;
                            strValue = 0;
                        }

                        return strValue;
                    }
                    else
                    {
                        IsNull = true;
                        return 0.0;
                    }
            }

            public static implicit operator DBDOUBLE(double value)
            {
                return new DBDOUBLE(value);
            }
            public static implicit operator double(DBDOUBLE value)
            {
                return value.doubleValue;
            }
            public static bool operator ==(DBDOUBLE a, double b)
            {
                return a.doubleValue == b;
            }
            public static bool operator !=(DBDOUBLE a, double b)
            {
                return a.doubleValue != b;
            }
            public static double operator +(DBDOUBLE a, DBDOUBLE b)
            {
                return a.doubleValue + b.doubleValue;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            protected override DbUnit add(DbUnit b)
            {
                return new DBDOUBLE(this.doubleValue + b.convert_to_double());
            }
            protected override DbUnit subtract(DbUnit b)
            {
                return new DBDOUBLE(this.doubleValue - b.convert_to_double());
            }

            public string ToString(bool full_format)
            {
                if (isNull || full_format)
                    return doubleValue.ToString(DBDOUBLE.full_format, provider);

                return doubleValue.ToString(format, provider);
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBDOUBLE(Name, Owner, attributes);
            }
        };
        
        public class DBATTACKMETHOD : IField
        {
            private AttackMethod amValue;

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBATTACKMETHOD()
            {
                amValue = AttackMethod.Melee;
            }
            public DBATTACKMETHOD(AttackMethod value)
            {
                amValue = value;
            }

            internal DBATTACKMETHOD(string Name, IRecord Owner, FieldAttributeCollection attributes)
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
            public int Size
            {
                get
                {
                    return Marshal.SizeOf(amValue);
                }
            }
            public DbValueType ValueType
            {
                get { return DbValueType.Unknown; }
            }
            public object Value
            {
                get
                {
                    return amValue;
                }
                set
                {
                    bool isNull;
                    amValue = (AttackMethod)ToValue(value, out isNull);
                }
            }
            public object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is AttackMethod)
                {
                    return (AttackMethod)value;
                }
                else
                    if (value is string)
                    {
                        AttackMethod strValue;

                        if (value as string == "_") return AttackMethod.Default;

                        try { strValue = (AttackMethod)Enum.Parse(typeof(AttackMethod),value as string,true); }
                        catch { IsNull = true; strValue = AttackMethod.Melee; }

                        return strValue;
                    }
                    else
                    {
                        IsNull = true;
                        return 0;
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
            public string Text
            {
                get
                {
                    return amValue.ToString();
                }
            }
            public object AutoValue
            {
                get
                {
                    return null;
                }
            }
            public bool ValueEquals(object obj)
            {
                return false;
            }
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return amValue == AttackMethod.None;
                }
            }

            public override string ToString()
            {
                return Text;
            }

            public static implicit operator DBATTACKMETHOD(AttackMethod value)
            {
                return new DBATTACKMETHOD(value);
            }
            public static implicit operator AttackMethod(DBATTACKMETHOD value)
            {
                return value.amValue;
            }
            public static bool operator ==(DBATTACKMETHOD a, AttackMethod b)
            {
                return a.amValue == b;
            }
            public static bool operator !=(DBATTACKMETHOD a, AttackMethod b)
            {
                return a.amValue != b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBATTACKMETHOD(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBATTACKMETHOD dbu = new DBATTACKMETHOD(Name, Owner, attributes);

                if (setValue && !this.IsNull)
                    dbu.amValue = this.amValue;

                return dbu;
            }
        };
        public class DBPRIMATTR : IField
        {
            private PrimAttrType patValue;

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBPRIMATTR()
            {
                patValue = PrimAttrType.Agi;
            }
            public DBPRIMATTR(PrimAttrType value)
            {
                patValue = value;
            }

            internal DBPRIMATTR(string Name, IRecord Owner, FieldAttributeCollection attributes) 
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
            public int Size
            {
                get
                {
                    return Marshal.SizeOf(patValue);
                }
            }
            public DbValueType ValueType
            {
                get { return DbValueType.Unknown; }
            }
            public object Value
            {
                get
                {
                    return patValue;
                }
                set
                {
                    bool isNull;
                    patValue = (PrimAttrType)ToValue(value, out isNull);
                }
            }

            public object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is PrimAttrType)
                {
                    return (PrimAttrType)value;
                }
                else
                    if (value is string)
                    {
                        PrimAttrType strValue;

                        try { strValue = (PrimAttrType)Enum.Parse(typeof(PrimAttrType), value as string, true); }
                        catch { IsNull = true; strValue = PrimAttrType.Agi; }

                        return strValue;
                    }
                    else
                    {
                        IsNull = true;
                        return 0;
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
            public string Text
            {
                get
                {
                    return patValue.ToString();
                }
            }
            public object AutoValue
            {
                get
                {
                    return null;
                }
            }
            public bool ValueEquals(object obj)
            {
                return false;
            }
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return patValue == PrimAttrType.None;
                }
            }

            public static implicit operator DBPRIMATTR(PrimAttrType value)
            {
                return new DBPRIMATTR(value);
            }
            public static implicit operator PrimAttrType(DBPRIMATTR value)
            {
                return value.patValue;
            }
            public static bool operator ==(DBPRIMATTR a, PrimAttrType b)
            {
                return a.patValue == b;
            }
            public static bool operator !=(DBPRIMATTR a, PrimAttrType b)
            {
                return a.patValue != b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }          

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBPRIMATTR(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBPRIMATTR dbu = new DBPRIMATTR(Name, Owner, attributes);

                if (setValue && !this.IsNull)
                    dbu.patValue = this.patValue;

                return dbu;
            }
        };

        public class DBTARGETTYPE : IField
        {
            private TargetType atValue;

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBTARGETTYPE()
            {
                atValue = TargetType.None;
            }
            public DBTARGETTYPE(TargetType value)
            {
                atValue = value;
            }

            internal DBTARGETTYPE(string Name, IRecord Owner, FieldAttributeCollection attributes)
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
            public int Size
            {
                get
                {
                    return Marshal.SizeOf(atValue);
                }
            }
            public DbValueType ValueType
            {
                get { return DbValueType.Unknown; }
            }
            public object Value
            {
                get
                {
                    return atValue;
                }
                set
                {
                    bool isNull;
                    atValue = (TargetType)ToValue(value, out isNull);
                }
            }

            public object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is TargetType)
                {
                    return (TargetType)value;
                }
                else
                    if (value is string)
                    {
                        TargetType resultValue = TargetType.None;

                        DBSTRINGCOLLECTION targList = new DBSTRINGCOLLECTION(value as string);

                        foreach (string targ in targList)
                        {
                            TargetType targValue = TargetType.None;

                            try
                            {
                                targValue = (TargetType)Enum.Parse(typeof(TargetType), targ, true);
                                resultValue |= targValue;
                            }
                            catch { }
                        }

                        if (resultValue == TargetType.None)
                            IsNull = true;

                        return resultValue;
                    }
                    else
                    {
                        IsNull = true;
                        return 0;
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
            public string Text
            {
                get
                {
                    return atValue.ToString();
                }
            }
            public object AutoValue
            {
                get
                {
                    return null;
                }
            }
            public bool ValueEquals(object obj)
            {
                return false;
            }
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return atValue == TargetType.None;
                }
            }

            public static implicit operator DBTARGETTYPE(TargetType value)
            {
                return new DBTARGETTYPE(value);
            }
            public static implicit operator TargetType(DBTARGETTYPE value)
            {
                return value.atValue;
            }
            public static bool operator ==(DBTARGETTYPE a, TargetType b)
            {
                return a.atValue == b;
            }
            public static bool operator !=(DBTARGETTYPE a, TargetType b)
            {
                return a.atValue != b;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return Text;
            }

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBTARGETTYPE(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBTARGETTYPE dbu = new DBTARGETTYPE(Name, Owner, attributes);

                if (setValue && !this.IsNull)
                    dbu.atValue = this.atValue;

                return dbu;
            }
        };
        
        public class DBITEMSLOT : IField
        {
            protected string name;
            protected IRecord owner;            
            private item item;            

            protected bool forceNoRefresh = false;
            protected bool forceNoMessages = false;

            public DBITEMSLOT()
            {
                this.item = null;//new ITEM();
            }
            public DBITEMSLOT(item item)
            {
                this.item = item;
            }

            internal DBITEMSLOT(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {                
                //item = new item();
                item = null;
                this.name = Name;
                this.owner = Owner;
            }

            internal DBITEMSLOT(string Name, IRecord Owner, int index)
            {                
                item = null;
                this.name = Name;
                this.owner = Owner;
                this.Index = index;
            }

            public int Size
            {
                get
                {
                    return 0;                    
                }
            }
            public DbValueType ValueType
            {
                get { return DbValueType.Unknown; }
            }
            public object Value
            {
                get
                {                    
                    return item;
                }
                set
                {
                    if (value is item)
                        item = (value as item);
                    else
                        item = null;//new item();
                }
            }            
            public object ToValue(object value, out bool IsNull)
            {
                IsNull = true;
                return null;
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
                    return null;
                }
            }
            public bool IsSubstituteNameMatch(string value)
            {
                return false;
            }            
            public FieldAttributeCollection Attributes
            {
                get
                {
                    return null;
                }
            }
            public IRecord Owner
            {
                get
                {
                    return owner;
                }
            }
            public string Text
            {
                get
                {
                    return "";
                }
            }
            public object AutoValue
            {
                get
                {
                    return item;
                }
            }
            public bool ValueEquals(object obj)
            {
                return false;
            }
            public object GetCopy()
            {
                return Value;
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                IField dbuNew = New(this.name, Owner, null);
                if (setValue)
                    dbuNew.Value = this;
                return dbuNew;
            }

            public bool ForceNoRefresh
            {
                get { return forceNoRefresh; }
                set { forceNoRefresh = value; }
            }

            public bool ForceNoMessages
            {
                get { return forceNoMessages; }
                set { forceNoMessages = value; }
            }

            public int Index = -1;

            public bool put_item(item item)
            {
                if (item == null || item.IsDisposed) return false;

                if (item.powerup)
                {
                    item.abilities.SetLevels(1);                    
                    //item.abilities.Activated = true;
                    item.abilities.SetActivatedState(AbilityState.Activated);
                    item.abilities.SetOwner(this.owner as unit);
                    item.abilities.Apply();
                    item.abilities.ResetLevels();
                    return true;
                }

                if (this.item == null)
                {
                    this.item = item;
                    this.item.abilities.SetLevels(1);
                    this.item.abilities.SetSlot(this.Index);
                    this.item.set_owner(this.owner as unit);
                    if (!forceNoMessages && owner != null) (owner as unit).OnPickupItem(item);
                    return true;
                }
                else
                    return false;               
            }
            public virtual bool put_item(item item, bool refresh_owner)
            {
                if (item == null || item.IsDisposed) return false;

                if (item.powerup)
                {
                    item.abilities.SetLevels(1);
                    //item.abilities.Activated = true;
                    item.abilities.SetActivatedState(AbilityState.Activated);
                    item.abilities.SetOwner(this.owner as unit);
                    item.abilities.Apply();
                    item.abilities.ResetLevels();
                    if (!forceNoMessages && owner != null) (owner as unit).OnPickupItem(item);

                    if (!forceNoRefresh && refresh_owner && Owner != null)
                        Owner.refresh();

                    return true;
                }

                if (this.item == null)
                {
                    this.item = item;
                    this.item.abilities.SetLevels(1);
                    this.item.abilities.SetSlot(this.Index);
                    this.item.set_owner(this.owner as unit);
                    if (!forceNoMessages && owner != null) (owner as unit).OnPickupItem(item);

                    if (!forceNoRefresh && refresh_owner && Owner != null)
                        Owner.refresh();

                    return true;
                }
                else
                    return false;                
            }

            public bool drop_item()
            {
                if (this.item != null)
                {
                    if (!forceNoMessages && owner != null) (owner as unit).OnDropItem(item);
                    item.abilities.ResetLevels();
                    item.set_owner(null);
                    item = null;
                    return true;
                }
                else
                    return false;               
            }
            public virtual bool drop_item(bool refresh_owner)
            {
                if (this.item != null)
                {
                    if (!forceNoMessages && owner != null) (owner as unit).OnDropItem(item);
                    item.abilities.ResetLevels();
                    item.set_owner(null);
                    item = null;//new item(); // replace this item with empty-item                 

                    if (!forceNoRefresh && refresh_owner && Owner != null)
                        Owner.refresh();

                    return true;
                }
                else
                    return false;              
            }

            public virtual bool drop_item(bool refresh_owner, bool noMessages)
            {
                if (this.item != null)
                {
                    if (!forceNoMessages && !noMessages && owner != null) (owner as unit).OnDropItem(item);
                    item.abilities.ResetLevels();
                    item.set_owner(null);
                    item = null;//new item(); // replace this item with empty-item                 

                    if (!forceNoRefresh && refresh_owner && Owner != null)
                        Owner.refresh();

                    return true;
                }
                else
                    return false;
            }

            public bool remove_item(item item)
            {                
                if (this.item != null && this.item.handle == item.handle)
                {
                    if (!forceNoMessages && owner != null) (owner as unit).OnDropItem(item);
                    item.abilities.ResetLevels();
                    item.set_owner(null);
                    this.item = null;//new item(); // replace this item with empty-item
                    return true;
                }
                else
                    return false;              
            }

            public bool remove_item(item item, bool refresh_owner)
            {
                if (this.item != null && this.item.handle == item.handle)
                {
                    if (!forceNoMessages && owner != null) (owner as unit).OnDropItem(item);
                    item.abilities.ResetLevels();
                    item.set_owner(null);
                    this.item = null;//new item(); // replace this item with empty-item

                    if (!forceNoRefresh && refresh_owner && Owner != null)
                        Owner.refresh();

                    return true;
                }
                else
                    return false;
            }

            public item grab_item()
            {
                if (this.item != null)
                {
                    item grabbed_item = this.item;
                    grabbed_item.abilities.ResetLevels();
                    grabbed_item.set_owner(null);

                    this.item = null;
                    return grabbed_item;
                }
                else
                    return null;                
            }
            public virtual item grab_item(bool refresh_owner)
            {
                if (this.item != null)
                {
                    item grabbed_item = this.item;
                    grabbed_item.abilities.ResetLevels();
                    grabbed_item.set_owner(null);

                    this.item = null;

                    if (!forceNoRefresh && refresh_owner && Owner != null)
                        Owner.refresh();

                    return grabbed_item;
                }
                else
                    return null;                
            }          

            public string get_description()
            {
                return "";
            }            

            public virtual DBABILITIES getAbilities()
            {   
                if (this.item == null)
                    return new DBABILITIES();                    
                else
                    return this.item.abilities;    
            }

            public DBIMAGE Image
            {
                get
                {
                    if (this.item != null)
                        return item.iconImage;
                    else
                        return new DBIMAGE();
                }
            }

            public item Item
            {
                get
                {
                    return item;
                }
                set
                {                    
                    this.item = value;
                    if (value != null)
                        this.item.abilities.SetSlot(this.Index);
                }
            }

            public bool IsNull
            {
                get
                {
                    return item == null;
                }
            }           

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBITEMSLOT(Name, Owner, attributes);
            }
        }
        public class DBBACKPACKITEMSLOT : DBITEMSLOT
        {
            public DBBACKPACKITEMSLOT(){}            
            public DBBACKPACKITEMSLOT(item item):base(item){}            

            internal DBBACKPACKITEMSLOT(string Name, IRecord Owner, FieldAttributeCollection attributes):base(Name,Owner,attributes){}

            public override bool put_item(item item, bool refresh_owner)
            {
                return base.put_item(item);
            }
            public override bool drop_item(bool refresh_owner)
            {
                return base.drop_item();
            }
            public override item grab_item(bool refresh_owner)
            {
                return base.grab_item();
            }

            public override DBABILITIES getAbilities()
            {
                return new DBABILITIES();
            }
        }

        public class DBINVENTORY : List<DBITEMSLOT>, IField
        {
            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBINVENTORY()
            {
            }

            public DBINVENTORY(DBITEMSLOT[] items)
            {
                this.AddRange(items);            
            }

            internal DBINVENTORY(string Name, IRecord Owner, FieldAttributeCollection attributes) 
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

            public int Size
            {
                get
                {
                    return this.Count;
                }
            }
            public DbValueType ValueType
            {
                get { return DbValueType.Array; }
            }
            public object Value
            {
                get
                {
                    return this;
                }
                set
                {
                    if (value is string)
                    {
                        int capacity; 
                        int.TryParse(value as string, out capacity);
                        init(capacity);
                    }
                    else
                        if (value is IEnumerable<DBITEMSLOT>)
                        {
                            bool refresh_owner = !IsNull;

                            // note that if 'value' is another inventory,
                            // then both inventories will point to same itemslots

                            this.Clear();
                            this.AddRange(value as IEnumerable<DBITEMSLOT>);

                            if (refresh_owner && Owner != null)
                                Owner.refresh();
                        }
                }
            }
            public object ToValue(object value, out bool IsNull)
            {
                IsNull = true;
                return null;
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
            public string Text
            {
                get
                {
                    return "";
                }
            }
            public object AutoValue
            {
                get
                {
                    return null;
                }
            }
            public bool ValueEquals(object obj)
            {
                return false;
            }
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return (this.Count == 0);
                }
            }
            public List<item> History = null;
            private bool enableHistory = false;
            public bool EnableHistory
            {
                get
                {
                    return enableHistory;
                }
                set
                {
                    enableHistory = value;
                    if (enableHistory) History = new List<item>();
                    else
                        History = null;
                }
            }
            
            public void init(DBITEMSLOT[] items, bool refresh_owner)
            {
                this.Clear();
                this.AddRange(items);                

                if (refresh_owner && Owner != null)
                    Owner.refresh();
            }

            public void init(int capacity)
            {
                this.Clear();
                this.Capacity = capacity;
                for (int i = 0; i < capacity; i++)
                    this.Add(new DBITEMSLOT("item_"+(i+1), this.owner, i));
            }
            public void init(int capacity, int backpack_capacity)
            {
                this.Clear();
                this.Capacity = capacity + backpack_capacity;

                for (int i = 0; i < capacity; i++)
                    this.Add(new DBITEMSLOT("item_" + (i + 1), this.owner, i));

                for (int i = 0; i < backpack_capacity; i++)
                {
                    DBBACKPACKITEMSLOT bpItemSlot = new DBBACKPACKITEMSLOT("item_bp_" + (i + 1), this.owner, null);
                    this.Add(bpItemSlot);
                }
            }

            public bool put_item(item item)
            {
                if (enableHistory) History.Add(item);

                foreach (DBITEMSLOT itemSlot in this)
                    if (itemSlot.put_item(item, true))                                            
                        return true;                    

                return false;
            }
            public bool put_item(item item, int slot)
            {
                if (slot >= 0 && slot < this.Count)
                    return this[slot].put_item(item, true);
                
                return false;
            }            

            public bool remove_item(string codeID)
            {
                foreach (DBITEMSLOT itemSlot in this)
                    if (itemSlot.Item.codeID == codeID)                    
                        return itemSlot.drop_item(true);
                return false;
            }
            public bool remove_item(string codeID, bool refresh_owner)
            {
                foreach (DBITEMSLOT itemSlot in this)
                    if (itemSlot.Item.codeID == codeID)
                        return itemSlot.drop_item(refresh_owner);
                return false;
            }
            public bool remove_item(item item)
            {
                foreach (DBITEMSLOT itemSlot in this)
                    if (itemSlot.remove_item(item, true))
                        return true;
                return false;
            }

            public void swap_items(DBITEMSLOT a, DBITEMSLOT b)
            {
                item tmp_item = a.Item;
                a.Item = b.Item;
                b.Item = tmp_item;

                if (Owner != null)
                    Owner.refresh();
            }
            public void swap_items(DBITEMSLOT a, DBITEMSLOT b, bool refresh_owner)
            {
                item tmp_item = a.Item;
                a.Item = b.Item;
                b.Item = tmp_item;

                if (refresh_owner && Owner != null)
                    Owner.refresh();
            }

            public void DisableRefresh()
            {
                foreach (DBITEMSLOT itemSlot in this)
                    itemSlot.ForceNoRefresh = true;
            }
            public void EnableRefresh()
            {
                foreach (DBITEMSLOT itemSlot in this)
                    itemSlot.ForceNoRefresh = false;
            }

            public void DisableMessages()
            {
                foreach (DBITEMSLOT itemSlot in this)
                    itemSlot.ForceNoMessages = true;
            }
            public void EnableMessages()
            {
                foreach (DBITEMSLOT itemSlot in this)
                    itemSlot.ForceNoMessages = false;
            }
            
            public bool TryFindComponents(DBSTRINGCOLLECTIONS componentsList, out DBSTRINGCOLLECTION components)
            {
                ArrayList inventoryBackUp = new ArrayList();                
                foreach (DBITEMSLOT itemSlot in this)
                    inventoryBackUp.Add(itemSlot.Item.codeID);

                ArrayList inventoryItemCodes = new ArrayList();

                foreach (DBSTRINGCOLLECTION comps in componentsList)
                {
                    bool componentsFound = true;
                    inventoryItemCodes.Clear();
                    inventoryItemCodes.AddRange(inventoryBackUp);

                    // проверяем каждый ID item-а в текущем списке компонент
                    foreach (string codeID in comps)
                    {
                        bool found = inventoryItemCodes.Contains(codeID);                                         

                        // если в инвентории item не был обнаружен,
                        // то прекращаем проверку этого списка компонент
                        if (found == false)
                        {
                            componentsFound = false; // указываем что компоненты не были найдены
                            break;
                        }
                        else // убираем код найденного item-а из списка, чтобы дважды на него не напороться
                            inventoryItemCodes.Remove(codeID);                        
                    }

                    if (componentsFound)
                    {
                        components = comps;
                        return true;
                    }
                }

                components = null;
                return false;
            }
            
            public item itemAt(int index)
            {
                if (index >= 0 && index < this.Count)
                    return this[index].Item;
                else
                    return null;
            }

            public int IndexOf(item item)
            {
                if (item == null) return -1;

                for(int i=0; i<this.Count; i++)
                    if (this[i].Item == item)
                        return i;
                return -1;
            }

            public override string ToString()
            {
                string str = "";

                foreach (DBITEMSLOT itemSlot in this)
                    if (itemSlot.Item != null)
                        str += itemSlot.Item.codeID + ",";
                    else
                        str += ",";
              
                return str.TrimEnd(DBSTRINGCOLLECTION.comma_separator);
            }

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBINVENTORY(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBINVENTORY dbu = new DBINVENTORY(this.Name, Owner, attributes);
                if (setValue && !this.IsNull)
                {
                    dbu.Clear();
                    dbu.AddRange(this);
                    //dbu.items = new DBITEMSLOT[this.items.Length];
                    //this.items.CopyTo(dbu.items, 0);
                    //dbu.isNull = this.isNull;
                }
                return dbu;
            }
        }
        public class DBDAMAGE : DbUnit
        {
            Damage damage;
            public AttackType AttackType;
            public DamageType DamageType;

            public DBDAMAGE()
            {
                damage = new Damage();
            }

            public DBDAMAGE(int damage)
            {
                this.damage = new Damage(damage);                
            }
            public DBDAMAGE(int damage, AttackType at, DamageType dt)
            {
                this.damage = new Damage(damage);
                this.AttackType = at;
                this.DamageType = dt;
            }

            public DBDAMAGE(double damage)
            {
                this.damage = new Damage(damage);
            }
            public DBDAMAGE(double damage, AttackType at, DamageType dt)
            {
                this.damage = new Damage(damage);
                this.AttackType = at;
                this.DamageType = dt;
            }

            public DBDAMAGE(int min, int max)
            {
                this.damage = new Damage(min, max);
            }
            public DBDAMAGE(int min, int max, AttackType at, DamageType dt)
            {
                this.damage = new Damage(min, max);
                this.AttackType = at;
                this.DamageType = dt;
            }

            public DBDAMAGE(Damage damage)
            {
                this.damage = damage;
            }
            public DBDAMAGE(Damage damage, AttackType at, DamageType dt)
            {
                this.damage = damage;
                this.AttackType = at;
                this.DamageType = dt;
            }

            internal DBDAMAGE(string Name, IRecord Owner, FieldAttributeCollection attributes) : base(Name, Owner, attributes) { }
            public override int Size
            {
                get
                {
                    return 0;
                }
            }
            public override DbValueType ValueType
            {
                get { return DbValueType.Number; }
            }
            public override object Value
            {
                get
                {
                    return this.damage;
                }
                set
                {
                    damage = (Damage)ToValue(value, out isNull);                   
                }
            }

            public override object ToValue(object value, out bool IsNull)
            {
                IsNull = false;                

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is Damage)
                    return value;
                else
                    if (value is int)
                        return new Damage((int)value);                                    
                    else
                        if (value is double)                    
                            return new Damage((double)value);                     
                        else
                            if (value is string)
                            {
                                return new Damage(value as string);
                            }
                            else
                            {
                                IsNull = true;
                                return new Damage();
                            }
            }                                 
            public override string ToString()
            {
                return damage.ToString();
            }

            public static implicit operator DBDAMAGE(int value)
            {
                return new DBDAMAGE(value);
            }
            public static implicit operator int(DBDAMAGE value)
            {
                return value.damage;
            }
            public static implicit operator DBDAMAGE(double value)
            {
                return new DBDAMAGE(value);
            }
            public static implicit operator double(DBDAMAGE value)
            {
                return value.damage;
            }
            public static implicit operator DBDAMAGE(Damage value)
            {
                return new DBDAMAGE(value);
            }
            public static implicit operator Damage(DBDAMAGE value)
            {
                return value.damage;
            }
            public static bool operator >(DBDAMAGE a, int b)
            {
                return a.damage > b;
            }
            public static bool operator <(DBDAMAGE a, int b)
            {
                return a.damage < b;
            }
            public static bool operator !=(DBDAMAGE a, int b)
            {
                return a.damage != b;
            }
            public static bool operator ==(DBDAMAGE a, int b)
            {
                return a.damage == b;
            }
            public static DBDAMAGE operator +(DBDAMAGE a, int b)
            {
                return new DBDAMAGE(a.damage + b,a.AttackType,a.DamageType);
            }
            public static DBDAMAGE operator -(DBDAMAGE a, int b)
            {
                return new DBDAMAGE(a.damage - b,a.AttackType, a.DamageType);
            }
            public static DBDAMAGE operator +(DBDAMAGE a, double b)
            {
                return new DBDAMAGE(a.damage + b, a.AttackType, a.DamageType);
            }
            public static DBDAMAGE operator -(DBDAMAGE a, double b)
            {
                return new DBDAMAGE(a.damage - b, a.AttackType, a.DamageType);
            }
            public static DBDAMAGE operator +(DBDAMAGE a, DBDAMAGE b)
            {
                return new DBDAMAGE(a.damage + b.damage, a.AttackType, a.DamageType);
            }
            public static DBDAMAGE operator -(DBDAMAGE a, DBDAMAGE b)
            {
                return new DBDAMAGE(a.damage - b.damage, a.AttackType, a.DamageType);
            }
            public static DBDAMAGE operator *(DBDAMAGE a, double b)
            {
                return new DBDAMAGE(a.damage * b, a.AttackType, a.DamageType);
            }
            public static DBDAMAGE operator /(DBDAMAGE a, double b)
            {
                return new DBDAMAGE(a.damage / b, a.AttackType, a.DamageType);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            protected override DbUnit add(DbUnit b)
            {
                if (b is DBDAMAGE)
                    return (this + (b as DBDAMAGE));
                else
                    return (new DBDAMAGE(this.damage + b.convert_to_double(),this.AttackType, this.DamageType));
            }
            protected override DbUnit subtract(DbUnit b)
            {
                if (b is DBDAMAGE)
                    return (this - (b as DBDAMAGE));
                else
                    return (new DBDAMAGE(this.damage - b.convert_to_double(), this.AttackType, this.DamageType));
            }

            public override double convert_to_double()
            {
                return damage.get_damage_double();
            }

            public override int convert_to_int()
            {
                return damage.get_damage_int();
            }

            public override IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBDAMAGE(Name, Owner, attributes);
            }
        }

        public class DBINTCOLLECTON : List<int>, IField
        {
            public bool AccumulateValues = false;

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBINTCOLLECTON() { }
            public DBINTCOLLECTON(int value)
            {
                this.Add(value);
            }
            public DBINTCOLLECTON(IEnumerable<int> value)
            {
                this.AddRange(value);
            }
            internal DBINTCOLLECTON(string Name, IRecord Owner, FieldAttributeCollection attributes)
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

            public List<int> getCollectionWithout(params int[] indices)
            {
                List<int> sc = new List<int>();
                List<int> indicesList = new List<int>(indices);

                for (int i = 0; i < this.Count; i++)
                    if (indicesList.Contains(i) == false)
                        sc.Add(this[i]);

                return sc;
            }

            public void Insert(int index, int value, bool canAddEmpty)
            {
                if (index > this.Count - 1)
                {
                    if (canAddEmpty)
                    {
                        int numOfNewEntries = index - (this.Count - 1);
                        int[] newEntries = new int[numOfNewEntries];
                        newEntries[numOfNewEntries - 1] = value;
                        this.AddRange(newEntries);
                    }
                    else
                        this.Add(value);
                }
                else
                    this.Insert(index, value);
            }
            public void Insert(int index, int value, bool canAddEmpty, bool canOverwrite)
            {
                if (index > this.Count - 1)
                {
                    if (canAddEmpty)
                    {
                        int numOfNewEntries = index - (this.Count - 1);
                        int[] newEntries = new int[numOfNewEntries];
                        newEntries[numOfNewEntries - 1] = value;
                        this.AddRange(newEntries);
                    }
                    else
                        this.Add(value);
                }
                else
                    if (canOverwrite)
                        this[index] = value;
                    else
                        this.Insert(index, value);
            }

            public DbValueType ValueType
            {
                get { return DbValueType.Array; }
            }

            public object Value
            {
                get
                {
                    return this;
                }
                set
                {
                    bool isNull;
                    if (AccumulateValues)
                    {
                        List<int> scollection = (List<int>)ToValue(value, out isNull);

                        for (int i = 0; i < scollection.Count; i++)
                            this.Add(scollection[i]);
                    }
                    else
                    {
                        this.Clear();
                        this.AddRange((List<int>)ToValue(value, out isNull));
                    }
                }
            }
            public object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is List<int>)
                    return value;
                else
                    if (value is int || value is int[])
                    {
                        List<int> sc = new List<int>();

                        if (value is int)
                            sc.Add((int)value);
                        else
                            sc.AddRange(value as int[]);

                        return sc;
                    }
                    else
                        if (value is string)
                        {
                            string[] elements = (value as string).Split(DBSTRINGCOLLECTION.comma_separator);
                            List<int> list = new List<int>();
                            foreach (string element in elements)
                            {
                                int result;
                                int.TryParse(element, out result);
                                list.Add(result);
                            }
                            return list;
                        }
                        else
                        {
                            IsNull = true;
                            return new List<int>();
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
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return (this.Count == 0);
                }
            }

            public int Size
            {
                get { return this.Count; }
            }

            public static implicit operator DBINTCOLLECTON(int[] value)
            {
                return new DBINTCOLLECTON(value);
            }
            public static implicit operator int[](DBINTCOLLECTON value)
            {
                return (value == null) ? new int[] { } : value.ToArray();
            }

            public string Text
            {
                get
                {
                    return this.ToString();
                }
            }

            public override string ToString()
            {
                string str = "";
                foreach (int s in this)
                    str += s + ",";
                return str.TrimEnd(',');
            }
            public bool ValueEquals(object obj)
            {
                if (obj is int)
                {
                    if (AccumulateValues)
                        return this.Contains((int)obj);
                    else
                        if (this.Count != 0)
                            return (this[0] == ((int)obj));
                        else
                            return false;
                }
                else
                    return DbUnit.ValueEquals(this, obj);
            }

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBINTCOLLECTON(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBINTCOLLECTON dbu = new DBINTCOLLECTON(this.Name, Owner, this.attributes);
                if (setValue)
                    dbu.AddRange(this);
                return dbu;
            }
        };
        public class DBSTRINGCOLLECTION : List<string>, IField
        {
            public static char[] comma_separator = new char[] { ',' };
            public static char[] default_trim_chars = new char[] { '\'',' ' };

            public bool AccumulateValues = false;

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();
            
            public DBSTRINGCOLLECTION() { }
            public DBSTRINGCOLLECTION(string value)
            {
                if (String.IsNullOrEmpty(value)) return;
                this.AddRange(value.Split(comma_separator, StringSplitOptions.RemoveEmptyEntries));
            }
            public DBSTRINGCOLLECTION(string value, char[] separator)
            {
                if (String.IsNullOrEmpty(value)) return;
                this.AddRange(value.Split(comma_separator, StringSplitOptions.RemoveEmptyEntries));
            }
            public DBSTRINGCOLLECTION(string value,params string[] separator)
            {
                if (String.IsNullOrEmpty(value)) return;
                this.AddRange(value.Split(comma_separator, StringSplitOptions.RemoveEmptyEntries));
            }
            public DBSTRINGCOLLECTION(string value, StringSplitOptions options, params string[] separator)
            {
                if (String.IsNullOrEmpty(value)) return;
                this.AddRange(value.Split(comma_separator, StringSplitOptions.RemoveEmptyEntries));
            }
            public DBSTRINGCOLLECTION(string value, char[] separator, char[] trimChars, params string[] suppressValues)
            {
                this.AddRange(value.Split(comma_separator, StringSplitOptions.RemoveEmptyEntries));

                List<string> suppress = new List<string>();
                suppress.AddRange(suppressValues);

                if (trimChars != null)
                    for (int i = 0; i < this.Count; i++)
                    {
                        string trimmed_value = this[i].Trim(trimChars);

                        if (suppress.Contains(trimmed_value))
                        {
                            this.RemoveAt(i);
                            i--;
                        }
                        else
                            this[i] = trimmed_value;
                    }
                else
                    for (int i = 0; i < this.Count; i++)
                        if (suppress.Contains(this[i]))
                        {
                            this.RemoveAt(i);
                            i--;
                        }                                         
            }
            public DBSTRINGCOLLECTION(IEnumerable<string> value)
            {
                this.AddRange(value);
            }           
            internal DBSTRINGCOLLECTION(string Name, IRecord Owner, FieldAttributeCollection attributes)
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

            public new void AddRange(IEnumerable<string> collection)
            {
                foreach (string s in collection)
                    this.Add(s);
            }

            public void Insert(int index, string value, bool canAddEmpty)
            {
                if (index > this.Count - 1)
                {
                    if (canAddEmpty)
                    {
                        int numOfNewEntries = index - (this.Count - 1);
                        string[] newEntries = new string[numOfNewEntries];
                        newEntries[numOfNewEntries - 1] = value;
                        this.AddRange(newEntries);
                    }
                    else
                        this.Add(value);
                }
                else
                    this.Insert(index, value);
            }
            public void Insert(int index, string value, bool canAddEmpty, bool canOverwrite)
            {
                if (index > this.Count - 1)
                {
                    if (canAddEmpty)
                    {
                        int numOfNewEntries = index - (this.Count - 1);
                        string[] newEntries = new string[numOfNewEntries];
                        newEntries[numOfNewEntries - 1] = value;
                        this.AddRange(newEntries);
                    }
                    else
                        this.Add(value);
                }
                else
                    if (canOverwrite)
                        this[index] = value;
                    else
                        this.Insert(index, value);
            }

            public DbValueType ValueType
            {
                get { return DbValueType.Array; }
            }

            public object Value
            {
                get
                {
                    return this;
                }
                set
                {
                    bool success;
                    IList<string> scollection = ToValue(value, out success) as IList<string>;
                    if (success)
                    {
                        if (!AccumulateValues) this.Clear();
                        this.AddRange(scollection);
                    }
                }
            }
            public object ToValue(object value, out bool success)
            {
                success = true;

                if (value is IField)
                {
                    success = ((IField)value).IsNull;
                    if (!success) return null;
                    value = ((IField)value).Value;
                }
                
                if (value is string)                                    
                    return (value as string).Split(comma_separator, StringSplitOptions.RemoveEmptyEntries);                
                else
                    if (value is IList<string>)
                        return value;

                success = false;
                return null;                        
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
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return (this.Count == 0);
                }
            }            

            public int Size
            {
                get { return this.Count; }
            }

            public static implicit operator DBSTRINGCOLLECTION(string[] value)
            {
                return new DBSTRINGCOLLECTION(value);
            }
            public static implicit operator string[](DBSTRINGCOLLECTION value)
            {
                return (value == null) ? new string[] { } : value.ToArray();
            }            

            public string Text
            {
                get
                {
                    return this.ToString();
                }
            }

            public override string ToString()
            {
                string str = "";
                foreach (string s in this)
                    str += s + comma_separator[0];
                return str.TrimEnd(comma_separator);
            }
            public bool ValueEquals(object obj)
            {
                if (!String.IsNullOrEmpty(obj as string))
                {
                    if (AccumulateValues)
                        return this.Contains(obj as string);
                    else
                        if (this.Count != 0)
                            return (this[0] == (obj as string));
                        else
                            return false;
                }
                else
                    return DbUnit.ValueEquals(this, obj);
            }

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBSTRINGCOLLECTION(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBSTRINGCOLLECTION dbu = new DBSTRINGCOLLECTION(this.Name, Owner, this.attributes);
                if (setValue)                                    
                    dbu.AddRange(this);
                return dbu;
            }
        };
               
        public class DBSTRINGCOLLECTIONS : List<DBSTRINGCOLLECTION>, IField
        {
            public bool AccumulateValues = false;

            protected string name;
            protected IRecord owner;
            protected SubstituteAttribute substituteName = null;
            protected FieldAttributeCollection attributes = new FieldAttributeCollection();

            public DBSTRINGCOLLECTIONS() { }
            public DBSTRINGCOLLECTIONS(DBSTRINGCOLLECTION value)
            {
                this.Add(value);
            }
            public DBSTRINGCOLLECTIONS(IEnumerable<DBSTRINGCOLLECTION> value)
            {
                this.AddRange(value);
            }
           
            internal DBSTRINGCOLLECTIONS(string Name, IRecord Owner, FieldAttributeCollection attributes) 
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

            public List<DBSTRINGCOLLECTION> getCollectionWithout(params int[] indices)
            {
                List<DBSTRINGCOLLECTION> sc = new List<DBSTRINGCOLLECTION>();
                List<int> indicesList = new List<int>(indices);

                for (int i = 0; i < this.Count; i++)
                    if (indicesList.Contains(i) == false)
                        sc.Add(this[i]);

                return sc;
            }
            
            public void Insert(int index, DBSTRINGCOLLECTION value, bool canAddEmpty)
            {
                if (index > this.Count - 1)
                {
                    if (canAddEmpty)
                    {
                        int numOfNewEntries = index - (this.Count - 1);
                        DBSTRINGCOLLECTION[] newEntries = new DBSTRINGCOLLECTION[numOfNewEntries];
                        newEntries[numOfNewEntries - 1] = value;
                        this.AddRange(newEntries);
                    }
                    else
                        this.Add(value);
                }
                else
                    this.Insert(index, value);
            }
            public void Insert(int index, DBSTRINGCOLLECTION value, bool canAddEmpty, bool canOverwrite)
            {
                if (index > this.Count - 1)
                {
                    if (canAddEmpty)
                    {
                        int numOfNewEntries = index - (this.Count - 1);
                        DBSTRINGCOLLECTION[] newEntries = new DBSTRINGCOLLECTION[numOfNewEntries];
                        newEntries[numOfNewEntries - 1] = value;
                        this.AddRange(newEntries);
                    }
                    else
                        this.Add(value);
                }
                else
                    if (canOverwrite)
                        this[index] = value;
                    else
                        this.Insert(index, value);
            }

            public DbValueType ValueType
            {
                get { return DbValueType.Array; }
            }

            public object Value
            {
                get
                {
                    return this;
                }
                set
                {
                    bool isNull;
                    if (AccumulateValues)
                    {
                        List<DBSTRINGCOLLECTION> scollection = (List<DBSTRINGCOLLECTION>)ToValue(value, out isNull);

                        for (int i = 0; i < scollection.Count; i++)
                            this.Add(scollection[i]);                       
                    }
                    else
                    {
                        this.Clear();
                        this.AddRange((List<DBSTRINGCOLLECTION>)ToValue(value, out isNull));
                    }
                }
            }
            public object ToValue(object value, out bool IsNull)
            {
                IsNull = false;

                if (value is IField)
                {
                    IsNull = ((IField)value).IsNull;
                    value = ((IField)value).Value;
                }

                if (value is List<DBSTRINGCOLLECTION>)
                    return value;
                else
                    if (value is DBSTRINGCOLLECTION || value is DBSTRINGCOLLECTION[])
                    {
                        List<DBSTRINGCOLLECTION> sc = new List<DBSTRINGCOLLECTION>();

                        if (value is DBSTRINGCOLLECTION)
                            sc.Add((DBSTRINGCOLLECTION)value);
                        else
                            sc.AddRange(value as DBSTRINGCOLLECTION[]);

                        return sc;
                    }
                    else
                        if (value is string)
                        {
                            DBSTRINGCOLLECTION dbs = new DBSTRINGCOLLECTION(value as string);                                                
                            List<DBSTRINGCOLLECTION> dbsl = new List<DBSTRINGCOLLECTION>();
                            dbsl.Add(dbs);
                            return dbsl;
                        }
                        else
                            if (value is IEnumerable<string>)
                            {
                                DBSTRINGCOLLECTION dbs = new DBSTRINGCOLLECTION(value as IEnumerable<string>);
                                List<DBSTRINGCOLLECTION> dbsl = new List<DBSTRINGCOLLECTION>();
                                dbsl.Add(dbs);
                                return dbsl;
                            }
                            else
                            {
                                IsNull = true;
                                return new List<DBSTRINGCOLLECTION>();
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
            public object GetCopy()
            {
                return Value;
            }
            public bool IsNull
            {
                get
                {
                    return (this.Count == 0);
                }
            }
            
            public int Size
            {
                get { return this.Count; }
            }

            public static implicit operator DBSTRINGCOLLECTIONS(DBSTRINGCOLLECTION[] value)
            {
                return new DBSTRINGCOLLECTIONS(value);
            }
            public static implicit operator DBSTRINGCOLLECTION[](DBSTRINGCOLLECTIONS value)
            {
                return (value == null) ? new DBSTRINGCOLLECTION[] { } : value.ToArray();
            }            

            public string Text
            {
                get
                {
                    return this.ToString();
                }
            }

            public override string ToString()
            {
                string str = "";
                foreach (DBSTRINGCOLLECTION s in this)
                    str += s + ";";
                return str.TrimEnd(';');
            }
            public bool ValueEquals(object obj)
            {
                if (obj is DBSTRINGCOLLECTION)
                {
                    if (AccumulateValues)
                        return this.Contains((DBSTRINGCOLLECTION)obj);
                    else
                        if (this.Count != 0)
                            return (this[0] == ((DBSTRINGCOLLECTION)obj));
                        else
                            return false;
                }
                else
                    return DbUnit.ValueEquals(this,obj);
            }

            public IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
            {
                return new DBSTRINGCOLLECTIONS(Name, Owner, attributes);
            }
            public IField NewCopy(IRecord Owner, bool setValue)
            {
                DBSTRINGCOLLECTIONS dbu = new DBSTRINGCOLLECTIONS(this.Name, Owner, this.attributes);
                if (setValue)                             
                    dbu.AddRange(this);

                return dbu;
            }
        };        

        #region pragma disable
#pragma warning restore 660
        #endregion
    }   
}


