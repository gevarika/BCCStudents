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
            dgvMainGroup = new DataGridView();
            txtGroupName = new TextBox();
            lblGroupName = new Label();
            numMaxStudents = new NumericUpDown();
            lblStuCount = new Label();
            btnAddGroup = new Button();
            txtPrice = new TextBox();
            lblPrice = new Label();
            lblTeacher = new Label();
            dgvSubGroups = new DataGridView();
            cmbSubGroupCount = new ComboBox();
            lblClass = new Label();
            menuStrip1 = new MenuStrip();
            tsEdit = new ToolStripMenuItem();
            tsEditGroups = new ToolStripMenuItem();
            tsEditSubGroups = new ToolStripMenuItem();
            txtTeacher = new TextBox();
            txtDocPath = new TextBox();
            lblDocPath = new Label();
            btnFileDialog = new Button();
            groupBox1 = new GroupBox();
            chkEnableSubGroupMaxStudents = new CheckBox();
            groupBox2 = new GroupBox();
            btnDeleteGroup = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvMainGroup).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxStudents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvSubGroups).BeginInit();
            menuStrip1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // dgvMainGroup
            // 
            dgvMainGroup.AllowUserToAddRows = false;
            dgvMainGroup.AllowUserToDeleteRows = false;
            dgvMainGroup.BorderStyle = BorderStyle.None;
            dgvMainGroup.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvMainGroup.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvMainGroup.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMainGroup.Location = new Point(13, 405);
            dgvMainGroup.Margin = new Padding(4, 3, 4, 3);
            dgvMainGroup.Name = "dgvMainGroup";
            dgvMainGroup.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvMainGroup.Size = new Size(1090, 256);
            dgvMainGroup.TabIndex = 9;
            dgvMainGroup.SelectionChanged += dataGridViewGroups_SelectionChanged;
            // 
            // txtGroupName
            // 
            txtGroupName.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtGroupName.Location = new Point(190, 19);
            txtGroupName.Margin = new Padding(4, 3, 4, 3);
            txtGroupName.Multiline = true;
            txtGroupName.Name = "txtGroupName";
            txtGroupName.Size = new Size(256, 34);
            txtGroupName.TabIndex = 0;
            // 
            // lblGroupName
            // 
            lblGroupName.AutoSize = true;
            lblGroupName.Location = new Point(50, 26);
            lblGroupName.Margin = new Padding(4, 0, 4, 0);
            lblGroupName.Name = "lblGroupName";
            lblGroupName.Size = new Size(107, 15);
            lblGroupName.TabIndex = 2;
            lblGroupName.Text = "ჯგუფის სახელი";
            // 
            // numMaxStudents
            // 
            numMaxStudents.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numMaxStudents.Location = new Point(190, 173);
            numMaxStudents.Margin = new Padding(4, 3, 4, 3);
            numMaxStudents.Name = "numMaxStudents";
            numMaxStudents.Size = new Size(187, 26);
            numMaxStudents.TabIndex = 4;
            // 
            // lblStuCount
            // 
            lblStuCount.AutoSize = true;
            lblStuCount.Location = new Point(-100, 179);
            lblStuCount.Margin = new Padding(4, 0, 4, 0);
            lblStuCount.Name = "lblStuCount";
            lblStuCount.Size = new Size(257, 15);
            lblStuCount.TabIndex = 4;
            lblStuCount.Text = "მოსწავლეების მაქსიმალური რაოდენობა";
            // 
            // btnAddGroup
            // 
            btnAddGroup.Location = new Point(190, 277);
            btnAddGroup.Margin = new Padding(4, 3, 4, 3);
            btnAddGroup.Name = "btnAddGroup";
            btnAddGroup.Size = new Size(148, 33);
            btnAddGroup.TabIndex = 6;
            btnAddGroup.Text = "დამატება";
            btnAddGroup.UseVisualStyleBackColor = true;
            btnAddGroup.Click += btnAddGroup_Click;
            // 
            // txtPrice
            // 
            txtPrice.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPrice.Location = new Point(190, 59);
            txtPrice.Margin = new Padding(4, 3, 4, 3);
            txtPrice.Multiline = true;
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(256, 34);
            txtPrice.TabIndex = 1;
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(30, 66);
            lblPrice.Margin = new Padding(4, 0, 4, 0);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(127, 15);
            lblPrice.TabIndex = 2;
            lblPrice.Text = "სწავლის საფასური";
            // 
            // lblTeacher
            // 
            lblTeacher.AutoSize = true;
            lblTeacher.Location = new Point(58, 140);
            lblTeacher.Margin = new Padding(4, 0, 4, 0);
            lblTeacher.Name = "lblTeacher";
            lblTeacher.Size = new Size(99, 15);
            lblTeacher.TabIndex = 4;
            lblTeacher.Text = "მასწავლებელი";
            // 
            // dgvSubGroups
            // 
            dgvSubGroups.AllowUserToAddRows = false;
            dgvSubGroups.AllowUserToDeleteRows = false;
            dgvSubGroups.BorderStyle = BorderStyle.None;
            dgvSubGroups.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvSubGroups.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvSubGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSubGroups.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvSubGroups.Location = new Point(13, 667);
            dgvSubGroups.Margin = new Padding(4, 3, 4, 3);
            dgvSubGroups.Name = "dgvSubGroups";
            dgvSubGroups.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvSubGroups.Size = new Size(1090, 225);
            dgvSubGroups.TabIndex = 10;
            // 
            // cmbSubGroupCount
            // 
            cmbSubGroupCount.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbSubGroupCount.FormattingEnabled = true;
            cmbSubGroupCount.ItemHeight = 20;
            cmbSubGroupCount.Items.AddRange(new object[] { "კლასის გარეშე", "1", "2", "3", "4", "5" });
            cmbSubGroupCount.Location = new Point(189, 99);
            cmbSubGroupCount.Margin = new Padding(4, 3, 4, 3);
            cmbSubGroupCount.Name = "cmbSubGroupCount";
            cmbSubGroupCount.Size = new Size(257, 28);
            cmbSubGroupCount.TabIndex = 2;
            cmbSubGroupCount.TextChanged += cmbSubGroupCount_TextChanged;
            // 
            // lblClass
            // 
            lblClass.AutoSize = true;
            lblClass.Location = new Point(18, 106);
            lblClass.Margin = new Padding(4, 0, 4, 0);
            lblClass.Name = "lblClass";
            lblClass.Size = new Size(139, 15);
            lblClass.TabIndex = 4;
            lblClass.Text = "კლასების რაოდენობა";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsEdit });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1113, 24);
            menuStrip1.TabIndex = 10;
            menuStrip1.Text = "menuStrip1";
            // 
            // tsEdit
            // 
            tsEdit.DropDownItems.AddRange(new ToolStripItem[] { tsEditGroups, tsEditSubGroups });
            tsEdit.Name = "tsEdit";
            tsEdit.Size = new Size(101, 20);
            tsEdit.Text = "რედაქტირება";
            // 
            // tsEditGroups
            // 
            tsEditGroups.Name = "tsEditGroups";
            tsEditGroups.Size = new Size(131, 22);
            tsEditGroups.Text = "ჯგუფები";
            // 
            // tsEditSubGroups
            // 
            tsEditSubGroups.Name = "tsEditSubGroups";
            tsEditSubGroups.Size = new Size(131, 22);
            tsEditSubGroups.Text = "კლასები";
            // 
            // txtTeacher
            // 
            txtTeacher.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTeacher.Location = new Point(190, 133);
            txtTeacher.Margin = new Padding(4, 3, 4, 3);
            txtTeacher.Multiline = true;
            txtTeacher.Name = "txtTeacher";
            txtTeacher.Size = new Size(256, 34);
            txtTeacher.TabIndex = 3;
            // 
            // txtDocPath
            // 
            txtDocPath.Enabled = false;
            txtDocPath.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtDocPath.Location = new Point(190, 205);
            txtDocPath.Margin = new Padding(4, 3, 4, 3);
            txtDocPath.Multiline = true;
            txtDocPath.Name = "txtDocPath";
            txtDocPath.Size = new Size(300, 66);
            txtDocPath.TabIndex = 5;
            // 
            // lblDocPath
            // 
            lblDocPath.AutoSize = true;
            lblDocPath.Location = new Point(53, 218);
            lblDocPath.Margin = new Padding(4, 0, 4, 0);
            lblDocPath.Name = "lblDocPath";
            lblDocPath.Size = new Size(104, 15);
            lblDocPath.TabIndex = 4;
            lblDocPath.Text = "ხელშეკრულება";
            // 
            // btnFileDialog
            // 
            btnFileDialog.Location = new Point(498, 206);
            btnFileDialog.Margin = new Padding(4, 3, 4, 3);
            btnFileDialog.Name = "btnFileDialog";
            btnFileDialog.Size = new Size(67, 27);
            btnFileDialog.TabIndex = 5;
            btnFileDialog.Text = "არჩევა";
            btnFileDialog.UseVisualStyleBackColor = true;
            btnFileDialog.Click += btnFileDialog_Click;
            // 
            // groupBox1
            // 
            groupBox1.Font = new Font("Microsoft Sans Serif", 10F);
            groupBox1.Location = new Point(680, 63);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(423, 305);
            groupBox1.TabIndex = 11;
            groupBox1.TabStop = false;
            groupBox1.Text = "ქვეჯგუფები";
            // 
            // chkEnableSubGroupMaxStudents
            // 
            chkEnableSubGroupMaxStudents.AutoSize = true;
            chkEnableSubGroupMaxStudents.Location = new Point(713, 37);
            chkEnableSubGroupMaxStudents.Name = "chkEnableSubGroupMaxStudents";
            chkEnableSubGroupMaxStudents.Size = new Size(329, 19);
            chkEnableSubGroupMaxStudents.TabIndex = 12;
            chkEnableSubGroupMaxStudents.Text = "ქვეჯგუფების მოსწავლეების რაოდენობის მართვა";
            chkEnableSubGroupMaxStudents.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lblGroupName);
            groupBox2.Controls.Add(txtGroupName);
            groupBox2.Controls.Add(lblPrice);
            groupBox2.Controls.Add(btnFileDialog);
            groupBox2.Controls.Add(numMaxStudents);
            groupBox2.Controls.Add(txtDocPath);
            groupBox2.Controls.Add(lblStuCount);
            groupBox2.Controls.Add(txtTeacher);
            groupBox2.Controls.Add(lblTeacher);
            groupBox2.Controls.Add(cmbSubGroupCount);
            groupBox2.Controls.Add(lblClass);
            groupBox2.Controls.Add(lblDocPath);
            groupBox2.Controls.Add(txtPrice);
            groupBox2.Controls.Add(btnAddGroup);
            groupBox2.Location = new Point(13, 37);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(661, 331);
            groupBox2.TabIndex = 13;
            groupBox2.TabStop = false;
            groupBox2.Text = "ჯგუფის დეტალები ";
            // 
            // btnDeleteGroup
            // 
            btnDeleteGroup.Location = new Point(13, 376);
            btnDeleteGroup.Name = "btnDeleteGroup";
            btnDeleteGroup.Size = new Size(75, 23);
            btnDeleteGroup.TabIndex = 14;
            btnDeleteGroup.Text = "წაშლა";
            btnDeleteGroup.UseVisualStyleBackColor = true;
            btnDeleteGroup.Click += BtnDeleteGroup_Click;
            // 
            // GroupManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1113, 899);
            Controls.Add(btnDeleteGroup);
            Controls.Add(groupBox2);
            Controls.Add(chkEnableSubGroupMaxStudents);
            Controls.Add(groupBox1);
            Controls.Add(dgvSubGroups);
            Controls.Add(dgvMainGroup);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "GroupManagementForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GroupManagementForm";
            Load += GroupManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMainGroup).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxStudents).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvSubGroups).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private GroupBox groupBox1;
        private CheckBox chkEnableSubGroupMaxStudents;
        private GroupBox groupBox2;
        private Button btnDeleteGroup;
    }
}
