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

    namespace Format
    {
        using System.Text;
        using System.Collections.Generic;
        using Core;

        public class DHFormatter
        {
            static DBCHAR stringFormatter;
            static DBBIT boolFormatter;
            static DBINT intFormatter;

            static DHFormatter()
            {
                RefreshFormatForCurrentThread();

                stringFormatter = new DBCHAR();
                stringFormatter.Attributes.Add(new TrimAttribute('"'));

                boolFormatter = new DBBIT();
                intFormatter = new DBINT();
            }
            public static void WakeUp() { }
            public static void RefreshFormatForCurrentThread()
            {                
                CultureInfo ci = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
                ci.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = ci;
            }
            public static string ToString(object value)
            {
                bool isNull;
                return (string)stringFormatter.ToValue(value, out isNull);
            }
            public static string ToString(TimeSpan ts)
            {
                return ((int)ts.TotalMinutes).ToString("00", DBDOUBLE.provider) + ":" + ts.Seconds.ToString("00", DBDOUBLE.provider);
            }
            public static string ToStringUTF8(object value)
            {
                bool isNull;
                string result = (string)stringFormatter.ToValue(value, out isNull);                
                result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(result));
                return result;
            }
            public static string ToString(object value, params object[] args)
            {
                bool isNull;
                string result = (string)stringFormatter.ToValue(value, out isNull);
                result = result.Replace("%d", "{0:D}");                
                return string.Format(result, args);
            }
            public static bool ToBool(object value)
            {
                bool isNull;
                return (byte)boolFormatter.ToValue(value, out isNull) != 0;
            }
            public static int ToInt(object value)
            {
                bool isNull;
                return (int)intFormatter.ToValue(value, out isNull);
            }
            public static string ToStringList(object value)
            {
                if (value is string)
                    return value as string;
                else
                    if (value is IEnumerable<string>)
                    {
                        string result = "";
                        foreach (string s in value as IEnumerable<string>)
                            if (string.IsNullOrEmpty(s))
                                result += ",";
                            else
                                if (s.StartsWith("\""))
                                    result += s + ",";
                                else
                                    result += (s.Contains(",") ? "\"" + s + "\"" : s) + ",";

                        return result.TrimEnd(',');
                    }
                    else
                        if (value is HabPropertiesCollection)
                            return (value as HabPropertiesCollection).ToString(true);
                        else
                            return value + "";
            }
        }
    }
}