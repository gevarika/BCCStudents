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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudentsEditForm));
            dgvStudents = new DataGridView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            წაშლაToolStripMenuItem = new ToolStripMenuItem();
            სვეტებისმართვაToolStripMenuItem = new ToolStripMenuItem();
            groupBox1 = new GroupBox();
            txtBalance = new TextBox();
            chkBoxStatus = new CheckBox();
            dgvGroupSubGroups = new DataGridView();
            btnStudentActivation = new Button();
            chlGroups = new CheckedListBox();
            btnCancelChanges = new Button();
            btnSaveChanges = new Button();
            lblStatus = new Label();
            label14 = new Label();
            lblGroup = new Label();
            label18 = new Label();
            label12 = new Label();
            lblStudentCode = new Label();
            label10 = new Label();
            label8 = new Label();
            label4 = new Label();
            label7 = new Label();
            label3 = new Label();
            label6 = new Label();
            label17 = new Label();
            label11 = new Label();
            label2 = new Label();
            label5 = new Label();
            label9 = new Label();
            label13 = new Label();
            label1 = new Label();
            txtPaymentDate = new TextBox();
            txtPhone = new TextBox();
            txtAddress = new TextBox();
            txtPaymentStatus = new TextBox();
            txtParent = new TextBox();
            txtRegistrationDate = new TextBox();
            txtAge = new TextBox();
            txtPersonalId = new TextBox();
            txtStatus = new TextBox();
            txtLastName = new TextBox();
            txtFirstName = new TextBox();
            btndel = new Button();
            cbGroups = new ComboBox();
            btnClear = new Button();
            label15 = new Label();
            groupBox2 = new GroupBox();
            txtSearch = new TextBox();
            rbByStCode = new RadioButton();
            rbByAge = new RadioButton();
            rbByIdNumber = new RadioButton();
            rbByLastName = new RadioButton();
            rbByAddress = new RadioButton();
            rbByParent = new RadioButton();
            rbByPhone = new RadioButton();
            rbByName = new RadioButton();
            groupBox3 = new GroupBox();
            chkstudentsToGroups = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dgvStudents).BeginInit();
            contextMenuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGroupSubGroups).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // dgvStudents
            // 
            dgvStudents.AllowUserToAddRows = false;
            dgvStudents.AllowUserToOrderColumns = true;
            dgvStudents.BorderStyle = BorderStyle.None;
            dgvStudents.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStudents.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvStudents.Location = new Point(804, 14);
            dgvStudents.Margin = new Padding(4, 3, 4, 3);
            dgvStudents.MultiSelect = false;
            dgvStudents.Name = "dgvStudents";
            dgvStudents.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvStudents.RowHeadersVisible = false;
            dgvStudents.RowHeadersWidth = 51;
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.Size = new Size(664, 849);
            dgvStudents.TabIndex = 17;
            dgvStudents.ContextMenuStrip = contextMenuStrip1;
            dgvStudents.CellClick += dgvStudents_CellClick;
            dgvStudents.CellContentClick += dgvStudents_CellContentClick;
            dgvStudents.CellValueChanged += dgvStudents_CellValueChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 
                წაშლაToolStripMenuItem,
                სვეტებისმართვაToolStripMenuItem
            });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(200, 48);
            // 
            // წაშლაToolStripMenuItem
            // 
            წაშლაToolStripMenuItem.Name = "წაშლაToolStripMenuItem";
            წაშლაToolStripMenuItem.Size = new Size(199, 22);
            წაშლაToolStripMenuItem.Text = "წაშლა";
            // 
            // სვეტებისმართვაToolStripMenuItem
            // 
            სვეტებისმართვაToolStripMenuItem.Name = "სვეტებისმართვაToolStripMenuItem";
            სვეტებისმართვაToolStripMenuItem.Size = new Size(199, 22);
            სვეტებისმართვაToolStripMenuItem.Text = "სვეტების მართვა";
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.DarkGray;
            groupBox1.Controls.Add(txtBalance);
            groupBox1.Controls.Add(chkBoxStatus);
            groupBox1.Controls.Add(dgvGroupSubGroups);
            groupBox1.Controls.Add(btnStudentActivation);
            groupBox1.Controls.Add(chlGroups);
            groupBox1.Controls.Add(btnCancelChanges);
            groupBox1.Controls.Add(btnSaveChanges);
            groupBox1.Controls.Add(lblStatus);
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(lblGroup);
            groupBox1.Controls.Add(label18);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(lblStudentCode);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label17);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtPaymentDate);
            groupBox1.Controls.Add(txtPhone);
            groupBox1.Controls.Add(txtAddress);
            groupBox1.Controls.Add(txtPaymentStatus);
            groupBox1.Controls.Add(txtParent);
            groupBox1.Controls.Add(txtRegistrationDate);
            groupBox1.Controls.Add(txtAge);
            groupBox1.Controls.Add(txtPersonalId);
            groupBox1.Controls.Add(txtStatus);
            groupBox1.Controls.Add(txtLastName);
            groupBox1.Controls.Add(txtFirstName);
            groupBox1.Location = new Point(2, 14);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(796, 849);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "მოსწავლის მონაცემები";
            // 
            // txtBalance
            // 
            txtBalance.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtBalance.Location = new Point(556, 314);
            txtBalance.Margin = new Padding(4, 3, 4, 3);
            txtBalance.Multiline = true;
            txtBalance.Name = "txtBalance";
            txtBalance.Size = new Size(198, 43);
            txtBalance.TabIndex = 16;
            // 
            // chkBoxStatus
            // 
            chkBoxStatus.AutoSize = true;
            chkBoxStatus.Location = new Point(182, 477);
            chkBoxStatus.Margin = new Padding(4, 3, 4, 3);
            chkBoxStatus.Name = "chkBoxStatus";
            chkBoxStatus.Size = new Size(82, 19);
            chkBoxStatus.TabIndex = 15;
            chkBoxStatus.Text = "checkBox1";
            chkBoxStatus.UseVisualStyleBackColor = true;
            chkBoxStatus.CheckedChanged += chkBoxStatus_CheckedChanged;
            // 
            // dgvGroupSubGroups
            // 
            dgvGroupSubGroups.AllowUserToAddRows = false;
            dgvGroupSubGroups.AllowUserToDeleteRows = false;
            dgvGroupSubGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGroupSubGroups.EnableHeadersVisualStyles = false;
            dgvGroupSubGroups.Location = new Point(100, 608);
            dgvGroupSubGroups.Margin = new Padding(4, 3, 4, 3);
            dgvGroupSubGroups.Name = "dgvGroupSubGroups";
            dgvGroupSubGroups.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvGroupSubGroups.RowHeadersVisible = false;
            dgvGroupSubGroups.RowHeadersWidth = 51;
            dgvGroupSubGroups.Size = new Size(415, 166);
            dgvGroupSubGroups.TabIndex = 2;
            // 
            // btnStudentActivation
            // 
            btnStudentActivation.Location = new Point(458, 808);
            btnStudentActivation.Margin = new Padding(4, 3, 4, 3);
            btnStudentActivation.Name = "btnStudentActivation";
            btnStudentActivation.Size = new Size(104, 27);
            btnStudentActivation.TabIndex = 14;
            btnStudentActivation.Text = "გააქტიურება";
            btnStudentActivation.UseVisualStyleBackColor = true;
            btnStudentActivation.Click += btnStudentActivation_Click;
            // 
            // chlGroups
            // 
            chlGroups.FormattingEnabled = true;
            chlGroups.Location = new Point(556, 216);
            chlGroups.Margin = new Padding(4, 3, 4, 3);
            chlGroups.Name = "chlGroups";
            chlGroups.Size = new Size(198, 76);
            chlGroups.TabIndex = 13;
            chlGroups.ItemCheck += chlGroups_ItemCheck;
            // 
            // btnCancelChanges
            // 
            btnCancelChanges.Location = new Point(668, 808);
            btnCancelChanges.Margin = new Padding(4, 3, 4, 3);
            btnCancelChanges.Name = "btnCancelChanges";
            btnCancelChanges.Size = new Size(88, 27);
            btnCancelChanges.TabIndex = 12;
            btnCancelChanges.Text = "გაუქმება";
            btnCancelChanges.UseVisualStyleBackColor = true;
            btnCancelChanges.Click += btnCancelChanges_Click;
            // 
            // btnSaveChanges
            // 
            btnSaveChanges.Location = new Point(570, 808);
            btnSaveChanges.Margin = new Padding(4, 3, 4, 3);
            btnSaveChanges.Name = "btnSaveChanges";
            btnSaveChanges.Size = new Size(88, 27);
            btnSaveChanges.TabIndex = 11;
            btnSaveChanges.Text = "შენახვა";
            btnSaveChanges.UseVisualStyleBackColor = true;
            btnSaveChanges.Click += btnSaveChanges_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(178, 23);
            lblStatus.Margin = new Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(13, 15);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "1";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(111, 23);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(68, 15);
            label14.TabIndex = 3;
            label14.Text = "სტატუსი: ";
            // 
            // lblGroup
            // 
            lblGroup.AutoSize = true;
            lblGroup.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblGroup.Location = new Point(182, 437);
            lblGroup.Margin = new Padding(4, 0, 4, 0);
            lblGroup.Name = "lblGroup";
            lblGroup.Size = new Size(0, 17);
            lblGroup.TabIndex = 3;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(122, 437);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(51, 15);
            label18.TabIndex = 3;
            label18.Text = "ჯგუფი";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(485, 320);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(60, 15);
            label12.TabIndex = 3;
            label12.Text = "ბალანსი";
            // 
            // lblStudentCode
            // 
            lblStudentCode.AutoSize = true;
            lblStudentCode.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStudentCode.Location = new Point(182, 515);
            lblStudentCode.Margin = new Padding(4, 0, 4, 0);
            lblStudentCode.Name = "lblStudentCode";
            lblStudentCode.Size = new Size(107, 13);
            lblStudentCode.TabIndex = 2;
            lblStudentCode.Text = "მოსწვლის კოდი";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(65, 515);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(107, 15);
            label10.TabIndex = 2;
            label10.Text = "მოსწვლის კოდი";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(422, 96);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(120, 15);
            label8.TabIndex = 1;
            label8.Text = "გადაღდისთარიღი";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(37, 228);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(130, 15);
            label4.TabIndex = 1;
            label4.Text = "ტელეფონის ნომერი";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(396, 53);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(147, 15);
            label7.TabIndex = 1;
            label7.Text = "რეგისტრაციის თარიღი";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(134, 186);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 1;
            label3.Text = "ასაკი";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(97, 314);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(74, 15);
            label6.TabIndex = 1;
            label6.Text = "მისამართი";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(42, 590);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(127, 15);
            label17.TabIndex = 1;
            label17.Text = "გადახდის სტატუსი";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(416, 181);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(127, 15);
            label11.TabIndex = 1;
            label11.Text = "გადახდის სტატუსი";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(108, 143);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(64, 15);
            label2.TabIndex = 1;
            label2.Text = "მშობელი";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(69, 271);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(101, 15);
            label5.TabIndex = 1;
            label5.Text = "პირადი ნომერი";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(485, 138);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(62, 15);
            label9.TabIndex = 1;
            label9.Text = "სტატუსი";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(54, 92);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(115, 15);
            label13.TabIndex = 1;
            label13.Text = "მოსწავლის გვარი";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 53);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(126, 15);
            label1.TabIndex = 1;
            label1.Text = "მოსწავლის სახელი";
            // 
            // txtPaymentDate
            // 
            txtPaymentDate.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPaymentDate.Location = new Point(556, 88);
            txtPaymentDate.Margin = new Padding(4, 3, 4, 3);
            txtPaymentDate.Multiline = true;
            txtPaymentDate.Name = "txtPaymentDate";
            txtPaymentDate.Size = new Size(200, 35);
            txtPaymentDate.TabIndex = 8;
            // 
            // txtPhone
            // 
            txtPhone.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPhone.Location = new Point(182, 220);
            txtPhone.Margin = new Padding(4, 3, 4, 3);
            txtPhone.Multiline = true;
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(200, 35);
            txtPhone.TabIndex = 4;
            // 
            // txtAddress
            // 
            txtAddress.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAddress.Location = new Point(182, 306);
            txtAddress.Margin = new Padding(4, 3, 4, 3);
            txtAddress.Multiline = true;
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(200, 35);
            txtAddress.TabIndex = 6;
            // 
            // txtPaymentStatus
            // 
            txtPaymentStatus.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPaymentStatus.Location = new Point(556, 173);
            txtPaymentStatus.Margin = new Padding(4, 3, 4, 3);
            txtPaymentStatus.Multiline = true;
            txtPaymentStatus.Name = "txtPaymentStatus";
            txtPaymentStatus.Size = new Size(200, 35);
            txtPaymentStatus.TabIndex = 10;
            // 
            // txtParent
            // 
            txtParent.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtParent.Location = new Point(182, 135);
            txtParent.Margin = new Padding(4, 3, 4, 3);
            txtParent.Multiline = true;
            txtParent.Name = "txtParent";
            txtParent.Size = new Size(200, 35);
            txtParent.TabIndex = 2;
            // 
            // txtRegistrationDate
            // 
            txtRegistrationDate.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtRegistrationDate.Location = new Point(556, 45);
            txtRegistrationDate.Margin = new Padding(4, 3, 4, 3);
            txtRegistrationDate.Multiline = true;
            txtRegistrationDate.Name = "txtRegistrationDate";
            txtRegistrationDate.Size = new Size(200, 35);
            txtRegistrationDate.TabIndex = 7;
            // 
            // txtAge
            // 
            txtAge.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAge.Location = new Point(182, 178);
            txtAge.Margin = new Padding(4, 3, 4, 3);
            txtAge.Multiline = true;
            txtAge.Name = "txtAge";
            txtAge.Size = new Size(200, 35);
            txtAge.TabIndex = 3;
            // 
            // txtPersonalId
            // 
            txtPersonalId.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPersonalId.Location = new Point(182, 263);
            txtPersonalId.Margin = new Padding(4, 3, 4, 3);
            txtPersonalId.Multiline = true;
            txtPersonalId.Name = "txtPersonalId";
            txtPersonalId.Size = new Size(200, 35);
            txtPersonalId.TabIndex = 5;
            // 
            // txtStatus
            // 
            txtStatus.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtStatus.Location = new Point(556, 130);
            txtStatus.Margin = new Padding(4, 3, 4, 3);
            txtStatus.Multiline = true;
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(200, 35);
            txtStatus.TabIndex = 9;
            // 
            // txtLastName
            // 
            txtLastName.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtLastName.Location = new Point(182, 92);
            txtLastName.Margin = new Padding(4, 3, 4, 3);
            txtLastName.Multiline = true;
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(200, 35);
            txtLastName.TabIndex = 1;
            // 
            // txtFirstName
            // 
            txtFirstName.Font = new Font("Microsoft Sans Serif", 11.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFirstName.Location = new Point(182, 50);
            txtFirstName.Margin = new Padding(4, 3, 4, 3);
            txtFirstName.Multiline = true;
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(200, 35);
            txtFirstName.TabIndex = 0;
            // 
            // btndel
            // 
            btndel.Location = new Point(912, 870);
            btndel.Margin = new Padding(4, 3, 4, 3);
            btndel.Name = "btndel";
            btndel.Size = new Size(88, 27);
            btndel.TabIndex = 14;
            btndel.Text = "წაშლა";
            btndel.UseVisualStyleBackColor = true;
            btndel.Click += btnDel_Click;
            // 
            // cbGroups
            // 
            cbGroups.FormattingEnabled = true;
            cbGroups.Location = new Point(97, 46);
            cbGroups.Margin = new Padding(4, 3, 4, 3);
            cbGroups.Name = "cbGroups";
            cbGroups.Size = new Size(140, 23);
            cbGroups.TabIndex = 16;
            cbGroups.SelectedValueChanged += cbGroups_SelectedValueChanged;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(806, 869);
            btnClear.Margin = new Padding(4, 3, 4, 3);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(107, 27);
            btnClear.TabIndex = 15;
            btnClear.Text = "გასუფთავება";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(23, 49);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(64, 15);
            label15.TabIndex = 18;
            label15.Text = "ჯგუფები";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chkstudentsToGroups);
            groupBox2.Controls.Add(txtSearch);
            groupBox2.Controls.Add(cbGroups);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(rbByStCode);
            groupBox2.Controls.Add(rbByAge);
            groupBox2.Controls.Add(rbByIdNumber);
            groupBox2.Controls.Add(rbByLastName);
            groupBox2.Controls.Add(rbByAddress);
            groupBox2.Controls.Add(rbByParent);
            groupBox2.Controls.Add(rbByPhone);
            groupBox2.Controls.Add(rbByName);
            groupBox2.Location = new Point(1475, 149);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(312, 525);
            groupBox2.TabIndex = 19;
            groupBox2.TabStop = false;
            groupBox2.Text = "დეტალური ძიება";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(33, 383);
            txtSearch.Margin = new Padding(4, 3, 4, 3);
            txtSearch.Multiline = true;
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(259, 33);
            txtSearch.TabIndex = 1;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // rbByStCode
            // 
            rbByStCode.AutoSize = true;
            rbByStCode.Location = new Point(23, 356);
            rbByStCode.Margin = new Padding(4, 3, 4, 3);
            rbByStCode.Name = "rbByStCode";
            rbByStCode.Size = new Size(58, 19);
            rbByStCode.TabIndex = 0;
            rbByStCode.TabStop = true;
            rbByStCode.Text = "კოდი";
            rbByStCode.UseVisualStyleBackColor = true;
            // 
            // rbByAge
            // 
            rbByAge.AutoSize = true;
            rbByAge.Location = new Point(23, 250);
            rbByAge.Margin = new Padding(4, 3, 4, 3);
            rbByAge.Name = "rbByAge";
            rbByAge.Size = new Size(57, 19);
            rbByAge.TabIndex = 0;
            rbByAge.TabStop = true;
            rbByAge.Text = "ასაკი";
            rbByAge.UseVisualStyleBackColor = true;
            // 
            // rbByIdNumber
            // 
            rbByIdNumber.AutoSize = true;
            rbByIdNumber.Location = new Point(23, 303);
            rbByIdNumber.Margin = new Padding(4, 3, 4, 3);
            rbByIdNumber.Name = "rbByIdNumber";
            rbByIdNumber.Size = new Size(119, 19);
            rbByIdNumber.TabIndex = 0;
            rbByIdNumber.TabStop = true;
            rbByIdNumber.Text = "პირადი ნომერი";
            rbByIdNumber.UseVisualStyleBackColor = true;
            // 
            // rbByLastName
            // 
            rbByLastName.AutoSize = true;
            rbByLastName.Location = new Point(23, 197);
            rbByLastName.Margin = new Padding(4, 3, 4, 3);
            rbByLastName.Name = "rbByLastName";
            rbByLastName.Size = new Size(60, 19);
            rbByLastName.TabIndex = 0;
            rbByLastName.TabStop = true;
            rbByLastName.Text = "გვარი";
            rbByLastName.UseVisualStyleBackColor = true;
            // 
            // rbByAddress
            // 
            rbByAddress.AutoSize = true;
            rbByAddress.Location = new Point(23, 330);
            rbByAddress.Margin = new Padding(4, 3, 4, 3);
            rbByAddress.Name = "rbByAddress";
            rbByAddress.Size = new Size(92, 19);
            rbByAddress.TabIndex = 0;
            rbByAddress.TabStop = true;
            rbByAddress.Text = "მისამართი";
            rbByAddress.UseVisualStyleBackColor = true;
            // 
            // rbByParent
            // 
            rbByParent.AutoSize = true;
            rbByParent.Location = new Point(23, 224);
            rbByParent.Margin = new Padding(4, 3, 4, 3);
            rbByParent.Name = "rbByParent";
            rbByParent.Size = new Size(82, 19);
            rbByParent.TabIndex = 0;
            rbByParent.TabStop = true;
            rbByParent.Text = "მშობელი";
            rbByParent.UseVisualStyleBackColor = true;
            // 
            // rbByPhone
            // 
            rbByPhone.AutoSize = true;
            rbByPhone.Location = new Point(23, 277);
            rbByPhone.Margin = new Padding(4, 3, 4, 3);
            rbByPhone.Name = "rbByPhone";
            rbByPhone.Size = new Size(93, 19);
            rbByPhone.TabIndex = 0;
            rbByPhone.TabStop = true;
            rbByPhone.Text = "ტელეფონი";
            rbByPhone.UseVisualStyleBackColor = true;
            // 
            // rbByName
            // 
            rbByName.AutoSize = true;
            rbByName.Location = new Point(23, 171);
            rbByName.Margin = new Padding(4, 3, 4, 3);
            rbByName.Name = "rbByName";
            rbByName.Size = new Size(71, 19);
            rbByName.TabIndex = 0;
            rbByName.TabStop = true;
            rbByName.Text = "სახელი";
            rbByName.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Location = new Point(1475, 14);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(313, 127);
            groupBox3.TabIndex = 20;
            groupBox3.TabStop = false;
            groupBox3.Text = "groupBox3";
            // 
            // chkstudentsToGroups
            // 
            chkstudentsToGroups.AutoSize = true;
            chkstudentsToGroups.Location = new Point(23, 128);
            chkstudentsToGroups.Name = "chkstudentsToGroups";
            chkstudentsToGroups.Size = new Size(232, 19);
            chkstudentsToGroups.TabIndex = 19;
            chkstudentsToGroups.Text = "მოსწავლეები რამდენიმე ჯგუფში";
            chkstudentsToGroups.UseVisualStyleBackColor = true;
            // 
            // StudentsEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1801, 932);
            Controls.Add(btndel);
            Controls.Add(groupBox3);
            Controls.Add(btnClear);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(dgvStudents);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "StudentsEditForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "StudentsEditForm";
            FormClosing += StudentsEditForm_FormClosing;
            Load += StudentsEditForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvStudents).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGroupSubGroups).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvStudents;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem წაშლაToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem სვეტებისმართვაToolStripMenuItem;
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
        private GroupBox groupBox3;
        private CheckBox chkstudentsToGroups;
    }
}
