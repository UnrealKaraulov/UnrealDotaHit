namespace DotaHIT.Extras.Replay_Parser
{
    partial class ReplayRawDataForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.actionNoneB = new System.Windows.Forms.Button();
            this.actionAllB = new System.Windows.Forms.Button();
            this.actionsCLB = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.playerNoneB = new System.Windows.Forms.Button();
            this.playerAllB = new System.Windows.Forms.Button();
            this.observersCLB = new System.Windows.Forms.CheckedListBox();
            this.scourgeCLB = new System.Windows.Forms.CheckedListBox();
            this.sentinelCLB = new System.Windows.Forms.CheckedListBox();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.timeFrameAllRB = new System.Windows.Forms.RadioButton();
            this.timeFrame5mRB = new System.Windows.Forms.RadioButton();
            this.timeFrame2mRB = new System.Windows.Forms.RadioButton();
            this.timeFrame1mRB = new System.Windows.Forms.RadioButton();
            this.timeFrame30RB = new System.Windows.Forms.RadioButton();
            this.timeFrame15RB = new System.Windows.Forms.RadioButton();
            this.timeframe5RB = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.forwardB = new System.Windows.Forms.Button();
            this.backB = new System.Windows.Forms.Button();
            this.DumpBtn = new System.Windows.Forms.Button();
            this.ShowSafeclick = new System.Windows.Forms.Button();
            this.showB = new System.Windows.Forms.Button();
            this.timeTextBox = new System.Windows.Forms.TextBox();
            this.timeTrackBar = new System.Windows.Forms.TrackBar();
            this.actionsDataGridView = new System.Windows.Forms.DataGridView();
            this.timeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.heroDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.actionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.actionsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(895, 149);
            this.panel1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.actionNoneB);
            this.groupBox2.Controls.Add(this.actionAllB);
            this.groupBox2.Controls.Add(this.actionsCLB);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(375, 142);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Action Filter";
            // 
            // actionNoneB
            // 
            this.actionNoneB.Location = new System.Drawing.Point(295, -1);
            this.actionNoneB.Name = "actionNoneB";
            this.actionNoneB.Size = new System.Drawing.Size(48, 19);
            this.actionNoneB.TabIndex = 3;
            this.actionNoneB.Text = "none";
            this.actionNoneB.UseVisualStyleBackColor = true;
            this.actionNoneB.Click += new System.EventHandler(this.actionNoneB_Click);
            // 
            // actionAllB
            // 
            this.actionAllB.Location = new System.Drawing.Point(240, -1);
            this.actionAllB.Name = "actionAllB";
            this.actionAllB.Size = new System.Drawing.Size(48, 19);
            this.actionAllB.TabIndex = 1;
            this.actionAllB.Text = "all";
            this.actionAllB.UseVisualStyleBackColor = true;
            this.actionAllB.Click += new System.EventHandler(this.actionAllB_Click);
            // 
            // actionsCLB
            // 
            this.actionsCLB.CheckOnClick = true;
            this.actionsCLB.FormattingEnabled = true;
            this.actionsCLB.Items.AddRange(new object[] {
            "Chat Message (not an action)",
            "Leave Game (not an action)",
            "0x01:  Pause Game",
            "0x02:  Resume Game",
            "0x03:  Set Game Speed",
            "0x04:  Increase Game Speed",
            "0x05:  Decrease Game Speed",
            "0x06:  Save Game",
            "0x07:  Save Game finished",
            "0x10:  Unit/Building ability",
            "0x11:  Unit/Building ability",
            "0x12:  Unit/Building ability",
            "0x13:  Give item to Unit / Drop item on ground",
            "0x14:  Unit/Building ability",
            "0x16:  Change Selection",
            "0x17:  Assign Group Hotkey",
            "0x18:  Select Group Hotkey",
            "0x19:  Select Subgroup",
            "0x1A:  Pre Subselection",
            "0x1B:  Change Selection Event",
            "0x1C:  Select Ground Item",
            "0x1D:  Cancel Hero revival",
            "0x1E:  Remove Unit from building queue",
            "0x21:  Unknown",
            "0x50:  Change ally options",
            "0x51:  Transfer resources",
            "0x60:  Map trigger chat command",
            "0x61:  ESC pressed",
            "0x62:  Scenario Trigger",
            "0x66:  Enter choose hero skill submenu",
            "0x67:  Enter choose building submenu",
            "0x68:  Minimap signal (ping)",
            "0x69:  Continue Game (BlockB)",
            "0x6A:  Continue Game (BlockA)",
            "0x6B:  SyncStoredInteger",
            "0x6C:  Unknown",
            "0x70:  Unknown",
            "0x75:  Unknown"});
            this.actionsCLB.Location = new System.Drawing.Point(6, 14);
            this.actionsCLB.Name = "actionsCLB";
            this.actionsCLB.Size = new System.Drawing.Size(369, 124);
            this.actionsCLB.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.playerNoneB);
            this.groupBox1.Controls.Add(this.playerAllB);
            this.groupBox1.Controls.Add(this.observersCLB);
            this.groupBox1.Controls.Add(this.scourgeCLB);
            this.groupBox1.Controls.Add(this.sentinelCLB);
            this.groupBox1.Location = new System.Drawing.Point(384, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 140);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Player filter";
            // 
            // playerNoneB
            // 
            this.playerNoneB.Location = new System.Drawing.Point(421, -1);
            this.playerNoneB.Name = "playerNoneB";
            this.playerNoneB.Size = new System.Drawing.Size(48, 19);
            this.playerNoneB.TabIndex = 5;
            this.playerNoneB.Text = "none";
            this.playerNoneB.UseVisualStyleBackColor = true;
            this.playerNoneB.Click += new System.EventHandler(this.playerNoneB_Click);
            // 
            // playerAllB
            // 
            this.playerAllB.Location = new System.Drawing.Point(366, -1);
            this.playerAllB.Name = "playerAllB";
            this.playerAllB.Size = new System.Drawing.Size(48, 19);
            this.playerAllB.TabIndex = 4;
            this.playerAllB.Text = "all";
            this.playerAllB.UseVisualStyleBackColor = true;
            this.playerAllB.Click += new System.EventHandler(this.playerAllB_Click);
            // 
            // observersCLB
            // 
            this.observersCLB.CheckOnClick = true;
            this.observersCLB.FormattingEnabled = true;
            this.observersCLB.Items.AddRange(new object[] {
            "1",
            "2"});
            this.observersCLB.Location = new System.Drawing.Point(6, 102);
            this.observersCLB.Name = "observersCLB";
            this.observersCLB.Size = new System.Drawing.Size(496, 34);
            this.observersCLB.TabIndex = 2;
            // 
            // scourgeCLB
            // 
            this.scourgeCLB.CheckOnClick = true;
            this.scourgeCLB.FormattingEnabled = true;
            this.scourgeCLB.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.scourgeCLB.Location = new System.Drawing.Point(257, 17);
            this.scourgeCLB.Name = "scourgeCLB";
            this.scourgeCLB.Size = new System.Drawing.Size(245, 79);
            this.scourgeCLB.TabIndex = 1;
            // 
            // sentinelCLB
            // 
            this.sentinelCLB.CheckOnClick = true;
            this.sentinelCLB.FormattingEnabled = true;
            this.sentinelCLB.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.sentinelCLB.Location = new System.Drawing.Point(6, 17);
            this.sentinelCLB.Name = "sentinelCLB";
            this.sentinelCLB.Size = new System.Drawing.Size(245, 79);
            this.sentinelCLB.TabIndex = 0;
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.White;
            this.controlPanel.Controls.Add(this.timeFrameAllRB);
            this.controlPanel.Controls.Add(this.timeFrame5mRB);
            this.controlPanel.Controls.Add(this.timeFrame2mRB);
            this.controlPanel.Controls.Add(this.timeFrame1mRB);
            this.controlPanel.Controls.Add(this.timeFrame30RB);
            this.controlPanel.Controls.Add(this.timeFrame15RB);
            this.controlPanel.Controls.Add(this.timeframe5RB);
            this.controlPanel.Controls.Add(this.label1);
            this.controlPanel.Controls.Add(this.forwardB);
            this.controlPanel.Controls.Add(this.backB);
            this.controlPanel.Controls.Add(this.DumpBtn);
            this.controlPanel.Controls.Add(this.ShowSafeclick);
            this.controlPanel.Controls.Add(this.showB);
            this.controlPanel.Controls.Add(this.timeTextBox);
            this.controlPanel.Controls.Add(this.timeTrackBar);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.controlPanel.Location = new System.Drawing.Point(0, 149);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(895, 67);
            this.controlPanel.TabIndex = 3;
            // 
            // timeFrameAllRB
            // 
            this.timeFrameAllRB.AutoSize = true;
            this.timeFrameAllRB.Location = new System.Drawing.Point(841, 42);
            this.timeFrameAllRB.Name = "timeFrameAllRB";
            this.timeFrameAllRB.Size = new System.Drawing.Size(36, 17);
            this.timeFrameAllRB.TabIndex = 13;
            this.timeFrameAllRB.TabStop = true;
            this.timeFrameAllRB.Tag = "-1";
            this.timeFrameAllRB.Text = "All";
            this.timeFrameAllRB.UseVisualStyleBackColor = true;
            this.timeFrameAllRB.CheckedChanged += new System.EventHandler(this.timeFrameAllRB_CheckedChanged);
            // 
            // timeFrame5mRB
            // 
            this.timeFrame5mRB.AutoSize = true;
            this.timeFrame5mRB.Location = new System.Drawing.Point(785, 42);
            this.timeFrame5mRB.Name = "timeFrame5mRB";
            this.timeFrame5mRB.Size = new System.Drawing.Size(50, 17);
            this.timeFrame5mRB.TabIndex = 12;
            this.timeFrame5mRB.TabStop = true;
            this.timeFrame5mRB.Tag = "300";
            this.timeFrame5mRB.Text = "5 min";
            this.timeFrame5mRB.UseVisualStyleBackColor = true;
            this.timeFrame5mRB.CheckedChanged += new System.EventHandler(this.timeFrame5mRB_CheckedChanged);
            // 
            // timeFrame2mRB
            // 
            this.timeFrame2mRB.AutoSize = true;
            this.timeFrame2mRB.Location = new System.Drawing.Point(729, 42);
            this.timeFrame2mRB.Name = "timeFrame2mRB";
            this.timeFrame2mRB.Size = new System.Drawing.Size(50, 17);
            this.timeFrame2mRB.TabIndex = 11;
            this.timeFrame2mRB.TabStop = true;
            this.timeFrame2mRB.Tag = "120";
            this.timeFrame2mRB.Text = "2 min";
            this.timeFrame2mRB.UseVisualStyleBackColor = true;
            this.timeFrame2mRB.CheckedChanged += new System.EventHandler(this.timeFrame2mRB_CheckedChanged);
            // 
            // timeFrame1mRB
            // 
            this.timeFrame1mRB.AutoSize = true;
            this.timeFrame1mRB.Location = new System.Drawing.Point(681, 42);
            this.timeFrame1mRB.Name = "timeFrame1mRB";
            this.timeFrame1mRB.Size = new System.Drawing.Size(42, 17);
            this.timeFrame1mRB.TabIndex = 10;
            this.timeFrame1mRB.TabStop = true;
            this.timeFrame1mRB.Tag = "60";
            this.timeFrame1mRB.Text = "60s";
            this.timeFrame1mRB.UseVisualStyleBackColor = true;
            this.timeFrame1mRB.CheckedChanged += new System.EventHandler(this.timeFrame1mRB_CheckedChanged);
            // 
            // timeFrame30RB
            // 
            this.timeFrame30RB.AutoSize = true;
            this.timeFrame30RB.Location = new System.Drawing.Point(633, 42);
            this.timeFrame30RB.Name = "timeFrame30RB";
            this.timeFrame30RB.Size = new System.Drawing.Size(42, 17);
            this.timeFrame30RB.TabIndex = 8;
            this.timeFrame30RB.TabStop = true;
            this.timeFrame30RB.Tag = "30";
            this.timeFrame30RB.Text = "30s";
            this.timeFrame30RB.UseVisualStyleBackColor = true;
            this.timeFrame30RB.CheckedChanged += new System.EventHandler(this.timeFrame30RB_CheckedChanged);
            // 
            // timeFrame15RB
            // 
            this.timeFrame15RB.AutoSize = true;
            this.timeFrame15RB.Location = new System.Drawing.Point(585, 42);
            this.timeFrame15RB.Name = "timeFrame15RB";
            this.timeFrame15RB.Size = new System.Drawing.Size(42, 17);
            this.timeFrame15RB.TabIndex = 7;
            this.timeFrame15RB.TabStop = true;
            this.timeFrame15RB.Tag = "15";
            this.timeFrame15RB.Text = "15s";
            this.timeFrame15RB.UseVisualStyleBackColor = true;
            this.timeFrame15RB.CheckedChanged += new System.EventHandler(this.timeFrame15RB_CheckedChanged);
            // 
            // timeframe5RB
            // 
            this.timeframe5RB.AutoSize = true;
            this.timeframe5RB.Location = new System.Drawing.Point(543, 42);
            this.timeframe5RB.Name = "timeframe5RB";
            this.timeframe5RB.Size = new System.Drawing.Size(36, 17);
            this.timeframe5RB.TabIndex = 6;
            this.timeframe5RB.TabStop = true;
            this.timeframe5RB.Tag = "5";
            this.timeframe5RB.Text = "5s";
            this.timeframe5RB.UseVisualStyleBackColor = true;
            this.timeframe5RB.CheckedChanged += new System.EventHandler(this.timeframe5RB_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(475, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Time frame:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // forwardB
            // 
            this.forwardB.Location = new System.Drawing.Point(418, 39);
            this.forwardB.Name = "forwardB";
            this.forwardB.Size = new System.Drawing.Size(41, 23);
            this.forwardB.TabIndex = 4;
            this.forwardB.Text = ">>";
            this.forwardB.UseVisualStyleBackColor = true;
            this.forwardB.Click += new System.EventHandler(this.forwardB_Click);
            // 
            // backB
            // 
            this.backB.Location = new System.Drawing.Point(371, 39);
            this.backB.Name = "backB";
            this.backB.Size = new System.Drawing.Size(41, 23);
            this.backB.TabIndex = 3;
            this.backB.Text = "<<";
            this.backB.UseVisualStyleBackColor = true;
            this.backB.Click += new System.EventHandler(this.backB_Click);
            // 
            // DumpBtn
            // 
            this.DumpBtn.Location = new System.Drawing.Point(254, 39);
            this.DumpBtn.Name = "DumpBtn";
            this.DumpBtn.Size = new System.Drawing.Size(102, 23);
            this.DumpBtn.TabIndex = 2;
            this.DumpBtn.Text = "Save dump";
            this.DumpBtn.UseVisualStyleBackColor = true;
            this.DumpBtn.Click += new System.EventHandler(this.showDump_Click);
            // 
            // ShowSafeclick
            // 
            this.ShowSafeclick.Location = new System.Drawing.Point(146, 39);
            this.ShowSafeclick.Name = "ShowSafeclick";
            this.ShowSafeclick.Size = new System.Drawing.Size(102, 23);
            this.ShowSafeclick.TabIndex = 2;
            this.ShowSafeclick.Text = "Show Safe Click";
            this.ShowSafeclick.UseVisualStyleBackColor = true;
            this.ShowSafeclick.Click += new System.EventHandler(this.showSafe_Click);
            // 
            // showB
            // 
            this.showB.Location = new System.Drawing.Point(99, 39);
            this.showB.Name = "showB";
            this.showB.Size = new System.Drawing.Size(41, 23);
            this.showB.TabIndex = 2;
            this.showB.Text = "show";
            this.showB.UseVisualStyleBackColor = true;
            this.showB.Click += new System.EventHandler(this.showB_Click);
            // 
            // timeTextBox
            // 
            this.timeTextBox.Location = new System.Drawing.Point(9, 41);
            this.timeTextBox.Name = "timeTextBox";
            this.timeTextBox.Size = new System.Drawing.Size(84, 20);
            this.timeTextBox.TabIndex = 1;
            this.timeTextBox.TextChanged += new System.EventHandler(this.timeTextBox_TextChanged);
            this.timeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.timeTextBox_KeyPress);
            // 
            // timeTrackBar
            // 
            this.timeTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.timeTrackBar.Location = new System.Drawing.Point(0, 0);
            this.timeTrackBar.Maximum = 3600;
            this.timeTrackBar.Name = "timeTrackBar";
            this.timeTrackBar.Size = new System.Drawing.Size(895, 45);
            this.timeTrackBar.TabIndex = 0;
            this.timeTrackBar.TickFrequency = 60;
            this.timeTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.timeTrackBar.Scroll += new System.EventHandler(this.timeTrackBar_Scroll);
            this.timeTrackBar.ValueChanged += new System.EventHandler(this.timeTrackBar_ValueChanged);
            // 
            // actionsDataGridView
            // 
            this.actionsDataGridView.AllowUserToAddRows = false;
            this.actionsDataGridView.AllowUserToDeleteRows = false;
            this.actionsDataGridView.AllowUserToResizeRows = false;
            this.actionsDataGridView.AutoGenerateColumns = false;
            this.actionsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.actionsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.actionsDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.actionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.actionsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timeDataGridViewTextBoxColumn,
            this.playerDataGridViewTextBoxColumn,
            this.heroDataGridViewTextBoxColumn,
            this.actionDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.dataDataGridViewTextBoxColumn});
            this.actionsDataGridView.DataSource = this.actionsBindingSource;
            this.actionsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsDataGridView.GridColor = System.Drawing.Color.Gainsboro;
            this.actionsDataGridView.Location = new System.Drawing.Point(0, 216);
            this.actionsDataGridView.Name = "actionsDataGridView";
            this.actionsDataGridView.ReadOnly = true;
            this.actionsDataGridView.RowHeadersVisible = false;
            this.actionsDataGridView.RowTemplate.Height = 19;
            this.actionsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.actionsDataGridView.ShowCellToolTips = false;
            this.actionsDataGridView.ShowRowErrors = false;
            this.actionsDataGridView.Size = new System.Drawing.Size(895, 300);
            this.actionsDataGridView.TabIndex = 39;
            this.actionsDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.actionsDataGridView_CellFormatting);
            // 
            // timeDataGridViewTextBoxColumn
            // 
            this.timeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.timeDataGridViewTextBoxColumn.DataPropertyName = "time";
            this.timeDataGridViewTextBoxColumn.HeaderText = "Time";
            this.timeDataGridViewTextBoxColumn.Name = "timeDataGridViewTextBoxColumn";
            this.timeDataGridViewTextBoxColumn.ReadOnly = true;
            this.timeDataGridViewTextBoxColumn.Width = 58;
            // 
            // playerDataGridViewTextBoxColumn
            // 
            this.playerDataGridViewTextBoxColumn.DataPropertyName = "player";
            this.playerDataGridViewTextBoxColumn.HeaderText = "Player";
            this.playerDataGridViewTextBoxColumn.Name = "playerDataGridViewTextBoxColumn";
            this.playerDataGridViewTextBoxColumn.ReadOnly = true;
            this.playerDataGridViewTextBoxColumn.Width = 115;
            // 
            // heroDataGridViewTextBoxColumn
            // 
            this.heroDataGridViewTextBoxColumn.DataPropertyName = "player";
            this.heroDataGridViewTextBoxColumn.HeaderText = "Hero";
            this.heroDataGridViewTextBoxColumn.Name = "heroDataGridViewTextBoxColumn";
            this.heroDataGridViewTextBoxColumn.ReadOnly = true;
            this.heroDataGridViewTextBoxColumn.Width = 122;
            // 
            // actionDataGridViewTextBoxColumn
            // 
            this.actionDataGridViewTextBoxColumn.DataPropertyName = "actionId";
            this.actionDataGridViewTextBoxColumn.HeaderText = "Action";
            this.actionDataGridViewTextBoxColumn.Name = "actionDataGridViewTextBoxColumn";
            this.actionDataGridViewTextBoxColumn.ReadOnly = true;
            this.actionDataGridViewTextBoxColumn.Width = 53;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "actionId";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            this.descriptionDataGridViewTextBoxColumn.Width = 148;
            // 
            // dataDataGridViewTextBoxColumn
            // 
            this.dataDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataDataGridViewTextBoxColumn.DataPropertyName = "actionId";
            this.dataDataGridViewTextBoxColumn.HeaderText = "Data";
            this.dataDataGridViewTextBoxColumn.Name = "dataDataGridViewTextBoxColumn";
            this.dataDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // actionsBindingSource
            // 
            this.actionsBindingSource.DataSource = typeof(DotaHIT.Extras.Replay_Parser.ReplayRawDataForm.Action);
            // 
            // ReplayRawDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 516);
            this.Controls.Add(this.actionsDataGridView);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.panel1);
            this.Name = "ReplayRawDataForm";
            this.ShowIcon = false;
            this.Text = "Replay Raw Data Viewer";
            this.Load += new System.EventHandler(this.ReplayRawDataForm_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.actionsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox observersCLB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox scourgeCLB;
        private System.Windows.Forms.CheckedListBox sentinelCLB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox actionsCLB;
        private System.Windows.Forms.Button actionNoneB;
        private System.Windows.Forms.Button actionAllB;
        private System.Windows.Forms.Button playerNoneB;
        private System.Windows.Forms.Button playerAllB;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.TrackBar timeTrackBar;
        private System.Windows.Forms.Button showB;
        private System.Windows.Forms.TextBox timeTextBox;
        private System.Windows.Forms.Button forwardB;
        private System.Windows.Forms.Button backB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton timeFrame5mRB;
        private System.Windows.Forms.RadioButton timeFrame2mRB;
        private System.Windows.Forms.RadioButton timeFrame1mRB;
        private System.Windows.Forms.RadioButton timeFrame30RB;
        private System.Windows.Forms.RadioButton timeFrame15RB;
        private System.Windows.Forms.RadioButton timeframe5RB;
        private System.Windows.Forms.RadioButton timeFrameAllRB;
        private System.Windows.Forms.DataGridView actionsDataGridView;
        private System.Windows.Forms.BindingSource actionsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn actionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button ShowSafeclick;
        private System.Windows.Forms.Button DumpBtn;
    }
}