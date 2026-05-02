using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Services;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class GroupManagementForm : Form
    {
        private readonly IGroupService _groupService;
        private readonly ISubGroupService _subGroupService;
        private readonly IStudentService _studentService;
        private readonly IStudentGroupsService _studentGroupsService;
        private readonly BackupService _backupManager;
        private readonly IUserContext _userContext;
        //private Button btnDeleteGroup;
        private int? _currentGroupId = null; // კლასის member
        private SubGroup _subgroup;
        private List<NumericUpDown> _subGroupMaxStudentsControls = new List<NumericUpDown>();
        private List<Label> _subGroupLabels = new List<Label>();
        public GroupManagementForm(IGroupService groupService, ISubGroupService subGroupService, BackupService backupManager, IStudentService studentService, IStudentGroupsService studentGroupsService, IUserContext userContext)
        {
            InitializeComponent();
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ახალი ჯგუფების და ქვეჯგუფების შექმნა");
            else FormTitleHelper.SetTitle(this, "ახალი ჯგუფების და ქვეჯგუფების შექმნა - სატესტო რეჟიმი");
            _groupService = groupService;
            _subGroupService = subGroupService;
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            _studentGroupsService = studentGroupsService ?? throw new ArgumentNullException(nameof(studentGroupsService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            // btnDeleteGroup = new Button { Text = "წაშლა", Width = 80 };
            //btnDeleteGroup.Enabled = false;
            // btnDeleteGroup.TabIndex = 8;
            // btnDeleteGroup.Click += BtnDeleteGroup_Click;
            // this.Controls.Add(btnDeleteGroup);
            // btnDeleteGroup.Location = new System.Drawing.Point(11, 280);
            txtGroupName.TextChanged += TxtGroupName_TextChanged;
            btnAddGroup.Enabled = false;

            txtGroupName.TextChanged += (s, e) => ValidateGroupFields();
            txtPrice.TextChanged += (s, e) => ValidateGroupFields();
            txtDocPath.TextChanged += (s, e) => ValidateGroupFields();
            numMaxStudents.ValueChanged += (s, e) =>
            {
                ValidateGroupFields();
            };

            // ქვეჯგუფზე MaxStudents კონტროლი გაუქმებულია: მხოლოდ Group-ზე მუშაობს.
            chkEnableSubGroupMaxStudents.Checked = false;
            chkEnableSubGroupMaxStudents.Visible = false;
            groupBox1.Visible = false;

            // ... სხვა საჭირო ველები

            ValidateGroupFields(); // თავდაპირველი ინიციალიზაცია
            _studentService = studentService;
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
            ClearSubGroupControls(); // ქვეჯგუფების კონტროლების გასუფთავება
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
            if (cmbSubGroupCount.SelectedIndex <= 0)
            {
                MessageBox.Show("გთხოვთ აირჩიოთ ქვეჯგუფების რაოდენობა.");
                return;
            }

            var group = _groupService.GetGroupById(_currentGroupId.Value);
            if (group == null) return;

            int subGroupCount = int.Parse(cmbSubGroupCount.SelectedItem.ToString());

            for (int i = 1; i <= subGroupCount; i++)
            {
                var newSubGroup = new SubGroup
                {
                    GroupId = _currentGroupId.Value,
                    TuitionFee = group.Price,
                    MaxStudents = 0,
                    Status = true
                };
                newSubGroup.Name = $"კლასი {i}";
                _subGroupService.AddSubGroup(newSubGroup);
            }

            ClearGroupFields();
            LoadGroups();
            MessageBox.Show("ჯგუფისთვის - " + txtGroupName.Text + " დაემატა: " + subGroupCount + " კლასი!");
        }
        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanManageGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (btnAddGroup.Text == "შენახვა")
            {
                AddSubGroups(); btnAddGroup.Text = "დამატება"; return;
            }

            // ვალიდაცია: შეამოწმე რომ ყველა საჭირო ველი შევსებულია
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ ჯგუფის სახელი.", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroupName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ ჯგუფის ფასი.", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ სწორი ფასი (დადებითი რიცხვი).", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                txtPrice.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDocPath.Text))
            {
                MessageBox.Show("გთხოვთ აირჩიოთ კონტრაქტის შაბლონი.", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnFileDialog.Focus();
                return;
            }

            if (numMaxStudents.Value <= 0)
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ მოსწავლეების მაქსიმალური რაოდენობა (1-ზე მეტი).", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numMaxStudents.Focus();
                return;
            }

            try
            {
                Group group = new Group
                {
                    Name = txtGroupName.Text.Trim(),
                    Price = price,
                    MaxStudents = Convert.ToInt32(numMaxStudents.Value),
                    Teacher = txtTeacher.Text?.Trim() ?? string.Empty,
                    ContractTemplatePath = txtDocPath.Text.Trim(),
                    Status = true
                };

                int? groupId = _groupService.AddGroup(group);

                if (groupId == null || groupId.Value <= 0)
                {
                    MessageBox.Show("ჯგუფის დამატება ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _backupManager.DbChangedSinceLastBackup = true;

                if (cmbSubGroupCount.SelectedIndex > 0)
                {
                    // ვალიდაცია: cmbSubGroupCount-ის SelectedItem
                    if (cmbSubGroupCount.SelectedItem == null)
                    {
                        MessageBox.Show("გთხოვთ აირჩიოთ ქვეჯგუფების რაოდენობა.", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!int.TryParse(cmbSubGroupCount.SelectedItem.ToString(), out int subGroupCount) || subGroupCount <= 0)
                    {
                        MessageBox.Show("გთხოვთ აირჩიოთ სწორი ქვეჯგუფების რაოდენობა.", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // ქვეჯგუფზე MaxStudents გაუქმებულია (0 = შეუზღუდავი)
                    for (int i = 1; i <= subGroupCount; i++)
                    {
                        var newSubGroup = new SubGroup
                        {
                            GroupId = groupId.Value,
                            TuitionFee = group.Price,
                            MaxStudents = 0,
                            Status = true
                        };
                        newSubGroup.Name = $"კლასი {i}";
                        _subGroupService.AddSubGroup(newSubGroup);
                    }
                }

                MessageBox.Show("ჯგუფი წარმატებით შეიქმნა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearGroupFields();
                LoadGroups();
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"შეცდომა მონაცემების ფორმატში: {ex.Message}", "ვალიდაციის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException ex)
            {
                MessageBox.Show($"შეცდომა: მითითებული რიცხვი ძალიან დიდია. {ex.Message}", "ვალიდაციის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"მოხდა შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GroupManagementForm_Load(object sender, EventArgs e)
        {
            //LoadParentGroups();
            LoadGroups();
            ApplySecurityChecks();
        }

        private void ApplySecurityChecks()
        {
            // btnAddGroup - CanManageGroups permission
            if (btnAddGroup != null)
            {
                btnAddGroup.Tag = $"Permission_{Permission.CanManageGroups}";
                btnAddGroup.Enabled = _userContext.HasPermission(Permission.CanManageGroups);
            }

            // btnDeleteGroup - CanManageGroups permission (or CanDelete, but Groups management typically uses CanManageGroups)
            if (btnDeleteGroup != null)
            {
                btnDeleteGroup.Tag = $"Permission_{Permission.CanManageGroups}";
                btnDeleteGroup.Enabled = _userContext.HasPermission(Permission.CanManageGroups);
            }

            // tsEditGroups - CanManageGroups permission
            if (tsEditGroups != null)
            {
                tsEditGroups.Tag = $"Permission_{Permission.CanManageGroups}";
                tsEditGroups.Enabled = _userContext.HasPermission(Permission.CanManageGroups);
            }

            // tsEditSubGroups - CanManageGroups permission
            if (tsEditSubGroups != null)
            {
                tsEditSubGroups.Tag = $"Permission_{Permission.CanManageGroups}";
                tsEditSubGroups.Enabled = _userContext.HasPermission(Permission.CanManageGroups);
            }
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
            // Security check
            if (!_userContext.HasPermission(Permission.CanManageGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvMainGroup.CurrentRow == null)
            {
                MessageBox.Show("გთხოვთ აირჩიოთ ჯგუფი.");
                return;
            }
            int groupId = (int)dgvMainGroup.CurrentRow.Cells["Id"].Value;
            string groupName = dgvMainGroup.CurrentRow.Cells["Name"].Value.ToString();

            var studentsInGroup = _studentService.GetStudentsByGroupId(groupId);
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
                        _studentService.MigrateStudentToGroup(student.Id, groupId, newGroupId);
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
                    _studentGroupsService.ArchiveStudentFromGroup(student.Id, groupId);
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
        private void ChkEnableSubGroupMaxStudents_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnableCustomSubGroupMaxStudents = chkEnableSubGroupMaxStudents.Checked;
            Properties.Settings.Default.Save();
            UpdateGroupBox1Enabled();

            // თუ გამორთულია, გავასუფთავოთ კონტროლები
            if (!chkEnableSubGroupMaxStudents.Checked)
            {
                ClearSubGroupControls();
            }
            else if (cmbSubGroupCount.SelectedIndex > 0)
            {
                // თუ ჩართულია და ქვეჯგუფების რაოდენობა არჩეულია, განვაახლოთ
                cmbSubGroupCount_TextChanged(sender, e);
            }
        }

        private void UpdateGroupBox1Enabled()
        {
            groupBox1.Enabled = chkEnableSubGroupMaxStudents.Checked;
        }

        private void cmbSubGroupCount_TextChanged(object sender, EventArgs e)
        {
            ClearSubGroupControls();

            // თუ CheckBox გამორთულია, არ გავაგრძელოთ
            if (!chkEnableSubGroupMaxStudents.Checked)
            {
                return;
            }

            if (cmbSubGroupCount.SelectedIndex <= 0)
            {
                groupBox1.Text = "ქვეჯგუფები";
                return;
            }

            int subGroupCount = int.Parse(cmbSubGroupCount.SelectedItem.ToString());
            if (subGroupCount <= 0)
            {
                groupBox1.Text = "ქვეჯგუფები";
                return;
            }

            groupBox1.Text = $"ქვეჯგუფების მოსწავლეების რაოდენობა ({subGroupCount})";

            // ჯგუფის MaxStudents
            int groupMaxStudents = (int)numMaxStudents.Value;
            if (groupMaxStudents <= 0) groupMaxStudents = 1; // default

            // ნაგულისხმევი მნიშვნელობა თითოეული ქვეჯგუფისთვის
            int defaultMaxStudents = groupMaxStudents / subGroupCount;
            if (defaultMaxStudents <= 0) defaultMaxStudents = 1;

            // სიმაღლე თითოეული რიგისთვის
            int rowHeight = 35;
            int startY = 25;
            int labelWidth = 120;
            int numericWidth = 100;
            int spacing = 10;

            for (int i = 1; i <= subGroupCount; i++)
            {
                int yPos = startY + (i - 1) * rowHeight;

                // Label
                var label = new Label
                {
                    Text = $"კლასი {i}:",
                    Location = new System.Drawing.Point(10, yPos),
                    Size = new System.Drawing.Size(labelWidth, 25),
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
                };
                groupBox1.Controls.Add(label);
                _subGroupLabels.Add(label);

                // NumericUpDown
                var numericUpDown = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = groupMaxStudents,
                    Value = defaultMaxStudents,
                    Location = new System.Drawing.Point(10 + labelWidth + spacing, yPos),
                    Size = new System.Drawing.Size(numericWidth, 25),
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
                };
                numericUpDown.ValueChanged += (s, args) => ValidateSubGroupMaxStudents();
                groupBox1.Controls.Add(numericUpDown);
                _subGroupMaxStudentsControls.Add(numericUpDown);
            }

            // შემოწმება თავიდან
            ValidateSubGroupMaxStudents();
        }

        private void ClearSubGroupControls()
        {
            foreach (var control in _subGroupMaxStudentsControls)
            {
                groupBox1.Controls.Remove(control);
                control.Dispose();
            }
            _subGroupMaxStudentsControls.Clear();

            foreach (var label in _subGroupLabels)
            {
                groupBox1.Controls.Remove(label);
                label.Dispose();
            }
            _subGroupLabels.Clear();
        }

        private void ValidateSubGroupMaxStudents()
        {
            if (_subGroupMaxStudentsControls.Count == 0) return;

            int groupMaxStudents = (int)numMaxStudents.Value;
            if (groupMaxStudents <= 0) return;

            int total = _subGroupMaxStudentsControls.Sum(n => (int)n.Value);

            if (total > groupMaxStudents)
            {
                groupBox1.BackColor = System.Drawing.Color.LightCoral;
                // შეგვიძლია დავამატოთ label-იც შეტყობინებით
                // ან MessageBox, მაგრამ ValueChanged-ში MessageBox ძალიან არის intrusive
            }
            else
            {
                groupBox1.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void UpdateSubGroupDefaultValues()
        {
            if (_subGroupMaxStudentsControls.Count == 0) return;

            int groupMaxStudents = (int)numMaxStudents.Value;
            if (groupMaxStudents <= 0) return;

            int subGroupCount = _subGroupMaxStudentsControls.Count;
            int defaultMaxStudents = groupMaxStudents / subGroupCount;
            if (defaultMaxStudents <= 0) defaultMaxStudents = 1;

            // განვაახლოთ Maximum-ები
            foreach (var numeric in _subGroupMaxStudentsControls)
            {
                numeric.Maximum = groupMaxStudents;
                // თუ მიმდინარე მნიშვნელობა აღემატება ახალ maximum-ს, შევცვალოთ
                if (numeric.Value > groupMaxStudents)
                {
                    numeric.Value = groupMaxStudents;
                }
            }

            ValidateSubGroupMaxStudents();
        }
    }
}

