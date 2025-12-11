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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.DatabaseSettings = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAddStudentToGroup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnCheckStudents = new System.Windows.Forms.Button();
            this.dGVUnassignedStudents = new System.Windows.Forms.DataGridView();
            this.btnResetData = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblDbMode = new System.Windows.Forms.Label();
            this.btnSwitchToTestDB = new System.Windows.Forms.Button();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnSaveConnection = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.UserManagement = new System.Windows.Forms.TabPage();
            this.lblusersInfo = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.dgvRegisteredUsers = new System.Windows.Forms.DataGridView();
            this.btnRegisterUser = new System.Windows.Forms.Button();
            this.PaymentsFinance = new System.Windows.Forms.TabPage();
            this.btnSetStudyStartDate = new System.Windows.Forms.Button();
            this.SystemOperations = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSaveDocPath = new System.Windows.Forms.Button();
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnChooseDir = new System.Windows.Forms.Button();
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.txtDownloadFolder = new System.Windows.Forms.TextBox();
            this.SMSServiceSettings = new System.Windows.Forms.TabPage();
            this.chkSmsEnabled = new System.Windows.Forms.CheckBox();
            this.btnSaveOverSmsTexts = new System.Windows.Forms.Button();
            this.btnSaveUpcPaySmsTexts = new System.Windows.Forms.Button();
            this.btnSavePaySmsTexts = new System.Windows.Forms.Button();
            this.btnSaveRegSmsTexts = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSmsOverdue = new System.Windows.Forms.TextBox();
            this.txtSmsPayment = new System.Windows.Forms.TextBox();
            this.txtSmsUpcoming = new System.Windows.Forms.TextBox();
            this.txtSmsRegistration = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTestNumber = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnSaveApiKey = new System.Windows.Forms.Button();
            this.txtSmsApiKey = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sogBox1 = new System.Windows.Forms.GroupBox();
            this.sogBox2 = new System.Windows.Forms.GroupBox();
            this.AutoFileDetectionSettings = new System.Windows.Forms.TabPage();
            this.groupBoxAutoDetection = new System.Windows.Forms.GroupBox();
            this.chkAutoDetectionEnabled = new System.Windows.Forms.CheckBox();
            this.labelWatchDirectory = new System.Windows.Forms.Label();
            this.txtWatchDirectory = new System.Windows.Forms.TextBox();
            this.btnSelectDirectory = new System.Windows.Forms.Button();
            this.labelFileNamePattern = new System.Windows.Forms.Label();
            this.txtFileNamePattern = new System.Windows.Forms.TextBox();
            this.btnSaveAutoDetectionSettings = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.DatabaseSettings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGVUnassignedStudents)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.UserManagement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegisteredUsers)).BeginInit();
            this.PaymentsFinance.SuspendLayout();
            this.SystemOperations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            this.SMSServiceSettings.SuspendLayout();
            this.AutoFileDetectionSettings.SuspendLayout();
            this.groupBoxAutoDetection.SuspendLayout();
            this.sogBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.DatabaseSettings);
            this.tabControl1.Controls.Add(this.UserManagement);
            this.tabControl1.Controls.Add(this.PaymentsFinance);
            this.tabControl1.Controls.Add(this.SystemOperations);
            this.tabControl1.Controls.Add(this.SMSServiceSettings);
            this.tabControl1.Controls.Add(this.AutoFileDetectionSettings);
            this.tabControl1.Location = new System.Drawing.Point(12, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1515, 891);
            this.tabControl1.TabIndex = 0;
            // 
            // DatabaseSettings
            // 
            this.DatabaseSettings.Controls.Add(this.groupBox2);
            this.DatabaseSettings.Controls.Add(this.groupBox1);
            this.DatabaseSettings.Location = new System.Drawing.Point(4, 22);
            this.DatabaseSettings.Name = "DatabaseSettings";
            this.DatabaseSettings.Padding = new System.Windows.Forms.Padding(3);
            this.DatabaseSettings.Size = new System.Drawing.Size(1507, 865);
            this.DatabaseSettings.TabIndex = 0;
            this.DatabaseSettings.Text = "Database Settings";
            this.DatabaseSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnAddStudentToGroup);
            this.groupBox2.Controls.Add(this.btnRestore);
            this.groupBox2.Controls.Add(this.btnSync);
            this.groupBox2.Controls.Add(this.btnBackup);
            this.groupBox2.Controls.Add(this.btnCheckStudents);
            this.groupBox2.Controls.Add(this.dGVUnassignedStudents);
            this.groupBox2.Controls.Add(this.btnResetData);
            this.groupBox2.Location = new System.Drawing.Point(810, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(691, 811);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // btnAddStudentToGroup
            // 
            this.btnAddStudentToGroup.Location = new System.Drawing.Point(156, 40);
            this.btnAddStudentToGroup.Name = "btnAddStudentToGroup";
            this.btnAddStudentToGroup.Size = new System.Drawing.Size(212, 63);
            this.btnAddStudentToGroup.TabIndex = 5;
            this.btnAddStudentToGroup.Text = "მოსწავლეების ჯგუფებთან და ქვეჯგუფებთან კავშირის შემოწმება";
            this.btnAddStudentToGroup.UseVisualStyleBackColor = true;
            this.btnAddStudentToGroup.Click += new System.EventHandler(this.btnAddStudentToGroup_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(156, 173);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(151, 39);
            this.btnRestore.TabIndex = 6;
            this.btnRestore.Text = "სარეზერვი კოპირების აღდგენა";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(374, 40);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(135, 55);
            this.btnSync.TabIndex = 0;
            this.btnSync.Text = "მონაცემების გადატანა";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(156, 117);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(151, 39);
            this.btnBackup.TabIndex = 6;
            this.btnBackup.Text = "სარეზერვი კოპირება";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnCheckStudents
            // 
            this.btnCheckStudents.Location = new System.Drawing.Point(374, 101);
            this.btnCheckStudents.Name = "btnCheckStudents";
            this.btnCheckStudents.Size = new System.Drawing.Size(135, 55);
            this.btnCheckStudents.TabIndex = 1;
            this.btnCheckStudents.Text = "მოსწავლეების ჯგუფთან კავშირის შემოწმება";
            this.btnCheckStudents.UseVisualStyleBackColor = true;
            this.btnCheckStudents.Click += new System.EventHandler(this.btnCheckStudents_Click);
            // 
            // dGVUnassignedStudents
            // 
            this.dGVUnassignedStudents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVUnassignedStudents.Location = new System.Drawing.Point(67, 278);
            this.dGVUnassignedStudents.Name = "dGVUnassignedStudents";
            this.dGVUnassignedStudents.Size = new System.Drawing.Size(551, 283);
            this.dGVUnassignedStudents.TabIndex = 2;
            // 
            // btnResetData
            // 
            this.btnResetData.Location = new System.Drawing.Point(374, 162);
            this.btnResetData.Name = "btnResetData";
            this.btnResetData.Size = new System.Drawing.Size(135, 50);
            this.btnResetData.TabIndex = 3;
            this.btnResetData.Text = "ყველა ჩანაწერის გასუფთავება";
            this.btnResetData.UseVisualStyleBackColor = true;
            this.btnResetData.Click += new System.EventHandler(this.btnResetData_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lblDbMode);
            this.groupBox1.Controls.Add(this.btnSwitchToTestDB);
            this.groupBox1.Controls.Add(this.lblConnectionStatus);
            this.groupBox1.Controls.Add(this.btnSaveConnection);
            this.groupBox1.Controls.Add(this.btnTestConnection);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.txtDatabase);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Location = new System.Drawing.Point(35, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(769, 811);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ბაზასთან კავშირი";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(34, 169);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(89, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "მომხმარებელი";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(71, 206);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "პაროლი";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(39, 133);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 13);
            this.label12.TabIndex = 6;
            this.label12.Text = "ბაზის სახელი";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(81, 98);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "პორტი";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(94, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Host";
            // 
            // lblDbMode
            // 
            this.lblDbMode.AutoSize = true;
            this.lblDbMode.Location = new System.Drawing.Point(407, 90);
            this.lblDbMode.Name = "lblDbMode";
            this.lblDbMode.Size = new System.Drawing.Size(41, 13);
            this.lblDbMode.TabIndex = 5;
            this.lblDbMode.Text = "label10";
            // 
            // btnSwitchToTestDB
            // 
            this.btnSwitchToTestDB.Location = new System.Drawing.Point(401, 56);
            this.btnSwitchToTestDB.Name = "btnSwitchToTestDB";
            this.btnSwitchToTestDB.Size = new System.Drawing.Size(108, 23);
            this.btnSwitchToTestDB.TabIndex = 4;
            this.btnSwitchToTestDB.Text = "გადართვა";
            this.btnSwitchToTestDB.UseVisualStyleBackColor = true;
            this.btnSwitchToTestDB.Click += new System.EventHandler(this.btnSwitchToTestDB_Click);
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(484, 211);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(55, 13);
            this.lblConnectionStatus.TabIndex = 3;
            this.lblConnectionStatus.Text = "სტატუსი";
            // 
            // btnSaveConnection
            // 
            this.btnSaveConnection.Location = new System.Drawing.Point(220, 249);
            this.btnSaveConnection.Name = "btnSaveConnection";
            this.btnSaveConnection.Size = new System.Drawing.Size(108, 30);
            this.btnSaveConnection.TabIndex = 2;
            this.btnSaveConnection.Text = "შენახვა";
            this.btnSaveConnection.UseVisualStyleBackColor = true;
            this.btnSaveConnection.Click += new System.EventHandler(this.btnSaveConnection_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(352, 206);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(126, 23);
            this.btnTestConnection.TabIndex = 1;
            this.btnTestConnection.Text = "კავშირის შემოწმება";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(129, 198);
            this.txtPassword.Multiline = true;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(199, 30);
            this.txtPassword.TabIndex = 0;
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(129, 162);
            this.txtUsername.Multiline = true;
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(199, 30);
            this.txtUsername.TabIndex = 0;
            // 
            // txtDatabase
            // 
            this.txtDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDatabase.Location = new System.Drawing.Point(129, 126);
            this.txtDatabase.Multiline = true;
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(199, 30);
            this.txtDatabase.TabIndex = 0;
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(129, 90);
            this.txtPort.Multiline = true;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(199, 30);
            this.txtPort.TabIndex = 0;
            // 
            // txtServer
            // 
            this.txtServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServer.Location = new System.Drawing.Point(129, 54);
            this.txtServer.Multiline = true;
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(199, 30);
            this.txtServer.TabIndex = 0;
            // 
            // UserManagement
            // 
            this.UserManagement.Controls.Add(this.lblusersInfo);
            this.UserManagement.Controls.Add(this.toolStrip1);
            this.UserManagement.Controls.Add(this.dgvRegisteredUsers);
            this.UserManagement.Controls.Add(this.btnRegisterUser);
            this.UserManagement.Location = new System.Drawing.Point(4, 22);
            this.UserManagement.Name = "UserManagement";
            this.UserManagement.Padding = new System.Windows.Forms.Padding(3);
            this.UserManagement.Size = new System.Drawing.Size(1507, 865);
            this.UserManagement.TabIndex = 1;
            this.UserManagement.Text = "User Management";
            this.UserManagement.UseVisualStyleBackColor = true;
            // 
            // lblusersInfo
            // 
            this.lblusersInfo.AutoSize = true;
            this.lblusersInfo.Location = new System.Drawing.Point(17, 306);
            this.lblusersInfo.Name = "lblusersInfo";
            this.lblusersInfo.Size = new System.Drawing.Size(37, 13);
            this.lblusersInfo.TabIndex = 3;
            this.lblusersInfo.Text = "ინფო";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1501, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // dgvRegisteredUsers
            // 
            this.dgvRegisteredUsers.AllowUserToAddRows = false;
            this.dgvRegisteredUsers.AllowUserToDeleteRows = false;
            this.dgvRegisteredUsers.AllowUserToOrderColumns = true;
            this.dgvRegisteredUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvRegisteredUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRegisteredUsers.Location = new System.Drawing.Point(20, 72);
            this.dgvRegisteredUsers.Name = "dgvRegisteredUsers";
            this.dgvRegisteredUsers.ReadOnly = true;
            this.dgvRegisteredUsers.Size = new System.Drawing.Size(954, 201);
            this.dgvRegisteredUsers.TabIndex = 1;
            // 
            // btnRegisterUser
            // 
            this.btnRegisterUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegisterUser.Location = new System.Drawing.Point(20, 31);
            this.btnRegisterUser.Name = "btnRegisterUser";
            this.btnRegisterUser.Size = new System.Drawing.Size(89, 35);
            this.btnRegisterUser.TabIndex = 0;
            this.btnRegisterUser.Text = "დამატება";
            this.btnRegisterUser.UseVisualStyleBackColor = true;
            this.btnRegisterUser.Click += new System.EventHandler(this.btnRegisterUser_Click);
            // 
            // PaymentsFinance
            // 
            this.PaymentsFinance.Controls.Add(this.btnSetStudyStartDate);
            this.PaymentsFinance.Location = new System.Drawing.Point(4, 22);
            this.PaymentsFinance.Name = "PaymentsFinance";
            this.PaymentsFinance.Padding = new System.Windows.Forms.Padding(3);
            this.PaymentsFinance.Size = new System.Drawing.Size(1507, 865);
            this.PaymentsFinance.TabIndex = 2;
            this.PaymentsFinance.Text = "Payments & Finance";
            this.PaymentsFinance.UseVisualStyleBackColor = true;
            // 
            // btnSetStudyStartDate
            // 
            this.btnSetStudyStartDate.Location = new System.Drawing.Point(26, 26);
            this.btnSetStudyStartDate.Name = "btnSetStudyStartDate";
            this.btnSetStudyStartDate.Size = new System.Drawing.Size(265, 30);
            this.btnSetStudyStartDate.TabIndex = 0;
            this.btnSetStudyStartDate.Text = "სწავლის დაწყების თარიღის დაყენება";
            this.btnSetStudyStartDate.UseVisualStyleBackColor = true;
            this.btnSetStudyStartDate.Click += new System.EventHandler(this.btnSetStudyStartDate_Click);
            // 
            // SystemOperations
            // 
            this.SystemOperations.Controls.Add(this.sogBox2);
            this.SystemOperations.Controls.Add(this.sogBox1);
            this.SystemOperations.Location = new System.Drawing.Point(4, 22);
            this.SystemOperations.Name = "SystemOperations";
            this.SystemOperations.Padding = new System.Windows.Forms.Padding(3);
            this.SystemOperations.Size = new System.Drawing.Size(1507, 865);
            this.SystemOperations.TabIndex = 3;
            this.SystemOperations.Text = "System Operations";
            this.SystemOperations.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(432, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "ამ ველში უნდა აირჩიოთ ჯგუფისთვის შესაბამისი ხელშეკრულების ფაილები";
            // 
            // btnSaveDocPath
            // 
            this.btnSaveDocPath.Location = new System.Drawing.Point(674, 374);
            this.btnSaveDocPath.Name = "btnSaveDocPath";
            this.btnSaveDocPath.Size = new System.Drawing.Size(75, 23);
            this.btnSaveDocPath.TabIndex = 4;
            this.btnSaveDocPath.Text = "შენახვა";
            this.btnSaveDocPath.UseVisualStyleBackColor = true;
            this.btnSaveDocPath.Click += new System.EventHandler(this.btnSaveDocPath_Click);
            // 
            // dgvGroups
            // 
            this.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroups.Location = new System.Drawing.Point(36, 218);
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.Size = new System.Drawing.Size(713, 150);
            this.dgvGroups.TabIndex = 3;
            this.dgvGroups.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGroups_CellContentClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "ფაილები სერვერზე";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "შენახვის ადგილი";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(143, 148);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "შენახვა";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnChooseDir
            // 
            this.btnChooseDir.Location = new System.Drawing.Point(354, 58);
            this.btnChooseDir.Name = "btnChooseDir";
            this.btnChooseDir.Size = new System.Drawing.Size(75, 23);
            this.btnChooseDir.TabIndex = 1;
            this.btnChooseDir.Text = "არჩევა";
            this.btnChooseDir.UseVisualStyleBackColor = true;
            this.btnChooseDir.Click += new System.EventHandler(this.btnChooseDir_Click);
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(143, 122);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(205, 20);
            this.txtBaseUrl.TabIndex = 0;
            // 
            // txtDownloadFolder
            // 
            this.txtDownloadFolder.Location = new System.Drawing.Point(143, 58);
            this.txtDownloadFolder.Name = "txtDownloadFolder";
            this.txtDownloadFolder.Size = new System.Drawing.Size(205, 20);
            this.txtDownloadFolder.TabIndex = 0;
            // 
            // SMSServiceSettings
            // 
            this.SMSServiceSettings.Controls.Add(this.chkSmsEnabled);
            this.SMSServiceSettings.Controls.Add(this.btnSaveOverSmsTexts);
            this.SMSServiceSettings.Controls.Add(this.btnSaveUpcPaySmsTexts);
            this.SMSServiceSettings.Controls.Add(this.btnSavePaySmsTexts);
            this.SMSServiceSettings.Controls.Add(this.btnSaveRegSmsTexts);
            this.SMSServiceSettings.Controls.Add(this.label9);
            this.SMSServiceSettings.Controls.Add(this.label8);
            this.SMSServiceSettings.Controls.Add(this.label7);
            this.SMSServiceSettings.Controls.Add(this.label6);
            this.SMSServiceSettings.Controls.Add(this.txtSmsOverdue);
            this.SMSServiceSettings.Controls.Add(this.txtSmsPayment);
            this.SMSServiceSettings.Controls.Add(this.txtSmsUpcoming);
            this.SMSServiceSettings.Controls.Add(this.txtSmsRegistration);
            this.SMSServiceSettings.Controls.Add(this.label5);
            this.SMSServiceSettings.Controls.Add(this.label4);
            this.SMSServiceSettings.Controls.Add(this.tbTestNumber);
            this.SMSServiceSettings.Controls.Add(this.btnTest);
            this.SMSServiceSettings.Controls.Add(this.btnSaveApiKey);
            this.SMSServiceSettings.Controls.Add(this.txtSmsApiKey);
            this.SMSServiceSettings.Location = new System.Drawing.Point(4, 22);
            this.SMSServiceSettings.Name = "SMSServiceSettings";
            this.SMSServiceSettings.Size = new System.Drawing.Size(1507, 865);
            this.SMSServiceSettings.TabIndex = 4;
            this.SMSServiceSettings.Text = "SMS შეტყობინების ფუნქციები";
            this.SMSServiceSettings.UseVisualStyleBackColor = true;
            // 
            // AutoFileDetectionSettings
            // 
            this.AutoFileDetectionSettings.Controls.Add(this.groupBoxAutoDetection);
            this.AutoFileDetectionSettings.Location = new System.Drawing.Point(4, 22);
            this.AutoFileDetectionSettings.Name = "AutoFileDetectionSettings";
            this.AutoFileDetectionSettings.Size = new System.Drawing.Size(1507, 865);
            this.AutoFileDetectionSettings.TabIndex = 5;
            this.AutoFileDetectionSettings.Text = "ავტომატური ფაილის აღმოჩენა";
            this.AutoFileDetectionSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxAutoDetection
            // 
            this.groupBoxAutoDetection.Controls.Add(this.btnSaveAutoDetectionSettings);
            this.groupBoxAutoDetection.Controls.Add(this.btnSelectDirectory);
            this.groupBoxAutoDetection.Controls.Add(this.txtFileNamePattern);
            this.groupBoxAutoDetection.Controls.Add(this.txtWatchDirectory);
            this.groupBoxAutoDetection.Controls.Add(this.labelFileNamePattern);
            this.groupBoxAutoDetection.Controls.Add(this.labelWatchDirectory);
            this.groupBoxAutoDetection.Controls.Add(this.chkAutoDetectionEnabled);
            this.groupBoxAutoDetection.Location = new System.Drawing.Point(20, 20);
            this.groupBoxAutoDetection.Name = "groupBoxAutoDetection";
            this.groupBoxAutoDetection.Size = new System.Drawing.Size(600, 200);
            this.groupBoxAutoDetection.TabIndex = 0;
            this.groupBoxAutoDetection.TabStop = false;
            this.groupBoxAutoDetection.Text = "ავტომატური ფაილის აღმოჩენის პარამეტრები";
            // 
            // chkAutoDetectionEnabled
            // 
            this.chkAutoDetectionEnabled.AutoSize = true;
            this.chkAutoDetectionEnabled.Location = new System.Drawing.Point(20, 30);
            this.chkAutoDetectionEnabled.Name = "chkAutoDetectionEnabled";
            this.chkAutoDetectionEnabled.Size = new System.Drawing.Size(200, 17);
            this.chkAutoDetectionEnabled.TabIndex = 0;
            this.chkAutoDetectionEnabled.Text = "ავტომატური ფაილის აღმოჩენა ჩართულია";
            this.chkAutoDetectionEnabled.UseVisualStyleBackColor = true;
            // 
            // labelWatchDirectory
            // 
            this.labelWatchDirectory.AutoSize = true;
            this.labelWatchDirectory.Location = new System.Drawing.Point(20, 70);
            this.labelWatchDirectory.Name = "labelWatchDirectory";
            this.labelWatchDirectory.Size = new System.Drawing.Size(120, 13);
            this.labelWatchDirectory.TabIndex = 1;
            this.labelWatchDirectory.Text = "მონიტორინგის საქაღალდე:";
            // 
            // txtWatchDirectory
            // 
            this.txtWatchDirectory.Location = new System.Drawing.Point(150, 67);
            this.txtWatchDirectory.Name = "txtWatchDirectory";
            this.txtWatchDirectory.Size = new System.Drawing.Size(350, 20);
            this.txtWatchDirectory.TabIndex = 2;
            // 
            // btnSelectDirectory
            // 
            this.btnSelectDirectory.Location = new System.Drawing.Point(510, 65);
            this.btnSelectDirectory.Name = "btnSelectDirectory";
            this.btnSelectDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDirectory.TabIndex = 3;
            this.btnSelectDirectory.Text = "არჩევა";
            this.btnSelectDirectory.UseVisualStyleBackColor = true;
            this.btnSelectDirectory.Click += new System.EventHandler(this.btnSelectDirectory_Click);
            // 
            // labelFileNamePattern
            // 
            this.labelFileNamePattern.AutoSize = true;
            this.labelFileNamePattern.Location = new System.Drawing.Point(20, 110);
            this.labelFileNamePattern.Name = "labelFileNamePattern";
            this.labelFileNamePattern.Size = new System.Drawing.Size(100, 13);
            this.labelFileNamePattern.TabIndex = 4;
            this.labelFileNamePattern.Text = "ფაილის სახელის ნიმუში:";
            // 
            // txtFileNamePattern
            // 
            this.txtFileNamePattern.Location = new System.Drawing.Point(150, 107);
            this.txtFileNamePattern.Name = "txtFileNamePattern";
            this.txtFileNamePattern.Size = new System.Drawing.Size(350, 20);
            this.txtFileNamePattern.TabIndex = 5;
            this.txtFileNamePattern.Text = "*.xlsx";
            // 
            // btnSaveAutoDetectionSettings
            // 
            this.btnSaveAutoDetectionSettings.Location = new System.Drawing.Point(20, 150);
            this.btnSaveAutoDetectionSettings.Name = "btnSaveAutoDetectionSettings";
            this.btnSaveAutoDetectionSettings.Size = new System.Drawing.Size(100, 30);
            this.btnSaveAutoDetectionSettings.TabIndex = 6;
            this.btnSaveAutoDetectionSettings.Text = "შენახვა";
            this.btnSaveAutoDetectionSettings.UseVisualStyleBackColor = true;
            this.btnSaveAutoDetectionSettings.Click += new System.EventHandler(this.btnSaveAutoDetectionSettings_Click);
            // 
            // chkSmsEnabled
            // 
            this.chkSmsEnabled.AutoSize = true;
            this.chkSmsEnabled.Location = new System.Drawing.Point(50, 55);
            this.chkSmsEnabled.Name = "chkSmsEnabled";
            this.chkSmsEnabled.Size = new System.Drawing.Size(80, 17);
            this.chkSmsEnabled.TabIndex = 22;
            this.chkSmsEnabled.Text = "checkBox1";
            this.chkSmsEnabled.UseVisualStyleBackColor = true;
            this.chkSmsEnabled.CheckedChanged += new System.EventHandler(this.chkSmsEnabled_CheckedChanged);
            // 
            // btnSaveOverSmsTexts
            // 
            this.btnSaveOverSmsTexts.Location = new System.Drawing.Point(1093, 370);
            this.btnSaveOverSmsTexts.Name = "btnSaveOverSmsTexts";
            this.btnSaveOverSmsTexts.Size = new System.Drawing.Size(75, 23);
            this.btnSaveOverSmsTexts.TabIndex = 21;
            this.btnSaveOverSmsTexts.Text = "შენახვა";
            this.btnSaveOverSmsTexts.UseVisualStyleBackColor = true;
            this.btnSaveOverSmsTexts.Click += new System.EventHandler(this.btnSaveOverSmsTexts_Click);
            // 
            // btnSaveUpcPaySmsTexts
            // 
            this.btnSaveUpcPaySmsTexts.Location = new System.Drawing.Point(1093, 251);
            this.btnSaveUpcPaySmsTexts.Name = "btnSaveUpcPaySmsTexts";
            this.btnSaveUpcPaySmsTexts.Size = new System.Drawing.Size(75, 23);
            this.btnSaveUpcPaySmsTexts.TabIndex = 21;
            this.btnSaveUpcPaySmsTexts.Text = "შენახვა";
            this.btnSaveUpcPaySmsTexts.UseVisualStyleBackColor = true;
            this.btnSaveUpcPaySmsTexts.Click += new System.EventHandler(this.btnSaveUpcPaySmsTexts_Click);
            // 
            // btnSavePaySmsTexts
            // 
            this.btnSavePaySmsTexts.Location = new System.Drawing.Point(469, 508);
            this.btnSavePaySmsTexts.Name = "btnSavePaySmsTexts";
            this.btnSavePaySmsTexts.Size = new System.Drawing.Size(75, 23);
            this.btnSavePaySmsTexts.TabIndex = 21;
            this.btnSavePaySmsTexts.Text = "შენახვა";
            this.btnSavePaySmsTexts.UseVisualStyleBackColor = true;
            this.btnSavePaySmsTexts.Click += new System.EventHandler(this.btnSavePaySmsTexts_Click);
            // 
            // btnSaveRegSmsTexts
            // 
            this.btnSaveRegSmsTexts.Location = new System.Drawing.Point(469, 389);
            this.btnSaveRegSmsTexts.Name = "btnSaveRegSmsTexts";
            this.btnSaveRegSmsTexts.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRegSmsTexts.TabIndex = 21;
            this.btnSaveRegSmsTexts.Text = "შენახვა";
            this.btnSaveRegSmsTexts.UseVisualStyleBackColor = true;
            this.btnSaveRegSmsTexts.Click += new System.EventHandler(this.btnSaveRegSmsTexts_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(655, 292);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(215, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "გადახდის გადაცილების შეტყობინება";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(655, 172);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(229, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "მოახლოვებული გადახდის შეტყობინება";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 430);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "გადახდის შეტყობინება";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 308);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(158, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "რეგისტრაციის შეტყობინება";
            // 
            // txtSmsOverdue
            // 
            this.txtSmsOverdue.Location = new System.Drawing.Point(658, 308);
            this.txtSmsOverdue.Multiline = true;
            this.txtSmsOverdue.Name = "txtSmsOverdue";
            this.txtSmsOverdue.Size = new System.Drawing.Size(429, 85);
            this.txtSmsOverdue.TabIndex = 16;
            // 
            // txtSmsPayment
            // 
            this.txtSmsPayment.Location = new System.Drawing.Point(34, 446);
            this.txtSmsPayment.Multiline = true;
            this.txtSmsPayment.Name = "txtSmsPayment";
            this.txtSmsPayment.Size = new System.Drawing.Size(429, 85);
            this.txtSmsPayment.TabIndex = 16;
            // 
            // txtSmsUpcoming
            // 
            this.txtSmsUpcoming.Location = new System.Drawing.Point(658, 189);
            this.txtSmsUpcoming.Multiline = true;
            this.txtSmsUpcoming.Name = "txtSmsUpcoming";
            this.txtSmsUpcoming.Size = new System.Drawing.Size(429, 85);
            this.txtSmsUpcoming.TabIndex = 16;
            // 
            // txtSmsRegistration
            // 
            this.txtSmsRegistration.Location = new System.Drawing.Point(34, 327);
            this.txtSmsRegistration.Multiline = true;
            this.txtSmsRegistration.Name = "txtSmsRegistration";
            this.txtSmsRegistration.Size = new System.Drawing.Size(429, 85);
            this.txtSmsRegistration.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(169, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "სატესტო ტელეფონის ნომერი";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "SMS Key";
            // 
            // tbTestNumber
            // 
            this.tbTestNumber.Location = new System.Drawing.Point(36, 248);
            this.tbTestNumber.Multiline = true;
            this.tbTestNumber.Name = "tbTestNumber";
            this.tbTestNumber.Size = new System.Drawing.Size(144, 26);
            this.tbTestNumber.TabIndex = 13;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(186, 251);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 12;
            this.btnTest.Text = "შემოწმება";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnSaveApiKey
            // 
            this.btnSaveApiKey.Location = new System.Drawing.Point(490, 172);
            this.btnSaveApiKey.Name = "btnSaveApiKey";
            this.btnSaveApiKey.Size = new System.Drawing.Size(75, 23);
            this.btnSaveApiKey.TabIndex = 11;
            this.btnSaveApiKey.Text = "შენახვა";
            this.btnSaveApiKey.UseVisualStyleBackColor = true;
            this.btnSaveApiKey.Click += new System.EventHandler(this.btnSaveApiKey_Click_1);
            // 
            // txtSmsApiKey
            // 
            this.txtSmsApiKey.Location = new System.Drawing.Point(36, 174);
            this.txtSmsApiKey.Name = "txtSmsApiKey";
            this.txtSmsApiKey.Size = new System.Drawing.Size(427, 20);
            this.txtSmsApiKey.TabIndex = 10;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1539, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // sogBox1
            // 
            this.sogBox1.Controls.Add(this.btnSaveDocPath);
            this.sogBox1.Controls.Add(this.label3);
            this.sogBox1.Controls.Add(this.txtDownloadFolder);
            this.sogBox1.Controls.Add(this.txtBaseUrl);
            this.sogBox1.Controls.Add(this.dgvGroups);
            this.sogBox1.Controls.Add(this.btnChooseDir);
            this.sogBox1.Controls.Add(this.label2);
            this.sogBox1.Controls.Add(this.btnSave);
            this.sogBox1.Controls.Add(this.label1);
            this.sogBox1.Location = new System.Drawing.Point(6, 6);
            this.sogBox1.Name = "sogBox1";
            this.sogBox1.Size = new System.Drawing.Size(764, 627);
            this.sogBox1.TabIndex = 6;
            this.sogBox1.TabStop = false;
            this.sogBox1.Text = "groupBox3";
            // 
            // sogBox2
            // 
            this.sogBox2.Location = new System.Drawing.Point(776, 6);
            this.sogBox2.Name = "sogBox2";
            this.sogBox2.Size = new System.Drawing.Size(725, 627);
            this.sogBox2.TabIndex = 7;
            this.sogBox2.TabStop = false;
            this.sogBox2.Text = "სინქრონიზაციის პარამეტრები";
            // 
            // AdminPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1539, 930);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AdminPanelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.AdminPanelForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.DatabaseSettings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGVUnassignedStudents)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.UserManagement.ResumeLayout(false);
            this.UserManagement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegisteredUsers)).EndInit();
            this.PaymentsFinance.ResumeLayout(false);
            this.SystemOperations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            this.SMSServiceSettings.ResumeLayout(false);
            this.SMSServiceSettings.PerformLayout();
            this.AutoFileDetectionSettings.ResumeLayout(false);
            this.groupBoxAutoDetection.ResumeLayout(false);
            this.groupBoxAutoDetection.PerformLayout();
            this.sogBox1.ResumeLayout(false);
            this.sogBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Button btnSaveConnection;
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
    }
}
