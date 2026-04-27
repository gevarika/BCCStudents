using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Services;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class GroupsEdit : Form
    {
        private readonly IGroupService _groupService;
        private readonly ISubGroupService _subGroupService;
        private readonly BackupService _backupManager;
        private readonly IUserContext _userContext;

        // UI Components
        private DataGridView dgvGroups;
        private DataGridView dgvSubGroups;
        private Panel pnlEditGroup;
        private Panel pnlEditSubGroup;

        // Group editing controls
        private TextBox txtGroupName;
        private TextBox txtGroupPrice;
        private TextBox txtGroupTeacher;
        private TextBox txtGroupContractPath;
        private TextBox txtGroupMaxStudents;
        private CheckBox chkGroupStatus;
        private Button btnBrowseContract;
        private Button btnSaveGroup;
        private Button btnCancelGroup;

        // SubGroup editing controls
        private TextBox txtSubGroupName;
        private TextBox txtSubGroupPrice;
        private TextBox txtSubGroupMaxStudents;
        private CheckBox chkSubGroupStatus;
        private Button btnSaveSubGroup;
        private Button btnCancelSubGroup;
        private Button btnDeleteSubGroup;

        // Close button
        private Button btnClose;

        // Current selection
        private Group _selectedGroup;
        private SubGroup _selectedSubGroup;

        public GroupsEdit(IGroupService groupService, ISubGroupService subGroupService, BackupService backupManager, IUserContext userContext)
        {
            InitializeComponent();
            _groupService = groupService;
            _subGroupService = subGroupService;
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            // Set form properties
            try
            {
                // Try to load icon from file
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "logo-new-32x32.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch
            {
                // If icon file not found, continue without icon
            }
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            InitializeCustomComponents();
            SetupEventHandlers();
            LoadGroups();
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ჯგუფების/ქვეჯგუფების რედაქტირება");
            else FormTitleHelper.SetTitle(this, "ჯგუფების/ქვეჯგუფების რედაქტირება - სატესტო რეჟიმი");

            // Apply security checks after form is loaded
            this.Load += GroupsEdit_Load;
        }

        private void GroupsEdit_Load(object sender, EventArgs e)
        {
            ApplySecurityChecks();
        }

        private void ApplySecurityChecks()
        {
            // Group editing panel - CanEditGroups permission
            if (pnlEditGroup != null)
            {
                bool canEditGroups = _userContext.HasPermission(Permission.CanEditGroups);
                pnlEditGroup.Enabled = canEditGroups && pnlEditGroup.Enabled; // Preserve current selection state

                // Individual controls in group panel
                if (btnSaveGroup != null)
                {
                    btnSaveGroup.Tag = $"Permission_{Permission.CanEditGroups}";
                    btnSaveGroup.Enabled = canEditGroups && _selectedGroup != null;
                }
            }

            // SubGroup editing panel - CanEditSubGroups permission
            if (pnlEditSubGroup != null)
            {
                bool canEditSubGroups = _userContext.HasPermission(Permission.CanEditSubGroups);
                pnlEditSubGroup.Enabled = canEditSubGroups && pnlEditSubGroup.Enabled; // Preserve current selection state

                // Individual controls in subgroup panel
                if (btnSaveSubGroup != null)
                {
                    btnSaveSubGroup.Tag = $"Permission_{Permission.CanEditSubGroups}";
                    btnSaveSubGroup.Enabled = canEditSubGroups && _selectedSubGroup != null;
                }

                if (btnDeleteSubGroup != null)
                {
                    btnDeleteSubGroup.Tag = $"Permission_{Permission.CanDeleteSubGroups}";
                    btnDeleteSubGroup.Enabled = _userContext.HasPermission(Permission.CanDeleteSubGroups) && _selectedSubGroup != null;
                }
            }
        }

        private void InitializeCustomComponents()
        {
            // Main layout - size is set in Designer file
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title labels
            var lblGroupsTitle = new Label
            {
                Text = "ჯგუფები",
                Location = new Point(12, 12),
                Size = new Size(480, 20),
                Font = new Font(this.Font, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSubGroupsTitle = new Label
            {
                Text = "ქვეჯგუფები",
                Location = new Point(12, 380),
                Size = new Size(480, 20),
                Font = new Font(this.Font, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Groups DataGridView - adjusted size
            dgvGroups = new DataGridView
            {
                Location = new Point(12, 35),
                Size = new Size(480, 340),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EditMode = DataGridViewEditMode.EditProgrammatically,
                BorderStyle = BorderStyle.None
            };

            // SubGroups DataGridView - adjusted size
            dgvSubGroups = new DataGridView
            {
                Location = new Point(12, 405),
                Size = new Size(480, 250),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EditMode = DataGridViewEditMode.EditProgrammatically,
                BorderStyle = BorderStyle.None
            };

            // Group Edit Panel - moved to right side
            pnlEditGroup = new Panel
            {
                Location = new Point(510, 35),
                Size = new Size(500, 340),
                BorderStyle = BorderStyle.FixedSingle
            };

            // SubGroup Edit Panel - moved to right side
            pnlEditSubGroup = new Panel
            {
                Location = new Point(510, 405),
                Size = new Size(500, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Group editing controls
            var lblGroupTitle = new Label
            {
                Text = "ჯგუფის რედაქტირება",
                Location = new Point(10, 10),
                Size = new Size(450, 20),
                Font = new Font(this.Font, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblGroupName = new Label { Text = "სახელი:", Location = new Point(10, 40), Size = new Size(80, 20) };
            txtGroupName = new TextBox { Location = new Point(140, 40), Size = new Size(300, 20) };

            var lblGroupPrice = new Label { Text = "ფასი:", Location = new Point(10, 70), Size = new Size(80, 20) };
            txtGroupPrice = new TextBox { Location = new Point(140, 70), Size = new Size(300, 20) };

            var lblGroupTeacher = new Label { Text = "მასწავლებელი:", Location = new Point(10, 100), Size = new Size(80, 20) };
            txtGroupTeacher = new TextBox { Location = new Point(140, 100), Size = new Size(300, 20) };

            var lblGroupContract = new Label { Text = "კონტრაქტი:", Location = new Point(10, 130), Size = new Size(80, 20) };
            txtGroupContractPath = new TextBox { Location = new Point(140, 130), Size = new Size(300, 20), ReadOnly = true };
            btnBrowseContract = new Button { Text = "...", Location = new Point(440, 130), Size = new Size(40, 25) };
            var lblGroupMaxStudents = new Label { Text = "მაქს. მოსწავლეები:", Location = new Point(10, 160), Size = new Size(130, 20) };
            txtGroupMaxStudents = new TextBox { Location = new Point(140, 160), Size = new Size(300, 20) };


            chkGroupStatus = new CheckBox { Text = "აქტიური", Location = new Point(140, 190), Size = new Size(100, 20), Checked = true };

            btnSaveGroup = new Button { Text = "შენახვა", Location = new Point(140, 230), Size = new Size(80, 25) };
            btnCancelGroup = new Button { Text = "გაუქმება", Location = new Point(240, 230), Size = new Size(80, 25) };

            // Add controls to group panel
            pnlEditGroup.Controls.AddRange(new Control[] {
                lblGroupTitle, lblGroupName, txtGroupName, lblGroupPrice, txtGroupPrice,
                lblGroupTeacher, txtGroupTeacher, lblGroupContract, txtGroupContractPath, lblGroupMaxStudents, txtGroupMaxStudents, btnBrowseContract,
                chkGroupStatus, btnSaveGroup, btnCancelGroup
            });

            // SubGroup editing controls
            var lblSubGroupTitle = new Label
            {
                Text = "ქვეჯგუფის რედაქტირება",
                Location = new Point(10, 10),
                Size = new Size(450, 20),
                Font = new Font(this.Font, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSubGroupName = new Label { Text = "სახელი:", Location = new Point(10, 40), Size = new Size(80, 20) };
            txtSubGroupName = new TextBox { Location = new Point(140, 40), Size = new Size(300, 20) };

            var lblSubGroupPrice = new Label { Text = "ფასი:", Location = new Point(10, 70), Size = new Size(80, 20) };
            txtSubGroupPrice = new TextBox { Location = new Point(140, 70), Size = new Size(300, 20) };
            var lblSubGroupMaxStudents = new Label { Text = "მაქს. მოსწავლეები:", Location = new Point(10, 100), Size = new Size(130, 20) };
            txtSubGroupMaxStudents = new TextBox { Location = new Point(140, 100), Size = new Size(300, 20) };
            chkSubGroupStatus = new CheckBox { Text = "აქტიური", Location = new Point(140, 130), Size = new Size(100, 20), Checked = true };

            btnSaveSubGroup = new Button { Text = "შენახვა", Location = new Point(140, 160), Size = new Size(80, 25) };
            btnCancelSubGroup = new Button { Text = "გაუქმება", Location = new Point(230, 160), Size = new Size(80, 25) };
            btnDeleteSubGroup = new Button { Text = "წაშლა", Location = new Point(320, 160), Size = new Size(80, 25) };

            // Close button - moved to center bottom
            btnClose = new Button { Text = "დახურვა", Location = new Point(450, 670), Size = new Size(100, 30) };

            // Add controls to subgroup panel
            pnlEditSubGroup.Controls.AddRange(new Control[] {
                lblSubGroupTitle, lblSubGroupName, txtSubGroupName, lblSubGroupPrice, txtSubGroupPrice, lblSubGroupMaxStudents, txtSubGroupMaxStudents,
                chkSubGroupStatus, btnSaveSubGroup, btnCancelSubGroup, btnDeleteSubGroup
            });

            // Add all controls to form
            this.Controls.AddRange(new Control[] {
                lblGroupsTitle, lblSubGroupsTitle, dgvGroups, dgvSubGroups, pnlEditGroup, pnlEditSubGroup, btnClose
            });

            // Initially disable edit panels
            pnlEditGroup.Enabled = false;
            pnlEditSubGroup.Enabled = false;
        }

        private void SetupEventHandlers()
        {
            // Group selection
            dgvGroups.SelectionChanged += DgvGroups_SelectionChanged;

            // SubGroup selection
            dgvSubGroups.SelectionChanged += DgvSubGroups_SelectionChanged;

            // Group editing
            btnSaveGroup.Click += BtnSaveGroup_Click;
            btnCancelGroup.Click += BtnCancelGroup_Click;
            btnBrowseContract.Click += BtnBrowseContract_Click;

            // SubGroup editing
            btnSaveSubGroup.Click += BtnSaveSubGroup_Click;
            btnCancelSubGroup.Click += BtnCancelSubGroup_Click;
            btnDeleteSubGroup.Click += BtnDeleteSubGroup_Click;

            // Close button
            btnClose.Click += BtnClose_Click;
        }

        private void LoadGroups()
        {
            var groups = _groupService.GetAllGroups();
            dgvGroups.DataSource = groups;

            // Set column headers
            if (dgvGroups.Columns.Count > 0)
            {
                dgvGroups.Columns["Id"].HeaderText = "ID";
                dgvGroups.Columns["Name"].HeaderText = "სახელი";
                dgvGroups.Columns["Price"].HeaderText = "ფასი";
                dgvGroups.Columns["Teacher"].HeaderText = "მასწავლებელი";
                dgvGroups.Columns["Status"].HeaderText = "სტატუსი";
                dgvGroups.Columns["MaxStudents"].HeaderText = "მაქს. მოსწავლეები";
                dgvGroups.Columns["ContractTemplatePath"].Visible = false;
            }
        }

        private void LoadSubGroups(int groupId)
        {
            var subGroups = _subGroupService.GetSubGroupsByGroupId(groupId);
            dgvSubGroups.DataSource = subGroups;

            // Set column headers
            if (dgvSubGroups.Columns.Count > 0)
            {
                dgvSubGroups.Columns["Id"].HeaderText = "ID";
                dgvSubGroups.Columns["Name"].HeaderText = "სახელი";
                dgvSubGroups.Columns["TuitionFee"].HeaderText = "ფასი";
                dgvSubGroups.Columns["Status"].HeaderText = "სტატუსი";
                dgvSubGroups.Columns["MaxStudents"].HeaderText = "მაქს. მოსწავლეები";
                dgvSubGroups.Columns["GroupId"].Visible = false;
            }
        }

        private void DgvGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGroups.CurrentRow != null)
            {
                _selectedGroup = dgvGroups.CurrentRow.DataBoundItem as Group;
                if (_selectedGroup != null)
                {
                    LoadSubGroups(_selectedGroup.Id);
                    EnableGroupEditing();
                    LoadGroupData();
                }
            }
            else
            {
                DisableGroupEditing();
                ClearGroupData();
            }
        }

        private void DgvSubGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSubGroups.CurrentRow != null)
            {
                _selectedSubGroup = dgvSubGroups.CurrentRow.DataBoundItem as SubGroup;
                if (_selectedSubGroup != null)
                {
                    EnableSubGroupEditing();
                    LoadSubGroupData();
                }
            }
            else
            {
                DisableSubGroupEditing();
                ClearSubGroupData();
            }
        }

        private void EnableGroupEditing()
        {
            bool canEditGroups = _userContext.HasPermission(Permission.CanEditGroups);
            pnlEditGroup.Enabled = canEditGroups;

            if (btnSaveGroup != null)
            {
                btnSaveGroup.Enabled = canEditGroups && _selectedGroup != null;
            }
        }

        private void DisableGroupEditing()
        {
            pnlEditGroup.Enabled = false;
        }

        private void EnableSubGroupEditing()
        {
            bool canEditSubGroups = _userContext.HasPermission(Permission.CanEditSubGroups);
            bool canDeleteSubGroups = _userContext.HasPermission(Permission.CanDeleteSubGroups);

            pnlEditSubGroup.Enabled = canEditSubGroups;

            if (btnSaveSubGroup != null)
            {
                btnSaveSubGroup.Enabled = canEditSubGroups && _selectedSubGroup != null;
            }

            if (btnDeleteSubGroup != null)
            {
                btnDeleteSubGroup.Enabled = canDeleteSubGroups && _selectedSubGroup != null;
            }
        }

        private void DisableSubGroupEditing()
        {
            pnlEditSubGroup.Enabled = false;
        }

        private void LoadGroupData()
        {
            if (_selectedGroup != null)
            {
                txtGroupName.Text = _selectedGroup.Name;
                txtGroupPrice.Text = _selectedGroup.Price.ToString();
                txtGroupTeacher.Text = _selectedGroup.Teacher;
                txtGroupContractPath.Text = _selectedGroup.ContractTemplatePath;
                chkGroupStatus.Checked = _selectedGroup.Status;
                txtGroupMaxStudents.Text = _selectedGroup.MaxStudents.ToString();
            }
        }

        private void LoadSubGroupData()
        {
            if (_selectedSubGroup != null)
            {
                txtSubGroupName.Text = _selectedSubGroup.Name;
                txtSubGroupPrice.Text = _selectedSubGroup.TuitionFee.ToString();
                chkSubGroupStatus.Checked = _selectedSubGroup.Status;
                txtSubGroupMaxStudents.Text = _selectedSubGroup.MaxStudents.ToString();
            }
        }

        private void ClearGroupData()
        {
            txtGroupName.Text = "";
            txtGroupPrice.Text = "";
            txtGroupTeacher.Text = "";
            txtGroupContractPath.Text = "";
            txtGroupMaxStudents.Text = "";
            chkGroupStatus.Checked = true;
        }

        private void ClearSubGroupData()
        {
            txtSubGroupName.Text = "";
            txtSubGroupPrice.Text = "";
            chkSubGroupStatus.Checked = true;
            txtSubGroupMaxStudents.Text = "";
        }

        private void BtnSaveGroup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedGroup == null) return;

            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ ჯგუფის სახელი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtGroupPrice.Text, out decimal price))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ სწორი ფასი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // MaxStudents-ის ვალიდაცია
                int newMaxStudents = int.TryParse(txtGroupMaxStudents.Text, out int maxStudents) ? maxStudents : 0;
                if (newMaxStudents > 0 && !_groupService.CanUpdateGroupMaxStudents(_selectedGroup.Id, newMaxStudents))
                {
                    var currentGroup = _groupService.GetGroupById(_selectedGroup.Id);
                    MessageBox.Show(
                        $"ჯგუფში არსებული მოსწავლეების რაოდენობა ({currentGroup?.StudentCount ?? 0}) აღემატება ახალ მაქსიმალურ რაოდენობას ({newMaxStudents}).\n\nგთხოვთ შეიყვანოთ მინიმუმ {currentGroup?.StudentCount ?? 0} ან მეტი.",
                        "ვალიდაციის შეცდომა",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _selectedGroup.Name = txtGroupName.Text.Trim();
                _selectedGroup.Price = price;
                _selectedGroup.Teacher = txtGroupTeacher.Text.Trim();
                _selectedGroup.ContractTemplatePath = txtGroupContractPath.Text.Trim();
                _selectedGroup.MaxStudents = newMaxStudents;
                _selectedGroup.Status = chkGroupStatus.Checked;

                bool success = _groupService.UpdateGroup(_selectedGroup);

                if (success)
                {
                    MessageBox.Show("ჯგუფი წარმატებით განახლდა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGroups();
                    _backupManager.DbChangedSinceLastBackup = true;
                }
                else
                {
                    MessageBox.Show("ჯგუფის განახლება ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveSubGroup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditSubGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedSubGroup == null) return;

            if (string.IsNullOrWhiteSpace(txtSubGroupName.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ ქვეჯგუფის სახელი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtSubGroupPrice.Text, out decimal price))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ სწორი ფასი.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // MaxStudents-ის ვალიდაცია
                int newMaxStudents = int.TryParse(txtSubGroupMaxStudents.Text, out int maxStudents) ? maxStudents : 0;
                if (newMaxStudents > 0 && !_groupService.CanUpdateSubGroupMaxStudents(_selectedSubGroup.Id, newMaxStudents))
                {
                    var currentSubGroup = _subGroupService.GetSubGroupById(_selectedSubGroup.Id);
                    MessageBox.Show(
                        $"ქვეჯგუფში არსებული მოსწავლეების რაოდენობა ({currentSubGroup?.StudentCount ?? 0}) აღემატება ახალ მაქსიმალურ რაოდენობას ({newMaxStudents}).\n\nგთხოვთ შეიყვანოთ მინიმუმ {currentSubGroup?.StudentCount ?? 0} ან მეტი.",
                        "ვალიდაციის შეცდომა",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _selectedSubGroup.Name = txtSubGroupName.Text.Trim();
                _selectedSubGroup.TuitionFee = price;
                _selectedSubGroup.MaxStudents = newMaxStudents;
                _selectedSubGroup.Status = chkSubGroupStatus.Checked;

                bool success = _subGroupService.UpdateSubGroup(_selectedSubGroup);

                if (success)
                {
                    MessageBox.Show("ქვეჯგუფი წარმატებით განახლდა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (_selectedGroup != null)
                    {
                        LoadSubGroups(_selectedGroup.Id);
                    }
                    _backupManager.DbChangedSinceLastBackup = true;
                }
                else
                {
                    MessageBox.Show("ქვეჯგუფის განახლება ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteSubGroup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanDeleteSubGroups))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_selectedSubGroup == null) return;

            var result = MessageBox.Show(
                $"დარწმუნებული ხართ რომ გსურთ ქვეჯგუფის წაშლა '{_selectedSubGroup.Name}'?",
                "დადასტურება",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success = _subGroupService.DeleteSubGroup(_selectedSubGroup.Id);

                    if (success)
                    {
                        MessageBox.Show("ქვეჯგუფი წარმატებით წაიშალა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (_selectedGroup != null)
                        {
                            LoadSubGroups(_selectedGroup.Id);
                        }
                        _backupManager.DbChangedSinceLastBackup = true;
                    }
                    else
                    {
                        MessageBox.Show("ქვეჯგუფის წაშლა ვერ მოხერხდა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancelGroup_Click(object sender, EventArgs e)
        {
            LoadGroupData(); // Reload original data
        }

        private void BtnCancelSubGroup_Click(object sender, EventArgs e)
        {
            LoadSubGroupData(); // Reload original data
        }

        private void BtnBrowseContract_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Word Documents (*.docx)|*.docx|All Files (*.*)|*.*";
                openFileDialog.Title = "აირჩიეთ კონტრაქტის შაბლონი";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtGroupContractPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

