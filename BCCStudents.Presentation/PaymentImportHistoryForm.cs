using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Services;
using BCCStudents;

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
            FormTitleHelper.SetTitle(this, "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒ˜áƒ¡áƒ¢áƒáƒ áƒ˜áƒ");
            
            // áƒ¢áƒáƒ‘áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            
            // áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¦áƒ˜áƒšáƒáƒ™áƒ˜
            btnRefresh.Click += BtnRefresh_Click;
            
            // áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜áƒ¡ áƒ•áƒ”áƒšáƒ”áƒ‘áƒ˜
            dtpFrom.Value = DateTime.Today.AddMonths(-1);
            dtpTo.Value = DateTime.Today;
            dtpFrom.ValueChanged += Filter_Changed;
            dtpTo.ValueChanged += Filter_Changed;
            
            // áƒ«áƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ•áƒ”áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ
            txtSearch.Text = "áƒ«áƒ˜áƒ”áƒ‘áƒ...";
            txtSearch.ForeColor = Color.Gray;
            
            txtSearch.Enter += (s, e) => {
                if (txtSearch.Text == "áƒ«áƒ˜áƒ”áƒ‘áƒ...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            
            txtSearch.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "áƒ«áƒ˜áƒ”áƒ‘áƒ...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };
            
            txtSearch.TextChanged += (s, e) => {
                if (txtSearch.Text != "áƒ«áƒ˜áƒ”áƒ‘áƒ...")
                {
                    FilterPayments();
                }
            };
        }

        private void SetupDataGridViews()
        {
            // áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒªáƒ®áƒ áƒ˜áƒšáƒ˜
            dgvSuccessful.AutoGenerateColumns = false;
            dgvSuccessful.Columns.Clear();
            
            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentDate",
                HeaderText = "áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜",
                DataPropertyName = "PaymentDate",
                Width = 100
            });
            
            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "áƒ—áƒáƒœáƒ®áƒ",
                DataPropertyName = "Amount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
            
            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StudentName",
                HeaderText = "áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”",
                DataPropertyName = "StudentName",
                Width = 200
            });
            
            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GroupName",
                HeaderText = "áƒ¯áƒ’áƒ£áƒ¤áƒ˜",
                DataPropertyName = "GroupName",
                Width = 150
            });
            
            dgvSuccessful.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "áƒáƒ¦áƒ¬áƒ”áƒ áƒ",
                DataPropertyName = "Description",
                Width = 300
            });

            // áƒ•áƒ”áƒ  áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒªáƒ®áƒ áƒ˜áƒšáƒ˜
            dgvFailed.AutoGenerateColumns = false;
            dgvFailed.Columns.Clear();
            
            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RowNumber",
                HeaderText = "áƒ¡áƒ¢áƒ áƒ˜áƒ¥áƒáƒœáƒ˜",
                DataPropertyName = "RowNumber",
                Width = 80
            });
            
            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentDate",
                HeaderText = "áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜",
                DataPropertyName = "PaymentDate",
                Width = 100
            });
            
            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "áƒ—áƒáƒœáƒ®áƒ",
                DataPropertyName = "Amount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
            
            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PersonalId",
                HeaderText = "áƒžáƒ˜áƒ áƒáƒ“áƒ˜ áƒœáƒáƒ›áƒ”áƒ áƒ˜",
                DataPropertyName = "PersonalId",
                Width = 120
            });
            
            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "áƒáƒ¦áƒ¬áƒ”áƒ áƒ",
                DataPropertyName = "Description",
                Width = 300
            });
            
            dgvFailed.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Reason",
                HeaderText = "áƒ›áƒ˜áƒ–áƒ”áƒ–áƒ˜",
                DataPropertyName = "Reason",
                Width = 200
            });
        }

        private void LoadData()
        {
            try
            {
                // áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ
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

                // áƒ•áƒ”áƒ  áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ
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

                // áƒ’áƒáƒœáƒ•áƒáƒáƒ®áƒšáƒáƒ— áƒ¡áƒ¢áƒáƒ¢áƒ˜áƒ¡áƒ¢áƒ˜áƒ™áƒ
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ›áƒáƒœáƒáƒªáƒ”áƒ›áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatistics()
        {
            var totalSuccessful = _successfulPayments?.Rows.Count ?? 0;
            var totalFailed = _failedPayments?.Rows.Count ?? 0;
            var totalAmount = _successfulPayments?.AsEnumerable()
                .Sum(row => row.Field<decimal>("Amount")) ?? 0;

            lblStatistics.Text = $"áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ£áƒšáƒ˜: {totalSuccessful} | áƒ•áƒ”áƒ  áƒ¨áƒ”áƒ¡áƒ áƒ£áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜: {totalFailed} | áƒ¯áƒáƒ›áƒ£áƒ áƒ˜ áƒ—áƒáƒœáƒ®áƒ: {totalAmount:N2} â‚¾";
        }

        private void FilterPayments()
        {
            try
            {
                var searchText = txtSearch.Text?.ToLower() ?? "";
                var fromDate = dtpFrom.Value.Date;
                var toDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1); // áƒ“áƒáƒ•áƒáƒ›áƒáƒ¢áƒáƒ— áƒ“áƒ¦áƒ˜áƒ¡ áƒ‘áƒáƒšáƒáƒ›áƒ“áƒ”

                // áƒ›áƒ˜áƒ•áƒ˜áƒ¦áƒáƒ— áƒ§áƒ•áƒ”áƒšáƒ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ
                var allPayments = _paymentRepository.GetAllPayments();

                // áƒ’áƒáƒ•áƒ¤áƒ˜áƒšáƒ¢áƒ áƒáƒ— áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜
                var filteredPayments = allPayments.Where(p =>
                {
                    // áƒ«áƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ¤áƒ˜áƒšáƒ¢áƒ áƒáƒªáƒ˜áƒ
                    if (!string.IsNullOrWhiteSpace(searchText) && searchText != "áƒ«áƒ˜áƒ”áƒ‘áƒ...")
                    {
                        var matchesSearch = 
                            (p.Description?.ToLower().Contains(searchText) ?? false) ||
                            (p.PayerName?.ToLower().Contains(searchText) ?? false) ||
                            (p.PersonalId?.ToString().Contains(searchText) ?? false) ||
                            (p.Amount.ToString().Contains(searchText));

                        if (!matchesSearch) return false;
                    }

                    // áƒ—áƒáƒ áƒ˜áƒ¦áƒ”áƒ‘áƒ˜áƒ¡ áƒ¤áƒ˜áƒšáƒ¢áƒ áƒáƒªáƒ˜áƒ
                    if (p.PaymentDate < fromDate || p.PaymentDate > toDate)
                        return false;

                    return true;
                }).ToList();

                // áƒ’áƒáƒœáƒ•áƒáƒáƒ®áƒšáƒáƒ— áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒªáƒ®áƒ áƒ˜áƒšáƒ˜
                _successfulPayments.Clear();
                foreach (var payment in filteredPayments)
                {
                    _successfulPayments.Rows.Add(
                        payment.PaymentDate,
                        payment.Amount,
                        payment.PayerName ?? "áƒ£áƒªáƒœáƒáƒ‘áƒ˜",
                        payment.GroupName ?? "áƒ£áƒªáƒœáƒáƒ‘áƒ˜",
                        payment.Description ?? ""
                    );
                }

                // áƒ’áƒáƒœáƒ•áƒáƒáƒ®áƒšáƒáƒ— áƒ¡áƒ¢áƒáƒ¢áƒ˜áƒ¡áƒ¢áƒ˜áƒ™áƒ
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ”áƒ‘áƒ˜áƒ¡ áƒ¤áƒ˜áƒšáƒ¢áƒ áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStatusLabel(int count)
        {
            // áƒáƒ› áƒ›áƒ”áƒ—áƒáƒ“áƒ¡ áƒ•áƒáƒ¨áƒáƒ áƒ”áƒ‘áƒ—, áƒ áƒáƒ“áƒ’áƒáƒœ lblStatus áƒáƒ  áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡
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
