namespace BCCStudents.Presentation
{
    partial class PaymentTestForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbStudents;
        private System.Windows.Forms.DataGridView dgvGroups;
        private System.Windows.Forms.DateTimePicker dtpNewDate;
        private System.Windows.Forms.Button btnUpdateDate;
        private System.Windows.Forms.Button btnRunPayment;
        private System.Windows.Forms.Button btnRunAutoPayments;
        private System.Windows.Forms.Label lblStudent;
        private System.Windows.Forms.Label lblCurrentDate;
        private System.Windows.Forms.Label lblNewDate;
        private System.Windows.Forms.RichTextBox txtLogs;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox gbStudentInfo;
        private System.Windows.Forms.GroupBox gbDateUpdate;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.GroupBox gbLogs;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            cmbStudents = new ComboBox();
            dgvGroups = new DataGridView();
            dtpNewDate = new DateTimePicker();
            btnUpdateDate = new Button();
            btnRunPayment = new Button();
            btnRunAutoPayments = new Button();
            lblStudent = new Label();
            lblCurrentDate = new Label();
            lblNewDate = new Label();
            txtLogs = new RichTextBox();
            btnRefresh = new Button();
            gbStudentInfo = new GroupBox();
            gbDateUpdate = new GroupBox();
            gbActions = new GroupBox();
            gbLogs = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dgvGroups).BeginInit();
            gbStudentInfo.SuspendLayout();
            gbDateUpdate.SuspendLayout();
            gbActions.SuspendLayout();
            gbLogs.SuspendLayout();
            SuspendLayout();
            // 
            // cmbStudents
            // 
            cmbStudents.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStudents.FormattingEnabled = true;
            cmbStudents.Location = new Point(84, 25);
            cmbStudents.Margin = new Padding(4, 3, 4, 3);
            cmbStudents.Name = "cmbStudents";
            cmbStudents.Size = new Size(583, 23);
            cmbStudents.TabIndex = 1;
            cmbStudents.SelectedIndexChanged += cmbStudents_SelectedIndexChanged;
            // 
            // dgvGroups
            // 
            dgvGroups.AllowUserToAddRows = false;
            dgvGroups.AllowUserToDeleteRows = false;
            dgvGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGroups.BorderStyle = BorderStyle.None;
            dgvGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGroups.Location = new Point(14, 90);
            dgvGroups.Margin = new Padding(4, 3, 4, 3);
            dgvGroups.MultiSelect = false;
            dgvGroups.Name = "dgvGroups";
            dgvGroups.ReadOnly = true;
            dgvGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGroups.Size = new Size(887, 231);
            dgvGroups.TabIndex = 1;
            // 
            // dtpNewDate
            // 
            dtpNewDate.Format = DateTimePickerFormat.Short;
            dtpNewDate.Location = new Point(102, 55);
            dtpNewDate.Margin = new Padding(4, 3, 4, 3);
            dtpNewDate.Name = "dtpNewDate";
            dtpNewDate.Size = new Size(139, 23);
            dtpNewDate.TabIndex = 2;
            // 
            // btnUpdateDate
            // 
            btnUpdateDate.Location = new Point(248, 53);
            btnUpdateDate.Margin = new Padding(4, 3, 4, 3);
            btnUpdateDate.Name = "btnUpdateDate";
            btnUpdateDate.Size = new Size(175, 29);
            btnUpdateDate.TabIndex = 3;
            btnUpdateDate.Text = "თარიღის განახლება";
            btnUpdateDate.UseVisualStyleBackColor = true;
            btnUpdateDate.Click += btnUpdateDate_Click;
            // 
            // btnRunPayment
            // 
            btnRunPayment.Location = new Point(7, 29);
            btnRunPayment.Margin = new Padding(4, 3, 4, 3);
            btnRunPayment.Name = "btnRunPayment";
            btnRunPayment.Size = new Size(420, 35);
            btnRunPayment.TabIndex = 0;
            btnRunPayment.Text = "გადახდა არჩეული მოსწავლისთვის (Balance > 0)";
            btnRunPayment.UseVisualStyleBackColor = true;
            btnRunPayment.Click += btnRunPayment_Click;
            // 
            // btnRunAutoPayments
            // 
            btnRunAutoPayments.Location = new Point(7, 70);
            btnRunAutoPayments.Margin = new Padding(4, 3, 4, 3);
            btnRunAutoPayments.Name = "btnRunAutoPayments";
            btnRunAutoPayments.Size = new Size(420, 35);
            btnRunAutoPayments.TabIndex = 1;
            btnRunAutoPayments.Text = "ავტომატური გადახდა ყველა მოსწავლისთვის";
            btnRunAutoPayments.UseVisualStyleBackColor = true;
            btnRunAutoPayments.Click += btnRunAutoPayments_Click;
            // 
            // lblStudent
            // 
            lblStudent.AutoSize = true;
            lblStudent.Location = new Point(7, 29);
            lblStudent.Margin = new Padding(4, 0, 4, 0);
            lblStudent.Name = "lblStudent";
            lblStudent.Size = new Size(136, 15);
            lblStudent.TabIndex = 0;
            lblStudent.Text = "მოსწავლე:";
            // 
            // lblCurrentDate
            // 
            lblCurrentDate.AutoSize = true;
            lblCurrentDate.Location = new Point(7, 29);
            lblCurrentDate.Margin = new Padding(4, 0, 4, 0);
            lblCurrentDate.Name = "lblCurrentDate";
            lblCurrentDate.Size = new Size(267, 15);
            lblCurrentDate.TabIndex = 0;
            lblCurrentDate.Text = "მიმდინარე თარიღი: -";
            // 
            // lblNewDate
            // 
            lblNewDate.AutoSize = true;
            lblNewDate.Location = new Point(7, 58);
            lblNewDate.Margin = new Padding(4, 0, 4, 0);
            lblNewDate.Name = "lblNewDate";
            lblNewDate.Size = new Size(191, 15);
            lblNewDate.TabIndex = 1;
            lblNewDate.Text = "ახალი თარიღი:";
            // 
            // txtLogs
            // 
            txtLogs.Dock = DockStyle.Fill;
            txtLogs.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtLogs.Location = new Point(4, 19);
            txtLogs.Margin = new Padding(4, 3, 4, 3);
            txtLogs.Name = "txtLogs";
            txtLogs.ReadOnly = true;
            txtLogs.Size = new Size(879, 209);
            txtLogs.TabIndex = 0;
            txtLogs.Text = "";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(674, 23);
            btnRefresh.Margin = new Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(205, 29);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "განახლება";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // gbStudentInfo
            // 
            gbStudentInfo.Controls.Add(lblStudent);
            gbStudentInfo.Controls.Add(cmbStudents);
            gbStudentInfo.Controls.Add(btnRefresh);
            gbStudentInfo.Location = new Point(14, 14);
            gbStudentInfo.Margin = new Padding(4, 3, 4, 3);
            gbStudentInfo.Name = "gbStudentInfo";
            gbStudentInfo.Padding = new Padding(4, 3, 4, 3);
            gbStudentInfo.Size = new Size(887, 69);
            gbStudentInfo.TabIndex = 0;
            gbStudentInfo.TabStop = false;
            gbStudentInfo.Text = "მოსწავლის არჩევა";
            // 
            // gbDateUpdate
            // 
            gbDateUpdate.Controls.Add(lblCurrentDate);
            gbDateUpdate.Controls.Add(lblNewDate);
            gbDateUpdate.Controls.Add(dtpNewDate);
            gbDateUpdate.Controls.Add(btnUpdateDate);
            gbDateUpdate.Location = new Point(14, 328);
            gbDateUpdate.Margin = new Padding(4, 3, 4, 3);
            gbDateUpdate.Name = "gbDateUpdate";
            gbDateUpdate.Padding = new Padding(4, 3, 4, 3);
            gbDateUpdate.Size = new Size(443, 115);
            gbDateUpdate.TabIndex = 2;
            gbDateUpdate.TabStop = false;
            gbDateUpdate.Text = "გადახდის თარიღის შეცვლა";
            // 
            // gbActions
            // 
            gbActions.Controls.Add(btnRunPayment);
            gbActions.Controls.Add(btnRunAutoPayments);
            gbActions.Location = new Point(464, 328);
            gbActions.Margin = new Padding(4, 3, 4, 3);
            gbActions.Name = "gbActions";
            gbActions.Padding = new Padding(4, 3, 4, 3);
            gbActions.Size = new Size(436, 115);
            gbActions.TabIndex = 3;
            gbActions.TabStop = false;
            gbActions.Text = "გადახდის ტესტირება";
            // 
            // gbLogs
            // 
            gbLogs.Controls.Add(txtLogs);
            gbLogs.Location = new Point(14, 450);
            gbLogs.Margin = new Padding(4, 3, 4, 3);
            gbLogs.Name = "gbLogs";
            gbLogs.Padding = new Padding(4, 3, 4, 3);
            gbLogs.Size = new Size(887, 231);
            gbLogs.TabIndex = 4;
            gbLogs.TabStop = false;
            gbLogs.Text = "ლოგები";
            // 
            // PaymentTestForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(915, 695);
            Controls.Add(gbLogs);
            Controls.Add(gbActions);
            Controls.Add(gbDateUpdate);
            Controls.Add(dgvGroups);
            Controls.Add(gbStudentInfo);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PaymentTestForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "გადახდის ტესტირება - DateOfPayment შეცვლა";
            Load += PaymentTestForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvGroups).EndInit();
            gbStudentInfo.ResumeLayout(false);
            gbStudentInfo.PerformLayout();
            gbDateUpdate.ResumeLayout(false);
            gbDateUpdate.PerformLayout();
            gbActions.ResumeLayout(false);
            gbLogs.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}


