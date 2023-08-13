namespace DotaHIT.Extras.Replay_Parser
{
    partial class ReplayFinder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplayFinder));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.heroesGridView = new System.Windows.Forms.DataGridView();
            this.heroImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.heroNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playerNickColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.subfoldersCB = new System.Windows.Forms.CheckBox();
            this.startB = new System.Windows.Forms.Button();
            this.unitsCheckTimer = new System.Windows.Forms.Timer(this.components);
            this.rpsLabel = new System.Windows.Forms.Label();
            this.resultsDataGridView = new System.Windows.Forms.DataGridView();
            this.iReplayBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gameModeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gameLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.playersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winnerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replayPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.heroesGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iReplayBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.MintCream;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(629, 97);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // heroesGridView
            // 
            this.heroesGridView.AllowUserToAddRows = false;
            this.heroesGridView.AllowUserToDeleteRows = false;
            this.heroesGridView.AllowUserToResizeColumns = false;
            this.heroesGridView.AllowUserToResizeRows = false;
            this.heroesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.heroesGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.heroesGridView.BackgroundColor = System.Drawing.Color.GhostWhite;
            this.heroesGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.heroesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.heroesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.heroesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.heroImageColumn,
            this.heroNameColumn,
            this.playerNickColumn});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.heroesGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.heroesGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.heroesGridView.EnableHeadersVisualStyles = false;
            this.heroesGridView.Location = new System.Drawing.Point(12, 109);
            this.heroesGridView.MultiSelect = false;
            this.heroesGridView.Name = "heroesGridView";
            this.heroesGridView.RowHeadersVisible = false;
            this.heroesGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.heroesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.heroesGridView.Size = new System.Drawing.Size(629, 129);
            this.heroesGridView.TabIndex = 30;
            this.heroesGridView.VirtualMode = true;
            this.heroesGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.heroesGridView_CellValueNeeded);
            this.heroesGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.heroesGridView_CellValuePushed);
            this.heroesGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.heroesGridView_KeyDown);
            // 
            // heroImageColumn
            // 
            this.heroImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.heroImageColumn.FillWeight = 60F;
            this.heroImageColumn.HeaderText = "Hero";
            this.heroImageColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.heroImageColumn.Name = "heroImageColumn";
            this.heroImageColumn.ReadOnly = true;
            this.heroImageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.heroImageColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.heroImageColumn.Width = 28;
            // 
            // heroNameColumn
            // 
            this.heroNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.heroNameColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.heroNameColumn.HeaderText = "Name";
            this.heroNameColumn.Name = "heroNameColumn";
            this.heroNameColumn.ReadOnly = true;
            this.heroNameColumn.Width = 300;
            // 
            // playerNickColumn
            // 
            this.playerNickColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.playerNickColumn.HeaderText = "Player";
            this.playerNickColumn.Name = "playerNickColumn";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathTextBox.Location = new System.Drawing.Point(12, 257);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(547, 20);
            this.pathTextBox.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 241);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Search in this folder";
            // 
            // subfoldersCB
            // 
            this.subfoldersCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.subfoldersCB.AutoSize = true;
            this.subfoldersCB.Checked = true;
            this.subfoldersCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.subfoldersCB.Location = new System.Drawing.Point(565, 259);
            this.subfoldersCB.Name = "subfoldersCB";
            this.subfoldersCB.Size = new System.Drawing.Size(74, 17);
            this.subfoldersCB.TabIndex = 34;
            this.subfoldersCB.Text = "subfolders";
            this.subfoldersCB.UseVisualStyleBackColor = true;
            // 
            // startB
            // 
            this.startB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.startB.Location = new System.Drawing.Point(12, 283);
            this.startB.Name = "startB";
            this.startB.Size = new System.Drawing.Size(629, 23);
            this.startB.TabIndex = 35;
            this.startB.Text = "Start Searching";
            this.startB.UseVisualStyleBackColor = true;
            this.startB.Click += new System.EventHandler(this.startB_Click);
            // 
            // unitsCheckTimer
            // 
            this.unitsCheckTimer.Interval = 1000;
            this.unitsCheckTimer.Tick += new System.EventHandler(this.unitsCheckTimer_Tick);
            // 
            // rpsLabel
            // 
            this.rpsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rpsLabel.BackColor = System.Drawing.Color.Transparent;
            this.rpsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rpsLabel.ForeColor = System.Drawing.Color.Gray;
            this.rpsLabel.Location = new System.Drawing.Point(154, 495);
            this.rpsLabel.Name = "rpsLabel";
            this.rpsLabel.Size = new System.Drawing.Size(489, 12);
            this.rpsLabel.TabIndex = 37;
            this.rpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // resultsDataGridView
            // 
            this.resultsDataGridView.AllowUserToAddRows = false;
            this.resultsDataGridView.AllowUserToDeleteRows = false;
            this.resultsDataGridView.AllowUserToResizeRows = false;
            this.resultsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsDataGridView.AutoGenerateColumns = false;
            this.resultsDataGridView.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.resultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gameModeDataGridViewTextBoxColumn,
            this.gameLengthDataGridViewTextBoxColumn,
            this.playersDataGridViewTextBoxColumn,
            this.winnerDataGridViewTextBoxColumn,
            this.replayPathDataGridViewTextBoxColumn});
            this.resultsDataGridView.DataSource = this.iReplayBindingSource;
            this.resultsDataGridView.Location = new System.Drawing.Point(15, 312);
            this.resultsDataGridView.MultiSelect = false;
            this.resultsDataGridView.Name = "resultsDataGridView";
            this.resultsDataGridView.ReadOnly = true;
            this.resultsDataGridView.RowHeadersVisible = false;
            this.resultsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.resultsDataGridView.ShowRowErrors = false;
            this.resultsDataGridView.Size = new System.Drawing.Size(626, 180);
            this.resultsDataGridView.TabIndex = 38;
            this.resultsDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.resultsDataGridView_CellDoubleClick);
            this.resultsDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.resultsDataGridView_CellFormatting);
            // 
            // iReplayBindingSource
            // 
            this.iReplayBindingSource.DataSource = typeof(Deerchao.War3Share.W3gParser.IReplay);
            // 
            // gameModeDataGridViewTextBoxColumn
            // 
            this.gameModeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.gameModeDataGridViewTextBoxColumn.DataPropertyName = "GameMode";
            this.gameModeDataGridViewTextBoxColumn.HeaderText = "Game Mode";
            this.gameModeDataGridViewTextBoxColumn.Name = "gameModeDataGridViewTextBoxColumn";
            this.gameModeDataGridViewTextBoxColumn.ReadOnly = true;
            this.gameModeDataGridViewTextBoxColumn.Width = 90;
            // 
            // gameLengthDataGridViewTextBoxColumn
            // 
            this.gameLengthDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.gameLengthDataGridViewTextBoxColumn.DataPropertyName = "GameLength";
            dataGridViewCellStyle4.NullValue = null;
            this.gameLengthDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.gameLengthDataGridViewTextBoxColumn.HeaderText = "Game Length";
            this.gameLengthDataGridViewTextBoxColumn.Name = "gameLengthDataGridViewTextBoxColumn";
            this.gameLengthDataGridViewTextBoxColumn.ReadOnly = true;
            this.gameLengthDataGridViewTextBoxColumn.Width = 96;
            // 
            // playersDataGridViewTextBoxColumn
            // 
            this.playersDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.playersDataGridViewTextBoxColumn.DataPropertyName = "PlayerCount";
            this.playersDataGridViewTextBoxColumn.HeaderText = "Players";
            this.playersDataGridViewTextBoxColumn.Name = "playersDataGridViewTextBoxColumn";
            this.playersDataGridViewTextBoxColumn.ReadOnly = true;
            this.playersDataGridViewTextBoxColumn.Width = 66;
            // 
            // winnerDataGridViewTextBoxColumn
            // 
            this.winnerDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.winnerDataGridViewTextBoxColumn.DataPropertyName = "Winner";
            this.winnerDataGridViewTextBoxColumn.HeaderText = "Winner";
            this.winnerDataGridViewTextBoxColumn.Name = "winnerDataGridViewTextBoxColumn";
            this.winnerDataGridViewTextBoxColumn.ReadOnly = true;
            this.winnerDataGridViewTextBoxColumn.Width = 66;
            // 
            // replayPathDataGridViewTextBoxColumn
            // 
            this.replayPathDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.replayPathDataGridViewTextBoxColumn.DataPropertyName = "ReplayPath";
            this.replayPathDataGridViewTextBoxColumn.HeaderText = "ReplayPath";
            this.replayPathDataGridViewTextBoxColumn.Name = "replayPathDataGridViewTextBoxColumn";
            this.replayPathDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ReplayFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(653, 509);
            this.Controls.Add(this.resultsDataGridView);
            this.Controls.Add(this.startB);
            this.Controls.Add(this.subfoldersCB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.heroesGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rpsLabel);
            this.MinimizeBox = false;
            this.Name = "ReplayFinder";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Replay Finder";
            ((System.ComponentModel.ISupportInitialize)(this.heroesGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iReplayBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView heroesGridView;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox subfoldersCB;
        private System.Windows.Forms.Button startB;
        private System.Windows.Forms.Timer unitsCheckTimer;
        private System.Windows.Forms.DataGridViewImageColumn heroImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playerNickColumn;
        private System.Windows.Forms.Label rpsLabel;
        private System.Windows.Forms.DataGridView resultsDataGridView;
        private System.Windows.Forms.BindingSource iReplayBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn gameModeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gameLengthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playersDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn winnerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn replayPathDataGridViewTextBoxColumn;
    }
}