namespace DotaHIT
{
    partial class DataDumpForm
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
            this.fullRB = new System.Windows.Forms.RadioButton();
            this.compactRB = new System.Windows.Forms.RadioButton();
            this.dumpB = new System.Windows.Forms.Button();
            this.compactGroupBox = new System.Windows.Forms.GroupBox();
            this.heroesCB = new System.Windows.Forms.CheckBox();
            this.heroesAbilsCB = new System.Windows.Forms.CheckBox();
            this.fullGroupBox = new System.Windows.Forms.GroupBox();
            this.unitsCB = new System.Windows.Forms.CheckBox();
            this.abilitiesCB = new System.Windows.Forms.CheckBox();
            this.itemsCB = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.upgradesCB = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.compactGroupBox.SuspendLayout();
            this.fullGroupBox.SuspendLayout();
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
            this.captionB.Size = new System.Drawing.Size(310, 26);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "Data Dump";
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
            this.closeB.Location = new System.Drawing.Point(283, 3);
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
            this.panel1.Controls.Add(this.fullRB);
            this.panel1.Controls.Add(this.compactRB);
            this.panel1.Controls.Add(this.dumpB);
            this.panel1.Controls.Add(this.compactGroupBox);
            this.panel1.Controls.Add(this.fullGroupBox);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(6, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(298, 227);
            this.panel1.TabIndex = 18;
            // 
            // fullRB
            // 
            this.fullRB.AutoSize = true;
            this.fullRB.BackColor = System.Drawing.Color.Transparent;
            this.fullRB.ForeColor = System.Drawing.Color.White;
            this.fullRB.Location = new System.Drawing.Point(171, 47);
            this.fullRB.Name = "fullRB";
            this.fullRB.Size = new System.Drawing.Size(41, 17);
            this.fullRB.TabIndex = 26;
            this.fullRB.Text = "Full";
            this.fullRB.UseVisualStyleBackColor = false;
            this.fullRB.CheckedChanged += new System.EventHandler(this.anyRB_CheckedChanged);
            // 
            // compactRB
            // 
            this.compactRB.AutoSize = true;
            this.compactRB.BackColor = System.Drawing.Color.Transparent;
            this.compactRB.ForeColor = System.Drawing.Color.White;
            this.compactRB.Location = new System.Drawing.Point(20, 47);
            this.compactRB.Name = "compactRB";
            this.compactRB.Size = new System.Drawing.Size(67, 17);
            this.compactRB.TabIndex = 27;
            this.compactRB.Text = "Compact";
            this.compactRB.UseVisualStyleBackColor = false;
            this.compactRB.CheckedChanged += new System.EventHandler(this.anyRB_CheckedChanged);
            // 
            // dumpB
            // 
            this.dumpB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dumpB.Location = new System.Drawing.Point(67, 192);
            this.dumpB.Name = "dumpB";
            this.dumpB.Size = new System.Drawing.Size(165, 23);
            this.dumpB.TabIndex = 32;
            this.dumpB.Text = "Dump";
            this.dumpB.UseVisualStyleBackColor = true;
            this.dumpB.Click += new System.EventHandler(this.dumpB_Click);
            // 
            // compactGroupBox
            // 
            this.compactGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.compactGroupBox.Controls.Add(this.heroesCB);
            this.compactGroupBox.Controls.Add(this.heroesAbilsCB);
            this.compactGroupBox.Location = new System.Drawing.Point(13, 50);
            this.compactGroupBox.Name = "compactGroupBox";
            this.compactGroupBox.Size = new System.Drawing.Size(134, 127);
            this.compactGroupBox.TabIndex = 31;
            this.compactGroupBox.TabStop = false;
            // 
            // heroesCB
            // 
            this.heroesCB.AutoSize = true;
            this.heroesCB.ForeColor = System.Drawing.Color.White;
            this.heroesCB.Location = new System.Drawing.Point(14, 26);
            this.heroesCB.Name = "heroesCB";
            this.heroesCB.Size = new System.Drawing.Size(60, 17);
            this.heroesCB.TabIndex = 28;
            this.heroesCB.Text = "Heroes";
            this.heroesCB.UseVisualStyleBackColor = true;
            // 
            // heroesAbilsCB
            // 
            this.heroesAbilsCB.AutoSize = true;
            this.heroesAbilsCB.ForeColor = System.Drawing.Color.White;
            this.heroesAbilsCB.Location = new System.Drawing.Point(14, 49);
            this.heroesAbilsCB.Name = "heroesAbilsCB";
            this.heroesAbilsCB.Size = new System.Drawing.Size(98, 17);
            this.heroesAbilsCB.TabIndex = 29;
            this.heroesAbilsCB.Text = "Heroes Abilities";
            this.heroesAbilsCB.UseVisualStyleBackColor = true;
            // 
            // fullGroupBox
            // 
            this.fullGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.fullGroupBox.Controls.Add(this.upgradesCB);
            this.fullGroupBox.Controls.Add(this.unitsCB);
            this.fullGroupBox.Controls.Add(this.abilitiesCB);
            this.fullGroupBox.Controls.Add(this.itemsCB);
            this.fullGroupBox.Location = new System.Drawing.Point(164, 50);
            this.fullGroupBox.Name = "fullGroupBox";
            this.fullGroupBox.Size = new System.Drawing.Size(122, 127);
            this.fullGroupBox.TabIndex = 30;
            this.fullGroupBox.TabStop = false;
            // 
            // unitsCB
            // 
            this.unitsCB.AutoSize = true;
            this.unitsCB.ForeColor = System.Drawing.Color.White;
            this.unitsCB.Location = new System.Drawing.Point(16, 26);
            this.unitsCB.Name = "unitsCB";
            this.unitsCB.Size = new System.Drawing.Size(50, 17);
            this.unitsCB.TabIndex = 23;
            this.unitsCB.Text = "Units";
            this.unitsCB.UseVisualStyleBackColor = true;
            // 
            // abilitiesCB
            // 
            this.abilitiesCB.AutoSize = true;
            this.abilitiesCB.ForeColor = System.Drawing.Color.White;
            this.abilitiesCB.Location = new System.Drawing.Point(16, 49);
            this.abilitiesCB.Name = "abilitiesCB";
            this.abilitiesCB.Size = new System.Drawing.Size(61, 17);
            this.abilitiesCB.TabIndex = 24;
            this.abilitiesCB.Text = "Abilities";
            this.abilitiesCB.UseVisualStyleBackColor = true;
            // 
            // itemsCB
            // 
            this.itemsCB.AutoSize = true;
            this.itemsCB.ForeColor = System.Drawing.Color.White;
            this.itemsCB.Location = new System.Drawing.Point(16, 72);
            this.itemsCB.Name = "itemsCB";
            this.itemsCB.Size = new System.Drawing.Size(51, 17);
            this.itemsCB.TabIndex = 25;
            this.itemsCB.Text = "Items";
            this.itemsCB.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(28, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(242, 36);
            this.label4.TabIndex = 22;
            this.label4.Text = "With this feature you can dump DotaHIT\'s current database into text files";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // upgradesCB
            // 
            this.upgradesCB.AutoSize = true;
            this.upgradesCB.ForeColor = System.Drawing.Color.White;
            this.upgradesCB.Location = new System.Drawing.Point(16, 95);
            this.upgradesCB.Name = "upgradesCB";
            this.upgradesCB.Size = new System.Drawing.Size(72, 17);
            this.upgradesCB.TabIndex = 26;
            this.upgradesCB.Text = "Upgrades";
            this.upgradesCB.UseVisualStyleBackColor = true;
            // 
            // DataDumpForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(310, 264);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataDumpForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gamestate";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.compactGroupBox.ResumeLayout(false);
            this.compactGroupBox.PerformLayout();
            this.fullGroupBox.ResumeLayout(false);
            this.fullGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Button closeB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox itemsCB;
        private System.Windows.Forms.CheckBox abilitiesCB;
        private System.Windows.Forms.CheckBox unitsCB;
        private System.Windows.Forms.CheckBox heroesAbilsCB;
        private System.Windows.Forms.CheckBox heroesCB;
        private System.Windows.Forms.RadioButton compactRB;
        private System.Windows.Forms.RadioButton fullRB;
        private System.Windows.Forms.GroupBox compactGroupBox;
        private System.Windows.Forms.GroupBox fullGroupBox;
        private System.Windows.Forms.Button dumpB;
        private System.Windows.Forms.CheckBox upgradesCB;
    }
}