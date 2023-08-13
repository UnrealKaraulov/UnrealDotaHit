using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using DotaHIT.Core;
using DotaHIT.Jass;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Jass.Native.Types;

namespace DotaHIT
{
    public class FloatingListForm : Form
    {
        internal ItemToolTipForm toolTip = null;
        internal object toolTipItem = null;        

        internal int mbX = -1;
        internal int mbY = -1; 

        internal Button captionButton = null;
        internal bool bound = true;

        internal MainForm mainOwner = null;

        internal int fullHeight = 0;
        internal int minHeight = 0;
        internal int width = 0;
        internal bool contentMinimized = false;
        protected bool shortMode = false;

        internal int shiftX = 0;
        internal int shiftY = 0;

        protected bool leftSide = true;        

        protected ListView base_itemsLV = null;
        protected System.Windows.Forms.Timer base_listMinMaxTimer = null;
        protected System.Windows.Forms.Timer base_toolTipTimer = null;

        public FloatingListForm()
        {            
        }          

        internal void baseInit()
        {            
            base_listMinMaxTimer.Tick += new EventHandler(listMinMaxTimer_Tick);

            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
        }

        public void Minimize()
        {
            if (contentMinimized)
                return;
            else
                ChangeState();
        }

        public void Minimize(bool instant)
        {
            if (contentMinimized)
                return;
            else
                ChangeState(instant);
        }

        public void Maximize()
        {
            if (!contentMinimized)
                return;
            else
                ChangeState();
        }

        public void Maximize(bool instant)
        {
            if (!contentMinimized)
                return;
            else
                ChangeState(instant);
        }

        public Button CaptionButton
        {
            get
            {
                return captionButton;
            }
            set
            {
                captionButton = value;
                captionButton.Click += new System.EventHandler(this.captionB_Click);
                captionButton.MouseDown += new MouseEventHandler(captionButton_MouseDown);
                captionButton.MouseMove += new MouseEventHandler(captionButton_MouseMove);
                captionButton.MouseUp += new MouseEventHandler(captionButton_MouseUp);
            }
        }

        public int ItemCount
        {
            get
            {
                return base_itemsLV.Items.Count;
            }
        }

        protected virtual void captionButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (bound) return;

            if (e.Button == MouseButtons.Left)
            {
                mbX = MousePosition.X - this.Location.X;
                mbY = MousePosition.Y - this.Location.Y;
            }
            else
                mbX = mbY = -1;
        }

        void captionButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (bound) return;

            if (mbX != -1 && mbY != -1)            
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    this.SetDesktopLocation(MousePosition.X - mbX, MousePosition.Y - mbY);                        
        }

        protected virtual void captionButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (bound) return;

            if (e.Button == MouseButtons.Left)
                mbX = mbY = -1;
        }

        protected void captionB_Click(object sender, EventArgs e)
        {            
            ChangeState();
        }

        internal virtual void ChangeState() 
        {
            if (!bound) return;

            contentMinimized = !contentMinimized;
            base_itemsLV.Visible = false;
            base_listMinMaxTimer.Start();
        }

        internal virtual void ChangeState(bool instant) 
        {
            if (!bound) return;

            contentMinimized = !contentMinimized;

            int jumpDistance = fullHeight - minHeight;

            if (contentMinimized)
            {
                shiftY -= jumpDistance;
                this.SetBounds(this.Left, this.Top + jumpDistance, this.Width, minHeight);                
            }
            else
            {
                shiftY += jumpDistance;
                this.SetBounds(this.Left, this.Top - jumpDistance, this.Width, fullHeight);
            }            
        }

        public void SetParent(MainForm parentForm)
        {
            mainOwner = parentForm;
            this.Owner = parentForm;

            Point bp = parentForm.GetBindPoint(leftSide);            
            parentForm.PreMove += new MainForm.PreMoveEvent(parentForm_PreMove);

            bp.Y = bp.Y - this.Height;

            if (leftSide == false)
                bp.X = bp.X - this.Width;

            this.SetDesktopLocation(bp.X, bp.Y);

            shiftX = parentForm.Location.X - bp.X;
            shiftY = parentForm.Location.Y - bp.Y;            
        }

        public void RefreshBound()
        {
            this.Owner = mainOwner;
            Point bp = (Owner as MainForm).GetBindPoint(leftSide);            

            bp.Y = bp.Y - this.Height;

            if (leftSide == false)
                bp.X = bp.X - this.Width;

            this.SetDesktopLocation(bp.X, bp.Y);

            shiftX = Owner.Location.X - bp.X;
            shiftY = Owner.Location.Y - bp.Y;
        }

        void parentForm_PreMove(object sender, Point point)
        {
            if (bound)
                SetPos(point);                               
        }        

        private void SetPos(Point point)
        {        
            this.SetDesktopLocation(point.X - shiftX, point.Y - shiftY);            
        }

        public void SetOffset(int x, int y)
        {
            shiftX += x;
            shiftY += y;
            if (bound)
                this.SetDesktopLocation(this.DesktopLocation.X - x, this.DesktopLocation.Y - y);
        }

        public void Display()
        {            
            this.Show();            
        }       

        public void Terminate()
        {
            this.Close();            
        }

        public void listMinMaxTimer_Tick(object sender, EventArgs e)
        {
            int minmaxStep = 18;//18;            

            int top = 0;
            int height = 0;

            if (contentMinimized == false)
            {
                if (this.Height + minmaxStep >= fullHeight)
                {
                    height = fullHeight;
                    top = this.Top - (fullHeight - this.Height);

                    base_listMinMaxTimer.Stop(); base_itemsLV.Visible = true;
                }
                else
                {
                    height = this.Height + minmaxStep;
                    top = this.Top - minmaxStep;
                }
            }
            else
            {
                if (this.Height - minmaxStep <= minHeight)
                {
                    top = this.Top + (this.Height - minHeight);
                    height = minHeight;

                    base_listMinMaxTimer.Stop(); base_itemsLV.Visible = true;
                }
                else
                {
                    top = this.Top + minmaxStep;
                    height = this.Height - minmaxStep;
                }
            }

            shiftY += this.Location.Y - top;
            this.SetBounds(this.Left, top, this.Width, height);
        }

        public bool Bound
        {
            get
            {
                return bound;
            }
            set
            {
                bound = value;
                if (bound)
                {
                    captionButton.FlatAppearance.MouseDownBackColor = Color.Empty;
                    this.Width = width;
                    this.Height = contentMinimized ? minHeight : fullHeight;
                    RefreshBound();
                }
                else
                {
                    //this.Owner = null;                    
                    captionButton.FlatAppearance.MouseDownBackColor = Color.Black;
                }
                
                this.UpdateStyles();
            }
        }        

        protected override CreateParams CreateParams
        {
            get
            {
                if (bound)
                    return base.CreateParams;
                else
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style = cp.Style | Win32Msg.WS_THICKFRAME;
                    return cp;
                }
            }
        }

        protected void CloseToolTip()
        {
            if (toolTip != null)
            {                
                toolTip.Close();
                toolTip = null;
                toolTipItem = null;
            }

            if (base_toolTipTimer != null) 
                base_toolTipTimer.Stop();
        }        

        public virtual void WriteConfig()
        {
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                mainOwner.CopyToolTipText();
            else
            if (e.Control && e.KeyCode == Keys.D)
                mainOwner.ToggleDetailedToolTip();
        }
    }
}
