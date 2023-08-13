namespace DotaHIT
{
    partial class PropertiesForm
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
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.viewLoadingScreenB = new System.Windows.Forms.Button();
            this.mapImagePanel = new System.Windows.Forms.Panel();
            this.mapDescriptionRTB = new System.Windows.Forms.RichTextBox();
            this.mapNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filePathTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.filePathTextBox.Location = new System.Drawing.Point(68, 6);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.ReadOnly = true;
            this.filePathTextBox.Size = new System.Drawing.Size(652, 20);
            this.filePathTextBox.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "File path:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.viewLoadingScreenB);
            this.groupBox1.Controls.Add(this.mapImagePanel);
            this.groupBox1.Controls.Add(this.mapDescriptionRTB);
            this.groupBox1.Controls.Add(this.mapNameTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(12, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(708, 309);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map properties";
            // 
            // viewLoadingScreenB
            // 
            this.viewLoadingScreenB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewLoadingScreenB.Location = new System.Drawing.Point(23, 263);
            this.viewLoadingScreenB.Name = "viewLoadingScreenB";
            this.viewLoadingScreenB.Size = new System.Drawing.Size(417, 34);
            this.viewLoadingScreenB.TabIndex = 24;
            this.viewLoadingScreenB.Text = "view loading screen";
            this.viewLoadingScreenB.UseVisualStyleBackColor = true;
            this.viewLoadingScreenB.Click += new System.EventHandler(this.viewLoadingScreenB_Click);
            // 
            // mapImagePanel
            // 
            this.mapImagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mapImagePanel.BackColor = System.Drawing.Color.DimGray;
            this.mapImagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mapImagePanel.Location = new System.Drawing.Point(446, 41);
            this.mapImagePanel.Name = "mapImagePanel";
            this.mapImagePanel.Size = new System.Drawing.Size(256, 256);
            this.mapImagePanel.TabIndex = 23;
            // 
            // mapDescriptionRTB
            // 
            this.mapDescriptionRTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapDescriptionRTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(76)))));
            this.mapDescriptionRTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mapDescriptionRTB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mapDescriptionRTB.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.mapDescriptionRTB.Location = new System.Drawing.Point(23, 87);
            this.mapDescriptionRTB.Name = "mapDescriptionRTB";
            this.mapDescriptionRTB.ReadOnly = true;
            this.mapDescriptionRTB.Size = new System.Drawing.Size(417, 170);
            this.mapDescriptionRTB.TabIndex = 21;
            this.mapDescriptionRTB.Text = "";
            // 
            // mapNameTextBox
            // 
            this.mapNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(76)))));
            this.mapNameTextBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mapNameTextBox.ForeColor = System.Drawing.Color.White;
            this.mapNameTextBox.Location = new System.Drawing.Point(23, 41);
            this.mapNameTextBox.Name = "mapNameTextBox";
            this.mapNameTextBox.ReadOnly = true;
            this.mapNameTextBox.Size = new System.Drawing.Size(417, 21);
            this.mapNameTextBox.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(20, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Map title:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(20, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Map name:";
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(732, 353);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.filePathTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Current DotA Map Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mapNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox mapDescriptionRTB;
        private System.Windows.Forms.Panel mapImagePanel;
        private System.Windows.Forms.Button viewLoadingScreenB;
    }
}