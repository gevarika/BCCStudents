using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Presentation.Data.Configuration;
using BCCStudents.Presentation.Properties;
using BCCStudents.Presentation.Services;
using System.Configuration;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class MainForm : Form
    {
        private readonly ISystemConfigurationService _systemConfigService;
        private readonly IStudentService _studentService;
        private readonly IPaymentDateService _paymentDateService;
        private readonly IPaymentService _paymentService;
        private readonly ILoggerRepository _loggerRepository;
        private readonly IStatisticsService _statisticService;
        //private readonly IServiceProvider _serviceProvider;
        private readonly AutoFileDetectionManager _autoDetectionManager;
        private readonly IDownStreamSyncService _downStreamSyncService;
        private readonly IDownStreamSyncManager _downStreamSyncManager;
        private readonly IUpStreamSyncManager _upStreamSyncManager;
        private readonly IConnectionMonitor _connectionMonitor;
        private readonly IBackupService _backupService;
        private readonly IConnectionStatusService _connectionStatusService;
        private readonly IApplicationStatus _appStatus;
        private bool _previousConnectionStatus; // წინა კავშირის სტატუსი MessageBox-ებისთვის
        private readonly IConfigurationService _configurationService;
        private readonly IUserContext _userContext;

        // სვეტების ხილულობის კონფიგურაცია (სვეტის სახელი -> Visible)
        private Dictionary<string, bool> columnVisibility;

        // სვეტების ზომების კონფიგურაცია (სვეტის სახელი -> Width)
        private Dictionary<string, int> columnWidths;
        private readonly IPaymentDescriptionAnalyzer _paymentDescriptionAnalyzer;
        private readonly StudentManagementFormFactory _studentFormFactory;
        private readonly GroupManFormFactory _groupManFormFactory;
        private readonly GroupsEditFormFactory _groupsEditFormFactory;
        private readonly StatisticsFormFactory _statisticsFormFactory;
        private readonly SetStudyStartDateFormFactory _setStudyStartDateFormFactory;
        private readonly FinanceFormFactory _financeFormFactory;
        private readonly PaymentFormFactory _paymentFormFactory;
        private readonly BackupManagementFormFactory _backupManagementFormFactory;
        private readonly LogViewerFormFactory _logViewerFormFactory;
        private readonly PaymentTestFormFactory _paymentTestFormFactory;
        private SyncStatusControl _syncStatusControl;
        private readonly AdminPanelFormFactory _adminPanelFormFactory;
        private readonly UserManagementFormFactory _userManagementFormFactory;
        private readonly BalanceTransferFormFactory _balanceTransferFormFactory;
        private readonly PendingRegistrationMonitor _pendingRegistrationMonitor;
        private readonly Dictionary<Type, Form> _openSingletonForms = new();
        private bool _allowClose;
        public MainForm(IPaymentService paymentService,
            IStudentService studentService,
            //IServiceProvider serviceProvider,
            ILoggerRepository loggerRepository,
            IStatisticsService statisticsService,
            AutoFileDetectionManager autoDetectionManager,
            IDownStreamSyncService downStreamSyncService,
            IDownStreamSyncManager downStreamSyncManager,
            IUpStreamSyncManager upStreamSyncManager,
            IConnectionMonitor connectionMonitor,
            IBackupService backupService,
            IPaymentDescriptionAnalyzer paymentDescriptionAnalyzer,
            IConnectionStatusService connectionstatusservice,
            IConfigurationService configService,
            StudentManagementFormFactory studentFormFactory,
            GroupManFormFactory groupManFormFactory,
            GroupsEditFormFactory groupsEditFormFactory,
            SetStudyStartDateFormFactory studyStartDateFormFactory,
            StatisticsFormFactory statisticsFormFactory,
            FinanceFormFactory financeFormFactory,
            PaymentFormFactory paymentFormFactory,
            AdminPanelFormFactory adminPanelFormFactory,
            BackupManagementFormFactory backupManagementFormFactory,
            LogViewerFormFactory logViewerFormFactory,
            PaymentTestFormFactory paymentTestFormFactory,
            BalanceTransferFormFactory balanceTransferFormFactory,
            UserManagementFormFactory userManagementFormFactory,
            PendingRegistrationMonitor pendingRegistrationMonitor,
            IApplicationStatus appStatus,
            ISystemConfigurationService systemConfigService,
            IPaymentDateService paymentDateService,
            IUserContext userContext
            )
        {
            InitializeComponent();

            //_connectionService = connectionService;
            _loggerRepository = loggerRepository;
            _statisticService = statisticsService;
            //_serviceProvider = serviceProvider;
            _paymentService = paymentService;
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _autoDetectionManager = autoDetectionManager;
            _downStreamSyncService = downStreamSyncService ?? throw new ArgumentNullException(nameof(downStreamSyncService));
            _downStreamSyncManager = downStreamSyncManager ?? throw new ArgumentNullException(nameof(downStreamSyncManager));
            _upStreamSyncManager = upStreamSyncManager ?? throw new ArgumentNullException(nameof(upStreamSyncManager));
            _connectionMonitor = connectionMonitor ?? throw new ArgumentNullException(nameof(connectionMonitor));
            _backupService = backupService ?? throw new ArgumentNullException(nameof(backupService));
            _connectionStatusService = connectionstatusservice ?? throw new ArgumentNullException(nameof(connectionstatusservice));
            _previousConnectionStatus = _connectionStatusService?.IsConnected ?? false; // საწყისი სტატუსი
            _setStudyStartDateFormFactory = studyStartDateFormFactory ?? throw new ArgumentNullException(nameof(studyStartDateFormFactory));
            _studentFormFactory = studentFormFactory; // ინიციალიზაცია
            _statisticsFormFactory = statisticsFormFactory;
            _adminPanelFormFactory = adminPanelFormFactory;
            _backupManagementFormFactory = backupManagementFormFactory;
            _logViewerFormFactory = logViewerFormFactory;
            _paymentTestFormFactory = paymentTestFormFactory;
            _paymentFormFactory = paymentFormFactory;
            _configurationService = configService;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _groupManFormFactory = groupManFormFactory;
            _groupsEditFormFactory = groupsEditFormFactory;
            _financeFormFactory = financeFormFactory;
            _userManagementFormFactory = userManagementFormFactory;
            _balanceTransferFormFactory = balanceTransferFormFactory ?? throw new ArgumentNullException(nameof(balanceTransferFormFactory));
            _pendingRegistrationMonitor = pendingRegistrationMonitor ?? throw new ArgumentNullException(nameof(pendingRegistrationMonitor));
            _pendingRegistrationMonitor.SetInvokeControl(this);
            _appStatus = appStatus ?? throw new ArgumentNullException(nameof(appStatus));
            _systemConfigService = systemConfigService ?? throw new ArgumentNullException(nameof(systemConfigService));
            _paymentDateService = paymentDateService ?? throw new ArgumentNullException(nameof(paymentDateService));
            if (configService.IsTestDb)
                FormTitleHelper.SetTitle(this, "სტუდენტების მართვა - საცდელი ბაზა");
            else
                FormTitleHelper.SetTitle(this, "სტუდენტების მართვა");

            // გამოვიწეროთ სტატუსის ცვლილება
            _connectionMonitor.ConnectionStatusChanged += (s, isConnected) =>
            {
                UpdateConnectionUI(isConnected);
            };

            // სვეტების კონფიგურაციის ჩატვირთვა SettingsHelper-იდან
            LoadColumnSettings();

            SetupDataGridView();
            SetupSyncStatus();

            // DataGridView სვეტების ზომის ცვლილების ივენთი - შენახვისთვის
            dgvPayments.ColumnWidthChanged += DgvPayments_ColumnWidthChanged;
        }
        #region  =========== Delegates ===========

        // Delegate, რომელიც იღებს პარამეტრებს (თუ საჭიროა) და აბრუნებს StudentManagementForm-ს.
        public delegate StudentManagementForm StudentManagementFormFactory();
        public delegate GroupManagementForm GroupManFormFactory();
        public delegate GroupsEdit GroupsEditFormFactory();
        // SetStudyStartDateFormFactory გადატანილია namespace-ში (StudentManagementForm.cs-ში)
        public delegate StatisticsForm StatisticsFormFactory();
        public delegate FinanceManagementForm FinanceFormFactory();
        public delegate PaymentForm PaymentFormFactory();
        public delegate AdminPanelForm AdminPanelFormFactory();
        public delegate BackupManagementForm BackupManagementFormFactory();
        public delegate LogViewerForm LogViewerFormFactory();
        public delegate PaymentTestForm PaymentTestFormFactory();
        public delegate UserManagementForm UserManagementFormFactory();
        public delegate BalanceTransferForm BalanceTransferFormFactory();
        #endregion
        private void SetupDataGridView()
        {
            // სვეტების გასუფთავება, თუ უკვე არსებობს
            dgvPayments.Columns.Clear();

            dgvPayments.ColumnCount = 9;
            dgvPayments.Columns[0].Name = "StudentCode";
            dgvPayments.Columns[1].Name = "StudentID";
            dgvPayments.Columns[2].Name = "FirstName";
            dgvPayments.Columns[3].Name = "LastName";
            dgvPayments.Columns[4].Name = "GroupName";
            dgvPayments.Columns[5].Name = "TuitionFee";
            dgvPayments.Columns[6].Name = "TotalPaid";
            dgvPayments.Columns[7].Name = "AmountDue";
            dgvPayments.Columns[8].Name = "NextPaymentDate";

            // ფერების განსაზღვრა, რომ ვიხილოთ სწორი ფერები
            dgvPayments.EnableHeadersVisualStyles = false;

            // სვეტების ავტომატური განსაზღვრა
            dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // სვეტების ხილულობისა და ზომების დაყენება
            Dictionary<string, int> defaultWidths = new Dictionary<string, int>
            {
                { "StudentCode", 80 },
                { "StudentID", 120 },
                { "FirstName", 120 },
                { "LastName", 150 },
                { "GroupName", 100 },
                { "TuitionFee", 100 },
                { "TotalPaid", 100 },
                { "AmountDue", 100 },
                { "NextPaymentDate", 120 }
            };

            // სვეტების ხილულობისა და ზომების განახლება
            foreach (DataGridViewColumn col in dgvPayments.Columns)
            {
                // ხილულობა
                if (columnVisibility != null && columnVisibility.ContainsKey(col.Name))
                {
                    col.Visible = columnVisibility[col.Name];
                }
                else
                {
                    col.Visible = true; // ნაგულისხმევად ყველა სვეტი ჩანს
                }

                // ზომა
                if (columnWidths != null && columnWidths.ContainsKey(col.Name))
                {
                    col.Width = columnWidths[col.Name];
                }
                else if (defaultWidths.ContainsKey(col.Name))
                {
                    col.Width = defaultWidths[col.Name];
                }
            }

            // სვეტების მეტად განსაზღვრა
            dgvPayments.VirtualMode = false;
        }


        /// <summary>
        /// Security Checks - ვიყენებთ IUserContext-ს უფლებების შემოწმებისთვის
        /// Control.Tag პატერნით იდენტიფიცირება (Tag = "Permission_CanManageStudents" ა.შ.)
        /// </summary>
        private void ApplySecurityChecks()
        {
            // Buttons
            if (btnStudents != null)
            {
                btnStudents.Enabled = _userContext.HasPermission(Permission.CanManageStudents);
                btnStudents.Tag = $"Permission_{Permission.CanManageStudents}";
            }

            if (btnGroups != null)
            {
                btnGroups.Enabled = _userContext.HasPermission(Permission.CanManageGroups);
                btnGroups.Tag = $"Permission_{Permission.CanManageGroups}";
            }

            if (btnGroupsEdit != null)
            {
                btnGroupsEdit.Enabled = _userContext.HasPermission(Permission.CanManageGroups);
                btnGroupsEdit.Tag = $"Permission_{Permission.CanManageGroups}";
            }

            if (btnRefreshPaymentProcess != null)
            {
                btnRefreshPaymentProcess.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
                btnRefreshPaymentProcess.Tag = $"Permission_{Permission.CanManagePayments}";
            }

            if (btnPaymentHistory != null)
            {
                btnPaymentHistory.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
                btnPaymentHistory.Tag = $"Permission_{Permission.CanManagePayments}";
            }

            // Menu Items
            if (StatisticToolStripMenuItem != null)
            {
                StatisticToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanViewReports);
                StatisticToolStripMenuItem.Tag = $"Permission_{Permission.CanViewReports}";
            }

            if (PaymentsToolStripMenuItem != null)
            {
                PaymentsToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
                PaymentsToolStripMenuItem.Tag = $"Permission_{Permission.CanManagePayments}";
            }

            if (PaymentToolStripMenuItem != null)
            {
                PaymentToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
                PaymentToolStripMenuItem.Tag = $"Permission_{Permission.CanManagePayments}";
            }

            if (PaymentTestToolStripMenuItem != null)
            {
                PaymentTestToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
                PaymentTestToolStripMenuItem.Tag = $"Permission_{Permission.CanManagePayments}";
            }

            if (BackupToolStripMenuItem != null)
            {
                BackupToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanEditSettings);
                BackupToolStripMenuItem.Tag = $"Permission_{Permission.CanEditSettings}";
            }

            if (tsmAdminPanel != null)
            {
                tsmAdminPanel.Enabled = _userContext.IsAdmin;
            }

            if (LogsToolStripMenuItem != null)
            {
                LogsToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanViewReports);
                LogsToolStripMenuItem.Tag = $"Permission_{Permission.CanViewReports}";
            }

            if (userManagementToolStripMenuItem != null)
            {
                // ყველა ავტენტიფიცირებული მომხმარებელი შეძლებს წვდომას
                // UserManagementForm-ში თავად არის შეზღუდვები admin/regular user-ისთვის
                userManagementToolStripMenuItem.Enabled = _userContext.IsAuthenticated;
            }
        }

        private void btnStudents_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManageStudents))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // StudentManagementForm გამოძახება
            OpenOrActivateForm(_studentFormFactory.Invoke);
        }
        private void btnGroups_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManageGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // GroupManagementForm გამოძახება
            OpenOrActivateForm(_groupManFormFactory.Invoke);
        }

        private void btnGroupsEdit_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManageGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // GroupsEdit ფორმის გამოძახება
            OpenOrActivateForm(_groupsEditFormFactory.Invoke);
        }

        private void OpenOrActivateForm<TForm>(Func<TForm> formFactory) where TForm : Form
        {
            var formType = typeof(TForm);
            if (_openSingletonForms.TryGetValue(formType, out var existingForm))
            {
                if (existingForm != null && !existingForm.IsDisposed)
                {
                    if (existingForm.WindowState == FormWindowState.Minimized)
                    {
                        existingForm.WindowState = FormWindowState.Normal;
                    }

                    existingForm.BringToFront();
                    existingForm.Activate();
                    return;
                }

                _openSingletonForms.Remove(formType);
            }

            var newForm = formFactory();
            _openSingletonForms[formType] = newForm;
            newForm.FormClosed += (_, __) => _openSingletonForms.Remove(formType);
            newForm.Show();
            newForm.BringToFront();
            newForm.Activate();
        }

        //ფორმის ჩატვირთვისას და ავტომატური გადახდების გაშვება
        private async void MainForm_Load(object sender, EventArgs e)
        {
            _pendingRegistrationMonitor.Initialize();

            // Security Checks - უფლებების შემოწმება IUserContext-ის მეშვეობით
            ApplySecurityChecks();
            SetStudyStartDate();
            StudyStartDateLabel_Update(); // ფორმის ჩატვირთვისას დაწყების თარიღის განახლება
            PaymentNextDateLabel_Update(); // გადახდის თარიღის განახლება
            // ავტომატური ფაილების აღმოჩენის გაშვება (თუ ჩართულია)
            try
            {
                await _autoDetectionManager.CheckForNewFilesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა ავტომატური ფაილების აღმოჩენისას: {ex.Message}");
            }

            // პერიოდული ბექაპის ინიციალიზაცია
            try
            {
                // სისტემური პარამეტრების განახლება
                BackupConfig.UpdateBackupManagerSettings();

                _backupService.InitializePeriodicBackup();
                Console.WriteLine("პერიოდული ბექაპი წარმატებით ინიციალიზებულია");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა პერიოდული ბექაპის ინიციალიზაციისას: {ex.Message}");
            }
            StartDownStreamSync();
            // UpStream სინქრონიზაციის გაშვება (SyncOutbox-დან ჩანაწერების გაგზავნა სერვერზე)
            StartUpStreamSync();
            // ბაზასთან კავშირის მონიტორინგის გაშვება
            StartConnectionMonitoring();

            // გადახდების სიის ჩატვირთვა
            LoadUpcomingPayments();
        }

        /// <summary>
        /// სვეტების კონფიგურაციის ჩატვირთვა SettingsHelper-იდან
        /// </summary>
        private void LoadColumnSettings()
        {
            string formName = this.Name;
            string dgvName = "dgvPayments"; // DataGridView-ის სახელი

            // სვეტების ხილულობის ჩატვირთვა
            columnVisibility = SettingsHelper.LoadColumnVisibility(
                formName,
                dgvName,
                GetDefaultColumnVisibility()
            );

            // სვეტების ზომების ჩატვირთვა
            columnWidths = SettingsHelper.LoadColumnWidths(
                formName,
                dgvName,
                new Dictionary<string, int>()
            );
        }

        /// <summary>
        /// ნაგულისხმევი სვეტების ხილულობის კონფიგურაცია dgvPayments-ისთვის
        /// </summary>
        private Dictionary<string, bool> GetDefaultColumnVisibility()
        {
            return new Dictionary<string, bool>
            {
                { "StudentCode", true },
                { "StudentID", true },
                { "FirstName", true },
                { "LastName", true },
                { "GroupName", true },
                { "TuitionFee", true },
                { "TotalPaid", true },
                { "AmountDue", true },
                { "NextPaymentDate", true }
            };
        }

        /// <summary>
        /// სვეტების კონფიგურაციის შენახვა SettingsHelper-ში
        /// </summary>
        private void SaveColumnSettings()
        {
            string formName = this.Name;
            string dgvName = "dgvPayments"; // DataGridView-ის სახელი

            // სვეტების ხილულობის შენახვა
            SettingsHelper.SaveColumnVisibility(formName, dgvName, columnVisibility);

            // სვეტების ზომების შენახვა
            if (columnWidths != null && columnWidths.Count > 0)
            {
                SettingsHelper.SaveColumnWidths(formName, dgvName, columnWidths);
            }
        }

        /// <summary>
        /// DataGridView სვეტების ზომის ცვლილების დამუშავება
        /// </summary>
        private void DgvPayments_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (columnWidths == null)
            {
                columnWidths = new Dictionary<string, int>();
            }

            columnWidths[e.Column.Name] = e.Column.Width;

            // შენახვა
            SaveColumnSettings();
        }

        /// <summary>
        /// სვეტების მართვის დიალოგის გახსნა
        /// </summary>
        private void ManageColumns_Click(object sender, EventArgs e)
        {
            using (var dialog = new ColumnManagementDialog(columnVisibility))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.ResultColumnVisibility != null)
                {
                    // columnVisibility-ის განახლება
                    columnVisibility = dialog.ResultColumnVisibility;

                    // კონფიგურაციის შენახვა
                    SaveColumnSettings();

                    // DataGridView-ის განახლება
                    SetupDataGridView();
                    LoadUpcomingPayments(); // მონაცემების განახლება
                }
            }
        }


        /// <summary>
        /// MainForm-ის ჩატვირთვისას და ავტომატური გადახდების გაშვება გაშვება გამოძახება გამოძახება გადახდების სიის განახლება
        /// </summary>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // ავტომატური გადახდების გაშვება (თუ ჩართულია)
            var result = MessageBox.Show(
                "გსურთ გადახდების სიის განახლება?",
                "გადახდების სია",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                _ = RunAutoPaymentsWithProgressAsync();
            }
        }

        /// <summary>
        /// ბაზასთან კავშირის მონიტორინგის გაშვება
        /// </summary>
        private void StartConnectionMonitoring()
        {
            // ConnectionStatusService-ის ივენთის გამოწერა
            if (_connectionStatusService != null)
            {
                _connectionStatusService.ConnectionStatusChanged += ConnectionStatusService_ConnectionStatusChanged;
            }

            // მონიტორინგის გაშვება
            _connectionMonitor.StopMonitoring();

            // საწყისი კავშირის შემოწმება
            UpdateConnectionStatus();
        }

        /// <summary>
        /// ბაზასთან კავშირის სტატუსის განახლება UI-ში
        /// </summary>
        private void UpdateConnectionStatus()
        {
            if (_connectionStatusService == null)
                return;

            bool isConnected = _connectionStatusService.IsConnected;
            UpdateUIBasedOnConnectionStatus(isConnected);
        }

        /// <summary>
        /// UI-ის განახლება კავშირის სტატუსის მიხედვით
        /// </summary>
        /// <param name="isConnected">კავშირის სტატუსი</param>
        private void UpdateUIBasedOnConnectionStatus(bool isConnected)
        {
            if (statusLabel != null)
            {
                if (isConnected)
                {
                    statusLabel.Text = "✅ ბაზა დაკავშირებულია";
                    statusLabel.ForeColor = Color.Green;
                }
                else
                {
                    statusLabel.Text = "❌ ბაზა გათიშულია";
                    statusLabel.ForeColor = Color.Red;
                }
            }

            // DB-ზე დამოკიდებული UI ელემენტების განახლება
            // მაგალითად: ToolStripMenuItem-ების Enabled სტატუსი
            // TODO: დაამატე სხვა UI ელემენტები, რომლებიც დამოკიდებულია კავშირზე
        }
        private void UpdateConnectionUI(bool isConnected)
        {
            // თუ მეთოდი გამოძახებულია ფონური Thread-იდან, გადავიყვანოთ UI Thread-ზე
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateConnectionUI(isConnected)));
                return;
            }

            // მაგალითად, თუ გაქვთ StatusLabel სახელად lblStatus
            if (isConnected)
            {
                statusLabel.Text = "დაკავშირებულია";
                statusLabel.ForeColor = Color.Green;
                // თუ გაქვთ რაიმე პატარა წითელი/მწვანე წრე (Icon)
                //imgStatusIndicator.Image = Properties.Resources.green_circle;
            }
            else
            {
                statusLabel.Text = "კავშირი გაწყდა!";
                statusLabel.ForeColor = Color.Red;
                //imgStatusIndicator.Image = Properties.Resources.red_circle;
            }
        }
        /// <summary>
        /// ConnectionStatusService-ის ConnectionStatusChanged ივენთის handler
        /// </summary>
        private void ConnectionStatusService_ConnectionStatusChanged(object sender, EventArgs e)
        {
            // UI thread-ზე განახლება
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ConnectionStatusService_ConnectionStatusChanged(sender, e)));
                return;
            }

            bool isConnected = _connectionStatusService.IsConnected;
            UpdateUIBasedOnConnectionStatus(isConnected);
        }

        /// <summary>
        /// ბაზასთან კავშირის სტატუსის შეცვლის event handler
        /// </summary>
        private void ConnectionMonitorService_ConnectionStatusChanged(object sender, bool isConnected)
        {
            // UI thread-ზე განახლება
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ConnectionMonitorService_ConnectionStatusChanged(sender, isConnected)));
                return;
            }

            bool previousStatus = _previousConnectionStatus;
            _previousConnectionStatus = isConnected;

            if (isConnected)
            {
                // ბაზა აღდგენილია
                UpdateUIBasedOnConnectionStatus(true);

                // აღდგენის შეტყობინება გამოჩენა
                if (!previousStatus)
                {
                    MessageBox.Show(
                        "✅ ბაზა აღდგენილია!",
                        "ბაზა აღდგენილია",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else
            {
                // ბაზა გათიშულია
                UpdateUIBasedOnConnectionStatus(false);

                // მხოლოდ პირველად გათიშვისას გამოჩენა MessageBox
                if (previousStatus)
                {
                    MessageBox.Show(
                        "⚠️ ბაზა გათიშულია!\n\n" +
                        "გთხოვთ, შეამოწმოთ და აღადგინოთ ბაზასთან კავშირი.\n\n" +
                        "სისტემა ავტომატურად შეეცდება ბაზასთან დაკავშირებას 30 წუთში.",
                        "ბაზა გათიშულია",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        // ბატონი და სტატისტიკა გადახდების გაშვება ფორმა
        private async Task RunAutoPaymentsWithProgressAsync()
        {
            try
            {
                tsProgressBar.Minimum = 0;
                tsProgressBar.Value = 0;
                tsProgressBar.Maximum = 100;

                await Task.Run(async () =>
                {
                    // ვიღებთ მნიშვნელობას Settings-იდან
                    //var configService = _configurationService;
                    bool useFullBalance = Settings.Default.UseFullBalanceForAutoPayment; // TODO: Add to IConfigurationService
                    await _paymentService.ProcessAutoPaymentsForAllStudentsAsync((current, total) =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            if (total > 0)
                            {
                                tsProgressBar.Value = (int)((current / (double)total) * 100);
                                tsPaymentStatus.Text = $"დამუშავებულია {current} სტუდენტი {total}-დან";
                            }
                            else
                            {
                                tsProgressBar.Value = 0;
                            }
                        }));
                    }, useFullBalance);
                });


                tsProgressBar.Value = 100;
                await Task.Delay(500); // დროებითი დაყოვნება ასახვისთვის პროგრესი
                tsProgressBar.Value = 0;

                // გადახდების სიის განახლება
                this.Invoke((Action)(() => LoadUpcomingPayments()));
            }
            catch (Exception ex)
            {
                MessageBox.Show("ავტომატური გადახდების გაშვებისას მოხდა შეცდომა:\n" + ex.Message);
                // შეცდომის დაფიქსირება ლოგებში
            }
        }

        // 📅 დაწყების თარიღის განსაზღვრა
        private void SetStudyStartDate()
        {
            _connectionMonitor.StartMonitoring();
            _connectionMonitor.ConnectionStatusChanged += ConnectionMonitorService_ConnectionStatusChanged;

            // ვამოწმებთ თარიღების არსებობას
            bool hasStudyStartDate = _systemConfigService.GetStudyStartDate().HasValue;
            bool hasPaymentDate = _systemConfigService.GetDefaultPaymentDate().HasValue;
            var existingStudentsCount = _studentService.GetAllStudents().Count;

            // თუ ორივე თარიღი არ არის დაყენებული
            if (!hasStudyStartDate && !hasPaymentDate)
            {
                // 1. ჯერ სწავლის დაწყების თარიღის დაყენება
                if (!SetStudyStartDateIfNeeded(existingStudentsCount))
                {
                    return; // მომხმარებელმა გაუქმა
                }

                // 2. შემდეგ გადახდის თარიღის დაყენება
                SetDefaultPaymentDateIfNeeded(existingStudentsCount);
            }
            // თუ მხოლოდ სწავლის თარიღი არ არის
            else if (!hasStudyStartDate)
            {
                SetStudyStartDateIfNeeded(existingStudentsCount);
            }
            // თუ მხოლოდ გადახდის თარიღი არ არის
            else if (!hasPaymentDate)
            {
                SetDefaultPaymentDateIfNeeded(existingStudentsCount);
            }
        }

        /// <summary>
        /// სწავლის დაწყების თარიღის დაყენება (თუ საჭიროა)
        /// </summary>
        /// <param name="existingStudentsCount">არსებული მოსწავლეების რაოდენობა</param>
        /// <returns>true, თუ თარიღი დაყენებულია ან უკვე არსებობდა; false, თუ მომხმარებელმა გაუქმა</returns>
        private bool SetStudyStartDateIfNeeded(int existingStudentsCount)
        {
            if (_systemConfigService.GetStudyStartDate().HasValue)
            {
                return true; // უკვე დაყენებულია
            }

            // თუ მოსწავლეები არსებობს - გაფრთხილება
            if (existingStudentsCount > 0)
            {
                var warningResult = MessageBox.Show(
                    "მოსწავლეები უკვე დარეგისტრირებულია!\n\n" +
                    "თუ დაყენებთ სწავლის დაწყების თარიღს, შეგიძლიათ არჩევანი გაუკეთოთ:\n" +
                    "- Yes: თარიღის ცვლილება გამოიწვევს მოსწავლეებისთვის გადახდის თარიღის ცვლილებას\n" +
                    "- No: თარიღის ცვლილება მხოლოდ სისტემური კონფიგურაციისთვის (მოსწავლეებისთვის არ შეიცვლება)\n\n" +
                    "გსურთ განაგრძოთ?",
                    "გაფრთხილება",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (warningResult != DialogResult.Yes)
                {
                    return false; // მომხმარებელმა გაუქმა
                }
            }

            // დიალოგი სწავლის დაწყების თარიღისთვის
            var result = MessageBox.Show(
                "სწავლის დაწყების თარიღი არ არის განსაზღვრული.\nგსურთ განსაზღვროთ?",
                "სწავლის დაწყების თარიღი",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
            {
                MessageBox.Show("სწავლის დაწყების თარიღის განსაზღვრა გაუქმებულია.",
                    "გაუქმება",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                System.Windows.Forms.Application.Exit();
                return false;
            }

            // თარიღის არჩევის ფორმა
            using (var dateForm = _setStudyStartDateFormFactory.Invoke())
            {
                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    DateTime selectedDate = dateForm.SelectedDate.Date;
                    _systemConfigService.SetStudyStartDate(selectedDate);
                    StudyStartDateLabel_Update();

                    MessageBox.Show($"✅ სწავლის დაწყების თარიღი განსაზღვრულია: {selectedDate:dd-MM-yyyy}",
                        "დადასტურება",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return true;
                }
                else
                {
                    MessageBox.Show("სწავლის დაწყების თარიღის განსაზღვრა გაუქმებულია.",
                        "გაუქმება",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    System.Windows.Forms.Application.Exit();
                    return false;
                }
            }
        }

        /// <summary>
        /// ნაგულისხმევი გადახდის თარიღის დაყენება (თუ საჭიროა)
        /// </summary>
        /// <param name="existingStudentsCount">არსებული მოსწავლეების რაოდენობა</param>
        private void SetDefaultPaymentDateIfNeeded(int existingStudentsCount)
        {
            if (_systemConfigService.GetDefaultPaymentDate().HasValue)
            {
                return; // უკვე დაყენებულია
            }

            // დიალოგი გადახდის თარიღისთვის
            var result = MessageBox.Show(
                "გადახდის თარიღი არ არის განსაზღვრული.\nგსურთ განსაზღვროთ?",
                "გადახდის თარიღი",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
            {
                return; // მომხმარებელმა გაუქმა
            }

            // თარიღის არჩევის ფორმა
            using (var dateForm = _setStudyStartDateFormFactory.Invoke())
            {
                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    DateTime selectedDate = dateForm.SelectedDate.Date;
                    _systemConfigService.SetDefaultPaymentDate(selectedDate);
                    _paymentDateService.UpdateNextPaymentDate(selectedDate);
                    PaymentNextDateLabel_Update(); // ვაახლებთ label-ებს

                    MessageBox.Show($"✅ გადახდის თარიღი განსაზღვრულია: {selectedDate:dd-MM-yyyy}",
                        "დადასტურება",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }


        public void StudyStartDateLabel_Update()
        {
            var date = _systemConfigService.GetStudyStartDate();
            lbStartStudyDate.Text = date.HasValue
                ? $"{date.Value}"
                : "სტუდენტების მართვის დაწყების თარიღი არ არის განსაზღვრული";
            lbStartStudyDate.Refresh();
            //lblPaymentNextDate.Text = date.Value.AddMonths(1).ToString();
        }
        public void PaymentNextDateLabel_Update()
        {
            var date = _systemConfigService.GetDefaultPaymentDate();
            lblPaymentNextDate.Text = date.HasValue
                ? $"{date.Value}"
                : "გადახდის თარიღი არ არის განსაზღვრული";
            lblPaymentNextDate.Refresh();
        }
        private void StatisticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanViewReports))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_statisticsFormFactory.Invoke);
        }
        private void PaymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_financeFormFactory.Invoke);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _downStreamSyncManager.Stop();

            // ბაზასთან კავშირის მონიტორინგის გაჩერება
            if (_connectionMonitor != null)
            {
                _connectionMonitor.StopMonitoring();
                _connectionMonitor.ConnectionStatusChanged -= ConnectionMonitorService_ConnectionStatusChanged;
            }

            // ConnectionStatusService-ის ივენთის გამოწერა
            if (_connectionStatusService != null)
            {
                _connectionStatusService.ConnectionStatusChanged -= ConnectionStatusService_ConnectionStatusChanged;
            }

            if (_allowClose)
            {
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MySQLConnection"]?.ConnectionString;
            try
            {
                _loggerRepository.WriteLog("Program Exit", "Success", "The application has been closed.", Environment.UserName);
                if (_backupService.DbChangedSinceLastBackup)
                {
                    var userBackupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "Backups");
                    Directory.CreateDirectory(userBackupDir);
                    _backupService.CreateBackup(userBackupDir);
                }
            }
            catch (Exception ex)
            {
                // ლოგების შეცდომა შეიძლება, Program Files-ში ლიმიტირებულია წვდომა
                Console.WriteLine($"Exit log/backup error: {ex.Message}");
            }

            try
            {
                var userBackupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BCCStudents", "Backups");
                Directory.CreateDirectory(userBackupDir);
                string backupFilePath = Path.Combine(userBackupDir, $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql");
                bool backupSuccess = _backupService.CreateMySQLBackup(backupFilePath);
                if (!backupSuccess)
                {
                    Console.WriteLine("ბექაპის შექმნა ვერ მოხერხდა, რადგან მიმდინარე მონაცემები არ არსებობს ან შექმნა გაუქმებულია დროებით");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ბექაპის შექმნისას მოხდა შეცდომა: {ex.Message}");
            }

            // პერიოდული ბექაპის გაჩერება
            try
            {
                BackupConfig.SaveBackupManagerSettings();

                _backupService.StopPeriodicBackup();
                Console.WriteLine("პერიოდული ბექაპი გაჩერებულია");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა პერიოდული ბექაპის გაჩერებისას: {ex.Message}");
            }


            // ტაიმერის გაჩერება
            if (timer1 != null)
            {
                timer1.Stop();
                timer1.Dispose();
            }
        }

        /// <summary>
        /// ინიციალიზაცია DownStream სინქრონიზაციის (სერვერიდან მონაცემების) და გაშვება პერიოდული სინქრონიზაცია.
        /// </summary>
        private void StartDownStreamSync()
        {
            _downStreamSyncManager.Start();
        }

        /// <summary>
        /// ინიციალიზაცია UpStream სინქრონიზაციის (ლოკალური ცვლილებების სერვერზე გაგზავნა) და გაშვება პერიოდული სინქრონიზაცია.
        /// </summary>
        private void StartUpStreamSync()
        {
            if (_appStatus.IsAuthenticated)
            {
                _upStreamSyncManager.Start();
            }
        }

        /// <summary>
        /// ასახვა სინქრონიზაციის სტატუსის statusStrip-ში
        /// </summary>
        private void SetupSyncStatus()
        {
            _syncStatusControl = new SyncStatusControl();
            // ვაყენებთ Control-ს Invoke-ისთვის
            _syncStatusControl.SetInvokeControl(statusStrip1);
            var statusItems = _syncStatusControl.GetStatusItems();

            // ვამატებთ statusStrip-ში
            foreach (var item in statusItems)
            {
                statusStrip1.Items.Add(item);
            }

            // Event handlers-ის დამატება
            _downStreamSyncManager.SyncCompleted += (sender, args) =>
            {
                _syncStatusControl.UpdateDownStreamStatus(args);
                RefreshUiAfterDownStreamSync(args);
            };

            _upStreamSyncManager.SyncCompleted += (sender, args) =>
            {
                _syncStatusControl.UpdateUpStreamStatus(args);
            };
        }

        private void RefreshUiAfterDownStreamSync(SyncStatusEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => RefreshUiAfterDownStreamSync(args)));
                return;
            }

            if (args.Success && args.RecordsSynced > 0)
            {
                LoadUpcomingPayments();
            }

            _pendingRegistrationMonitor.HandleSyncCompleted(args);
        }

        private void PaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var paymentForm = _paymentFormFactory.Invoke();
            paymentForm.ShowDialog();
            LoadUpcomingPayments(); // გადახდების სიის განახლება
            // აქ შეიძლება დამატებითი ლოგიკა იყოს საჭირო
        }

        /// <summary>
        /// გადახდების სიის ჩატვირთვა dgvPayments-ში
        /// </summary>
        private void LoadUpcomingPayments()
        {
            try
            {
                // UI განახლებისთვის შეჩერება სვეტების ავტომატური განსაზღვრა
                dgvPayments.SuspendLayout();

                //var paymentRepo = _serviceProvider.GetRequiredService<IPaymentRepository>();
                var payments = _paymentService.GetPendingPayments();
                dgvPayments.Rows.Clear();

                var paymentStartDate = _systemConfigService.GetDefaultPaymentDate();

                // დალაგება გადახდების სიის
                var sortedPayments = payments
                    .OrderByDescending(p => p.AmountDue > 0)  // უფრო მაღალი გადახდილი გადახდები პირველად
                    .ThenBy(p => p.NextPaymentDate ?? paymentStartDate)  // შემდეგ შემდეგი თარიღის მიხედვით
                    .ToList();
                int paid = 0; int unpaid = 0; int totalPaid = 0; int overdue = 0; int upcomming = 0;
                foreach (var payment in sortedPayments)
                {
                    int rowIndex = dgvPayments.Rows.Add();
                    DataGridViewRow row = dgvPayments.Rows[rowIndex];

                    row.Cells["StudentCode"].Value = payment.StudentCode;
                    row.Cells["StudentID"].Value = payment.StudentID;
                    row.Cells["FirstName"].Value = payment.FirstName;
                    row.Cells["LastName"].Value = payment.LastName;
                    row.Cells["GroupName"].Value = payment.GroupName;
                    row.Cells["TuitionFee"].Value = payment.TuitionFee;
                    row.Cells["TotalPaid"].Value = payment.TotalPaid;
                    row.Cells["AmountDue"].Value = payment.AmountDue;
                    row.Cells["NextPaymentDate"].Value = payment.NextPaymentDate?.ToString("yyyy-MM-dd") ?? "";

                    // თარიღის განსაზღვრა დარჩენილი დღეების გამოსათვლელად
                    DateTime nextPaymentDate = payment.NextPaymentDate ?? paymentStartDate ?? DateTime.Today.AddMonths(1);
                    TimeSpan remainingDays = nextPaymentDate.Date - DateTime.Today;

                    // ვამოწმებთ AmountDue-ს (TuitionFee - TotalPaid)
                    // გამოყენებულია payment.AmountDue property, რომელიც ავტომატურად გამოითვლება
                    decimal amountDue = payment.AmountDue;

                    // დავადგინოთ გადახდილია თუ არა: თუ TotalPaid >= TuitionFee, მაშინ გადახდილია
                    bool isPaid = payment.TotalPaid >= payment.TuitionFee || amountDue <= 0;

                    // ვქმნით ახალ Style-ს, რომ ვიხილოთ სწორი ფერები
                    DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
                    // გადახდილი და გადაუხდელი სტუდენტების განსაზღვრა

                    if (isPaid)
                    {
                        // გადახდილია - მწვანე
                        cellStyle.BackColor = Color.LightGreen;
                        cellStyle.ForeColor = Color.Black;
                        cellStyle.SelectionBackColor = Color.Green;
                        cellStyle.SelectionForeColor = Color.White;
                        paid++;
                    }
                    else if (remainingDays.TotalDays <= 7 && remainingDays.TotalDays > 0)
                    {
                        // მომდინარე - ყვითელი
                        cellStyle.BackColor = Color.LightYellow;
                        cellStyle.ForeColor = Color.Black;
                        cellStyle.SelectionBackColor = Color.Orange;
                        cellStyle.SelectionForeColor = Color.White;
                        upcomming++;
                    }
                    else if (remainingDays.TotalDays <= 0)
                    {
                        // გადაუხდელია - წითელი
                        cellStyle.BackColor = Color.LightCoral;
                        cellStyle.ForeColor = Color.Black;
                        cellStyle.SelectionBackColor = Color.Red;
                        cellStyle.SelectionForeColor = Color.White;
                        overdue++;
                    }
                    else
                    {
                        // ნორმალური - თეთრი
                        cellStyle.BackColor = Color.White;
                        cellStyle.ForeColor = Color.Black;
                        cellStyle.SelectionBackColor = Color.LightBlue;
                        cellStyle.SelectionForeColor = Color.Black;
                    }

                    // ვამოწმებთ, რომ style სწორად გამოითვლება
                    row.DefaultCellStyle = cellStyle;

                    // დავადგინოთ ყველა cell-ისთვის: თითოეული cell-ისთვის განსაზღვრა
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = cellStyle.BackColor;
                        cell.Style.ForeColor = cellStyle.ForeColor;
                        cell.Style.SelectionBackColor = cellStyle.SelectionBackColor;
                        cell.Style.SelectionForeColor = cellStyle.SelectionForeColor;
                    }
                }
                StudentAmount.Text = @"გადახდილი:-" + paid + " გადაუხდელი:-" + overdue + " მომდინარე:-" + upcomming;
                // UI განახლების განახლება
                dgvPayments.ResumeLayout();
            }
            catch (Exception ex)
            {
                // გამოყენებული, რომ ResumeLayout სწორად გამოითვლება
                dgvPayments.ResumeLayout();
                MessageBox.Show($"შეცდომა გადახდების სიის ჩატვირთვისას:\n{ex.Message}",
                    "შეცდომა",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void tsmAdminPanel_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.IsAdmin)
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_adminPanelFormFactory.Invoke);
        }

        private void BackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditSettings))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_backupManagementFormFactory.Invoke);
        }

        private async void btnRefreshPaymentProcess_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await RunAutoPaymentsWithProgressAsync();
        }

        private void LogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanViewReports))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_logViewerFormFactory.Invoke);
        }

        private void balanceTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_userContext.IsAuthenticated)
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var transferForm = _balanceTransferFormFactory.Invoke();
            transferForm.ShowDialog();
        }

        private void PaymentTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_paymentTestFormFactory.Invoke);
        }

        private void userManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check - ყველა ავტენტიფიცირებული მომხმარებელი შეძლებს წვდომას
            // UserManagementForm-ში თავად არის შეზღუდვები admin/regular user-ისთვის
            if (!_userContext.IsAuthenticated)
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenOrActivateForm(_userManagementFormFactory.Invoke);
        }
    }
}
