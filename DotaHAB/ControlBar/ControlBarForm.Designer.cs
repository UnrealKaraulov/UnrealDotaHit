namespace DotaHIT
{
    partial class ControlBarForm
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
            this.captionB = new System.Windows.Forms.Button();
            this.itemCostMenuStrip = new System.Windows.Forms.MenuStrip();
            this.heroGoldTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.buildCostTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.buildCostSwitchTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.goldPanel = new System.Windows.Forms.Panel();
            this.chatLogB = new System.Windows.Forms.Button();
            this.actionsB = new System.Windows.Forms.Button();
            this.sentinelPlayersLL = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.scourgePlayersLL = new System.Windows.Forms.LinkLabel();
            this.messageRTB = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.speedTS = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.unitsB = new System.Windows.Forms.Button();
            this.itemCostMenuStrip.SuspendLayout();
            this.goldPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.speedTS.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionB
            // 
            this.captionB.BackColor = System.Drawing.Color.Black;
            this.captionB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captionB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.captionB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.captionB.ForeColor = System.Drawing.Color.Gold;
            this.captionB.Location = new System.Drawing.Point(0, 0);
            this.captionB.Margin = new System.Windows.Forms.Padding(0);
            this.captionB.MaximumSize = new System.Drawing.Size(0, 30);
            this.captionB.Name = "captionB";
            this.captionB.Size = new System.Drawing.Size(555, 27);
            this.captionB.TabIndex = 13;
            this.captionB.UseVisualStyleBackColor = false;
            this.captionB.Click += new System.EventHandler(this.captionB_Click);
            // 
            // itemCostMenuStrip
            // 
            this.itemCostMenuStrip.AutoSize = false;
            this.itemCostMenuStrip.BackColor = System.Drawing.Color.Transparent;
            this.itemCostMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.itemCostMenuStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.itemCostMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.heroGoldTSMI,
            this.buildCostTSMI,
            this.buildCostSwitchTSMI});
            this.itemCostMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.itemCostMenuStrip.Name = "itemCostMenuStrip";
            this.itemCostMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.itemCostMenuStrip.Size = new System.Drawing.Size(112, 24);
            this.itemCostMenuStrip.TabIndex = 15;
            // 
            // heroGoldTSMI
            // 
            this.heroGoldTSMI.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.heroGoldTSMI.ForeColor = System.Drawing.Color.White;
            this.heroGoldTSMI.Image = global::DotaHIT.Properties.Resources.goldcost;
            this.heroGoldTSMI.Name = "heroGoldTSMI";
            this.heroGoldTSMI.Padding = new System.Windows.Forms.Padding(0);
            this.heroGoldTSMI.Size = new System.Drawing.Size(33, 20);
            this.heroGoldTSMI.Text = "0";
            this.heroGoldTSMI.Click += new System.EventHandler(this.heroGoldTSMI_Click);
            // 
            // buildCostTSMI
            // 
            this.buildCostTSMI.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buildCostTSMI.Checked = true;
            this.buildCostTSMI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buildCostTSMI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buildCostTSMI.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buildCostTSMI.ForeColor = System.Drawing.Color.Silver;
            this.buildCostTSMI.Name = "buildCostTSMI";
            this.buildCostTSMI.Padding = new System.Windows.Forms.Padding(0);
            this.buildCostTSMI.Size = new System.Drawing.Size(17, 20);
            this.buildCostTSMI.Text = "0";
            this.buildCostTSMI.Click += new System.EventHandler(this.buildCostSwitchTSMI_Click);
            // 
            // buildCostSwitchTSMI
            // 
            this.buildCostSwitchTSMI.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.buildCostSwitchTSMI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buildCostSwitchTSMI.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buildCostSwitchTSMI.ForeColor = System.Drawing.Color.Gray;
            this.buildCostSwitchTSMI.Name = "buildCostSwitchTSMI";
            this.buildCostSwitchTSMI.Padding = new System.Windows.Forms.Padding(0);
            this.buildCostSwitchTSMI.Size = new System.Drawing.Size(19, 20);
            this.buildCostSwitchTSMI.Text = "$";
            this.buildCostSwitchTSMI.Visible = false;
            this.buildCostSwitchTSMI.Click += new System.EventHandler(this.buildCostSwitchTSMI_Click);
            // 
            // goldPanel
            // 
            this.goldPanel.Controls.Add(this.itemCostMenuStrip);
            this.goldPanel.Location = new System.Drawing.Point(431, 2);
            this.goldPanel.Name = "goldPanel";
            this.goldPanel.Size = new System.Drawing.Size(118, 21);
            this.goldPanel.TabIndex = 16;
            // 
            // chatLogB
            // 
            this.chatLogB.BackColor = System.Drawing.Color.Gray;
            this.chatLogB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chatLogB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chatLogB.Location = new System.Drawing.Point(151, 3);
            this.chatLogB.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.chatLogB.Name = "chatLogB";
            this.chatLogB.Size = new System.Drawing.Size(35, 21);
            this.chatLogB.TabIndex = 17;
            this.chatLogB.Text = "log";
            this.chatLogB.UseVisualStyleBackColor = false;
            this.chatLogB.Click += new System.EventHandler(this.chatLogB_Click);
            // 
            // actionsB
            // 
            this.actionsB.BackColor = System.Drawing.Color.Gray;
            this.actionsB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.actionsB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.actionsB.ForeColor = System.Drawing.Color.Silver;
            this.actionsB.Location = new System.Drawing.Point(376, 3);
            this.actionsB.Name = "actionsB";
            this.actionsB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.actionsB.Size = new System.Drawing.Size(56, 21);
            this.actionsB.TabIndex = 18;
            this.actionsB.Text = "actions";
            this.actionsB.UseVisualStyleBackColor = false;
            this.actionsB.Click += new System.EventHandler(this.actionsB_Click);
            // 
            // sentinelPlayersLL
            // 
            this.sentinelPlayersLL.ActiveLinkColor = System.Drawing.Color.Gold;
            this.sentinelPlayersLL.BackColor = System.Drawing.Color.Transparent;
            this.sentinelPlayersLL.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.sentinelPlayersLL.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.sentinelPlayersLL.LinkColor = System.Drawing.Color.White;
            this.sentinelPlayersLL.Location = new System.Drawing.Point(268, 7);
            this.sentinelPlayersLL.Name = "sentinelPlayersLL";
            this.sentinelPlayersLL.Size = new System.Drawing.Size(16, 14);
            this.sentinelPlayersLL.TabIndex = 19;
            this.sentinelPlayersLL.TabStop = true;
            this.sentinelPlayersLL.Text = "1";
            this.sentinelPlayersLL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.teamPlayersLL_MouseDown);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(279, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "x";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scourgePlayersLL
            // 
            this.scourgePlayersLL.ActiveLinkColor = System.Drawing.Color.Gold;
            this.scourgePlayersLL.BackColor = System.Drawing.Color.Transparent;
            this.scourgePlayersLL.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.scourgePlayersLL.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.scourgePlayersLL.LinkColor = System.Drawing.Color.White;
            this.scourgePlayersLL.Location = new System.Drawing.Point(288, 7);
            this.scourgePlayersLL.Name = "scourgePlayersLL";
            this.scourgePlayersLL.Size = new System.Drawing.Size(16, 14);
            this.scourgePlayersLL.TabIndex = 21;
            this.scourgePlayersLL.TabStop = true;
            this.scourgePlayersLL.Text = "1";
            this.scourgePlayersLL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.teamPlayersLL_MouseDown);
            // 
            // messageRTB
            // 
            this.messageRTB.BackColor = System.Drawing.Color.DarkSlateGray;
            this.messageRTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messageRTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.messageRTB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.messageRTB.ForeColor = System.Drawing.Color.White;
            this.messageRTB.Location = new System.Drawing.Point(5, 6);
            this.messageRTB.Name = "messageRTB";
            this.messageRTB.Size = new System.Drawing.Size(143, 16);
            this.messageRTB.TabIndex = 22;
            this.messageRTB.Text = "";
            this.messageRTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.messageRTB_KeyDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.speedTS);
            this.panel2.Location = new System.Drawing.Point(203, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(52, 20);
            this.panel2.TabIndex = 37;
            // 
            // speedTS
            // 
            this.speedTS.AllowMerge = false;
            this.speedTS.AutoSize = false;
            this.speedTS.BackColor = System.Drawing.Color.Black;
            this.speedTS.CanOverflow = false;
            this.speedTS.Dock = System.Windows.Forms.DockStyle.None;
            this.speedTS.GripMargin = new System.Windows.Forms.Padding(0);
            this.speedTS.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.speedTS.ImageScalingSize = new System.Drawing.Size(10, 10);
            this.speedTS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5});
            this.speedTS.Location = new System.Drawing.Point(0, 0);
            this.speedTS.Name = "speedTS";
            this.speedTS.Padding = new System.Windows.Forms.Padding(0);
            this.speedTS.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.speedTS.Size = new System.Drawing.Size(51, 25);
            this.speedTS.TabIndex = 35;
            this.speedTS.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.BackColor = System.Drawing.Color.Red;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(10, 10);
            this.toolStripButton1.Text = "Game speed: 1/2";
            this.toolStripButton1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_MouseDown);
            this.toolStripButton1.MouseLeave += new System.EventHandler(this.tsb_MouseLeave);
            this.toolStripButton1.MouseEnter += new System.EventHandler(this.tsb_MouseEnter);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.BackColor = System.Drawing.Color.Lime;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton2.MergeIndex = 0;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(10, 10);
            this.toolStripButton2.Text = "Game speed: 1";
            this.toolStripButton2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_MouseDown);
            this.toolStripButton2.MouseLeave += new System.EventHandler(this.tsb_MouseLeave);
            this.toolStripButton2.MouseEnter += new System.EventHandler(this.tsb_MouseEnter);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AutoSize = false;
            this.toolStripButton3.BackColor = System.Drawing.Color.Aquamarine;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton3.MergeIndex = 1;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(10, 10);
            this.toolStripButton3.Text = "Game speed: 2";
            this.toolStripButton3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_MouseDown);
            this.toolStripButton3.MouseLeave += new System.EventHandler(this.tsb_MouseLeave);
            this.toolStripButton3.MouseEnter += new System.EventHandler(this.tsb_MouseEnter);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoSize = false;
            this.toolStripButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton4.MergeIndex = 2;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(10, 10);
            this.toolStripButton4.Text = "Game speed: 3";
            this.toolStripButton4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_MouseDown);
            this.toolStripButton4.MouseLeave += new System.EventHandler(this.tsb_MouseLeave);
            this.toolStripButton4.MouseEnter += new System.EventHandler(this.tsb_MouseEnter);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.AutoSize = false;
            this.toolStripButton5.BackColor = System.Drawing.Color.Aqua;
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton5.MergeIndex = 3;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(10, 10);
            this.toolStripButton5.Text = "Game speed: 4";
            this.toolStripButton5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tsb_MouseDown);
            this.toolStripButton5.MouseLeave += new System.EventHandler(this.tsb_MouseLeave);
            this.toolStripButton5.MouseEnter += new System.EventHandler(this.tsb_MouseEnter);
            // 
            // unitsB
            // 
            this.unitsB.BackColor = System.Drawing.Color.Gray;
            this.unitsB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.unitsB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.unitsB.ForeColor = System.Drawing.Color.Silver;
            this.unitsB.Location = new System.Drawing.Point(319, 3);
            this.unitsB.Name = "unitsB";
            this.unitsB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.unitsB.Size = new System.Drawing.Size(54, 21);
            this.unitsB.TabIndex = 38;
            this.unitsB.Text = "units";
            this.unitsB.UseVisualStyleBackColor = false;
            this.unitsB.Click += new System.EventHandler(this.unitsB_Click);
            // 
            // ControlBarForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(555, 27);
            this.ControlBox = false;
            this.Controls.Add(this.unitsB);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.messageRTB);
            this.Controls.Add(this.scourgePlayersLL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sentinelPlayersLL);
            this.Controls.Add(this.actionsB);
            this.Controls.Add(this.chatLogB);
            this.Controls.Add(this.goldPanel);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(555, 30);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(555, 0);
            this.Name = "ControlBarForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "gamestate";
            this.itemCostMenuStrip.ResumeLayout(false);
            this.itemCostMenuStrip.PerformLayout();
            this.goldPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.speedTS.ResumeLayout(false);
            this.speedTS.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.MenuStrip itemCostMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem heroGoldTSMI;
        private System.Windows.Forms.ToolStripMenuItem buildCostTSMI;
        private System.Windows.Forms.Panel goldPanel;
        private System.Windows.Forms.Button chatLogB;
        private System.Windows.Forms.Button actionsB;
        private System.Windows.Forms.LinkLabel sentinelPlayersLL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel scourgePlayersLL;
        private System.Windows.Forms.RichTextBox messageRTB;
        private System.Windows.Forms.ToolStripMenuItem buildCostSwitchTSMI;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip speedTS;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.Button unitsB;
    }
}