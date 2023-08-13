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
using DotaHIT.DatabaseModel.DataTypes;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace DotaHIT
{
    using Jass.Native.Types;

    namespace Core
    {
        using Resources;
        using System.Security.AccessControl;
        using DotaHIT.DatabaseModel.Format;
        using MpqReader;

        public class HabProperty
        {
            public static char HeaderSeparator = ',';
            public static string GetNameWithHeader(string name, string header)
            {
                return header + HeaderSeparator + name;
            }
            public static KeyValuePair<string, object> FromString(string lpText)
            {
                unsafe
                {
                    fixed (char* pStr = lpText)
                    {
                        char* ptr = pStr;

                        while (*ptr != '=' && *ptr != 0) ptr++;

                        if (*ptr == 0)
                            return new KeyValuePair<string, object>(lpText, null);
                        else
                            return new KeyValuePair<string, object>(
                                new string(pStr, 0, (int)(ptr - pStr)),
                                new string(++ptr));
                    }
                }
            }
            unsafe public static void FromStringFastUnsafe(ref sbyte* ptr, sbyte* end, out string name, out string value)
            {
                sbyte* pStart = ptr;

                while (*ptr != '=' && *ptr != '\r') ptr++;

                if (*ptr == '\r')
                {
                    name = new string(pStart, 0, (int)(ptr - pStart));
                    value = null;

                    Console.WriteLine("Unexpected property name: " + name);
                }
                else
                {
                    name = new string(pStart, 0, (int)(ptr - pStart));

                    pStart = ++ptr;
                    while (*ptr != '\r' && ptr < end) ptr++;

                    value = new string(pStart, 0, (int)(ptr - pStart), Encoding.UTF8);
                }

                ptr += 2; // go beyond \r\n
            }
            public static KeyValuePair<string, object> Create(string name, object value)
            {
                return new KeyValuePair<string, object>(name, value);
            }
            public static List<string> SplitValue(string value)
            {
                List<string> result = new List<string>();
                unsafe
                {
                    char* pStr;
                    char* pEnd;

                    fixed (char* pText = value)
                    {
                        char* ptr = pText;

                        while (*ptr != 0)
                        {
                            if (*ptr == '"') // if this string begins with quote
                            {
                                pStr = ++ptr;
                                while (*ptr != '"') ptr++;
                                pEnd = ptr++;
                                while (*ptr != 0 && *(ptr++) != ',') ;
                            }
                            else // no quotes in this string
                            {
                                pStr = ptr;
                                while (*ptr != ',' && *ptr != 0) ptr++;
                                pEnd = ptr;
                                if (*ptr != 0) ptr++;
                            }
                            result.Add(new string(pStr, 0, (int)(pEnd - pStr)));
                            //result.Add(new string((sbyte*)pStr, 0, (int)(pEnd - pStr), Encoding.UTF8));
                        }
                    }
                }
                return result;
            }
            public static List<string> SplitValue(string value, int offset)
            {
                List<string> result = new List<string>();
                unsafe
                {
                    char* pStr;
                    char* pEnd;

                    fixed (char* pText = value)
                    {
                        char* ptr = pText;

                        while (*ptr != 0)
                        {
                            if (*ptr == '"') // if this string begins with quote
                            {
                                pStr = ++ptr;
                                while (*ptr != '"') ptr++;
                                pEnd = ptr++;
                                while (*ptr != 0 && *(ptr++) != ',') ;
                            }
                            else // no quotes in this string
                            {
                                pStr = ptr;
                                while (*ptr != ',' && *ptr != 0) ptr++;
                                pEnd = ptr;
                                if (*ptr != 0) ptr++;
                            }
                            result.Add(new string((sbyte*)pStr, 0, (int)(pEnd - pStr), Encoding.UTF8));
                        }
                    }
                }

                while (offset-- > 0) result.Insert(0, null);
                return result;
            }
            public static void SetValue(List<string> list, int index, string value)
            {
                SetValue(list, index, value, string.Empty);
            }
            public static void SetValue(List<string> list, int index, string value, string paddingValue)
            {
                if (index >= list.Count)
                {
                    int numOfNewEntries = index - list.Count;

                    while (numOfNewEntries-- > 0) list.Add(paddingValue);

                    list.Add(value);
                }
                else
                    list[index] = value;
            }
        }

        public class HabProperties : Dictionary<string, object>
        {
            public string name = null;
            public object priority = null;

            public HabProperties() : base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase) { }
            public HabProperties(string name, params KeyValuePair<string, object>[] kvpArray)
                : base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
            {
                this.name = name;
                this.AddRange(kvpArray);
            }
            public HabProperties(IDictionary<string, object> dictionary)
                : base(dictionary, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
            {
            }

            public HabProperties(int capacity)
                : base(capacity, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
            {
            }

            public void Add(KeyValuePair<string, object> kvp)
            {
                base.Add(kvp.Key, kvp.Value);
            }
            /// <summary>
            /// adds only keys that is not contained in this collection
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>                                            
            public void AddEx(string name, object value)
            {
                if (!base.ContainsKey(name))
                    base.Add(name, value);
            }
            public void AddRange(ICollection<KeyValuePair<string, object>> kvps)
            {
                foreach (KeyValuePair<string, object> kvp in kvps)
                    base.Add(kvp.Key, kvp.Value);
            }
            /// <summary>
            /// adds keys from 'hps' not contained in this collection when 'update' is set to false.
            /// adds keys from 'hps' not contained in this collection and updates existing keys with the values from 'hps' when 'update' is set to true.            
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            /// <param name="update"></param>
            public void AddRangeEx(HabProperties hps, bool update)
            {
                if (update)
                    foreach (KeyValuePair<string, object> hp in hps)
                        this[hp.Key] = hp.Value;
                else
                    foreach (KeyValuePair<string, object> hp in hps)
                        this.AddEx(hp.Key, hp.Value);
            }
            public void SetRange(HabProperties hps)
            {
                foreach (KeyValuePair<string, object> kvp in hps)
                    base[kvp.Key] = kvp.Value;
            }
            public void Merge(HabProperties hps)
            {
                if (hps.priority != null)
                    this.priority = hps.priority;

                this.SetRange(hps);
            }
            public void Merge(HabProperties hps, bool update)
            {
                if (hps.priority != null)
                    this.priority = hps.priority;

                this.AddRangeEx(hps, update);
            }
            public object GetValue(string Name)
            {
                object value;
                base.TryGetValue(Name, out value);
                return value;
            }
            public object GetValue(string Name, bool strictMatch)
            {
                if (strictMatch)
                {
                    object value;
                    base.TryGetValue(Name, out value);
                    return value;
                }
                else
                    foreach (KeyValuePair<string, object> kvp in this)
                        if (kvp.Key.Contains(Name))
                            return kvp.Value;
                return null;
            }
            public HabProperties AddHeader(string value)
            {
                HabProperties hps = new HabProperties();
                hps.name = this.name;
                foreach (KeyValuePair<string, object> kvp in this)
                    hps.Add(HabProperty.GetNameWithHeader(kvp.Key, value), kvp.Value);
                return hps;
            }
            public HabProperties GetRangeByPattern(string name_pattern, bool caseSensitive)
            {
                Regex r = DHRC.GetRegex(name_pattern, caseSensitive, false);

                HabProperties hps = new HabProperties();

                foreach (KeyValuePair<string, object> kvp in this)
                    if (r.IsMatch(kvp.Key))
                        hps.Add(kvp.Key, kvp.Value);

                return hps;
            }
            public HabProperties GetCopy()
            {
                HabProperties copy = new HabProperties(this);

                copy.name = this.name;
                copy.priority = this.priority;

                return copy;
            }
            /// <summary>
            /// провер€ет также наличие в значении ссылки на HabPropertiesCollection или List
            /// и если находит, делает копию этой коллекции
            /// </summary>
            /// <returns></returns>
            public HabProperties GetCopyEx()
            {
                HabProperties copy = new HabProperties(this.Count);

                foreach (KeyValuePair<string, object> kvp in this)
                {
                    object value = kvp.Value;

                    if (value is List<string>)
                        value = new List<string>(value as List<string>);
                    else
                        if (value is HabPropertiesCollection)
                        value = (value as HabPropertiesCollection).GetCopy();

                    copy.Add(kvp.Key, value);
                }

                copy.name = this.name;
                copy.priority = this.priority;

                return copy;
            }
            public void SetValueToFirstMatchedKey(string name, object value)
            {
                foreach (KeyValuePair<string, object> kvp in this)
                    if (kvp.Key.Contains(name))
                    {
                        this[kvp.Key] = value;
                        break;
                    }
            }

            public string GetStringValue(string key)
            {
                object obj;
                this.TryGetValue(key, out obj);

                if (obj is List<string>) return (obj as List<string>)[0];
                else return obj + "";
            }
            public string GetStringValue(string key, string retOnFail)
            {
                object obj;
                if (!this.TryGetValue(key, out obj)) return retOnFail;

                if (obj is List<string>) return (obj as List<string>)[0];
                else return obj + "";
            }
            public List<string> GetStringListValue(string key)
            {
                object obj;
                this.TryGetValue(key, out obj);

                if (obj is List<string>) return obj as List<string>;
                else return HabProperty.SplitValue(obj + "");
            }

            public List<string> GetStringListValue(string key, bool keepReference)
            {
                object obj;
                this.TryGetValue(key, out obj);

                if ((obj is List<string>) == false)
                {
                    obj = HabProperty.SplitValue(obj + "");
                    if (keepReference) this[key] = obj;
                }

                return obj as List<string>;
            }

            public int GetIntValue(string key)
            {
                object obj;
                if (this.TryGetValue(key, out obj))
                {
                    try { return Convert.ToInt32(obj); } catch { }
                }
                return 0;
            }
            public int GetIntValue(string key, int retOnFail)
            {
                object obj;
                if (this.TryGetValue(key, out obj))
                {
                    try { return Convert.ToInt32(obj); }
                    catch { }
                }
                return retOnFail;
            }

            public long GetLongValue(string key)
            {
                object obj;
                if (this.TryGetValue(key, out obj))
                {
                    try { return Convert.ToInt64(obj); }
                    catch { }
                }
                return 0;
            }
            public long GetLongValue(string key, long retOnFail)
            {
                object obj;
                if (this.TryGetValue(key, out obj))
                {
                    try { return Convert.ToInt64(obj); }
                    catch { }
                }
                return retOnFail;
            }

            public double GetDoubleValue(string key)
            {
                object obj;
                if (this.TryGetValue(key, out obj))
                {
                    try { return Convert.ToDouble(obj, DBDOUBLE.provider); }
                    catch { }
                }
                return 0.0;
            }
            public double GetDoubleValue(string key, double retOnFail)
            {
                object obj;
                if (this.TryGetValue(key, out obj))
                {
                    try { return Convert.ToDouble(obj, DBDOUBLE.provider); }
                    catch { }
                }
                return retOnFail;
            }

            public HabPropertiesCollection GetHpcValue(string key)
            {
                return GetHpcValue(key, false);
            }
            public HabPropertiesCollection GetHpcValue(string key, bool keepReference)
            {
                object obj;
                this.TryGetValue(key, out obj);

                if (obj is HabPropertiesCollection) return (obj as HabPropertiesCollection);

                if (obj is string)
                    obj = HabPropertiesCollection.ParseFrom(obj + "");
                else
                    obj = new HabPropertiesCollection();

                if (keepReference) this[key] = obj;

                return obj as HabPropertiesCollection;
            }

            public static HabProperties ParseFrom(string hpsString)
            {
                HabProperties hps = new HabProperties();

                unsafe
                {
                    fixed (char* str = hpsString)
                    {
                        char* ptr = str;
                        char* start = null;
                        string name;
                        string value;

                        while (*ptr != 0)
                        {
                            while (*ptr != '[') ptr++;
                            start = ++ptr;
                            while (*ptr != ']') ptr++;
                            name = new string(start, 0, (int)(ptr - start));

                            ptr++;
                            while (*ptr != '=') ptr++;
                            start = ++ptr;
                            while (*ptr != 0 && *ptr != '[') ptr++;

                            value = new string(start, 0, (int)(ptr - start));

                            if (name == "hpsName") hps.name = value;
                            else hps.Add(name, value.Trim('\''));
                        }
                    }
                }

                return hps;
            }

            public override string ToString()
            {
                string output = "";

                foreach (KeyValuePair<string, object> kvp in this)
                    output += "[" + kvp.Key + "]=" + "'" + kvp.Value + "'";

                return output;
            }
        }
        public class HabPropertiesCollection : Dictionary<string, HabProperties>, IEnumerable<HabProperties>
        {
            public HabPropertiesCollection() : base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase) { }
            public HabPropertiesCollection(int capacity) : base(capacity, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase) { }
            public HabPropertiesCollection(MemoryStream ms)
                : base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
            {
                LoadFromMemoryFast(ms, this);
            }

            public new void Add(string name, HabProperties hps)
            {
                hps.name = name;
                string hpsKeyName = GetNewKeyName(hps.name);
                base.Add(hpsKeyName, hps);
            }
            public void AddUnchecked(string name, HabProperties hps)
            {
                hps.name = name;
                base[name] = hps;
            }
            public void Add(HabProperties hps)
            {
                string hpsKeyName = GetNewKeyName(hps.name);
                base.Add(hpsKeyName, hps);
            }
            public void AddUnchecked(HabProperties hps)
            {
                base.Add(hps.name, hps);
            }
            public void Add(KeyValuePair<string, HabProperties> kvp)
            {
                base.Add(kvp.Key, kvp.Value);
            }
            public void Add(HabProperties hps, bool update)
            {
                string hpsKeyName = hps.name;

                if (update)
                {
                    HabProperties thisHps;
                    if (base.TryGetValue(hpsKeyName, out thisHps))
                        thisHps.AddRange(hps);
                    else
                        base.Add(hpsKeyName, hps);
                }
                else
                {
                    hpsKeyName = GetNewKeyName(hpsKeyName);
                    base.Add(hpsKeyName, hps);
                }
            }
            /// <summary>
            /// adds only those HabProperties that is not contained in this collection
            /// </summary>
            /// <param name="hps"></param>
            public void AddEx(HabProperties hps)
            {
                string hpsKeyName = hps.name;

                if (!base.ContainsKey(hpsKeyName))
                    base.Add(hpsKeyName, hps);
            }
            public void AddRange(HabPropertiesCollection hpc)
            {
                foreach (KeyValuePair<string, HabProperties> kvp in (hpc as Dictionary<string, HabProperties>))
                    base.Add(kvp.Key, kvp.Value);
            }

            public string GetNewKeyName(string name)
            {
                if (String.IsNullOrEmpty(name))
                    return Guid.NewGuid().ToString();
                else
                {
                    if (base.ContainsKey(name))
                        return name + Guid.NewGuid().ToString();
                    else
                        return name;
                }
            }

            /// <summary>
            /// копирует все данные из hpc в эту коллекцию. ≈сли найдены одноименные HabProperties, 
            /// то данные копируютс€ в уже существующую HabProperties
            /// </summary>
            /// <param name="hpc"></param>
            public void Merge(HabPropertiesCollection hpc)
            {
                HabProperties thisHps;

                foreach (KeyValuePair<string, HabProperties> kvp in (hpc as Dictionary<string, HabProperties>))
                    if (base.TryGetValue(kvp.Key, out thisHps))
                        thisHps.Merge(kvp.Value);
                    else
                        base.Add(kvp.Key, kvp.Value);
            }

            public void Merge(HabPropertiesCollection hpc, bool onlyExisting)
            {
                HabProperties thisHps;

                foreach (KeyValuePair<string, HabProperties> kvp in (hpc as Dictionary<string, HabProperties>))
                    if (base.TryGetValue(kvp.Key, out thisHps))
                        thisHps.Merge(kvp.Value);
                    else
                        if (onlyExisting == false)
                        base.Add(kvp.Key, kvp.Value);
            }

            public void Merge(HabPropertiesCollection hpc, bool onlyExisting, bool update)
            {
                HabProperties thisHps;

                foreach (KeyValuePair<string, HabProperties> kvp in (hpc as Dictionary<string, HabProperties>))
                    if (base.TryGetValue(kvp.Key, out thisHps))//dc.TryGetValue(hps.name, out thisHps))
                        thisHps.Merge(kvp.Value, update);
                    else
                        if (onlyExisting == false)
                        base.Add(kvp.Key, kvp.Value);
            }

            public HabProperties GetValue(string Name)
            {
                HabProperties hps;
                if (base.TryGetValue(Name, out hps))
                    return hps;
                else
                    return null;
            }
            public HabProperties GetValue(string Name, HabProperties retOnFail)
            {
                HabProperties hps;
                if (base.TryGetValue(Name, out hps))
                    return hps;
                else
                    return retOnFail;
            }
            public string GetStringValue(string hpsname, string pname)
            {
                HabProperties hps;
                if (base.TryGetValue(hpsname, out hps))
                    return hps.GetStringValue(pname);
                else
                    return hpsname;
            }

            public string GetStringValue(string hpsname, string pname, string retOnFail)
            {
                HabProperties hps;
                if (base.TryGetValue(hpsname, out hps))
                    return hps.GetStringValue(pname, retOnFail);
                else
                    return retOnFail;
            }

            public List<string> GetStringListValue(string hpsname, string pname)
            {
                HabProperties hps;
                if (base.TryGetValue(hpsname, out hps))
                {
                    return hps.GetStringListValue(pname);
                }

                return new List<string>();
            }
            public string GetStringListItemValue(string hpsname, string pname, int itemIndex)
            {
                HabProperties hps;
                if (base.TryGetValue(hpsname, out hps))
                {
                    List<string> list = hps.GetStringListValue(pname);
                    return (itemIndex < list.Count) ? list[itemIndex] : "";
                }
                else
                    return "";
            }

            public void SetStringListItemValue(string hpsname, string pname, int itemIndex, string value)
            {
                HabProperties hps;
                if (!base.TryGetValue(hpsname, out hps))
                {
                    hps = new HabProperties(hpsname);
                    base.Add(hpsname, hps);
                }

                List<string> list = hps.GetStringListValue(pname, true);

                HabProperty.SetValue(list, itemIndex, value);
            }

            public int GetIntValue(string hpsname, string pname, int retOnFail)
            {
                HabProperties hps;
                if (base.TryGetValue(hpsname, out hps))
                    return hps.GetIntValue(pname, retOnFail);
                else
                    return retOnFail;
            }

            public new HabProperties this[string name]
            {
                get
                {
                    HabProperties hps;
                    if (base.TryGetValue(name, out hps))
                        return hps;
                    else
                        return null;
                }
                set
                {
                    base[name] = value;
                }
            }
            public object this[string hpsname, string pname]
            {
                get
                {
                    HabProperties hps;
                    if (base.TryGetValue(hpsname, out hps))
                        return hps.GetValue(pname);
                    else
                        return null;
                }
                set
                {
                    HabProperties hps;
                    if (base.TryGetValue(hpsname, out hps))
                        hps[pname] = value;
                    else
                    {
                        hps = new HabProperties(1);
                        hps.name = hpsname;
                        hps[pname] = value;
                        base[hpsname] = hps;
                    }
                }
            }

            public HabProperties GetByOrder(int index)
            {
                foreach (HabProperties hps in this)
                    if (--index < 0) return hps;
                return null;
            }

            public void ReadFromFile(string filename)
            {
                this.Clear();

                if (!File.Exists(filename)) return;

                FileStream fs = new FileStream(filename, FileMode.Open);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);

                MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length, false, true);
                LoadFromMemoryFast(ms, this);

                ms.Close();

                fs.Close();
            }
            public void SaveToFile(string filename)
            {
                SaveToStream(new FileStream(filename, FileMode.Create));
            }
            public void SaveToStream(Stream s)
            {
                using (StreamWriter sw = new StreamWriter(s))
                {
                    foreach (HabProperties hps in this)
                    {
                        sw.WriteLine("[" + hps.name + "]");
                        foreach (KeyValuePair<string, object> kvp in hps)
                            sw.WriteLine(kvp.Key + "=" + DHFormatter.ToStringList(kvp.Value));//kvp.Value.ToString());                        
                    }
                }
            }

            public void SaveToFile(string filename, bool keepParams, params string[] propNames)
            {
                SaveToStream(new FileStream(filename, FileMode.Create), keepParams, propNames);
            }
            public void SaveToFile(string filename, string requiredPropName, bool keepParams, params string[] propNames)
            {
                SaveToStream(new FileStream(filename, FileMode.Create), requiredPropName, keepParams, propNames);
            }
            public void SaveToStream(Stream s, bool keepParams, params string[] propNames)
            {
                SaveToStream(s, null, keepParams, propNames);
            }
            public void SaveToStream(Stream s, string requiredPropName, bool keepParams, params string[] propNames)
            {
                SaveToStream(s, requiredPropName, null, keepParams, propNames);
            }
            public void SaveToStream(Stream s, string requiredPropName, string[] skipPropValues, bool keepParams, params string[] propNames)
            {
                List<string> skipPropValuesList = skipPropValues != null ? new List<string>(skipPropValues) : new List<string>();

                Dictionary<string, int> dcProps = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
                foreach (string propName in propNames)
                    dcProps.Add(propName, 0);

                using (StreamWriter sw = new StreamWriter(s))
                {
                    object propValue;
                    foreach (HabProperties hps in this)
                    {
                        if (requiredPropName != null && (hps.TryGetValue(requiredPropName, out propValue) == false || propValue as string == string.Empty || skipPropValuesList.Contains(propValue as string)))
                            continue;

                        sw.WriteLine("[" + hps.name + "]");

                        foreach (KeyValuePair<string, object> kvp in hps)
                            if (dcProps.ContainsKey(kvp.Key) != keepParams)
                                continue;
                            else
                                if (skipPropValuesList.Contains(kvp.Value as string))
                                continue;
                            else
                                sw.WriteLine(kvp.Key + "=" + DHFormatter.ToStringList(kvp.Value));//kvp.Value.ToString());                                 
                    }
                }
            }

            public static HabPropertiesCollection ParseFrom(string hpcString)
            {
                HabPropertiesCollection hpc = new HabPropertiesCollection();

                string[] hpcStrings = hpcString.Split(';');

                foreach (string s in hpcStrings)
                    hpc.Add(HabProperties.ParseFrom(s));

                return hpc;
            }
            public static void LoadFromMemoryFast(MemoryStream ms, HabPropertiesCollection hpc)
            {
                unsafe
                {
                    byte[] buffer = ms.GetBuffer();
                    if (buffer.Length == 0) return;

                    fixed (byte* buffPtr = buffer)
                    {
                        // skip UTF-8 header, if found
                        int offset = 0;
                        if ((*(UInt32*)buffPtr << 8) == 0xBFBBEF00)
                            offset = 3;

                        sbyte* ptr = (sbyte*)buffPtr + offset;
                        sbyte* pEnd = ptr + buffer.Length - offset;

                        string name;
                        string value;
                        HabProperties hps = null;

                        while (ptr < pEnd)
                        {
                            switch ((char)*ptr)
                            {
                                case '/':
                                    ++ptr;
                                    while (*(ptr++) != '\n') ; // go beyond \n
                                    break;

                                case '\r':
                                    ++ptr;
                                    while (*(ptr++) != '\n') ; // go beyond \n
                                    break;

                                case '[':
                                    name = checkNewHPC(ref ptr);

                                    while (*(ptr++) != '\n') ; // go beyond \n

                                    if (!hpc.TryGetValue(name, out hps))
                                    {
                                        hps = new HabProperties();
                                        hpc.AddUnchecked(name, hps);
                                    }
                                    break;

                                case '\n':
                                    while (*(++ptr) == '\n') ; // go beyond \n
                                    break;

                                default:
                                    HabProperty.FromStringFastUnsafe(ref ptr, pEnd, out name, out value);
                                    hps[name] = value;
                                    break;
                            }
                        }
                    }
                }
            }

            unsafe private static string checkNewHPC(ref char* ptr)
            {
                char* pStart = ++ptr;
                while (*ptr != ']') ptr++;
                return new string(pStart, 0, (int)(ptr++ - pStart));
            }
            unsafe private static string checkNewHPC(ref sbyte* ptr)
            {
                sbyte* pStart = ++ptr;
                while (*ptr != ']') ptr++;
                return new string(pStart, 0, (int)(ptr++ - pStart));
            }

            public override string ToString()
            {
                string output = "";

                foreach (HabProperties hps in this.Values)
                    output += hps.ToString() + ";";

                return output.TrimEnd(new char[] { ';' });
            }
            public string ToString(bool named)
            {
                string output = "";

                foreach (HabProperties hps in this.Values)
                {
                    if (named) output += "[hpsName]=" + hps.name;
                    output += hps.ToString() + ";";
                }

                return output.TrimEnd(new char[] { ';' });
            }

            public HabPropertiesCollection GetCopy()
            {
                HabPropertiesCollection hpc = new HabPropertiesCollection(this.Count);
                foreach (HabProperties hps in this)
                    hpc.Add(hps.GetCopy());
                return hpc;
            }

            /// <summary>
            /// отличаетс€ от GetCopy тем что вызывает GetCopyEx у элементов
            /// </summary>
            /// <returns></returns>
            public HabPropertiesCollection GetCopyEx()
            {
                HabPropertiesCollection hpc = new HabPropertiesCollection(this.Count);
                foreach (HabProperties hps in this)
                    hpc.Add(hps.GetCopyEx());
                return hpc;
            }

            public new IEnumerator<HabProperties> GetEnumerator()
            {
                return base.Values.GetEnumerator();
            }

            public void RenameUnchecked(string oldName, string newName)
            {
                HabProperties hps;
                if (base.TryGetValue(oldName, out hps))
                {
                    this.Remove(oldName);
                    this.AddUnchecked(newName, hps);
                }
            }
            public void MakeCopyUnchecked(string orgKey, string copyKey)
            {
                HabProperties hps;
                if (base.TryGetValue(orgKey, out hps))
                    this.AddUnchecked(copyKey, hps.GetCopy());
            }
            public void MakeCopyUncheckedEx(string orgKey, string copyKey)
            {
                HabProperties hps;
                if (base.TryGetValue(orgKey, out hps))
                    this.AddUnchecked(copyKey, hps.GetCopyEx());
            }
        }

        public class HabPropertiesComparer : IComparer
        {
            private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                HabProperties hpsX = x as HabProperties;
                HabProperties hpsY = y as HabProperties;

                return cic.Compare(hpsX.priority, hpsY.priority);
            }
        }

        public class HabPropertiesNameComparer : IComparer
        {
            private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                HabProperties hpsX = x as HabProperties;
                HabProperties hpsY = y as HabProperties;

                return cic.Compare(hpsX.name, hpsY.name);
            }
        }

        public class HabPropertiesExComparer : IComparer
        {
            private KeyValuePair<string, Type>[] NameTypePairs = { };
            private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

            public HabPropertiesExComparer(params KeyValuePair<string, Type>[] NameTypePairs)
            {
                this.NameTypePairs = NameTypePairs;
            }
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare(Object x, Object y)
            {
                HabProperties hps1 = x as HabProperties;
                HabProperties hps2 = y as HabProperties;

                return Compare(hps1, hps2, NameTypePairs, 0);
            }
            public int Compare(HabProperties hps1, HabProperties hps2)
            {
                return Compare(hps1, hps2, NameTypePairs, 0);
            }
            private int Compare(HabProperties hps1, HabProperties hps2, KeyValuePair<string, Type>[] NameTypePairs, int index)
            {
                if (index >= NameTypePairs.Length) return 0;
                else
                {
                    string propName = NameTypePairs[index].Key;
                    Type propType = NameTypePairs[index].Value;

                    object prop1Value = hps1.GetValue(propName);
                    object prop2Value = hps2.GetValue(propName);

                    try
                    {
                        prop1Value = Convert.ChangeType(prop1Value, propType);
                        prop2Value = Convert.ChangeType(prop2Value, propType);
                    }
                    catch { }

                    int result = cic.Compare(prop1Value, prop2Value);

                    if (result == 0)
                        return Compare(hps1, hps2, NameTypePairs, index + 1);
                    else
                        return result;
                }
            }
        }

        public enum FileFormat
        {
            Txt = 0,
            W3U,
            W3T,
            W3A,
            PWXL,
            J_SCRIPT,
            Unknown
        }

        public class DHMpqArchive : IComparable<DHMpqArchive>
        {
            MpqArchive hMPQ = null;
            string name = null;
            uint priority = 0;

            public DHMpqArchive(string mpqname)
            {
                try { hMPQ = new MpqArchive(mpqname); }
                catch (Exception e) { Console.WriteLine("Error opening '" + mpqname + "'\nException:" + e.Message + "\n" + e.StackTrace); }
                name = mpqname;
            }
            public DHMpqArchive(string mpqname, uint priority)
            {
                try { hMPQ = new MpqArchive(mpqname); }
                catch (Exception e) { Console.WriteLine("Error opening '" + mpqname + "'\nException:" + e.Message + "\n" + e.StackTrace); }
                name = mpqname;
                this.priority = priority;
            }

            public string Name
            {
                get { return name; }
            }

            public uint Priority
            {
                get { return priority; }
            }

            public static uint PriorityMax
            {
                get
                {
                    return 11;
                }
            }

            public void Close()
            {
                if (hMPQ != null)
                {
                    hMPQ.Dispose();
                    hMPQ = null;
                }
            }

            public bool IsNull
            {
                get
                {
                    return hMPQ == null || !hMPQ.IsValid;
                }
            }

            public MpqArchive MpqHandle
            {
                get
                {
                    return hMPQ;
                }
            }

            ~DHMpqArchive()
            {
                Close();
            }

            public virtual int CompareTo(DHMpqArchive a)
            {
                return CaseInsensitiveComparer.DefaultInvariant.Compare(a.Priority, this.Priority);
            }
        }

        public class DHMpqFile : IDisposable
        {
            IntPtr hFile = IntPtr.Zero;
            FileFormat format = FileFormat.Unknown;
            uint fileSize = 0;
            byte[] buffer = null;
            string name;
            DHMpqArchive mpq;

            public DHMpqFile(DHMpqArchive mpq, MpqHash filehash, string filename)
            {
                if (String.IsNullOrEmpty(filename))
                {
                    fileSize = 0xFFFFFFFF;
                    format = FileFormat.Unknown;
                    return;
                }

                this.name = filename;

                if (mpq != null)
                {
                    this.fileSize = mpq.MpqHandle.GetFileSize(filehash);
                    this.mpq = mpq;
                }

                string extension = Path.GetExtension(filename).ToLower();

                if (extension.Contains(".txt"))
                    format = FileFormat.Txt;
                else
                    if (extension == ".w3a")
                    format = FileFormat.W3A;
                else
                        if (extension == ".w3u")
                    format = FileFormat.W3U;
                else
                            if (extension == ".w3t")
                    format = FileFormat.W3T;
                else
                                if (extension.Contains(".slk"))
                    format = FileFormat.PWXL;
                else
                                    if (extension == ".j")
                    format = FileFormat.J_SCRIPT;
            }

            public DHMpqFile(DHMpqArchive mpq, string filename)
            {
                //Console.WriteLine("mpq==null:" + ((mpq == null) ? "yes" : "no"));
                //Console.WriteLine("filename:" + filename);
                if (String.IsNullOrEmpty(filename))
                {
                    fileSize = 0xFFFFFFFF;
                    format = FileFormat.Unknown;
                    return;
                }

                this.name = filename;

                if (mpq != null)
                {
                    this.fileSize = mpq.MpqHandle.GetFileSize(filename);
                    this.mpq = mpq;
                }

                string extension = Path.GetExtension(filename).ToLower();

                if (extension.Contains(".txt"))
                    format = FileFormat.Txt;
                else
                    if (extension == ".w3a")
                    format = FileFormat.W3A;
                else
                        if (extension == ".w3u")
                    format = FileFormat.W3U;
                else
                            if (extension == ".w3t")
                    format = FileFormat.W3T;
                else
                                if (extension.Contains(".slk"))
                    format = FileFormat.PWXL;
                else
                                    if (extension == ".j")
                    format = FileFormat.J_SCRIPT;
            }

            ~DHMpqFile()
            {
                Dispose();
            }

            public void Dispose()
            {
                this.Close();
            }

            public bool Close()
            {
                if (buffer != null)
                {
                    buffer = null;
                    return true;
                }
                else
                    return false;
            }

            public bool IsNull
            {
                get
                {
                    return (fileSize == 0xFFFFFFFF || fileSize == 0);
                }
            }
            public string Name
            {
                get { return name; }
            }
            public DHMpqArchive Mpq
            {
                get { return mpq; }
            }
            public MpqArchive MpqPtr
            {
                get
                {
                    return (this.mpq != null) ? this.mpq.MpqHandle : null;
                }
            }

            public FileFormat Format
            {
                get { return format; }
            }

            public MemoryStream GetStream()
            {
                if (buffer == null)
                {
                    if (!IsNull)
                    {
                        buffer = new byte[fileSize];
                        MpqStream mpqStream = mpq.MpqHandle.OpenFile(this.name);
                        mpqStream.Read(buffer, 0, (int)fileSize);
                        mpqStream.Close();
                    }
                    else
                        buffer = new byte[] { };
                }

                return new MemoryStream(buffer, 0, buffer.Length, false, true);
            }
            public void ToFile(string filename)
            {
                MemoryStream ms = this.GetStream();
                FileStream fs = new FileStream(filename, FileMode.Create);
                byte[] buffer = ms.GetBuffer();
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }

            public string read_text()
            {
                if (IsNull)
                    return "";
                else
                {
                    unsafe
                    {
                        byte[] buffer = this.GetStream().GetBuffer();
                        fixed (byte* ptr = buffer)
                        {
                            return new string((sbyte*)ptr, 0, buffer.Length, UTF8Encoding.UTF8);
                        }
                    }
                }
            }

            public HabPropertiesCollection read_hpc()
            {
                if (IsNull)
                    return new HabPropertiesCollection();
                else
                    return get_hpc(this.GetStream());
            }

            public void read_hpc_to(HabPropertiesCollection hpc)
            {
                if (IsNull)
                    return;
                else
                    add_hpc(this.GetStream(), hpc);
            }

            public HabPropertiesCollection read_data_hpc()
            {
                if (IsNull || format != FileFormat.PWXL)
                    return new HabPropertiesCollection();
                else
                    return hpc_from_data_pwxl_fast(this.GetStream());
            }

            public MemoryStream get_with_replaced_strings(Dictionary<string, string> OldNewValuePairs)
            {
                if (IsNull)
                    return null;
                else
                    switch (format)
                    {
                        case FileFormat.W3U:
                        case FileFormat.W3T:
                            return replace_w3_strings(this.GetStream(), OldNewValuePairs);

                        case FileFormat.W3A:
                            return replace_w3a_strings(this.GetStream(), OldNewValuePairs);

                        case FileFormat.Txt:
                        case FileFormat.PWXL:
                            HabPropertiesCollection hpc = get_hpc(this.GetStream());
                            return replace_hpc_strings(hpc, OldNewValuePairs);

                        default:
                            return null;
                    }
            }
            public void save_with_replaced_strings_to_file(string filename, Dictionary<string, string> OldNewValuePairs)
            {
                MemoryStream ms = this.get_with_replaced_strings(OldNewValuePairs);

                //////////////////////////////////
                // create ouput file to store
                // this mpq-file content
                //////////////////////////////////                

                FileStream fs = new FileStream(filename,
                    FileMode.Create,
                    FileSystemRights.FullControl,
                    FileShare.ReadWrite,
                    (int)ms.Length,
                    FileOptions.None);

                //////////////////////////////////////////
                // write mpq-file content into that file                    
                //////////////////////////////////////////

                byte[] buffer = ms.GetBuffer();
                fs.Write(buffer, 0, (int)ms.Length);

                ///////////////////////
                // close output file
                ///////////////////////

                fs.Flush();
                fs.Close();
                ms.Close();
            }

            protected HabPropertiesCollection get_hpc(MemoryStream ms)
            {
                switch (format)
                {
                    case FileFormat.Txt:
                        return new HabPropertiesCollection(ms);
                    //return HabPropertiesCollection.LoadFromMemory(ms);                    

                    case FileFormat.PWXL:
                        return hpc_from_pwxl_fast(ms);
                }

                return new HabPropertiesCollection();
            }
            protected void add_hpc(MemoryStream ms, HabPropertiesCollection hpc)
            {
                switch (format)
                {
                    case FileFormat.Txt:
                        HabPropertiesCollection.LoadFromMemoryFast(ms, hpc);
                        break;
                }
            }

            protected HabPropertiesCollection hpc_from_data_pwxl_fast(MemoryStream ms)
            {
                HabPropertiesCollection hpc;

                string[] dcColumnHeaders;
                KeyValuePair<string, string>[] dcLevelsLookup;

                ms.Seek(0, SeekOrigin.Begin); // just in case

                unsafe
                {
                    fixed (byte* memPtr = this.buffer)
                    {
                        sbyte* ptr = (sbyte*)memPtr;
                        SkipLineUnsafe(ref ptr); // skip header //"ID;PWXL;N;E"

                        int X;
                        int Y;

                        // read dimensions // B;X95;Y1371;D0
                        if (!ReadPWXLDimensionsUnsafe(ref ptr, out X, out Y)) return new HabPropertiesCollection();

                        dcColumnHeaders = new string[X + 300];
                        dcLevelsLookup = new KeyValuePair<string, string>[X + 300];

                        hpc = new HabPropertiesCollection(Y);

                        // skip irrelevant // C;Y1;X1;AABond:
                        SkipPWXLIrrelevantLinesUnsafe(ref ptr);

                        int posX = 1;
                        int posY = 1;

                        ReadPWXLColumnHeadersUnsafe(ref ptr, dcColumnHeaders, ref posX, ref posY);

                        string IDcolumnName = dcColumnHeaders[1];

                        // recognize which column headers are related to levels
                        // and fill level-lookup dictionary
                        string property;
                        string level;
                        for (int i = 1; i < dcColumnHeaders.Length; i++)
                            if (extractNameLevelPair(dcColumnHeaders[i], out property, out level))
                                dcLevelsLookup[i] = new KeyValuePair<string, string>(level, property);

                        HabProperties hps = null;
                        HabPropertiesCollection hpcLevels = null;

                        // read pwxl table
                        while (*ptr != 'E')
                        {
                            if (*ptr != 'C')
                            {
                                ++ptr;
                                SkipLineUnsafe(ref ptr);
                                continue;
                            }

                            // C;Y1;X1;K"alias"
                            // or
                            // C;X1;Y1;K"alias"

                            ptr += 2;

                            switch (*ptr)
                            {
                                case (sbyte)'X':
                                    posX = ReadPWXLRowPosUnsafe(ref ptr);

                                    if (*ptr == 'Y')
                                    {
                                        if (hps != null && hps.Count > 1) hpc.AddUnchecked(hps[IDcolumnName].ToString(), hps);

                                        hps = new HabProperties(20);
                                        hpcLevels = new HabPropertiesCollection(4);
                                        hps.Add("hpcLevels", hpcLevels);

                                        SkipPWXLRowPosUnsafe(ref ptr);
                                    }

                                    if (*ptr == 'K')
                                    {
                                        string value = ReadPWXLRowValueUnsafe(ref ptr);

                                        if (posX >= dcColumnHeaders.Length)
                                            break;

                                        string columnName = dcColumnHeaders[posX];

                                        KeyValuePair<string, string> kvp = dcLevelsLookup[posX];

                                        if (kvp.Key != null) // DataA1 -> key=1 value=DataA
                                        {
                                            HabProperties hpsLevel;
                                            if (hpcLevels.TryGetValue(kvp.Key, out hpsLevel))
                                                hpsLevel[kvp.Value] = value;
                                            else
                                            {
                                                hpsLevel = new HabProperties(20);
                                                hpsLevel.Add(kvp.Value, value);
                                                hpcLevels.Add(kvp.Key, hpsLevel);
                                            }
                                        }
                                        else
                                        {
                                            if (columnName != null && columnName.Length > 0)
                                                hps.Add(columnName, value);
                                        }
                                    }
                                    break;

                                case (sbyte)'Y':
                                    if (hps != null && hps.Count > 1)
                                    {
                                        if (IDcolumnName != null && IDcolumnName.Length > 0)
                                            hpc.AddUnchecked(hps[IDcolumnName].ToString(), hps);
                                    }

                                    hps = new HabProperties(20);
                                    hpcLevels = new HabPropertiesCollection(4);
                                    hps.Add("hpcLevels", hpcLevels);

                                    SkipPWXLRowPosUnsafe(ref ptr);

                                    if (*ptr == 'X')
                                        posX = ReadPWXLRowPosUnsafe(ref ptr);

                                    if (*ptr == 'K')
                                    {
                                        string value = ReadPWXLRowValueUnsafe(ref ptr);
                                        if (posX >= dcColumnHeaders.Length)
                                            break;

                                        string columnName = dcColumnHeaders[posX];

                                        KeyValuePair<string, string> kvp = dcLevelsLookup[posX];
                                        if (kvp.Key != null) // DataA1 -> key=1 value=DataA
                                        {
                                            HabProperties hpsLevel;
                                            if (hpcLevels.TryGetValue(kvp.Key, out hpsLevel))
                                                hpsLevel[kvp.Value] = value;
                                            else
                                            {
                                                hpsLevel = new HabProperties(20);
                                                hpsLevel.Add(kvp.Value, value);
                                                hpcLevels.Add(kvp.Key, hpsLevel);
                                            }
                                        }
                                        else
                                        {
                                            if (columnName != null && columnName.Length > 0)
                                                hps.Add(columnName, value);
                                        }
                                    }
                                    break;
                            }
                        }

                        if (hps != null && hps.Count > 1)
                        {
                            if (IDcolumnName != null && IDcolumnName.Length > 0)
                                hpc.AddUnchecked(hps[IDcolumnName].ToString(), hps);
                        }
                    }
                }
                return hpc;
            }
            protected HabPropertiesCollection hpc_from_pwxl_fast(MemoryStream ms)
            {
                HabPropertiesCollection hpc;

                string[] dcColumnHeaders;

                ms.Seek(0, SeekOrigin.Begin); // just in case

                unsafe
                {
                    fixed (byte* memPtr = this.buffer)
                    {
                        sbyte* ptr = (sbyte*)memPtr;
                        SkipLineUnsafe(ref ptr); // skip header //"ID;PWXL;N;E"

                        int X;
                        int Y;

                        // read dimensions // B;X95;Y1371;D0
                        if (!ReadPWXLDimensionsUnsafe(ref ptr, out X, out Y)) return new HabPropertiesCollection();

                        dcColumnHeaders = new string[X + 300];
                        hpc = new HabPropertiesCollection(Y);

                        // skip irrelevant // C;Y1;X1;AABond:
                        SkipPWXLIrrelevantLinesUnsafe(ref ptr);

                        int posX = 1;
                        int posY = 1;

                        ReadPWXLColumnHeadersUnsafe(ref ptr, dcColumnHeaders, ref posX, ref posY);

                        string IDcolumnName = dcColumnHeaders[1];
                        HabProperties hps = null;

                        // read pwxl table
                        while (*ptr != 'E')
                        {
                            if (*ptr != 'C')
                            {
                                ++ptr;
                                SkipLineUnsafe(ref ptr);
                                continue;
                            }

                            // C;Y1;X1;K"alias"
                            // or
                            // C;X1;Y1;K"alias"

                            ptr += 2;

                            if (*ptr == 'X')
                            {
                                posX = ReadPWXLRowPosUnsafe(ref ptr);

                                if (*ptr == 'Y')
                                {
                                    if (hps != null) hpc.Add(hps[IDcolumnName].ToString(), hps);

                                    hps = new HabProperties(20);

                                    SkipPWXLRowPosUnsafe(ref ptr);
                                }

                                if (*ptr == 'K')
                                {
                                    string value = ReadPWXLRowValueUnsafe(ref ptr);

                                    string columnName = dcColumnHeaders[posX];

                                    try
                                    {
                                        if (columnName != null && columnName.Length > 0)
                                            hps.Add(columnName, value);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            else
                                if (*ptr == 'Y')
                            {
                                if (hps != null)
                                {
                                    if (IDcolumnName != null && IDcolumnName.Length > 0)
                                    {
                                        string tstr = hps[IDcolumnName].ToString();
                                        if (tstr != null && tstr.Length > 0)
                                            hpc.AddUnchecked(hps[IDcolumnName].ToString(), hps);
                                    }
                                }

                                hps = new HabProperties(20);

                                SkipPWXLRowPosUnsafe(ref ptr);

                                if (*ptr == 'X')
                                    posX = ReadPWXLRowPosUnsafe(ref ptr);

                                if (*ptr == 'K')
                                {
                                    string value = ReadPWXLRowValueUnsafe(ref ptr);

                                    string columnName = dcColumnHeaders[posX];
                                    if (columnName != null && columnName.Length > 0)
                                        hps.Add(columnName, value);
                                }
                            }
                        }

                        if (hps != null)
                        {
                            if (IDcolumnName != null && IDcolumnName.Length > 0)
                                hpc.Add(hps[IDcolumnName].ToString(), hps);
                        }
                    }
                }
                return hpc;
            }

            protected bool extractNameLevelPair(string s, out string name, out string level)
            {
                unsafe
                {
                    fixed (char* ptrS = s)
                    {
                        char* ptr = ptrS;

                        while (true)
                        {
                            if (ptr == null)
                            {
                                name = null;
                                level = null;
                                return false;
                            }
                            switch (*ptr)
                            {
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    name = new string(ptrS, 0, (int)(ptr - ptrS));
                                    level = new string(ptr);
                                    return true;

                                case '\0':
                                    name = null;
                                    level = null;
                                    return false;

                                default:
                                    ptr++;
                                    break;
                            }
                        }
                    }
                }
            }

            protected MemoryStream replace_w3_strings(MemoryStream ms, Dictionary<string, string> OldNewValuePairs)
            {
                MemoryStream new_ms = new MemoryStream(ms.Capacity);

                try
                {
                    using (BinaryWriter bw = new BinaryWriter(new_ms, Encoding.ASCII))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            ms.Seek(0, SeekOrigin.Begin); // just in case

                            uint id = br.ReadUInt32(); // read format id // 01 00 00 00
                            bw.Write(id);

                            if (id == 1) // read only type-1 format
                            {
                                uint num_of_objects = br.ReadUInt32(); // read number of objects in the first table
                                bw.Write(num_of_objects);

                                for (int i = 0; i < num_of_objects; i++)
                                {
                                    uint orgObjID = br.ReadUInt32(); // read current object's name
                                    bw.Write(orgObjID);

                                    uint newObjID = br.ReadUInt32(); // read new object's name (unused in original table)
                                    bw.Write(newObjID);

                                    uint num_of_props = br.ReadUInt32(); // read number of object's properties
                                    bw.Write(num_of_props);

                                    for (int j = 0; j < num_of_props; j++)
                                    {
                                        uint pName = br.ReadUInt32(); // read current property's name
                                        bw.Write(pName);

                                        uint variableType = br.ReadUInt32(); // read variable type
                                        bw.Write(variableType);

                                        object pValue = null;

                                        switch (variableType)
                                        {
                                            case 0:
                                                pValue = br.ReadInt32();
                                                bw.Write((int)pValue);
                                                break;

                                            case 1:
                                            case 2:
                                                pValue = br.ReadSingle();
                                                bw.Write((float)pValue);
                                                break;

                                            case 3:
                                                pValue = ReadString(ms);
                                                string oldValue = pValue as string;
                                                string newValue;
                                                if (OldNewValuePairs.TryGetValue(oldValue, out newValue) && !string.IsNullOrEmpty(newValue))
                                                    WriteString(bw, newValue);
                                                else
                                                    WriteString(bw, oldValue);
                                                break;
                                        }

                                        uint endOfDef = br.ReadUInt32(); // read end of property definition
                                        bw.Write(endOfDef);
                                    }
                                }

                                num_of_objects = br.ReadUInt32(); // read number of objects in second table
                                bw.Write(num_of_objects);

                                if (num_of_objects > 0) Console.WriteLine("Unexpected table");
                            }
                        }

                        byte[] buffer = new_ms.GetBuffer();
                        new_ms = new MemoryStream(buffer, 0, (int)new_ms.Length, true, true);
                    }
                }
                catch (Exception) { };

                return new_ms;
            }
            protected MemoryStream replace_w3a_strings(MemoryStream ms, Dictionary<string, string> OldNewValuePairs)
            {
                MemoryStream new_ms = new MemoryStream(ms.Capacity);

                try
                {
                    using (BinaryWriter bw = new BinaryWriter(new_ms, Encoding.ASCII))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            ms.Seek(0, SeekOrigin.Begin); // just in case

                            uint id = br.ReadUInt32(); // read format id // 01 00 00 00
                            bw.Write(id);

                            if (id == 1) // read only type-1 format
                            {
                                ////////////////////////////
                                //  read original table
                                ////////////////////////////

                                uint num_of_objects = br.ReadUInt32(); // read number of objects
                                bw.Write(num_of_objects);

                                for (int i = 0; i < num_of_objects; i++)
                                {
                                    uint orgObjName = br.ReadUInt32(); // read current object's name
                                    bw.Write(orgObjName);

                                    uint newObjName = br.ReadUInt32(); // read new object's name (unused in original table)
                                    bw.Write(newObjName);

                                    uint num_of_props = br.ReadUInt32(); // read number of object's properties
                                    bw.Write(num_of_props);

                                    for (int j = 0; j < num_of_props; j++)
                                    {
                                        uint pName = br.ReadUInt32(); // read current property's name
                                        bw.Write(pName);

                                        uint variableType = br.ReadUInt32(); // read variable type
                                        bw.Write(variableType);

                                        uint level = br.ReadUInt32(); // read level
                                        bw.Write(level);

                                        uint dataPtr = br.ReadUInt32(); // read data letter (0=A, 1=B, 2=C, ...)
                                        bw.Write(dataPtr);

                                        object pValue = null;

                                        switch (variableType)
                                        {
                                            case 0:
                                                pValue = br.ReadInt32();
                                                bw.Write((int)pValue);
                                                break;

                                            case 1:
                                            case 2:
                                                pValue = br.ReadSingle();
                                                bw.Write((float)pValue);
                                                break;

                                            case 3:
                                                pValue = ReadString(ms);
                                                string oldValue = pValue as string;
                                                string newValue;
                                                if (OldNewValuePairs.TryGetValue(oldValue, out newValue) && !string.IsNullOrEmpty(newValue))
                                                    WriteString(bw, newValue);
                                                else
                                                    WriteString(bw, oldValue);
                                                break;
                                        }

                                        uint endOfDef = br.ReadUInt32(); // read end of property definition
                                        bw.Write(endOfDef);
                                    }
                                }

                                ////////////////////////////
                                //  read custom table
                                ////////////////////////////

                                num_of_objects = br.ReadUInt32(); // read number of objects
                                bw.Write(num_of_objects);

                                for (int i = 0; i < num_of_objects; i++)
                                {
                                    uint orgName = br.ReadUInt32(); // read original object's name
                                    bw.Write(orgName);

                                    uint objName = br.ReadUInt32(); // read new object's name
                                    bw.Write(objName);

                                    uint num_of_props = br.ReadUInt32(); // read number of object's properties
                                    bw.Write(num_of_props);

                                    for (int j = 0; j < num_of_props; j++)
                                    {
                                        uint pName = br.ReadUInt32(); // read current property's name
                                        bw.Write(pName);

                                        uint variableType = br.ReadUInt32(); // read variable type
                                        bw.Write(variableType);

                                        uint level = br.ReadUInt32(); // read level
                                        bw.Write(level);

                                        uint dataPtr = br.ReadUInt32(); // read data letter (0=A, 1=B, 2=C, ...)
                                        bw.Write(dataPtr);

                                        object pValue = null;

                                        switch (variableType)
                                        {
                                            case 0:
                                                pValue = br.ReadInt32();
                                                bw.Write((int)pValue);
                                                break;

                                            case 1:
                                            case 2:
                                                pValue = br.ReadSingle();
                                                bw.Write((float)pValue);
                                                break;

                                            case 3:
                                                pValue = ReadString(ms);
                                                string oldValue = pValue as string;
                                                string newValue;
                                                if (OldNewValuePairs.TryGetValue(oldValue, out newValue) && !string.IsNullOrEmpty(newValue))
                                                    WriteString(bw, newValue);
                                                else
                                                    WriteString(bw, oldValue);
                                                break;
                                        }

                                        uint endOfDef = br.ReadUInt32(); // read end of property definition
                                        bw.Write(endOfDef);
                                    }
                                }
                            }
                        }

                        byte[] buffer = new_ms.GetBuffer();
                        new_ms = new MemoryStream(buffer, 0, (int)new_ms.Length, true, true);
                    }
                }
                catch (Exception) { };

                return new_ms;
            }
            protected MemoryStream replace_hpc_strings(HabPropertiesCollection hpc, Dictionary<string, string> OldNewValuePairs)
            {
                MemoryStream new_ms = new MemoryStream();

                string value;
                string newValue;
                List<string> list;

                bool isQuoted = false;

                using (StreamWriter sw = new StreamWriter(new_ms, Encoding.ASCII))
                {
                    foreach (HabProperties hps in hpc)
                    {
                        sw.WriteLine("[" + hps.name + "]");
                        foreach (KeyValuePair<string, object> kvp in hps)
                        {
                            value = kvp.Value + "";

                            if (string.IsNullOrEmpty(value))
                            {
                                sw.WriteLine(kvp.Key + "=");
                                continue;
                            }

                            isQuoted = value.StartsWith("\"");
                            list = HabProperty.SplitValue(value);
                            value = "";
                            foreach (string s in list)
                            {
                                if (!(OldNewValuePairs.TryGetValue(s, out newValue) && !string.IsNullOrEmpty(newValue)))
                                    newValue = s;

                                value += (isQuoted) ? "\"" + newValue + "\"" : newValue;
                                value += ",";
                            }
                            value = value.TrimEnd(','); // remove excess comma

                            sw.WriteLine(kvp.Key + "=" + value);
                        }
                    }

                    byte[] buffer = new_ms.GetBuffer();
                    new_ms = new MemoryStream(buffer, 0, (int)new_ms.Length, true, true);
                }

                return new_ms;
            }

            protected string ReadLine(MemoryStream ms)
            {
                unsafe
                {
                    fixed (byte* memPtr = buffer)
                    {
                        sbyte* charPtr = (sbyte*)(memPtr + ms.Position);
                        sbyte* lfCharPtr = charPtr;
                        while (*lfCharPtr++ != '\r') ;

                        string str = new string(charPtr, 0, (int)(lfCharPtr - 1 - charPtr));
                        ms.Position = (long)((byte*)lfCharPtr + 1 - memPtr);

                        return str;
                    }
                }
            }
            protected void SkipLine(MemoryStream ms)
            {
                unsafe
                {
                    fixed (byte* memPtr = buffer)
                    {
                        sbyte* lfCharPtr = (sbyte*)(memPtr + ms.Position);
                        while (*lfCharPtr++ != '\n') ;
                        ms.Position = (long)((byte*)lfCharPtr - memPtr);
                    }
                }
            }

            unsafe protected void SkipLineUnsafe(ref sbyte* memPtr)
            {
                while (*(memPtr++) != '\n') ;
            }
            unsafe protected bool ReadPWXLDimensionsUnsafe(ref sbyte* memPtr, out int X, out int Y)
            {
                // B;Y400;X56;D0 0 399 55
                // or
                // B;X95;Y1371;D0

                while (*memPtr != 'B')
                {
                    memPtr++;
                    SkipLineUnsafe(ref memPtr);
                }

                memPtr += 2;

                X = -1;
                Y = -1;

                while (*memPtr != 'D')
                {
                    switch (*memPtr)
                    {
                        case (sbyte)'X':
                            var oldmemptr = memPtr;
                            X = ReadPWXLRowPosUnsafe(ref memPtr);
                            if (memPtr == oldmemptr)
                                memPtr++;
                            break;

                        case (sbyte)'Y':
                            var oldmemptr2 = memPtr;
                            Y = ReadPWXLRowPosUnsafe(ref memPtr);
                            if (memPtr == oldmemptr2)
                                memPtr++;
                            break;
                        default:
                            memPtr++;
                            break;
                    }
                }

                ++memPtr;
                SkipLineUnsafe(ref memPtr);

                if (X == -1 || Y == -1)
                    return false;
                else
                    return true;
            }
            unsafe protected void SkipPWXLIrrelevantLinesUnsafe(ref sbyte* memPtr)
            {
                // O;L;S;D;V0;K47;G100 0.001
                // or
                // F;W4 4 39
                // or
                // C;Y1;X1;AABond: :Allows for duplicate entries of an ability with different spell data.


                // C;X17;AComma delimited list of specific unit IDs for which this field is valid
                // C;X1;K"ID"

                sbyte* pLine;

                while (true)
                {
                    switch (*memPtr)
                    {
                        case (sbyte)'C':
                            pLine = memPtr;
                            memPtr += 2;

                            switch (*memPtr)
                            {
                                case (sbyte)'Y':
                                    while (*(++memPtr) != ';') ;
                                    memPtr++;

                                    if (*memPtr == 'X')
                                    {
                                        while (*(++memPtr) != ';') ;
                                        memPtr++;
                                    }
                                    break;

                                case (sbyte)'X':
                                    while (*(++memPtr) != ';') ;
                                    memPtr++;

                                    if (*memPtr == 'Y')
                                    {
                                        while (*(++memPtr) != ';') ;
                                        memPtr++;
                                    }
                                    break;

                            }

                            if (*memPtr == 'K')
                            {
                                memPtr = pLine;
                                return;
                            }

                            ++memPtr;
                            SkipLineUnsafe(ref memPtr);
                            break;

                        default:
                            ++memPtr;
                            SkipLineUnsafe(ref memPtr);
                            break;
                    }
                }
            }
            unsafe protected void ReadPWXLColumnHeadersUnsafe(ref sbyte* memPtr, string[] dcColumnHeaders, ref int X, ref int Y)
            {
                // C;Y1;X1;K"alias"
                // or
                // C;X1;Y1;K"alias"                   

                memPtr += 2;

                switch (*memPtr)
                {
                    case (sbyte)'X':
                        X = ReadPWXLRowPosUnsafe(ref memPtr);

                        if (*memPtr == 'Y')
                            Y = ReadPWXLRowPosUnsafe(ref memPtr);

                        if (*memPtr == 'K')
                        {
                            if (X >= dcColumnHeaders.Length)
                                break;
                            dcColumnHeaders[X] = ReadPWXLRowValueUnsafe(ref memPtr);
                        }
                        break;

                    case (sbyte)'Y':
                        Y = ReadPWXLRowPosUnsafe(ref memPtr);

                        if (*memPtr == 'X')
                            X = ReadPWXLRowPosUnsafe(ref memPtr);

                        if (*memPtr == 'K')
                            dcColumnHeaders[X] = ReadPWXLRowValueUnsafe(ref memPtr);
                        break;
                }


                sbyte* pLine;

                while (true)
                {
                    pLine = memPtr;

                    memPtr += 2;

                    switch (*memPtr)
                    {
                        case (sbyte)'X':
                            X = ReadPWXLRowPosUnsafe(ref memPtr);
                            switch (*memPtr)
                            {
                                case (sbyte)'Y':
                                    memPtr = pLine;
                                    return;

                                case (sbyte)'K':
                                    string header = ReadPWXLRowValueUnsafe(ref memPtr);
                                    if (dcColumnHeaders.Length <= X)
                                    {
                                        List<string> tmpListStr = new List<string>(dcColumnHeaders);
                                        while (tmpListStr.Count <= X)
                                        {
                                            tmpListStr.Add("");
                                        }
                                        dcColumnHeaders = tmpListStr.ToArray();
                                    }
                                    dcColumnHeaders[X] = header;
                                    break;

                            }
                            break;

                        case (sbyte)'Y':
                            memPtr = pLine;
                            return;
                    }
                }
            }

            unsafe protected int ReadPWXLRowPosUnsafe(ref sbyte* memPtr)
            {
                UInt32* ptr = (UInt32*)(++memPtr);

                // '123;' or '123\r'
                switch (*ptr & 0xFF000000)
                {
                    case 0x3B000000:
                    case 0x0D000000:
                        memPtr += 4;
                        return (int)(((*ptr >> 16) & 0x0000000F) + ((*ptr >> 8) & 0x0000000F) * 10 + (*ptr & 0x0000000F) * 100);
                }

                // '12;' or '12\r'
                switch (*ptr & 0x00FF0000)
                {
                    case 0x003B0000:
                    case 0x000D0000:
                        memPtr += 3;
                        return (int)(((*ptr >> 8) & 0x0000000F) + (*ptr & 0x0000000F) * 10);
                }

                // '1;' or '1\r'
                switch (*ptr & 0x0000FF00)
                {
                    case 0x00003B00:
                    case 0x00000D00:
                        memPtr += 2;
                        return (int)(*ptr & 0x0000000F);
                }

                // '1234;' or '1234\r'
                memPtr += 5;
                return (int)(((*ptr >> 24) & 0x0000000F) + ((*ptr >> 16) & 0x0000000F) * 10 + ((*ptr >> 8) & 0x0000000F) * 100 + (*ptr & 0x0000000F) * 1000);
            }
            unsafe protected void SkipPWXLRowPosUnsafe(ref sbyte* memPtr)
            {
                ++memPtr;
                while (*(memPtr++) != ';') ;
            }
            unsafe protected string ReadPWXLRowValueUnsafe(ref sbyte* memPtr)
            {
                sbyte* pStart;
                string result;

                if (*(++memPtr) == '"')
                {
                    pStart = ++memPtr;
                    while (*memPtr != '"') memPtr++;

                    result = new string(pStart, 0, (int)(memPtr - pStart));

                    // skip '"\r\n' or '"\n'
                    memPtr += (*(memPtr + 1) == '\r') ? 3 : 2;
                }
                else
                {
                    pStart = memPtr;
                    while (*(++memPtr) != '\n') ;

                    result = new string(pStart, 0, (int)(memPtr - ((*(memPtr - 1) == '\r') ? 1 : 0) - pStart));
                    memPtr += 1;
                }
                return result;
            }

            public string ReadString(MemoryStream ms)
            {
                unsafe
                {
                    fixed (byte* memPtr = buffer)
                    {
                        sbyte* charPtr = (sbyte*)(memPtr + ms.Position);

                        sbyte* pEnd = charPtr;
                        while (*pEnd != 0) pEnd++;

                        int length = (int)(pEnd - charPtr);

                        string str = new string(charPtr, 0, length, Encoding.UTF8);
                        ms.Position = ms.Position + length + 1;//including zero char
                        return str;
                    }
                }
            }
            protected string ReadString(MemoryStream ms, byte[] ms_buffer)
            {
                unsafe
                {
                    fixed (byte* memPtr = ms_buffer)
                    {
                        sbyte* charPtr = (sbyte*)(memPtr + ms.Position);
                        string str = new string(charPtr);
                        ms.Position = ms.Position + str.Length + 1;//including zero char
                        return str;
                    }
                }
            }

            protected void WriteString(BinaryWriter bw, string s)
            {
                bw.Write(ASCIIEncoding.ASCII.GetBytes(s.ToCharArray()));
                bw.Write((byte)0);// zero char
            }

            public string ReadChars(MemoryStream ms, int count)
            {
                unsafe
                {
                    fixed (byte* memPtr = buffer)
                    {
                        string str = new string((sbyte*)memPtr, (int)ms.Position, count);
                        ms.Position = ms.Position + count;//including zero char
                        return str;
                    }
                }
            }
        }

        public class DHMpqArchiveCollection : List<DHMpqArchive>
        {
            public DHMpqArchiveCollection() { }

            public new void Add(DHMpqArchive mpq)
            {
                base.Insert(0, mpq);
                this.Sort();
            }
            public void AddEx(DHMpqArchive mpq)
            {
                if (!this.Contains(mpq.Name))
                {
                    base.Insert(0, mpq);
                    this.Sort();
                }
            }

            public bool Contains(string Name)
            {
                foreach (DHMpqArchive mpq in this)
                    if (mpq.Name == Name) return true;
                return false;
            }
            public int IndexOf(string Name)
            {
                for (int i = 0; i < Count; i++)
                    if ((this[i] as DHMpqArchive).Name == Name)
                        return i;
                return -1;
            }
            public void Remove(string Name)
            {
                for (int i = 0; i < Count; i++)
                    if (this[i].Name == Name)
                        RemoveAt(i);
            }

            public DHMpqArchive this[string name]
            {
                get
                {
                    foreach (DHMpqArchive mpq in this)
                        if (mpq.Name == name)
                            return mpq;
                    return null;
                }
            }
        }

        public class DHMpqDatabase
        {
            public static Dictionary<string, HabPropertiesCollection> UnitSlkDatabase = new Dictionary<string, HabPropertiesCollection>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            public static Dictionary<string, HabPropertiesCollection> ItemSlkDatabase = new Dictionary<string, HabPropertiesCollection>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            public static Dictionary<string, HabPropertiesCollection> AbilitySlkDatabase = new Dictionary<string, HabPropertiesCollection>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            public static Dictionary<string, HabPropertiesCollection> UpgradeSlkDatabase = new Dictionary<string, HabPropertiesCollection>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            public static Dictionary<string, HabPropertiesCollection> EditorDatabase = new Dictionary<string, HabPropertiesCollection>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

            public static void LoadUnits()
            {
                HabPropertiesCollection hpc;
                UnitSlkDatabase.Clear();

                // metadata

                hpc = DHRC.Default.GetFile(MpqPath.Unit.MetaData).read_hpc();
                UnitSlkDatabase.Add("MetaData", hpc);

                // data                

                hpc = DHRC.Default.GetFile(MpqPath.Unit.Data).read_hpc();
                UnitSlkDatabase.Add("UnitData", hpc);

                // weapons

                hpc = DHRC.Default.GetFile(MpqPath.Unit.Weapons).read_hpc();
                UnitSlkDatabase.Add("UnitWeapons", hpc);

                // balance

                hpc = DHRC.Default.GetFile(MpqPath.Unit.Balance).read_hpc();
                UnitSlkDatabase.Add("UnitBalance", hpc);

                // abilities

                hpc = DHRC.Default.GetFile(MpqPath.Unit.Abilities).read_hpc();
                UnitSlkDatabase.Add("UnitAbilities", hpc);

                // UI

                hpc = DHRC.Default.GetFile(MpqPath.Unit.UI).read_hpc();
                UnitSlkDatabase.Add("UnitUI", hpc);

                // profile

                hpc = new HabPropertiesCollection(hpc.Count);

                foreach (string filename in MpqPath.Unit.ProfileList)
                    DHRC.Default.GetFile(filename).read_hpc_to(hpc);

                UnitSlkDatabase.Add("Profile", hpc);

                /////////////////////////////////////
                // load and apply custom data
                /////////////////////////////////////

                ApplyCustomTable(MpqPath.Unit.CustomTable);
            }

            public static void LoadItems()
            {
                HabPropertiesCollection hpc;
                ItemSlkDatabase.Clear();

                // metadata

                hpc = DHRC.Default.GetFile(MpqPath.Item.MetaData).read_hpc();
                ItemSlkDatabase.Add("MetaData", hpc);

                // data

                hpc = DHRC.Default.GetFile(MpqPath.Item.Data).read_hpc();
                ItemSlkDatabase.Add("ItemData", hpc);

                // profile

                hpc = new HabPropertiesCollection(hpc.Count);

                foreach (string filename in MpqPath.Item.ProfileList)
                    DHRC.Default.GetFile(filename).read_hpc_to(hpc);

                ItemSlkDatabase.Add("Profile", hpc);

                /////////////////////////////////////
                // load and apply custom data
                /////////////////////////////////////

                ApplyCustomTable(MpqPath.Item.CustomTable);
            }

            public static void LoadAbilities(SplashScreen ss)
            {
                HabPropertiesCollection hpc;
                AbilitySlkDatabase.Clear();

                // metadata

                hpc = DHRC.Default.GetFile(MpqPath.Ability.MetaData).read_hpc();
                AbilitySlkDatabase.Add("MetaData", hpc); if (ss != null) ss.ProgressAdd(6);

                // data

                hpc = DHRC.Default.GetFile(MpqPath.Ability.Data).read_data_hpc();
                AbilitySlkDatabase.Add("AbilityData", hpc); if (ss != null) ss.ProgressAdd(6);

                // profile

                hpc = new HabPropertiesCollection(hpc.Count);

                foreach (string filename in MpqPath.Ability.ProfileList)
                    DHRC.Default.GetFile(filename).read_hpc_to(hpc);

                AbilitySlkDatabase.Add("Profile", hpc); if (ss != null) ss.ProgressAdd(6);

                /////////////////////////////////////
                // load and apply custom data
                /////////////////////////////////////

                ApplyCustomTable(MpqPath.Ability.CustomTable); if (ss != null) ss.ProgressAdd(6);
            }

            public static void LoadUpgrades(SplashScreen ss)
            {
                HabPropertiesCollection hpc;
                UpgradeSlkDatabase.Clear();

                // data

                hpc = DHRC.Default.GetFile(MpqPath.Upgrade.Data).read_hpc();
                UpgradeSlkDatabase.Add("UpgradeData", hpc); if (ss != null) ss.ProgressAdd(2);

                // profile

                hpc = new HabPropertiesCollection(hpc.Count);

                foreach (string filename in MpqPath.Upgrade.ProfileList)
                    DHRC.Default.GetFile(filename).read_hpc_to(hpc);

                UpgradeSlkDatabase.Add("Profile", hpc); if (ss != null) ss.ProgressAdd(4);
            }

            public static void LoadEditorData()
            {
                HabPropertiesCollection hpc;
                EditorDatabase.Clear();

                // data

                hpc = DHRC.Default.GetFile(MpqPath.Editor.Data).read_hpc();
                EditorDatabase.Add("Data", hpc);

                // strings

                DHRC.Default.GetFile(MpqPath.Editor.Strings).read_hpc_to(hpc);

                // misc

                DHRC.Default.GetFile(MpqPath.Editor.Misc).read_hpc_to(hpc);
            }

            private static void ApplyCustomTable(string filename)
            {
                DHMpqFile ctFile = DHRC.Default.GetFile(filename);
                if (ctFile.IsNull) return;

                switch (ctFile.Format)
                {
                    case FileFormat.W3U:
                        apply_w3_table(ctFile, DHMpqDatabase.UnitSlkDatabase);
                        break;

                    case FileFormat.W3T:
                        apply_w3_table(ctFile, DHMpqDatabase.ItemSlkDatabase);
                        break;

                    case FileFormat.W3A:
                        apply_w3a_table(ctFile, DHMpqDatabase.AbilitySlkDatabase);
                        break;
                }
            }

            static void apply_w3_table(DHMpqFile ctFile, Dictionary<string, HabPropertiesCollection> SlkDatabase)
            {
                MemoryStream ms = ctFile.GetStream();
                HabPropertiesCollection hpcMetaData = SlkDatabase["MetaData"];

                try
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin); // just in case
                        uint id = br.ReadUInt32(); // read format id // 01 00 00 00                        

                        ////////////////////////////
                        //  read original table
                        ////////////////////////////

                        uint num_of_objects = br.ReadUInt32(); // read number of objects

                        for (int i = 0; i < num_of_objects; i++)
                        {
                            string objName = ctFile.ReadChars(ms, 4); // read current object's name                                 

                            br.ReadUInt32(); // read new object's name (unused in original table)

                            uint num_of_props = br.ReadUInt32(); // read number of object's properties

                            for (int j = 0; j < num_of_props; j++)
                            {
                                string pName = ctFile.ReadChars(ms, 4); // read current property's name

                                uint variableType = br.ReadUInt32(); // read variable type

                                object pValue = null;

                                switch (variableType)
                                {
                                    case 0:
                                        pValue = br.ReadInt32();
                                        break;

                                    case 1:
                                    case 2:
                                        pValue = br.ReadSingle();
                                        break;

                                    case 3:
                                        pValue = ctFile.ReadString(ms);
                                        break;
                                }

                                HabProperties hpsMetaData;
                                if (hpcMetaData.TryGetValue(pName, out hpsMetaData))
                                {
                                    pName = hpsMetaData.GetValue("field") as string;
                                    string slkName = hpsMetaData.GetValue("slk") as string;
                                    string index = hpsMetaData.GetValue("index") as string;

                                    HabPropertiesCollection hpcSlk;
                                    if (SlkDatabase.TryGetValue(slkName, out hpcSlk))
                                    {
                                        HabProperties hpsObj;
                                        if (hpcSlk.TryGetValue(objName, out hpsObj))
                                        {
                                            if (index == "-1")
                                                hpsObj[pName] = pValue;
                                            else
                                            {
                                                object value;
                                                if (hpsObj.TryGetValue(pName, out value))
                                                {
                                                    if (value is string)
                                                    {
                                                        value = HabProperty.SplitValue(value as string);
                                                        hpsObj[pName] = value;
                                                    }
                                                }
                                                else
                                                {
                                                    value = new List<string>(1);
                                                    hpsObj[pName] = value;
                                                }

                                                if (value is List<string>)
                                                {
                                                    if (index == "0")
                                                    {
                                                        HabProperty.SetValue(value as List<string>, 0, pValue.ToString());
                                                        //(value as List<string>).Add(pValue.ToString());
                                                    }
                                                    else
                                                        if (index == "1")
                                                        HabProperty.SetValue(value as List<string>, 1, pValue.ToString());
                                                }
                                            }
                                        }
                                    }
                                }

                                br.ReadUInt32(); // read end of property definition
                            }
                        }

                        ////////////////////////////
                        //  read custom table
                        ////////////////////////////

                        num_of_objects = br.ReadUInt32(); // read number of objects

                        for (int i = 0; i < num_of_objects; i++)
                        {
                            string orgName = ctFile.ReadChars(ms, 4); // read original object's name

                            string objName = ctFile.ReadChars(ms, 4); // read new object's name

                            uint num_of_props = br.ReadUInt32(); // read number of object's properties

                            for (int j = 0; j < num_of_props; j++)
                            {
                                string pName = ctFile.ReadChars(ms, 4); // read current property's name

                                uint variableType = br.ReadUInt32(); // read variable type

                                object pValue = null;

                                switch (variableType)
                                {
                                    case 0:
                                        pValue = br.ReadInt32();
                                        break;

                                    case 1:
                                    case 2:
                                        pValue = br.ReadSingle();
                                        break;

                                    case 3:
                                        pValue = ctFile.ReadString(ms);
                                        break;
                                }

                                HabProperties hpsMetaData;
                                if (hpcMetaData.TryGetValue(pName, out hpsMetaData))
                                {
                                    pName = hpsMetaData.GetValue("field") as string;
                                    string slkName = hpsMetaData.GetValue("slk") as string;
                                    string index = hpsMetaData.GetValue("index") as string;

                                    HabPropertiesCollection hpcSlk;
                                    if (SlkDatabase.TryGetValue(slkName, out hpcSlk))
                                    {
                                        HabProperties hpsObj;
                                        if (hpcSlk.TryGetValue(objName, out hpsObj))
                                        {
                                            if (index == "-1")
                                                hpsObj[pName] = pValue;
                                            else
                                            {
                                                object value;
                                                if (hpsObj.TryGetValue(pName, out value))
                                                {
                                                    if (value is string)
                                                    {
                                                        value = HabProperty.SplitValue(value as string);
                                                        hpsObj[pName] = value;
                                                    }
                                                }
                                                else
                                                {
                                                    value = new List<string>(1);
                                                    hpsObj[pName] = value;
                                                }

                                                if (value is List<string>)
                                                {
                                                    if (index == "0")
                                                    {
                                                        HabProperty.SetValue(value as List<string>, 0, pValue.ToString());
                                                        //(value as List<string>).Add(pValue.ToString());
                                                    }
                                                    else
                                                        if (index == "1")
                                                        HabProperty.SetValue(value as List<string>, 1, pValue.ToString());
                                                }
                                            }
                                        }
                                    }
                                }

                                br.ReadUInt32(); // read end of property definition
                            }
                        }
                    }
                }
                catch (Exception)
                {
                };
            }
            static void apply_w3a_table(DHMpqFile ctFile, Dictionary<string, HabPropertiesCollection> SlkDatabase)
            {
                MemoryStream ms = ctFile.GetStream();
                HabPropertiesCollection hpcMetaData = SlkDatabase["MetaData"];

                try
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        ms.Seek(0, SeekOrigin.Begin); // just in case
                        uint id = br.ReadUInt32(); // read format id // 01 00 00 00

                        ////////////////////////////
                        //  read original table
                        ////////////////////////////

                        uint num_of_objects = br.ReadUInt32(); // read number of objects

                        for (int i = 0; i < num_of_objects; i++)
                        {
                            string objName = ctFile.ReadChars(ms, 4); // read current object's name                                

                            br.ReadUInt32(); // read new object's name (unused in original table)

                            uint num_of_props = br.ReadUInt32(); // read number of object's properties

                            for (int j = 0; j < num_of_props; j++)
                            {
                                string pName = ctFile.ReadChars(ms, 4); // read current property's name

                                uint variableType = br.ReadUInt32(); // read variable type

                                uint level = br.ReadUInt32(); // read level
                                uint dataPtr = br.ReadUInt32(); // read data letter (1=A, 2=B, 3=C, ...)

                                object pValue = null;

                                switch (variableType)
                                {
                                    case 0:
                                        pValue = br.ReadInt32();
                                        break;

                                    case 1:
                                    case 2:
                                        pValue = br.ReadSingle();
                                        break;

                                    case 3:
                                        pValue = ctFile.ReadString(ms);
                                        break;
                                }

                                HabProperties hpsMetaData;
                                if (hpcMetaData.TryGetValue(pName, out hpsMetaData))
                                {
                                    pName = hpsMetaData.GetValue("field") as string;
                                    string slkName = hpsMetaData.GetValue("slk") as string;
                                    string index = hpsMetaData.GetValue("index") as string;

                                    if (dataPtr != 0) pName += ((char)(dataPtr + 64));

                                    HabPropertiesCollection hpcSlk;
                                    if (SlkDatabase.TryGetValue(slkName, out hpcSlk))
                                    {
                                        HabProperties hpsObj;
                                        if (hpcSlk.TryGetValue(objName, out hpsObj))
                                        {
                                            if (index == "-1")
                                                w3a_set_value_for_level(hpsObj, pName, pValue, level);
                                            else
                                            {
                                                object value;
                                                if (hpsObj.TryGetValue(pName, out value))
                                                {
                                                    if (value is string)
                                                    {
                                                        value = HabProperty.SplitValue(value as string);
                                                        hpsObj[pName] = value;
                                                    }
                                                }
                                                else
                                                {
                                                    value = new List<string>(1);
                                                    hpsObj[pName] = value;
                                                }

                                                if (value is List<string>)
                                                {
                                                    if (index == "0")
                                                        HabProperty.SetValue(value as List<string>,
                                                                        (level == 0) ? (int)level : (int)level - 1,
                                                                        pValue.ToString());
                                                    else
                                                        if (index == "1")
                                                        HabProperty.SetValue(value as List<string>, 1, pValue.ToString());
                                                }
                                            }
                                        }
                                    }
                                }

                                br.ReadUInt32(); // read end of property definition
                            }
                        }

                        ////////////////////////////
                        //  read custom table
                        ////////////////////////////

                        num_of_objects = br.ReadUInt32(); // read number of objects

                        for (int i = 0; i < num_of_objects; i++)
                        {
                            string orgName = ctFile.ReadChars(ms, 4); // read original object's name

                            string objName = ctFile.ReadChars(ms, 4); // read new object's name                                

                            foreach (HabPropertiesCollection hpcSlk in SlkDatabase.Values)
                                hpcSlk.MakeCopyUncheckedEx(orgName, objName);

                            uint num_of_props = br.ReadUInt32(); // read number of object's properties

                            for (int j = 0; j < num_of_props; j++)
                            {
                                string pName = new string(br.ReadChars(4)); // read current property's name

                                uint variableType = br.ReadUInt32(); // read variable type

                                uint level = br.ReadUInt32(); // read level
                                uint dataPtr = br.ReadUInt32(); // read data letter (1=A, 2=B, 3=C, ...)

                                object pValue = null;

                                switch (variableType)
                                {
                                    case 0:
                                        pValue = br.ReadInt32();
                                        break;

                                    case 1:
                                    case 2:
                                        pValue = br.ReadSingle();
                                        break;

                                    case 3:
                                        pValue = ctFile.ReadString(ms);
                                        break;
                                }

                                HabProperties hpsMetaData;
                                if (hpcMetaData.TryGetValue(pName, out hpsMetaData))
                                {
                                    pName = hpsMetaData.GetValue("field") as string;
                                    string slkName = hpsMetaData.GetValue("slk") as string;
                                    string index = hpsMetaData.GetValue("index") as string;

                                    if (dataPtr != 0) pName += ((char)(dataPtr + 64));

                                    HabPropertiesCollection hpcSlk;
                                    if (SlkDatabase.TryGetValue(slkName, out hpcSlk))
                                    {
                                        HabProperties hpsObj;
                                        if (hpcSlk.TryGetValue(objName, out hpsObj))
                                        {
                                            if (index == "-1")
                                                w3a_set_value_for_level(hpsObj, pName, pValue, level);
                                            else
                                            {
                                                object value;
                                                if (hpsObj.TryGetValue(pName, out value))
                                                {
                                                    if (value is string)
                                                    {
                                                        value = HabProperty.SplitValue(value as string);
                                                        hpsObj[pName] = value;
                                                    }
                                                }
                                                else
                                                {
                                                    value = new List<string>(1);
                                                    hpsObj[pName] = value;
                                                }

                                                if (value is List<string>)
                                                {
                                                    if (index == "0")
                                                        HabProperty.SetValue(value as List<string>,
                                                                        (level == 0) ? (int)level : (int)level - 1,
                                                                        pValue.ToString());
                                                    else
                                                        if (index == "1")
                                                        HabProperty.SetValue(value as List<string>, 1, pValue.ToString());
                                                }
                                            }
                                        }
                                    }
                                }

                                br.ReadUInt32(); // read end of property definition
                            }
                        }
                    }
                }
                catch (Exception) { };
            }
            static void w3a_set_value_for_level(HabProperties hpsObj, string pName, object pValue, uint level)
            {
                if (level == 0) hpsObj[pName] = pValue;
                else
                {
                    object tmp;
                    HabPropertiesCollection hpcLevels;
                    if (hpsObj.TryGetValue("hpcLevels", out tmp))
                        hpcLevels = tmp as HabPropertiesCollection;
                    else
                    {
                        hpcLevels = new HabPropertiesCollection(4);
                        hpsObj.Add("hpcLevels", hpcLevels);
                    }

                    HabProperties hpsLevel;
                    string level_key = level.ToString();

                    // check if level with this key already exists

                    if (hpcLevels.TryGetValue(level_key, out hpsLevel))
                    {
                        // if this is not a custom level                        
                        if (hpsLevel.priority == null)
                        {
                            // then move this level to the next free spot

                            do
                            {
                                string nokeylevel_key = (++level).ToString();

                                // check if hpcLevels does not contain level with specified key

                                if (!hpcLevels.ContainsKey(nokeylevel_key))
                                {
                                    // if level with that key does not exists,
                                    // move this level to it                                                                        

                                    hpcLevels.AddUnchecked(nokeylevel_key, hpsLevel);
                                    break;
                                }
                            }
                            while (true);

                            // now create a custom level in this slot

                            hpsLevel = hpsLevel.GetCopy();
                            hpsLevel[pName] = pValue;
                            hpsLevel.name = level_key;
                            hpsLevel.priority = 0; // mark this level as a custom level
                            hpcLevels[level_key] = hpsLevel;
                        }
                        else // if this is a custom level
                            hpsLevel[pName] = pValue;
                    }
                    else
                    {
                        hpsLevel = new HabProperties(20);
                        hpsLevel.Add(pName, pValue);
                        hpsLevel.priority = 0; // mark this level as a custom level
                        hpcLevels.AddUnchecked(level_key, hpsLevel);
                    }
                }
            }
        }

        public class DHMATH
        {
            public static List<List<T>> GetCombinations<T>(IList<T> list, int max_length)
            {
                long capacity = (int)LCC(list.Count, max_length);
                List<List<T>> combinations = new List<List<T>>();
                DHTIMER.ResetCount();
                DHTIMER.StartCount();
                for (int i = 0; i < list.Count; i++)
                {
                    List<T> gathered = new List<T>(max_length);
                    gathered.Add(list[i]);

                    addCombinations<T>(list, i, gathered, combinations, max_length);
                }
                DHTIMER.EndCount();
                DHTIMER.PrintCount();
                return combinations;
            }
            static void addCombinations<T>(IList<T> list, int index, List<T> gathered, List<List<T>> combinations, int max_length)
            {
                combinations.Add(gathered);

                if (gathered.Count >= max_length) return;

                List<T> previous_gathered = gathered;

                for (int i = index; i < list.Count; i++)
                {
                    gathered = new List<T>(previous_gathered);
                    gathered.Add(list[i]);

                    addCombinations<T>(list, i, gathered, combinations, max_length);
                }
            }

            #region C(N,K)
            // C(N,K) function code, copied from some webpage :)
            static long gcd(long a, long b)
            {
                if (a % b == 0) return b;
                else return gcd(b, a % b);
            }
            static void Divbygcd(ref long a, ref long b)
            {
                long g = gcd(a, b);
                a /= g;
                b /= g;
            }
            /// <summary>
            /// сочетани€ без повторений
            /// </summary>
            /// <param name="n"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public static long C(int n, int k)
            {
                long numerator = 1, denominator = 1, toMul, toDiv, i;

                if (k > n / 2) k = n - k; /* use smaller k */

                for (i = k; i > 0; i--)
                {
                    toMul = n - k + i;
                    toDiv = i;
                    Divbygcd(ref toMul, ref toDiv);       /* always divide before multiply */
                    Divbygcd(ref numerator, ref toDiv);
                    Divbygcd(ref toMul, ref denominator);
                    numerator *= toMul;
                    denominator *= toDiv;
                }

                return numerator / denominator;
            }
            #endregion

            /// <summary>
            /// сочетани€ с повторени€ми (Complete Combinations)
            /// </summary>
            /// <param name="n">number of elements</param>
            /// <param name="k">combination length</param>
            /// <returns></returns>
            public static long CC(int n, int k)
            {
                return C(n + k - 1, k);
            }

            /// <summary>
            /// ladder complete combinations (my own function)
            /// calculates total number of ladder-like combinations for given number of elements
            /// Example:  M = {1,2,3}
            /// Combinations = {1,11,111,112,113,12,122,123,13,133,2,22,222,223,23,233,3,33,333}
            /// LC(3,3) = 19
            /// </summary>
            /// <param name="n">number of elements</param>
            /// <param name="k">maximum combination length</param>
            /// <returns></returns>
            public static long LCC(int n, int k)
            {
                long result = 0;
                for (int i = 1; i <= k; i++)
                    result += CC(n, i);
                return result;
            }
            /// <summary>
            /// ladder combinations (my own function)
            /// calculates total number of ladder-like combinations for given number of elements
            /// Example:  M = {1,2,3}
            /// LC(3,3) = 7
            /// Combinations = {1,12,123,13,2,23,3}
            /// LC(3,2) = 6
            /// Combinations = {1,12,13,2,23,3}
            /// </summary>
            /// <param name="n">number of elements</param>
            /// <param name="k">maximum combination length</param>
            /// <returns></returns>
            public static long LC(int n, int k)
            {
                long result = 0;
                for (int i = 1; i <= k; i++)
                    result += C(n, i);
                return result;
            }

            /// <summary>
            /// Bash's stun time converted to dps
            /// </summary>
            /// <param name="BAT"></param>
            /// <param name="IAS"></param>
            /// <param name="C"></param>
            /// <param name="?"></param>
            /// <returns></returns>
            public static double BDPS(double C, double T, double BAT, double IAS, double dpa, double P)
            {
                double cooldown = BAT / (1 + IAS);

                double N = Math.Floor(T / cooldown); // N = floor(T/cooldown)

                double V = 1 - Math.Pow(1 - C, N); // V = 1-(1-C)^N

                double OBD = bdpsOBD(C, T, N, cooldown, dpa);

                double k = Math.Ceiling(Math.Log10(P / (OBD * V)) / Math.Log10(V)); // k = ceiling( lg(P/OBD*V) / lg(V) )

                double result = (OBD * (Math.Pow(V, k + 1) - 1.0) * (1 + IAS) * C) / (BAT * (V - 1)); // BDPS = C*OBD(V^(k+1) - 1) / ((V-1)*cooldown)

                return result;
            }

            private static double bdpsOBD(double C, double T, double N, double cooldown, double dpa)
            {
                double dps = dpa / cooldown;

                double result = 0.0;

                for (int i = 1; i <= N; i++)
                    result += ((double)i) * dpa * C * Math.Pow(1 - C, i - 1); // i*dmg*C(1-C)^i-1

                result += (N * dpa + Math.IEEERemainder(T, cooldown) * dps) * Math.Pow(1 - C, N); // (N*dmg + rem(T,cooldown)dps)*(1-C)^N

                return result;
            }
        }

        /*
         * object fields description:
         * 
         * upro - name ("Crixalis")
         * unam - class type ("Sand King")
         */

        /*
         *  ability code description:
         *  
         * AInv - inventory with DataA1 slots
         * Arel - DataA1 hp regen
         * AIml - DataA1 hp bonus
         * Assk - DataA1% chance to block DataC1 damage
         * Alsb/AIsb? - DataB1=C1=D1 chance to release chain lightning
         * AOcl - chaing lightning with DataA1 damage and jumps DataB1 times
         * AIat - DataA1 damage bonus
         * AIab - DataA1 agility & DataB1 intelligence & DataC1 strength bonus
         * Aamk - DataA1 agility & DataB1 intelligence & DataC1 strength bonus
         * AOhx - hex
         * AIrm - DataA1 enhanced mana regen (example: 1.5 for 150%)
         * AIas - DataA1 increased attack speed (example: 0.15 for 15%)
         * AIms - DataA1 increased movement speed
         * AHbh - DataA1% chance to deal DataC1 bonus damage 
         * AIde - DataA1 bonus armor
         * 
         * (AHad) - DataA1 bonus armor to units in the Area1 area
         * 
         * AIdd - DataE1 of spell damage received (example: 0.9 makes 0.1 spell resistance)
         * AIsr - DataB1 spell resistance (example: 0.15)
         * 
         * AEev - evasion
         * AOcr - DataA1% chance to deal DataB1 crit
         * ANdb - DataA1% chance to deal DataB1 crit
         * 
         * AHbh - DataA1% chance to bash for HeroDur1 seconds, dealing DataC1 bonus damage
         * Atdg - tornado DataA1 damage in Area1 radius
         * ANpi - strikes for DataA1 damage in Area1 radius every HeroDur1 seconds
         *          
         */
    }

    public class UIColors
    {
        public static Color bonus_damage = Color.Lime;
        public static Color negative_damage = Color.Red;
        public static Color unstable_bonus_damage = Color.DeepSkyBlue;
        public static Color activeAbility = Color.NavajoWhite;
        public static Color toolTipData = Color.LightSkyBlue;
        public static Color activeEffect = Color.LightBlue;
    }
    public class UIFonts
    {
        public static string ArialFontName;
        public static string VerdanaFontName;

        static UIFonts()
        {
            ResetFonts();
        }

        public static void ResetFonts()
        {
            ArialFontName = Core.Resources.DHCFG.Items["Fonts"].GetStringValue("ArialFontName", "Arial");
            VerdanaFontName = Core.Resources.DHCFG.Items["Fonts"].GetStringValue("VerdanaFontName", "Verdana");

            smallArial = new Font(ArialFontName, 3, FontStyle.Regular);
            boldArial8 = new Font(ArialFontName, 8, FontStyle.Bold);
            normalVerdana = new Font(VerdanaFontName, 8);
            boldVerdana = new Font(VerdanaFontName, 8, FontStyle.Bold);
            boldLargeVerdana = new Font(VerdanaFontName, 15, FontStyle.Bold);
            boldHugeVerdana = new Font(VerdanaFontName, 22, FontStyle.Bold);
            tahoma9_75Bold = new Font("Tahoma", 9.75f, FontStyle.Bold);
        }

        public static Font smallArial;
        public static Font boldArial8;
        public static Font normalVerdana;
        public static Font boldVerdana;
        public static Font boldLargeVerdana;
        public static Font boldHugeVerdana;
        public static Font tahoma9_75Bold;
    }
    public class UIGraphics
    {
        public static void DrawAutoCastFrame(PaintEventArgs pe, Color color)
        {
            Rectangle rc = pe.ClipRectangle;

            double factor = 0.2;
            int width = 4;
            Pen pen = new Pen(color, width);

            // top-left

            pe.Graphics.DrawLine(pen,
                rc.X, rc.Y + (width - 1), rc.X + width + (int)(rc.Width * factor), rc.Y + (width - 1));

            pe.Graphics.DrawLine(pen,
                rc.X + (int)(width / 2), rc.Y + 1, rc.X + (int)(width / 2), rc.Y + width + (int)(rc.Height * factor));

            // bottom-left

            pe.Graphics.DrawLine(pen,
                rc.X, rc.Bottom - width, rc.X + width + (int)(rc.Width * factor), rc.Bottom - width);

            pe.Graphics.DrawLine(pen,
                rc.X + (int)(width / 2), rc.Bottom - width, rc.X + (int)(width / 2), rc.Bottom - width - 1 - (int)(rc.Height * factor));

            // top-right

            pe.Graphics.DrawLine(pen,
                rc.Right, rc.Y + (width - 1), rc.Right - width - (int)(rc.Width * factor), rc.Y + (width - 1));

            pe.Graphics.DrawLine(pen,
                rc.Right - (int)(width / 2), rc.Y + 1, rc.Right - (int)(width / 2), rc.Y + width + (int)(rc.Height * factor));

            // bottom-right

            pe.Graphics.DrawLine(pen,
                rc.Right, rc.Bottom - width, rc.Right - width - (int)(rc.Width * factor), rc.Bottom - width);

            pe.Graphics.DrawLine(pen,
                rc.Right - (int)(width / 2), rc.Bottom - width, rc.Right - (int)(width / 2), rc.Bottom - width - 1 - (int)(rc.Height * factor));
        }
    }

    public class UIRichText
    {
        public static readonly UIRichText Default = new UIRichText();

        public readonly RichTextBox buffer;

        public UIRichText() { buffer = new RichTextBox(); }
        public UIRichText(RichTextBox buffer) { this.buffer = buffer; }

        public void ClearText()
        {
            buffer.Text = "";
        }

        public void AddText(string text)
        {
            buffer.AppendText(text);
        }

        public void AddText(string text, Font font)
        {
            buffer.SelectionFont = font;
            buffer.AppendText(text);
        }

        public void AddText(string text, Color color)
        {
            buffer.SelectionColor = color;
            buffer.AppendText(text);
        }

        public void AddText(string text, Font font, Color color)
        {
            buffer.SelectionFont = font;
            buffer.SelectionColor = color;
            buffer.AppendText(text);
        }

        public void AddNameValueText(string name, string value, Font font, Color nameColor, Color valueColor)
        {
            AddText(name, font, nameColor);
            AddText(value, valueColor);
        }

        public void AddTaggedText(string text, Font font, Color color)
        {
            text = text.Replace("|n", "\n");

            int index = text.IndexOf("|c");

            if (index == -1)
                AddText(text, font, color);
            else
            {
                AddText(text.Substring(0, index), font, color);
                text = text.Remove(0, index);
                string[] coloredStrings = text.Split(new string[] { "|c", "|r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in coloredStrings)
                {
                    int colorArgb;
                    if ((str.Length > 8) && int.TryParse(str.Substring(0, 8), System.Globalization.NumberStyles.AllowHexSpecifier, DBDOUBLE.provider, out colorArgb))
                    {
                        string colored_text = str.Remove(0, 8);
                        AddText(colored_text, font, Color.FromArgb(colorArgb));
                    }
                    else
                        AddText(str, font, color);
                }
            }
        }

        public void AddLineInterval()
        {
            buffer.SelectionFont = UIFonts.smallArial;
            buffer.AppendText("\n \n");
            buffer.SelectionFont = UIFonts.boldArial8;
        }

        public void PasteImage(Bitmap img)
        {
            // Copy the bitmap to the clipboard.

            Clipboard.SetDataObject(img);

            // Get the format for the object type.

            DataFormats.Format bmpFormat = DataFormats.GetFormat(DataFormats.Bitmap);

            // After verifying that the data can be pasted, paste

            if (buffer.CanPaste(bmpFormat))
            {
                buffer.Paste(bmpFormat);
            }
        }

        public static Size MeasureString(Graphics g, string text, Font fnt)
        {
            Size gdiSize = new Size(0, 0);

            // get hdc
            IntPtr hdc = g.GetHdc();

            // set new font and save old font
            IntPtr hOldFont = SelectObject(hdc, fnt.ToHfont());

            // measure string width
            GetTextExtentPoint32(hdc, text, text.Length, ref gdiSize);

            // restore old font
            SelectObject(hdc, hOldFont);

            // release hdc
            g.ReleaseHdc(hdc);

            return gdiSize;
        }

        [DllImport("gdi32", EntryPoint = "GetTextExtentPoint32A")]
        static extern int GetTextExtentPoint32(IntPtr hDC, string lpsz, int cbString, ref Size lpSize);

        [DllImport("gdi32")]
        static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }

    public class UIRichTextEx
    {
        public static readonly UIRichTextEx Default = new UIRichTextEx();

        Font font = null;
        readonly Net.Sgoliver.NRtfTree.Util.RtfDocument rtfDoc;
        Net.Sgoliver.NRtfTree.Util.RtfTextFormat format = new Net.Sgoliver.NRtfTree.Util.RtfTextFormat();

        public UIRichTextEx() { rtfDoc = new Net.Sgoliver.NRtfTree.Util.RtfDocument(null, Encoding.UTF8); }
        public UIRichTextEx(Net.Sgoliver.NRtfTree.Util.RtfDocument rtfDoc) { this.rtfDoc = rtfDoc; }

        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;

                format.font = font.Name;
                format.bold = font.Bold;
                format.italic = font.Italic;
                format.size = (int)font.Size;
                format.underline = font.Underline;
            }
        }

        public Color Color
        {
            get { return format.color; }
            set { format.color = value; }
        }

        public bool IsEmpty
        {
            get
            {
                return rtfDoc.IsEmpty;
            }
        }

        public string CloseRtf()
        {
            rtfDoc.Close(false);
            return rtfDoc.Rtf;
        }

        public void ClearText()
        {
            rtfDoc.Clear();
        }

        public void AddText(string text)
        {
            rtfDoc.AddText(text);
        }

        public void AddText(string text, Font font)
        {
            this.Font = font;
            rtfDoc.AddText(text, format);
        }

        public void AddText(string text, Color color)
        {
            this.Color = color;
            rtfDoc.AddText(text, format);
        }

        public void AddText(string text, Font font, Color color)
        {
            this.Font = font;
            this.Color = color;
            rtfDoc.AddText(text, format);
        }

        public void AddNameValueText(string name, string value, Font font, Color nameColor, Color valueColor)
        {
            AddText(name, font, nameColor);
            AddText(value, valueColor);
        }

        public void AddTaggedText(string text, Font font, Color color)
        {
            text = text.Replace("|n", "\n");

            int index = text.IndexOf("|c");

            if (index == -1)
                AddText(text, font, color);
            else
            {
                AddText(text.Substring(0, index), font, color);
                text = text.Remove(0, index);
                string[] coloredStrings = text.Split(new string[] { "|c", "|r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in coloredStrings)
                {
                    int colorArgb;
                    if ((str.Length > 8) && int.TryParse(str.Substring(0, 8), System.Globalization.NumberStyles.AllowHexSpecifier, DBDOUBLE.provider, out colorArgb))
                    {
                        string colored_text = str.Remove(0, 8);
                        AddText(colored_text, font, Color.FromArgb(colorArgb));
                    }
                    else
                        AddText(str, font, color);
                }
            }
        }

        public void AddLineInterval()
        {
            this.Font = UIFonts.smallArial;
            rtfDoc.AddText("\n \n", format);
            this.Font = UIFonts.boldArial8;
        }

        public void AddNewLine()
        {
            rtfDoc.AddNewLine();
        }
    }

    public class UIStrings
    {
        public static string Warning = "DotA H.I.T. Warning";
    }

    public class UIRenderers
    {
        private static Core.Resources.NoBorderRenderer noBorderRenderer;
        public static Core.Resources.NoBorderRenderer NoBorderRenderer
        {
            get
            {
                if (noBorderRenderer == null)
                    noBorderRenderer = new Core.Resources.NoBorderRenderer();
                return noBorderRenderer;
            }
        }
    }

    public class UIDialogs
    {
        public static bool UseStandardDialogs = true;

        public class OpenFileDialogBoxWrapper
        {
            public static readonly OpenFileDialogBoxWrapper Default = new OpenFileDialogBoxWrapper();

            public OpenFileDialog openFileDialog;
            private FileBrowserForm m_fileBrowserForm;
            public FileBrowserForm fileBrowserForm
            {
                get
                {
                    if (m_fileBrowserForm == null)
                        m_fileBrowserForm = new FileBrowserForm();
                    return m_fileBrowserForm;
                }
            }

            public string InitialDirectory
            {
                get
                {
                    return UIDialogs.UseStandardDialogs ? openFileDialog.InitialDirectory : fileBrowserForm.InitialDirectory;
                }
                set
                {
                    if (UIDialogs.UseStandardDialogs)
                        openFileDialog.InitialDirectory = value;
                    else
                        fileBrowserForm.InitialDirectory = value;
                }
            }
            public string FileName
            {
                get
                {
                    return UIDialogs.UseStandardDialogs ? openFileDialog.FileName : fileBrowserForm.FileName;
                }
                set
                {
                    if (UIDialogs.UseStandardDialogs)
                        openFileDialog.FileName = value;
                    else
                        fileBrowserForm.FileName = value;
                }
            }
            public string Filter
            {
                get
                {
                    return UIDialogs.UseStandardDialogs ? openFileDialog.Filter : fileBrowserForm.Filter;
                }
                set
                {
                    if (UIDialogs.UseStandardDialogs)
                        openFileDialog.Filter = value;
                    else
                        fileBrowserForm.Filter = value;
                }
            }

            public DialogResult ShowDialog()
            {
                if (UIDialogs.UseStandardDialogs)
                    return openFileDialog.ShowDialog();
                else
                    return fileBrowserForm.ShowDialog();
            }
        }
        public class FolderBrowserDialogBoxWrapper
        {
            public static readonly FolderBrowserDialogBoxWrapper Default = new FolderBrowserDialogBoxWrapper();

            public FolderBrowserDialog folderBrowsingDialog = null;
            private FolderBrowserForm m_folderBrowserForm;
            public FolderBrowserForm folderBrowserForm
            {
                get
                {
                    if (m_folderBrowserForm == null)
                        m_folderBrowserForm = new FolderBrowserForm();
                    return m_folderBrowserForm;
                }
            }

            public string SelectedPath
            {
                get
                {
                    return UIDialogs.UseStandardDialogs ? folderBrowsingDialog.SelectedPath : folderBrowserForm.SelectedPath;
                }
            }

            public DialogResult ShowDialog()
            {
                if (UIDialogs.UseStandardDialogs)
                    return folderBrowsingDialog.ShowDialog();
                else
                    return folderBrowserForm.ShowDialog();
            }

            private delegate DialogResult showDialogDelegate(Control controlForInvoke);
            public DialogResult ShowDialog(Control controlForInvoke)
            {
                if (controlForInvoke.InvokeRequired)
                    return (DialogResult)controlForInvoke.Invoke((showDialogDelegate)ShowDialog, controlForInvoke);

                if (UIDialogs.UseStandardDialogs)
                    return folderBrowsingDialog.ShowDialog();
                else
                    return folderBrowserForm.ShowDialog();
            }
        }
    }

    public class UICheckedListBox
    {
        public static void SetItemsChecked(CheckedListBox clb, bool state)
        {
            for (int i = 0; i < clb.Items.Count; i++)
                clb.SetItemChecked(i, state);
        }
    }

    public enum Theme
    {
        None = 0,
        NightElf = 1,
        Human = 2,
        Orc = 3,
        Undead = 4
    }

    public class Current
    {
        public static bool sessionHasWar3DB = false;
        public static bool sessionAllowsSounds = true;
        public static MainForm mainForm = null;
        public static player player = null;
        public static unit unit = null;
        public static Core.DHMpqArchive map = null;
        public static string filename = null;
        public static Theme theme = Theme.None;
        public static Core.HabPropertiesCollection plugins = new Core.HabPropertiesCollection();
    }
}
