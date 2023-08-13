namespace DotaHIT
{
    partial class LogForm
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
            this.logRTB = new System.Windows.Forms.RichTextBox();
            this.closeB = new System.Windows.Forms.Button();
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
            this.captionB.Size = new System.Drawing.Size(270, 26);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "Chat Log";
            this.captionB.UseVisualStyleBackColor = false;
            this.captionB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseDown);
            this.captionB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseMove);
            this.captionB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseUp);
            // 
            // logRTB
            // 
            this.logRTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logRTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(116)))), ((int)(((byte)(126)))));
            this.logRTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logRTB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.logRTB.ForeColor = System.Drawing.Color.White;
            this.logRTB.Location = new System.Drawing.Point(3, 29);
            this.logRTB.Margin = new System.Windows.Forms.Padding(0);
            this.logRTB.Name = "logRTB";
            this.logRTB.Size = new System.Drawing.Size(264, 238);
            this.logRTB.TabIndex = 14;
            this.logRTB.Text = "";
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackColor = System.Drawing.Color.Black;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeB.ForeColor = System.Drawing.Color.Silver;
            this.closeB.Location = new System.Drawing.Point(243, 3);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(23, 19);
            this.closeB.TabIndex = 15;
            this.closeB.Text = "x";
            this.closeB.UseVisualStyleBackColor = false;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // LogForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(270, 270);
            this.ControlBox = false;
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.logRTB);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gamestate";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.RichTextBox logRTB;
        private System.Windows.Forms.Button closeB;
    }
}