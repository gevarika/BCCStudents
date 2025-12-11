namespace BCCStudents.Presentation
{
    partial class ImportForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSheetMapping = new System.Windows.Forms.Panel();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.chkEnableMapping = new System.Windows.Forms.CheckBox();
            this.chkIsActive = new System.Windows.Forms.CheckBox();
            this.tabWorksheets = new System.Windows.Forms.TabControl();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.syncStatusLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnImport = new System.Windows.Forms.Button();
            this.pnlOptions.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(5);
            this.lblTitle.Size = new System.Drawing.Size(1644, 35);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Excel ფაილიდან სტუდენტების იმპორტი";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSheetMapping
            // 
            this.pnlSheetMapping.AutoScroll = true;
            this.pnlSheetMapping.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSheetMapping.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSheetMapping.Location = new System.Drawing.Point(10, 45);
            this.pnlSheetMapping.Name = "pnlSheetMapping";
            this.pnlSheetMapping.Padding = new System.Windows.Forms.Padding(10);
            this.pnlSheetMapping.Size = new System.Drawing.Size(1644, 350);
            this.pnlSheetMapping.TabIndex = 1;
            // 
            // pnlOptions
            // 
            this.pnlOptions.Controls.Add(this.chkEnableMapping);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptions.Location = new System.Drawing.Point(10, 395);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pnlOptions.Size = new System.Drawing.Size(1644, 40);
            this.pnlOptions.TabIndex = 2;
            // 
            // chkEnableMapping
            // 
            this.chkEnableMapping.AutoSize = true;
            this.chkEnableMapping.Checked = true;
            this.chkEnableMapping.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableMapping.Location = new System.Drawing.Point(23, 13);
            this.chkEnableMapping.Name = "chkEnableMapping";
            this.chkEnableMapping.Size = new System.Drawing.Size(179, 17);
            this.chkEnableMapping.TabIndex = 0;
            this.chkEnableMapping.Text = "გამოიყენე სვეტების მეპინგი";
            this.chkEnableMapping.UseVisualStyleBackColor = true;
            // 
            // chkIsActive
            // 
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Checked = true;
            this.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsActive.Location = new System.Drawing.Point(220, 8);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new System.Drawing.Size(270, 17);
            this.chkIsActive.TabIndex = 1;
            this.chkIsActive.Text = "იმპორტირებული სტუდენტები იყოს აქტიური";
            this.chkIsActive.UseVisualStyleBackColor = true;
            // 
            // tabWorksheets
            // 
            this.tabWorksheets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabWorksheets.Location = new System.Drawing.Point(10, 435);
            this.tabWorksheets.Name = "tabWorksheets";
            this.tabWorksheets.SelectedIndex = 0;
            this.tabWorksheets.Size = new System.Drawing.Size(1644, 300);
            this.tabWorksheets.TabIndex = 3;
            // 
            // pnlStatus
            // 
            this.pnlStatus.Controls.Add(this.syncStatusLabel);
            this.pnlStatus.Controls.Add(this.statusLabel);
            this.pnlStatus.Controls.Add(this.progressBar);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(10, 735);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(5);
            this.pnlStatus.Size = new System.Drawing.Size(1644, 70);
            this.pnlStatus.TabIndex = 4;
            // 
            // syncStatusLabel
            // 
            this.syncStatusLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.syncStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syncStatusLabel.ForeColor = System.Drawing.Color.DarkBlue;
            this.syncStatusLabel.Location = new System.Drawing.Point(5, 25);
            this.syncStatusLabel.Name = "syncStatusLabel";
            this.syncStatusLabel.Size = new System.Drawing.Size(1634, 20);
            this.syncStatusLabel.TabIndex = 2;
            this.syncStatusLabel.Text = "სინქრონიზაცია: მზადაა";
            this.syncStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusLabel.Location = new System.Drawing.Point(5, 5);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(1634, 20);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "მზადაა";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(5, 45);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1634, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnImport);
            this.pnlBottom.Controls.Add(this.chkIsActive);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(10, 805);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBottom.Size = new System.Drawing.Size(1644, 50);
            this.pnlBottom.TabIndex = 6;
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.Location = new System.Drawing.Point(1458, 10);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(176, 30);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "იმპორტის დაწყება";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1664, 865);
            this.Controls.Add(this.tabWorksheets);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.pnlSheetMapping);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlBottom);
            this.Name = "ImportForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "სტუდენტების იმპორტი";
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSheetMapping;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.CheckBox chkEnableMapping;
        private System.Windows.Forms.CheckBox chkIsActive;
        private System.Windows.Forms.TabControl tabWorksheets;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label syncStatusLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnImport;
    }
}

