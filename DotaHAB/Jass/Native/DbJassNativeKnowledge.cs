using System;
using System.Collections.Generic;
using System.Text;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;
using DotaHIT.Jass.Commands;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Functions;
using DotaHIT.Jass.Native.Constants;
using System.Reflection;

namespace DotaHIT.Jass.Native
{
    public class DbJassNativeKnowledge
    {
        public static Dictionary<string, DHJassFunction> NameFunctionPairs = new Dictionary<string, DHJassFunction>();

        static DbJassNativeKnowledge()
        {
            NameFunctionPairs = CollectNameFunctionPairs();
        }
        public static void WakeUp() { }

        public static Dictionary<string, DHJassFunction> CollectNameFunctionPairs()
        {
            Module m = Assembly.GetExecutingAssembly().GetModules(false)[0];

            Type[] types = m.FindTypes(new TypeFilter(SearchCriteria), "DotaHIT.Jass.Native.Functions");

            Dictionary<string, DHJassFunction> nameFunctionPairs = new Dictionary<string, DHJassFunction>();

            for (int i = 0; i < types.Length; i++)
            {
                try
                {
                    DHJassFunction value = (DHJassFunction)types[i].InvokeMember(null,
                                BindingFlags.Public | BindingFlags.DeclaredOnly |
                                BindingFlags.Instance | BindingFlags.CreateInstance,
                                null, null, null);

                    nameFunctionPairs.Add(types[i].Name, value);
                    value.Name = types[i].Name;
                }
                catch
                {
                }
            }

            return nameFunctionPairs;
        }
        private static bool SearchCriteria(Type t, object filter)
        {
            if ((t.Namespace == (string)filter) && t.IsPublic
                && t.IsSubclassOf(typeof(DHJassNativeFunction))
                && !t.IsAbstract)
                return true;
            return false;
        }
    }
}
