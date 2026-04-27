namespace BCCStudents.Presentation
{
    partial class FailedStudentsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FailedStudentsForm));
            dataGridViewStudents = new DataGridView();
            btnDeleteStudent = new Button();
            btnAddStudent = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewStudents
            // 
            dataGridViewStudents.AllowUserToAddRows = false;
            dataGridViewStudents.AllowUserToDeleteRows = false;
            dataGridViewStudents.BorderStyle = BorderStyle.None;
            dataGridViewStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewStudents.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridViewStudents.EnableHeadersVisualStyles = false;
            dataGridViewStudents.Location = new Point(14, 14);
            dataGridViewStudents.Margin = new Padding(4, 3, 4, 3);
            dataGridViewStudents.Name = "dataGridViewStudents";
            dataGridViewStudents.Size = new Size(810, 705);
            dataGridViewStudents.TabIndex = 0;
            dataGridViewStudents.CellEndEdit += dataGridViewStudents_CellEndEdit;
            // 
            // btnDeleteStudent
            // 
            btnDeleteStudent.Location = new Point(832, 14);
            btnDeleteStudent.Margin = new Padding(4, 3, 4, 3);
            btnDeleteStudent.Name = "btnDeleteStudent";
            btnDeleteStudent.Size = new Size(88, 27);
            btnDeleteStudent.TabIndex = 1;
            btnDeleteStudent.Text = "წაშლა";
            btnDeleteStudent.UseVisualStyleBackColor = true;
            btnDeleteStudent.Click += btnDeleteStudent_Click;
            // 
            // btnAddStudent
            // 
            btnAddStudent.Location = new Point(832, 47);
            btnAddStudent.Margin = new Padding(4, 3, 4, 3);
            btnAddStudent.Name = "btnAddStudent";
            btnAddStudent.Size = new Size(88, 27);
            btnAddStudent.TabIndex = 1;
            btnAddStudent.Text = "დამატება";
            btnAddStudent.UseVisualStyleBackColor = true;
            btnAddStudent.Click += btnAddStudent_Click;
            // 
            // FailedStudentsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 760);
            Controls.Add(btnAddStudent);
            Controls.Add(btnDeleteStudent);
            Controls.Add(dataGridViewStudents);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "FailedStudentsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FailedStudentsForm";
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewStudents;
        private System.Windows.Forms.Button btnDeleteStudent;
        private System.Windows.Forms.Button btnAddStudent;
    }
}
