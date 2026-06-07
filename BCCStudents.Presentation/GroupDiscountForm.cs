using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    public partial class GroupDiscountForm : Form
    {
        private readonly List<Group> selectedGroups;
        private readonly Dictionary<int, double> groupDiscounts;
        private readonly double defaultDiscount;

        public Dictionary<int, double> GroupDiscounts => groupDiscounts;

        public GroupDiscountForm(List<Group> groups, double defaultDiscount = 0)
        {
            InitializeComponent();
            this.selectedGroups = groups;
            this.defaultDiscount = defaultDiscount;
            this.groupDiscounts = new Dictionary<int, double>();

            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "ჯგუფების ფასდაკლების დაყენება";
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
                RowCount = 3,
                Padding = new Padding(10)
            };

            // სათაური
            var lblTitle = new Label
            {
                Text = "აირჩიეთ ფასდაკლება თითოეული ჯგუფისთვის:",
                Font = new Font("Sylfaen", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ჯგუფების სია
            var groupPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            foreach (var group in selectedGroups)
            {
                var groupRow = new TableLayoutPanel
                {
                    Width = 450,
                    Height = 50,
                    ColumnCount = 3,
                    RowCount = 1,
                    Margin = new Padding(5)
                };

                // ჯგუფის სახელი
                var lblGroupName = new Label
                {
                    Text = group.Name,
                    Dock = DockStyle.Fill,
                    Font = new Font("Sylfaen", 10),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // ფასდაკლების ველი
                var numDiscount = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 100,
                    DecimalPlaces = 0,
                    Value = (decimal)defaultDiscount,
                    Width = 80,
                    Dock = DockStyle.Fill
                };

                // პროცენტის ნიშანი
                var lblPercent = new Label
                {
                    Text = "%",
                    Dock = DockStyle.Fill,
                    Font = new Font("Sylfaen", 10),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                groupRow.Controls.Add(lblGroupName, 0, 0);
                groupRow.Controls.Add(numDiscount, 1, 0);
                groupRow.Controls.Add(lblPercent, 2, 0);

                groupPanel.Controls.Add(groupRow);

                // შევინახოთ შეცვლილი ფასდაკლება
                numDiscount.ValueChanged += (sender, e) =>
                {
                    groupDiscounts[group.Id] = (double)numDiscount.Value;
                };

                // საწყისი მნიშვნელობის ჩაწერა
                groupDiscounts[group.Id] = defaultDiscount;
            }

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

            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(groupPanel, 0, 1);
            mainPanel.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(mainPanel);
        }
    }
}
