namespace DotaHIT.Extras
{
    partial class FileBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.browsePanel = new System.Windows.Forms.Panel();
            this.explorerPanel = new System.Windows.Forms.Panel();
            this.explorerLV = new System.Windows.Forms.ListView();
            this.ColumnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeaderAttributes = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeaderSize = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeaderType = new System.Windows.Forms.ColumnHeader();
            this.ColumnHeaderModifyDate = new System.Windows.Forms.ColumnHeader();
            this.fileChooserPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.filterCmbB = new System.Windows.Forms.ComboBox();
            this.cancelB = new System.Windows.Forms.Button();
            this.openB = new System.Windows.Forms.Button();
            this.expListSplitter = new System.Windows.Forms.Splitter();
            this.foldersExpTree = new ExpTreeLib.ExpTree();
            this.navigationPanel = new System.Windows.Forms.Panel();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.tsCropPanel = new System.Windows.Forms.Panel();
            this.naviToolStrip = new System.Windows.Forms.ToolStrip();
            this.upToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toDHToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toWar3ReplayToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toWar3MapsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.goB = new System.Windows.Forms.Button();
            this.browseStatusBar = new System.Windows.Forms.StatusBar();
            this.folderChooserPanel = new System.Windows.Forms.Panel();
            this.folderCancelB = new System.Windows.Forms.Button();
            this.folderOpenB = new System.Windows.Forms.Button();
            this.filesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noitemsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteShortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browsePanel.SuspendLayout();
            this.explorerPanel.SuspendLayout();
            this.fileChooserPanel.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.tsCropPanel.SuspendLayout();
            this.naviToolStrip.SuspendLayout();
            this.folderChooserPanel.SuspendLayout();
            this.filesContextMenuStrip.SuspendLayout();
            this.noitemsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // browsePanel
            // 
            this.browsePanel.BackColor = System.Drawing.SystemColors.Control;
            this.browsePanel.Controls.Add(this.explorerPanel);
            this.browsePanel.Controls.Add(this.expListSplitter);
            this.browsePanel.Controls.Add(this.foldersExpTree);
            this.browsePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browsePanel.Location = new System.Drawing.Point(0, 26);
            this.browsePanel.Name = "browsePanel";
            this.browsePanel.Size = new System.Drawing.Size(733, 286);
            this.browsePanel.TabIndex = 8;
            // 
            // explorerPanel
            // 
            this.explorerPanel.Controls.Add(this.explorerLV);
            this.explorerPanel.Controls.Add(this.fileChooserPanel);
            this.explorerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerPanel.Location = new System.Drawing.Point(228, 0);
            this.explorerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.explorerPanel.Name = "explorerPanel";
            this.explorerPanel.Size = new System.Drawing.Size(505, 286);
            this.explorerPanel.TabIndex = 7;
            // 
            // explorerLV
            // 
            this.explorerLV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.explorerLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderName,
            this.ColumnHeaderAttributes,
            this.ColumnHeaderSize,
            this.ColumnHeaderType,
            this.ColumnHeaderModifyDate});
            this.explorerLV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerLV.HideSelection = false;
            this.explorerLV.LabelEdit = true;
            this.explorerLV.Location = new System.Drawing.Point(0, 0);
            this.explorerLV.MultiSelect = false;
            this.explorerLV.Name = "explorerLV";
            this.explorerLV.Size = new System.Drawing.Size(505, 207);
            this.explorerLV.TabIndex = 6;
            this.explorerLV.UseCompatibleStateImageBehavior = false;
            this.explorerLV.View = System.Windows.Forms.View.Details;
            this.explorerLV.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.explorerLV_MouseDoubleClick);
            this.explorerLV.VisibleChanged += new System.EventHandler(this.lv1_VisibleChanged);
            this.explorerLV.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.explorerLV_AfterLabelEdit);
            this.explorerLV.SelectedIndexChanged += new System.EventHandler(this.lv1_SelectedIndexChanged);
            this.explorerLV.MouseUp += new System.Windows.Forms.MouseEventHandler(this.explorerLV_MouseUp);
            this.explorerLV.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.explorerLV_BeforeLabelEdit);
            this.explorerLV.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lv1_KeyDown);
            // 
            // ColumnHeaderName
            // 
            this.ColumnHeaderName.Text = "Name";
            this.ColumnHeaderName.Width = 178;
            // 
            // ColumnHeaderAttributes
            // 
            this.ColumnHeaderAttributes.Text = "Attributes";
            this.ColumnHeaderAttributes.Width = 72;
            // 
            // ColumnHeaderSize
            // 
            this.ColumnHeaderSize.Text = "Size";
            this.ColumnHeaderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ColumnHeaderSize.Width = 72;
            // 
            // ColumnHeaderType
            // 
            this.ColumnHeaderType.Text = "Type";
            this.ColumnHeaderType.Width = 100;
            // 
            // ColumnHeaderModifyDate
            // 
            this.ColumnHeaderModifyDate.Text = "Modified";
            this.ColumnHeaderModifyDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.ColumnHeaderModifyDate.Width = 80;
            // 
            // fileChooserPanel
            // 
            this.fileChooserPanel.Controls.Add(this.label2);
            this.fileChooserPanel.Controls.Add(this.label1);
            this.fileChooserPanel.Controls.Add(this.fileNameTextBox);
            this.fileChooserPanel.Controls.Add(this.filterCmbB);
            this.fileChooserPanel.Controls.Add(this.cancelB);
            this.fileChooserPanel.Controls.Add(this.openB);
            this.fileChooserPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fileChooserPanel.Location = new System.Drawing.Point(0, 207);
            this.fileChooserPanel.Name = "fileChooserPanel";
            this.fileChooserPanel.Size = new System.Drawing.Size(505, 79);
            this.fileChooserPanel.TabIndex = 7;
            this.fileChooserPanel.Visible = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Files of type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "File name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTextBox.Location = new System.Drawing.Point(106, 12);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(297, 20);
            this.fileNameTextBox.TabIndex = 3;
            // 
            // filterCmbB
            // 
            this.filterCmbB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filterCmbB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterCmbB.FormattingEnabled = true;
            this.filterCmbB.Location = new System.Drawing.Point(106, 38);
            this.filterCmbB.Name = "filterCmbB";
            this.filterCmbB.Size = new System.Drawing.Size(297, 21);
            this.filterCmbB.TabIndex = 2;
            this.filterCmbB.SelectedIndexChanged += new System.EventHandler(this.filterCmbB_SelectedIndexChanged);
            // 
            // cancelB
            // 
            this.cancelB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelB.Location = new System.Drawing.Point(430, 38);
            this.cancelB.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.cancelB.Name = "cancelB";
            this.cancelB.Size = new System.Drawing.Size(75, 23);
            this.cancelB.TabIndex = 1;
            this.cancelB.Text = "Cancel";
            this.cancelB.UseVisualStyleBackColor = true;
            // 
            // openB
            // 
            this.openB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openB.Location = new System.Drawing.Point(430, 9);
            this.openB.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.openB.Name = "openB";
            this.openB.Size = new System.Drawing.Size(75, 23);
            this.openB.TabIndex = 0;
            this.openB.Text = "Open";
            this.openB.UseVisualStyleBackColor = true;
            this.openB.Click += new System.EventHandler(this.explorer_ItemActivate);
            // 
            // expListSplitter
            // 
            this.expListSplitter.Location = new System.Drawing.Point(222, 0);
            this.expListSplitter.Name = "expListSplitter";
            this.expListSplitter.Size = new System.Drawing.Size(6, 286);
            this.expListSplitter.TabIndex = 2;
            this.expListSplitter.TabStop = false;
            // 
            // foldersExpTree
            // 
            this.foldersExpTree.AllowDrop = true;
            this.foldersExpTree.Cursor = System.Windows.Forms.Cursors.Default;
            this.foldersExpTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.foldersExpTree.Location = new System.Drawing.Point(0, 0);
            this.foldersExpTree.Name = "foldersExpTree";
            this.foldersExpTree.ShowRootLines = false;
            this.foldersExpTree.Size = new System.Drawing.Size(222, 286);
            this.foldersExpTree.TabIndex = 0;
            this.foldersExpTree.ExpTreeNodeSelected += new ExpTreeLib.ExpTree.ExpTreeNodeSelectedEventHandler(this.expTree1_ExpTreeNodeSelected);
            // 
            // navigationPanel
            // 
            this.navigationPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.navigationPanel.BackColor = System.Drawing.Color.Gainsboro;
            this.navigationPanel.Controls.Add(this.pathTextBox);
            this.navigationPanel.Controls.Add(this.tsCropPanel);
            this.navigationPanel.Controls.Add(this.goB);
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.navigationPanel.Location = new System.Drawing.Point(0, 0);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.navigationPanel.Size = new System.Drawing.Size(733, 26);
            this.navigationPanel.TabIndex = 9;
            // 
            // pathTextBox
            // 
            this.pathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTextBox.Location = new System.Drawing.Point(227, 3);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(468, 20);
            this.pathTextBox.TabIndex = 1;
            this.pathTextBox.WordWrap = false;
            this.pathTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pathTextBox_KeyPress);
            // 
            // tsCropPanel
            // 
            this.tsCropPanel.BackColor = System.Drawing.Color.Transparent;
            this.tsCropPanel.Controls.Add(this.naviToolStrip);
            this.tsCropPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.tsCropPanel.Location = new System.Drawing.Point(0, 3);
            this.tsCropPanel.Name = "tsCropPanel";
            this.tsCropPanel.Size = new System.Drawing.Size(227, 21);
            this.tsCropPanel.TabIndex = 0;
            // 
            // naviToolStrip
            // 
            this.naviToolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.naviToolStrip.AutoSize = false;
            this.naviToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.naviToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.naviToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.naviToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upToolStripButton,
            this.toDHToolStripButton,
            this.toWar3ReplayToolStripButton,
            this.toWar3MapsToolStripButton});
            this.naviToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.naviToolStrip.Location = new System.Drawing.Point(0, -2);
            this.naviToolStrip.MinimumSize = new System.Drawing.Size(0, 25);
            this.naviToolStrip.Name = "naviToolStrip";
            this.naviToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.naviToolStrip.Size = new System.Drawing.Size(227, 25);
            this.naviToolStrip.TabIndex = 7;
            this.naviToolStrip.Text = "toolStrip1";
            // 
            // upToolStripButton
            // 
            this.upToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.upToolStripButton.AutoSize = false;
            this.upToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.upToolStripButton.Image = global::DotaHIT.Properties.Resources.GoToParentFolderHS;
            this.upToolStripButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.upToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.upToolStripButton.Name = "upToolStripButton";
            this.upToolStripButton.Size = new System.Drawing.Size(23, 20);
            this.upToolStripButton.Text = "Up One Level";
            this.upToolStripButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.upToolStripButton.Click += new System.EventHandler(this.upToolStripButton_Click);
            // 
            // toDHToolStripButton
            // 
            this.toDHToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toDHToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toDHToolStripButton.Name = "toDHToolStripButton";
            this.toDHToolStripButton.Size = new System.Drawing.Size(55, 22);
            this.toDHToolStripButton.Text = "DotaHIT\\";
            this.toDHToolStripButton.ToolTipText = "go to DotaHIT folder";
            this.toDHToolStripButton.Click += new System.EventHandler(this.toDHToolStripButton_Click);
            // 
            // toWar3ReplayToolStripButton
            // 
            this.toWar3ReplayToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toWar3ReplayToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toWar3ReplayToolStripButton.Name = "toWar3ReplayToolStripButton";
            this.toWar3ReplayToolStripButton.Size = new System.Drawing.Size(97, 22);
            this.toWar3ReplayToolStripButton.Text = "WarCraft\\Replay\\";
            this.toWar3ReplayToolStripButton.ToolTipText = "go to WarCraft Replay folder";
            this.toWar3ReplayToolStripButton.Visible = false;
            this.toWar3ReplayToolStripButton.Click += new System.EventHandler(this.toWar3ReplatToolStripButton_Click);
            // 
            // toWar3MapsToolStripButton
            // 
            this.toWar3MapsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toWar3MapsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toWar3MapsToolStripButton.Name = "toWar3MapsToolStripButton";
            this.toWar3MapsToolStripButton.Size = new System.Drawing.Size(89, 22);
            this.toWar3MapsToolStripButton.Text = "WarCraft\\Maps\\";
            this.toWar3MapsToolStripButton.ToolTipText = "go to WarCraft Maps folder";
            this.toWar3MapsToolStripButton.Visible = false;
            this.toWar3MapsToolStripButton.Click += new System.EventHandler(this.toWar3MapsToolStripButton_Click);
            // 
            // goB
            // 
            this.goB.Dock = System.Windows.Forms.DockStyle.Right;
            this.goB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.goB.Location = new System.Drawing.Point(695, 3);
            this.goB.Name = "goB";
            this.goB.Size = new System.Drawing.Size(38, 21);
            this.goB.TabIndex = 9;
            this.goB.Text = "Go";
            this.goB.UseVisualStyleBackColor = false;
            this.goB.Click += new System.EventHandler(this.goB_Click);
            // 
            // browseStatusBar
            // 
            this.browseStatusBar.Location = new System.Drawing.Point(0, 351);
            this.browseStatusBar.Name = "browseStatusBar";
            this.browseStatusBar.Size = new System.Drawing.Size(733, 22);
            this.browseStatusBar.TabIndex = 8;
            this.browseStatusBar.Text = "Ready";
            // 
            // folderChooserPanel
            // 
            this.folderChooserPanel.Controls.Add(this.folderCancelB);
            this.folderChooserPanel.Controls.Add(this.folderOpenB);
            this.folderChooserPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.folderChooserPanel.Location = new System.Drawing.Point(0, 312);
            this.folderChooserPanel.Name = "folderChooserPanel";
            this.folderChooserPanel.Size = new System.Drawing.Size(733, 39);
            this.folderChooserPanel.TabIndex = 8;
            this.folderChooserPanel.Visible = false;
            // 
            // folderCancelB
            // 
            this.folderCancelB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.folderCancelB.Location = new System.Drawing.Point(644, 8);
            this.folderCancelB.Name = "folderCancelB";
            this.folderCancelB.Size = new System.Drawing.Size(75, 23);
            this.folderCancelB.TabIndex = 2;
            this.folderCancelB.Text = "Cancel";
            this.folderCancelB.UseVisualStyleBackColor = true;
            // 
            // folderOpenB
            // 
            this.folderOpenB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.folderOpenB.Location = new System.Drawing.Point(563, 8);
            this.folderOpenB.Name = "folderOpenB";
            this.folderOpenB.Size = new System.Drawing.Size(75, 23);
            this.folderOpenB.TabIndex = 1;
            this.folderOpenB.Text = "OK";
            this.folderOpenB.UseVisualStyleBackColor = true;
            // 
            // filesContextMenuStrip
            // 
            this.filesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem});
            this.filesContextMenuStrip.Name = "filesContextMenuStrip";
            this.filesContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.filesContextMenuStrip.Size = new System.Drawing.Size(114, 126);
            this.filesContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.filesContextMenuStrip_Opening);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(110, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(110, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // noitemsContextMenuStrip
            // 
            this.noitemsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pasteToolStripMenuItem,
            this.pasteShortcutToolStripMenuItem});
            this.noitemsContextMenuStrip.Name = "noitemsContextMenuStrip";
            this.noitemsContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.noitemsContextMenuStrip.Size = new System.Drawing.Size(146, 48);
            this.noitemsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.noitemsContextMenuStrip_Opening);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // pasteShortcutToolStripMenuItem
            // 
            this.pasteShortcutToolStripMenuItem.Name = "pasteShortcutToolStripMenuItem";
            this.pasteShortcutToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.pasteShortcutToolStripMenuItem.Text = "Paste Shortcut";
            this.pasteShortcutToolStripMenuItem.Click += new System.EventHandler(this.pasteShortcutToolStripMenuItem_Click);
            // 
            // FileBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.browsePanel);
            this.Controls.Add(this.navigationPanel);
            this.Controls.Add(this.folderChooserPanel);
            this.Controls.Add(this.browseStatusBar);
            this.Name = "FileBrowser";
            this.Size = new System.Drawing.Size(733, 373);
            this.Load += new System.EventHandler(this.FileBrowser_Load);
            this.browsePanel.ResumeLayout(false);
            this.explorerPanel.ResumeLayout(false);
            this.fileChooserPanel.ResumeLayout(false);
            this.fileChooserPanel.PerformLayout();
            this.navigationPanel.ResumeLayout(false);
            this.navigationPanel.PerformLayout();
            this.tsCropPanel.ResumeLayout(false);
            this.naviToolStrip.ResumeLayout(false);
            this.naviToolStrip.PerformLayout();
            this.folderChooserPanel.ResumeLayout(false);
            this.filesContextMenuStrip.ResumeLayout(false);
            this.noitemsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel browsePanel;
        internal System.Windows.Forms.ListView explorerLV;
        internal System.Windows.Forms.ColumnHeader ColumnHeaderName;
        internal System.Windows.Forms.ColumnHeader ColumnHeaderAttributes;
        internal System.Windows.Forms.ColumnHeader ColumnHeaderSize;
        internal System.Windows.Forms.ColumnHeader ColumnHeaderType;
        internal System.Windows.Forms.ColumnHeader ColumnHeaderModifyDate;
        private System.Windows.Forms.Splitter expListSplitter;
        private ExpTreeLib.ExpTree foldersExpTree;
        private System.Windows.Forms.Panel navigationPanel;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Panel tsCropPanel;
        private System.Windows.Forms.ToolStrip naviToolStrip;
        private System.Windows.Forms.ToolStripButton upToolStripButton;
        private System.Windows.Forms.ToolStripButton toDHToolStripButton;
        private System.Windows.Forms.Button goB;
        private System.Windows.Forms.StatusBar browseStatusBar;
        private System.Windows.Forms.Panel explorerPanel;
        private System.Windows.Forms.Panel fileChooserPanel;
        private System.Windows.Forms.ComboBox filterCmbB;
        private System.Windows.Forms.Button cancelB;
        private System.Windows.Forms.Button openB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Panel folderChooserPanel;
        private System.Windows.Forms.Button folderCancelB;
        private System.Windows.Forms.Button folderOpenB;
        private System.Windows.Forms.ToolStripButton toWar3ReplayToolStripButton;
        private System.Windows.Forms.ToolStripButton toWar3MapsToolStripButton;
        private System.Windows.Forms.ContextMenuStrip filesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip noitemsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteShortcutToolStripMenuItem;
    }
}
