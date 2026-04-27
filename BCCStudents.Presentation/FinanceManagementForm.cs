using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Presentation.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace BCCStudents.Presentation
{
    public partial class FinanceManagementForm : Form
    {
        private readonly IPaymentService _paymentService;
        private readonly ISystemConfigurationService _systemConfigService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserContext _userContext;

        public FinanceManagementForm(IPaymentService paymentService, ISystemConfigurationService systemConfigService, IServiceProvider serviceProvider, IUserContext userContext)
        {
            InitializeComponent();
            _paymentService = paymentService;
            _systemConfigService = systemConfigService ?? throw new ArgumentNullException(nameof(systemConfigService));
            _serviceProvider = serviceProvider;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            if (!Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ფინანსების მენეჯერი");
            else FormTitleHelper.SetTitle(this, "ფინანსური მენეჯერი - სატესტო რეჟიმი");
            SetupDataGridView();
        }

        private void FinanceManagementForm_Load(object sender, EventArgs e)
        {
            LoadPaymentsData();
            ApplySecurityChecks();
        }

        private void ApplySecurityChecks()
        {
            // btnRefresh - CanManagePayments permission (viewing payment data)
            if (btnRefresh != null)
            {
                btnRefresh.Tag = $"Permission_{Permission.CanManagePayments}";
                btnRefresh.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
            }

            // გადახდებისისტორიაToolStripMenuItem - CanViewReports or CanManagePayments
            if (გადახდებისისტორიაToolStripMenuItem != null)
            {
                გადახდებისისტორიაToolStripMenuItem.Tag = $"Permission_{Permission.CanManagePayments}";
                გადახდებისისტორიაToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
            }

            // დაუდასტურებელიგადახდებიToolStripMenuItem - CanManagePayments
            if (დაუდასტურებელიგადახდებიToolStripMenuItem != null)
            {
                დაუდასტურებელიგადახდებიToolStripMenuItem.Tag = $"Permission_{Permission.CanManagePayments}";
                დაუდასტურებელიგადახდებიToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanManagePayments);
            }
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

        private void ApplyPaymentRowColor(DataGridViewRow row, PaymentSummary payment)
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

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadPaymentsData();
        }

        private void გადახდებისისტორიაToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = _serviceProvider.GetRequiredService<PaymentImportHistoryForm>())
            {
                form.ShowDialog(this);
            }
        }

        private void დაუდასტურებელიგადახდებიToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManagePayments))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var unmatchedPaymentsForm = _serviceProvider.GetRequiredService<UnmatchedPaymentsForm>();
            unmatchedPaymentsForm.Show();
        }
    }
}


