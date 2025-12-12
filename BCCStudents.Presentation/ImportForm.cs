using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BCCStudents.Domain.Entities;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Interfaces;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.IO;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// სტუდენტების იმპორტის ფორმა Excel ფაილებიდან
    /// </summary>
    public partial class ImportForm : Form
    {
        #region ==================== Fields ====================

        private readonly IImportService _importService;
        private readonly IGroupRepository _groupRepository;
        private readonly GroupService _groupService;
        private readonly SubGroupService _subGroupService;
        private string _filePath;

        // UI კომპონენტები - ყველა უკვე არის ImportForm.Designer.cs-ში
        // lblTitle, pnlSheetMapping, pnlOptions, chkEnableMapping, chkIsActive, 
        // tabWorksheets, pnlStatus, statusLabel, progressBar, logTextBox, pnlBottom, btnImport
        
        // დამატებითი კომპონენტები, რომლებიც შეიქმნება დინამიურად
        private TableLayoutPanel tblSheetMapping;

        // მონაცემები
        private Dictionary<string, ComboBox> _sheetGroupComboBoxes = new Dictionary<string, ComboBox>();
        private Dictionary<string, int> _groupNameToIdMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, DataGridView> _worksheetGrids = new Dictionary<string, DataGridView>();
        private Dictionary<string, Dictionary<string, ComboBox>> _worksheetComboBoxes =
            new Dictionary<string, Dictionary<string, ComboBox>>();
        private DataGridView _columnMappingGrid; // Column Mapping Grid (Excel სვეტები = მწკრივები, სტუდენტის ველები = სვეტები)

        // სტანდარტული ველები
        private readonly List<string> _studentFields = new List<string>
        {
            "StudentCode", "FirstName", "LastName", "Age", "ParentName", "PhoneNumber", "Id_Numb", "Address",
            "TuitionFee", "Discount", "SubGroup"
        };

        private readonly List<string> _studentFieldNamesGeorgian = new List<string>
        {
            "სტუდენტის კოდი", "სახელი", "გვარი", "ასაკი", "მშობლის სახელი", "ტელეფონი", "პირადი ნომერი",
            "მისამართი", "საფასური", "ფასდაკლება", "ქვეჯგუფი"
        };

        #endregion

        #region ==================== Constructor ====================

        public ImportForm(
            IImportService importService,
            IGroupRepository groupRepository,
            GroupService groupService,
            SubGroupService subGroupService)
        {
            _importService = importService;
            _groupRepository = groupRepository;
            _groupService = groupService;
            _subGroupService = subGroupService;

            InitializeComponent();
            SetupUI();
        }

        #endregion

        #region ==================== Public Methods ====================

        /// <summary>
        /// იმპორტის ინიციალიზაცია ფაილის გზით
        /// </summary>
        public void InitializeImport(string filePath)
        {
            _filePath = filePath;
            LoadGroups();
            CheckAndCreateMissingGroups();
            LoadSheetMappings();
        }

        #endregion

        #region ==================== UI Setup ====================

        /// <summary>
        /// UI კომპონენტების დაყენება
        /// ყველა კომპონენტი უკვე არის Designer.cs-ში და დამატებულია ფორმაზე
        /// აქ მხოლოდ ტექსტებს, event handlers-ს და დამატებით კონფიგურაციას ვაკეთებთ
        /// </summary>
        private void SetupUI()
        {
            // ყველა კომპონენტი უკვე არის Designer.cs-ში და დამატებულია ფორმაზე
            // აქ მხოლოდ კონფიგურაციას ვაკეთებთ
            
            lblTitle.Text = "Excel ფაილიდან სტუდენტების იმპორტი";
            statusLabel.Text = "მზადაა";
            syncStatusLabel.Text = "სინქრონიზაცია: მზადაა";
            progressBar.Visible = false;
            //logTextBox.Visible = false;
            
            // Event handlers
            btnImport.Click += ImportButton_Click;
            
            // chkEnableMapping და chkIsActive უკვე დამატებულია Designer.cs-ში
            
            // გამყოფი ხაზი pnlOptions-სა და tabWorksheets-ს შორის
            var separator = new Label
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = Color.Gray
            };
            this.Controls.Add(separator);
            this.Controls.SetChildIndex(separator, this.Controls.IndexOf(tabWorksheets) - 1);
        }

        #endregion

        #region ==================== Data Loading ====================

        /// <summary>
        /// ჯგუფების ჩატვირთვა
        /// </summary>
        private void LoadGroups()
        {
            var allGroups = _groupRepository.GetAllGroups();
            _groupNameToIdMap.Clear();

            if (allGroups == null) return;

            var deduped = allGroups
                .Where(g => !string.IsNullOrWhiteSpace(g.Name))
                .GroupBy(g => g.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(grp => grp.OrderByDescending(g => g.Status).ThenByDescending(g => g.Id).First())
                .ToList();

            foreach (var group in deduped)
            {
                _groupNameToIdMap[group.Name] = group.Id;
            }
        }

        /// <summary>
        /// Excel-ის შიტების ამოღება და ჯგუფების შემოწმება
        /// </summary>
        private List<string> GetExcelSheets()
        {
            if (string.IsNullOrEmpty(_filePath) || !File.Exists(_filePath))
                return new List<string>();

            try
            {
                using (var workbook = new XLWorkbook(_filePath))
                {
                    return workbook.Worksheets.Select(w => w.Name).ToList();
                }
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// აკლია ჯგუფების შემოწმება
        /// </summary>
        private List<string> GetMissingGroups()
        {
            var sheets = GetExcelSheets();
            var missing = new List<string>();

            foreach (var sheet in sheets)
            {
                if (!_groupNameToIdMap.ContainsKey(sheet))
                {
                    missing.Add(sheet);
                }
            }

            return missing;
        }

        /// <summary>
        /// ჯგუფების შემოწმება და შექმნის შეთავაზება
        /// </summary>
        private void CheckAndCreateMissingGroups()
        {
            var missingGroups = GetMissingGroups();
            if (missingGroups.Count == 0) return;

            var message = $"Excel ფაილში ნაპოვნია შემდეგი ჯგუფები, რომლებიც ბაზაში არ არსებობენ:\n\n";
            message += string.Join("\n", missingGroups);
            message += "\n\nროგორ გსურთ გააგრძელოთ?";
            message += "\n\n• 'დიახ' - შექმენით ყველა ჯგუფი ნაგულისხმევი პარამეტრებით";
            message += "\n• 'არა' - გააგრძელეთ მხოლოდ არსებული ჯგუფებისთვის";
            message += "\n• 'გააუქმეთ' - გააუქმეთ იმპორტი";

            var result = MessageBox.Show(message, "აკლია ჯგუფები",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes:
                    CreateMissingGroups(missingGroups);
                    LoadGroups(); // ხელახლა ჩატვირთვა
                    break;
                case DialogResult.No:
                    break; // გააგრძელე მხოლოდ არსებული ჯგუფებისთვის
                case DialogResult.Cancel:
                default:
                    this.Close();
                    break;
            }
        }

        /// <summary>
        /// აკლია ჯგუფების შექმნა
        /// </summary>
        private void CreateMissingGroups(List<string> missingGroups)
        {
            var groupDetails = new Dictionary<string, (decimal price, string teacher)>();

            foreach (var groupName in missingGroups)
            {
                var result = ShowGroupCreationDialog(groupName);
                if (result == null) // მომხმარებელმა გააუქმა
                {
                    return;
                }
                groupDetails[groupName] = (result.Value.price, result.Value.teacher);
            }

            int createdCount = 0;
            foreach (var kvp in groupDetails)
            {
                if (kvp.Value.price == 0) continue; // გამოტოვება

                var group = new Group
                {
                    Name = kvp.Key,
                    Price = kvp.Value.price,
                    Teacher = string.IsNullOrWhiteSpace(kvp.Value.teacher) ? "მასწავლებელი" : kvp.Value.teacher,
                    Status = true,
                    MaxStudents = 30,
                    StudentCount = 0,
                    UpdatedAt = DateTime.Now
                };

                var groupId = _groupService.AddGroup(group);
                if (groupId.HasValue && groupId.Value > 0)
                {
                    var subGroup = new SubGroup
                    {
                        GroupId = groupId.Value,
                        TuitionFee = kvp.Value.price,
                        Status = true
                    };
                    _groupNameToIdMap[kvp.Key] = groupId.Value;
                    _subGroupService.AddSubGroups(subGroup, 4);
                    createdCount++;
                }
            }

            if (createdCount > 0)
            {
                MessageBox.Show($"წარმატებით შეიქმნა {createdCount} ჯგუფი", "ჯგუფები შეიქმნა",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// ჯგუფის შექმნის დიალოგი
        /// </summary>
        private (decimal price, string teacher)? ShowGroupCreationDialog(string groupName)
        {
            var form = new Form
            {
                Text = $"ჯგუფის შექმნა: {groupName}",
                Size = new Size(450, 250),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblName = new Label
            {
                Text = $"ჯგუფის სახელი: {groupName}",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font(form.Font, FontStyle.Bold)
            };

            var lblPrice = new Label { Text = "საფასური (₾):", Location = new Point(20, 60), AutoSize = true };
            var txtPrice = new TextBox { Location = new Point(120, 57), Width = 150, Text = "100" };

            var lblTeacher = new Label { Text = "მასწავლებელი:", Location = new Point(20, 90), AutoSize = true };
            var txtTeacher = new TextBox { Location = new Point(120, 87), Width = 150, Text = "მასწავლებელი" };

            var btnOK = new Button
            {
                Text = "შექმნა",
                Location = new Point(250, 150),
                Width = 80,
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "გააუქმეთ",
                Location = new Point(340, 150),
                Width = 80,
                DialogResult = DialogResult.Cancel
            };

            var btnSkip = new Button { Text = "გამოტოვეთ", Location = new Point(160, 150), Width = 80 };
            btnSkip.Click += (s, e) =>
            {
                form.DialogResult = DialogResult.Ignore;
                form.Close();
            };

            form.Controls.AddRange(new Control[] { lblName, lblPrice, txtPrice, lblTeacher, txtTeacher, btnOK, btnCancel, btnSkip });
            form.AcceptButton = btnOK;
            form.CancelButton = btnCancel;

            var dialogResult = form.ShowDialog();

            switch (dialogResult)
            {
                case DialogResult.OK:
                    if (decimal.TryParse(txtPrice.Text, out decimal price))
                    {
                        return (price, txtTeacher.Text.Trim());
                    }
                    else
                    {
                        MessageBox.Show("გთხოვთ შეიყვანოთ სწორი ფასი.", "შეცდომა", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return ShowGroupCreationDialog(groupName);
                    }
                case DialogResult.Ignore:
                    return (0, "მასწავლებელი");
                case DialogResult.Cancel:
                default:
                    return null;
            }
        }

        #endregion

        #region ==================== Sheet Mapping ====================

        /// <summary>
        /// შიტების მეპინგის UI-ის ჩატვირთვა
        /// ზედა ნაწილში: Sheet to Group Mapping (პირველი worksheet-ისთვის)
        /// შემდეგ: Column Mapping Grid (Excel სვეტები = მწკრივები, სტუდენტის ველები = სვეტები)
        /// </summary>
        private void LoadSheetMappings()
        {
            if (string.IsNullOrEmpty(_filePath) || !File.Exists(_filePath))
                return;

            _sheetGroupComboBoxes.Clear();
            _worksheetComboBoxes.Clear();
            tabWorksheets.TabPages.Clear();
            pnlSheetMapping.Controls.Clear();

            using (var workbook = new XLWorkbook(_filePath))
            {
                var worksheets = workbook.Worksheets.ToList();
                if (worksheets.Count == 0)
                    return;

                // Sheet to Group Mapping (ყველა worksheet-ისთვის)
                // TableLayoutPanel-ის შექმნა Sheet to Group mapping-ისთვის
                if (tblSheetMapping == null)
                {
                    tblSheetMapping = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 1,
                        RowCount = worksheets.Count,
                        Padding = new Padding(5)
                    };
                }
                else
                {
                    tblSheetMapping.Controls.Clear();
                    tblSheetMapping.RowCount = worksheets.Count;
                }

                // ყველა worksheet-ისთვის Sheet to Group mapping UI-ის შექმნა
                for (int i = 0; i < worksheets.Count; i++)
                {
                    var worksheet = worksheets[i];
                    var sheetName = worksheet.Name;
                    var rowIndex = i;

                    var pnlRow = new Panel
                    {
                        Dock = DockStyle.Top,
                        Height = 50,
                        Padding = new Padding(5)
                    };

                    var lblSheetGroup = new Label
                    {
                        Text = sheetName,
                        Font = new Font(this.Font, FontStyle.Bold),
                        Dock = DockStyle.Left,
                        Width = 200,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(5, 0, 10, 0)
                    };

                    var cmbGroup = new ComboBox
                    {
                        Dock = DockStyle.Fill,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Margin = new Padding(0, 5, 0, 5)
                    };
                    cmbGroup.Items.Add("-- აირჩიეთ ჯგუფი --");
                    foreach (var groupName in _groupNameToIdMap.Keys.OrderBy(k => k))
                    {
                        cmbGroup.Items.Add(groupName);
                    }
                    cmbGroup.SelectedIndex = 0;

                    // Auto-select თუ ჯგუფი არსებობს
                    if (_groupNameToIdMap.ContainsKey(sheetName))
                    {
                        var index = cmbGroup.Items.IndexOf(sheetName);
                        if (index >= 0)
                            cmbGroup.SelectedIndex = index;
                    }

                    _sheetGroupComboBoxes[sheetName] = cmbGroup;

                    pnlRow.Controls.Add(cmbGroup);
                    pnlRow.Controls.Add(lblSheetGroup);
                    tblSheetMapping.Controls.Add(pnlRow, 0, rowIndex);
                }

                var firstWorksheet = worksheets[0];

                // Column Mapping Grid
                // Excel-ის სვეტები = მწკრივები, სტუდენტის ველები = სვეტები
                var headerRow = firstWorksheet.FirstRow();
                var excelHeaders = new List<string>();
                foreach (var cell in headerRow.CellsUsed())
                {
                    excelHeaders.Add(cell.GetString());
                }

                // Grid-ის შექმნა
                var columnMappingGrid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = false,
                    RowHeadersVisible = true
                };

                // სვეტების დამატება (სტუდენტის ველები)
                columnMappingGrid.Columns.Add("ExcelColumn", "Excel სვეტი");
                columnMappingGrid.Columns[0].ReadOnly = true;
                columnMappingGrid.Columns[0].Width = 200;

                for (int i = 0; i < _studentFields.Count; i++)
                {
                    var fieldName = _studentFields[i];
                    var fieldNameGeorgian = _studentFieldNamesGeorgian[i];
                    
                    var column = new DataGridViewComboBoxColumn
                    {
                        Name = fieldName,
                        HeaderText = fieldNameGeorgian,
                        Width = 150
                    };
                    column.Items.Add("-- არ გამოიყენო --");
                    
                    // Auto-match
                    string matchedHeader = null;
                    foreach (var header in excelHeaders)
                    {
                        column.Items.Add(header);
                        if (matchedHeader == null && header.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                        {
                            matchedHeader = header;
                        }
                    }
                    
                    columnMappingGrid.Columns.Add(column);
                }

                // მწკრივების დამატება (Excel-ის სვეტები)
                var worksheetComboBoxes = new Dictionary<string, ComboBox>();
                foreach (var excelHeader in excelHeaders)
                {
                    int rowIndex = columnMappingGrid.Rows.Add();
                    var row = columnMappingGrid.Rows[rowIndex];
                    row.Cells[0].Value = excelHeader; // Excel სვეტის სახელი

                    // თითოეული სტუდენტის ველისთვის ComboBox-ის დაყენება
                    for (int colIndex = 1; colIndex < columnMappingGrid.Columns.Count; colIndex++)
                    {
                        var comboCell = row.Cells[colIndex] as DataGridViewComboBoxCell;
                        if (comboCell != null)
                        {
                            // Auto-match თუ სახელები ემთხვევა
                            var fieldName = columnMappingGrid.Columns[colIndex].Name;
                            if (excelHeader.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                            {
                                comboCell.Value = excelHeader;
                            }
                            else
                            {
                                comboCell.Value = "-- არ გამოიყენო --";
                            }
                        }
                    }
                }

                _worksheetComboBoxes[firstWorksheet.Name] = worksheetComboBoxes;
                _columnMappingGrid = columnMappingGrid;

                // Panel-ის შევსება: Sheet to Group mapping მარცხნივ, Column Mapping Grid მარჯვნივ
                var splitContainer = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical,
                    FixedPanel = FixedPanel.None // SplitterDistance-ის ცვლილება იმოქმედებს
                };

                // Sheet to Group mapping მარცხნივ
                var pnlSheetGroupMapping = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true
                };
                pnlSheetGroupMapping.Controls.Add(tblSheetMapping);
                splitContainer.Panel1.Controls.Add(pnlSheetGroupMapping);

                // Column Mapping Grid მარჯვნივ
                splitContainer.Panel2.Controls.Add(columnMappingGrid);

                pnlSheetMapping.Controls.Add(splitContainer);
                
                // SplitterDistance-ის დაყენება Panel-ების დამატების შემდეგ
                splitContainer.SplitterDistance = 450; // Panel1-ის სიგანე (Sheet to Group mapping) - მეტად გაზრდილი

                // Worksheet Tab-ების შექმნა
                foreach (var worksheet in worksheets)
                {
                    CreateWorksheetTab(worksheet);
                }
            }
        }

        /// <summary>
        /// Worksheet Tab-ის შექმნა Excel-ის მონაცემების პრევიუსთვის
        /// </summary>
        private void CreateWorksheetTab(IXLWorksheet worksheet)
        {
            var tabPage = new TabPage(worksheet.Name);
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10) };

            var lblInfo = new Label
            {
                Text = "აქ შესაბამისი მოსწავლეები",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font(this.Font, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            // Header-ების წაკითხვა
            var headerRow = worksheet.FirstRow();
            var headers = new List<string>();
            foreach (var cell in headerRow.CellsUsed())
            {
                headers.Add(cell.GetString());
            }

            // Grid-ის მონაცემების დამატება (ყველა მწკრივი)
            var dataTable = new System.Data.DataTable();
            foreach (var header in headers)
            {
                dataTable.Columns.Add(header);
            }

            // Header-ის სვეტების ინდექსების მიღება
            var headerColumnIndexes = new Dictionary<int, int>(); // Excel column index -> DataTable column index
            int dataTableColIndex = 0;
            foreach (var headerCell in headerRow.CellsUsed())
            {
                headerColumnIndexes[headerCell.Address.ColumnNumber] = dataTableColIndex;
                dataTableColIndex++;
            }

            // ყველა მონაცემთა მწკრივის წაკითხვა
            var allRows = worksheet.RowsUsed().Skip(1);
            foreach (var row in allRows)
            {
                var dataRow = dataTable.NewRow();
                
                // თითოეული header-ისთვის შესაბამისი უჯრის მნიშვნელობის წაკითხვა
                foreach (var kvp in headerColumnIndexes)
                {
                    var excelColumnIndex = kvp.Key;
                    var dataTableColumnIndex = kvp.Value;
                    
                    var cell = row.Cell(excelColumnIndex);
                    if (cell != null && !cell.IsEmpty())
                    {
                        dataRow[dataTableColumnIndex] = cell.GetString();
                    }
                    else
                    {
                        dataRow[dataTableColumnIndex] = string.Empty;
                    }
                }
                
                dataTable.Rows.Add(dataRow);
            }

            grid.DataSource = dataTable;
            _worksheetGrids[worksheet.Name] = grid;

            panel.Controls.Add(grid);
            panel.Controls.Add(lblInfo);
            tabPage.Controls.Add(panel);
            tabWorksheets.TabPages.Add(tabPage);
        }

        #endregion

        #region ==================== Import Process ====================

        /// <summary>
        /// იმპორტის ღილაკის დაჭერა
        /// </summary>
        private async void ImportButton_Click(object sender, EventArgs e)
        {
            btnImport.Enabled = false;
            progressBar.Visible = true;
            //logTextBox.Visible = true;
            //logTextBox.Clear();
            //logTextBox.AppendText("=== იმპორტის ლოგი ===\n\n");

            var logMessages = new List<string>();
            /*void LogMessage(string message)
            {
                var logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
                logMessages.Add(logEntry);
                
                // Thread-safe UI update
                if (logTextBox.InvokeRequired)
                {
                    logTextBox.Invoke(new Action(() =>
                    {
                        logTextBox.AppendText(logEntry + Environment.NewLine);
                        logTextBox.SelectionStart = logTextBox.Text.Length;
                        logTextBox.ScrollToCaret();
                    }));
                }
                else
                {
                    logTextBox.AppendText(logEntry + Environment.NewLine);
                    logTextBox.SelectionStart = logTextBox.Text.Length;
                    logTextBox.ScrollToCaret();
                }

                // სინქრონიზაციის სტატუსის განახლება
                if (message.Contains("სერვერზე ატვირთვა") || message.Contains("სინქრონიზაცია"))
                {
                    UpdateSyncStatus(message);
                }
            }*/

            // სინქრონიზაციის სტატუსის განახლების ფუნქცია
            void UpdateSyncStatus(string message)
            {
                if (syncStatusLabel.InvokeRequired)
                {
                    syncStatusLabel.Invoke(new Action(() =>
                    {
                        if (message.Contains("დაწყებულია"))
                        {
                            syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                            syncStatusLabel.ForeColor = Color.DarkBlue;
                        }
                        else if (message.Contains("დასრულდა"))
                        {
                            syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                            syncStatusLabel.ForeColor = Color.DarkGreen;
                        }
                        else if (message.Contains("შეცდომა"))
                        {
                            syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                            syncStatusLabel.ForeColor = Color.DarkRed;
                        }
                        else
                        {
                            syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                        }
                    }));
                }
                else
                {
                    if (message.Contains("დაწყებულია"))
                    {
                        syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                        syncStatusLabel.ForeColor = Color.DarkBlue;
                    }
                    else if (message.Contains("დასრულდა"))
                    {
                        syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                        syncStatusLabel.ForeColor = Color.DarkGreen;
                    }
                    else if (message.Contains("შეცდომა"))
                    {
                        syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                        syncStatusLabel.ForeColor = Color.DarkRed;
                    }
                    else
                    {
                        syncStatusLabel.Text = $"სინქრონიზაცია: {message}";
                    }
                }
            }

            try
            {
                //LogMessage("იმპორტის პროცესი დაწყებულია");

                // Sheet to Group Mapping
                var sheetToGroupIdMap = new Dictionary<string, int>();
                foreach (var entry in _sheetGroupComboBoxes)
                {
                    if (entry.Value.SelectedItem != null &&
                        entry.Value.SelectedItem.ToString() != "-- აირჩიეთ ჯგუფი --")
                    {
                        var groupName = entry.Value.SelectedItem.ToString();
                        if (_groupNameToIdMap.ContainsKey(groupName))
                        {
                            sheetToGroupIdMap.Add(entry.Key, _groupNameToIdMap[groupName]);
                            //LogMessage($"ჯგუფი '{groupName}' (ID: {_groupNameToIdMap[groupName]}) მიმაგრებულია worksheet '{entry.Key}'-ზე");
                        }
                    }
                }

                if (sheetToGroupIdMap.Count == 0)
                {
                    MessageBox.Show("გთხოვთ მიუთითოთ მინიმუმ ერთი worksheet ჯგუფზე.", "მეპინგი საჭიროა",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnImport.Enabled = true;
                    return;
                }

                // Column Mappings
                var columnMappings = new Dictionary<string, Dictionary<string, int>>();
                int totalRows = 0;

                using (var workbook = new XLWorkbook(_filePath))
                {
                    foreach (var sheetName in sheetToGroupIdMap.Keys)
                    {
                        var worksheet = workbook.Worksheet(sheetName);
                        var headers = new List<string>();
                        var headerColumnIndexes = new Dictionary<string, int>(); // header name -> Excel column index (1-based)

                        // Header-ების წაკითხვა Excel-ის რეალური სვეტის ინდექსებით
                        foreach (var cell in worksheet.FirstRow().CellsUsed())
                        {
                            var headerName = cell.GetString();
                            headers.Add(headerName);
                            headerColumnIndexes[headerName] = cell.Address.ColumnNumber; // Excel column index (1-based)
                        }

                        var mapping = new Dictionary<string, int>();

                        if (chkEnableMapping.Checked && _columnMappingGrid != null)
                        {
                            // Column Mapping Grid-იდან წაკითხვა
                            // Excel სვეტები = მწკრივები, სტუდენტის ველები = სვეტები
                            // თითოეული Excel სვეტისთვის (მწკრივი) და თითოეული სტუდენტის ველისთვის (სვეტი)
                            // თუ ComboBox-ში არჩეული Excel header-ის სახელი არ არის "-- არ გამოიყენო --",
                            // მაშინ ეს Excel სვეტი (row.Cells[0]) უნდა დაუკავშირდეს ამ სტუდენტის ველს
                            foreach (DataGridViewRow row in _columnMappingGrid.Rows)
                            {
                                if (row.IsNewRow) continue;
                                
                                var excelColumnName = row.Cells[0].Value?.ToString();
                                if (string.IsNullOrEmpty(excelColumnName)) continue;
                                
                                // თითოეული სტუდენტის ველისთვის (სვეტები 1-დან)
                                for (int colIndex = 1; colIndex < _columnMappingGrid.Columns.Count; colIndex++)
                                {
                                    var fieldName = _columnMappingGrid.Columns[colIndex].Name;
                                    var comboCell = row.Cells[colIndex] as DataGridViewComboBoxCell;
                                    
                                    if (comboCell != null && comboCell.Value != null)
                                    {
                                        var selectedValue = comboCell.Value.ToString();
                                        
                                        // თუ ComboBox-ში არჩეული Excel header-ის სახელი არ არის "-- არ გამოიყენო --"
                                        // და ეს Excel header არსებობს headers-ში, მაშინ ეს Excel სვეტი უნდა დაუკავშირდეს ამ სტუდენტის ველს
                                        // მნიშვნელოვანია: ComboBox-ში არჩეული Excel header-ის სახელი უნდა ემთხვეოდეს Excel სვეტის სახელს (row.Cells[0])
                                        // რადგან Column Mapping Grid-ში თითოეული მწკრივი წარმოადგენს Excel სვეტს
                                        if (selectedValue != "-- არ გამოიყენო --" && 
                                            selectedValue == excelColumnName &&
                                            headerColumnIndexes.ContainsKey(excelColumnName))
                                        {
                                            // Excel-ის სვეტის რეალური ინდექსი (1-based), რომელიც გამოიყენება ImportService-ში
                                            mapping[fieldName] = headerColumnIndexes[excelColumnName] - 1; // Convert to 0-based for ImportService
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Auto-match
                            foreach (var field in _studentFields)
                            {
                                var matchedHeader = headers.FirstOrDefault(h =>
                                    h.Equals(field, StringComparison.OrdinalIgnoreCase));
                                if (matchedHeader != null && headerColumnIndexes.ContainsKey(matchedHeader))
                                {
                                    // Excel-ის სვეტის რეალური ინდექსი (1-based), რომელიც გამოიყენება ImportService-ში
                                    mapping[field] = headerColumnIndexes[matchedHeader] - 1; // Convert to 0-based for ImportService
                                }
                            }
                        }

                        columnMappings[sheetName] = mapping;

                        var worksheetRows = worksheet.RowsUsed().Skip(1).Count();
                        totalRows += worksheetRows;
                        //LogMessage($"Worksheet '{sheetName}': {worksheetRows} მონაცემთა მწკრივი, {mapping.Count} ველის მეპინგი");
                    }
                }

                //LogMessage($"სულ ნაპოვნია {totalRows} მონაცემთა მწკრივი {sheetToGroupIdMap.Count} worksheet-ში");

                // Progress Bar
                progressBar.Maximum = totalRows * 2;
                progressBar.Value = 0;

                var dbProgress = new Progress<(int current, int total, string worksheet)>(p =>
                {
                    if (progressBar.InvokeRequired)
                    {
                        progressBar.Invoke(new Action(() =>
                        {
                            progressBar.Maximum = p.total;
                            progressBar.Value = p.current;
                            statusLabel.Text =
                                $"დამუშავებულია {p.current} / {p.total} სტუდენტი: {p.worksheet}";
                        }));
                    }
                    else
                    {
                        progressBar.Maximum = p.total;
                        progressBar.Value = p.current;
                        statusLabel.Text = $"დამუშავებულია {p.current} / {p.total} სტუდენტი: {p.worksheet}";
                    }
                });

                // იმპორტის გაშვება
                //LogMessage("იმპორტის სერვისის გამოძახება დაწყებულია");

                // სინქრონიზაციის სტატუსის განახლების callback
                Action<string> syncStatusCallback = (message) =>
                {
                    if (message.Contains("სერვერზე ატვირთვა") || message.Contains("სინქრონიზაცია"))
                    {
                        UpdateSyncStatus(message);
                    }
                };

                var result = await Task.Run(() =>
                    _importService.ImportStudentsAsync(_filePath, columnMappings, sheetToGroupIdMap,
                        chkIsActive.Checked, dbProgress, syncStatusCallback));

                //LogMessage($"იმპორტის შედეგი: Success={result.Success}, ImportedCount={result.ImportedCount}, Dublicates={result.Dublicates}");

                // სინქრონიზაციის სტატუსის განახლება იმპორტის დასრულების შემდეგ
                if (result.Success && result.ImportedCount > 0)
                {
                    UpdateSyncStatus("სინქრონიზაცია მიმდინარეობს...");
                }
                else
                {
                    UpdateSyncStatus("სინქრონიზაცია: მზადაა");
                }

                // Thread-safe MessageBox
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (result.Success)
                        {
                            var message = $"იმპორტი დასრულდა!\n\n";
                            message += $"✅ დაიმპორტირებული სტუდენტები: {result.ImportedCount}\n";
                            message += $"⚠️ დუბლირების მცდელობა/შეცდომები: {result.Dublicates}";

                            MessageBox.Show(message, "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"იმპორტი ვერ მოხერხდა: {result.ErrorMessage}", "შეცდომა",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                }
                else
                {
                    if (result.Success)
                    {
                        var message = $"იმპორტი დასრულდა!\n\n";
                        message += $"✅ დაიმპორტირებული სტუდენტები: {result.ImportedCount}\n";
                        message += $"⚠️ დუბლირების მცდელობა/შეცდომები: {result.Dublicates}";

                        MessageBox.Show(message, "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"იმპორტი ვერ მოხერხდა: {result.ErrorMessage}", "შეცდომა",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Thread-safe UI update
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        statusLabel.Text = "იმპორტი დასრულებულია";
                    }));
                }
                else
                {
                    statusLabel.Text = "იმპორტი დასრულებულია";
                }
            }
            catch (Exception ex)
            {
                //LogMessage($"მოულოდნელი შეცდომა: {ex.Message}");
                
                // Thread-safe MessageBox
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"მოულოდნელი შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show($"მოულოდნელი შეცდომა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Thread-safe UI update
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        btnImport.Enabled = true;
                        if (progressBar.Maximum > 0)
                            progressBar.Value = progressBar.Maximum;
                    }));
                }
                else
                {
                    btnImport.Enabled = true;
                    if (progressBar.Maximum > 0)
                        progressBar.Value = progressBar.Maximum;
                }

                // ლოგის შენახვა
                try
                {
                    var logDirectory = Path.Combine(System.Windows.Forms.Application.StartupPath, "ImportLogs");
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }

                    var logFileName = Path.Combine(logDirectory, $"ImportLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    var logContent = string.Join("\n", logMessages);
                    File.WriteAllText(logFileName, logContent);

                    //LogMessage($"\n✅ ლოგი შენახულია: {logFileName}");
                }
                catch
                {
                    // იგნორირება
                }
            }
        }

        #endregion

    }
}

