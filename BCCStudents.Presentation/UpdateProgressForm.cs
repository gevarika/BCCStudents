using System;
using System.Windows.Forms;

namespace BCCStudents.Presentation
{
    public partial class UpdateProgressForm : Form
    {
        private ProgressBar _bar;
        private Label _label;

        public UpdateProgressForm()
        {
            this.Text = "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ";
            this.Width = 420;
            this.Height = 140;
            this.StartPosition = FormStartPosition.CenterParent;

            _bar = new ProgressBar { Left = 20, Top = 20, Width = 360, Minimum = 0, Maximum = 100 };
            _label = new Label { Left = 20, Top = 60, Width = 360, Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”áƒáƒ‘áƒ¡ áƒ©áƒáƒ›áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ..." };

            this.Controls.Add(_bar);
            this.Controls.Add(_label);
        }

        public void Report(long current, long total)
        {
            if (total <= 0) return;
            var percent = (int)Math.Max(0, Math.Min(100, (current * 100.0 / total)));
            _bar.Value = percent;
            _label.Text = $"áƒ©áƒáƒ›áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ£áƒšáƒ˜áƒ {current / 1024 / 1024} MB {total / 1024 / 1024} MB-áƒ“áƒáƒœ ({percent}%)";
            _label.Refresh();
        }
    }
}



