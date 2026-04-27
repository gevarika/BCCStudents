namespace BCCStudents.Presentation
{
    public partial class UpdateProgressForm : Form
    {
        private ProgressBar _bar;
        private Label _label;

        public UpdateProgressForm()
        {
            this.Text = "განახლება";
            this.Width = 420;
            this.Height = 140;
            this.StartPosition = FormStartPosition.CenterParent;

            _bar = new ProgressBar { Left = 20, Top = 20, Width = 360, Minimum = 0, Maximum = 100 };
            _label = new Label { Left = 20, Top = 60, Width = 360, Text = "მიმდინარეობს ჩამოტვირთვა..." };

            this.Controls.Add(_bar);
            this.Controls.Add(_label);
        }

        public void Report(long current, long total)
        {
            if (total <= 0) return;
            var percent = (int)Math.Max(0, Math.Min(100, (current * 100.0 / total)));
            _bar.Value = percent;
            _label.Text = $"ჩამოტვირთულია {current / 1024 / 1024} MB {total / 1024 / 1024} MB-დან ({percent}%)";
            _label.Refresh();
        }
    }
}



