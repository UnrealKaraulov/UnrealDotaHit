namespace DotaHIT
{
    partial class UnitsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.captionB = new System.Windows.Forms.Button();
            this.closeB = new System.Windows.Forms.Button();
            this.unitsLV = new System.Windows.Forms.ListView();
            this.unitsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitColumn = new System.Windows.Forms.ColumnHeader();
            this.unitsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // captionB
            // 
            this.captionB.BackColor = System.Drawing.Color.Black;
            this.captionB.Dock = System.Windows.Forms.DockStyle.Top;
            this.captionB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.captionB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.captionB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.captionB.ForeColor = System.Drawing.Color.Gold;
            this.captionB.Location = new System.Drawing.Point(0, 0);
            this.captionB.Margin = new System.Windows.Forms.Padding(0);
            this.captionB.Name = "captionB";
            this.captionB.Size = new System.Drawing.Size(270, 26);
            this.captionB.TabIndex = 13;
            this.captionB.Text = "Player\'s Units";
            this.captionB.UseVisualStyleBackColor = false;
            this.captionB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseDown);
            this.captionB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseMove);
            this.captionB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.captionB_MouseUp);
            // 
            // closeB
            // 
            this.closeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeB.BackColor = System.Drawing.Color.Black;
            this.closeB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeB.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.closeB.ForeColor = System.Drawing.Color.Silver;
            this.closeB.Location = new System.Drawing.Point(243, 3);
            this.closeB.Name = "closeB";
            this.closeB.Size = new System.Drawing.Size(23, 19);
            this.closeB.TabIndex = 15;
            this.closeB.Text = "x";
            this.closeB.UseVisualStyleBackColor = false;
            this.closeB.Click += new System.EventHandler(this.closeB_Click);
            // 
            // unitsLV
            // 
            this.unitsLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.unitsLV.BackColor = System.Drawing.Color.Black;
            this.unitsLV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.unitsLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.unitColumn});
            this.unitsLV.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.unitsLV.ForeColor = System.Drawing.Color.White;
            this.unitsLV.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.unitsLV.HideSelection = false;
            this.unitsLV.Location = new System.Drawing.Point(3, 29);
            this.unitsLV.Name = "unitsLV";
            this.unitsLV.OwnerDraw = true;
            this.unitsLV.Size = new System.Drawing.Size(264, 238);
            this.unitsLV.TabIndex = 16;
            this.unitsLV.UseCompatibleStateImageBehavior = false;
            this.unitsLV.View = System.Windows.Forms.View.List;
            this.unitsLV.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.unitsLV_DrawItem);
            this.unitsLV.MouseUp += new System.Windows.Forms.MouseEventHandler(this.unitsLV_MouseUp);
            this.unitsLV.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.unitsLV_ItemSelectionChanged);
            this.unitsLV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.unitsLV_MouseDown);
            // 
            // unitsContextMenuStrip
            // 
            this.unitsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.unitsContextMenuStrip.Name = "unitContextMenuStrip";
            this.unitsContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.unitsContextMenuStrip.Size = new System.Drawing.Size(114, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // unitColumn
            // 
            this.unitColumn.Text = "";
            this.unitColumn.Width = 0;
            // 
            // UnitsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(270, 270);
            this.ControlBox = false;
            this.Controls.Add(this.unitsLV);
            this.Controls.Add(this.closeB);
            this.Controls.Add(this.captionB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnitsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "gamestate";
            this.VisibleChanged += new System.EventHandler(this.UnitsForm_VisibleChanged);
            this.unitsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button captionB;
        private System.Windows.Forms.Button closeB;
        private System.Windows.Forms.ListView unitsLV;
        private System.Windows.Forms.ContextMenuStrip unitsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader unitColumn;
    }
}