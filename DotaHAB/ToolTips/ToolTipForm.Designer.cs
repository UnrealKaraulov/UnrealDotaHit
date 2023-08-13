namespace DotaHIT
{
    partial class ToolTipForm
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
            this.toolTipTLP = new System.Windows.Forms.TableLayoutPanel();
            this.topRightPanel = new System.Windows.Forms.Panel();
            this.botRightPanel = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.botLeftPanel = new System.Windows.Forms.Panel();
            this.topLeftPanel = new System.Windows.Forms.Panel();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.botPanel = new System.Windows.Forms.Panel();
            this.bgPanel = new System.Windows.Forms.Panel();
            this.contentTLP = new System.Windows.Forms.TableLayoutPanel();
            this.contentRTB = new System.Windows.Forms.RichTextBox();
            this.toolTipTLP.SuspendLayout();
            this.bgPanel.SuspendLayout();
            this.contentTLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipTLP
            // 
            this.toolTipTLP.AutoSize = true;
            this.toolTipTLP.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.toolTipTLP.BackColor = System.Drawing.Color.Transparent;
            this.toolTipTLP.ColumnCount = 3;
            this.toolTipTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.toolTipTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.toolTipTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.toolTipTLP.Controls.Add(this.topRightPanel, 2, 0);
            this.toolTipTLP.Controls.Add(this.botRightPanel, 2, 2);
            this.toolTipTLP.Controls.Add(this.rightPanel, 2, 1);
            this.toolTipTLP.Controls.Add(this.topPanel, 1, 0);
            this.toolTipTLP.Controls.Add(this.botLeftPanel, 0, 2);
            this.toolTipTLP.Controls.Add(this.topLeftPanel, 0, 0);
            this.toolTipTLP.Controls.Add(this.leftPanel, 0, 1);
            this.toolTipTLP.Controls.Add(this.botPanel, 1, 2);
            this.toolTipTLP.Controls.Add(this.bgPanel, 1, 1);
            this.toolTipTLP.Location = new System.Drawing.Point(0, 0);
            this.toolTipTLP.Margin = new System.Windows.Forms.Padding(0);
            this.toolTipTLP.Name = "toolTipTLP";
            this.toolTipTLP.RowCount = 3;
            this.toolTipTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.toolTipTLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.toolTipTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.toolTipTLP.Size = new System.Drawing.Size(275, 56);
            this.toolTipTLP.TabIndex = 1;
            // 
            // topRightPanel
            // 
            this.topRightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.topRightPanel.BackColor = System.Drawing.Color.Transparent;
            this.topRightPanel.BackgroundImage = global::DotaHIT.Properties.Resources.top_right;
            this.topRightPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.topRightPanel.Location = new System.Drawing.Point(270, 0);
            this.topRightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topRightPanel.Name = "topRightPanel";
            this.topRightPanel.Size = new System.Drawing.Size(5, 5);
            this.topRightPanel.TabIndex = 4;
            // 
            // botRightPanel
            // 
            this.botRightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.botRightPanel.BackColor = System.Drawing.Color.Transparent;
            this.botRightPanel.BackgroundImage = global::DotaHIT.Properties.Resources.bot_right;
            this.botRightPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botRightPanel.Location = new System.Drawing.Point(270, 51);
            this.botRightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.botRightPanel.Name = "botRightPanel";
            this.botRightPanel.Size = new System.Drawing.Size(5, 5);
            this.botRightPanel.TabIndex = 5;
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.Gray;
            this.rightPanel.BackgroundImage = global::DotaHIT.Properties.Resources.right;
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(270, 5);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(5, 5);
            this.rightPanel.TabIndex = 0;
            this.rightPanel.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.rightPanel_PreviewKeyDown);
            // 
            // topPanel
            // 
            this.topPanel.BackgroundImage = global::DotaHIT.Properties.Resources.top;
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(5, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(5, 5);
            this.topPanel.TabIndex = 2;
            // 
            // botLeftPanel
            // 
            this.botLeftPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.botLeftPanel.BackColor = System.Drawing.Color.Transparent;
            this.botLeftPanel.BackgroundImage = global::DotaHIT.Properties.Resources.bot_left;
            this.botLeftPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botLeftPanel.Location = new System.Drawing.Point(0, 51);
            this.botLeftPanel.Margin = new System.Windows.Forms.Padding(0);
            this.botLeftPanel.Name = "botLeftPanel";
            this.botLeftPanel.Size = new System.Drawing.Size(5, 5);
            this.botLeftPanel.TabIndex = 5;
            // 
            // topLeftPanel
            // 
            this.topLeftPanel.BackColor = System.Drawing.Color.Transparent;
            this.topLeftPanel.BackgroundImage = global::DotaHIT.Properties.Resources.top_left;
            this.topLeftPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.topLeftPanel.Location = new System.Drawing.Point(0, 0);
            this.topLeftPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topLeftPanel.Name = "topLeftPanel";
            this.topLeftPanel.Size = new System.Drawing.Size(5, 5);
            this.topLeftPanel.TabIndex = 5;
            // 
            // leftPanel
            // 
            this.leftPanel.BackColor = System.Drawing.Color.Gray;
            this.leftPanel.BackgroundImage = global::DotaHIT.Properties.Resources.left;
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(0, 5);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(5, 5);
            this.leftPanel.TabIndex = 1;
            // 
            // botPanel
            // 
            this.botPanel.BackgroundImage = global::DotaHIT.Properties.Resources.bot;
            this.botPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.botPanel.Location = new System.Drawing.Point(5, 51);
            this.botPanel.Margin = new System.Windows.Forms.Padding(0);
            this.botPanel.Name = "botPanel";
            this.botPanel.Size = new System.Drawing.Size(5, 5);
            this.botPanel.TabIndex = 3;
            // 
            // bgPanel
            // 
            this.bgPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bgPanel.AutoSize = true;
            this.bgPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bgPanel.BackColor = System.Drawing.Color.Black;
            this.bgPanel.Controls.Add(this.contentTLP);
            this.bgPanel.Location = new System.Drawing.Point(5, 5);
            this.bgPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bgPanel.MinimumSize = new System.Drawing.Size(100, 24);
            this.bgPanel.Name = "bgPanel";
            this.bgPanel.Size = new System.Drawing.Size(265, 46);
            this.bgPanel.TabIndex = 6;
            // 
            // contentTLP
            // 
            this.contentTLP.AutoSize = true;
            this.contentTLP.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.contentTLP.BackColor = System.Drawing.Color.Black;
            this.contentTLP.ColumnCount = 1;
            this.contentTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentTLP.Controls.Add(this.contentRTB, 0, 0);
            this.contentTLP.Location = new System.Drawing.Point(0, 0);
            this.contentTLP.Margin = new System.Windows.Forms.Padding(0);
            this.contentTLP.MinimumSize = new System.Drawing.Size(265, 24);
            this.contentTLP.Name = "contentTLP";
            this.contentTLP.Padding = new System.Windows.Forms.Padding(4);
            this.contentTLP.RowCount = 1;
            this.contentTLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentTLP.Size = new System.Drawing.Size(265, 46);
            this.contentTLP.TabIndex = 0;
            // 
            // contentRTB
            // 
            this.contentRTB.BackColor = System.Drawing.Color.Black;
            this.contentRTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.contentRTB.DetectUrls = false;
            this.contentRTB.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.contentRTB.ForeColor = System.Drawing.Color.White;
            this.contentRTB.Location = new System.Drawing.Point(4, 4);
            this.contentRTB.Margin = new System.Windows.Forms.Padding(0);
            this.contentRTB.MinimumSize = new System.Drawing.Size(150, 38);
            this.contentRTB.Name = "contentRTB";
            this.contentRTB.ReadOnly = true;
            this.contentRTB.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.contentRTB.Size = new System.Drawing.Size(206, 38);
            this.contentRTB.TabIndex = 0;
            this.contentRTB.Text = "";
            this.contentRTB.WordWrap = false;
            this.contentRTB.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.contentRTB_ContentsResized);
            this.contentRTB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.contentRTB_KeyDown);
            // 
            // ToolTipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Aqua;
            this.ClientSize = new System.Drawing.Size(422, 53);
            this.ControlBox = false;
            this.Controls.Add(this.toolTipTLP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolTipForm";
            this.Opacity = 0.95;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.Color.Aqua;
            this.Shown += new System.EventHandler(this.ToolTipForm_Shown);
            this.toolTipTLP.ResumeLayout(false);
            this.toolTipTLP.PerformLayout();
            this.bgPanel.ResumeLayout(false);
            this.bgPanel.PerformLayout();
            this.contentTLP.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel topRightPanel;
        private System.Windows.Forms.Panel botRightPanel;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Panel botLeftPanel;
        private System.Windows.Forms.Panel topLeftPanel;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel botPanel;
        private System.Windows.Forms.Panel bgPanel;
        public System.Windows.Forms.TableLayoutPanel toolTipTLP;
        public System.Windows.Forms.TableLayoutPanel contentTLP;
        private System.Windows.Forms.RichTextBox contentRTB;
    }
}