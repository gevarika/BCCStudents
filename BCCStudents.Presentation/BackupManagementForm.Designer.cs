namespace BCCStudents.Presentation
{
    partial class BackupManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupManagementForm));
            panel1 = new Panel();
            lblLastBackup = new Label();
            lblAutoBackupStatus = new Label();
            btnDisableAutoBackup = new Button();
            btnEnableAutoBackup = new Button();
            btnOpenBackupFolder = new Button();
            btnClearFilter = new Button();
            btnApplyFilter = new Button();
            dtpTo = new DateTimePicker();
            dtpFrom = new DateTimePicker();
            label3 = new Label();
            label2 = new Label();
            btnDeleteBackup = new Button();
            btnRestoreBackup = new Button();
            btnCreateBackup = new Button();
            btnRefresh = new Button();
            lblStatus = new Label();
            dgvBackups = new DataGridView();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBackups).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(lblLastBackup);
            panel1.Controls.Add(lblAutoBackupStatus);
            panel1.Controls.Add(btnDisableAutoBackup);
            panel1.Controls.Add(btnEnableAutoBackup);
            panel1.Controls.Add(btnOpenBackupFolder);
            panel1.Controls.Add(btnClearFilter);
            panel1.Controls.Add(btnApplyFilter);
            panel1.Controls.Add(dtpTo);
            panel1.Controls.Add(dtpFrom);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(btnDeleteBackup);
            panel1.Controls.Add(btnRestoreBackup);
            panel1.Controls.Add(btnCreateBackup);
            panel1.Controls.Add(btnRefresh);
            panel1.Controls.Add(lblStatus);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(12, 12, 12, 12);
            panel1.Size = new Size(1148, 208);
            panel1.TabIndex = 0;
            // 
            // lblLastBackup
            // 
            lblLastBackup.AutoSize = true;
            lblLastBackup.Location = new Point(15, 173);
            lblLastBackup.Margin = new Padding(4, 0, 4, 0);
            lblLastBackup.Name = "lblLastBackup";
            lblLastBackup.Size = new Size(86, 15);
            lblLastBackup.TabIndex = 16;
            lblLastBackup.Text = "ბოლო ბექაპი";
            // 
            // lblAutoBackupStatus
            // 
            lblAutoBackupStatus.AutoSize = true;
            lblAutoBackupStatus.Location = new Point(15, 115);
            lblAutoBackupStatus.Margin = new Padding(4, 0, 4, 0);
            lblAutoBackupStatus.Name = "lblAutoBackupStatus";
            lblAutoBackupStatus.Size = new Size(62, 15);
            lblAutoBackupStatus.TabIndex = 15;
            lblAutoBackupStatus.Text = "სტატუსი";
            // 
            // btnDisableAutoBackup
            // 
            btnDisableAutoBackup.Location = new Point(1022, 81);
            btnDisableAutoBackup.Margin = new Padding(4, 3, 4, 3);
            btnDisableAutoBackup.Name = "btnDisableAutoBackup";
            btnDisableAutoBackup.Size = new Size(111, 27);
            btnDisableAutoBackup.TabIndex = 14;
            btnDisableAutoBackup.Text = "გამორთვა";
            btnDisableAutoBackup.UseVisualStyleBackColor = true;
            // 
            // btnEnableAutoBackup
            // 
            btnEnableAutoBackup.Location = new Point(904, 81);
            btnEnableAutoBackup.Margin = new Padding(4, 3, 4, 3);
            btnEnableAutoBackup.Name = "btnEnableAutoBackup";
            btnEnableAutoBackup.Size = new Size(111, 27);
            btnEnableAutoBackup.TabIndex = 13;
            btnEnableAutoBackup.Text = "ჩართვა";
            btnEnableAutoBackup.UseVisualStyleBackColor = true;
            // 
            // btnOpenBackupFolder
            // 
            btnOpenBackupFolder.Location = new Point(786, 81);
            btnOpenBackupFolder.Margin = new Padding(4, 3, 4, 3);
            btnOpenBackupFolder.Name = "btnOpenBackupFolder";
            btnOpenBackupFolder.Size = new Size(111, 50);
            btnOpenBackupFolder.TabIndex = 12;
            btnOpenBackupFolder.Text = "საქაღალდის გახსნა";
            btnOpenBackupFolder.UseVisualStyleBackColor = true;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Location = new Point(668, 81);
            btnClearFilter.Margin = new Padding(4, 3, 4, 3);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(111, 50);
            btnClearFilter.TabIndex = 11;
            btnClearFilter.Text = "ფილტრის გასუფთავება";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.Location = new Point(551, 81);
            btnApplyFilter.Margin = new Padding(4, 3, 4, 3);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(111, 50);
            btnApplyFilter.TabIndex = 10;
            btnApplyFilter.Text = "ფილტრის გამოყენება";
            btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // dtpTo
            // 
            dtpTo.Location = new Point(350, 83);
            dtpTo.Margin = new Padding(4, 3, 4, 3);
            dtpTo.Name = "dtpTo";
            dtpTo.Size = new Size(174, 23);
            dtpTo.TabIndex = 9;
            // 
            // dtpFrom
            // 
            dtpFrom.Location = new Point(104, 83);
            dtpFrom.Margin = new Padding(4, 3, 4, 3);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.Size = new Size(174, 23);
            dtpFrom.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(292, 87);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(34, 15);
            label3.TabIndex = 7;
            label3.Text = "მდე:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 87);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 6;
            label2.Text = "თარიღი:";
            // 
            // btnDeleteBackup
            // 
            btnDeleteBackup.Location = new Point(1022, 12);
            btnDeleteBackup.Margin = new Padding(4, 3, 4, 3);
            btnDeleteBackup.Name = "btnDeleteBackup";
            btnDeleteBackup.Size = new Size(111, 27);
            btnDeleteBackup.TabIndex = 5;
            btnDeleteBackup.Text = "წაშლა";
            btnDeleteBackup.UseVisualStyleBackColor = true;
            // 
            // btnRestoreBackup
            // 
            btnRestoreBackup.Location = new Point(904, 12);
            btnRestoreBackup.Margin = new Padding(4, 3, 4, 3);
            btnRestoreBackup.Name = "btnRestoreBackup";
            btnRestoreBackup.Size = new Size(111, 27);
            btnRestoreBackup.TabIndex = 4;
            btnRestoreBackup.Text = "აღდგენა";
            btnRestoreBackup.UseVisualStyleBackColor = true;
            // 
            // btnCreateBackup
            // 
            btnCreateBackup.Location = new Point(786, 12);
            btnCreateBackup.Margin = new Padding(4, 3, 4, 3);
            btnCreateBackup.Name = "btnCreateBackup";
            btnCreateBackup.Size = new Size(111, 27);
            btnCreateBackup.TabIndex = 3;
            btnCreateBackup.Text = "ბექაპის შექმნა";
            btnCreateBackup.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(668, 12);
            btnRefresh.Margin = new Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(111, 27);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "განახლება";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 17);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(62, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "სტატუსი";
            // 
            // dgvBackups
            // 
            dgvBackups.AllowUserToAddRows = false;
            dgvBackups.AllowUserToDeleteRows = false;
            dgvBackups.BackgroundColor = Color.White;
            dgvBackups.BorderStyle = BorderStyle.None;
            dgvBackups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBackups.Dock = DockStyle.Fill;
            dgvBackups.Location = new Point(0, 208);
            dgvBackups.Margin = new Padding(4, 3, 4, 3);
            dgvBackups.Name = "dgvBackups";
            dgvBackups.ReadOnly = true;
            dgvBackups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBackups.Size = new Size(1148, 439);
            dgvBackups.TabIndex = 1;
            // 
            // BackupManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1148, 647);
            Controls.Add(dgvBackups);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(931, 456);
            Name = "BackupManagementForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "ბექაპის მართვა";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBackups).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvBackups;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnCreateBackup;
        private System.Windows.Forms.Button btnRestoreBackup;
        private System.Windows.Forms.Button btnDeleteBackup;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Button btnOpenBackupFolder;
        private System.Windows.Forms.Button btnEnableAutoBackup;
        private System.Windows.Forms.Button btnDisableAutoBackup;
        private System.Windows.Forms.Label lblAutoBackupStatus;
        private System.Windows.Forms.Label lblLastBackup;
    }
} 
