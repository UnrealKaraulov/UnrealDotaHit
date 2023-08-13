namespace DotaHIT
{
    partial class ActionsForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.deathsCmbB = new System.Windows.Forms.ComboBox();
            this.killedByRB = new System.Windows.Forms.RadioButton();
            this.suicideRB = new System.Windows.Forms.RadioButton();
            this.dieB = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.streakCmbB = new System.Windows.Forms.ComboBox();
            this.heroLevelNumUD = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.heroesNumUD = new System.Windows.Forms.NumericUpDown();
            this.killHeroB = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.creepLevelTextBox = new System.Windows.Forms.TextBox();
            this.creepGoldTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.creepArtPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.creepsNumUD = new System.Windows.Forms.NumericUpDown();
            this.creepsCmbB = new System.Windows.Forms.ComboBox();
            this.killCreepB = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heroLevelNumUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heroesNumUD)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.creepsNumUD)).BeginInit();
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
            this.captionB.Size = new System.Drawing.Size(309, 26);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "Hero Actions";
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
            this.closeB.Location = new System.Drawing.Point(282, 3);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(23, 19);
            this.closeB.TabIndex = 15;
            this.closeB.Text = "x";
            this.closeB.UseVisualStyleBackColor = false;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(4, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 302);
            this.panel1.TabIndex = 16;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.deathsCmbB);
            this.groupBox3.Controls.Add(this.killedByRB);
            this.groupBox3.Controls.Add(this.suicideRB);
            this.groupBox3.Controls.Add(this.dieB);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Location = new System.Drawing.Point(8, 220);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(285, 76);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Die";
            // 
            // deathsCmbB
            // 
            this.deathsCmbB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.deathsCmbB.BackColor = System.Drawing.Color.White;
            this.deathsCmbB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.deathsCmbB.FormattingEnabled = true;
            this.deathsCmbB.Items.AddRange(new object[] {
            "enemy hero",
            "allied hero",
            "creep (enemy)",
            "creep (neutral)",
            "unknown source"});
            this.deathsCmbB.Location = new System.Drawing.Point(150, 17);
            this.deathsCmbB.Name = "deathsCmbB";
            this.deathsCmbB.Size = new System.Drawing.Size(129, 21);
            this.deathsCmbB.TabIndex = 23;
            this.deathsCmbB.SelectedIndexChanged += new System.EventHandler(this.deathsCmbB_SelectedIndexChanged);
            // 
            // killedByRB
            // 
            this.killedByRB.Location = new System.Drawing.Point(75, 19);
            this.killedByRB.Name = "killedByRB";
            this.killedByRB.Size = new System.Drawing.Size(76, 17);
            this.killedByRB.TabIndex = 21;
            this.killedByRB.TabStop = true;
            this.killedByRB.Text = "killed by:";
            this.killedByRB.UseVisualStyleBackColor = true;
            // 
            // suicideRB
            // 
            this.suicideRB.AutoSize = true;
            this.suicideRB.Location = new System.Drawing.Point(9, 19);
            this.suicideRB.Name = "suicideRB";
            this.suicideRB.Size = new System.Drawing.Size(65, 17);
            this.suicideRB.TabIndex = 20;
            this.suicideRB.TabStop = true;
            this.suicideRB.Text = "suicide";
            this.suicideRB.UseVisualStyleBackColor = true;
            this.suicideRB.CheckedChanged += new System.EventHandler(this.suicideRB_CheckedChanged);
            // 
            // dieB
            // 
            this.dieB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dieB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dieB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.dieB.ForeColor = System.Drawing.Color.Black;
            this.dieB.Location = new System.Drawing.Point(6, 44);
            this.dieB.Name = "dieB";
            this.dieB.Size = new System.Drawing.Size(273, 23);
            this.dieB.TabIndex = 19;
            this.dieB.Text = "Die";
            this.dieB.UseVisualStyleBackColor = true;
            this.dieB.Click += new System.EventHandler(this.dieB_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.streakCmbB);
            this.groupBox2.Controls.Add(this.heroLevelNumUD);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.heroesNumUD);
            this.groupBox2.Controls.Add(this.killHeroB);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(8, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(285, 75);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Kill a Hero";
            // 
            // streakCmbB
            // 
            this.streakCmbB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.streakCmbB.BackColor = System.Drawing.Color.White;
            this.streakCmbB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.streakCmbB.FormattingEnabled = true;
            this.streakCmbB.Items.AddRange(new object[] {
            "no killing streak",
            "killing spree",
            "dominating",
            "mega kill",
            "unstoppable",
            "wicked sick",
            "monster kill",
            "GODLIKE",
            "beyond GODLIKE"});
            this.streakCmbB.Location = new System.Drawing.Point(132, 18);
            this.streakCmbB.Name = "streakCmbB";
            this.streakCmbB.Size = new System.Drawing.Size(147, 21);
            this.streakCmbB.TabIndex = 22;
            // 
            // heroLevelNumUD
            // 
            this.heroLevelNumUD.BackColor = System.Drawing.Color.White;
            this.heroLevelNumUD.Location = new System.Drawing.Point(61, 18);
            this.heroLevelNumUD.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.heroLevelNumUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.heroLevelNumUD.Name = "heroLevelNumUD";
            this.heroLevelNumUD.Size = new System.Drawing.Size(65, 20);
            this.heroLevelNumUD.TabIndex = 22;
            this.heroLevelNumUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "amount:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "level:";
            // 
            // heroesNumUD
            // 
            this.heroesNumUD.Location = new System.Drawing.Point(61, 44);
            this.heroesNumUD.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.heroesNumUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.heroesNumUD.Name = "heroesNumUD";
            this.heroesNumUD.Size = new System.Drawing.Size(65, 20);
            this.heroesNumUD.TabIndex = 21;
            this.heroesNumUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // killHeroB
            // 
            this.killHeroB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.killHeroB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.killHeroB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.killHeroB.ForeColor = System.Drawing.Color.Black;
            this.killHeroB.Location = new System.Drawing.Point(132, 42);
            this.killHeroB.Name = "killHeroB";
            this.killHeroB.Size = new System.Drawing.Size(147, 23);
            this.killHeroB.TabIndex = 19;
            this.killHeroB.Text = "kill";
            this.killHeroB.UseVisualStyleBackColor = true;
            this.killHeroB.Click += new System.EventHandler(this.killHeroB_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.creepLevelTextBox);
            this.groupBox1.Controls.Add(this.creepGoldTextBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.creepArtPanel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.creepsNumUD);
            this.groupBox1.Controls.Add(this.creepsCmbB);
            this.groupBox1.Controls.Add(this.killCreepB);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(285, 126);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Kill a Creep";
            // 
            // creepLevelTextBox
            // 
            this.creepLevelTextBox.BackColor = System.Drawing.Color.SteelBlue;
            this.creepLevelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.creepLevelTextBox.ForeColor = System.Drawing.Color.White;
            this.creepLevelTextBox.Location = new System.Drawing.Point(176, 73);
            this.creepLevelTextBox.Name = "creepLevelTextBox";
            this.creepLevelTextBox.ReadOnly = true;
            this.creepLevelTextBox.Size = new System.Drawing.Size(103, 13);
            this.creepLevelTextBox.TabIndex = 31;
            // 
            // creepGoldTextBox
            // 
            this.creepGoldTextBox.BackColor = System.Drawing.Color.SteelBlue;
            this.creepGoldTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.creepGoldTextBox.ForeColor = System.Drawing.Color.White;
            this.creepGoldTextBox.Location = new System.Drawing.Point(176, 49);
            this.creepGoldTextBox.Name = "creepGoldTextBox";
            this.creepGoldTextBox.ReadOnly = true;
            this.creepGoldTextBox.Size = new System.Drawing.Size(103, 13);
            this.creepGoldTextBox.TabIndex = 30;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(132, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "level:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(135, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "gold:";
            // 
            // creepArtPanel
            // 
            this.creepArtPanel.BackColor = System.Drawing.Color.Silver;
            this.creepArtPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.creepArtPanel.Location = new System.Drawing.Point(61, 45);
            this.creepArtPanel.Name = "creepArtPanel";
            this.creepArtPanel.Size = new System.Drawing.Size(48, 48);
            this.creepArtPanel.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "art:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "amount:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "type:";
            // 
            // creepsNumUD
            // 
            this.creepsNumUD.BackColor = System.Drawing.Color.White;
            this.creepsNumUD.Location = new System.Drawing.Point(61, 99);
            this.creepsNumUD.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.creepsNumUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.creepsNumUD.Name = "creepsNumUD";
            this.creepsNumUD.Size = new System.Drawing.Size(65, 20);
            this.creepsNumUD.TabIndex = 21;
            this.creepsNumUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // creepsCmbB
            // 
            this.creepsCmbB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.creepsCmbB.BackColor = System.Drawing.Color.White;
            this.creepsCmbB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.creepsCmbB.FormattingEnabled = true;
            this.creepsCmbB.Location = new System.Drawing.Point(61, 17);
            this.creepsCmbB.Name = "creepsCmbB";
            this.creepsCmbB.Size = new System.Drawing.Size(218, 21);
            this.creepsCmbB.Sorted = true;
            this.creepsCmbB.TabIndex = 20;
            this.creepsCmbB.SelectedIndexChanged += new System.EventHandler(this.creepsCmbB_SelectedIndexChanged);
            this.creepsCmbB.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.creepsCmbB_Format);
            // 
            // killCreepB
            // 
            this.killCreepB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.killCreepB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.killCreepB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.killCreepB.ForeColor = System.Drawing.Color.Black;
            this.killCreepB.Location = new System.Drawing.Point(132, 97);
            this.killCreepB.Name = "killCreepB";
            this.killCreepB.Size = new System.Drawing.Size(147, 23);
            this.killCreepB.TabIndex = 19;
            this.killCreepB.Text = "kill";
            this.killCreepB.UseVisualStyleBackColor = true;
            this.killCreepB.Click += new System.EventHandler(this.killCreepB_Click);
            // 
            // ActionsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(309, 335);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gamestate";
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heroLevelNumUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heroesNumUD)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.creepsNumUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Button closeB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button killCreepB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox creepsCmbB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown creepsNumUD;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown heroesNumUD;
        private System.Windows.Forms.Button killHeroB;
        private System.Windows.Forms.NumericUpDown heroLevelNumUD;
        private System.Windows.Forms.ComboBox streakCmbB;
        private System.Windows.Forms.Panel creepArtPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox creepGoldTextBox;
        private System.Windows.Forms.TextBox creepLevelTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton killedByRB;
        private System.Windows.Forms.RadioButton suicideRB;
        private System.Windows.Forms.Button dieB;
        private System.Windows.Forms.ComboBox deathsCmbB;
    }
}