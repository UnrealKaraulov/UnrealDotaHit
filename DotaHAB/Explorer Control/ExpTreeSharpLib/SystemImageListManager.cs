using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using DotaHIT.Core.Resources;
using System.Collections.Generic;
//using ExpTreeLib.ShellDll;

//Customisation:
//GetIcon will retrieve a small image:
//Old Method:
//    Public Shared Function GetIcon(ByVal Index As Integer) As Icon
//        Initializer()
//        Dim icon As Icon = Nothing
//        Dim hIcon As IntPtr = IntPtr.Zero
//        hIcon = ImageList_GetIcon(m_lgImgList, Index, 0)
//        If Not IsNothing(hIcon) Then
//            icon = System.Drawing.Icon.FromHandle(hIcon)
//        End If
//        Return icon
//    End Function


namespace ExpTreeLib
{
	public class SystemImageListManager
	{
		
		
		#region "       ImageList Related Constants"
		// For ImageList manipulation
		private const int LVM_FIRST = 0x1000;
		private const int LVM_SETIMAGELIST = (LVM_FIRST + 3);
		
		private const int LVSIL_NORMAL = 0;
		private const int LVSIL_SMALL = 1;
		private const int LVSIL_STATE = 2;
		
		private const int TV_FIRST = 0x1100;
		private const int TVM_SETIMAGELIST = (TV_FIRST + 9);
		
		private const int TVSIL_NORMAL = 0;
		private const int TVSIL_STATE = 2;
		#endregion
		
		#region "   Private Fields"
		private static bool m_Initialized = false;
		private static IntPtr m_smImgList = IntPtr.Zero; //Handle to System Small ImageList
		private static IntPtr m_lgImgList = IntPtr.Zero; //Handle to System Large ImageList
		private static Hashtable m_Table = new Hashtable(128);
		
		private static Mutex m_Mutex = new Mutex();
		#endregion
		
		#region "   New"
		/// <summary>
		/// Summary of Initializer.
		/// </summary>
		///
		private static void Initializer()
		{
			if (m_Initialized)
			{
				return;
			}
			
			int dwFlag = (int)(ExpTreeLib.ShellDll.SHGFI.USEFILEATTRIBUTES | ExpTreeLib.ShellDll.SHGFI.SYSICONINDEX | ExpTreeLib.ShellDll.SHGFI.SMALLICON);
			ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
			m_smImgList = ExpTreeLib.ShellDll.SHGetFileInfo(".txt", ExpTreeLib.ShellDll.FILE_ATTRIBUTE_NORMAL, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwFlag);
			Debug.Assert((! m_smImgList.Equals(IntPtr.Zero)), "Failed to create Image Small ImageList");
			if (m_smImgList.Equals(IntPtr.Zero))
			{
				throw (new Exception("Failed to create Small ImageList"));
			}
			
			/*dwFlag = (int)(ExpTreeLib.ShellDll.SHGFI.USEFILEATTRIBUTES | ExpTreeLib.ShellDll.SHGFI.SYSICONINDEX | ExpTreeLib.ShellDll.SHGFI.LARGEICON);
			m_lgImgList = ExpTreeLib.ShellDll.SHGetFileInfo(".txt", ExpTreeLib.ShellDll.FILE_ATTRIBUTE_NORMAL, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwFlag);
			Debug.Assert((! m_lgImgList.Equals(IntPtr.Zero)), "Failed to create Image Small ImageList");
			if (m_lgImgList.Equals(IntPtr.Zero))
			{
				throw (new Exception("Failed to create Large ImageList"));
			}*/
			
			m_Initialized = true;
		}
		#endregion
		
		#region "   Public Properties"
		public static IntPtr hSmallImageList
		{
			get
			{
				return m_smImgList;
			}
		}
		public static IntPtr hLargeImageList
		{
			get
			{
				return m_lgImgList;
			}
		}
		#endregion
		
		#region "   Public Methods"
		#region "       GetIconIndex"
		private static int mCnt;
		private static int bCnt;

        static Dictionary<int, int> dcAttrIndexCache = new Dictionary<int, int>();
		/// <summary>
		/// Summary of GetIconIndex.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="GetOpenIcon"></param>
		///
		public static int GetIconIndex(CShItem item, bool GetOpenIcon, bool GetSelectedIcon)
		{
            int rVal; //The returned Index
            if (dcAttrIndexCache.TryGetValue((int)item.Attributes, out rVal))
                return rVal;

			Initializer();            
			bool HasOverlay = false; //true if it's an overlay
			//int rVal; //The returned Index
			
			int dwflag = (int)(ExpTreeLib.ShellDll.SHGFI.SYSICONINDEX | ExpTreeLib.ShellDll.SHGFI.PIDL | ExpTreeLib.ShellDll.SHGFI.ICON);
			int dwAttr = 0;
			//build Key into HashTable for this Item
            int Key = ! GetOpenIcon ? item.IconIndexNormal * 256 : item.IconIndexOpen * 256;          
			CShItem with_1 = item;            
			if (with_1.IsLink)
			{                
				Key = Key | 1;
				dwflag = dwflag | (int)ExpTreeLib.ShellDll.SHGFI.LINKOVERLAY;
				HasOverlay = true;
			}
			if (with_1.IsShared)
			{                
				Key = Key | 2;
                dwflag = dwflag | (int)ExpTreeLib.ShellDll.SHGFI.ADDOVERLAYS;
				HasOverlay = true;                
			}
			if (GetSelectedIcon)
			{                
				Key = Key | 4;
                dwflag = dwflag | (int)ExpTreeLib.ShellDll.SHGFI.SELECTED;
				HasOverlay = true; //not really an overlay, but handled the same                
			}
			if (m_Table.ContainsKey(Key))
			{                
				rVal = (int)m_Table[Key];
				mCnt++;
			}
			else if (! HasOverlay) //for non-overlay icons, we already have
			{                
				rVal = Key / 256; //  the right index -- put in table
				m_Table[Key] = rVal;
				bCnt++;                
			}
			else //don't have iconindex for an overlay, get it.
            {                
				//This is the tricky part -- add overlaid Icon to systemimagelist
				//  use of SmallImageList from Calum McLellan
				//ExpTreeLib.ShellDll.SHFILEINFO shfi = new ExpTreeLib.ShellDll.SHFILEINFO();
				ExpTreeLib.ShellDll.SHFILEINFO shfi_small = new ExpTreeLib.ShellDll.SHFILEINFO();
				//IntPtr HR;
				IntPtr HR_SMALL;
				if (with_1.IsFileSystem && ! with_1.IsDisk && ! with_1.IsFolder)
				{
					dwflag = dwflag | (int)ExpTreeLib.ShellDll.SHGFI.USEFILEATTRIBUTES;
					dwAttr = ExpTreeLib.ShellDll.FILE_ATTRIBUTE_NORMAL;
				}
				//HR = ExpTreeLib.ShellDll.SHGetFileInfo(with_1.PIDL, dwAttr, ref shfi, ExpTreeLib.ShellDll.cbFileInfo, dwflag);                
				HR_SMALL = ExpTreeLib.ShellDll.SHGetFileInfo(with_1.PIDL, dwAttr, ref shfi_small, ExpTreeLib.ShellDll.cbFileInfo, dwflag | (int)ExpTreeLib.ShellDll.SHGFI.SMALLICON);                
				m_Mutex.WaitOne();                
				rVal = ExpTreeLib.ShellDll.ImageList_ReplaceIcon(m_smImgList, - 1, shfi_small.hIcon);                
				Debug.Assert(rVal > - 1, "Failed to add overlaid small icon");
				/*int rVal2;
				rVal2 = ExpTreeLib.ShellDll.ImageList_ReplaceIcon(m_lgImgList, - 1, shfi.hIcon);
				Debug.Assert(rVal2 > - 1, "Failed to add overlaid large icon");
				Debug.Assert(rVal == rVal2, "Small & Large IconIndices are Different");*/
				m_Mutex.ReleaseMutex();
				//ExpTreeLib.ShellDll.DestroyIcon(shfi.hIcon);
				ExpTreeLib.ShellDll.DestroyIcon(shfi_small.hIcon);
				if (rVal < 0/* || rVal != rVal2*/)
				{
					throw (new ApplicationException("Failed to add Icon for " + item.DisplayName));
				}
				m_Table[Key] = rVal;                
			}

            if (item.IsFileSystem && item.IsFolder)
            {
                dcAttrIndexCache[(int)item.Attributes] = rVal;
            }

			return rVal;
		}
		//Private Shared Sub DebugShowImages(ByVal useSmall As Boolean, ByVal iFrom As Integer, ByVal iTo As Integer)
		//    Dim RightIcon As Icon = GetIcon(iFrom, Not useSmall)
		//    Dim rightIndex As Integer = iFrom
		//    Do While iFrom <= iTo
		//        Dim ico As Icon = GetIcon(iFrom, useSmall)
		//        Dim fShow As New frmDebugShowImage(rightIndex, RightIcon, ico, IIf(useSmall, "Small ImageList", "Large ImageList"), iFrom)
		//        fShow.ShowDialog()
		//        fShow.Dispose()
		//        iFrom += 1
		//    Loop
		//End Sub
		#endregion
		
		#region "       GetIcon"
		/// <summary>
		/// Returns a GDI+ copy of the icon from the ImageList
		/// at the specified index.</summary>
		/// <param name="Index">The IconIndex of the desired Icon</param>
		/// <param name="smallIcon">Optional, default = False. If True, return the
		///   icon from the Small ImageList rather than the Large.</param>
		/// <returns>The specified Icon or Nothing</returns>
		public static Icon GetIcon(int Index, bool smallIcon)
		{
			Initializer();
			Icon icon = null;
			IntPtr hIcon = IntPtr.Zero;
			//Customisation to return a small image
			if (smallIcon)
			{
				hIcon = ExpTreeLib.ShellDll.ImageList_GetIcon(m_smImgList, Index, 0);
			}
			else
			{
				hIcon = ExpTreeLib.ShellDll.ImageList_GetIcon(m_lgImgList, Index, 0);
			}
			if (hIcon != null)
			{
				icon = System.Drawing.Icon.FromHandle(hIcon);
			}
			return icon;
		}
		#endregion
		
		#region "       GetDeskTopIconIndex"
		// No longer used. Retained for information purposes
		// <summary>
		// public int GetDeskTopIconIndex()
		// Returns the Icon Index for the Desktop. This is not
		//		available using the normal methods and the image
		//		itself is not placed into the ImageList unless this
		//		call is made.
		// </summary>
		// <returns>Returns the Icon Index for the Desktop
		// </returns>
		//Public Shared Function GetDeskTopIconIndex() As Integer
		//    Dim ppidl As IntPtr
		//    Dim hDum As Integer = 0
		//    Dim rVal As Integer
		
		//    rVal = SHGetSpecialFolderLocation(hDum, CSIDL.DESKTOP, ppidl)
		//    If rVal = 0 Then
		//        Dim dwFlags As Integer = SHGFI.SYSICONINDEX _
		//                                 Or SHGFI.PIDL
		//        Dim dwAttr As Integer = 0
		
		//        Dim shfi As SHFILEINFO = New SHFILEINFO()
		//        Dim resp As IntPtr
		//        resp = SHGetFileInfo(ppidl, _
		//                                dwAttr, _
		//                                shfi, _
		//                                cbFileInfo, _
		//                                dwFlags)
		//        Marshal.FreeCoTaskMem(ppidl)  ' free the pointer
		//        If resp.Equals(IntPtr.Zero) Then
		//            Debug.Assert(Not resp.Equals(IntPtr.Zero), "Failed to get icon index")
		//            rVal = -1  'Failed to get IconIndex
		//        Else
		//            rVal = shfi.iIcon  'got it, return it
		//        End If
		//    Else        'failed to get DesktopLocation
		//        rVal = -1
		//    End If
		//    If rVal > -1 Then
		//        Return rVal
		//    Else
		//        Throw New ApplicationException("Failed to get Desktop Icon Index")
		//        Return -1
		//    End If
		//End Function
		#endregion
		
		#region "       SetListViewImageList"
		//    <summary>
		//    Associates a SysImageList with a ListView control
		//    </summary>
		//    <param name="listView">ListView control to associate ImageList with</param>
		//    <param name="forLargeIcons">True=Set Large Icon List
		//                   False=Set Small Icon List</param>
		//    <param name="forStateImages">Whether to add ImageList as StateImageList</param>
		/// <summary>
		/// Summary of SetListViewImageList.
		/// </summary>
		/// <param name="listView"></param>
		/// <param name="forLargeIcons"></param>
		/// <param name="forStateImages"></param>
		///
		public static void SetListViewImageList(ListView listView, bool forLargeIcons, bool forStateImages)
		{
			
			Initializer();
			int wParam = 0;
			IntPtr HImageList = m_lgImgList;
			if (! forLargeIcons)
			{
				wParam = LVSIL_SMALL;
				HImageList = m_smImgList;
			}
			if (forStateImages)
			{
				wParam = LVSIL_STATE;
			}
			ExpTreeLib.ShellDll.SendMessage(listView.Handle, LVM_SETIMAGELIST, wParam, HImageList);
		}
		#endregion
		
		#region "       SetTreeViewImageList"
		///// <summary>
		///// Associates a SysImageList with a TreeView control
		///// </summary>
		///// <param name="treeView">TreeView control to associated ImageList with</param>
		///// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
		/// <summary>
		/// Summary of SetTreeViewImageList.
		/// </summary>
		/// <param name="treeView"></param>
		/// <param name="forStateImages"></param>
		///
		public static void SetTreeViewImageList(TreeView treeView, bool forStateImages)
		{
			
			Initializer();
			int wParam = 0;
			if (forStateImages)
			{
				wParam = LVSIL_STATE;
			}
			int HR;
			HR = ExpTreeLib.ShellDll.SendMessage(treeView.Handle, TVM_SETIMAGELIST, wParam, m_smImgList);
		}
		
		#endregion
		
		#endregion
	}
	
}
