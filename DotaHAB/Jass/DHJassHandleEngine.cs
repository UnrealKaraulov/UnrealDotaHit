using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DotaHIT.Jass
{
    using Types;
    using Operations;
    using Native.Types;
    using DotaHIT.Core;
    using DotaHIT.Core.Resources;
    using Native.Events;
    using Native.Constants;
    using System.Collections.Specialized;

    public class DHJassHandleEngine
    {
        static int handleCounter = 0;
        static Dictionary<int, handlevalue> HandleValues = new Dictionary<int, handlevalue>();

        static DHJassHandleEngine()
        {
            AddNewHandle(null); // 0-th element will point to 'null'
        }

        public static IEnumerable<handlevalue> Values
        {
            get { return HandleValues.Values; }
        }

        public static void Reset()
        {
            do
            {
                List<handlevalue> hvList = new List<handlevalue>(HandleValues.Values);

                foreach (handlevalue hv in hvList)
                    if (hv != null) hv.destroy();
            }
            while (HandleValues.Count > 1);

            handleCounter = 0;
            HandleValues.Clear();
            AddNewHandle(null); // 0-th element will point to 'null'
        }

        public static int AddNewHandle(handlevalue value)
        {
            lock((HandleValues as ICollection).SyncRoot)
                HandleValues.Add(handleCounter, value);

            return handleCounter++;
        }
        public static bool RemoveHandle(int handle)
        {
            lock ((HandleValues as ICollection).SyncRoot)            
                return HandleValues.Remove(handle);            
        }

        public static bool TryGetValue(int handle, out handlevalue value)
        {
            return HandleValues.TryGetValue(handle, out value);
        }
    }
}
