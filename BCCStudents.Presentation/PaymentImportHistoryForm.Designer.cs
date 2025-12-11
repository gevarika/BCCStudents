namespace BCCStudents.Presentation
{
    partial class PaymentImportHistoryForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSuccessful = new System.Windows.Forms.TabPage();
            this.dgvSuccessful = new System.Windows.Forms.DataGridView();
            this.tabFailed = new System.Windows.Forms.TabPage();
            this.dgvFailed = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatistics = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabSuccessful.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSuccessful)).BeginInit();
            this.tabFailed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFailed)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSuccessful);
            this.tabControl.Controls.Add(this.tabFailed);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 60);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1359, 628);
            this.tabControl.TabIndex = 0;
            // 
            // tabSuccessful
            // 
            this.tabSuccessful.Controls.Add(this.dgvSuccessful);
            this.tabSuccessful.Location = new System.Drawing.Point(4, 22);
            this.tabSuccessful.Name = "tabSuccessful";
            this.tabSuccessful.Padding = new System.Windows.Forms.Padding(3);
            this.tabSuccessful.Size = new System.Drawing.Size(1351, 602);
            this.tabSuccessful.TabIndex = 0;
            this.tabSuccessful.Text = "áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜";
            this.tabSuccessful.UseVisualStyleBackColor = true;
            // 
            // dgvSuccessful
            // 
            this.dgvSuccessful.AllowUserToAddRows = false;
            this.dgvSuccessful.AllowUserToDeleteRows = false;
            this.dgvSuccessful.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSuccessful.BackgroundColor = System.Drawing.Color.White;
            this.dgvSuccessful.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSuccessful.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSuccessful.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSuccessful.Location = new System.Drawing.Point(3, 3);
            this.dgvSuccessful.Name = "dgvSuccessful";
            this.dgvSuccessful.ReadOnly = true;
            this.dgvSuccessful.RowHeadersVisible = false;
            this.dgvSuccessful.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSuccessful.Size = new System.Drawing.Size(1345, 596);
            this.dgvSuccessful.TabIndex = 0;
            // 
            // tabFailed
            // 
            this.tabFailed.Controls.Add(this.dgvFailed);
            this.tabFailed.Location = new System.Drawing.Point(4, 22);
            this.tabFailed.Name = "tabFailed";
            this.tabFailed.Padding = new System.Windows.Forms.Padding(3);
            this.tabFailed.Size = new System.Drawing.Size(1159, 547);
            this.tabFailed.TabIndex = 1;
            this.tabFailed.Text = "áƒ•áƒ”áƒ  áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜";
            this.tabFailed.UseVisualStyleBackColor = true;
            // 
            // dgvFailed
            // 
            this.dgvFailed.AllowUserToAddRows = false;
            this.dgvFailed.AllowUserToDeleteRows = false;
            this.dgvFailed.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFailed.BackgroundColor = System.Drawing.Color.White;
            this.dgvFailed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFailed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFailed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFailed.Location = new System.Drawing.Point(3, 3);
            this.dgvFailed.Name = "dgvFailed";
            this.dgvFailed.ReadOnly = true;
            this.dgvFailed.RowHeadersVisible = false;
            this.dgvFailed.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFailed.Size = new System.Drawing.Size(1153, 541);
            this.dgvFailed.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatistics);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dtpTo);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dtpFrom);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1359, 60);
            this.panel1.TabIndex = 1;
            // 
            // lblStatistics
            // 
            this.lblStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatistics.AutoSize = true;
            this.lblStatistics.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatistics.Location = new System.Drawing.Point(775, 22);
            this.lblStatistics.Name = "lblStatistics";
            this.lblStatistics.Size = new System.Drawing.Size(0, 15);
            this.lblStatistics.TabIndex = 7;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(1272, 19);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(443, 20);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 20);
            this.txtSearch.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(393, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "áƒ«áƒ˜áƒ”áƒ‘áƒ:";
            // 
            // dtpTo
            // 
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(287, 19);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(100, 20);
            this.dtpTo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "áƒ“áƒáƒœ";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(142, 19);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(100, 20);
            this.dtpFrom.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ“áƒ˜áƒáƒžáƒáƒ–áƒáƒœáƒ˜:";
            // 
            // PaymentImportHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1359, 688);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "PaymentImportHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒ˜áƒ¡áƒ¢áƒáƒ áƒ˜áƒ";
            this.tabControl.ResumeLayout(false);
            this.tabSuccessful.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSuccessful)).EndInit();
            this.tabFailed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFailed)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSuccessful;
        private System.Windows.Forms.DataGridView dgvSuccessful;
        private System.Windows.Forms.TabPage tabFailed;
        private System.Windows.Forms.DataGridView dgvFailed;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatistics;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label1;
    }
} 
