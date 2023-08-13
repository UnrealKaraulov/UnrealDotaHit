namespace DotaHIT.Extras.Replay_Parser
{
    partial class ReplayDataExtractForm
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
            this.filenameTextBox = new System.Windows.Forms.TextBox();
            this.chatlogCB = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statisticsCB = new System.Windows.Forms.CheckBox();
            this.killLogCB = new System.Windows.Forms.CheckBox();
            this.okB = new System.Windows.Forms.Button();
            this.cancelB = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filenameTextBox
            // 
            this.filenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filenameTextBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.filenameTextBox.Location = new System.Drawing.Point(12, 12);
            this.filenameTextBox.Name = "filenameTextBox";
            this.filenameTextBox.ReadOnly = true;
            this.filenameTextBox.Size = new System.Drawing.Size(281, 20);
            this.filenameTextBox.TabIndex = 2;
            // 
            // chatlogCB
            // 
            this.chatlogCB.AutoSize = true;
            this.chatlogCB.Location = new System.Drawing.Point(10, 21);
            this.chatlogCB.Name = "chatlogCB";
            this.chatlogCB.Size = new System.Drawing.Size(69, 17);
            this.chatlogCB.TabIndex = 0;
            this.chatlogCB.Text = "Chat Log";
            this.chatlogCB.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.statisticsCB);
            this.groupBox1.Controls.Add(this.killLogCB);
            this.groupBox1.Controls.Add(this.chatlogCB);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 94);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Extract what?";
            // 
            // statisticsCB
            // 
            this.statisticsCB.AutoSize = true;
            this.statisticsCB.Location = new System.Drawing.Point(10, 67);
            this.statisticsCB.Name = "statisticsCB";
            this.statisticsCB.Size = new System.Drawing.Size(68, 17);
            this.statisticsCB.TabIndex = 2;
            this.statisticsCB.Text = "Statistics";
            this.statisticsCB.UseVisualStyleBackColor = true;
            // 
            // killLogCB
            // 
            this.killLogCB.AutoSize = true;
            this.killLogCB.Checked = true;
            this.killLogCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.killLogCB.Location = new System.Drawing.Point(10, 44);
            this.killLogCB.Name = "killLogCB";
            this.killLogCB.Size = new System.Drawing.Size(60, 17);
            this.killLogCB.TabIndex = 1;
            this.killLogCB.Text = "Kill Log";
            this.killLogCB.UseVisualStyleBackColor = true;
            // 
            // okB
            // 
            this.okB.Location = new System.Drawing.Point(12, 138);
            this.okB.Name = "okB";
            this.okB.Size = new System.Drawing.Size(192, 23);
            this.okB.TabIndex = 0;
            this.okB.Text = "OK";
            this.okB.UseVisualStyleBackColor = true;
            this.okB.Click += new System.EventHandler(this.okB_Click);
            // 
            // cancelB
            // 
            this.cancelB.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelB.Location = new System.Drawing.Point(210, 138);
            this.cancelB.Name = "cancelB";
            this.cancelB.Size = new System.Drawing.Size(83, 23);
            this.cancelB.TabIndex = 1;
            this.cancelB.Text = "Cancel";
            this.cancelB.UseVisualStyleBackColor = true;
            // 
            // ReplayDataExtractForm
            // 
            this.AcceptButton = this.okB;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelB;
            this.ClientSize = new System.Drawing.Size(305, 168);
            this.Controls.Add(this.cancelB);
            this.Controls.Add(this.okB);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.filenameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReplayDataExtractForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Extract Replay Data";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filenameTextBox;
        private System.Windows.Forms.CheckBox chatlogCB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox statisticsCB;
        private System.Windows.Forms.CheckBox killLogCB;
        private System.Windows.Forms.Button okB;
        private System.Windows.Forms.Button cancelB;
    }
}