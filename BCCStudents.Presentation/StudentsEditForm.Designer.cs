namespace BCCStudents.Presentation
{
    partial class StudentsEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudentsEditForm));
            this.dgvStudents = new System.Windows.Forms.DataGridView();
            this.txtStudentSearch = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.წაშლაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBalance = new System.Windows.Forms.TextBox();
            this.chkBoxStatus = new System.Windows.Forms.CheckBox();
            this.dgvGroupSubGroups = new System.Windows.Forms.DataGridView();
            this.btnStudentActivation = new System.Windows.Forms.Button();
            this.chlGroups = new System.Windows.Forms.CheckedListBox();
            this.btnCancelChanges = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblGroup = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblStudentCode = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPaymentDate = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtPaymentStatus = new System.Windows.Forms.TextBox();
            this.txtParent = new System.Windows.Forms.TextBox();
            this.txtRegistrationDate = new System.Windows.Forms.TextBox();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.txtPersonalId = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.btndel = new System.Windows.Forms.Button();
            this.cbGroups = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.rbByStCode = new System.Windows.Forms.RadioButton();
            this.rbByAge = new System.Windows.Forms.RadioButton();
            this.rbByIdNumber = new System.Windows.Forms.RadioButton();
            this.rbByLastName = new System.Windows.Forms.RadioButton();
            this.rbByAddress = new System.Windows.Forms.RadioButton();
            this.rbByParent = new System.Windows.Forms.RadioButton();
            this.rbByPhone = new System.Windows.Forms.RadioButton();
            this.rbByName = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStudents)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroupSubGroups)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvStudents
            // 
            this.dgvStudents.AllowUserToAddRows = false;
            this.dgvStudents.AllowUserToOrderColumns = true;
            this.dgvStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStudents.Location = new System.Drawing.Point(689, 57);
            this.dgvStudents.Name = "dgvStudents";
            this.dgvStudents.RowHeadersWidth = 51;
            this.dgvStudents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStudents.Size = new System.Drawing.Size(569, 731);
            this.dgvStudents.TabIndex = 17;
            this.dgvStudents.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStudents_CellContentClick);
            this.dgvStudents.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStudents_CellDoubleClick);
            this.dgvStudents.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStudents_CellValueChanged);
            // 
            // txtStudentSearch
            // 
            this.txtStudentSearch.Location = new System.Drawing.Point(976, 24);
            this.txtStudentSearch.Name = "txtStudentSearch";
            this.txtStudentSearch.Size = new System.Drawing.Size(118, 20);
            this.txtStudentSearch.TabIndex = 13;
            this.txtStudentSearch.TextChanged += new System.EventHandler(this.txtStudentsSearch_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.წაშლაToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(116, 26);
            // 
            // წაშლაToolStripMenuItem
            // 
            this.წაშლაToolStripMenuItem.Name = "წაშლაToolStripMenuItem";
            this.წაშლაToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.წაშლაToolStripMenuItem.Text = "წაშლა";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.DarkGray;
            this.groupBox1.Controls.Add(this.txtBalance);
            this.groupBox1.Controls.Add(this.chkBoxStatus);
            this.groupBox1.Controls.Add(this.dgvGroupSubGroups);
            this.groupBox1.Controls.Add(this.btnStudentActivation);
            this.groupBox1.Controls.Add(this.chlGroups);
            this.groupBox1.Controls.Add(this.btnCancelChanges);
            this.groupBox1.Controls.Add(this.btnSaveChanges);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.lblGroup);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lblStudentCode);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPaymentDate);
            this.groupBox1.Controls.Add(this.txtPhone);
            this.groupBox1.Controls.Add(this.txtAddress);
            this.groupBox1.Controls.Add(this.txtPaymentStatus);
            this.groupBox1.Controls.Add(this.txtParent);
            this.groupBox1.Controls.Add(this.txtRegistrationDate);
            this.groupBox1.Controls.Add(this.txtAge);
            this.groupBox1.Controls.Add(this.txtPersonalId);
            this.groupBox1.Controls.Add(this.txtStatus);
            this.groupBox1.Controls.Add(this.txtLastName);
            this.groupBox1.Controls.Add(this.txtFirstName);
            this.groupBox1.Location = new System.Drawing.Point(2, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(682, 764);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "მოსწავლის მონაცემები";
            // 
            // txtBalance
            // 
            this.txtBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBalance.Location = new System.Drawing.Point(477, 272);
            this.txtBalance.Multiline = true;
            this.txtBalance.Name = "txtBalance";
            this.txtBalance.Size = new System.Drawing.Size(170, 38);
            this.txtBalance.TabIndex = 16;
            // 
            // chkBoxStatus
            // 
            this.chkBoxStatus.AutoSize = true;
            this.chkBoxStatus.Location = new System.Drawing.Point(156, 413);
            this.chkBoxStatus.Name = "chkBoxStatus";
            this.chkBoxStatus.Size = new System.Drawing.Size(80, 17);
            this.chkBoxStatus.TabIndex = 15;
            this.chkBoxStatus.Text = "checkBox1";
            this.chkBoxStatus.UseVisualStyleBackColor = true;
            this.chkBoxStatus.CheckedChanged += new System.EventHandler(this.chkBoxStatus_CheckedChanged);
            // 
            // dgvGroupSubGroups
            // 
            this.dgvGroupSubGroups.AllowUserToAddRows = false;
            this.dgvGroupSubGroups.AllowUserToDeleteRows = false;
            this.dgvGroupSubGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroupSubGroups.Location = new System.Drawing.Point(86, 527);
            this.dgvGroupSubGroups.Name = "dgvGroupSubGroups";
            this.dgvGroupSubGroups.RowHeadersWidth = 51;
            this.dgvGroupSubGroups.Size = new System.Drawing.Size(356, 144);
            this.dgvGroupSubGroups.TabIndex = 2;
            // 
            // btnStudentActivation
            // 
            this.btnStudentActivation.Location = new System.Drawing.Point(148, 727);
            this.btnStudentActivation.Name = "btnStudentActivation";
            this.btnStudentActivation.Size = new System.Drawing.Size(89, 23);
            this.btnStudentActivation.TabIndex = 14;
            this.btnStudentActivation.Text = "გააქტიურება";
            this.btnStudentActivation.UseVisualStyleBackColor = true;
            this.btnStudentActivation.Click += new System.EventHandler(this.btnStudentActivation_Click);
            // 
            // chlGroups
            // 
            this.chlGroups.FormattingEnabled = true;
            this.chlGroups.Location = new System.Drawing.Point(477, 187);
            this.chlGroups.Name = "chlGroups";
            this.chlGroups.Size = new System.Drawing.Size(170, 79);
            this.chlGroups.TabIndex = 13;
            this.chlGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlGroups_ItemCheck);
            // 
            // btnCancelChanges
            // 
            this.btnCancelChanges.Location = new System.Drawing.Point(477, 727);
            this.btnCancelChanges.Name = "btnCancelChanges";
            this.btnCancelChanges.Size = new System.Drawing.Size(75, 23);
            this.btnCancelChanges.TabIndex = 12;
            this.btnCancelChanges.Text = "გაუქმება";
            this.btnCancelChanges.UseVisualStyleBackColor = true;
            this.btnCancelChanges.Click += new System.EventHandler(this.btnCancelChanges_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(393, 727);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.btnSaveChanges.TabIndex = 11;
            this.btnSaveChanges.Text = "შენახვა";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(153, 20);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(13, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "1";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(95, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "სტატუსი: ";
            // 
            // lblGroup
            // 
            this.lblGroup.AutoSize = true;
            this.lblGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGroup.Location = new System.Drawing.Point(156, 379);
            this.lblGroup.Name = "lblGroup";
            this.lblGroup.Size = new System.Drawing.Size(0, 17);
            this.lblGroup.TabIndex = 3;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(105, 379);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 13);
            this.label18.TabIndex = 3;
            this.label18.Text = "ჯგუფი";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(416, 277);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "ბალანსი";
            // 
            // lblStudentCode
            // 
            this.lblStudentCode.AutoSize = true;
            this.lblStudentCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStudentCode.Location = new System.Drawing.Point(156, 446);
            this.lblStudentCode.Name = "lblStudentCode";
            this.lblStudentCode.Size = new System.Drawing.Size(107, 13);
            this.lblStudentCode.TabIndex = 2;
            this.lblStudentCode.Text = "მოსწვლის კოდი";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(56, 446);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "მოსწვლის კოდი";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(362, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(109, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "გადაღდისთარიღი";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "ტელეფონის ნომერი";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(339, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "რეგისტრაციის თარიღი";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "ასაკი";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(83, 272);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "მისამართი";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(36, 511);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(114, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "გადახდის სტატუსი";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(357, 157);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(114, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "გადახდის სტატუსი";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "მშობელი";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(59, 235);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "პირადი ნომერი";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(416, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "სტატუსი";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(46, 80);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(104, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "მოსწავლის გვარი";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "მოსწავლის სახელი";
            // 
            // txtPaymentDate
            // 
            this.txtPaymentDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPaymentDate.Location = new System.Drawing.Point(477, 76);
            this.txtPaymentDate.Multiline = true;
            this.txtPaymentDate.Name = "txtPaymentDate";
            this.txtPaymentDate.Size = new System.Drawing.Size(172, 31);
            this.txtPaymentDate.TabIndex = 8;
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhone.Location = new System.Drawing.Point(156, 191);
            this.txtPhone.Multiline = true;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(172, 31);
            this.txtPhone.TabIndex = 4;
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(156, 265);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(172, 31);
            this.txtAddress.TabIndex = 6;
            // 
            // txtPaymentStatus
            // 
            this.txtPaymentStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPaymentStatus.Location = new System.Drawing.Point(477, 150);
            this.txtPaymentStatus.Multiline = true;
            this.txtPaymentStatus.Name = "txtPaymentStatus";
            this.txtPaymentStatus.Size = new System.Drawing.Size(172, 31);
            this.txtPaymentStatus.TabIndex = 10;
            // 
            // txtParent
            // 
            this.txtParent.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParent.Location = new System.Drawing.Point(156, 117);
            this.txtParent.Multiline = true;
            this.txtParent.Name = "txtParent";
            this.txtParent.Size = new System.Drawing.Size(172, 31);
            this.txtParent.TabIndex = 2;
            // 
            // txtRegistrationDate
            // 
            this.txtRegistrationDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRegistrationDate.Location = new System.Drawing.Point(477, 39);
            this.txtRegistrationDate.Multiline = true;
            this.txtRegistrationDate.Name = "txtRegistrationDate";
            this.txtRegistrationDate.Size = new System.Drawing.Size(172, 31);
            this.txtRegistrationDate.TabIndex = 7;
            // 
            // txtAge
            // 
            this.txtAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAge.Location = new System.Drawing.Point(156, 154);
            this.txtAge.Multiline = true;
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(172, 31);
            this.txtAge.TabIndex = 3;
            // 
            // txtPersonalId
            // 
            this.txtPersonalId.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPersonalId.Location = new System.Drawing.Point(156, 228);
            this.txtPersonalId.Multiline = true;
            this.txtPersonalId.Name = "txtPersonalId";
            this.txtPersonalId.Size = new System.Drawing.Size(172, 31);
            this.txtPersonalId.TabIndex = 5;
            // 
            // txtStatus
            // 
            this.txtStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatus.Location = new System.Drawing.Point(477, 113);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(172, 31);
            this.txtStatus.TabIndex = 9;
            // 
            // txtLastName
            // 
            this.txtLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(156, 80);
            this.txtLastName.Multiline = true;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(172, 31);
            this.txtLastName.TabIndex = 1;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(156, 43);
            this.txtFirstName.Multiline = true;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(172, 31);
            this.txtFirstName.TabIndex = 0;
            // 
            // btndel
            // 
            this.btndel.Location = new System.Drawing.Point(1105, 22);
            this.btndel.Name = "btndel";
            this.btndel.Size = new System.Drawing.Size(75, 23);
            this.btndel.TabIndex = 14;
            this.btndel.Text = "წაშლა";
            this.btndel.UseVisualStyleBackColor = true;
            this.btndel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // cbGroups
            // 
            this.cbGroups.FormattingEnabled = true;
            this.cbGroups.Location = new System.Drawing.Point(759, 24);
            this.cbGroups.Name = "cbGroups";
            this.cbGroups.Size = new System.Drawing.Size(121, 21);
            this.cbGroups.TabIndex = 16;
            this.cbGroups.SelectedValueChanged += new System.EventHandler(this.cbGroups_SelectedValueChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(1186, 22);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(92, 23);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "გასუფთავება";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(696, 27);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "ჯგუფები";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(886, 27);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(84, 13);
            this.label16.TabIndex = 18;
            this.label16.Text = "ძიება ჯგუფში";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtSearch);
            this.groupBox2.Controls.Add(this.rbByStCode);
            this.groupBox2.Controls.Add(this.rbByAge);
            this.groupBox2.Controls.Add(this.rbByIdNumber);
            this.groupBox2.Controls.Add(this.rbByLastName);
            this.groupBox2.Controls.Add(this.rbByAddress);
            this.groupBox2.Controls.Add(this.rbByParent);
            this.groupBox2.Controls.Add(this.rbByPhone);
            this.groupBox2.Controls.Add(this.rbByName);
            this.groupBox2.Location = new System.Drawing.Point(1265, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(267, 655);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "დეტალური ძიება";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(25, 221);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(223, 29);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // rbByStCode
            // 
            this.rbByStCode.AutoSize = true;
            this.rbByStCode.Location = new System.Drawing.Point(16, 198);
            this.rbByStCode.Name = "rbByStCode";
            this.rbByStCode.Size = new System.Drawing.Size(53, 17);
            this.rbByStCode.TabIndex = 0;
            this.rbByStCode.TabStop = true;
            this.rbByStCode.Text = "კოდი";
            this.rbByStCode.UseVisualStyleBackColor = true;
            // 
            // rbByAge
            // 
            this.rbByAge.AutoSize = true;
            this.rbByAge.Location = new System.Drawing.Point(16, 106);
            this.rbByAge.Name = "rbByAge";
            this.rbByAge.Size = new System.Drawing.Size(53, 17);
            this.rbByAge.TabIndex = 0;
            this.rbByAge.TabStop = true;
            this.rbByAge.Text = "ასაკი";
            this.rbByAge.UseVisualStyleBackColor = true;
            // 
            // rbByIdNumber
            // 
            this.rbByIdNumber.AutoSize = true;
            this.rbByIdNumber.Location = new System.Drawing.Point(16, 152);
            this.rbByIdNumber.Name = "rbByIdNumber";
            this.rbByIdNumber.Size = new System.Drawing.Size(109, 17);
            this.rbByIdNumber.TabIndex = 0;
            this.rbByIdNumber.TabStop = true;
            this.rbByIdNumber.Text = "პირადი ნომერი";
            this.rbByIdNumber.UseVisualStyleBackColor = true;
            // 
            // rbByLastName
            // 
            this.rbByLastName.AutoSize = true;
            this.rbByLastName.Location = new System.Drawing.Point(16, 60);
            this.rbByLastName.Name = "rbByLastName";
            this.rbByLastName.Size = new System.Drawing.Size(57, 17);
            this.rbByLastName.TabIndex = 0;
            this.rbByLastName.TabStop = true;
            this.rbByLastName.Text = "გვარი";
            this.rbByLastName.UseVisualStyleBackColor = true;
            // 
            // rbByAddress
            // 
            this.rbByAddress.AutoSize = true;
            this.rbByAddress.Location = new System.Drawing.Point(16, 175);
            this.rbByAddress.Name = "rbByAddress";
            this.rbByAddress.Size = new System.Drawing.Size(85, 17);
            this.rbByAddress.TabIndex = 0;
            this.rbByAddress.TabStop = true;
            this.rbByAddress.Text = "მისამართი";
            this.rbByAddress.UseVisualStyleBackColor = true;
            // 
            // rbByParent
            // 
            this.rbByParent.AutoSize = true;
            this.rbByParent.Location = new System.Drawing.Point(16, 83);
            this.rbByParent.Name = "rbByParent";
            this.rbByParent.Size = new System.Drawing.Size(75, 17);
            this.rbByParent.TabIndex = 0;
            this.rbByParent.TabStop = true;
            this.rbByParent.Text = "მშობელი";
            this.rbByParent.UseVisualStyleBackColor = true;
            // 
            // rbByPhone
            // 
            this.rbByPhone.AutoSize = true;
            this.rbByPhone.Location = new System.Drawing.Point(16, 129);
            this.rbByPhone.Name = "rbByPhone";
            this.rbByPhone.Size = new System.Drawing.Size(87, 17);
            this.rbByPhone.TabIndex = 0;
            this.rbByPhone.TabStop = true;
            this.rbByPhone.Text = "ტელეფონი";
            this.rbByPhone.UseVisualStyleBackColor = true;
            // 
            // rbByName
            // 
            this.rbByName.AutoSize = true;
            this.rbByName.Location = new System.Drawing.Point(16, 37);
            this.rbByName.Name = "rbByName";
            this.rbByName.Size = new System.Drawing.Size(67, 17);
            this.rbByName.TabIndex = 0;
            this.rbByName.TabStop = true;
            this.rbByName.Text = "სახელი";
            this.rbByName.UseVisualStyleBackColor = true;
            // 
            // StudentsEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1544, 808);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.cbGroups);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btndel);
            this.Controls.Add(this.txtStudentSearch);
            this.Controls.Add(this.dgvStudents);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StudentsEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StudentsEditForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StudentsEditForm_FormClosing);
            this.Load += new System.EventHandler(this.StudentsEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStudents)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroupSubGroups)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvStudents;
        private System.Windows.Forms.TextBox txtStudentSearch;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem წაშლაToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPaymentDate;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtParent;
        private System.Windows.Forms.TextBox txtRegistrationDate;
        private System.Windows.Forms.TextBox txtAge;
        private System.Windows.Forms.TextBox txtPersonalId;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPaymentStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblStudentCode;
        private System.Windows.Forms.Button btndel;
        private System.Windows.Forms.ComboBox cbGroups;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Button btnCancelChanges;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckedListBox chlGroups;
        private System.Windows.Forms.Button btnStudentActivation;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbByStCode;
        private System.Windows.Forms.RadioButton rbByAge;
        private System.Windows.Forms.RadioButton rbByIdNumber;
        private System.Windows.Forms.RadioButton rbByLastName;
        private System.Windows.Forms.RadioButton rbByAddress;
        private System.Windows.Forms.RadioButton rbByParent;
        private System.Windows.Forms.RadioButton rbByPhone;
        private System.Windows.Forms.RadioButton rbByName;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvGroupSubGroups;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox chkBoxStatus;
        private System.Windows.Forms.TextBox txtBalance;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.Label label18;
    }
}
