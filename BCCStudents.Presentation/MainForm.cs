using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using BCCStudents.Domain.Entities;
using System.Configuration;
using BCCStudents.Application.Services.AutoFileDetection;
using BCCStudents.Application.Services.Sync.DownStream;
using BCCStudents.Application.Services.Sync.UpStream;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Application.Interfaces; 
using BCCStudents.Presentation;
using BCCStudents.Presentation.Properties;
using BCCStudents.Presentation.Data.Configuration;
using BCCStudents.Infrastructure.Services;
using System.Linq;

namespace BCCStudents.Presentation
{
    public partial class MainForm : Form
    {
        private readonly StudyStartDateManager _studyStartDateManager;
        private readonly IPaymentService _paymentService;
        private readonly ILoggerRepository _loggerRepository;
        private readonly IStatisticsService _statisticService;
        private readonly IServiceProvider _serviceProvider;
        private readonly AutoFileDetectionManager _autoDetectionManager;
        private readonly IDownStreamSyncService _downStreamSyncService;
        private readonly IDownStreamSyncManager _downStreamSyncManager;
        private readonly IUpStreamSyncManager _upStreamSyncManager;
        private readonly ConnectionMonitorService _connectionMonitorService;
        private readonly BCCStudents.Infrastructure.Services.BackupManager _backupManager;
        private readonly IConnectionStatusService _connectionStatusService;
        private readonly IConfigurationService _configurationService;
        private readonly IPaymentDescriptionAnalyzer _paymentDescriptionAnalyzer;
        private SyncStatusControl _syncStatusControl;
        private bool _allowClose;
        public MainForm( IPaymentService paymentService, 
            IServiceProvider serviceProvider, 
            ILoggerRepository loggerRepository, 
            IStatisticsService statisticsService, 
            AutoFileDetectionManager autoDetectionManager,
            IDownStreamSyncService downStreamSyncService,
            IDownStreamSyncManager downStreamSyncManager,
            IUpStreamSyncManager upStreamSyncManager,
            ConnectionMonitorService connectionMonitorService,
            BCCStudents.Infrastructure.Services.BackupManager backupManager,
            IPaymentDescriptionAnalyzer paymentDescriptionAnalyzer,
            IConnectionStatusService connectionstatusservice)
        {
            InitializeComponent();
            
            //_connectionService = connectionService;
            _loggerRepository = loggerRepository;
            _statisticService = statisticsService;
            _serviceProvider = serviceProvider;
            _paymentService = paymentService;
            _autoDetectionManager = autoDetectionManager;
            _downStreamSyncService = downStreamSyncService ?? throw new ArgumentNullException(nameof(downStreamSyncService));
            _downStreamSyncManager = downStreamSyncManager ?? throw new ArgumentNullException(nameof(downStreamSyncManager));
            _upStreamSyncManager = upStreamSyncManager ?? throw new ArgumentNullException(nameof(upStreamSyncManager));
            _connectionMonitorService = connectionMonitorService ?? throw new ArgumentNullException(nameof(connectionMonitorService));
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            _connectionStatusService = connectionstatusservice ?? throw new ArgumentNullException(nameof(connectionstatusservice));
            _studyStartDateManager = _serviceProvider.GetRequiredService<StudyStartDateManager>();
            
            var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
            if (configService.IsTestDb)
                FormTitleHelper.SetTitle(this, "სტუდენტების მართვა - საცდელი ბაზა");
            else
                FormTitleHelper.SetTitle(this, "სტუდენტების მართვა");
            
            // გამოვიწეროთ ConnectionStatusChanged ივენთი
            _connectionStatusService.ConnectionStatusChanged += ConnectionStatusService_ConnectionStatusChanged;
            
            SetupDataGridView();
            SetupSyncStatus();
        }

        private void SetupDataGridView()
        {
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
            dgvPayments.Columns[0].Width = 80;  // StudentID
            dgvPayments.Columns[1].Width = 120; // FirstName
            dgvPayments.Columns[2].Width = 120; // LastName
            dgvPayments.Columns[3].Width = 150; // GroupName
            dgvPayments.Columns[4].Width = 100;  // TuitionFee
            dgvPayments.Columns[5].Width = 100; // TotalPaid
            dgvPayments.Columns[6].Width = 100; // AmountDue
            dgvPayments.Columns[7].Width = 120; // NextPaymentDate
            dgvPayments.Columns[8].Width = 100;
            
            // სვეტების მეტად განსაზღვრა
            dgvPayments.VirtualMode = false;
        }
        
        private void btnStudents_Click(object sender, EventArgs e)
        {
            // StudentManagementForm გამოძახება
            var studentForm = _serviceProvider.GetRequiredService<StudentManagementForm>();
            studentForm.Show();
        }
        private void btnGroups_Click(object sender, EventArgs e)
        {
            // GroupManagementForm გამოძახება
            var groupForm = _serviceProvider.GetRequiredService<GroupManagementForm>();
            groupForm.Show();
        }
        
        private void btnGroupsEdit_Click(object sender, EventArgs e)
        {
            // GroupsEdit ფორმის გამოძახება
            var groupsEditForm = _serviceProvider.GetRequiredService<GroupsEdit>();
            groupsEditForm.Show();
        }
        
        //ფორმის ჩატვირთვისას და ავტომატური გადახდების გაშვება
        private async void MainForm_Load(object sender, EventArgs e)
        {
            // ადმინისტრატორის გარდა სხვა მომხმარებლებისთვის უფლებების შეზღუდვა
            if (UserSession.Role != "Administrator")
            {
                tsmAdminPanel.Enabled = false;
                გადახდებიToolStripMenuItem.Enabled = false;
            }
            SetStudyStartDate();
            UpdateStudyStartDateLabel(); // ფორმის ჩატვირთვისას დაწყების თარიღის განახლება

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
                
                _backupManager.InitializePeriodicBackup();
                Console.WriteLine("პერიოდული ბექაპი წარმატებით ინიციალიზებულია");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"შეცდომა პერიოდული ბექაპის ინიციალიზაციისას: {ex.Message}");
            }
            StartDownStreamSync();
            // ბაზასთან კავშირის მონიტორინგის გაშვება
            StartConnectionMonitoring();
            
            // გადახდების სიის ჩატვირთვა
            LoadUpcomingPayments();
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
            // Event handler-ის დამატება
            _connectionMonitorService.ConnectionStatusChanged += ConnectionMonitorService_ConnectionStatusChanged;
            
            // მონიტორინგის გაშვება
            _connectionMonitorService.Start();
            
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
        private void ConnectionMonitorService_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs e)
        {
            // UI thread-ზე განახლება
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ConnectionMonitorService_ConnectionStatusChanged(sender, e)));
                return;
            }

            if (e.CurrentStatus)
            {
                // ბაზა აღდგენილია
                UpdateUIBasedOnConnectionStatus(true);
                
                // აღდგენის შეტყობინება გამოჩენა
                if (!e.PreviousStatus)
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
                if (e.PreviousStatus)
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
                    var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
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
            // ვამოწმებთ არსებულ სტუდენტებს, თუ არის მაშინ არ გამოვაჩენთ დიალოგს
            var studentService = _serviceProvider.GetRequiredService<IStudentService>();
            var existingStudentsCount = studentService.GetAllStudents().Count;
            
            if (existingStudentsCount > 0)
            {
                /*MessageBox.Show(
                    "სტუდენტების მართვის დაწყების თარიღი უკვე განსაზღვრულია და ახალი ვერ განსაზღვრდება, რადგან არსებობს მონაცემები.",
                    "თარიღი უკვე განსაზღვრულია",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );*/
                return;
            }
            
            if (!StudyStartDateManager.IsStudyStartDateSet())
            {
                var result = MessageBox.Show(
                    "სტუდენტების მართვის დაწყების თარიღი არ არის განსაზღვრული. გსურთ განსაზღვროთ?",
                    "განსაზღვრა",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    var dateForm = _serviceProvider.GetRequiredService<SetStudyStartDateForm>();
                    if (dateForm.ShowDialog() == DialogResult.OK)
                    {
                        DateTime selectedDate = dateForm.SelectedDate;
                        StudyStartDateManager.SaveStudyStartDate(selectedDate,_studyStartDateManager);

                        
                            MessageBox.Show($"✅ თარიღი განსაზღვრულია: {selectedDate:dd-MM-yyyy}",
                            "დადასტურება",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        lbStartStudyDate.Text = selectedDate.ToString();
                        lblPaymentNextDate.Text = selectedDate.AddMonths(1).ToString();
                    }
                    else
                    {
                        MessageBox.Show("სტუდენტების მართვის დაწყების თარიღის განსაზღვრა გაუქმებულია.",
                            "შეცდომა",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        System.Windows.Forms.Application.Exit();
                    }
                }
                else
                {
                    MessageBox.Show("სტუდენტების მართვის დაწყების თარიღის განსაზღვრა გაუქმებულია.",
                        "შეცდომა",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    System.Windows.Forms.Application.Exit();
                }
            }
            
        }
        public void UpdateStudyStartDateLabel()
        {
            var date = StudyStartDateManager.GetStudyStartDate();
            lbStartStudyDate.Text = date.HasValue
                ? $"{date.Value}"
                : "სტუდენტების მართვის დაწყების თარიღი არ არის განსაზღვრული";
            lbStartStudyDate.Refresh();
            lblPaymentNextDate.Text = date.Value.AddMonths(1).ToString();
        }
        private void სტატისტიკაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var groupRepository = _serviceProvider.GetRequiredService<BCCStudents.Domain.Interfaces.IGroupRepository>();
            StatisticsForm statisticForm = new StatisticsForm(_statisticService, groupRepository);
            statisticForm.Show();
        }
        private void გადახდებიToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var financeManagementForm = _serviceProvider.GetRequiredService<FinanceManagementForm>();
            financeManagementForm.Show();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _downStreamSyncManager.Stop();
            
            // ბაზასთან კავშირის მონიტორინგის გაჩერება
            if (_connectionMonitorService != null)
            {
                _connectionMonitorService.Stop();
                _connectionMonitorService.ConnectionStatusChanged -= ConnectionMonitorService_ConnectionStatusChanged;
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
                if (_backupManager.DbChangedSinceLastBackup)
                {
                    var userBackupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "Backups");
                    Directory.CreateDirectory(userBackupDir);
                    _backupManager.AutoBackup("SchoolManagement.db", userBackupDir);
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
                bool backupSuccess = _backupManager.CreateBackup(backupFilePath);
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
                
                _backupManager.StopPeriodicBackup();
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
            _ = Task.Run(async () =>
            {
                try
                {
                    var syncResult = await _downStreamSyncService.SyncFromServerAsync().ConfigureAwait(false);
                    if (!syncResult.Success && syncResult.Errors.Count > 0)
                    {
                        var message = string.Join(" | ", syncResult.Errors);
                        _loggerRepository.WriteLog("DownStream", "Failed", message, Environment.UserName);
                    }
                }
                catch (Exception ex)
                {
                    _loggerRepository.WriteLog("DownStream", "Failed", ex.ToString(), Environment.UserName);
                }
            });

            _downStreamSyncManager.Start();
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
            };

            _upStreamSyncManager.SyncCompleted += (sender, args) =>
            {
                _syncStatusControl.UpdateUpStreamStatus(args);
            };
        }

        private void გადახდაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paymentForm = _serviceProvider.GetRequiredService<PaymentForm>();
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
                
                var paymentRepo = _serviceProvider.GetRequiredService<IPaymentRepository>();
                var payments = paymentRepo.GetPendingPayments();
                dgvPayments.Rows.Clear();

                var studyStartDate = StudyStartDateManager.GetStudyStartDate();
                
                // დალაგება გადახდების სიის
                var sortedPayments = payments
                    .OrderByDescending(p => p.AmountDue > 0)  // უფრო მაღალი გადახდილი გადახდები პირველად
                    .ThenBy(p => p.NextPaymentDate ?? studyStartDate?.AddMonths(1))  // შემდეგ შემდეგი თარიღის მიხედვით
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
                    DateTime nextPaymentDate = payment.NextPaymentDate ?? studyStartDate?.AddMonths(1) ?? DateTime.Today.AddMonths(1);
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
                StudentAmount.Text = @"გადახდილი:" + paid + " გადაუხდელი:" + overdue + " მომდინარე:" + upcomming;
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
            var adminForm = _serviceProvider.GetRequiredService<AdminPanelForm>();
            adminForm.Show();
        }
        
        private void ბექაპისმართვაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var backupForm = _serviceProvider.GetRequiredService<BackupManagementForm>();
            backupForm.Show();
        }

        private async void  btnRefreshPaymentProcess_Click(object sender, EventArgs e)
        {
           await RunAutoPaymentsWithProgressAsync();
        }

        private void ლოგებიToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logViewer = _serviceProvider.GetRequiredService<LogViewerForm>();
            logViewer.Show();
        }

        private void გადახდისტესტირებაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var paymentTestForm = _serviceProvider.GetRequiredService<PaymentTestForm>();
            paymentTestForm.Show();
        }
    }
}
