namespace DotaHIT
{
    partial class FileBrowserForm
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
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.browser = new DotaHIT.Extras.FileBrowser();
            this.SuspendLayout();
            // 
            // browser
            // 
            this.browser.BrowseMode = DotaHIT.Extras.FileBrowser.BrowseModes.FileBrowser;
            this.browser.BrowserPanelBackColor = System.Drawing.SystemColors.Control;
            this.browser.BrowserPanelPadding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.ExplorerBorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.browser.FileName = "";
            this.browser.Location = new System.Drawing.Point(0, 0);
            this.browser.Name = "browser";
            this.browser.NavigationPanelBackColor = System.Drawing.SystemColors.Control;
            this.browser.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.browser.SelectedPath = "";
            this.browser.ShowExplorer = true;
            this.browser.ShowStatusBar = false;
            this.browser.ShowWar3MapsShortCut = true;
            this.browser.Size = new System.Drawing.Size(705, 378);
            this.browser.TabIndex = 1;
            this.browser.FileOk += new System.ComponentModel.CancelEventHandler(this.browser_FileOk);
            this.browser.FileCancelButtonClick += new System.EventHandler(this.browser_FileCancelButtonClick);
            // 
            // FileBrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 378);
            this.Controls.Add(this.browser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileBrowserForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DotA H.I.T. File Browser";
            this.ResumeLayout(false);

        }

        #endregion

        private DotaHIT.Extras.FileBrowser browser;
    }
}