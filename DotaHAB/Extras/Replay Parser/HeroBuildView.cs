using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Format;

namespace DotaHIT.Extras
{
    public partial class HeroBuildView : UserControl
    {
        Player player = null;
        Hero hero = null;
        ReplayMapCache cache;
        bool separateScrolling = true;
        bool autoFit = false;
        int shiftOffset = 0;

        public HeroBuildView()
        {
            InitializeComponent();

            skillsDGrV.RowHeadersWidth = skillsDGrV.Width;
            itemsDGrV.RowHeadersWidth = itemsDGrV.Width;
        }

        public HeroBuildView(Player p):this()
        {            
            DisplayHero(p);
        }

        public void DisplayHero(Player p)
        {
            if (p.Heroes.Count == 0)
            {
                playerNameLabel.Text = p.Name;                
                return;
            }

            player = p;
            hero = p.GetMostUsedHero();

            cache = p.MapCache;

            string imagePath = cache.hpcUnitProfiles[hero.Name, "Art"] as string;
            if (imagePath == null) return;

            heroImagePanel.BackgroundImage = cache.Resources.GetImage(imagePath);

            playerNameLabel.Text = p.Name;
            heroNameLabel.Text = DHFormatter.ToString(cache.hpcUnitProfiles[hero.Name, "Name"]);

            skillsDGrV.RowCount = hero.Abilities.BuildOrders.Count;
            itemsDGrV.RowCount = player.Items.BuildOrders.Count;            
        }

        private string timeToString(int time)
        {
            int minutes = time / (1000 * 60);
            int seconds = (time % (1000 * 60))/1000;

            return minutes.ToString("00", DBDOUBLE.provider) + ":" + seconds.ToString("00", DBDOUBLE.provider);
        }

        private void skillsDGrV_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            OrderItem skill = hero.Abilities.BuildOrders[e.RowIndex];

            switch (e.ColumnIndex)
            {
                case 0: // Time
                    e.Value = timeToString(skill.Time);
                    break;

                case 1: // Level
                    e.Value = "" + skill.Tag;//(e.RowIndex + 1);
                    break;

                case 2: // Skill Image
                    e.Value = cache.Resources.GetImage(DHFormatter.ToString(cache.hpcAbilityData[skill.Name, "Art"]));
                    break;

                case 3: // Skill Name
                    e.Value = DHFormatter.ToString(cache.hpcAbilityData[skill.Name, "Name"]) + " - Level " + skill.Count;
                    break;
            }
        }

        private void itemsDGrV_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            OrderItem item = player.Items.BuildOrders[e.RowIndex];

            switch (e.ColumnIndex)
            {
                case 0: // Time
                    e.Value = timeToString(item.Time);
                    break;

                case 1: // Item Image
                    if (cache.IsNewVersionItem(item.Name))
                        e.Value = cache.Resources.GetImage(DHFormatter.ToString(cache.hpcUnitProfiles[item.Name, "Art"]));
                    else
                        e.Value = cache.Resources.GetImage(DHFormatter.ToString(cache.hpcItemProfiles[item.Name, "Art"]));
                    break;

                case 2: // Item Name
                    if (cache.IsNewVersionItem(item.Name))
                        e.Value = DHFormatter.ToString(cache.hpcUnitProfiles[item.Name, "Name"]);
                    else
                        e.Value = DHFormatter.ToString(cache.hpcItemProfiles[item.Name, "Name"]);
                    break;
            }
        }

        private void separateScrollingLL_MouseDown(object sender, MouseEventArgs e)
        {
            separateScrolling = !separateScrolling;
            separateScrollingLL.Text = "Separate Scrolling: " + (separateScrolling ? "On" : "Off");
            adjustSeparateScrolling(separateScrolling);
        }

        private void adjustSeparateScrolling(bool separate)
        {
            if (separate)
            {
                skillsDGrV.Height = (this.Height - skillsDGrV.Top) - skillsDGrV.Left;
                itemsDGrV.Height = (this.Height - itemsDGrV.Top) - skillsDGrV.Left;                

                skillsDGrV.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                itemsDGrV.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

                if (shiftOffset != 0)
                {
                    separateScrollingLL.Left = separateScrollingLL.Left + shiftOffset;
                    autoFitLL.Left = autoFitLL.Left + shiftOffset;

                    itemsDGrV.Width = itemsDGrV.Width + shiftOffset;
                    shiftOffset = 0;
                }
            }
            else
            {
                int x = itemsDGrV.Left;

                skillsDGrV.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                itemsDGrV.Anchor = AnchorStyles.Left | AnchorStyles.Top;

                skillsDGrV.Height = skillsDGrV.PreferredSize.Height;
                itemsDGrV.Height = itemsDGrV.PreferredSize.Height;         

                if (itemsDGrV.Bounds.Right >= this.DisplayRectangle.X + this.ClientRectangle.Right)
                {
                    shiftOffset = 3 + (itemsDGrV.Bounds.Right - (this.DisplayRectangle.X + this.ClientRectangle.Right));

                    separateScrollingLL.Left = separateScrollingLL.Left - shiftOffset;
                    autoFitLL.Left = autoFitLL.Left - shiftOffset;

                    itemsDGrV.Width = itemsDGrV.Width - shiftOffset;
                }
            }
        }

        private void autoFitLL_MouseDown(object sender, MouseEventArgs e)
        {
            autoFit = !autoFit;
            autoFitLL.Text = "Auto-Fit: " + (autoFit ? "On" : "Off");
            adjustAutoFit(autoFit);
        }

        private void adjustAutoFit(bool enabled)
        {
            if (enabled)
            {
                int spaceToFit = this.DisplayRectangle.X + this.ClientRectangle.Right - (itemsDGrV.Bounds.Right + 3);
                int widthIncrease = spaceToFit / 2;

                widthIncrease = Math.Max(itemsDGrV.RowHeadersWidth, itemsDGrV.Width + widthIncrease) - itemsDGrV.Width;

                itemsDGrV.Width = itemsDGrV.Width + widthIncrease;
                itemsDGrV.Left = itemsDGrV.Left + widthIncrease;

                skillsDGrV.Width = skillsDGrV.Width + widthIncrease;
            }
            else
            {
                int difference = itemsDGrV.Width - itemsDGrV.RowHeadersWidth;
                itemsDGrV.Width = itemsDGrV.Width - difference;
                itemsDGrV.Left = itemsDGrV.Left - difference;

                skillsDGrV.Width = skillsDGrV.RowHeadersWidth;

                skillImageColumn.Width = skillImageColumn.MinimumWidth;
                itemImageColumn.Width = itemImageColumn.MinimumWidth;
            }
        }

        private void HeroBuildView_Resize(object sender, EventArgs e)
        {
            if (autoFit) adjustAutoFit(true);
        }

        private void skillsDGrV_SizeChanged(object sender, EventArgs e)
        {
            if (autoFit)
            {
                DataGridView view = sender as DataGridView;
                int difference = (view.Size.Width - view.RowHeadersWidth);
                skillImageColumn.Width = Math.Min(64, Math.Max(skillImageColumn.MinimumWidth, skillImageColumn.MinimumWidth + difference));
            }
        }

        private void itemsDGrV_SizeChanged(object sender, EventArgs e)
        {
            if (autoFit)
            {
                DataGridView view = sender as DataGridView;
                int difference = (view.Size.Width - view.RowHeadersWidth);
                itemImageColumn.Width = Math.Min(64, Math.Max(itemImageColumn.MinimumWidth, itemImageColumn.MinimumWidth + difference));
            }
        }

        private void HeroBuildView_Click(object sender, EventArgs e)
        {
            skillsDGrV.ClearSelection();
            itemsDGrV.ClearSelection();
        }      
    }
}
