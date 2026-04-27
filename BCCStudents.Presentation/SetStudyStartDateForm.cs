using BCCStudents.Application.Interfaces;

namespace BCCStudents.Presentation
{
    public partial class SetStudyStartDateForm : Form
    {
        public DateTime SelectedDate { get; private set; }
        private readonly IPaymentDateService _paymentDateService;

        public SetStudyStartDateForm(IPaymentDateService paymentDateService = null)
        {
            InitializeComponent();
            FormTitleHelper.SetTitle(this, "სწავლის დაწყების თარიღის დაყენება");
            _paymentDateService = paymentDateService;
            dtpStartDate.Value = DateTime.Today;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SelectedDate = dtpStartDate.Value.Date;
            // მხოლოდ თარიღის დაყენება, მოსწავლეების გადახდის თარიღების განახლება MainForm-ში ხდება
            DialogResult = DialogResult.OK;
            Close();
        }

        /*private void btnSave_Click(object sender, EventArgs e)
        {
            SelectedDate = dtpStartDate.Value;
            // 🟢 4. განაახლე გადახდის დაწყების თარიღი
            DatabaseHelper.UpdatePaymentStartDate(SelectedDate.AddMonths(1));
            //PaymentDateManager.SaveNextPaymentDate(SelectedDate.AddMonths(1));
            DialogResult = DialogResult.OK;
            Close();
        }*/
    }
}

