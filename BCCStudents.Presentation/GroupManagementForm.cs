using System;
using System.Windows.Forms;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Application.Services;

namespace BCCStudents.Presentation
{
    public partial class GroupManagementForm : Form
    {
        private readonly GroupService _groupService;
        private readonly SubGroupService _subGroupService;
        private Button btnDeleteGroup;
        private int? _currentGroupId = null; // კლასის member
        private SubGroup _subgroup;
        public GroupManagementForm(GroupService groupService, SubGroupService subGroupService)
        {
            InitializeComponent();
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ახალი ჯგუფების და ქვეჯგუფების შექმნა");
            else FormTitleHelper.SetTitle(this, "ახალი ჯგუფების და ქვეჯგუფების შექმნა - სატესტო რეჟიმი");
            _groupService = groupService;
            _subGroupService = subGroupService;
            btnDeleteGroup = new Button { Text = "წაშლა", Width = 80 };
            btnDeleteGroup.Enabled = false;
            btnDeleteGroup.TabIndex = 8;
            btnDeleteGroup.Click += BtnDeleteGroup_Click;
            this.Controls.Add(btnDeleteGroup);
            btnDeleteGroup.Location = new System.Drawing.Point(11, 280);
            txtGroupName.TextChanged += TxtGroupName_TextChanged;
            btnAddGroup.Enabled = false;

            txtGroupName.TextChanged += (s, e) => ValidateGroupFields();
            txtPrice.TextChanged += (s, e) => ValidateGroupFields();
            txtDocPath.TextChanged += (s, e) => ValidateGroupFields();
            // ... სხვა საჭირო ველები

            ValidateGroupFields(); // თავდაპირველი ინიციალიზაცია
        }

        private void TxtGroupName_TextChanged(object sender, EventArgs e)
        {
            var name = txtGroupName.Text.Trim();
            var group = _groupService.GetGroupByName(name);
            if (group != null)
            {
                var subGroups = _subGroupService.GetSubGroupsByGroupId(group.Id);
                if (name == group.Name)
                {
                    txtPrice.Text = Convert.ToString(group.Price);
                    txtTeacher.Text = group.Teacher;
                    txtDocPath.Text = group.ContractTemplatePath;
                    if (subGroups.Count > 0)
                    {
                        cmbSubGroupCount.SelectedIndex = subGroups.Count;
                    }
                    else if (subGroups.Count == 0)
                    {
                        cmbSubGroupCount.Focus();
                        cmbSubGroupCount.SelectedIndex = 0;
                        MessageBox.Show("ამ ჯგუფისთვის არ გაქვს კლასები, შეგიძლია დაამატო!", "შეტყობინება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnAddGroup.Text = "შენახვა";
                        btnAddGroup.Enabled = true;
                        _currentGroupId = group.Id;
                        _subgroup = new SubGroup { GroupId = group.Id, TuitionFee = group.Price, Status = true };
                    }
                }
            }
            else
            {
                txtPrice.Text = "";
                txtTeacher.Text = "";
                txtDocPath.Text = "";
                cmbSubGroupCount.SelectedIndex = -1;
            }
        }
        private void ClearGroupFields()
        {
            txtGroupName.Text = "";
            txtPrice.Text = "";
            txtTeacher.Text = "";
            txtDocPath.Text = "";
            cmbSubGroupCount.SelectedIndex = -1; // ან თუ გინდა, აირჩიოს პირველი, 0
                                                 // შეგიძლია დაამატო სხვა ველებიც, თუ გაქვს
        }

        private void ValidateGroupFields()
        {
            bool allFilled =
                !string.IsNullOrWhiteSpace(txtGroupName.Text)
                && !string.IsNullOrWhiteSpace(txtPrice.Text)
                && !string.IsNullOrWhiteSpace(txtDocPath.Text);

            btnAddGroup.Enabled = allFilled;
        }
        private void LoadGroups()
        {
            dgvMainGroup.DataSource = _groupService.GetAllGroups();
            if (dgvMainGroup.Rows.Count > 0)
                btnDeleteGroup.Enabled = true;
        }

        private void AddSubGroups()
        {
            var count = Convert.ToInt16(cmbSubGroupCount.SelectedItem);
            _subGroupService.AddSubGroups(_subgroup, count);
            ClearGroupFields();
            LoadGroups();
            MessageBox.Show("ჯგუფისთვის - " + txtGroupName.Text + " დაემატა: " + count + " კლასი!");
        }
        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            if (btnAddGroup.Text == "შენახვა")
            {
                AddSubGroups(); btnAddGroup.Text = "დამატება";  return;
            }
            Group group = new Group { Name = txtGroupName.Text, Price = Convert.ToDecimal(txtPrice.Text), Teacher = txtTeacher.Text, ContractTemplatePath = txtDocPath.Text, Status = true};
            
            if (string.IsNullOrWhiteSpace(group.Name))
            {
                MessageBox.Show("Please enter a group name.");
                return;
            }

            int? groupId = _groupService.AddGroup(group);
            SubGroup subGroup = new SubGroup { GroupId = groupId.Value,TuitionFee = group.Price, Status = true };
            if (groupId == null)
            {
                MessageBox.Show("ჯგუფის დამატება ვერ მოხერხდა.", "Error");
                return;
            }
            else
            {
                BackupManager.DbChangedSinceLastBackup = true;

                if (cmbSubGroupCount.SelectedIndex > 0)
                {
                    int subGroupCount = int.Parse(cmbSubGroupCount.SelectedItem.ToString());
                    _subGroupService.AddSubGroups(subGroup, subGroupCount);
                }

                MessageBox.Show("ჯგუფი წარმატებით შეიქმნა!");
                ClearGroupFields();
                LoadGroups();
            }
        }

        private void GroupManagementForm_Load(object sender, EventArgs e)
        {
            //LoadParentGroups();
            LoadGroups();

        }

        private void dataGridViewGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMainGroup.CurrentRow != null)
            {
                int selectedGroupId = (int)dgvMainGroup.CurrentRow.Cells["Id"].Value;
                dgvSubGroups.DataSource = _subGroupService.GetSubGroupsByGroupId(selectedGroupId);
            }
        }

        private void BtnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (dgvMainGroup.CurrentRow == null)
            {
                MessageBox.Show("გთხოვთ აირჩიოთ ჯგუფი.");
                return;
            }
            int groupId = (int)dgvMainGroup.CurrentRow.Cells["Id"].Value;
            string groupName = dgvMainGroup.CurrentRow.Cells["Name"].Value.ToString();

            var studentsInGroup = _groupService.GetStudentsByGroupId(groupId);
            if (studentsInGroup.Count == 0)
            {
                if (MessageBox.Show($"დარწმუნებული ხართ რომ გსურთ ჯგუფის წაშლა?", "დადასტურება", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _groupService.DeleteGroup(groupId);
                    MessageBox.Show("ჯგუფი წაიშალა.");
                    LoadGroups();
                }
                return;
            }

            var result = MessageBox.Show($"ჯგუფში '{groupName}' არის {studentsInGroup.Count} სტუდენტი.\n\nგსურთ მათი სხვა ჯგუფში გადაყვანა? დააჭირეთ 'Yes' თუ გსურთ გადაყვანა, 'No' თუ გსურთ უბრალოდ ჯგუფის გარეშე დატოვება.", "სტუდენტები ჯგუფში", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel) return;

            if (result == DialogResult.Yes)
            {
                var groups = _groupService.GetAllGroups().FindAll(g => g.Id != groupId);
                if (groups.Count == 0)
                {
                    MessageBox.Show("სხვა ჯგუფი არ არსებობს.");
                    return;
                }
                var selectForm = new Form { Text = "აირჩიეთ ახალი ჯგუფი", Width = 350, Height = 150, StartPosition = FormStartPosition.CenterParent };
                var cmbGroups = new ComboBox { DataSource = groups, DisplayMember = "Name", ValueMember = "Id", Dock = DockStyle.Top };
                var btnOk = new Button { Text = "გადაიყვანე", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom };
                selectForm.Controls.Add(cmbGroups);
                selectForm.Controls.Add(btnOk);
                if (selectForm.ShowDialog() == DialogResult.OK)
                {
                    int newGroupId = (int)cmbGroups.SelectedValue;
                    foreach (var student in studentsInGroup)
                    {
                        _groupService.MigrateStudentToGroup(student.Id, groupId, newGroupId);
                    }
                    _groupService.DeleteGroup(groupId);
                    MessageBox.Show("ჯგუფი წაიშალა და სტუდენტები გადაყვანილია ახალ ჯგუფში.");
                    LoadGroups();
                }
            }
            else if (result == DialogResult.No)
            {
                foreach (var student in studentsInGroup)
                {
                    _groupService.ArchiveStudentFromGroup(student.Id, groupId);
                }
                _groupService.DeleteGroup(groupId);
                MessageBox.Show("ჯგუფი წაიშალა. სტუდენტები დარჩნენ ჯგუფის გარეშე (სტატუსი: გადატანილი).");
                LoadGroups();
            }
        }

        private void btnFileDialog_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDocPath.Text = fileDialog.FileName;
                }
            }
        }
    }
}

