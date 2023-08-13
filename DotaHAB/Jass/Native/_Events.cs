using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DotaHIT.Jass.Operations;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;
using DotaHIT.Jass.Commands;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Constants;
using System.Collections.Specialized;
using System.Collections;

namespace DotaHIT.Jass.Native.Events
{
    public delegate void DHJassEventHandler(object sender, DHJassEventArgs e);
    public class DHJassEventArgs : EventArgs
    {
        public Dictionary<string, object> args;
        public DHJassEventArgs()
        {
            args = new Dictionary<string, object>();
        }
        public DHJassEventArgs(params KeyValuePair<string, object>[] args)
        {
            this.args = new Dictionary<string, object>(args.Length);
            foreach (KeyValuePair<string, object> kvp in args)
                this.args.Add(kvp.Key, kvp.Value);
        }
    }

    public class playerevent
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static playerevent()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (syntax.IsMatch(kvp.Key))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        static Regex syntax = new Regex(@"\AEVENT_PLAYER_(?!HERO|UNIT).*");
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }
    public class playerunitevent
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static playerunitevent()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (syntax.IsMatch(kvp.Key))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        public static void WakeUp() { }
        static Regex syntax = new Regex(@"\AEVENT_PLAYER_(HERO|UNIT).*");
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }
    public class playerstate
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static playerstate()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (kvp.Key.StartsWith("PLAYER_STATE_"))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }

    public class unitevent
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static unitevent()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (kvp.Key.StartsWith("EVENT_UNIT_"))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }
    public class unitstate
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static unitstate()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (kvp.Key.StartsWith("UNIT_STATE_"))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
    }

    public class gamestate
    {
        static Dictionary<int, string> ValueNamePairs = new Dictionary<int, string>();
        static gamestate()
        {
            foreach (KeyValuePair<string, DHJassValue> kvp in DHJassExecutor.War3Globals)
                if (kvp.Key.StartsWith("GAME_STATE_"))
                    ValueNamePairs[kvp.Value.IntValue] = kvp.Key;
        }
        public static void WakeUp() { }
        public static string getName(int value)
        {
            string name;
            ValueNamePairs.TryGetValue(value, out name);
            return name;
        }
        public static DHJassEventHandler statechanged;
        public static void OnStateChanged()
        {
            if (statechanged != null)
                statechanged(null, null);
        }
    }
}
