using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Interfaces;

namespace BCCStudents.Presentation
{
    public partial class PaymentTestForm : Form
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IPaymentService _paymentService;
        private int _selectedStudentId = -1;
        private int _selectedGroupId = -1;

        public PaymentTestForm(
            IStudentRepository studentRepository,
            IStudentGroupRepository studentGroupRepository,
            IGroupRepository groupRepository,
            PaymentService paymentService)
        {
            InitializeComponent();
            _studentRepository = studentRepository;
            _studentGroupRepository = studentGroupRepository;
            _groupRepository = groupRepository;
            _paymentService = paymentService;

            // DataGridView-ის სვეტების დაყენება
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dgvGroups.Columns.Clear();
            dgvGroups.Columns.Add("GroupId", "ჯგუფის ID");
            dgvGroups.Columns.Add("GroupName", "ჯგუფის სახელი");
            dgvGroups.Columns.Add("DateOfPayment", "გადახდის თარიღი");
            dgvGroups.Columns.Add("PaymentStatus", "გადახდის სტატუსი");
            dgvGroups.Columns.Add("Price", "ფასი");
            dgvGroups.Columns.Add("Discount", "ფასდაკლება %");
            dgvGroups.Columns.Add("IsActive", "აქტიური");
            dgvGroups.SelectionChanged += DgvGroups_SelectionChanged;
        }

        private void PaymentTestForm_Load(object sender, EventArgs e)
        {
            LoadStudents();
            dtpNewDate.Value = DateTime.Today; // ნაგულისხმევად დღევანდელი
            AddLog("----------------------------------------");
            AddLog("გადახდის ტესტირების ფორმა ჩაიტვირთა");
            AddLog($"მიმდინარე თარიღი: {DateTime.Today:dd.MM.yyyy}");
            AddLog("----------------------------------------");
            AddLog("");
            AddLog("ინსტრუქცია:");
            AddLog("1. აირჩიეთ მოსწავლე ComboBox-დან");
            AddLog("2. აირჩიეთ ჯგუფი ცხრილიდან");
            AddLog("3. შეცვალეთ DateOfPayment (მაგ: დღევანდელით ან წარსულში)");
            AddLog("4. დააჭირეთ 'თარიღის განახლება'");
            AddLog("5. დააჭირეთ 'გადახდა' ან 'ავტომატური გადახდა'");
            AddLog("");
        }

        private void LoadStudents()
        {
            try
            {
                cmbStudents.Items.Clear();
                var students = _studentRepository.GetAllActiveStudents();

                foreach (var student in students.OrderBy(s => s.FirstName).ThenBy(s => s.LastName))
                {
                    var item = new ComboBoxItem
                    {
                        Text = $"{student.FirstName} {student.LastName} (ID: {student.Id}, Balance: {student.Balance} ლარი)",
                        Value = student.Id
                    };
                    cmbStudents.Items.Add(item);
                }

                if (cmbStudents.Items.Count > 0)
                {
                    cmbStudents.SelectedIndex = 0;
                }

                AddLog($"ჩაიტვირთა {students.Count} აქტიური მოსწავლე");
            }
            catch (Exception ex)
            {
                AddLog($"შეცდომა მოსწავლეების ჩატვირთვისას: {ex.Message}");
                MessageBox.Show($"შეცდომა მოსწავლეების ჩატვირთვისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedItem == null) return;

            _selectedStudentId = ((ComboBoxItem)cmbStudents.SelectedItem).Value;
            LoadStudentGroups();
        }

        private void LoadStudentGroups()
        {
            try
            {
                dgvGroups.Rows.Clear();

                if (_selectedStudentId <= 0)
                {
                    AddLog("მოსწავლე არ არის არჩეული");
                    return;
                }

                var student = _studentRepository.GetStudentById(_selectedStudentId);
                if (student == null)
                {
                    AddLog($"მოსწავლე ID {_selectedStudentId} ვერ მოიძებნა");
                    return;
                }

                var groups = _studentGroupRepository.GetForPayment(_selectedStudentId);

                if (groups == null || !groups.Any())
                {
                    AddLog($"მოსწავლეს {student.FirstName} {student.LastName} არ აქვს აქტიური ჯგუფები");
                    return;
                }

                foreach (var group in groups)
                {
                    var groupName = _groupRepository.GetGroupNameById(group.GroupId) ?? "უცნობი";
                    var dateStr = group.DateOfPayment.HasValue
                        ? group.DateOfPayment.Value.ToString("dd.MM.yyyy")
                        : "არ არის";

                    var isOverdue = group.DateOfPayment.HasValue && group.DateOfPayment.Value <= DateTime.Today;
                    var statusColor = isOverdue ? "OVERDUE" : "OK";

                    int rowIndex = dgvGroups.Rows.Add(
                        group.GroupId,
                        groupName,
                        dateStr,
                        group.PaymentStatus ?? "უცნობი",
                        $"{group.Price:F2} ლარი",
                        $"{group.Discount:F1}%",
                        group.Status ? "დიახ" : "არა"
                    );

                    // თუ გადახდის დრო დადგა, მონიშნე წითლად
                    if (isOverdue)
                    {
                        dgvGroups.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    }
                }

                AddLog($"ჩაიტვირთა {groups.Count} ჯგუფი მოსწავლისთვის: {student.FirstName} {student.LastName}");
                AddLog($"ბალანსი: {student.Balance} ლარი");
            }
            catch (Exception ex)
            {
                AddLog($"შეცდომა ჯგუფების ჩატვირთვისას: {ex.Message}");
                MessageBox.Show($"შეცდომა ჯგუფების ჩატვირთვისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                _selectedGroupId = -1;
                lblCurrentDate.Text = "მიმდინარე თარიღი: -";
                return;
            }

            var selectedRow = dgvGroups.SelectedRows[0];
            _selectedGroupId = Convert.ToInt32(selectedRow.Cells["GroupId"].Value);

            var dateStr = selectedRow.Cells["DateOfPayment"].Value?.ToString();
            if (!string.IsNullOrEmpty(dateStr) && dateStr != "არ არის")
            {
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    lblCurrentDate.Text = $"მიმდინარე თარიღი: {date:dd.MM.yyyy}";
                    dtpNewDate.Value = date;
                }
                else
                {
                    lblCurrentDate.Text = "მიმდინარე თარიღი: -";
                }
            }
            else
            {
                lblCurrentDate.Text = "მიმდინარე თარიღი: არ არის დაყენებული";
                dtpNewDate.Value = DateTime.Today;
            }
        }

        private void btnUpdateDate_Click(object sender, EventArgs e)
        {
            if (_selectedStudentId <= 0 || _selectedGroupId <= 0)
            {
                MessageBox.Show("გთხოვთ, აირჩიოთ მოსწავლე და ჯგუფი!", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newDate = dtpNewDate.Value.Date;
                bool success = _studentGroupRepository.UpdateDateOfPayment(_selectedStudentId, _selectedGroupId, newDate);

                if (success)
                {
                    AddLog($"თარიღი განახლდა: {newDate:dd.MM.yyyy}");
                    AddLog($"   მოსწავლე ID: {_selectedStudentId}, ჯგუფი ID: {_selectedGroupId}");

                    // შევამოწმოთ, დადგა თუ არა გადახდის დრო
                    if (newDate <= DateTime.Today)
                    {
                        AddLog($"გადახდის დრო დადგა! (თარიღი <= {DateTime.Today:dd.MM.yyyy})");
                    }
                    else
                    {
                        AddLog($"გადახდის დრო ჯერ არ დადგა (თარიღი > {DateTime.Today:dd.MM.yyyy})");
                    }

                    MessageBox.Show(
                        $"თარიღი წარმატებით განახლდა!\n\n" +
                        $"ახალი თარიღი: {newDate:dd.MM.yyyy}\n" +
                        (newDate <= DateTime.Today ? "გადახდის დრო დადგა!" : "გადახდის დრო ჯერ არ დადგა"),
                        "წარმატება",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadStudentGroups(); // განახლება
                }
                else
                {
                    AddLog("თარიღის განახლება ვერ მოხერხდა");
                    MessageBox.Show("თარიღის განახლება ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AddLog($"შეცდომა: {ex.Message}");
                MessageBox.Show($"შეცდომა თარიღის განახლებისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRunPayment_Click(object sender, EventArgs e)
        {
            if (_selectedStudentId <= 0)
            {
                MessageBox.Show("გთხოვთ, აირჩიოთ მოსწავლე!", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var student = _studentRepository.GetStudentById(_selectedStudentId);
                if (student == null)
                {
                    MessageBox.Show("მოსწავლე ვერ მოიძებნა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (student.Balance <= 0)
                {
                    MessageBox.Show(
                        $"მოსწავლეს არ აქვს ბალანსი!\n\n" +
                        $"ბალანსი: {student.Balance} ლარი\n\n" +
                        $"გთხოვთ, ჯერ დაუმატოთ თანხა ბალანსზე.",
                        "გაფრთხილება",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                AddLog("");
                AddLog("----------------------------------------");
                AddLog($"გადახდის დაწყება მოსწავლისთვის: {student.FirstName} {student.LastName}");
                AddLog($"ბალანსი: {student.Balance} ლარი");
                AddLog("ჩარიცხული თანხა: 0 ლარი (მხოლოდ ბალანსიდან)");
                AddLog("----------------------------------------");
                AddLog("");

                // paymentAmount = 0 -> მხოლოდ ბალანსიდან
                var result = await _paymentService.ProcessPayment(_selectedStudentId, 0);

                AddLog("");
                AddLog("----------------------------------------");
                AddLog($"გადახდის შედეგი: {result.Status}");
                AddLog("----------------------------------------");
                AddLog("");

                foreach (var log in result.Logs)
                {
                    AddLog(log);
                }

                // განახლებული ინფორმაცია
                var updatedStudent = _studentRepository.GetStudentById(_selectedStudentId);
                if (updatedStudent != null)
                {
                    AddLog("");
                    AddLog($"განახლებული ბალანსი: {updatedStudent.Balance} ლარი");
                }

                MessageBox.Show(
                    $"გადახდის შედეგი: {result.Status}\n\n" +
                    $"დეტალური ინფორმაცია ნაჩვენებია ლოგებში.",
                    "გადახდა დასრულდა",
                    MessageBoxButtons.OK,
                    result.Status == "Paid" || result.Status == "Partial"
                        ? MessageBoxIcon.Information
                        : MessageBoxIcon.Warning
                );

                LoadStudentGroups(); // განახლება
            }
            catch (Exception ex)
            {
                AddLog($"შეცდომა: {ex.Message}");
                AddLog($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"შეცდომა გადახდისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRunAutoPayments_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "გსურთ გაუშვათ ავტომატური გადახდა ყველა აქტიური მოსწავლისთვის?\n\n" +
                "ეს პროცესი ამუშავებს ყველა მოსწავლეს, ვისაც აქვს Balance > 0.",
                "დადასტურება",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                AddLog("");
                AddLog("----------------------------------------");
                AddLog("ავტომატური გადახდის დაწყება ყველა მოსწავლისთვის");
                AddLog("----------------------------------------");
                AddLog("");

                int processed = 0;
                int total = 0;

                bool useFullBalance = Properties.Settings.Default.UseFullBalanceForAutoPayment;
                await _paymentService.ProcessAutoPaymentsForAllStudentsAsync((current, totalCount) =>
                {
                    processed = current;
                    total = totalCount;
                    this.Invoke((Action)(() =>
                    {
                        AddLog($"პროგრესი: {current} / {totalCount} მოსწავლე");
                    }));
                });

                AddLog("");
                AddLog("----------------------------------------");
                AddLog("ავტომატური გადახდა დასრულდა!");
                AddLog($"დამუშავებული: {processed} / {total} მოსწავლე");
                AddLog("----------------------------------------");

                MessageBox.Show(
                    $"ავტომატური გადახდა დასრულდა!\n\n" +
                    $"დამუშავებული: {processed} / {total} მოსწავლე",
                    "დასრულება",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadStudentGroups(); // განახლება
            }
            catch (Exception ex)
            {
                AddLog($"შეცდომა: {ex.Message}");
                AddLog($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"შეცდომა ავტომატური გადახდისას:\n{ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStudents();
            if (_selectedStudentId > 0)
            {
                LoadStudentGroups();
            }
            AddLog("მონაცემები განახლდა");
        }

        private void AddLog(string message)
        {
            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke((Action)(() => AddLog(message)));
                return;
            }

            txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLogs.ScrollToCaret();
        }

        // ComboBoxItem კლასი
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString() => Text;
        }
    }
}


