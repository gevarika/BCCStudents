using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCCStudents.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;

namespace BCCStudents.Presentation
{
    public partial class StudentManagementForm : Form
    {
        private readonly StudentService _studentService;
        private readonly GroupService _groupService;
        private readonly PaymentDateService _paymentDateService;
        private readonly SubGroupService _subGroupService;
        private readonly StudentExportService _studentExportService;
        //private readonly ImportService _importService;
        private readonly StudentCodeGenerator _studentCodeGenerator;
        private readonly IServiceProvider _serviceProvider;
        //UserSession _userSession = new UserSession();
        public StudentManagementForm(
            StudentService studentService,
            GroupService groupService, 
            SubGroupService subGroupService, 
            StudentCodeGenerator studentCodeGenerator,
            IServiceProvider serviceProvider,
            PaymentDateService paymentDateService,
            StudentExportService studentExportService
            //LoginForm loginForm
            )
        {
            InitializeComponent();
            _studentService = studentService;
            _groupService = groupService;
            _subGroupService = subGroupService;
            _studentCodeGenerator = studentCodeGenerator;
            _serviceProvider = serviceProvider;
            _paymentDateService = paymentDateService;
            _studentExportService = studentExportService;
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ახალი მოსწავლის რეგისტრაცია");
            else FormTitleHelper.SetTitle(this, "ახალი მოსწავლის რეგისტრაცია - სატესტო რეჟიმი");
        }
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
                if (Convert.ToInt16(cmbDiscount.Text) > 0 && string.IsNullOrWhiteSpace(txtStudentInfo.Text))
                    MessageBox.Show("თქვენ მოსწავლისთვის დააყენეთ ფასდაკლება, ამითომ ასევე საჭიროა მიუთითოთ მოსწავლის სტატუსი!", "მოსწავლის სტატუსი", MessageBoxButtons.OK);

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
                DateTime? studyStartDate = StudyStartDateManager.GetStudyStartDate().Value.AddMonths(1);
                
                // თუ ეს პირველი მოსწავლეა, ვამოწმებთ სწავლის დაწყების თარიღს
                if (existingStudentsCount == 0)
                {
                    var studyDate = StudyStartDateManager.GetStudyStartDate();
                    if (studyDate.HasValue)
                    {
                        var resultMessage = MessageBox.Show(
                            $"ეს არის პირველი მოსწავლე რეგისტრაცია.\n\n" +
                            $"სწავლის დაწყების თარიღი: {studyDate.Value:dd-MM-yyyy}\n" +
                            $"პირველი გადახდის თარიღი: {studyDate.Value.AddMonths(1):dd-MM-yyyy}\n\n" +
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
                        DateOfPayment = studyStartDate, // მთავარი ხაზი
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
               
                var selectedGroupIds = clbGroups.CheckedItems
                    .Cast<Group>()
                    .Select(g => g.Id)
                    .ToList();

                int studentId;
                bool success = _studentService.AddStudent(student, selectedGroupIds, UserSession.Id, chkPrintContract.Checked, result, out studentId);

                if (success)
                {
                    BackupManager.DbChangedSinceLastBackup = true;

                    MessageBox.Show("სტუდენტი წარმატებით დაემატა!", "დადასტურება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //ClearFields();
                    LoadStudents();
                    if (Properties.Settings.Default.SmsEnabled)
                    {
                        var smsService = new SmsService();
                        var message = Properties.Settings.Default.SmsText_Registration;
                        var smsresult = await smsService.SendSmsAsync(student.PhoneNumber, message);
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
        private void StudentManagementForm_Load(object sender, EventArgs e)
        {
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

            //_groupService.LoadGroupsFromDatabase(groupIds);
        }
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
            using (var dateForm = _serviceProvider.GetRequiredService<SetStudyStartDateForm>())
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
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "აირჩიეთ Excel ფაილი";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    // ფაილის არჩევის შემდეგ, გახსენით ImportForm
                    //ImportForm importForm = new ImportForm(filePath);
                    //importForm.ImportCompleted += ImportForm_ImportCompleted;
                    //importForm.ShowDialog();
                    using (var importForm = _serviceProvider.GetRequiredService<ImportForm>())
                    {
                        importForm.InitializeImport(filePath);
                        //importForm.ImportCompleted += ImportForm_ImportCompleted;
                        if (importForm.ShowDialog() == DialogResult.OK)
                        {
                            // იმპორტის შემდეგ განახლება
                            LoadStudents();
                        }
                    }
                }
                else if(openFileDialog.ShowDialog() == DialogResult.Cancel)
                    MessageBox.Show("მონაცემები გადმოტანილია!");
            }
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
            FailedStudentsForm failedStudentsForm = new FailedStudentsForm(_studentService);
            failedStudentsForm.ShowDialog();
        }
        private void ონლაინრეგისტრირებულიმოსწავლეებიToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var approvalForm = _serviceProvider.GetRequiredService<PendingStudentsForm>();
            approvalForm.ShowDialog();

        }

        private void btnSearchFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                txtStudentDocPath.Text = folderBrowser.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var testform = _serviceProvider.GetRequiredService<ImportTestForm>();
            testform.Show();
        }

        private void მოსწავლისრედაქტირებაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var studentsEditForm = _serviceProvider.GetRequiredService<StudentsEditForm>();
            studentsEditForm.ShowDialog();
        }
    }
}

