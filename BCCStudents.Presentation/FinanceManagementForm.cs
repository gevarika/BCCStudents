using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Presentation.Properties;
using BCCStudents.Application.Services;

using Microsoft.Extensions.DependencyInjection;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    public partial class FinanceManagementForm : Form
    {
        private readonly PaymentService _paymentService;
        private readonly IServiceProvider _serviceProvider;
        public FinanceManagementForm(PaymentService paymentService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _paymentService = paymentService;
            _serviceProvider = serviceProvider;
            if (!Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ფინანსების მენეჯერი");
            else FormTitleHelper.SetTitle(this, "ფინანსური მენეჯერი - სატესტო რეჟიმი");
            SetupDataGridView();
        }

        private void FinanceManagementForm_Load(object sender, EventArgs e)
        {
            LoadPaymentsData();
        }
        private void SetupDataGridView()
        {
            dgvPayments.Columns.Clear();
            dgvPayments.Columns.Add("StudentID", "სტუდენტის ID");
            dgvPayments.Columns.Add("FirstName", "სახელი");
            dgvPayments.Columns.Add("LastName", "გვარი");
            dgvPayments.Columns.Add("GroupName", "ჯგუფი");
            dgvPayments.Columns.Add("TuitionFee", "სწავლის საფასური");
            dgvPayments.Columns.Add("TotalPaid", "გადახდილი თანხა");
            dgvPayments.Columns.Add("AmountDue", "დავალიანება");
            dgvPayments.Columns.Add("NextPaymentDate", "შემდეგი გადახდის თარიღი");
            dgvPayments.Columns.Add("DaysRemaining", "დარჩენილი/გადაცილებული დღეები");
        }

        private void ApplyPaymentRowColor( DataGridViewRow row, PaymentSummary payment)
        {
            TimeSpan? remainingDays = payment.NextPaymentDate - DateTime.Today;

            if (payment.AmountDue <= 0)
            {
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
            else if (remainingDays?.TotalDays <= 7 && remainingDays?.TotalDays > 0)
            {
                row.DefaultCellStyle.BackColor = Color.LightYellow;
            }
            else if (remainingDays?.TotalDays <= 0)
            {
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
        }
        private void LoadPaymentsData()
        {
            /*var payments = _paymentService.GetPendingPayments();
            dgvPayments.Rows.Clear();

            var studyStartDate = StudyStartDateManager.GetStudyStartDate();
            // დავახარისხოთ გადაუხდელები თავში
            var sortedPayments = payments
                .OrderByDescending(p => p.AmountDue > 0)  // ჯერ ვისაც გადაუხდელი აქვს
                .ThenBy(p => p.NextPaymentDate ?? studyStartDate?.AddMonths(1))            // და შემდეგ თარიღის მიხედვით
                .ToList();

            foreach (var payment in sortedPayments)
            {
                int rowIndex = dgvPayments.Rows.Add();
                DataGridViewRow row = dgvPayments.Rows[rowIndex];

                row.Cells["StudentID"].Value = payment.StudentID;
                row.Cells["FirstName"].Value = payment.FirstName;
                row.Cells["LastName"].Value = payment.LastName;
                row.Cells["GroupName"].Value = payment.GroupName;
                row.Cells["TuitionFee"].Value = payment.TuitionFee;
                row.Cells["TotalPaid"].Value = payment.TotalPaid;
                row.Cells["AmountDue"].Value = payment.AmountDue;

                DateTime nextPaymentDate;

                if (payment.NextPaymentDate.HasValue)
                {
                    nextPaymentDate = payment.NextPaymentDate.Value;
                }
                else if (studyStartDate.HasValue)
                {
                    nextPaymentDate = studyStartDate.Value.AddMonths(1);
                }
                else
                {
                    nextPaymentDate = DateTime.Today.AddMonths(1); // fallback უსაფრთხოებისთვის
                }

                row.Cells["NextPaymentDate"].Value = payment.NextPaymentDate?.ToString("yyyy-MM-dd");

                // დარჩენილი ან გადაცილებული დღეები
                int daysRemaining = (nextPaymentDate.Date - DateTime.Today).Days;
                if (daysRemaining >= 0)
                    row.Cells["DaysRemaining"].Value = $"დარჩენილი: {daysRemaining} დღე";
                else
                    row.Cells["DaysRemaining"].Value = $"გადაცილებული: {Math.Abs(daysRemaining)} დღე";

                ApplyPaymentRowColor(row, payment);
            }*/
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPaymentsData();
        }

        private void გადახდებისისტორიაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = _serviceProvider.GetRequiredService<PaymentImportHistoryForm>())
            {
                form.ShowDialog(this);
            }
        }

        private void დაუდასტურებელიგადახდებიToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var unmatchedPaymentsForm = _serviceProvider.GetRequiredService<UnmatchedPaymentsForm>();
            unmatchedPaymentsForm.Show();
        }
    }
}


