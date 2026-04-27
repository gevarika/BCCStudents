namespace BCCStudents.Presentation
{
    partial class UnmatchedPaymentsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvFailedPayments;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.ListBox lstSimilarStudents;
        private System.Windows.Forms.Button btnFindSimilar;
        private System.Windows.Forms.Button btnAttachToStudent;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblSimilar;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnmatchedPaymentsForm));
            dgvFailedPayments = new DataGridView();
            txtDetails = new TextBox();
            lstSimilarStudents = new ListBox();
            btnFindSimilar = new Button();
            btnAttachToStudent = new Button();
            btnRefresh = new Button();
            lblDetails = new Label();
            lblSimilar = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvFailedPayments).BeginInit();
            SuspendLayout();
            // 
            // dgvFailedPayments
            // 
            dgvFailedPayments.AllowUserToAddRows = false;
            dgvFailedPayments.AllowUserToDeleteRows = false;
            dgvFailedPayments.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvFailedPayments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFailedPayments.Location = new Point(10, 12);
            dgvFailedPayments.Margin = new Padding(2);
            dgvFailedPayments.MultiSelect = false;
            dgvFailedPayments.Name = "dgvFailedPayments";
            dgvFailedPayments.ReadOnly = true;
            dgvFailedPayments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFailedPayments.Size = new Size(1059, 869);
            dgvFailedPayments.TabIndex = 0;
            dgvFailedPayments.SelectionChanged += dgvFailedPayments_SelectionChanged;
            // 
            // txtDetails
            // 
            txtDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtDetails.Location = new Point(1150, 30);
            txtDetails.Margin = new Padding(2);
            txtDetails.Multiline = true;
            txtDetails.Name = "txtDetails";
            txtDetails.ReadOnly = true;
            txtDetails.ScrollBars = ScrollBars.Vertical;
            txtDetails.Size = new Size(514, 257);
            txtDetails.TabIndex = 1;
            // 
            // lstSimilarStudents
            // 
            lstSimilarStudents.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lstSimilarStudents.FormattingEnabled = true;
            lstSimilarStudents.ItemHeight = 15;
            lstSimilarStudents.Location = new Point(1152, 347);
            lstSimilarStudents.Margin = new Padding(2);
            lstSimilarStudents.Name = "lstSimilarStudents";
            lstSimilarStudents.Size = new Size(514, 379);
            lstSimilarStudents.TabIndex = 3;
            // 
            // btnFindSimilar
            // 
            btnFindSimilar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFindSimilar.Location = new Point(1385, 292);
            btnFindSimilar.Margin = new Padding(2);
            btnFindSimilar.Name = "btnFindSimilar";
            btnFindSimilar.Size = new Size(280, 28);
            btnFindSimilar.TabIndex = 2;
            btnFindSimilar.Text = "იპოვე მსგავსი სტუდენტები";
            btnFindSimilar.UseVisualStyleBackColor = true;
            btnFindSimilar.Click += btnFindSimilar_Click;
            // 
            // btnAttachToStudent
            // 
            btnAttachToStudent.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAttachToStudent.Location = new Point(1385, 751);
            btnAttachToStudent.Margin = new Padding(2);
            btnAttachToStudent.Name = "btnAttachToStudent";
            btnAttachToStudent.Size = new Size(280, 28);
            btnAttachToStudent.TabIndex = 4;
            btnAttachToStudent.Text = "მიმაგრება სტუდენტზე";
            btnAttachToStudent.UseVisualStyleBackColor = true;
            btnAttachToStudent.Click += btnAttachToStudent_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Location = new Point(1385, 798);
            btnRefresh.Margin = new Padding(2);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(280, 28);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "განახლება";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblDetails
            // 
            lblDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblDetails.AutoSize = true;
            lblDetails.Location = new Point(1384, 12);
            lblDetails.Margin = new Padding(2, 0, 2, 0);
            lblDetails.Name = "lblDetails";
            lblDetails.Size = new Size(72, 15);
            lblDetails.TabIndex = 6;
            lblDetails.Text = "დეტალები";
            // 
            // lblSimilar
            // 
            lblSimilar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblSimilar.AutoSize = true;
            lblSimilar.Location = new Point(1385, 329);
            lblSimilar.Margin = new Padding(2, 0, 2, 0);
            lblSimilar.Name = "lblSimilar";
            lblSimilar.Size = new Size(136, 15);
            lblSimilar.TabIndex = 7;
            lblSimilar.Text = "მსგავსი სტუდენტები";
            // 
            // UnmatchedPaymentsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1682, 898);
            Controls.Add(btnRefresh);
            Controls.Add(btnAttachToStudent);
            Controls.Add(lstSimilarStudents);
            Controls.Add(lblSimilar);
            Controls.Add(btnFindSimilar);
            Controls.Add(txtDetails);
            Controls.Add(lblDetails);
            Controls.Add(dgvFailedPayments);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "UnmatchedPaymentsForm";
            Text = "დაუდასტურებელი გადახდები";
            ((System.ComponentModel.ISupportInitialize)dgvFailedPayments).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
    }
} 
