using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DotaHIT
{
    public partial class FolderBrowserForm : Form
    {
        private string initialDirectory = string.Empty;

        public FolderBrowserForm()
        {
            InitializeComponent();            
        }

        public string InitialDirectory
        {
            get
            {
                return initialDirectory;
            }
            set
            {
                initialDirectory = value;
            }
        }
        public string SelectedPath
        {
            get
            {
                return browser.SelectedPath;
            }
        }
        public string FileName
        {
            get
            {
                return string.Empty;
            }
            set
            {                
            }
        }
        public string Filter
        {
            get
            {
                return string.Empty;
            }
            set
            {                
            }
        }       

        public event CancelEventHandler FileOk;        

        private void browser_FolderOpenButtonClick(object sender, EventArgs e)
        {
            if (FileOk != null)
            {
                CancelEventArgs ce = new CancelEventArgs(false);
                FileOk(this, ce);

                if (ce.Cancel) return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void browser_FolderCancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }           

        public new DialogResult ShowDialog()
        {
            browser.SelectedPath = initialDirectory;
            DialogResult dr = base.ShowDialog();
            initialDirectory = browser.SelectedPath;

            return dr;
        }        
    }
}