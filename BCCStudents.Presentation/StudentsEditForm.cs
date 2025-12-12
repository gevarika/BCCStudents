using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;
//using BCCStudents.Infrastructure.DataBase;
using BCCStudents.Application.Services;
using BCCStudents.Infrastructure.Services;

namespace BCCStudents.Presentation
{
    public partial class StudentsEditForm : Form
    {
        private readonly IStudentRepository _studentService;
        private readonly StudentService _studentSvc;
        private readonly GroupService _groupService;
        private readonly SubGroupService _subGroupService;
        private readonly BackupManager _backupManager;
        public StudentsEditForm(IStudentRepository studentService, GroupService groupService, SubGroupService subGroupService, StudentService studentSvc, BackupManager backupManager)
        {
            InitializeComponent();
            _studentService = studentService;
            _studentSvc = studentSvc;
            _groupService = groupService;
            _subGroupService = subGroupService;
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "მოსწავლის ინფორმაციის რედაქტირება");
            else FormTitleHelper.SetTitle(this, "მოსწავლის ინფორმაციის რედაქტირება - სატესტო რეჟიმი");
            //InitializeContextMenuStrip();
            txtFirstName.TextChanged += TextBox_TextChanged;
            txtLastName.TextChanged += TextBox_TextChanged;
            txtParent.TextChanged += TextBox_TextChanged;
            txtAge.TextChanged += TextBox_TextChanged;
            txtPhone.TextChanged += TextBox_TextChanged;
            txtPersonalId.TextChanged += TextBox_TextChanged;
            txtAddress.TextChanged += TextBox_TextChanged;
            txtRegistrationDate.TextChanged += TextBox_TextChanged;
            txtPaymentDate.TextChanged += TextBox_TextChanged;
            txtStatus.TextChanged += TextBox_TextChanged;
            txtPaymentStatus.TextChanged += TextBox_TextChanged;

            // ... ჯგუფების ჩატვირთვა
            chlGroups.DisplayMember = "Name";
            chlGroups.ValueMember = "Id";
            var allGroups = _groupService.GetAllGroups().OrderBy(g => g.Id).ToList(); // ID-ის მიხედვით ზრდადობით დალაგება
            chlGroups.Items.Clear();
            foreach (var group in allGroups)
            {
                // Add unchecked initially
                chlGroups.Items.Add(group, false);
            }
            // ... ქვეჯგუფების ჩატვირთვა

            var allSubGroups = _subGroupService.GetAllSubGroups(); // შენს სერვისში

            dgvGroupSubGroups.Columns.Add("GroupId", "Group ID");           // hidden column
            dgvGroupSubGroups.Columns.Add("GroupName", "ჯგუფი");            // Text column

            var subGroupColumn = new DataGridViewComboBoxColumn
            {
                Name = "SubGroup",
                HeaderText = "ქვეჯგუფი",
                DisplayMember = "Name",
                ValueMember = "Id"
            };
            dgvGroupSubGroups.Columns.Add(subGroupColumn);

            

        }

        private Dictionary<int, bool> selectedRows = new Dictionary<int, bool>();
        private List<int> selectedStudentIds = new List<int>();
        // სტუდენტების მონიშნული დეტალების შესანახი სია
        private List<(int Id, string Code, string FirstName, string LastName)> selectedStudentDetails = new List<(int, string, string, string)>();
        private Dictionary<int, HashSet<int>> selectedRowsByGroup = new Dictionary<int, HashSet<int>>();
        private Dictionary<(int StudentId, int GroupId), bool> selectedStudents = new Dictionary<(int, int), bool>();
        private Dictionary<int, List<int>> studentGroupsMap = new Dictionary<int, List<int>>();
        private bool isProgrammaticCheck = false;
        private bool isProgrammaticSubGroupCheck = false;
        private HashSet<int> originalCheckedGroupIds = new HashSet<int>();
        bool statusChanged = false;
        private void txtStudentsSearch_TextChanged(object sender, EventArgs e)
        {
            /*string searchText = txtStudentSearch.Text.Trim();
            int selectedGroupId = cbGroups.SelectedValue != null ? Convert.ToInt32(cbGroups.SelectedValue) : -1;

            List<Student> students;
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                students = _studentService.SearchStudentsByNameAndGroup(searchText, selectedGroupId);
            }
            else
            {
                students = _studentService.GetStudentsByGroupId(selectedGroupId);
            }

            RefreshDataGridView(students);*/
        }
        private void deleteSingleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 1)
            {
                var row = dgvStudents.SelectedRows[0];
                row.DefaultCellStyle.BackColor = Color.Red;
                row.DefaultCellStyle.ForeColor = Color.White;
                row.Tag = "ToDelete"; // ნიშნული, რომ ეს ჩანაწერი წასაშლელია
            }
            else
            {
                MessageBox.Show("Please select a single record to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {

            var studentsToDelete = selectedStudents.Where(s => s.Value).Select(s => s.Key).ToList();
            /*foreach (var kvp in selectedStudents)
            {
                var studentId = kvp.Key.StudentId;
                var groupId = kvp.Key.GroupId;
                var isSelected = kvp.Value;

                Console.WriteLine($"[CHECK] StudentId={studentId}, GroupId={groupId}, Selected={isSelected}");

            }*/

            if (studentsToDelete.Count == 0)
            {
                MessageBox.Show("No students selected for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // ჯგუფების მიხედვით დალაგებული სტუდენტების სიის შექმნა
            /*var groupedStudents = studentsToDelete
                .GroupBy(s => s.GroupId)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(s => s.StudentId).ToList());*/
            var groupedStudents = studentsToDelete.GroupBy(s => s.GroupId).OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Select(x => x.StudentId).ToList());
            StringBuilder message = new StringBuilder();
            foreach (var group in groupedStudents)
            {
                string groupName = _groupService.GetGroupName(group.Key);
                message.AppendLine($"Group: {groupName}");

                foreach (var studentId in group.Value)
                {
                    string studentName = _studentService.GetStudentName(studentId);
                    message.AppendLine($"  - {studentName}");
                }
                message.AppendLine();
            }
            DialogResult result = MessageBox.Show(
                $"The following students will be marked as 'Inactive':\n\n{message}\n\nDo you want to continue?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                foreach (var (studentId, groupId) in studentsToDelete)
                {
                    _studentSvc.UpdateStudentStatus(studentId, groupId, false);
                }

                MessageBox.Show("Selected students have been marked as 'Inactive'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // გავწმინდოთ მონიშნული მოსწავლეების ლექსიკონი
                selectedStudents.Clear();

                int selectedGroupId = cbGroups.SelectedValue != null ? Convert.ToInt32(cbGroups.SelectedValue) : -1;
                var students = _studentService.GetStudentsByGroupId(selectedGroupId);
                RefreshDataGridView(students);
                
                // განვახლოთ originalStudentData თუ არჩეული მოსწავლე იყო წაშლილებში
                if (originalStudentData != null && studentsToDelete.Any(s => s.StudentId == originalStudentData.Id))
                {
                    originalStudentData.StudentGroupsList = _groupService.GetStudentGroupsByStudentId(originalStudentData.Id);
                    originalStudentData.StudentSubGroupsList = _subGroupService.GetAllSubGroups();
                    LoadStudentSubGroupsToGrid(originalStudentData);
                }
            }
        }
        private void StudentsEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (selectedStudents.Any(s => s.Value)) // თუ დარჩა მონიშნული სტუდენტები
            {
                DialogResult result = MessageBox.Show(
                    "You have selected students for deletion but have not completed the process.\nDo you want to exit without deleting them?",
                    "Unfinished Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true; // ფორმის დახურვის გაუქმება
                }
            }
        }

        private void dgvStudents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvStudents.Columns["Select"].Index && e.RowIndex >= 0)
            {
                int studentId = Convert.ToInt32(dgvStudents.Rows[e.RowIndex].Cells["Id"].Value);
                int groupId = Convert.ToInt32(dgvStudents.Rows[e.RowIndex].Cells["GroupId"].Value);
                bool isChecked = (bool)dgvStudents.Rows[e.RowIndex].Cells["Select"].Value;

                if (selectedStudents.ContainsKey((studentId, groupId)))
                {
                    selectedStudents[(studentId, groupId)] = isChecked;
                }
                else
                {
                    selectedStudents.Add((studentId, groupId), isChecked);
                }

                // წითლად მონიშვნა თუ მონიშნულია, თეთრი ფერი თუ არა
                dgvStudents.Rows[e.RowIndex].DefaultCellStyle.BackColor = isChecked ? Color.Red : Color.White;
            }
        }

        private void StudentsEditForm_Load(object sender, EventArgs e)
        {
            dgvGroupSubGroups.CellValueChanged += dgvGroupSubGroups_CellValueChanged;

            var groups = _groupService.GetAllGroups().OrderBy(g => g.Id).ToList(); // ID-ის მიხედვით ზრდადობით დალაგება
            // ➕ ვამატებთ "ყველა ჯგუფი" ხელით (პირველ ადგილზე)
            groups.Insert(0, new Group { Id = 0, Name = "ყველა ჯგუფი" });

            cbGroups.DisplayMember = "Name";
            cbGroups.ValueMember = "Id";
            cbGroups.DataSource = groups;
        }
        private void dgvGroupSubGroups_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isProgrammaticSubGroupCheck) return;
            
            // ვამოწმებთ რომ ნამდვილად ქვეჯგუფის ComboBox იცვლება
            if (dgvGroupSubGroups.Columns[e.ColumnIndex].Name == "SubGroup")
            {
                subGroupsChanged = true;
                dgvGroupSubGroups.DefaultCellStyle.BackColor = Color.LightYellow;
            }
        }

        private void cbGroups_SelectedValueChanged(object sender, EventArgs e)
        {
            string searchText = txtStudentSearch.Text.Trim();
            int selectedGroupId = cbGroups.SelectedValue != null ? Convert.ToInt32(cbGroups.SelectedValue) : -1;

            List<Student> students;
            // ყოველთვის გამოიყენე groupId-ზე დამოკიდებული მეთოდები
            if (selectedGroupId == 0)
            {
                students = string.IsNullOrWhiteSpace(searchText)
                    ? _studentService.GetAllStudentsWithGroups() // 🟢 შეიცავს JOIN და GroupId
                    : _studentService.SearchStudentsByNameAcrossAllGroups(searchText); // 🟢 იგივე
            }
            else
            {
                students = string.IsNullOrWhiteSpace(searchText)
                    ? _studentService.GetStudentsByGroupId(selectedGroupId)
                    : _studentService.SearchStudentsByNameAndGroup(searchText, selectedGroupId);
            }
            /*if (selectedGroupId == 0)
            {
                students = string.IsNullOrWhiteSpace(searchText)
            ? _studentService.GetAllStudents()
            : _studentService.SearchStudentsByName(searchText);
            }
            else
            {
                students = string.IsNullOrWhiteSpace(searchText)
                    ? _studentService.GetStudentsByGroupId(selectedGroupId)
                    : _studentService.SearchStudentsByNameAndGroup(searchText, selectedGroupId);
            }*/
            RefreshDataGridView(students);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (selectedStudents.Count == 0)
            {
                MessageBox.Show("No students are selected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var row in dgvStudents.Rows.Cast<DataGridViewRow>())
            {
                row.Cells["Select"].Value = false;
                row.DefaultCellStyle.BackColor = Color.White; // მონიშვნის ფერის აღდგენა
            }

            selectedStudents.Clear();

            MessageBox.Show("All selections have been cleared.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void RefreshDataGridView(List<Student> students)
        {
            dgvStudents.DataSource = null;
            dgvStudents.Columns.Clear();
            dgvStudents.AutoGenerateColumns = false;

            dgvStudents.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "Select", Width = 50 });
            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", Visible = false });
            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "GroupId", DataPropertyName = "GroupId", Visible = false });
            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "FirstName", DataPropertyName = "FirstName", HeaderText = "First Name" });
            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastName", DataPropertyName = "LastName", HeaderText = "Last Name" });
            dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "GroupName", DataPropertyName = "GroupName", HeaderText = "Group" });

            // გავწმინდოთ წაშლისთვის მონიშნული მოსწავლეები რეფრეშისას
            // (გარდა იმ შემთხვევისა, როცა მომხმარებელმა უკვე მონიშნა)
            // selectedStudents.Clear(); // ეს გააკეთებს btnDel ან btnClear
            
            dgvStudents.Rows.Clear();
            foreach (var student in students)
            {
                int resolvedGroupId = student.GroupId;
                try
                {
                    if (!string.IsNullOrWhiteSpace(student.GroupName))
                    {
                        var g = _groupService.GetGroupByName(student.GroupName);
                        if (g != null) resolvedGroupId = g.Id;
                    }
                }
                catch { }
                dgvStudents.Rows.Add(
                    false,  // Checkbox default unchecked
                    student.Id,
                    resolvedGroupId,
                    student.FirstName,
                    student.LastName,
                    student.GroupName
                );
            }

            // აღვადგენთ მონიშნული სტუდენტების სტატუსს
            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                int studentId = Convert.ToInt32(row.Cells["Id"].Value);
                int groupId = Convert.ToInt32(row.Cells["GroupId"].Value);

                if (selectedStudents.ContainsKey((studentId, groupId)) && selectedStudents[(studentId, groupId)])
                {
                    row.Cells["Select"].Value = true;
                    row.DefaultCellStyle.BackColor = Color.Red; // წითლად მონიშნული
                }
            }

            dgvStudents.Refresh();
        }
        
        private void FieldStatusAsStudent(bool status)
        {
            if (status)
            {
                lblStatus.ForeColor = Color.Green;
                txtFirstName.Enabled = true;
                txtLastName.Enabled = true;
                txtParent.Enabled = true;
                txtAge.Enabled = true;
                txtPhone.Enabled = true;
                txtPersonalId.Enabled = true;
                txtAddress.Enabled = true;
                txtRegistrationDate.Enabled = true;
                txtPaymentDate.Enabled = true;
                txtStatus.Enabled = true;
                txtPaymentStatus.Enabled = true;
                lblStatus.Text = "Active";
                txtBalance.Enabled = true;
                dgvGroupSubGroups.Enabled = true;
                chlGroups.Enabled = true;
            }
            else if (!status)
            {
                lblStatus.ForeColor = Color.Red;
                txtFirstName.Enabled = false;
                txtLastName.Enabled = false;
                txtParent.Enabled = false;
                txtAge.Enabled = false;
                txtPhone.Enabled = false;
                txtPersonalId.Enabled = false;
                txtAddress.Enabled = false;
                txtRegistrationDate.Enabled = false;
                txtPaymentDate.Enabled = false;
                txtStatus.Enabled = false;
                txtPaymentStatus.Enabled = false;
                lblStatus.Text = "Inactive";
                txtBalance.Enabled = false;
                dgvGroupSubGroups.Enabled = false;
                //chlGroups.Enabled = false;
            }
            else
            {
                lblStatus.ForeColor = Color.Yellow;
                lblStatus.Text = "სტატუსი უცნობია";
            }
        }
        private void LoadStudentSubGroupsToGrid(Student student)
        {
            isProgrammaticSubGroupCheck = true; // პროგრამული ცვლილების ფლაგი
            
            var studentGroups = student.StudentGroupsList?.OrderBy(sg => sg.GroupId).ToList(); // ID-ის მიხედვით ზრდადობით დალაგება
            var allSubGroups = _subGroupService.GetAllSubGroups();

            dgvGroupSubGroups.Rows.Clear();

            // თუ მოსწავლეს არ აქვს ჯგუფები, არ ვაჩვენებთ არაფერს
            if (studentGroups != null && studentGroups.Any())
            {
                foreach (var sg in studentGroups)
                {
                    var groupId = sg.GroupId;
                    var groupName = _groupService.GetGroupName(groupId);
                    var groupSubGroups = allSubGroups.Where(s => s.GroupId == groupId).ToList();

                    var rowIndex = dgvGroupSubGroups.Rows.Add();
                    dgvGroupSubGroups.Rows[rowIndex].Cells["GroupId"].Value = groupId;
                    dgvGroupSubGroups.Rows[rowIndex].Cells["GroupName"].Value = groupName;

                    var subGroupCell = (DataGridViewComboBoxCell)dgvGroupSubGroups.Rows[rowIndex].Cells["SubGroup"];
                    subGroupCell.DataSource = groupSubGroups;
                    subGroupCell.DisplayMember = "Name";
                    subGroupCell.ValueMember = "Id";

                    var selectedSubGroupId = studentGroups
                        .Where(s => s.GroupId == groupId)
                        .Select(s => s.SubGroupId)
                        .FirstOrDefault();

                    subGroupCell.Value = selectedSubGroupId;
                }
            }
            
            isProgrammaticSubGroupCheck = false; // ფლაგის განულება
        }

        private bool hasChanges = false;
        private bool subGroupsChanged = false;
        private bool groupsChanged = false;
        private Student originalStudentData;
        private int studentId = 0;  // არჩეული მოსწავლის ID
        private int groupId = 0;    // არჩეული მოსწავლის ჯგუფის ID
        private bool isLoadingData = false; // პროგრამული მონაცემების ჩატვირთვის ფლაგი
        
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // პროგრამული ჩატვირთვისას არ ჩაითვალოს ცვლილებად
            if (isLoadingData) return;
            
            if (!hasChanges) hasChanges = true;
            ((TextBox)sender).BackColor = Color.LightYellow; // ვიზუალურად ცვლილების ჩვენება
        }

        /// <summary>
        /// მოსწავლის მონაცემების შენახვა \ ჯგუფის ცვლილება
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            // 1. ვალიდაცია - არის თუ არა არჩეული მოსწავლე
            if (originalStudentData == null || studentId <= 0)
            {
                MessageBox.Show("გთხოვთ აირჩიოთ მოსწავლე.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // თუ არაფერი შეცვლილა
            if (!hasChanges && !groupsChanged && !subGroupsChanged && !statusChanged)
            {
                MessageBox.Show("ცვლილებები არ არის.", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 2. მოსწავლის ძირითადი მონაცემების განახლება
                if (hasChanges)
                {
                    SaveStudentBasicInfo();
                }

                // 3. ჯგუფების ცვლილება
                if (groupsChanged)
                {
                    SaveGroupChanges();
                }

                // 4. ქვეჯგუფების ცვლილება
                if (subGroupsChanged)
                {
                    SaveSubGroupChanges();
                }

                // 5. სტატუსის ცვლილება
                if (statusChanged)
                {
                    SaveStatusChange();
                }

                // 6. წარმატების შეტყობინება
                MessageBox.Show("ცვლილებები წარმატებით შეინახა.", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _backupManager.DbChangedSinceLastBackup = true;

                // 7. მონაცემების და UI-ს სრული განახლება
                RefreshAfterSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა ცვლილებების შენახვისას: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// მოსწავლის ძირითადი მონაცემების შენახვა (Students ცხრილი)
        /// </summary>
        private void SaveStudentBasicInfo()
        {
            // მხოლოდ შეცვლილი ველების განახლება
            var changedFields = new Dictionary<string, object>();

            // სახელი
            string newFirstName = txtFirstName.Text.Trim();
            if (newFirstName != (originalStudentData.FirstName ?? string.Empty))
                changedFields["FirstName"] = newFirstName;

            // გვარი
            string newLastName = txtLastName.Text.Trim();
            if (newLastName != (originalStudentData.LastName ?? string.Empty))
                changedFields["LastName"] = newLastName;

            // მშობელი
            string newParentName = txtParent.Text.Trim();
            if (newParentName != (originalStudentData.ParentName ?? string.Empty))
                changedFields["ParentName"] = newParentName;

            // ასაკი
            int newAge = int.TryParse(txtAge.Text, out int age) ? age : 0;
            if (newAge != originalStudentData.Age)
                changedFields["Age"] = newAge;

            // ტელეფონი
            string newPhone = txtPhone.Text.Trim();
            if (newPhone != (originalStudentData.PhoneNumber ?? string.Empty))
                changedFields["PhoneNumber"] = newPhone;

            // პირადი ნომერი
            long newIdNumb = long.TryParse(txtPersonalId.Text, out long idNumb) ? idNumb : 0;
            if (newIdNumb != originalStudentData.Id_Numb)
                changedFields["Id_Numb"] = newIdNumb;

            // მისამართი
            string newAddress = txtAddress.Text.Trim();
            if (newAddress != (originalStudentData.Address ?? string.Empty))
                changedFields["Address"] = newAddress;

            // ინფორმაცია/შენიშვნა
            string newInfo = txtStatus.Text.Trim();
            if (newInfo != (originalStudentData.Info ?? string.Empty))
                changedFields["Info"] = newInfo;

            // ბალანსი
            decimal newBalance = decimal.TryParse(txtBalance.Text, out decimal balance) ? balance : 0;
            if (newBalance != originalStudentData.Balance)
                changedFields["Balance"] = newBalance;

            // თუ არის შეცვლილი ველები - განახლება
            if (changedFields.Count > 0)
            {
                _studentSvc.UpdateStudentFields(studentId, changedFields);
            }
        }

        /// <summary>
        /// ჯგუფების ცვლილებების შენახვა
        /// </summary>
        private void SaveGroupChanges()
        {
            // მიმდინარე მონიშნული ჯგუფების ID-ები
            var currentCheckedGroupIds = new HashSet<int>();
            for (int i = 0; i < chlGroups.Items.Count; i++)
            {
                if (chlGroups.GetItemChecked(i))
                {
                    var groupObj = (Group)chlGroups.Items[i];
                    currentCheckedGroupIds.Add(groupObj.Id);
                }
            }

            // ახალი ჯგუფები (დაემატა)
            var addedGroupIds = currentCheckedGroupIds.Except(originalCheckedGroupIds).ToList();
            
            // წაშლილი ჯგუფები (ამოიღეს)
            var removedGroupIds = originalCheckedGroupIds.Except(currentCheckedGroupIds).ToList();

            // ახალი ჯგუფების დამატება
            foreach (var newGroupId in addedGroupIds)
            {
                AddStudentToGroup(studentId, newGroupId);
            }

            // ძველი ჯგუფებიდან ამოღება
            foreach (var oldGroupId in removedGroupIds)
            {
                RemoveStudentFromGroup(studentId, oldGroupId);
            }
        }

        /// <summary>
        /// მოსწავლის ჯგუფში დამატება (StudentGroups + StudentSubGroups + რაოდენობები)
        /// </summary>
        private void AddStudentToGroup(int studId, int grpId)
        {
            // ჯგუფის ფასის მიღება
            var group = _groupService.GetGroupById(grpId);
            if (group == null) return;

            // StudentGroups-ში დამატება
            _studentSvc.AddStudentToGroup(studId, grpId, true, null, "Pending", group.Price, 0);

            // პირველი ქვეჯგუფის მიღება და StudentSubGroups-ში დამატება
            var subGroups = _subGroupService.GetSubGroupsByGroupId(grpId);
            if (subGroups != null && subGroups.Any())
            {
                var firstSubGroup = subGroups.First();
                _studentSvc.AddStudentToSubGroup(studId, grpId, firstSubGroup.Id, "Pending", null, firstSubGroup.TuitionFee, 0, true);
            }

            // Groups.StudentCount გაზრდა (AddStudentToGroup მეთოდში უკვე ხდება)
        }

        /// <summary>
        /// მოსწავლის ჯგუფიდან ამოღება (Soft Delete + რაოდენობები)
        /// </summary>
        private void RemoveStudentFromGroup(int studId, int grpId)
        {
            // StudentService-ის მეთოდი რომელიც ასრულებს:
            // - StudentGroups.Status = 0
            // - StudentSubGroups.Status = 0
            // - Groups.StudentCount--
            // - SubGroups.StudentCount--
            _studentSvc.RemoveStudentFromGroup(studId, grpId);
        }

        /// <summary>
        /// ქვეჯგუფების ცვლილებების შენახვა
        /// </summary>
        private void SaveSubGroupChanges()
        {
            // dgvGroupSubGroups-დან ვიღებთ მიმდინარე მნიშვნელობებს
            foreach (DataGridViewRow row in dgvGroupSubGroups.Rows)
            {
                if (row.IsNewRow) continue;

                var rowGroupId = Convert.ToInt32(row.Cells["GroupId"].Value);
                var newSubGroupId = row.Cells["SubGroup"].Value != null ? Convert.ToInt32(row.Cells["SubGroup"].Value) : 0;

                if (newSubGroupId <= 0) continue;

                // ვიპოვოთ ორიგინალი ქვეჯგუფის ID ამ ჯგუფისთვის
                var originalSubGroupId = GetOriginalSubGroupId(rowGroupId);

                // თუ ქვეჯგუფი შეიცვალა
                if (originalSubGroupId != newSubGroupId && originalSubGroupId > 0)
                {
                    // StudentService/SubGroupService-ის მეთოდი:
                    // - ძველი SubGroup.StudentCount--
                    // - UPDATE StudentSubGroups SET SubGroupId = newId
                    // - ახალი SubGroup.StudentCount++
                    _subGroupService.UpdateStudentSubGroup(studentId, rowGroupId, newSubGroupId, originalSubGroupId);
                }
                else if (originalSubGroupId <= 0 && newSubGroupId > 0)
                {
                    // ახალი ქვეჯგუფის დამატება (თუ არ არსებობდა)
                    var subGroup = _subGroupService.GetSubGroupById(newSubGroupId);
                    if (subGroup != null)
                    {
                        _studentSvc.AddStudentToSubGroup(studentId, rowGroupId, newSubGroupId, "Pending", null, subGroup.TuitionFee, 0, true);
                    }
                }
            }
        }

        /// <summary>
        /// ორიგინალი ქვეჯგუფის ID-ის მიღება ჯგუფისთვის
        /// </summary>
        private int GetOriginalSubGroupId(int grpId)
        {
            if (originalStudentData?.StudentGroupsList == null) return 0;

            var studentGroup = originalStudentData.StudentGroupsList.FirstOrDefault(sg => sg.GroupId == grpId);
            return studentGroup?.SubGroupId ?? 0;
        }

        /// <summary>
        /// სტატუსის ცვლილების შენახვა
        /// </summary>
        private void SaveStatusChange()
        {
            bool newStatus = chkBoxStatus.Checked;
            
            // მხოლოდ მიმდინარე ჯგუფის სტატუსის შეცვლა
            if (groupId > 0)
            {
                _studentSvc.UpdateStudentStatus(studentId, groupId, newStatus);
            }
        }

        /// <summary>
        /// შენახვის შემდეგ მონაცემების და UI-ს სრული განახლება
        /// </summary>
        private void RefreshAfterSave()
        {
            isLoadingData = true; // პროგრამული ჩატვირთვის დაწყება
            
            try
            {
                // 1. DataGridView-ის განახლება
                var updatedStudents = _studentService.GetAllStudentsWithGroups();
                RefreshDataGridView(updatedStudents);

                // 2. მოსწავლის მონაცემების ხელახლა ჩატვირთვა
                var refreshedStudent = _studentSvc.GetStudentDetailsById(studentId, groupId);
                if (refreshedStudent != null)
                {
                    refreshedStudent.StudentGroupsList = _groupService.GetStudentGroupsByStudentId(studentId);
                    refreshedStudent.StudentSubGroupsList = _subGroupService.GetAllSubGroups();
                    
                    // 3. originalStudentData-ს განახლება
                    originalStudentData = refreshedStudent;

                    // 4. ქვეჯგუფების გრიდის განახლება
                    LoadStudentSubGroupsToGrid(refreshedStudent);

                    // 5. ჯგუფების CheckedListBox-ის განახლება
                    isProgrammaticCheck = true;
                    for (int i = 0; i < chlGroups.Items.Count; i++)
                    {
                        var groupObj = (Group)chlGroups.Items[i];
                        bool isInGroup = refreshedStudent.StudentGroupsList.Any(g => g.GroupId == groupObj.Id);
                        chlGroups.SetItemChecked(i, isInGroup);
                    }
                    isProgrammaticCheck = false;

                    // 6. originalCheckedGroupIds-ის განახლება
                    originalCheckedGroupIds.Clear();
                    foreach (var sg in refreshedStudent.StudentGroupsList)
                    {
                        originalCheckedGroupIds.Add(sg.GroupId);
                    }

                    // 7. ტექსტური ველების განახლება
                    txtFirstName.Text = refreshedStudent.FirstName;
                    txtLastName.Text = refreshedStudent.LastName;
                    txtParent.Text = refreshedStudent.ParentName;
                    txtAge.Text = refreshedStudent.Age.ToString();
                    txtPhone.Text = refreshedStudent.PhoneNumber;
                    txtPersonalId.Text = refreshedStudent.Id_Numb.ToString();
                    txtAddress.Text = refreshedStudent.Address;
                    txtStatus.Text = refreshedStudent.Info ?? string.Empty;
                    txtBalance.Text = refreshedStudent.Balance.ToString("0.00");
                    lblStudentCode.Text = refreshedStudent.StudentCode;
                }
            }
            finally
            {
                isLoadingData = false; // პროგრამული ჩატვირთვის დასრულება
            }

            // 8. ყველა ფლაგის განულება
            hasChanges = false;
            groupsChanged = false;
            subGroupsChanged = false;
            statusChanged = false;

            // 9. ფერების რესეტი
            ResetTextBoxColors();
            chlGroups.BackColor = SystemColors.Window;
            dgvGroupSubGroups.BackColor = SystemColors.Window;

            // 10. selectedStudents dictionary-ის გასუფთავება
            selectedStudents.Clear();
        }
        private void btnCancelChanges_Click(object sender, EventArgs e)
        {
            if (!hasChanges && !statusChanged && !subGroupsChanged && !groupsChanged) return;

            DialogResult result = MessageBox.Show("Are you sure you want to discard changes?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (dgvStudents.CurrentCell != null)
                {
                    dgvStudents_CellDoubleClick(null, new DataGridViewCellEventArgs(dgvStudents.CurrentCell.ColumnIndex, dgvStudents.CurrentCell.RowIndex));
                }
                hasChanges = false;
                statusChanged = false;
                subGroupsChanged = false;
                groupsChanged = false;
                ResetTextBoxColors();
            }
        }
        private void ResetTextBoxColors()
        {
            txtFirstName.BackColor = Color.White;
            txtLastName.BackColor = Color.White;
            txtParent.BackColor = Color.White;
            txtAge.BackColor = Color.White;
            txtPhone.BackColor = Color.White;
            txtPersonalId.BackColor = Color.White;
            txtAddress.BackColor = Color.White;
            txtRegistrationDate.BackColor = Color.White;
            txtPaymentDate.BackColor = Color.White;
            txtStatus.BackColor = Color.White;
            txtPaymentStatus.BackColor = Color.White;
            // ტექსტური ველების მნიშვნელობების განულება UI ფერის აღდგენისას არასაჭიროა, მაგრამ Info-ს არ დავტოვოთ ბინადარი
            if (originalStudentData == null)
            {
                txtStatus.Text = string.Empty;
            }
        }
        /// <summary>
        /// DataGridView-ზე უჯრაზე დაკლიკება - მოსწავლის არჩევა და მონაცემების ჩატვირთვა
        /// </summary>
        private void dgvStudents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Header-ზე დაკლიკება - იგნორირება
            if (e.RowIndex < 0) return;

            // Select სვეტზე დაკლიკება - checkbox-ის ლოგიკა (ცალკე მეთოდში)
            if (e.ColumnIndex == dgvStudents.Columns["Select"]?.Index)
            {
                HandleSelectCheckboxClick(e.RowIndex);
                return;
            }

            // სხვა უჯრაზე დაკლიკება - მოსწავლის არჩევა და მონაცემების ჩატვირთვა
            LoadSelectedStudentData(e.RowIndex);
        }

        /// <summary>
        /// DataGridView-ზე ორმაგი დაკლიკება - მოსწავლის არჩევა
        /// </summary>
        private void dgvStudents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            LoadSelectedStudentData(e.RowIndex);
        }

        /// <summary>
        /// Select checkbox-ზე დაკლიკების დამუშავება
        /// </summary>
        private void HandleSelectCheckboxClick(int rowIndex)
        {
            var row = dgvStudents.Rows[rowIndex];
            int studId = Convert.ToInt32(row.Cells["Id"].Value);
            int grpId = Convert.ToInt32(row.Cells["GroupId"].Value);
            
            // მიმდინარე მნიშვნელობის ინვერსია
            bool currentValue = row.Cells["Select"].Value != null && (bool)row.Cells["Select"].Value;
            bool newValue = !currentValue;
            row.Cells["Select"].Value = newValue;

            // Dictionary-ში შენახვა
            var key = (studId, grpId);
            if (selectedStudents.ContainsKey(key))
            {
                selectedStudents[key] = newValue;
            }
            else
            {
                selectedStudents.Add(key, newValue);
            }
        }

        /// <summary>
        /// არჩეული მოსწავლის მონაცემების ჩატვირთვა ფორმაში
        /// </summary>
        private void LoadSelectedStudentData(int rowIndex)
        {
            // 1. წინა ცვლილებების შემოწმება
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "გაქვთ შეუნახავი ცვლილებები. გინდათ შენახვა?",
                    "შეუნახავი ცვლილებები",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    btnSaveChanges_Click(null, null);
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // არ შეცვალოთ არჩეული მოსწავლე
                }
                // No - გააგრძელეთ შეუნახავი ცვლილებების დაკარგვით
            }

            // 2. ფლაგების განულება
            ResetAllFlags();

            // 3. არჩეული მოსწავლის ID-ების მიღება
            var row = dgvStudents.Rows[rowIndex];
            studentId = Convert.ToInt32(row.Cells["Id"].Value);
            
            // GroupId შეიძლება იყოს NULL თუ მოსწავლე რამდენიმე ჯგუფშია (GROUP_CONCAT გამოყენებისას)
            var groupIdValue = row.Cells["GroupId"].Value;
            groupId = (groupIdValue == null || groupIdValue == DBNull.Value) ? 0 : Convert.ToInt32(groupIdValue);

            // 4. მოსწავლის სრული მონაცემების ჩატვირთვა
            var student = _studentSvc.GetStudentDetailsById(studentId, groupId);
            if (student == null)
            {
                MessageBox.Show("მოსწავლის მონაცემები ვერ მოიძებნა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearStudentForm();
                return;
            }

            // 5. StudentGroups და SubGroups ჩატვირთვა
            student.StudentGroupsList = _groupService.GetStudentGroupsByStudentId(studentId);
            student.StudentSubGroupsList = _subGroupService.GetAllSubGroups();

            // 6. originalStudentData-ს შენახვა (ცვლილებების შესადარებლად)
            originalStudentData = student;

            // 7. ტექსტური ველების შევსება
            PopulateStudentFields(student);

            // 8. სტატუსის მიხედვით ველების ჩართვა/გამორთვა
            bool isActive = student.Status == true;
            FieldStatusAsStudent(isActive);

            // 9. ჯგუფების CheckedListBox-ის განახლება
            UpdateGroupsCheckedListBox(student);

            // 10. ქვეჯგუფების DataGridView-ის განახლება
            LoadStudentSubGroupsToGrid(student);

            // 11. ფერების რესეტი
            ResetTextBoxColors();
            chlGroups.BackColor = SystemColors.Window;
            dgvGroupSubGroups.BackColor = SystemColors.Window;
        }

        /// <summary>
        /// შეუნახავი ცვლილებების შემოწმება
        /// </summary>
        private bool HasUnsavedChanges()
        {
            return hasChanges || groupsChanged || subGroupsChanged || statusChanged;
        }

        /// <summary>
        /// ყველა ფლაგის განულება
        /// </summary>
        private void ResetAllFlags()
        {
            hasChanges = false;
            groupsChanged = false;
            subGroupsChanged = false;
            statusChanged = false;
        }

        /// <summary>
        /// მოსწავლის ფორმის გასუფთავება
        /// </summary>
        private void ClearStudentForm()
        {
            isLoadingData = true; // პროგრამული ცვლილებების ფლაგი
            
            try
            {
                studentId = 0;
                groupId = 0;
                originalStudentData = null;

                txtFirstName.Text = string.Empty;
                txtLastName.Text = string.Empty;
                txtParent.Text = string.Empty;
                txtAge.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtPersonalId.Text = string.Empty;
                txtAddress.Text = string.Empty;
                txtRegistrationDate.Text = string.Empty;
                txtPaymentDate.Text = string.Empty;
                txtStatus.Text = string.Empty;
                txtPaymentStatus.Text = string.Empty;
                txtBalance.Text = string.Empty;
                lblStudentCode.Text = string.Empty;

                // ჯგუფების გასუფთავება
                isProgrammaticCheck = true;
                for (int i = 0; i < chlGroups.Items.Count; i++)
                {
                    chlGroups.SetItemChecked(i, false);
                }
                isProgrammaticCheck = false;
                originalCheckedGroupIds.Clear();

                // ქვეჯგუფების გრიდის გასუფთავება
                dgvGroupSubGroups.Rows.Clear();

                ResetAllFlags();
                ResetTextBoxColors();
            }
            finally
            {
                isLoadingData = false;
            }
        }

        /// <summary>
        /// მოსწავლის ტექსტური ველების შევსება
        /// </summary>
        private void PopulateStudentFields(Student student)
        {
            isLoadingData = true; // პროგრამული ჩატვირთვის დაწყება
            
            try
            {
                txtFirstName.Text = student.FirstName ?? string.Empty;
                txtLastName.Text = student.LastName ?? string.Empty;
                txtParent.Text = student.ParentName ?? string.Empty;
                txtAge.Text = student.Age.ToString();
                txtPhone.Text = student.PhoneNumber ?? string.Empty;
                txtPersonalId.Text = student.Id_Numb.ToString();
                txtAddress.Text = student.Address ?? string.Empty;
                txtRegistrationDate.Text = student.RegistrationDate.ToString("dd.MM.yyyy") ?? string.Empty;
                txtStatus.Text = student.Info ?? string.Empty;
                txtBalance.Text = student.Balance.ToString("0.00");
                lblStudentCode.Text = student.StudentCode ?? string.Empty;

                // ჯგუფიდან მოსული მონაცემები (StudentGroups)
                // თუ groupId > 0, ვეძებთ კონკრეტულ ჯგუფს StudentGroupsList-ში
                // თუ groupId <= 0 (რამდენიმე ჯგუფშია), ვიყენებთ student ობიექტში შენახულ მონაცემებს
                if (groupId > 0)
                {
                    var currentGroup = student.StudentGroupsList?.FirstOrDefault(g => g.GroupId == groupId);
                    if (currentGroup != null)
                    {
                        txtPaymentDate.Text = currentGroup.DateOfPayment?.ToString("dd.MM.yyyy") ?? string.Empty;
                        txtPaymentStatus.Text = currentGroup.PaymentStatus ?? string.Empty;
                        chkBoxStatus.Checked = currentGroup.Status;
                        chkBoxStatus.Text = currentGroup.Status ? "აქტიური" : "არააქტიური";
                    }
                    else
                    {
                        txtPaymentDate.Text = string.Empty;
                        txtPaymentStatus.Text = string.Empty;
                        chkBoxStatus.Checked = false;
                        chkBoxStatus.Text = "არააქტიური";
                    }
                }
                else
                {
                    // რამდენიმე ჯგუფშია - ვიყენებთ student ობიექტში შენახულ მონაცემებს (MAX მნიშვნელობები)
                    txtPaymentDate.Text = student.DateOfPayment?.ToString("dd.MM.yyyy") ?? string.Empty;
                    txtPaymentStatus.Text = student.PaymentStatus ?? string.Empty;
                    chkBoxStatus.Checked = student.Status;
                    chkBoxStatus.Text = student.Status ? "აქტიური" : "არააქტიური";
                }
            }
            finally
            {
                isLoadingData = false; // პროგრამული ჩატვირთვის დასრულება
            }
        }

        /// <summary>
        /// ჯგუფების CheckedListBox-ის განახლება
        /// </summary>
        private void UpdateGroupsCheckedListBox(Student student)
        {
            isProgrammaticCheck = true;
            originalCheckedGroupIds.Clear();

            for (int i = 0; i < chlGroups.Items.Count; i++)
            {
                var groupObj = (Group)chlGroups.Items[i];
                bool isInGroup = student.StudentGroupsList?.Any(g => g.GroupId == groupObj.Id) ?? false;
                chlGroups.SetItemChecked(i, isInGroup);

                if (isInGroup)
                {
                    originalCheckedGroupIds.Add(groupObj.Id);
                }
            }

            isProgrammaticCheck = false;
        }
        
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchStudents();
        }
        private void SearchStudents()
        {
            string searchText = txtSearch.Text.Trim();
            List<Student> results = new List<Student>();
            int selectedGroupId = cbGroups.SelectedValue != null ? Convert.ToInt32(cbGroups.SelectedValue) : -1;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgvStudents.DataSource = null;
                return;
            }

            if (rbByName.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("FirstName", searchText) : _studentService.SearchStudents("FirstName", searchText, selectedGroupId);
                RefreshDataGridView(results);
            }
            else if (rbByLastName.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("LastName", searchText) : _studentService.SearchStudents("LastName", searchText, selectedGroupId);
                RefreshDataGridView(results);
                //results = _studentService.SearchByLastName(searchText);
            }
            else if (rbByParent.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("ParentName", searchText) : _studentService.SearchStudents("ParentName", searchText, selectedGroupId);
                RefreshDataGridView(results);
            }
            else if (rbByAge.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("Age", searchText) : _studentService.SearchStudents("Age", searchText, selectedGroupId);
                RefreshDataGridView(results);
            }
            else if (rbByIdNumber.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("Id_Numb", searchText) : _studentService.SearchStudents("Id_Numb", searchText, selectedGroupId);
                RefreshDataGridView(results);
            }
            else if (rbByAddress.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("Address", searchText) : _studentService.SearchStudents("Address", searchText, selectedGroupId);
                RefreshDataGridView(results);
            }
            else if (rbByStCode.Checked)
            {
                results = selectedGroupId == 0 ? _studentService.SearchStudents("StudentCode", searchText) : _studentService.SearchStudents("StudentCode", searchText, selectedGroupId);
                RefreshDataGridView(results);
            }

            RefreshDataGridView(results);
        }

        private void chlGroups_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (isProgrammaticCheck)
                return;

            var group = (Group)chlGroups.Items[e.Index];
            var newCheckedIds = new HashSet<int>(originalCheckedGroupIds);

            if (e.NewValue == CheckState.Checked)
            {
                newCheckedIds.Add(group.Id);
            }
            else
            {
                newCheckedIds.Remove(group.Id);
            }
            
            groupsChanged = !newCheckedIds.SetEquals(originalCheckedGroupIds);
            chlGroups.BackColor = groupsChanged ? Color.LightYellow : SystemColors.Window;
        }

        private void chkBoxStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (isProgrammaticCheck || isLoadingData) return;
            statusChanged = true;
        }

        private void btnStudentActivation_Click(object sender, EventArgs e)
        {
            if (_studentService.UpdateStudentStatus(studentId, groupId, true))
            {
                txtFirstName.Enabled = true;
                txtLastName.Enabled = true;
                txtParent.Enabled = true;
                txtAge.Enabled = true;
                txtPhone.Enabled = true;
                txtPersonalId.Enabled = true;
                txtAddress.Enabled = true;
                txtRegistrationDate.Enabled = true;
                txtPaymentDate.Enabled = true;
                txtStatus.Enabled = true;
                txtPaymentStatus.Enabled = true;
                _backupManager.DbChangedSinceLastBackup = true;

            }
        }
    }
}

