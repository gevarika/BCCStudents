namespace BCCStudents.Presentation
{
    partial class PendingStudentsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingStudentsForm));
            dataGridView1 = new DataGridView();
            btnApprove = new Button();
            btnDelete = new Button();
            btnSaveChanges = new Button();
            btnDownloadAll = new Button();
            progressBar1 = new ProgressBar();
            lblSelection = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.Location = new Point(14, 51);
            dataGridView1.Margin = new Padding(4, 3, 4, 3);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(905, 418);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            // 
            // btnApprove
            // 
            btnApprove.Location = new Point(805, 479);
            btnApprove.Margin = new Padding(4, 3, 4, 3);
            btnApprove.Name = "btnApprove";
            btnApprove.Size = new Size(114, 27);
            btnApprove.TabIndex = 1;
            btnApprove.Text = "დადასტურება";
            btnApprove.UseVisualStyleBackColor = true;
            btnApprove.Click += btnApprove_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(684, 479);
            btnDelete.Margin = new Padding(4, 3, 4, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(114, 27);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "წაშლა";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnSaveChanges
            // 
            btnSaveChanges.Location = new Point(14, 479);
            btnSaveChanges.Margin = new Padding(4, 3, 4, 3);
            btnSaveChanges.Name = "btnSaveChanges";
            btnSaveChanges.Size = new Size(88, 27);
            btnSaveChanges.TabIndex = 2;
            btnSaveChanges.Text = "შენახვა";
            btnSaveChanges.UseVisualStyleBackColor = true;
            btnSaveChanges.Click += btnSaveChanges_Click;
            // 
            // btnDownloadAll
            // 
            btnDownloadAll.Location = new Point(14, 17);
            btnDownloadAll.Margin = new Padding(4, 3, 4, 3);
            btnDownloadAll.Name = "btnDownloadAll";
            btnDownloadAll.Size = new Size(184, 27);
            btnDownloadAll.TabIndex = 3;
            btnDownloadAll.Text = "ფაილების ჩამოტვირთვა";
            btnDownloadAll.UseVisualStyleBackColor = true;
            btnDownloadAll.Click += btnDownloadAll_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(141, 479);
            progressBar1.Margin = new Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(471, 27);
            progressBar1.TabIndex = 4;
            // 
            // lblSelection
            // 
            lblSelection.AutoSize = true;
            lblSelection.Location = new Point(680, 29);
            lblSelection.Margin = new Padding(4, 0, 4, 0);
            lblSelection.Name = "lblSelection";
            lblSelection.Size = new Size(38, 15);
            lblSelection.TabIndex = 5;
            lblSelection.Text = "label1";
            // 
            // PendingStudentsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 519);
            Controls.Add(lblSelection);
            Controls.Add(progressBar1);
            Controls.Add(btnDownloadAll);
            Controls.Add(btnSaveChanges);
            Controls.Add(btnDelete);
            Controls.Add(btnApprove);
            Controls.Add(dataGridView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "PendingStudentsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PendingStudentsForm";
            Load += PendingStudentsForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Button btnDownloadAll;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblSelection;
    }
}
