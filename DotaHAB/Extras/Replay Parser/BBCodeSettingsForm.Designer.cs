namespace DotaHIT.Extras
{
    partial class BBCodeSettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.bbCodeGridView = new System.Windows.Forms.DataGridView();
            this.heroClassColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.heroNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.bbCodeGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(0, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(597, 31);
            this.label1.TabIndex = 30;
            this.label1.Text = "The data you enter here is automatically saved in the replay export configuration" +
                " file (default is \'dhrexport.cfg\')\r\nNote: BBBCode settings are ignored when disp" +
                "laying export preview";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bbCodeGridView
            // 
            this.bbCodeGridView.AllowUserToAddRows = false;
            this.bbCodeGridView.AllowUserToDeleteRows = false;
            this.bbCodeGridView.AllowUserToResizeColumns = false;
            this.bbCodeGridView.AllowUserToResizeRows = false;
            this.bbCodeGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.bbCodeGridView.BackgroundColor = System.Drawing.Color.GhostWhite;
            this.bbCodeGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightSkyBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.bbCodeGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.bbCodeGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bbCodeGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.heroClassColumn,
            this.itemColumn,
            this.heroNameColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.bbCodeGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.bbCodeGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbCodeGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.bbCodeGridView.EnableHeadersVisualStyles = false;
            this.bbCodeGridView.Location = new System.Drawing.Point(0, 0);
            this.bbCodeGridView.MultiSelect = false;
            this.bbCodeGridView.Name = "bbCodeGridView";
            this.bbCodeGridView.RowHeadersVisible = false;
            this.bbCodeGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.bbCodeGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.bbCodeGridView.Size = new System.Drawing.Size(597, 244);
            this.bbCodeGridView.TabIndex = 31;
            this.bbCodeGridView.VirtualMode = true;
            this.bbCodeGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.bbCodeGridView_CellValueNeeded);
            this.bbCodeGridView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.bbCodeGridView_CellValuePushed);
            // 
            // heroClassColumn
            // 
            this.heroClassColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.heroClassColumn.FillWeight = 120F;
            this.heroClassColumn.HeaderText = "preCode";
            this.heroClassColumn.Name = "heroClassColumn";
            // 
            // itemColumn
            // 
            this.itemColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.itemColumn.FillWeight = 80F;
            this.itemColumn.HeaderText = "Item";
            this.itemColumn.Name = "itemColumn";
            this.itemColumn.ReadOnly = true;
            this.itemColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // heroNameColumn
            // 
            this.heroNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.heroNameColumn.FillWeight = 120F;
            this.heroNameColumn.HeaderText = "postCode";
            this.heroNameColumn.Name = "heroNameColumn";
            // 
            // BBCodeSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 275);
            this.Controls.Add(this.bbCodeGridView);
            this.Controls.Add(this.label1);
            this.Name = "BBCodeSettingsForm";
            this.ShowIcon = false;
            this.Text = "BBCode Settings Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BBCodeSettingsForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.bbCodeGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView bbCodeGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroClassColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn heroNameColumn;        

    }
}