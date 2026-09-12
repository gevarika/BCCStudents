using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Data;

namespace BCCStudents.Presentation
{
    public partial class UnmatchedPaymentsForm : Form
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IExcelPaymentImportService _importService;
        private DataTable _failedPayments;

        public UnmatchedPaymentsForm(
            IPaymentRepository paymentRepository,
            IStudentRepository studentRepository,
            IExcelPaymentImportService importService)
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
                    $"სტრიქონი: {row.Cells["RowNumber"].Value}\n" +
                    $"თარიღი: {row.Cells["PaymentDate"].Value}\n" +
                    $"თანხა: {row.Cells["Amount"].Value}\n" +
                    $"პირადი ნომერი: {row.Cells["PersonalId"].Value}\n" +
                    $"აღწერა: {row.Cells["Description"].Value}\n" +
                    $"მიზეზი: {row.Cells["Reason"].Value}";
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
                MessageBox.Show("აირჩიეთ გადახდა!", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var row = dgvFailedPayments.SelectedRows[0];
            string description = row.Cells["Description"].Value?.ToString() ?? string.Empty;
            decimal amount = row.Cells["Amount"].Value != null ? Convert.ToDecimal(row.Cells["Amount"].Value) : 0;

            // მოძებნოს სტუდენტები სახელით, გვარით ან თანხით მსგავსად
            var students = _studentRepository.GetAllStudents();
            var similarStudents = students.Where(s =>
                    (!string.IsNullOrEmpty(s.FirstName) && description.IndexOf(s.FirstName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(s.LastName) && description.IndexOf(s.LastName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (Math.Abs(s.TuitionFee - amount) < 0.01m)
                ).ToList();


            lstSimilarStudents.Items.Clear();
            foreach (var s in similarStudents)
            {
                lstSimilarStudents.Items.Add($"{s.FirstName} {s.LastName} | პირადი: {s.Id_Numb} | გადასახადი: {s.TuitionFee}");
            }
            if (similarStudents.Count == 0)
            {
                lstSimilarStudents.Items.Add("მსგავსი სტუდენტი ვერ მოიძებნა");
            }
        }

        private void btnAttachToStudent_Click(object sender, EventArgs e)
        {
            if (dgvFailedPayments.SelectedRows.Count == 0 || lstSimilarStudents.SelectedIndex == -1)
            {
                MessageBox.Show("აირჩიეთ გადახდა და სტუდენტი!", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var row = dgvFailedPayments.SelectedRows[0];
            var studentInfo = lstSimilarStudents.SelectedItem.ToString();
            // სტუდენტის პირადი ნომრის ამოღება ტექსტიდან
            var idPart = studentInfo.Split('|').FirstOrDefault(x => x.Trim().StartsWith("პირადი:"));
            if (idPart == null)
            {
                MessageBox.Show("სტუდენტის იდენტიფიკაცია ვერ მოხერხდა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var personalId = Convert.ToInt64(idPart.Replace("პირადი:", "").Trim());
            var student = _studentRepository.GetStudentByPersonalId(personalId);
            if (student == null)
            {
                MessageBox.Show("სტუდენტი ვერ მოიძებნა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // გადახდის ბალანსზე ასახვა
            decimal amount = row.Cells["Amount"].Value != null ? Convert.ToDecimal(row.Cells["Amount"].Value) : 0;
            _studentRepository.UpdateStudentBalance(student.Id, amount);
            // ჩანაწერის წაშლა FailedPayments-დან
            DateTime paymentDate = (DateTime)row.Cells["PaymentDate"].Value;
            //long? personalId = row.Cells["PersonalId"].Value != null ? Convert.ToInt64(row.Cells["PersonalId"].Value) : (long?)null;
            string description = row.Cells["Description"].Value?.ToString();
            _paymentRepository.DeleteFailedPayment(paymentDate, amount, personalId, description);
            MessageBox.Show("გადახდა წარმატებით მიენიჭა სტუდენტს და წაიშალა დაუდასტურებელ სიიდან!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadFailedPayments();
        }
    }
}
