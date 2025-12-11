namespace BCCStudents.Presentation
{
    partial class StudentManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudentManagementForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.btnAddStudent = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtParentName = new System.Windows.Forms.TextBox();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbDiscount = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label14 = new System.Windows.Forms.Label();
            this.btnExportToExcell = new System.Windows.Forms.Button();
            this.btnImportFromExcell = new System.Windows.Forms.Button();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.რედაქტირებაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmFailedStudents = new System.Windows.Forms.ToolStripMenuItem();
            this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtIdNumb = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.numTuitionFee = new System.Windows.Forms.TextBox();
            this.clbGroups = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTuitionFee = new System.Windows.Forms.Label();
            this.labelTF = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkPrintContract = new System.Windows.Forms.CheckBox();
            this.btnSearchFolder = new System.Windows.Forms.Button();
            this.txtStudentDocPath = new System.Windows.Forms.TextBox();
            this.txtStudentInfo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.მოსწავლისრედაქტირებაToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 462);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1401, 267);
            this.dataGridView1.TabIndex = 0;
            // 
            // txtFirstName
            // 
            this.txtFirstName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtFirstName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.txtFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(133, 33);
            this.txtFirstName.Multiline = true;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(206, 30);
            this.txtFirstName.TabIndex = 1;
            this.txtFirstName.Text = "გიორგი";
            // 
            // txtLastName
            // 
            this.txtLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(133, 69);
            this.txtLastName.Multiline = true;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(206, 29);
            this.txtLastName.TabIndex = 2;
            this.txtLastName.Text = "თაყნიაშვილი";
            // 
            // txtAge
            // 
            this.txtAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAge.Location = new System.Drawing.Point(133, 104);
            this.txtAge.Multiline = true;
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(206, 24);
            this.txtAge.TabIndex = 3;
            this.txtAge.Text = "12";
            // 
            // btnAddStudent
            // 
            this.btnAddStudent.Location = new System.Drawing.Point(946, 383);
            this.btnAddStudent.Name = "btnAddStudent";
            this.btnAddStudent.Size = new System.Drawing.Size(109, 34);
            this.btnAddStudent.TabIndex = 6;
            this.btnAddStudent.Text = "დამატება";
            this.btnAddStudent.UseVisualStyleBackColor = true;
            this.btnAddStudent.Click += new System.EventHandler(this.btnAddStudent_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(71, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "სახელი";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(81, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "გვარი";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(85, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "ასაკი";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(75, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "ჯგუფი";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(35, 308);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "ფასდაკლება";
            // 
            // txtParentName
            // 
            this.txtParentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParentName.Location = new System.Drawing.Point(161, 33);
            this.txtParentName.Multiline = true;
            this.txtParentName.Name = "txtParentName";
            this.txtParentName.Size = new System.Drawing.Size(200, 30);
            this.txtParentName.TabIndex = 8;
            this.txtParentName.Text = "ლევანი თაყნიაშვილი";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhoneNumber.Location = new System.Drawing.Point(161, 72);
            this.txtPhoneNumber.Multiline = true;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(200, 26);
            this.txtPhoneNumber.TabIndex = 8;
            this.txtPhoneNumber.Text = "557115233";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(35, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "მშობლის სახელი";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 16);
            this.label8.TabIndex = 7;
            this.label8.Text = "ტელეფონის ნომერი";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(39, 275);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 16);
            this.label11.TabIndex = 7;
            this.label11.Text = "გადასახადი";
            // 
            // cmbDiscount
            // 
            this.cmbDiscount.FormattingEnabled = true;
            this.cmbDiscount.Items.AddRange(new object[] {
            "0",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "35",
            "40",
            "45",
            "50",
            "55",
            "60",
            "65",
            "70",
            "75",
            "80",
            "85",
            "90",
            "95",
            "100"});
            this.cmbDiscount.Location = new System.Drawing.Point(133, 303);
            this.cmbDiscount.Name = "cmbDiscount";
            this.cmbDiscount.Size = new System.Drawing.Size(206, 21);
            this.cmbDiscount.TabIndex = 13;
            this.cmbDiscount.Text = "0";
            this.cmbDiscount.SelectedIndexChanged += new System.EventHandler(this.cmbDiscount_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(292, 307);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(20, 17);
            this.label13.TabIndex = 7;
            this.label13.Text = "%";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Black;
            this.linkLabel1.Location = new System.Drawing.Point(330, 75);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(13, 13);
            this.linkLabel1.TabIndex = 14;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "+";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(12, 303);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 17);
            this.label14.TabIndex = 15;
            this.label14.Text = "label14";
            // 
            // btnExportToExcell
            // 
            this.btnExportToExcell.Location = new System.Drawing.Point(946, 87);
            this.btnExportToExcell.Name = "btnExportToExcell";
            this.btnExportToExcell.Size = new System.Drawing.Size(124, 23);
            this.btnExportToExcell.TabIndex = 17;
            this.btnExportToExcell.Text = "ექსპორტი";
            this.btnExportToExcell.UseVisualStyleBackColor = true;
            this.btnExportToExcell.Click += new System.EventHandler(this.btnExportToExcell_Click);
            // 
            // btnImportFromExcell
            // 
            this.btnImportFromExcell.Location = new System.Drawing.Point(946, 59);
            this.btnImportFromExcell.Name = "btnImportFromExcell";
            this.btnImportFromExcell.Size = new System.Drawing.Size(124, 23);
            this.btnImportFromExcell.TabIndex = 18;
            this.btnImportFromExcell.Text = "იმპორტი";
            this.btnImportFromExcell.UseVisualStyleBackColor = true;
            this.btnImportFromExcell.Click += new System.EventHandler(this.btnImportFromExcell_Click);
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(161, 135);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(200, 32);
            this.txtAddress.TabIndex = 3;
            this.txtAddress.Text = "ლეჩხუმის ქუჩა N11";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(74, 142);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 16);
            this.label7.TabIndex = 7;
            this.label7.Text = "მისამართი";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.რედაქტირებაToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1407, 24);
            this.menuStrip1.TabIndex = 19;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // რედაქტირებაToolStripMenuItem
            // 
            this.რედაქტირებაToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmFailedStudents,
            this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem,
            this.მოსწავლისრედაქტირებაToolStripMenuItem});
            this.რედაქტირებაToolStripMenuItem.Name = "რედაქტირებაToolStripMenuItem";
            this.რედაქტირებაToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.რედაქტირებაToolStripMenuItem.Text = "რედაქტირება";
            // 
            // tsmFailedStudents
            // 
            this.tsmFailedStudents.Name = "tsmFailedStudents";
            this.tsmFailedStudents.Size = new System.Drawing.Size(305, 22);
            this.tsmFailedStudents.Text = "პრობლემური მოსწავლეები";
            this.tsmFailedStudents.Click += new System.EventHandler(this.tsmFailedStudents_Click);
            // 
            // ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem
            // 
            this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Name = "ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem";
            this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Text = "ონლაინ რეგისტრაციის დადასტურება";
            this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Click += new System.EventHandler(this.ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem_Click);
            // 
            // txtIdNumb
            // 
            this.txtIdNumb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIdNumb.Location = new System.Drawing.Point(161, 104);
            this.txtIdNumb.Multiline = true;
            this.txtIdNumb.Name = "txtIdNumb";
            this.txtIdNumb.Size = new System.Drawing.Size(200, 24);
            this.txtIdNumb.TabIndex = 22;
            this.txtIdNumb.Text = "10001030549";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(45, 107);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(109, 16);
            this.label15.TabIndex = 23;
            this.label15.Text = "პირადი ნომერი";
            // 
            // numTuitionFee
            // 
            this.numTuitionFee.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numTuitionFee.Location = new System.Drawing.Point(133, 269);
            this.numTuitionFee.Multiline = true;
            this.numTuitionFee.Name = "numTuitionFee";
            this.numTuitionFee.ReadOnly = true;
            this.numTuitionFee.Size = new System.Drawing.Size(153, 28);
            this.numTuitionFee.TabIndex = 3;
            this.numTuitionFee.Text = "0";
            // 
            // clbGroups
            // 
            this.clbGroups.CheckOnClick = true;
            this.clbGroups.FormattingEnabled = true;
            this.clbGroups.Location = new System.Drawing.Point(133, 134);
            this.clbGroups.Name = "clbGroups";
            this.clbGroups.Size = new System.Drawing.Size(233, 124);
            this.clbGroups.TabIndex = 24;
            this.clbGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbGroups_ItemCheck);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtFirstName);
            this.groupBox1.Controls.Add(this.numTuitionFee);
            this.groupBox1.Controls.Add(this.clbGroups);
            this.groupBox1.Controls.Add(this.txtLastName);
            this.groupBox1.Controls.Add(this.txtAge);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblTuitionFee);
            this.groupBox1.Controls.Add(this.labelTF);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.cmbDiscount);
            this.groupBox1.Location = new System.Drawing.Point(12, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 384);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "მოსწავლის მონაცემები";
            // 
            // lblTuitionFee
            // 
            this.lblTuitionFee.AutoSize = true;
            this.lblTuitionFee.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTuitionFee.Location = new System.Drawing.Point(130, 341);
            this.lblTuitionFee.Name = "lblTuitionFee";
            this.lblTuitionFee.Size = new System.Drawing.Size(0, 20);
            this.lblTuitionFee.TabIndex = 7;
            // 
            // labelTF
            // 
            this.labelTF.AutoSize = true;
            this.labelTF.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTF.Location = new System.Drawing.Point(33, 341);
            this.labelTF.Name = "labelTF";
            this.labelTF.Size = new System.Drawing.Size(90, 16);
            this.labelTF.TabIndex = 7;
            this.labelTF.Text = "გადასახადი:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkPrintContract);
            this.groupBox2.Controls.Add(this.btnSearchFolder);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtStudentDocPath);
            this.groupBox2.Controls.Add(this.txtStudentInfo);
            this.groupBox2.Controls.Add(this.txtAddress);
            this.groupBox2.Controls.Add(this.txtIdNumb);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.txtParentName);
            this.groupBox2.Controls.Add(this.txtPhoneNumber);
            this.groupBox2.Controls.Add(this.linkLabel1);
            this.groupBox2.Location = new System.Drawing.Point(549, 47);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(391, 379);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "მშობლის მონაცემები";
            // 
            // chkPrintContract
            // 
            this.chkPrintContract.AutoSize = true;
            this.chkPrintContract.Location = new System.Drawing.Point(20, 340);
            this.chkPrintContract.Name = "chkPrintContract";
            this.chkPrintContract.Size = new System.Drawing.Size(160, 17);
            this.chkPrintContract.TabIndex = 24;
            this.chkPrintContract.Text = "ხელშეკრულების ბეჭდვა";
            this.chkPrintContract.UseVisualStyleBackColor = true;
            // 
            // btnSearchFolder
            // 
            this.btnSearchFolder.Location = new System.Drawing.Point(282, 297);
            this.btnSearchFolder.Name = "btnSearchFolder";
            this.btnSearchFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSearchFolder.TabIndex = 24;
            this.btnSearchFolder.Text = "არჩევა";
            this.btnSearchFolder.UseVisualStyleBackColor = true;
            this.btnSearchFolder.Click += new System.EventHandler(this.btnSearchFolder_Click);
            // 
            // txtStudentDocPath
            // 
            this.txtStudentDocPath.Location = new System.Drawing.Point(161, 235);
            this.txtStudentDocPath.Multiline = true;
            this.txtStudentDocPath.Name = "txtStudentDocPath";
            this.txtStudentDocPath.ReadOnly = true;
            this.txtStudentDocPath.Size = new System.Drawing.Size(200, 56);
            this.txtStudentDocPath.TabIndex = 3;
            // 
            // txtStudentInfo
            // 
            this.txtStudentInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStudentInfo.Location = new System.Drawing.Point(161, 173);
            this.txtStudentInfo.Multiline = true;
            this.txtStudentInfo.Name = "txtStudentInfo";
            this.txtStudentInfo.Size = new System.Drawing.Size(200, 32);
            this.txtStudentInfo.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(19, 236);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 16);
            this.label9.TabIndex = 7;
            this.label9.Text = "მოსწავლის საქაღალდე";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(13, 180);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(141, 16);
            this.label10.TabIndex = 7;
            this.label10.Text = "მოსწავლის სტატუსი";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(946, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "ტესტ ფორმა";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // მოსწავლისრედაქტირებაToolStripMenuItem
            // 
            this.მოსწავლისრედაქტირებაToolStripMenuItem.Name = "მოსწავლისრედაქტირებაToolStripMenuItem";
            this.მოსწავლისრედაქტირებაToolStripMenuItem.Size = new System.Drawing.Size(305, 22);
            this.მოსწავლისრედაქტირებაToolStripMenuItem.Text = "მოსწავლის რედაქტირება";
            this.მოსწავლისრედაქტირებაToolStripMenuItem.Click += new System.EventHandler(this.მოსწავლისრედაქტირებაToolStripMenuItem_Click);
            // 
            // StudentManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1407, 735);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnImportFromExcell);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnExportToExcell);
            this.Controls.Add(this.btnAddStudent);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "StudentManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.StudentManagementForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtAge;
        private System.Windows.Forms.Button btnAddStudent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtParentName;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbDiscount;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnExportToExcell;
        private System.Windows.Forms.Button btnImportFromExcell;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem რედაქტირებაToolStripMenuItem;
        private System.Windows.Forms.TextBox txtIdNumb;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox numTuitionFee;
        private System.Windows.Forms.CheckedListBox clbGroups;
        private System.Windows.Forms.ToolStripMenuItem tsmFailedStudents;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkPrintContract;
        private System.Windows.Forms.ToolStripMenuItem ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem;
        private System.Windows.Forms.Label lblTuitionFee;
        private System.Windows.Forms.Label labelTF;
        private System.Windows.Forms.Button btnSearchFolder;
        private System.Windows.Forms.TextBox txtStudentDocPath;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtStudentInfo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem მოსწავლისრედაქტირებაToolStripMenuItem;
    }
}
