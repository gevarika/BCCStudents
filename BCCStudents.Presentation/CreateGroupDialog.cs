using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// დიალოგი ახალი ჯგუფის შექმნისთვის
    /// </summary>
    /// 
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class CreateGroupDialog : Form
    {
        public Group CreatedGroup { get; private set; }
        public int SubGroupCount { get; private set; }

        private TextBox txtGroupName;
        private TextBox txtPrice;
        private TextBox txtTeacher;
        private NumericUpDown nudMaxStudents;
        private NumericUpDown nudSubGroupCount;
        private Button btnOK;
        private Button btnCancel;

        public CreateGroupDialog(string groupName)
        {
            InitializeComponent(groupName);
        }

        private void InitializeComponent(string groupName)
        {
            this.Text = "ახალი ჯგუფის შექმნა";
            this.Size = new System.Drawing.Size(500, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Group Name
            var lblGroupName = new Label
            {
                Text = "ჯგუფის სახელი:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            txtGroupName = new TextBox
            {
                Text = groupName,
                Location = new System.Drawing.Point(220, 18),
                Size = new System.Drawing.Size(220, 23),
                ReadOnly = true // Name comes from Excel sheet
            };

            // Price
            var lblPrice = new Label
            {
                Text = "ფასი:",
                Location = new System.Drawing.Point(20, 55),
                AutoSize = true
            };
            txtPrice = new TextBox
            {
                Text = "100",
                Location = new System.Drawing.Point(220, 53),
                Size = new System.Drawing.Size(220, 23)
            };

            // Teacher
            var lblTeacher = new Label
            {
                Text = "მასწავლებელი:",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true
            };
            txtTeacher = new TextBox
            {
                Text = "მასწავლებელი",
                Location = new System.Drawing.Point(220, 88),
                Size = new System.Drawing.Size(220, 23)
            };

            // Max Students
            var lblMaxStudents = new Label
            {
                Text = "მაქს. მოსწავლეების რაოდენობა:",
                Location = new System.Drawing.Point(20, 125),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            };
            nudMaxStudents = new NumericUpDown
            {
                Location = new System.Drawing.Point(220, 123),
                Size = new System.Drawing.Size(220, 23),
                Minimum = 1,
                Maximum = 1000,
                Value = 30
            };

            // SubGroup Count
            var lblSubGroupCount = new Label
            {
                Text = "ქვეჯგუფების რაოდენობა:",
                Location = new System.Drawing.Point(20, 160),
                AutoSize = true
            };
            nudSubGroupCount = new NumericUpDown
            {
                Location = new System.Drawing.Point(220, 158),
                Size = new System.Drawing.Size(220, 23),
                Minimum = 0,
                Maximum = 50,
                Value = 0
            };

            // Buttons
            var btnSkip = new Button
            {
                Text = "გამოტოვება",
                DialogResult = DialogResult.Ignore,
                Location = new System.Drawing.Point(20, 190),
                Size = new System.Drawing.Size(90, 30)
            };

            btnOK = new Button
            {
                Text = "შექმნა",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(210, 190),
                Size = new System.Drawing.Size(75, 30)
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "გააუქმეთ",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(295, 190),
                Size = new System.Drawing.Size(75, 30)
            };

            // Add controls
            this.Controls.Add(lblGroupName);
            this.Controls.Add(txtGroupName);
            this.Controls.Add(lblPrice);
            this.Controls.Add(txtPrice);
            this.Controls.Add(lblTeacher);
            this.Controls.Add(txtTeacher);
            this.Controls.Add(lblMaxStudents);
            this.Controls.Add(nudMaxStudents);
            this.Controls.Add(lblSubGroupCount);
            this.Controls.Add(nudSubGroupCount);
            this.Controls.Add(btnSkip);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("ჯგუფის სახელი არ შეიძლება იყოს ცარიელი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("ფასი უნდა იყოს დადებითი რიცხვი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Create Group object
            CreatedGroup = new Group
            {
                Name = txtGroupName.Text.Trim(),
                Price = price,
                Teacher = txtTeacher.Text.Trim(),
                MaxStudents = (int)nudMaxStudents.Value,
                Status = true,
                StudentCount = 0,
                UpdatedAt = DateTime.Now
            };

            SubGroupCount = (int)nudSubGroupCount.Value;
        }
    }
}
