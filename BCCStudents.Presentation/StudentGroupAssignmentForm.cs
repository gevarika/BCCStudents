using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    public partial class StudentGroupAssignmentForm : Form
    {
        private readonly List<Group> availableGroups;
        private readonly Student student;
        private Group selectedGroup;
        private double discount = 0;

        public Group SelectedGroup => selectedGroup;
        public double Discount => discount;

        public StudentGroupAssignmentForm(Student student, List<Group> groups)
        {
            InitializeComponent();
            this.student = student;
            this.availableGroups = groups;

            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "მოსწავლის ჯგუფთან დაკავშირება";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // მთავარი პანელი
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(10)
            };

            // სათაური
            var lblTitle = new Label
            {
                Text = $"მოსწავლე '{student.FirstName} {student.LastName}' არ არის დაკავშირებული არც ერთ ჯგუფთან.",
                Font = new Font("Sylfaen", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ჯგუფის არჩევის ლეიბლი
            var lblGroupSelection = new Label
            {
                Text = "აირჩიეთ ჯგუფი, რომელშიც დაემატება მოსწავლე:",
                Font = new Font("Sylfaen", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ჯგუფების ComboBox
            var cbGroups = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Sylfaen", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cbGroups.DataSource = availableGroups;
            cbGroups.DisplayMember = "Name";
            cbGroups.ValueMember = "Id";

            // ფასდაკლების ლეიბლი
            var lblDiscount = new Label
            {
                Text = "ფასდაკლება (%):",
                Font = new Font("Sylfaen", 10),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ფასდაკლების NumericUpDown
            var numDiscount = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 0,
                Width = 100,
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Sylfaen", 10)
            };

            // ჯგუფის ინფორმაციის ლეიბლი
            var lblGroupInfo = new Label
            {
                Text = "",
                Font = new Font("Sylfaen", 9),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Blue
            };

            // ღილაკების პანელი
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft
            };

            var btnCancel = new Button
            {
                Text = "გაუქმება",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            var btnOK = new Button
            {
                Text = "დამტკიცება",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.OK
            };

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOK);

            // ჯგუფის ინფორმაციის განახლება
            cbGroups.SelectedIndexChanged += (sender, e) =>
            {
                if (cbGroups.SelectedItem is Group selectedGroup)
                {
                    lblGroupInfo.Text = $"ჯგუფი: {selectedGroup.Name}\nფასი: {selectedGroup.Price:C}\nმოსწავლეების რაოდენობა: {selectedGroup.StudentCount}";
                    this.selectedGroup = selectedGroup;
                }
            };

            // ფასდაკლების შეცვლა
            numDiscount.ValueChanged += (sender, e) =>
            {
                discount = (double)numDiscount.Value;
            };

            // ღილაკების ივენთები
            btnOK.Click += (sender, e) =>
            {
                if (cbGroups.SelectedItem == null)
                {
                    MessageBox.Show("გთხოვთ აირჩიოთ ჯგუფი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.selectedGroup = (Group)cbGroups.SelectedItem;
                this.discount = (double)numDiscount.Value;
            };

            btnCancel.Click += (sender, e) =>
            {
                this.selectedGroup = null;
                this.discount = 0;
            };

            // კონტროლების დამატება
            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(lblGroupSelection, 0, 1);
            mainPanel.Controls.Add(cbGroups, 0, 2);
            mainPanel.Controls.Add(lblDiscount, 0, 3);
            mainPanel.Controls.Add(numDiscount, 0, 4);
            mainPanel.Controls.Add(lblGroupInfo, 0, 5);
            mainPanel.Controls.Add(buttonPanel, 0, 6);

            this.Controls.Add(mainPanel);

            // საწყისი ჯგუფის არჩევა
            if (availableGroups.Any() && cbGroups.Items.Count > 0)
            {
                cbGroups.SelectedIndex = 0;
            }
        }
    }
}
