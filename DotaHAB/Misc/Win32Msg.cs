using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DotaHIT
{
    class Win32Msg
    {
        public const int WM_CREATE = 0x0001;
        public const int WS_THICKFRAME = 0x00040000;

        /*[StructLayout(LayoutKind.Sequential,Pack=1)]
        public struct CREATESTRUCT
        {
            public IntPtr lpCreateParams;
            public IntPtr hInstance;
            public IntPtr hMenu;
            public IntPtr hwndParent;
            public short cy;
            public short cx;
            public short y;
            public short x;
            public int style;
            public string lpszName;
            public string lpszClass;
            public uint dwExStyle;
        }*/

      /*  public const int WM_SETREDRAW = 0x000B;        

        private const int WM_USER = 0x400;

        private const int EM_GETEVENTMASK = (WM_USER + 59);

        private const int EM_SETEVENTMASK = (WM_USER + 69);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SendMessage(IntPtr handle, int wMsg, int wParam, int lparam);

        public static void SetRedraw(Control c, bool redraw, ref int eventMask)
        {
            if (redraw)
            {                
                // turn on events
                SendMessage(c.Handle, EM_SETEVENTMASK, 0, eventMask);
                // turn on redrawing
                SendMessage(c.Handle, WM_SETREDRAW, 1, 0);
            }
            else
            {
                // Stop redrawing
                SendMessage(c.Handle, WM_SETREDRAW, 0, 0);
                // Stop sending of events
                eventMask = SendMessage(c.Handle, EM_GETEVENTMASK, 0, 0);
            }
        }
        */
        /*[DllImport("user32.dll", EntryPoint = "LockWindowUpdate", SetLastError = true,
ExactSpelling = true, CharSet = CharSet.Auto,
CallingConvention = CallingConvention.StdCall)]
        public static extern long LockWindow(IntPtr Handle);*/
    }
}
