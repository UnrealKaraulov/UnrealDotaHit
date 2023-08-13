using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass;
using Tga;

namespace DotaHIT
{
    public partial class PropertiesForm : Form
    {
        private UIRichText mapDescriptionRichText;

        public PropertiesForm()
        {
            InitializeComponent();

            mapDescriptionRichText = new UIRichText(mapDescriptionRTB);

            filePathTextBox.Text = Current.map.Name;
            mapNameTextBox.Text = DHSTRINGS.GetTriggerString(DHJassExecutor.Globals["DHMapName"].StringValue);
            mapDescriptionRichText.AddTaggedText(DHSTRINGS.GetTriggerString(DHJassExecutor.Globals["DHMapDescription"].StringValue), UIFonts.boldVerdana, Color.White);            
            mapImagePanel.BackgroundImage = DHRC.Default.GetTgaImage("war3mapPreview.tga");
        }

        private void viewLoadingScreenB_Click(object sender, EventArgs e)
        {
            Form loadinScreenForm = new Form();
            loadinScreenForm.AutoScroll = true;
            loadinScreenForm.StartPosition = FormStartPosition.CenterScreen;
            loadinScreenForm.ShowIcon = false;
            loadinScreenForm.ShowInTaskbar = false;
            loadinScreenForm.MinimizeBox = false;
            loadinScreenForm.MaximizeBox = false;
            loadinScreenForm.FormBorderStyle = FormBorderStyle.FixedDialog;

            try
            {
                Bitmap topLeft = DHRC.Default.GetTgaImage("LoadingScreenTL.tga");
                Bitmap topRight = DHRC.Default.GetTgaImage("LoadingScreenTR.tga");
                Bitmap bottomLeft = DHRC.Default.GetTgaImage("LoadingScreenBL.tga");
                Bitmap bottomRight = DHRC.Default.GetTgaImage("LoadingScreenBR.tga");

                Bitmap bmp = new Bitmap(topLeft.Width + topRight.Width, topLeft.Height + bottomLeft.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmp);

                g.DrawImageUnscaled(topLeft, 0, 0);
                g.DrawImageUnscaled(topRight, topLeft.Width, 0);
                g.DrawImageUnscaled(bottomLeft, 0, topLeft.Height);
                g.DrawImageUnscaled(bottomRight, topLeft.Width, topLeft.Height);

                loadinScreenForm.BackgroundImage = bmp;
                loadinScreenForm.BackgroundImageLayout = ImageLayout.None;
                loadinScreenForm.ClientSize = bmp.Size;
                loadinScreenForm.Text = "Press CTRL+S to save loading screen as png file";
                loadinScreenForm.KeyDown += delegate(object form, KeyEventArgs keys)
                {
                    if (!(keys.Control && keys.KeyCode == Keys.S))
                        return;

                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.DefaultExt = "png";
                    sfd.Filter = "PNG file|*.png|All files|*.*";
                    sfd.Title = "Save loading screen as...";
                    sfd.InitialDirectory = Application.StartupPath;
                    sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(Current.filename) + " Loading Screen";
                    sfd.FileOk += delegate(object o, CancelEventArgs cea)
                    {
                        loadinScreenForm.BackgroundImage.Save(sfd.FileName);
                    };

                    sfd.ShowDialog();
                };
            }
            catch
            {
                MessageBox.Show("Couldn't get loading screen image");
            }

            loadinScreenForm.ShowDialog();
        }
    }
}