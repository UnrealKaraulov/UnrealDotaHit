using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Format;

namespace DotaHIT.Extras
{
    public partial class BansPicksForm : Form
    {        
        HabPropertiesCollection hpcCfg;        
        string cfgFileName = null;

        public BansPicksForm()
        {
            InitializeComponent();
        }
       
        private void BansPicksForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            hpcCfg["BanPick", "BanEnumType"] = this.BanEnumerationType;
            hpcCfg["BanPick", "FirstBanner"] = showFirstBannerCB.Checked ? 1 : 0;
            hpcCfg["BanPick", "BanSeparator"] = banSeparatorTextBox.Text;

            hpcCfg["BanPick", "PickEnumType"] = this.PickEnumerationType;
            hpcCfg["BanPick", "FirstPicker"] = showFirstPickerCB.Checked ? 1 : 0;
            hpcCfg["BanPick", "PickSeparator"] = pickSeparatorTextBox.Text;
            hpcCfg["BanPick", "PickPairSeparator"] = pickPairSeparatorTextBox.Text;

            hpcCfg.SaveToFile(cfgFileName);
        }

        int BanEnumerationType
        {
            get
            {
                if (banSeparatedRB.Checked)
                    return 1;                                
                return 0;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        banSingleLineRB.Checked = true;
                        break;

                    case 1:
                        banSeparatedRB.Checked = true;
                        break;
                }
            }
        }

        int PickEnumerationType
        {
            get
            {
                if (pickSeparatedRB.Checked)
                    return 1;
                return 0;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        pickSingleLineRB.Checked = true;
                        break;

                    case 1:
                        pickSeparatedRB.Checked = true;
                        break;
                }
            }
        }

        public DialogResult ShowDialog(string cfgFileName, HabPropertiesCollection hpcCfg)
        {            
            this.cfgFileName = cfgFileName;
            this.hpcCfg = hpcCfg;

            this.BanEnumerationType = hpcCfg.GetIntValue("BanPick", "BanEnumType", 0);
            showFirstBannerCB.Checked = hpcCfg.GetIntValue("BanPick", "FirstBanner", 1) == 1;
            banSeparatorTextBox.Text = hpcCfg.GetStringValue("BanPick", "BanSeparator", ", ");

            this.PickEnumerationType = hpcCfg.GetIntValue("BanPick", "PickEnumType", 0);
            showFirstPickerCB.Checked = hpcCfg.GetIntValue("BanPick", "FirstPicker", 1) == 1;
            pickSeparatorTextBox.Text = hpcCfg.GetStringValue("BanPick", "PickSeparator", ", ");
            pickPairSeparatorTextBox.Text = hpcCfg.GetStringValue("BanPick", "PickPairSeparator", " + ");

            return base.ShowDialog();
        }
    }
}