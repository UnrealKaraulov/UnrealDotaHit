using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System;
using System.IO;
//using System.IO.FileSystemInfo;
using System.Runtime.InteropServices;
using System.Text;
using DotaHIT.Core.Resources;
using System.Collections.Generic;
//using ExpTreeLib.ShellDll;



namespace ExpTreeLib
{
	public class CShItem : IDisposable, IComparable
	{
		#region "   Shared Private Fields"
		//This class has occasion to refer to the TypeName as reported by
		// SHGetFileInfo. It needs to compare this to the string
		// (in English) "System Folder"
		//on non-English systems, we do not know, in the general case,
		// what the equivalent string is to compare against
		//The following variable is set by Sub New() to the string that
		// corresponds to "System Folder" on the current machine
		// Sub New() depends on the existance of My Computer(CSIDL.DRIVES),
		// to determine what the equivalent string is
		private static string m_strSystemFolder;
		
		//My Computer is also commonly used (though not internally),
		// so save & expose its name on the current machine
		private static string m_strMyComputer;
		
		//To get My Documents sorted first, we need to know the Locale
		//specific name of that folder.
		private static string m_strMyDocuments;
		
		// The DesktopBase is set up via Sub New() (one time only) and
		//  disposed of only when DesktopBase is finally disposed of
		private static CShItem DesktopBase;
		
		//We can avoid an extra SHGetFileInfo call once this is set up
		private static int OpenFolderIconIndex = - 1;
		
		// It is also useful to know if the OS is XP or above.
		// Set up in Sub New() to avoid multiple calls to find this info
		private static bool XPorAbove;
		// Likewise if OS is Win2K or Above
		private static bool Win2KOrAbove;
		
		// DragDrop, possibly among others, needs to know the Path of
		// the DeskTopDirectory in addition to the Desktop itself
		// Also need the actual CShItem for the DeskTopDirectory, so get it
		private static CShItem m_DeskTopDirectory;
		
		
		#endregion
		
		#region "   Instance Private Fields"
		//m_Folder and m_Pidl must be released/freed at Dispose time
		private ExpTreeLib.ShellDll.IShellFolder m_Folder; //if item is a folder, contains the Folder interface for this instance
		private IntPtr m_Pidl; //The Absolute PIDL for this item (not retained for files)
		private string m_DisplayName = "";
		private string m_Path;
		private string m_TypeName;
		private CShItem m_Parent; //= Nothing
		private int m_IconIndexNormal; //index into the System Image list for Normal icon
		private int m_IconIndexOpen; //index into the SystemImage list for Open icon
		private bool m_IsBrowsable;
		private bool m_IsFileSystem;
		private bool m_IsFolder;
		private bool m_HasSubFolders;
		private bool m_IsLink;
		private bool m_IsDisk;
		private bool m_IsShared;
		private bool m_IsHidden;
		private bool m_IsNetWorkDrive; //= False
		private bool m_IsRemovable; //= False
		private bool m_IsReadOnly; //= False
		//Properties of interest to Drag Operations
		private bool m_CanMove; //= False
		private bool m_CanCopy; //= False
		private bool m_CanDelete; //= False
		private bool m_CanLink; //= False
		private bool m_IsDropTarget; //= False
		private ExpTreeLib.ShellDll.SFGAO m_Attributes; //the original, returned from GetAttributesOf
		
		private int m_SortFlag; //= 0 'Used in comparisons
		
		private List<CShItem> m_Directories;
		
		//The following elements are only filled in on demand
		private bool m_XtrInfo; //= False
		private DateTime m_LastWriteTime;
		private DateTime m_CreationTime;
		private DateTime m_LastAccessTime;
		private long m_Length;
		
		//Indicates whether DisplayName, TypeName, SortFlag have been set up
		private bool m_HasDispType; //= False
		
		//Indicates whether IsReadOnly has been set up
		private bool m_IsReadOnlySetup; //= False
		
		//Holds a byte() representation of m_PIDL -- filled when needed
		private cPidl m_cPidl;
		
		//Flags for Dispose state
        //Danat: private bool m_IsDisposing;
		private bool m_Disposed;
		
		#endregion
		
		#region "   Destructor"
		/// <summary>
		/// Summary of Dispose.
		/// </summary>
		///
		public void Dispose()
		{
			Dispose(true);
			// Take yourself off of the finalization queue
			// to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Deallocates CoTaskMem contianing m_Pidl and removes reference to m_Folder
		/// </summary>
		/// <param name="disposing"></param>
		///
		protected virtual void Dispose(bool disposing)
		{
			// Allow your Dispose method to be called multiple times,
			// but throw an exception if the object has been disposed.
			// Whenever you do something with this class,
			// check to see if it has been disposed.
			if (!(m_Disposed))
			{
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				m_Disposed = true;
				if (disposing)
				{
				}
				// Release unmanaged resources. If disposing is false,
				// only the following code is executed.
				if (m_Folder != null)
				{
					Marshal.ReleaseComObject(m_Folder);
				}
				if (! m_Pidl.Equals(IntPtr.Zero))
				{
					Marshal.FreeCoTaskMem(m_Pidl);
				}
			}
			else
			{
				throw (new Exception("CShItem Disposed more than once"));
			}
		}
		
		// This Finalize method will run only if the
		// Dispose method does not get called.
		// By default, methods are NotOverridable.
		// This prevents a derived class from overriding this method.
		/// <summary>
		/// Summary of Finalize.
		/// </summary>
		///
		~CShItem()
		{
			// Do not re-create Dispose clean-up code here.
			// Calling Dispose(false) is optimal in terms of
			// readability and maintainability.
			Dispose(false);
		}
		
		#endregion
		
		#region "   Constructors"
		
		#region "       Private Sub New(ByVal folder As IShellFolder, ByVal pidl As IntPtr, ByVal parent As CShItem)"
		/// <summary>
		/// Private Constructor, creates new CShItem from the item's parent folder and
		///  the item's PIDL relative to that folder.</summary>
		/// <param name="folder">the folder interface of the parent</param>
		/// <param name="pidl">the Relative PIDL of this item</param>
		/// <param name="parent">the CShitem of the parent</param>
		///
		private CShItem(ExpTreeLib.ShellDll.IShellFolder folder, IntPtr pidl, CShItem parent)
		{
			if (DesktopBase == null)
			{
				DesktopBase = new CShItem(); //This initializes the Desktop folder
			}
			m_Parent = parent;
			m_Pidl = concatPidls(parent.PIDL, pidl);
			
			//Get some attributes
			SetUpAttributes(folder, pidl);
			
			//Set unfetched value for IconIndex....
			m_IconIndexNormal = - 1;
			m_IconIndexOpen = - 1;
			//finally, set up my Folder
			if (m_IsFolder)
			{
				int HR;
				HR = folder.BindToObject(pidl, IntPtr.Zero, ref ExpTreeLib.ShellDll.IID_IShellFolder, ref m_Folder);
				if (HR != ExpTreeLib.ShellDll.NOERROR)
				{
					Marshal.ThrowExceptionForHR(HR);
				}
			}
		}
		#endregion
		
		#region "       Sub New()"
		/// <summary>
		/// Private Constructor. Creates CShItem of the Desktop
		/// </summary>
		///
		private CShItem() //only used when desktopfolder has not been intialized
		{
			if (DesktopBase != null)
			{
				throw (new Exception("Attempt to initialize CShItem for second time"));
			}
			
			int HR;
			//firstly determine what the local machine calls a "System Folder" and "My Computer"
			IntPtr tmpPidl = IntPtr.Zero;
			HR = ExpTreeLib.ShellDll.SHGetSpecialFolderLocation(0, (int)ExpTreeLib.ShellDll.CSIDL.DRIVES, ref tmpPidl);
			ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
			int dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.DISPLAYNAME | ExpTreeLib.ShellDll.SHGFI.TYPENAME | ExpTreeLib.ShellDll.SHGFI.PIDL);
			int dwAttr = 0;
			ExpTreeLib.ShellDll.SHGetFileInfo(tmpPidl, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);
			m_strSystemFolder = shfi.szTypeName;
			m_strMyComputer = shfi.szDisplayName;
			Marshal.FreeCoTaskMem(tmpPidl);
			//set OS version info
			XPorAbove = ShellDll.IsXpOrAbove();
			Win2KOrAbove = ShellDll.Is2KOrAbove();
			
			//With That done, now set up Desktop CShItem
			m_Path = "::{" + ExpTreeLib.ShellDll.DesktopGUID.ToString() + "}";
			m_IsFolder = true;
			m_HasSubFolders = true;
			m_IsBrowsable = false;
			HR = ExpTreeLib.ShellDll.SHGetDesktopFolder(ref m_Folder);
			m_Pidl = ShellDll.GetSpecialFolderLocation(IntPtr.Zero, (int)ExpTreeLib.ShellDll.CSIDL.DESKTOP);
			dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.DISPLAYNAME | ExpTreeLib.ShellDll.SHGFI.TYPENAME | ExpTreeLib.ShellDll.SHGFI.SYSICONINDEX | ExpTreeLib.ShellDll.SHGFI.PIDL);
			dwAttr = 0;
			IntPtr H = ExpTreeLib.ShellDll.SHGetFileInfo(m_Pidl, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);
			m_DisplayName = shfi.szDisplayName;
			m_TypeName = strSystemFolder; //not returned correctly by SHGetFileInfo
			m_IconIndexNormal = shfi.iIcon;
			m_IconIndexOpen = shfi.iIcon;
			m_HasDispType = true;
			m_IsDropTarget = true;
			m_IsReadOnly = false;
			m_IsReadOnlySetup = true;
			
			//also get local name for "My Documents"
			int pchEaten = 0;
			tmpPidl = IntPtr.Zero;
            int pdwAttributes = 0;
            HR = m_Folder.ParseDisplayName(0, IntPtr.Zero, "::{450d8fba-ad25-11d0-98a8-0800361b1103}", ref pchEaten, ref tmpPidl, ref pdwAttributes);
			shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
			dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.DISPLAYNAME | ExpTreeLib.ShellDll.SHGFI.TYPENAME | ExpTreeLib.ShellDll.SHGFI.PIDL);
			dwAttr = 0;
			ExpTreeLib.ShellDll.SHGetFileInfo(tmpPidl, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);
			m_strMyDocuments = shfi.szDisplayName;
			Marshal.FreeCoTaskMem(tmpPidl);
			//this must be done after getting "My Documents" string
			m_SortFlag = ComputeSortFlag();
			//Set DesktopBase
			DesktopBase = this;
			// Lastly, get the Path and CShItem of the DesktopDirectory -- useful for DragDrop
			m_DeskTopDirectory = new CShItem(ExpTreeLib.ShellDll.CSIDL.DESKTOPDIRECTORY);
		}
		#endregion
		
		#region "       New(ByVal ID As CSIDL)"
		/// <summary>Create instance based on a non-desktop CSIDL.
		/// Will create based on any CSIDL Except the DeskTop CSIDL</summary>
		/// <param name="ID">Value from CSIDL enumeration denoting the folder to create this CShItem of.</param>
		///
		public CShItem(ExpTreeLib.ShellDll.CSIDL ID)
		{
			if (DesktopBase == null)
			{
				DesktopBase = new CShItem(); //This initializes the Desktop folder
			}
			int HR;
			if (ID == ExpTreeLib.ShellDll.CSIDL.MYDOCUMENTS)
			{
				int pchEaten = 0;
                int pdwAttributes = 0;
                HR = DesktopBase.m_Folder.ParseDisplayName(0, IntPtr.Zero, "::{450d8fba-ad25-11d0-98a8-0800361b1103}", ref pchEaten, ref m_Pidl, ref pdwAttributes);
			}
			else
			{
				HR = ExpTreeLib.ShellDll.SHGetSpecialFolderLocation(0, (int)ID, ref m_Pidl);
			}
			if (HR == ExpTreeLib.ShellDll.NOERROR)
			{
				ShellDll.IShellFolder pParent;
				IntPtr relPidl = IntPtr.Zero;
				
				pParent = GetParentOf(m_Pidl, ref relPidl);
				//Get the Attributes
				SetUpAttributes(pParent, relPidl);
				//Set unfetched value for IconIndex....
				m_IconIndexNormal = - 1;
				m_IconIndexOpen = - 1;
				//finally, set up my Folder
				if (m_IsFolder)
				{
					HR = pParent.BindToObject(relPidl, IntPtr.Zero, ref ExpTreeLib.ShellDll.IID_IShellFolder, ref m_Folder);
					if (HR != ExpTreeLib.ShellDll.NOERROR)
					{
						Marshal.ThrowExceptionForHR(HR);
					}
				}
				Marshal.ReleaseComObject(pParent);
				//if PidlCount(m_Pidl) = 1 then relPidl is same as m_Pidl, don't release
				if (PidlCount(m_Pidl) > 1)
				{
					Marshal.FreeCoTaskMem(relPidl);
				}
			}
			else
			{
				Marshal.ThrowExceptionForHR(HR);
			}
		}
		#endregion
		
		#region "       New(ByVal path As String)"
		/// <summary>Create a new CShItem based on a Path Must be a valid FileSystem Path</summary>
		/// <param name="path"></param>
		///
		public CShItem(string path)
		{            
			if (DesktopBase == null)
			{
				DesktopBase = new CShItem(); //This initializes the Desktop folder
			}
			//Removal of following code allows Path(GUID) of Special FOlders to serve
			//  as a valid Path for CShItem creation (part of Calum's refresh code needs this
			//If Not Directory.Exists(path) AndAlso Not File.Exists(path) Then
			//    Throw New Exception("CShItem -- Invalid Path specified")
			//End If
			int HR;
            int pchEaten = 0;
            int pdwAttributes = 0;            
            HR = DesktopBase.m_Folder.ParseDisplayName(0, IntPtr.Zero, path, ref pchEaten, ref m_Pidl, ref pdwAttributes);            
			if (HR != ExpTreeLib.ShellDll.NOERROR)
			{
				Marshal.ThrowExceptionForHR(HR);
			}
			ShellDll.IShellFolder pParent;
			IntPtr relPidl = IntPtr.Zero;
			
			pParent = GetParentOf(m_Pidl, ref relPidl);
			
			//Get the Attributes
			SetUpAttributes(pParent, relPidl);
			//Set unfetched value for IconIndex....
			m_IconIndexNormal = - 1;
			m_IconIndexOpen = - 1;
			//finally, set up my Folder
			if (m_IsFolder)
			{
				HR = pParent.BindToObject(relPidl, IntPtr.Zero, ref ExpTreeLib.ShellDll.IID_IShellFolder, ref m_Folder);
				if (HR != ExpTreeLib.ShellDll.NOERROR)
				{
					Marshal.ThrowExceptionForHR(HR);
				}
			}
			Marshal.ReleaseComObject(pParent);
			//if PidlCount(m_Pidl) = 1 then relPidl is same as m_Pidl, don't release
			if (PidlCount(m_Pidl) > 1)
			{
				Marshal.FreeCoTaskMem(relPidl);
			}            
		}
		#endregion
		
		#region "       New(ByVal FoldBytes() as Byte, ByVal ItemBytes() as Byte)"
		///<Summary>Given a Byte() containing the Pidl of the parent
		/// folder and another Byte() containing the Pidl of the Item,
		/// relative to the Folder, Create a CShItem for the Item.
		/// This is of primary use in dealing with "Shell IDList Array"
		/// formatted info passed in a Drag Operation
		/// </Summary>
		public CShItem(byte[] FoldBytes, byte[] ItemBytes)
		{
			Debug.WriteLine("CShItem.New(FoldBytes,ItemBytes) Fold len= " + FoldBytes.Length + " Item Len = " + ItemBytes.Length);
			if (DesktopBase == null)
			{
				DesktopBase = new CShItem(); //This initializes the Desktop folder
			}
			ShellDll.IShellFolder pParent = MakeFolderFromBytes(FoldBytes);

            IntPtr ipParent = IntPtr.Zero;
            IntPtr ipItem = IntPtr.Zero;

			if (pParent == null)
			{
				goto XIT; //m_PIDL will = IntPtr.Zero for really bad CShitem
			}
			ipParent = cPidl.BytesToPidl(FoldBytes);
			ipItem = cPidl.BytesToPidl(ItemBytes);
			if (ipParent.Equals(IntPtr.Zero) || ipItem.Equals(IntPtr.Zero))
			{
				goto XIT;
			}
			// Now process just like sub new(folder,pidl,parent) version
			m_Pidl = concatPidls(ipParent, ipItem);
			
			//Get some attributes
			SetUpAttributes(pParent, ipItem);
			
			//Set unfetched value for IconIndex....
			m_IconIndexNormal = - 1;
			m_IconIndexOpen = - 1;
			//finally, set up my Folder
			if (m_IsFolder)
			{
				int HR;
				HR = pParent.BindToObject(ipItem, IntPtr.Zero, ref ExpTreeLib.ShellDll.IID_IShellFolder, ref m_Folder);
				#if DEBUG
				if (HR != ExpTreeLib.ShellDll.NOERROR)
				{
					Marshal.ThrowExceptionForHR(HR);
				}
				#endif
			}
XIT: //On any kind of exit, free the allocated memory
			#if DEBUG
			if (m_Pidl.Equals(IntPtr.Zero))
			{
				Debug.WriteLine("CShItem.New(FoldBytes,ItemBytes) Failed");
			}
			else
			{
				Debug.WriteLine("CShItem.New(FoldBytes,ItemBytes) Created " + this.Path);
			}
			#endif
            
			if (! ipParent.Equals(IntPtr.Zero))
			{
				Marshal.FreeCoTaskMem(ipParent);
			}
			if (! ipItem.Equals(IntPtr.Zero))
			{
				Marshal.FreeCoTaskMem(ipItem);
			}
		}
		
		#endregion
		
		#region "       Utility functions used in Constructors"
		
		#region "       IsValidPidl"
		///<Summary>It is impossible to validate a PIDL completely since its contents
		/// are arbitrarily defined by the creating Shell Namespace.  However, it
		/// is possible to validate the structure of a PIDL.</Summary>
		public static bool IsValidPidl(byte[] b)
		{
			bool returnValue;
			returnValue = false; //assume failure
			int bMax = b.Length - 1; //max value that index can have
			if (bMax < 1)
			{
				return returnValue; //min size is 2 bytes
			}
			int cb = b[0] + (b[1] * 256);
			int indx = 0;
			while (cb > 0)
			{
				if ((indx + cb + 1) > bMax)
				{
					return returnValue; //an error
				}
				indx += cb;
				cb = b[indx] + (b[indx + 1] * 256);
			}
			// on fall thru, it is ok as far as we can check
			returnValue = true;
			return returnValue;
		}
		#endregion
		
		#region "   MakeFolderFromBytes"
		public static ShellDll.IShellFolder MakeFolderFromBytes(byte[] b)
		{
			ShellDll.IShellFolder returnValue;
			returnValue = null; //get rid of VS2005 warning
			if (! IsValidPidl(b))
			{
				return null;
			}
			if (b.Length == 2 && ((b[0] == 0) && (b[1] == 0))) //this is the desktop
			{
				return DesktopBase.Folder;
			}
			else if (b.Length == 0) //Also indicates the desktop
			{
				return DesktopBase.Folder;
			}
			else
			{
				IntPtr ptr = Marshal.AllocCoTaskMem(b.Length);
				if (ptr.Equals(IntPtr.Zero))
				{
					return null;
				}
				Marshal.Copy(b, 0, ptr, b.Length);
				//the next statement assigns a IshellFolder object to the function return, or has an error
				int hr = DesktopBase.Folder.BindToObject(ptr, IntPtr.Zero, ref ExpTreeLib.ShellDll.IID_IShellFolder, ref returnValue);
				if (hr != 0)
				{
					returnValue = null;
				}
				Marshal.FreeCoTaskMem(ptr);
			}
			return returnValue;
		}
		#endregion
		
		#region "           GetParentOf"
		
		///<Summary>Returns both the IShellFolder interface of the parent folder
		///  and the relative pidl of the input PIDL</Summary>
		///<remarks>Several internal functions need this information and do not have
		/// it readily available. GetParentOf serves those functions</remarks>
		private static ExpTreeLib.ShellDll.IShellFolder GetParentOf(IntPtr pidl, ref IntPtr relPidl)
		{
			ShellDll.IShellFolder returnValue;
			returnValue = null; //avoid VB2005 warning
			int HR;
			int itemCnt = PidlCount(pidl);
			if (itemCnt == 1) //parent is desktop
			{
				HR = ExpTreeLib.ShellDll.SHGetDesktopFolder(ref returnValue);
				relPidl = pidl;
			}
			else
			{
				IntPtr tmpPidl;
				tmpPidl = TrimPidl(pidl, ref relPidl);
				HR = DesktopBase.m_Folder.BindToObject(tmpPidl, IntPtr.Zero, ref ExpTreeLib.ShellDll.IID_IShellFolder, ref returnValue);
				Marshal.FreeCoTaskMem(tmpPidl);
			}
			if (HR != ExpTreeLib.ShellDll.NOERROR)
			{
				Marshal.ThrowExceptionForHR(HR);
			}
			return returnValue;
		}
		#endregion
		
		#region "           SetUpAttributes"
		/// <summary>Get the base attributes of the folder/file that this CShItem represents</summary>
		/// <param name="folder">Parent Folder of this Item</param>
		/// <param name="pidl">Relative Pidl of this Item.</param>
		///        
		private void SetUpAttributes(ExpTreeLib.ShellDll.IShellFolder folder, IntPtr pidl)
		{
			ExpTreeLib.ShellDll.SFGAO attrFlag;
			attrFlag = ExpTreeLib.ShellDll.SFGAO.BROWSABLE;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.FILESYSTEM;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.HASSUBFOLDER;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.FOLDER;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.LINK;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.SHARE;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.HIDDEN;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.REMOVABLE;
			//attrFlag = attrFlag Or SFGAO.RDONLY   'made into an on-demand attribute
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.CANCOPY;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.CANDELETE;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.CANLINK;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.CANMOVE;
			attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.DROPTARGET;
			//Note: for GetAttributesOf, we must provide an array, in  all cases with 1 element
			IntPtr[] aPidl = new IntPtr[1];
			aPidl[0] = pidl;
			folder.GetAttributesOf(1, aPidl, ref attrFlag);
			m_Attributes = attrFlag;
			m_IsBrowsable = (attrFlag & ExpTreeLib.ShellDll.SFGAO.BROWSABLE)!=0;
			m_IsFileSystem = (attrFlag & ExpTreeLib.ShellDll.SFGAO.FILESYSTEM)!=0;
			m_HasSubFolders = (attrFlag & ExpTreeLib.ShellDll.SFGAO.HASSUBFOLDER)!=0;
			m_IsFolder = (attrFlag & ExpTreeLib.ShellDll.SFGAO.FOLDER)!=0;
			m_IsLink = (attrFlag & ExpTreeLib.ShellDll.SFGAO.LINK)!=0;
			m_IsShared = (attrFlag & ExpTreeLib.ShellDll.SFGAO.SHARE)!=0;
			m_IsHidden = (attrFlag & ExpTreeLib.ShellDll.SFGAO.HIDDEN)!=0;
			m_IsRemovable = (attrFlag & ExpTreeLib.ShellDll.SFGAO.REMOVABLE)!=0;
			//m_IsReadOnly = CBool(attrFlag And SFGAO.RDONLY)      'made into an on-demand attribute
			m_CanCopy = (attrFlag & ExpTreeLib.ShellDll.SFGAO.CANCOPY)!=0;
			m_CanDelete = (attrFlag & ExpTreeLib.ShellDll.SFGAO.CANDELETE)!=0;
			m_CanLink = (attrFlag & ExpTreeLib.ShellDll.SFGAO.CANLINK)!=0;
			m_CanMove = (attrFlag & ExpTreeLib.ShellDll.SFGAO.CANMOVE)!=0;
			m_IsDropTarget = (attrFlag & ExpTreeLib.ShellDll.SFGAO.DROPTARGET)!=0;
			
			//Get the Path                          
			IntPtr strr = Marshal.AllocCoTaskMem(ExpTreeLib.ShellDll.MAX_PATH * 2 + 4);
			Marshal.WriteInt32(strr, 0, 0);
			StringBuilder buf = new StringBuilder(ExpTreeLib.ShellDll.MAX_PATH);
			ExpTreeLib.ShellDll.SHGDN itemflags = ExpTreeLib.ShellDll.SHGDN.FORPARSING;
			folder.GetDisplayNameOf(pidl, itemflags, strr);            
            int HR = ExpTreeLib.ShellDll.StrRetToBuf(strr, pidl, buf, ExpTreeLib.ShellDll.MAX_PATH);            
			Marshal.FreeCoTaskMem(strr); //now done with it           
			if (HR == ExpTreeLib.ShellDll.NOERROR)
			{
				m_Path = buf.ToString();
				//check for zip file = folder on xp, leave it a file
				if (m_IsFolder && m_IsFileSystem && XPorAbove)
				{
					//Note:meaning of SFGAO.STREAM changed between win2k and winXP
					//Version 20 code
					//If File.Exists(m_Path) Then
					//    m_IsFolder = False
					//End If
					//Version 21 code
					aPidl[0] = pidl;
					attrFlag = ExpTreeLib.ShellDll.SFGAO.STREAM;
					folder.GetAttributesOf(1, aPidl, ref attrFlag);
					if ((attrFlag & ExpTreeLib.ShellDll.SFGAO.STREAM)!=0)
					{
						m_IsFolder = false;
					}
				}
				if (m_Path.Length == 3 && m_Path.Substring(1).Equals(":\\"))
				{
					m_IsDisk = true;
				}
			}
			else
			{
				Marshal.ThrowExceptionForHR(HR);
			}
		}
		
		#endregion
		
		#endregion
		
		#region "       Public Shared Function GetCShItem(ByVal path As String) As CShItem"
		public static CShItem GetCShItem(string path)
		{
			CShItem returnValue;
			returnValue = null; //assume failure
			int HR;
			IntPtr tmpPidl = IntPtr.Zero;
            int pchEaten = 0;
            int pdwAttributes = 0;
            HR = GetDeskTop().Folder.ParseDisplayName(0, IntPtr.Zero, path, ref pchEaten, ref tmpPidl, ref pdwAttributes);
			if (HR == 0)
			{
				returnValue = FindCShItem(tmpPidl);
				if (returnValue == null)
				{
					try
					{
						returnValue = new CShItem(path);
					}
					catch
					{
						returnValue = null;
					}
				}
			}
			if (! tmpPidl.Equals(IntPtr.Zero))
			{
				Marshal.FreeCoTaskMem(tmpPidl);
			}
			return returnValue;
		}
		#endregion
		
		#region "       Public Shared Function GetCShItem(ByVal ID As CSIDL) As CShItem"
		public static CShItem GetCShItem(ExpTreeLib.ShellDll.CSIDL ID)
		{
			CShItem returnValue;
			returnValue = null; //avoid VB2005 Warning
			if (ID == ExpTreeLib.ShellDll.CSIDL.DESKTOP)
			{
				return GetDeskTop();
			}
			int HR;
			IntPtr tmpPidl = IntPtr.Zero;
			if (ID == ExpTreeLib.ShellDll.CSIDL.MYDOCUMENTS)
			{
				int pchEaten = 0;
                int pdwAttributes = 0;
                HR = GetDeskTop().Folder.ParseDisplayName(0, IntPtr.Zero, "::{450d8fba-ad25-11d0-98a8-0800361b1103}", ref pchEaten, ref tmpPidl, ref pdwAttributes);
			}
			else
			{
				HR = ExpTreeLib.ShellDll.SHGetSpecialFolderLocation(0, (int)ID, ref tmpPidl);
			}
			if (HR == ExpTreeLib.ShellDll.NOERROR)
			{
				returnValue = FindCShItem(tmpPidl);
				if (returnValue == null)
				{
					try
					{
						returnValue = new CShItem(ID);
					}
					catch
					{
						returnValue = null;
					}
				}
			}
			if (! tmpPidl.Equals(IntPtr.Zero))
			{
				Marshal.FreeCoTaskMem(tmpPidl);
			}
			return returnValue;
		}
		#endregion
		
		#region "       Public Shared Function GetCShItem(ByVal FoldBytes() As Byte, ByVal ItemBytes() As Byte) As CShItem"
		public static CShItem GetCShItem(byte[] FoldBytes, byte[] ItemBytes)
		{
			CShItem returnValue;
			returnValue = null; //assume failure
			byte[] b = cPidl.JoinPidlBytes(FoldBytes, ItemBytes);
			if (b == null)
			{
				return returnValue; //can do no more with invalid pidls
			}
			//otherwise do like below, skipping unnecessary validation check
			IntPtr thisPidl = Marshal.AllocCoTaskMem(b.Length);
			if (thisPidl.Equals(IntPtr.Zero))
			{
				return null;
			}
			Marshal.Copy(b, 0, thisPidl, b.Length);
			returnValue = FindCShItem(thisPidl);
			Marshal.FreeCoTaskMem(thisPidl);
			if (returnValue == null) //didn't find it, make new
			{
				try
				{
					returnValue = new CShItem(FoldBytes, ItemBytes);
				}
				catch
				{
					
				}
			}
			if (returnValue.PIDL.Equals(IntPtr.Zero))
			{
				returnValue = null;
			}
			return returnValue;
		}
		#endregion
		
		#region "       Public Shared Function FindCShItem(ByVal b() As Byte) As CShItem"
		public static CShItem FindCShItem(byte[] b)
		{
			CShItem returnValue;
			if (! IsValidPidl(b))
			{
				return null;
			}
			IntPtr thisPidl = Marshal.AllocCoTaskMem(b.Length);
			if (thisPidl.Equals(IntPtr.Zero))
			{
				return null;
			}
			Marshal.Copy(b, 0, thisPidl, b.Length);
			returnValue = FindCShItem(thisPidl);
			Marshal.FreeCoTaskMem(thisPidl);
			return returnValue;
		}
		#endregion
		
		#region "       Public Shared Function FindCShItem(ByVal ptr As IntPtr) As CShItem"
		public static CShItem FindCShItem(IntPtr ptr)
		{
			CShItem returnValue;
			returnValue = null; //avoid VB2005 Warning
			CShItem BaseItem = CShItem.GetDeskTop();
			CShItem CSI;
			bool FoundIt = false; //True if we found item or an ancestor
			while (!FoundIt)
			{
				foreach (CShItem tempLoopVar_CSI in BaseItem.GetDirectories(true))
				{
					CSI = tempLoopVar_CSI;
					if (IsAncestorOf(CSI.PIDL, ptr, false))
					{
						if (CShItem.IsEqual(CSI.PIDL, ptr)) //we found the desired item
						{
							return CSI;
						}
						else
						{
							BaseItem = CSI;
							FoundIt = true;
							goto endOfForLoop;
						}
					}
				}
endOfForLoop:
				if (! FoundIt)
				{
					return null; //didn't find an ancestor
				}
				//The complication is that the desired item may not be a directory
				if (! IsAncestorOf(BaseItem.PIDL, ptr, true)) //Don't have immediate ancestor
				{
					FoundIt = false; //go around again
				}
				else
				{
					foreach (CShItem tempLoopVar_CSI in BaseItem.GetItems())
					{
						CSI = tempLoopVar_CSI;
						if (CShItem.IsEqual(CSI.PIDL, ptr))
						{
							return CSI;
						}
					}
					//fall thru here means it doesn't exist or we can't find it because of funny PIDL from SHParseDisplayName
					return null;
				}
			}
			return returnValue;
		}
		#endregion
		
		#endregion
		
		#region "   Icomparable -- for default Sorting"
		
		/// <summary>Computes the Sort key of this CShItem, based on its attributes</summary>
		///
		private int ComputeSortFlag()
		{
			int rVal = 0;
			if (m_IsDisk)
			{
				rVal = 0x100000;
			}
			if (m_TypeName.Equals(strSystemFolder))
			{
				if (! m_IsBrowsable)
				{
					rVal = rVal | 0x10000;
					if (m_strMyDocuments.Equals(m_DisplayName))
					{
						rVal = rVal | 0x1;
					}
				}
				else
				{
					rVal = rVal | 0x1000;
				}
			}
			if (m_IsFolder)
			{
				rVal = rVal | 0x100;
			}
			return rVal;
		}
		
		///<Summary> CompareTo(obj as object)
		///  Compares obj to this instance based on SortFlag-- obj must be a CShItem</Summary>
		///<SortOrder>  (low)Disks,non-browsable System Folders,
		///              browsable System Folders,
		///               Directories, Files, Nothing (high)</SortOrder>
		public virtual int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1; //non-existant is always low
			}
			CShItem Other = obj as CShItem;
			if (! m_HasDispType)
			{
				SetDispType();
			}
			int cmp = Other.SortFlag - m_SortFlag; //Note the reversal
			if (cmp != 0)
			{
				return cmp;
			}
			else
			{
				if (m_IsDisk) //implies that both are
				{
					return string.Compare(m_Path, Other.Path);
				}
				else
				{
					return string.Compare(m_DisplayName, Other.DisplayName);
				}
			}
		}
		#endregion
		
		#region "   Properties"
		
		#region "       Shared Properties"
		public static string strMyComputer
		{
			get
			{
				return m_strMyComputer;
			}
		}
		
		public static string strSystemFolder
		{
			get
			{
				return m_strSystemFolder;
			}
		}
		
		public static string DesktopDirectoryPath
		{
			get
			{
				return m_DeskTopDirectory.Path;
			}
		}
		
		#endregion
		
		#region "       Normal Properties"
		public IntPtr PIDL
		{
			get
			{
				return m_Pidl;
			}
		}
		
		public ExpTreeLib.ShellDll.IShellFolder Folder
		{
			get
			{
				return m_Folder;
			}
		}
		
		public string Path
		{
			get
			{
				return m_Path;
			}
		}
		public CShItem Parent
		{
			get
			{
				return m_Parent;
			}
		}
		
		public ExpTreeLib.ShellDll.SFGAO Attributes
		{
			get
			{
				return m_Attributes;
			}
		}
		public bool IsBrowsable
		{
			get
			{
				return m_IsBrowsable;
			}
		}
		public bool IsFileSystem
		{
			get
			{
				return m_IsFileSystem;
			}
		}
		public bool IsFolder
		{
			get
			{
				return m_IsFolder;
			}
		}
		public bool HasSubFolders
		{
			get
			{
				return m_HasSubFolders;
			}
		}
		public bool IsDisk
		{
			get
			{
				return m_IsDisk;
			}
		}
		public bool IsLink
		{
			get
			{
				return m_IsLink;
			}
		}
		public bool IsShared
		{
			get
			{
				return m_IsShared;
			}
		}
		public bool IsHidden
		{
			get
			{
				return m_IsHidden;
			}
		}
		public bool IsRemovable
		{
			get
			{
				return m_IsRemovable;
			}
		}
		
		#region "       Drag Ops Properties"
		public bool CanMove
		{
			get
			{
				return m_CanMove;
			}
		}
		public bool CanCopy
		{
			get
			{
				return m_CanCopy;
			}
		}
		public bool CanDelete
		{
			get
			{
				return m_CanDelete;
			}
		}
		public bool CanLink
		{
			get
			{
				return m_CanLink;
			}
		}
		public bool IsDropTarget
		{
			get
			{
				return m_IsDropTarget;
			}
		}
		#endregion
		
		#endregion
		
		#region "       Filled on Demand Properties"
		
		#region "           Filled based on m_HasDispType"
		/// <summary>
		/// Set DisplayName, TypeName, and SortFlag when actually needed
		/// </summary>
		///
		private void SetDispType()
		{
			//Get Displayname, TypeName
			ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
			int dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.DISPLAYNAME | ExpTreeLib.ShellDll.SHGFI.TYPENAME | ExpTreeLib.ShellDll.SHGFI.PIDL);
			int dwAttr = 0;
			if (m_IsFileSystem && ! m_IsFolder)
			{
				dwflag = dwflag | (int)ExpTreeLib.ShellDll.SHGFI.USEFILEATTRIBUTES;
				dwAttr = ExpTreeLib.ShellDll.FILE_ATTRIBUTE_NORMAL;
			}
			IntPtr H = ExpTreeLib.ShellDll.SHGetFileInfo(m_Pidl, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);
			m_DisplayName = shfi.szDisplayName;
			m_TypeName = shfi.szTypeName;
			//fix DisplayName
			if (m_DisplayName.Equals(""))
			{
				m_DisplayName = m_Path;
			}
			//Fix TypeName
			//If m_IsFolder And m_TypeName.Equals("File") Then
			//    m_TypeName = "File Folder"
			//End If
			m_SortFlag = ComputeSortFlag();
			m_HasDispType = true;
		}
		
		public string DisplayName
		{
			get
			{
				if (! m_HasDispType)
				{
					SetDispType();
				}
				return m_DisplayName;
			}
		}
		
		private int SortFlag
		{
			get
			{
				if (! m_HasDispType)
				{
					SetDispType();
				}
				return m_SortFlag;
			}
		}
		
		public string TypeName
		{
			get
			{
				if (! m_HasDispType)
				{
					SetDispType();
				}
				return m_TypeName;
			}
		}
		#endregion
		
		#region "           IconIndex properties"
		public int IconIndexNormal
		{
			get
			{
				if (m_IconIndexNormal < 0)
				{
					if (! m_HasDispType)
					{
						SetDispType();
					}
					ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
					int dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.PIDL | ExpTreeLib.ShellDll.SHGFI.SYSICONINDEX);
					int dwAttr = 0;
					if (m_IsFileSystem && ! m_IsFolder)
					{
						dwflag = dwflag | (int)ExpTreeLib.ShellDll.SHGFI.USEFILEATTRIBUTES;
						dwAttr = ExpTreeLib.ShellDll.FILE_ATTRIBUTE_NORMAL;
					}                    
					IntPtr H = ExpTreeLib.ShellDll.SHGetFileInfo(m_Pidl, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);                    
					m_IconIndexNormal = shfi.iIcon;                    
				}
				return m_IconIndexNormal;
			}
		}
		// IconIndexOpen is Filled on demand
		public int IconIndexOpen
		{
			get
			{
				if (m_IconIndexOpen < 0)
				{
					if (! m_HasDispType)
					{
						SetDispType();
					}
					if (! m_IsDisk && m_IsFileSystem && m_IsFolder)
					{
						if (OpenFolderIconIndex < 0)
						{
							int dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.SYSICONINDEX | ExpTreeLib.ShellDll.SHGFI.PIDL);
							ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
							IntPtr H = ExpTreeLib.ShellDll.SHGetFileInfo(m_Pidl, 0, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag | (int)ExpTreeLib.ShellDll.SHGFI.OPENICON);
							m_IconIndexOpen = shfi.iIcon;
							//If m_TypeName.Equals("File Folder") Then
							//    OpenFolderIconIndex = shfi.iIcon
							//End If
						}
						else
						{
							m_IconIndexOpen = OpenFolderIconIndex;
						}
					}
					else
					{
						m_IconIndexOpen = m_IconIndexNormal;
					}
				}
				return m_IconIndexOpen;
			}
		}
		#endregion
		
		#region "           FileInfo type Information"
		
		/// <summary>
		/// Obtains information available from FileInfo.
		/// </summary>
		///
		private void FillDemandInfo()
		{
			if (m_IsDisk)
			{
				try
				{
					//See if this is a network drive
					//NoRoot = 1
					//Removable = 2
					//LocalDisk = 3
					//Network = 4
					//CD = 5
					//RAMDrive = 6
					System.Management.ManagementObject disk = new System.Management.ManagementObject("win32_logicaldisk.deviceid=\"" + m_Path.Substring(0, 2) + "\"");
                    m_Length = (long)System.Convert.ToUInt64(disk["Size"]);//.ToString().Length;
					if (System.Convert.ToUInt32(disk["DriveType"]).ToString() == 4.ToString())
					{
						m_IsNetWorkDrive = true;
					}
				}
				catch (Exception)
				{
					//Disconnected Network Drives etc. will generate
					//an error here, just assume that it is a network
					//drive
					m_IsNetWorkDrive = true;
				}
				finally
				{
					m_XtrInfo = true;
				}
			}
			else if (! m_IsDisk && m_IsFileSystem && ! m_IsFolder)
			{
				//in this case, it's a file
				if (File.Exists(m_Path))
				{
					FileInfo fi = new FileInfo(m_Path);
					m_LastWriteTime = fi.LastWriteTime;
					m_LastAccessTime = fi.LastAccessTime;
					m_CreationTime = fi.CreationTime;
					m_Length = fi.Length;
					m_XtrInfo = true;
				}
			}
			else
			{
				if (m_IsFileSystem && m_IsFolder)
				{
					if (Directory.Exists(m_Path))
					{
						DirectoryInfo di = new DirectoryInfo(m_Path);
						m_LastWriteTime = di.LastWriteTime;
						m_LastAccessTime = di.LastAccessTime;
						m_CreationTime = di.CreationTime;
						m_XtrInfo = true;
					}
				}
			}
		}
		
		public DateTime LastWriteTime
		{
			get
			{
				if (! m_XtrInfo)
				{
					FillDemandInfo();
				}
				return m_LastWriteTime;
			}
		}
		public DateTime LastAccessTime
		{
			get
			{
				if (! m_XtrInfo)
				{
					FillDemandInfo();
				}
				return m_LastAccessTime;
			}
		}
		public DateTime CreationTime
		{
			get
			{
				if (! m_XtrInfo)
				{
					FillDemandInfo();
				}
				return m_CreationTime;
			}
		}
		public long Length
		{
			get
			{
				if (! m_XtrInfo)
				{
					FillDemandInfo();
				}
				return m_Length;
			}
		}
		public bool IsNetworkDrive
		{
			get
			{
				if (! m_XtrInfo)
				{
					FillDemandInfo();
				}
				return m_IsNetWorkDrive;
			}
		}
		#endregion
		
		#region "           cPidl information"
		public cPidl clsPidl
		{
			get
			{
				if (m_cPidl == null)
				{
					m_cPidl = new cPidl(m_Pidl);
				}
				return m_cPidl;
			}
		}
		#endregion
		
		#region "       IsReadOnly and IsSystem"
		///<Summary>The IsReadOnly attribute causes an annoying access to any floppy drives
		/// on the system. To postpone this (or avoid, depending on user action),
		/// the attribute is only queried when asked for</Summary>
		public bool IsReadOnly
		{
			get
			{
				if (m_IsReadOnlySetup)
				{
					return m_IsReadOnly;
				}
				else
				{
					ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
					shfi.dwAttributes = (int)ExpTreeLib.ShellDll.SFGAO.RDONLY;
					int dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.PIDL | ExpTreeLib.ShellDll.SHGFI.ATTRIBUTES | ExpTreeLib.ShellDll.SHGFI.ATTR_SPECIFIED);
					int dwAttr = 0;
					IntPtr H = ExpTreeLib.ShellDll.SHGetFileInfo(m_Pidl, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);
					if (H.ToInt32() != ExpTreeLib.ShellDll.NOERROR && H.ToInt32() != 1)
					{
						Marshal.ThrowExceptionForHR(H.ToInt32());
					}
					m_IsReadOnly = System.Convert.ToBoolean(shfi.dwAttributes & (int)ExpTreeLib.ShellDll.SFGAO.RDONLY);
					//If m_IsReadOnly Then Debug.WriteLine("IsReadOnly -- " & m_Path)
					m_IsReadOnlySetup = true;
					return m_IsReadOnly;
				}
				//If Not m_XtrInfo Then
				//    FillDemandInfo()
				//End If
				//Return m_Attributes And FileAttributes.ReadOnly = FileAttributes.ReadOnly
			}
		}
		///<Summary>The IsSystem attribute is seldom used, but required by DragDrop operations.
		/// Since there is no way of getting ONLY the System attribute without getting
		/// the RO attribute (which forces a reference to the floppy drive), we pay
		/// the price of getting its own File/DirectoryInfo for this purpose alone.
		///</Summary>
		static bool IsSystem_HaveSysInfo;
		static bool IsSystem_m_IsSystem;
		public bool IsSystem
		{
			get
			{
				if (! IsSystem_HaveSysInfo)
				{
					try
					{
						IsSystem_m_IsSystem = (File.GetAttributes(m_Path) & FileAttributes.System) == FileAttributes.System;
						IsSystem_HaveSysInfo = true;
					}
					catch (Exception)
					{
						IsSystem_HaveSysInfo = true;
					}
				}
				Debug.WriteLine("In IsSystem -- Path = " + m_Path + " IsSystem = " + IsSystem_m_IsSystem);
				return IsSystem_m_IsSystem;
			}
		}
		
		#endregion
		
		#endregion
		
		#endregion
		
		#region "   Public Methods"
		
		#region "       Shared Public Methods"
		
		#region "       GetDeskTop"
		/// <summary>
		/// If not initialized, then build DesktopBase
		/// once done, or if initialized already,
		/// </summary>
		///<returns>The DesktopBase CShItem representing the desktop</returns>
		///
		public static CShItem GetDeskTop()
		{
			if (DesktopBase == null)
			{
				DesktopBase = new CShItem();
			}
			return DesktopBase;
		}
		#endregion
		
		#region "       IsAncestorOf"
		///<Summary>IsAncestorOf returns True if CShItem ancestor is an ancestor of CShItem current
		/// if OS is Win2K or above, uses the ILIsParent API, otherwise uses the
		/// cPidl function StartsWith.  This is necessary since ILIsParent in only available
		/// in Win2K or above systems AND StartsWith fails on some folders on XP systems (most
		/// obviously some Network Folder Shortcuts, but also Control Panel. Note, StartsWith
		/// always works on systems prior to XP.
		/// NOTE: if ancestor and current reference the same Item, both
		/// methods return True</Summary>
		public static bool IsAncestorOf(CShItem ancestor, CShItem current, bool fParent)
		{
			return IsAncestorOf(ancestor.PIDL, current.PIDL, fParent);
		}
		///<Summary> Compares a candidate Ancestor PIDL with a Child PIDL and
		/// returns True if Ancestor is an ancestor of the child.
		/// if fParent is True, then only return True if Ancestor is the immediate
		/// parent of the Child</Summary>
		public static bool IsAncestorOf(IntPtr AncestorPidl, IntPtr ChildPidl, bool fParent)
		{
			bool returnValue;
			if (ShellDll.Is2KOrAbove())
			{
				return ExpTreeLib.ShellDll.ILIsParent(AncestorPidl, ChildPidl, fParent);
			}
			else
			{
				cPidl Child = new cPidl(ChildPidl);
				cPidl Ancestor = new cPidl(AncestorPidl);
				returnValue = Child.StartsWith(Ancestor);
				if (! returnValue)
				{
					return returnValue;
				}
				if (fParent) // check for immediate ancestor, if desired
				{
					object[] oAncBytes = Ancestor.Decompose();
					object[] oChildBytes = Child.Decompose();
					if (oAncBytes.Length != (oChildBytes.Length - 1))
					{
						returnValue = false;
					}
				}
			}
			return returnValue;
		}
		#endregion
		
		#region "      AllFolderWalk"
		///<Summary>The WalkAllCallBack delegate defines the signature of
		/// the routine to be passed to DirWalker
		/// Usage:  dim d1 as new CshItem.WalkAllCallBack(addressof yourroutine)
		///   Callback function receives a CShItem for each file and Directory in
		///   Starting Directory and each sub-directory of this directory and
		///   each sub-dir of each sub-dir ....
		///</Summary>
		public delegate bool WalkAllCallBack(CShItem info, int UserLevel, int Tag);
		///<Summary>
		/// AllFolderWalk recursively walks down directories from cStart, calling its
		///   callback routine, WalkAllCallBack, for each Directory and File encountered, including those in
		///   cStart.  UserLevel is incremented by 1 for each level of dirs that DirWalker
		///  recurses thru.  Tag in an Integer that is simply passed, unmodified to the
		///  callback, with each CShItem encountered, both File and Directory CShItems.
		/// </Summary>
		/// <param name="cStart"></param>
		/// <param name="cback"></param>
		/// <param name="UserLevel"></param>
		/// <param name="Tag"></param>
		///
		public static bool AllFolderWalk(CShItem cStart, WalkAllCallBack cback, int UserLevel, int Tag)
		{
			if (cStart != null&& cStart.IsFolder)
			{
				CShItem cItem;
				//first processes all files in this directory
				foreach (CShItem tempLoopVar_cItem in cStart.GetFiles())
				{
					cItem = tempLoopVar_cItem;
					if (! cback(cItem, UserLevel, Tag))
					{
						return false; //user said stop
					}
				}
				//then process all dirs in this directory, recursively
				foreach (CShItem tempLoopVar_cItem in cStart.GetDirectories(true))
				{
					cItem = tempLoopVar_cItem;
					if (! cback(cItem, UserLevel + 1, Tag))
					{
						return false; //user said stop
					}
					else
					{
						if (! AllFolderWalk(cItem, cback, UserLevel + 1, Tag))
						{
							return false;
						}
					}
				}
				return true;
			}
			else //Invalid call
			{
				throw (new ApplicationException("AllFolderWalk -- Invalid Start Directory"));
			}
		}
		#endregion
		
		#endregion
		
		#region "       Public Instance Methods"
		
		#region "           Equals"
		public bool Equals(CShItem other)
		{
			bool returnValue;
			returnValue = this.Path.Equals(other.Path);
			return returnValue;
		}
		#endregion
		
		#region "       GetDirectories"
		/// <summary>
		/// Returns the Directories of this sub-folder as an ArrayList of CShitems
		/// </summary>
		/// <param name="doRefresh">Optional, default=True, Refresh the directories</param>
		/// <returns>An ArrayList of CShItems. May return an empty ArrayList if there are none.</returns>
		/// <remarks>revised to alway return an up-to-date list unless
		/// specifically instructed not to (useful in constructs like:
		/// if CSI.RefreshDirectories then
		///     Dirs = CSI.GetDirectories(False)  ' just did a Refresh </remarks>
		public List<CShItem> GetDirectories(bool doRefresh)
		{
			if (m_IsFolder)
			{
				if (doRefresh)
				{
					RefreshDirectories(); // return an up-to-date List
				}
				else if (m_Directories == null)
				{
					RefreshDirectories();
				}
				return m_Directories;
			}
			else //if it is not a Folder, then return empty arraylist
			{
                return new List<CShItem>();
			}
		}
		
		#endregion
		
		#region "       GetFiles"
		/// <summary>
		/// Returns the Files of this sub-folder as an
		///   ArrayList of CShitems
		/// Note: we do not keep the arraylist of files, Generate it each time
		/// </summary>
		/// <returns>An ArrayList of CShItems. May return an empty ArrayList if there are none.</returns>
		///
        public List<CShItem> GetFiles()
		{
			if (m_IsFolder)
			{                
				return GetContents(ExpTreeLib.ShellDll.SHCONTF.NONFOLDERS | ExpTreeLib.ShellDll.SHCONTF.INCLUDEHIDDEN);
			}
			else
			{
                return new List<CShItem>();
			}
		}
		
		/// <summary>
		/// Returns the Files of this sub-folder, filtered by a filtering string, as an
		///   ArrayList of CShitems
		/// Note: we do not keep the arraylist of files, Generate it each time
		/// </summary>
		/// <param name="Filter">A filter string (for example: *.Doc)</param>
		/// <returns>An ArrayList of CShItems. May return an empty ArrayList if there are none.</returns>
		///
        public List<CShItem> GetFiles(string Filter)
		{
            if (m_IsFolder)
            {
                List<CShItem> al = GetContents(ExpTreeLib.ShellDll.SHCONTF.NONFOLDERS | ExpTreeLib.ShellDll.SHCONTF.INCLUDEHIDDEN);
                List<CShItem> result = new List<CShItem>(al.Count);
                string filterExt = Filter.Substring(Filter.IndexOf('.'));
                foreach (CShItem item in al)
                    if (string.Compare(System.IO.Path.GetExtension(item.Path), filterExt, true) == 0)
                        result.Add(item);
                return result;
            }
            else
            {
                return new List<CShItem>();
            }			
		}
		#endregion
		
		#region "       GetItems"
		/// <summary>
		/// Returns the Directories and Files of this sub-folder as an
		///   ArrayList of CShitems
		/// Note: we do not keep the arraylist of files, Generate it each time
		/// </summary>
		/// <returns>An ArrayList of CShItems. May return an empty ArrayList if there are none.</returns>
		public ArrayList GetItems()
		{
			ArrayList rVal = new ArrayList();
			if (m_IsFolder)
			{
				rVal.AddRange(this.GetDirectories(true));
				rVal.AddRange(this.GetContents(ExpTreeLib.ShellDll.SHCONTF.NONFOLDERS | ExpTreeLib.ShellDll.SHCONTF.INCLUDEHIDDEN));
				rVal.Sort();
				return rVal;
			}
			else
			{
				return rVal;
			}
		}
		#endregion
		
		#region "       GetFileName"
		///<Summary>GetFileName returns the Full file name of this item.
		///  Specifically, for a link file (xxx.txt.lnk for example) the
		///  DisplayName property will return xxx.txt, this method will
		///  return xxx.txt.lnk.  In most cases this is equivalent of
		///  System.IO.Path.GetFileName(m_Path).  However, some m_Paths
		///  actually are GUIDs.  In that case, this routine returns the
		///  DisplayName</Summary>
		public string GetFileName()
		{
			if (m_Path.StartsWith("::{")) //Path is really a GUID
			{
				return this.DisplayName;
			}
			else
			{
				if (m_IsDisk)
				{
					return m_Path.Substring(0, 1);
				}
				else
				{
					return System.IO.Path.GetFileName(m_Path);
				}
			}
		}
		#endregion
		
		#region "       ReFreshDirectories"
		///<Summary> A lower cost way of Refreshing the Directories of this CShItem</Summary>
		///<returns> Returns True if there were any changes</returns>
		public bool RefreshDirectories()
		{
			bool returnValue;
			returnValue = false; //value unless there were changes
			if (m_IsFolder) //if not a folder, then return false
			{
				ArrayList InvalidDirs = new ArrayList(); //holds CShItems of not found dirs
				if (m_Directories == null)
				{
					m_Directories = GetContents(ExpTreeLib.ShellDll.SHCONTF.FOLDERS | ExpTreeLib.ShellDll.SHCONTF.INCLUDEHIDDEN);
					returnValue = true; //changed from unexamined to examined
				}
				else
				{
					//Get relative PIDLs from current directory items
                    List<IntPtr> curPidls = GetContentsPIDL(ExpTreeLib.ShellDll.SHCONTF.FOLDERS | ExpTreeLib.ShellDll.SHCONTF.INCLUDEHIDDEN);
					
					if (curPidls.Count < 1)
					{
						if (m_Directories.Count > 0)
						{
                            m_Directories = new List<CShItem>(); //nothing there anymore
							returnValue = true; //Changed from had some to have none
						}
						else //Empty before, Empty now, do nothing -- just a logic marker
						{
						}
					}
					else //still has some. Are they the same?
					{
						if (m_Directories.Count < 1) //didn't have any before, so different
						{
							m_Directories = GetContents(ExpTreeLib.ShellDll.SHCONTF.FOLDERS | ExpTreeLib.ShellDll.SHCONTF.INCLUDEHIDDEN);
							returnValue = true; //changed from had none to have some
						}
						else //some before, some now. Same?  This is the complicated part
						{
							//Firstly, build ArrayLists of Relative Pidls
							ArrayList compList = new ArrayList(curPidls);
							//Since we are only comparing relative PIDLs, build a list of
							// the relative PIDLs of the old content -- saving repeated building
							int iOld;
							IntPtr[] OldRel = new IntPtr[m_Directories.Count];
							for (iOld = 0; iOld <= m_Directories.Count - 1; iOld++)
							{
								//GetLastID returns a ptr into an EXISTING IDLIST -- never release that ptr
								// and never release the EXISTING IDLIST before thru with OldRel
								OldRel[iOld] = GetLastID(((CShItem) (m_Directories[iOld])).PIDL);
							}
							int iNew;
							for (iOld = 0; iOld <= m_Directories.Count - 1; iOld++)
							{
								for (iNew = 0; iNew <= compList.Count - 1; iNew++)
								{
									if (IsEqual(((IntPtr) (compList[iNew])), OldRel[iOld]))
									{
										compList.RemoveAt(iNew); //Match, don't look at this one again
										goto NXTOLD; //content item exists in both
									}
								}
								//falling thru here means couldn't find iOld entry
								InvalidDirs.Add(m_Directories[iOld]); //save off the unmatched CShItem
								returnValue = true;
NXTOLD:
								1.GetHashCode() ; //nop
							}
							//any not found should be removed from m_Directories
							CShItem csi;
							foreach (CShItem tempLoopVar_csi in InvalidDirs)
							{
								csi = tempLoopVar_csi;
								m_Directories.Remove(csi);
							}
							//anything remaining in compList is a new entry
							if (compList.Count > 0)
							{
								returnValue = true;
								foreach (IntPtr iptr in compList) //these are relative PIDLs																	
									m_Directories.Add(new CShItem(m_Folder, iptr, this));								
							}
							if (returnValue) //something added or removed, resort
							{
								m_Directories.Sort();
							}
						}
						//we obtained some new relative PIDLs in curPidls, so free them
                        foreach (IntPtr iptr in curPidls)													
							Marshal.FreeCoTaskMem(iptr);

					} //end of content comparison
				} //end of IsNothing test
			} //end of IsFolder test
			return returnValue;
		}
		
		#endregion
		
		#region "       ToString"
		/// <summary>
		/// Returns the DisplayName as the normal ToString value
		/// </summary>
		///
		public override string ToString()
		{
			return m_DisplayName;
		}
		#endregion
		
		#region "       Debug Dumper"
		/// <summary>
		/// Summary of DebugDump.
		/// </summary>
		///
		public void DebugDump()
		{
			Debug.WriteLine("DisplayName = " + m_DisplayName);
			Debug.WriteLine("PIDL        = " + m_Pidl.ToString());
			Debug.WriteLine("\t" + "Path        = " + m_Path);
			Debug.WriteLine("\t" + "TypeName    = " + this.TypeName);
			Debug.WriteLine("\t" + "iIconNormal = " + m_IconIndexNormal);
			Debug.WriteLine("\t" + "iIconSelect = " + m_IconIndexOpen);
			Debug.WriteLine("\t" + "IsBrowsable = " + m_IsBrowsable);
			Debug.WriteLine("\t" + "IsFileSystem= " + m_IsFileSystem);
			Debug.WriteLine("\t" + "IsFolder    = " + m_IsFolder);
			Debug.WriteLine("\t" + "IsLink    = " + m_IsLink);
			Debug.WriteLine("\t" + "IsDropTarget = " + m_IsDropTarget);
			Debug.WriteLine("\t" + "IsReadOnly   = " + this.IsReadOnly);
			Debug.WriteLine("\t" + "CanCopy = " + this.CanCopy);
			Debug.WriteLine("\t" + "CanLink = " + this.CanLink);
			Debug.WriteLine("\t" + "CanMove = " + this.CanMove);
			Debug.WriteLine("\t" + "CanDelete = " + this.CanDelete);
			if (m_IsFolder)
			{
				if (m_Directories != null)
				{
					Debug.WriteLine("\t" + "Directory Count = " + m_Directories.Count);
				}
				else
				{
					Debug.WriteLine("\t" + "Directory Count Not yet set");
				}
			}
		}
		#endregion
		
		#region "       GetDropTargetOf"
		public IDropTarget GetDropTargetOf(Control tn)
		{
			if (m_Folder == null)
			{
				return null;
			}
			IntPtr[] apidl = new IntPtr[1];
			int HR;
            ShellDll.IUnknown theInterface = null;
			IntPtr tnH = tn.Handle;
			HR = m_Folder.CreateViewObject(tnH, ref ShellDll.IID_IDropTarget, ref theInterface);
			if (HR != 0)
			{
				Marshal.ThrowExceptionForHR(HR);
			}
            return theInterface as IDropTarget;
		}
		#endregion
		
		#endregion
		
		#endregion
		
		#region "   Private Instance Methods"
		
		#region "       GetContents"
		///<Summary>
		/// Returns the requested Items of this Folder as an ArrayList of CShitems
		///  unless the IntPtrOnly flag is set.  If IntPtrOnly is True, return an
		///  ArrayList of PIDLs.
		///</Summary>
		/// <param name="flags">A set of one or more SHCONTF flags indicating which items to return</param>
		/// <param name="IntPtrOnly">True to suppress generation of CShItems, returning only an
		///  ArrayList of IntPtrs to RELATIVE PIDLs for the contents of this Folder</param>
        private List<IntPtr> GetContentsPIDL(ExpTreeLib.ShellDll.SHCONTF flags)
		{
            List<IntPtr> rVal = new List<IntPtr>();
			int HR;
			ShellDll.IEnumIDList IEnum = null;
			HR = m_Folder.EnumObjects(0, flags, ref IEnum);
			if (HR == ExpTreeLib.ShellDll.NOERROR)
			{
				IntPtr item = IntPtr.Zero;
				int itemCnt = 0;
				HR = IEnum.GetNext(1, ref item, ref itemCnt);
				if (HR == ExpTreeLib.ShellDll.NOERROR)
				{
					while (itemCnt > 0 && ! item.Equals(IntPtr.Zero))
					{
						//there is no setting to exclude folders from enumeration,
						// just one to include non-folders
						// so we have to screen the results to return only
						//  non-folders if folders are not wanted
						if ((flags & ExpTreeLib.ShellDll.SHCONTF.FOLDERS)==0) //don't want folders. see if this is one
						{
							ExpTreeLib.ShellDll.SFGAO attrFlag = 0;
							attrFlag = attrFlag | ExpTreeLib.ShellDll.SFGAO.FOLDER | ExpTreeLib.ShellDll.SFGAO.STREAM;
							//Note: for GetAttributesOf, we must provide an array, in all cases with 1 element
							IntPtr[] aPidl = new IntPtr[1];
							aPidl[0] = item;
							m_Folder.GetAttributesOf(1, aPidl, ref attrFlag);
							if (! XPorAbove)
							{
								if (System.Convert.ToBoolean(attrFlag & ExpTreeLib.ShellDll.SFGAO.FOLDER)) //Don't need it
								{
									goto SKIPONE;
								}
							}
							else //XP or above
							{
								if ((attrFlag & ExpTreeLib.ShellDll.SFGAO.FOLDER)!=0 && (attrFlag & ExpTreeLib.ShellDll.SFGAO.STREAM)==0)
								{
									goto SKIPONE;
								}
							}
						}

                        //just relative pidls for fast look, no CShITem overhead						
						rVal.Add(PIDLClone(item)); //caller must free						
SKIPONE:
						Marshal.FreeCoTaskMem(item); //if New kept it, it kept a copy
						item = IntPtr.Zero;
						itemCnt = 0;
						// Application.DoEvents()
						HR = IEnum.GetNext(1, ref item, ref itemCnt);
					}
				}
				else
				{
					if (HR != 1)
					{
						goto HRError; //1 means no more
					}
				}
			}
			else
			{
				
				goto HRError;
			}
			//Normal Exit
NORMAL:
			if (IEnum != null)
			{
				Marshal.ReleaseComObject(IEnum);
			}
			//rVal.TrimToSize();
			return rVal;
			
			// Error Exit for all Com errors
HRError: //not ready disks will return the following error
			//If HR = &HFFFFFFFF800704C7 Then
			//    GoTo NORMAL
			//ElseIf HR = &HFFFFFFFF80070015 Then
			//    GoTo NORMAL
			//    'unavailable net resources will return these
			//ElseIf HR = &HFFFFFFFF80040E96 Or HR = &HFFFFFFFF80040E19 Then
			//    GoTo NORMAL
			//ElseIf HR = &HFFFFFFFF80004001 Then 'Certain "Not Implemented" features will return this
			//    GoTo NORMAL
			//ElseIf HR = &HFFFFFFFF80004005 Then
			//    GoTo NORMAL
			//ElseIf HR = &HFFFFFFFF800704C6 Then
			//    GoTo NORMAL
			//End If
			if (IEnum != null)
			{
                Marshal.ReleaseComObject(IEnum);
            }
				//#If Debug Then
				//        Marshal.ThrowExceptionForHR(HR)
				//#End If
				rVal = new List<IntPtr>(); //sometimes it is a non-fatal error,ignored
				goto NORMAL;
            }
        private List<CShItem> GetContents(ExpTreeLib.ShellDll.SHCONTF flags)
        {
            List<IntPtr> pidlList = GetContentsPIDL(flags);

            List<CShItem> rVal = new List<CShItem>(pidlList.Count);            

            foreach(IntPtr pidl in pidlList)
                rVal.Add(new CShItem(m_Folder, pidl, this));

            return rVal;
        }
			#endregion
			
			#region "       Really nasty Pidl manipulation"
			
			/// <Summary>
			/// Get Size in bytes of the first (possibly only)
			///  SHItem in an ID list.  Note: the full size of
			///   the item is the sum of the sizes of all SHItems
			///   in the list!!
			/// </Summary>
			/// <param name="pidl"></param>
			///
			private static int ItemIDSize(IntPtr pidl)
			{
				if (! pidl.Equals(IntPtr.Zero))
				{
					byte[] b = new byte[2];
					Marshal.Copy(pidl, b, 0, 2);
					return b[1] * 256 + b[0];
				}
				else
				{
					return 0;
				}
			}
			
			/// <summary>
			/// computes the actual size of the ItemIDList pointed to by pidl
			/// </summary>
			/// <param name="pidl">The pidl pointing to an ItemIDList</param>
			///<returns> Returns actual size of the ItemIDList, less the terminating nulnul</returns>
			public static int ItemIDListSize(IntPtr pidl)
			{
				if (! pidl.Equals(IntPtr.Zero))
				{
					int i = ItemIDSize(pidl);
					int b = Marshal.ReadByte(pidl, i) + (Marshal.ReadByte(pidl, i + 1) * 256);
					while (b > 0)
					{
						i += b;
						b = Marshal.ReadByte(pidl, i) + (Marshal.ReadByte(pidl, i + 1) * 256);
					}
					return i;
				}
				else
				{
					
					return 0;
				}
			}
			/// <summary>
			/// Counts the total number of SHItems in input pidl
			/// </summary>
			/// <param name="pidl">The pidl to obtain the count for</param>
			/// <returns> Returns the count of SHItems pointed to by pidl</returns>
			public static int PidlCount(IntPtr pidl)
			{
				if (! pidl.Equals(IntPtr.Zero))
				{
					int cnt = 0;
					int i = 0;
					int b = Marshal.ReadByte(pidl, i) + (Marshal.ReadByte(pidl, i + 1) * 256);
					while (b > 0)
					{
						cnt++;
						i += b;
						b = Marshal.ReadByte(pidl, i) + (Marshal.ReadByte(pidl, i + 1) * 256);
					}
					return cnt;
				}
				else
				{
					
					return 0;
				}
			}
			
			///<Summary>GetLastId -- returns a pointer to the last ITEMID in a valid
			/// ITEMIDLIST. Returned pointer SHOULD NOT be released since it
			/// points to place within the original PIDL</Summary>
			///<returns>IntPtr pointing to last ITEMID in ITEMIDLIST structure,
			/// Returns IntPtr.Zero if given a null pointer.
			/// If given a pointer to the Desktop, will return same pointer.</returns>
			///<remarks>This is what the API ILFindLastID does, however IL...
			/// functions are not supported before Win2K.</remarks>
			public static IntPtr GetLastID(IntPtr pidl)
			{
				if (! pidl.Equals(IntPtr.Zero))
				{
					int prev = 0;
					int i = 0;
					int b = Marshal.ReadByte(pidl, i) + (Marshal.ReadByte(pidl, i + 1) * 256);
					while (b > 0)
					{
						prev = i;
						i += b;
						b = Marshal.ReadByte(pidl, i) + (Marshal.ReadByte(pidl, i + 1) * 256);
					}
					return new IntPtr(pidl.ToInt32() + prev);
				}
				else
				{
					
					return IntPtr.Zero;
				}
			}
			
			public static IntPtr[] DecomposePIDL(IntPtr pidl)
			{
				int lim = ItemIDListSize(pidl);
				IntPtr[] PIDLs = new IntPtr[PidlCount(pidl)];
				int i = 0;
				int curB = 0;
				int offSet = 0;
				while (curB < lim)
				{
					IntPtr thisPtr = new IntPtr(pidl.ToInt32() + curB);
					offSet = Marshal.ReadByte(thisPtr) + (Marshal.ReadByte(thisPtr, 1) * 256);
					PIDLs[i] = Marshal.AllocCoTaskMem(offSet + 2);
					byte[] b = new byte[offSet+1 + 1];
					Marshal.Copy(thisPtr, b, 0, offSet);
					b[offSet] = 0;
					
					b[offSet + 1] = 0;
					Marshal.Copy(b, 0, PIDLs[i], offSet + 2);
					//DumpPidl(PIDLs(i))
					curB += offSet;
					i++;
				}
				return PIDLs;
			}
			
			private static IntPtr PIDLClone(IntPtr pidl)
			{
				IntPtr returnValue;
				int cb = ItemIDListSize(pidl);
				byte[] b = new byte[cb+1 + 1];
				Marshal.Copy(pidl, b, 0, cb); //not including terminating nulnul
				b[cb] = 0;
				
				b[cb + 1] = 0; //force to nulnul
				returnValue = Marshal.AllocCoTaskMem(cb + 2);
				Marshal.Copy(b, 0, returnValue, cb + 2);
				return returnValue;
			}
			
			public static bool IsEqual(IntPtr Pidl1, IntPtr Pidl2)
			{
				if (Win2KOrAbove)
				{
					return ExpTreeLib.ShellDll.ILIsEqual(Pidl1, Pidl2);
				}
				else //do hard way, may not work for some folders on XP
				{
					
					int cb1;
					int cb2;
					cb1 = ItemIDListSize(Pidl1);
					cb2 = ItemIDListSize(Pidl2);
					if (cb1 != cb2)
					{
						return false;
					}
					int lim32 = cb1 / 4;
					
					int i;
					for (i = 0; i <= lim32 - 1; i++)
					{
						if (Marshal.ReadInt32(Pidl1, i) != Marshal.ReadInt32(Pidl2, i))
						{
							return false;
						}
					}
					int limB = cb1 % 4;
					int offset = lim32 * 4;
					for (i = 0; i <= limB - 1; i++)
					{
						if (Marshal.ReadByte(Pidl1, offset + i) != Marshal.ReadByte(Pidl2, offset + i))
						{
							return false;
						}
					}
					return true; //made it to here, so they are equal
				}
			}
			
			/// <summary>
			/// Concatenates the contents of two pidls into a new Pidl (ended by 00)
			/// allocating CoTaskMem to hold the result,
			/// placing the concatenation (followed by 00) into the
			/// allocated Memory,
			/// and returning an IntPtr pointing to the allocated mem
			/// </summary>
			/// <param name="pidl1">IntPtr to a well formed SHItemIDList or IntPtr.Zero</param>
			/// <param name="pidl2">IntPtr to a well formed SHItemIDList or IntPtr.Zero</param>
			/// <returns>Returns a ptr to an ItemIDList containing the
			///   concatenation of the two (followed by the req 2 zeros
			///   Caller must Free this pidl when done with it</returns>
			public static IntPtr concatPidls(IntPtr pidl1, IntPtr pidl2)
			{
				int cb1;
				int cb2;
				cb1 = ItemIDListSize(pidl1);
				cb2 = ItemIDListSize(pidl2);
				int rawCnt = cb1 + cb2;
				if ((rawCnt) > 0)
				{
					byte[] b = new byte[rawCnt+1 + 1];
					if (cb1 > 0)
					{
						Marshal.Copy(pidl1, b, 0, cb1);
					}
					if (cb2 > 0)
					{
						Marshal.Copy(pidl2, b, cb1, cb2);
					}
					IntPtr rVal = Marshal.AllocCoTaskMem(cb1 + cb2 + 2);
					b[rawCnt] = 0;
					
					b[rawCnt + 1] = 0;
					Marshal.Copy(b, 0, rVal, rawCnt + 2);
					return rVal;
				}
				else
				{
					return IntPtr.Zero;
				}
			}
			
			/// <summary>
			/// Returns an ItemIDList with the last ItemID trimed off
			///  This is necessary since I cannot get SHBindToParent to work
			///  It's purpose is to generate an ItemIDList for the Parent of a
			///  Special Folder which can then be processed with DesktopBase.BindToObject,
			///  yeilding a Folder for the parent of the Special Folder
			///  It also creates and passes back a RELATIVE pidl for this item
			/// </summary>
			/// <param name="pidl">A pointer to a well formed ItemIDList. The PIDL to trim</param>
			/// <param name="relPidl">BYREF IntPtr which will point to a new relative pidl
			///        containing the contents of the last ItemID in the ItemIDList
			///        terminated by the required 2 nulls.</param>
			/// <returns> an ItemIDList with the last element removed.
			///  Caller must Free this item when through with it
			///  Also returns the new relative pidl in the 2nd parameter
			///   Caller must Free this pidl as well, when through with it
			///</returns>
			public static IntPtr TrimPidl(IntPtr pidl, ref IntPtr relPidl)
			{
				int cb = ItemIDListSize(pidl);
				byte[] b = new byte[cb+1 + 1];
				Marshal.Copy(pidl, b, 0, cb);
				int prev = 0;
				int i = b[0] + (b[1] * 256);
				//Do While i < cb AndAlso b(i) <> 0
				while (i > 0 && i < cb) //Changed code
				{
					prev = i;
					i += b[i] + (b[i + 1] * 256);
				}
				if ((prev + 1) < cb)
				{
					//first set up the relative pidl
					b[cb] = 0;
					b[cb + 1] = 0;
					int cb1 = b[prev] + (b[prev + 1] * 256);
					relPidl = Marshal.AllocCoTaskMem(cb1 + 2);
					Marshal.Copy(b, prev, relPidl, cb1 + 2);
					b[prev] = 0;
					
					b[prev + 1] = 0;
					IntPtr rVal = Marshal.AllocCoTaskMem(prev + 2);
					Marshal.Copy(b, 0, rVal, prev + 2);
					return rVal;
				}
				else
				{
					return IntPtr.Zero;
				}
			}
			
			#region "   DumpPidl Routines"
			/// <summary>
			/// Dumps, to the Debug output, the contents of the mem block pointed to by
			/// a PIDL. Depends on the internal structure of a PIDL
			/// </summary>
			/// <param name="pidl">The IntPtr(a PIDL) pointing to the block to dump</param>
			///
			public static void DumpPidl(IntPtr pidl)
			{
				int cb = ItemIDListSize(pidl);
				Debug.WriteLine("PIDL " + pidl.ToString() + " contains " + cb + " bytes");
				if (cb > 0)
				{
					byte[] b = new byte[cb+1 + 1];
					Marshal.Copy(pidl, b, 0, cb + 1);
					int pidlCnt = 1;
					int i = b[0] + (b[1] * 256);
					int curB = 0;
					while (i > 0)
					{
						Debug.Write("ItemID #" + pidlCnt + " Length = " + i);
						DumpHex(b, curB, curB + i - 1);
						pidlCnt++;
						curB += i;
						i = b[curB] + (b[curB + 1] * 256);
					}
				}
			}
			
			///<Summary>Dump a portion or all of a Byte Array to Debug output</Summary>
			///<param name = "b">A single dimension Byte Array</param>
			///<param name = "sPos">Optional start index of area to dump (default = 0)</param>
			///<param name = "epos">Optional last index position to dump (default = end of array)</param>
			///<Remarks>
			///</Remarks>
			public static void DumpHex(byte[] b, int sPos, int ePos)
			{
				if (ePos == 0)
				{
					ePos = b.Length - 1;
				}
				int j;
				int curB = sPos;
				string sTmp;
				char ch;
				StringBuilder SBH = new StringBuilder();
				StringBuilder SBT = new StringBuilder();
				for (j = 0; j <= ePos - sPos; j++)
				{
					if (j % 16 == 0)
					{
						Debug.WriteLine(SBH.ToString() + SBT.ToString());
						SBH = new StringBuilder();
						
						SBT = new StringBuilder("          ");
						SBH.Append(HexNum(j + sPos, 4) + "). ");
					}
					if (b[curB] < 16)
					{
						sTmp = "0" + Convert.ToString(b[curB],16);
					}
					else
					{
                        sTmp = Convert.ToString(b[curB], 16);
					}
					SBH.Append(sTmp);
					
					SBH.Append(" ");
					ch = Convert.ToChar(b[curB]);
					if (char.IsControl(ch))
					{
						SBT.Append(".");
					}
					else
					{
						SBT.Append(ch);
					}
					curB++;
				}
				
				int fill = (j) % 16;
				if (fill != 0)
				{
					SBH.Append(' ', 48 - (3 * ((j) % 16)));
				}
				Debug.WriteLine(SBH.ToString() + SBT.ToString());
			}
			
			public static string HexNum(int num, int nrChrs)
			{
				string h = Convert.ToString(num, 16);
				StringBuilder SB = new StringBuilder();
				int i;
				for (i = 1; i <= nrChrs - h.Length; i++)
				{
					SB.Append("0");
				}
				SB.Append(h);
				return SB.ToString();
			}
			#endregion
			
			#endregion
			
			#endregion
			
			#region "   TagComparer Class"
			///<Summary> It is sometimes useful to sort a list of TreeNodes,
			/// ListViewItems, or other objects in an order based on CShItems in their Tag
			/// use this Icomparer for that Sort</Summary>
			public class TagComparer : IComparer
			{
				
				public int Compare(object x, object y)
				{
                    CShItem xTag = (x as TreeNode).Tag as CShItem;
                    CShItem yTag = (y as TreeNode).Tag as CShItem;
                    return xTag.CompareTo(yTag);
				}
			}
			#endregion
			
			#region "   cPidl Class"
			///<Summary>cPidl class contains a Byte() representation of a PIDL and
			/// certain Methods and Properties for comparing one cPidl to another</Summary>
			public class cPidl : IEnumerable
			{
				
				
				#region "       Private Fields"
				byte[] m_bytes; //The local copy of the PIDL
				int m_ItemCount; //the # of ItemIDs in this ItemIDList (PIDL)
                //Danat: int m_OffsetToRelative; //the index of the start of the last itemID in m_bytes
				#endregion
				
				#region "       Constructor"
				public cPidl(IntPtr pidl)
				{
					int cb = ItemIDListSize(pidl);
					if (cb > 0)
					{
						m_bytes = new byte[cb+1 + 1];
						Marshal.Copy(pidl, m_bytes, 0, cb);
						//DumpPidl(pidl)
					}
					else
					{
						m_bytes = new byte[2]; //This is the DeskTop (we hope)
					}
					//ensure nulnul
					m_bytes[m_bytes.Length - 2] = 0;
					
					m_bytes[m_bytes.Length - 1] = 0;
					m_ItemCount = PidlCount(pidl);
				}
				#endregion
				
				#region "       Public Properties"
				public byte[] PidlBytes
				{
					get
					{
						return m_bytes;
					}
				}
				
				public int Length
				{
					get
					{
						return m_bytes.Length;
					}
				}
				
				public int ItemCount
				{
					get
					{
						return m_ItemCount;
					}
				}
				
				#endregion
				
				#region "       Public Intstance Methods -- ToPIDL, Decompose, and IsEqual"
				
				///<Summary> Copy the contents of a byte() containing a pidl to
				///  CoTaskMemory, returning an IntPtr that points to that mem block
				/// Assumes that this cPidl is properly terminated, as all New
				/// cPidls are.
				/// Caller must Free the returned IntPtr when done with mem block.
				///</Summary>
				public IntPtr ToPIDL()
				{
					IntPtr returnValue;
					returnValue = BytesToPidl(m_bytes);
					return returnValue;
				}
				
				///<Summary>Returns an object containing a byte() for each of this cPidl's
				/// ITEMIDs (individual PIDLS), in order such that obj(0) is
				/// a byte() containing the bytes of the first ITEMID, etc.
				/// Each ITEMID is properly terminated with a nulnul
				///</Summary>
				public object[] Decompose()
				{
					object[] bArrays = new object[this.ItemCount];
                    ICPidlEnumerator eByte = this.GetEnumerator() as ICPidlEnumerator;
					int i = 0;
					while (eByte.MoveNext())
					{
						bArrays[i] = eByte.Current;
						i++;
					}
					return bArrays;
				}
				
				///<Summary>Returns True if input cPidl's content exactly match the
				/// contents of this instance</Summary>
				public bool IsEqual(cPidl other)
				{
					bool returnValue;
					returnValue = false; //assume not
					if (other.Length != this.Length)
					{
						return returnValue;
					}
					byte[] ob = other.PidlBytes;
					int i;
					for (i = 0; i <= this.Length - 1; i++) //note: we look at nulnul also
					{
						if (ob[i] != m_bytes[i])
						{
							return returnValue;
						}
					}
					return true; //all equal on fall thru
				}
				#endregion
				
				#region "       Public Shared Methods"
				
				#region "           JoinPidlBytes"
				///<Summary> Join two byte arrays containing PIDLS, returning a
				/// Byte() containing the resultant ITEMIDLIST. Both Byte() must
				/// be properly terminated (nulnul)
				/// Returns NOTHING if error
				/// </Summary>
				public static byte[] JoinPidlBytes(byte[] b1, byte[] b2)
				{
					if (IsValidPidl(b1) && IsValidPidl(b2))
					{
						byte[] b = new byte[b1.Length+b2.Length-3 + 1]; //allow for leaving off first nulnul
						Array.Copy(b1, b, b1.Length - 2);
						Array.Copy(b2, 0, b, b1.Length - 2, b2.Length);
						if (IsValidPidl(b))
						{
							return b;
						}
						else
						{
							return null;
						}
					}
					else
					{
						return null;
					}
				}
				#endregion
				
				#region "           BytesToPidl"
				///<Summary> Copy the contents of a byte() containing a pidl to
				///  CoTaskMemory, returning an IntPtr that points to that mem block
				/// Caller must free the IntPtr when done with it
				///</Summary>
				public static IntPtr BytesToPidl(byte[] b)
				{
					IntPtr returnValue;
					returnValue = IntPtr.Zero; //assume failure
					if (IsValidPidl(b))
					{
						int bLen = b.Length;
						returnValue = Marshal.AllocCoTaskMem(bLen);
						if (returnValue.Equals(IntPtr.Zero))
						{
							return returnValue; //another bad error
						}
						Marshal.Copy(b, 0, returnValue, bLen);
					}
					return returnValue;
				}
				#endregion
				
				#region "           StartsWith"
				///<Summary>returns True if the beginning of pidlA matches PidlB exactly for pidlB's entire length</Summary>
				public static bool StartsWith(IntPtr pidlA, IntPtr pidlB)
				{
					return cPidl.StartsWith(new cPidl(pidlA), new cPidl(pidlB));
				}
				
				///<Summary>returns True if the beginning of A matches B exactly for B's entire length</Summary>
				public static bool StartsWith(cPidl A, cPidl B)
				{
					return A.StartsWith(B);
				}
				
				///<Summary>Returns true if the CPidl input parameter exactly matches the
				/// beginning of this instance of CPidl</Summary>
				public bool StartsWith(cPidl cp)
				{
					byte[] b = cp.PidlBytes;
					if (b.Length > m_bytes.Length)
					{
						return false; //input is longer
					}
					int i;
					for (i = 0; i <= b.Length - 3; i++) //allow for nulnul at end of cp.PidlBytes
					{
						if (b[i] != m_bytes[i])
						{
							return false;
						}
					}
					return true;
				}
				#endregion
				
				#endregion
				
				#region "       GetEnumerator"
				public System.Collections.IEnumerator GetEnumerator()
				{
					return new ICPidlEnumerator(m_bytes);
				}
				#endregion
				
				#region "       PIDL enumerator Class"
				private class ICPidlEnumerator : IEnumerator
				{
					
					
					private int m_sPos; //the first index in the current PIDL
					private int m_ePos; //the last index in the current PIDL
					private byte[] m_bytes; //the local copy of the PIDL
					private bool m_NotEmpty = false; //the desktop PIDL is zero length
					
					public ICPidlEnumerator(byte[] b)
					{
						m_bytes = b;
						if (b.Length > 0)
						{
							m_NotEmpty = true;
						}
						m_sPos = - 1;
						
						m_ePos = - 1;
					}
					
					public object Current
					{
						get
						{
							if (m_sPos < 0)
							{
								throw (new InvalidOperationException("ICPidlEnumerator --- attempt to get Current with invalidated list"));
							}
							byte[] b = new byte[(m_ePos-m_sPos)+2 + 1]; //room for nulnul
							Array.Copy(m_bytes, m_sPos, b, 0, b.Length - 2);
							b[b.Length - 2] = 0;
							
							b[b.Length - 1] = 0; //add nulnul
							return b;
						}
					}
					
					public bool MoveNext()
					{
						if (m_NotEmpty)
						{
							if (m_sPos < 0)
							{
								m_sPos = 0;
								
								m_ePos = - 1;
							}
							else
							{
								m_sPos = m_ePos + 1;
							}
							if (m_bytes.Length < m_sPos + 1)
							{
								throw (new InvalidCastException("Malformed PIDL"));
							}
							int cb = m_bytes[m_sPos] + m_bytes[m_sPos + 1] * 256;
							if (cb == 0)
							{
								return false; //have passed all back
							}
							else
							{
								m_ePos += cb;
							}
						}
						else
						{
							m_sPos = 0;
							
							m_ePos = 0;
							return false; //in this case, we have exhausted the list of 0 ITEMIDs
						}
						return true;
					}
					
					public void Reset()
					{
						m_sPos = - 1;
						
						m_ePos = - 1;
					}
				}
				#endregion
				
			}
			#endregion
			
		}
	}
