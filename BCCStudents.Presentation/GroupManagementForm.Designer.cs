namespace BCCStudents.Presentation
{
    partial class GroupManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupManagementForm));
            this.dgvMainGroup = new System.Windows.Forms.DataGridView();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.lblGroupName = new System.Windows.Forms.Label();
            this.numMaxStudents = new System.Windows.Forms.NumericUpDown();
            this.lblStuCount = new System.Windows.Forms.Label();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblTeacher = new System.Windows.Forms.Label();
            this.dgvSubGroups = new System.Windows.Forms.DataGridView();
            this.cmbSubGroupCount = new System.Windows.Forms.ComboBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEditGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.tsEditSubGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.txtTeacher = new System.Windows.Forms.TextBox();
            this.txtDocPath = new System.Windows.Forms.TextBox();
            this.lblDocPath = new System.Windows.Forms.Label();
            this.btnFileDialog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxStudents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubGroups)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMainGroup
            // 
            this.dgvMainGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMainGroup.Location = new System.Drawing.Point(11, 302);
            this.dgvMainGroup.Name = "dgvMainGroup";
            this.dgvMainGroup.Size = new System.Drawing.Size(1029, 184);
            this.dgvMainGroup.TabIndex = 9;
            this.dgvMainGroup.SelectionChanged += new System.EventHandler(this.dataGridViewGroups_SelectionChanged);
            // 
            // txtGroupName
            // 
            this.txtGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGroupName.Location = new System.Drawing.Point(277, 42);
            this.txtGroupName.Multiline = true;
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(194, 30);
            this.txtGroupName.TabIndex = 0;
            // 
            // lblGroupName
            // 
            this.lblGroupName.AutoSize = true;
            this.lblGroupName.Location = new System.Drawing.Point(158, 45);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(96, 13);
            this.lblGroupName.TabIndex = 2;
            this.lblGroupName.Text = "ჯგუფის სახელი";
            // 
            // numMaxStudents
            // 
            this.numMaxStudents.Enabled = false;
            this.numMaxStudents.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numMaxStudents.Location = new System.Drawing.Point(277, 114);
            this.numMaxStudents.Name = "numMaxStudents";
            this.numMaxStudents.Size = new System.Drawing.Size(160, 26);
            this.numMaxStudents.TabIndex = 2;
            // 
            // lblStuCount
            // 
            this.lblStuCount.AutoSize = true;
            this.lblStuCount.Location = new System.Drawing.Point(23, 121);
            this.lblStuCount.Name = "lblStuCount";
            this.lblStuCount.Size = new System.Drawing.Size(231, 13);
            this.lblStuCount.TabIndex = 4;
            this.lblStuCount.Text = "მოსწავლეების მაქსიმალური რაოდენობა";
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Location = new System.Drawing.Point(277, 267);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(127, 29);
            this.btnAddGroup.TabIndex = 7;
            this.btnAddGroup.Text = "დამატება";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // txtPrice
            // 
            this.txtPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrice.Location = new System.Drawing.Point(277, 78);
            this.txtPrice.Multiline = true;
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(160, 30);
            this.txtPrice.TabIndex = 1;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(140, 81);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(114, 13);
            this.lblPrice.TabIndex = 2;
            this.lblPrice.Text = "სწავლის საფასური";
            // 
            // lblTeacher
            // 
            this.lblTeacher.AutoSize = true;
            this.lblTeacher.Location = new System.Drawing.Point(156, 149);
            this.lblTeacher.Name = "lblTeacher";
            this.lblTeacher.Size = new System.Drawing.Size(91, 13);
            this.lblTeacher.TabIndex = 4;
            this.lblTeacher.Text = "მასწავლებელი";
            // 
            // dgvSubGroups
            // 
            this.dgvSubGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubGroups.Location = new System.Drawing.Point(11, 492);
            this.dgvSubGroups.Name = "dgvSubGroups";
            this.dgvSubGroups.Size = new System.Drawing.Size(1029, 217);
            this.dgvSubGroups.TabIndex = 10;
            // 
            // cmbSubGroupCount
            // 
            this.cmbSubGroupCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSubGroupCount.FormattingEnabled = true;
            this.cmbSubGroupCount.ItemHeight = 20;
            this.cmbSubGroupCount.Items.AddRange(new object[] {
            "კლასის გარეშე",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cmbSubGroupCount.Location = new System.Drawing.Point(277, 182);
            this.cmbSubGroupCount.Name = "cmbSubGroupCount";
            this.cmbSubGroupCount.Size = new System.Drawing.Size(187, 28);
            this.cmbSubGroupCount.TabIndex = 4;
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(128, 190);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(126, 13);
            this.lblClass.TabIndex = 4;
            this.lblClass.Text = "კლასების რაოდენობა";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsEdit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1052, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsEdit
            // 
            this.tsEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsEditGroups,
            this.tsEditSubGroups});
            this.tsEdit.Name = "tsEdit";
            this.tsEdit.Size = new System.Drawing.Size(101, 20);
            this.tsEdit.Text = "რედაქტირება";
            // 
            // tsEditGroups
            // 
            this.tsEditGroups.Name = "tsEditGroups";
            this.tsEditGroups.Size = new System.Drawing.Size(131, 22);
            this.tsEditGroups.Text = "ჯგუფები";
            // 
            // tsEditSubGroups
            // 
            this.tsEditSubGroups.Name = "tsEditSubGroups";
            this.tsEditSubGroups.Size = new System.Drawing.Size(131, 22);
            this.tsEditSubGroups.Text = "კლასები";
            // 
            // txtTeacher
            // 
            this.txtTeacher.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTeacher.Location = new System.Drawing.Point(277, 146);
            this.txtTeacher.Multiline = true;
            this.txtTeacher.Name = "txtTeacher";
            this.txtTeacher.Size = new System.Drawing.Size(220, 30);
            this.txtTeacher.TabIndex = 3;
            // 
            // txtDocPath
            // 
            this.txtDocPath.Enabled = false;
            this.txtDocPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDocPath.Location = new System.Drawing.Point(277, 216);
            this.txtDocPath.Multiline = true;
            this.txtDocPath.Name = "txtDocPath";
            this.txtDocPath.Size = new System.Drawing.Size(360, 34);
            this.txtDocPath.TabIndex = 5;
            // 
            // lblDocPath
            // 
            this.lblDocPath.AutoSize = true;
            this.lblDocPath.Location = new System.Drawing.Point(161, 227);
            this.lblDocPath.Name = "lblDocPath";
            this.lblDocPath.Size = new System.Drawing.Size(93, 13);
            this.lblDocPath.TabIndex = 4;
            this.lblDocPath.Text = "ხელშეკრულება";
            // 
            // btnFileDialog
            // 
            this.btnFileDialog.Location = new System.Drawing.Point(643, 216);
            this.btnFileDialog.Name = "btnFileDialog";
            this.btnFileDialog.Size = new System.Drawing.Size(75, 34);
            this.btnFileDialog.TabIndex = 6;
            this.btnFileDialog.Text = "არჩევა";
            this.btnFileDialog.UseVisualStyleBackColor = true;
            this.btnFileDialog.Click += new System.EventHandler(this.btnFileDialog_Click);
            // 
            // GroupManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 721);
            this.Controls.Add(this.btnFileDialog);
            this.Controls.Add(this.txtDocPath);
            this.Controls.Add(this.txtTeacher);
            this.Controls.Add(this.cmbSubGroupCount);
            this.Controls.Add(this.dgvSubGroups);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.btnAddGroup);
            this.Controls.Add(this.lblDocPath);
            this.Controls.Add(this.lblClass);
            this.Controls.Add(this.lblTeacher);
            this.Controls.Add(this.lblStuCount);
            this.Controls.Add(this.numMaxStudents);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblGroupName);
            this.Controls.Add(this.txtGroupName);
            this.Controls.Add(this.dgvMainGroup);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GroupManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GroupManagementForm";
            this.Load += new System.EventHandler(this.GroupManagementForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxStudents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubGroups)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMainGroup;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.NumericUpDown numMaxStudents;
        private System.Windows.Forms.Label lblStuCount;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblTeacher;
        private System.Windows.Forms.DataGridView dgvSubGroups;
        private System.Windows.Forms.ComboBox cmbSubGroupCount;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsEdit;
        private System.Windows.Forms.ToolStripMenuItem tsEditGroups;
        private System.Windows.Forms.ToolStripMenuItem tsEditSubGroups;
        private System.Windows.Forms.TextBox txtTeacher;
        private System.Windows.Forms.TextBox txtDocPath;
        private System.Windows.Forms.Label lblDocPath;
        private System.Windows.Forms.Button btnFileDialog;
    }
}
