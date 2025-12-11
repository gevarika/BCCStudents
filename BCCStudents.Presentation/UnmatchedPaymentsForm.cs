using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Services;
using BCCStudents;

namespace BCCStudents.Presentation
{
    public partial class UnmatchedPaymentsForm : Form
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IExcelPaymentImportService _importService;
        private DataTable _failedPayments;

        public UnmatchedPaymentsForm(IPaymentRepository paymentRepository, IStudentRepository studentRepository, IExcelPaymentImportService importService)
        {
            InitializeComponent();
            _paymentRepository = paymentRepository;
            _studentRepository = studentRepository;
            _importService = importService;
            LoadFailedPayments();
        }

        private void LoadFailedPayments()
        {
            var failedPayments = _importService.GetFailedPayments();
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
                    payment.Description ?? string.Empty,
                    payment.Reason ?? string.Empty
                );
            }
            dgvFailedPayments.DataSource = _failedPayments;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadFailedPayments();
        }

        private void dgvFailedPayments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFailedPayments.SelectedRows.Count > 0)
            {
                var row = dgvFailedPayments.SelectedRows[0];
                txtDetails.Text =
                    $"áƒ¡áƒ¢áƒ áƒ˜áƒ¥áƒáƒœáƒ˜: {row.Cells["RowNumber"].Value}\n" +
                    $"áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {row.Cells["PaymentDate"].Value}\n" +
                    $"áƒ—áƒáƒœáƒ®áƒ: {row.Cells["Amount"].Value}\n" +
                    $"áƒžáƒ˜áƒ áƒáƒ“áƒ˜ áƒœáƒáƒ›áƒ”áƒ áƒ˜: {row.Cells["PersonalId"].Value}\n" +
                    $"áƒáƒ¦áƒ¬áƒ”áƒ áƒ: {row.Cells["Description"].Value}\n" +
                    $"áƒ›áƒ˜áƒ–áƒ”áƒ–áƒ˜: {row.Cells["Reason"].Value}";
            }
            else
            {
                txtDetails.Text = string.Empty;
            }
        }

        private void btnFindSimilar_Click(object sender, EventArgs e)
        {
            if (dgvFailedPayments.SelectedRows.Count == 0)
            {
                MessageBox.Show("áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ!", "áƒ’áƒáƒ¤áƒ áƒ—áƒ®áƒ˜áƒšáƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var row = dgvFailedPayments.SelectedRows[0];
            string description = row.Cells["Description"].Value?.ToString() ?? string.Empty;
            decimal amount = row.Cells["Amount"].Value != null ? Convert.ToDecimal(row.Cells["Amount"].Value) : 0;

            // áƒ›áƒáƒ«áƒ”áƒ‘áƒœáƒáƒ¡ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ”áƒ‘áƒ˜ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜áƒ—, áƒ’áƒ•áƒáƒ áƒ˜áƒ— áƒáƒœ áƒ—áƒáƒœáƒ®áƒ˜áƒ— áƒ›áƒ¡áƒ’áƒáƒ•áƒ¡áƒáƒ“
            var students = _studentRepository.GetAllStudents();
            var similarStudents = students.Where(s =>
                    (!string.IsNullOrEmpty(s.FirstName) && description.IndexOf(s.FirstName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.LastName) && description.IndexOf(s.LastName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (Math.Abs(s.TuitionFee - amount) < 0.01m)
                ).ToList();


            lstSimilarStudents.Items.Clear();
            foreach (var s in similarStudents)
            {
                lstSimilarStudents.Items.Add($"{s.FirstName} {s.LastName} | áƒžáƒ˜áƒ áƒáƒ“áƒ˜: {s.Id_Numb} | áƒ’áƒáƒ“áƒáƒ¡áƒáƒ®áƒáƒ“áƒ˜: {s.TuitionFee}");
            }
            if (similarStudents.Count == 0)
            {
                lstSimilarStudents.Items.Add("áƒ›áƒ¡áƒ’áƒáƒ•áƒ¡áƒ˜ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ");
            }
        }

        private void btnAttachToStudent_Click(object sender, EventArgs e)
        {
            if (dgvFailedPayments.SelectedRows.Count == 0 || lstSimilarStudents.SelectedIndex == -1)
            {
                MessageBox.Show("áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ“áƒ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜!", "áƒ’áƒáƒ¤áƒ áƒ—áƒ®áƒ˜áƒšáƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var row = dgvFailedPayments.SelectedRows[0];
            var studentInfo = lstSimilarStudents.SelectedItem.ToString();
            // áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜áƒ¡ áƒžáƒ˜áƒ áƒáƒ“áƒ˜ áƒœáƒáƒ›áƒ áƒ˜áƒ¡ áƒáƒ›áƒáƒ¦áƒ”áƒ‘áƒ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ“áƒáƒœ
            var idPart = studentInfo.Split('|').FirstOrDefault(x => x.Trim().StartsWith("áƒžáƒ˜áƒ áƒáƒ“áƒ˜:"));
            if (idPart == null)
            {
                MessageBox.Show("áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜áƒ¡ áƒ˜áƒ“áƒ”áƒœáƒ¢áƒ˜áƒ¤áƒ˜áƒ™áƒáƒªáƒ˜áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var personalId = Convert.ToInt64(idPart.Replace("áƒžáƒ˜áƒ áƒáƒ“áƒ˜:", "").Trim());
            var student = _studentRepository.GetStudentByPersonalId(personalId);
            if (student == null)
            {
                MessageBox.Show("áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ–áƒ” áƒáƒ¡áƒáƒ®áƒ•áƒ
            decimal amount = row.Cells["Amount"].Value != null ? Convert.ToDecimal(row.Cells["Amount"].Value) : 0;
            _studentRepository.UpdateStudentBalance(student.Id, amount);
            // áƒ©áƒáƒœáƒáƒ¬áƒ”áƒ áƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ FailedPayments-áƒ“áƒáƒœ
            DateTime paymentDate = (DateTime)row.Cells["PaymentDate"].Value;
            //long? personalId = row.Cells["PersonalId"].Value != null ? Convert.ToInt64(row.Cells["PersonalId"].Value) : (long?)null;
            string description = row.Cells["Description"].Value?.ToString();
            _paymentRepository.DeleteFailedPayment(paymentDate, amount, personalId, description);
            MessageBox.Show("áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ›áƒ˜áƒ”áƒœáƒ˜áƒ­áƒ áƒ¡áƒ¢áƒ£áƒ“áƒ”áƒœáƒ¢áƒ¡ áƒ“áƒ áƒ¬áƒáƒ˜áƒ¨áƒáƒšáƒ áƒ“áƒáƒ£áƒ“áƒáƒ¡áƒ¢áƒ£áƒ áƒ”áƒ‘áƒ”áƒš áƒ¡áƒ˜áƒ˜áƒ“áƒáƒœ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadFailedPayments();
        }
    }
} 
