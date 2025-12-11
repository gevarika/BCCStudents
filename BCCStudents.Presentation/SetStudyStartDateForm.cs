using BCCStudents.Infrastructure.Data;
using BCCStudents.Application.Services;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCCStudents.Presentation
{
    public partial class SetStudyStartDateForm : Form
    {
        public DateTime SelectedDate { get; private set; }
        private readonly PaymentDateService _paymentDateService;

        public SetStudyStartDateForm(PaymentDateService paymentDateService)
        {
            InitializeComponent();
            FormTitleHelper.SetTitle(this, "სწავლის დაწყების თარიღის დაყენება");
            _paymentDateService = paymentDateService;
            dtpStartDate.Value = DateTime.Today;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SelectedDate = dtpStartDate.Value;
            _paymentDateService.UpdateNextPaymentDate(SelectedDate);
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

