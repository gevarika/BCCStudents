using BCCStudents.Domain.Interfaces;
using System.Data;

namespace BCCStudents.Presentation
{
    public partial class PaymentImportHistoryForm : Form
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IExcelPaymentImportService _importService;
        private DataTable _successfulPayments;
        private DataTable _failedPayments;

        public PaymentImportHistoryForm(IPaymentRepository paymentRepository, IExcelPaymentImportService importService)
        {
            InitializeComponent();
            _paymentRepository = paymentRepository;
            _importService = importService;

            InitializeForm();
            SetupDataGridViews();
            LoadData();
        }

        private void InitializeForm()
        {
            FormTitleHelper.SetTitle(this, "გადახდების იმპორტის ისტორია");

            // ტაბების ინიციალიზაცია
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // განახლების ღილაკი
            btnRefresh.Click += BtnRefresh_Click;

            // ფილტრის ველები
            dtpFrom.Value = DateTime.Today.AddMonths(-1);
            dtpTo.Value = DateTime.Today;
            dtpFrom.ValueChanged += Filter_Changed;
            dtpTo.ValueChanged += Filter_Changed;

            // ძიების ველის ინიციალიზაცია
            txtSearch.Text = "ძიება...";
            txtSearch.ForeColor = Color.Gray;

            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "ძიება...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };

            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "ძიება...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text != "ძიება...")
                {
                    FilterPayments();
                }
            };
        }

        private void SetupDataGridViews()
        {
            // წარმატებული გადახდების ცხრილი
            dgvSuccessful.AutoGenerateColumns = false;
            dgvSuccessful.Columns.Clear();

            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentDate",
                HeaderText = "თარიღი",
                DataPropertyName = "PaymentDate",
                Width = 100
            });

            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "თანხა",
                DataPropertyName = "Amount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentName",
                HeaderText = "მოსწავლე",
                DataPropertyName = "StudentName",
                Width = 200
            });

            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GroupName",
                HeaderText = "ჯგუფი",
                DataPropertyName = "GroupName",
                Width = 150
            });

            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "აღწერა",
                DataPropertyName = "Description",
                Width = 300
            });

            // ვერ შესრულებული გადახდების ცხრილი
            dgvFailed.AutoGenerateColumns = false;
            dgvFailed.Columns.Clear();

            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RowNumber",
                HeaderText = "სტრიქონი",
                DataPropertyName = "RowNumber",
                Width = 80
            });

            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentDate",
                HeaderText = "თარიღი",
                DataPropertyName = "PaymentDate",
                Width = 100
            });

            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "თანხა",
                DataPropertyName = "Amount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });

            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PersonalId",
                HeaderText = "პირადი ნომერი",
                DataPropertyName = "PersonalId",
                Width = 120
            });

            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "აღწერა",
                DataPropertyName = "Description",
                Width = 300
            });

            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Reason",
                HeaderText = "მიზეზი",
                DataPropertyName = "Reason",
                Width = 200
            });
        }

        private void LoadData()
        {
            try
            {
                // წარმატებული გადახდების ჩატვირთვა
                var successfulPayments = _paymentRepository.GetImportHistory()
                    .Where(p => p.NextPaymentDate >= dtpFrom.Value && p.NextPaymentDate <= dtpTo.Value)
                    .ToList();

                _successfulPayments = new DataTable();
                _successfulPayments.Columns.Add("PaymentDate", typeof(DateTime));
                _successfulPayments.Columns.Add("Amount", typeof(decimal));
                _successfulPayments.Columns.Add("StudentName", typeof(string));
                _successfulPayments.Columns.Add("GroupName", typeof(string));
                _successfulPayments.Columns.Add("Description", typeof(string));

                foreach (var payment in successfulPayments)
                {
                    _successfulPayments.Rows.Add(
                        payment.NextPaymentDate,
                        payment.TotalPaid,
                        $"{payment.LastName} {payment.FirstName}",
                        payment.GroupName,
                        payment.Description
                    );
                }

                dgvSuccessful.DataSource = _successfulPayments;

                // ვერ შესრულებული გადახდების ჩატვირთვა
                var failedPayments = _importService.GetFailedPayments();
                /*.Where(p => p.PaymentDate >= dtpFrom.Value && p.PaymentDate <= dtpTo.Value)
                .ToList();*/

                _failedPayments = new DataTable();
                _failedPayments.Columns.Add("RowNumber", typeof(int));
                _failedPayments.Columns.Add("PaymentDate", typeof(DateTime));
                _failedPayments.Columns.Add("Amount", typeof(decimal));
                _failedPayments.Columns.Add("PersonalId", typeof(long));
                _failedPayments.Columns.Add("Description", typeof(string));
                _failedPayments.Columns.Add("Reason", typeof(string));

                foreach (var payment in failedPayments)
                {
                    _failedPayments.Rows.Add(
                        payment.RowNumber,
                        payment.PaymentDate,
                        payment.Amount,
                        payment.PersonalId ?? 0,
                        payment.Description ?? "",
                        payment.Reason ?? ""
                    );
                }

                dgvFailed.DataSource = _failedPayments;

                // განვაახლოთ სტატისტიკა
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა მონაცემების ჩატვირთვის დროს: {ex.Message}",
                    "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatistics()
        {
            var totalSuccessful = _successfulPayments?.Rows.Count ?? 0;
            var totalFailed = _failedPayments?.Rows.Count ?? 0;
            var totalAmount = _successfulPayments?.AsEnumerable()
                .Sum(row => row.Field<decimal>("Amount")) ?? 0;

            lblStatistics.Text = $"წარმატებული: {totalSuccessful} | ვერ შესრულებული: {totalFailed} | ჯამური თანხა: {totalAmount:N2} ₾";
        }

        private void FilterPayments()
        {
            try
            {
                var searchText = txtSearch.Text?.ToLower() ?? "";
                var fromDate = dtpFrom.Value.Date;
                var toDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1); // დავამატოთ დღის ბოლომდე

                // მივიღოთ ყველა გადახდა
                var allPayments = _paymentRepository.GetAllPayments();

                // გავფილტროთ გადახდები
                var filteredPayments = allPayments.Where(p =>
                {
                    // ძიების ტექსტის ფილტრაცია
                    if (!string.IsNullOrWhiteSpace(searchText) && searchText != "ძიება...")
                    {
                        var matchesSearch =
                            (p.Description?.ToLower().Contains(searchText) ?? false) ||
                            (p.PayerName?.ToLower().Contains(searchText) ?? false) ||
                            (p.PersonalId?.ToString().Contains(searchText) ?? false) ||
                            (p.Amount.ToString().Contains(searchText));

                        if (!matchesSearch) return false;
                    }

                    // თარიღების ფილტრაცია
                    if (p.PaymentDate < fromDate || p.PaymentDate > toDate)
                        return false;

                    return true;
                }).ToList();

                // განვაახლოთ წარმატებული გადახდების ცხრილი
                _successfulPayments.Clear();
                foreach (var payment in filteredPayments)
                {
                    _successfulPayments.Rows.Add(
                        payment.PaymentDate,
                        payment.Amount,
                        payment.PayerName ?? "უცნობი",
                        payment.GroupName ?? "უცნობი",
                        payment.Description ?? ""
                    );
                }

                // განვაახლოთ სტატისტიკა
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა გადახდების ფილტრაციის დროს: {ex.Message}",
                    "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatusLabel(int count)
        {
            // ამ მეთოდს ვაშორებთ, რადგან lblStatus არ არსებობს
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            FilterPayments();
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatistics();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
