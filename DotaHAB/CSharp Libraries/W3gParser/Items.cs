using System.Collections.Generic;

namespace Deerchao.War3Share.W3gParser
{
    public class Items
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();
        readonly List<OrderItem> buildOrders = new List<OrderItem>();

        internal void Order(OrderItem item)
        {
            if (items.ContainsKey(item.Name))
                item.Count = ++items[item.Name];
            else
            {
                item.Count = 1;
                items.Add(item.Name, item.Count);
            }
            buildOrders.Add(item);            
        }

        internal void Cancel(OrderItem item)
        {
            if (items.ContainsKey(item.Name))
            {
                items[item.Name]--;
                buildOrders.Add(item);
            }
        }

        public IEnumerable<string> Names
        {
            get
            {
                foreach (string name in items.Keys)
                {
                    yield return name;
                }
            }
        }

        public int this[string name]
        {
            get
            {
                if (items.ContainsKey(name))
                    return items[name];
                return 0;
            }
        }

        public List<OrderItem> BuildOrders
        {
            get { return buildOrders; }
        }
    }
}