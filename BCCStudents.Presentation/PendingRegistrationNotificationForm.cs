namespace BCCStudents.Presentation
{
    /// <summary>
    /// შეტყობინება ახალი ონლაინ რეგისტრაციების შესახებ.
    /// </summary>
    public class PendingRegistrationNotificationForm : Form
    {
        public bool ShouldOpenPendingForm { get; private set; }

        public PendingRegistrationNotificationForm(int newRegistrations)
        {
            Text = "ახალი ონლაინ რეგისტრაცია";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 150);

            var lblMessage = new Label
            {
                Text = newRegistrations == 1
                    ? "აღმოჩენილია 1 ახალი ონლაინ რეგისტრაცია."
                    : $"აღმოჩენილია {newRegistrations} ახალი ონლაინ რეგისტრაცია.",
                Location = new Point(20, 20),
                Size = new Size(380, 40),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };

            var btnOpen = new Button
            {
                Text = "გახსნა",
                DialogResult = DialogResult.OK,
                Location = new Point(210, 90),
                Size = new Size(90, 30)
            };
            btnOpen.Click += (_, __) => ShouldOpenPendingForm = true;

            var btnClose = new Button
            {
                Text = "დახურვა",
                DialogResult = DialogResult.Cancel,
                Location = new Point(310, 90),
                Size = new Size(90, 30)
            };

            AcceptButton = btnOpen;
            CancelButton = btnClose;

            Controls.Add(lblMessage);
            Controls.Add(btnOpen);
            Controls.Add(btnClose);
        }
    }
}
