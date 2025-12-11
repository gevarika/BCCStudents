using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Domain.Entities;
using Services;
using System.Threading.Tasks;

namespace BCCStudents.Presentation
{
    public partial class PaymentTestForm : Form
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentGroupRepository _studentGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly PaymentService _paymentService;
        private int _selectedStudentId = -1;
        private int _selectedGroupId = -1;

        public PaymentTestForm(
            IStudentRepository studentRepository,
            IStudentGroupRepository studentGroupRepository,
            IGroupRepository groupRepository,
            PaymentService paymentService)
        {
            InitializeComponent();
            _studentRepository = studentRepository;
            _studentGroupRepository = studentGroupRepository;
            _groupRepository = groupRepository;
            _paymentService = paymentService;

            // DataGridView-áƒ˜áƒ¡ áƒ¡áƒ•áƒ”áƒ¢áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dgvGroups.Columns.Clear();
            dgvGroups.Columns.Add("GroupId", "áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ ID");
            dgvGroups.Columns.Add("GroupName", "áƒ¯áƒ’áƒ£áƒ¤áƒ˜áƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜");
            dgvGroups.Columns.Add("DateOfPayment", "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜");
            dgvGroups.Columns.Add("PaymentStatus", "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜");
            dgvGroups.Columns.Add("Price", "áƒ¤áƒáƒ¡áƒ˜");
            dgvGroups.Columns.Add("Discount", "áƒ¤áƒáƒ¡áƒ“áƒáƒ™áƒšáƒ”áƒ‘áƒ %");
            dgvGroups.Columns.Add("IsActive", "áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜");
            dgvGroups.SelectionChanged += DgvGroups_SelectionChanged;
        }

        private void PaymentTestForm_Load(object sender, EventArgs e)
        {
            LoadStudents();
            dtpNewDate.Value = DateTime.Today; // áƒœáƒáƒ’áƒ£áƒšáƒ˜áƒ¡áƒ®áƒ›áƒ”áƒ•áƒáƒ“ áƒ“áƒ¦áƒ”áƒ¡áƒ“áƒ¦áƒ”áƒáƒ‘áƒ˜áƒ—
            AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
            AddLog("áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ¤áƒáƒ áƒ›áƒ áƒ©áƒáƒ˜áƒ¢áƒ•áƒ˜áƒ áƒ—áƒ");
            AddLog($"áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {DateTime.Today:dd.MM.yyyy}");
            AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
            AddLog("");
            AddLog("ðŸ“Œ áƒ˜áƒœáƒ¡áƒ¢áƒ áƒ£áƒ¥áƒªáƒ˜áƒ:");
            AddLog("1. áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” ComboBox-áƒ“áƒáƒœ");
            AddLog("2. áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— áƒ¯áƒ’áƒ£áƒ¤áƒ˜ áƒªáƒ®áƒ áƒ˜áƒšáƒ˜áƒ“áƒáƒœ");
            AddLog("3. áƒ¨áƒ”áƒªáƒ•áƒáƒšáƒ”áƒ— DateOfPayment (áƒ›áƒáƒ’: áƒ“áƒ¦áƒ”áƒ¡áƒ“áƒ¦áƒ”áƒáƒ‘áƒ˜áƒ— áƒáƒœ áƒ¬áƒáƒ áƒ¡áƒ£áƒšáƒ¨áƒ˜)");
            AddLog("4. áƒ“áƒáƒáƒ­áƒ˜áƒ áƒ”áƒ— 'áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ'");
            AddLog("5. áƒ“áƒáƒáƒ­áƒ˜áƒ áƒ”áƒ— 'áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ' áƒáƒœ 'áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ'");
            AddLog("");
        }

        private void LoadStudents()
        {
            try
            {
                cmbStudents.Items.Clear();
                var students = _studentRepository.GetAllActiveStudents();
                
                foreach (var student in students.OrderBy(s => s.FirstName).ThenBy(s => s.LastName))
                {
                    var item = new ComboBoxItem
                    {
                        Text = $"{student.FirstName} {student.LastName} (ID: {student.Id}, Balance: {student.Balance} â‚¾)",
                        Value = student.Id
                    };
                    cmbStudents.Items.Add(item);
                }

                if (cmbStudents.Items.Count > 0)
                {
                    cmbStudents.SelectedIndex = 0;
                }

                AddLog($"âœ… áƒ©áƒáƒ˜áƒ¢áƒ•áƒ˜áƒ áƒ—áƒ {students.Count} áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”");
            }
            catch (Exception ex)
            {
                AddLog($"âŒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡áƒáƒ¡: {ex.Message}");
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡áƒáƒ¡:\n{ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStudents.SelectedItem == null) return;

            _selectedStudentId = ((ComboBoxItem)cmbStudents.SelectedItem).Value;
            LoadStudentGroups();
        }

        private void LoadStudentGroups()
        {
            try
            {
                dgvGroups.Rows.Clear();
                
                if (_selectedStudentId <= 0)
                {
                    AddLog("âš ï¸ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” áƒáƒ  áƒáƒ áƒ˜áƒ¡ áƒáƒ áƒ©áƒ”áƒ£áƒšáƒ˜");
                    return;
                }

                var student = _studentRepository.GetStudentById(_selectedStudentId);
                if (student == null)
                {
                    AddLog($"âŒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” ID {_selectedStudentId} áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ");
                    return;
                }

                var groups = _studentGroupRepository.GetForPayment(_selectedStudentId);
                
                if (groups == null || !groups.Any())
                {
                    AddLog($"âš ï¸ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” {student.FirstName} {student.LastName} áƒáƒ  áƒáƒ¥áƒ•áƒ¡ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜");
                    return;
                }

                foreach (var group in groups)
                {
                    var groupName = _groupRepository.GetGroupNameById(group.GroupId) ?? "áƒ£áƒªáƒœáƒáƒ‘áƒ˜";
                    var dateStr = group.DateOfPayment.HasValue 
                        ? group.DateOfPayment.Value.ToString("dd.MM.yyyy") 
                        : "áƒáƒ  áƒáƒ áƒ˜áƒ¡";
                    
                    var isOverdue = group.DateOfPayment.HasValue && group.DateOfPayment.Value <= DateTime.Today;
                    var statusColor = isOverdue ? "ðŸ”´" : "ðŸŸ¢";

                    int rowIndex = dgvGroups.Rows.Add(
                        group.GroupId,
                        groupName,
                        dateStr,
                        group.PaymentStatus ?? "áƒ£áƒªáƒœáƒáƒ‘áƒ˜",
                        $"{group.Price:F2} â‚¾",
                        $"{group.Discount:F1}%",
                        group.Status ? "áƒ“áƒ˜áƒáƒ®" : "áƒáƒ áƒ"
                    );

                    // áƒ—áƒ£ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ áƒ áƒ“áƒáƒ“áƒ’áƒ, áƒ›áƒáƒœáƒ˜áƒ¨áƒœáƒ” áƒ¬áƒ˜áƒ—áƒšáƒáƒ“
                    if (isOverdue)
                    {
                        dgvGroups.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                    }
                }

                AddLog($"âœ… áƒ©áƒáƒ˜áƒ¢áƒ•áƒ˜áƒ áƒ—áƒ {groups.Count} áƒ¯áƒ’áƒ£áƒ¤áƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡: {student.FirstName} {student.LastName}");
                AddLog($"ðŸ’° áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜: {student.Balance} â‚¾");
            }
            catch (Exception ex)
            {
                AddLog($"âŒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡áƒáƒ¡: {ex.Message}");
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡áƒáƒ¡:\n{ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                _selectedGroupId = -1;
                lblCurrentDate.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: -";
                return;
            }

            var selectedRow = dgvGroups.SelectedRows[0];
            _selectedGroupId = Convert.ToInt32(selectedRow.Cells["GroupId"].Value);
            
            var dateStr = selectedRow.Cells["DateOfPayment"].Value?.ToString();
            if (!string.IsNullOrEmpty(dateStr) && dateStr != "áƒáƒ  áƒáƒ áƒ˜áƒ¡")
            {
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    lblCurrentDate.Text = $"áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {date:dd.MM.yyyy}";
                    dtpNewDate.Value = date;
                }
                else
                {
                    lblCurrentDate.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: -";
                }
            }
            else
            {
                lblCurrentDate.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: áƒáƒ  áƒáƒ áƒ˜áƒ¡ áƒ“áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ£áƒšáƒ˜";
                dtpNewDate.Value = DateTime.Today;
            }
        }

        private void btnUpdateDate_Click(object sender, EventArgs e)
        {
            if (_selectedStudentId <= 0 || _selectedGroupId <= 0)
            {
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” áƒ“áƒ áƒ¯áƒ’áƒ£áƒ¤áƒ˜!", "áƒ’áƒáƒ¤áƒ áƒ—áƒ®áƒ˜áƒšáƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var newDate = dtpNewDate.Value.Date;
                bool success = _studentGroupRepository.UpdateDateOfPayment(_selectedStudentId, _selectedGroupId, newDate);

                if (success)
                {
                    AddLog($"âœ… áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜ áƒ’áƒáƒœáƒáƒ®áƒšáƒ“áƒ: {newDate:dd.MM.yyyy}");
                    AddLog($"   áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” ID: {_selectedStudentId}, áƒ¯áƒ’áƒ£áƒ¤áƒ˜ ID: {_selectedGroupId}");
                    
                    // áƒ¨áƒ”áƒáƒ›áƒáƒ¬áƒ›áƒ”, áƒ“áƒáƒ“áƒ’áƒ áƒ—áƒ£ áƒáƒ áƒ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ áƒ
                    if (newDate <= DateTime.Today)
                    {
                        AddLog($"ðŸŸ¢ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ áƒ áƒ“áƒáƒ“áƒ’áƒ! (áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜ <= {DateTime.Today:dd.MM.yyyy})");
                    }
                    else
                    {
                        AddLog($"ðŸŸ¡ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ áƒ áƒ¯áƒ”áƒ  áƒáƒ  áƒ“áƒáƒ“áƒ’áƒ (áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜ > {DateTime.Today:dd.MM.yyyy})");
                    }

                    MessageBox.Show(
                        $"áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ’áƒáƒœáƒáƒ®áƒšáƒ“áƒ!\n\n" +
                        $"áƒáƒ®áƒáƒšáƒ˜ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {newDate:dd.MM.yyyy}\n" +
                        (newDate <= DateTime.Today ? "âœ… áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ áƒ áƒ“áƒáƒ“áƒ’áƒ!" : "â³ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒ áƒ áƒ¯áƒ”áƒ  áƒáƒ  áƒ“áƒáƒ“áƒ’áƒ"),
                        "áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadStudentGroups(); // áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
                }
                else
                {
                    AddLog($"âŒ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ");
                    MessageBox.Show("áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ.", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AddLog($"âŒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ˜áƒ¡áƒáƒ¡:\n{ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRunPayment_Click(object sender, EventArgs e)
        {
            if (_selectedStudentId <= 0)
            {
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”!", "áƒ’áƒáƒ¤áƒ áƒ—áƒ®áƒ˜áƒšáƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var student = _studentRepository.GetStudentById(_selectedStudentId);
                if (student == null)
                {
                    MessageBox.Show("áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ” áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (student.Balance <= 0)
                {
                    MessageBox.Show(
                        $"áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ¡ áƒáƒ  áƒáƒ¥áƒ•áƒ¡ áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜!\n\n" +
                        $"áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜: {student.Balance} â‚¾\n\n" +
                        $"áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒ¯áƒ”áƒ  áƒ“áƒáƒ£áƒ›áƒáƒ¢áƒáƒ— áƒ—áƒáƒœáƒ®áƒ áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ–áƒ”.",
                        "áƒ’áƒáƒ¤áƒ áƒ—áƒ®áƒ˜áƒšáƒ”áƒ‘áƒ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                AddLog("");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog($"ðŸ’° áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡: {student.FirstName} {student.LastName}");
                AddLog($"ðŸ’° áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜: {student.Balance} â‚¾");
                AddLog($"ðŸ’³ áƒ©áƒáƒ áƒ˜áƒªáƒ®áƒ£áƒšáƒ˜ áƒ—áƒáƒœáƒ®áƒ: 0 â‚¾ (áƒ›áƒ®áƒáƒšáƒáƒ“ áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜áƒ“áƒáƒœ)");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog("");

                // paymentAmount = 0 â†’ áƒ›áƒ®áƒáƒšáƒáƒ“ áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜áƒ“áƒáƒœ
                var result = await _paymentService.ProcessPayment(_selectedStudentId, 0);

                AddLog("");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog($"ðŸ“Š áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ˜: {result.Status}");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog("");

                foreach (var log in result.Logs)
                {
                    AddLog(log);
                }

                // áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ
                var updatedStudent = _studentRepository.GetStudentById(_selectedStudentId);
                if (updatedStudent != null)
                {
                    AddLog("");
                    AddLog($"ðŸ’° áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ‘áƒáƒšáƒáƒœáƒ¡áƒ˜: {updatedStudent.Balance} â‚¾");
                }

                MessageBox.Show(
                    $"áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ˜: {result.Status}\n\n" +
                    $"áƒ“áƒ”áƒ¢áƒáƒšáƒ£áƒ áƒ˜ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ áƒœáƒáƒ©áƒ•áƒ”áƒœáƒ”áƒ‘áƒ˜áƒ áƒšáƒáƒ’áƒ”áƒ‘áƒ¨áƒ˜.",
                    "áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ",
                    MessageBoxButtons.OK,
                    result.Status == "Paid" || result.Status == "Partial" 
                        ? MessageBoxIcon.Information 
                        : MessageBoxIcon.Warning
                );

                LoadStudentGroups(); // áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
            }
            catch (Exception ex)
            {
                AddLog($"âŒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                AddLog($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡áƒáƒ¡:\n{ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRunAutoPayments_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "áƒ’áƒ¡áƒ£áƒ áƒ— áƒ’áƒáƒ£áƒ¨áƒ•áƒáƒ— áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ§áƒ•áƒ”áƒšáƒ áƒáƒ¥áƒ¢áƒ˜áƒ£áƒ áƒ˜ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡?\n\n" +
                "áƒ”áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜ áƒ¨áƒ”áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ¡ áƒ§áƒ•áƒ”áƒšáƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”áƒ¡, áƒ•áƒ˜áƒ¡áƒáƒª áƒáƒ¥áƒ•áƒ¡ Balance > 0.",
                "áƒ“áƒáƒ“áƒáƒ¡áƒ¢áƒ£áƒ áƒ”áƒ‘áƒ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                AddLog("");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog("ðŸ”„ áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ áƒ§áƒ•áƒ”áƒšáƒ áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog("");

                int processed = 0;
                int total = 0;

                bool useFullBalance = Properties.Settings.Default.UseFullBalanceForAutoPayment;
                await _paymentService.ProcessAutoPaymentsForAllStudentsAsync((current, totalCount) =>
                {
                    processed = current;
                    total = totalCount;
                    this.Invoke((Action)(() =>
                    {
                        AddLog($"ðŸ“Š áƒžáƒ áƒáƒ’áƒ áƒ”áƒ¡áƒ˜: {current} / {totalCount} áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”");
                    }));
                });

                AddLog("");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");
                AddLog($"âœ… áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ!");
                AddLog($"ðŸ“Š áƒ“áƒáƒ›áƒ£áƒ¨áƒáƒ•áƒ”áƒ‘áƒ£áƒšáƒ˜: {processed} / {total} áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”");
                AddLog("â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•");

                MessageBox.Show(
                    $"áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ!\n\n" +
                    $"áƒ“áƒáƒ›áƒ£áƒ¨áƒáƒ•áƒ”áƒ‘áƒ£áƒšáƒ˜: {processed} / {total} áƒ›áƒáƒ¡áƒ¬áƒáƒ•áƒšáƒ”",
                    "áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ”áƒ‘áƒ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadStudentGroups(); // áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
            }
            catch (Exception ex)
            {
                AddLog($"âŒ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                AddLog($"Stack Trace: {ex.StackTrace}");
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒ˜ áƒ’áƒáƒ“áƒáƒ®áƒ“áƒ˜áƒ¡áƒáƒ¡:\n{ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStudents();
            if (_selectedStudentId > 0)
            {
                LoadStudentGroups();
            }
            AddLog("ðŸ”„ áƒ›áƒáƒœáƒáƒªáƒ”áƒ›áƒ”áƒ‘áƒ˜ áƒ’áƒáƒœáƒáƒ®áƒšáƒ“áƒ");
        }

        private void AddLog(string message)
        {
            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke((Action)(() => AddLog(message)));
                return;
            }

            txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLogs.ScrollToCaret();
        }

        // ComboBoxItem áƒ™áƒšáƒáƒ¡áƒ˜
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString() => Text;
        }
    }
}


