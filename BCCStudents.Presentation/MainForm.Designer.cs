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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            contextMenuStrip1 = new ContextMenuStrip(components);
            სვეტებისმართვაToolStripMenuItem = new ToolStripMenuItem();
            btnStudents = new Button();
            btnGroups = new Button();
            btnGroupsEdit = new Button();
            menuStrip1 = new MenuStrip();
            მთავარიToolStripMenuItem = new ToolStripMenuItem();
            StatisticToolStripMenuItem = new ToolStripMenuItem();
            PaymentsToolStripMenuItem = new ToolStripMenuItem();
            PaymentToolStripMenuItem = new ToolStripMenuItem();
            balanceTransferToolStripMenuItem = new ToolStripMenuItem();
            PaymentTestToolStripMenuItem = new ToolStripMenuItem();
            tsmAdminPanel = new ToolStripMenuItem();
            LogsToolStripMenuItem = new ToolStripMenuItem();
            userManagementToolStripMenuItem = new ToolStripMenuItem();
            dgvPayments = new DataGridView();
            statusStrip1 = new StatusStrip();
            btnReconnect = new ToolStripSplitButton();
            statusLabel = new ToolStripStatusLabel();
            serverStatusLabel = new ToolStripStatusLabel();
            tsProgressBar = new ToolStripProgressBar();
            tsPaymentStatus = new ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            panel3 = new Panel();
            lblPaymentNextDate = new Label();
            label2 = new Label();
            panel2 = new Panel();
            lbStartStudyDate = new Label();
            label1 = new Label();
            bWPayment = new System.ComponentModel.BackgroundWorker();
            btnPaymentHistory = new Button();
            btnRefreshPaymentProcess = new Button();
            StudentAmount = new Label();
            panel4 = new Panel();
            panel5 = new Panel();
            contextMenuStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPayments).BeginInit();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { სვეტებისმართვაToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(178, 26);
            // 
            // სვეტებისმართვაToolStripMenuItem
            // 
            სვეტებისმართვაToolStripMenuItem.Name = "სვეტებისმართვაToolStripMenuItem";
            სვეტებისმართვაToolStripMenuItem.Size = new Size(177, 22);
            სვეტებისმართვაToolStripMenuItem.Text = "სვეტების მართვა";
            სვეტებისმართვაToolStripMenuItem.Click += ManageColumns_Click;
            // 
            // btnStudents
            // 
            btnStudents.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStudents.Location = new Point(7, 42);
            btnStudents.Margin = new Padding(4, 3, 4, 3);
            btnStudents.Name = "btnStudents";
            btnStudents.Size = new Size(310, 58);
            btnStudents.TabIndex = 0;
            btnStudents.Text = "მოსწავლეები";
            btnStudents.UseVisualStyleBackColor = true;
            btnStudents.Click += btnStudents_Click;
            // 
            // btnGroups
            // 
            btnGroups.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGroups.Location = new Point(7, 107);
            btnGroups.Margin = new Padding(4, 3, 4, 3);
            btnGroups.Name = "btnGroups";
            btnGroups.Size = new Size(310, 58);
            btnGroups.TabIndex = 1;
            btnGroups.Text = "ჯგუფები";
            btnGroups.UseVisualStyleBackColor = true;
            btnGroups.Click += btnGroups_Click;
            // 
            // btnGroupsEdit
            // 
            btnGroupsEdit.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGroupsEdit.Location = new Point(7, 172);
            btnGroupsEdit.Margin = new Padding(4, 3, 4, 3);
            btnGroupsEdit.Name = "btnGroupsEdit";
            btnGroupsEdit.Size = new Size(310, 58);
            btnGroupsEdit.TabIndex = 2;
            btnGroupsEdit.Text = "ჯგუფების რედაქტირება";
            btnGroupsEdit.UseVisualStyleBackColor = true;
            btnGroupsEdit.Click += btnGroupsEdit_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { მთავარიToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1334, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // მთავარიToolStripMenuItem
            // 
            მთავარიToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { StatisticToolStripMenuItem, PaymentsToolStripMenuItem, PaymentToolStripMenuItem, balanceTransferToolStripMenuItem, PaymentTestToolStripMenuItem, tsmAdminPanel, LogsToolStripMenuItem, userManagementToolStripMenuItem });
            მთავარიToolStripMenuItem.Name = "მთავარიToolStripMenuItem";
            მთავარიToolStripMenuItem.Size = new Size(71, 20);
            მთავარიToolStripMenuItem.Text = "მთავარი";
            // 
            // StatisticToolStripMenuItem
            // 
            StatisticToolStripMenuItem.Name = "StatisticToolStripMenuItem";
            StatisticToolStripMenuItem.Size = new Size(214, 22);
            StatisticToolStripMenuItem.Text = "სტატისტიკა";
            StatisticToolStripMenuItem.Click += StatisticToolStripMenuItem_Click;
            // 
            // PaymentsToolStripMenuItem
            // 
            PaymentsToolStripMenuItem.Name = "PaymentsToolStripMenuItem";
            PaymentsToolStripMenuItem.Size = new Size(214, 22);
            PaymentsToolStripMenuItem.Text = "გადახდები";
            PaymentsToolStripMenuItem.Click += PaymentsToolStripMenuItem_Click;
            // 
            // PaymentToolStripMenuItem
            // 
            PaymentToolStripMenuItem.Name = "PaymentToolStripMenuItem";
            PaymentToolStripMenuItem.Size = new Size(214, 22);
            PaymentToolStripMenuItem.Text = "გადახდა";
            PaymentToolStripMenuItem.Click += PaymentToolStripMenuItem_Click;
            // 
            // balanceTransferToolStripMenuItem
            // 
            balanceTransferToolStripMenuItem.Name = "balanceTransferToolStripMenuItem";
            balanceTransferToolStripMenuItem.Size = new Size(214, 22);
            balanceTransferToolStripMenuItem.Text = "ბალანსის გადატანა";
            balanceTransferToolStripMenuItem.Click += balanceTransferToolStripMenuItem_Click;
            // 
            // PaymentTestToolStripMenuItem
            // 
            PaymentTestToolStripMenuItem.Name = "PaymentTestToolStripMenuItem";
            PaymentTestToolStripMenuItem.Size = new Size(214, 22);
            PaymentTestToolStripMenuItem.Text = "გადახდის ტესტირება";
            PaymentTestToolStripMenuItem.Click += PaymentTestToolStripMenuItem_Click;
            // 
            // tsmAdminPanel
            // 
            tsmAdminPanel.Name = "tsmAdminPanel";
            tsmAdminPanel.Size = new Size(214, 22);
            tsmAdminPanel.Text = "ადმინპანელი";
            tsmAdminPanel.Click += tsmAdminPanel_Click;
            // 
            // LogsToolStripMenuItem
            // 
            LogsToolStripMenuItem.Name = "LogsToolStripMenuItem";
            LogsToolStripMenuItem.Size = new Size(214, 22);
            LogsToolStripMenuItem.Text = "ლოგები";
            LogsToolStripMenuItem.Click += LogsToolStripMenuItem_Click;
            // 
            // userManagementToolStripMenuItem
            // 
            userManagementToolStripMenuItem.Name = "userManagementToolStripMenuItem";
            userManagementToolStripMenuItem.Size = new Size(214, 22);
            userManagementToolStripMenuItem.Text = "მომხმარებლის მართვა";
            userManagementToolStripMenuItem.Click += userManagementToolStripMenuItem_Click;
            // 
            // dgvPayments
            // 
            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.AllowUserToDeleteRows = false;
            dgvPayments.AllowUserToOrderColumns = true;
            dgvPayments.BorderStyle = BorderStyle.None;
            dgvPayments.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvPayments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPayments.ContextMenuStrip = contextMenuStrip1;
            dgvPayments.Dock = DockStyle.Fill;
            dgvPayments.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvPayments.EnableHeadersVisualStyles = false;
            dgvPayments.Location = new Point(0, 0);
            dgvPayments.Margin = new Padding(4, 3, 4, 3);
            dgvPayments.MultiSelect = false;
            dgvPayments.Name = "dgvPayments";
            dgvPayments.ReadOnly = true;
            dgvPayments.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPayments.RowHeadersVisible = false;
            dgvPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayments.Size = new Size(1017, 677);
            dgvPayments.TabIndex = 3;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { btnReconnect, statusLabel, serverStatusLabel, tsProgressBar, tsPaymentStatus });
            statusStrip1.Location = new Point(0, 737);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1334, 24);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // btnReconnect
            // 
            btnReconnect.Name = "btnReconnect";
            btnReconnect.Size = new Size(16, 22);
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(118, 19);
            statusLabel.Text = "toolStripStatusLabel1";
            // 
            // serverStatusLabel
            // 
            serverStatusLabel.Margin = new Padding(8, 3, 0, 2);
            serverStatusLabel.Name = "serverStatusLabel";
            serverStatusLabel.Size = new Size(118, 19);
            serverStatusLabel.Text = "toolStripStatusLabel2";
            // 
            // tsProgressBar
            // 
            tsProgressBar.Name = "tsProgressBar";
            tsProgressBar.Size = new Size(233, 18);
            // 
            // tsPaymentStatus
            // 
            tsPaymentStatus.Name = "tsPaymentStatus";
            tsPaymentStatus.Size = new Size(118, 19);
            tsPaymentStatus.Text = "toolStripStatusLabel1";
            // 
            // panel1
            // 
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(btnStudents);
            panel1.Controls.Add(btnGroups);
            panel1.Controls.Add(btnGroupsEdit);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 24);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(317, 713);
            panel1.TabIndex = 5;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panel3.Controls.Add(lblPaymentNextDate);
            panel3.Controls.Add(label2);
            panel3.Location = new Point(13, 605);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(296, 105);
            panel3.TabIndex = 5;
            // 
            // lblPaymentNextDate
            // 
            lblPaymentNextDate.AutoSize = true;
            lblPaymentNextDate.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPaymentNextDate.Location = new Point(47, 53);
            lblPaymentNextDate.Margin = new Padding(4, 0, 4, 0);
            lblPaymentNextDate.Name = "lblPaymentNextDate";
            lblPaymentNextDate.Size = new Size(51, 20);
            lblPaymentNextDate.TabIndex = 2;
            lblPaymentNextDate.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 21);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(182, 15);
            label2.TabIndex = 3;
            label2.Text = "გადახდის დაწყების თარიღი";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panel2.Controls.Add(lbStartStudyDate);
            panel2.Controls.Add(label1);
            panel2.Location = new Point(13, 484);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(296, 115);
            panel2.TabIndex = 4;
            // 
            // lbStartStudyDate
            // 
            lbStartStudyDate.AutoSize = true;
            lbStartStudyDate.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbStartStudyDate.Location = new Point(47, 50);
            lbStartStudyDate.Margin = new Padding(4, 0, 4, 0);
            lbStartStudyDate.Name = "lbStartStudyDate";
            lbStartStudyDate.Size = new Size(51, 20);
            lbStartStudyDate.TabIndex = 2;
            lbStartStudyDate.Text = "label1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 23);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(174, 15);
            label1.TabIndex = 3;
            label1.Text = "სწავლის დაწყების თარიღი";
            // 
            // btnPaymentHistory
            // 
            btnPaymentHistory.Location = new Point(14, 115);
            btnPaymentHistory.Margin = new Padding(4, 3, 4, 3);
            btnPaymentHistory.Name = "btnPaymentHistory";
            btnPaymentHistory.Size = new Size(233, 35);
            btnPaymentHistory.TabIndex = 1;
            btnPaymentHistory.Text = "გადახდების ისტორია";
            btnPaymentHistory.UseVisualStyleBackColor = true;
            // 
            // btnRefreshPaymentProcess
            // 
            btnRefreshPaymentProcess.Location = new Point(8, 3);
            btnRefreshPaymentProcess.Margin = new Padding(4, 3, 4, 3);
            btnRefreshPaymentProcess.Name = "btnRefreshPaymentProcess";
            btnRefreshPaymentProcess.Size = new Size(99, 27);
            btnRefreshPaymentProcess.TabIndex = 6;
            btnRefreshPaymentProcess.Text = "განახლება";
            btnRefreshPaymentProcess.UseVisualStyleBackColor = true;
            btnRefreshPaymentProcess.Click += btnRefreshPaymentProcess_Click;
            // 
            // StudentAmount
            // 
            StudentAmount.AutoSize = true;
            StudentAmount.Location = new Point(166, 9);
            StudentAmount.Margin = new Padding(4, 0, 4, 0);
            StudentAmount.Name = "StudentAmount";
            StudentAmount.Size = new Size(38, 15);
            StudentAmount.TabIndex = 7;
            StudentAmount.Text = "label3";
            // 
            // panel4
            // 
            panel4.Controls.Add(StudentAmount);
            panel4.Controls.Add(btnRefreshPaymentProcess);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(317, 24);
            panel4.Name = "panel4";
            panel4.Size = new Size(1017, 36);
            panel4.TabIndex = 9;
            // 
            // panel5
            // 
            panel5.Controls.Add(dgvPayments);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(317, 60);
            panel5.Name = "panel5";
            panel5.Size = new Size(1017, 677);
            panel5.TabIndex = 10;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1334, 761);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(btnPaymentHistory);
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(1350, 800);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ბოლნისის კულტურის ცენტრი - მოსწავლეთა მართვის პროგრამა";
            TransparencyKey = Color.White;
            WindowState = FormWindowState.Maximized;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Shown += MainForm_Shown;
            contextMenuStrip1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPayments).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel5.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStudents;
        private System.Windows.Forms.Button btnGroups;
        private System.Windows.Forms.Button btnGroupsEdit;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem მთავარიToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StatisticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PaymentsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem სვეტებისმართვაToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgvPayments;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel serverStatusLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbStartStudyDate;
        private System.Windows.Forms.ToolStripMenuItem PaymentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem balanceTransferToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PaymentTestToolStripMenuItem;
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
        private System.Windows.Forms.Button btnRefreshPaymentProcess;
        private System.Windows.Forms.ToolStripMenuItem LogsToolStripMenuItem;
        private System.Windows.Forms.Label StudentAmount;
        private Panel panel4;
        private Panel panel5;
        private ToolStripMenuItem userManagementToolStripMenuItem;
    }
}


