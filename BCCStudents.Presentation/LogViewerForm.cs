using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace BCCStudents.Presentation
{
    public class LogViewerForm : BaseForm
    {
        private readonly IApplicationLogQueryService _queryService;
        private readonly IApplicationLogDeleteService _deleteService;
        private readonly IUserContext _userContext;
        private readonly ILogStorageSettings _logStorageSettings;

        private DataGridView dgvLogs;
        private ComboBox cmbLogSource;
        private ComboBox cmbLogFiles;
        private ComboBox cmbSourceType;
        private ComboBox cmbCategory;
        private ComboBox cmbLevel;
        private TextBox txtSearch;
        private ComboBox cmbUser, cmbOperation, cmbStatus;
        private DateTimePicker dtFrom, dtTo;
        private Button btnFilter, btnResetFilters, btnRefresh, btnDelete;
        private ContextMenuStrip _deleteMenu;
        private List<LogEntry> allLogs = new List<LogEntry>();
        private List<LogEntry> displayedLogs = new List<LogEntry>();
        private string currentLogFile = "students_log.txt";
        private bool _isSerilogLogFile;
        private bool _useDatabaseSource = true;

        private static readonly Regex SerilogAppLineRegex = new(
            @"^\[(?<ts>[^\]]+)\s+(?<level>\w{3})\]\s+\[(?<source>[^\]]*)\]\s+(?<msg>.*)$",
            RegexOptions.Compiled);
        private static readonly Regex SerilogSyncLineRegex = new(
            @"^\[(?<ts>[^\]]+)\]\s+\[(?<level>\w{3})\]\s+(?<msg>.*)$",
            RegexOptions.Compiled);

        public LogViewerForm(
            IApplicationLogQueryService queryService,
            IApplicationLogDeleteService deleteService,
            IUserContext userContext,
            ILogStorageSettings logStorageSettings)
        {
            _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
            _deleteService = deleteService ?? throw new ArgumentNullException(nameof(deleteService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logStorageSettings = logStorageSettings ?? throw new ArgumentNullException(nameof(logStorageSettings));

            this.Text = "ლოგების ნახვა";
            this.Width = 1280;
            this.Height = 700;
            InitializeComponents();
            ApplyStorageModeUi();
            LoadLogs();
        }

        public void ApplyStorageModeUi()
        {
            var dbLabel = _logStorageSettings.IsServer
                ? "ბაზა (სერვერი ApplicationLogs)"
                : "ბაზა (ლოკალური ApplicationLogs)";

            if (cmbLogSource.Items.Count > 0)
                cmbLogSource.Items[0] = dbLabel;

            var allowFileArchive = _logStorageSettings.IsLocal && _userContext.IsAdmin;
            while (cmbLogSource.Items.Count > 1)
                cmbLogSource.Items.RemoveAt(1);

            if (allowFileArchive)
                cmbLogSource.Items.Add("ფაილური არქივი");

            if (cmbLogSource.SelectedIndex < 0 || cmbLogSource.SelectedIndex >= cmbLogSource.Items.Count)
                cmbLogSource.SelectedIndex = 0;

            if (_logStorageSettings.IsServer && cmbLogSource.SelectedIndex != 0)
            {
                cmbLogSource.SelectedIndex = 0;
            }
            else
            {
                OnSourceChanged();
            }
        }

        private void InitializeComponents()
        {
            dgvLogs = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true
            };
            dgvLogs.DataBindingComplete += (_, _) => HideIdColumn();

            cmbLogSource = new ComboBox { Width = 180, Left = 10, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLogSource.Items.Add("ბაზა (ApplicationLogs)");
            if (_userContext.IsAdmin)
                cmbLogSource.Items.Add("ფაილური არქივი");
            cmbLogSource.SelectedIndex = 0;
            cmbLogSource.SelectedIndexChanged += (_, _) => OnSourceChanged();

            cmbLogFiles = new ComboBox { Width = 150, Left = 140, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };
            cmbSourceType = new ComboBox { Width = 100, Left = 140, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategory = new ComboBox { Width = 100, Left = 250, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLevel = new ComboBox { Width = 90, Left = 360, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbUser = new ComboBox { Width = 110, Left = 460, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbOperation = new ComboBox { Width = 120, Left = 580, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            dtFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100, Left = 710, Top = 10 };
            dtTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100, Left = 820, Top = 10 };
            txtSearch = new TextBox { Width = 120, Left = 930, Top = 10, PlaceholderText = "ტექსტი..." };
            btnFilter = new Button { Text = "გაფილტრე", Left = 1060, Top = 10, Width = 80 };
            btnRefresh = new Button { Text = "განახლება", Left = 1150, Top = 10, Width = 90 };
            btnResetFilters = new Button { Text = "ფილტრი", Left = 1250, Top = 10, Width = 70 };
            btnDelete = new Button { Text = "წაშლა ▼", Left = 1330, Top = 10, Width = 90, Visible = _userContext.IsAdmin };
            cmbStatus = new ComboBox { Width = 0, Visible = false };

            if (_userContext.IsAdmin)
            {
                _deleteMenu = new ContextMenuStrip();
                _deleteMenu.Items.Add("მონიშნულის წაშლა", null, (_, _) => DeleteSelectedAsync());
                _deleteMenu.Items.Add("ფილტრის მიხედვით წაშლა", null, (_, _) => DeleteFilteredAsync());
                _deleteMenu.Items.Add("სრული გასუფთავება", null, (_, _) => DeleteAllAsync());
                btnDelete.Click += (_, _) => _deleteMenu.Show(btnDelete, new System.Drawing.Point(0, btnDelete.Height));
            }

            PopulateDbFilterCombos();

            cmbUser.Items.Add("ყველა მომხმარებელი");
            cmbUser.SelectedIndex = 0;
            cmbOperation.Items.Add("ყველა ოპერაცია");
            cmbOperation.SelectedIndex = 0;

            btnFilter.Click += (_, _) => ApplyFilter();
            btnRefresh.Click += (_, _) => LoadLogs();
            btnResetFilters.Click += (_, _) =>
            {
                cmbUser.SelectedIndex = 0;
                cmbOperation.SelectedIndex = 0;
                cmbSourceType.SelectedIndex = 0;
                cmbCategory.SelectedIndex = 0;
                cmbLevel.SelectedIndex = 0;
                txtSearch.Text = "";
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
                ApplyFilter();
            };
            cmbLogFiles.SelectedIndexChanged += (_, _) => { currentLogFile = cmbLogFiles.SelectedItem?.ToString(); LoadLogs(); };

            var panel = new Panel { Height = 45, Dock = DockStyle.Top, AutoScroll = true };
            panel.Controls.AddRange(new Control[]
            {
                cmbLogSource, cmbLogFiles, cmbSourceType, cmbCategory, cmbLevel,
                cmbUser, cmbOperation, dtFrom, dtTo, txtSearch, btnFilter, btnRefresh, btnResetFilters, btnDelete
            });
            this.Controls.Add(dgvLogs);
            this.Controls.Add(panel);

            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;
            LoadLogFiles();
        }

        private void HideIdColumn()
        {
            if (dgvLogs.Columns.Contains("Id"))
                dgvLogs.Columns["Id"].Visible = false;
        }

        private void PopulateDbFilterCombos()
        {
            cmbSourceType.Items.Clear();
            cmbSourceType.Items.Add("ყველა წყარო");
            cmbSourceType.Items.AddRange(new object[] { LogSourceType.Audit, LogSourceType.App, LogSourceType.Sync, LogSourceType.Connection });
            cmbSourceType.SelectedIndex = 0;

            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("ყველა კატეგორია");
            cmbCategory.Items.AddRange(new object[]
            {
                LogCategory.Students, LogCategory.Groups, LogCategory.Payments,
                LogCategory.Import, LogCategory.System, LogCategory.Sms
            });
            cmbCategory.SelectedIndex = 0;

            cmbLevel.Items.Clear();
            cmbLevel.Items.Add("ყველა დონე");
            cmbLevel.Items.AddRange(new object[] { "Information", "Warning", "Error", "Fatal" });
            cmbLevel.SelectedIndex = 0;
        }

        private void OnSourceChanged()
        {
            _useDatabaseSource = cmbLogSource.SelectedIndex == 0;
            cmbLogFiles.Visible = !_useDatabaseSource;
            cmbSourceType.Visible = _useDatabaseSource;
            cmbCategory.Visible = _useDatabaseSource;
            cmbLevel.Visible = _useDatabaseSource;
            LoadLogs();
        }

        private void LoadLogFiles()
        {
            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

            var serilogFiles = Directory.GetFiles(logDir, "app-*.log")
                .Concat(Directory.GetFiles(logDir, "sync-*.log"))
                .OrderByDescending(f => f)
                .Select(Path.GetFileName);

            var auditFiles = Directory.GetFiles(logDir, "*.txt")
                .Select(Path.GetFileName)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

            var files = serilogFiles.Concat(auditFiles).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            cmbLogFiles.Items.Clear();
            foreach (var file in files) cmbLogFiles.Items.Add(file);
            if (cmbLogFiles.Items.Count > 0)
                cmbLogFiles.SelectedIndex = 0;
        }

        private async void LoadLogs()
        {
            allLogs.Clear();

            if (_useDatabaseSource)
            {
                try
                {
                    var entries = await _queryService.GetLogsForCurrentUserAsync(BuildDbFilter());
                    allLogs = entries.Select(MapDbEntry).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ლოგების ჩატვირთვა ვერ მოხერხდა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                _isSerilogLogFile = IsSerilogFileName(currentLogFile);
                string logPath = GetCurrentLogPath();
                if (!File.Exists(logPath)) { dgvLogs.DataSource = null; return; }

                if (_isSerilogLogFile)
                    LoadSerilogLogs(File.ReadAllLines(logPath));
                else
                    LoadAuditLogs(File.ReadAllLines(logPath));
            }

            PopulateFilterCombos();
            BindGrid();
        }

        private ApplicationLogFilter BuildDbFilter()
        {
            return new ApplicationLogFilter
            {
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date.AddDays(1),
                SourceType = cmbSourceType.SelectedIndex > 0 ? cmbSourceType.SelectedItem?.ToString() : null,
                Category = cmbCategory.SelectedIndex > 0 ? cmbCategory.SelectedItem?.ToString() : null,
                Level = cmbLevel.SelectedIndex > 0 ? cmbLevel.SelectedItem?.ToString() : null,
                Username = cmbUser.SelectedIndex > 0 ? cmbUser.SelectedItem?.ToString() : null,
                Operation = cmbOperation.SelectedIndex > 0 ? cmbOperation.SelectedItem?.ToString() : null,
                SearchText = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim()
            };
        }

        private static LogEntry MapDbEntry(ApplicationLogEntry e)
        {
            var details = e.Details;
            if (!string.IsNullOrWhiteSpace(e.Exception))
                details = string.IsNullOrWhiteSpace(details) ? e.Exception : details + "\n" + e.Exception;

            return new LogEntry
            {
                Id = e.Id,
                Date = e.CreatedAt.ToLocalTime(),
                SourceType = e.SourceType,
                Category = e.Category,
                Level = e.Level,
                Operation = e.Operation ?? e.Level,
                User = string.IsNullOrWhiteSpace(e.Username) ? (e.MachineName ?? "-") : e.Username,
                Status = e.Status ?? "-",
                Details = details ?? e.Message ?? "",
                MachineName = e.MachineName
            };
        }

        private void LoadAuditLogs(string[] lines)
        {
            LogEntry current = null;
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"\[(.*?)\] Operation: (.*?) \| User: (.*?) \| Status: (.*?)$");
                if (match.Success)
                {
                    if (current != null) allLogs.Add(current);
                    current = new LogEntry
                    {
                        Date = DateTime.Parse(match.Groups[1].Value),
                        Operation = match.Groups[2].Value.Trim(),
                        User = match.Groups[3].Value.Trim(),
                        Status = match.Groups[4].Value.Trim(),
                        Details = "",
                        SourceType = LogSourceType.Audit
                    };
                }
                else if (current != null)
                {
                    if (line.StartsWith("Details:"))
                        current.Details = line.Substring(8).Trim();
                    else
                        current.Details += "\n" + line.Trim();
                }
            }
            if (current != null) allLogs.Add(current);
        }

        private void LoadSerilogLogs(string[] lines)
        {
            LogEntry current = null;
            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var appMatch = SerilogAppLineRegex.Match(line);
                if (appMatch.Success)
                {
                    if (current != null) allLogs.Add(current);
                    current = CreateSerilogEntry(appMatch.Groups["ts"].Value, appMatch.Groups["level"].Value, appMatch.Groups["source"].Value, appMatch.Groups["msg"].Value);
                    continue;
                }

                var syncMatch = SerilogSyncLineRegex.Match(line);
                if (syncMatch.Success)
                {
                    if (current != null) allLogs.Add(current);
                    current = CreateSerilogEntry(syncMatch.Groups["ts"].Value, syncMatch.Groups["level"].Value, "Sync", syncMatch.Groups["msg"].Value);
                    continue;
                }

                if (current != null)
                    current.Details += (string.IsNullOrEmpty(current.Details) ? "" : "\n") + line;
            }
            if (current != null) allLogs.Add(current);
        }

        private static LogEntry CreateSerilogEntry(string timestamp, string level, string source, string message)
        {
            if (!DateTime.TryParse(timestamp, out var date))
                date = DateTime.MinValue;

            return new LogEntry
            {
                Date = date,
                Operation = level,
                User = string.IsNullOrWhiteSpace(source) ? "-" : source.Trim(),
                Status = "-",
                Details = message,
                SourceType = source == "Sync" ? LogSourceType.Sync : LogSourceType.App,
                Level = level
            };
        }

        private static bool IsSerilogFileName(string fileName) =>
            fileName.EndsWith(".log", StringComparison.OrdinalIgnoreCase);

        private void PopulateFilterCombos()
        {
            var users = allLogs.Select(l => l.User).Where(u => !string.IsNullOrWhiteSpace(u) && u != "-").Distinct().OrderBy(u => u).ToList();
            var selectedUser = cmbUser.SelectedIndex > 0 ? cmbUser.SelectedItem?.ToString() : null;
            cmbUser.Items.Clear();
            cmbUser.Items.Add("ყველა მომხმარებელი");
            foreach (var user in users) cmbUser.Items.Add(user);
            cmbUser.SelectedIndex = 0;
            if (selectedUser != null && cmbUser.Items.Contains(selectedUser))
                cmbUser.SelectedItem = selectedUser;

            var operations = allLogs.Select(l => l.Operation).Where(o => !string.IsNullOrWhiteSpace(o)).Distinct().OrderBy(o => o).ToList();
            var selectedOp = cmbOperation.SelectedIndex > 0 ? cmbOperation.SelectedItem?.ToString() : null;
            cmbOperation.Items.Clear();
            cmbOperation.Items.Add("ყველა ოპერაცია");
            foreach (var op in operations) cmbOperation.Items.Add(op);
            cmbOperation.SelectedIndex = 0;
            if (selectedOp != null && cmbOperation.Items.Contains(selectedOp))
                cmbOperation.SelectedItem = selectedOp;
        }

        private void BindGrid()
        {
            ApplyFilter();
        }

        private List<LogEntry> GetClientFilteredLogs()
        {
            string selectedUser = cmbUser.SelectedIndex == 0 ? null : (cmbUser.SelectedItem?.ToString() ?? "").Trim();
            string selectedOperation = cmbOperation.SelectedIndex == 0 ? null : (cmbOperation.SelectedItem?.ToString() ?? "").Trim();
            string search = txtSearch.Text?.Trim();

            return allLogs.Where(l =>
                (selectedUser == null || string.Equals(l.User?.Trim(), selectedUser, StringComparison.OrdinalIgnoreCase)) &&
                (selectedOperation == null || string.Equals(l.Operation?.Trim(), selectedOperation, StringComparison.OrdinalIgnoreCase)) &&
                l.Date.Date >= dtFrom.Value.Date && l.Date.Date <= dtTo.Value.Date &&
                (string.IsNullOrWhiteSpace(search) ||
                 (l.Details?.IndexOf(search, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                 (l.Operation?.IndexOf(search, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0)
            ).ToList();
        }

        private void ApplyFilter()
        {
            displayedLogs = GetClientFilteredLogs();

            if (_useDatabaseSource)
            {
                dgvLogs.DataSource = displayedLogs.Select(l => new
                {
                    l.Id,
                    l.Date,
                    l.SourceType,
                    l.Category,
                    l.Level,
                    l.Operation,
                    l.User,
                    Machine = l.MachineName,
                    l.Status,
                    l.Details
                }).ToList();
            }
            else
            {
                dgvLogs.DataSource = displayedLogs.Select(l => new
                {
                    l.Id,
                    l.Date,
                    l.Operation,
                    l.User,
                    l.Status,
                    l.Details
                }).ToList();
            }
        }

        private async void DeleteSelectedAsync()
        {
            if (dgvLogs.SelectedRows.Count == 0)
            {
                MessageBox.Show("აირჩიეთ ერთი ან მეტი ჩანაწერი.", "წაშლა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = dgvLogs.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.DataBoundItem)
                .Where(item => item != null)
                .ToList();

            if (selected.Count == 0)
                return;

            var confirm = MessageBox.Show(
                _useDatabaseSource
                    ? $"ნამდვილად გსურთ {selected.Count} ჩანაწერის წაშლა?\nჩანაწერები წაიშლება ლოკალურად და სერვერზე."
                    : $"ნამდვილად გსურთ {selected.Count} ჩანაწერის წაშლა ფაილიდან?",
                "წაშლის დადასტურება",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                if (_useDatabaseSource)
                {
                    var ids = selected
                        .Select(GetRowId)
                        .Where(id => id > 0)
                        .Distinct()
                        .ToList();
                    if (ids.Count == 0)
                        return;

                    var deleted = await _deleteService.DeleteByIdsAsync(ids);
                    MessageBox.Show($"წაიშალა {deleted} ჩანაწერი ლოკალურად და სერვერზე.", "წაშლა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadLogs();
                    return;
                }

                var entriesToDelete = GetSelectedLogEntries(selected);
                if (entriesToDelete.Count == 0)
                    return;

                RemoveFileEntries(entriesToDelete);
                MessageBox.Show($"წაიშალა {entriesToDelete.Count} ჩანაწერი ფაილიდან.", "წაშლა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"წაშლა ვერ მოხერხდა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteFilteredAsync()
        {
            var filtered = GetClientFilteredLogs();
            if (filtered.Count == 0)
            {
                MessageBox.Show("ფილტრს არ შეესაბამება არც ერთი ჩანაწერი.", "წაშლა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"ნამდვილად გსურთ {filtered.Count} ჩანაწერის წაშლა მიმდინარე ფილტრის მიხედვით?\nჩანაწერები წაიშლება ლოკალურად და სერვერზე.",
                "წაშლის დადასტურება",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                if (_useDatabaseSource)
                {
                    var deleted = await _deleteService.DeleteFilteredAsync(BuildDbFilter());
                    MessageBox.Show($"წაიშალა {deleted} ჩანაწერი ლოკალურად და სერვერზე.", "წაშლა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadLogs();
                    return;
                }

                RemoveFileEntries(filtered);
                MessageBox.Show($"წაიშალა {filtered.Count} ჩანაწერი ფაილიდან.", "წაშლა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"წაშლა ვერ მოხერხდა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteAllAsync()
        {
            var target = _useDatabaseSource
                ? "ბაზის ApplicationLogs ცხრილი (ლოკალური და სერვერი)"
                : $"ფაილი \"{currentLogFile}\" (მხოლოდ ლოკალური არქივი)";
            var confirm = MessageBox.Show(
                $"ნამდვილად გსურთ {target}-ის სრული გასუფთავება?\nეს ოპერაცია შეუქცევადია.",
                "სრული გასუფთავება",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            var secondConfirm = MessageBox.Show(
                "დარწმუნებული ხართ? ყველა ჩანაწერი წაიშლება.",
                "საბოლოო დადასტურება",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation);
            if (secondConfirm != DialogResult.Yes)
                return;

            try
            {
                if (_useDatabaseSource)
                {
                    var deleted = await _deleteService.DeleteAllAsync();
                    MessageBox.Show($"წაიშალა {deleted} ჩანაწერი ლოკალურად და სერვერზე.", "სრული გასუფთავება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    File.WriteAllText(GetCurrentLogPath(), string.Empty, Encoding.UTF8);
                    MessageBox.Show("ფაილი სრულად გასუფთავდა.", "სრული გასუფთავება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"წაშლა ვერ მოხერხდა: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<LogEntry> GetSelectedLogEntries(IReadOnlyList<object> selectedRows)
        {
            var result = new List<LogEntry>();
            foreach (var item in selectedRows)
            {
                var date = GetRowProperty<DateTime>(item, "Date");
                var operation = GetRowProperty<string>(item, "Operation");
                var user = GetRowProperty<string>(item, "User");
                var details = GetRowProperty<string>(item, "Details");

                var match = displayedLogs.FirstOrDefault(l =>
                    l.Date == date &&
                    string.Equals(l.Operation, operation, StringComparison.Ordinal) &&
                    string.Equals(l.User, user, StringComparison.Ordinal) &&
                    string.Equals(l.Details, details, StringComparison.Ordinal));

                if (match != null)
                    result.Add(match);
            }

            return result;
        }

        private void RemoveFileEntries(IReadOnlyList<LogEntry> entriesToDelete)
        {
            var deleteKeys = new HashSet<string>(entriesToDelete.Select(EntryKey));
            var remaining = allLogs.Where(l => !deleteKeys.Contains(EntryKey(l))).ToList();
            WriteLogFile(remaining);
        }

        private static string EntryKey(LogEntry entry) =>
            $"{entry.Date:O}|{entry.Operation}|{entry.User}|{entry.Details}";

        private void WriteLogFile(IReadOnlyList<LogEntry> entries)
        {
            var path = GetCurrentLogPath();
            if (_isSerilogLogFile)
            {
                var lines = new List<string>();
                foreach (var entry in entries)
                {
                    var firstLine = entry.Details?.Split('\n')[0] ?? string.Empty;
                    if (entry.SourceType == LogSourceType.Sync)
                        lines.Add($"[{entry.Date:yyyy-MM-dd HH:mm:ss.fff}] [{entry.Level?.PadRight(3)}] {firstLine}");
                    else
                        lines.Add($"[{entry.Date:yyyy-MM-dd HH:mm:ss.fff} {entry.Level?.Substring(0, Math.Min(3, entry.Level?.Length ?? 0))}] [{entry.User}] {firstLine}");

                    if (!string.IsNullOrWhiteSpace(entry.Details) && entry.Details.Contains('\n'))
                    {
                        foreach (var continuation in entry.Details.Split('\n').Skip(1))
                            lines.Add(continuation);
                    }
                }

                File.WriteAllLines(path, lines, Encoding.UTF8);
                return;
            }

            var sb = new StringBuilder();
            foreach (var entry in entries)
            {
                sb.Append($"[{entry.Date:yyyy-MM-dd HH:mm:ss}] Operation: {entry.Operation} | User: {entry.User} | Status: {entry.Status}\n");
                sb.Append($"Details:\n{entry.Details}\n\n");
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private string GetCurrentLogPath() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs", currentLogFile);

        private static long GetRowId(object row)
        {
            var idProperty = row.GetType().GetProperty("Id");
            if (idProperty == null)
                return 0;

            var value = idProperty.GetValue(row);
            return value == null ? 0 : Convert.ToInt64(value);
        }

        private static T GetRowProperty<T>(object row, string propertyName)
        {
            var property = row.GetType().GetProperty(propertyName);
            if (property == null)
                return default;

            var value = property.GetValue(row);
            if (value == null)
                return default;

            return (T)value;
        }

        private class LogEntry
        {
            public long Id { get; set; }
            public DateTime Date { get; set; }
            public string SourceType { get; set; }
            public string Category { get; set; }
            public string Level { get; set; }
            public string Operation { get; set; }
            public string User { get; set; }
            public string Status { get; set; }
            public string Details { get; set; }
            public string MachineName { get; set; }
        }
    }
}
