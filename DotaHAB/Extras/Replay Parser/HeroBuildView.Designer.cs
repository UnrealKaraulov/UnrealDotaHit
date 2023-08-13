namespace DotaHIT.Extras
{
    partial class HeroBuildView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.heroImagePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.heroNameLabel = new System.Windows.Forms.TextBox();
            this.playerNameLabel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.skillsDGrV = new System.Windows.Forms.DataGridView();
            this.skillTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.heroLevelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.skillImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.skillNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemsDGrV = new System.Windows.Forms.DataGridView();
            this.itemTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.itemNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.separateScrollingLL = new System.Windows.Forms.LinkLabel();
            this.autoFitLL = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.skillsDGrV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemsDGrV)).BeginInit();
            this.SuspendLayout();
            // 
            // heroImagePanel
            // 
            this.heroImagePanel.BackgroundImage = global::DotaHIT.Properties.Resources.armor;
            this.heroImagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.heroImagePanel.Location = new System.Drawing.Point(7, 7);
            this.heroImagePanel.Name = "heroImagePanel";
            this.heroImagePanel.Size = new System.Drawing.Size(64, 64);
            this.heroImagePanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Gold;
            this.label1.Location = new System.Drawing.Point(78, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 21);
            this.label1.TabIndex = 9;
            this.label1.Text = "Hero:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // heroNameLabel
            // 
            this.heroNameLabel.BackColor = System.Drawing.Color.Teal;
            this.heroNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.heroNameLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.heroNameLabel.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.heroNameLabel.Location = new System.Drawing.Point(139, 30);
            this.heroNameLabel.Name = "heroNameLabel";
            this.heroNameLabel.ReadOnly = true;
            this.heroNameLabel.Size = new System.Drawing.Size(221, 15);
            this.heroNameLabel.TabIndex = 4;
            this.heroNameLabel.Text = "None";
            // 
            // playerNameLabel
            // 
            this.playerNameLabel.BackColor = System.Drawing.Color.Teal;
            this.playerNameLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.playerNameLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.playerNameLabel.ForeColor = System.Drawing.Color.White;
            this.playerNameLabel.Location = new System.Drawing.Point(139, 10);
            this.playerNameLabel.Name = "playerNameLabel";
            this.playerNameLabel.ReadOnly = true;
            this.playerNameLabel.Size = new System.Drawing.Size(221, 15);
            this.playerNameLabel.TabIndex = 3;
            this.playerNameLabel.Text = "None";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.Gold;
            this.label4.Location = new System.Drawing.Point(81, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 21);
            this.label4.TabIndex = 10;
            this.label4.Text = "Player:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // skillsDGrV
            // 
            this.skillsDGrV.AllowUserToAddRows = false;
            this.skillsDGrV.AllowUserToDeleteRows = false;
            this.skillsDGrV.AllowUserToResizeRows = false;
            this.skillsDGrV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.skillsDGrV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.skillsDGrV.BackgroundColor = System.Drawing.Color.Teal;
            this.skillsDGrV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.skillsDGrV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.skillsDGrV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.skillTimeColumn,
            this.heroLevelColumn,
            this.skillImageColumn,
            this.skillNameColumn});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Ivory;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.skillsDGrV.DefaultCellStyle = dataGridViewCellStyle3;
            this.skillsDGrV.Location = new System.Drawing.Point(7, 77);
            this.skillsDGrV.MultiSelect = false;
            this.skillsDGrV.Name = "skillsDGrV";
            this.skillsDGrV.ReadOnly = true;
            this.skillsDGrV.RowHeadersVisible = false;
            this.skillsDGrV.RowHeadersWidth = 30;
            this.skillsDGrV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.skillsDGrV.Size = new System.Drawing.Size(285, 209);
            this.skillsDGrV.TabIndex = 1;
            this.skillsDGrV.VirtualMode = true;
            this.skillsDGrV.SizeChanged += new System.EventHandler(this.skillsDGrV_SizeChanged);
            this.skillsDGrV.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.skillsDGrV_CellValueNeeded);
            // 
            // skillTimeColumn
            // 
            this.skillTimeColumn.HeaderText = "Time";
            this.skillTimeColumn.MinimumWidth = 40;
            this.skillTimeColumn.Name = "skillTimeColumn";
            this.skillTimeColumn.ReadOnly = true;
            this.skillTimeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.skillTimeColumn.Width = 40;
            // 
            // heroLevelColumn
            // 
            this.heroLevelColumn.HeaderText = "Level";
            this.heroLevelColumn.MinimumWidth = 40;
            this.heroLevelColumn.Name = "heroLevelColumn";
            this.heroLevelColumn.ReadOnly = true;
            this.heroLevelColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.heroLevelColumn.Width = 40;
            // 
            // skillImageColumn
            // 
            this.skillImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.skillImageColumn.HeaderText = "Skill";
            this.skillImageColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.skillImageColumn.MinimumWidth = 30;
            this.skillImageColumn.Name = "skillImageColumn";
            this.skillImageColumn.ReadOnly = true;
            this.skillImageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.skillImageColumn.Width = 30;
            // 
            // skillNameColumn
            // 
            this.skillNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.skillNameColumn.HeaderText = "Name";
            this.skillNameColumn.Name = "skillNameColumn";
            this.skillNameColumn.ReadOnly = true;
            // 
            // itemsDGrV
            // 
            this.itemsDGrV.AllowUserToAddRows = false;
            this.itemsDGrV.AllowUserToDeleteRows = false;
            this.itemsDGrV.AllowUserToResizeRows = false;
            this.itemsDGrV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.itemsDGrV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.itemsDGrV.BackgroundColor = System.Drawing.Color.Teal;
            this.itemsDGrV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.itemsDGrV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.itemsDGrV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.itemTimeColumn,
            this.itemImageColumn,
            this.itemNameColumn});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Ivory;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemsDGrV.DefaultCellStyle = dataGridViewCellStyle4;
            this.itemsDGrV.Location = new System.Drawing.Point(299, 77);
            this.itemsDGrV.MinimumSize = new System.Drawing.Size(250, 0);
            this.itemsDGrV.MultiSelect = false;
            this.itemsDGrV.Name = "itemsDGrV";
            this.itemsDGrV.ReadOnly = true;
            this.itemsDGrV.RowHeadersVisible = false;
            this.itemsDGrV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.itemsDGrV.Size = new System.Drawing.Size(277, 209);
            this.itemsDGrV.TabIndex = 2;
            this.itemsDGrV.VirtualMode = true;
            this.itemsDGrV.SizeChanged += new System.EventHandler(this.itemsDGrV_SizeChanged);
            this.itemsDGrV.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.itemsDGrV_CellValueNeeded);
            // 
            // itemTimeColumn
            // 
            this.itemTimeColumn.HeaderText = "Time";
            this.itemTimeColumn.MinimumWidth = 40;
            this.itemTimeColumn.Name = "itemTimeColumn";
            this.itemTimeColumn.ReadOnly = true;
            this.itemTimeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.itemTimeColumn.Width = 40;
            // 
            // itemImageColumn
            // 
            this.itemImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.itemImageColumn.HeaderText = "Item";
            this.itemImageColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.itemImageColumn.MinimumWidth = 30;
            this.itemImageColumn.Name = "itemImageColumn";
            this.itemImageColumn.ReadOnly = true;
            this.itemImageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.itemImageColumn.Width = 30;
            // 
            // itemNameColumn
            // 
            this.itemNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.itemNameColumn.HeaderText = "Name";
            this.itemNameColumn.Name = "itemNameColumn";
            this.itemNameColumn.ReadOnly = true;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.Gold;
            this.label5.Location = new System.Drawing.Point(110, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 21);
            this.label5.TabIndex = 8;
            this.label5.Text = "Skill Order:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.Color.Gold;
            this.label6.Location = new System.Drawing.Point(399, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 21);
            this.label6.TabIndex = 7;
            this.label6.Text = "Item Order:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // separateScrollingLL
            // 
            this.separateScrollingLL.ActiveLinkColor = System.Drawing.Color.White;
            this.separateScrollingLL.LinkColor = System.Drawing.Color.LightGray;
            this.separateScrollingLL.Location = new System.Drawing.Point(463, 10);
            this.separateScrollingLL.Name = "separateScrollingLL";
            this.separateScrollingLL.Size = new System.Drawing.Size(113, 13);
            this.separateScrollingLL.TabIndex = 5;
            this.separateScrollingLL.TabStop = true;
            this.separateScrollingLL.Text = "Separate Scrolling: On";
            this.separateScrollingLL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.separateScrollingLL_MouseDown);
            // 
            // autoFitLL
            // 
            this.autoFitLL.ActiveLinkColor = System.Drawing.Color.White;
            this.autoFitLL.LinkColor = System.Drawing.Color.LightGray;
            this.autoFitLL.Location = new System.Drawing.Point(513, 27);
            this.autoFitLL.Name = "autoFitLL";
            this.autoFitLL.Size = new System.Drawing.Size(63, 13);
            this.autoFitLL.TabIndex = 6;
            this.autoFitLL.TabStop = true;
            this.autoFitLL.Text = "Auto-Fit: Off";
            this.autoFitLL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.autoFitLL_MouseDown);
            // 
            // HeroBuildView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Teal;
            this.Controls.Add(this.autoFitLL);
            this.Controls.Add(this.separateScrollingLL);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.itemsDGrV);
            this.Controls.Add(this.skillsDGrV);
            this.Controls.Add(this.playerNameLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.heroNameLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.heroImagePanel);
            this.Name = "HeroBuildView";
            this.Size = new System.Drawing.Size(584, 292);
            this.Click += new System.EventHandler(this.HeroBuildView_Click);
            this.Resize += new System.EventHandler(this.HeroBuildView_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.skillsDGrV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemsDGrV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox playerNameLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView skillsDGrV;
        private System.Windows.Forms.DataGridView itemsDGrV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel separateScrollingLL;
        internal System.Windows.Forms.Panel heroImagePanel;
        internal System.Windows.Forms.TextBox heroNameLabel;
        private System.Windows.Forms.LinkLabel autoFitLL;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroLevelColumn;
        private System.Windows.Forms.DataGridViewImageColumn skillImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn skillNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemTimeColumn;
        private System.Windows.Forms.DataGridViewImageColumn itemImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemNameColumn;
    }
}
