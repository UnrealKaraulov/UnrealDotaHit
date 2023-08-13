using System;
using System.Collections.Generic;
using System.Text;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Extras;
using System.Diagnostics;

namespace Deerchao.War3Share.W3gParser
{
    [DebuggerTypeProxy(typeof(Inventory.InventoryDebugView))]
    public class Inventory
    {
        public byte playerId = 0xFF;
        List<string> slots = new List<string>(7);   // 6 + 1 pickup    
        private readonly ReplayMapCache cache;

        public Inventory(ReplayMapCache cache)
        {
            this.cache = cache;            
        }        

        public List<string> Slots
        {
            get
            {
                return slots;
            }
        }        

        public bool PutItem(string itemID)
        {            
            // if this item stacks with itself
            if (cache.StackableItems.ContainsKey(itemID))
            {
                // try to find same item in the inventory
                foreach (string item in slots)
                    if (string.Equals(item, itemID, StringComparison.OrdinalIgnoreCase))
                        // item found. no action required
                        return true;
            }

            // try to combine this item first
            slots.Add(itemID);

            // no need to check for complex items if inventory was empty
            if (slots.Count == 1) return true;

            // buffer used to make sure every required item is present in the inventory
            List<string> items = new List<string>(slots.Count);

            if (cache.hpcComplexItems != null)
                foreach (HabProperties hps in cache.hpcComplexItems)
                {
                    foreach (List<string> components in hps.Values)
                    {
                        // reload items 
                        items.Clear(); items.AddRange(slots);

                        int index = -1;
                        foreach (string component in components)
                        {
                            index = -1;
                            for (int i = 0; i < items.Count; i++)
                                if (string.Equals(items[i], component, StringComparison.OrdinalIgnoreCase))
                                {
                                    index = i;
                                    break;
                                }

                            // hide found item, so that next search will not use it
                            if (index != -1)
                                items[index] = null;
                            else
                                break;
                        }

                        // if one of the components wasnt found
                        if (index == -1)
                            // try next components
                            continue;
                        else
                        {
                            // otherwise all components were found
                            slots.Clear();

                            // items that were not hidden are to be preserved in the inventory
                            foreach (string item in items)
                                if (item != null)
                                    slots.Add(item);

                            // now add the complex item
                            // (and check if it combines with other items)
                            return PutItem(hps.name);
                        }
                    }
                }

            // this item does not combine with items in the inventory

            // check if inventory was full before this item
            if (slots.Count > 6)
            {
                // cancel this item
                slots.RemoveAt(slots.Count - 1);

                //Console.WriteLine("Couldn't pickup item: " + itemID);

                // item was not added
                return false;
            }

            // item was added
            return true;
        }

        public void PutItem(string itemID, int slot)
        {
            slots[slot] = itemID;
        }       

        public void DropItem(string itemID)
        {
            if (slots.Remove(itemID) == false)
            {
                // bugfix for Bottle (6.64)
                if (itemID == "I0AP")
                {
                    if (slots.Remove("I0AV")) return;

                    for (int i = 0; i < slots.Count; i++)                    
                        if (cache.hpcItemProfiles.GetStringValue(slots[i], "Art").Contains("Bottle"))
                        {
                            slots.RemoveAt(i);
                            return;
                        }                    
                }

                // bugfix for Kelen's Dagger (6.64)
                if (itemID == "I04I")
                {
                    if (slots.Remove("I04H")) return;
                }

                // bugfix for Poorman's shield (6.64)
                if (itemID == "I0KF")
                {
                    if (slots.Remove("I0JF")) return;
                }

                // bugfix for Aghanim's scepter (6.64)
                if (cache.hpcItemProfiles.GetStringValue(itemID, "Name").Contains("Aghanim"))
                {
                    if (slots.Remove("I0AY")) return;
                }

                Console.WriteLine("Couldn't drop item: " + itemID);
            }            
        }      
        
        public void RefreshOwner(Player player)
        {
            if (this.playerId == player.Id) return;            

            this.playerId = player.Id;
        }

        public override string ToString()
        {
            string output = "";
            foreach(string item in slots)
                output += item + ",";

            return output.TrimEnd(',');
        }

        internal class InventoryDebugView
        {
            internal class ItemDebugView
            {
                private string itemString;
                public ItemDebugView(string itemString)
                {
                    this.itemString = itemString;
                }

                public override string ToString()
                {
                    return itemString;
                }
            }
            private Inventory _inventory;
            public InventoryDebugView(Inventory inventory)
            {
                _inventory = inventory;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ItemDebugView[] Items
            {
                get
                {
                    ItemDebugView[] items = new ItemDebugView[_inventory.slots.Count];

                    for (int i = 0; i < items.Length; i++)
                    {
                        string itemID = _inventory.slots[i];
                        items[i] = new ItemDebugView("[" + itemID + "]" + _inventory.cache.hpcItemProfiles.GetStringValue(itemID, "Name").Trim('"'));
                    }

                    return items;
                }
            }
        }
    }
}
