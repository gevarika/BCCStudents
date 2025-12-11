namespace BCCStudents.Presentation
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnStudents = new System.Windows.Forms.Button();
            this.btnGroups = new System.Windows.Forms.Button();
            this.btnGroupsEdit = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.მთავარიToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.სტატისტიკაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.გადახდებიToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.გადახდაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.გადახდისტესტირებაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ბექაპისმართვაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmAdminPanel = new System.Windows.Forms.ToolStripMenuItem();
            this.ლოგებიToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvPayments = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnReconnect = new System.Windows.Forms.ToolStripSplitButton();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsPaymentStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblPaymentNextDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbStartStudyDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bWPayment = new System.ComponentModel.BackgroundWorker();
            this.btnPaymentHistory = new System.Windows.Forms.Button();
            this.btnRefreshPaymentProcess = new System.Windows.Forms.Button();
            this.StudentAmount = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStudents
            // 
            this.btnStudents.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStudents.Location = new System.Drawing.Point(3, 3);
            this.btnStudents.Name = "btnStudents";
            this.btnStudents.Size = new System.Drawing.Size(266, 50);
            this.btnStudents.TabIndex = 0;
            this.btnStudents.Text = "მოსწავლეები";
            this.btnStudents.UseVisualStyleBackColor = true;
            this.btnStudents.Click += new System.EventHandler(this.btnStudents_Click);
            // 
            // btnGroups
            // 
            this.btnGroups.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGroups.Location = new System.Drawing.Point(3, 59);
            this.btnGroups.Name = "btnGroups";
            this.btnGroups.Size = new System.Drawing.Size(266, 50);
            this.btnGroups.TabIndex = 1;
            this.btnGroups.Text = "ჯგუფები";
            this.btnGroups.UseVisualStyleBackColor = true;
            this.btnGroups.Click += new System.EventHandler(this.btnGroups_Click);
            // 
            // btnGroupsEdit
            // 
            this.btnGroupsEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGroupsEdit.Location = new System.Drawing.Point(3, 115);
            this.btnGroupsEdit.Name = "btnGroupsEdit";
            this.btnGroupsEdit.Size = new System.Drawing.Size(266, 50);
            this.btnGroupsEdit.TabIndex = 2;
            this.btnGroupsEdit.Text = "ჯგუფების რედაქტირება";
            this.btnGroupsEdit.UseVisualStyleBackColor = true;
            this.btnGroupsEdit.Click += new System.EventHandler(this.btnGroupsEdit_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.მთავარიToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1299, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // მთავარიToolStripMenuItem
            // 
            this.მთავარიToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.სტატისტიკაToolStripMenuItem,
            this.გადახდებიToolStripMenuItem,
            this.გადახდაToolStripMenuItem,
            this.გადახდისტესტირებაToolStripMenuItem,
            this.ბექაპისმართვაToolStripMenuItem,
            this.tsmAdminPanel,
            this.ლოგებიToolStripMenuItem});
            this.მთავარიToolStripMenuItem.Name = "მთავარიToolStripMenuItem";
            this.მთავარიToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.მთავარიToolStripMenuItem.Text = "მთავარი";
            // 
            // სტატისტიკაToolStripMenuItem
            // 
            this.სტატისტიკაToolStripMenuItem.Name = "სტატისტიკაToolStripMenuItem";
            this.სტატისტიკაToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.სტატისტიკაToolStripMenuItem.Text = "სტატისტიკა";
            this.სტატისტიკაToolStripMenuItem.Click += new System.EventHandler(this.სტატისტიკაToolStripMenuItem_Click);
            // 
            // გადახდებიToolStripMenuItem
            // 
            this.გადახდებიToolStripMenuItem.Name = "გადახდებიToolStripMenuItem";
            this.გადახდებიToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.გადახდებიToolStripMenuItem.Text = "გადახდები";
            this.გადახდებიToolStripMenuItem.Click += new System.EventHandler(this.გადახდებიToolStripMenuItem_Click);
            // 
            // გადახდაToolStripMenuItem
            // 
            this.გადახდაToolStripMenuItem.Name = "გადახდაToolStripMenuItem";
            this.გადახდაToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.გადახდაToolStripMenuItem.Text = "გადახდა";
            this.გადახდაToolStripMenuItem.Click += new System.EventHandler(this.გადახდაToolStripMenuItem_Click);
            // 
            // გადახდისტესტირებაToolStripMenuItem
            // 
            this.გადახდისტესტირებაToolStripMenuItem.Name = "გადახდისტესტირებაToolStripMenuItem";
            this.გადახდისტესტირებაToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.გადახდისტესტირებაToolStripMenuItem.Text = "გადახდის ტესტირება";
            this.გადახდისტესტირებაToolStripMenuItem.Click += new System.EventHandler(this.გადახდისტესტირებაToolStripMenuItem_Click);
            // 
            // ბექაპისმართვაToolStripMenuItem
            // 
            this.ბექაპისმართვაToolStripMenuItem.Name = "ბექაპისმართვაToolStripMenuItem";
            this.ბექაპისმართვაToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.ბექაპისმართვაToolStripMenuItem.Text = "ბექაპის მართვა";
            this.ბექაპისმართვაToolStripMenuItem.Click += new System.EventHandler(this.ბექაპისმართვაToolStripMenuItem_Click);
            // 
            // tsmAdminPanel
            // 
            this.tsmAdminPanel.Name = "tsmAdminPanel";
            this.tsmAdminPanel.Size = new System.Drawing.Size(205, 22);
            this.tsmAdminPanel.Text = "ადმინპანელი";
            this.tsmAdminPanel.Click += new System.EventHandler(this.tsmAdminPanel_Click);
            // 
            // ლოგებიToolStripMenuItem
            // 
            this.ლოგებიToolStripMenuItem.Name = "ლოგებიToolStripMenuItem";
            this.ლოგებიToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.ლოგებიToolStripMenuItem.Text = "ლოგები";
            this.ლოგებიToolStripMenuItem.Click += new System.EventHandler(this.ლოგებიToolStripMenuItem_Click);
            // 
            // dgvPayments
            // 
            this.dgvPayments.AllowUserToAddRows = false;
            this.dgvPayments.AllowUserToDeleteRows = false;
            this.dgvPayments.AllowUserToOrderColumns = true;
            this.dgvPayments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPayments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPayments.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPayments.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayments.EnableHeadersVisualStyles = false;
            this.dgvPayments.Location = new System.Drawing.Point(290, 86);
            this.dgvPayments.MultiSelect = false;
            this.dgvPayments.Name = "dgvPayments";
            this.dgvPayments.ReadOnly = true;
            this.dgvPayments.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvPayments.RowHeadersVisible = false;
            this.dgvPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayments.Size = new System.Drawing.Size(997, 543);
            this.dgvPayments.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnReconnect,
            this.statusLabel,
            this.tsProgressBar,
            this.tsPaymentStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 632);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1299, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // btnReconnect
            // 
            this.btnReconnect.Name = "btnReconnect";
            this.btnReconnect.Size = new System.Drawing.Size(16, 20);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(118, 17);
            this.statusLabel.Text = "toolStripStatusLabel1";
            // 
            // tsProgressBar
            // 
            this.tsProgressBar.Name = "tsProgressBar";
            this.tsProgressBar.Size = new System.Drawing.Size(200, 16);
            // 
            // tsPaymentStatus
            // 
            this.tsPaymentStatus.Name = "tsPaymentStatus";
            this.tsPaymentStatus.Size = new System.Drawing.Size(118, 17);
            this.tsPaymentStatus.Text = "toolStripStatusLabel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.btnStudents);
            this.panel1.Controls.Add(this.btnGroups);
            this.panel1.Controls.Add(this.btnGroupsEdit);
            this.panel1.Location = new System.Drawing.Point(12, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(272, 582);
            this.panel1.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblPaymentNextDate);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(37, 462);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 100);
            this.panel3.TabIndex = 5;
            // 
            // lblPaymentNextDate
            // 
            this.lblPaymentNextDate.AutoSize = true;
            this.lblPaymentNextDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaymentNextDate.Location = new System.Drawing.Point(40, 46);
            this.lblPaymentNextDate.Name = "lblPaymentNextDate";
            this.lblPaymentNextDate.Size = new System.Drawing.Size(51, 20);
            this.lblPaymentNextDate.TabIndex = 2;
            this.lblPaymentNextDate.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "გადახდის დაწყების თარიღი";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbStartStudyDate);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(37, 356);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 100);
            this.panel2.TabIndex = 4;
            // 
            // lbStartStudyDate
            // 
            this.lbStartStudyDate.AutoSize = true;
            this.lbStartStudyDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStartStudyDate.Location = new System.Drawing.Point(40, 43);
            this.lbStartStudyDate.Name = "lbStartStudyDate";
            this.lbStartStudyDate.Size = new System.Drawing.Size(51, 20);
            this.lbStartStudyDate.TabIndex = 2;
            this.lbStartStudyDate.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "სწავლის დაწყების თარიღი";
            // 
            // btnPaymentHistory
            // 
            this.btnPaymentHistory.Location = new System.Drawing.Point(12, 100);
            this.btnPaymentHistory.Name = "btnPaymentHistory";
            this.btnPaymentHistory.Size = new System.Drawing.Size(200, 30);
            this.btnPaymentHistory.TabIndex = 1;
            this.btnPaymentHistory.Text = "გადახდების ისტორია";
            this.btnPaymentHistory.UseVisualStyleBackColor = true;
            // 
            // btnRefreshPaymentProcess
            // 
            this.btnRefreshPaymentProcess.Location = new System.Drawing.Point(1202, 57);
            this.btnRefreshPaymentProcess.Name = "btnRefreshPaymentProcess";
            this.btnRefreshPaymentProcess.Size = new System.Drawing.Size(85, 23);
            this.btnRefreshPaymentProcess.TabIndex = 6;
            this.btnRefreshPaymentProcess.Text = "განახლება";
            this.btnRefreshPaymentProcess.UseVisualStyleBackColor = true;
            this.btnRefreshPaymentProcess.Click += new System.EventHandler(this.btnRefreshPaymentProcess_Click);
            // 
            // StudentAmount
            // 
            this.StudentAmount.AutoSize = true;
            this.StudentAmount.Location = new System.Drawing.Point(302, 62);
            this.StudentAmount.Name = "StudentAmount";
            this.StudentAmount.Size = new System.Drawing.Size(35, 13);
            this.StudentAmount.TabIndex = 7;
            this.StudentAmount.Text = "label3";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1299, 654);
            this.Controls.Add(this.StudentAmount);
            this.Controls.Add(this.btnRefreshPaymentProcess);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dgvPayments);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnPaymentHistory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ბოლნისის კულტურის ცენტრი - მოსწავლეთა მართვის პროგრამა";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStudents;
        private System.Windows.Forms.Button btnGroups;
        private System.Windows.Forms.Button btnGroupsEdit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem მთავარიToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem სტატისტიკაToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem გადახდებიToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgvPayments;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbStartStudyDate;
        private System.Windows.Forms.ToolStripMenuItem გადახდაToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem გადახდისტესტირებაToolStripMenuItem;
        private System.Windows.Forms.Label lblPaymentNextDate;
        private System.Windows.Forms.ToolStripMenuItem tsmAdminPanel;
        private System.ComponentModel.BackgroundWorker bWPayment;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripSplitButton btnReconnect;
        private System.Windows.Forms.ToolStripProgressBar tsProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tsPaymentStatus;
        private System.Windows.Forms.Button btnPaymentHistory;
        private System.Windows.Forms.ToolStripMenuItem ბექაპისმართვაToolStripMenuItem;
        private System.Windows.Forms.Button btnRefreshPaymentProcess;
        private System.Windows.Forms.ToolStripMenuItem ლოგებიToolStripMenuItem;
        private System.Windows.Forms.Label StudentAmount;
    }
}


