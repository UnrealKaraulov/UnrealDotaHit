using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;

namespace DotaHIT
{
    public partial class HeroListForm : FloatingListForm
    {
        public enum ListSwitch
        {
            Sentinel,
            Scourge,
            All,
            None
        };
        public ListSwitch listSwitch = ListSwitch.None;               

        internal RecordCollection Heroes = new RecordCollection();

        public delegate void ItemActivateEvent(object sender, IRecord item);
        public event ItemActivateEvent ItemActivate = null;

        public HeroListForm()
        {            
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
            this.listSwitch = ListSwitch.None;            
            this.Heroes = new RecordCollection();

            DHLOOKUP.RefreshTaverns();
        }

        public void Reset()
        {
            itemsLV.Clear();
            captionB.Text = "Heroes";
        }

        internal void PrepareList()
        {
            itemsLV.Groups.Clear();            
            
            foreach (unit u in DHLOOKUP.taverns)
                itemsLV.Groups.Add(u.ID,u.ID);
        }

        internal void PickNewHero(string heroID)
        {
            HabProperties hpsHero = DHLOOKUP.hpcUnitProfiles[heroID];

            Heroes.RemoveByUnit("ID", hpsHero.GetStringValue("Name"));            

            unit hero = SellHero(hpsHero);

            mainOwner.cbForm.manual_summon(hero);
        }
        internal unit SellHero(HabProperties hps)
        {
            if (hps == null) return null;

            // commented this line so that multiple same heroes would be possible
            //string ID = hps.GetStringValue("Name").Trim('"');
            unit hero = null; //Heroes.GetByUnit("ID", ID) as unit;

            if (hero != null && hero.IsDisposed)
            {
                if (Current.unit != null && Current.unit != hero
                    && Current.unit.codeID == hero.codeID
                    && !Current.unit.IsDisposed)
                {
                    Heroes.Remove(hero);
                    Heroes.Add(Current.unit);
                    hero = Current.unit;
                }
                else
                {
                    Heroes.Remove(hero);
                    hero = null;
                }
            }

            if (hero == null)
            {
                unit sellingTavern = null;

                // find the tavern that sold this hero

                string tavernID = DHLOOKUP.dcHeroesTaverns[hps.name];
                foreach (unit tavern in DHLOOKUP.taverns)
                    if (tavern.ID == tavernID)                    
                        sellingTavern = tavern;

                // create new hero

                hero = new unit(hps.name);                
                hero.DoSummon = true;
                hero.set_owningPlayer(Current.player, sellingTavern.x, sellingTavern.y);                
                Heroes.Add(hero);

                // only new heroes process onsell event

                sellingTavern.OnSell(hero);               

                // pay the gold for this hero

                Current.player.Gold = Current.player.Gold - hero.goldCost;
            }
            
            return hero;
        }        

        internal void SetListState(Button switchButton)
        {
            foreach (Control c in contentPanel.Controls)
                if (c is Button)
                {
                    if (switchButton == (c as Button))
                        (c as Button).BackColor = Color.Gray;
                    else
                        (c as Button).BackColor = Color.Black;
                }

            ListSwitch ls = (ListSwitch)Enum.Parse(typeof(ListSwitch), switchButton.Name, true);

            if (listSwitch == ls)
                return;
            else
                listSwitch = ls;

            InitListByState(ls);
        }
        internal void SetListState(ListSwitch ls)
        {
            if (listSwitch == ls)
                return;
            else
                listSwitch = ls;

            foreach (Control c in contentPanel.Controls)
                if (c is Button)
                {
                    if (String.Compare((c as Button).Name, ls.ToString(), true) == 0)
                        (c as Button).BackColor = Color.Gray;
                    else
                        (c as Button).BackColor = Color.Black;
                }

            InitListByState(ls);
        }

        internal void InitListByState(ListSwitch ls)
        {
            if (DHMpqDatabase.UnitSlkDatabase.Count == 0) return;

            HabPropertiesCollection hpcShortHeroes = new HabPropertiesCollection();

            switch (ls)
            {
                case ListSwitch.All:
                    foreach (unit tavern in DHLOOKUP.taverns)
                        foreach (string unitID in tavern.sellunits)
                        {
                            HabProperties hps = DHLOOKUP.hpcUnitProfiles[unitID];
                            hpcShortHeroes.Add(hps);
                        }                    
                    break;

                case ListSwitch.Sentinel:
                    foreach (unit tavern in DHLOOKUP.taverns)                    
                        if (tavern.y <0)
                            foreach (string unitID in tavern.sellunits)
                            {
                                HabProperties hps = DHLOOKUP.hpcUnitProfiles[unitID];
                                hpcShortHeroes.Add(hps);
                            }                    
                    break;

                case ListSwitch.Scourge:
                    foreach (unit tavern in DHLOOKUP.taverns)
                        if (tavern.y > 0)
                            foreach (string unitID in tavern.sellunits)
                            {
                                HabProperties hps = DHLOOKUP.hpcUnitProfiles[unitID];
                                hpcShortHeroes.Add(hps);
                            }
                    break;                    
            }

            InitList(hpcShortHeroes);
        }

        internal void InitList(RecordCollection heroes)
        {
            itemsLV.BeginUpdate();

            // init list

            itemsLV.Items.Clear();

            foreach (unit hero in heroes)
            {
                if (hero.iconImage.IsNull)
                    continue;

                ListViewItem lvi_Hero = new ListViewItem();

                lvi_Hero.ImageKey = hero.iconName;
                //lvi_Hero.Text = hero.ID;
                lvi_Hero.Tag = hero;
                foreach (unit tavern in DHLOOKUP.taverns)
                    if (tavern.sellunits.Contains(hero.codeID))
                        lvi_Hero.Group = itemsLV.Groups[tavern.ID];

                itemsLV.Items.Add(lvi_Hero);
            }

            itemsLV.EndUpdate();

            captionB.Text = "Heroes (" + itemsLV.Items.Count + ")";
        }
        internal void InitList(HabPropertiesCollection hpcShortHeroes)
        {
            itemsLV.BeginUpdate();

            // init list

            itemsLV.Items.Clear();

            foreach (HabProperties hpsHero in hpcShortHeroes)
            {
                string iconName = hpsHero.GetValue("Art") as string;
                if (String.IsNullOrEmpty(iconName))
                    continue;                

                ListViewItem lvi_Hero = new ListViewItem();                

                lvi_Hero.ImageKey = iconName;                
                lvi_Hero.Tag = hpsHero;
                lvi_Hero.Group = itemsLV.Groups[DHLOOKUP.dcHeroesTaverns[hpsHero.name]];

                itemsLV.Items.Add(lvi_Hero);
            }

            itemsLV.EndUpdate();

            captionB.Text = "Heroes (" + itemsLV.Items.Count + ")";
        }               

        private void anyB_MouseDown(object sender, MouseEventArgs e)
        {
            SetListState(sender as Button);
        }

        private void itemsLV_ItemActivate(object sender, EventArgs e)
        {
            foreach (int index in itemsLV.SelectedIndices)
            {
                unit hero = SellItem(index) as unit;
                mainOwner.cbForm.manual_summon(hero);
                //OnItemActivate(SellItem(index));
                break;
            }
        }

        private void OnItemActivate(IRecord e)
        {
            if (ItemActivate != null)
                ItemActivate(null, e);
        }

        private IRecord SellItem(int index)
        {
            object tag = itemsLV.Items[index].Tag;

            return SellHero(tag as HabProperties);
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

            if (toolTipItem is HabProperties)
                toolTip.showHeroToolTip(toolTipItem as HabProperties);
        }

        private void itemsLV_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Image image = (Image)DHRC.Default.GetImage(e.Item.ImageKey);

            if (image != null)
                e.Graphics.DrawImage(image, e.Bounds.X, e.Bounds.Y, 48,48);
            else
                Console.WriteLine("Couldn't get image: '" + e.Item.ImageKey + "'");
        }            
    }
}