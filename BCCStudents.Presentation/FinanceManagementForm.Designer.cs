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
            this.dgvPayments = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.მენიუToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.გადახდებისისტორიაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.დაუდასტურებელიგადახდებიToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.მენიუToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1070, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // მენიუToolStripMenuItem
            // 
            this.მენიუToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.გადახდებისისტორიაToolStripMenuItem,
            this.დაუდასტურებელიგადახდებიToolStripMenuItem});
            this.მენიუToolStripMenuItem.Name = "მენიუToolStripMenuItem";
            this.მენიუToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.მენიუToolStripMenuItem.Text = "მენიუ";
            // 
            // გადახდებისისტორიაToolStripMenuItem
            // 
            this.გადახდებისისტორიაToolStripMenuItem.Name = "გადახდებისისტორიაToolStripMenuItem";
            this.გადახდებისისტორიაToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.გადახდებისისტორიაToolStripMenuItem.Text = "გადახდების ისტორია";
            this.გადახდებისისტორიაToolStripMenuItem.Click += new System.EventHandler(this.გადახდებისისტორიაToolStripMenuItem_Click);
            // 
            // დაუდასტურებელიგადახდებიToolStripMenuItem
            // 
            this.დაუდასტურებელიგადახდებიToolStripMenuItem.Name = "დაუდასტურებელიგადახდებიToolStripMenuItem";
            this.დაუდასტურებელიგადახდებიToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.დაუდასტურებელიგადახდებიToolStripMenuItem.Text = "დაუდასტურებელი გადახდები";
            this.დაუდასტურებელიგადახდებიToolStripMenuItem.Click += new System.EventHandler(this.დაუდასტურებელიგადახდებიToolStripMenuItem_Click);
            // 
            // dgvPayments
            // 
            this.dgvPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPayments.Location = new System.Drawing.Point(12, 61);
            this.dgvPayments.Name = "dgvPayments";
            this.dgvPayments.Size = new System.Drawing.Size(1046, 732);
            this.dgvPayments.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "განახლება";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // FinanceManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 805);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.dgvPayments);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FinanceManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ფინანსების მართვა";
            this.Load += new System.EventHandler(this.FinanceManagementForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
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
