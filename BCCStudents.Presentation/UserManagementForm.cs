using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Newtonsoft.Json;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class UserManagementForm : BaseForm
    {
        private readonly IUserService _userService;
        private readonly IUserContext _userContext;
        private DataGridView dgvUsers;
        private Button btnAddUser;
        private Button btnEditUser;
        private Button btnChangePassword;
        private Button btnDeleteUser;
        private Button btnRefresh;
        private int? _selectedUserId = null;

        public UserManagementForm(IUserService userService, IUserContext userContext)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            InitializeComponent();
            InitializeCustomComponents();
            FormTitleHelper.SetTitle(this, "მომხმარებლების მართვა");
            LoadUsers();
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Location = new Point(10, 50),
                Size = new Size(960, 450)
            };

            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;

            // Buttons Panel
            var panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            btnAddUser = new Button
            {
                Text = "ახალი მომხმარებელი",
                Location = new Point(10, 10),
                Size = new Size(150, 30),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnAddUser.Click += BtnAddUser_Click;

            btnEditUser = new Button
            {
                Text = "რედაქტირება",
                Location = new Point(170, 10),
                Size = new Size(120, 30),
                Enabled = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnEditUser.Click += BtnEditUser_Click;

            btnChangePassword = new Button
            {
                Text = "პაროლის შეცვლა",
                Location = new Point(300, 10),
                Size = new Size(120, 30),
                Enabled = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnChangePassword.Click += BtnChangePassword_Click;

            btnDeleteUser = new Button
            {
                Text = "წაშლა",
                Location = new Point(430, 10),
                Size = new Size(100, 30),
                Enabled = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnDeleteUser.Click += BtnDeleteUser_Click;

            btnRefresh = new Button
            {
                Text = "განახლება",
                Location = new Point(800, 10),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnRefresh.Click += BtnRefresh_Click;

            panelButtons.Controls.AddRange(new Control[] { btnAddUser, btnEditUser, btnChangePassword, btnDeleteUser, btnRefresh });

            var panelMain = new Panel { Dock = DockStyle.Fill };
            panelMain.Controls.Add(dgvUsers);

            this.Controls.Add(panelMain);
            this.Controls.Add(panelButtons);

            // ჩვეულებრივი მომხმარებლებისთვის — დამატება მხოლოდ CanAddUsers/Admin-ით
            bool canAddUsers = _userContext.IsAdmin || _userContext.HasPermission(Permission.CanAddUsers);
            btnAddUser.Visible = canAddUsers;
            if (!_userContext.IsAdmin && !_userContext.CanAccessUsers())
            {
                this.Text = "პროფილის მართვა";
                FormTitleHelper.SetTitle(this, "პროფილის მართვა");
            }
        }

        private void LoadUsers()
        {
            try
            {
                List<UserModel> users;

                // სრული სია: Admin ან მომხმარებლების CRUD უფლება; სხვაგვარად მხოლოდ საკუთარი პროფილი
                if (_userContext.IsAdmin || _userContext.CanAccessUsers())
                {
                    users = _userService.GetAllUsers();
                }
                else
                {
                    var currentUser = _userService.GetUserById(_userContext.UserId);
                    users = currentUser != null ? new List<UserModel> { currentUser } : new List<UserModel>();
                }

                dgvUsers.Columns.Clear();
                dgvUsers.Rows.Clear();

                // სვეტები
                dgvUsers.Columns.Add("Id", "ID");
                dgvUsers.Columns["Id"].Visible = false;
                dgvUsers.Columns.Add("Username", "მომხმარებლის სახელი");
                dgvUsers.Columns.Add("FullName", "სრული სახელი");
                dgvUsers.Columns.Add("Email", "ელ. ფოსტა");
                dgvUsers.Columns.Add("Role", "როლი");
                dgvUsers.Columns.Add("CreatedAt", "რეგისტრაციის თარიღი");
                dgvUsers.Columns.Add("LastLogin", "ბოლო შესვლა");

                foreach (var user in users)
                {
                    int rowIndex = dgvUsers.Rows.Add(
                        user.Id,
                        user.UserName,
                        user.FullName,
                        user.Email ?? "",
                        user.Role ?? "",
                        user.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "",
                        user.LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? ""
                    );
                }

                // თუ მხოლოდ ერთი მომხმარებელია (ჩვეულებრივი მომხმარებლის შემთხვევაში), ავტომატურად ავარჩიოთ
                if (users.Count == 1)
                {
                    dgvUsers.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა მომხმარებლების ჩატვირთვისას: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var row = dgvUsers.SelectedRows[0];
                _selectedUserId = (int)row.Cells["Id"].Value;

                bool canEditOthers = _userContext.IsAdmin || _userContext.HasPermission(Permission.CanEditUsers);
                bool canDeleteOthers = _userContext.IsAdmin || _userContext.HasPermission(Permission.CanDeleteUsers);
                bool isSelf = _selectedUserId.Value == _userContext.UserId;

                if (!canEditOthers && !isSelf)
                {
                    btnEditUser.Enabled = false;
                    btnChangePassword.Enabled = false;
                    btnDeleteUser.Enabled = false;
                }
                else
                {
                    btnEditUser.Enabled = canEditOthers || isSelf;
                    btnChangePassword.Enabled = canEditOthers || isSelf;
                    btnDeleteUser.Enabled = canDeleteOthers || isSelf;
                }
            }
            else
            {
                _selectedUserId = null;
                btnEditUser.Enabled = false;
                btnChangePassword.Enabled = false;
                btnDeleteUser.Enabled = false;
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            if (!_userContext.IsAdmin && !_userContext.HasPermission(Permission.CanAddUsers))
            {
                MessageBox.Show("თქვენ არ გაქვთ მომხმარებლის დამატების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new UserEditForm(_userService, null))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void BtnEditUser_Click(object sender, EventArgs e)
        {
            if (!_selectedUserId.HasValue) return;

            bool isSelf = _selectedUserId.Value == _userContext.UserId;
            bool canEditOthers = _userContext.IsAdmin || _userContext.HasPermission(Permission.CanEditUsers);
            if (!canEditOthers && !isSelf)
            {
                MessageBox.Show("თქვენ შეგიძლიათ მხოლოდ საკუთარი პროფილის რედაქტირება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _userService.GetUserById(_selectedUserId.Value);
            if (user == null)
            {
                MessageBox.Show("მომხმარებელი ვერ მოიძებნა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (canEditOthers)
            {
                using (var form = new UserEditForm(_userService, user))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _userContext.Refresh();
                        LoadUsers();
                    }
                }
            }
            else
            {
                using (var form = new UserProfileEditForm(_userService, user, _userContext))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _userContext.Refresh();
                        LoadUsers();
                    }
                }
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            if (!_selectedUserId.HasValue) return;

            bool isSelf = _selectedUserId.Value == _userContext.UserId;
            bool canEditOthers = _userContext.IsAdmin || _userContext.HasPermission(Permission.CanEditUsers);
            if (!canEditOthers && !isSelf)
            {
                MessageBox.Show("თქვენ შეგიძლიათ მხოლოდ საკუთარი პაროლის შეცვლა!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _userService.GetUserById(_selectedUserId.Value);
            if (user == null)
            {
                MessageBox.Show("მომხმარებელი ვერ მოიძებნა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var form = new UserPasswordChangeForm(_userService, _selectedUserId.Value))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (!_selectedUserId.HasValue) return;

            int targetUserId = _selectedUserId.Value;
            bool isSelf = targetUserId == _userContext.UserId;
            bool canDeleteOthers = _userContext.IsAdmin || _userContext.HasPermission(Permission.CanDeleteUsers);

            if (!canDeleteOthers && !isSelf)
            {
                MessageBox.Show("თქვენ შეგიძლიათ მხოლოდ საკუთარი ანგარიშის წაშლა!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = _userService.GetUserById(targetUserId);
            if (user == null)
            {
                MessageBox.Show("მომხმარებელი ვერ მოიძებნა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string confirmMessage = isSelf
                ? "ნამდვილად გსურთ საკუთარი ანგარიშის წაშლა? ამ მოქმედების შემდეგ სისტემიდან გამოხვალთ."
                : $"ნამდვილად გსურთ მომხმარებლის '{user.UserName}' წაშლა?";

            var confirm = MessageBox.Show(confirmMessage, "წაშლის დადასტურება", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                _userService.DeleteUser(targetUserId);

                if (isSelf)
                {
                    MessageBox.Show("თქვენი ანგარიში წაიშალა. აპლიკაცია გადაიტვირთება.", "წაშლილია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UserSession.Clear();
                    _userContext.Refresh();
                    System.Windows.Forms.Application.Restart();
                    return;
                }

                MessageBox.Show("მომხმარებელი წარმატებით წაიშალა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManagementForm));
            SuspendLayout();
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UserManagementForm";
            Text = "მომხმარებლების მართვა";
            ResumeLayout(false);
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    // User Edit Form - მომხმარებლის დამატება/რედაქტირება
    public partial class UserEditForm : Form
    {
        private readonly IUserService _userService;
        private readonly UserModel _existingUser;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private GroupBox grpPermissions;
        private Panel grpPanell;
        private Dictionary<CheckBox, string> _permissionCheckboxes;
        private Button btnSelectAllPermissions;
        private Button btnDeselectAllPermissions;
        private Button btnSave;
        private Button btnCancel;

        public UserEditForm(IUserService userService, UserModel existingUser = null)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _existingUser = existingUser;
            InitializeCustomComponents();
            FormTitleHelper.SetTitle(this, existingUser == null ? "ახალი მომხმარებელი" : "მომხმარებლის რედაქტირება");

            if (existingUser != null)
            {
                LoadUserData(existingUser);
            }
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;

            // Username
            var lblUsername = new Label { Text = "მომხმარებლის სახელი:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            txtUsername = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(280, 25), Enabled = _existingUser == null };
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            yPos += 40;

            // Full Name
            var lblFullName = new Label { Text = "სრული სახელი:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            txtFullName = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(280, 25) };
            this.Controls.Add(lblFullName);
            this.Controls.Add(txtFullName);
            yPos += 40;

            // Email
            var lblEmail = new Label { Text = "ელ. ფოსტა:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            txtEmail = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(280, 25) };
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            yPos += 40;

            // Role
            var lblRole = new Label { Text = "როლი:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            cmbRole = new ComboBox { Location = new Point(180, yPos - 3), Size = new Size(280, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new[] { "Administrator", "User" });
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            yPos += 50;

            // Permissions GroupBox
            grpPermissions = new GroupBox
            {
                Text = "უფლებები",
                Location = new Point(20, yPos),
                Size = new Size(440, 300),
                AutoScrollOffset = new Point(0, 0)
            };
            var panelPermissionActions = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35
            };

            btnSelectAllPermissions = new Button
            {
                Text = "ყველას მონიშვნა",
                Location = new Point(15, 5),
                Size = new Size(130, 25)
            };
            btnSelectAllPermissions.Click += (_, _) => SetAllPermissionsChecked(true);

            btnDeselectAllPermissions = new Button
            {
                Text = "მონიშვნის მოხსნა",
                Location = new Point(155, 5),
                Size = new Size(130, 25)
            };
            btnDeselectAllPermissions.Click += (_, _) => SetAllPermissionsChecked(false);

            panelPermissionActions.Controls.Add(btnSelectAllPermissions);
            panelPermissionActions.Controls.Add(btnDeselectAllPermissions);

            grpPanell = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            _permissionCheckboxes = new Dictionary<CheckBox, string>();
            var permissions = Permission.GetAssignablePermissions();
            int checkY = 10;

            foreach (var permission in permissions)
            {
                var chk = new CheckBox
                {
                    Text = GetPermissionDisplayName(permission),
                    Location = new Point(15, checkY),
                    Size = new Size(400, 20)
                };
                grpPanell.Controls.Add(chk);

                _permissionCheckboxes[chk] = permission;
                checkY += 30;
            }
            grpPermissions.Controls.Add(grpPanell);
            grpPermissions.Controls.Add(panelPermissionActions);
            this.Controls.Add(grpPermissions);
            yPos += 320;

            // Buttons
            btnSave = new Button { Text = "შენახვა", Location = new Point(280, yPos), Size = new Size(85, 30), DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "გაუქმება", Location = new Point(370, yPos), Size = new Size(85, 30), DialogResult = DialogResult.Cancel };

            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void SetAllPermissionsChecked(bool isChecked)
        {
            foreach (var checkbox in _permissionCheckboxes.Keys)
            {
                checkbox.Checked = isChecked;
            }
        }

        private string GetPermissionDisplayName(string permission)
        {
            var names = new Dictionary<string, string>
            {
                { Permission.CanImport, "იმპორტი" },
                { Permission.CanAddStudents, "მოსწავლის დამატება" },
                { Permission.CanEditStudents, "მოსწავლის რედაქტირება" },
                { Permission.CanDeleteStudents, "მოსწავლის წაშლა" },
                { Permission.CanAddGroups, "ჯგუფის დამატება" },
                { Permission.CanEditGroups, "ჯგუფის რედაქტირება" },
                { Permission.CanDeleteGroups, "ჯგუფის წაშლა" },
                { Permission.CanAddSubGroups, "ქვეჯგუფის დამატება" },
                { Permission.CanEditSubGroups, "ქვეჯგუფის რედაქტირება" },
                { Permission.CanDeleteSubGroups, "ქვეჯგუფის წაშლა" },
                { Permission.CanAddPayments, "გადახდის დამატება" },
                { Permission.CanEditPayments, "გადახდის რედაქტირება" },
                { Permission.CanDeletePayments, "გადახდის წაშლა" },
                { Permission.CanAddUsers, "მომხმარებლის დამატება" },
                { Permission.CanEditUsers, "მომხმარებლის რედაქტირება" },
                { Permission.CanDeleteUsers, "მომხმარებლის წაშლა" },
                { Permission.CanEditSettings, "პარამეტრების რედაქტირება" },
                { Permission.CanExportData, "მონაცემების ექსპორტი" },
                { Permission.CanViewReports, "ანგარიშების ნახვა" },
                { Permission.CanViewSystemLogs, "სისტემური ლოგების ნახვა" }
            };
            return names.ContainsKey(permission) ? names[permission] : permission;
        }

        private void LoadUserData(UserModel user)
        {
            txtUsername.Text = user.UserName;
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email ?? "";
            cmbRole.SelectedItem = user.Role ?? "User";

            // Load permissions
            Dictionary<string, bool> permissions = new Dictionary<string, bool>();
            if (!string.IsNullOrWhiteSpace(user.Permissions))
            {
                try
                {
                    permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(user.Permissions) ?? new Dictionary<string, bool>();
                }
                catch { }
            }

            ExpandManagePermissionsIntoCrud(permissions);

            foreach (var kvp in _permissionCheckboxes)
            {
                kvp.Key.Checked = permissions.ContainsKey(kvp.Value) && permissions[kvp.Value];
            }
        }

        /// <summary>
        /// ძველი CanManage* JSON → UI-ში შესაბამისი Add/Edit/Delete ჩექბოქსები.
        /// </summary>
        private static void ExpandManagePermissionsIntoCrud(Dictionary<string, bool> permissions)
        {
            if (permissions == null) return;

            void Expand(string manageKey, params string[] crudKeys)
            {
                if (permissions.TryGetValue(manageKey, out var enabled) && enabled)
                {
                    foreach (var key in crudKeys)
                        permissions[key] = true;
                }
            }

            Expand(Permission.CanManageStudents,
                Permission.CanAddStudents, Permission.CanEditStudents, Permission.CanDeleteStudents);
            Expand(Permission.CanManageGroups,
                Permission.CanAddGroups, Permission.CanEditGroups, Permission.CanDeleteGroups,
                Permission.CanAddSubGroups, Permission.CanEditSubGroups, Permission.CanDeleteSubGroups);
            Expand(Permission.CanManagePayments,
                Permission.CanAddPayments, Permission.CanEditPayments, Permission.CanDeletePayments);
            Expand(Permission.CanManageUsers,
                Permission.CanAddUsers, Permission.CanEditUsers, Permission.CanDeleteUsers);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ მომხმარებლის სახელი!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ სრული სახელი!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("გთხოვთ აირჩიოთ როლი!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Build permissions JSON
                var permissions = new Dictionary<string, bool>();
                foreach (var kvp in _permissionCheckboxes)
                {
                    permissions[kvp.Value] = kvp.Key.Checked;
                }
                string permissionsJson = JsonConvert.SerializeObject(permissions);

                if (_existingUser == null)
                {
                    // New user - prompt for password
                    using (var passwordForm = new UserPasswordInputForm("შეიყვანეთ პაროლი ახალი მომხმარებლისთვის:"))
                    {
                        if (passwordForm.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(passwordForm.Password))
                        {
                            MessageBox.Show("პაროლი აუცილებელია ახალი მომხმარებლისთვის!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.DialogResult = DialogResult.None;
                            return;
                        }

                        _userService.RegisterUser(
                            txtUsername.Text.Trim(),
                            txtFullName.Text.Trim(),
                            txtEmail.Text.Trim(),
                            passwordForm.Password,
                            cmbRole.SelectedItem.ToString(),
                            permissionsJson
                        );
                    }
                }
                else
                {
                    // Update existing user
                    _userService.UpdateUser(
                        _existingUser.Id,
                        txtFullName.Text.Trim(),
                        txtEmail.Text.Trim(),
                        cmbRole.SelectedItem.ToString(),
                        permissionsJson
                    );
                }

                MessageBox.Show("მონაცემები წარმატებით შენახულია!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    // Password Change Form
    public partial class UserPasswordChangeForm : Form
    {
        private readonly IUserService _userService;
        private readonly int _userId;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;

        public UserPasswordChangeForm(IUserService userService, int userId)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userId = userId;
            InitializeCustomComponents();
            FormTitleHelper.SetTitle(this, "პაროლის შეცვლა");
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;

            var lblNewPassword = new Label { Text = "ახალი პაროლი:", Location = new Point(20, yPos), Size = new Size(120, 20) };
            txtNewPassword = new TextBox { Location = new Point(150, yPos - 3), Size = new Size(220, 25), UseSystemPasswordChar = true };
            this.Controls.Add(lblNewPassword);
            this.Controls.Add(txtNewPassword);
            yPos += 40;

            var lblConfirmPassword = new Label { Text = "დადასტურება:", Location = new Point(20, yPos), Size = new Size(120, 20) };
            txtConfirmPassword = new TextBox { Location = new Point(150, yPos - 3), Size = new Size(220, 25), UseSystemPasswordChar = true };
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(txtConfirmPassword);
            yPos += 50;

            var btnSave = new Button { Text = "შენახვა", Location = new Point(200, yPos), Size = new Size(85, 30) };
            var btnCancel = new Button { Text = "გაუქმება", Location = new Point(290, yPos), Size = new Size(85, 30), DialogResult = DialogResult.Cancel };

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                {
                    MessageBox.Show("გთხოვთ შეიყვანოთ პაროლი!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("პაროლები არ ემთხვევა!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    _userService.UpdateUserPassword(_userId, txtNewPassword.Text);
                    MessageBox.Show("პაროლი წარმატებით შეიცვალა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    // User Profile Edit Form - გამარტივებული ფორმა ჩვეულებრივი მომხმარებლებისთვის (მხოლოდ სახელი და ელფოსტა)
    public partial class UserProfileEditForm : Form
    {
        private readonly IUserService _userService;
        private readonly UserModel _existingUser;
        private readonly IUserContext _userContext;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private Button btnSave;
        private Button btnCancel;

        public UserProfileEditForm(IUserService userService, UserModel existingUser, IUserContext userContext = null)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _existingUser = existingUser ?? throw new ArgumentNullException(nameof(existingUser));
            _userContext = userContext;
            InitializeCustomComponents();
            FormTitleHelper.SetTitle(this, "პროფილის რედაქტირება");
            LoadUserData(existingUser);
        }

        private void InitializeCustomComponents()
        {
            this.Size = new Size(450, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;

            // Username (read-only)
            var lblUsername = new Label { Text = "მომხმარებლის სახელი:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            txtUsername = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(230, 25), Enabled = false, ReadOnly = true };
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            yPos += 40;

            // Full Name
            var lblFullName = new Label { Text = "სრული სახელი:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            txtFullName = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(230, 25) };
            this.Controls.Add(lblFullName);
            this.Controls.Add(txtFullName);
            yPos += 40;

            // Email
            var lblEmail = new Label { Text = "ელ. ფოსტა:", Location = new Point(20, yPos), Size = new Size(150, 20) };
            txtEmail = new TextBox { Location = new Point(180, yPos - 3), Size = new Size(230, 25) };
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            yPos += 50;

            // Buttons
            btnSave = new Button { Text = "შენახვა", Location = new Point(230, yPos), Size = new Size(85, 30), DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "გაუქმება", Location = new Point(320, yPos), Size = new Size(85, 30), DialogResult = DialogResult.Cancel };

            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadUserData(UserModel user)
        {
            txtUsername.Text = user.UserName;
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email ?? "";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("გთხოვთ შეიყვანოთ სრული სახელი!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ვაახლებთ მხოლოდ სახელსა და ელფოსტას, როლსა და უფლებებს ვტოვებთ უცვლელად
                _userService.UpdateUser(
                    _existingUser.Id,
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    _existingUser.Role ?? "User", // ვტოვებთ ძველ როლს
                    _existingUser.Permissions ?? "{}" // ვტოვებთ ძველ უფლებებს
                );

                MessageBox.Show("მონაცემები წარმატებით შენახულია!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    // Password Input Form
    public partial class UserPasswordInputForm : Form
    {
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        public string Password => txtPassword.Text;

        public UserPasswordInputForm(string prompt)
        {
            InitializeCustomComponents(prompt);
        }

        private void InitializeCustomComponents(string prompt)
        {
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;

            var lblPrompt = new Label { Text = prompt, Location = new Point(20, yPos), Size = new Size(350, 40), AutoSize = true };
            this.Controls.Add(lblPrompt);
            yPos += 50;

            var lblPassword = new Label { Text = "პაროლი:", Location = new Point(20, yPos), Size = new Size(120, 20) };
            txtPassword = new TextBox { Location = new Point(150, yPos - 3), Size = new Size(220, 25), UseSystemPasswordChar = true };
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            yPos += 40;

            var lblConfirm = new Label { Text = "დადასტურება:", Location = new Point(20, yPos), Size = new Size(120, 20) };
            txtConfirmPassword = new TextBox { Location = new Point(150, yPos - 3), Size = new Size(220, 25), UseSystemPasswordChar = true };
            this.Controls.Add(lblConfirm);
            this.Controls.Add(txtConfirmPassword);
            yPos += 50;

            var btnOk = new Button { Text = "OK", Location = new Point(200, yPos), Size = new Size(85, 30), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "გაუქმება", Location = new Point(290, yPos), Size = new Size(85, 30), DialogResult = DialogResult.Cancel };

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("გთხოვთ შეიყვანოთ პაროლი!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("პაროლები არ ემთხვევა!", "ვალიდაცია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }
            };

            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}
