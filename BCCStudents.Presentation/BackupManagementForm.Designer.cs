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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblLastBackup = new System.Windows.Forms.Label();
            this.lblAutoBackupStatus = new System.Windows.Forms.Label();
            this.btnDisableAutoBackup = new System.Windows.Forms.Button();
            this.btnEnableAutoBackup = new System.Windows.Forms.Button();
            this.btnOpenBackupFolder = new System.Windows.Forms.Button();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDeleteBackup = new System.Windows.Forms.Button();
            this.btnRestoreBackup = new System.Windows.Forms.Button();
            this.btnCreateBackup = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvBackups = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblLastBackup);
            this.panel1.Controls.Add(this.lblAutoBackupStatus);
            this.panel1.Controls.Add(this.btnDisableAutoBackup);
            this.panel1.Controls.Add(this.btnEnableAutoBackup);
            this.panel1.Controls.Add(this.btnOpenBackupFolder);
            this.panel1.Controls.Add(this.btnClearFilter);
            this.panel1.Controls.Add(this.btnApplyFilter);
            this.panel1.Controls.Add(this.dtpTo);
            this.panel1.Controls.Add(this.dtpFrom);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnDeleteBackup);
            this.panel1.Controls.Add(this.btnRestoreBackup);
            this.panel1.Controls.Add(this.btnCreateBackup);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(984, 180);
            this.panel1.TabIndex = 0;
            // 
            // lblLastBackup
            // 
            this.lblLastBackup.AutoSize = true;
            this.lblLastBackup.Location = new System.Drawing.Point(13, 150);
            this.lblLastBackup.Name = "lblLastBackup";
            this.lblLastBackup.Size = new System.Drawing.Size(77, 13);
            this.lblLastBackup.TabIndex = 16;
            this.lblLastBackup.Text = "áƒ‘áƒáƒšáƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜";
            // 
            // lblAutoBackupStatus
            // 
            this.lblAutoBackupStatus.AutoSize = true;
            this.lblAutoBackupStatus.Location = new System.Drawing.Point(13, 100);
            this.lblAutoBackupStatus.Name = "lblAutoBackupStatus";
            this.lblAutoBackupStatus.Size = new System.Drawing.Size(55, 13);
            this.lblAutoBackupStatus.TabIndex = 15;
            this.lblAutoBackupStatus.Text = "áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜";
            // 
            // btnDisableAutoBackup
            // 
            this.btnDisableAutoBackup.Location = new System.Drawing.Point(876, 70);
            this.btnDisableAutoBackup.Name = "btnDisableAutoBackup";
            this.btnDisableAutoBackup.Size = new System.Drawing.Size(95, 23);
            this.btnDisableAutoBackup.TabIndex = 14;
            this.btnDisableAutoBackup.Text = "áƒ’áƒáƒ›áƒáƒ áƒ—áƒ•áƒ";
            this.btnDisableAutoBackup.UseVisualStyleBackColor = true;
            // 
            // btnEnableAutoBackup
            // 
            this.btnEnableAutoBackup.Location = new System.Drawing.Point(775, 70);
            this.btnEnableAutoBackup.Name = "btnEnableAutoBackup";
            this.btnEnableAutoBackup.Size = new System.Drawing.Size(95, 23);
            this.btnEnableAutoBackup.TabIndex = 13;
            this.btnEnableAutoBackup.Text = "áƒ©áƒáƒ áƒ—áƒ•áƒ";
            this.btnEnableAutoBackup.UseVisualStyleBackColor = true;
            // 
            // btnOpenBackupFolder
            // 
            this.btnOpenBackupFolder.Location = new System.Drawing.Point(674, 70);
            this.btnOpenBackupFolder.Name = "btnOpenBackupFolder";
            this.btnOpenBackupFolder.Size = new System.Drawing.Size(95, 43);
            this.btnOpenBackupFolder.TabIndex = 12;
            this.btnOpenBackupFolder.Text = "áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ˜áƒ¡ áƒ’áƒáƒ®áƒ¡áƒœáƒ";
            this.btnOpenBackupFolder.UseVisualStyleBackColor = true;
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Location = new System.Drawing.Point(573, 70);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(95, 43);
            this.btnClearFilter.TabIndex = 11;
            this.btnClearFilter.Text = "áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜áƒ¡ áƒ’áƒáƒ¡áƒ£áƒ¤áƒ—áƒáƒ•áƒ”áƒ‘áƒ";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Location = new System.Drawing.Point(472, 70);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(95, 43);
            this.btnApplyFilter.TabIndex = 10;
            this.btnApplyFilter.Text = "áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(300, 72);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(150, 20);
            this.dtpTo.TabIndex = 9;
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(89, 72);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(150, 20);
            this.dtpFrom.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(250, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "áƒ›áƒ“áƒ”:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜:";
            // 
            // btnDeleteBackup
            // 
            this.btnDeleteBackup.Location = new System.Drawing.Point(876, 10);
            this.btnDeleteBackup.Name = "btnDeleteBackup";
            this.btnDeleteBackup.Size = new System.Drawing.Size(95, 23);
            this.btnDeleteBackup.TabIndex = 5;
            this.btnDeleteBackup.Text = "áƒ¬áƒáƒ¨áƒšáƒ";
            this.btnDeleteBackup.UseVisualStyleBackColor = true;
            // 
            // btnRestoreBackup
            // 
            this.btnRestoreBackup.Location = new System.Drawing.Point(775, 10);
            this.btnRestoreBackup.Name = "btnRestoreBackup";
            this.btnRestoreBackup.Size = new System.Drawing.Size(95, 23);
            this.btnRestoreBackup.TabIndex = 4;
            this.btnRestoreBackup.Text = "áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ";
            this.btnRestoreBackup.UseVisualStyleBackColor = true;
            // 
            // btnCreateBackup
            // 
            this.btnCreateBackup.Location = new System.Drawing.Point(674, 10);
            this.btnCreateBackup.Name = "btnCreateBackup";
            this.btnCreateBackup.Size = new System.Drawing.Size(95, 23);
            this.btnCreateBackup.TabIndex = 3;
            this.btnCreateBackup.Text = "áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ";
            this.btnCreateBackup.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(573, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(55, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜";
            // 
            // dgvBackups
            // 
            this.dgvBackups.AllowUserToAddRows = false;
            this.dgvBackups.AllowUserToDeleteRows = false;
            this.dgvBackups.BackgroundColor = System.Drawing.Color.White;
            this.dgvBackups.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBackups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBackups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBackups.Location = new System.Drawing.Point(0, 180);
            this.dgvBackups.Name = "dgvBackups";
            this.dgvBackups.ReadOnly = true;
            this.dgvBackups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBackups.Size = new System.Drawing.Size(984, 381);
            this.dgvBackups.TabIndex = 1;
            // 
            // BackupManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.dgvBackups);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "BackupManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ›áƒáƒ áƒ—áƒ•áƒ";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBackups)).EndInit();
            this.ResumeLayout(false);

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
