namespace BCCStudents.Presentation
{
    partial class FinanceManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinanceManagementForm));
            dgvPayments = new DataGridView();
            btnRefresh = new Button();
            menuStrip1 = new MenuStrip();
            მენიუToolStripMenuItem = new ToolStripMenuItem();
            გადახდებისისტორიაToolStripMenuItem = new ToolStripMenuItem();
            დაუდასტურებელიგადახდებიToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dgvPayments).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvPayments
            // 
            dgvPayments.AllowUserToAddRows = false;
            dgvPayments.AllowUserToDeleteRows = false;
            dgvPayments.BorderStyle = BorderStyle.None;
            dgvPayments.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvPayments.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPayments.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPayments.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvPayments.Location = new Point(14, 70);
            dgvPayments.Margin = new Padding(4, 3, 4, 3);
            dgvPayments.Name = "dgvPayments";
            dgvPayments.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPayments.Size = new Size(1220, 845);
            dgvPayments.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(14, 37);
            btnRefresh.Margin = new Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(88, 27);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "განახლება";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { მენიუToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1248, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // მენიუToolStripMenuItem
            // 
            მენიუToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { გადახდებისისტორიაToolStripMenuItem, დაუდასტურებელიგადახდებიToolStripMenuItem });
            მენიუToolStripMenuItem.Name = "მენიუToolStripMenuItem";
            მენიუToolStripMenuItem.Size = new Size(56, 20);
            მენიუToolStripMenuItem.Text = "მენიუ";
            // 
            // გადახდებისისტორიაToolStripMenuItem
            // 
            გადახდებისისტორიაToolStripMenuItem.Name = "გადახდებისისტორიაToolStripMenuItem";
            გადახდებისისტორიაToolStripMenuItem.Size = new Size(263, 22);
            გადახდებისისტორიაToolStripMenuItem.Text = "გადახდების ისტორია";
            გადახდებისისტორიაToolStripMenuItem.Click += გადახდებისისტორიაToolStripMenuItem_Click;
            // 
            // დაუდასტურებელიგადახდებიToolStripMenuItem
            // 
            დაუდასტურებელიგადახდებიToolStripMenuItem.Name = "დაუდასტურებელიგადახდებიToolStripMenuItem";
            დაუდასტურებელიგადახდებიToolStripMenuItem.Size = new Size(263, 22);
            დაუდასტურებელიგადახდებიToolStripMenuItem.Text = "დაუდასტურებელი გადახდები";
            დაუდასტურებელიგადახდებიToolStripMenuItem.Click += დაუდასტურებელიგადახდებიToolStripMenuItem_Click;
            // 
            // FinanceManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1248, 929);
            Controls.Add(btnRefresh);
            Controls.Add(dgvPayments);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FinanceManagementForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ფინანსების მართვა";
            Load += FinanceManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPayments).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPayments;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem მენიუToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem გადახდებისისტორიაToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem დაუდასტურებელიგადახდებიToolStripMenuItem;
    }
}
