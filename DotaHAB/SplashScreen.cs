using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DotaHIT
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            loadPrgB.Value = 0;
        }
        public SplashScreen(Form owner) : this()            
        {
            this.Owner = owner;
        }

        public void ShowText(string text)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ShowText(text); });
                return;
            }

            loadStateLabel.Text = text;
            this.Refresh();
        }
              
        public void ProgressAdd(int amount)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ProgressAdd(amount); });
                return;
            }

            loadPrgB.Value = loadPrgB.Value + amount;
        }
        public void ShowProgress(double current, double total)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ShowProgress(current, total); });
                return;
            }

            loadPrgB.Value = (int)((100 / total) * current);
        }

        private void stopB_Click(object sender, EventArgs e)
        {
            StopButtonClicked = true;
        }

        public bool StopButtonVisible
        {
            get
            {
                return stopB.Visible;
            }
            set
            {
                stopB.Visible = value;
            }
        }

        public bool StopButtonClicked 
        { 
            get; 
            set; 
        }

        public event EventHandler StopButtonClick
        {
            add
            {
                stopB.Click += value;
            }
            remove
            {
                stopB.Click -= value;
            }
        }
    
    }
}