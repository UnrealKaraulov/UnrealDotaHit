using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;
//using ExpTreeLib.CShItem;
//using ExpTreeLib.ShellDll;
//using ExpTreeLib.SystemImageListManager;




namespace ExpTreeLib
{
	[DefaultProperty("StartUpDirectory"), DefaultEvent("StartUpDirectoryChanged")]public class ExpTree : System.Windows.Forms.UserControl
	{
		private TreeNode Root;
		
		public delegate void StartUpDirectoryChangedEventHandler(StartDir newVal);
		private StartUpDirectoryChangedEventHandler StartUpDirectoryChangedEvent;
		
		public event StartUpDirectoryChangedEventHandler StartUpDirectoryChanged
		{
			add
			{
				StartUpDirectoryChangedEvent = (StartUpDirectoryChangedEventHandler) System.Delegate.Combine(StartUpDirectoryChangedEvent, value);
			}
			remove
			{
				StartUpDirectoryChangedEvent = (StartUpDirectoryChangedEventHandler) System.Delegate.Remove(StartUpDirectoryChangedEvent, value);
			}
		}
		
		
		public delegate void ExpTreeNodeSelectedEventHandler(string SelPath, CShItem Item);
		private ExpTreeNodeSelectedEventHandler ExpTreeNodeSelectedEvent;
		
		public event ExpTreeNodeSelectedEventHandler ExpTreeNodeSelected
		{
			add
			{
				ExpTreeNodeSelectedEvent = (ExpTreeNodeSelectedEventHandler) System.Delegate.Combine(ExpTreeNodeSelectedEvent, value);
			}
			remove
			{
				ExpTreeNodeSelectedEvent = (ExpTreeNodeSelectedEventHandler) System.Delegate.Remove(ExpTreeNodeSelectedEvent, value);
			}
		}
		
		
		private bool EnableEventPost = true; //flag to supress ExpTreeNodeSelected raising during refresh and
		
		private TVDragWrapper DragDropHandler;
		
		private bool m_showHiddenFolders = true;
		
		
		#region " Windows Form Designer generated code "
		
		public ExpTree()
		{
			
			//This call is required by the Windows Form Designer.
			InitializeComponent();
			
			//Add any initialization after the InitializeComponent() call
			
			
			//setting the imagelist here allows many good things to happen, but
			// also one bad thing -- the "tooltip" like display of selectednode.text
			// is made invisible.  This remains a problem to be solved.
			SystemImageListManager.SetTreeViewImageList(tv1, false);
			
			StartUpDirectoryChanged += new ExpTreeLib.ExpTree.StartUpDirectoryChangedEventHandler(OnStartUpDirectoryChanged);
			
			OnStartUpDirectoryChanged(m_StartUpDirectory);
			
			if (tv1.IsHandleCreated)
			{
				if (this.AllowDrop)
				{
					if (Application.OleRequired() == System.Threading.ApartmentState.STA)
					{
						DragDropHandler = new TVDragWrapper(tv1);
						DragDropHandler.ShDragEnter += new ExpTreeLib.TVDragWrapper.ShDragEnterEventHandler(DragWrapper_ShDragEnter);
						DragDropHandler.ShDragLeave += new ExpTreeLib.TVDragWrapper.ShDragLeaveEventHandler(DragWrapper_ShDragLeave);
						DragDropHandler.ShDragOver += new ExpTreeLib.TVDragWrapper.ShDragOverEventHandler(DragWrapper_ShDragOver);
						DragDropHandler.ShDragDrop += new ExpTreeLib.TVDragWrapper.ShDragDropEventHandler(DragWrapper_ShDragDrop);
						int res;
						res = ShellDll.RegisterDragDrop(tv1.Handle, DragDropHandler);
						if (!(res == 0) || (res == - 2147221247))
						{
							Marshal.ThrowExceptionForHR(res);
							throw (new Exception("Failed to Register DragDrop for " + this.Name));
						}
					}
					else
					{
						throw (new ThreadStateException("ThreadMustBeSTA"));
					}
				}
			}
			
			
		}
		//ExpTree overrides dispose to clean up the component list.
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!(components == null))
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		internal System.Windows.Forms.TreeView tv1;
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.tv1 = new System.Windows.Forms.TreeView();
			this.tv1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(tv1_BeforeExpand);
			this.tv1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(tv1_AfterSelect);
			this.tv1.VisibleChanged += new System.EventHandler(tv1_VisibleChanged);
			this.tv1.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(tv1_BeforeCollapse);
			this.tv1.HandleDestroyed += new System.EventHandler(tv1_HandleDestroyed);
			this.tv1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(tv1_ItemDrag);
			this.SuspendLayout();
			//
			//tv1
			//
			this.tv1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tv1.HideSelection = false;
			this.tv1.ImageIndex = - 1;
			this.tv1.Name = "tv1";
			this.tv1.SelectedImageIndex = - 1;
			this.tv1.ShowRootLines = false;
			this.tv1.Size = new System.Drawing.Size(200, 264);
			this.tv1.TabIndex = 0;
			//
			//ExpTree
			//
			this.AllowDrop = true;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {this.tv1});
			this.Name = "ExpTree";
			this.Size = new System.Drawing.Size(200, 264);
			this.ResumeLayout(false);
			
		}
		
		#endregion
		
		#region "   Public Properties"
		
		#region "       RootItem"
		//<Summary>
		// RootItem is a Run-Time only Property
		// Setting this Item via an External call results in
		//  re-setting the entire tree to be rooted in the
		//  input CShItem
		// The new CShItem must be a valid CShItem of some kind
		//  of Folder (File Folder or System Folder)
		// Attempts to set it using a non-Folder CShItem are ignored
		//</Summary>
		[Browsable(false)]public CShItem RootItem
		{
			get
			{
                return Root.Tag as CShItem;
			}
			set
			{
				if (value.IsFolder)
				{
					if (Root != null)
					{
						ClearTree();
					}
					Root = new TreeNode(value.DisplayName);
					BuildTree(value.GetDirectories(true));
					Root.ImageIndex = SystemImageListManager.GetIconIndex(value, false, false);
					Root.SelectedImageIndex = Root.ImageIndex;
					Root.Tag = value;
					tv1.Nodes.Add(Root);
					Root.Expand();
					tv1.SelectedNode = Root;
				}
			}
		}
		#endregion
		
		#region "       SelectedItem"
		[Browsable(false)]public CShItem SelectedItem
		{
			get
			{
				if (tv1.SelectedNode != null)
				{
                    return tv1.SelectedNode.Tag as CShItem;
				}
				else
				{
					return null;
				}
			}
		}
		#endregion
		
		#region "       ShowHidden"
		[Category("Options"), Description("Show Hidden Directories."), DefaultValue(true), Browsable(true)]public bool ShowHiddenFolders
		{
			get
			{
				return m_showHiddenFolders;
			}
			set
			{
				m_showHiddenFolders = value;
			}
		}
		#endregion
		
		#region "       ShowRootLines"
		[Category("Options"), Description("Allow Collapse of Root Item."), DefaultValue(true), Browsable(true)]public bool ShowRootLines
		{
			get
			{
				return tv1.ShowRootLines;
			}
			set
			{
				if (!(value == tv1.ShowRootLines))
				{
					tv1.ShowRootLines = value;
					tv1.Refresh();
				}
			}
		}
		#endregion
		
		#region "       StartupDir"
		
		public enum StartDir
		{
			Desktop = 0x0,
			Programs = 0x2,
			Controls = 0x3,
			Printers = 0x4,
			Personal = 0x5,
			Favorites = 0x6,
			Startup = 0x7,
			Recent = 0x8,
			SendTo = 0x9,
			StartMenu = 0xB,
			MyDocuments = 0xC,
			//MyMusic = &HD
			//MyVideo = &HE
			DesktopDirectory = 0x10,
			MyComputer = 0x11,
			My_Network_Places = 0x12,
			//NETHOOD = &H13
			//FONTS = &H14
			ApplicatationData = 0x1A,
			//PRINTHOOD = &H1B
			Internet_Cache = 0x20,
			Cookies = 0x21,
			History = 0x22,
			Windows = 0x24,
			System = 0x25,
			Program_Files = 0x26,
			MyPictures = 0x27,
			Profile = 0x28,
			Systemx86 = 0x29,
			AdminTools = 0x30
		}
		
		private StartDir m_StartUpDirectory = StartDir.Desktop;
		
		[Category("Options"), Description("Sets the Initial Directory of the Tree"), DefaultValue(StartDir.Desktop), Browsable(true)]public StartDir StartUpDirectory
		{
			get
			{
				return m_StartUpDirectory;
			}
			set
			{
				if (Array.IndexOf(@Enum.GetValues(value.GetType()), value) >= 0)
				{
					m_StartUpDirectory = value;
					if (StartUpDirectoryChangedEvent != null)
						StartUpDirectoryChangedEvent(value);
				}
				else
				{
					throw (new ApplicationException("Invalid Initial StartUpDirectory"));
				}
			}
		}
		#endregion
		
		#endregion
		
		#region "   Public Methods"
		
		#region "       RefreshTree"
		///<Summary>RefreshTree Method thanks to Calum McLellan</Summary>
		[Description("Refresh the Tree and all nodes through the currently selected item")]public void RefreshTree(CShItem rootCSI)
		{
			//Modified to use ExpandANode(CShItem) rather than ExpandANode(path)
			//Set refresh variable for BeforeExpand method
			EnableEventPost = false;
			//Begin Calum's change -- With some modification
			TreeNode Selnode;
			if (this.tv1.SelectedNode == null)
			{
				Selnode = this.Root;
			}
			else
			{
				Selnode = this.tv1.SelectedNode;
			}
			//End Calum's change
			try
			{
				this.tv1.BeginUpdate();
				CShItem SelCSI = Selnode.Tag as CShItem;
				//Set root node
				if (rootCSI == null)
				{
					this.RootItem = this.RootItem;
				}
				else
				{
					this.RootItem = rootCSI;
				}
				//Try to expand the node
				if (! this.ExpandANode(SelCSI))
				{
					ArrayList nodeList = new ArrayList();
					while (Selnode.Parent != null)
					{
						nodeList.Add(Selnode.Parent);
						Selnode = Selnode.Parent;
					}
					
					foreach (TreeNode tempLoopVar_Selnode in nodeList)
					{
						Selnode = tempLoopVar_Selnode;
						if (this.ExpandANode((CShItem) Selnode.Tag))
						{
							goto endOfForLoop;
						}
					}
endOfForLoop:
					1.GetHashCode() ; //nop
				}
				//Reset refresh variable for BeforeExpand method
			}
			finally
			{
				this.tv1.EndUpdate();
			}
			EnableEventPost = true;
			//We suppressed EventPosting during refresh, so give it one now
			tv1_AfterSelect(this, new TreeViewEventArgs(tv1.SelectedNode));
		}
		#endregion
		
		#region "       ExpandANode"
		public bool ExpandANode(string newPath)
		{
			bool returnValue;
			returnValue = false; //assume failure
			CShItem newItem;
			try
			{
				newItem = ExpTreeLib.CShItem.GetCShItem(newPath);
				if (newItem == null)
				{
					return returnValue;
				}
				if (! newItem.IsFolder)
				{
					return returnValue;
				}
			}
			catch
			{
				return returnValue;
			}
			return ExpandANode(newItem);
		}
		
		public bool ExpandANode(CShItem newItem)
		{
			bool returnValue;
			returnValue = false; //assume failure
			System.Windows.Forms.TreeNode baseNode = Root;
			tv1.BeginUpdate();
			baseNode.Expand(); //Ensure base is filled in
			//do the drill down -- Node to expand must be included in tree
			TreeNode testNode;
			int lim = CShItem.PidlCount(newItem.PIDL) - CShItem.PidlCount((baseNode.Tag as CShItem).PIDL);
			//TODO: Test ExpandARow again on XP to ensure that the CP problem ix fixed
			while (lim > 0)
			{
				foreach (TreeNode tempLoopVar_testNode in baseNode.Nodes)
				{
					testNode = tempLoopVar_testNode;
					if (CShItem.IsAncestorOf(testNode.Tag as CShItem, newItem, false))
					{
						baseNode = testNode;
						RefreshNode(baseNode); //ensure up-to-date
						baseNode.Expand();
						lim--;
						goto NEXLEV;
					}
				}
				goto XIT; //on falling thru For, we can't find it, so get out
NEXLEV:
				1.GetHashCode() ; //nop
			}
			//after falling thru here, we have found & expanded the node
			this.tv1.HideSelection = false;
			this.Select();
			this.tv1.SelectedNode = baseNode;
			returnValue = true;
XIT:
			tv1.EndUpdate();
			return returnValue;
		}
		
		public void EnsureSelectedVisible()
		{
			if (tv1.SelectedNode != null)
			{
				tv1.SelectedNode.EnsureVisible();
			}
		}
		#endregion
		
		#endregion
		
		#region "   Initial Dir Set Handler"
		
		private void OnStartUpDirectoryChanged(StartDir newVal)
		{
			if (Root != null)
			{
				ClearTree();
			}
			CShItem special;
			special = ExpTreeLib.CShItem.GetCShItem((ShellDll.CSIDL) (int)(m_StartUpDirectory));
			Root = new TreeNode(special.DisplayName);
			Root.ImageIndex = SystemImageListManager.GetIconIndex(special, false, false);
			Root.SelectedImageIndex = Root.ImageIndex;
			Root.Tag = special;
			BuildTree(special.GetDirectories(true));
			tv1.Nodes.Add(Root);
			Root.Expand();
		}

        private void BuildTree(List<CShItem> L1)
		{
			L1.Sort();
			CShItem CSI;
			foreach (CShItem tempLoopVar_CSI in L1)
			{
				CSI = tempLoopVar_CSI;
				if (!(CSI.IsHidden && ! m_showHiddenFolders))
				{
					Root.Nodes.Add(MakeNode(CSI));
				}
			}
		}
		
		private TreeNode MakeNode(CShItem item)
		{
			TreeNode newNode = new TreeNode(item.DisplayName);
			newNode.Tag = item;
			newNode.ImageIndex = SystemImageListManager.GetIconIndex(item, false, false);
			newNode.SelectedImageIndex = SystemImageListManager.GetIconIndex(item, true, false);
			//The following code, from Calum implements the following logic
			// Allow/disallow the showing of Hidden folders based on ShowHidden Propert
			// For Removable disks, always show + (allow expansion) - avoids floppy access
			// For all others, add + based on HasSubFolders
			//  Except - If showing Hidden dirs, do extra check to  allow for
			//  the case of all hidden items in the Dir which will cause
			//  HasSubFolders to be always left unset
			if (item.IsRemovable) //Calum's fix to hidden file fix
			{
				newNode.Nodes.Add(new TreeNode(" : "));
			}
			else if (item.HasSubFolders)
			{
				newNode.Nodes.Add(new TreeNode(" : "));
				//Begin Calum's change so Hidden dirs with all hidden content are expandable
			}
			else if (item.GetDirectories(true).Count > 0) //Added Code
			{
				newNode.Nodes.Add(new TreeNode(" : ")); //Added Code
				//End Calum's change
			}
			return newNode;
		}
		
		private void ClearTree()
		{
			tv1.Nodes.Clear();
			Root = null;
		}
		#endregion
		
		#region "   TreeView BeforeExpand Event"
		
		private void tv1_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			Cursor oldCursor = Cursor;
			Cursor = Cursors.WaitCursor;
			if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text.Equals(" : "))
			{
				//Debug.WriteLine("Expanding -- " & e.Node.Text)
				e.Node.Nodes.Clear();
                CShItem CSI = e.Node.Tag as CShItem;
                List<CShItem> D = CSI.GetDirectories(true);
				
				if (D.Count > 0)
				{
					D.Sort(); //uses the class comparer
					CShItem item;
					foreach (CShItem tempLoopVar_item in D)
					{
						item = tempLoopVar_item;
						if (!(item.IsHidden && ! m_showHiddenFolders))
						{
							e.Node.Nodes.Add(MakeNode(item));
						}
					}
				}
			}
			else //Ensure content is accurate
			{
				RefreshNode(e.Node);
			}
			Cursor = oldCursor;
		}
		#endregion
		
		#region "   TreeView AfterSelect Event"
		private void tv1_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode node = e.Node;
            CShItem CSI = e.Node.Tag as CShItem;
			if (CSI == Root.Tag && ! tv1.ShowRootLines)
			{
				try
				{
					tv1.BeginUpdate();
					tv1.ShowRootLines = true;
					RefreshNode(node);
					tv1.ShowRootLines = false;
				}
				finally
				{
					tv1.EndUpdate();
				}
			}
			else
			{
				RefreshNode(node);
			}
			if (EnableEventPost) //turned off during RefreshTree
			{
				if (CSI.Path.StartsWith(":"))
				{
					if (ExpTreeNodeSelectedEvent != null)
						ExpTreeNodeSelectedEvent(CSI.DisplayName, CSI);
				}
				else
				{
					if (ExpTreeNodeSelectedEvent != null)
						ExpTreeNodeSelectedEvent(CSI.Path, CSI);
				}
			}
		}
		#endregion
		
		#region "   RefreshNode Sub"
		
		private void RefreshNode(TreeNode thisRoot)
		{
			//Debug.WriteLine("In RefreshNode: Node = " & thisRoot.Tag.path & " -- " & thisRoot.Tag.displayname)
			if (!(thisRoot.Nodes.Count == 1 && thisRoot.Nodes[0].Text.Equals(" : ")))
			{
                CShItem thisItem = thisRoot.Tag as CShItem;
				if (thisItem.RefreshDirectories()) //RefreshDirectories True = the contained list of Directories has changed
				{
                    List<CShItem> curDirs = thisItem.GetDirectories(false); //suppress 2nd refresh
					ArrayList delNodes = new ArrayList();
					TreeNode node;
					foreach (TreeNode tempLoopVar_node in thisRoot.Nodes) //this is the old node contents
					{
						node = tempLoopVar_node;
						int i;
						for (i = 0; i <= curDirs.Count - 1; i++)
						{
							if (((CShItem) (curDirs[i])).Equals(node.Tag))
							{
								curDirs.RemoveAt(i); //found it, don't compare again
								goto NXTOLD;
							}
						}
						//fall thru = node no longer here
						delNodes.Add(node);
NXTOLD:
						1.GetHashCode() ; //nop
					}
					if (delNodes.Count + curDirs.Count > 0) //had changes
					{
						try
						{
							tv1.BeginUpdate();
							foreach (TreeNode tempLoopVar_node in delNodes) //dir not here anymore, delete node
							{
								node = tempLoopVar_node;
								thisRoot.Nodes.Remove(node);
							}
							//any CShItems remaining in curDirs is a new dir under thisRoot
							CShItem csi;
							foreach (CShItem tempLoopVar_csi in curDirs)
							{
								csi = tempLoopVar_csi;
								if (!(csi.IsHidden && ! m_showHiddenFolders))
								{
									thisRoot.Nodes.Add(MakeNode(csi));
								}
							}
							//we only need to resort if we added
							//sort is based on CShItem in .Tag
							if (curDirs.Count > 0)
							{
								TreeNode[] tmpA = new TreeNode[thisRoot.Nodes.Count];
								thisRoot.Nodes.CopyTo(tmpA, 0);
								Array.Sort(tmpA, new ExpTreeLib.CShItem.TagComparer());
								thisRoot.Nodes.Clear();
								thisRoot.Nodes.AddRange(tmpA);
							}
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Error in RefreshNode -- " + ex.ToString() + "\r\n" + ex.StackTrace);
						}
						finally
						{
							tv1.EndUpdate();
						}
					}
				}
			}
			//Debug.WriteLine("Exited RefreshNode")
		}
		
		#endregion
		
		#region "   TreeView VisibleChanged Event"
		///<Summary>When a form containing this control is Hidden and then re-Shown,
		/// the association to the SystemImageList is lost.  Also lost is the
		/// Expanded state of the various TreeNodes.
		/// The VisibleChanged Event occurs when the form is re-shown (and other times
		///  as well).
		/// We re-establish the SystemImageList as the ImageList for the TreeView and
		/// restore at least some of the Expansion.</Summary>
		private void tv1_VisibleChanged(object sender, System.EventArgs e)
		{
			if (tv1.Visible)
			{
				SystemImageListManager.SetTreeViewImageList(tv1, false);
				if (Root != null)
				{
					Root.Expand();
					if (tv1.SelectedNode != null)
					{
						tv1.SelectedNode.Expand();
					}
					else
					{
						tv1.SelectedNode = this.Root;
					}
				}
			}
		}
		#endregion
		
		#region "   TreeView BeforeCollapse Event"
		///<Summary>Should never occur since if the condition tested for is True,
		/// the user should never be able to Collapse the node. However, it is
		/// theoretically possible for the code to request a collapse of this node
		/// If it occurs, cancel it</Summary>
		private void tv1_BeforeCollapse(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
			if (! tv1.ShowRootLines && e.Node == Root)
			{
				e.Cancel = true;
			}
		}
		#endregion
		
		#region "   tv1_HandleDestroyed"
		private void tv1_HandleDestroyed(object sender, EventArgs e)
		{
			//Debug.WriteLine("in handle destroyed")
			if (this.AllowDrop)
			{
				int res;
				res = ShellDll.RevokeDragDrop(tv1.Handle);
				if (res != 0)
				{
					Debug.WriteLine("RevokeDragDrop returned " + res);
				}
				//Else
				//    Debug.WriteLine("HandleDestroyed with allowdrop false")
			}
		}
		#endregion
		
		#region "   FindAncestorNode"
		///<Summary>Given a CShItem, find the TreeNode that belongs to the
		/// equivalent (matching PIDL) CShItem's most immediate surviving ancestor.
		///  Note: referential comparison might not work since there is no guarantee
		/// that the exact same CShItem is stored in the tree.</Summary>
		///<returns> Me.Root if not found, otherwise the Treenode whose .Tag is
		/// equivalent to the input CShItem's most immediate surviving ancestor </returns>
		private TreeNode FindAncestorNode(CShItem CSI)
		{
			TreeNode returnValue;
			returnValue = null;
			if (! CSI.IsFolder)
			{
				return returnValue; //only folders in tree
			}
			System.Windows.Forms.TreeNode baseNode = Root;
			//Dim cp As cPidl = CSI.clsPidl     'the cPidl rep of the PIDL to be found
			TreeNode testNode;
			int lim = ExpTreeLib.CShItem.PidlCount(CSI.PIDL) - ExpTreeLib.CShItem.PidlCount((baseNode.Tag as CShItem).PIDL);
			while (lim > 1)
			{
				foreach (TreeNode tempLoopVar_testNode in baseNode.Nodes)
				{
					testNode = tempLoopVar_testNode;
                    if (CShItem.IsAncestorOf(testNode.Tag as CShItem, CSI, false))
					{
						baseNode = testNode;
						baseNode.Expand();
						lim--;
						goto NEXTLEV;
					}
				}
				//CSI's Ancestor may have moved or been deleted, return the last one
				// found (if none, will return Me.Root)
				return baseNode;
NEXTLEV:
				1.GetHashCode() ; //nop
			}
			//on fall thru, we have it
			return baseNode;
		}
		#endregion
		
		#region "   Drag/Drop From Tree Processing"
		
		private void tv1_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			//Primary (internal) data type
			ArrayList toDrag = new ArrayList();
            CShItem csi = ((TreeNode)e.Item).Tag as CShItem;
			toDrag.Add(csi);
			//also need Shell IDList Array
			System.IO.MemoryStream MS;
			MS = CProcDataObject.MakeShellIDArray(toDrag);
			//Fairly universal data type (must be an array)
			string[] strD = new string[1];
			strD[0] = csi.Path;
			//Build data to drag
			DataObject dataObj = new DataObject();
			dataObj.SetData(toDrag);
			if (MS != null)
			{
				dataObj.SetData("Shell IDList Array", true, MS);
			}
			dataObj.SetData("FileDrop", true, strD);
			//Do drag, allowing Copy and Move
			DragDropEffects ddeff;
			ddeff = tv1.DoDragDrop(dataObj, DragDropEffects.Copy | DragDropEffects.Move);
			//the following line commented out, since we can't depend on ddeff
			//If ddeff = DragDropEffects.None Then Exit Sub 'nothing happened
			RefreshNode(FindAncestorNode(csi));
		}
		
		#endregion
		
		#region "   DragWrapper Event Handling"
		
		// dropNode is the TreeNode that most recently was DraggedOver or
		//    Dropped onto.
		private TreeNode dropNode;
		
		//expandNodeTimer is used to expand a node that is hovered over, with a delay
		private System.Windows.Forms.Timer expandNodeTimer = new System.Windows.Forms.Timer();
		
		#region "       expandNodeTimer_Tick"
		private void expandNodeTimer_Tick(object sender, EventArgs e)
		{
			expandNodeTimer.Stop();
			if (dropNode != null)
			{
				DragDropHandler.ShDragOver -= new ExpTreeLib.TVDragWrapper.ShDragOverEventHandler(DragWrapper_ShDragOver);
				try
				{
					tv1.BeginUpdate();
					dropNode.Expand();
					dropNode.EnsureVisible();
				}
				finally
				{
					tv1.EndUpdate();
				}
				DragDropHandler.ShDragOver += new ExpTreeLib.TVDragWrapper.ShDragOverEventHandler(DragWrapper_ShDragOver);
			}
		}
		#endregion
		
		///<Summary>ShDragEnter does nothing. It is here for debug tracking</Summary>
		private void DragWrapper_ShDragEnter(ArrayList Draglist, IntPtr pDataObj, int grfKeyState, int pdwEffect)
		{
			//Debug.WriteLine("Enter ExpTree ShDragEnter. PdwEffect = " & pdwEffect)
		}
		
		///<Summary>Drag has left the control. Cleanup what we have to</Summary>
		private void DragWrapper_ShDragLeave()
		{
			expandNodeTimer.Stop(); //shut off the dragging over nodes timer
			//Debug.WriteLine("Enter ExpTree ShDragLeave")
			if (dropNode != null)
			{
				ResetTreeviewNodeColor(dropNode);
			}
			dropNode = null;
		}
		
		///<Summary>ShDragOver manages the appearance of the TreeView.  Management of
		/// the underlying FolderItem is done in DragWrapper
		/// Credit to Cory Smith for TreeView colorizing technique and code,
		/// at http://addressof.com/blog/archive/2004/10/01/955.aspx
		/// Node expansion based on expandNodeTimer added by me.
		///</Summary>
		private void DragWrapper_ShDragOver(TreeNode Node, System.Drawing.Point pt, int grfKeyState, int pdwEffect)
		{
			//Debug.WriteLine("Enter ExpTree ShDragOver. PdwEffect = " & pdwEffect)
			//Debug.WriteLine(vbTab & "Over node: " & CType(Node, TreeNode).Text)
			
			if (Node == null) //clean up node stuff & fix color. Leave Draginfo alone-cleaned up on DragLeave
			{
				expandNodeTimer.Stop();
				if (dropNode != null)
				{
					ResetTreeviewNodeColor(dropNode);
					dropNode = null;
				}
			}
			else //Drag is Over a node - fix color & DragDropEffects
			{
				if (Node == dropNode)
				{
					return; //we've already done it all
				}
				
				expandNodeTimer.Stop(); //not over previous node anymore
				try
				{
					tv1.BeginUpdate();
					int delta = tv1.Height - pt.Y;
					if (delta < tv1.Height / 2 && delta > 0)
					{
						if (Node != null&& !(Node.NextVisibleNode == null))
						{
							Node.NextVisibleNode.EnsureVisible();
							// Thread.Sleep(250)  'slow down a bit
						}
					}
					if (delta > tv1.Height / 2 && delta < tv1.Height)
					{
						if (Node != null&& !(Node.PrevVisibleNode == null))
						{
							Node.PrevVisibleNode.EnsureVisible();
							// Thread.Sleep(250)   'slow down a bit
						}
					}
					if (! Node.BackColor.Equals(SystemColors.Highlight))
					{
						ResetTreeviewNodeColor(tv1.Nodes[0]);
						Node.BackColor = SystemColors.Highlight;
						Node.ForeColor = SystemColors.HighlightText;
					}
				}
				finally
				{
					tv1.EndUpdate();
				}
				dropNode = Node; //dropNode is the Saved Global version of Node
				if (! dropNode.IsExpanded)
				{
					expandNodeTimer.Interval = 1200;
					expandNodeTimer.Start();
				}
			}
		}
		
		private void DragWrapper_ShDragDrop(ArrayList DragList, object Node, int grfKeyState, int pdwEffect)
		{
			expandNodeTimer.Stop();
			//Debug.WriteLine("Enter ExpTree ShDragDrop. PdwEffect = " & pdwEffect)
			//Debug.WriteLine(vbTab & "Over node: " & CType(Node, TreeNode).Text)
			
			if (dropNode != null)
			{
				ResetTreeviewNodeColor(dropNode);
			}
			else
			{
				ResetTreeviewNodeColor(tv1.Nodes[0]);
			}
			// If Directories were Moved, we must find and update the DragSource TreeNodes
			//  of course, it is possible that the Drag was external to the App and
			//  the DragSource TreeNode might not exist in the Tree
			//All of this is somewhat chancy since we can't count on pdwEffect or
			//  on a Move having actually started, let alone finished
			CShItem CSI; //that is what is in DragList
			foreach (CShItem tempLoopVar_CSI in DragList)
			{
				CSI = tempLoopVar_CSI;
				if (CSI.IsFolder) //only care about Folders
				{
					RefreshNode(FindAncestorNode(CSI));
				}
			}
			if (tv1.SelectedNode == dropNode) //Fake a reselect
			{
				System.Windows.Forms.TreeViewEventArgs e = new System.Windows.Forms.TreeViewEventArgs(tv1.SelectedNode, TreeViewAction.Unknown);
				tv1_AfterSelect(tv1, e); //will do a RefreshNode and raise AfterNodeSelect Event
			}
			else
			{
				RefreshNode(dropNode); //Otherwise, just refresh the Target
				if (pdwEffect != (int)DragDropEffects.Copy && pdwEffect != (int)DragDropEffects.Link)
				{
					//it may have been a move. if so need to do an AfterSelect on the DragSource if it is SelectedNode
					if (DragList.Count > 0) //can't happen but check
					{
						if (tv1.SelectedNode != null) //ditto
						{
                            CShItem csiSel = tv1.SelectedNode.Tag as CShItem;
                            CShItem csiSource = DragList[0] as CShItem; //assume all from same dir
							if (CShItem.IsAncestorOf(csiSel, csiSource, false)) //also true for equality
							{
								System.Windows.Forms.TreeViewEventArgs e = new System.Windows.Forms.TreeViewEventArgs(tv1.SelectedNode, TreeViewAction.Unknown);
								tv1_AfterSelect(tv1, e); //will do a RefreshNode and raise AfterNodeSelect Event
							}
						}
					}
				}
			}
			dropNode = null;
			//Debug.WriteLine("Leaving ExpTree ShDragDrop")
		}
		
		private void ResetTreeviewNodeColor(TreeNode node)
		{
			if (! node.BackColor.Equals(Color.Empty))
			{
				node.BackColor = Color.Empty;
				node.ForeColor = Color.Empty;
			}
			if (node.FirstNode != null&& node.IsExpanded)
			{
				TreeNode child;
				foreach (TreeNode tempLoopVar_child in node.Nodes)
				{
					child = tempLoopVar_child;
					if (! child.BackColor.Equals(Color.Empty))
					{
						child.BackColor = Color.Empty;
						child.ForeColor = Color.Empty;
					}
					if (child.FirstNode != null&& child.IsExpanded)
					{
						ResetTreeviewNodeColor(child);
					}
				}
			}
		}
		#endregion
		
	}
	
}
