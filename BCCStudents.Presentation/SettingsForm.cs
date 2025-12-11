using BCCStudents.Application.Services;
using Services;
using System;
using System.Windows.Forms;

namespace BCCStudents.Presentation
{
    public partial class SettingsForm : Form
    {
        private readonly PaymentDateService _paymentDateService;
        public SettingsForm(PaymentDateService paymentDateService)
        {
            InitializeComponent();
            _paymentDateService = paymentDateService;
        }

        private void setStudyStart_Click(object sender, EventArgs e)
        {
            SetStudyStartDate();
        }
        private void SetStudyStartDate()
        {
            using (var dateForm = new SetStudyStartDateForm(_paymentDateService)) // სპეციალური ფორმა თარიღის ასარჩევად
            {
                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    DateTime selectedDate = dateForm.SelectedDate;
                    MessageBox.Show($"✅ სწავლის დაწყების თარიღი შენახულია: {selectedDate:yyyy-MM-dd}", "დადასტურება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

