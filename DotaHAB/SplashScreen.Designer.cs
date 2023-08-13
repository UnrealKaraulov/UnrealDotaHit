namespace DotaHIT
{
    partial class SplashScreen
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
            this.loadPrgB = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.loadStateLabel = new System.Windows.Forms.Label();
            this.stopB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadPrgB
            // 
            this.loadPrgB.Location = new System.Drawing.Point(45, 75);
            this.loadPrgB.Name = "loadPrgB";
            this.loadPrgB.Size = new System.Drawing.Size(203, 11);
            this.loadPrgB.Step = 1;
            this.loadPrgB.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(78, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "DotA H.I.T.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(119, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "by Danat";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loadStateLabel
            // 
            this.loadStateLabel.BackColor = System.Drawing.Color.Transparent;
            this.loadStateLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadStateLabel.ForeColor = System.Drawing.Color.White;
            this.loadStateLabel.Location = new System.Drawing.Point(45, 89);
            this.loadStateLabel.Name = "loadStateLabel";
            this.loadStateLabel.Size = new System.Drawing.Size(203, 15);
            this.loadStateLabel.TabIndex = 3;
            this.loadStateLabel.Text = "loading...";
            this.loadStateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // stopB
            // 
            this.stopB.Location = new System.Drawing.Point(213, 53);
            this.stopB.Name = "stopB";
            this.stopB.Size = new System.Drawing.Size(35, 22);
            this.stopB.TabIndex = 4;
            this.stopB.Text = "stop";
            this.stopB.UseVisualStyleBackColor = true;
            this.stopB.Visible = false;
            this.stopB.Click += new System.EventHandler(this.stopB_Click);
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGreen;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(292, 107);
            this.ControlBox = false;
            this.Controls.Add(this.stopB);
            this.Controls.Add(this.loadStateLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.loadPrgB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label loadStateLabel;
        internal System.Windows.Forms.ProgressBar loadPrgB;
        private System.Windows.Forms.Button stopB;
    }
}