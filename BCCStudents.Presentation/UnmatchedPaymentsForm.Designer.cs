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
            this.dgvFailedPayments = new System.Windows.Forms.DataGridView();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.lstSimilarStudents = new System.Windows.Forms.ListBox();
            this.btnFindSimilar = new System.Windows.Forms.Button();
            this.btnAttachToStudent = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblSimilar = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFailedPayments)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFailedPayments
            // 
            this.dgvFailedPayments.AllowUserToAddRows = false;
            this.dgvFailedPayments.AllowUserToDeleteRows = false;
            this.dgvFailedPayments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvFailedPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFailedPayments.Location = new System.Drawing.Point(9, 10);
            this.dgvFailedPayments.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvFailedPayments.MultiSelect = false;
            this.dgvFailedPayments.Name = "dgvFailedPayments";
            this.dgvFailedPayments.ReadOnly = true;
            this.dgvFailedPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFailedPayments.Size = new System.Drawing.Size(908, 753);
            this.dgvFailedPayments.TabIndex = 0;
            this.dgvFailedPayments.SelectionChanged += new System.EventHandler(this.dgvFailedPayments_SelectionChanged);
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(986, 26);
            this.txtDetails.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(441, 223);
            this.txtDetails.TabIndex = 1;
            // 
            // lstSimilarStudents
            // 
            this.lstSimilarStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSimilarStudents.FormattingEnabled = true;
            this.lstSimilarStudents.Location = new System.Drawing.Point(987, 301);
            this.lstSimilarStudents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstSimilarStudents.Name = "lstSimilarStudents";
            this.lstSimilarStudents.Size = new System.Drawing.Size(441, 329);
            this.lstSimilarStudents.TabIndex = 3;
            // 
            // btnFindSimilar
            // 
            this.btnFindSimilar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindSimilar.Location = new System.Drawing.Point(1187, 253);
            this.btnFindSimilar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFindSimilar.Name = "btnFindSimilar";
            this.btnFindSimilar.Size = new System.Drawing.Size(240, 24);
            this.btnFindSimilar.TabIndex = 2;
            this.btnFindSimilar.Text = "áƒ˜áƒžáƒáƒ•áƒ” áƒ›áƒ¡áƒ’áƒáƒ•áƒ¡áƒ˜ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ”áƒ‘áƒ˜";
            this.btnFindSimilar.UseVisualStyleBackColor = true;
            this.btnFindSimilar.Click += new System.EventHandler(this.btnFindSimilar_Click);
            // 
            // btnAttachToStudent
            // 
            this.btnAttachToStudent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAttachToStudent.Location = new System.Drawing.Point(1187, 651);
            this.btnAttachToStudent.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAttachToStudent.Name = "btnAttachToStudent";
            this.btnAttachToStudent.Size = new System.Drawing.Size(240, 24);
            this.btnAttachToStudent.TabIndex = 4;
            this.btnAttachToStudent.Text = "áƒ›áƒ˜áƒ›áƒáƒ’áƒ áƒ”áƒ‘áƒ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ–áƒ”";
            this.btnAttachToStudent.UseVisualStyleBackColor = true;
            this.btnAttachToStudent.Click += new System.EventHandler(this.btnAttachToStudent_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(1187, 692);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(240, 24);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblDetails
            // 
            this.lblDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(1186, 10);
            this.lblDetails.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(67, 13);
            this.lblDetails.TabIndex = 6;
            this.lblDetails.Text = "áƒ“áƒ”áƒ¢áƒáƒšáƒ”áƒ‘áƒ˜";
            // 
            // lblSimilar
            // 
            this.lblSimilar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSimilar.AutoSize = true;
            this.lblSimilar.Location = new System.Drawing.Point(1187, 285);
            this.lblSimilar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSimilar.Name = "lblSimilar";
            this.lblSimilar.Size = new System.Drawing.Size(122, 13);
            this.lblSimilar.TabIndex = 7;
            this.lblSimilar.Text = "áƒ›áƒ¡áƒ’áƒáƒ•áƒ¡áƒ˜ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ”áƒ‘áƒ˜";
            // 
            // UnmatchedPaymentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1442, 778);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnAttachToStudent);
            this.Controls.Add(this.lstSimilarStudents);
            this.Controls.Add(this.lblSimilar);
            this.Controls.Add(this.btnFindSimilar);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.dgvFailedPayments);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UnmatchedPaymentsForm";
            this.Text = "áƒ“áƒáƒ£áƒ“áƒáƒ¡áƒ¢áƒ£áƒ áƒ”áƒ‘áƒ”áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFailedPayments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
} 
