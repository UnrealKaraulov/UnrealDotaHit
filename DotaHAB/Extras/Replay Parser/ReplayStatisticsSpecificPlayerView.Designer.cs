namespace DotaHIT.Extras.Replay_Parser
{
    partial class ReplayStatisticsSpecificPlayerView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label gamesPlayedLabel;
            System.Windows.Forms.Label gamesWonLabel;
            System.Windows.Forms.Label gamesLostLabel;
            System.Windows.Forms.Label winPercentageLabel;
            System.Windows.Forms.Label totalKillsLabel;
            System.Windows.Forms.Label totalDeathsLabel;
            System.Windows.Forms.Label totalAssistsLabel;
            System.Windows.Forms.Label killsPerGameLabel;
            System.Windows.Forms.Label deathsPerGameLabel;
            System.Windows.Forms.Label assistsPerGameLabel;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            this.playerHeroesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.playersTextBox = new System.Windows.Forms.TextBox();
            this.closeB = new System.Windows.Forms.Button();
            this.playerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gamesPlayedTextBox = new System.Windows.Forms.TextBox();
            this.gamesWonTextBox = new System.Windows.Forms.TextBox();
            this.gamesLostTextBox = new System.Windows.Forms.TextBox();
            this.winPercentageTextBox = new System.Windows.Forms.TextBox();
            this.totalKillsTextBox = new System.Windows.Forms.TextBox();
            this.totalDeathsTextBox = new System.Windows.Forms.TextBox();
            this.totalAssistsTextBox = new System.Windows.Forms.TextBox();
            this.killsPerGameTextBox = new System.Windows.Forms.TextBox();
            this.deathsPerGameTextBox = new System.Windows.Forms.TextBox();
            this.assistsPerGameTextBox = new System.Windows.Forms.TextBox();
            this.heroesDataGridView = new System.Windows.Forms.DataGridView();
            this.heroDummyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assistsPerGameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.deathsPerGameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.killsPerGameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalAssistsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalDeathsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalKillsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pickPercentageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winPercentageDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gamesLostDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gamesWonDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gamesPlayedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iconDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            gamesPlayedLabel = new System.Windows.Forms.Label();
            gamesWonLabel = new System.Windows.Forms.Label();
            gamesLostLabel = new System.Windows.Forms.Label();
            winPercentageLabel = new System.Windows.Forms.Label();
            totalKillsLabel = new System.Windows.Forms.Label();
            totalDeathsLabel = new System.Windows.Forms.Label();
            totalAssistsLabel = new System.Windows.Forms.Label();
            killsPerGameLabel = new System.Windows.Forms.Label();
            deathsPerGameLabel = new System.Windows.Forms.Label();
            assistsPerGameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.playerHeroesBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heroesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // playerHeroesBindingSource
            // 
            this.playerHeroesBindingSource.DataSource = typeof(DotaHIT.Extras.Replay_Parser.ReplayStatistics.HeroStatistics);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.AliceBlue;
            this.panel1.Controls.Add(assistsPerGameLabel);
            this.panel1.Controls.Add(this.assistsPerGameTextBox);
            this.panel1.Controls.Add(deathsPerGameLabel);
            this.panel1.Controls.Add(this.deathsPerGameTextBox);
            this.panel1.Controls.Add(killsPerGameLabel);
            this.panel1.Controls.Add(this.killsPerGameTextBox);
            this.panel1.Controls.Add(totalAssistsLabel);
            this.panel1.Controls.Add(this.totalAssistsTextBox);
            this.panel1.Controls.Add(totalDeathsLabel);
            this.panel1.Controls.Add(this.totalDeathsTextBox);
            this.panel1.Controls.Add(totalKillsLabel);
            this.panel1.Controls.Add(this.totalKillsTextBox);
            this.panel1.Controls.Add(winPercentageLabel);
            this.panel1.Controls.Add(this.winPercentageTextBox);
            this.panel1.Controls.Add(gamesLostLabel);
            this.panel1.Controls.Add(this.gamesLostTextBox);
            this.panel1.Controls.Add(gamesWonLabel);
            this.panel1.Controls.Add(this.gamesWonTextBox);
            this.panel1.Controls.Add(gamesPlayedLabel);
            this.panel1.Controls.Add(this.gamesPlayedTextBox);
            this.panel1.Controls.Add(this.closeB);
            this.panel1.Controls.Add(this.playersTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(611, 124);
            this.panel1.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Players found:";
            // 
            // playersTextBox
            // 
            this.playersTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.playersTextBox.Location = new System.Drawing.Point(92, 7);
            this.playersTextBox.Name = "playersTextBox";
            this.playersTextBox.ReadOnly = true;
            this.playersTextBox.Size = new System.Drawing.Size(458, 20);
            this.playersTextBox.TabIndex = 1;
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackColor = System.Drawing.Color.Black;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeB.ForeColor = System.Drawing.Color.Silver;
            this.closeB.Location = new System.Drawing.Point(585, 4);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(23, 19);
            this.closeB.TabIndex = 16;
            this.closeB.Text = "x";
            this.closeB.UseVisualStyleBackColor = false;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // playerBindingSource
            // 
            this.playerBindingSource.DataSource = typeof(DotaHIT.Extras.Replay_Parser.ReplayStatistics.PlayerStatistics);
            // 
            // gamesPlayedLabel
            // 
            gamesPlayedLabel.AutoSize = true;
            gamesPlayedLabel.Location = new System.Drawing.Point(8, 36);
            gamesPlayedLabel.Name = "gamesPlayedLabel";
            gamesPlayedLabel.Size = new System.Drawing.Size(78, 13);
            gamesPlayedLabel.TabIndex = 16;
            gamesPlayedLabel.Text = "Games Played:";
            // 
            // gamesPlayedTextBox
            // 
            this.gamesPlayedTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "GamesPlayed", true));
            this.gamesPlayedTextBox.Location = new System.Drawing.Point(92, 33);
            this.gamesPlayedTextBox.Name = "gamesPlayedTextBox";
            this.gamesPlayedTextBox.ReadOnly = true;
            this.gamesPlayedTextBox.Size = new System.Drawing.Size(60, 20);
            this.gamesPlayedTextBox.TabIndex = 17;
            // 
            // gamesWonLabel
            // 
            gamesWonLabel.AutoSize = true;
            gamesWonLabel.Location = new System.Drawing.Point(53, 62);
            gamesWonLabel.Name = "gamesWonLabel";
            gamesWonLabel.Size = new System.Drawing.Size(33, 13);
            gamesWonLabel.TabIndex = 17;
            gamesWonLabel.Text = "Won:";
            // 
            // gamesWonTextBox
            // 
            this.gamesWonTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "GamesWon", true));
            this.gamesWonTextBox.Location = new System.Drawing.Point(92, 59);
            this.gamesWonTextBox.Name = "gamesWonTextBox";
            this.gamesWonTextBox.ReadOnly = true;
            this.gamesWonTextBox.Size = new System.Drawing.Size(60, 20);
            this.gamesWonTextBox.TabIndex = 18;
            // 
            // gamesLostLabel
            // 
            gamesLostLabel.AutoSize = true;
            gamesLostLabel.Location = new System.Drawing.Point(56, 88);
            gamesLostLabel.Name = "gamesLostLabel";
            gamesLostLabel.Size = new System.Drawing.Size(30, 13);
            gamesLostLabel.TabIndex = 18;
            gamesLostLabel.Text = "Lost:";
            // 
            // gamesLostTextBox
            // 
            this.gamesLostTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "GamesLost", true));
            this.gamesLostTextBox.Location = new System.Drawing.Point(92, 85);
            this.gamesLostTextBox.Name = "gamesLostTextBox";
            this.gamesLostTextBox.ReadOnly = true;
            this.gamesLostTextBox.Size = new System.Drawing.Size(60, 20);
            this.gamesLostTextBox.TabIndex = 19;
            // 
            // winPercentageLabel
            // 
            winPercentageLabel.AutoSize = true;
            winPercentageLabel.Location = new System.Drawing.Point(158, 36);
            winPercentageLabel.Name = "winPercentageLabel";
            winPercentageLabel.Size = new System.Drawing.Size(40, 13);
            winPercentageLabel.TabIndex = 19;
            winPercentageLabel.Text = "Win %:";
            // 
            // winPercentageTextBox
            // 
            this.winPercentageTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "WinPercentage", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N1"));
            this.winPercentageTextBox.Location = new System.Drawing.Point(204, 33);
            this.winPercentageTextBox.Name = "winPercentageTextBox";
            this.winPercentageTextBox.ReadOnly = true;
            this.winPercentageTextBox.Size = new System.Drawing.Size(60, 20);
            this.winPercentageTextBox.TabIndex = 20;
            // 
            // totalKillsLabel
            // 
            totalKillsLabel.AutoSize = true;
            totalKillsLabel.Location = new System.Drawing.Point(284, 36);
            totalKillsLabel.Name = "totalKillsLabel";
            totalKillsLabel.Size = new System.Drawing.Size(28, 13);
            totalKillsLabel.TabIndex = 20;
            totalKillsLabel.Text = "Kills:";
            // 
            // totalKillsTextBox
            // 
            this.totalKillsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "TotalKills", true));
            this.totalKillsTextBox.Location = new System.Drawing.Point(318, 33);
            this.totalKillsTextBox.Name = "totalKillsTextBox";
            this.totalKillsTextBox.ReadOnly = true;
            this.totalKillsTextBox.Size = new System.Drawing.Size(60, 20);
            this.totalKillsTextBox.TabIndex = 21;
            // 
            // totalDeathsLabel
            // 
            totalDeathsLabel.AutoSize = true;
            totalDeathsLabel.Location = new System.Drawing.Point(268, 62);
            totalDeathsLabel.Name = "totalDeathsLabel";
            totalDeathsLabel.Size = new System.Drawing.Size(44, 13);
            totalDeathsLabel.TabIndex = 21;
            totalDeathsLabel.Text = "Deaths:";
            // 
            // totalDeathsTextBox
            // 
            this.totalDeathsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "TotalDeaths", true));
            this.totalDeathsTextBox.Location = new System.Drawing.Point(318, 59);
            this.totalDeathsTextBox.Name = "totalDeathsTextBox";
            this.totalDeathsTextBox.ReadOnly = true;
            this.totalDeathsTextBox.Size = new System.Drawing.Size(60, 20);
            this.totalDeathsTextBox.TabIndex = 22;
            // 
            // totalAssistsLabel
            // 
            totalAssistsLabel.AutoSize = true;
            totalAssistsLabel.Location = new System.Drawing.Point(270, 88);
            totalAssistsLabel.Name = "totalAssistsLabel";
            totalAssistsLabel.Size = new System.Drawing.Size(42, 13);
            totalAssistsLabel.TabIndex = 22;
            totalAssistsLabel.Text = "Assists:";
            // 
            // totalAssistsTextBox
            // 
            this.totalAssistsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "TotalAssists", true));
            this.totalAssistsTextBox.Location = new System.Drawing.Point(318, 85);
            this.totalAssistsTextBox.Name = "totalAssistsTextBox";
            this.totalAssistsTextBox.ReadOnly = true;
            this.totalAssistsTextBox.Size = new System.Drawing.Size(60, 20);
            this.totalAssistsTextBox.TabIndex = 23;
            // 
            // killsPerGameLabel
            // 
            killsPerGameLabel.AutoSize = true;
            killsPerGameLabel.Location = new System.Drawing.Point(417, 36);
            killsPerGameLabel.Name = "killsPerGameLabel";
            killsPerGameLabel.Size = new System.Drawing.Size(67, 13);
            killsPerGameLabel.TabIndex = 23;
            killsPerGameLabel.Text = "Kills / Game:";
            // 
            // killsPerGameTextBox
            // 
            this.killsPerGameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "KillsPerGame", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N1"));
            this.killsPerGameTextBox.Location = new System.Drawing.Point(490, 33);
            this.killsPerGameTextBox.Name = "killsPerGameTextBox";
            this.killsPerGameTextBox.ReadOnly = true;
            this.killsPerGameTextBox.Size = new System.Drawing.Size(60, 20);
            this.killsPerGameTextBox.TabIndex = 24;
            // 
            // deathsPerGameLabel
            // 
            deathsPerGameLabel.AutoSize = true;
            deathsPerGameLabel.Location = new System.Drawing.Point(401, 62);
            deathsPerGameLabel.Name = "deathsPerGameLabel";
            deathsPerGameLabel.Size = new System.Drawing.Size(83, 13);
            deathsPerGameLabel.TabIndex = 24;
            deathsPerGameLabel.Text = "Deaths / Game:";
            // 
            // deathsPerGameTextBox
            // 
            this.deathsPerGameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "DeathsPerGame", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N1"));
            this.deathsPerGameTextBox.Location = new System.Drawing.Point(490, 59);
            this.deathsPerGameTextBox.Name = "deathsPerGameTextBox";
            this.deathsPerGameTextBox.ReadOnly = true;
            this.deathsPerGameTextBox.Size = new System.Drawing.Size(60, 20);
            this.deathsPerGameTextBox.TabIndex = 25;
            // 
            // assistsPerGameLabel
            // 
            assistsPerGameLabel.AutoSize = true;
            assistsPerGameLabel.Location = new System.Drawing.Point(403, 88);
            assistsPerGameLabel.Name = "assistsPerGameLabel";
            assistsPerGameLabel.Size = new System.Drawing.Size(81, 13);
            assistsPerGameLabel.TabIndex = 25;
            assistsPerGameLabel.Text = "Assists / Game:";
            // 
            // assistsPerGameTextBox
            // 
            this.assistsPerGameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "AssistsPerGame", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N1"));
            this.assistsPerGameTextBox.Location = new System.Drawing.Point(490, 85);
            this.assistsPerGameTextBox.Name = "assistsPerGameTextBox";
            this.assistsPerGameTextBox.ReadOnly = true;
            this.assistsPerGameTextBox.Size = new System.Drawing.Size(60, 20);
            this.assistsPerGameTextBox.TabIndex = 26;
            // 
            // heroesDataGridView
            // 
            this.heroesDataGridView.AllowUserToAddRows = false;
            this.heroesDataGridView.AllowUserToDeleteRows = false;
            this.heroesDataGridView.AllowUserToResizeRows = false;
            this.heroesDataGridView.AutoGenerateColumns = false;
            this.heroesDataGridView.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.heroesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.heroesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconDataGridViewImageColumn,
            this.nameDataGridViewTextBoxColumn,
            this.gamesPlayedDataGridViewTextBoxColumn,
            this.gamesWonDataGridViewTextBoxColumn,
            this.gamesLostDataGridViewTextBoxColumn,
            this.winPercentageDataGridViewTextBoxColumn,
            this.pickPercentageDataGridViewTextBoxColumn,
            this.totalKillsDataGridViewTextBoxColumn,
            this.totalDeathsDataGridViewTextBoxColumn,
            this.totalAssistsDataGridViewTextBoxColumn,
            this.killsPerGameDataGridViewTextBoxColumn,
            this.deathsPerGameDataGridViewTextBoxColumn,
            this.assistsPerGameDataGridViewTextBoxColumn,
            this.heroDummyColumn});
            this.heroesDataGridView.DataSource = this.playerHeroesBindingSource;
            this.heroesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heroesDataGridView.Location = new System.Drawing.Point(0, 124);
            this.heroesDataGridView.MultiSelect = false;
            this.heroesDataGridView.Name = "heroesDataGridView";
            this.heroesDataGridView.ReadOnly = true;
            this.heroesDataGridView.RowHeadersVisible = false;
            this.heroesDataGridView.RowTemplate.Height = 34;
            this.heroesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.heroesDataGridView.ShowRowErrors = false;
            this.heroesDataGridView.Size = new System.Drawing.Size(611, 202);
            this.heroesDataGridView.TabIndex = 42;
            // 
            // heroDummyColumn
            // 
            this.heroDummyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.heroDummyColumn.HeaderText = "";
            this.heroDummyColumn.Name = "heroDummyColumn";
            this.heroDummyColumn.ReadOnly = true;
            this.heroDummyColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.heroDummyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // assistsPerGameDataGridViewTextBoxColumn
            // 
            this.assistsPerGameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.assistsPerGameDataGridViewTextBoxColumn.DataPropertyName = "AssistsPerGame";
            dataGridViewCellStyle21.Format = "N1";
            this.assistsPerGameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle21;
            this.assistsPerGameDataGridViewTextBoxColumn.HeaderText = "Assists/Game";
            this.assistsPerGameDataGridViewTextBoxColumn.Name = "assistsPerGameDataGridViewTextBoxColumn";
            this.assistsPerGameDataGridViewTextBoxColumn.ReadOnly = true;
            this.assistsPerGameDataGridViewTextBoxColumn.Width = 97;
            // 
            // deathsPerGameDataGridViewTextBoxColumn
            // 
            this.deathsPerGameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.deathsPerGameDataGridViewTextBoxColumn.DataPropertyName = "DeathsPerGame";
            dataGridViewCellStyle22.Format = "N1";
            this.deathsPerGameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle22;
            this.deathsPerGameDataGridViewTextBoxColumn.HeaderText = "Deaths/Game";
            this.deathsPerGameDataGridViewTextBoxColumn.Name = "deathsPerGameDataGridViewTextBoxColumn";
            this.deathsPerGameDataGridViewTextBoxColumn.ReadOnly = true;
            this.deathsPerGameDataGridViewTextBoxColumn.Width = 99;
            // 
            // killsPerGameDataGridViewTextBoxColumn
            // 
            this.killsPerGameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.killsPerGameDataGridViewTextBoxColumn.DataPropertyName = "KillsPerGame";
            dataGridViewCellStyle23.Format = "N1";
            dataGridViewCellStyle23.NullValue = null;
            this.killsPerGameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle23;
            this.killsPerGameDataGridViewTextBoxColumn.HeaderText = "Kills/Game";
            this.killsPerGameDataGridViewTextBoxColumn.Name = "killsPerGameDataGridViewTextBoxColumn";
            this.killsPerGameDataGridViewTextBoxColumn.ReadOnly = true;
            this.killsPerGameDataGridViewTextBoxColumn.Width = 83;
            // 
            // totalAssistsDataGridViewTextBoxColumn
            // 
            this.totalAssistsDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.totalAssistsDataGridViewTextBoxColumn.DataPropertyName = "TotalAssists";
            this.totalAssistsDataGridViewTextBoxColumn.HeaderText = "Assists";
            this.totalAssistsDataGridViewTextBoxColumn.Name = "totalAssistsDataGridViewTextBoxColumn";
            this.totalAssistsDataGridViewTextBoxColumn.ReadOnly = true;
            this.totalAssistsDataGridViewTextBoxColumn.Width = 64;
            // 
            // totalDeathsDataGridViewTextBoxColumn
            // 
            this.totalDeathsDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.totalDeathsDataGridViewTextBoxColumn.DataPropertyName = "TotalDeaths";
            this.totalDeathsDataGridViewTextBoxColumn.HeaderText = "Deaths";
            this.totalDeathsDataGridViewTextBoxColumn.Name = "totalDeathsDataGridViewTextBoxColumn";
            this.totalDeathsDataGridViewTextBoxColumn.ReadOnly = true;
            this.totalDeathsDataGridViewTextBoxColumn.Width = 66;
            // 
            // totalKillsDataGridViewTextBoxColumn
            // 
            this.totalKillsDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.totalKillsDataGridViewTextBoxColumn.DataPropertyName = "TotalKills";
            this.totalKillsDataGridViewTextBoxColumn.HeaderText = "Kills";
            this.totalKillsDataGridViewTextBoxColumn.Name = "totalKillsDataGridViewTextBoxColumn";
            this.totalKillsDataGridViewTextBoxColumn.ReadOnly = true;
            this.totalKillsDataGridViewTextBoxColumn.Width = 50;
            // 
            // pickPercentageDataGridViewTextBoxColumn
            // 
            this.pickPercentageDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.pickPercentageDataGridViewTextBoxColumn.DataPropertyName = "PickPercentage";
            dataGridViewCellStyle24.Format = "N1";
            dataGridViewCellStyle24.NullValue = null;
            this.pickPercentageDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle24;
            this.pickPercentageDataGridViewTextBoxColumn.HeaderText = "Pick %";
            this.pickPercentageDataGridViewTextBoxColumn.Name = "pickPercentageDataGridViewTextBoxColumn";
            this.pickPercentageDataGridViewTextBoxColumn.ReadOnly = true;
            this.pickPercentageDataGridViewTextBoxColumn.Width = 64;
            // 
            // winPercentageDataGridViewTextBoxColumn
            // 
            this.winPercentageDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.winPercentageDataGridViewTextBoxColumn.DataPropertyName = "WinPercentage";
            dataGridViewCellStyle25.Format = "N1";
            dataGridViewCellStyle25.NullValue = null;
            this.winPercentageDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle25;
            this.winPercentageDataGridViewTextBoxColumn.HeaderText = "Win %";
            this.winPercentageDataGridViewTextBoxColumn.Name = "winPercentageDataGridViewTextBoxColumn";
            this.winPercentageDataGridViewTextBoxColumn.ReadOnly = true;
            this.winPercentageDataGridViewTextBoxColumn.Width = 62;
            // 
            // gamesLostDataGridViewTextBoxColumn
            // 
            this.gamesLostDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.gamesLostDataGridViewTextBoxColumn.DataPropertyName = "GamesLost";
            this.gamesLostDataGridViewTextBoxColumn.HeaderText = "Lost";
            this.gamesLostDataGridViewTextBoxColumn.Name = "gamesLostDataGridViewTextBoxColumn";
            this.gamesLostDataGridViewTextBoxColumn.ReadOnly = true;
            this.gamesLostDataGridViewTextBoxColumn.Width = 52;
            // 
            // gamesWonDataGridViewTextBoxColumn
            // 
            this.gamesWonDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.gamesWonDataGridViewTextBoxColumn.DataPropertyName = "GamesWon";
            this.gamesWonDataGridViewTextBoxColumn.HeaderText = "Won";
            this.gamesWonDataGridViewTextBoxColumn.Name = "gamesWonDataGridViewTextBoxColumn";
            this.gamesWonDataGridViewTextBoxColumn.ReadOnly = true;
            this.gamesWonDataGridViewTextBoxColumn.Width = 55;
            // 
            // gamesPlayedDataGridViewTextBoxColumn
            // 
            this.gamesPlayedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.gamesPlayedDataGridViewTextBoxColumn.DataPropertyName = "GamesPlayed";
            this.gamesPlayedDataGridViewTextBoxColumn.HeaderText = "Games";
            this.gamesPlayedDataGridViewTextBoxColumn.Name = "gamesPlayedDataGridViewTextBoxColumn";
            this.gamesPlayedDataGridViewTextBoxColumn.ReadOnly = true;
            this.gamesPlayedDataGridViewTextBoxColumn.Width = 65;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 60;
            // 
            // iconDataGridViewImageColumn
            // 
            this.iconDataGridViewImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.iconDataGridViewImageColumn.DataPropertyName = "Icon";
            this.iconDataGridViewImageColumn.HeaderText = "Icon";
            this.iconDataGridViewImageColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.iconDataGridViewImageColumn.Name = "iconDataGridViewImageColumn";
            this.iconDataGridViewImageColumn.ReadOnly = true;
            this.iconDataGridViewImageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.iconDataGridViewImageColumn.Width = 34;
            // 
            // ReplayStatisticsSpecificPlayerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.heroesDataGridView);
            this.Controls.Add(this.panel1);
            this.Name = "ReplayStatisticsSpecificPlayerView";
            this.Size = new System.Drawing.Size(611, 326);
            ((System.ComponentModel.ISupportInitialize)(this.playerHeroesBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heroesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource playerHeroesBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox playersTextBox;
        private System.Windows.Forms.Button closeB;
        private System.Windows.Forms.BindingSource playerBindingSource;
        private System.Windows.Forms.TextBox winPercentageTextBox;
        private System.Windows.Forms.TextBox gamesLostTextBox;
        private System.Windows.Forms.TextBox gamesWonTextBox;
        private System.Windows.Forms.TextBox gamesPlayedTextBox;
        private System.Windows.Forms.TextBox totalDeathsTextBox;
        private System.Windows.Forms.TextBox totalKillsTextBox;
        private System.Windows.Forms.TextBox totalAssistsTextBox;
        private System.Windows.Forms.TextBox killsPerGameTextBox;
        private System.Windows.Forms.TextBox assistsPerGameTextBox;
        private System.Windows.Forms.TextBox deathsPerGameTextBox;
        private System.Windows.Forms.DataGridView heroesDataGridView;
        private System.Windows.Forms.DataGridViewImageColumn iconDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gamesPlayedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gamesWonDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gamesLostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn winPercentageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pickPercentageDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalKillsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalDeathsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn totalAssistsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn killsPerGameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn deathsPerGameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn assistsPerGameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroDummyColumn;
    }
}
