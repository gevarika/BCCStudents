namespace BCCStudents.Presentation
{
    partial class PaymentsImportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PaymentsImportForm));
            panel1 = new Panel();
            btnSelectValid = new Button();
            btnDeselectAll = new Button();
            btnSelectAll = new Button();
            lblStatus = new Label();
            progressBar = new ProgressBar();
            btnImport = new Button();
            btnPreview = new Button();
            btnSelectFile = new Button();
            txtFilePath = new TextBox();
            label1 = new Label();
            dgvPreview = new DataGridView();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(btnSelectValid);
            panel1.Controls.Add(btnDeselectAll);
            panel1.Controls.Add(btnSelectAll);
            panel1.Controls.Add(lblStatus);
            panel1.Controls.Add(progressBar);
            panel1.Controls.Add(btnImport);
            panel1.Controls.Add(btnPreview);
            panel1.Controls.Add(btnSelectFile);
            panel1.Controls.Add(txtFilePath);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(12, 12, 12, 12);
            panel1.Size = new Size(1148, 115);
            panel1.TabIndex = 0;
            // 
            // btnSelectValid
            // 
            btnSelectValid.Enabled = false;
            btnSelectValid.Location = new Point(668, 12);
            btnSelectValid.Margin = new Padding(4, 3, 4, 3);
            btnSelectValid.Name = "btnSelectValid";
            btnSelectValid.Size = new Size(111, 27);
            btnSelectValid.TabIndex = 9;
            btnSelectValid.Text = "აირჩიეთ მოქმედი";
            btnSelectValid.UseVisualStyleBackColor = true;
            // 
            // btnDeselectAll
            // 
            btnDeselectAll.Enabled = false;
            btnDeselectAll.Location = new Point(551, 12);
            btnDeselectAll.Margin = new Padding(4, 3, 4, 3);
            btnDeselectAll.Name = "btnDeselectAll";
            btnDeselectAll.Size = new Size(111, 27);
            btnDeselectAll.TabIndex = 8;
            btnDeselectAll.Text = "ყველას მოხსნა";
            btnDeselectAll.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            btnSelectAll.Enabled = false;
            btnSelectAll.Location = new Point(433, 12);
            btnSelectAll.Margin = new Padding(4, 3, 4, 3);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(111, 27);
            btnSelectAll.TabIndex = 7;
            btnSelectAll.Text = "ყველას არჩევა";
            btnSelectAll.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 81);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(62, 15);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "სტატუსი";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 52);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1118, 27);
            progressBar.TabIndex = 5;
            // 
            // btnImport
            // 
            btnImport.Enabled = false;
            btnImport.Location = new Point(1022, 12);
            btnImport.Margin = new Padding(4, 3, 4, 3);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(111, 27);
            btnImport.TabIndex = 4;
            btnImport.Text = "იმპორტი";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // btnPreview
            // 
            btnPreview.Enabled = false;
            btnPreview.Location = new Point(904, 12);
            btnPreview.Margin = new Padding(4, 3, 4, 3);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(111, 27);
            btnPreview.TabIndex = 3;
            btnPreview.Text = "პრევიუს";
            btnPreview.UseVisualStyleBackColor = true;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Location = new Point(786, 12);
            btnSelectFile.Margin = new Padding(4, 3, 4, 3);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(111, 27);
            btnSelectFile.TabIndex = 2;
            btnSelectFile.Text = "ფაილის არჩევა";
            btnSelectFile.UseVisualStyleBackColor = true;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(104, 14);
            txtFilePath.Margin = new Padding(4, 3, 4, 3);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(321, 23);
            txtFilePath.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 17);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(131, 15);
            label1.TabIndex = 0;
            label1.Text = "ფაილის მისამართი:";
            // 
            // dgvPreview
            // 
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.AllowUserToDeleteRows = false;
            dgvPreview.BackgroundColor = Color.White;
            dgvPreview.BorderStyle = BorderStyle.None;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.Location = new Point(0, 115);
            dgvPreview.Margin = new Padding(4, 3, 4, 3);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.Size = new Size(1148, 532);
            dgvPreview.TabIndex = 1;
            // 
            // PaymentsImportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 647);
            Controls.Add(dgvPreview);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(931, 456);
            Name = "PaymentsImportForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "გადახდების იმპორტი";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Button btnSelectValid;
    }
}
