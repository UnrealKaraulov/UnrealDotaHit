namespace DotaHIT
{
    partial class CustomKeysForm
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
            this.closeB = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.noteInfoB = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.hotkeyColorB = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.saveToWar3B = new System.Windows.Forms.Button();
            this.saveCKinfoB = new System.Windows.Forms.Button();
            this.saveCustomKeysB = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.openFromWar3B = new System.Windows.Forms.Button();
            this.loadCKinfoB = new System.Windows.Forms.Button();
            this.loadCustomKeysB = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.moveTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.stopTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.holdTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.attackTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.patrolTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.selectSkillTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cancelTextBox = new System.Windows.Forms.TextBox();
            this.saveCommandHotkeysCB = new System.Windows.Forms.CheckBox();
            this.commandGroupBox = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.defaultsB = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.commandGroupBox.SuspendLayout();
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
            this.captionB.Size = new System.Drawing.Size(488, 26);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "CustomKeys Generator";
            this.captionB.UseVisualStyleBackColor = false;
            this.captionB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseDown);
            this.captionB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseMove);
            this.captionB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseUp);
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackColor = System.Drawing.Color.Black;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeB.ForeColor = System.Drawing.Color.Silver;
            this.closeB.Location = new System.Drawing.Point(461, 3);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(23, 19);
            this.closeB.TabIndex = 15;
            this.closeB.Text = "x";
            this.closeB.UseVisualStyleBackColor = false;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.RoyalBlue;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(475, 47);
            this.label1.TabIndex = 16;
            this.label1.Text = "DotaHIT is now in CustomKeys-Generation mode. If you want to return to original m" +
                "ode, then close this window.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.panel1.Controls.Add(this.commandGroupBox);
            this.panel1.Controls.Add(this.saveCommandHotkeysCB);
            this.panel1.Controls.Add(this.noteInfoB);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.hotkeyColorB);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(6, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(476, 289);
            this.panel1.TabIndex = 18;
            // 
            // noteInfoB
            // 
            this.noteInfoB.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.noteInfoB.ForeColor = System.Drawing.Color.Black;
            this.noteInfoB.Location = new System.Drawing.Point(444, 163);
            this.noteInfoB.Name = "noteInfoB";
            this.noteInfoB.Size = new System.Drawing.Size(17, 20);
            this.noteInfoB.TabIndex = 23;
            this.noteInfoB.Text = "?";
            this.noteInfoB.UseVisualStyleBackColor = true;
            this.noteInfoB.Click += new System.EventHandler(this.noteInfoB_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.GreenYellow;
            this.label2.Location = new System.Drawing.Point(8, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(461, 14);
            this.label2.TabIndex = 22;
            this.label2.Text = "NOTE: Some abilities may have interchangeable clones, which must be saved as well" +
                "";
            // 
            // hotkeyColorB
            // 
            this.hotkeyColorB.BackColor = System.Drawing.Color.DarkSlateGray;
            this.hotkeyColorB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hotkeyColorB.ForeColor = System.Drawing.Color.DodgerBlue;
            this.hotkeyColorB.Location = new System.Drawing.Point(173, 75);
            this.hotkeyColorB.Name = "hotkeyColorB";
            this.hotkeyColorB.Size = new System.Drawing.Size(129, 27);
            this.hotkeyColorB.TabIndex = 19;
            this.hotkeyColorB.Text = "change hotkey color";
            this.hotkeyColorB.UseVisualStyleBackColor = false;
            this.hotkeyColorB.Click += new System.EventHandler(this.hotkeyColorB_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(8, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(461, 28);
            this.label4.TabIndex = 21;
            this.label4.Text = "You can change any skill\'s hotkey by clicking on it and then pressing the button " +
                "on the keyboard. Just go through all the heroes and set your hotkeys.";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.saveToWar3B);
            this.groupBox2.Controls.Add(this.saveCKinfoB);
            this.groupBox2.Controls.Add(this.saveCustomKeysB);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(307, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 73);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Generate CustomKeys";
            // 
            // saveToWar3B
            // 
            this.saveToWar3B.ForeColor = System.Drawing.Color.Black;
            this.saveToWar3B.Location = new System.Drawing.Point(7, 48);
            this.saveToWar3B.Name = "saveToWar3B";
            this.saveToWar3B.Size = new System.Drawing.Size(117, 20);
            this.saveToWar3B.TabIndex = 22;
            this.saveToWar3B.Text = "save to war3 folder";
            this.saveToWar3B.UseVisualStyleBackColor = true;
            this.saveToWar3B.Click += new System.EventHandler(this.saveToWar3B_Click);
            // 
            // saveCKinfoB
            // 
            this.saveCKinfoB.ForeColor = System.Drawing.Color.Black;
            this.saveCKinfoB.Location = new System.Drawing.Point(130, 23);
            this.saveCKinfoB.Name = "saveCKinfoB";
            this.saveCKinfoB.Size = new System.Drawing.Size(24, 23);
            this.saveCKinfoB.TabIndex = 22;
            this.saveCKinfoB.Text = "?";
            this.saveCKinfoB.UseVisualStyleBackColor = true;
            this.saveCKinfoB.Click += new System.EventHandler(this.saveCKinfoB_Click);
            // 
            // saveCustomKeysB
            // 
            this.saveCustomKeysB.ForeColor = System.Drawing.Color.Black;
            this.saveCustomKeysB.Location = new System.Drawing.Point(7, 23);
            this.saveCustomKeysB.Name = "saveCustomKeysB";
            this.saveCustomKeysB.Size = new System.Drawing.Size(117, 23);
            this.saveCustomKeysB.TabIndex = 18;
            this.saveCustomKeysB.Text = "Save as...";
            this.saveCustomKeysB.UseVisualStyleBackColor = true;
            this.saveCustomKeysB.Click += new System.EventHandler(this.saveCustomKeysB_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.openFromWar3B);
            this.groupBox1.Controls.Add(this.loadCKinfoB);
            this.groupBox1.Controls.Add(this.loadCustomKeysB);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(6, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(162, 73);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load CustomKeys";
            // 
            // openFromWar3B
            // 
            this.openFromWar3B.ForeColor = System.Drawing.Color.Black;
            this.openFromWar3B.Location = new System.Drawing.Point(7, 48);
            this.openFromWar3B.Name = "openFromWar3B";
            this.openFromWar3B.Size = new System.Drawing.Size(117, 20);
            this.openFromWar3B.TabIndex = 23;
            this.openFromWar3B.Text = "open from war3 folder";
            this.openFromWar3B.UseVisualStyleBackColor = true;
            this.openFromWar3B.Click += new System.EventHandler(this.openFromWar3B_Click);
            // 
            // loadCKinfoB
            // 
            this.loadCKinfoB.ForeColor = System.Drawing.Color.Black;
            this.loadCKinfoB.Location = new System.Drawing.Point(130, 23);
            this.loadCKinfoB.Name = "loadCKinfoB";
            this.loadCKinfoB.Size = new System.Drawing.Size(24, 23);
            this.loadCKinfoB.TabIndex = 19;
            this.loadCKinfoB.Text = "?";
            this.loadCKinfoB.UseVisualStyleBackColor = true;
            this.loadCKinfoB.Click += new System.EventHandler(this.loadCKinfoB_Click);
            // 
            // loadCustomKeysB
            // 
            this.loadCustomKeysB.ForeColor = System.Drawing.Color.Black;
            this.loadCustomKeysB.Location = new System.Drawing.Point(7, 23);
            this.loadCustomKeysB.Name = "loadCustomKeysB";
            this.loadCustomKeysB.Size = new System.Drawing.Size(117, 23);
            this.loadCustomKeysB.TabIndex = 18;
            this.loadCustomKeysB.Text = "Open CustomKeys";
            this.loadCustomKeysB.UseVisualStyleBackColor = true;
            this.loadCustomKeysB.Click += new System.EventHandler(this.loadCustomKeysB_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Filter = "txt files|*.txt|All files|*.*";
            this.saveFileDialog.Title = "Save custom keys as...";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "txt";
            this.openFileDialog.Filter = "txt files|*.txt|All files|*.*";
            this.openFileDialog.Title = "Open custom keys file...";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // moveTextBox
            // 
            this.moveTextBox.BackColor = System.Drawing.Color.Azure;
            this.moveTextBox.Location = new System.Drawing.Point(14, 31);
            this.moveTextBox.Name = "moveTextBox";
            this.moveTextBox.Size = new System.Drawing.Size(26, 20);
            this.moveTextBox.TabIndex = 25;
            this.moveTextBox.Tag = "M";
            this.moveTextBox.Text = "M";
            this.moveTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(9, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Move";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(63, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Stop";
            // 
            // stopTextBox
            // 
            this.stopTextBox.BackColor = System.Drawing.Color.Azure;
            this.stopTextBox.Location = new System.Drawing.Point(66, 31);
            this.stopTextBox.Name = "stopTextBox";
            this.stopTextBox.Size = new System.Drawing.Size(26, 20);
            this.stopTextBox.TabIndex = 27;
            this.stopTextBox.Tag = "S";
            this.stopTextBox.Text = "S";
            this.stopTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(111, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Hold Position";
            // 
            // holdTextBox
            // 
            this.holdTextBox.BackColor = System.Drawing.Color.Azure;
            this.holdTextBox.Location = new System.Drawing.Point(137, 31);
            this.holdTextBox.Name = "holdTextBox";
            this.holdTextBox.Size = new System.Drawing.Size(26, 20);
            this.holdTextBox.TabIndex = 29;
            this.holdTextBox.Tag = "H";
            this.holdTextBox.Text = "H";
            this.holdTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(204, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Attack";
            // 
            // attackTextBox
            // 
            this.attackTextBox.BackColor = System.Drawing.Color.Azure;
            this.attackTextBox.Location = new System.Drawing.Point(212, 31);
            this.attackTextBox.Name = "attackTextBox";
            this.attackTextBox.Size = new System.Drawing.Size(26, 20);
            this.attackTextBox.TabIndex = 31;
            this.attackTextBox.Tag = "A";
            this.attackTextBox.Text = "A";
            this.attackTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(266, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Patrol";
            // 
            // patrolTextBox
            // 
            this.patrolTextBox.BackColor = System.Drawing.Color.Azure;
            this.patrolTextBox.Location = new System.Drawing.Point(273, 31);
            this.patrolTextBox.Name = "patrolTextBox";
            this.patrolTextBox.Size = new System.Drawing.Size(26, 20);
            this.patrolTextBox.TabIndex = 33;
            this.patrolTextBox.Tag = "P";
            this.patrolTextBox.Text = "P";
            this.patrolTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(322, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "Hero Abilities";
            // 
            // selectSkillTextBox
            // 
            this.selectSkillTextBox.BackColor = System.Drawing.Color.Azure;
            this.selectSkillTextBox.Location = new System.Drawing.Point(350, 31);
            this.selectSkillTextBox.Name = "selectSkillTextBox";
            this.selectSkillTextBox.Size = new System.Drawing.Size(26, 20);
            this.selectSkillTextBox.TabIndex = 35;
            this.selectSkillTextBox.Tag = "O";
            this.selectSkillTextBox.Text = "O";
            this.selectSkillTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(410, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = "Cancel";
            // 
            // cancelTextBox
            // 
            this.cancelTextBox.BackColor = System.Drawing.Color.Azure;
            this.cancelTextBox.Location = new System.Drawing.Point(421, 31);
            this.cancelTextBox.Name = "cancelTextBox";
            this.cancelTextBox.Size = new System.Drawing.Size(26, 20);
            this.cancelTextBox.TabIndex = 37;
            this.cancelTextBox.Tag = "512";
            this.cancelTextBox.Text = "ESC";
            this.cancelTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // saveCommandHotkeysCB
            // 
            this.saveCommandHotkeysCB.AutoSize = true;
            this.saveCommandHotkeysCB.Checked = true;
            this.saveCommandHotkeysCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveCommandHotkeysCB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveCommandHotkeysCB.ForeColor = System.Drawing.Color.White;
            this.saveCommandHotkeysCB.Location = new System.Drawing.Point(156, 185);
            this.saveCommandHotkeysCB.Name = "saveCommandHotkeysCB";
            this.saveCommandHotkeysCB.Size = new System.Drawing.Size(164, 17);
            this.saveCommandHotkeysCB.TabIndex = 39;
            this.saveCommandHotkeysCB.Text = "Save command hotkeys";
            this.saveCommandHotkeysCB.UseVisualStyleBackColor = true;
            this.saveCommandHotkeysCB.CheckedChanged += new System.EventHandler(this.saveCommandHotkeysCB_CheckedChanged);
            // 
            // commandGroupBox
            // 
            this.commandGroupBox.Controls.Add(this.defaultsB);
            this.commandGroupBox.Controls.Add(this.label11);
            this.commandGroupBox.Controls.Add(this.stopTextBox);
            this.commandGroupBox.Controls.Add(this.attackTextBox);
            this.commandGroupBox.Controls.Add(this.label10);
            this.commandGroupBox.Controls.Add(this.label6);
            this.commandGroupBox.Controls.Add(this.label7);
            this.commandGroupBox.Controls.Add(this.cancelTextBox);
            this.commandGroupBox.Controls.Add(this.holdTextBox);
            this.commandGroupBox.Controls.Add(this.moveTextBox);
            this.commandGroupBox.Controls.Add(this.patrolTextBox);
            this.commandGroupBox.Controls.Add(this.label9);
            this.commandGroupBox.Controls.Add(this.label5);
            this.commandGroupBox.Controls.Add(this.label3);
            this.commandGroupBox.Controls.Add(this.label8);
            this.commandGroupBox.Controls.Add(this.selectSkillTextBox);
            this.commandGroupBox.Location = new System.Drawing.Point(6, 202);
            this.commandGroupBox.Name = "commandGroupBox";
            this.commandGroupBox.Size = new System.Drawing.Size(463, 81);
            this.commandGroupBox.TabIndex = 40;
            this.commandGroupBox.TabStop = false;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(7, 57);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(405, 21);
            this.label11.TabIndex = 39;
            this.label11.Text = "Click on a textbox and then press a key. Vaild hotkeys: A-Z, ESC.";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // defaultsB
            // 
            this.defaultsB.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.defaultsB.ForeColor = System.Drawing.Color.Black;
            this.defaultsB.Location = new System.Drawing.Point(396, 57);
            this.defaultsB.Name = "defaultsB";
            this.defaultsB.Size = new System.Drawing.Size(61, 20);
            this.defaultsB.TabIndex = 40;
            this.defaultsB.Text = "set defaults";
            this.defaultsB.UseVisualStyleBackColor = true;
            this.defaultsB.Click += new System.EventHandler(this.defaultsB_Click);
            // 
            // CustomKeysForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(488, 326);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomKeysForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gamestate";
            this.VisibleChanged += new System.EventHandler(this.CustomKeysForm_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.commandGroupBox.ResumeLayout(false);
            this.commandGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Button closeB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button loadCustomKeysB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button saveCustomKeysB;
        private System.Windows.Forms.ColorDialog colorDialog;
        internal System.Windows.Forms.Button hotkeyColorB;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button loadCKinfoB;
        private System.Windows.Forms.Button saveCKinfoB;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button saveToWar3B;
        private System.Windows.Forms.Button openFromWar3B;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button noteInfoB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox moveTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox selectSkillTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox patrolTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox attackTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox holdTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox stopTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox cancelTextBox;
        private System.Windows.Forms.CheckBox saveCommandHotkeysCB;
        private System.Windows.Forms.GroupBox commandGroupBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button defaultsB;
    }
}