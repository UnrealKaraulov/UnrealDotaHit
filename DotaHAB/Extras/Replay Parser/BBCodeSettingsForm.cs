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
    public partial class BBCodeSettingsForm : Form
    {
        string[] bbCodeItems = null;
        HabPropertiesCollection hpcCfg;        
        string cfgFileName = null;

        public BBCodeSettingsForm()
        {
            InitializeComponent();
        }

        private void bbCodeGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            string bbCodeItem = bbCodeItems[e.RowIndex];            

            switch (e.ColumnIndex)
            {
                case 0: // preCode
                    e.Value = hpcCfg.GetStringListItemValue("bbCode", bbCodeItem, 0);
                    break;

                case 1: // item name
                    e.Value = bbCodeItem;
                    break;

                case 2: // postCode
                    e.Value = hpcCfg.GetStringListItemValue("bbCode", bbCodeItem, 1);
                    break;               
            }
        }

        private void bbCodeGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            string bbCodeItem = bbCodeItems[e.RowIndex];

            switch (e.ColumnIndex)
            {
                case 0: // preCode                    
                    hpcCfg.SetStringListItemValue("bbCode", bbCodeItem, 0, e.Value + "");
                    break;                

                case 2: // postCode
                    hpcCfg.SetStringListItemValue("bbCode", bbCodeItem, 1, e.Value + "");
                    break;
            }            
        }

        private void BBCodeSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            label1.Focus(); // remove focus from last cell

            hpcCfg.SaveToFile(cfgFileName);
        }

        public DialogResult ShowDialog(string[] bbCodeItems, string cfgFileName, HabPropertiesCollection hpcCfg)
        {
            this.bbCodeItems = bbCodeItems;
            this.cfgFileName = cfgFileName;
            this.hpcCfg = hpcCfg;

            bbCodeGridView.RowCount = bbCodeItems.Length; 

            return base.ShowDialog();
        }
    }
}