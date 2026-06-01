using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Services;
using BCCStudents.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace BCCStudents.Presentation
{
    public delegate SetStudyStartDateForm SetStudyStartDateFormFactory();
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class StudentManagementForm : Form
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly IPaymentDateService _paymentDateService;
        private readonly ISystemConfigurationService _systemConfigService;
        private readonly ISubGroupService _subGroupService;
        private readonly IStudentExportService _studentExportService;
        //private readonly ImportService _importService;
        private readonly IStudentCodeGenerator _studentCodeGenerator;
        private readonly IServiceProvider _serviceProvider;
        private readonly BackupService _backupManager;
        private readonly ISmsService _smsservice;

        private readonly ImportFormFactory _importFormFactory;
        private readonly StudentsEditFormFactory _studentsEditFormFactory;
        private readonly PendingStudentsFormFactory _pendingStudentsFormFactory;
        private readonly FailedStudentsFormFactory _failedStudentsFormFactory;
        private readonly SetStudyStartDateFormFactory _setStudyStartDateFormFactory;
        private readonly PendingRegistrationMonitor _pendingRegistrationMonitor;
        private readonly IUserContext _userContext;
        private const string PendingMenuBaseText = "ონლაინ რეგისტრაციის დადასტურება";
        //UserSession _userSession = new UserSession();
        public StudentManagementForm(
            IStudentService studentService,
            IGroupService groupService,
            ISubGroupService subGroupService,
            IStudentCodeGenerator studentCodeGenerator,
            IServiceProvider serviceProvider,
            IPaymentDateService paymentDateService,
            ISystemConfigurationService systemConfigService,
            IStudentExportService studentExportService,
            BackupService backupManager,
            ISmsService smsService,
            ImportFormFactory importFormFactory,
            StudentsEditFormFactory studentsEditFormFactory,
            PendingStudentsFormFactory pendingStudentsFormFactory,
            FailedStudentsFormFactory failedStudentsFormFactory,
            SetStudyStartDateFormFactory setStudyStartDateFormFactory,
            PendingRegistrationMonitor pendingRegistrationMonitor,
            IUserContext userContext
            )
        {
            InitializeComponent();
            _studentService = studentService;
            _groupService = groupService;
            _subGroupService = subGroupService;
            _studentCodeGenerator = studentCodeGenerator;
            _serviceProvider = serviceProvider;
            _paymentDateService = paymentDateService;
            _systemConfigService = systemConfigService ?? throw new ArgumentNullException(nameof(systemConfigService));
            _studentExportService = studentExportService;
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ახალი მოსწავლის რეგისტრაცია");
            else FormTitleHelper.SetTitle(this, "ახალი მოსწავლის რეგისტრაცია - სატესტო რეჟიმი");
            _backupManager = backupManager;
            _smsservice = smsService;
            _importFormFactory = importFormFactory;
            _studentsEditFormFactory = studentsEditFormFactory;
            _pendingStudentsFormFactory = pendingStudentsFormFactory;
            _failedStudentsFormFactory = failedStudentsFormFactory;
            _setStudyStartDateFormFactory = setStudyStartDateFormFactory;
            _pendingRegistrationMonitor = pendingRegistrationMonitor ?? throw new ArgumentNullException(nameof(pendingRegistrationMonitor));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            // Apply security checks after form is loaded
            this.Load += StudentManagementForm_Load;
            this.FormClosing += StudentManagementForm_FormClosing;
        }

        private void StudentManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pendingRegistrationMonitor.PendingCountChanged -= PendingRegistrationMonitor_PendingCountChanged;
        }

        private void PendingRegistrationMonitor_PendingCountChanged(object sender, int count)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => PendingRegistrationMonitor_PendingCountChanged(sender, count)));
                return;
            }

            UpdatePendingMenuBadge(count);
        }

        private void UpdatePendingMenuBadge(int count)
        {
            if (ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem == null)
            {
                return;
            }

            ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Text = count > 0
                ? $"{PendingMenuBaseText} ({count})"
                : PendingMenuBaseText;
        }
        private void StudentManagementForm_Load(object sender, EventArgs e)
        {
            ApplySecurityChecks();
            _pendingRegistrationMonitor.PendingCountChanged += PendingRegistrationMonitor_PendingCountChanged;
            UpdatePendingMenuBadge(_pendingRegistrationMonitor.CurrentCount);
            if (UserSession.Role != "Administrator")
            {
                btnImportFromExcell.Enabled = false;
            }
            CheckGroupsExistence(); // პირველი ნაბიჯი
            DateTime? studyStartDate = PaymentDateManager.GetNextPaymentDate();

            if (studyStartDate == null)
            {
                MessageBox.Show("⚠️ ჯერ უნდა მიუთითოთ სწავლის დაწყების თარიღი!", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetStudyStartDate();
                studyStartDate = PaymentDateManager.GetNextPaymentDate();
            }
            txtStudentInfo.Enabled = false;
            string filePath = "cachedTexts.txt";
            LoadCachedTextsFromFile(filePath, txtFirstName);
            LoadStudents();
            LoadGroups();
        }
        private void ApplySecurityChecks()
        {
            // btnAddStudent - CanAddStudents or CanManageStudents permission
            if (btnAddStudent != null)
            {
                btnAddStudent.Tag = $"Permission_{Permission.CanAddStudents}";
                btnAddStudent.Enabled = _userContext.HasPermission(Permission.CanAddStudents) ||
                                       _userContext.HasPermission(Permission.CanManageStudents);
            }

            // მოსწავლისრედაქტირებაToolStripMenuItem - CanEditStudents or CanManageStudents permission
            if (მოსწავლისრედაქტირებაToolStripMenuItem != null)
            {
                მოსწავლისრედაქტირებაToolStripMenuItem.Tag = $"Permission_{Permission.CanEditStudents}";
                მოსწავლისრედაქტირებაToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanEditStudents) ||
                                                                   _userContext.HasPermission(Permission.CanManageStudents);
            }

            // tsmFailedStudents - CanViewReports or CanManageStudents permission (viewing failed students is a read operation)
            if (tsmFailedStudents != null)
            {
                tsmFailedStudents.Tag = $"Permission_{Permission.CanViewReports}";
                tsmFailedStudents.Enabled = _userContext.HasPermission(Permission.CanViewReports) ||
                                           _userContext.HasPermission(Permission.CanManageStudents);
            }

            // ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem - CanViewReports or CanManageStudents permission
            if (ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem != null)
            {
                ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Tag = $"Permission_{Permission.CanViewReports}";
                ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanViewReports) ||
                                                                              _userContext.HasPermission(Permission.CanManageStudents);
            }

            // btnExportToExcell - CanExportData permission
            if (btnExportToExcell != null)
            {
                btnExportToExcell.Tag = $"Permission_{Permission.CanExportData}";
                btnExportToExcell.Enabled = _userContext.HasPermission(Permission.CanExportData);
            }

            // btnImportFromExcell - CanImport permission
            if (btnImportFromExcell != null)
            {
                btnImportFromExcell.Tag = $"Permission_{Permission.CanImport}";
                btnImportFromExcell.Enabled = _userContext.HasPermission(Permission.CanImport);
            }
        }
        #region Delegates
        public delegate ImportFormV2 ImportFormFactory();
        public delegate StudentsEditForm StudentsEditFormFactory();
        public delegate PendingStudentsForm PendingStudentsFormFactory();
        public delegate FailedStudentsForm FailedStudentsFormFactory();

        #endregion
        private void LoadCachedTextsFromFile(string filePath, TextBox textBox)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                textBox.AutoCompleteCustomSource.AddRange(lines);
            }
        }
        private void LoadStudents()
        {
            try
            {
                var students = _studentService.GetAllStudentsSomeInfo();
                dataGridView1.DataSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadGroups()
        {
            List<Group> groups = _groupService.GetAllActiveGroups();
            clbGroups.DataSource = groups;
            clbGroups.DisplayMember = "Name";
            clbGroups.ValueMember = "Id";
        }
        public Dictionary<string, List<int>> groupIds = new Dictionary<string, List<int>>(); // ჯგუფების ID-ების შესანახად
        private void ImportForm_ImportCompleted(object sender, EventArgs e)
        {
            // აქ  უნდა  დაამატოთ  კოდი,  რომელიც  მიიღებს  იმპორტირებულ  მონაცემებს  და  გამოიყვანს  StudentManagementForm-ზე.
            // მაგალითად,  DataGridView-ში  ან  სხვა  კონტროლში.
            // ... (მონაცემების მიღება და გამოტანა) ...

            // განახლება პროცესში და სტატუსბარის განახლება
            LoadStudents();

        }
        private async void btnAddStudent_Click(object sender, EventArgs e)
        {
            var result = new OperationResultContext();
            try
            {
                //UserSession userSession = new UserSession();
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("გთხოვთ შეავსოთ ყველა სავალდებულო ველი.", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (clbGroups.CheckedItems.Count == 0)
                {
                    MessageBox.Show("⚠️ გთხოვთ მონიშნოთ მინიმუმ ერთი ჯგუფი.", "ჯგუფი არ არის არჩეული", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (int.TryParse(cmbDiscount.Text.Replace("%", "").Trim(), out var discountPercent)
                    && discountPercent > 0
                    && string.IsNullOrWhiteSpace(txtStudentInfo.Text))
                {
                    MessageBox.Show(
                        "თქვენ მოსწავლისთვის დააყენეთ ფასდაკლება, ასევე საჭიროა მიუთითოთ მოსწავლის სტატუსი!",
                        "მოსწავლის სტატუსი",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Duplicate checks
                var firstName = txtFirstName.Text.Trim();
                var lastName = txtLastName.Text.Trim();
                var parentName = txtParentName.Text.Trim();
                var address = txtAddress.Text.Trim();

                // 2) Full match: FirstName, LastName, ParentName, Address -> Block with OK
                if (_studentService.ExistsStudentByNameParentAddress(firstName, lastName, parentName, address))
                {
                    MessageBox.Show("მოსწავლე ამ მონაცემებით არის ბაზაში", "დუბლიკატი", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1) Name + Surname -> Ask continue or cancel
                if (_studentService.ExistsStudentByName(firstName, lastName))
                {
                    var choice = MessageBox.Show("მოსწავლე უკვე არსებობს ამ სახელით და გვარით. გსურთ გაგრძელება?", "დუბლიკატი", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (choice != DialogResult.Yes)
                        return;
                }

                // 3) Address-only info
                var sameAddressCount = _studentService.CountStudentsByAddress(address);
                if (sameAddressCount > 0)
                {
                    var addrChoice = MessageBox.Show($"ამ მისამართზე რეგისტრირებულია {sameAddressCount} სტუდენტი. გსურთ გაგრძელება?", "მისამართის დამთხვევა", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (addrChoice != DialogResult.Yes)
                        return;
                }

                // ვამოწმებთ არის თუ არა ეს პირველი მოსწავლე
                var existingStudentsCount = _studentService.GetAllStudents().Count;
                var studyStartDate = _systemConfigService?.GetStudyStartDate();
                var paymentStartDate = _systemConfigService.GetDefaultPaymentDate();
                if (!studyStartDate.HasValue && !paymentStartDate.HasValue)
                {
                    var dateresult = MessageBox.Show(
                        "⚠️ ჯერ უნდა მიუთითოთ სწავლის დაწყების თარიღი! '\n " +
                        "ასევე უნდა დააყენოთ პირველი გადახდის თარიღი! ",
                        "შეტყობინება",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                        );
                    if (dateresult != DialogResult.OK)
                        SetStudyStartDate();
                    studyStartDate = _systemConfigService?.GetStudyStartDate();
                    paymentStartDate = _systemConfigService.GetDefaultPaymentDate();
                }



                // თუ ეს პირველი მოსწავლეა, ვამოწმებთ სწავლის დაწყების თარიღს
                if (existingStudentsCount == 0)
                {
                    if (studyStartDate.HasValue)
                    {
                        var resultMessage = MessageBox.Show(
                            $"ეს არის პირველი მოსწავლე რეგისტრაცია.\n\n" +
                            $"სწავლის დაწყების თარიღი: {studyStartDate.Value:dd-MM-yyyy}\n" +
                            $"პირველი გადახდის თარიღი: {paymentStartDate.Value:dd-MM-yyyy}\n\n" +
                            $"გსურთ გაგრძელება ამ თარიღებით?",
                            "სწავლის დაწყების თარიღის დადასტურება",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (resultMessage != DialogResult.Yes)
                        {
                            MessageBox.Show("რეგისტრაცია გაუქმებულია. გთხოვთ შეცვალოთ სწავლის დაწყების თარიღი.",
                                "რეგისტრაცია გაუქმებულია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                Student student = new Student
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    ParentName = txtParentName.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Id_Numb = Convert.ToInt64(txtIdNumb.Text.Trim()),
                    Address = txtAddress.Text.Trim(),
                    Age = int.Parse(txtAge.Text.Trim()),
                    RegistrationDate = DateTime.Now,
                    DateOfPayment = paymentStartDate, // მთავარი ხაზი
                    TuitionFee = Convert.ToDecimal(numTuitionFee.Text),
                    Discount = int.Parse(cmbDiscount.Text.Replace("%", "").Trim()),
                    StudentCode = _studentCodeGenerator.GenerateStudentCode(),
                    Status = true,
                    Info = txtStudentInfo.Text,
                    PaymentStatus = "Pending",
                    IdCardPath = "FromPC",
                    AdditionalDocsPath = "FromPC",
                    Balance = 0
                };

                var selectedGroups = clbGroups.CheckedItems
                    .Cast<Group>()
                    .ToList();

                var selectedGroupIds = selectedGroups.Select(g => g.Id).ToList();

                // ვალიდაცია: შემოწმება ჯგუფების სიმძლავრისთვის
                List<string> fullGroups = new List<string>();
                List<string> nearFullGroups = new List<string>();

                foreach (var group in selectedGroups)
                {
                    // ვამოწმებთ არსებულ ჯგუფს ბაზიდან (თანამედროვე მონაცემებისთვის)
                    var currentGroup = _groupService.GetGroupById(group.Id);
                    if (currentGroup == null) continue;

                    // თუ MaxStudents = 0, შეზღუდვა არ არის
                    if (currentGroup.MaxStudents == 0) continue;

                    // თუ ჯგუფი სავსეა
                    if (currentGroup.StudentCount >= currentGroup.MaxStudents)
                    {
                        fullGroups.Add($"{group.Name} ({currentGroup.StudentCount}/{currentGroup.MaxStudents})");
                    }
                    // თუ მაქსიმალურ რაოდენობამდე 1-ით ნაკლებია (ბოლო ადგილი)
                    else if (currentGroup.StudentCount == currentGroup.MaxStudents - 1)
                    {
                        nearFullGroups.Add($"{group.Name} ({currentGroup.StudentCount}/{currentGroup.MaxStudents})");
                    }
                }

                // თუ რომელიმე ჯგუფი სავსეა - გაჩერება
                if (fullGroups.Count > 0)
                {
                    string message = "შემდეგ ჯგუფებში მაქსიმალური რაოდენობა უკვე დარეგისტრირებულია:\n\n";
                    message += string.Join("\n", fullGroups);
                    message += "\n\nგთხოვთ აირჩიოთ სხვა ჯგუფები.";
                    MessageBox.Show(message, "ჯგუფი სავსეა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // თუ რომელიმე ჯგუფში ბოლო ადგილია - შეტყობინება
                if (nearFullGroups.Count > 0)
                {
                    string message = "ყურადღება! შემდეგ ჯგუფებში ბოლო ადგილია:\n\n";
                    message += string.Join("\n", nearFullGroups);
                    message += "\n\nგსურთ გაგრძელება?";
                    var dialogResult = MessageBox.Show(message, "ბოლო ადგილი", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return;
                    }
                }

                int studentId;
                bool success = _studentService.AddStudent(student, selectedGroupIds, UserSession.Id, chkPrintContract.Checked, result, out studentId);

                if (success)
                {
                    _backupManager.DbChangedSinceLastBackup = true;

                    MessageBox.Show("სტუდენტი წარმატებით დაემატა!", "დადასტურება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //ClearFields();
                    LoadStudents();
                    if (Properties.Settings.Default.SmsEnabled)
                    {
                        var message = Properties.Settings.Default.SmsText_Registration;
                        var smsresult = await _smsservice.SendSmsAsync(student.PhoneNumber, message);
                        if (smsresult.Success)
                        {
                            MessageBox.Show("SMS გაგზავნილია!");
                        }
                        else
                        {
                            MessageBox.Show($"შეცდომა: {smsresult.Status}");
                        }
                    }
                }
                else
                {
                    var firstError = result.Steps.FirstOrDefault(s => !s.IsSuccess);
                    string errorMsg = firstError != null ? $"დაფიქსირდა შეცდომა: {firstError.Step}\n\n{firstError.Message}" : "დაფიქსირდა შეცდომა სტუდენტის დამატებისას.";
                    MessageBox.Show(errorMsg, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"დამატების შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private decimal baseFee = 0;
        private void CheckGroupsExistence()
        {
            var groups = _groupService.GetAllGroups();

            if (groups == null || groups.Count == 0)
            {
                var result = MessageBox.Show(
                    "ჯგუფები არ არსებობს.\nგსურთ ახალი ჯგუფის შექმნა?",
                    "გაფრთხილება",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    var groupForm = _serviceProvider.GetRequiredService<GroupManagementForm>(); // ან თუ იყენებ DI-ს, გამოიძახე ServiceProvider-ით
                    groupForm.ShowDialog();

                    // კიდევ ერთხელ გადავამოწმოთ შეიქმნა თუ არა ჯგუფი
                    groups = _groupService.GetAllGroups();
                    if (groups.Count == 0)
                    {
                        MessageBox.Show("ჯგუფების გარეშე პროგრამის გაგრძელება შეუძლებელია.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close(); // ფორმის დახურვა
                    }
                }
                else
                {
                    MessageBox.Show("ჯგუფების გარეშე პროგრამის გაგრძელება შეუძლებელია.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }
        private void SetStudyStartDate()
        {
            using (var dateForm = _setStudyStartDateFormFactory.Invoke())
            {
                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    DateTime selectedDate = dateForm.SelectedDate;
                    _paymentDateService.UpdateNextPaymentDate(selectedDate);

                    MessageBox.Show($"✅ სწავლის დაწყების თარიღი შენახულია: {selectedDate:yyyy-MM-dd}",
                        "დადასტურება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void ApplyDiscount()
        {
            decimal discountPercentage = 0;

            if (!string.IsNullOrWhiteSpace(cmbDiscount.Text) &&
                decimal.TryParse(cmbDiscount.Text.Replace("%", ""), out discountPercentage))
            {
                if (Convert.ToInt16(cmbDiscount.Text) > 0)
                    txtStudentInfo.Enabled = true;
                else
                    txtStudentInfo.Enabled = false;
                decimal finalFee = _studentService.CalculateFinalFee(baseFee, discountPercentage);
                lblTuitionFee.Text = finalFee.ToString();

            }
        }
        private void cmbDiscount_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDiscount();
        }
        private void btnExportToExcell_Click(object sender, EventArgs e)
        {
            ExportStudentsToExcel();
        }
        public void ExportStudentsToExcel()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "ფაილის შენახვა";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _studentExportService.ExportStudentsToExcel(saveFileDialog.FileName);
                        MessageBox.Show("✅ ფაილი წარმატებით შეინახა!", "დადასტურება");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"შეცდომა ექსპორტისას: {ex.Message}", "შეცდომა");
                    }
                }
            }
        }
        private void btnImportFromExcell_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanImport))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var importForm = _importFormFactory.Invoke();
            importForm.ShowDialog();
        }
        private void clbGroups_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            decimal totalBaseFee = 0;

            // ვამოწმებთ ყველა მონიშნულ ჯგუფს (მათ შორის ახლად მონიშნულს)
            for (int i = 0; i < clbGroups.Items.Count; i++)
            {
                if (clbGroups.Items[i] is Group group) // ვამოწმებთ, რომ ელემენტი Group ტიპისაა
                {
                    // ვამოწმებთ არა CheckedItems-ს, არამედ GetItemChecked(i)-ს გარდა იმ ელემენტისა, რომელიც იცვლება
                    bool isChecked = (i == e.Index) ? (e.NewValue == CheckState.Checked) : clbGroups.GetItemChecked(i);

                    if (isChecked)
                    {
                        totalBaseFee += group.Price; // ✅ ჯგუფის ფასს ვუმატებთ
                    }
                }
            }
            numTuitionFee.Text = totalBaseFee.ToString();
            baseFee = totalBaseFee; // ვანახლებთ საერთო თანხას

            ApplyDiscount(); // ფასდაკლების გამოთვლა
        }
        private void tsmFailedStudents_Click(object sender, EventArgs e)
        {
            var failedStudentsForm = _failedStudentsFormFactory.Invoke();
            failedStudentsForm.ShowDialog();
        }
        private void ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanViewReports) &&
                !_userContext.HasPermission(Permission.CanManageStudents))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var approvalForm = _pendingStudentsFormFactory.Invoke();
            approvalForm.ShowDialog();

        }
        private void btnSearchFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtStudentDocPath.Text = folderBrowser.SelectedPath;
        }
        private void მოსწავლისრედაქტირებაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditStudents) &&
                !_userContext.HasPermission(Permission.CanManageStudents))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var studentsEditForm = _studentsEditFormFactory.Invoke();
            studentsEditForm.ShowDialog();
        }
        private void LinklblRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadGroups();
        }
    }
}

