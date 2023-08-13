using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System;
using System.Text;
using System.Runtime.InteropServices;
//using ExpTreeLib.ShellDll;


namespace ExpTreeLib
{
	public class TVDragWrapper : ShellDll.IDropTarget
	{
		
		#region "   Private Fields"
		private Control m_View;
		private int m_Original_Effect; //Save it
		private int m_OriginalRefCount; //Set in DragEnter, used in DragDrop
		private IntPtr m_DragDataObj; //Saved on DragEnter for use in DragOver
		private ExpTreeLib.ShellDll.IDropTarget m_LastTarget; //Of most recent Folder dragged over
		private object m_LastNode; //Most recent node dragged over
		private ArrayList m_DropList; //CShItems of Items dragged/dropped
		private CProcDataObject m_MyDataObject; //Does parsing of dragged IDataObject
		#endregion
		
		#region "   Public Events"
		public delegate void ShDragEnterEventHandler(ArrayList DragItemList, IntPtr pDataObj, int grfKeyState, int pdwEffect);
		private ShDragEnterEventHandler ShDragEnterEvent;
		
		public event ShDragEnterEventHandler ShDragEnter
		{
			add
			{
				ShDragEnterEvent = (ShDragEnterEventHandler) System.Delegate.Combine(ShDragEnterEvent, value);
			}
			remove
			{
				ShDragEnterEvent = (ShDragEnterEventHandler) System.Delegate.Remove(ShDragEnterEvent, value);
			}
		}
		
		
		public delegate void ShDragOverEventHandler(TreeNode Node, System.Drawing.Point ClientPoint, int grfKeyState, int pdwEffect);
		private ShDragOverEventHandler ShDragOverEvent;
		
		public event ShDragOverEventHandler ShDragOver
		{
			add
			{
				ShDragOverEvent = (ShDragOverEventHandler) System.Delegate.Combine(ShDragOverEvent, value);
			}
			remove
			{
				ShDragOverEvent = (ShDragOverEventHandler) System.Delegate.Remove(ShDragOverEvent, value);
			}
		}
		
		public delegate void ShDragLeaveEventHandler();
		private ShDragLeaveEventHandler ShDragLeaveEvent;
		
		public event ShDragLeaveEventHandler ShDragLeave
		{
			add
			{
				ShDragLeaveEvent = (ShDragLeaveEventHandler) System.Delegate.Combine(ShDragLeaveEvent, value);
			}
			remove
			{
				ShDragLeaveEvent = (ShDragLeaveEventHandler) System.Delegate.Remove(ShDragLeaveEvent, value);
			}
		}
		
		
		public delegate void ShDragDropEventHandler(ArrayList DragItemList, object Node, int grfKeyState, int pdwEffect);
		private ShDragDropEventHandler ShDragDropEvent;
		
		public event ShDragDropEventHandler ShDragDrop
		{
			add
			{
				ShDragDropEvent = (ShDragDropEventHandler) System.Delegate.Combine(ShDragDropEvent, value);
			}
			remove
			{
				ShDragDropEvent = (ShDragDropEventHandler) System.Delegate.Remove(ShDragDropEvent, value);
			}
		}
		
		
		#endregion
		
		#region "   Constructor"
		public TVDragWrapper(TreeView ctl)
		{
			m_View = ctl;
		}
		#endregion
		
		private void ResetPrevTarget()
		{
			if (m_LastTarget != null)
			{
				//ShowCnt("RSP Prior to LT.DragLeave")
				int hr = m_LastTarget.DragLeave();
				//ShowCnt("RSP After LT.DragLeave")
				Marshal.ReleaseComObject(m_LastTarget);
				//ShowCnt("RSP After Release of LT")
				m_LastTarget = null;
				m_LastNode = null;
			}
		}
		
		public int DragEnter(IntPtr pDataObj, int grfKeyState, Point pt, ref int pdwEffect)
		{
			Debug.WriteLine("In DragEnter: Effect = " + pdwEffect + " Keystate = " + grfKeyState);
			m_Original_Effect = pdwEffect;
			m_DragDataObj = pDataObj;
			m_OriginalRefCount = Marshal.AddRef(m_DragDataObj); //note: includes our count
			Debug.WriteLine("DragEnter: pDataObj RefCnt = " + m_OriginalRefCount);
			
			m_MyDataObject = new CProcDataObject(pDataObj);
			
			if (m_MyDataObject.IsValid)
			{
				m_DropList = m_MyDataObject.DragList;
				if (ShDragEnterEvent != null)
					ShDragEnterEvent(m_DropList, pDataObj, grfKeyState, pdwEffect);
			}
			else
			{
				pdwEffect = (int)System.Windows.Forms.DragDropEffects.None;
			}
			return 0;
			
		}
		
		//Private Sub ShowCnt(ByVal S As String)
		//    If Not IsNothing(m_DragData) Then
		//        Debug.WriteLine(S & " RefCnt = " & Marshal.AddRef(m_DragData) - 1)
		//        Marshal.Release(m_DragData)
		//    End If
		//End Sub
		
		public int DragOver(int grfKeyState, Point pt, ref int pdwEffect)
		{
			//Debug.WriteLine("In DragOver: Effect = " & pdwEffect & " Keystate = " & grfKeyState)
			
			TreeNode tn;
			System.Drawing.Point ptClient = m_View.PointToClient(new System.Drawing.Point(pt.X, pt.Y));
			tn = ((TreeView) m_View).GetNodeAt(ptClient);
			if (tn == null) //not over a TreeNode
			{
				ResetPrevTarget();
			}
			else //currently over Treenode
			{
				if (m_LastNode != null) //not the first, check if same
				{
					if (tn == m_LastNode)
					{
						//Debug.WriteLine("DragOver: Same node")
						return 0; //all set up anyhow
					}
					else
					{
						ResetPrevTarget();
						m_LastNode = tn;
					}
				}
				else //is the first
				{
					ResetPrevTarget(); //just in case
					m_LastNode = tn; //save current node
				}
				
				//Drag is now over a new node with new capabilities

                CShItem CSI = tn.Tag as CShItem;
				if (CSI.IsDropTarget)
				{
					m_LastTarget = CSI.GetDropTargetOf(m_View) as ShellDll.IDropTarget;
					if (m_LastTarget != null)
					{
						pdwEffect = m_Original_Effect;
						//ShowCnt("Prior to LT.DragEnter")
						int res = m_LastTarget.DragEnter(m_DragDataObj, grfKeyState, pt, ref pdwEffect);
						if (res == 0)
						{
							//ShowCnt("Prior to LT.DragOver")
							res = m_LastTarget.DragOver(grfKeyState, pt, ref pdwEffect);
							//ShowCnt("After LT.DragOver")
						}
						if (res != 0)
						{
							Marshal.ThrowExceptionForHR(res);
						}
					}
					else
					{
						pdwEffect = 0; //couldn't get IDropTarget, so report effect None
					}
				}
				else
				{
					pdwEffect = 0; //CSI not a drop target, so report effect None
				}
				if (ShDragOverEvent != null)
					ShDragOverEvent(tn, ptClient, grfKeyState, pdwEffect);
			}
			return 0;
		}
		
		public int DragLeave()
		{
			//Debug.WriteLine("In DragLeave")
			m_Original_Effect = 0;
			ResetPrevTarget();
			int cnt = Marshal.Release(m_DragDataObj);
			Debug.WriteLine("DragLeave: cnt = " + cnt);
			m_DragDataObj = IntPtr.Zero;
			m_OriginalRefCount = 0; //just in case
			m_MyDataObject = null;
			if (ShDragLeaveEvent != null)
				ShDragLeaveEvent();
			return 0;
		}
		
		public int DragDrop(IntPtr pDataObj, int grfKeyState, Point pt, ref int pdwEffect)
		{
			//Debug.WriteLine("In DragDrop: Effect = " & pdwEffect & " Keystate = " & grfKeyState)
			int res;
			if (m_LastTarget != null)
			{
				res = m_LastTarget.DragDrop(pDataObj, grfKeyState, pt, ref pdwEffect);
				//version 21 change
				if (res != 0 && res != 1)
				{
					Debug.WriteLine("Error in dropping on DropTarget. res = " + Convert.ToString(res,16));
				} //No error on drop
				// it is quite possible that the actual Drop has not completed.
				// in fact it could be Canceled with nothing happening.
				// All we are going to do is hope for the best
				// The documented norm for Optimized Moves is pdwEffect=None, so leave it
				if (ShDragDropEvent != null)
					ShDragDropEvent(m_DropList, m_LastNode, grfKeyState, pdwEffect);
			}
			ResetPrevTarget();
			int cnt = Marshal.Release(m_DragDataObj); //get rid of cnt added in DragEnter
			m_DragDataObj = IntPtr.Zero;
			return 0;
		}
	}
	
}
