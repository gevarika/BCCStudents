using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
//using BCCStudents.Infrastructure.DataBase;
using System.Data;
using System.Text;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class StudentsEditForm : BaseForm
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly ISubGroupService _subGroupService;
        private readonly IStudentGroupsService _studentGroupsService;
        private readonly IUserContext _userContext;
        public StudentsEditForm(IStudentService studentService, IGroupService groupService, ISubGroupService subGroupService, IStudentGroupsService studentGroupsService, IUserContext userContext)
        {
            InitializeComponent();
            _studentService = studentService;
            _groupService = groupService;
            _subGroupService = subGroupService;
            _studentGroupsService = studentGroupsService;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
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

            // CheckBox event handler for filtering students in multiple groups
            chkstudentsToGroups.CheckedChanged += ChkstudentsToGroups_CheckedChanged;

            // Context Menu event handlers
            სვეტებისმართვაToolStripMenuItem.Click += ManageColumns_Click;

            // სვეტების კონფიგურაციის ჩატვირთვა SettingsHelper-იდან
            LoadColumnSettings();

            // DataGridView სვეტების ზომის ცვლილების ივენთი - შენახვისთვის
            dgvStudents.ColumnWidthChanged += DgvStudents_ColumnWidthChanged;

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

        // სვეტების ხილულობის კონფიგურაცია (სვეტის სახელი -> Visible)
        // ვტვირთავთ შენახულ კონფიგურაციას SettingsHelper-დან, ან ვიყენებთ ნაგულისხმევ მნიშვნელობებს
        private Dictionary<string, bool> columnVisibility;

        // სვეტების ზომების კონფიგურაცია (სვეტის სახელი -> Width)
        private Dictionary<string, int> columnWidths;

        /// <summary>
        /// ვიყენებთ Control.Tag პატერნით Security Checks-ისთვის
        /// Tag = "Permission_CanDelete" ან "Permission_CanManageStudents" ა.შ.
        /// </summary>
        private void ApplySecurityChecks()
        {
            // btnDel / წაშლა — CanDeleteStudents (არა ძველი ზოგადი CanDelete)
            if (btndel != null)
            {
                btndel.Tag = $"Permission_{Permission.CanDeleteStudents}";
                btndel.Enabled = _userContext.HasPermission(Permission.CanDeleteStudents);
            }

            if (წაშლაToolStripMenuItem != null)
            {
                წაშლაToolStripMenuItem.Tag = $"Permission_{Permission.CanDeleteStudents}";
                წაშლაToolStripMenuItem.Enabled = _userContext.HasPermission(Permission.CanDeleteStudents);
            }

            // btnSaveChanges / აქტივაცია — CanEditStudents
            if (btnSaveChanges != null)
            {
                btnSaveChanges.Tag = $"Permission_{Permission.CanEditStudents}";
                btnSaveChanges.Enabled = _userContext.HasPermission(Permission.CanEditStudents);
            }

            if (btnStudentActivation != null)
            {
                btnStudentActivation.Tag = $"Permission_{Permission.CanEditStudents}";
                btnStudentActivation.Enabled = _userContext.HasPermission(Permission.CanEditStudents);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanDeleteStudents))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var studentsToDelete = selectedStudents.Where(s => s.Value).Select(s => s.Key).ToList();

            if (studentsToDelete.Count == 0)
            {
                MessageBox.Show("No students selected for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                    _studentGroupsService.UpdateStudentStatus(studentId, groupId, false);
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
                    originalStudentData.StudentGroupsList = _studentGroupsService.GetActiveByStudentId(originalStudentData.Id);
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
            // Security Checks - Control.Tag პატერნით
            ApplySecurityChecks();

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
            // გამოიძახე ApplyFiltersAsync რომელიც გაითვალისწინებს ყველა ფილტრს
            _ = ApplyFiltersAsync();
        }

        /// <summary>
        /// ფილტრების გამოყენება და სტუდენტების ჩატვირთვა (Async)
        /// გაითვალისწინებს: txtSearch, chkstudentsToGroups, cbGroups, radio buttons
        /// </summary>
        private async Task ApplyFiltersAsync()
        {
            try
            {
                // 1. Search text-ის მიღება
                string searchText = txtSearch.Text.Trim();

                // 2. cbGroups-ის selected value (ფილტრაციისთვის გამოიყენება მხოლოდ cbGroups, chlGroups გამოიყენება მოსწავლის რედაქტირებისთვის)
                int selectedGroupId = cbGroups.SelectedValue != null ? Convert.ToInt32(cbGroups.SelectedValue) : -1;

                // 3. chkstudentsToGroups checkbox-ის მდგომარეობა
                bool filterMultipleGroups = chkstudentsToGroups.Checked;

                List<Student> students;

                // 4. ჯგუფის მიხედვით სტუდენტების მიღება
                if (filterMultipleGroups)
                {
                    // თუ chkstudentsToGroups მონიშნულია, ვიღებთ რამდენიმე ჯგუფში არსებულ სტუდენტებს
                    students = _studentService.GetStudentsInMultipleGroups();
                }
                else if (selectedGroupId > 0)
                {
                    // თუ cbGroups-ში არჩეულია კონკრეტული ჯგუფი
                    students = _studentService.GetStudentsByGroupId(selectedGroupId);
                }
                else
                {
                    // თუ არცერთი ჯგუფი არაა არჩეული (selectedGroupId == 0 ან -1), ვიღებთ ყველა სტუდენტს
                    students = _studentService.GetAllStudentsWithGroups();
                }

                // 5. Search text-ის ფილტრაცია (თუ search text არის)
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    List<Student> searchResults = new List<Student>();
                    string searchField = null;

                    if (rbByName.Checked)
                        searchField = "FirstName";
                    else if (rbByLastName.Checked)
                        searchField = "LastName";
                    else if (rbByParent.Checked)
                        searchField = "ParentName";
                    else if (rbByAge.Checked)
                        searchField = "Age";
                    else if (rbByIdNumber.Checked)
                        searchField = "Id_Numb";
                    else if (rbByAddress.Checked)
                        searchField = "Address";
                    else if (rbByStCode.Checked)
                        searchField = "StudentCode";

                    if (!string.IsNullOrEmpty(searchField))
                    {
                        // Search-ის გაკეთება: თუ ჯგუფი არჩეულია, search-ს ვაკეთებთ ამ ჯგუფში, წინააღმდეგ შემთხვევაში ყველა ჯგუფში
                        if (selectedGroupId > 0 && !filterMultipleGroups)
                        {
                            searchResults = _studentService.SearchStudents(searchField, searchText, selectedGroupId);
                        }
                        else
                        {
                            searchResults = _studentService.SearchStudents(searchField, searchText);
                        }

                        // AND ლოგიკა: ვიღებთ მხოლოდ იმ სტუდენტებს, რომლებიც არის როგორც students-ში, ისე searchResults-ში
                        var studentIdsInResults = searchResults.Select(s => s.Id).ToHashSet();
                        students = students.Where(s => studentIdsInResults.Contains(s.Id)).ToList();
                    }
                }

                // 7. UI-ზე განახლება (UI thread-ზე)
                if (InvokeRequired)
                {
                    Invoke(new Action(() => RefreshDataGridView(students)));
                }
                else
                {
                    RefreshDataGridView(students);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა ფილტრაციისას: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            // Select checkbox column
            var selectCol = new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "Select", Width = 50 };
            selectCol.Visible = columnVisibility.GetValueOrDefault("Select", true);
            dgvStudents.Columns.Add(selectCol);

            // Id column (ყოველთვის ფარული, მაგრამ Dictionary-ში შეიძლება იყოს)
            var idCol = new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id" };
            idCol.Visible = columnVisibility.GetValueOrDefault("Id", false);
            dgvStudents.Columns.Add(idCol);

            // GroupId column (ყოველთვის ფარული, მაგრამ Dictionary-ში შეიძლება იყოს)
            var groupIdCol = new DataGridViewTextBoxColumn { Name = "GroupId", DataPropertyName = "GroupId" };
            groupIdCol.Visible = columnVisibility.GetValueOrDefault("GroupId", false);
            dgvStudents.Columns.Add(groupIdCol);

            // FirstName
            var firstNameCol = new DataGridViewTextBoxColumn { Name = "FirstName", DataPropertyName = "FirstName", HeaderText = "First Name", Width = 120 };
            firstNameCol.Visible = columnVisibility.GetValueOrDefault("FirstName", true);
            dgvStudents.Columns.Add(firstNameCol);

            // LastName
            var lastNameCol = new DataGridViewTextBoxColumn { Name = "LastName", DataPropertyName = "LastName", HeaderText = "Last Name", Width = 120 };
            lastNameCol.Visible = columnVisibility.GetValueOrDefault("LastName", true);
            dgvStudents.Columns.Add(lastNameCol);

            // GroupName
            var groupNameCol = new DataGridViewTextBoxColumn { Name = "GroupName", DataPropertyName = "GroupName", HeaderText = "Group", Width = 150 };
            groupNameCol.Visible = columnVisibility.GetValueOrDefault("GroupName", true);
            dgvStudents.Columns.Add(groupNameCol);

            // Age
            var ageCol = new DataGridViewTextBoxColumn { Name = "Age", DataPropertyName = "Age", HeaderText = "Age", Width = 60 };
            ageCol.Visible = columnVisibility.GetValueOrDefault("Age", false);
            dgvStudents.Columns.Add(ageCol);

            // ParentName
            var parentNameCol = new DataGridViewTextBoxColumn { Name = "ParentName", DataPropertyName = "ParentName", HeaderText = "Parent Name", Width = 150 };
            parentNameCol.Visible = columnVisibility.GetValueOrDefault("ParentName", false);
            dgvStudents.Columns.Add(parentNameCol);

            // PhoneNumber
            var phoneCol = new DataGridViewTextBoxColumn { Name = "PhoneNumber", DataPropertyName = "PhoneNumber", HeaderText = "Phone", Width = 120 };
            phoneCol.Visible = columnVisibility.GetValueOrDefault("PhoneNumber", false);
            dgvStudents.Columns.Add(phoneCol);

            // Id_Numb (Personal ID)
            var idNumbCol = new DataGridViewTextBoxColumn { Name = "Id_Numb", DataPropertyName = "Id_Numb", HeaderText = "Personal ID", Width = 120 };
            idNumbCol.Visible = columnVisibility.GetValueOrDefault("Id_Numb", false);
            dgvStudents.Columns.Add(idNumbCol);

            // Address
            var addressCol = new DataGridViewTextBoxColumn { Name = "Address", DataPropertyName = "Address", HeaderText = "Address", Width = 200 };
            addressCol.Visible = columnVisibility.GetValueOrDefault("Address", false);
            dgvStudents.Columns.Add(addressCol);

            // StudentCode
            var studentCodeCol = new DataGridViewTextBoxColumn { Name = "StudentCode", DataPropertyName = "StudentCode", HeaderText = "Student Code", Width = 120 };
            studentCodeCol.Visible = columnVisibility.GetValueOrDefault("StudentCode", false);
            dgvStudents.Columns.Add(studentCodeCol);

            // RegistrationDate
            var regDateCol = new DataGridViewTextBoxColumn { Name = "RegistrationDate", DataPropertyName = "RegistrationDate", HeaderText = "Registration Date", Width = 130 };
            regDateCol.Visible = columnVisibility.GetValueOrDefault("RegistrationDate", false);
            dgvStudents.Columns.Add(regDateCol);

            // DateOfPayment
            var paymentDateCol = new DataGridViewTextBoxColumn { Name = "DateOfPayment", DataPropertyName = "DateOfPayment", HeaderText = "Payment Date", Width = 130 };
            paymentDateCol.Visible = columnVisibility.GetValueOrDefault("DateOfPayment", false);
            dgvStudents.Columns.Add(paymentDateCol);

            // TuitionFee
            var tuitionFeeCol = new DataGridViewTextBoxColumn { Name = "TuitionFee", DataPropertyName = "TuitionFee", HeaderText = "Tuition Fee", Width = 100 };
            tuitionFeeCol.Visible = columnVisibility.GetValueOrDefault("TuitionFee", false);
            dgvStudents.Columns.Add(tuitionFeeCol);

            // Discount
            var discountCol = new DataGridViewTextBoxColumn { Name = "Discount", DataPropertyName = "Discount", HeaderText = "Discount", Width = 80 };
            discountCol.Visible = columnVisibility.GetValueOrDefault("Discount", false);
            dgvStudents.Columns.Add(discountCol);

            // PaymentStatus
            var paymentStatusCol = new DataGridViewTextBoxColumn { Name = "PaymentStatus", DataPropertyName = "PaymentStatus", HeaderText = "Payment Status", Width = 120 };
            paymentStatusCol.Visible = columnVisibility.GetValueOrDefault("PaymentStatus", false);
            dgvStudents.Columns.Add(paymentStatusCol);

            // Status
            var statusCol = new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "Status", Width = 80 };
            statusCol.Visible = columnVisibility.GetValueOrDefault("Status", false);
            dgvStudents.Columns.Add(statusCol);

            // Balance
            var balanceCol = new DataGridViewTextBoxColumn { Name = "Balance", DataPropertyName = "Balance", HeaderText = "Balance", Width = 100 };
            balanceCol.Visible = columnVisibility.GetValueOrDefault("Balance", false);
            dgvStudents.Columns.Add(balanceCol);

            // UpdatedAt
            var updatedAtCol = new DataGridViewTextBoxColumn { Name = "UpdatedAt", DataPropertyName = "UpdatedAt", HeaderText = "Updated At", Width = 130 };
            updatedAtCol.Visible = columnVisibility.GetValueOrDefault("UpdatedAt", false);
            dgvStudents.Columns.Add(updatedAtCol);

            // სვეტების ზომების განახლება, თუ შენახულია
            if (columnWidths != null && columnWidths.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvStudents.Columns)
                {
                    if (columnWidths.ContainsKey(col.Name))
                    {
                        col.Width = columnWidths[col.Name];
                    }
                }
            }

            // გავწმინდოთ წაშლისთვის მონიშნული მოსწავლეები რეფრეშისას
            // (გარდა იმ შემთხვევისა, როცა მომხმარებელმა უკვე მონიშნა)
            // selectedStudents.Clear(); // ეს გააკეთებს btnDel ან btnClear

            dgvStudents.Rows.Clear();

            // ვამოწმებთ, რომ Select სვეტი დამატებულია
            if (!dgvStudents.Columns.Contains("Select"))
            {
                MessageBox.Show("შეცდომა: Select სვეტი არ არის DataGridView-ში", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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

                // ყველა სვეტის მონაცემების დამატება - სვეტები უკვე დამატებულია, ახლა ვქმნით რიგს
                // CreateCells ქმნის უჯრებს (cells) DataGridView-ის ყველა სვეტისთვის
                // ეს მეთოდი ქმნის უჯრებს სვეტების ტიპის მიხედვით (CheckBoxColumn -> CheckBoxCell, TextBoxColumn -> TextBoxCell)
                var row = new DataGridViewRow();

                try
                {
                    row.CreateCells(dgvStudents);

                    // შევამოწმოთ, რომ CreateCells-მა შექმნა ყველა უჯრა
                    if (row.Cells.Count != dgvStudents.Columns.Count)
                    {
                        throw new InvalidOperationException($"CreateCells შექმნა {row.Cells.Count} უჯრა, მაგრამ საჭიროა {dgvStudents.Columns.Count} სვეტი");
                    }
                }
                catch (Exception ex)
                {
                    // თუ CreateCells არ მუშაობს, ხელით ვქმნით უჯრებს
                    //row = new DataGridViewRow();
                    foreach (DataGridViewColumn col in dgvStudents.Columns)
                    {
                        DataGridViewCell cell;
                        if (col is DataGridViewCheckBoxColumn)
                        {
                            cell = new DataGridViewCheckBoxCell();
                        }
                        else
                        {
                            cell = new DataGridViewTextBoxCell();
                        }
                        row.Cells.Add(cell);
                    }
                }
                int rowIndex = dgvStudents.Rows.Add(); // ჯერ ვამატებთ ცარიელ რიგს
                row = dgvStudents.Rows[rowIndex];   // ვიღებთ უკვე დამატებულ რიგს

                row.Cells["Select"].Value = false;
                row.Cells["Id"].Value = student.Id;
                row.Cells["GroupId"].Value = resolvedGroupId;
                row.Cells["FirstName"].Value = student.FirstName ?? "";
                row.Cells["LastName"].Value = student.LastName ?? "";
                row.Cells["GroupName"].Value = student.GroupName ?? "";
                row.Cells["Age"].Value = student.Age;
                row.Cells["ParentName"].Value = student.ParentName ?? "";
                row.Cells["PhoneNumber"].Value = student.PhoneNumber ?? "";
                row.Cells["Id_Numb"].Value = student.Id_Numb;
                row.Cells["Address"].Value = student.Address ?? "";
                row.Cells["StudentCode"].Value = student.StudentCode ?? "";
                row.Cells["RegistrationDate"].Value = student.RegistrationDate != default(DateTime) ? student.RegistrationDate.ToString("yyyy-MM-dd") : "";
                row.Cells["DateOfPayment"].Value = student.DateOfPayment.HasValue ? student.DateOfPayment.Value.ToString("yyyy-MM-dd") : "";
                row.Cells["TuitionFee"].Value = student.TuitionFee;
                row.Cells["Discount"].Value = student.Discount;
                row.Cells["PaymentStatus"].Value = student.PaymentStatus ?? "";
                row.Cells["Status"].Value = student.Status ? "Active" : "Inactive";
                row.Cells["Balance"].Value = student.Balance;
                row.Cells["UpdatedAt"].Value = student.UpdatedAt != default(DateTime) ? student.UpdatedAt.ToString("yyyy-MM-dd HH:mm") : "";

                // რიგი უკვე დამატებულია dgvStudents.Rows.Add()-ით ხაზ 524-ზე, ამიტომ აღარ გვჭირდება მისი დამატება
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
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditStudents))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                _studentService.UpdateStudentFields(studentId, changedFields);
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
            var group = _groupService.GetGroupById(grpId);
            if (group == null) return;

            _studentService.AddStudentToGroup(studId, grpId, true);

            var subGroups = _subGroupService.GetSubGroupsByGroupId(grpId);
            if (subGroups != null && subGroups.Any())
            {
                var firstSubGroup = subGroups.First();
                _studentService.AddStudentToSubGroup(studId, grpId, firstSubGroup.Id, "Pending", null, firstSubGroup.TuitionFee, 0, true);
            }
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
            _studentService.RemoveStudentFromGroup(studId, grpId);
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
                        _studentService.AddStudentToSubGroup(studentId, rowGroupId, newSubGroupId, "Pending", null, subGroup.TuitionFee, 0, true);
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
                _studentGroupsService.UpdateStudentStatus(studentId, groupId, newStatus);
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
                var refreshedStudent = _studentService.GetStudentDetailsById(studentId, groupId);
                if (refreshedStudent != null)
                {
                    refreshedStudent.StudentGroupsList = _studentGroupsService.GetActiveByStudentId(studentId);
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
                /*if (dgvStudents.CurrentCell != null)
                {
                    dgvStudents_CellDoubleClick(null, new DataGridViewCellEventArgs(dgvStudents.CurrentCell.ColumnIndex, dgvStudents.CurrentCell.RowIndex));
                }*/
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
            var student = _studentService.GetStudentDetailsById(studentId, groupId);
            if (student == null)
            {
                MessageBox.Show("მოსწავლის მონაცემები ვერ მოიძებნა.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearStudentForm();
                return;
            }

            // 5. StudentGroups და SubGroups ჩატვირთვა
            student.StudentGroupsList = _studentGroupsService.GetActiveByStudentId(studentId);
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
            // გამოიძახე ApplyFiltersAsync რომელიც გაითვალისწინებს ყველა ფილტრს
            _ = ApplyFiltersAsync();
        }

        /// <summary>
        /// CheckBox-ის event handler - განაახლებს სტუდენტების სიას
        /// </summary>
        private void ChkstudentsToGroups_CheckedChanged(object sender, EventArgs e)
        {
            // გამოიძახე ApplyFiltersAsync რომელიც გაითვალისწინებს ყველა ფილტრს
            _ = ApplyFiltersAsync();
        }

        /// <summary>
        /// ფილტრაცია: დაბრუნება მხოლოდ ის სტუდენტების, რომლებიც 1-ზე მეტ ჯგუფში არიან
        /// </summary>
        private List<Student> FilterStudentsInMultipleGroups(List<Student> students)
        {
            if (students == null || !students.Any())
                return new List<Student>();

            var filteredStudents = new List<Student>();
            var checkedStudentIds = new HashSet<int>();

            foreach (var student in students)
            {
                // თუ უკვე შევამოწმეთ ეს სტუდენტი, გამოვტოვოთ (დუბლიკატების თავიდან ასაცილებლად)
                if (checkedStudentIds.Contains(student.Id))
                    continue;

                // მივიღოთ სტუდენტის ჯგუფების სია
                var studentGroups = _studentGroupsService.GetActiveByStudentId(student.Id);

                // თუ სტუდენტი 1-ზე მეტ ჯგუფშია, დავამატოთ შედეგებში
                if (studentGroups != null && studentGroups.Count > 1)
                {
                    checkedStudentIds.Add(student.Id);

                    // დავამატოთ სტუდენტი ყველა ჯგუფისთვის (თითოეული ჯგუფისთვის ცალ-ცალკე row)
                    foreach (var sg in studentGroups)
                    {
                        var studentCopy = new Student
                        {
                            Id = student.Id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Age = student.Age,
                            ParentName = student.ParentName,
                            PhoneNumber = student.PhoneNumber,
                            Id_Numb = student.Id_Numb,
                            Address = student.Address,
                            RegistrationDate = student.RegistrationDate,
                            StudentCode = student.StudentCode,
                            GroupId = sg.GroupId,
                            GroupName = _groupService.GetGroupName(sg.GroupId),
                            Status = student.Status,
                            Balance = student.Balance
                        };
                        filteredStudents.Add(studentCopy);
                    }
                }
            }

            return filteredStudents;
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

            // chlGroups გამოიყენება მხოლოდ მოსწავლის რედაქტირებისთვის, არა ფილტრაციისთვის
            // ამიტომ აქ არ ვიძახებთ ApplyFiltersAsync-ს
        }

        private void chkBoxStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (isProgrammaticCheck || isLoadingData) return;
            statusChanged = true;
        }

        private void btnStudentActivation_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditStudents))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_studentService.UpdateStudentStatus(studentId, groupId, true))
            {
                SetFieldsReadOnly(false);
            }
        }

        /// <summary>
        /// ველების read-only მდგომარეობის დაყენება
        /// </summary>
        private void SetFieldsReadOnly(bool readOnly)
        {
            txtFirstName.Enabled = !readOnly;
            txtLastName.Enabled = !readOnly;
            txtParent.Enabled = !readOnly;
            txtAge.Enabled = !readOnly;
            txtPhone.Enabled = !readOnly;
            txtPersonalId.Enabled = !readOnly;
            txtAddress.Enabled = !readOnly;
            txtRegistrationDate.Enabled = !readOnly;
            txtPaymentDate.Enabled = !readOnly;
            txtStatus.Enabled = !readOnly;
            txtPaymentStatus.Enabled = !readOnly;
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Header-ზე დაკლიკება - იგნორირება
            if (e.RowIndex < 0) return;

            // 1. Select სვეტზე დაკლიკება (Checkbox)
            // აქ ჯობია ისევ CellContentClick-ის მსგავსი ლოგიკა, ან პირდაპირ შემოწმება
            if (e.ColumnIndex == dgvStudents.Columns["Select"]?.Index)
            {
                //HandleSelectCheckboxClick(e.RowIndex);
                // მნიშვნელოვანია: Checkbox-ის შემთხვევაში ხშირად გვჭირდება 
                // EndEdit(), რომ ცვლილება მომენტალურად აისახოს
                dgvStudents.EndEdit();
                return;
            }

            // 2. ნებისმიერ სხვა უჯრაზე დაკლიკება - მოსწავლის არჩევა
            LoadSelectedStudentData(e.RowIndex);
        }

        /// <summary>
        /// სვეტების კონფიგურაციის ჩატვირთვა SettingsHelper-იდან
        /// </summary>
        private void LoadColumnSettings()
        {
            string formName = this.Name;
            string dgvName = "dgvStudents"; // DataGridView-ის სახელი

            // სვეტების ხილულობის ჩატვირთვა
            columnVisibility = SettingsHelper.LoadColumnVisibility(
                formName,
                dgvName,
                GetDefaultColumnVisibility()
            );

            // სვეტების ზომების ჩატვირთვა
            columnWidths = SettingsHelper.LoadColumnWidths(
                formName,
                dgvName,
                new Dictionary<string, int>()
            );
        }

        /// <summary>
        /// ნაგულისხმევი სვეტების ხილულობის კონფიგურაცია
        /// </summary>
        private Dictionary<string, bool> GetDefaultColumnVisibility()
        {
            return new Dictionary<string, bool>
            {
                { "Select", true },        // Checkbox - ყოველთვის ჩანს
                { "Id", false },           // ID - ყოველთვის ფარული (არ ჩანს)
                { "GroupId", false },      // GroupId - ყოველთვის ფარული (არ ჩანს)
                { "FirstName", true },     // First Name - ნაგულისხმევად ჩანს
                { "LastName", true },      // Last Name - ნაგულისხმევად ჩანს
                { "GroupName", true },     // Group - ნაგულისხმევად ჩანს
                { "Age", false },          // ასაკი
                { "ParentName", false },   // მშობლის სახელი
                { "PhoneNumber", false },  // ტელეფონი
                { "Id_Numb", false },      // პირადი ნომერი
                { "Address", false },      // მისამართი
                { "StudentCode", false },  // სტუდენტის კოდი
                { "RegistrationDate", false }, // რეგისტრაციის თარიღი
                { "DateOfPayment", false },    // გადახდის თარიღი
                { "TuitionFee", false },       // გადასახადი
                { "Discount", false },         // ფასდაკლება
                { "PaymentStatus", false },    // გადახდის სტატუსი
                { "Status", false },           // სტატუსი
                { "Balance", false },          // ბალანსი
                { "UpdatedAt", false }         // განახლების თარიღი
            };
        }

        /// <summary>
        /// სვეტების კონფიგურაციის შენახვა SettingsHelper-ში
        /// </summary>
        private void SaveColumnSettings()
        {
            string formName = this.Name;
            string dgvName = "dgvStudents"; // DataGridView-ის სახელი

            // სვეტების ხილულობის შენახვა
            SettingsHelper.SaveColumnVisibility(formName, dgvName, columnVisibility);

            // სვეტების ზომების შენახვა
            if (columnWidths != null && columnWidths.Count > 0)
            {
                SettingsHelper.SaveColumnWidths(formName, dgvName, columnWidths);
            }
        }

        /// <summary>
        /// DataGridView სვეტების ზომის ცვლილების დამუშავება
        /// </summary>
        private void DgvStudents_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (columnWidths == null)
            {
                columnWidths = new Dictionary<string, int>();
            }

            columnWidths[e.Column.Name] = e.Column.Width;

            // შენახვა (debounce - შეიძლება დავამატოთ Timer თუ ხშირად იცვლება)
            SaveColumnSettings();
        }

        /// <summary>
        /// სვეტების მართვის დიალოგის გახსნა
        /// </summary>
        private void ManageColumns_Click(object sender, EventArgs e)
        {
            using (var dialog = new ColumnManagementDialog(columnVisibility))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.ResultColumnVisibility != null)
                {
                    // columnVisibility-ის განახლება
                    columnVisibility = dialog.ResultColumnVisibility;

                    // კონფიგურაციის შენახვა
                    SaveColumnSettings();

                    // DataGridView-ის განახლება - ApplyFiltersAsync-ის გამოძახებით, რათა შენარჩუნდეს მიმდინარე ფილტრები
                    _ = ApplyFiltersAsync();
                }
            }
        }
    }
}

