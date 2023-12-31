using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Core;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Specialized;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;

namespace DotaHIT
{
    public partial class SettingsForm : Form
    {
        internal int mbX = -1;
        internal int mbY = -1;

        private RichTextBox buffer = new RichTextBox();

        public SettingsForm()
        {
            InitializeComponent();

            fileTypesGridView.DefaultCellStyle.ForeColor = Color.Black;

            war3PathTextBox.Text = DHCFG.Items["Path"].GetStringValue("War3");

            fileTypesGridView.RowCount = 3;            

            restoreCB.Checked = DHCFG.Items["Settings"].GetIntValue("RestoreOnStartup") == 1;
            useOwnDialogsCB.Checked = DHCFG.Items["UI"].GetIntValue("UseOwnDialogs") == 1;
            showDetailSwitchTipCB.Checked = DHCFG.Items["UI"].GetIntValue("ShowDetailModeSwitchTip", 1) == 1;
            blurryFontFixCB.Checked = DHCFG.Items["UI"].GetIntValue("TextBoxBlurryFontFix", 0) == 1;

            arialFontTextBox.Text = UIFonts.ArialFontName;
            verdanaFontTextBox.Text = UIFonts.VerdanaFontName;
            showUpdateSplashCB.Checked = DHCFG.Items["Update"].GetIntValue("ShowSplash", 1) == 1;
        }

        public void SetParent(MainForm parentForm)
        {         
            this.Owner = parentForm;         
        }

        private void captionB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mbX = MousePosition.X - this.Location.X;
                mbY = MousePosition.Y - this.Location.Y;
            }
            else
                mbX = mbY = -1;
        }

        private void captionB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbX != -1 && mbY != -1)
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    this.SetDesktopLocation(MousePosition.X - mbX, MousePosition.Y - mbY);
        }

        private void captionB_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mbX = mbY = -1;
        }

        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | Win32Msg.WS_THICKFRAME;
                return cp;
            }
        }*/        

        private void closeB_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void applyB_Click(object sender, EventArgs e)
        {   
            DHCFG.Items["Settings"]["RestoreOnStartup"] = restoreCB.Checked ? 1 : 0;            
            Current.mainForm.SetUIDialogs(!useOwnDialogsCB.Checked);

            DHCFG.Items["UI"]["ShowDetailModeSwitchTip"] = showDetailSwitchTipCB.Checked ? 1 : 0;
            DHCFG.Items["UI"]["TextBoxBlurryFontFix"] = blurryFontFixCB.Checked ? 1 : 0;

            if (IsCurrentProcessAdmin())
            {
                DHFILE.AssociateFileTypes(DHCFG.Items["Settings"].GetStringListValue("FileTypes"));
                DHFILE.RegisterContextMenus(DHCFG.Items["Settings"].GetStringListValue("ContextMenus"));
            }

            DHCFG.Items["Path"]["War3"] = war3PathTextBox.Text;
            if (Current.map == null) DHMAIN.LoadWar3Mpq();                       

            DHCFG.Items["Fonts"]["ArialFontName"] = arialFontTextBox.Text;
            DHCFG.Items["Fonts"]["VerdanaFontName"] = verdanaFontTextBox.Text;

            UIFonts.ResetFonts();

            DHCFG.Items["Update"]["ShowSplash"] = showUpdateSplashCB.Checked ? 1 : 0;

            this.Hide();
        }

        private void cancelB_Click(object sender, EventArgs e)
        {
            this.Hide();
        }     

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Hide();
        }

        private void war3PathBrowseB_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                war3PathTextBox.Text = folderBrowserDialog.SelectedPath;
        }

        private string getFileInfo(int index, bool extension)
        {
            switch (index)
            {
                case 0: return extension ? ".w3x" : "DotA map";
                case 1: return extension ? ".dhb" : "DotA H.I.T. Hero Build";
                case 2: return extension ? ".w3g" : "WarCraft III Replay";
            }
            return string.Empty;
        }

        private void fileTypesGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 0:
                    e.Value = getFileInfo(e.RowIndex, true);
                    break;

                case 1:
                    e.Value = getFileInfo(e.RowIndex, false);
                    break;

                case 2:
                    e.Value = DHCFG.Items["Settings"].GetStringListValue("FileTypes").Contains(getFileInfo(e.RowIndex, true));
                    break;

                case 3:
                    e.Value = DHCFG.Items["Settings"].GetStringListValue("ContextMenus").Contains(getFileInfo(e.RowIndex, true));
                    break;
            }
        }

        private void fileTypesGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            string fileType = getFileInfo(e.RowIndex, true);

            switch (e.ColumnIndex)
            {
                case 2:
                    List<string> fileTypes = DHCFG.Items["Settings"].GetStringListValue("FileTypes");                    

                    if ((bool)e.Value)
                    { 
                        if (!fileTypes.Contains(fileType)) fileTypes.Add(fileType); 
                    }
                    else
                        fileTypes.Remove(fileType);

                    DHCFG.Items["Settings"]["FileTypes"] = fileTypes;
                    break;

                case 3:                    
                    List<string> contextMenus = DHCFG.Items["Settings"].GetStringListValue("ContextMenus");

                    if ((bool)e.Value)
                    {
                        if (!contextMenus.Contains(fileType)) contextMenus.Add(fileType);
                    }
                    else
                        contextMenus.Remove(fileType);

                    DHCFG.Items["Settings"]["ContextMenus"] = contextMenus;
                    break;
            }
        }

        private void chooseArialFontB_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = new Font(UIFonts.ArialFontName, 10);

            if (fontDialog1.ShowDialog() == DialogResult.OK)            
                arialFontTextBox.Text = fontDialog1.Font.FontFamily.Name;            
        }

        private void chooseVerdanaFontB_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = new Font(UIFonts.VerdanaFontName, 10);

            if (fontDialog1.ShowDialog() == DialogResult.OK)
                verdanaFontTextBox.Text = fontDialog1.Font.FontFamily.Name;  
        }
        private void captionB_Click(object sender, EventArgs e)
        {

        }
        private bool IsCurrentProcessAdmin()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = IsCurrentProcessAdmin();
        }
    }
}