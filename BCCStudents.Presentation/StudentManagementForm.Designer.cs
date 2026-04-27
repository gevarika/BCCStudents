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
            dataGridView1 = new DataGridView();
            txtFirstName = new TextBox();
            txtLastName = new TextBox();
            txtAge = new TextBox();
            btnAddStudent = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            txtParentName = new TextBox();
            txtPhoneNumber = new TextBox();
            label6 = new Label();
            label8 = new Label();
            label11 = new Label();
            cmbDiscount = new ComboBox();
            label13 = new Label();
            linkLabel1 = new LinkLabel();
            label14 = new Label();
            btnExportToExcell = new Button();
            btnImportFromExcell = new Button();
            txtAddress = new TextBox();
            label7 = new Label();
            menuStrip1 = new MenuStrip();
            რედაქტირებაToolStripMenuItem = new ToolStripMenuItem();
            tsmFailedStudents = new ToolStripMenuItem();
            ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem = new ToolStripMenuItem();
            მოსწავლისრედაქტირებაToolStripMenuItem = new ToolStripMenuItem();
            txtIdNumb = new TextBox();
            label15 = new Label();
            numTuitionFee = new TextBox();
            clbGroups = new CheckedListBox();
            groupBox1 = new GroupBox();
            LinklblRefresh = new LinkLabel();
            lblTuitionFee = new Label();
            labelTF = new Label();
            groupBox2 = new GroupBox();
            chkPrintContract = new CheckBox();
            btnSearchFolder = new Button();
            txtStudentDocPath = new TextBox();
            txtStudentInfo = new TextBox();
            label9 = new Label();
            label10 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(4, 533);
            dataGridView1.Margin = new Padding(4, 3, 4, 3);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1634, 308);
            dataGridView1.TabIndex = 0;
            // 
            // txtFirstName
            // 
            txtFirstName.AutoCompleteMode = AutoCompleteMode.Suggest;
            txtFirstName.AutoCompleteSource = AutoCompleteSource.HistoryList;
            txtFirstName.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFirstName.Location = new Point(155, 38);
            txtFirstName.Margin = new Padding(4, 3, 4, 3);
            txtFirstName.Multiline = true;
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(240, 34);
            txtFirstName.TabIndex = 1;
            txtFirstName.Text = "გიორგი";
            // 
            // txtLastName
            // 
            txtLastName.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtLastName.Location = new Point(155, 80);
            txtLastName.Margin = new Padding(4, 3, 4, 3);
            txtLastName.Multiline = true;
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(240, 33);
            txtLastName.TabIndex = 2;
            txtLastName.Text = "თაყნიაშვილი";
            // 
            // txtAge
            // 
            txtAge.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAge.Location = new Point(155, 120);
            txtAge.Margin = new Padding(4, 3, 4, 3);
            txtAge.Multiline = true;
            txtAge.Name = "txtAge";
            txtAge.Size = new Size(240, 27);
            txtAge.TabIndex = 3;
            txtAge.Text = "12";
            // 
            // btnAddStudent
            // 
            btnAddStudent.Location = new Point(1104, 442);
            btnAddStudent.Margin = new Padding(4, 3, 4, 3);
            btnAddStudent.Name = "btnAddStudent";
            btnAddStudent.Size = new Size(127, 39);
            btnAddStudent.TabIndex = 6;
            btnAddStudent.Text = "დამატება";
            btnAddStudent.UseVisualStyleBackColor = true;
            btnAddStudent.Click += btnAddStudent_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(83, 42);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(56, 16);
            label1.TabIndex = 7;
            label1.Text = "სახელი";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(94, 81);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(45, 16);
            label2.TabIndex = 7;
            label2.Text = "გვარი";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(99, 121);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 16);
            label3.TabIndex = 7;
            label3.Text = "ასაკი";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(88, 153);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(51, 16);
            label4.TabIndex = 7;
            label4.Text = "ჯგუფი";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(41, 355);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(91, 16);
            label5.TabIndex = 7;
            label5.Text = "ფასდაკლება";
            // 
            // txtParentName
            // 
            txtParentName.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtParentName.Location = new Point(188, 38);
            txtParentName.Margin = new Padding(4, 3, 4, 3);
            txtParentName.Multiline = true;
            txtParentName.Name = "txtParentName";
            txtParentName.Size = new Size(233, 34);
            txtParentName.TabIndex = 8;
            txtParentName.Text = "ლევანი თაყნიაშვილი";
            // 
            // txtPhoneNumber
            // 
            txtPhoneNumber.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPhoneNumber.Location = new Point(188, 83);
            txtPhoneNumber.Margin = new Padding(4, 3, 4, 3);
            txtPhoneNumber.Multiline = true;
            txtPhoneNumber.Name = "txtPhoneNumber";
            txtPhoneNumber.Size = new Size(233, 29);
            txtPhoneNumber.TabIndex = 8;
            txtPhoneNumber.Text = "557115233";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(41, 42);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(119, 16);
            label6.TabIndex = 7;
            label6.Text = "მშობლის სახელი";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(14, 89);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(142, 16);
            label8.TabIndex = 7;
            label8.Text = "ტელეფონის ნომერი";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.Location = new Point(46, 317);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(87, 16);
            label11.TabIndex = 7;
            label11.Text = "გადასახადი";
            // 
            // cmbDiscount
            // 
            cmbDiscount.FormattingEnabled = true;
            cmbDiscount.Items.AddRange(new object[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            cmbDiscount.Location = new Point(155, 350);
            cmbDiscount.Margin = new Padding(4, 3, 4, 3);
            cmbDiscount.Name = "cmbDiscount";
            cmbDiscount.Size = new Size(240, 23);
            cmbDiscount.TabIndex = 13;
            cmbDiscount.Text = "0";
            cmbDiscount.SelectedIndexChanged += cmbDiscount_SelectedIndexChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label13.Location = new Point(403, 354);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(20, 17);
            label13.TabIndex = 7;
            label13.Text = "%";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabel1.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabel1.LinkColor = Color.Black;
            linkLabel1.Location = new Point(385, 87);
            linkLabel1.Margin = new Padding(4, 0, 4, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(13, 13);
            linkLabel1.TabIndex = 14;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "+";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label14.Location = new Point(14, 350);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(54, 17);
            label14.TabIndex = 15;
            label14.Text = "label14";
            // 
            // btnExportToExcell
            // 
            btnExportToExcell.Location = new Point(1104, 100);
            btnExportToExcell.Margin = new Padding(4, 3, 4, 3);
            btnExportToExcell.Name = "btnExportToExcell";
            btnExportToExcell.Size = new Size(145, 27);
            btnExportToExcell.TabIndex = 17;
            btnExportToExcell.Text = "ექსპორტი";
            btnExportToExcell.UseVisualStyleBackColor = true;
            btnExportToExcell.Click += btnExportToExcell_Click;
            // 
            // btnImportFromExcell
            // 
            btnImportFromExcell.Location = new Point(1104, 68);
            btnImportFromExcell.Margin = new Padding(4, 3, 4, 3);
            btnImportFromExcell.Name = "btnImportFromExcell";
            btnImportFromExcell.Size = new Size(145, 27);
            btnImportFromExcell.TabIndex = 18;
            btnImportFromExcell.Text = "იმპორტი";
            btnImportFromExcell.UseVisualStyleBackColor = true;
            btnImportFromExcell.Click += btnImportFromExcell_Click;
            // 
            // txtAddress
            // 
            txtAddress.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAddress.Location = new Point(188, 156);
            txtAddress.Margin = new Padding(4, 3, 4, 3);
            txtAddress.Multiline = true;
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(233, 36);
            txtAddress.TabIndex = 3;
            txtAddress.Text = "ლეჩხუმის ქუჩა N11";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.Location = new Point(86, 164);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(80, 16);
            label7.TabIndex = 7;
            label7.Text = "მისამართი";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { რედაქტირებაToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1642, 24);
            menuStrip1.TabIndex = 19;
            menuStrip1.Text = "menuStrip1";
            // 
            // რედაქტირებაToolStripMenuItem
            // 
            რედაქტირებაToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmFailedStudents, ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem, მოსწავლისრედაქტირებაToolStripMenuItem });
            რედაქტირებაToolStripMenuItem.Name = "რედაქტირებაToolStripMenuItem";
            რედაქტირებაToolStripMenuItem.Size = new Size(101, 20);
            რედაქტირებაToolStripMenuItem.Text = "რედაქტირება";
            // 
            // tsmFailedStudents
            // 
            tsmFailedStudents.Name = "tsmFailedStudents";
            tsmFailedStudents.Size = new Size(305, 22);
            tsmFailedStudents.Text = "პრობლემური მოსწავლეები";
            tsmFailedStudents.Click += tsmFailedStudents_Click;
            // 
            // ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem
            // 
            ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Name = "ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem";
            ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Size = new Size(305, 22);
            ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Text = "ონლაინ რეგისტრაციის დადასტურება";
            ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Click += ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem_Click;
            // 
            // მოსწავლისრედაქტირებაToolStripMenuItem
            // 
            მოსწავლისრედაქტირებაToolStripMenuItem.Name = "მოსწავლისრედაქტირებაToolStripMenuItem";
            მოსწავლისრედაქტირებაToolStripMenuItem.Size = new Size(305, 22);
            მოსწავლისრედაქტირებაToolStripMenuItem.Text = "მოსწავლის რედაქტირება";
            მოსწავლისრედაქტირებაToolStripMenuItem.Click += მოსწავლისრედაქტირებაToolStripMenuItem_Click;
            // 
            // txtIdNumb
            // 
            txtIdNumb.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtIdNumb.Location = new Point(188, 120);
            txtIdNumb.Margin = new Padding(4, 3, 4, 3);
            txtIdNumb.Multiline = true;
            txtIdNumb.Name = "txtIdNumb";
            txtIdNumb.Size = new Size(233, 27);
            txtIdNumb.TabIndex = 22;
            txtIdNumb.Text = "10001030549";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.Location = new Point(52, 123);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(109, 16);
            label15.TabIndex = 23;
            label15.Text = "პირადი ნომერი";
            // 
            // numTuitionFee
            // 
            numTuitionFee.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numTuitionFee.Location = new Point(155, 310);
            numTuitionFee.Margin = new Padding(4, 3, 4, 3);
            numTuitionFee.Multiline = true;
            numTuitionFee.Name = "numTuitionFee";
            numTuitionFee.ReadOnly = true;
            numTuitionFee.Size = new Size(178, 32);
            numTuitionFee.TabIndex = 3;
            numTuitionFee.Text = "0";
            // 
            // clbGroups
            // 
            clbGroups.CheckOnClick = true;
            clbGroups.FormattingEnabled = true;
            clbGroups.Location = new Point(155, 155);
            clbGroups.Margin = new Padding(4, 3, 4, 3);
            clbGroups.Name = "clbGroups";
            clbGroups.Size = new Size(271, 130);
            clbGroups.TabIndex = 24;
            clbGroups.ItemCheck += clbGroups_ItemCheck;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(LinklblRefresh);
            groupBox1.Controls.Add(txtFirstName);
            groupBox1.Controls.Add(numTuitionFee);
            groupBox1.Controls.Add(clbGroups);
            groupBox1.Controls.Add(txtLastName);
            groupBox1.Controls.Add(txtAge);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(lblTuitionFee);
            groupBox1.Controls.Add(labelTF);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(cmbDiscount);
            groupBox1.Location = new Point(14, 48);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(611, 443);
            groupBox1.TabIndex = 25;
            groupBox1.TabStop = false;
            groupBox1.Text = "მოსწავლის მონაცემები";
            // 
            // LinklblRefresh
            // 
            LinklblRefresh.AutoSize = true;
            LinklblRefresh.Location = new Point(433, 162);
            LinklblRefresh.Name = "LinklblRefresh";
            LinklblRefresh.Size = new Size(72, 15);
            LinklblRefresh.TabIndex = 25;
            LinklblRefresh.TabStop = true;
            LinklblRefresh.Text = "განახლება";
            LinklblRefresh.VisitedLinkColor = Color.Blue;
            LinklblRefresh.LinkClicked += LinklblRefresh_LinkClicked;
            // 
            // lblTuitionFee
            // 
            lblTuitionFee.AutoSize = true;
            lblTuitionFee.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTuitionFee.Location = new Point(152, 393);
            lblTuitionFee.Margin = new Padding(4, 0, 4, 0);
            lblTuitionFee.Name = "lblTuitionFee";
            lblTuitionFee.Size = new Size(0, 20);
            lblTuitionFee.TabIndex = 7;
            // 
            // labelTF
            // 
            labelTF.AutoSize = true;
            labelTF.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelTF.Location = new Point(38, 393);
            labelTF.Margin = new Padding(4, 0, 4, 0);
            labelTF.Name = "labelTF";
            labelTF.Size = new Size(90, 16);
            labelTF.TabIndex = 7;
            labelTF.Text = "გადასახადი:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chkPrintContract);
            groupBox2.Controls.Add(btnSearchFolder);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(txtStudentDocPath);
            groupBox2.Controls.Add(txtStudentInfo);
            groupBox2.Controls.Add(txtAddress);
            groupBox2.Controls.Add(txtIdNumb);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(txtParentName);
            groupBox2.Controls.Add(txtPhoneNumber);
            groupBox2.Controls.Add(linkLabel1);
            groupBox2.Location = new Point(640, 48);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(456, 443);
            groupBox2.TabIndex = 26;
            groupBox2.TabStop = false;
            groupBox2.Text = "მშობლის მონაცემები";
            // 
            // chkPrintContract
            // 
            chkPrintContract.AutoSize = true;
            chkPrintContract.Location = new Point(23, 392);
            chkPrintContract.Margin = new Padding(4, 3, 4, 3);
            chkPrintContract.Name = "chkPrintContract";
            chkPrintContract.Size = new Size(176, 19);
            chkPrintContract.TabIndex = 24;
            chkPrintContract.Text = "ხელშეკრულების ბეჭდვა";
            chkPrintContract.UseVisualStyleBackColor = true;
            // 
            // btnSearchFolder
            // 
            btnSearchFolder.Location = new Point(329, 343);
            btnSearchFolder.Margin = new Padding(4, 3, 4, 3);
            btnSearchFolder.Name = "btnSearchFolder";
            btnSearchFolder.Size = new Size(88, 27);
            btnSearchFolder.TabIndex = 24;
            btnSearchFolder.Text = "არჩევა";
            btnSearchFolder.UseVisualStyleBackColor = true;
            btnSearchFolder.Click += btnSearchFolder_Click;
            // 
            // txtStudentDocPath
            // 
            txtStudentDocPath.Location = new Point(188, 271);
            txtStudentDocPath.Margin = new Padding(4, 3, 4, 3);
            txtStudentDocPath.Multiline = true;
            txtStudentDocPath.Name = "txtStudentDocPath";
            txtStudentDocPath.ReadOnly = true;
            txtStudentDocPath.Size = new Size(233, 64);
            txtStudentDocPath.TabIndex = 3;
            // 
            // txtStudentInfo
            // 
            txtStudentInfo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtStudentInfo.Location = new Point(188, 200);
            txtStudentInfo.Margin = new Padding(4, 3, 4, 3);
            txtStudentInfo.Multiline = true;
            txtStudentInfo.Name = "txtStudentInfo";
            txtStudentInfo.Size = new Size(233, 36);
            txtStudentInfo.TabIndex = 3;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(22, 272);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(160, 16);
            label9.TabIndex = 7;
            label9.Text = "მოსწავლის საქაღალდე";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Microsoft Sans Serif", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(15, 208);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(141, 16);
            label10.TabIndex = 7;
            label10.Text = "მოსწავლის სტატუსი";
            // 
            // StudentManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1642, 848);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnImportFromExcell);
            Controls.Add(btnExportToExcell);
            Controls.Add(btnAddStudent);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "StudentManagementForm";
            StartPosition = FormStartPosition.CenterScreen;
            Load += StudentManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.ToolStripMenuItem მოსწავლისრედაქტირებაToolStripMenuItem;
        private LinkLabel LinklblRefresh;
    }
}
