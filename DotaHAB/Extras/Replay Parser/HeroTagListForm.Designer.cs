namespace DotaHIT.Extras
{
    partial class HeroTagListForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.heroTagGridView = new System.Windows.Forms.DataGridView();
            this.heroImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.heroTagColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.heroClassColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.heroNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.heroTagGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // heroTagGridView
            // 
            this.heroTagGridView.AllowUserToAddRows = false;
            this.heroTagGridView.AllowUserToDeleteRows = false;
            this.heroTagGridView.AllowUserToResizeColumns = false;
            this.heroTagGridView.AllowUserToResizeRows = false;
            this.heroTagGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.heroTagGridView.BackgroundColor = System.Drawing.Color.GhostWhite;
            this.heroTagGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.heroTagGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.heroTagGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.heroTagGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.heroImageColumn,
            this.heroTagColumn,
            this.heroClassColumn,
            this.heroNameColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.heroTagGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.heroTagGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.heroTagGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.heroTagGridView.EnableHeadersVisualStyles = false;
            this.heroTagGridView.Location = new System.Drawing.Point(0, 0);
            this.heroTagGridView.MultiSelect = false;
            this.heroTagGridView.Name = "heroTagGridView";
            this.heroTagGridView.RowHeadersVisible = false;
            this.heroTagGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.heroTagGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.heroTagGridView.Size = new System.Drawing.Size(510, 238);
            this.heroTagGridView.TabIndex = 29;
            this.heroTagGridView.VirtualMode = true;
            this.heroTagGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.heroTagGridView_CellValueNeeded);
            this.heroTagGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.heroTagGridView_CellValuePushed);
            // 
            // heroImageColumn
            // 
            this.heroImageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.heroImageColumn.FillWeight = 60F;
            this.heroImageColumn.HeaderText = "Hero";
            this.heroImageColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.heroImageColumn.Name = "heroImageColumn";
            this.heroImageColumn.ReadOnly = true;
            this.heroImageColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.heroImageColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.heroImageColumn.Width = 28;
            // 
            // heroTagColumn
            // 
            this.heroTagColumn.HeaderText = "Tag";
            this.heroTagColumn.Name = "heroTagColumn";
            this.heroTagColumn.Width = 68;
            // 
            // heroClassColumn
            // 
            this.heroClassColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.heroClassColumn.FillWeight = 120F;
            this.heroClassColumn.HeaderText = "Class";
            this.heroClassColumn.Name = "heroClassColumn";
            // 
            // heroNameColumn
            // 
            this.heroNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.heroNameColumn.HeaderText = "Name";
            this.heroNameColumn.Name = "heroNameColumn";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(510, 33);
            this.label1.TabIndex = 30;
            this.label1.Text = "The data you enter here is automatically saved in the replay \r\nexport configurati" +
                "on file (default is \'dhrexport.cfg\')";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HeroTagListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 271);
            this.Controls.Add(this.heroTagGridView);
            this.Controls.Add(this.label1);
            this.Name = "HeroTagListForm";
            this.ShowIcon = false;
            this.Text = "Hero<->Tag Binding List Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HeroTagListForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.heroTagGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView heroTagGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewImageColumn heroImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroTagColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroClassColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroNameColumn;        

    }
}