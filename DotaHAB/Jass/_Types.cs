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
using DotaHIT.Jass.Native.Types;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Constants;
using System.Collections.Specialized;

namespace DotaHIT.Jass.Types
{
    using System.Globalization;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using Native.Events;
    using Jass.Commands;

    public class DbJassTypeValueKnowledge
    {
        public static Dictionary<Type, DHJassValue> TypeValuePairs = new Dictionary<Type, DHJassValue>();
        public static Dictionary<string, DHJassValue> TypenameValuePairs = new Dictionary<string, DHJassValue>();

        static DbJassTypeValueKnowledge()
        {
            TypeValuePairs = CollectTypeValuePairs();
        }
        public static void WakeUp() { }

        public static Dictionary<Type, DHJassValue> CollectTypeValuePairs()
        {
            Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

            Type[] types = m.FindTypes(new TypeFilter(SearchCriteria), "DotaHIT.Jass.Types");

            Dictionary<Type, DHJassValue> typeValuePairs = new Dictionary<Type, DHJassValue>();

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    DHJassValue value = (DHJassValue)types[i].InvokeMember(null,
                                BindingFlags.Public | BindingFlags.DeclaredOnly |
                                BindingFlags.Instance | BindingFlags.CreateInstance,
                                null, null, null);

                    typeValuePairs.Add(types[i], value);

                    if (value.Syntax == null) continue;

                    if (value.Syntax is string)
                        TypenameValuePairs.Add(value.Syntax as string, value);
                    else
                        if (value.Syntax is Dictionary<string, string>)
                            foreach (string key in (value.Syntax as Dictionary<string, string>).Keys)
                                TypenameValuePairs.Add(key, value);
                }
                catch
                {
                }
            }

            return typeValuePairs;
        }
        private static bool SearchCriteria(Type t, object filter)
        {
            if ((t.Namespace == (string)filter) && t.IsPublic
                && t.IsSubclassOf(typeof(DHJassValue))
                && !t.IsAbstract)
                return true;
            return false;
        }

        public static DHJassValue CreateKnownValue(string code)
        {
            if (code == "nothing") return null;

            DHJassValue value;
            List<string> args;

            if (DHJassValue.TryParseDeclaration(code, out value, out args))
                return value.GetNew(args);

            return null;
        }
    }

    public abstract class DHJassValue
    {
        public string Name = null;

        public DHJassValue() { }
        public DHJassValue(string name)
        {
            this.Name = name;
        }

        public abstract object GetValue();
        public abstract void SetValue(string code);
        public virtual void SetValue(DHJassValue value)
        {
            this.Value = value.Value;
        }

        public virtual object Value
        {
            get { return null; }
            set { }
        }

        public abstract object Syntax
        {
            get;
        }

        public virtual bool TryParseDirect(string code, out object value)
        {
            value = null;
            return false;
        }

        // @"\A\s*(constant\s+)?" + typename + @"(\s+(?<name>\b(?!array)\w+)\s*(=\s*(?<value>.+))?|\z)";
        // @"\A\s*(?<element_type>\w+)\s+array\s+(?<name>\w+)(=(?<value>\w+))?"
        public static bool TryParseDeclaration(string code, out DHJassValue value, out List<string> args)
        {
            if (code.Length < 4)
            {
                value = null;
                args = null;
                return false;
            }

            string constantStr = "constant"; // should be interned
            string arrayStr = "array";

            bool isConstant = false;

            unsafe
            {
                fixed (char* pCode = code, ptrCONSTANT = constantStr, ptrARRAY = arrayStr)
                {
                    char* ptr = pCode;

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    // "constant" keyword could be here
                    if (DHJassSyntax.checkStringsEqual(ptr, ptrCONSTANT, constantStr.Length))
                    {
                        isConstant = true;

                        // skip the "constant" bytes
                        ptr += constantStr.Length;

                        // skip whitespaces
                        DHJassSyntax.skipWhiteSpacesFast(ref ptr);
                    }

                    char* pStart = ptr;

                    // typename should be here
                    DHJassSyntax.skipNonWhiteSpacesFast(ref ptr);
                    string typeName = new string(pStart, 0, (int)(ptr - pStart));

                    if (!DbJassTypeValueKnowledge.TypenameValuePairs.TryGetValue(typeName, out value))
                    {
                        args = null;
                        return false;
                    }

                    args = new List<string>(2);

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    // check for "array" keyword
                    if (!isConstant && DHJassSyntax.checkStringsEqual(ptr, ptrARRAY, arrayStr.Length))
                    {
                        value = DbJassTypeValueKnowledge.TypeValuePairs[typeof(DHJassArray)];

                        // skip the "array" bytes
                        ptr += arrayStr.Length;

                        // skip whitespaces
                        DHJassSyntax.skipWhiteSpacesFast(ref ptr);
                    }

                    pStart = ptr;

                    // reach end of the variable name
                    DHJassSyntax.reachEndOfVariableNameSyntaxFast(ref ptr);

                    // add variable name
                    args.Add(new string(pStart, 0, (int)(ptr - pStart)));

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    if (*ptr != '=') return true;

                    ptr++;

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    // add variable's initial value
                    args.Add(new string(ptr));

                    return true;
                }
            }
        }

        public static DHJassValue CreateValueFromCode(string code)
        {
            DHJassValue result;
            DHJassExecutor.TryGetValue(code, out result);
            return result;
        }
        public abstract DHJassValue GetNew();
        public abstract DHJassValue GetNew(List<string> args);

        public virtual int IntValue
        {
            get
            {
                return 0;
            }
        }
        public virtual double RealValue
        {
            get
            {
                return 0.0;
            }
        }
        public virtual string StringValue
        {
            get
            {
                return this.Value + "";
            }
        }
        public virtual bool BoolValue
        {
            get
            {
                return false;
            }
        }
        public virtual handlevalue HandleValue
        {
            get
            {
                return null;
            }
        }
        public virtual DHJassFunction CodeValue
        {
            get
            {
                return null;
            }
        }

        public virtual DHJassValue Add(DHJassValue value) { return null; }
        public virtual DHJassValue Subtract(DHJassValue value) { return null; }
        public virtual DHJassValue Multiply(DHJassValue value) { return null; }
        public virtual DHJassValue Divide(DHJassValue value) { return null; }
        public virtual DHJassBoolean Equals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            try
            {
                result.Value = (DHJassSyntax.Comparer.Compare(this.Value, value.Value) == 0);
            }
            catch { result.Value = false; }
            return result;
        }
        public virtual DHJassBoolean NotEquals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            try
            {
                result.Value = (DHJassSyntax.Comparer.Compare(this.Value, value.Value) != 0);
            }
            catch { result.Value = true; }
            return result;
        }
        public virtual DHJassBoolean Greater(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            try
            {
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, value.Value) > 0;
            }
            catch { result.Value = false; }
            return result;
        }
        public virtual DHJassBoolean Less(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            try
            {
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, value.Value) < 0;
            }
            catch { result.Value = false; }
            return result;
        }
        public virtual DHJassBoolean GreaterOrEqual(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();            
            result.Value = DHJassSyntax.Comparer.Compare(this.Value, value.Value) >= 0;
            return result;
        }
        public virtual DHJassBoolean LessOrEqual(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            result.Value = DHJassSyntax.Comparer.Compare(this.Value, value.Value) <= 0;
            return result;
        }

        public virtual DHJassBoolean And(DHJassValue value)
        {
            return new DHJassBoolean(null, this.BoolValue && value.BoolValue);
        }
        public virtual DHJassBoolean Or(DHJassValue value)
        {
            return new DHJassBoolean(null, this.BoolValue || value.BoolValue);
        }
        public virtual DHJassBoolean Not()
        {
            return new DHJassBoolean(null, !this.BoolValue);
        }

        public override string ToString()
        {
            return this.GetType().Name + (string.IsNullOrEmpty(this.Name) ? "" : " (" + this.Name + ")");
        }
    }

    public class DHJassInt : DHJassValue
    {
        int value;
        public DHJassInt() { }
        public DHJassInt(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassInt(string code)
        {
            TryParse(code, out value);
        }
        public DHJassInt(string name, int value)
        {
            this.Name = name;
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }
        public override void SetValue(DHJassValue value)
        {           
            this.value = value.IntValue;
        }

        public override bool TryParseDirect(string code, out object value)
        {
            Match match;

            int intValue;
            if (int.TryParse(code, out intValue))
            {
                value = intValue;
                return true;
            }
            else
                if ((match = charSyntax.Match(code)).Success)
                {
                    value = id2int(match.Groups["value"].Value);
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is int)
                    this.value = (int)value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassInt();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassInt(args);
        }

        public override int IntValue
        {
            get
            {
                return this.value;
            }
        }
        public override double RealValue
        {
            get
            {
                return (double)value;
            }
        }
        public override handlevalue HandleValue
        {
            get
            {
                handlevalue handle;
                DHJassHandleEngine.TryGetValue(this.value, out handle);
                return handle;
            }
        }
        public string IDValue
        {
            get
            {
                return int2id(this.value);
            }
        }

        public static bool TryParse(string code, out int value)
        {
            DHJassValue jValue;
            if (DHJassExecutor.TryGetValue<DHJassInt>(code, out jValue))
            {
                value = jValue.IntValue;
                return true;
            }
            value = 0;
            return false;
        }

        public override object Syntax
        {
            get { return "integer"; }
        }

        public static Regex charSyntax = new Regex(@"\A'(?<value>.{1,4})'\z");

        public override DHJassValue Add(DHJassValue value)
        {
            DHJassInt returnValue = new DHJassInt();

            returnValue.value = this.value + value.IntValue;

            return returnValue;
        }
        public override DHJassValue Subtract(DHJassValue value)
        {
            DHJassInt returnValue = new DHJassInt();

            returnValue.value = this.value - value.IntValue;

            return returnValue;
        }
        public override DHJassValue Multiply(DHJassValue value)
        {
            DHJassInt returnValue = new DHJassInt();

            if (value is DHJassReal)
                returnValue.value = (int)(this.value * value.RealValue);
            else
                returnValue.value = this.value * value.IntValue;

            return returnValue;
        }
        public override DHJassValue Divide(DHJassValue value)
        {
            DHJassInt returnValue = new DHJassInt();

            try
            {
                if (value is DHJassReal)
                    returnValue.value = (int)(this.value / value.RealValue);
                else
                    returnValue.value = this.value / value.IntValue;
            }
            catch { }

            return returnValue;
        }

        public override DHJassBoolean Less(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is double)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (int)(double)value.Value) < 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, value.Value) < 0;

            return result;
        }

        public static int id2int(string id)
        {
            unsafe
            {
                fixed (char* ptr = id)
                {
                    switch (id.Length)
                    {
                        case 1:
                            return (int)ptr[0];

                        case 4:
                            return (((int)ptr[3])) |
                                (((int)ptr[2]) << 8) |
                                (((int)ptr[1]) << 16) |
                                (((int)ptr[0]) << 24);

                        default:
                            return 0;
                    }
                }
            }
        }
        public static string int2id(int intValue)
        {
            unsafe
            {
                // reverse byte order: 1234 -> 4321

                intValue =
                    ((int)(intValue & 0xFF000000) >> 24) |
                    ((int)(intValue & 0x00FF0000) >> 8) |
                    ((int)(intValue & 0x0000FF00) << 8) |
                    ((int)(intValue & 0x000000FF) << 24);

                return new string((sbyte*)(int*)&intValue, 0, 4);
            }
        }
        public static string int2id(uint intValue)
        {
            unsafe
            {
                // reverse byte order: 1234 -> 4321

                intValue =
                    ((uint)(intValue & 0xFF000000) >> 24) |
                    ((uint)(intValue & 0x00FF0000) >> 8) |
                    ((uint)(intValue & 0x0000FF00) << 8) |
                    ((uint)(intValue & 0x000000FF) << 24);

                return new string((sbyte*)(uint*)&intValue, 0, 4);
            }
        }

        public override string ToString()
        {
            return base.ToString() + ": " + this.value + " {" + IDValue + "}";
        }
    }
    public class DHJassReal : DHJassValue
    {
        double value;
        public DHJassReal() { }
        public DHJassReal(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassReal(string code)
        {
            TryParse(code, out value);
        }
        public DHJassReal(string name, double value)
        {
            this.Name = name;
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }
        public override void SetValue(DHJassValue value)
        {
            this.value = value.RealValue;
        }

        public override bool TryParseDirect(string code, out object value)
        {
            double dblValue;
            if (double.TryParse(code, NumberStyles.Float, provider, out dblValue))
            {
                value = dblValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is double)
                    this.value = (double)value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassReal();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassReal(args);
        }

        public override int IntValue
        {
            get
            {
                return (int)this.value;
            }
        }

        public override double RealValue
        {
            get
            {
                return value;
            }
        }

        public static bool TryParse(string code, out double value)
        {
            DHJassValue jValue;
            if (DHJassExecutor.TryGetValue<DHJassReal>(code, out jValue))
            {
                value = jValue.RealValue;
                return true;
            }
            value = 0;
            return false;
        }

        public static NumberFormatInfo provider = new NumberFormatInfo();

        public override object Syntax
        {
            get { return "real"; }
        }

        public override DHJassValue Add(DHJassValue value)
        {
            DHJassReal returnValue = new DHJassReal();

            returnValue.value = this.value + value.RealValue;

            return returnValue;
        }

        public override DHJassValue Subtract(DHJassValue value)
        {
            DHJassReal returnValue = new DHJassReal();

            returnValue.value = this.value - value.RealValue;

            return returnValue;
        }

        public override DHJassValue Multiply(DHJassValue value)
        {
            DHJassReal returnValue = new DHJassReal();

            returnValue.value = this.value * value.RealValue;

            return returnValue;
        }

        public override DHJassValue Divide(DHJassValue value)
        {
            DHJassReal returnValue = new DHJassReal();

            returnValue.value = this.value / value.RealValue;

            return returnValue;
        }        

        public override DHJassBoolean Greater(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is int)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (double)(int)obj) > 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, obj) > 0;
            return result;
        }

        public override DHJassBoolean GreaterOrEqual(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is int)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (double)(int)obj) >= 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, obj) >= 0;
            return result;
        }

        public override DHJassBoolean Less(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is int)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (double)(int)obj) < 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, obj) < 0;
            return result;
        }

        public override DHJassBoolean LessOrEqual(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is int)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (double)(int)obj) <= 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, obj) <= 0;
            return result;
        }

        public override DHJassBoolean Equals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is int)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (double)(int)obj) == 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, obj) == 0;           
            return result;
        }

        public override DHJassBoolean NotEquals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj is int)
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, (double)(int)obj) != 0;
            else
                result.Value = DHJassSyntax.Comparer.Compare(this.Value, obj) != 0;
            return result;
        }
    }
    public class DHJassBoolean : DHJassValue
    {
        bool value;
        public DHJassBoolean() { }
        public DHJassBoolean(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassBoolean(string code)
        {
            TryParse(code, out value);
        }
        public DHJassBoolean(string name, bool value)
        {
            this.Name = name;
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }
        public override void SetValue(DHJassValue value)
        {
            this.value = value.BoolValue;
        }

        public override bool TryParseDirect(string code, out object value)
        {
            bool parsedValue;
            if (bool.TryParse(code, out parsedValue))
            {
                value = parsedValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is bool)
                    this.value = (bool)value;
            }
        }

        public override int IntValue
        {
            get
            {
                return this.value ? 1 : 0;
            }
        }
        public override bool BoolValue
        {
            get
            {
                return this.value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassBoolean();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassBoolean(args);
        }

        public static bool TryParse(string code, out bool value)
        {
            DHJassValue jValue;
            if (DHJassExecutor.TryGetValue<DHJassBoolean>(code, out jValue))
            {
                value = (jValue as DHJassBoolean).value;
                return true;
            }
            value = false;
            return false;
        }

        public override object Syntax
        {
            get { return "boolean"; }
        }
    }
    public class DHJassString : DHJassValue
    {
        string value;
        public DHJassString() { }
        public DHJassString(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassString(string code)
        {
            TryParse(code, out value);
        }
        public DHJassString(string name, string value)
        {
            this.Name = name;
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }
        public override void SetValue(DHJassValue value)
        {
            this.value = value.StringValue;
        }

        public override bool TryParseDirect(string code, out object value)
        {
            Match m = valueSyntax.Match(code);
            if (m.Success)
            {
                value = m.Groups["value"].Value;
                return true;
            }
            value = null;
            return false;
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is string)
                    this.value = (string)value;
            }
        }
        public override string StringValue
        {
            get
            {
                return value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassString();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassString(args);
        }

        public static bool TryParse(string code, out string value)
        {
            DHJassValue jValue;
            if (DHJassExecutor.TryGetValue<DHJassString>(code, out jValue))
            {
                value = jValue.StringValue;
                return true;
            }
            value = null;
            return false;
        }

        public override object Syntax
        {
            get { return "string"; }
        }

        public static Regex valueSyntax = new Regex(@"\A" + "\"" + @"(?<value>[^" + "\"" + "]*)" + "\"\\z");

        public override DHJassValue Add(DHJassValue value)
        {
            return new DHJassString(null, this.value + value.StringValue);
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + "'" + this.value + "'";
        }
    }
    public class DHJassArray : DHJassValue
    {
        Dictionary<int, DHJassValue> array = new Dictionary<int, DHJassValue>();

        public DHJassArray() { }
        public DHJassArray(List<string> args)
            : base(args[0])
        {
        }

        public override object GetValue()
        {
            return null;
        }

        public override void SetValue(string value)
        {

        }
        public override void SetValue(DHJassValue value)
        {

        }

        public void SetElementValue(int index, string value)
        {
            DHJassValue result;
            if (array.TryGetValue(index, out result))
            {
                if (result != null)
                    result.SetValue(value);
                else
                    array[index] = DHJassValue.CreateValueFromCode(value);
            }
            else
                array.Add(index, DHJassValue.CreateValueFromCode(value));
        }

        public void SetElementValue(int index, DHJassValue value)
        {
            DHJassValue result;
            if (!array.TryGetValue(index, out result) || result == null
                || (result is DHJassUnusedType && !(value is DHJassUnusedType))
                || (!(result is DHJassUnusedType) && value is DHJassUnusedType))
            {
                result = value.GetNew();
                array[index] = result;
            }

            result.SetValue(value);
        }

        public DHJassValue this[int index]
        {
            get
            {
                DHJassValue result;
                if (array.TryGetValue(index, out result))
                    return result;
                else
                    return new DHJassUnusedType(); // return null
                //return array[index];
            }
            set
            {
                array[index] = value;
            }
        }
        public DHJassValue this[DHJassValue index]
        {
            get
            {
                object intIndex = index.GetValue();

                if (intIndex is int)
                    return array[(int)intIndex];
                else
                    return null;
            }
            set
            {
                object intIndex = index.GetValue();

                if (intIndex is int)
                    array[(int)intIndex] = value;
            }
        }

        public Dictionary<int, DHJassValue> Array
        {
            get
            {
                return array;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassArray();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassArray(args);
        }

        public override object Syntax
        {
            get { return "array"; }
        }
    }

    public class DHJassCode : DHJassValue
    {
        DHJassFunction value;
        public DHJassCode() { }
        public DHJassCode(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassCode(string code)
        {
            TryParse(code, out value);
        }
        public DHJassCode(string name, DHJassFunction value)
        {
            this.Name = name;
            this.value = value;
        }
        public DHJassCode(DHJassFunction value)
        {
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }
        public override void SetValue(DHJassValue value)
        {
            this.value = value.CodeValue;
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is DHJassFunction)
                    this.value = (DHJassFunction)value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassCode();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassCode(args);
        }

        public override DHJassFunction CodeValue
        {
            get
            {
                return value;
            }
        }

        public static bool TryParse(string code, out DHJassFunction value)
        {
            DHJassValue jValue;
            if (DHJassExecutor.TryGetValue<DHJassCode>(code, out jValue))
            {
                value = jValue.CodeValue;
                return true;
            }
            value = null;
            return false;
        }

        public override object Syntax
        {
            get { return "code"; }
        }
    }

    public class DHJassHandle : DHJassValue
    {
        int value;
        public DHJassHandle() { }
        public DHJassHandle(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassHandle(string code)
        {
            TryParse(code, out value);
        }
        public DHJassHandle(string name, int value)
        {
            this.Name = name;
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }
        public override void SetValue(DHJassValue value)
        {
            this.value = value.IntValue;
        }

        public override bool TryParseDirect(string code, out object value)
        {
            int intValue;
            if (int.TryParse(code, out intValue))
            {
                value = intValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is int)
                    this.value = (int)value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassHandle();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassHandle(args);
        }

        public override int IntValue
        {
            get
            {
                return this.value;
            }
        }
        public override handlevalue HandleValue
        {
            get
            {
                handlevalue handle;
                DHJassHandleEngine.TryGetValue(this.value, out handle);
                return handle;
            }
        }

        public static bool TryParse(string code, out int value)
        {
            DHJassValue jValue;
            if (DHJassExecutor.TryGetValue<DHJassInt>(code, out jValue))
            {
                value = jValue.IntValue;
                return true;
            }
            value = 0;
            return false;
        }

        public override object Syntax
        {
            get { return "handle"; }
        }

        public override DHJassValue Add(DHJassValue value)
        {
            DHJassInt returnValue = new DHJassInt();

            returnValue.Value = this.value + value.IntValue;

            return returnValue;
        }
        public override DHJassValue Subtract(DHJassValue value)
        {
            DHJassInt returnValue = new DHJassInt();

            returnValue.Value = this.value - value.IntValue;

            return returnValue;
        }        
        public override DHJassBoolean Equals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj == null)
                result.Value = (this.value == 0);
            else
                result.Value = (DHJassSyntax.Comparer.Compare(this.Value, obj) == 0);

            return result;
        }
        public override DHJassBoolean NotEquals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();

            object obj = value.Value;
            if (obj == null)
                result.Value = (this.value != 0);
            else
                result.Value = (DHJassSyntax.Comparer.Compare(this.Value, obj) != 0);

            return result;
        }
    }

    public class DHJassAnyHandle : DHJassHandle
    {
        public DHJassAnyHandle() { }
        public DHJassAnyHandle(List<string> args) : base(args) { }
        public DHJassAnyHandle(string code) : base(code) { }
        public DHJassAnyHandle(string name, int value) : base(name, value) { }

        public override DHJassValue GetNew()
        {
            return new DHJassAnyHandle();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassAnyHandle(args);
        }

        public override object Syntax
        {
            get { return DHJassSyntax.native_types; }
        }
    }

    public class DHJassUnusedType : DHJassValue
    {
        string value = null;

        public DHJassUnusedType() { }
        public DHJassUnusedType(List<string> args)
            : base(args[0])
        {
            if (args.Count > 1) TryParse(args[1], out value);
        }
        public DHJassUnusedType(string code)
        {
            TryParse(code, out value);
        }
        public DHJassUnusedType(string name, string value)
        {
            this.Name = name;
            this.value = value;
        }

        public override object GetValue()
        {
            return value;
        }
        public override void SetValue(string code)
        {
            TryParse(code, out value);
        }

        public override bool TryParseDirect(string code, out object value)
        {
            value = null;
            return false;
        }

        public override object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = (value == null) ? value as string : value + ""; //+ "";
            }
        }
        public override string StringValue
        {
            get
            {
                return value;
            }
        }

        public override DHJassValue GetNew()
        {
            return new DHJassUnusedType();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassUnusedType(args);
        }

        public static bool TryParse(string code, out string value)
        {
            value = code;
            return true;
        }

        public override object Syntax
        {
            get { return null; }
        }

        public override DHJassValue Add(DHJassValue value)
        {
            return this;
        }

        public override DHJassValue Subtract(DHJassValue value)
        {
            return this;
        }

        public override DHJassValue Multiply(DHJassValue value)
        {
            return this;
        }

        public override DHJassValue Divide(DHJassValue value)
        {
            return this;
        }

        public override DHJassBoolean Equals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            try
            {
                if (value is DHJassString)
                    result.Value = (DHJassSyntax.Comparer.Compare(this.Value, value.Value) == 0);
                else
                    result.Value = (DHJassSyntax.Comparer.Compare(this.RealValue, value.RealValue) == 0);
            }
            catch { result.Value = false; }
            return result;
        }

        public override DHJassBoolean NotEquals(DHJassValue value)
        {
            DHJassBoolean result = new DHJassBoolean();
            try
            {
                if (value is DHJassString)
                    result.Value = (DHJassSyntax.Comparer.Compare(this.Value, value.Value) != 0);
                else
                    result.Value = (DHJassSyntax.Comparer.Compare(this.RealValue, value.RealValue) != 0);
            }
            catch { result.Value = false; }
            return result;
        }
    }

    public class DHJassFunction : DHJassValue, IBodyEndSyntaxHolder
    {
        string declaration = null;
        List<string> lines;

        protected Dictionary<string, DHJassValue> args = new Dictionary<string, DHJassValue>();
        protected DHJassValue[] argsArray = null;

        public Dictionary<string, DHJassValue> Locals;// = new Dictionary<string, DHJassValue>();
        public List<DHJassLocalDeclaration> LocalDeclarations = new List<DHJassLocalDeclaration>();

        DHJassOperation entryPoint = null;

        DHJassValue returnValue = null;

        public DHJassFunction() { argsArray = new DHJassValue[] { }; }

        public DHJassFunction(List<string> declaration, List<string> lines)
            : base(declaration[0])
        {
            this.declaration = declaration[0];

            ///////////////////////
            //   parameters
            ///////////////////////                    

            argsArray = new DHJassValue[declaration.Count - 2];

            for (int i = 0; i < declaration.Count - 2; i++)
            {
                DHJassValue value = DbJassTypeValueKnowledge.CreateKnownValue(declaration[i + 2]);
                if (value != null)
                {
                    args.Add(value.Name, value);
                    argsArray[i] = value;
                }
            }
            ///////////////////////
            //   return value
            ///////////////////////
            //string return_value_declaration = declaration.Groups["returns"].Value;
            returnValue = new DHJassUnusedType();//DbJassTypeValueKnowledge.CreateKnownValue(return_value_declaration);

            //////////////////////////////////////////////
            //   save body lines for compiling on demand
            //////////////////////////////////////////////
            this.lines = lines;
        }

        public DHJassValue Execute()
        {
            return Run();
        }
        public DHJassValue Execute(List<string> args)
        {
            for (int i = 0; i < argsArray.Length; i++)
                if (argsArray[i] != null)
                {
                    DHJassExecutor.traceIntoExpression();

                    argsArray[i].SetValue(args[i]);

                    if (DHJassExecutor.traceOutofExpression())
                        Console.WriteLine("Found reference in:" + declaration);
                }

            return Run();
        }
        public DHJassValue Execute(params DHJassValue[] args)
        {
            for (int i = 0; i < argsArray.Length; i++)
                if (argsArray[i] != null)
                    argsArray[i].SetValue(args[i]);

            return Run();
        }
        public DHJassValue Execute(DHJassCommand[] args)
        {
            for (int i = 0; i < argsArray.Length; i++)
                if (argsArray[i] != null)
                    argsArray[i].SetValue(args[i].GetResult());

            return Run();
        }
        public void Callback(object sender, DHJassEventArgs args)
        {
            if (DHJassExecutor.ShowTriggerActions)
                Console.WriteLine("Callback: " + this.Name);

            DHJassExecutor.TriggerStack.Push(args.args);

            this.Execute();

            DHJassExecutor.TriggerStack.Pop();
        }

        protected virtual DHJassValue Run()
        {
            if (lines != null)
            {
                CompileBody();
                lines = null;
            }

            Locals = new Dictionary<string, DHJassValue>(args.Count + LocalDeclarations.Count);

            DHJassExecutor.Stack.Push(this.Locals);

            foreach (KeyValuePair<string, DHJassValue> kvp in args)
                Locals.Add(kvp.Key, kvp.Value);


            foreach (DHJassLocalDeclaration Declaration in LocalDeclarations)
            {
                DHJassValue local = Declaration.CreateLocal();
                Locals.Add(local.Name, local);
            }

            DHJassOperation currentOperation = entryPoint;

#if DEBUG
            if (DHJassExecutor.LogExecution)
            {
                Console.WriteLine(">>>" + this.Name);
            }
#endif

            while (currentOperation != null)
                currentOperation = currentOperation.ProceedToNext(ref returnValue);

#if DEBUG
            if (DHJassExecutor.LogExecution)
            {
                Console.WriteLine("<<<" + this.Name);
            }
#endif

            DHJassExecutor.Stack.Pop();
            //Locals.Clear();                    

            return returnValue;
        }

        protected void CompileBody()
        {
            DHJassCompiler.Functions.Push(this);

            int line = 0;
            DHJassOperation exitPoint;
            if (lines.Count != 0)
                TryCreateBody(lines, ref line, this, out entryPoint, out exitPoint);

            DHJassCompiler.Functions.Pop();
        }

        public override object GetValue()
        {
            return Execute();
        }

        public override void SetValue(string value)
        {
        }
        public override void SetValue(DHJassValue value)
        {

        }

        public override DHJassValue GetNew()
        {
            return new DHJassFunction();
        }
        public override DHJassValue GetNew(List<string> args)
        {
            return new DHJassFunction();
        }

        public override object Syntax
        {
            get { return null; }
        }

        public static bool IsSyntaxMatchFast(string code, ref List<string> args)
        {
            if (code.Length < 30)
                return false;

            string constantStr = "constant"; // should be interned
            string functionStr = "function";
            string takesStr = "takes";
            string nothingStr = "nothing";
            string returnsStr = "returns";

            unsafe
            {
                fixed (char* pCode = code, ptrCONSTANT = constantStr, ptrFUNCTION = functionStr, ptrTAKES = takesStr, ptrNOTHING = nothingStr, ptrRETURNS = returnsStr)
                {
                    char* ptr = pCode;

                    // "constant" keyword could be here
                    if (DHJassSyntax.checkStringsEqual(ptr, ptrCONSTANT, constantStr.Length))
                    {
                        // skip the "constant" bytes
                        ptr += constantStr.Length;

                        // skip whitespaces
                        DHJassSyntax.skipWhiteSpacesFast(ref ptr);
                    }

                    // "function" keyword should be here
                    if (DHJassSyntax.checkStringsEqual(ptr, ptrFUNCTION, functionStr.Length))
                        ptr += functionStr.Length; // skip the "function" bytes
                    else
                        return false;

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    char* pStart = ptr;

                    // reach end of the function name
                    DHJassSyntax.skipNonWhiteSpacesFast(ref ptr);

                    // add function name
                    args.Add(new string(pStart, 0, (int)(ptr - pStart)));

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    // "takes" keyword should be here
                    if (DHJassSyntax.checkStringsEqual(ptr, ptrTAKES, takesStr.Length))
                        ptr += takesStr.Length; // skip the "takes" bytes
                    else
                        return false;

                    // skip whitespaces
                    DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                    args.Add(string.Empty); // reserve the second element for return type

                    // read parameters
                    while (true)
                    {
                        if (DHJassSyntax.checkStringsEqual(ptr, ptrNOTHING, nothingStr.Length))
                        {
                            ptr += nothingStr.Length;
                            DHJassSyntax.skipWhiteSpacesFast(ref ptr);
                            break;
                        }
                        else
                        {
                            pStart = ptr;
                            // reach end of parameter type declaration
                            DHJassSyntax.skipNonWhiteSpacesFast(ref ptr);

                            // skip whitespaces
                            DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                            // reach end of parameter name declaration
                            DHJassSyntax.skipNonWhiteSpacesEnumFast(ref ptr);

                            // add this parameter to the list
                            args.Add(new string(pStart, 0, (int)(ptr - pStart)));

                            // skip whitespaces
                            DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                            if (*ptr == ',')
                            {
                                ptr++;
                                DHJassSyntax.skipWhiteSpacesFast(ref ptr);
                                continue;
                            }
                        }

                        break;
                    }

                    if (DHJassSyntax.checkStringsEqual(ptr, ptrRETURNS, returnsStr.Length))
                    {
                        ptr += returnsStr.Length;

                        // skip whitespaces
                        DHJassSyntax.skipWhiteSpacesFast(ref ptr);

                        // read the remaining part of the string as the return type
                        args[1] = new string(ptr);

                        return true;
                    }

                    return false;
                }
            }
        }

        bool IBodyEndSyntaxHolder.CheckBodyEndSyntax(string code)
        {
            return false;
        }

        public override string ToString()
        {
            if (declaration != null)
                return declaration;
            else
                return "";
        }

        public static bool TryCreateBody(List<string> lines, ref int i, IBodyEndSyntaxHolder besh, out DHJassOperation EntryPoint, out DHJassOperation ExitPoint)
        {
            DHJassOperation LastExitPoint;
            DHJassOperation NextEntryPoint;


            // check if this line is already the end of the body
            if (besh.CheckBodyEndSyntax(lines[i]))
            {
                EntryPoint = new DHJassEmptyOperation();
                ExitPoint = EntryPoint;
                return true;
            }

            // create operation based on the specified line
            if (DHJassOperation.TryCreate(lines, ref i, out EntryPoint, out ExitPoint))
            {
                // update the pointer to the body's last operation
                LastExitPoint = ExitPoint;

                // now start creating the rest of the operations until the body end syntax is met
                for (i++; i < lines.Count; i++)
                {
                    // check if this line is the end of the body
                    if (besh.CheckBodyEndSyntax(lines[i]))
                        return true;

                    // create operation based on the specified line
                    DHJassOperation.TryCreate(lines, ref i, out NextEntryPoint, out ExitPoint);

                    // link previous operation's body exit point with this operation's body entry point
                    LastExitPoint.SetNext(NextEntryPoint);

                    // the pointer to the body's last operation will now point to
                    // the exit point of this operation
                    LastExitPoint = ExitPoint;
                }
            }
            else
                return false;

            return true;
        }
    }
}
