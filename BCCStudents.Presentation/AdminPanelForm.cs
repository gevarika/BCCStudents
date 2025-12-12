using System;
using BCCStudents.Domain.Entities;
using System.Windows.Forms;
using BCCStudents.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Data;
using System.Drawing;
using System.Configuration;
using BCCStudents.Presentation.Properties;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.IO;
using BCCStudents.Domain.Entities;
using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Update;
using Microsoft.Office.Interop.Word;
using BCCStudents.Infrastructure.Services;

namespace BCCStudents.Presentation
{
    public partial class AdminPanelForm : Form
    {
        private readonly StudentService _studentService;
        private readonly CleanupService _cleanupService;
        private readonly UserService _userService;
        private readonly DocumentService _documentService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IGroupRepository _groupRepository;
        private readonly DatabaseHelper _dbHelper;
        private readonly IConnectionStatusService _connectionStatusService;
        private readonly BackupManager _backupManager;
        private readonly AdminCodeManager _adminCodeManager;
        private DocumentConfig _config;
        private MainForm _mainForm;
        private TextBox txtAdminCode;
        private Button btnSetAdminCode;
        private TabPage tabLogs;
        private Panel panelLogs;
        private LogViewerForm logViewerForm;
        private System.Windows.Forms.CheckBox chkUseLocalDb;
        private System.Windows.Forms.CheckBox chkTestMode;
        private Button btnTestLocal;
        private Button btnTestServer;
        private Button btnSaveDbSettings;
        private Label lblLocalDb;
        private Label lblServerDb;
        private Label lblTestMode;
        private TextBox txtLocalConnectionString;
        private TextBox txtServerConnectionString;
        private Label lblLocalConnString;
        private Label lblServerConnString;
        private System.Windows.Forms.CheckBox chkAutoDownstream;
        private System.Windows.Forms.CheckBox chkAutoUpstream;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkUseFullBalance;
        private Button btnCheckUpdates;
        public AdminPanelForm(DatabaseHelper dbHelper, IGroupRepository groupRepository, StudentService studentService, CleanupService cleanupService, UserService userService, DocumentService documentService, IServiceProvider serviceProvider, IConnectionStatusService connectionStatusService, BackupManager backupManager, AdminCodeManager adminCodeManager)
        {
            InitializeComponent();
            _studentService = studentService;
            _cleanupService = cleanupService;
            _userService = userService;
            _documentService = documentService;
            _serviceProvider = serviceProvider;
            _groupRepository = groupRepository;
            _dbHelper = dbHelper;
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            _adminCodeManager = adminCodeManager ?? throw new ArgumentNullException(nameof(adminCodeManager));
            if (Settings.Default.IsTestDb)
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

            // DB mode controls - positioned below existing controls in groupBox1
            lblTestMode = new Label { Text = "ტესტ რეჟიმი:", AutoSize = true, Location = new System.Drawing.Point(20, 320) };
            chkTestMode = new System.Windows.Forms.CheckBox { Text = "ტესტ ბაზის გამოყენება", AutoSize = true, Location = new System.Drawing.Point(120, 320) };
            
            lblLocalDb = new Label { Text = "ლოკალური ბაზა:", AutoSize = true, Location = new System.Drawing.Point(20, 350) };
            chkUseLocalDb = new System.Windows.Forms.CheckBox { Text = "ლოკალური ბაზის გამოყენება", AutoSize = true, Location = new System.Drawing.Point(120, 350) };
            
            btnTestLocal = new Button { Text = "ტესტი", Location = new System.Drawing.Point(350, 380), Width = 80 };
            
            lblServerDb = new Label { Text = "სერვერის ბაზა:", AutoSize = true, Location = new System.Drawing.Point(20, 410) };
            btnTestServer = new Button { Text = "ტესტი", Location = new System.Drawing.Point(350, 440), Width = 80 };
            
            btnSaveDbSettings = new Button { Text = "შენახვა", Location = new System.Drawing.Point(20, 470), Width = 100 };
            
            // Connection string textboxes
            lblLocalConnString = new Label { Text = "ლოკალური კავშირი:", AutoSize = true, Location = new System.Drawing.Point(20, 500) };
            txtLocalConnectionString = new TextBox { Location = new System.Drawing.Point(20, 520), Width = 400, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };
            
            lblServerConnString = new Label { Text = "სერვერის კავშირი:", AutoSize = true, Location = new System.Drawing.Point(20, 590) };
            txtServerConnectionString = new TextBox { Location = new System.Drawing.Point(20, 610), Width = 400, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            // Set initial values from settings
            chkTestMode.Checked = Settings.Default.IsTestDb;
            chkUseLocalDb.Checked = Settings.Default.UseLocalDb;
            chkAutoDownstream = new System.Windows.Forms.CheckBox { Text = "Downstream ავტო-სინქი", AutoSize = true, Location = new System.Drawing.Point(20, 20) };
            chkAutoUpstream = new System.Windows.Forms.CheckBox { Text = "Upstream ავტო-სინქი", AutoSize = true, Location = new System.Drawing.Point(20, 50) };
            chkAutoUpdate = new System.Windows.Forms.CheckBox { Text = "ავტომატური განახლება", AutoSize = true, Location = new System.Drawing.Point(20, 80) };
            chkUseFullBalance = new System.Windows.Forms.CheckBox { Text = "სრული ბალანსის გამოყენება (რამდენ თვესაც ფარავს)", AutoSize = true, Location = new System.Drawing.Point(20, 110) };
            btnCheckUpdates = new Button { Text = "პროგრამის განახლება", AutoSize = true, Location = new System.Drawing.Point(20, 140) };
            chkAutoDownstream.Checked = Settings.Default.AutoDownstreamSyncEnabled;
            chkAutoUpstream.Checked = Settings.Default.AutoUpstreamSyncEnabled;
            chkAutoUpdate.Checked = Settings.Default.AutoUpdateEnabled;
            chkUseFullBalance.Checked = Settings.Default.UseFullBalanceForAutoPayment; // default = true
            
            // Load connection strings to fields
            LoadConnectionStringsToFields();
            
            // Load connection strings to textboxes
            LoadConnectionStringsToTextboxes();

            // Event handlers
            btnTestLocal.Click += (s, e) => TestLocalConnection();
            btnTestServer.Click += (s, e) => TestServerConnection();
            btnSaveDbSettings.Click += (s, e) => SaveDbSettings();
            chkTestMode.CheckedChanged += (s, e) => TestModeChanged();
            chkUseLocalDb.CheckedChanged += (s, e) => UseLocalChanged();
            chkAutoDownstream.CheckedChanged += (s, e) => { Settings.Default.AutoDownstreamSyncEnabled = chkAutoDownstream.Checked; Settings.Default.Save(); };
            chkAutoUpstream.CheckedChanged += (s, e) => { Settings.Default.AutoUpstreamSyncEnabled = chkAutoUpstream.Checked; Settings.Default.Save(); };
            chkAutoUpdate.CheckedChanged += (s, e) => { Settings.Default.AutoUpdateEnabled = chkAutoUpdate.Checked; Settings.Default.Save(); };
            chkUseFullBalance.CheckedChanged += (s, e) => { Settings.Default.UseFullBalanceForAutoPayment = chkUseFullBalance.Checked; Settings.Default.Save(); };

            // Add controls to groupBox1
            this.groupBox1.Controls.Add(lblTestMode);
            this.groupBox1.Controls.Add(chkTestMode);
            this.groupBox1.Controls.Add(lblLocalDb);
            this.groupBox1.Controls.Add(chkUseLocalDb);
            this.groupBox1.Controls.Add(btnTestLocal);
            this.groupBox1.Controls.Add(lblServerDb);
            this.groupBox1.Controls.Add(btnTestServer);
            this.groupBox1.Controls.Add(btnSaveDbSettings);
            this.groupBox1.Controls.Add(lblLocalConnString);
            this.groupBox1.Controls.Add(txtLocalConnectionString);
            this.groupBox1.Controls.Add(lblServerConnString);
            this.groupBox1.Controls.Add(txtServerConnectionString);
            this.sogBox2.Controls.Add(chkAutoDownstream);
            this.sogBox2.Controls.Add(chkAutoUpstream);
            this.sogBox2.Controls.Add(chkAutoUpdate);
            this.sogBox2.Controls.Add(chkUseFullBalance);
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
            try
            {
                _studentService.MigrateStudentGroups();
                MessageBox.Show("მონაცემები წარმატებით სინქრონიზებულია.", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა: " + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            if (!_connectionStatusService.IsConnected)
            {
                MessageBox.Show("ბაზასთან კავშირი ამჟამად მიუწვდომელია. ფორმა გაიხსნება მხოლოდ ნახვისთვის.", "კავშირი", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadConnectionSettings();
                _config = DocumentConfig.Load();

                txtDownloadFolder.Text = _config.DownloadPath;
                txtBaseUrl.Text = _config.FileUrl;

                txtSmsApiKey.Text = Settings.Default.SmsApiKey;
                txtSmsRegistration.Text = Settings.Default.SmsText_Registration;
                txtSmsPayment.Text = Settings.Default.SmsText_Payment;
                txtSmsUpcoming.Text = Settings.Default.SmsText_UpcomingReminder;
                txtSmsOverdue.Text = Settings.Default.SmsText_OverdueReminder;
                chkSmsEnabled.Checked = Settings.Default.SmsEnabled;
                if (chkSmsEnabled.Checked)
                    chkSmsEnabled.Text = "SMS სერვისი ჩართულია";
                else chkSmsEnabled.Text = "SMS სერვისი გამორთულია";
                return;
            }
            lblDbMode.Text = Settings.Default.IsTestDb ? "ტესტ რეჟიმი" : "სამუშაო რეჟიმი";
            lblDbMode.ForeColor = Settings.Default.IsTestDb ? Color.OrangeRed : Color.DarkGreen;


            LoadConnectionSettings();
            _config = DocumentConfig.Load();

            txtDownloadFolder.Text = _config.DownloadPath;
            txtBaseUrl.Text = _config.FileUrl;

            txtSmsApiKey.Text = Settings.Default.SmsApiKey;
            txtSmsRegistration.Text = Settings.Default.SmsText_Registration;
            txtSmsPayment.Text = Settings.Default.SmsText_Payment;
            txtSmsUpcoming.Text = Settings.Default.SmsText_UpcomingReminder;
            txtSmsOverdue.Text = Settings.Default.SmsText_OverdueReminder;
            chkSmsEnabled.Checked = Settings.Default.SmsEnabled;
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
            lblusersInfo.Text = "სულ - "+users.Count + " რეგისტრირებული მომხმარებელი";
        }
        private void btnSetStudyStartDate_Click(object sender, EventArgs e)
        {
            var setStudyStartDateForm = _serviceProvider.GetRequiredService<SetStudyStartDateForm>();
            // მიიღე საჭირო რეპოზიტორიები (DI-დან ან ხელით)
            if (setStudyStartDateForm.ShowDialog() == DialogResult.OK)
            {
                DateTime selectedDate = setStudyStartDateForm.SelectedDate.Date;
                var studyStartManager = _serviceProvider.GetRequiredService<StudyStartDateManager>();
                StudyStartDateManager.SaveStudyStartDate(selectedDate, studyStartManager);
                if (_mainForm != null)
                {
                    _mainForm.UpdateStudyStartDateLabel();
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
            Settings.Default.SmsText_Registration = txtSmsRegistration.Text.Trim();

            Settings.Default.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void btnSavePaySmsTexts_Click(object sender, EventArgs e)
        {
            Settings.Default.SmsText_Payment = txtSmsPayment.Text.Trim();

            Settings.Default.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void btnSaveUpcPaySmsTexts_Click(object sender, EventArgs e)
        {
            Settings.Default.SmsText_UpcomingReminder = txtSmsUpcoming.Text.Trim();

            Settings.Default.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void btnSaveOverSmsTexts_Click(object sender, EventArgs e)
        {
            Settings.Default.SmsText_OverdueReminder = txtSmsOverdue.Text.Trim();

            Settings.Default.Save();
            MessageBox.Show("წარმატებით შეინახა", "დადასტურება");
        }

        private void chkSmsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSmsEnabled.Checked)
            { chkSmsEnabled.Text = "SMS სერვისი ჩართულია"; Settings.Default.SmsEnabled = true; Settings.Default.Save(); }
            else
            { chkSmsEnabled.Text = "SMS სერვისი გამორთულია"; Settings.Default.SmsEnabled = false; Settings.Default.Save(); }
        }
        public void LoadConnectionSettings()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySQLConnection"]?.ConnectionString;

            if (!string.IsNullOrWhiteSpace(connStr))
            {
                // Regex-ით ამოვიღოთ ძირითადი ველები
                txtServer.Text = GetValue(connStr, "Server");
                txtPort.Text = GetValue(connStr, "Port");
                txtDatabase.Text = GetValue(connStr, "Database");
                txtUsername.Text = GetValue(connStr, "User");
                txtPassword.Text = GetValue(connStr, "Password");
            }

            lblConnectionStatus.Text = _connectionStatusService.IsConnected ? "✅ კავშირი დამყარებულია" : "❌ კავშირი არ არის";
            lblConnectionStatus.ForeColor = _connectionStatusService.IsConnected ? Color.Green : Color.Red;
        }

        private void LoadConnectionStringsToFields()
        {
            try
            {
                bool isTest = Settings.Default.IsTestDb;
                bool useLocal = Settings.Default.UseLocalDb;

                if (isTest)
                {
                    // Load test connection strings
                    var localConn = Settings.Default.LocalMySqlConnectionString_Test;
                    var serverConn = Settings.Default.ServerMySqlConnectionString_Test;
                    
                    // Fallback to production if test strings are empty
                    if (string.IsNullOrWhiteSpace(localConn))
                        localConn = Settings.Default.LocalMySqlConnectionString;
                    if (string.IsNullOrWhiteSpace(serverConn))
                        serverConn = Settings.Default.ServerMySqlConnectionString;
                    
                    LoadConnectionStringToFields(localConn, serverConn);
                }
                else
                {
                    // Load production connection strings
                    var localConn = Settings.Default.LocalMySqlConnectionString;
                    var serverConn = Settings.Default.ServerMySqlConnectionString;
                    
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
                txtServer.Text = GetValue(localConn, "Server");
                txtPort.Text = "3306";
                txtDatabase.Text = GetValue(localConn, "Database");
                txtUsername.Text = GetValue(localConn, "User");
                txtPassword.Text = GetValue(localConn, "Password");
            }
            
            if (!string.IsNullOrWhiteSpace(serverConn))
            {
                // Server connection string will be loaded when needed
            }
        }
        
        private void LoadConnectionStringsToTextboxes()
        {
            try
            {
                bool isTest = Settings.Default.IsTestDb;
                
                if (isTest)
                {
                    txtLocalConnectionString.Text = Settings.Default.LocalMySqlConnectionString_Test ?? "";
                    txtServerConnectionString.Text = Settings.Default.ServerMySqlConnectionString_Test ?? "";
                }
                else
                {
                    txtLocalConnectionString.Text = Settings.Default.LocalMySqlConnectionString ?? "";
                    txtServerConnectionString.Text = Settings.Default.ServerMySqlConnectionString ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა კავშირის სტრინგების ჩატვირთვისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestLocalConnection()
        {
            try
            {
                string connectionString = $"Server={txtServer.Text.Trim()};Port={txtPort.Text.Trim()};Database={txtDatabase.Text.Trim()};User Id={txtUsername.Text.Trim()};Password={txtPassword.Text};AllowPublicKeyRetrieval=True;SslMode=Preferred;CharSet=utf8mb4;";
                
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show("ლოკალური ბაზასთან კავშირი წარმატებულია!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ლოკალური ბაზასთან კავშირის შეცდომა:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestServerConnection()
        {
            try
            {
                // For server connection, we'll use the existing connection string from settings
                string serverConn = Settings.Default.IsTestDb ? 
                    Settings.Default.ServerMySqlConnectionString_Test : 
                    Settings.Default.ServerMySqlConnectionString;
                
                if (string.IsNullOrWhiteSpace(serverConn))
                {
                    MessageBox.Show("სერვერის კავშირის სტრინგი არ არის დაყენებული!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                using (var connection = new MySqlConnection(serverConn))
                {
                    connection.Open();
                    MessageBox.Show("სერვერის ბაზასთან კავშირი წარმატებულია!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"სერვერის ბაზასთან კავშირის შეცდომა:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveDbSettings()
        {
            try
            {
                bool isTest = Settings.Default.IsTestDb;
                bool useLocal = Settings.Default.UseLocalDb;

                // Build local connection string from existing textboxes
                string localConn = $"Server={txtServer.Text.Trim()};Port={txtPort.Text.Trim()};Database={txtDatabase.Text.Trim()};User Id={txtUsername.Text.Trim()};Password={txtPassword.Text};AllowPublicKeyRetrieval=True;SslMode=Preferred;CharSet=utf8mb4;";
                
                // Save connection strings from textboxes
                if (isTest)
                {
                    Settings.Default.LocalMySqlConnectionString_Test = localConn;
                    Settings.Default.ServerMySqlConnectionString_Test = txtServerConnectionString.Text.Trim();
                }
                else
                {
                    Settings.Default.LocalMySqlConnectionString = localConn;
                    Settings.Default.ServerMySqlConnectionString = txtServerConnectionString.Text.Trim();
                }

                // Save settings
                Settings.Default.Save();
                
                MessageBox.Show("ბაზის პარამეტრები წარმატებით შენახულია!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Reload settings to reflect changes
                LoadConnectionStringsToFields();
                LoadConnectionStringsToTextboxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა ბაზის პარამეტრების შენახვისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestModeChanged()
        {
            try
            {
                Settings.Default.IsTestDb = chkTestMode.Checked;
                Settings.Default.Save();
                
                // Reload connection settings to reflect the new mode
                LoadConnectionStringsToFields();
                LoadConnectionStringsToTextboxes();
                
                MessageBox.Show($"ტესტ რეჟიმი {(chkTestMode.Checked ? "ჩართულია" : "გამორთულია")}", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა ტესტ რეჟიმის შეცვლისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UseLocalChanged()
        {
            try
            {
                Settings.Default.UseLocalDb = chkUseLocalDb.Checked;
                Settings.Default.Save();
                
                // Save local connection string from textbox
                bool isTest = Settings.Default.IsTestDb;
                if (isTest)
                {
                    Settings.Default.LocalMySqlConnectionString_Test = txtLocalConnectionString.Text.Trim();
                }
                else
                {
                    Settings.Default.LocalMySqlConnectionString = txtLocalConnectionString.Text.Trim();
                }
                Settings.Default.Save();
                
                MessageBox.Show($"ლოკალური ბაზის გამოყენება {(chkUseLocalDb.Checked ? "ჩართულია" : "გამორთულია")}", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა ლოკალური ბაზის გამოყენების შეცვლისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ეხმარება კონკრეტული Key=Value წყვილის გამოტანაში
        private string GetValue(string connStr, string key)
        {
            var match = Regex.Match(connStr, $@"{key}\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : "";
        }
        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string server = txtServer.Text.Trim();
            string port = txtPort.Text.Trim();
            string database = txtDatabase.Text.Trim();
            string user = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            string connStr = $"Server={server};Port={port};Database={database};User={user};Password={password};SslMode=Preferred;";

            using (var conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    lblConnectionStatus.Text = "✅ კავშირი წარმატებით დამყარდა";
                    lblConnectionStatus.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    lblConnectionStatus.Text = "❌ კავშირის შეცდომა";
                    lblConnectionStatus.ForeColor = Color.Red;
                    MessageBox.Show("შეცდომა კავშირის დროს:\n" + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnSaveConnection_Click(object sender, EventArgs e)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var section = config.ConnectionStrings;

                string connStr = $"Server={txtServer.Text.Trim()};Port={txtPort.Text.Trim()};" +
                                 $"Database={txtDatabase.Text.Trim()};User={txtUsername.Text.Trim()};" +
                                 $"Password={txtPassword.Text.Trim()};SslMode=Preferred;";

                if (section.ConnectionStrings["MySQLConnection"] != null)
                {
                    section.ConnectionStrings["MySQLConnection"].ConnectionString = connStr;
                }
                else
                {
                    section.ConnectionStrings.Add(new ConnectionStringSettings
                    {
                        Name = "MySQLConnection",
                        ConnectionString = connStr,
                        ProviderName = "MySql.Data.MySqlClient"
                    });
                }

                await System.Threading.Tasks.Task.Run(() => config.Save(ConfigurationSaveMode.Modified));
                ConfigurationManager.RefreshSection("connectionStrings");

                MessageBox.Show("კავშირის პარამეტრები შენახულია", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა შენახვისას:\n" + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddStudentToGroup_Click(object sender, EventArgs e)
        {

        }

        private void btnSwitchToTestDB_Click(object sender, EventArgs e)
        {
            // შეცვალე სატესტო/სამუშაო ბაზის რეჟიმი
            Settings.Default.IsTestDb = !Settings.Default.IsTestDb;
            Settings.Default.Save();

            // ვიზუალური ინდიკატორის განახლება (სურვილისამებრ)
            if (lblDbMode != null)
            {
                lblDbMode.Text = Settings.Default.IsTestDb ? "ტესტ რეჟიმი" : "სამუშაო რეჟიმი";
                lblDbMode.ForeColor = Settings.Default.IsTestDb ? Color.OrangeRed : Color.DarkGreen;
            }

            // შეტყობინება
            MessageBox.Show(Settings.Default.IsTestDb
                ? "✅ გადაერთე სატესტო ბაზაზე!"
                : "✅ გადაერთე სამუშაო ბაზაზე!");

            // სურვილისამებრ, თუ გინდა პირდაპირ ახალ კავშირებს გამოჩნდეს ეფექტი, რესტარტი ყველაზე მარტივია
            System.Windows.Forms.Application.Restart();
        }

        private void btnSaveApiKey_Click_1(object sender, EventArgs e)
        {
            Settings.Default.SmsApiKey = txtSmsApiKey.Text.Trim();
            Settings.Default.Save();

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
            if (UserSession.Role == "Administrator")
            {
                var registerForm = _serviceProvider.GetRequiredService<RegisterForm>();
                if (registerForm.ShowDialog() == DialogResult.OK)
                    this.Close();
            }
            else
                MessageBox.Show("თქვენ არ გაქვთ ამ ფუნქციის გამოყენების უფლება!", "არასწორი მომხმარებელი", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                // კონფიგურაციის ჩატვირთვა
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                
                // ახალი მნიშვნელობების დაყენება
                config.AppSettings.Settings["AutoFileDetection.Enabled"].Value = chkAutoDetectionEnabled.Checked.ToString();
                config.AppSettings.Settings["AutoFileDetection.WatchFolderPath"].Value = txtWatchDirectory.Text.Trim();
                config.AppSettings.Settings["AutoFileDetection.FileNamePattern"].Value = txtFileNamePattern.Text.Trim();
                
                // კონფიგურაციის შენახვა
                config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                
                MessageBox.Show("კონფიგურაცია წარმატებით შეინახა!", "შენახვა", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა კონფიგურაციის შენახვისას: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAutoDetectionSettings()
        {
            try
            {
                chkAutoDetectionEnabled.Checked = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["AutoFileDetection.Enabled"] ?? "true");
                txtWatchDirectory.Text = System.Configuration.ConfigurationManager.AppSettings["AutoFileDetection.WatchFolderPath"] ?? "";
                txtFileNamePattern.Text = System.Configuration.ConfigurationManager.AppSettings["AutoFileDetection.FileNamePattern"] ?? "*.xlsx";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა კონფიგურაციის ჩატვირთვისას: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


