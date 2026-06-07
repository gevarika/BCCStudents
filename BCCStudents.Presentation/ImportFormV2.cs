using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Newtonsoft.Json;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// სტუდენტების იმპორტის ფორმა Excel ფაილებიდან - Refactored Version
    /// </summary>
    /// 
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class ImportFormV2 : BaseForm
    {
        #region Fields

        private readonly IImportService _importService;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupService _groupService;
        private readonly ISubGroupService _subGroupService;
        private readonly IUserContext _userContext;

        private string _filePath;
        private Stream _fileStream;
        private ImportMappingConfiguration _mappingConfiguration;
        private Dictionary<string, int> _groupNameToIdMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // UI Components that are NOT in Designer file (dynamically created)
        private Dictionary<string, DataGridView> _columnMappingGrids = new Dictionary<string, DataGridView>(StringComparer.OrdinalIgnoreCase);
        private TabControl _tabControlColumnMapping;
        private bool _splitterDistanceSet = false;

        // Student Fields
        private static readonly List<string> StudentFields = new List<string>
        {
            nameof(Student.StudentCode),
            nameof(Student.FirstName),
            nameof(Student.LastName),
            nameof(Student.Age),
            nameof(Student.ParentName),
            nameof(Student.PhoneNumber),
            nameof(Student.Id_Numb),
            nameof(Student.Address),
            nameof(Student.Discount),
            "SubGroup"
        };

        private static readonly List<string> StudentFieldNamesGeorgian = new List<string>
        {
            "სტუდენტის კოდი",
            "სახელი",
            "გვარი",
            "ასაკი",
            "მშობლის სახელი",
            "ტელეფონი",
            "პირადი ნომერი",
            "მისამართი",
            "საფასური",
            "ფასდაკლება",
            "ქვეჯგუფი"
        };

        private static readonly HashSet<string> RequiredFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Student.FirstName),
            nameof(Student.LastName),
            nameof(Student.Age),
            nameof(Student.PhoneNumber),
            nameof(Student.Id_Numb),
            nameof(Student.Discount),
            nameof(Student.Address),
            nameof(Student.ParentName)
        };

        #endregion

        #region Constructor

        public ImportFormV2(
            IImportService importService,
            IGroupRepository groupRepository,
            IGroupService groupService,
            ISubGroupService subGroupService,
            IUserContext userContext)
        {
            _importService = importService ?? throw new ArgumentNullException(nameof(importService));
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
            _subGroupService = subGroupService ?? throw new ArgumentNullException(nameof(subGroupService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));

            InitializeComponent();
            InitializeUIAfterDesigner();
            InitializeUI();
            LoadGroups();
        }

        private void ImportFormV2_Load(object sender, EventArgs e)
        {
            // Set WindowState to Maximized after form is loaded
            this.WindowState = FormWindowState.Maximized;
            ApplySecurityChecks();
        }

        private void ApplySecurityChecks()
        {
            // _btnImport - CanImport permission
            if (_btnImport != null)
            {
                _btnImport.Tag = $"Permission_{Permission.CanImport}";
                _btnImport.Enabled = _userContext.HasPermission(Permission.CanImport);
            }

            // _btnSelectFile - CanImport permission (file selection is part of import process)
            if (_btnSelectFile != null)
            {
                _btnSelectFile.Tag = $"Permission_{Permission.CanImport}";
                _btnSelectFile.Enabled = _userContext.HasPermission(Permission.CanImport);
            }
        }

        private void ImportFormV2_Shown(object sender, EventArgs e)
        {
            // Set splitter distance after form is fully shown and laid out
            // Use BeginInvoke to ensure all layout operations are complete
            this.BeginInvoke(new Action(() =>
            {
                SetSplitterDistanceSafe();
            }));
            this.Shown -= ImportFormV2_Shown; // Remove handler after first use
        }

        private void SetSplitterDistanceSafe()
        {
            if (_splitContainerMain == null || _splitterDistanceSet) return;

            try
            {
                // Ensure the container has been laid out and has a valid width
                if (!_splitContainerMain.IsHandleCreated || _splitContainerMain.Width <= 0)
                {
                    // Retry after a short delay if handle not created yet or width is invalid
                    this.BeginInvoke(new Action(() => SetSplitterDistanceSafe()), 50);
                    return;
                }

                int containerWidth = _splitContainerMain.Width;
                int minDistance = _splitContainerMain.Panel1MinSize;
                int splitterWidth = _splitContainerMain.SplitterWidth;
                int panel2MinSize = _splitContainerMain.Panel2MinSize;
                int maxDistance = containerWidth - panel2MinSize - splitterWidth;

                // Ensure we have enough space
                if (maxDistance >= minDistance)
                {
                    // Set to 30% of width, but ensure it's within valid range
                    int desiredDistance = Math.Max(minDistance,
                        Math.Min(maxDistance, (int)(containerWidth * 0.3)));

                    // Validate that desiredDistance is within valid range before setting
                    if (desiredDistance >= minDistance && desiredDistance <= maxDistance)
                    {
                        _splitContainerMain.SplitterDistance = desiredDistance;
                        _splitterDistanceSet = true;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // If setting fails, SplitContainer will keep its default
                // Don't set _splitterDistanceSet to true, so it will retry if needed
            }
        }

        #endregion

        #region UI Initialization

        // InitializeComponent is now in ImportFormV2.Designer.cs
        // Additional UI initialization that can't be done in Designer
        private void InitializeUIAfterDesigner()
        {
            LoadSavedMappings();
            this.Load += ImportFormV2_Load;
        }

        private void InitializeUI()
        {
            _mappingConfiguration = new ImportMappingConfiguration
            {
                IsActive = _chkIsActive.Checked
            };
        }

        #endregion

        #region Event Handlers

        private async void BtnSelectFile_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanImport))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "აირჩიეთ Excel ფაილი";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = openFileDialog.FileName;
                    _lblFileName.Text = Path.GetFileName(_filePath);

                    await LoadExcelFileAsync();
                }
            }
        }

        private void ChkEnableMapping_CheckedChanged(object sender, EventArgs e)
        {
            if (_splitContainerMain != null)
            {
                _splitContainerMain.Panel1Collapsed = !_chkEnableMapping.Checked;
                _pnlMapping.Visible = _chkEnableMapping.Checked;
            }
        }

        private async void BtnImport_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanImport))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await StartImportAsync();
        }

        #endregion

        #region Excel Loading

        private async Task LoadExcelFileAsync()
        {
            try
            {
                UpdateStatus("Excel ფაილის ჩატვირთვა...");

                // Open file stream
                if (_fileStream != null)
                {
                    _fileStream.Dispose();
                }
                _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Get sheets
                var sheets = await _importService.GetExcelSheetsAsync(_fileStream);

                if (!sheets.Any())
                {
                    MessageBox.Show("Excel ფაილში sheet-ები არ მოიძებნა.", "შეცდომა",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate groups exist for all sheets
                var missingGroups = await ValidateGroupsForSheetsAsync(sheets);
                if (missingGroups.Any())
                {
                    var result = ShowMissingGroupsDialog(missingGroups);
                    if (result == DialogResult.Cancel)
                        return;

                    if (result == DialogResult.Yes)
                    {
                        await CreateMissingGroupsAsync(missingGroups);
                        LoadGroups(); // Reload groups
                    }
                }

                // Load sheets preview
                await LoadSheetsPreviewAsync(sheets);

                // Initialize mapping configuration only if not already set (from LoadMapping for example)
                if (_mappingConfiguration == null)
                {
                    _mappingConfiguration = new ImportMappingConfiguration
                    {
                        IsActive = _chkIsActive.Checked
                    };
                }
                else
                {
                    // Preserve existing mapping configuration, but update IsActive if needed
                    _mappingConfiguration.IsActive = _chkIsActive.Checked;
                }

                // Setup mapping UI
                if (_chkEnableMapping.Checked)
                {
                    await SetupMappingUIAsync(sheets);
                    _btnSaveMapping.Enabled = true;
                }

                _btnImport.Enabled = true;
                UpdateStatus($"ფაილი ჩატვირთულია: {sheets.Count} sheet");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ფაილის ჩატვირთვის შეცდომა: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("შეცდომა ფაილის ჩატვირთვისას");
            }
        }

        private async Task LoadSheetsPreviewAsync(List<string> sheets)
        {
            _tabControlSheets.TabPages.Clear();

            foreach (var sheetName in sheets)
            {
                try
                {
                    var preview = await _importService.GetSheetPreviewAsync(_fileStream, sheetName, 100);

                    var tabPage = new TabPage(sheetName);
                    var grid = new DataGridView
                    {
                        Dock = DockStyle.Fill,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                        ReadOnly = true,
                        AllowUserToAddRows = false,
                        DataSource = preview
                    };

                    tabPage.Controls.Add(grid);
                    _tabControlSheets.TabPages.Add(tabPage);
                }
                catch (Exception ex)
                {
                    var tabPage = new TabPage(sheetName);
                    var lblError = new Label
                    {
                        Text = $"შეცდომა: {ex.Message}",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.Red
                    };
                    tabPage.Controls.Add(lblError);
                    _tabControlSheets.TabPages.Add(tabPage);
                }
            }
        }

        #endregion

        #region Group Management

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

        private async Task<List<string>> ValidateGroupsForSheetsAsync(List<string> sheets)
        {
            return await Task.Run(() =>
            {
                var missing = new List<string>();
                foreach (var sheet in sheets)
                {
                    if (!_groupNameToIdMap.ContainsKey(sheet))
                    {
                        missing.Add(sheet);
                    }
                }
                return missing;
            });
        }

        private DialogResult ShowMissingGroupsDialog(List<string> missingGroups)
        {
            var message = "Excel ფაილში ნაპოვნია შემდეგი sheet-ები, რომლებიც ბაზაში არ არსებობს:\n\n";
            message += string.Join("\n", missingGroups);
            message += "\n\nროგორ გსურთ გააგრძელოთ?";
            message += "\n\n• 'დიახ' - შექმენით ყველა ჯგუფი";
            message += "\n• 'არა' - გააგრძელეთ ხელით მეპინგით";
            message += "\n• 'გააუქმეთ' - გააუქმეთ იმპორტი";

            return MessageBox.Show(message, "აკლია ჯგუფები",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        private async Task CreateMissingGroupsAsync(List<string> missingGroups)
        {
            foreach (var groupName in missingGroups)
            {
                // Show dialog for each group to get user input
                using (var dialog = new CreateGroupDialog(groupName))
                {
                    var result = dialog.ShowDialog(this);

                    if (result == DialogResult.OK && dialog.CreatedGroup != null)
                    {
                        // User wants to create the group
                        var group = dialog.CreatedGroup;

                        // Create the group
                        var groupId = _groupService.AddGroup(group);
                        if (groupId.HasValue && groupId.Value > 0)
                        {
                            _groupNameToIdMap[groupName] = groupId.Value;

                            // Create SubGroups if requested
                            if (dialog.SubGroupCount > 0)
                            {
                                var subGroup = new SubGroup
                                {
                                    GroupId = groupId.Value,
                                    ParentGroupName = group.Name,
                                    TuitionFee = group.Price,
                                    MaxStudents = group.MaxStudents,
                                    Status = true,
                                    StudentCount = 0,
                                    UpdatedAt = DateTime.Now
                                };

                                _subGroupService.AddSubGroups(subGroup, dialog.SubGroupCount);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"ჯგუფის '{groupName}' შექმნა ვერ მოხერხდა.", "შეცდომა",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (result == DialogResult.Ignore)
                    {
                        // User wants to skip this group, continue to next
                        continue;
                    }
                    else
                    {
                        // User cancelled (Cancel button), stop creating groups
                        break;
                    }
                }
            }
        }

        #endregion

        #region Mapping UI

        private Dictionary<string, ComboBox> _sheetGroupComboBoxes = new Dictionary<string, ComboBox>();
        private Dictionary<string, List<string>> _sheetHeaders = new Dictionary<string, List<string>>();

        private async Task SetupMappingUIAsync(List<string> sheets)
        {
            // Clear only dynamic content, keep static containers from Designer
            flowLayoutSheetMapping.Controls.Clear();
            _sheetGroupComboBoxes.Clear();
            _sheetHeaders.Clear();

            // Clear column mapping grids if they exist
            foreach (var grid in _columnMappingGrids.Values)
            {
                if (grid != null && grid.Parent != null)
                {
                    grid.Parent.Controls.Remove(grid);
                    grid.Dispose();
                }
            }
            _columnMappingGrids.Clear();

            // Clear TabControl if it exists
            if (_tabControlColumnMapping != null && pnlColumnMapping.Controls.Contains(_tabControlColumnMapping))
            {
                pnlColumnMapping.Controls.Remove(_tabControlColumnMapping);
                _tabControlColumnMapping.Dispose();
                _tabControlColumnMapping = null;
            }

            // Load headers for each sheet
            foreach (var sheetName in sheets)
            {
                try
                {
                    _fileStream.Position = 0;
                    var headers = await _importService.GetSheetHeadersAsync(_fileStream, sheetName);
                    _sheetHeaders[sheetName] = headers;

                    // Create sheet-to-group mapping control
                    var pnlSheet = new Panel
                    {
                        Width = 350,
                        Height = 35,
                        Margin = new Padding(5),
                        BackColor = Color.LightGray
                    };

                    var lblSheet = new Label
                    {
                        Text = sheetName,
                        Location = new Point(0, 9),
                        Width = 150,
                        AutoSize = false,
                        ForeColor = Color.Blue
                    };

                    var cmbGroup = new ComboBox
                    {
                        Location = new Point(155, 8),
                        Width = 160,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmbGroup.Items.Add("-- აირჩიეთ ჯგუფი --");
                    foreach (var groupName in _groupNameToIdMap.Keys.OrderBy(k => k))
                    {
                        cmbGroup.Items.Add(groupName);
                    }
                    cmbGroup.SelectedIndex = 0;

                    // Auto-select if group name matches sheet name
                    if (_groupNameToIdMap.ContainsKey(sheetName))
                    {
                        var index = cmbGroup.Items.IndexOf(sheetName);
                        if (index >= 0)
                            cmbGroup.SelectedIndex = index;
                    }

                    _sheetGroupComboBoxes[sheetName] = cmbGroup;

                    pnlSheet.Controls.Add(lblSheet);
                    pnlSheet.Controls.Add(cmbGroup);
                    flowLayoutSheetMapping.Controls.Add(pnlSheet);
                }
                catch (Exception ex)
                {
                    // Log error but continue
                }
            }

            // Setup column mapping TabControl (one TabPage per sheet)
            await SetupColumnMappingTabControlAsync(pnlColumnMapping, sheets);
        }

        private async Task SetupColumnMappingTabControlAsync(Panel container, List<string> sheets)
        {
            // Create TabControl for Column Mapping
            _tabControlColumnMapping = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Create TabPage for each sheet
            foreach (var sheetName in sheets)
            {
                var tabPage = new TabPage(sheetName);
                var panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(5)
                };

                // Create DataGridView for this sheet
                await SetupColumnMappingGridAsync(panel, sheetName);

                tabPage.Controls.Add(panel);
                _tabControlColumnMapping.TabPages.Add(tabPage);
            }

            container.Controls.Add(_tabControlColumnMapping);
        }

        private async Task SetupColumnMappingGridAsync(Panel container, string sheetName)
        {
            // Get headers for this specific sheet
            if (!_sheetHeaders.TryGetValue(sheetName, out var headers) || headers == null)
            {
                return;
            }

            var allHeaders = headers.OrderBy(h => h).ToList();

            // Create DataGridView for this sheet
            var columnMappingGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                RowHeadersVisible = true
            };

            _columnMappingGrids[sheetName] = columnMappingGrid;

            // Get saved column mapping for this sheet (if exists)
            Dictionary<string, string> savedColumnMapping = null;
            if (_mappingConfiguration?.ColumnMapping != null)
            {
                _mappingConfiguration.ColumnMapping.TryGetValue(sheetName, out savedColumnMapping);
            }

            // Add Excel Column column
            columnMappingGrid.Columns.Add("ExcelColumn", "Excel სვეტი");
            columnMappingGrid.Columns[0].ReadOnly = true;
            columnMappingGrid.Columns[0].Width = 200;

            // Add Student Field columns
            for (int i = 0; i < StudentFields.Count; i++)
            {
                var fieldName = StudentFields[i];
                var fieldNameGeorgian = StudentFieldNamesGeorgian[i];
                bool isRequired = RequiredFields.Contains(fieldName);

                var column = new DataGridViewComboBoxColumn
                {
                    Name = fieldName,
                    HeaderText = isRequired ? $"{fieldNameGeorgian} *" : fieldNameGeorgian,
                    Width = 150
                };

                // Add "-- არ გამოიყენო --" only for non-required fields
                if (!isRequired)
                {
                    column.Items.Add("-- არ გამოიყენო --");
                }

                // Add all headers for this sheet
                foreach (var header in allHeaders)
                {
                    column.Items.Add(header);
                }

                columnMappingGrid.Columns.Add(column);
            }

            // Add rows (one for each Excel header in this sheet)
            foreach (var header in allHeaders)
            {
                int rowIndex = columnMappingGrid.Rows.Add();
                var row = columnMappingGrid.Rows[rowIndex];
                row.Cells[0].Value = header;

                // Set value based on saved mapping or auto-match
                for (int colIndex = 1; colIndex < columnMappingGrid.Columns.Count; colIndex++)
                {
                    var fieldName = columnMappingGrid.Columns[colIndex].Name;
                    var comboCell = row.Cells[colIndex] as DataGridViewComboBoxCell;

                    if (comboCell != null)
                    {
                        bool isRequired = RequiredFields.Contains(fieldName);

                        // Check if we have saved mapping for this column
                        if (savedColumnMapping != null && savedColumnMapping.TryGetValue(header, out var mappedField) && mappedField == fieldName)
                        {
                            comboCell.Value = header;
                        }
                        else if (header.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Auto-match: if header matches field name
                            comboCell.Value = header;
                        }
                        else if (!isRequired)
                        {
                            comboCell.Value = "-- არ გამოიყენო --";
                        }
                    }
                }
            }

            // DataError handler
            columnMappingGrid.DataError += (sender, e) =>
            {
                e.ThrowException = false;
            };

            container.Controls.Add(columnMappingGrid);
        }

        #endregion

        #region Import Process

        private async Task StartImportAsync()
        {
            _btnImport.Enabled = false;
            _progressBar.Visible = true;
            _progressBar.Value = 0;

            try
            {
                // Build mapping configuration
                if (!BuildMappingConfiguration())
                {
                    MessageBox.Show("მეპინგის კონფიგურაცია არავალიდურია.", "შეცდომა",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate configuration
                if (!_mappingConfiguration.Validate())
                {
                    MessageBox.Show("გთხოვთ დაასრულოთ sheet-ების და column-ების მეპინგი.", "შეცდომა",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create progress reporter
                var progress = new Progress<(int current, int total, string status)>(p =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => UpdateProgress(p.current, p.total, p.status)));
                    }
                    else
                    {
                        UpdateProgress(p.current, p.total, p.status);
                    }
                });

                // Reset stream position
                _fileStream.Position = 0;

                // Start import
                var result = await _importService.ImportStudentsAsync(_fileStream, _mappingConfiguration, progress);

                // Show result
                ShowImportResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"იმპორტის შეცდომა: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"შეცდომა: {ex.Message}");
            }
            finally
            {
                _btnImport.Enabled = true;
                _progressBar.Visible = false;
            }
        }

        private bool BuildMappingConfiguration()
        {
            _mappingConfiguration = new ImportMappingConfiguration
            {
                IsActive = _chkIsActive.Checked
            };

            // Build Sheet-to-Group mapping
            foreach (var kvp in _sheetGroupComboBoxes)
            {
                var sheetName = kvp.Key;
                var comboBox = kvp.Value;

                if (comboBox.SelectedItem != null &&
                    comboBox.SelectedItem.ToString() != "-- აირჩიეთ ჯგუფი --")
                {
                    var groupName = comboBox.SelectedItem.ToString();
                    if (_groupNameToIdMap.TryGetValue(groupName, out int groupId))
                    {
                        _mappingConfiguration.SheetToGroupMapping[sheetName] = groupId;
                    }
                }
            }

            if (_mappingConfiguration.SheetToGroupMapping.Count == 0)
            {
                MessageBox.Show("გთხოვთ მიუთითოთ მინიმუმ ერთი sheet-ის დაკავშირება ჯგუფთან.",
                    "მეპინგი საჭიროა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Build Column Mapping per sheet (if enabled)
            if (_chkEnableMapping.Checked && _tabControlColumnMapping != null)
            {
                // Manual mapping from grids (one grid per sheet)
                foreach (var sheetName in _mappingConfiguration.SheetToGroupMapping.Keys)
                {
                    if (!_columnMappingGrids.TryGetValue(sheetName, out var columnMappingGrid) || columnMappingGrid == null)
                        continue;

                    var sheetColumnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    foreach (DataGridViewRow row in columnMappingGrid.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var excelColumnName = row.Cells[0].Value?.ToString();
                        if (string.IsNullOrEmpty(excelColumnName)) continue;

                        for (int colIndex = 1; colIndex < columnMappingGrid.Columns.Count; colIndex++)
                        {
                            var fieldName = columnMappingGrid.Columns[colIndex].Name;
                            var comboCell = row.Cells[colIndex] as DataGridViewComboBoxCell;

                            if (comboCell != null && comboCell.Value != null)
                            {
                                var selectedValue = comboCell.Value.ToString();

                                // თუ ComboBox-ში არჩეულია ეს Excel column (excelColumnName)
                                if (selectedValue != "-- არ გამოიყენო --" &&
                                    selectedValue.Equals(excelColumnName, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Excel column -> Student property mapping for this sheet
                                    sheetColumnMapping[excelColumnName] = fieldName;
                                    break; // One column can only map to one field
                                }
                            }
                        }
                    }

                    // Store column mapping for this sheet
                    _mappingConfiguration.ColumnMapping[sheetName] = sheetColumnMapping;

                    // Validate required fields for this sheet
                    foreach (var requiredField in RequiredFields)
                    {
                        if (!sheetColumnMapping.ContainsValue(requiredField))
                        {
                            var fieldNameGeorgian = StudentFieldNamesGeorgian[StudentFields.IndexOf(requiredField)];
                            MessageBox.Show($"Sheet '{sheetName}': აუცილებელი ველი '{fieldNameGeorgian}' არ არის მეპინგში.",
                                "მეპინგის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            else
            {
                // Auto-match: for each sheet, try to match headers to field names
                foreach (var sheetMapping in _mappingConfiguration.SheetToGroupMapping)
                {
                    var sheetName = sheetMapping.Key;
                    var sheetColumnMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    if (_sheetHeaders.TryGetValue(sheetName, out var headers))
                    {
                        foreach (var header in headers)
                        {
                            // Try to find matching field name
                            var matchingField = StudentFields.FirstOrDefault(f =>
                                string.Equals(f.Replace(" ", ""), header.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));

                            if (matchingField != null)
                            {
                                sheetColumnMapping[header] = matchingField;
                            }
                        }
                    }

                    // Store column mapping for this sheet
                    _mappingConfiguration.ColumnMapping[sheetName] = sheetColumnMapping;

                    // Validate required fields for auto-match (per sheet)
                    foreach (var requiredField in RequiredFields)
                    {
                        if (!sheetColumnMapping.ContainsValue(requiredField))
                        {
                            var fieldNameGeorgian = StudentFieldNamesGeorgian[StudentFields.IndexOf(requiredField)];
                            MessageBox.Show($"Sheet '{sheetName}': Auto-match-მა ვერ იპოვა აუცილებელი ველი '{fieldNameGeorgian}'. გთხოვთ ჩართოთ სვეტების მეპინგი.",
                                "Auto-match შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void UpdateProgress(int current, int total, string status)
        {
            if (total > 0)
            {
                _progressBar.Maximum = total;
                _progressBar.Value = Math.Min(current, total);
            }
            _lblStatus.Text = status;
            _statusLabel.Text = status;
            System.Windows.Forms.Application.DoEvents();
        }

        private void ShowImportResult(ImportResult result)
        {
            if (result.IsSuccess)
            {
                var message = $"იმპორტი დასრულდა!\n\n";
                message += $"✅ დაიმპორტირებული სტუდენტები: {result.ImportedCount}\n";
                message += $"⚠️ დუბლირების მცდელობა/შეცდომები: {result.Dublicates}";

                if (result.Errors != null && result.Errors.Any())
                {
                    message += $"\n\n⚠️ შეცდომები: {result.Errors.Count}";
                }

                var dialogResult = MessageBox.Show(message, "წარმატება",
                    result.Errors != null && result.Errors.Any()
                        ? MessageBoxButtons.OK
                        : MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Show errors if any
                if (result.Errors != null && result.Errors.Any())
                {
                    var errorForm = new Form
                    {
                        Text = "იმპორტის შეცდომები",
                        Size = new Size(600, 400),
                        StartPosition = FormStartPosition.CenterParent
                    };

                    var txtErrors = new TextBox
                    {
                        Multiline = true,
                        ReadOnly = true,
                        ScrollBars = ScrollBars.Vertical,
                        Dock = DockStyle.Fill,
                        Font = new Font("Consolas", 9)
                    };

                    txtErrors.Text = string.Join(Environment.NewLine, result.Errors.Take(100)); // Show first 100 errors
                    if (result.Errors.Count > 100)
                    {
                        txtErrors.Text += $"\n\n... და კიდევ {result.Errors.Count - 100} შეცდომა";
                    }

                    errorForm.Controls.Add(txtErrors);
                    errorForm.ShowDialog();
                }

                UpdateStatus($"იმპორტი დასრულებულია: {result.ImportedCount} სტუდენტი");
            }
            else
            {
                var errorMessage = result.ErrorMessage;
                if (result.Errors != null && result.Errors.Any())
                {
                    errorMessage += $"\n\nშეცდომების რაოდენობა: {result.Errors.Count}";
                    errorMessage += "\n\nპირველი შეცდომები:";
                    errorMessage += "\n" + string.Join("\n", result.Errors.Take(5));
                }

                MessageBox.Show(errorMessage, "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"შეცდომა: {result.ErrorMessage}");
            }
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    _statusLabel.Text = message;
                    System.Windows.Forms.Application.DoEvents();
                }));
            }
            else
            {
                _statusLabel.Text = message;
            }
        }

        #endregion

        #region Mapping Preset Management

        private void LoadSavedMappings()
        {
            _cmbSavedMappings.Items.Clear();
            _cmbSavedMappings.Items.Add("-- აირჩიეთ შენახული მეპინგი --");
            _cmbSavedMappings.SelectedIndex = 0;

            try
            {
                var mappingsDirectory = Path.Combine(System.Windows.Forms.Application.StartupPath, "ImportMappings");
                if (!Directory.Exists(mappingsDirectory))
                    return;

                var presetFiles = Directory.GetFiles(mappingsDirectory, "*.json");
                foreach (var file in presetFiles)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var preset = JsonConvert.DeserializeObject<ImportMappingPreset>(json);
                        if (preset != null)
                        {
                            _cmbSavedMappings.Items.Add(new { Preset = preset, FilePath = file });
                        }
                    }
                    catch
                    {
                        // Skip invalid files
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void BtnSaveMapping_Click(object sender, EventArgs e)
        {
            if (_mappingConfiguration == null || !_mappingConfiguration.Validate())
            {
                MessageBox.Show("მეპინგი არავალიდურია შენახვისთვის.", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simple input dialog
            var inputForm = new Form
            {
                Text = "მეპინგის შენახვა",
                Size = new Size(400, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblPrompt = new Label
            {
                Text = "შეიყვანეთ მეპინგის სახელი:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var txtName = new TextBox
            {
                Location = new Point(20, 45),
                Width = 340,
                Text = $"Mapping_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            var btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 80),
                Width = 75
            };

            var btnCancel = new Button
            {
                Text = "გააუქმეთ",
                DialogResult = DialogResult.Cancel,
                Location = new Point(285, 80),
                Width = 75
            };

            inputForm.Controls.Add(lblPrompt);
            inputForm.Controls.Add(txtName);
            inputForm.Controls.Add(btnOK);
            inputForm.Controls.Add(btnCancel);
            inputForm.AcceptButton = btnOK;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(txtName.Text))
                return;

            var presetName = txtName.Text.Trim();

            try
            {
                var mappingsDirectory = Path.Combine(System.Windows.Forms.Application.StartupPath, "ImportMappings");
                if (!Directory.Exists(mappingsDirectory))
                {
                    Directory.CreateDirectory(mappingsDirectory);
                }

                var preset = new ImportMappingPreset
                {
                    Name = presetName,
                    CreatedAt = DateTime.Now,
                    LastUsed = DateTime.Now,
                    Configuration = _mappingConfiguration,
                    FileNamePattern = !string.IsNullOrEmpty(_filePath) ? Path.GetFileName(_filePath) : null
                };

                var fileName = $"{presetName}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(mappingsDirectory, fileName);
                var json = JsonConvert.SerializeObject(preset, Formatting.Indented);
                File.WriteAllText(filePath, json);

                MessageBox.Show($"მეპინგი წარმატებით შენახულია: {presetName}", "წარმატება",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                inputForm.Dispose();
                LoadSavedMappings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"მეპინგის შენახვის შეცდომა: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnLoadMapping_Click(object sender, EventArgs e)
        {
            if (_cmbSavedMappings.SelectedItem == null ||
                _cmbSavedMappings.SelectedIndex == 0)
            {
                MessageBox.Show("გთხოვთ აირჩიოთ შენახული მეპინგი.", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dynamic selectedItem = _cmbSavedMappings.SelectedItem;
                var filePath = selectedItem.FilePath;

                var json = File.ReadAllText(filePath);
                var preset = JsonConvert.DeserializeObject<ImportMappingPreset>(json);

                if (preset?.Configuration == null)
                {
                    MessageBox.Show("მეპინგის ფაილი დაზიანებულია.", "შეცდომა",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Save mapping configuration BEFORE reloading file (so SetupColumnMappingGridAsync can use it)
                _mappingConfiguration = preset.Configuration;
                _chkIsActive.Checked = preset.Configuration.IsActive;

                // Update UI with loaded mapping
                if (!string.IsNullOrEmpty(_filePath) && _mappingConfiguration.SheetToGroupMapping.Any())
                {
                    // Reload file to apply mapping (this will use saved _mappingConfiguration for column mapping)
                    await LoadExcelFileAsync();

                    // Update Sheet-to-Group mapping comboboxes
                    foreach (var kvp in _mappingConfiguration.SheetToGroupMapping)
                    {
                        if (_sheetGroupComboBoxes.TryGetValue(kvp.Key, out var comboBox))
                        {
                            var groupName = _groupNameToIdMap.FirstOrDefault(g => g.Value == kvp.Value).Key;
                            if (groupName != null)
                            {
                                var index = comboBox.Items.IndexOf(groupName);
                                if (index >= 0)
                                    comboBox.SelectedIndex = index;
                            }
                        }
                    }
                }

                // Update LastUsed
                preset.LastUsed = DateTime.Now;
                var updatedJson = JsonConvert.SerializeObject(preset, Formatting.Indented);
                File.WriteAllText(filePath, updatedJson);

                MessageBox.Show($"მეპინგი დატვირთულია: {preset.Name}", "წარმატება",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"მეპინგის დატვირთვის შეცდომა: {ex.Message}", "შეცდომა",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Cleanup

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _fileStream?.Dispose();
            base.OnFormClosing(e);
        }

        #endregion
    }
}
