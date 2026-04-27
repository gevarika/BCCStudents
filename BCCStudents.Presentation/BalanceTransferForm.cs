using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using System.Globalization;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class BalanceTransferForm : Form
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly ILoggerRepository _loggerRepository;
        private readonly IUserContext _userContext;
        private readonly IUpStreamChangeTracker _upStreamChangeTracker;

        private ComboBox cmbFromStudent;
        private ComboBox cmbToStudent;
        private Label lblFromBalanceValue;
        private Label lblToBalanceValue;
        private TextBox txtAmount;
        private RadioButton rdoManual;
        private RadioButton rdoHalf;
        private RadioButton rdoFull;
        private Button btnTransfer;
        private Button btnClose;

        private readonly List<StudentOption> _fromStudents = new List<StudentOption>();
        private readonly List<StudentOption> _toStudents = new List<StudentOption>();

        public BalanceTransferForm(
            IStudentRepository studentRepository,
            IBalanceRepository balanceRepository,
            ILoggerRepository loggerRepository,
            IUserContext userContext,
            IUpStreamChangeTracker upStreamChangeTracker)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _loggerRepository = loggerRepository ?? throw new ArgumentNullException(nameof(loggerRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _upStreamChangeTracker = upStreamChangeTracker ?? throw new ArgumentNullException(nameof(upStreamChangeTracker));

            InitializeComponent();
            FormTitleHelper.SetTitle(this, "ბალანსის გადატანა");
            LoadStudents();
        }

        private void InitializeComponent()
        {
            Size = new Size(640, 360);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Padding = new Padding(12),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            var lblFrom = new Label { Text = "გამგზავნი მოსწავლე", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            cmbFromStudent = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFromStudent.SelectedIndexChanged += CmbFromStudent_SelectedIndexChanged;

            var lblBalance = new Label { Text = "ბალანსი", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            lblFromBalanceValue = new Label { Text = "-", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };

            var lblTo = new Label { Text = "მიმღები მოსწავლე", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            cmbToStudent = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbToStudent.SelectedIndexChanged += CmbToStudent_SelectedIndexChanged;

            var lblToBalance = new Label { Text = "მიმღების ბალანსი", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            lblToBalanceValue = new Label { Text = "-", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };

            var lblType = new Label { Text = "გადატანის ტიპი", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            var typePanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            rdoManual = new RadioButton { Text = "ხელით", Checked = true, AutoSize = true };
            rdoHalf = new RadioButton { Text = "ნახევარი", AutoSize = true };
            rdoFull = new RadioButton { Text = "სრული", AutoSize = true };
            rdoManual.CheckedChanged += TransferTypeChanged;
            rdoHalf.CheckedChanged += TransferTypeChanged;
            rdoFull.CheckedChanged += TransferTypeChanged;
            typePanel.Controls.AddRange(new Control[] { rdoManual, rdoHalf, rdoFull });

            var lblAmount = new Label { Text = "თანხა (₾)", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            txtAmount = new TextBox { Dock = DockStyle.Fill };
            txtAmount.TextChanged += AmountChanged;

            var buttonsPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            btnTransfer = new Button { Text = "გადატანა", AutoSize = true };
            btnClose = new Button { Text = "დახურვა", AutoSize = true };
            btnTransfer.Click += BtnTransfer_Click;
            btnClose.Click += (s, e) => Close();
            buttonsPanel.Controls.AddRange(new Control[] { btnTransfer, btnClose });

            layout.Controls.Add(lblFrom, 0, 0);
            layout.Controls.Add(cmbFromStudent, 1, 0);
            layout.Controls.Add(lblBalance, 0, 1);
            layout.Controls.Add(lblFromBalanceValue, 1, 1);
            layout.Controls.Add(lblTo, 0, 2);
            layout.Controls.Add(cmbToStudent, 1, 2);
            layout.Controls.Add(lblToBalance, 0, 3);
            layout.Controls.Add(lblToBalanceValue, 1, 3);
            layout.Controls.Add(lblType, 0, 4);
            layout.Controls.Add(typePanel, 1, 4);
            layout.Controls.Add(lblAmount, 0, 5);
            layout.Controls.Add(txtAmount, 1, 5);
            layout.Controls.Add(buttonsPanel, 1, 6);

            Controls.Add(layout);
        }

        private void LoadStudents()
        {
            _fromStudents.Clear();
            _toStudents.Clear();
            var studentDtos = _studentRepository.GetAllStudentsSomeInfo();
            foreach (var student in studentDtos)
            {
                var display = $"{student.LastName} {student.FirstName}";
                if (!string.IsNullOrWhiteSpace(student.StudentCode))
                {
                    display += $" ({student.StudentCode})";
                }

                var option = new StudentOption
                {
                    Id = student.Id,
                    Display = display,
                    Balance = student.Balance
                };

                if (student.Balance > 0)
                {
                    _fromStudents.Add(option);
                }

                _toStudents.Add(option);
            }

            cmbFromStudent.Items.Clear();
            cmbToStudent.Items.Clear();
            cmbFromStudent.Items.AddRange(_fromStudents.ToArray());
            cmbToStudent.Items.AddRange(_toStudents.ToArray());

            UpdateSenderBalanceDisplay();
            UpdateReceiverBalanceDisplay();
            UpdateTransferButtonState();
        }

        private void CmbFromStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSenderBalanceDisplay();
            UpdateAmountFromQuickOption();
            UpdateTransferButtonState();
        }

        private void CmbToStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReceiverBalanceDisplay();
            UpdateTransferButtonState();
        }

        private void TransferTypeChanged(object sender, EventArgs e)
        {
            txtAmount.ReadOnly = !rdoManual.Checked;
            UpdateAmountFromQuickOption();
            UpdateTransferButtonState();
        }

        private void UpdateAmountFromQuickOption()
        {
            if (rdoManual.Checked)
            {
                return;
            }

            var sender = cmbFromStudent.SelectedItem as StudentOption;
            var balance = sender?.Balance ?? 0m;

            if (rdoHalf.Checked)
            {
                txtAmount.Text = Math.Round(balance / 2m, 2, MidpointRounding.AwayFromZero).ToString("0.##", CultureInfo.CurrentCulture);
            }
            else if (rdoFull.Checked)
            {
                txtAmount.Text = balance.ToString("0.##", CultureInfo.CurrentCulture);
            }

            UpdateTransferButtonState();
        }

        private void UpdateSenderBalanceDisplay()
        {
            var sender = cmbFromStudent.SelectedItem as StudentOption;
            lblFromBalanceValue.Text = sender == null
                ? "-"
                : sender.Balance.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private void UpdateReceiverBalanceDisplay()
        {
            var receiver = cmbToStudent.SelectedItem as StudentOption;
            lblToBalanceValue.Text = receiver == null
                ? "-"
                : receiver.Balance.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private void AmountChanged(object sender, EventArgs e)
        {
            UpdateTransferButtonState();
        }

        private bool TryGetTransferAmount(out decimal amount)
        {
            return decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out amount) && amount > 0;
        }

        private void UpdateTransferButtonState()
        {
            var sender = cmbFromStudent.SelectedItem as StudentOption;
            var receiver = cmbToStudent.SelectedItem as StudentOption;
            if (sender == null || receiver == null || sender.Id == receiver.Id)
            {
                btnTransfer.Enabled = false;
                return;
            }

            if (!TryGetTransferAmount(out var amount))
            {
                btnTransfer.Enabled = false;
                return;
            }

            btnTransfer.Enabled = amount <= sender.Balance;
        }

        private async void BtnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                var senderOption = cmbFromStudent.SelectedItem as StudentOption;
                var receiverOption = cmbToStudent.SelectedItem as StudentOption;

                if (senderOption == null || receiverOption == null)
                {
                    MessageBox.Show("აირჩიეთ გამგზავნი და მიმღები მოსწავლე.", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (senderOption.Id == receiverOption.Id)
                {
                    MessageBox.Show("გამგზავნი და მიმღები ერთნაირი ვერ იქნება.", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) || amount <= 0)
                {
                    MessageBox.Show("შეიყვანეთ სწორი თანხა.", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (amount > senderOption.Balance)
                {
                    MessageBox.Show("გამგზავნის ბალანსი არასაკმარისია.", "გაფრთხილება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var success = _balanceRepository.TransferBalance(senderOption.Id, receiverOption.Id, amount);
                if (!success)
                {
                    var failDetails = BuildLogDetails(senderOption, receiverOption, amount);
                    LogBalanceTransfer("Failed", failDetails);
                    MessageBox.Show("გადატანა ვერ შესრულდა. გადაამოწმეთ მონაცემები.", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                senderOption.Balance -= amount;
                receiverOption.Balance += amount;
                UpdateSenderBalanceDisplay();
                UpdateReceiverBalanceDisplay();
                UpdateAmountFromQuickOption();
                UpdateTransferButtonState();

                var details = BuildLogDetails(senderOption, receiverOption, amount);
                LogBalanceTransfer("Success", details);
                await TrackBalanceTransferSyncAsync(senderOption.Id, receiverOption.Id);

                MessageBox.Show("თანხა წარმატებით გადაიტანა.", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                var senderOption = cmbFromStudent.SelectedItem as StudentOption;
                var receiverOption = cmbToStudent.SelectedItem as StudentOption;
                var details = BuildLogDetails(senderOption, receiverOption, 0m) + $" | Error: {ex.Message}";
                LogBalanceTransfer("Error", details);
                MessageBox.Show($"შეცდომა გადატანისას: {ex.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildLogDetails(StudentOption sender, StudentOption receiver, decimal amount)
        {
            var fromText = sender == null ? "-" : $"{sender.Display} (ID={sender.Id})";
            var toText = receiver == null ? "-" : $"{receiver.Display} (ID={receiver.Id})";
            return $"From: {fromText} | To: {toText} | Amount: {amount:0.##}";
        }

        private void LogBalanceTransfer(string status, string details)
        {
            var user = string.IsNullOrWhiteSpace(_userContext?.Username) ? "System" : _userContext.Username;
            _loggerRepository.LogPaymentAction("BalanceTransfer", status, details, user);
            WriteBalanceTransferFile("BalanceTransfer", status, details, user);
        }

        private async System.Threading.Tasks.Task TrackBalanceTransferSyncAsync(int fromStudentId, int toStudentId)
        {
            try
            {
                var fromStudent = _studentRepository.GetStudentById(fromStudentId);
                if (fromStudent != null)
                {
                    await _upStreamChangeTracker.TrackStudentChangeAsync(fromStudentId, SyncOperationType.Update, fromStudent);
                }

                var toStudent = _studentRepository.GetStudentById(toStudentId);
                if (toStudent != null)
                {
                    await _upStreamChangeTracker.TrackStudentChangeAsync(toStudentId, SyncOperationType.Update, toStudent);
                }
            }
            catch (Exception ex)
            {
                _loggerRepository.LogPaymentAction(
                    "BalanceTransferSync",
                    "Error",
                    $"Sync failed: {ex.Message}",
                    string.IsNullOrWhiteSpace(_userContext?.Username) ? "System" : _userContext.Username);
            }
        }

        private void WriteBalanceTransferFile(string operationType, string status, string details, string user)
        {
            try
            {
                var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
                Directory.CreateDirectory(basePath);
                var filePath = Path.Combine(basePath, "balance_transfers_log.txt");
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Operation: {operationType} | User: {user} | Status: {status}\nDetails:\n{details}\n\n";
                File.AppendAllText(filePath, logEntry);
            }
            catch
            {
                // ignore log file errors
            }
        }

        private class StudentOption
        {
            public int Id { get; set; }
            public string Display { get; set; }
            public decimal Balance { get; set; }

            public override string ToString()
            {
                return Display;
            }
        }
    }
}
