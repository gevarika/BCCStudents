using System.Data;
using System.Text.RegularExpressions;

namespace BCCStudents.Presentation
{
    public class LogViewerForm : Form
    {
        private DataGridView dgvLogs;
        private ComboBox cmbLogFiles;
        private TextBox txtUser, txtStudentCode, txtOperation;
        private DateTimePicker dtFrom, dtTo;
        private Button btnFilter, btnClear;
        private List<LogEntry> allLogs = new List<LogEntry>();
        private string currentLogFile = "students_log.txt";
        private ComboBox cmbStatus, cmbOperation, cmbUser;

        public LogViewerForm()
        {
            this.Text = "სტუდენტების ლოგების ნახვა";
            this.Width = 1100;
            this.Height = 700;
            InitializeComponents();
            LoadLogs();
        }

        private void SetPlaceholder(TextBox tb, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = placeholder;
                tb.ForeColor = System.Drawing.Color.Gray;
            }
        }
        private void RemovePlaceholder(TextBox tb, string placeholder)
        {
            if (tb.Text == placeholder)
            {
                tb.Text = "";
                tb.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void InitializeComponents()
        {
            dgvLogs = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            cmbLogFiles = new ComboBox { Width = 180, Left = 10, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbUser = new ComboBox { Width = 120, Left = 200, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            txtStudentCode = new TextBox { Width = 120, Left = 330, Top = 10 };
            cmbOperation = new ComboBox { Width = 140, Left = 460, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            dtFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Left = 610, Top = 10 };
            dtTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Left = 730, Top = 10 };
            cmbStatus = new ComboBox { Width = 120, Left = 850, Top = 10, DropDownStyle = ComboBoxStyle.DropDownList };
            btnFilter = new Button { Text = "გაფილტრე", Left = 980, Top = 10, Width = 90 };
            btnClear = new Button { Text = "გასუფთავება", Left = 1080, Top = 10, Width = 100 };

            cmbUser.Items.Add("ყველა მომხმარებელი");
            cmbUser.SelectedIndex = 0;
            cmbUser.SelectedIndexChanged += (s, e) => ApplyFilter();
            cmbOperation.Items.Add("ყველა ოპერაცია");
            cmbOperation.SelectedIndex = 0;
            cmbOperation.SelectedIndexChanged += (s, e) => ApplyFilter();
            cmbStatus.Items.Add("ყველა სტატუსი");
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += (s, e) => ApplyFilter();

            btnFilter.Click += (s, e) => ApplyFilter();
            btnClear.Click += (s, e) => { cmbUser.SelectedIndex = 0; txtStudentCode.Text = ""; cmbOperation.SelectedIndex = 0; dtFrom.Value = DateTime.Today.AddMonths(-1); dtTo.Value = DateTime.Today; cmbStatus.SelectedIndex = 0; ApplyFilter(); };
            cmbLogFiles.SelectedIndexChanged += (s, e) => { currentLogFile = cmbLogFiles.SelectedItem.ToString(); LoadLogs(); };

            var panel = new Panel { Height = 45, Dock = DockStyle.Top, AutoScroll = true };
            panel.Controls.AddRange(new Control[] { cmbLogFiles, cmbUser, txtStudentCode, cmbOperation, dtFrom, dtTo, cmbStatus, btnFilter, btnClear });
            this.Controls.Add(dgvLogs);
            this.Controls.Add(panel);
            LoadLogFiles();
        }

        private void LoadLogFiles()
        {
            string logDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
            var files = Directory.GetFiles(logDir, "*.txt").Select(f => Path.GetFileName(f)).ToList();
            cmbLogFiles.Items.Clear();
            foreach (var file in files) cmbLogFiles.Items.Add(file);
            if (cmbLogFiles.Items.Count > 0)
            {
                if (cmbLogFiles.Items.Contains(currentLogFile))
                    cmbLogFiles.SelectedItem = currentLogFile;
                else
                    cmbLogFiles.SelectedIndex = 0;
            }
        }

        private bool IsAnyFilterActive()
        {
            bool userActive = cmbUser != null && cmbUser.SelectedIndex > 0;
            bool codeActive = !string.IsNullOrWhiteSpace(txtStudentCode.Text);
            bool opActive = cmbOperation != null && cmbOperation.SelectedIndex > 0;
            bool statusActive = cmbStatus != null && cmbStatus.SelectedIndex > 0;

            // შევამოწმოთ არის თუ არა allLogs ცარიელი
            bool dateActive = false;
            if (allLogs.Any())
            {
                dateActive = dtFrom.Value.Date > allLogs.Min(l => l.Date.Date) || dtTo.Value.Date < allLogs.Max(l => l.Date.Date);
            }

            return userActive || codeActive || opActive || statusActive || dateActive;
        }

        private void LoadLogs()
        {
            allLogs.Clear();
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs", currentLogFile);
            if (!File.Exists(logPath)) { dgvLogs.DataSource = null; return; }
            var lines = File.ReadAllLines(logPath);
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
                        Details = ""
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

            // Populate status ComboBox
            var statuses = allLogs.Select(l => l.Status).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("ყველა სტატუსი");
            foreach (var status in statuses) cmbStatus.Items.Add(status);
            cmbStatus.SelectedIndex = 0;

            // Populate user ComboBox
            var users = allLogs.Select(l => l.User).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().OrderBy(u => u).ToList();
            cmbUser.Items.Clear();
            cmbUser.Items.Add("ყველა მომხმარებელი");
            foreach (var user in users) cmbUser.Items.Add(user);
            cmbUser.SelectedIndex = 0;

            // Populate operation ComboBox
            var operations = allLogs.Select(l => l.Operation).Where(o => !string.IsNullOrWhiteSpace(o)).Distinct().OrderBy(o => o).ToList();
            cmbOperation.Items.Clear();
            cmbOperation.Items.Add("ყველა ოპერაცია");
            foreach (var op in operations) cmbOperation.Items.Add(op);
            cmbOperation.SelectedIndex = 0;

            // Set date range pickers to cover all log entries
            if (allLogs.Count > 0)
            {
                dtFrom.Value = allLogs.Min(l => l.Date.Date);
                dtTo.Value = allLogs.Max(l => l.Date.Date);
            }
            else
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
            }

            // თუ არცერთი ფილტრი არ არის აქტიური, აჩვენე ყველა ჩანაწერი
            if (!IsAnyFilterActive())
                dgvLogs.DataSource = allLogs.Select(l => new { l.Date, l.Operation, l.User, l.Status, l.Details }).ToList();
            else
                ApplyFilter();
        }

        private void ApplyFilter()
        {
            string selectedUser = cmbUser.SelectedIndex == 0 ? null : (cmbUser.SelectedItem?.ToString() ?? "").Trim();
            string selectedOperation = cmbOperation.SelectedIndex == 0 ? null : (cmbOperation.SelectedItem?.ToString() ?? "").Trim();
            string selectedStatus = cmbStatus.SelectedIndex == 0 ? null : (cmbStatus.SelectedItem?.ToString() ?? "").Trim();

            var filtered = allLogs.Where(l =>
                (selectedUser == null || string.Equals(l.User?.Trim(), selectedUser, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(txtStudentCode.Text) || l.Details.IndexOf(txtStudentCode.Text, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (selectedOperation == null || string.Equals(l.Operation?.Trim(), selectedOperation, StringComparison.OrdinalIgnoreCase)) &&
                l.Date.Date >= dtFrom.Value.Date && l.Date.Date <= dtTo.Value.Date &&
                (selectedStatus == null || string.Equals(l.Status?.Trim(), selectedStatus, StringComparison.OrdinalIgnoreCase))
            ).ToList();
            dgvLogs.DataSource = filtered.Select(l => new { l.Date, l.Operation, l.User, l.Status, l.Details }).ToList();
        }

        private class LogEntry
        {
            public DateTime Date { get; set; }
            public string Operation { get; set; }
            public string User { get; set; }
            public string Status { get; set; }
            public string Details { get; set; }
        }
    }
}
