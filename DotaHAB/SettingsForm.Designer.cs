namespace DotaHIT
{
    partial class SettingsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.captionB = new System.Windows.Forms.Button();
            this.closeB = new System.Windows.Forms.Button();
            this.war3PathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.applyB = new System.Windows.Forms.Button();
            this.cancelB = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.showUpdateSplashCB = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.updateIntervalNumUD = new System.Windows.Forms.NumericUpDown();
            this.updateAtStartupPerDaysRB = new System.Windows.Forms.RadioButton();
            this.updateNeverRB = new System.Windows.Forms.RadioButton();
            this.updateAtStartupRB = new System.Windows.Forms.RadioButton();
            this.showDetailSwitchTipCB = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chooseVerdanaFontB = new System.Windows.Forms.Button();
            this.verdanaFontTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chooseArialFontB = new System.Windows.Forms.Button();
            this.arialFontTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.useOwnDialogsCB = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fileTypesGridView = new System.Windows.Forms.DataGridView();
            this.extensionCoumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.associateColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.restoreCB = new System.Windows.Forms.CheckBox();
            this.war3PathBrowseB = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.blurryFontFixCB = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateIntervalNumUD)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileTypesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // captionB
            // 
            this.captionB.BackColor = System.Drawing.Color.Black;
            this.captionB.Dock = System.Windows.Forms.DockStyle.Top;
            this.captionB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.captionB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.captionB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.captionB.ForeColor = System.Drawing.Color.Gold;
            this.captionB.Location = new System.Drawing.Point(0, 0);
            this.captionB.Margin = new System.Windows.Forms.Padding(0);
            this.captionB.Name = "captionB";
            this.captionB.Size = new System.Drawing.Size(402, 26);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "DotA H.I.T. Settings";
            this.captionB.UseVisualStyleBackColor = false;
            this.captionB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseMove);
            this.captionB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseDown);
            this.captionB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseUp);
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackColor = System.Drawing.Color.Black;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeB.ForeColor = System.Drawing.Color.Silver;
            this.closeB.Location = new System.Drawing.Point(375, 3);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(23, 19);
            this.closeB.TabIndex = 15;
            this.closeB.Text = "x";
            this.closeB.UseVisualStyleBackColor = false;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // war3PathTextBox
            // 
            this.war3PathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.war3PathTextBox.Location = new System.Drawing.Point(8, 26);
            this.war3PathTextBox.Name = "war3PathTextBox";
            this.war3PathTextBox.Size = new System.Drawing.Size(355, 20);
            this.war3PathTextBox.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Warcraft III path";
            // 
            // applyB
            // 
            this.applyB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.applyB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.applyB.Location = new System.Drawing.Point(8, 376);
            this.applyB.Name = "applyB";
            this.applyB.Size = new System.Drawing.Size(98, 23);
            this.applyB.TabIndex = 18;
            this.applyB.Text = "apply";
            this.applyB.UseVisualStyleBackColor = true;
            this.applyB.Click += new System.EventHandler(this.applyB_Click);
            // 
            // cancelB
            // 
            this.cancelB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cancelB.Location = new System.Drawing.Point(289, 377);
            this.cancelB.Name = "cancelB";
            this.cancelB.Size = new System.Drawing.Size(98, 23);
            this.cancelB.TabIndex = 19;
            this.cancelB.Text = "cancel";
            this.cancelB.UseVisualStyleBackColor = true;
            this.cancelB.Click += new System.EventHandler(this.cancelB_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(147)))), ((int)(((byte)(148)))));
            this.panel1.Controls.Add(this.blurryFontFixCB);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.showDetailSwitchTipCB);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.useOwnDialogsCB);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.war3PathBrowseB);
            this.panel1.Controls.Add(this.war3PathTextBox);
            this.panel1.Controls.Add(this.cancelB);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.applyB);
            this.panel1.Location = new System.Drawing.Point(3, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 408);
            this.panel1.TabIndex = 20;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.showUpdateSplashCB);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.updateIntervalNumUD);
            this.groupBox3.Controls.Add(this.updateAtStartupPerDaysRB);
            this.groupBox3.Controls.Add(this.updateNeverRB);
            this.groupBox3.Controls.Add(this.updateAtStartupRB);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(9, 299);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(379, 71);
            this.groupBox3.TabIndex = 30;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Check for Updates";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(205, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "(checks silently if disabled)";
            // 
            // showUpdateSplashCB
            // 
            this.showUpdateSplashCB.AutoSize = true;
            this.showUpdateSplashCB.Checked = true;
            this.showUpdateSplashCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showUpdateSplashCB.Location = new System.Drawing.Point(88, 48);
            this.showUpdateSplashCB.Name = "showUpdateSplashCB";
            this.showUpdateSplashCB.Size = new System.Drawing.Size(121, 17);
            this.showUpdateSplashCB.TabIndex = 5;
            this.showUpdateSplashCB.Text = "Show splash screen";
            this.showUpdateSplashCB.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(339, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "days";
            // 
            // updateIntervalNumUD
            // 
            this.updateIntervalNumUD.Location = new System.Drawing.Point(305, 21);
            this.updateIntervalNumUD.Name = "updateIntervalNumUD";
            this.updateIntervalNumUD.Size = new System.Drawing.Size(32, 20);
            this.updateIntervalNumUD.TabIndex = 3;
            this.updateIntervalNumUD.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // updateAtStartupPerDaysRB
            // 
            this.updateAtStartupPerDaysRB.AutoSize = true;
            this.updateAtStartupPerDaysRB.Checked = true;
            this.updateAtStartupPerDaysRB.Location = new System.Drawing.Point(177, 21);
            this.updateAtStartupPerDaysRB.Name = "updateAtStartupPerDaysRB";
            this.updateAtStartupPerDaysRB.Size = new System.Drawing.Size(128, 17);
            this.updateAtStartupPerDaysRB.TabIndex = 2;
            this.updateAtStartupPerDaysRB.TabStop = true;
            this.updateAtStartupPerDaysRB.Text = "At Startup once every";
            this.updateAtStartupPerDaysRB.UseVisualStyleBackColor = true;
            // 
            // updateNeverRB
            // 
            this.updateNeverRB.AutoSize = true;
            this.updateNeverRB.Location = new System.Drawing.Point(12, 21);
            this.updateNeverRB.Name = "updateNeverRB";
            this.updateNeverRB.Size = new System.Drawing.Size(54, 17);
            this.updateNeverRB.TabIndex = 1;
            this.updateNeverRB.TabStop = true;
            this.updateNeverRB.Text = "Never";
            this.updateNeverRB.UseVisualStyleBackColor = true;
            // 
            // updateAtStartupRB
            // 
            this.updateAtStartupRB.AutoSize = true;
            this.updateAtStartupRB.Location = new System.Drawing.Point(87, 21);
            this.updateAtStartupRB.Name = "updateAtStartupRB";
            this.updateAtStartupRB.Size = new System.Drawing.Size(72, 17);
            this.updateAtStartupRB.TabIndex = 0;
            this.updateAtStartupRB.TabStop = true;
            this.updateAtStartupRB.Text = "At Startup";
            this.updateAtStartupRB.UseVisualStyleBackColor = true;
            // 
            // showDetailSwitchTipCB
            // 
            this.showDetailSwitchTipCB.AutoSize = true;
            this.showDetailSwitchTipCB.ForeColor = System.Drawing.Color.White;
            this.showDetailSwitchTipCB.Location = new System.Drawing.Point(14, 221);
            this.showDetailSwitchTipCB.Name = "showDetailSwitchTipCB";
            this.showDetailSwitchTipCB.Size = new System.Drawing.Size(233, 17);
            this.showDetailSwitchTipCB.TabIndex = 29;
            this.showDetailSwitchTipCB.Text = "Show detail-mode switch tip for hero abilities";
            this.showDetailSwitchTipCB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chooseVerdanaFontB);
            this.groupBox2.Controls.Add(this.verdanaFontTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.chooseArialFontB);
            this.groupBox2.Controls.Add(this.arialFontTextBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(8, 244);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(379, 49);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Font replacements";
            // 
            // chooseVerdanaFontB
            // 
            this.chooseVerdanaFontB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseVerdanaFontB.ForeColor = System.Drawing.Color.Black;
            this.chooseVerdanaFontB.Location = new System.Drawing.Point(352, 17);
            this.chooseVerdanaFontB.Name = "chooseVerdanaFontB";
            this.chooseVerdanaFontB.Size = new System.Drawing.Size(19, 23);
            this.chooseVerdanaFontB.TabIndex = 15;
            this.chooseVerdanaFontB.Text = "…";
            this.chooseVerdanaFontB.UseVisualStyleBackColor = true;
            this.chooseVerdanaFontB.Click += new System.EventHandler(this.chooseVerdanaFontB_Click);
            // 
            // verdanaFontTextBox
            // 
            this.verdanaFontTextBox.Location = new System.Drawing.Point(256, 19);
            this.verdanaFontTextBox.Name = "verdanaFontTextBox";
            this.verdanaFontTextBox.Size = new System.Drawing.Size(95, 20);
            this.verdanaFontTextBox.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(206, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Verdana";
            // 
            // chooseArialFontB
            // 
            this.chooseArialFontB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseArialFontB.ForeColor = System.Drawing.Color.Black;
            this.chooseArialFontB.Location = new System.Drawing.Point(144, 17);
            this.chooseArialFontB.Name = "chooseArialFontB";
            this.chooseArialFontB.Size = new System.Drawing.Size(19, 23);
            this.chooseArialFontB.TabIndex = 12;
            this.chooseArialFontB.Text = "…";
            this.chooseArialFontB.UseVisualStyleBackColor = true;
            this.chooseArialFontB.Click += new System.EventHandler(this.chooseArialFontB_Click);
            // 
            // arialFontTextBox
            // 
            this.arialFontTextBox.Location = new System.Drawing.Point(48, 19);
            this.arialFontTextBox.Name = "arialFontTextBox";
            this.arialFontTextBox.Size = new System.Drawing.Size(95, 20);
            this.arialFontTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Arial";
            // 
            // useOwnDialogsCB
            // 
            this.useOwnDialogsCB.AutoSize = true;
            this.useOwnDialogsCB.ForeColor = System.Drawing.Color.White;
            this.useOwnDialogsCB.Location = new System.Drawing.Point(14, 200);
            this.useOwnDialogsCB.Name = "useOwnDialogsCB";
            this.useOwnDialogsCB.Size = new System.Drawing.Size(279, 17);
            this.useOwnDialogsCB.TabIndex = 27;
            this.useOwnDialogsCB.Text = "Use DotA H.I.T. Explorer for browsing files and folders";
            this.useOwnDialogsCB.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fileTypesGridView);
            this.groupBox1.Controls.Add(this.restoreCB);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(8, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 141);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File type associations";
            // 
            // fileTypesGridView
            // 
            this.fileTypesGridView.AllowUserToAddRows = false;
            this.fileTypesGridView.AllowUserToDeleteRows = false;
            this.fileTypesGridView.AllowUserToResizeColumns = false;
            this.fileTypesGridView.AllowUserToResizeRows = false;
            this.fileTypesGridView.BackgroundColor = System.Drawing.Color.GhostWhite;
            this.fileTypesGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.PaleTurquoise;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.fileTypesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.fileTypesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fileTypesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.extensionCoumn,
            this.descriptionColumn,
            this.associateColumn,
            this.contextMenuColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.fileTypesGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.fileTypesGridView.EnableHeadersVisualStyles = false;
            this.fileTypesGridView.Location = new System.Drawing.Point(7, 18);
            this.fileTypesGridView.MultiSelect = false;
            this.fileTypesGridView.Name = "fileTypesGridView";
            this.fileTypesGridView.RowHeadersVisible = false;
            this.fileTypesGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.fileTypesGridView.Size = new System.Drawing.Size(364, 97);
            this.fileTypesGridView.TabIndex = 28;
            this.fileTypesGridView.VirtualMode = true;
            this.fileTypesGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.fileTypesGridView_CellValueNeeded);
            this.fileTypesGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.fileTypesGridView_CellValuePushed);
            // 
            // extensionCoumn
            // 
            this.extensionCoumn.FillWeight = 60F;
            this.extensionCoumn.HeaderText = "File Type";
            this.extensionCoumn.Name = "extensionCoumn";
            this.extensionCoumn.ReadOnly = true;
            this.extensionCoumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.extensionCoumn.Width = 40;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.HeaderText = "Description";
            this.descriptionColumn.Name = "descriptionColumn";
            this.descriptionColumn.ReadOnly = true;
            this.descriptionColumn.Width = 120;
            // 
            // associateColumn
            // 
            this.associateColumn.HeaderText = "Associate with DotA H.I.T.";
            this.associateColumn.Name = "associateColumn";
            this.associateColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.associateColumn.Width = 90;
            // 
            // contextMenuColumn
            // 
            this.contextMenuColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.contextMenuColumn.HeaderText = "Add context menu item in Explorer";
            this.contextMenuColumn.Name = "contextMenuColumn";
            this.contextMenuColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // restoreCB
            // 
            this.restoreCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.restoreCB.AutoSize = true;
            this.restoreCB.ForeColor = System.Drawing.Color.White;
            this.restoreCB.Location = new System.Drawing.Point(6, 120);
            this.restoreCB.Name = "restoreCB";
            this.restoreCB.Size = new System.Drawing.Size(247, 17);
            this.restoreCB.TabIndex = 23;
            this.restoreCB.Text = "Restore file associations at DotA H.I.T. start-up";
            this.restoreCB.UseVisualStyleBackColor = true;
            // 
            // war3PathBrowseB
            // 
            this.war3PathBrowseB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.war3PathBrowseB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.war3PathBrowseB.Location = new System.Drawing.Point(366, 24);
            this.war3PathBrowseB.Name = "war3PathBrowseB";
            this.war3PathBrowseB.Size = new System.Drawing.Size(21, 23);
            this.war3PathBrowseB.TabIndex = 20;
            this.war3PathBrowseB.Text = "…";
            this.war3PathBrowseB.UseVisualStyleBackColor = true;
            this.war3PathBrowseB.Click += new System.EventHandler(this.war3PathBrowseB_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Specify a folder that contains .mpq files from WarCraft III, or the WarCraft III " +
                "game folder";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowserDialog.ShowNewFolderButton = false;
            // 
            // fontDialog1
            // 
            this.fontDialog1.FontMustExist = true;
            this.fontDialog1.ShowEffects = false;
            // 
            // blurryFontFixCB
            // 
            this.blurryFontFixCB.AutoSize = true;
            this.blurryFontFixCB.ForeColor = System.Drawing.Color.White;
            this.blurryFontFixCB.Location = new System.Drawing.Point(264, 221);
            this.blurryFontFixCB.Name = "blurryFontFixCB";
            this.blurryFontFixCB.Size = new System.Drawing.Size(126, 17);
            this.blurryFontFixCB.TabIndex = 31;
            this.blurryFontFixCB.Text = "Textbox blurry font fix";
            this.blurryFontFixCB.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(402, 440);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gamestate";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingsForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updateIntervalNumUD)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileTypesGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Button closeB;
        private System.Windows.Forms.TextBox war3PathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button applyB;
        private System.Windows.Forms.Button cancelB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button war3PathBrowseB;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.CheckBox restoreCB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView fileTypesGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn extensionCoumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn associateColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn contextMenuColumn;
        private System.Windows.Forms.CheckBox useOwnDialogsCB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.TextBox arialFontTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button chooseVerdanaFontB;
        private System.Windows.Forms.TextBox verdanaFontTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button chooseArialFontB;
        private System.Windows.Forms.CheckBox showDetailSwitchTipCB;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton updateAtStartupRB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown updateIntervalNumUD;
        private System.Windows.Forms.RadioButton updateAtStartupPerDaysRB;
        private System.Windows.Forms.RadioButton updateNeverRB;
        private System.Windows.Forms.CheckBox showUpdateSplashCB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox blurryFontFixCB;
    }
}