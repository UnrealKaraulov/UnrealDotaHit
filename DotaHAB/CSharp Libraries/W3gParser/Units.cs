using System.Collections.Generic;

namespace Deerchao.War3Share.W3gParser
{
    public class Units
    {
        private readonly Dictionary<string, short> units = new Dictionary<string, short>();
        readonly List<OrderItem> buildOrders = new List<OrderItem>();

        private short multiplier = 1;

        internal short Multiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        internal void Order(OrderItem item)
        {
            if (units.ContainsKey(item.Name))
                units[item.Name] += multiplier;
            else
                units.Add(item.Name, multiplier);

            buildOrders.Add(item);
        }

        internal void Cancel(OrderItem item)
        {
            if (units.ContainsKey(item.Name))
            {
                units[item.Name] -= multiplier;
                buildOrders.Add(item);
            }
        }

        public IEnumerable<string> Names
        {
            get
            {
                return units.Keys;
            }
        }

        public int this[string name]
        {
            get
            {
                if (units.ContainsKey(name))
                    return units[name];
                return 0;
            }
        }

        public List<OrderItem> BuildOrders
        {
            get
            {
                return buildOrders;
            }
        }

    }
}