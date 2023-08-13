namespace DotaHIT
{
    partial class HeroListForm
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
            this.all = new System.Windows.Forms.Button();
            this.sentinel = new System.Windows.Forms.Button();
            this.scourge = new System.Windows.Forms.Button();
            this.itemsLV = new System.Windows.Forms.ListView();
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
            this.captionB.Size = new System.Drawing.Size(280, 23);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "Heroes";
            this.captionB.UseVisualStyleBackColor = false;
            // 
            // contentPanel
            // 
            this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contentPanel.BackColor = System.Drawing.Color.Black;
            this.contentPanel.Controls.Add(this.all);
            this.contentPanel.Controls.Add(this.sentinel);
            this.contentPanel.Controls.Add(this.scourge);
            this.contentPanel.Controls.Add(this.itemsLV);
            this.contentPanel.Location = new System.Drawing.Point(0, 23);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(280, 243);
            this.contentPanel.TabIndex = 15;
            // 
            // all
            // 
            this.all.BackColor = System.Drawing.Color.Black;
            this.all.FlatAppearance.BorderSize = 0;
            this.all.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.all.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.all.ForeColor = System.Drawing.Color.Gold;
            this.all.Location = new System.Drawing.Point(118, 2);
            this.all.Name = "all";
            this.all.Size = new System.Drawing.Size(45, 21);
            this.all.TabIndex = 17;
            this.all.Text = "[ All ]";
            this.all.UseVisualStyleBackColor = false;
            this.all.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyB_MouseDown);
            // 
            // sentinel
            // 
            this.sentinel.BackColor = System.Drawing.Color.Black;
            this.sentinel.FlatAppearance.BorderSize = 0;
            this.sentinel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sentinel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sentinel.ForeColor = System.Drawing.Color.Gold;
            this.sentinel.Location = new System.Drawing.Point(3, 2);
            this.sentinel.Name = "sentinel";
            this.sentinel.Size = new System.Drawing.Size(120, 21);
            this.sentinel.TabIndex = 15;
            this.sentinel.Text = "The Sentinel";
            this.sentinel.UseVisualStyleBackColor = false;
            this.sentinel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyB_MouseDown);
            // 
            // scourge
            // 
            this.scourge.BackColor = System.Drawing.Color.Black;
            this.scourge.FlatAppearance.BorderSize = 0;
            this.scourge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scourge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.scourge.ForeColor = System.Drawing.Color.Gold;
            this.scourge.Location = new System.Drawing.Point(157, 2);
            this.scourge.Name = "scourge";
            this.scourge.Size = new System.Drawing.Size(120, 21);
            this.scourge.TabIndex = 16;
            this.scourge.Text = "The Scourge";
            this.scourge.UseVisualStyleBackColor = false;
            this.scourge.MouseDown += new System.Windows.Forms.MouseEventHandler(this.anyB_MouseDown);
            // 
            // itemsLV
            // 
            this.itemsLV.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.itemsLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsLV.AutoArrange = false;
            this.itemsLV.BackColor = System.Drawing.Color.Gray;
            this.itemsLV.ForeColor = System.Drawing.Color.White;
            this.itemsLV.Location = new System.Drawing.Point(3, 24);
            this.itemsLV.Margin = new System.Windows.Forms.Padding(0);
            this.itemsLV.MultiSelect = false;
            this.itemsLV.Name = "itemsLV";
            this.itemsLV.OwnerDraw = true;
            this.itemsLV.Size = new System.Drawing.Size(274, 216);
            this.itemsLV.TabIndex = 11;
            this.itemsLV.TileSize = new System.Drawing.Size(56, 56);
            this.itemsLV.UseCompatibleStateImageBehavior = false;
            this.itemsLV.View = System.Windows.Forms.View.Tile;
            this.itemsLV.ItemActivate += new System.EventHandler(this.itemsLV_ItemActivate);
            this.itemsLV.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.itemsLV_DrawItem);
            this.itemsLV.MouseMove += new System.Windows.Forms.MouseEventHandler(this.itemsLV_MouseMove);
            this.itemsLV.MouseLeave += new System.EventHandler(this.itemsLV_MouseLeave);
            // 
            // listMinMaxTimer
            // 
            this.listMinMaxTimer.Interval = 1;
            // 
            // toolTipTimer
            // 
            this.toolTipTimer.Tick += new System.EventHandler(this.toolTipTimer_Tick);
            // 
            // HeroListForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(280, 266);
            this.ControlBox = false;
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeroListForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "HeroListForm";
            this.contentPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Button all;
        private System.Windows.Forms.Button sentinel;
        private System.Windows.Forms.Button scourge;
        private System.Windows.Forms.ListView itemsLV;
        private System.Windows.Forms.Timer listMinMaxTimer;
        private System.Windows.Forms.Timer toolTipTimer;
    }
}