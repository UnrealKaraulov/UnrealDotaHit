using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DotaHIT
{
    public partial class FileBrowserForm : Form
    {
        private string initialDirectory = string.Empty;

        public FileBrowserForm()
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
        public string SelectedFile
        {
            get
            {
                return browser.SelectedFile;
            }
        }
        public string FileName
        {
            get
            {
                return browser.FileName;
            }
            set
            {
                browser.FileName = value;
            }
        }
        public string Filter
        {
            get
            {
                return browser.Filter;
            }
            set
            {
                browser.Filter = value;
            }
        }       

        public event CancelEventHandler FileOk;

        private void browser_FileOk(object sender, EventArgs e)
        {           
            if (FileOk!= null)
            {
                CancelEventArgs ce = new CancelEventArgs(false);
                FileOk(this, ce);

                if (ce.Cancel) return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void browser_FileCancelButtonClick(object sender, EventArgs e)
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