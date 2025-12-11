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
            this.cmbStudents = new System.Windows.Forms.ComboBox();
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.dtpNewDate = new System.Windows.Forms.DateTimePicker();
            this.btnUpdateDate = new System.Windows.Forms.Button();
            this.btnRunPayment = new System.Windows.Forms.Button();
            this.btnRunAutoPayments = new System.Windows.Forms.Button();
            this.lblStudent = new System.Windows.Forms.Label();
            this.lblCurrentDate = new System.Windows.Forms.Label();
            this.lblNewDate = new System.Windows.Forms.Label();
            this.txtLogs = new System.Windows.Forms.RichTextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.gbStudentInfo = new System.Windows.Forms.GroupBox();
            this.gbDateUpdate = new System.Windows.Forms.GroupBox();
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.gbLogs = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            this.gbStudentInfo.SuspendLayout();
            this.gbDateUpdate.SuspendLayout();
            this.gbActions.SuspendLayout();
            this.gbLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbStudentInfo
            // 
            this.gbStudentInfo.Controls.Add(this.lblStudent);
            this.gbStudentInfo.Controls.Add(this.cmbStudents);
            this.gbStudentInfo.Controls.Add(this.btnRefresh);
            this.gbStudentInfo.Location = new System.Drawing.Point(12, 12);
            this.gbStudentInfo.Name = "gbStudentInfo";
            this.gbStudentInfo.Size = new System.Drawing.Size(760, 60);
            this.gbStudentInfo.TabIndex = 0;
            this.gbStudentInfo.TabStop = false;
            this.gbStudentInfo.Text = "áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒáƒ áƒ©áƒ”áƒ•áƒ";
            // 
            // lblStudent
            // 
            this.lblStudent.AutoSize = true;
            this.lblStudent.Location = new System.Drawing.Point(6, 25);
            this.lblStudent.Name = "lblStudent";
            this.lblStudent.Size = new System.Drawing.Size(60, 13);
            this.lblStudent.TabIndex = 0;
            this.lblStudent.Text = "áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”:";
            // 
            // cmbStudents
            // 
            this.cmbStudents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStudents.FormattingEnabled = true;
            this.cmbStudents.Location = new System.Drawing.Point(72, 22);
            this.cmbStudents.Name = "cmbStudents";
            this.cmbStudents.Size = new System.Drawing.Size(500, 21);
            this.cmbStudents.TabIndex = 1;
            this.cmbStudents.SelectedIndexChanged += new System.EventHandler(this.cmbStudents_SelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(578, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(176, 25);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "ðŸ”„ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgvGroups
            // 
            this.dgvGroups.AllowUserToAddRows = false;
            this.dgvGroups.AllowUserToDeleteRows = false;
            this.dgvGroups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroups.Location = new System.Drawing.Point(12, 78);
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.ReadOnly = true;
            this.dgvGroups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGroups.Size = new System.Drawing.Size(760, 200);
            this.dgvGroups.TabIndex = 1;
            // 
            // gbDateUpdate
            // 
            this.gbDateUpdate.Controls.Add(this.lblCurrentDate);
            this.gbDateUpdate.Controls.Add(this.lblNewDate);
            this.gbDateUpdate.Controls.Add(this.dtpNewDate);
            this.gbDateUpdate.Controls.Add(this.btnUpdateDate);
            this.gbDateUpdate.Location = new System.Drawing.Point(12, 284);
            this.gbDateUpdate.Name = "gbDateUpdate";
            this.gbDateUpdate.Size = new System.Drawing.Size(380, 100);
            this.gbDateUpdate.TabIndex = 2;
            this.gbDateUpdate.TabStop = false;
            this.gbDateUpdate.Text = "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ";
            // 
            // lblCurrentDate
            // 
            this.lblCurrentDate.AutoSize = true;
            this.lblCurrentDate.Location = new System.Drawing.Point(6, 25);
            this.lblCurrentDate.Name = "lblCurrentDate";
            this.lblCurrentDate.Size = new System.Drawing.Size(100, 13);
            this.lblCurrentDate.TabIndex = 0;
            this.lblCurrentDate.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: -";
            // 
            // lblNewDate
            // 
            this.lblNewDate.AutoSize = true;
            this.lblNewDate.Location = new System.Drawing.Point(6, 50);
            this.lblNewDate.Name = "lblNewDate";
            this.lblNewDate.Size = new System.Drawing.Size(75, 13);
            this.lblNewDate.TabIndex = 1;
            this.lblNewDate.Text = "áƒáƒ®áƒáƒšáƒ˜ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜:";
            // 
            // dtpNewDate
            // 
            this.dtpNewDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNewDate.Location = new System.Drawing.Point(87, 48);
            this.dtpNewDate.Name = "dtpNewDate";
            this.dtpNewDate.Size = new System.Drawing.Size(120, 20);
            this.dtpNewDate.TabIndex = 2;
            // 
            // btnUpdateDate
            // 
            this.btnUpdateDate.Location = new System.Drawing.Point(213, 46);
            this.btnUpdateDate.Name = "btnUpdateDate";
            this.btnUpdateDate.Size = new System.Drawing.Size(150, 25);
            this.btnUpdateDate.TabIndex = 3;
            this.btnUpdateDate.Text = "âœ… áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ";
            this.btnUpdateDate.UseVisualStyleBackColor = true;
            this.btnUpdateDate.Click += new System.EventHandler(this.btnUpdateDate_Click);
            // 
            // gbActions
            // 
            this.gbActions.Controls.Add(this.btnRunPayment);
            this.gbActions.Controls.Add(this.btnRunAutoPayments);
            this.gbActions.Location = new System.Drawing.Point(398, 284);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(374, 100);
            this.gbActions.TabIndex = 3;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ";
            // 
            // btnRunPayment
            // 
            this.btnRunPayment.Location = new System.Drawing.Point(6, 25);
            this.btnRunPayment.Name = "btnRunPayment";
            this.btnRunPayment.Size = new System.Drawing.Size(360, 30);
            this.btnRunPayment.TabIndex = 0;
            this.btnRunPayment.Text = "ðŸ’° áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒáƒ áƒ©áƒ”áƒ£áƒšáƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ (Balance > 0)";
            this.btnRunPayment.UseVisualStyleBackColor = true;
            this.btnRunPayment.Click += new System.EventHandler(this.btnRunPayment_Click);
            // 
            // btnRunAutoPayments
            // 
            this.btnRunAutoPayments.Location = new System.Drawing.Point(6, 61);
            this.btnRunAutoPayments.Name = "btnRunAutoPayments";
            this.btnRunAutoPayments.Size = new System.Drawing.Size(360, 30);
            this.btnRunAutoPayments.TabIndex = 1;
            this.btnRunAutoPayments.Text = "ðŸ”„ áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ§áƒ•áƒ”áƒšáƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡";
            this.btnRunAutoPayments.UseVisualStyleBackColor = true;
            this.btnRunAutoPayments.Click += new System.EventHandler(this.btnRunAutoPayments_Click);
            // 
            // gbLogs
            // 
            this.gbLogs.Controls.Add(this.txtLogs);
            this.gbLogs.Location = new System.Drawing.Point(12, 390);
            this.gbLogs.Name = "gbLogs";
            this.gbLogs.Size = new System.Drawing.Size(760, 200);
            this.gbLogs.TabIndex = 4;
            this.gbLogs.TabStop = false;
            this.gbLogs.Text = "áƒšáƒáƒ’áƒ”áƒ‘áƒ˜";
            // 
            // txtLogs
            // 
            this.txtLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLogs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogs.Location = new System.Drawing.Point(3, 16);
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.Size = new System.Drawing.Size(754, 181);
            this.txtLogs.TabIndex = 0;
            this.txtLogs.Text = "";
            // 
            // PaymentTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 602);
            this.Controls.Add(this.gbLogs);
            this.Controls.Add(this.gbActions);
            this.Controls.Add(this.gbDateUpdate);
            this.Controls.Add(this.dgvGroups);
            this.Controls.Add(this.gbStudentInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PaymentTestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ - DateOfPayment áƒ¨áƒ”áƒªáƒ•áƒšáƒ";
            this.Load += new System.EventHandler(this.PaymentTestForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            this.gbStudentInfo.ResumeLayout(false);
            this.gbStudentInfo.PerformLayout();
            this.gbDateUpdate.ResumeLayout(false);
            this.gbDateUpdate.PerformLayout();
            this.gbActions.ResumeLayout(false);
            this.gbLogs.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}


