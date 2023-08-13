namespace DotaHIT
{
    partial class ItemListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.captionB = new System.Windows.Forms.Button();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.itemsLV = new System.Windows.Forms.ListView();
            this.shopsTS = new System.Windows.Forms.ToolStrip();
            this.listMinMaxTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTipTimer = new System.Windows.Forms.Timer(this.components);
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionB
            // 
            this.captionB.BackColor = System.Drawing.Color.Black;
            this.captionB.Dock = System.Windows.Forms.DockStyle.Top;
            this.captionB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.captionB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.captionB.ForeColor = System.Drawing.Color.Gold;
            this.captionB.Location = new System.Drawing.Point(0, 0);
            this.captionB.Name = "captionB";
            this.captionB.Size = new System.Drawing.Size(276, 23);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "Items";
            this.captionB.UseVisualStyleBackColor = false;
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.Black;
            this.contentPanel.Controls.Add(this.itemsLV);
            this.contentPanel.Controls.Add(this.shopsTS);
            this.contentPanel.Location = new System.Drawing.Point(0, 23);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(276, 241);
            this.contentPanel.TabIndex = 15;
            // 
            // itemsLV
            // 
            this.itemsLV.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.itemsLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsLV.AutoArrange = false;
            this.itemsLV.BackColor = System.Drawing.Color.Gray;
            this.itemsLV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.itemsLV.ForeColor = System.Drawing.Color.White;
            this.itemsLV.Location = new System.Drawing.Point(3, 24);
            this.itemsLV.Margin = new System.Windows.Forms.Padding(0);
            this.itemsLV.MultiSelect = false;
            this.itemsLV.Name = "itemsLV";
            this.itemsLV.OwnerDraw = true;
            this.itemsLV.Size = new System.Drawing.Size(270, 216);
            this.itemsLV.TabIndex = 11;
            this.itemsLV.TileSize = new System.Drawing.Size(56, 56);
            this.itemsLV.UseCompatibleStateImageBehavior = false;
            this.itemsLV.View = System.Windows.Forms.View.Tile;
            this.itemsLV.ItemActivate += new System.EventHandler(this.itemsLV_ItemActivate);
            this.itemsLV.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.itemsLV_DrawItem);
            this.itemsLV.MouseMove += new System.Windows.Forms.MouseEventHandler(this.itemsLV_MouseMove);
            this.itemsLV.MouseLeave += new System.EventHandler(this.itemsLV_MouseLeave);
            // 
            // shopsTS
            // 
            this.shopsTS.AllowItemReorder = true;
            this.shopsTS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shopsTS.AutoSize = false;
            this.shopsTS.BackColor = System.Drawing.Color.Transparent;
            this.shopsTS.CanOverflow = false;
            this.shopsTS.Dock = System.Windows.Forms.DockStyle.None;
            this.shopsTS.GripMargin = new System.Windows.Forms.Padding(0);
            this.shopsTS.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.shopsTS.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.shopsTS.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.shopsTS.Location = new System.Drawing.Point(3, 2);
            this.shopsTS.Name = "shopsTS";
            this.shopsTS.Padding = new System.Windows.Forms.Padding(0);
            this.shopsTS.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.shopsTS.Size = new System.Drawing.Size(270, 27);
            this.shopsTS.TabIndex = 32;
            this.shopsTS.Text = "toolStrip1";
            this.shopsTS.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.shopsTS_ItemAdded);
            // 
            // listMinMaxTimer
            // 
            this.listMinMaxTimer.Interval = 1;
            // 
            // toolTipTimer
            // 
            this.toolTipTimer.Tick += new System.EventHandler(this.toolTipTimer_Tick);
            // 
            // ItemListForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(276, 266);
            this.ControlBox = false;
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ItemListForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "HeroListForm";
            this.Shown += new System.EventHandler(this.ItemListForm_Shown);
            this.Load += new System.EventHandler(this.ItemListForm_Load);
            this.contentPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.ListView itemsLV;
        private System.Windows.Forms.Timer listMinMaxTimer;
        private System.Windows.Forms.ToolStrip shopsTS;
        private System.Windows.Forms.Timer toolTipTimer;
    }
}