namespace BCCStudents.Presentation
{
    partial class AdminPanelForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminPanelForm));
            tabControl1 = new TabControl();
            DatabaseSettings = new TabPage();
            groupBox2 = new GroupBox();
            btnAddStudentToGroup = new Button();
            btnRestore = new Button();
            btnSync = new Button();
            btnBackup = new Button();
            btnCheckStudents = new Button();
            dGVUnassignedStudents = new DataGridView();
            btnResetData = new Button();
            groupBox1 = new GroupBox();
            groupBox4 = new GroupBox();
            serverConnStatus = new Label();
            serverHost = new TextBox();
            label21 = new Label();
            serverPort = new TextBox();
            label20 = new Label();
            saveServerConn = new Button();
            serverDbName = new TextBox();
            label19 = new Label();
            testSrvConn = new Button();
            serverUsrName = new TextBox();
            label17 = new Label();
            label18 = new Label();
            serverUsrPass = new TextBox();
            groupBox3 = new GroupBox();
            localHost = new TextBox();
            label14 = new Label();
            LocalPort = new TextBox();
            label13 = new Label();
            localDbName = new TextBox();
            label12 = new Label();
            localUsrName = new TextBox();
            label11 = new Label();
            localUsrPass = new TextBox();
            label10 = new Label();
            testLocalConn = new Button();
            lblDbMode = new Label();
            saveLocalConn = new Button();
            btnSwitchToTestDB = new Button();
            localConnStatus = new Label();
            UserManagement = new TabPage();
            lblusersInfo = new Label();
            toolStrip1 = new ToolStrip();
            dgvRegisteredUsers = new DataGridView();
            btnRegisterUser = new Button();
            PaymentsFinance = new TabPage();
            btnSetPaymentDate = new Button();
            btnSetStudyStartDate = new Button();
            SystemOperations = new TabPage();
            sogBox2 = new GroupBox();
            sogBox1 = new GroupBox();
            btnSaveDocPath = new Button();
            label3 = new Label();
            txtDownloadFolder = new TextBox();
            txtBaseUrl = new TextBox();
            dgvGroups = new DataGridView();
            btnChooseDir = new Button();
            label2 = new Label();
            btnSave = new Button();
            label1 = new Label();
            SMSServiceSettings = new TabPage();
            chkSmsEnabled = new CheckBox();
            btnSaveOverSmsTexts = new Button();
            btnSaveUpcPaySmsTexts = new Button();
            btnSavePaySmsTexts = new Button();
            btnSaveRegSmsTexts = new Button();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            txtSmsOverdue = new TextBox();
            txtSmsPayment = new TextBox();
            txtSmsUpcoming = new TextBox();
            txtSmsRegistration = new TextBox();
            label5 = new Label();
            label4 = new Label();
            tbTestNumber = new TextBox();
            btnTest = new Button();
            btnSaveApiKey = new Button();
            txtSmsApiKey = new TextBox();
            AutoFileDetectionSettings = new TabPage();
            groupBoxAutoDetection = new GroupBox();
            btnSaveAutoDetectionSettings = new Button();
            btnSelectDirectory = new Button();
            txtFileNamePattern = new TextBox();
            txtWatchDirectory = new TextBox();
            labelFileNamePattern = new Label();
            labelWatchDirectory = new Label();
            chkAutoDetectionEnabled = new CheckBox();
            menuStrip1 = new MenuStrip();
            tabControl1.SuspendLayout();
            DatabaseSettings.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dGVUnassignedStudents).BeginInit();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            UserManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRegisteredUsers).BeginInit();
            PaymentsFinance.SuspendLayout();
            SystemOperations.SuspendLayout();
            sogBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGroups).BeginInit();
            SMSServiceSettings.SuspendLayout();
            AutoFileDetectionSettings.SuspendLayout();
            groupBoxAutoDetection.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(DatabaseSettings);
            tabControl1.Controls.Add(UserManagement);
            tabControl1.Controls.Add(PaymentsFinance);
            tabControl1.Controls.Add(SystemOperations);
            tabControl1.Controls.Add(SMSServiceSettings);
            tabControl1.Controls.Add(AutoFileDetectionSettings);
            tabControl1.Location = new Point(14, 31);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1768, 1028);
            tabControl1.TabIndex = 0;
            // 
            // DatabaseSettings
            // 
            DatabaseSettings.Controls.Add(groupBox2);
            DatabaseSettings.Controls.Add(groupBox1);
            DatabaseSettings.Location = new Point(4, 24);
            DatabaseSettings.Margin = new Padding(4, 3, 4, 3);
            DatabaseSettings.Name = "DatabaseSettings";
            DatabaseSettings.Padding = new Padding(4, 3, 4, 3);
            DatabaseSettings.Size = new Size(1760, 1000);
            DatabaseSettings.TabIndex = 0;
            DatabaseSettings.Text = "Database Settings";
            DatabaseSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnAddStudentToGroup);
            groupBox2.Controls.Add(btnRestore);
            groupBox2.Controls.Add(btnSync);
            groupBox2.Controls.Add(btnBackup);
            groupBox2.Controls.Add(btnCheckStudents);
            groupBox2.Controls.Add(dGVUnassignedStudents);
            groupBox2.Controls.Add(btnResetData);
            groupBox2.Location = new Point(945, 43);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(806, 936);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "groupBox2";
            // 
            // btnAddStudentToGroup
            // 
            btnAddStudentToGroup.Location = new Point(182, 46);
            btnAddStudentToGroup.Margin = new Padding(4, 3, 4, 3);
            btnAddStudentToGroup.Name = "btnAddStudentToGroup";
            btnAddStudentToGroup.Size = new Size(247, 73);
            btnAddStudentToGroup.TabIndex = 5;
            btnAddStudentToGroup.Text = "მოსწავლეების ჯგუფებთან და ქვეჯგუფებთან კავშირის შემოწმება";
            btnAddStudentToGroup.UseVisualStyleBackColor = true;
            btnAddStudentToGroup.Click += btnAddStudentToGroup_Click;
            // 
            // btnRestore
            // 
            btnRestore.Location = new Point(182, 200);
            btnRestore.Margin = new Padding(4, 3, 4, 3);
            btnRestore.Name = "btnRestore";
            btnRestore.Size = new Size(176, 45);
            btnRestore.TabIndex = 6;
            btnRestore.Text = "სარეზერვი კოპირების აღდგენა";
            btnRestore.UseVisualStyleBackColor = true;
            btnRestore.Click += btnRestore_Click;
            // 
            // btnSync
            // 
            btnSync.Location = new Point(436, 46);
            btnSync.Margin = new Padding(4, 3, 4, 3);
            btnSync.Name = "btnSync";
            btnSync.Size = new Size(158, 63);
            btnSync.TabIndex = 0;
            btnSync.Text = "მონაცემების გადატანა";
            btnSync.UseVisualStyleBackColor = true;
            btnSync.Click += btnSync_Click;
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(182, 135);
            btnBackup.Margin = new Padding(4, 3, 4, 3);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(176, 45);
            btnBackup.TabIndex = 6;
            btnBackup.Text = "სარეზერვი კოპირება";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnCheckStudents
            // 
            btnCheckStudents.Location = new Point(436, 117);
            btnCheckStudents.Margin = new Padding(4, 3, 4, 3);
            btnCheckStudents.Name = "btnCheckStudents";
            btnCheckStudents.Size = new Size(158, 63);
            btnCheckStudents.TabIndex = 1;
            btnCheckStudents.Text = "მოსწავლეების ჯგუფთან კავშირის შემოწმება";
            btnCheckStudents.UseVisualStyleBackColor = true;
            btnCheckStudents.Click += btnCheckStudents_Click;
            // 
            // dGVUnassignedStudents
            // 
            dGVUnassignedStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dGVUnassignedStudents.Location = new Point(78, 321);
            dGVUnassignedStudents.Margin = new Padding(4, 3, 4, 3);
            dGVUnassignedStudents.Name = "dGVUnassignedStudents";
            dGVUnassignedStudents.Size = new Size(643, 327);
            dGVUnassignedStudents.TabIndex = 2;
            // 
            // btnResetData
            // 
            btnResetData.Location = new Point(436, 187);
            btnResetData.Margin = new Padding(4, 3, 4, 3);
            btnResetData.Name = "btnResetData";
            btnResetData.Size = new Size(158, 58);
            btnResetData.TabIndex = 3;
            btnResetData.Text = "ყველა ჩანაწერის გასუფთავება";
            btnResetData.UseVisualStyleBackColor = true;
            btnResetData.Click += btnResetData_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(groupBox4);
            groupBox1.Controls.Add(groupBox3);
            groupBox1.Location = new Point(41, 43);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(897, 936);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "ბაზასთან კავშირი";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(serverConnStatus);
            groupBox4.Controls.Add(serverHost);
            groupBox4.Controls.Add(label21);
            groupBox4.Controls.Add(serverPort);
            groupBox4.Controls.Add(label20);
            groupBox4.Controls.Add(saveServerConn);
            groupBox4.Controls.Add(serverDbName);
            groupBox4.Controls.Add(label19);
            groupBox4.Controls.Add(testSrvConn);
            groupBox4.Controls.Add(serverUsrName);
            groupBox4.Controls.Add(label17);
            groupBox4.Controls.Add(label18);
            groupBox4.Controls.Add(serverUsrPass);
            groupBox4.Location = new Point(32, 414);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(657, 335);
            groupBox4.TabIndex = 8;
            groupBox4.TabStop = false;
            groupBox4.Text = "სერვერის მონაცემთა ბაზა";
            // 
            // serverConnStatus
            // 
            serverConnStatus.AutoSize = true;
            serverConnStatus.Location = new Point(574, 246);
            serverConnStatus.Name = "serverConnStatus";
            serverConnStatus.Size = new Size(44, 15);
            serverConnStatus.TabIndex = 7;
            serverConnStatus.Text = "label22";
            // 
            // serverHost
            // 
            serverHost.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverHost.Location = new Point(144, 57);
            serverHost.Margin = new Padding(4, 3, 4, 3);
            serverHost.Multiline = true;
            serverHost.Name = "serverHost";
            serverHost.Size = new Size(231, 34);
            serverHost.TabIndex = 0;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(34, 190);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new Size(98, 15);
            label21.TabIndex = 6;
            label21.Text = "მომხმარებელი";
            // 
            // serverPort
            // 
            serverPort.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverPort.Location = new Point(144, 99);
            serverPort.Margin = new Padding(4, 3, 4, 3);
            serverPort.Multiline = true;
            serverPort.Name = "serverPort";
            serverPort.Size = new Size(231, 34);
            serverPort.TabIndex = 0;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(77, 233);
            label20.Margin = new Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new Size(57, 15);
            label20.TabIndex = 6;
            label20.Text = "პაროლი";
            // 
            // saveServerConn
            // 
            saveServerConn.Location = new Point(251, 282);
            saveServerConn.Margin = new Padding(4, 3, 4, 3);
            saveServerConn.Name = "saveServerConn";
            saveServerConn.Size = new Size(126, 35);
            saveServerConn.TabIndex = 2;
            saveServerConn.Text = "შენახვა";
            saveServerConn.UseVisualStyleBackColor = true;
            saveServerConn.Click += saveServerConn_Click;
            // 
            // serverDbName
            // 
            serverDbName.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverDbName.Location = new Point(144, 140);
            serverDbName.Margin = new Padding(4, 3, 4, 3);
            serverDbName.Multiline = true;
            serverDbName.Name = "serverDbName";
            serverDbName.Size = new Size(231, 34);
            serverDbName.TabIndex = 0;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(40, 148);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(92, 15);
            label19.TabIndex = 6;
            label19.Text = "ბაზის სახელი";
            // 
            // testSrvConn
            // 
            testSrvConn.Location = new Point(405, 233);
            testSrvConn.Margin = new Padding(4, 3, 4, 3);
            testSrvConn.Name = "testSrvConn";
            testSrvConn.Size = new Size(147, 27);
            testSrvConn.TabIndex = 1;
            testSrvConn.Text = "კავშირის შემოწმება";
            testSrvConn.UseVisualStyleBackColor = true;
            testSrvConn.Click += testSrvConn_Click;
            // 
            // serverUsrName
            // 
            serverUsrName.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverUsrName.Location = new Point(144, 182);
            serverUsrName.Margin = new Padding(4, 3, 4, 3);
            serverUsrName.Multiline = true;
            serverUsrName.Name = "serverUsrName";
            serverUsrName.Size = new Size(231, 34);
            serverUsrName.TabIndex = 0;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(104, 65);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(32, 15);
            label17.TabIndex = 6;
            label17.Text = "Host";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(88, 108);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(47, 15);
            label18.TabIndex = 6;
            label18.Text = "პორტი";
            // 
            // serverUsrPass
            // 
            serverUsrPass.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverUsrPass.Location = new Point(144, 223);
            serverUsrPass.Margin = new Padding(4, 3, 4, 3);
            serverUsrPass.Multiline = true;
            serverUsrPass.Name = "serverUsrPass";
            serverUsrPass.Size = new Size(231, 34);
            serverUsrPass.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(localHost);
            groupBox3.Controls.Add(label14);
            groupBox3.Controls.Add(LocalPort);
            groupBox3.Controls.Add(label13);
            groupBox3.Controls.Add(localDbName);
            groupBox3.Controls.Add(label12);
            groupBox3.Controls.Add(localUsrName);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(localUsrPass);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(testLocalConn);
            groupBox3.Controls.Add(lblDbMode);
            groupBox3.Controls.Add(saveLocalConn);
            groupBox3.Controls.Add(btnSwitchToTestDB);
            groupBox3.Controls.Add(localConnStatus);
            groupBox3.Location = new Point(32, 46);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(657, 342);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            groupBox3.Text = "ლოკალური მონაცემთა ბაზა";
            // 
            // localHost
            // 
            localHost.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            localHost.Location = new Point(142, 53);
            localHost.Margin = new Padding(4, 3, 4, 3);
            localHost.Multiline = true;
            localHost.Name = "localHost";
            localHost.Size = new Size(231, 34);
            localHost.TabIndex = 0;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(32, 186);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(98, 15);
            label14.TabIndex = 6;
            label14.Text = "მომხმარებელი";
            // 
            // LocalPort
            // 
            LocalPort.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LocalPort.Location = new Point(142, 95);
            LocalPort.Margin = new Padding(4, 3, 4, 3);
            LocalPort.Multiline = true;
            LocalPort.Name = "LocalPort";
            LocalPort.Size = new Size(231, 34);
            LocalPort.TabIndex = 0;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(75, 229);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(57, 15);
            label13.TabIndex = 6;
            label13.Text = "პაროლი";
            // 
            // localDbName
            // 
            localDbName.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            localDbName.Location = new Point(142, 136);
            localDbName.Margin = new Padding(4, 3, 4, 3);
            localDbName.Multiline = true;
            localDbName.Name = "localDbName";
            localDbName.Size = new Size(231, 34);
            localDbName.TabIndex = 0;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(38, 144);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(92, 15);
            label12.TabIndex = 6;
            label12.Text = "ბაზის სახელი";
            // 
            // localUsrName
            // 
            localUsrName.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            localUsrName.Location = new Point(142, 178);
            localUsrName.Margin = new Padding(4, 3, 4, 3);
            localUsrName.Multiline = true;
            localUsrName.Name = "localUsrName";
            localUsrName.Size = new Size(231, 34);
            localUsrName.TabIndex = 0;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(86, 104);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(47, 15);
            label11.TabIndex = 6;
            label11.Text = "პორტი";
            // 
            // localUsrPass
            // 
            localUsrPass.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            localUsrPass.Location = new Point(142, 219);
            localUsrPass.Margin = new Padding(4, 3, 4, 3);
            localUsrPass.Multiline = true;
            localUsrPass.Name = "localUsrPass";
            localUsrPass.Size = new Size(231, 34);
            localUsrPass.TabIndex = 0;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(102, 61);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(32, 15);
            label10.TabIndex = 6;
            label10.Text = "Host";
            // 
            // testLocalConn
            // 
            testLocalConn.Location = new Point(403, 229);
            testLocalConn.Margin = new Padding(4, 3, 4, 3);
            testLocalConn.Name = "testLocalConn";
            testLocalConn.Size = new Size(147, 27);
            testLocalConn.TabIndex = 1;
            testLocalConn.Text = "კავშირის შემოწმება";
            testLocalConn.UseVisualStyleBackColor = true;
            testLocalConn.Click += testLocalConn_Click;
            // 
            // lblDbMode
            // 
            lblDbMode.AutoSize = true;
            lblDbMode.Location = new Point(467, 95);
            lblDbMode.Margin = new Padding(4, 0, 4, 0);
            lblDbMode.Name = "lblDbMode";
            lblDbMode.Size = new Size(44, 15);
            lblDbMode.TabIndex = 5;
            lblDbMode.Text = "label10";
            // 
            // saveLocalConn
            // 
            saveLocalConn.Location = new Point(249, 278);
            saveLocalConn.Margin = new Padding(4, 3, 4, 3);
            saveLocalConn.Name = "saveLocalConn";
            saveLocalConn.Size = new Size(126, 35);
            saveLocalConn.TabIndex = 2;
            saveLocalConn.Text = "შენახვა";
            saveLocalConn.UseVisualStyleBackColor = true;
            saveLocalConn.Click += saveLocalConn_Click;
            // 
            // btnSwitchToTestDB
            // 
            btnSwitchToTestDB.Location = new Point(460, 56);
            btnSwitchToTestDB.Margin = new Padding(4, 3, 4, 3);
            btnSwitchToTestDB.Name = "btnSwitchToTestDB";
            btnSwitchToTestDB.Size = new Size(126, 27);
            btnSwitchToTestDB.TabIndex = 4;
            btnSwitchToTestDB.Text = "გადართვა";
            btnSwitchToTestDB.UseVisualStyleBackColor = true;
            btnSwitchToTestDB.Click += btnSwitchToTestDB_Click;
            // 
            // localConnStatus
            // 
            localConnStatus.AutoSize = true;
            localConnStatus.Location = new Point(557, 234);
            localConnStatus.Margin = new Padding(4, 0, 4, 0);
            localConnStatus.Name = "localConnStatus";
            localConnStatus.Size = new Size(62, 15);
            localConnStatus.TabIndex = 3;
            localConnStatus.Text = "სტატუსი";
            // 
            // UserManagement
            // 
            UserManagement.Controls.Add(lblusersInfo);
            UserManagement.Controls.Add(toolStrip1);
            UserManagement.Controls.Add(dgvRegisteredUsers);
            UserManagement.Controls.Add(btnRegisterUser);
            UserManagement.Location = new Point(4, 24);
            UserManagement.Margin = new Padding(4, 3, 4, 3);
            UserManagement.Name = "UserManagement";
            UserManagement.Padding = new Padding(4, 3, 4, 3);
            UserManagement.Size = new Size(1760, 1000);
            UserManagement.TabIndex = 1;
            UserManagement.Text = "User Management";
            UserManagement.UseVisualStyleBackColor = true;
            // 
            // lblusersInfo
            // 
            lblusersInfo.AutoSize = true;
            lblusersInfo.Location = new Point(20, 353);
            lblusersInfo.Margin = new Padding(4, 0, 4, 0);
            lblusersInfo.Name = "lblusersInfo";
            lblusersInfo.Size = new Size(41, 15);
            lblusersInfo.TabIndex = 3;
            lblusersInfo.Text = "ინფო";
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(4, 3);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1752, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // dgvRegisteredUsers
            // 
            dgvRegisteredUsers.AllowUserToAddRows = false;
            dgvRegisteredUsers.AllowUserToDeleteRows = false;
            dgvRegisteredUsers.AllowUserToOrderColumns = true;
            dgvRegisteredUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            dgvRegisteredUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRegisteredUsers.Location = new Point(23, 83);
            dgvRegisteredUsers.Margin = new Padding(4, 3, 4, 3);
            dgvRegisteredUsers.Name = "dgvRegisteredUsers";
            dgvRegisteredUsers.ReadOnly = true;
            dgvRegisteredUsers.Size = new Size(1113, 232);
            dgvRegisteredUsers.TabIndex = 1;
            // 
            // btnRegisterUser
            // 
            btnRegisterUser.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRegisterUser.Location = new Point(23, 36);
            btnRegisterUser.Margin = new Padding(4, 3, 4, 3);
            btnRegisterUser.Name = "btnRegisterUser";
            btnRegisterUser.Size = new Size(104, 40);
            btnRegisterUser.TabIndex = 0;
            btnRegisterUser.Text = "დამატება";
            btnRegisterUser.UseVisualStyleBackColor = true;
            btnRegisterUser.Click += btnRegisterUser_Click;
            // 
            // PaymentsFinance
            // 
            PaymentsFinance.Controls.Add(btnSetPaymentDate);
            PaymentsFinance.Controls.Add(btnSetStudyStartDate);
            PaymentsFinance.Location = new Point(4, 24);
            PaymentsFinance.Margin = new Padding(4, 3, 4, 3);
            PaymentsFinance.Name = "PaymentsFinance";
            PaymentsFinance.Padding = new Padding(4, 3, 4, 3);
            PaymentsFinance.Size = new Size(1760, 1000);
            PaymentsFinance.TabIndex = 2;
            PaymentsFinance.Text = "Payments & Finance";
            PaymentsFinance.UseVisualStyleBackColor = true;
            // 
            // btnSetPaymentDate
            // 
            btnSetPaymentDate.Location = new Point(30, 71);
            btnSetPaymentDate.Margin = new Padding(4, 3, 4, 3);
            btnSetPaymentDate.Name = "btnSetPaymentDate";
            btnSetPaymentDate.Size = new Size(309, 35);
            btnSetPaymentDate.TabIndex = 0;
            btnSetPaymentDate.Tag = "Payment";
            btnSetPaymentDate.Text = "გადახდის თარიღის დაყენება";
            btnSetPaymentDate.UseVisualStyleBackColor = true;
            btnSetPaymentDate.Click += btnSetStudyStartDate_Click;
            // 
            // btnSetStudyStartDate
            // 
            btnSetStudyStartDate.Location = new Point(30, 30);
            btnSetStudyStartDate.Margin = new Padding(4, 3, 4, 3);
            btnSetStudyStartDate.Name = "btnSetStudyStartDate";
            btnSetStudyStartDate.Size = new Size(309, 35);
            btnSetStudyStartDate.TabIndex = 0;
            btnSetStudyStartDate.Tag = "Study";
            btnSetStudyStartDate.Text = "სწავლის დაწყების თარიღის დაყენება";
            btnSetStudyStartDate.UseVisualStyleBackColor = true;
            btnSetStudyStartDate.Click += btnSetStudyStartDate_Click;
            // 
            // SystemOperations
            // 
            SystemOperations.Controls.Add(sogBox2);
            SystemOperations.Controls.Add(sogBox1);
            SystemOperations.Location = new Point(4, 24);
            SystemOperations.Margin = new Padding(4, 3, 4, 3);
            SystemOperations.Name = "SystemOperations";
            SystemOperations.Padding = new Padding(4, 3, 4, 3);
            SystemOperations.Size = new Size(1760, 1000);
            SystemOperations.TabIndex = 3;
            SystemOperations.Text = "System Operations";
            SystemOperations.UseVisualStyleBackColor = true;
            // 
            // sogBox2
            // 
            sogBox2.Location = new Point(905, 7);
            sogBox2.Margin = new Padding(4, 3, 4, 3);
            sogBox2.Name = "sogBox2";
            sogBox2.Padding = new Padding(4, 3, 4, 3);
            sogBox2.Size = new Size(846, 723);
            sogBox2.TabIndex = 7;
            sogBox2.TabStop = false;
            sogBox2.Text = "სინქრონიზაციის პარამეტრები";
            // 
            // sogBox1
            // 
            sogBox1.Controls.Add(btnSaveDocPath);
            sogBox1.Controls.Add(label3);
            sogBox1.Controls.Add(txtDownloadFolder);
            sogBox1.Controls.Add(txtBaseUrl);
            sogBox1.Controls.Add(dgvGroups);
            sogBox1.Controls.Add(btnChooseDir);
            sogBox1.Controls.Add(label2);
            sogBox1.Controls.Add(btnSave);
            sogBox1.Controls.Add(label1);
            sogBox1.Location = new Point(7, 7);
            sogBox1.Margin = new Padding(4, 3, 4, 3);
            sogBox1.Name = "sogBox1";
            sogBox1.Padding = new Padding(4, 3, 4, 3);
            sogBox1.Size = new Size(891, 723);
            sogBox1.TabIndex = 6;
            sogBox1.TabStop = false;
            sogBox1.Text = "groupBox3";
            // 
            // btnSaveDocPath
            // 
            btnSaveDocPath.Location = new Point(786, 432);
            btnSaveDocPath.Margin = new Padding(4, 3, 4, 3);
            btnSaveDocPath.Name = "btnSaveDocPath";
            btnSaveDocPath.Size = new Size(88, 27);
            btnSaveDocPath.TabIndex = 4;
            btnSaveDocPath.Text = "შენახვა";
            btnSaveDocPath.UseVisualStyleBackColor = true;
            btnSaveDocPath.Click += btnSaveDocPath_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(51, 228);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(483, 15);
            label3.TabIndex = 5;
            label3.Text = "ამ ველში უნდა აირჩიოთ ჯგუფისთვის შესაბამისი ხელშეკრულების ფაილები";
            // 
            // txtDownloadFolder
            // 
            txtDownloadFolder.Location = new Point(167, 67);
            txtDownloadFolder.Margin = new Padding(4, 3, 4, 3);
            txtDownloadFolder.Name = "txtDownloadFolder";
            txtDownloadFolder.Size = new Size(238, 23);
            txtDownloadFolder.TabIndex = 0;
            // 
            // txtBaseUrl
            // 
            txtBaseUrl.Location = new Point(167, 141);
            txtBaseUrl.Margin = new Padding(4, 3, 4, 3);
            txtBaseUrl.Name = "txtBaseUrl";
            txtBaseUrl.Size = new Size(238, 23);
            txtBaseUrl.TabIndex = 0;
            // 
            // dgvGroups
            // 
            dgvGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGroups.Location = new Point(42, 252);
            dgvGroups.Margin = new Padding(4, 3, 4, 3);
            dgvGroups.Name = "dgvGroups";
            dgvGroups.Size = new Size(832, 173);
            dgvGroups.TabIndex = 3;
            dgvGroups.CellContentClick += dgvGroups_CellContentClick;
            // 
            // btnChooseDir
            // 
            btnChooseDir.Location = new Point(413, 67);
            btnChooseDir.Margin = new Padding(4, 3, 4, 3);
            btnChooseDir.Name = "btnChooseDir";
            btnChooseDir.Size = new Size(88, 27);
            btnChooseDir.TabIndex = 1;
            btnChooseDir.Text = "არჩევა";
            btnChooseDir.UseVisualStyleBackColor = true;
            btnChooseDir.Click += btnChooseDir_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(24, 144);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(125, 15);
            label2.TabIndex = 2;
            label2.Text = "ფაილები სერვერზე";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(167, 171);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 1;
            btnSave.Text = "შენახვა";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 70);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(115, 15);
            label1.TabIndex = 2;
            label1.Text = "შენახვის ადგილი";
            // 
            // SMSServiceSettings
            // 
            SMSServiceSettings.Controls.Add(chkSmsEnabled);
            SMSServiceSettings.Controls.Add(btnSaveOverSmsTexts);
            SMSServiceSettings.Controls.Add(btnSaveUpcPaySmsTexts);
            SMSServiceSettings.Controls.Add(btnSavePaySmsTexts);
            SMSServiceSettings.Controls.Add(btnSaveRegSmsTexts);
            SMSServiceSettings.Controls.Add(label9);
            SMSServiceSettings.Controls.Add(label8);
            SMSServiceSettings.Controls.Add(label7);
            SMSServiceSettings.Controls.Add(label6);
            SMSServiceSettings.Controls.Add(txtSmsOverdue);
            SMSServiceSettings.Controls.Add(txtSmsPayment);
            SMSServiceSettings.Controls.Add(txtSmsUpcoming);
            SMSServiceSettings.Controls.Add(txtSmsRegistration);
            SMSServiceSettings.Controls.Add(label5);
            SMSServiceSettings.Controls.Add(label4);
            SMSServiceSettings.Controls.Add(tbTestNumber);
            SMSServiceSettings.Controls.Add(btnTest);
            SMSServiceSettings.Controls.Add(btnSaveApiKey);
            SMSServiceSettings.Controls.Add(txtSmsApiKey);
            SMSServiceSettings.Location = new Point(4, 24);
            SMSServiceSettings.Margin = new Padding(4, 3, 4, 3);
            SMSServiceSettings.Name = "SMSServiceSettings";
            SMSServiceSettings.Size = new Size(1760, 1000);
            SMSServiceSettings.TabIndex = 4;
            SMSServiceSettings.Text = "SMS შეტყობინების ფუნქციები";
            SMSServiceSettings.UseVisualStyleBackColor = true;
            // 
            // chkSmsEnabled
            // 
            chkSmsEnabled.AutoSize = true;
            chkSmsEnabled.Location = new Point(58, 63);
            chkSmsEnabled.Margin = new Padding(4, 3, 4, 3);
            chkSmsEnabled.Name = "chkSmsEnabled";
            chkSmsEnabled.Size = new Size(82, 19);
            chkSmsEnabled.TabIndex = 22;
            chkSmsEnabled.Text = "checkBox1";
            chkSmsEnabled.UseVisualStyleBackColor = true;
            chkSmsEnabled.CheckedChanged += chkSmsEnabled_CheckedChanged;
            // 
            // btnSaveOverSmsTexts
            // 
            btnSaveOverSmsTexts.Location = new Point(1275, 427);
            btnSaveOverSmsTexts.Margin = new Padding(4, 3, 4, 3);
            btnSaveOverSmsTexts.Name = "btnSaveOverSmsTexts";
            btnSaveOverSmsTexts.Size = new Size(88, 27);
            btnSaveOverSmsTexts.TabIndex = 21;
            btnSaveOverSmsTexts.Text = "შენახვა";
            btnSaveOverSmsTexts.UseVisualStyleBackColor = true;
            btnSaveOverSmsTexts.Click += btnSaveOverSmsTexts_Click;
            // 
            // btnSaveUpcPaySmsTexts
            // 
            btnSaveUpcPaySmsTexts.Location = new Point(1275, 290);
            btnSaveUpcPaySmsTexts.Margin = new Padding(4, 3, 4, 3);
            btnSaveUpcPaySmsTexts.Name = "btnSaveUpcPaySmsTexts";
            btnSaveUpcPaySmsTexts.Size = new Size(88, 27);
            btnSaveUpcPaySmsTexts.TabIndex = 21;
            btnSaveUpcPaySmsTexts.Text = "შენახვა";
            btnSaveUpcPaySmsTexts.UseVisualStyleBackColor = true;
            btnSaveUpcPaySmsTexts.Click += btnSaveUpcPaySmsTexts_Click;
            // 
            // btnSavePaySmsTexts
            // 
            btnSavePaySmsTexts.Location = new Point(547, 586);
            btnSavePaySmsTexts.Margin = new Padding(4, 3, 4, 3);
            btnSavePaySmsTexts.Name = "btnSavePaySmsTexts";
            btnSavePaySmsTexts.Size = new Size(88, 27);
            btnSavePaySmsTexts.TabIndex = 21;
            btnSavePaySmsTexts.Text = "შენახვა";
            btnSavePaySmsTexts.UseVisualStyleBackColor = true;
            btnSavePaySmsTexts.Click += btnSavePaySmsTexts_Click;
            // 
            // btnSaveRegSmsTexts
            // 
            btnSaveRegSmsTexts.Location = new Point(547, 449);
            btnSaveRegSmsTexts.Margin = new Padding(4, 3, 4, 3);
            btnSaveRegSmsTexts.Name = "btnSaveRegSmsTexts";
            btnSaveRegSmsTexts.Size = new Size(88, 27);
            btnSaveRegSmsTexts.TabIndex = 21;
            btnSaveRegSmsTexts.Text = "შენახვა";
            btnSaveRegSmsTexts.UseVisualStyleBackColor = true;
            btnSaveRegSmsTexts.Click += btnSaveRegSmsTexts_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(764, 337);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(237, 15);
            label9.TabIndex = 20;
            label9.Text = "გადახდის გადაცილების შეტყობინება";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(764, 198);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(253, 15);
            label8.TabIndex = 19;
            label8.Text = "მოახლოვებული გადახდის შეტყობინება";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(38, 496);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(150, 15);
            label7.TabIndex = 18;
            label7.Text = "გადახდის შეტყობინება";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(38, 355);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(176, 15);
            label6.TabIndex = 17;
            label6.Text = "რეგისტრაციის შეტყობინება";
            // 
            // txtSmsOverdue
            // 
            txtSmsOverdue.Location = new Point(768, 355);
            txtSmsOverdue.Margin = new Padding(4, 3, 4, 3);
            txtSmsOverdue.Multiline = true;
            txtSmsOverdue.Name = "txtSmsOverdue";
            txtSmsOverdue.Size = new Size(500, 97);
            txtSmsOverdue.TabIndex = 16;
            // 
            // txtSmsPayment
            // 
            txtSmsPayment.Location = new Point(40, 515);
            txtSmsPayment.Margin = new Padding(4, 3, 4, 3);
            txtSmsPayment.Multiline = true;
            txtSmsPayment.Name = "txtSmsPayment";
            txtSmsPayment.Size = new Size(500, 97);
            txtSmsPayment.TabIndex = 16;
            // 
            // txtSmsUpcoming
            // 
            txtSmsUpcoming.Location = new Point(768, 218);
            txtSmsUpcoming.Margin = new Padding(4, 3, 4, 3);
            txtSmsUpcoming.Multiline = true;
            txtSmsUpcoming.Name = "txtSmsUpcoming";
            txtSmsUpcoming.Size = new Size(500, 97);
            txtSmsUpcoming.TabIndex = 16;
            // 
            // txtSmsRegistration
            // 
            txtSmsRegistration.Location = new Point(40, 377);
            txtSmsRegistration.Margin = new Padding(4, 3, 4, 3);
            txtSmsRegistration.Multiline = true;
            txtSmsRegistration.Name = "txtSmsRegistration";
            txtSmsRegistration.Size = new Size(500, 97);
            txtSmsRegistration.TabIndex = 16;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(38, 268);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(186, 15);
            label5.TabIndex = 15;
            label5.Text = "სატესტო ტელეფონის ნომერი";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(38, 182);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(52, 15);
            label4.TabIndex = 14;
            label4.Text = "SMS Key";
            // 
            // tbTestNumber
            // 
            tbTestNumber.Location = new Point(42, 286);
            tbTestNumber.Margin = new Padding(4, 3, 4, 3);
            tbTestNumber.Multiline = true;
            tbTestNumber.Name = "tbTestNumber";
            tbTestNumber.Size = new Size(167, 29);
            tbTestNumber.TabIndex = 13;
            // 
            // btnTest
            // 
            btnTest.Location = new Point(217, 290);
            btnTest.Margin = new Padding(4, 3, 4, 3);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(88, 27);
            btnTest.TabIndex = 12;
            btnTest.Text = "შემოწმება";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnSaveApiKey
            // 
            btnSaveApiKey.Location = new Point(572, 198);
            btnSaveApiKey.Margin = new Padding(4, 3, 4, 3);
            btnSaveApiKey.Name = "btnSaveApiKey";
            btnSaveApiKey.Size = new Size(88, 27);
            btnSaveApiKey.TabIndex = 11;
            btnSaveApiKey.Text = "შენახვა";
            btnSaveApiKey.UseVisualStyleBackColor = true;
            btnSaveApiKey.Click += btnSaveApiKey_Click_1;
            // 
            // txtSmsApiKey
            // 
            txtSmsApiKey.Location = new Point(42, 201);
            txtSmsApiKey.Margin = new Padding(4, 3, 4, 3);
            txtSmsApiKey.Name = "txtSmsApiKey";
            txtSmsApiKey.Size = new Size(497, 23);
            txtSmsApiKey.TabIndex = 10;
            // 
            // AutoFileDetectionSettings
            // 
            AutoFileDetectionSettings.Controls.Add(groupBoxAutoDetection);
            AutoFileDetectionSettings.Location = new Point(4, 24);
            AutoFileDetectionSettings.Margin = new Padding(4, 3, 4, 3);
            AutoFileDetectionSettings.Name = "AutoFileDetectionSettings";
            AutoFileDetectionSettings.Size = new Size(1760, 1000);
            AutoFileDetectionSettings.TabIndex = 5;
            AutoFileDetectionSettings.Text = "ავტომატური ფაილის აღმოჩენა";
            AutoFileDetectionSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxAutoDetection
            // 
            groupBoxAutoDetection.Controls.Add(btnSaveAutoDetectionSettings);
            groupBoxAutoDetection.Controls.Add(btnSelectDirectory);
            groupBoxAutoDetection.Controls.Add(txtFileNamePattern);
            groupBoxAutoDetection.Controls.Add(txtWatchDirectory);
            groupBoxAutoDetection.Controls.Add(labelFileNamePattern);
            groupBoxAutoDetection.Controls.Add(labelWatchDirectory);
            groupBoxAutoDetection.Controls.Add(chkAutoDetectionEnabled);
            groupBoxAutoDetection.Location = new Point(23, 23);
            groupBoxAutoDetection.Margin = new Padding(4, 3, 4, 3);
            groupBoxAutoDetection.Name = "groupBoxAutoDetection";
            groupBoxAutoDetection.Padding = new Padding(4, 3, 4, 3);
            groupBoxAutoDetection.Size = new Size(700, 231);
            groupBoxAutoDetection.TabIndex = 0;
            groupBoxAutoDetection.TabStop = false;
            groupBoxAutoDetection.Text = "ავტომატური ფაილის აღმოჩენის პარამეტრები";
            // 
            // btnSaveAutoDetectionSettings
            // 
            btnSaveAutoDetectionSettings.Location = new Point(23, 173);
            btnSaveAutoDetectionSettings.Margin = new Padding(4, 3, 4, 3);
            btnSaveAutoDetectionSettings.Name = "btnSaveAutoDetectionSettings";
            btnSaveAutoDetectionSettings.Size = new Size(117, 35);
            btnSaveAutoDetectionSettings.TabIndex = 6;
            btnSaveAutoDetectionSettings.Text = "შენახვა";
            btnSaveAutoDetectionSettings.UseVisualStyleBackColor = true;
            btnSaveAutoDetectionSettings.Click += btnSaveAutoDetectionSettings_Click;
            // 
            // btnSelectDirectory
            // 
            btnSelectDirectory.Location = new Point(595, 75);
            btnSelectDirectory.Margin = new Padding(4, 3, 4, 3);
            btnSelectDirectory.Name = "btnSelectDirectory";
            btnSelectDirectory.Size = new Size(88, 27);
            btnSelectDirectory.TabIndex = 3;
            btnSelectDirectory.Text = "არჩევა";
            btnSelectDirectory.UseVisualStyleBackColor = true;
            btnSelectDirectory.Click += btnSelectDirectory_Click;
            // 
            // txtFileNamePattern
            // 
            txtFileNamePattern.Location = new Point(175, 123);
            txtFileNamePattern.Margin = new Padding(4, 3, 4, 3);
            txtFileNamePattern.Name = "txtFileNamePattern";
            txtFileNamePattern.Size = new Size(408, 23);
            txtFileNamePattern.TabIndex = 5;
            txtFileNamePattern.Text = "*.xlsx";
            // 
            // txtWatchDirectory
            // 
            txtWatchDirectory.Location = new Point(175, 77);
            txtWatchDirectory.Margin = new Padding(4, 3, 4, 3);
            txtWatchDirectory.Name = "txtWatchDirectory";
            txtWatchDirectory.Size = new Size(408, 23);
            txtWatchDirectory.TabIndex = 2;
            // 
            // labelFileNamePattern
            // 
            labelFileNamePattern.AutoSize = true;
            labelFileNamePattern.Location = new Point(23, 127);
            labelFileNamePattern.Margin = new Padding(4, 0, 4, 0);
            labelFileNamePattern.Name = "labelFileNamePattern";
            labelFileNamePattern.Size = new Size(166, 15);
            labelFileNamePattern.TabIndex = 4;
            labelFileNamePattern.Text = "ფაილის სახელის ნიმუში:";
            // 
            // labelWatchDirectory
            // 
            labelWatchDirectory.AutoSize = true;
            labelWatchDirectory.Location = new Point(23, 81);
            labelWatchDirectory.Margin = new Padding(4, 0, 4, 0);
            labelWatchDirectory.Name = "labelWatchDirectory";
            labelWatchDirectory.Size = new Size(175, 15);
            labelWatchDirectory.TabIndex = 1;
            labelWatchDirectory.Text = "მონიტორინგის საქაღალდე:";
            // 
            // chkAutoDetectionEnabled
            // 
            chkAutoDetectionEnabled.AutoSize = true;
            chkAutoDetectionEnabled.Location = new Point(23, 35);
            chkAutoDetectionEnabled.Margin = new Padding(4, 3, 4, 3);
            chkAutoDetectionEnabled.Name = "chkAutoDetectionEnabled";
            chkAutoDetectionEnabled.Size = new Size(290, 19);
            chkAutoDetectionEnabled.TabIndex = 0;
            chkAutoDetectionEnabled.Text = "ავტომატური ფაილის აღმოჩენა ჩართულია";
            chkAutoDetectionEnabled.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(1795, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // AdminPanelForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1795, 1061);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "AdminPanelForm";
            StartPosition = FormStartPosition.CenterScreen;
            Load += AdminPanelForm_Load;
            tabControl1.ResumeLayout(false);
            DatabaseSettings.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dGVUnassignedStudents).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            UserManagement.ResumeLayout(false);
            UserManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRegisteredUsers).EndInit();
            PaymentsFinance.ResumeLayout(false);
            SystemOperations.ResumeLayout(false);
            sogBox1.ResumeLayout(false);
            sogBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGroups).EndInit();
            SMSServiceSettings.ResumeLayout(false);
            SMSServiceSettings.PerformLayout();
            AutoFileDetectionSettings.ResumeLayout(false);
            groupBoxAutoDetection.ResumeLayout(false);
            groupBoxAutoDetection.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage DatabaseSettings;
        private System.Windows.Forms.TabPage UserManagement;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TabPage PaymentsFinance;
        private System.Windows.Forms.TabPage SystemOperations;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnCheckStudents;
        private System.Windows.Forms.DataGridView dGVUnassignedStudents;
        private System.Windows.Forms.Button btnResetData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnChooseDir;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.TextBox txtDownloadFolder;
        private System.Windows.Forms.Button btnSetStudyStartDate;
        private System.Windows.Forms.DataGridView dgvGroups;
        private System.Windows.Forms.Button btnSaveDocPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage SMSServiceSettings;
        private System.Windows.Forms.TabPage AutoFileDetectionSettings;
        private System.Windows.Forms.TextBox tbTestNumber;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnSaveApiKey;
        private System.Windows.Forms.TextBox txtSmsApiKey;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSmsOverdue;
        private System.Windows.Forms.TextBox txtSmsPayment;
        private System.Windows.Forms.TextBox txtSmsUpcoming;
        private System.Windows.Forms.TextBox txtSmsRegistration;
        private System.Windows.Forms.Button btnSaveOverSmsTexts;
        private System.Windows.Forms.Button btnSaveUpcPaySmsTexts;
        private System.Windows.Forms.Button btnSavePaySmsTexts;
        private System.Windows.Forms.Button btnSaveRegSmsTexts;
        private System.Windows.Forms.CheckBox chkSmsEnabled;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox localUsrPass;
        private System.Windows.Forms.TextBox localUsrName;
        private System.Windows.Forms.TextBox localDbName;
        private System.Windows.Forms.TextBox LocalPort;
        private System.Windows.Forms.TextBox localHost;
        private System.Windows.Forms.Button testLocalConn;
        private System.Windows.Forms.Label localConnStatus;
        private System.Windows.Forms.Button saveLocalConn;
        private System.Windows.Forms.Button btnAddStudentToGroup;
        private System.Windows.Forms.Button btnSwitchToTestDB;
        private System.Windows.Forms.Label lblDbMode;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRegisterUser;
        private System.Windows.Forms.DataGridView dgvRegisteredUsers;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Label lblusersInfo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox sogBox2;
        private System.Windows.Forms.GroupBox sogBox1;
        private System.Windows.Forms.GroupBox groupBoxAutoDetection;
        private System.Windows.Forms.CheckBox chkAutoDetectionEnabled;
        private System.Windows.Forms.Label labelWatchDirectory;
        private System.Windows.Forms.TextBox txtWatchDirectory;
        private System.Windows.Forms.Button btnSelectDirectory;
        private System.Windows.Forms.Label labelFileNamePattern;
        private System.Windows.Forms.TextBox txtFileNamePattern;
        private System.Windows.Forms.Button btnSaveAutoDetectionSettings;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private TextBox serverHost;
        private Label label21;
        //private Label label15;
        private TextBox serverPort;
        //private Button button1;
        private Label label20;
        private Button saveServerConn;
        private TextBox serverDbName;
        //private Label label16;
        private Label label19;
        private Button testSrvConn;
        private TextBox serverUsrName;
        private Label label17;
        private Label label18;
        private TextBox serverUsrPass;
        private Label serverConnStatus;
        private Button btnSetPaymentDate;
    }
}
