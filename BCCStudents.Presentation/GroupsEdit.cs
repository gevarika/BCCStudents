using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCCStudents.Presentation
{
    public partial class GroupsEdit : Form
    {
        private readonly GroupService _groupService;
        private readonly SubGroupService _subGroupService;
        
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
        private CheckBox chkGroupStatus;
        private Button btnBrowseContract;
        private Button btnSaveGroup;
        private Button btnCancelGroup;
        
        // SubGroup editing controls
        private TextBox txtSubGroupName;
        private TextBox txtSubGroupPrice;
        private CheckBox chkSubGroupStatus;
        private Button btnSaveSubGroup;
        private Button btnCancelSubGroup;
        private Button btnDeleteSubGroup;
        
        // Close button
        private Button btnClose;
        
        // Current selection
        private Group _selectedGroup;
        private SubGroup _selectedSubGroup;
        
        public GroupsEdit(GroupService groupService, SubGroupService subGroupService)
        {
            InitializeComponent();
            _groupService = groupService;
            _subGroupService = subGroupService;
            
            // Set form properties
            try
            {
                this.Icon = Properties.Resources.AppIcon;
            }
            catch
            {
                // If icon not found in resources, try direct file
                try
                {
                    this.Icon = new Icon("logo-new-32x32.ico");
                }
                catch
                {
                    // If icon file not found, continue without icon
                }
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            
            // Group Edit Panel - moved to right side
            pnlEditGroup = new Panel
            {
                Location = new Point(510, 35),
                Size = new Size(470, 340),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // SubGroup Edit Panel - moved to right side
            pnlEditSubGroup = new Panel
            {
                Location = new Point(510, 405),
                Size = new Size(470, 250),
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
            txtGroupName = new TextBox { Location = new Point(100, 40), Size = new Size(350, 20) };
            
            var lblGroupPrice = new Label { Text = "ფასი:", Location = new Point(10, 70), Size = new Size(80, 20) };
            txtGroupPrice = new TextBox { Location = new Point(100, 70), Size = new Size(350, 20) };
            
            var lblGroupTeacher = new Label { Text = "მასწავლებელი:", Location = new Point(10, 100), Size = new Size(80, 20) };
            txtGroupTeacher = new TextBox { Location = new Point(100, 100), Size = new Size(350, 20) };
            
            var lblGroupContract = new Label { Text = "კონტრაქტი:", Location = new Point(10, 130), Size = new Size(80, 20) };
            txtGroupContractPath = new TextBox { Location = new Point(100, 130), Size = new Size(300, 20), ReadOnly = true };
            btnBrowseContract = new Button { Text = "...", Location = new Point(410, 130), Size = new Size(40, 20) };
            
            chkGroupStatus = new CheckBox { Text = "აქტიური", Location = new Point(100, 160), Size = new Size(100, 20), Checked = true };
            
            btnSaveGroup = new Button { Text = "შენახვა", Location = new Point(100, 200), Size = new Size(80, 25) };
            btnCancelGroup = new Button { Text = "გაუქმება", Location = new Point(190, 200), Size = new Size(80, 25) };
            
            // Add controls to group panel
            pnlEditGroup.Controls.AddRange(new Control[] {
                lblGroupTitle, lblGroupName, txtGroupName, lblGroupPrice, txtGroupPrice,
                lblGroupTeacher, txtGroupTeacher, lblGroupContract, txtGroupContractPath, btnBrowseContract,
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
            txtSubGroupName = new TextBox { Location = new Point(100, 40), Size = new Size(350, 20) };
            
            var lblSubGroupPrice = new Label { Text = "ფასი:", Location = new Point(10, 70), Size = new Size(80, 20) };
            txtSubGroupPrice = new TextBox { Location = new Point(100, 70), Size = new Size(350, 20) };
            
            chkSubGroupStatus = new CheckBox { Text = "აქტიური", Location = new Point(100, 100), Size = new Size(100, 20), Checked = true };
            
            btnSaveSubGroup = new Button { Text = "შენახვა", Location = new Point(50, 140), Size = new Size(80, 25) };
            btnCancelSubGroup = new Button { Text = "გაუქმება", Location = new Point(140, 140), Size = new Size(80, 25) };
            btnDeleteSubGroup = new Button { Text = "წაშლა", Location = new Point(230, 140), Size = new Size(80, 25) };
            
            // Close button - moved to center bottom
            btnClose = new Button { Text = "დახურვა", Location = new Point(450, 670), Size = new Size(100, 30) };
            
            // Add controls to subgroup panel
            pnlEditSubGroup.Controls.AddRange(new Control[] {
                lblSubGroupTitle, lblSubGroupName, txtSubGroupName, lblSubGroupPrice, txtSubGroupPrice,
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
            pnlEditGroup.Enabled = true;
        }
        
        private void DisableGroupEditing()
        {
            pnlEditGroup.Enabled = false;
        }
        
        private void EnableSubGroupEditing()
        {
            pnlEditSubGroup.Enabled = true;
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
            }
        }
        
        private void LoadSubGroupData()
        {
            if (_selectedSubGroup != null)
            {
                txtSubGroupName.Text = _selectedSubGroup.Name;
                txtSubGroupPrice.Text = _selectedSubGroup.TuitionFee.ToString();
                chkSubGroupStatus.Checked = _selectedSubGroup.Status;
            }
        }
        
        private void ClearGroupData()
        {
            txtGroupName.Text = "";
            txtGroupPrice.Text = "";
            txtGroupTeacher.Text = "";
            txtGroupContractPath.Text = "";
            chkGroupStatus.Checked = true;
        }
        
        private void ClearSubGroupData()
        {
            txtSubGroupName.Text = "";
            txtSubGroupPrice.Text = "";
            chkSubGroupStatus.Checked = true;
        }
        
        private void BtnSaveGroup_Click(object sender, EventArgs e)
        {
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
                _selectedGroup.Name = txtGroupName.Text.Trim();
                _selectedGroup.Price = price;
                _selectedGroup.Teacher = txtGroupTeacher.Text.Trim();
                _selectedGroup.ContractTemplatePath = txtGroupContractPath.Text.Trim();
                _selectedGroup.Status = chkGroupStatus.Checked;
                
                bool success = _groupService.UpdateGroup(_selectedGroup);
                
                if (success)
                {
                    MessageBox.Show("ჯგუფი წარმატებით განახლდა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGroups();
                    BackupManager.DbChangedSinceLastBackup = true;
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
                _selectedSubGroup.Name = txtSubGroupName.Text.Trim();
                _selectedSubGroup.TuitionFee = price;
                _selectedSubGroup.Status = chkSubGroupStatus.Checked;
                
                bool success = _subGroupService.UpdateSubGroup(_selectedSubGroup);
                
                if (success)
                {
                    MessageBox.Show("ქვეჯგუფი წარმატებით განახლდა!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (_selectedGroup != null)
                    {
                        LoadSubGroups(_selectedGroup.Id);
                    }
                    BackupManager.DbChangedSinceLastBackup = true;
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
                        BackupManager.DbChangedSinceLastBackup = true;
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

