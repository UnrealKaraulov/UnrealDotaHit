using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ExpTreeLib;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Format;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Jass.Native.Types;
using BitmapUtils;
using System.Windows.Forms.Design;

namespace DotaHIT.Extras
{
    [Designer(typeof(ParentControlDesigner))]
    public partial class FileBrowser : UserControl
    {        
        private ManualResetEvent Event1 = new ManualResetEvent(true);        
        private string filter = "All files|*.*";
        private string[] filters = new string[1] { "*.*" };
        private int filterIndex = 1;        
        private List<CShItem> selectedItems = new List<CShItem>();        

        public FileBrowser()
        {
            InitializeComponent();                        

            explorerLV.View = View.List;
            explorerLV.Columns[0].Width = -1;

            Control.CheckForIllegalCrossThreadCalls = false;
        }

        public FileBrowser(string path)
            : base()
        {
            foldersExpTree.ExpandANode(path);

            this.Show();
        }

        private void FileBrowser_Load(object sender, EventArgs e)
        {
            parseFilters();
        }

        private void lv1_VisibleChanged(object sender, EventArgs e)
        {
            if (explorerLV.Visible && browseMode != BrowseModes.FolderBrowser)
            {                
                //SystemImageListManager.SetListViewImageList(explorerLV, true, false);
                SystemImageListManager.SetListViewImageList(explorerLV, false, false);                
            }
        }

        private void LoadLV1Images()
        {
            ThreadStart ts = new ThreadStart(DoLoadLv);
            Thread ot = new Thread(ts);

            ot.SetApartmentState(ApartmentState.STA);
            Event1.Reset();
            ot.Start();
        }

        private void DoLoadLv()
        {
            foreach (ListViewItem lvi in explorerLV.Items)
            {
                CShItem cshItem = lvi.Tag as CShItem;
                lvi.ImageIndex = SystemImageListManager.GetIconIndex(cshItem, false, false);
            }

            Event1.Set();
        }

        private void expTree1_ExpTreeNodeSelected(string pathName, CShItem CSI)
        {
            if (SelectedPathChanged != null)
                SelectedPathChanged(this, EventArgs.Empty);

            if (browseMode == BrowseModes.FolderBrowser) return;

            List<CShItem> dirList = new List<CShItem>();
            List<CShItem> fileList = new List<CShItem>();

            int TotalItems;

            if (CSI.DisplayName.Equals(CShItem.strMyComputer))
                //avoid re-query since only has dirs
                dirList = CSI.GetDirectories(true);
            else
            {
                dirList = CSI.GetDirectories(true);                
                fileList = (this.DesignMode || SelectedFilter == "*.*")? CSI.GetFiles() : CSI.GetFiles(SelectedFilter);                
            }            

            TotalItems = dirList.Count + fileList.Count;

            if (TotalItems > 0)
            {
                dirList.Sort();
                fileList.Sort();
                             
                browseStatusBar.Text = dirList.Count + " Directories " +
                            fileList.Count + " Files";

                ArrayList combList = new ArrayList(TotalItems);

                combList.AddRange(dirList);
                combList.AddRange(fileList);

                //Build the ListViewItems & add to lv1
                explorerLV.BeginUpdate();
                explorerLV.Items.Clear();

                foreach (CShItem item in combList)
                {
                    ListViewItem lvi = new ListViewItem(item.DisplayName);
                    /*if (!item.IsDisk && item.IsFileSystem)
                    {
                        FileAttributes attr = File.GetAttributes(item.Path);
                        StringBuilder SB = new StringBuilder();

                        if ((attr & FileAttributes.System) == FileAttributes.System) SB.Append("S");
                        if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) SB.Append("H");
                        if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) SB.Append("R");
                        if ((attr & FileAttributes.Archive) == FileAttributes.Archive) SB.Append("A");

                        lvi.SubItems.Add(SB.ToString());
                    }
                    else
                        lvi.SubItems.Add("");

                    if ((!item.IsDisk) && item.IsFileSystem && (!item.IsFolder))
                    {
                        if (item.Length > 1024)
                            lvi.SubItems.Add(string.Format("{0:#,###} KB", item.Length / 1024));
                        else
                            lvi.SubItems.Add(string.Format("{0:##0} Bytes", item.Length));
                    }
                    else
                        lvi.SubItems.Add("");

                    lvi.SubItems.Add(item.TypeName);
                    if (item.IsDisk)
                        lvi.SubItems.Add("");*/

                    lvi.Tag = item;
                    CShItem refItem = item;                    
                    lvi.ImageIndex = SystemImageListManager.GetIconIndex(refItem, false, false);                    
                    explorerLV.Items.Add(lvi);
                }
                
                explorerLV.EndUpdate();
                //LoadLV1Images();
            }
            else
            {
                explorerLV.Items.Clear();
                browseStatusBar.Text = "No Items";
            }

            pathTextBox.Text = (CSI.IsFileSystem) ? pathName : "";            
        }        

        private void explorer_ItemActivate(object sender, EventArgs e)
        {
            if (explorerLV.SelectedItems.Count == 0) return;

            CShItem csi = explorerLV.SelectedItems[0].Tag as CShItem;

            explorerLV.Cursor = Cursors.AppStarting;

            if (csi.IsFolder)
                foldersExpTree.ExpandANode(csi);
            else
                switch (browseMode)
                {
                    case BrowseModes.Explorer:
                        if (!OnFileOk())
                            try
                            {
                                ProcessStartInfo psi = new ProcessStartInfo(csi.Path);
                                psi.WorkingDirectory = Path.GetDirectoryName(csi.Path);
                                Process.Start(psi);//csi.Path); 
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error in starting application");
                            }
                        break;

                    case BrowseModes.FileBrowser:
                        OnFileOk();
                        break;
                }

            explorerLV.Cursor = Cursors.Default;
        }       

        private void explorerLV_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                explorer_ItemActivate(sender, EventArgs.Empty);
        } 

        private void upToolStripButton_Click(object sender, EventArgs e)
        {
            if (foldersExpTree.SelectedItem.Parent != null)
            {
                foldersExpTree.ExpandANode(foldersExpTree.SelectedItem.Parent);
                foldersExpTree.EnsureSelectedVisible();
            }
        }

        private void toDHToolStripButton_Click(object sender, EventArgs e)
        {
            foldersExpTree.ExpandANode(Application.StartupPath);
        }

        private void toWar3ReplatToolStripButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.AppStarting;
            foldersExpTree.ExpandANode(DHCFG.Items["Path"].GetStringValue("War3") + "\\" + "Replay");
            Cursor = Cursors.Default;
        }

        private void toWar3MapsToolStripButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.AppStarting;
            foldersExpTree.ExpandANode(DHCFG.Items["Path"].GetStringValue("War3") + "\\" + "Maps");
            Cursor = Cursors.Default;
        }

        private void goB_Click(object sender, EventArgs e)
        {
            if (!foldersExpTree.ExpandANode(pathTextBox.Text))
                browseStatusBar.Text = "Path not found";
        }

        private void pathTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                goB_Click(null, EventArgs.Empty);
            }
        }

        private void lv1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedItemsChanged != null)
            {
                List<CShItem> items = new List<CShItem>(explorerLV.SelectedItems.Count);

                foreach (ListViewItem lvi in explorerLV.SelectedItems)
                    items.Add(lvi.Tag as CShItem);                

                this.selectedItems = items;                
                
                SelectedItemsChanged(this, new SelectedItemsChangedEventArgs(items));
            }

            if (browseMode == BrowseModes.FileBrowser)            
                fileNameTextBox.Text = Path.GetFileName(this.SelectedFile);            
        }

        private void lv1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:
                    e.Handled = true;
                    upToolStripButton_Click(null, EventArgs.Empty);
                    break;

                case Keys.Enter:
                    e.Handled = true;
                    explorer_ItemActivate(null, EventArgs.Empty);                    
                    break;

                case Keys.F2:
                    if (selectedItems.Count == 1)
                    {
                        e.Handled = true;
                        renameToolStripMenuItem_Click(null, EventArgs.Empty);
                    }
                    break;
            }
        }
        
        public void RefreshFiles()
        {            
            expTree1_ExpTreeNodeSelected(foldersExpTree.SelectedItem.Path, foldersExpTree.SelectedItem);
        }

        private void filterCmbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFiles();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            explorer_ItemActivate(sender, e);
        }

        private string[] GetSelectedFiles()
        {
            if (explorerLV.SelectedItems.Count == 0)
                return null;

            string[] files = new string[explorerLV.SelectedItems.Count];
            int i = 0;
            foreach (ListViewItem lvi in explorerLV.SelectedItems)
            {
                CShItem item = lvi.Tag as CShItem;
                if (item.IsFileSystem && !item.IsDisk)
                    files[i++] = item.Path;
            }
            return files;
        }

        /// <summary>
        /// Write files to clipboard (from http://blogs.wdevs.com/idecember/archive/2005/10/27/10979.aspx)
        /// </summary>
        /// <param name="cut">True if cut, false if copy</param>
        void CopyToClipboard(bool cut)
        {
            string[] files = GetSelectedFiles();

            if (files != null)
            {
                IDataObject data = new DataObject(DataFormats.FileDrop, files);
                MemoryStream memo = new MemoryStream(4);
                byte[] bytes = new byte[] { (byte)(cut ? 2 : 5), 0, 0, 0 };
                memo.Write(bytes, 0, bytes.Length);
                data.SetData("Preferred DropEffect", memo);
                Clipboard.SetDataObject(data);
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard(true);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard(false);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CShItem item = explorerLV.SelectedItems[0].Tag as CShItem;

            if (item.IsFileSystem && !item.IsDisk)
            {
                try
                {
                    System.IO.File.Delete(item.Path);
                    explorerLV.Items.Remove(explorerLV.SelectedItems[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CShItem item = explorerLV.SelectedItems[0].Tag as CShItem;

            if (item.IsFileSystem && !item.IsDisk)
                explorerLV.SelectedItems[0].BeginEdit();
        }

        private void explorerLV_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }

            CShItem item = explorerLV.Items[e.Item].Tag as CShItem;
            try
            {
                if (item.IsFolder)
                    Directory.Move(item.Path, (item.Parent != null ? item.Parent.Path : this.SelectedPath) + "\\" + e.Label);
                else
                    System.IO.File.Move(item.Path, (item.Parent != null ? item.Parent.Path : this.SelectedPath) + "\\" + e.Label);

                CShItem newItem = new CShItem(item.Parent.Path + "\\" + e.Label);
                explorerLV.Items[e.Item].Tag = newItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                e.CancelEdit = true;
                return;
            }
        }

        private void explorerLV_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            CShItem item = explorerLV.Items[e.Item].Tag as CShItem;

            if ((item.IsFileSystem && !item.IsDisk) == false)
                e.CancelEdit = true;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (!data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])data.GetData(DataFormats.FileDrop);
            MemoryStream stream = (MemoryStream)data.GetData("Preferred DropEffect", true);
            int flag = stream.ReadByte();
            if (flag != 2 && flag != 5)
                return;
            bool cut = (flag == 2);
            foreach (string file in files)
            {
                string dest = this.SelectedPath + "\\" + Path.GetFileName(file);
                try
                {
                    if (cut)
                        System.IO.File.Move(file, dest);
                    else
                        System.IO.File.Copy(file, dest, false);

                    ListViewItem lvi = new ListViewItem(Path.GetFileName(dest));
                    CShItem item = new CShItem(dest);
                    lvi.Tag = item;
                    lvi.ImageIndex = SystemImageListManager.GetIconIndex(item, false, false);
                    explorerLV.Items.Add(lvi);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(this, "Failed to perform the specified operation:\n\n" + ex.Message, "File operation failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
        
        private void explorerLV_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                switch (explorerLV.SelectedItems.Count)
                {
                    case 0:
                        noitemsContextMenuStrip.Show(MousePosition);
                        break;

                    case 1:
                        if (ContextMenuShowing != null)
                            ContextMenuShowing(this, EventArgs.Empty);
                        filesContextMenuStrip.Show(MousePosition);
                        break;
                }
        }

        private void pasteShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (!data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length > 1) return;

            MessageBox.Show("Creating shortcuts is currently not supported");

            /*// Create a new instance of WshShellClass

            WshShell = new WshShellClass();

            // Create the shortcut

            IWshRuntimeLibrary.IWshShortcut MyShortcut;

            // Choose the path for the shortcut

            string name = Path.GetFileName(files[0]);
            string shortcutName = this.SelectedPath + "\\" + name + ".lnk";

            MyShortcut = (IWshRuntimeLibrary.IWshShortcut)WshShell.CreateShortcut(shortcutName);

            // Where the shortcut should point to

            MyShortcut.TargetPath = files[0];

            // Description for the shortcut

            MyShortcut.Description = "Shortcut to " + name;

            // Location for the shortcut's icon

            //MyShortcut.IconLocation = Application.StartupPath + @"\app.ico";

            // Create the shortcut at the given path

            MyShortcut.Save();

            CShItem item;
            try
            {
                item = new CShItem(shortcutName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            ListViewItem lvi = new ListViewItem(name);

            lvi.Tag = item;
            lvi.ImageIndex = SystemImageListManager.GetIconIndex(item, false, false);
            explorerLV.Items.Add(lvi);*/
        }

        private void filesContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            CShItem item = explorerLV.SelectedItems[0].Tag as CShItem;

            cutToolStripMenuItem.Enabled = item.IsFileSystem;
            copyToolStripMenuItem.Enabled = item.IsFileSystem;
            deleteToolStripMenuItem.Enabled = item.IsFileSystem;
            renameToolStripMenuItem.Enabled = item.IsFileSystem;
        }

        private void noitemsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                pasteToolStripMenuItem.Enabled = true;
                pasteShortcutToolStripMenuItem.Enabled = true;
            }
            else
            {
                pasteToolStripMenuItem.Enabled = false;
                pasteShortcutToolStripMenuItem.Enabled = false;
            }
        }

        public bool MoveNext(bool onlyFiles)
        {
            if (explorerLV.SelectedItems.Count == 0)
            {
                foreach (ListViewItem lvi in explorerLV.Items)
                {
                    CShItem item = lvi.Tag as CShItem;
                    if (!onlyFiles || ((!item.IsDisk) && item.IsFileSystem && !item.IsFolder))
                    {
                        explorerLV.SelectedIndices.Add(lvi.Index);
                        explorerLV.EnsureVisible(lvi.Index);
                        return true;
                    }
                }                
            }
            else
            {
                int selectedItem = explorerLV.SelectedItems[0].Index;
                for (int i = selectedItem + 1; i < explorerLV.Items.Count; i++)
                {
                    CShItem item = explorerLV.Items[i].Tag as CShItem;
                    if (!onlyFiles || ((!item.IsDisk) && item.IsFileSystem && !item.IsFolder))
                    {
                        explorerLV.SelectedIndices.Clear();
                        explorerLV.SelectedIndices.Add(i);
                        explorerLV.EnsureVisible(i);
                        return true;
                    }
                }                
            }

            return false;
        }

        public bool MovePrev(bool onlyFiles)
        {
            if (explorerLV.SelectedItems.Count == 0)
            {
                foreach (ListViewItem lvi in explorerLV.Items)
                {
                    CShItem item = lvi.Tag as CShItem;
                    if (!onlyFiles || ((!item.IsDisk) && item.IsFileSystem && !item.IsFolder))
                    {
                        explorerLV.SelectedIndices.Add(lvi.Index);
                        explorerLV.EnsureVisible(lvi.Index);
                        return true;
                    }
                }
            }
            else
            {
                int selectedItem = explorerLV.SelectedItems[0].Index;
                for (int i = selectedItem - 1; i >= 0; i--)
                {
                    CShItem item = explorerLV.Items[i].Tag as CShItem;
                    if (!onlyFiles || ((!item.IsDisk) && item.IsFileSystem && !item.IsFolder))
                    {
                        explorerLV.SelectedIndices.Clear();
                        explorerLV.SelectedIndices.Add(i);                        
                        explorerLV.EnsureVisible(i);
                        return true;
                    }
                }
            }

            return false;
        }

        public void EnsureExplorerLoaded()
        {
            // workaround to wake up list-view
            explorerLV.AccessibilityObject.ToString();
        }

        [Browsable(false)]
        public string SelectedPath
        {
            get
            {                
                return foldersExpTree.SelectedItem != null && !foldersExpTree.SelectedItem.Path.StartsWith(":") ? foldersExpTree.SelectedItem.Path : string.Empty;
            }
            set
            {
                foldersExpTree.ExpandANode(value);
            }
        }

        [Browsable(false)]
        public string SelectedFile
        {
            get
            {
                if (explorerLV.SelectedItems.Count == 0) return string.Empty;

                CShItem item = explorerLV.SelectedItems[0].Tag as CShItem;
                
                if ((!item.IsDisk) && item.IsFileSystem && !item.IsFolder)
                    return item.Path;
                else
                    return string.Empty;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    explorerLV.SelectedItems.Clear();
                    return;
                }

                foreach (ListViewItem lvItem in explorerLV.Items)
                {
                    CShItem item = lvItem.Tag as CShItem;
                    if ((!item.IsDisk) && item.IsFileSystem && !item.IsFolder && item.Path.ToLower() == value.ToLower())
                    {
                        explorerLV.SelectedIndices.Clear();
                        explorerLV.SelectedIndices.Add(lvItem.Index);
                        explorerLV.EnsureVisible(lvItem.Index);
                        break;
                    }
                }
            }
        }        

        [Browsable(false)]
        public string FileName
        {
            get
            {
                return SelectedFile;
            }
            set
            {
                fileNameTextBox.Text = value;                
                foldersExpTree.ExpandANode(string.IsNullOrEmpty(value) ? string.Empty : Path.GetDirectoryName(value));
            }
        }       

        [Browsable(false)]
        public List<CShItem> SelectedItems
        {
            get { return selectedItems; }
        }

        public bool ShowExplorer
        {
            get
            {
                return explorerPanel.Tag is bool ? (bool)explorerPanel.Tag : true;
            }
            set
            {
                if (browseMode == BrowseModes.Explorer)
                    showExplorer(value);
            }
        }
        private void showExplorer(bool show)
        {
            explorerPanel.Tag = show;
            explorerPanel.Visible = expListSplitter.Visible = show;
            foldersExpTree.Dock = show ? DockStyle.Left : DockStyle.Fill;
        }

        public bool ShowStatusBar
        {
            get
            {
                return browseStatusBar.Visible;
            }

            set
            {
                if (browseMode == BrowseModes.Explorer)
                    browseStatusBar.Visible = value;
            }
        }

        public BorderStyle ExplorerBorderStyle
        {
            get
            {
                return explorerLV.BorderStyle;
            }
            set
            {
                explorerLV.BorderStyle = value;
            }
        }
        public Padding BrowserPanelPadding
        {
            get
            {
                return browsePanel.Padding;
            }
            set
            {
                browsePanel.Padding = value;
            }
        }

        [DefaultValue(typeof(Color),"Gainsboro")]
        public Color NavigationPanelBackColor
        {
            get
            {
                return navigationPanel.BackColor;
            }
            set
            {
                navigationPanel.BackColor = value;
            }
        }

        [DefaultValue(typeof(SystemColors),"Control")]        
        public Color BrowserPanelBackColor
        {
            get
            {
                return browsePanel.BackColor;
            }
            set
            {
                browsePanel.BackColor = value;
            }
        }

        [Category("Options"), DefaultValue("All files|*.*")]
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                parseFilters();
            }
        }

        [Category("Options"), DefaultValue(1)]
        public int FilterIndex
        {
            get
            {
                return filterIndex;
            }
            set
            {
                filterIndex = value;
                filterCmbB.SelectedIndex = value - 1;
            }
        }

        private void parseFilters()
        {
            filterCmbB.Items.Clear();

            filters = filter.Split('|');
            for (int i = 0; i < filters.Length / 2; i++)
            {
                string description = filters[i * 2];
                string ext = filters[i * 2 + 1];

                filterCmbB.Items.Add(description + " (" + ext + ")");
            }

            filterCmbB.SelectedIndex = filterIndex - 1;
        }
        string SelectedFilter
        {
            get
            {               
                return (filterCmbB.SelectedIndex != -1) ? filters[(filterCmbB.SelectedIndex * 2) + 1] : "*.*";
            }
        }

        [Category("Options"), Description("Sets the Initial Directory"), DefaultValue(ExpTreeLib.ExpTree.StartDir.Desktop), Browsable(true)]
        public ExpTreeLib.ExpTree.StartDir StartUpDirectory
		{
			get
			{
				return foldersExpTree.StartUpDirectory;
			}
			set
			{
                foldersExpTree.StartUpDirectory = value;
			}
		}

        public ContextMenuStrip FilesContextMenuStrip 
        { 
            get { return filesContextMenuStrip; } 
        }

        [Category("ShortCuts"), DefaultValue(true)]
        public bool ShowDotaHITShortCut
        {
            get
            {                
                return toDHToolStripButton.Tag is bool ? (bool)toDHToolStripButton.Tag : true;
            }
            set
            {
                showShortCut(toDHToolStripButton, value);
            }
        }

        [Category("ShortCuts"), DefaultValue(false)]
        public bool ShowWar3ReplayShortCut
        {
            get
            {
                return toWar3ReplayToolStripButton.Tag is bool ? (bool)toWar3ReplayToolStripButton.Tag : false;
            }
            set
            {
                showShortCut(toWar3ReplayToolStripButton, value);
            }
        }

        [Category("ShortCuts"), DefaultValue(false)]
        public bool ShowWar3MapsShortCut
        {
            get
            {
                return toWar3MapsToolStripButton.Tag is bool ? (bool)toWar3MapsToolStripButton.Tag : false;
            }
            set
            {
                showShortCut(toWar3MapsToolStripButton, value);                
            }
        }

        private void showShortCut(ToolStripButton sc, bool show)
        {
            sc.Visible = show;
            sc.Tag = show;           
        }

        public enum BrowseModes
        {
            Explorer = 0,
            FileBrowser = 1,
            FolderBrowser = 2
        }
        private BrowseModes browseMode;
        [Category("Options"), Description("Sets the Browse Mode"), DefaultValue(FileBrowser.BrowseModes.Explorer), Browsable(true)]
        public BrowseModes BrowseMode
        {
            get
            {
                return browseMode;
            }
            set
            {                
                switch (value)
                {
                    case BrowseModes.Explorer:
                        showExplorer(true);
                        browseStatusBar.Visible = true;
                        navigationPanel.Visible = true;
                        fileChooserPanel.Visible = false;
                        folderChooserPanel.Visible = false;                        
                        break;
                    case BrowseModes.FileBrowser:
                        showExplorer(true);
                        browseStatusBar.Visible = false;
                        navigationPanel.Visible = true;
                        fileChooserPanel.Visible = true;
                        folderChooserPanel.Visible = false;
                        break;

                    case BrowseModes.FolderBrowser:                   
                        showExplorer(false);
                        browseStatusBar.Visible = false;
                        navigationPanel.Visible = false;
                        fileChooserPanel.Visible = false;
                        folderChooserPanel.Visible = true;
                        break;
                }
                browseMode = value;
            }
        }

        public delegate void SelectedItemsChangedEventHandler(object sender, SelectedItemsChangedEventArgs e);
        public class SelectedItemsChangedEventArgs : EventArgs
        {
            public List<CShItem> SelectedItems;

            public SelectedItemsChangedEventArgs(List<CShItem> SelectedItems)
            {
                this.SelectedItems = SelectedItems;
            }
        }
        public event SelectedItemsChangedEventHandler SelectedItemsChanged;
        public event EventHandler SelectedPathChanged;
        public event EventHandler ContextMenuShowing;

        public event CancelEventHandler FileOk;
        public bool OnFileOk() 
        {
            if (FileOk != null)
            {
                CancelEventArgs ce = new CancelEventArgs();
                FileOk(this, ce);

                return ce.Cancel;
            }

            return false;
        }

        public event EventHandler FileOpenButtonClick
        {
            add
            {
                openB.Click += value;
            }
            remove
            {
                openB.Click -= value;
            }
        }        
        public event EventHandler FileCancelButtonClick
        {
            add
            {
                cancelB.Click += value;
            }
            remove
            {
                cancelB.Click -= value;
            }
        }

        public event EventHandler FolderOpenButtonClick
        {
            add
            {
                folderOpenB.Click += value;
            }
            remove
            {
                folderOpenB.Click -= value;
            }
        }
        public event EventHandler FolderCancelButtonClick
        {
            add
            {
                folderCancelB.Click += value;
            }
            remove
            {
                folderCancelB.Click -= value;
            }
        }        
    }
}
