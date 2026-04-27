using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services;
using BCCStudents.Application.Services.Update;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using static BCCStudents.Presentation.MainForm;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class AdminPanelForm : Form
    {
        private readonly IStudentService _studentService;
        private readonly ICleanupService _cleanupService;
        private readonly IUserService _userService;
        private readonly DocumentService _documentService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGroupRepository _groupRepository;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IConnectionStatusService _connectionStatusService;
        private readonly IConfigurationService _configService;
        private readonly ISystemConfigurationService _systemConfigService;
        private readonly BackupService _backupManager;
        private readonly AdminCodeManager _adminCodeManager;
        private readonly IApplicationStatus _appStatus;
        private readonly UserManagementFormFactory _userManagementFormFactory;
        private DocumentConfig _config;
        private MainForm _mainForm;
        private TextBox txtAdminCode;
        private Button btnSetAdminCode;
        private TabPage tabLogs;
        private Panel panelLogs;
        private LogViewerForm logViewerForm;
        private System.Windows.Forms.CheckBox chkAutoDownstream;
        private System.Windows.Forms.CheckBox chkAutoUpstream;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkUseFullBalance;
        private System.Windows.Forms.CheckBox chkAllowPartialPayments;
        private delegate SetStudyStartDateForm setStudyStartDateFormFactory();
        private readonly SetStudyStartDateFormFactory _setStudyStartDateFormFactory;
        private Button btnCheckUpdates;
        public AdminPanelForm(IDatabaseConnectionProvider databaseConnectionProvider,
            IGroupRepository groupRepository,
            IStudentService studentService,
            ICleanupService cleanupService,
            IUserService userService,
            DocumentService documentService,
            IServiceProvider serviceProvider,
            IConnectionStatusService connectionStatusService,
            IConfigurationService configurationService,
            BackupService backupManager,
            AdminCodeManager adminCodeManager,
            IApplicationStatus appStatus,
            ISystemConfigurationService systemConfigurationService,
            SetStudyStartDateFormFactory setStudyStartDateFormFactory,
            UserManagementFormFactory userManagementFormFactory
            )
        {
            InitializeComponent();
            _studentService = studentService;
            _cleanupService = cleanupService;
            _userService = userService;
            _documentService = documentService;
            _serviceProvider = serviceProvider;
            _groupRepository = groupRepository;
            _connectionProvider = databaseConnectionProvider;
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            _adminCodeManager = adminCodeManager ?? throw new ArgumentNullException(nameof(adminCodeManager));
            _configService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            _systemConfigService = systemConfigurationService;
            _appStatus = appStatus ?? throw new ArgumentNullException(nameof(appStatus));
            _setStudyStartDateFormFactory = setStudyStartDateFormFactory ?? throw new ArgumentNullException(nameof(setStudyStartDateFormFactory));
            _userManagementFormFactory = userManagementFormFactory ?? throw new ArgumentNullException(nameof(userManagementFormFactory));
            if (_configService.IsTestDb)
                FormTitleHelper.SetTitle(this, "პროგრამის პარამეტრები - სატესტო რეჟიმი");
            else
                FormTitleHelper.SetTitle(this, "პროგრამის პარამეტრები");
            // GroupId – ფარული, აუცილებელი მხოლოდ შინაგანად
            var idColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                Name = "GroupId",
                Visible = false
            };

            // GroupName – გამორჩეულად სვეტი სახელით
            var nameColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "ჯგუფის დასახელება",
                Name = "GroupName",
                ReadOnly = true,
                Width = 200
            };
            var contractColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "ხელშეკრულების შაბლონი",
                Name = "ContractTemplatePath",
                Width = 300
            };

            var browseColumn = new DataGridViewButtonColumn
            {
                HeaderText = "არჩევა",
                Name = "BrowseTemplate",
                Text = "აირჩიე",
                UseColumnTextForButtonValue = true
            };
            dgvGroups.Columns.AddRange(idColumn, nameColumn, contractColumn, browseColumn);

            // სცადე MainForm-ის ამოღება DI-დან, რათა მოგვიანებით თარიღის ლეიბლის განახლებისას NullReference არ იყოს
            try
            {
                _mainForm = _serviceProvider.GetService<MainForm>();
            }
            catch { }

            chkAutoDownstream = new System.Windows.Forms.CheckBox { Text = "Downstream ავტო-სინქი", AutoSize = true, Location = new System.Drawing.Point(20, 20) };
            chkAutoUpstream = new System.Windows.Forms.CheckBox { Text = "Upstream ავტო-სინქი", AutoSize = true, Location = new System.Drawing.Point(20, 50) };
            chkAutoUpdate = new System.Windows.Forms.CheckBox { Text = "ავტომატური განახლება", AutoSize = true, Location = new System.Drawing.Point(20, 80) };
            chkUseFullBalance = new System.Windows.Forms.CheckBox { Text = "სრული ბალანსის გამოყენება (რამდენ თვესაც ფარავს)", AutoSize = true, Location = new System.Drawing.Point(20, 110) };
            chkAllowPartialPayments = new System.Windows.Forms.CheckBox { Text = "ნაწილობრივი გადახდის დაშვება", AutoSize = true, Location = new System.Drawing.Point(20, 140) };
            btnCheckUpdates = new Button { Text = "პროგრამის განახლება", AutoSize = true, Location = new System.Drawing.Point(20, 170) };
            chkAutoDownstream.Checked = _configService.AutoDownstreamSyncEnabled;
            chkAutoUpstream.Checked = _configService.AutoUpstreamSyncEnabled;
            chkAutoUpdate.Checked = _configService.AutoUpdateEnabled;
            chkUseFullBalance.Checked = _configService.UseFullBalanceForAutoPayment; // default = true
            chkAllowPartialPayments.Checked = _configService.AllowPartialPayments;

            // Load connection strings to fields
            LoadConnectionStringsToFields();

            chkAutoDownstream.CheckedChanged += (s, e) => { _configService.AutoDownstreamSyncEnabled = chkAutoDownstream.Checked; _configService.Save(); };
            chkAutoUpstream.CheckedChanged += (s, e) => { _configService.AutoUpstreamSyncEnabled = chkAutoUpstream.Checked; _configService.Save(); };
            chkAutoUpdate.CheckedChanged += (s, e) => { _configService.AutoUpdateEnabled = chkAutoUpdate.Checked; _configService.Save(); };
            chkUseFullBalance.CheckedChanged += (s, e) => { _configService.UseFullBalanceForAutoPayment = chkUseFullBalance.Checked; _configService.Save(); };
            chkAllowPartialPayments.CheckedChanged += (s, e) => { _configService.AllowPartialPayments = chkAllowPartialPayments.Checked; _configService.Save(); };

            this.sogBox2.Controls.Add(chkAutoDownstream);
            this.sogBox2.Controls.Add(chkAutoUpstream);
            this.sogBox2.Controls.Add(chkAutoUpdate);
            this.sogBox2.Controls.Add(chkUseFullBalance);
            this.sogBox2.Controls.Add(chkAllowPartialPayments);
            this.sogBox2.Controls.Add(btnCheckUpdates);

            btnCheckUpdates.Click += async (s, e) =>
            {
                try
                {
                    var svc = _serviceProvider.GetRequiredService<UpdateService>();
                    var manifest = await svc.GetManifestAsync(System.Threading.CancellationToken.None);
                    if (manifest == null)
                    {
                        MessageBox.Show("მანიფესტი ვერ მოიძებნა.");
                        return;
                    }
                    var current = svc.GetCurrentVersion();
                    var latest = new System.Version(manifest.latestVersion);
                    if (!svc.IsNewer(latest, current))
                    {
                        MessageBox.Show("უკვე ბოლო ვერსია გაქვთ.");
                        return;
                    }

                    // შეტყობინების ჩვენება
                    var isRequired = svc.IsUpdateRequired(latest, current);
                    var message = $"გამოვიდა პროგრამის ახალი ვერსია.\n\n" +
                                 $"მიმდინარე ვერსია: {current}\n" +
                                 $"ახალი ვერსია: {latest}\n\n" +
                                 $"{(isRequired ? "განახლება აუცილებელია!" : "გსურთ განახლება?")}";

                    var result = MessageBox.Show(message, "განახლება",
                        isRequired ? MessageBoxButtons.OK : MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (!isRequired && result != DialogResult.Yes)
                    {
                        MessageBox.Show("განახლება გადაიდო.", "განახლება გადაიდო",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    using (var dlg = new UpdateProgressForm())
                    {
                        dlg.Show(this);
                        string zip = null;
                        try
                        {
                            var progress = new Progress<(long current, long total)>(p => dlg.Report(p.current, p.total));
                            zip = await svc.DownloadAsync(manifest, progress, System.Threading.CancellationToken.None);
                        }
                        finally { dlg.Close(); }
                        if (!string.IsNullOrWhiteSpace(zip))
                        {
                            await svc.ScheduleApplyAndRestartAsync(zip, this);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"განახლების შეცდომა: {ex.Message}");
                }
            };
        }
        public void SetMainForm(MainForm mainForm)
        {
            _mainForm = mainForm;
        }
        private void btnSync_Click(object sender, EventArgs e)
        {
            /*try
            {
                 _studentService.MigrateStudentToGroup();
                MessageBox.Show("მონაცემები წარმატებით სინქრონიზებულია.", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა: " + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void btnCheckStudents_Click(object sender, EventArgs e)
        {
            try
            {
                var unassignedStudents = _studentService.GetUnassignedStudents();

                if (unassignedStudents.Rows.Count == 0)
                {
                    MessageBox.Show("ყველა სტუდენტი დაკავშირებულია ჯგუფებთან!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    dGVUnassignedStudents.DataSource = unassignedStudents;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა: " + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            if (!_userService.IsCurrentUserAdmin())
            {
                MessageBox.Show("⛔ ამ ქმედების შესრულება შეუძლია მხოლოდ ადმინისტრატორს!", "წვდომა აკრძალულია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var confirm = MessageBox.Show("ნამდვილად გსურს ყველა მონაცემის განულება?", "დასტური", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _cleanupService.ResetAllData();
                MessageBox.Show("✅ მონაცემები წარმატებით განულდა!");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("⛔ მხოლოდ ადმინისტრატორს აქვს წვდომა.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ შეცდომა: {ex.Message}");
            }
        }

        private void btnChooseDir_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDownloadFolder.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _documentService.DownloadBaseFolder = txtDownloadFolder.Text;
            _documentService.FileServerBaseUrl = txtBaseUrl.Text;

            // 2. შეინახე ცვლილებები ფაილშიც
            var config = new DocumentConfig
            {
                DownloadPath = _documentService.DownloadBaseFolder,
                FileUrl = _documentService.FileServerBaseUrl
            };
            config.Save();
            MessageBox.Show("✅ პარამეტრები შენახულია!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void AdminPanelForm_Load(object sender, EventArgs e)
        {
            // Emergency Setup Mode: Offline რეჟიმში მხოლოდ Database Settings ტაბი უნდა იყოს ხელმისაწვდომი
            if (!_appStatus.IsDatabaseOnline)
            {
                foreach (TabPage tab in tabControl1.TabPages)
                {
                    if (tab != DatabaseSettings)
                    {
                        tab.Enabled = false;
                    }
                }
            }
            if (!_connectionStatusService.IsConnected)
            {
                MessageBox.Show("ბაზასთან კავშირი ამჟამად მიუწვდომელია. ფორმა გაიხსნება მხოლოდ ნახვისთვის.", "კავშირი", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadConnectionSettings();
                _config = DocumentConfig.Load();

                txtDownloadFolder.Text = _config.DownloadPath;
                txtBaseUrl.Text = _config.FileUrl;

                txtSmsApiKey.Text = _configService.SmsApiKey;
                txtSmsRegistration.Text = _configService.SmsText_Registration;
                txtSmsPayment.Text = _configService.SmsText_Payment;
                txtSmsUpcoming.Text = _configService.SmsText_UpcomingReminder;
                txtSmsOverdue.Text = _configService.SmsText_OverdueReminder;
                chkSmsEnabled.Checked = _configService.SmsEnabled;
                if (chkSmsEnabled.Checked)
                    chkSmsEnabled.Text = "SMS სერვისი ჩართულია";
                else chkSmsEnabled.Text = "SMS სერვისი გამორთულია";
                return;
            }
            lblDbMode.Text = _configService.IsTestDb ? "ტესტ რეჟიმი" : "სამუშაო რეჟიმი";
            lblDbMode.ForeColor = _configService.IsTestDb ? Color.OrangeRed : Color.DarkGreen;


            LoadConnectionSettings();
            _config = DocumentConfig.Load();

            txtDownloadFolder.Text = _config.DownloadPath;
            txtBaseUrl.Text = _config.FileUrl;

            txtSmsApiKey.Text = _configService.SmsApiKey;
            txtSmsRegistration.Text = _configService.SmsText_Registration;
            txtSmsPayment.Text = _configService.SmsText_Payment;
            txtSmsUpcoming.Text = _configService.SmsText_UpcomingReminder;
            txtSmsOverdue.Text = _configService.SmsText_OverdueReminder;
            chkSmsEnabled.Checked = _configService.SmsEnabled;
            if (chkSmsEnabled.Checked)
                chkSmsEnabled.Text = "SMS სერვისი ჩართულია";
            else chkSmsEnabled.Text = "SMS სერვისი გამორთულია";
            GetAllGroups();
            DisplayAllUsers();
            InitializeLogsTab();
            LoadAutoDetectionSettings();
        }
        private void DisplayAllUsers()
        {
            var users = _userService.GetAllUsers();
            dgvRegisteredUsers.DataSource = users;
            lblusersInfo.Text = "სულ - " + users.Count + " რეგისტრირებული მომხმარებელი";
        }
        private void btnSetStudyStartDate_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var setStudyStartDateForm = _setStudyStartDateFormFactory.Invoke();
            // მიიღე საჭირო რეპოზიტორიები (DI-დან ან ხელით)
            if (setStudyStartDateForm.ShowDialog() == DialogResult.OK)
            {
                DateTime selectedDate = setStudyStartDateForm.SelectedDate.Date;
                switch (btn.Tag?.ToString())
                {
                    case "Study":
                        _systemConfigService.SetStudyStartDate(selectedDate);
                        break;
                    case "Payment":
                        _systemConfigService.SetDefaultPaymentDate(selectedDate);
                        _mainForm.PaymentNextDateLabel_Update();
                        break;
                }

                if (_mainForm != null)
                {
                    _mainForm.StudyStartDateLabel_Update();
                }
                MessageBox.Show($"✅ თარიღი შენახულია: {selectedDate:dd-MM-yyyy}",
                            "დადასტურება",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            }
        }

        private void dgvGroups_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvGroups.Columns[e.ColumnIndex].Name == "BrowseTemplate" && e.RowIndex >= 0)
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Word Documents|*.docx";
                ofd.Title = "აირჩიე ხელშეკრულების შაბლონი";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    dgvGroups.Rows[e.RowIndex].Cells["ContractTemplatePath"].Value = ofd.FileName;
                }
            }
        }

        private void btnSaveDocPath_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvGroups.Rows)
            {
                int groupId = Convert.ToInt32(row.Cells["GroupId"].Value);
                string templatePath = row.Cells["ContractTemplatePath"].Value?.ToString();

                _groupRepository.UpdateGroupContractTemplatePath(groupId, templatePath);
            }

        }
        public void GetAllGroups()
        {
            var groups = _groupRepository.GetAllGroups();

            foreach (var group in groups)
            {
                dgvGroups.Rows.Add(group.Id, group.Name, group.ContractTemplatePath);
            }

        }

        private async void btnTest_Click(object sender, EventArgs e)
        {
            var smsService = _serviceProvider.GetRequiredService<ISmsService>();
            var smsresult = await smsService.SendSmsAsync(tbTestNumber.Text, "სატესტო შეტყობინება");
            if (smsresult.Success)
            {
                MessageBox.Show("სატესტო შეტყობინება წარმატებით გაიგზავნა!");
            }
            else
            {
                MessageBox.Show($"შეცდომა: {smsresult.Status}");
            }
        }

        private void btnSaveRegSmsTexts_Click(object sender, EventArgs e)
        {
            _configService.SmsText_Registration = txtSmsRegistration.Text.Trim();

            _configService.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void btnSavePaySmsTexts_Click(object sender, EventArgs e)
        {
            _configService.SmsText_Payment = txtSmsPayment.Text.Trim();

            _configService.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void btnSaveUpcPaySmsTexts_Click(object sender, EventArgs e)
        {
            _configService.SmsText_UpcomingReminder = txtSmsUpcoming.Text.Trim();

            _configService.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void btnSaveOverSmsTexts_Click(object sender, EventArgs e)
        {
            _configService.SmsText_OverdueReminder = txtSmsOverdue.Text.Trim();

            _configService.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void chkSmsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSmsEnabled.Checked)
            { chkSmsEnabled.Text = "SMS სერვისი ჩართულია"; _configService.SmsEnabled = true; _configService.Save(); }
            else
            { chkSmsEnabled.Text = "SMS სერვისი გამორთულია"; _configService.SmsEnabled = false; _configService.Save(); }
        }
        public void LoadConnectionSettings()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySQLConnection"]?.ConnectionString;

            if (!string.IsNullOrWhiteSpace(connStr))
            {
                // Regex-ით ამოვიღოთ ძირითადი ველები
                localHost.Text = GetValue(connStr, "Server");
                LocalPort.Text = GetValue(connStr, "Port");
                localDbName.Text = GetValue(connStr, "Database");
                localUsrName.Text = GetValue(connStr, "User");
                localUsrPass.Text = GetValue(connStr, "Password");
            }

            localConnStatus.Text = _connectionStatusService.IsConnected ? "✅ კავშირი დამყარებულია" : "❌ კავშირი არ არის";
            localConnStatus.ForeColor = _connectionStatusService.IsConnected ? Color.Green : Color.Red;
        }

        private void LoadConnectionStringsToFields()
        {
            try
            {
                bool isTest = _configService.IsTestDb;
                bool useLocal = _configService.UseLocalDb;

                if (isTest)
                {
                    // Load test connection strings
                    var localConn = _configService.LocalMySqlConnectionString_Test;
                    var serverConn = _configService.ServerMySqlConnectionString_Test;

                    // Fallback to production if test strings are empty
                    if (string.IsNullOrWhiteSpace(localConn))
                        localConn = _configService.LocalMySqlConnectionString;
                    if (string.IsNullOrWhiteSpace(serverConn))
                        serverConn = _configService.ServerMySqlConnectionString;

                    LoadConnectionStringToFields(localConn, serverConn);
                }
                else
                {
                    // Load production connection strings
                    var localConn = _configService.LocalMySqlConnectionString;
                    var serverConn = _configService.ServerMySqlConnectionString;

                    LoadConnectionStringToFields(localConn, serverConn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა კავშირის სტრინგების ჩატვირთვისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadConnectionStringToFields(string localConn, string serverConn)
        {
            if (!string.IsNullOrWhiteSpace(localConn))
            {
                localHost.Text = GetValue(localConn, "Server");
                LocalPort.Text = GetValue(localConn, "Port");
                localDbName.Text = GetValue(localConn, "Database");
                localUsrName.Text = GetValue(localConn, "User");
                localUsrPass.Text = GetValue(localConn, "Password");
            }

            if (!string.IsNullOrWhiteSpace(serverConn))
            {
                serverHost.Text = GetValue(serverConn, "Server");
                serverPort.Text = GetValue(serverConn, "Port");
                serverDbName.Text = GetValue(serverConn, "Database");
                serverUsrName.Text = GetValue(serverConn, "User");
                serverUsrPass.Text = GetValue(serverConn, "Password");
            }
        }

        // ეხმარება კონკრეტული Key=Value წყვილის გამოტანაში
        private string GetValue(string connStr, string key)
        {
            var match = Regex.Match(connStr, $@"{key}\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : "";
        }
        private void testLocalConn_Click(object sender, EventArgs e)
        {
            string server = localHost.Text.Trim();
            string port = LocalPort.Text.Trim();
            string database = localDbName.Text.Trim();
            string user = localUsrName.Text.Trim();
            string password = localUsrPass.Text.Trim();

            string localConnStr = $"Server={server};Port={port};Database={database};User={user};Password={password};SslMode=Preferred;";

            using (var conn = new MySqlConnection(localConnStr))
            {
                try
                {
                    conn.Open();
                    localConnStatus.Text = "✅ კავშირი წარმატებით დამყარდა";
                    localConnStatus.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    localConnStatus.Text = "❌ კავშირის შეცდომა";
                    localConnStatus.ForeColor = Color.Red;
                    MessageBox.Show("შეცდომა კავშირის დროს:\n" + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void testSrvConn_Click(object sender, EventArgs e)
        {
            string server = serverHost.Text.Trim();
            string port = serverPort.Text.Trim();
            string database = serverDbName.Text.Trim();
            string user = serverUsrName.Text.Trim();
            string password = serverUsrPass.Text.Trim();

            string serverConnStr = $"Server={server};Port={port};Database={database};User={user};Password={password};SslMode=Preferred;";

            using (var conn = new MySqlConnection(serverConnStr))
            {
                try
                {
                    conn.Open();
                    serverConnStatus.Text = "✅ კავშირი წარმატებით დამყარდა";
                    serverConnStatus.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    serverConnStatus.Text = "❌ კავშირის შეცდომა";
                    serverConnStatus.ForeColor = Color.Red;
                    MessageBox.Show("შეცდომა კავშირის დროს:\n" + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveLocalConn_Click(object sender, EventArgs e)
        {
            try
            {
                string server = localHost.Text.Trim();
                string port = LocalPort.Text.Trim();
                string database = localDbName.Text.Trim();
                string user = localUsrName.Text.Trim();
                string password = localUsrPass.Text.Trim();

                // ავაწყოთ სრული connection string
                string localConnStr =
                    $"Server={server};Port={port};Database={database};User={user};Password={password};SslMode=Preferred;CharSet=utf8mb4;";

                // შევინახოთ Settings.settings-ში მთლიანად
                if (_configService.IsTestDb)
                    _configService.LocalMySqlConnectionString_Test = localConnStr;
                else
                    _configService.LocalMySqlConnectionString = localConnStr;

                _configService.Save();

                MessageBox.Show("ლოკალური კავშირის პარამეტრები შენახულია Settings-ში.", "წარმატება",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა ლოკალური კავშირის შენახვისას: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveServerConn_Click(object sender, EventArgs e)
        {
            try
            {
                string server = serverHost.Text.Trim();
                string port = serverPort.Text.Trim();
                string database = serverDbName.Text.Trim();
                string user = serverUsrName.Text.Trim();
                string password = serverUsrPass.Text.Trim();

                // ავაწყოთ სრული connection string
                string serverConnStr =
                    $"Server={server};Port={port};Database={database};User={user};Password={password};SslMode=Preferred;CharSet=utf8mb4;";

                // შევინახოთ Settings.settings-ში მთლიანად
                if (_configService.IsTestDb)
                    _configService.ServerMySqlConnectionString_Test = serverConnStr;
                else
                    _configService.ServerMySqlConnectionString = serverConnStr;

                _configService.Save();

                MessageBox.Show("სერვერის კავშირის პარამეტრები შენახულია Settings-ში.", "წარმატება",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა სერვერის კავშირის შენახვისას: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnAddStudentToGroup_Click(object sender, EventArgs e)
        {

        }

        private void btnSwitchToTestDB_Click(object sender, EventArgs e)
        {
            // შეცვალე სატესტო/სამუშაო ბაზის რეჟიმი
            _configService.IsTestDb = !_configService.IsTestDb;
            _configService.Save();

            // ვიზუალური ინდიკატორის განახლება (სურვილისამებრ)
            if (lblDbMode != null)
            {
                lblDbMode.Text = _configService.IsTestDb ? "ტესტ რეჟიმი" : "სამუშაო რეჟიმი";
                lblDbMode.ForeColor = _configService.IsTestDb ? Color.OrangeRed : Color.DarkGreen;
            }

            // შეტყობინება
            MessageBox.Show(_configService.IsTestDb
                ? "✅ გადაერთე სატესტო ბაზაზე!"
                : "✅ გადაერთე სამუშაო ბაზაზე!");

            // სურვილისამებრ, თუ გინდა პირდაპირ ახალ კავშირებს გამოჩნდეს ეფექტი, რესტარტი ყველაზე მარტივია
            System.Windows.Forms.Application.Restart();
        }

        private void btnSaveApiKey_Click_1(object sender, EventArgs e)
        {
            _configService.SmsApiKey = txtSmsApiKey.Text.Trim();
            _configService.Save();

            MessageBox.Show("✅ API Key წარმატებით შენახულია.", "დადასტურება", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            _backupManager.ManualBackup("SchoolManagement.db");
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            _backupManager.ManualRestore("SchoolManagement.db");
        }

        private void btnRegisterUser_Click(object sender, EventArgs e)
        {
            // იხსნება UserManagementForm-ის ნაცვლად RegisterForm-ის
            var userManagementForm = _userManagementFormFactory.Invoke();
            userManagementForm.Show();
        }
        private void InitializeAdminCodeControls()
        {
            txtAdminCode = new TextBox { Width = 120, PasswordChar = '*' };
            btnSetAdminCode = new Button { Text = "კოდის შეცვლა", Width = 100 };
            btnSetAdminCode.Click += (s, e) =>
            {
                _adminCodeManager.SetAdminCode(txtAdminCode.Text);
                MessageBox.Show("ადმინისტრატორის კოდი განახლდა.");
                txtAdminCode.Text = string.Empty;
            };
            // UI-ში დაამატეთ შესაბამის ადგილას, მაგალითად პანელზე ან ფორმაზე:
            this.Controls.Add(txtAdminCode);
            this.Controls.Add(btnSetAdminCode);
            txtAdminCode.Top = 10; txtAdminCode.Left = 10;
            btnSetAdminCode.Top = 10; btnSetAdminCode.Left = 140;
        }
        private void InitializeLogsTab()
        {
            tabLogs = new TabPage("ლოგები");
            panelLogs = new Panel { Dock = DockStyle.Fill };
            logViewerForm = new LogViewerForm();
            logViewerForm.TopLevel = false;
            logViewerForm.FormBorderStyle = FormBorderStyle.None;
            logViewerForm.Dock = DockStyle.Fill;
            panelLogs.Controls.Add(logViewerForm);
            tabLogs.Controls.Add(panelLogs);
            tabControl1.TabPages.Add(tabLogs);
            logViewerForm.Show();
        }
        public AdminPanelForm()
        {
            InitializeComponent();
            InitializeAdminCodeControls();
        }

        private void SetPlaceholder(TextBox tb, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = placeholder;
                tb.ForeColor = Color.Gray;
            }
        }

        private void RemovePlaceholder(TextBox tb, string placeholder)
        {
            if (tb.Text == placeholder)
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
            }
        }

        private void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "აირჩიეთ საქაღალდე ფაილების მონიტორინგისთვის";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtWatchDirectory.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSaveAutoDetectionSettings_Click(object sender, EventArgs e)
        {
            try
            {
                _configService.SaveAutoDetectionSettings(
                    chkAutoDetectionEnabled.Checked,
                    txtWatchDirectory.Text.Trim(),
                    txtFileNamePattern.Text.Trim()
                );

                MessageBox.Show("კონფიგურაცია წარმატებით შეინახა!", "შენახვა",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAutoDetectionSettings()
        {
            try
            {
                // ფორმა მხოლოდ სერვისს ეკითხება მნიშვნელობებს
                chkAutoDetectionEnabled.Checked = _configService.IsAutoDetectionEnabled();
                txtWatchDirectory.Text = _configService.GetWatchFolderPath();
                txtFileNamePattern.Text = _configService.GetFileNamePattern();
            }
            catch (Exception ex)
            {
                // შეცდომის დამუშავება რჩება UI დონეზე
                MessageBox.Show($"შეცდომა კონფიგურაციის ჩატვირთვისას: {ex.Message}",
                    "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


