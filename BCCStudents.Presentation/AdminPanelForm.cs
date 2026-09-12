using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services;
using BCCStudents.Application.Services.Update;
using BCCStudents.Domain.Enums;
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
    public partial class AdminPanelForm : BaseForm
    {
        private readonly IStudentService _studentService;
        private readonly ICleanupService _cleanupService;
        private readonly IUserService _userService;
        private readonly DocumentService _documentService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupService _groupService;
        private readonly IDatabaseConnectionProvider _connectionProvider;
        private readonly IConnectionStatusService _connectionStatusService;
        private readonly IConfigurationService _configService;
        private readonly ISystemConfigurationService _systemConfigService;
        private readonly AdminCodeManager _adminCodeManager;
        private readonly IApplicationStatus _appStatus;
        private readonly UserManagementFormFactory _userManagementFormFactory;
        private readonly LogViewerFormFactory _logViewerFormFactory;
        private readonly ILogStorageSettings _logStorageSettings;
        private readonly IDatabaseConnectionChecker _connectionChecker;
        private readonly IUserContext _userContext;
        private DocumentConfig _config;
        private MainForm _mainForm;
        private TextBox txtAdminCode;
        private Button btnSetAdminCode;
        private TabPage tabLogs;
        private Panel panelLogs;
        private LogViewerForm logViewerForm;
        private GroupBox grpLogStorage;
        private RadioButton rbLogStorageLocal;
        private RadioButton rbLogStorageServer;
        private Button btnSaveLogStorage;
        private Label lblLogStorageStatus;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkUseFullBalance;
        private System.Windows.Forms.CheckBox chkAllowPartialPayments;
        private delegate SetStudyStartDateForm setStudyStartDateFormFactory();
        private readonly SetStudyStartDateFormFactory _setStudyStartDateFormFactory;
        private Button btnCheckUpdates;
        public AdminPanelForm(IDatabaseConnectionProvider databaseConnectionProvider,
            IGroupRepository groupRepository,
            IGroupService groupService,
            IStudentService studentService,
            ICleanupService cleanupService,
            IUserService userService,
            DocumentService documentService,
            IServiceProvider serviceProvider,
            IConnectionStatusService connectionStatusService,
            IConfigurationService configurationService,
            AdminCodeManager adminCodeManager,
            IApplicationStatus appStatus,
            ISystemConfigurationService systemConfigurationService,
            SetStudyStartDateFormFactory setStudyStartDateFormFactory,
            UserManagementFormFactory userManagementFormFactory,
            LogViewerFormFactory logViewerFormFactory,
            ILogStorageSettings logStorageSettings,
            IDatabaseConnectionChecker connectionChecker,
            IUserContext userContext
            )
        {
            InitializeComponent();
            _studentService = studentService;
            _cleanupService = cleanupService;
            _userService = userService;
            _documentService = documentService;
            _serviceProvider = serviceProvider;
            _groupRepository = groupRepository;
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
            _connectionProvider = databaseConnectionProvider;
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));
            _adminCodeManager = adminCodeManager ?? throw new ArgumentNullException(nameof(adminCodeManager));
            btnBackup.Visible = false;
            btnRestore.Visible = false;
            _configService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            _systemConfigService = systemConfigurationService;
            _appStatus = appStatus ?? throw new ArgumentNullException(nameof(appStatus));
            _setStudyStartDateFormFactory = setStudyStartDateFormFactory ?? throw new ArgumentNullException(nameof(setStudyStartDateFormFactory));
            _userManagementFormFactory = userManagementFormFactory ?? throw new ArgumentNullException(nameof(userManagementFormFactory));
            _logViewerFormFactory = logViewerFormFactory ?? throw new ArgumentNullException(nameof(logViewerFormFactory));
            _logStorageSettings = logStorageSettings ?? throw new ArgumentNullException(nameof(logStorageSettings));
            _connectionChecker = connectionChecker ?? throw new ArgumentNullException(nameof(connectionChecker));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
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

            chkAutoUpdate = new System.Windows.Forms.CheckBox { Text = "ავტომატური განახლება", AutoSize = true, Location = new System.Drawing.Point(20, 20) };
            chkUseFullBalance = new System.Windows.Forms.CheckBox { Text = "სრული ბალანსის გამოყენება (რამდენ თვესაც ფარავს)", AutoSize = true, Location = new System.Drawing.Point(20, 50) };
            chkAllowPartialPayments = new System.Windows.Forms.CheckBox { Text = "ნაწილობრივი გადახდის დაშვება", AutoSize = true, Location = new System.Drawing.Point(20, 80) };
            btnCheckUpdates = new Button { Text = "პროგრამის განახლება", AutoSize = true, Location = new System.Drawing.Point(20, 110) };
            chkAutoUpdate.Checked = _configService.AutoUpdateEnabled;
            chkUseFullBalance.Checked = _configService.UseFullBalanceForAutoPayment; // default = true
            chkAllowPartialPayments.Checked = _configService.AllowPartialPayments;

            // Load connection strings to fields
            LoadConnectionStringsToFields();

            chkAutoUpdate.CheckedChanged += (s, e) => { _configService.AutoUpdateEnabled = chkAutoUpdate.Checked; _configService.Save(); };
            chkUseFullBalance.CheckedChanged += (s, e) => { _configService.UseFullBalanceForAutoPayment = chkUseFullBalance.Checked; _configService.Save(); };
            chkAllowPartialPayments.CheckedChanged += (s, e) => { _configService.AllowPartialPayments = chkAllowPartialPayments.Checked; _configService.Save(); };

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
                        string downloadError = null;
                        try
                        {
                            var progress = new Progress<(long current, long total)>(p => dlg.Report(p.current, p.total));
                            zip = await svc.DownloadAsync(manifest, progress, System.Threading.CancellationToken.None);
                        }
                        catch (Exception downloadEx)
                        {
                            // ჩამოტვირთვის შეცდომას ვიჭერთ ცალკე, რომ ფანჯრის ჩუმად დახურვის ნაცვლად
                            // მომხმარებელს გასაგები შეტყობინება მივცეთ.
                            downloadError = downloadEx.Message;
                        }
                        finally { dlg.Close(); }

                        if (!string.IsNullOrWhiteSpace(zip))
                        {
                            await svc.ScheduleApplyAndRestartAsync(zip, this);
                        }
                        else
                        {
                            // ჩამოტვირთვა ჩავარდა (zip == null) — ვაჩვენებთ შეცდომას
                            MessageBox.Show(
                                "განახლების ფაილი ვერ ჩამოიტვირთა." +
                                (string.IsNullOrWhiteSpace(downloadError) ? string.Empty : $"\n\nდეტალები: {downloadError}") +
                                "\n\nგთხოვთ სცადოთ მოგვიანებით ან მიმართოთ ადმინისტრატორს.",
                                "ჩამოტვირთვის შეცდომა",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
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
            if (!DocumentService.IsDownloadFolderConfigured(txtDownloadFolder.Text))
            {
                MessageBox.Show(
                    "გთხოვთ, მიუთითოთ დოკუმენტების შენახვის საქაღალდე (ველი „შენახვის ადგილი“).",
                    "შენახვის ადგილი",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtDownloadFolder.Focus();
                return;
            }

            string downloadPath = txtDownloadFolder.Text.Trim();
            try
            {
                Directory.CreateDirectory(downloadPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"საქაღალდის შექმნა ვერ მოხერხდა:\n{ex.Message}",
                    "შენახვის ადგილი",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            string fileServerUrl = txtBaseUrl.Text?.Trim();
            if (!DocumentService.IsFileServerConfigured(fileServerUrl))
            {
                MessageBox.Show(
                    "გთხოვთ, მიუთითოთ ფაილების სერვერის საბაზო URL (მაგ. https://bccenter.ge/).",
                    "ფაილები სერვერზე",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtBaseUrl.Focus();
                return;
            }

            _documentService.DownloadBaseFolder = downloadPath;
            _documentService.FileServerBaseUrl = fileServerUrl;

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

                _groupService.UpdateGroupContractTemplatePath(groupId, templatePath);
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
            MessageBox.Show("ბექაპის ფუნქცია დროებით გამორთულია.", "ინფორმაცია",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ბექაპის ფუნქცია დროებით გამორთულია.", "ინფორმაცია",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRegisterUser_Click(object sender, EventArgs e)
        {
            // იხსნება მომხმარებლების მართვის ფორმა
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

            var storagePanel = new Panel { Dock = DockStyle.Top, Height = 95, Padding = new Padding(8) };
            var lblStorage = new Label
            {
                Text = "ლოგების შენახვა:",
                AutoSize = true,
                Left = 8,
                Top = 12
            };

            grpLogStorage = new GroupBox
            {
                Text = "რეჟიმი",
                Left = 130,
                Top = 4,
                Width = 320,
                Height = 48
            };

            rbLogStorageLocal = new RadioButton
            {
                Text = "ლოკალურად",
                AutoSize = true,
                Left = 12,
                Top = 18
            };
            rbLogStorageServer = new RadioButton
            {
                Text = "სერვერზე",
                AutoSize = true,
                Left = 140,
                Top = 18
            };
            grpLogStorage.Controls.Add(rbLogStorageLocal);
            grpLogStorage.Controls.Add(rbLogStorageServer);

            btnSaveLogStorage = new Button
            {
                Text = "შენახვა",
                Left = 460,
                Top = 16,
                Width = 90,
                Height = 28
            };
            btnSaveLogStorage.Click += BtnSaveLogStorage_Click;

            lblLogStorageStatus = new Label
            {
                AutoSize = true,
                Left = 8,
                Top = 62,
                ForeColor = Color.DarkSlateGray
            };

            storagePanel.Controls.AddRange(new Control[]
            {
                lblStorage, grpLogStorage, btnSaveLogStorage, lblLogStorageStatus
            });

            panelLogs = new Panel { Dock = DockStyle.Fill };
            logViewerForm = _logViewerFormFactory();
            logViewerForm.TopLevel = false;
            logViewerForm.FormBorderStyle = FormBorderStyle.None;
            logViewerForm.Dock = DockStyle.Fill;
            panelLogs.Controls.Add(logViewerForm);

            tabLogs.Controls.Add(panelLogs);
            tabLogs.Controls.Add(storagePanel);
            tabControl1.TabPages.Add(tabLogs);
            logViewerForm.Show();

            LoadLogStorageSettingsUi();

            var canEditStorage = _userContext.IsAdmin;
            grpLogStorage.Enabled = canEditStorage;
            btnSaveLogStorage.Enabled = canEditStorage;
            btnSaveLogStorage.Visible = canEditStorage;
        }

        private void LoadLogStorageSettingsUi()
        {
            if (_logStorageSettings.IsServer)
                rbLogStorageServer.Checked = true;
            else
                rbLogStorageLocal.Checked = true;

            UpdateLogStorageStatusLabel();
        }

        private void UpdateLogStorageStatusLabel()
        {
            lblLogStorageStatus.Text = _logStorageSettings.IsServer
                ? "მიმდინარე: სერვერზე შენახვა"
                : "მიმდინარე: ლოკალურად შენახვა";
        }

        private void BtnSaveLogStorage_Click(object sender, EventArgs e)
        {
            if (!_userContext.IsAdmin)
            {
                MessageBox.Show("ლოგების რეჟიმის შეცვლა მხოლოდ ადმინისტრატორისთვისაა.", "წვდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var target = rbLogStorageServer.Checked ? LogStorageTarget.Server : LogStorageTarget.Local;
            if (target == LogStorageTarget.Server && !_connectionChecker.CanConnectToServer())
            {
                MessageBox.Show("სერვერთან კავშირი არ არის. სერვერის რეჟიმის შენახვა ვერ მოხერხდება.",
                    "კავშირი", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadLogStorageSettingsUi();
                return;
            }

            try
            {
                _systemConfigService.SetLogStorageTarget(target);
                _logStorageSettings.SetTarget(target);
                UpdateLogStorageStatusLabel();
                logViewerForm.ApplyStorageModeUi();
                MessageBox.Show("ლოგების შენახვის რეჟიმი განახლდა.", "შენახვა",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadLogStorageSettingsUi();
            }
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


