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

namespace DotaHIT.DatabaseModel.Data
{    
    public interface IField
    {
        /// <summary>
        /// указывает если данное поле с точки зрения БД должно представлять собой NULL
        /// также используется чтобы узнать было ли присвоено значение этому полю с момента его создания
        /// </summary>
        bool IsNull
        {
            get;
        }

        /// <summary>
        /// значение этого поля
        /// не всегда указывает верную информацию так как
        /// если isNull==true то для наследующего класса представляющего тип данных Int
        /// значение этого свойства будет 0 вместо предполагаемого NULL
        /// это намеренно используется в коде иногда для избежания ошибок связанных с NULL-указателями
        /// </summary>
        object Value
        {
            get;
            set;
        }
        object AutoValue
        {
            get;
        }

        string Text
        {
            get;
        }
        object ToValue(object value, out bool IsNull);

        /// <summary>
        /// указывает на структуру в которой находится это поле (иначе NULL для 'самостоятельного' DbUnit)
        /// </summary>
        IRecord Owner
        {
            get;
        }
        /// <summary>
        /// имя этого поля в содержащей его структуре
        /// передается в конструкторе
        /// </summary>
        string Name
        {
            get;
            set;
        }

        string SubstituteName
        {
            get;
        }
        bool IsSubstituteNameMatch(string value);

        FieldAttributeCollection Attributes
        {
            get;
        }

        /// <summary>
        /// используется для сравнения значения obj со значение этого поля
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool ValueEquals(object obj);

        object GetCopy();
        IField New(string Name, IRecord Owner, FieldAttributeCollection attributes);
        IField NewCopy(IRecord Owner, bool setValue);
    }
    /// <summary>
    /// используется как поля для сруктуры HabRecord.
    /// </summary>
    public abstract class DbUnit : IField
    {
        /// <summary>
        /// указывает если это поле принимает значение NULL
        /// введено для решения тех случаев когда к примеру вместо значения Int в БД установлено NULL
        /// а тип данных Int в коде такое значение может разве что перевести в 0 что исказит верность представления данных
        /// </summary>
        protected bool isNull = false;

        protected IRecord owner;
        protected string name;
        protected SubstituteAttribute substituteName = null;
        protected FieldAttributeCollection attributes = new FieldAttributeCollection();

        public DbUnit() { }
        public DbUnit(object value)
        {
            Value = value;
        }
        public DbUnit(string Name, IRecord Owner, FieldAttributeCollection attributes)
        {
            name = Name;
            this.owner = Owner;
            isNull = true;

            this.attributes = attributes;

            substituteName = this.attributes.Get(typeof(SubstituteAttribute)) as SubstituteAttribute;
            if (substituteName == null)
            {
                substituteName = new SubstituteAttribute(name);
                this.attributes.Add(substituteName);
            }
        }

        /// <summary>
        /// размер в байтах этого поля
        /// используется при добавлении параметров к sql-команде
        /// где необходимо указать размер передаваемых данных
        /// </summary>
        public abstract int Size
        {
            get;
        }

        public abstract DbValueType ValueType
        {
            get;
        }
        /// <summary>
        /// значение этого поля
        /// не всегда указывает верную информацию так как
        /// если isNull==true то для наследующего класса представляющего тип данных Int
        /// значение этого свойства будет 0 вместо предполагаемого NULL
        /// это намеренно используется в коде иногда для избежания ошибок связанных с NULL-указателями
        /// </summary>
        public abstract object Value
        {
            get;
            set;
        }
        /// <summary>
        /// используется в тех случаях когда необходимо получить гарантировано не-NULL значение, то есть ссылку на существующий объект
        /// к примеру в классе DBSTRING свойство AutoValue возвращает "" в тех случаях когда Value возвращает NULL
        /// </summary>
        public virtual object AutoValue
        {
            get
            {
                return Value;
            }
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

        /// <summary>
        /// возвращает текстовое представление Value
        /// </summary>
        public virtual string Text
        {
            get
            {
                if (isNull) return "";
                else return Value.ToString();
            }
        }
        /// <summary>
        /// переводит value в тип данного поля
        /// если конвертация невозмножна, вместо результата подставляется NULL
        /// IsNull указывает если результатом конвертации стал NULL
        /// </summary>
        /// <param name="value"></param>
        /// <param name="IsNull"></param>
        /// <returns></returns>
        public abstract object ToValue(object value, out bool IsNull);
        /// <summary>
        /// указывает если данное поле с точки зрения БД должно представлять собой NULL
        /// также используется чтобы узнать было ли присвоено значение этому полю с момента его создания
        /// </summary>
        public virtual bool IsNull
        {
            get
            {
                return isNull;
            }
        }
        /// <summary>
        /// текстовое представление AutoValue
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }
        /// <summary>
        /// сравнивает значения obj и этого поля
        /// алгоритм сравнения немного отличается от системного
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ValueEquals(obj);
        }
        /// <summary>
        /// используется для сравнения значения obj со значение этого поля
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool ValueEquals(object obj)
        {
            // в системе предполагается что x.equals(null) будет возвращать false
            if (obj == null) return false;

            if (obj is IField)// если сверяемый объект - типа IField
            {
                if (this.isNull == (obj as IField).IsNull)
                {
                    // если оба объекта не обозначают null,
                    if (this.isNull == false)
                        // то сверяем их значения
                        return this.Value.Equals((obj as IField).Value);
                    else
                        // если оба объекта обозначают null, считаем что они равны
                        return true;
                }
                else
                    // если один null, другой нет, то считаем что они не равны
                    return false;
            }
            else // если сверяемый объект - значение
            {
                // если текущий объект обозначает null
                if (this.isNull)
                    // считаем что он равен сверяемому если тот - DBNull
                    return (obj is DBNull);
                else// иначе сверяем this.Value с этим объектом
                    return this.Value.Equals(obj);
            }
        }
        /// <summary>
        /// перегружено просто для того чтобы компиляция давала меньше предупреждений
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// указывает на структуру в которой находится это поле (иначе NULL для 'самостоятельного' DbUnit)
        /// </summary>
        public IRecord Owner
        {
            get
            {
                return owner;
            }
        }
        /// <summary>
        /// имя этого поля в содержащей его структуре
        /// передается в конструкторе
        /// </summary>
        public virtual string Name
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
        public virtual bool IsSubstituteNameMatch(string value)
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

        public virtual object GetCopy()
        {
            return this.Value;
        }
        public virtual IField New(string Name, IRecord Owner, FieldAttributeCollection attributes)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public virtual IField NewCopy(IRecord Owner, bool setValue)
        {
            IField dbuNew = New(this.name, Owner, attributes);
            if (setValue)
                dbuNew.Value = this;
            return dbuNew;
        }

        public static implicit operator string(DbUnit value)
        {
            return value.Text;
        }
        public static DbUnit operator +(DbUnit a, DbUnit b)
        {
            return Add(a, b);
        }
        public static DbUnit operator -(DbUnit a, DbUnit b)
        {
            return Subtract(a, b);
        }
        protected abstract DbUnit add(DbUnit b);
        protected abstract DbUnit subtract(DbUnit b);

        public static DbUnit Add(DbUnit a, DbUnit b)
        {
            if (a.ValueType == b.ValueType)
                return a.add(b);
            else
                return null;
        }
        public static DbUnit Subtract(DbUnit a, DbUnit b)
        {
            if (a.ValueType == b.ValueType)
                return a.subtract(b);
            else
                return null;
        }

        public static bool ValueEquals(IField field, object obj)
        {
            // в системе предполагается что x.equals(null) будет возвращать false
            if (obj == null) return false;

            if (obj is IField)// если сверяемый объект - типа IField
            {
                if (field.IsNull == (obj as IField).IsNull)
                {
                    // если оба объекта не обозначают null,
                    if (field.IsNull == false)
                        // то сверяем их значения
                        return field.Value.Equals((obj as IField).Value);
                    else
                        // если оба объекта обозначают null, считаем что они равны
                        return true;
                }
                else
                    // если один null, другой нет, то считаем что они не равны
                    return false;
            }
            else // если сверяемый объект - значение
            {
                // если текущий объект обозначает null
                if (field.IsNull)
                    // считаем что он равен сверяемому если тот - DBNull
                    return (obj is DBNull);
                else// иначе сверяем this.Value с этим объектом
                    return field.Value.Equals(obj);
            }
        }
    }
    /// <summary>
    /// коллекция IField
    /// содержит некоторые удобные методы для фильтровки коллекции
    /// </summary>
    public class FieldCollection : List<IField>
    {
        public Dictionary<string, string> SubstCache;
        protected Dictionary<string, IField> dc = new Dictionary<string, IField>();

        public FieldCollection() { SubstCache = new Dictionary<string, string>(); }
        public FieldCollection(Dictionary<string, string> SubstCache) { this.SubstCache = SubstCache; }

        public new void Add(IField dbu)
        {
            dc.Add(dbu.Name, dbu);
            base.Add(dbu);
        }
        public void AddEx(IField dbu)
        {
            IField thisDbu;
            if (dc.TryGetValue(dbu.Name, out thisDbu) == false)
                this.Add(dbu);
        }
        new public IField this[int index]
        {
            get
            {
                return base[index] as IField;
            }
        }
        public bool TryGetByName(string name, out IField dbu)
        {
            return dc.TryGetValue(name, out dbu);
        }
        public bool TryGetBySubstituteName(string substName, out IField dbu)
        {
            string name;
            if (SubstCache.TryGetValue(substName, out name))
            {
                if (name == null)
                {
                    dbu = null;
                    return false;
                }
                else
                {
                    dbu = dc[name];
                    return true;
                }
            }
            else
            {
                foreach (IField sdu in this)
                    if (sdu.IsSubstituteNameMatch(substName))
                    {
                        dbu = sdu;
                        SubstCache.Add(substName, dbu.Name);
                        return true;
                    }

                dbu = null;
                SubstCache.Add(substName, null);
                return false;
            }
        }
        public FieldCollection GetRangeByName(string name, bool useSubstituteName)
        {
            FieldCollection dbuc = new FieldCollection();

            if (useSubstituteName)
            {
                foreach (IField sdu in this)
                    if (sdu.IsSubstituteNameMatch(name))
                        dbuc.Add(sdu);
            }
            else
                foreach (IField sdu in this)
                    if (sdu.Name == name)
                        dbuc.Add(sdu);

            return dbuc;
        }
        public bool Contains(string name)
        {
            return dc.ContainsKey(name);
        }

        public void InitValues(FieldCollection Units)
        {
            foreach (IField dbu in this)
                foreach (IField init_dbu in Units)
                    if (dbu.Name == init_dbu.Name)
                        dbu.Value = init_dbu.Value;
        }

        public new void Sort()
        {
            base.Sort(new FieldComparer());
        }
    }
    public class FieldComparer : IComparer<IField>
    {
        private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

        public FieldComparer()
        {
        }
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<IField>.Compare(IField x, IField y)
        {
            return cic.Compare(x.Name, y.Name);
        }
    }    
}


