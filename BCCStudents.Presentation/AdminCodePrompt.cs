namespace BCCStudents.Presentation
{
    public class AdminCodePrompt : Form
    {
        private TextBox txtCode;
        private Button btnOK;
        private Button btnCancel;

        private void InitializeComponent()
        {

        }

        public string EnteredCode => txtCode.Text;
        public AdminCodePrompt()
        {
            this.Text = "ადმინის კოდი";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Width = 300;
            this.Height = 120;
            txtCode = new TextBox { PasswordChar = '*', Left = 20, Top = 20, Width = 240 };
            btnOK = new Button { Text = "OK", Left = 60, Width = 70, Top = 60, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancel", Left = 150, Width = 70, Top = 60, DialogResult = DialogResult.Cancel };
            this.Controls.Add(txtCode);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}
