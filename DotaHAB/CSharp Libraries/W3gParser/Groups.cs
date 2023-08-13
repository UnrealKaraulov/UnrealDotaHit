using System.Collections.Generic;

namespace Deerchao.War3Share.W3gParser
{
    internal class Groups
    {
        private readonly Dictionary<byte, List<int>> groups = new Dictionary<byte, List<int>>();

        public void SetGroup(byte groupNo, List<int> units)
        {
            if (groups.ContainsKey(groupNo))
                groups[groupNo] = units;
            else
                groups.Add(groupNo, units);
        }

        public List<int> this[byte groupNo]
        {
            get
            {
                List<int> list;
                if (!groups.TryGetValue(groupNo, out list))
                    list = new List<int>();

                return list;
            }
        }
    }
}