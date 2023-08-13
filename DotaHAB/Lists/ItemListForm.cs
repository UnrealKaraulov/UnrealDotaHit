using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Core;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Specialized;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;
using System.Diagnostics;

namespace DotaHIT
{
    public partial class ItemListForm : FloatingListForm
    {
        internal enum CombineMode
        {
            Normal = 0,
            Default = Normal,
            Fast = 1
        }
        internal CombineMode combineMode = CombineMode.Default;

        internal object switchButton = null;

        internal RecordCollection Items = new RecordCollection();      

        public delegate void ItemActivateEvent(object sender, IRecord item);
        public event ItemActivateEvent ItemActivate = null;
        
        ToolStripButton emptyButton = new ToolStripButton();
        
        Dictionary<string, int> ComplexItemCosts = null;
        Dictionary<string, List<HabProperties>> sameLookingComplexItems = null;

        public ItemListForm()
        {            
            leftSide = false;            

            InitializeComponent();

            base_itemsLV = this.itemsLV;
            base_listMinMaxTimer = this.listMinMaxTimer;

            base_toolTipTimer = this.toolTipTimer;

            this.baseInit();           

            CaptionButton = this.captionB;

            fullHeight = this.Height;
            minHeight = captionB.Height;
            width = this.Width;
        }

        public void Init()
        {
            this.Items = new RecordCollection();
        }

        public void Reset()
        {
            itemsLV.Clear();
            shopsTS.Items.Clear();
            captionB.Text = "Items";
        }

        internal void PrepareList()
        {
            shopsTS.Items.Clear();

            foreach (unit shop in DHLOOKUP.shops)
            {
                ToolStripButton tsb = new ToolStripButton();
                tsb.BackColor = Color.Black;//Color.Red;//Color.LightGray;
                tsb.AutoSize = false;
                tsb.Size = shopsTS.ImageScalingSize;
                tsb.Margin = Padding.Empty;
                tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;                
                tsb.Image = (Image)shop.iconImage;
                tsb.TextImageRelation = TextImageRelation.Overlay;
                tsb.Tag = shop;
                tsb.ToolTipText = shop.ID;
                tsb.Padding = new Padding(2);

                tsb.MouseEnter += new EventHandler(tsb_MouseEnter);
                tsb.MouseDown += new MouseEventHandler(tsb_MouseDown);
                tsb.MouseLeave += new EventHandler(tsb_MouseLeave);

                shopsTS.Items.Add(tsb);
            }

            DHLOOKUP.ResetItemCombiningData();
            ComplexItemCosts = null;
            sameLookingComplexItems = null;
            
            SetCombineMode((CombineMode)DHCFG.Items["Items"].GetIntValue("CombineMode"));
        }

        void tsb_MouseLeave(object sender, EventArgs e)
        {
            if (switchButton != sender)
                (sender as ToolStripItem).BackColor = Color.Black;
        }

        void tsb_MouseEnter(object sender, EventArgs e)
        {
            if (!shopsTS.Focused)
                shopsTS.Focus();
            if (switchButton != sender)
                (sender as ToolStripItem).BackColor = Color.Gray;
        }

        void tsb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SetListState(sender as ToolStripButton);
            else            
                SetListState(emptyButton);            
        }

        internal void SetListState(ToolStripButton switchButton)
        {
            if (this.switchButton == switchButton)
                return;

            this.switchButton = switchButton;

            foreach (ToolStripButton b in shopsTS.Items)
            {
                if (switchButton == b)
                    //b.Padding = new Padding(2);
                    b.BackColor = Color.White;
                else
                    b.BackColor = Color.Black;
                    //b.Padding = Padding.Empty;                    
            }
          
            SetListState(switchButton.Tag as unit);
        }

        internal void SetListState(int index)
        {
            if (shopsTS.Items.Count > 0)
                SetListState(shopsTS.Items[index] as ToolStripButton);
            else
                SetListState((unit)null);
        }

        internal void SetListState(unit shop)
        {           
            InitListByShop(shop);
        }           

        internal void InitListByShop(unit shop)
        {
            HabPropertiesCollection hpcListItems = new HabPropertiesCollection();

            if (shop == null)
                InitList(DHLOOKUP.shops);            
            else
            {
                DBSTRINGCOLLECTION itemList;
                HabPropertiesCollection hpcItemProfiles;                

                if (DHHELPER.IsNewVersionItemShop(shop))
                {
                    itemList = shop.sellunits;
                    hpcItemProfiles = DHLOOKUP.hpcUnitProfiles;
                }
                else
                {
                    itemList = shop.sellitems;
                    hpcItemProfiles = DHLOOKUP.hpcItemProfiles;
                }

                foreach (string itemID in itemList)
                {
                    HabProperties hps = hpcItemProfiles[itemID];
                    hpcListItems.Add(hps);
                }

                InitList(hpcListItems);
            }
        }

        internal void InitList(List<unit> shops)
        {
            if (bound)
                this.Width = this.width + 4;

            itemsLV.BeginUpdate();

            // init list

            itemsLV.Items.Clear();
            itemsLV.Groups.Clear();

            foreach (unit s in shops)
            {
                ListViewGroup Group = new ListViewGroup(s.codeID, s.ID);
                Group.Tag = s;

                itemsLV.Groups.Add(Group);

                DBSTRINGCOLLECTION itemList;
                HabPropertiesCollection hpcItemProfiles;

                if (DHHELPER.IsNewVersionItemShop(s))
                {
                    itemList = s.sellunits;
                    hpcItemProfiles = DHLOOKUP.hpcUnitProfiles;
                }
                else
                {
                    itemList = s.sellitems;
                    hpcItemProfiles = DHLOOKUP.hpcItemProfiles;
                }


                foreach (string itemID in itemList)
                {
                    HabProperties hpsItem = hpcItemProfiles[itemID];

                    string iconName = hpsItem.GetStringValue("Art");
                    if (String.IsNullOrEmpty(iconName)) continue;

                    ListViewItem lvi_Item = new ListViewItem();

                    lvi_Item.ImageKey = iconName;
                    lvi_Item.Tag = hpsItem;
                    lvi_Item.Group = Group;

                    itemsLV.Items.Add(lvi_Item);
                }
            }           

            itemsLV.EndUpdate();

            DisplayCaption();
        }
        internal void InitList(HabPropertiesCollection hpcListItems)
        {
            if (bound)
                this.Width = this.width;

            itemsLV.BeginUpdate();

            // init list

            itemsLV.Items.Clear();
            itemsLV.Groups.Clear();

            foreach (HabProperties hpsItem in hpcListItems.Values)
            {
                string iconName = hpsItem.GetStringValue("Art");
                if (String.IsNullOrEmpty(iconName)) continue;

                ListViewItem lvi_Item = new ListViewItem();

                lvi_Item.ImageKey = iconName;
                lvi_Item.Tag = hpsItem;

                itemsLV.Items.Add(lvi_Item);
            }

            itemsLV.EndUpdate();

            DisplayCaption();
        }        

        private void itemsLV_ItemActivate(object sender, EventArgs e)
        {
            foreach (int index in itemsLV.SelectedIndices)
            {
                ListViewItem lvItem = itemsLV.Items[index];
                HabProperties hpsItem = lvItem.Tag as HabProperties;

                unit shop = (switchButton as ToolStripButton).Tag as unit;
                if (shop == null) shop = lvItem.Group.Tag as unit;

                OnItemActivate(SellItem(shop,hpsItem));
                break;
            }
        }        

        internal item SellItem(unit shop, HabProperties hpsItem)
        {
            if (hpsItem == null || Current.unit == null) return null;

            // the map script checks if buying unit 
            // is located near the shop, so here we set
            // the hero's location to the selected shop's location
            Current.unit.set_location(shop.get_location());

            // place all other heroes away from this shop, 
            // so they would not get the item bought
            foreach (unit u in Current.player.units.Values)
                if (u != Current.unit && u.IsHero)
                    u.set_location(Jass.Native.Constants.map.maxX*2.0, Jass.Native.Constants.map.maxY*2.0); // place them out of the map

            switch (combineMode)
            {
                case CombineMode.Fast:
                    return SellItemFast(shop, hpsItem);

                default:
                    return SellItemNormal(shop, hpsItem);
            }
        }

        internal item SellItemNormal(unit shop, HabProperties hpsItem)
        {
            string ID = hpsItem.name;
            IRecord item = Items.GetByUnit("codeID", ID);

            bool isNewVersionItem = DHHELPER.IsNewVersionItem(ID);

            if (!(item is item) && !(item is unit))
            {
                item = isNewVersionItem ? (IRecord)new unit(hpsItem.name) : (IRecord)new item(hpsItem.name);
                Items.Add(item);
            }

            item = item.Clone();          

            if (isNewVersionItem)
            {
                (item as unit).DoSummon = true;
                (item as unit).set_owningPlayer(Current.player);
                
                shop.OnSell(item as unit); // sell item as unit                                
            }
            else
                shop.OnSellItem(item as item, Current.unit);
            
            // pay the gold for this item

            int goldCost = isNewVersionItem ? (item as unit).goldCost : (item as item).goldCost;
            Current.player.Gold = Current.player.Gold - goldCost;            

            return item as item;
        }

        internal item SellItemNormal(unit shop, HabProperties hpsItem, int goldCost)
        {
            string ID = hpsItem.name;
            item item = Items.GetByUnit("codeID", ID) as item;

            if (item == null)
            {
                item = new item(hpsItem.name);                
                Items.Add(item);
            }

            item = item.Clone() as item;
            item.set_owningPlayer(Current.player);
            
            shop.OnSellItem(item, Current.unit);

            // pay the gold for this item

            Current.player.Gold = Current.player.Gold - goldCost;            

            return item;
        }

        internal item SellItemFast(unit shop, HabProperties hpsItem)
        {
            List<HabProperties> hpsList = FindSameLookingComplexItem(hpsItem);            

            if (hpsList.Count == 0) return SellItemNormal(shop, hpsItem);
            else
            {
                int goldCost = GetGoldCost(hpsList[0]);
                return SellItemNormal(shop, hpsList[0], goldCost);
            }
        }

        private void OnItemActivate(IRecord e)
        {            
            if (ItemActivate != null)
                ItemActivate(null,e);
        }

        private void ItemListForm_Load(object sender, EventArgs e)
        {
            //this.SetDesktopLocation(Screen.PrimaryScreen.WorkingArea.Width,
                 //                    Screen.PrimaryScreen.WorkingArea.Height); 
        }

        private void ItemListForm_Shown(object sender, EventArgs e)
        {
            //this.Visible = false;            
        }       

        private void itemsLV_MouseMove(object sender, MouseEventArgs e)
        {
            Point cp = itemsLV.PointToClient(MousePosition);
            ListViewItem lvItem = itemsLV.GetItemAt(cp.X, cp.Y);

            if (lvItem == null)
            {
                CloseToolTip();
                return;
            }            

            if (toolTipItem != lvItem.Tag)
            {
                CloseToolTip();
                
                toolTipItem = lvItem.Tag;
                toolTipTimer.Start();
            }

            if (toolTip != null)
                toolTip.DisplayAtCursor(MousePosition);
        }

        private void itemsLV_MouseLeave(object sender, EventArgs e)
        {
            CloseToolTip();           
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            toolTip = new ItemToolTipForm(this);

            HabProperties hpsItem = toolTipItem as HabProperties;

            switch (combineMode)
            {
                case CombineMode.Fast:
                    List<HabProperties> hpsList = FindSameLookingComplexItem(hpsItem);
                    if (hpsList.Count == 0)
                        toolTip.showItemToolTip(hpsItem, false);
                    else
                        toolTip.showComplexItemToolTip(hpsItem, GetGoldCost(hpsList[0]));
                    break;

                default:
                    toolTip.showItemToolTip(hpsItem, false);
                    break;
            }            
        }

        private void itemsLV_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Image image = (Image)DHRC.Default.GetImage(e.Item.ImageKey);

            if (image != null)
                e.Graphics.DrawImage(image, e.Bounds.X, e.Bounds.Y, 48, 48);
            else
                Console.WriteLine("Couldn't get image: '" + e.Item.ImageKey + "'");
            /*HabProperties hpsX = e.Item.Tag as HabProperties;
            e.Item.Text = RecordSlotComparer.get_slot(hpsX.GetValue("Buttonpos") + "")+"";            
            e.DrawText();*/
        }

        private void all_MouseDown(object sender, MouseEventArgs e)
        {
            SetListState((unit)null);
        }        

        internal void DisplayCaption()
        {
            string caption = "Items (" + itemsLV.Items.Count + ")";

            switch (combineMode)
            {
                case CombineMode.Fast:
                    caption += " Fast";
                    break;
            }

            captionB.Text = caption;
        }

        protected override void captionButton_MouseDown(object sender, MouseEventArgs e)
        {            
            if (e.Button == MouseButtons.Right)
            {
                if (DHLOOKUP.hpcItemProfiles != null && DHLOOKUP.hpcItemProfiles.Count != 0)
                {
                    captionButton.BackColor = Color.Gray;

                    combineMode++;                    
                    if ((int)combineMode > (int)CombineMode.Fast)
                        combineMode = CombineMode.Default;

                    SetCombineMode(combineMode);
                    WriteConfig();
                }
                else
                    captionButton.BackColor = Color.Red;
            }

            base.captionButton_MouseDown(sender, e);
        }
        protected override void captionButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) captionButton.BackColor = Color.Black;
            base.captionButton_MouseUp(sender, e);
        }

        internal void SetCombineMode(CombineMode combineMode)
        {
            this.combineMode = combineMode;

            switch (combineMode)
            {
                case CombineMode.Fast:
                    DHLOOKUP.CollectItemCombiningData();
                    if (DHLOOKUP.hpcComplexItems == null)
                    {
                        MessageBox.Show("Fast Item Combining mode is not supported in this version,\n due to different item combining script in the map", UIStrings.Warning);
                        this.combineMode = CombineMode.Default;
                        captionB.BackColor = Color.Black;
                    }
                    break;
            }

            DisplayCaption();            
        }

        internal List<HabProperties> FindSameLookingComplexItem(HabProperties hpsItem)
        {
            if (sameLookingComplexItems == null)
                sameLookingComplexItems = new Dictionary<string, List<HabProperties>>(DHLOOKUP.hpcComplexItems.Count);

            string itemArt = hpsItem.GetStringValue("art");

            List<HabProperties> hpsList;
            if (sameLookingComplexItems.TryGetValue(itemArt, out hpsList))            
                return hpsList;                     
            else
                hpsList = new List<HabProperties>();

            foreach (string codeID in DHLOOKUP.hpcComplexItems.Keys)
            {
                HabProperties hpsCItem = DHLOOKUP.hpcItemProfiles[codeID];
                if (hpsCItem != null && IsArtSimilar(itemArt, hpsCItem.GetStringValue("art"))
                    && IsItemNameSimilar(hpsItem, hpsCItem))
                    hpsList.Add(hpsCItem);
            }

            sameLookingComplexItems.Add(itemArt, hpsList);
            
            return hpsList;
        }
        internal bool IsArtSimilar(string art1, string art2)
        {
            // remove any prefixes that could screw up art name comparision for shop/inventory item

            art1 = art1.Replace("Improved", "");
            art2 = art2.Replace("Improved", "");

            return art1 == art2;
        }
        internal bool IsItemNameSimilar(string itemID1, string itemID2)
        {
            return IsItemNameSimilar(
                DHHELPER.IsNewVersionItem(itemID1) ? DHLOOKUP.hpcUnitProfiles[itemID1] : DHLOOKUP.hpcItemProfiles[itemID1],
                DHHELPER.IsNewVersionItem(itemID2) ? DHLOOKUP.hpcUnitProfiles[itemID2] : DHLOOKUP.hpcItemProfiles[itemID2]);         
        }
        internal bool IsItemNameSimilar(HabProperties hpsItemA, HabProperties hpsItemB)
        {
            string[] name_partsA = DHSTRINGS.GetUntaggedString(hpsItemA.GetStringValue("Name")).Trim('"').Split(' ');
            string[] name_partsB = DHSTRINGS.GetUntaggedString(hpsItemB.GetStringValue("Name")).Trim('"').Split(' ');

            int matches = 0;
            for (int i = 0; i < name_partsA.Length; i++)
                for(int j=0; j < name_partsB.Length; j++)
                    if (name_partsA[i] == name_partsB[j]) matches++;

            return (matches > 0);
        }
        internal int GetGoldCost(HabProperties hpsItem)
        {
            if (ComplexItemCosts == null)
                ComplexItemCosts = new Dictionary<string, int>(DHLOOKUP.hpcComplexItems.Count);

            int result = 0;
            if (ComplexItemCosts.TryGetValue(hpsItem.name, out result))
                return result;

            HabProperties componentsList;
            if (!DHLOOKUP.hpcComplexItems.TryGetValue(hpsItem.name, out componentsList))            
                return DHLOOKUP.hpcItemData[hpsItem.name].GetIntValue("goldcost");        

            List<string> components = componentsList.GetStringListValue("0");            

            foreach (string codeID in components)
            {
                string originalCodeID;
                if (!DHLOOKUP.MorphingItems.TryGetValue(codeID, out originalCodeID))
                    originalCodeID = codeID;

                HabProperties hpsComponent;
                if (DHHELPER.IsNewVersionItem(originalCodeID))
                    hpsComponent = DHMpqDatabase.UnitSlkDatabase["UnitBalance"][originalCodeID];
                else
                    hpsComponent = DHLOOKUP.hpcItemData[originalCodeID];

                result += hpsComponent.GetIntValue("GoldCost");
            }

            ComplexItemCosts.Add(hpsItem.name, result);

            return result;
        }
        internal int GetGoldCost(item item)
        {
            if (DHLOOKUP.hpcComplexItems == null)            
                return GetDirectGoldCost(item);            

            if (ComplexItemCosts == null)
                ComplexItemCosts = new Dictionary<string, int>(DHLOOKUP.hpcComplexItems.Count);            

            int result = 0;
            if (ComplexItemCosts.TryGetValue(item.codeID, out result))
                return result;
            
            HabProperties componentsList;
            if (!DHLOOKUP.hpcComplexItems.TryGetValue(item.codeID, out componentsList))            
                return GetDirectGoldCost(item);

            List<string> components = componentsList.GetStringListValue("0");   

            foreach (string codeID in components)                            
                result += GetDirectGoldCost(codeID);            

            ComplexItemCosts.Add(item.codeID, result);

            return result;
        }

        internal int GetDirectGoldCost(string itemID)
        {
            string originalCodeID;
            if (DHLOOKUP.MorphingItems.TryGetValue(itemID, out originalCodeID))
            {
                if (DHHELPER.IsNewVersionItem(originalCodeID))
                    return DHMpqDatabase.UnitSlkDatabase["UnitBalance"][originalCodeID].GetIntValue("GoldCost");
                else
                    return DHLOOKUP.hpcItemData[originalCodeID].GetIntValue("GoldCost");
            }
            else
                return DHLOOKUP.hpcItemData[itemID].GetIntValue("GoldCost");
        }

        internal int GetDirectGoldCost(item item)
        {
            string originalCodeID;
            if (DHLOOKUP.MorphingItems.TryGetValue(item.codeID, out originalCodeID))
            {
                if (DHHELPER.IsNewVersionItem(originalCodeID))
                    return DHMpqDatabase.UnitSlkDatabase["UnitBalance"][originalCodeID].GetIntValue("GoldCost");
                else
                    return DHLOOKUP.hpcItemData[originalCodeID].GetIntValue("GoldCost");
            }
            else
                return item.goldCost;
        }

        public override void WriteConfig()
        {
            DHCFG.Items["Items"]["CombineMode"] = (int)combineMode;
        }

        private void shopsTS_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            int linesNeeded = ((shopsTS.Items.Count * (shopsTS.ImageScalingSize.Width + 2)) + (shopsTS.Width -1)) / shopsTS.Width;
            int currentLines = shopsTS.Height / shopsTS.ImageScalingSize.Height;
            if (linesNeeded != currentLines)
                ResizeShopList(linesNeeded - currentLines);
        }

        void ResizeShopList(int linesToAdd)
        {
            int difference = (shopsTS.ImageScalingSize.Height + 1) * linesToAdd;

            shopsTS.Height = shopsTS.Height + difference;
            itemsLV.Top += difference;
            itemsLV.Height -= difference;            
        }
    }
}