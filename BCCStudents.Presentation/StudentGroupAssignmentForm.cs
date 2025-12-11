using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
            this.Text = "áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡ áƒ¯áƒ’áƒ£áƒ¤áƒ—áƒáƒœ áƒ“áƒáƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ”áƒ‘áƒ";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // áƒ›áƒ—áƒáƒ•áƒáƒ áƒ˜ áƒžáƒáƒœáƒ”áƒšáƒ˜
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(10)
            };

            // áƒ¡áƒáƒ—áƒáƒ£áƒ áƒ˜
            var lblTitle = new Label
            {
                Text = $"áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” '{student.FirstName} {student.LastName}' áƒáƒ  áƒáƒ áƒ˜áƒ¡ áƒ“áƒáƒ™áƒáƒ•áƒ¨áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒáƒ áƒªáƒ”áƒ áƒ— áƒ¯áƒ’áƒ£áƒ¤áƒ—áƒáƒœ.",
                Font = new Font("Sylfaen", 12, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒáƒ áƒ©áƒ”áƒ•áƒ˜áƒ¡ áƒšáƒ”áƒ˜áƒ‘áƒšáƒ˜
            var lblGroupSelection = new Label
            {
                Text = "áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ¯áƒ’áƒ£áƒ¤áƒ˜ áƒ áƒáƒ›áƒ”áƒšáƒ¨áƒ˜áƒª áƒ“áƒáƒ”áƒ›áƒáƒ¢áƒ”áƒ‘áƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”:",
                Font = new Font("Sylfaen", 10, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ ComboBox
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

            // áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒšáƒ”áƒ˜áƒ‘áƒšáƒ˜
            var lblDiscount = new Label
            {
                Text = "áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ (%):",
                Font = new Font("Sylfaen", 10),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ¡ NumericUpDown
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

            // áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ˜áƒ¡ áƒšáƒ”áƒ˜áƒ‘áƒšáƒ˜
            var lblGroupInfo = new Label
            {
                Text = "",
                Font = new Font("Sylfaen", 9),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Blue
            };

            // áƒ¦áƒ˜áƒšáƒáƒ™áƒ”áƒ‘áƒ˜áƒ¡ áƒžáƒáƒœáƒ”áƒšáƒ˜
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft
            };

            var btnCancel = new Button
            {
                Text = "áƒ’áƒáƒ£áƒ¥áƒ›áƒ”áƒ‘áƒ",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            var btnOK = new Button
            {
                Text = "áƒ“áƒáƒ›áƒ¢áƒ™áƒ˜áƒªáƒ”áƒ‘áƒ",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.OK
            };

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOK);

            // áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
            cbGroups.SelectedIndexChanged += (sender, e) =>
            {
                if (cbGroups.SelectedItem is Group selectedGroup)
                {
                    lblGroupInfo.Text = $"áƒ¯áƒ’áƒ£áƒ¤áƒ˜: {selectedGroup.Name}\náƒ¤áƒáƒ¡áƒ˜: {selectedGroup.Price:C}\náƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ”áƒ‘áƒ˜áƒ¡ áƒ áƒáƒáƒ“áƒ”áƒœáƒáƒ‘áƒ: {selectedGroup.StudentCount}";
                    this.selectedGroup = selectedGroup;
                }
            };

            // áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒªáƒ•áƒšáƒ˜áƒšáƒ”áƒ‘áƒ
            numDiscount.ValueChanged += (sender, e) =>
            {
                discount = (double)numDiscount.Value;
            };

            // áƒ¦áƒ˜áƒšáƒáƒ™áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ”áƒ‘áƒ˜
            btnOK.Click += (sender, e) =>
            {
                if (cbGroups.SelectedItem == null)
                {
                    MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ— áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— áƒ¯áƒ’áƒ£áƒ¤áƒ˜.", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // áƒ™áƒáƒœáƒ¢áƒ áƒáƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒáƒ›áƒáƒ¢áƒ”áƒ‘áƒ
            mainPanel.Controls.Add(lblTitle, 0, 0);
            mainPanel.Controls.Add(lblGroupSelection, 0, 1);
            mainPanel.Controls.Add(cbGroups, 0, 2);
            mainPanel.Controls.Add(lblDiscount, 0, 3);
            mainPanel.Controls.Add(numDiscount, 0, 4);
            mainPanel.Controls.Add(lblGroupInfo, 0, 5);
            mainPanel.Controls.Add(buttonPanel, 0, 6);

            this.Controls.Add(mainPanel);

            // áƒ¡áƒáƒ¬áƒ§áƒ˜áƒ¡áƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒáƒ áƒ©áƒ”áƒ•áƒ
            if (availableGroups.Any() && cbGroups.Items.Count > 0)
            {
                cbGroups.SelectedIndex = 0;
            }
        }
    }
} 
