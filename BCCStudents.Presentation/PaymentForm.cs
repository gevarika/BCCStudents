using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text;

namespace BCCStudents.Presentation
{
    public partial class PaymentForm : BaseForm
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IExcelPaymentImportService _excelPaymentImportService;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserContext _userContext;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        public PaymentForm(
            IServiceProvider serviceProvider,
            IPaymentService paymentService,
            IStudentService studentService,
            IExcelPaymentImportService excelPaymentImportService,
            IStudentRepository studentRepository,
            IStudentGroupRepository studentGroupRepository,
            IGroupRepository groupRepository,
            IUserContext userContext,
            IUpStreamChangeTracker upStreamChangeTracker)
        {
            InitializeComponent();
            FormTitleHelper.SetTitle(this, "გადახდის ხელით დაფიქსირება");
            _paymentService = paymentService;
            _studentService = studentService;
            _serviceProvider = serviceProvider;
            _excelPaymentImportService = excelPaymentImportService;
            _studentRepository = studentRepository;
            _studentGroupRepository = studentGroupRepository;
            _groupRepository = groupRepository;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));
            LoadStudents(); // მოსწავლეების ჩამოტვირთვა ComboBox-ში
            cmbStudents.SelectedIndexChanged += CmbStudents_SelectedIndexChanged;
            label1.Text = "მოსწავლე:";
            label2.Text = "თანხა:";

            // Apply security checks after form is loaded
            this.Load += PaymentForm_Load;
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            ApplySecurityChecks();
        }

        private void ApplySecurityChecks()
        {
            // btnConfirmPayment - CanAddPayments permission
            if (btnConfirmPayment != null)
            {
                btnConfirmPayment.Tag = $"Permission_{Permission.CanAddPayments}";
                btnConfirmPayment.Enabled = _userContext.HasPermission(Permission.CanAddPayments);
            }
        }
        private void LoadStudents()
        {
            var students = _studentService.GetStudentNames();
            foreach (var student in students)
            {
                cmbStudents.Items.Add(new ComboBoxItem(student.FullName, student.Id));
            }
        }

        private async void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanAddPayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbStudents.SelectedItem == null || string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("⚠️ გთხოვთ, შეავსოთ ყველა ველი!", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = ((ComboBoxItem)cmbStudents.SelectedItem).Value;
            decimal amount;
            if (!decimal.TryParse(txtAmount.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("⚠️ მიუთითეთ სწორი თანხა!", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ლოგების გასუფთავება
            txtLogs.Clear();
            AddLog("═══════════════════════════════════════════════════════");
            AddLog($"გადახდის პროცესის დაწყება");
            AddLog($"═══════════════════════════════════════════════════════");
            AddLog($"");

            // მოსწავლის ინფორმაცია
            var student = _studentRepository.GetStudentById(studentId);
            if (student != null)
            {
                AddLog($"მოსწავლე: {student.FirstName} {student.LastName} (ID: {studentId})");
                AddLog($"მიმდინარე ბალანსი: {student.Balance} ₾");
                AddLog($"ჩარიცხული თანხა: {amount} ₾");
                AddLog($"საერთო ბალანსი: {student.Balance + amount} ₾");
                AddLog($"");
            }

            // ⚠️ დროებითი ფუნქცია სატესტოდ: მხოლოდ ბალანსზე თანხის დამატება
            AddLog("ბალანსის განახლება (დროებითი ფუნქცია სატესტოდ)...");
            AddLog("");

            try
            {
                // მხოლოდ ბალანსის განახლება (ProcessPayment-ის გარეშე)
                bool success = _studentRepository.IncrementStudentBalance(studentId, amount);

                if (success)
                {
                    // განახლებული ინფორმაცია
                    var updatedStudent = _studentRepository.GetStudentById(studentId);
                    if (updatedStudent != null)
                    {
                        _upStreamChangeTracker.TrackStudentChange(studentId, SyncOperationType.Update, updatedStudent);
                        AddLog("═══════════════════════════════════════════════════════");
                        AddLog($"✅ ბალანსი წარმატებით განახლდა!");
                        AddLog($"ახალი ბალანსი: {updatedStudent.Balance} ₾");
                        AddLog("═══════════════════════════════════════════════════════");
                        AddLog("");
                        AddLog("⚠️ შენიშვნა: ეს არის დროებითი ფუნქცია სატესტოდ.");
                        AddLog("გადახდის სრული დამუშავება არ მოხდა (Payments ცხრილი, StudentGroups განახლება და ა.შ.)");

                        MessageBox.Show(
                            $"თანხა წარმატებით დაემატა ბალანსზე!\n\n" +
                            $"ძველი ბალანსი: {student.Balance} ₾\n" +
                            $"ჩარიცხული: {amount} ₾\n" +
                            $"ახალი ბალანსი: {updatedStudent.Balance} ₾\n\n" +
                            $"⚠️ შენიშვნა: ეს არის დროებითი ფუნქცია სატესტოდ.",
                            "წარმატება",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        RefreshStudentInfo(studentId);
                    }
                    else
                    {
                        AddLog("❌ შეცდომა: მოსწავლე ვერ მოიძებნა განახლების შემდეგ");
                        MessageBox.Show("ბალანსი განახლდა, მაგრამ მოსწავლის ინფორმაცია ვერ მოიძებნა.", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    AddLog("❌ შეცდომა: ბალანსის განახლება ვერ მოხერხდა");
                    MessageBox.Show("ბალანსის განახლება ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AddLog($"❌ შეცდომა: {ex.Message}");
                AddLog($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"შეცდომა ბალანსის განახლებისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ==================== კომენტარი: სრული გადახდის დამუშავებისთვის ====================
            // თუ გსურთ სრული გადახდის დამუშავება (Payments ცხრილი, StudentGroups განახლება და ა.შ.),
            // გამოიყენეთ ეს კოდი:
            /*
            var result = await _paymentService.ProcessPayment(studentId, amount);
            if (result.Status == "Paid" || result.Status == "Partial")
            {
                MessageBox.Show("გადახდა შესრულდა!\n\nდეტალური ინფორმაცია ნაჩვენებია ლოგებში.", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshStudentInfo(studentId);
            }
            else if (result.Status == "Credited")
            {
                MessageBox.Show("თანხა დაემატა ბალანსზე — როგორც კი გადახდის დრო დადგება, მოხდება ავტომატური გადახდა!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshStudentInfo(studentId);
            }
            else
            {
                MessageBox.Show("გადახდა ვერ შესრულდა!\n\nდეტალური ინფორმაცია ნაჩვენებია ლოგებში.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            */
        }

        private void AddLog(string message)
        {
            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke(new Action(() =>
                {
                    txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                    txtLogs.ScrollToCaret();
                }));
            }
            else
            {
                txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                txtLogs.ScrollToCaret();
            }
        }

        private void CmbStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedItem != null)
            {
                int studentId = ((ComboBoxItem)cmbStudents.SelectedItem).Value;
                RefreshStudentInfo(studentId);
            }
        }

        private void RefreshStudentInfo(int studentId)
        {
            var student = _studentRepository.GetStudentById(studentId);
            if (student != null)
            {
                lblBalance.Text = $"ბალანსი: {student.Balance} ₾";

                // ჯგუფების ინფორმაცია
                var groups = _studentGroupRepository.GetForPayment(studentId);
                var groupInfo = new StringBuilder();
                groupInfo.AppendLine($"მოსწავლე: {student.FirstName} {student.LastName}");
                groupInfo.AppendLine($"ბალანსი: {student.Balance} ₾");
                groupInfo.AppendLine("");
                groupInfo.AppendLine("აქტიური ჯგუფები:");

                if (groups != null && groups.Any())
                {
                    foreach (var group in groups.OrderBy(g => g.DateOfPayment))
                    {
                        var groupName = _groupRepository.GetGroupNameById(group.GroupId);
                        var discount = (group.Discount > 0) ? group.Price * (decimal)group.Discount / 100m : 0m;
                        var finalFee = group.Price - discount;
                        var dateStr = group.DateOfPayment.HasValue ? group.DateOfPayment.Value.ToString("dd.MM.yyyy") : "არ არის დაყენებული";
                        var statusStr = group.PaymentStatus ?? "Pending";

                        groupInfo.AppendLine($"  • {groupName} (ID: {group.GroupId})");
                        groupInfo.AppendLine($"    ფასი: {group.Price} ₾, ფასდაკლება: {discount} ₾ ({group.Discount}%), საბოლოო: {finalFee} ₾");
                        groupInfo.AppendLine($"    გადახდის თარიღი: {dateStr}");
                        groupInfo.AppendLine($"    სტატუსი: {statusStr}");
                        groupInfo.AppendLine("");
                    }
                }
                else
                {
                    groupInfo.AppendLine("  არ არის აქტიური ჯგუფები");
                }

                lblStudentInfo.Text = groupInfo.ToString();
            }
        }

        private void btnRefreshInfo_Click(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedItem != null)
            {
                int studentId = ((ComboBoxItem)cmbStudents.SelectedItem).Value;
                RefreshStudentInfo(studentId);
            }
            else
            {
                MessageBox.Show("გთხოვთ, აირჩიოთ მოსწავლე", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        class ComboBoxItem
        {
            public string Text { get; }
            public int Value { get; }

            public ComboBoxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var paymentsImportForm = _serviceProvider.GetRequiredService<PaymentsImportForm>();
            paymentsImportForm.ShowDialog();
        }
    }
}


