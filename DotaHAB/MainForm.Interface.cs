using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Core;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using Microsoft.Win32;
using DotaHIT.Core.Resources.Media;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.MpqPath;
using DotaHIT.DatabaseModel.Format;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Extras;
using BitmapUtils;
using System.Drawing.Imaging;

namespace DotaHIT
{
    public partial class MainForm
    {
        internal void UpdateRecentMap(string filename)
        {
            HabProperties hps = DHCFG.Items["RecentMaps"];

            if (filename != null && !hps.ContainsValue(Path.GetFullPath(filename)))
            {
                int length = (hps.Count >= 5) ? hps.Count : (hps.Count + 1);

                for (int i = length; i > 1; i--)
                    hps[i + ""] = hps[(i - 1) + ""];

                hps["1"] = Path.GetFullPath(filename);
            }

            fileToolStripMenuItem.DropDownItems.Clear();

            fileToolStripMenuItem.DropDownItems.Add(openFileToolStripMenuItem);

            if (hps.Count > 0)
                fileToolStripMenuItem.DropDownItems.Add(recentToolStripSeparator);

            object value;
            string key;
            for (int i = 1; i <= hps.Count; i++)
                if (hps.TryGetValue(key = i + "", out value))
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem();
                    tsmi.Name = key;
                    tsmi.Text = Path.GetFileName(value as string);
                    tsmi.Tag = value;
                    tsmi.Click += new EventHandler(loadRecentMap_Click);

                    fileToolStripMenuItem.DropDownItems.Add(tsmi);
                }

            fileToolStripMenuItem.DropDownItems.Add(propertiesToolStripSeparator);
            fileToolStripMenuItem.DropDownItems.Add(propertiesToolStripMenuItem);
            fileToolStripMenuItem.DropDownItems.Add(exitToolStripSeparator);
            fileToolStripMenuItem.DropDownItems.Add(exitToolStripMenuItem);
        }

        internal void LoadUITheme(Theme theme)
        {
            if (theme == Theme.None)
            {
                heroInfoPanel.BackgroundImage = Properties.Resources.defaultInfoBox;
                heroStatsPanel.BackgroundImage = Properties.Resources.defaultMainPanel;
                heroSkillsPanel.BackgroundImage = Properties.Resources.defaultSkillTable;
                inventoryBgPanel.BackgroundImage = heroStatsPanel.BackgroundImage;
                return;
            }

            if (!LoadUIThemeFromDisk(theme))
            {
                if (!Current.sessionHasWar3DB)
                {
                    MessageBox.Show("The theme '" + theme + "' requires WarCraft location to be known." +
                        "\nYou can specify WarCraft location in Options->Settings...", "Cannot load selected theme");
                    return;
                }
                LoadUIThemeFromWar3(theme);
            }
        }
        internal bool LoadUIThemeFromDisk(Theme theme)
        {
            string skinPath = Application.StartupPath + "\\Skins\\" + theme + "\\";

            string infoSkinPath = skinPath + "Info.png";
            string statsSkinPath = skinPath + "Stats.png";
            string skillsSkinPath = skinPath + "Skills.png";

            if (File.Exists(infoSkinPath) && File.Exists(statsSkinPath) && File.Exists(skillsSkinPath))
            {
                try
                {
                    heroInfoPanel.BackgroundImageLayout = ImageLayout.Zoom;
                    heroInfoPanel.BackgroundImage = Bitmap.FromFile(infoSkinPath);

                    heroSkillsPanel.BackgroundImageLayout = ImageLayout.Zoom;
                    heroSkillsPanel.BackgroundImage = Bitmap.FromFile(skillsSkinPath);

                    heroStatsPanel.BackgroundImageLayout = ImageLayout.Zoom;
                    heroStatsPanel.BackgroundImage = Bitmap.FromFile(statsSkinPath);

                    inventoryBgPanel.BackgroundImage = heroStatsPanel.BackgroundImage;
                }
                catch { return false; }

                return true;
            }
            else
                return false;
        }
        internal void LoadUIThemeFromWar3(Theme theme)
        {
            try
            {
                // Example path: UI\Console\Human\HumanUITile01.blp

                Bitmap bmp01 = DHRC.Default.GetImage(@"UI\Console\" + theme + "\\" + theme + "UITile01.blp", PixelFormat.Format32bppArgb);
                Bitmap bmp02 = DHRC.Default.GetImage(@"UI\Console\" + theme + "\\" + theme + "UITile02.blp", PixelFormat.Format32bppArgb);
                Bitmap bmp03 = DHRC.Default.GetImage(@"UI\Console\" + theme + "\\" + theme + "UITile03.blp", PixelFormat.Format32bppArgb);
                Bitmap bmp04 = DHRC.Default.GetImage(@"UI\Console\" + theme + "\\" + theme + "UITile04.blp", PixelFormat.Format32bppArgb);

                Brush brush = new SolidBrush(Color.Black);

                int w = 361;
                int h = 339;
                Bitmap bmpInfo = new Bitmap(w, h, bmp01.PixelFormat);

                Graphics gInfo = Graphics.FromImage(bmpInfo);

                // fill rectangle inside the info panel with black color, to make it nontransparent
                gInfo.FillRectangle(brush, 0, 74, bmpInfo.Width, bmpInfo.Height);

                // now draw the transparent image of the info panel
                gInfo.DrawImageUnscaled(bmp01,
                    new Point(0, -(bmp01.Height - h)));

                heroInfoPanel.BackgroundImageLayout = ImageLayout.Zoom;
                heroInfoPanel.BackgroundImage = bmpInfo;

                w = 372;
                Bitmap bmpSkill = new Bitmap(w, h, bmp03.PixelFormat);

                Graphics gSkill = Graphics.FromImage(bmpSkill);

                // fill rectangle inside the skill panel with black color, to make it nontransparent
                gSkill.FillRectangle(brush, 0, 74, bmpSkill.Width, bmpSkill.Height);

                // now draw the transparent image of the skill panel
                gSkill.DrawImageUnscaled(bmp04,
                    new Point(bmpSkill.Width - bmp04.Width, bmpSkill.Height - bmp04.Height));

                gSkill.DrawImage(bmp03, 0, 0,
                    new Rectangle(
                    bmp03.Width - (bmpSkill.Width - bmp04.Width), bmp03.Height - bmpSkill.Height,
                    (bmpSkill.Width - bmp04.Width), bmpSkill.Height),
                    GraphicsUnit.Pixel);

                heroSkillsPanel.BackgroundImageLayout = ImageLayout.Zoom;
                heroSkillsPanel.BackgroundImage = bmpSkill;

                w = 867;
                Bitmap bmpMain = new Bitmap(w, h, PixelFormat.Format32bppRgb);

                Graphics gMain = Graphics.FromImage(bmpMain);

                gMain.DrawImageUnscaled(bmp01,
                    new Point(-bmpInfo.Width, -(bmp01.Height - bmpMain.Height)));

                gMain.DrawImageUnscaled(bmp02,
                    new Point(bmp01.Width - bmpInfo.Width, -(bmp02.Height - bmpMain.Height)));

                gMain.DrawImageUnscaled(bmp03,
                    new Point((bmp01.Width - bmpInfo.Width) + bmp02.Width, -(bmp03.Height - bmpMain.Height)));

                heroStatsPanel.BackgroundImageLayout = ImageLayout.Zoom;
                heroStatsPanel.BackgroundImage = new Bitmap(bmpMain, heroStatsPanel.Size);

                inventoryBgPanel.BackgroundImage = heroStatsPanel.BackgroundImage;

                // save images to disk

                string skinPath = Application.StartupPath + "\\Skins\\" + theme;

                if (!Directory.Exists(skinPath))
                    Directory.CreateDirectory(skinPath);

                heroInfoPanel.BackgroundImage.Save(skinPath + "\\Info.png");
                heroSkillsPanel.BackgroundImage.Save(skinPath + "\\Skills.png");
                heroStatsPanel.BackgroundImage.Save(skinPath + "\\Stats.png");
            }
            catch
            {
                MessageBox.Show("An error occured while loading theme '" + theme + "' from WarCraft datafiles", "Error");
            }
        }

        internal bool IsThemeAvailableOnDisk(Theme theme)
        {
            if (theme == Theme.None) return false;

            string skinPath = Application.StartupPath + "\\Skins\\" + theme + "\\";

            string infoSkinPath = skinPath + "Info.png";
            string statsSkinPath = skinPath + "Stats.png";
            string skillsSkinPath = skinPath + "Skills.png";

            return (File.Exists(infoSkinPath) && File.Exists(statsSkinPath) && File.Exists(skillsSkinPath));
        }

        internal void SetUITheme(Theme theme)
        {
            if (theme == Current.theme)
                return;

            Current.theme = theme;

            switch (theme)
            {
                case Theme.NightElf:
                    nightElfToolStripMenuItem.Checked = true;
                    humanToolStripMenuItem.Checked = false;
                    orcToolStripMenuItem.Checked = false;
                    undeadToolStripMenuItem.Checked = false;
                    noneDefaultToolStripMenuItem.Checked = false;
                    break;

                case Theme.Human:
                    nightElfToolStripMenuItem.Checked = false;
                    humanToolStripMenuItem.Checked = true;
                    orcToolStripMenuItem.Checked = false;
                    undeadToolStripMenuItem.Checked = false;
                    noneDefaultToolStripMenuItem.Checked = false;
                    break;

                case Theme.Orc:
                    nightElfToolStripMenuItem.Checked = false;
                    humanToolStripMenuItem.Checked = false;
                    orcToolStripMenuItem.Checked = true;
                    undeadToolStripMenuItem.Checked = false;
                    noneDefaultToolStripMenuItem.Checked = false;
                    break;

                case Theme.Undead:
                    nightElfToolStripMenuItem.Checked = false;
                    humanToolStripMenuItem.Checked = false;
                    orcToolStripMenuItem.Checked = false;
                    undeadToolStripMenuItem.Checked = true;
                    noneDefaultToolStripMenuItem.Checked = false;
                    break;

                case Theme.None: // default
                    nightElfToolStripMenuItem.Checked = false;
                    humanToolStripMenuItem.Checked = false;
                    orcToolStripMenuItem.Checked = false;
                    undeadToolStripMenuItem.Checked = false;
                    noneDefaultToolStripMenuItem.Checked = true;
                    break;
            }

            LoadUITheme(theme);
        }
        internal void SetUIDialogs(bool useStandard)
        {
            UIDialogs.UseStandardDialogs = useStandard;
            if (useStandard)
            {
                UIDialogs.OpenFileDialogBoxWrapper.Default.openFileDialog = this.openFileDialog;
                UIDialogs.FolderBrowserDialogBoxWrapper.Default.folderBrowsingDialog = this.folderBrowserDialog;
            }
        }
    }    
}
