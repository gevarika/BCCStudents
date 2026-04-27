using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// მრავალნაბიჯიანი Setup Wizard – ძირითადი კონფიგურაციის დასაყენებლად.
    /// </summary>
    public class SetupWizardForm : Form
    {
        private readonly IConfigurationService _config;
        private readonly IApplicationStatus _appStatus;

        private Panel[] _steps = Array.Empty<Panel>();
        private int _currentStepIndex;

        // Step1 - DB
        private TextBox txtLocalConn;
        private TextBox txtServerConn;
        private CheckBox chkUseLocal;
        private CheckBox chkIsTestDb;

        // Step2 - Backup
        private TextBox txtBackupDir;
        private CheckBox chkAutoBackup;
        private NumericUpDown numBackupHours;
        private NumericUpDown numBackupMaxFiles;
        private TextBox txtMySqlDumpPath;

        // Step3 - SMS & AutoDetection
        private CheckBox chkSmsEnabled;
        private TextBox txtSmsApiKey;
        private TextBox txtSmsRegistration;
        private TextBox txtSmsPayment;
        private TextBox txtSmsUpcoming;
        private TextBox txtSmsOverdue;

        private CheckBox chkAutoDetectionEnabled;
        private TextBox txtWatchDir;
        private TextBox txtFilePattern;

        // Navigation buttons
        private Button btnBack;
        private Button btnNext;
        private Button btnFinish;

        public SetupWizardForm(IConfigurationService config, IApplicationStatus appStatus)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _appStatus = appStatus ?? throw new ArgumentNullException(nameof(appStatus));

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "პირველი გაშვების მექსტერი (Setup Wizard)";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 800;
            Height = 550;

            var contentPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 430
            };

            btnBack = new Button { Text = "← Back", Width = 100, Left = 10, Top = 440 };
            btnNext = new Button { Text = "Next →", Width = 100, Left = 120, Top = 440 };
            btnFinish = new Button { Text = "Finish", Width = 100, Left = 230, Top = 440 };

            btnBack.Click += BtnBack_Click;
            btnNext.Click += BtnNext_Click;
            btnFinish.Click += BtnFinish_Click;

            Controls.Add(contentPanel);
            Controls.Add(btnBack);
            Controls.Add(btnNext);
            Controls.Add(btnFinish);

            // Step panels
            var step1 = BuildStep1_Database();
            var step2 = BuildStep2_Backup();
            var step3 = BuildStep3_SmsAndAutoDetection();

            _steps = new[] { step1, step2, step3 };
            foreach (var p in _steps)
            {
                p.Dock = DockStyle.Fill;
                p.Visible = false;
                contentPanel.Controls.Add(p);
                p.BringToFront();
            }

            Load += SetupWizardForm_Load;
        }

        private Panel BuildStep1_Database()
        {
            var panel = new Panel();

            var lblTitle = new Label
            {
                Text = "საფუძვლიანი მონაცემთა ბაზის პარამეტრები",
                AutoSize = true,
                Left = 20,
                Top = 20,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold)
            };

            var lblLocal = new Label
            {
                Text = "ლოკალური ბაზის Connection String:",
                AutoSize = true,
                Top = 60,
                Left = 20
            };

            txtLocalConn = new TextBox
            {
                Multiline = true,
                Width = 720,
                Height = 80,
                Top = 85,
                Left = 20,
                ScrollBars = ScrollBars.Vertical
            };

            var lblServer = new Label
            {
                Text = "სერვერის ბაზის Connection String:",
                AutoSize = true,
                Top = 180,
                Left = 20
            };

            txtServerConn = new TextBox
            {
                Multiline = true,
                Width = 720,
                Height = 80,
                Top = 205,
                Left = 20,
                ScrollBars = ScrollBars.Vertical
            };

            chkUseLocal = new CheckBox
            {
                Text = "ლოკალური ბაზის გამოყენება (UseLocalDb)",
                AutoSize = true,
                Top = 300,
                Left = 20
            };

            chkIsTestDb = new CheckBox
            {
                Text = "სატესტო ბაზის რეჟიმი (IsTestDb)",
                AutoSize = true,
                Top = 330,
                Left = 20
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblLocal);
            panel.Controls.Add(txtLocalConn);
            panel.Controls.Add(lblServer);
            panel.Controls.Add(txtServerConn);
            panel.Controls.Add(chkUseLocal);
            panel.Controls.Add(chkIsTestDb);

            return panel;
        }

        private Panel BuildStep2_Backup()
        {
            var panel = new Panel();

            var lblTitle = new Label
            {
                Text = "ბექაპის პარამეტრები",
                AutoSize = true,
                Left = 20,
                Top = 20,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold)
            };

            var lblDir = new Label
            {
                Text = "ბექაპის საქაღალდე (BackupDirectory):",
                AutoSize = true,
                Top = 60,
                Left = 20
            };

            txtBackupDir = new TextBox
            {
                Width = 600,
                Top = 85,
                Left = 20
            };

            chkAutoBackup = new CheckBox
            {
                Text = "ავტომატური ბექაპი (AutoBackupEnabled)",
                AutoSize = true,
                Top = 120,
                Left = 20
            };

            var lblHours = new Label
            {
                Text = "ბექაპის ინტერვალი (საათები):",
                AutoSize = true,
                Top = 160,
                Left = 20
            };

            numBackupHours = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 168,
                Top = 185,
                Left = 20,
                Width = 80
            };

            var lblMaxFiles = new Label
            {
                Text = "ბექაპის მაქს. რაოდენობა (MaxBackupFiles):",
                AutoSize = true,
                Top = 230,
                Left = 20
            };

            numBackupMaxFiles = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 1000,
                Top = 255,
                Left = 20,
                Width = 80
            };

            var lblDump = new Label
            {
                Text = "mysqldump.exe გზა (MySqlDumpPath):",
                AutoSize = true,
                Top = 300,
                Left = 20
            };

            txtMySqlDumpPath = new TextBox
            {
                Width = 600,
                Top = 325,
                Left = 20
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDir);
            panel.Controls.Add(txtBackupDir);
            panel.Controls.Add(chkAutoBackup);
            panel.Controls.Add(lblHours);
            panel.Controls.Add(numBackupHours);
            panel.Controls.Add(lblMaxFiles);
            panel.Controls.Add(numBackupMaxFiles);
            panel.Controls.Add(lblDump);
            panel.Controls.Add(txtMySqlDumpPath);

            return panel;
        }

        private Panel BuildStep3_SmsAndAutoDetection()
        {
            var panel = new Panel();

            var lblTitle = new Label
            {
                Text = "SMS და ავტო-დეტექციის პარამეტრები",
                AutoSize = true,
                Left = 20,
                Top = 20,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold)
            };

            chkSmsEnabled = new CheckBox
            {
                Text = "SMS სერვისის ჩართვა (SmsEnabled)",
                AutoSize = true,
                Top = 60,
                Left = 20
            };

            var lblApiKey = new Label
            {
                Text = "SMS API Key:",
                AutoSize = true,
                Top = 90,
                Left = 20
            };

            txtSmsApiKey = new TextBox
            {
                Width = 400,
                Top = 115,
                Left = 20
            };

            var lblReg = new Label { Text = "რეგისტრაციის ტექსტი:", AutoSize = true, Top = 150, Left = 20 };
            txtSmsRegistration = new TextBox { Width = 400, Top = 175, Left = 20 };

            var lblPay = new Label { Text = "გადახდის ტექსტი:", AutoSize = true, Top = 205, Left = 20 };
            txtSmsPayment = new TextBox { Width = 400, Top = 230, Left = 20 };

            var lblUpcoming = new Label { Text = "მიახლოებული გადახდის ტექსტი:", AutoSize = true, Top = 260, Left = 20 };
            txtSmsUpcoming = new TextBox { Width = 400, Top = 285, Left = 20 };

            var lblOverdue = new Label { Text = "ვადაგადაცილების ტექსტი:", AutoSize = true, Top = 315, Left = 20 };
            txtSmsOverdue = new TextBox { Width = 400, Top = 340, Left = 20 };

            // Auto File Detection
            chkAutoDetectionEnabled = new CheckBox
            {
                Text = "ავტომატური ფაილების მოძიება ჩართულია",
                AutoSize = true,
                Top = 60,
                Left = 460
            };

            var lblWatch = new Label
            {
                Text = "საქაღალდე (WatchFolderPath):",
                AutoSize = true,
                Top = 90,
                Left = 460
            };

            txtWatchDir = new TextBox
            {
                Width = 260,
                Top = 115,
                Left = 460
            };

            var lblPattern = new Label
            {
                Text = "ფაილის შაბლონი (FileNamePattern):",
                AutoSize = true,
                Top = 150,
                Left = 460
            };

            txtFilePattern = new TextBox
            {
                Width = 260,
                Top = 175,
                Left = 460
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(chkSmsEnabled);
            panel.Controls.Add(lblApiKey);
            panel.Controls.Add(txtSmsApiKey);
            panel.Controls.Add(lblReg);
            panel.Controls.Add(txtSmsRegistration);
            panel.Controls.Add(lblPay);
            panel.Controls.Add(txtSmsPayment);
            panel.Controls.Add(lblUpcoming);
            panel.Controls.Add(txtSmsUpcoming);
            panel.Controls.Add(lblOverdue);
            panel.Controls.Add(txtSmsOverdue);
            panel.Controls.Add(chkAutoDetectionEnabled);
            panel.Controls.Add(lblWatch);
            panel.Controls.Add(txtWatchDir);
            panel.Controls.Add(lblPattern);
            panel.Controls.Add(txtFilePattern);

            return panel;
        }

        private void SetupWizardForm_Load(object? sender, EventArgs e)
        {
            // Step 1 - DB
            txtLocalConn.Text = _config.LocalMySqlConnectionString ?? string.Empty;
            txtServerConn.Text = _config.ServerMySqlConnectionString ?? string.Empty;
            chkUseLocal.Checked = _config.UseLocalDb;
            chkIsTestDb.Checked = _config.IsTestDb;

            // Step 2 - Backup
            txtBackupDir.Text = _config.BackupDirectory ?? string.Empty;
            chkAutoBackup.Checked = _config.AutoBackupEnabled;
            numBackupHours.Value = _config.BackupIntervalHours > 0 ? _config.BackupIntervalHours : 24;
            numBackupMaxFiles.Value = _config.MaxBackupFiles > 0 ? _config.MaxBackupFiles : 30;
            txtMySqlDumpPath.Text = _config.MySqlDumpPath ?? string.Empty;

            // Step 3 - SMS
            chkSmsEnabled.Checked = _config.SmsEnabled;
            txtSmsApiKey.Text = _config.SmsApiKey ?? string.Empty;
            txtSmsRegistration.Text = _config.SmsText_Registration ?? string.Empty;
            txtSmsPayment.Text = _config.SmsText_Payment ?? string.Empty;
            txtSmsUpcoming.Text = _config.SmsText_UpcomingReminder ?? string.Empty;
            txtSmsOverdue.Text = _config.SmsText_OverdueReminder ?? string.Empty;

            // Step 3 - AutoDetection (App.config)
            chkAutoDetectionEnabled.Checked = _config.IsAutoDetectionEnabled();
            txtWatchDir.Text = _config.GetWatchFolderPath() ?? string.Empty;
            txtFilePattern.Text = _config.GetFileNamePattern() ?? "*.xlsx";

            _currentStepIndex = 0;
            UpdateStepVisibility();
        }

        private void UpdateStepVisibility()
        {
            for (int i = 0; i < _steps.Length; i++)
            {
                _steps[i].Visible = (i == _currentStepIndex);
            }

            btnBack.Enabled = _currentStepIndex > 0;
            btnNext.Enabled = _currentStepIndex < _steps.Length - 1;
            btnFinish.Enabled = _currentStepIndex == _steps.Length - 1;
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            if (_currentStepIndex > 0)
            {
                _currentStepIndex--;
                UpdateStepVisibility();
            }
        }

        private void BtnNext_Click(object? sender, EventArgs e)
        {
            if (_currentStepIndex < _steps.Length - 1)
            {
                _currentStepIndex++;
                UpdateStepVisibility();
            }
        }

        private void BtnFinish_Click(object? sender, EventArgs e)
        {
            try
            {
                // Step1 - DB
                _config.LocalMySqlConnectionString = txtLocalConn.Text.Trim();
                _config.ServerMySqlConnectionString = txtServerConn.Text.Trim();
                _config.UseLocalDb = chkUseLocal.Checked;
                _config.IsTestDb = chkIsTestDb.Checked;

                // Step2 - Backup
                _config.BackupDirectory = txtBackupDir.Text.Trim();
                _config.AutoBackupEnabled = chkAutoBackup.Checked;
                _config.BackupIntervalHours = (int)numBackupHours.Value;
                _config.BackupIntervalMinutes = _config.BackupIntervalHours * 60;
                _config.MaxBackupFiles = (int)numBackupMaxFiles.Value;
                _config.MySqlDumpPath = txtMySqlDumpPath.Text.Trim();

                // Step3 - SMS
                _config.SmsEnabled = chkSmsEnabled.Checked;
                _config.SmsApiKey = txtSmsApiKey.Text.Trim();
                _config.SmsText_Registration = txtSmsRegistration.Text.Trim();
                _config.SmsText_Payment = txtSmsPayment.Text.Trim();
                _config.SmsText_UpcomingReminder = txtSmsUpcoming.Text.Trim();
                _config.SmsText_OverdueReminder = txtSmsOverdue.Text.Trim();

                // Step3 - AutoDetection (App.config)
                _config.SaveAutoDetectionSettings(
                    chkAutoDetectionEnabled.Checked,
                    txtWatchDir.Text.Trim(),
                    txtFilePattern.Text.Trim());

                _config.Save();

                MessageBox.Show("პარამეტრები წარმატებით შეინახა. პროგრამა გადაიტვირთება.",
                    "Setup დასრულდა",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                System.Windows.Forms.Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა პარამეტრების შენახვისას: {ex.Message}",
                    "შეცდომა",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

