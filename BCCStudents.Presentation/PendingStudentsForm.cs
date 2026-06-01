using BCCStudents.Application.Services;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Presentation.Services;
using System.Data;
using System.Windows.Forms.VisualStyles;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class PendingStudentsForm : Form
    {
        private readonly IPendingStudentService _pendingService;
        private readonly PendingRegistrationMonitor _pendingRegistrationMonitor;
        private Dictionary<int, PendingStudent> _originalStudents = new Dictionary<int, PendingStudent>();
        private readonly DocumentService _documentService;
        //private Dictionary<string, string> failedAdd = new Dictionary<string, string>();
        public PendingStudentsForm(
            IPendingStudentService pendingService,
            DocumentService documentService,
            PendingRegistrationMonitor pendingRegistrationMonitor)
        {
            InitializeComponent();
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "ონლაინ რეგისტრირებული მოსწავლეები");
            else FormTitleHelper.SetTitle(this, "ონლაინ რეგისტრირებული მოსწავლეები - სატესტო რეჟიმი");
            _pendingService = pendingService;
            _documentService = documentService;
            _pendingRegistrationMonitor = pendingRegistrationMonitor ?? throw new ArgumentNullException(nameof(pendingRegistrationMonitor));
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = false; // თუ არ გინდა დამატება
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            //Events
            this.FormClosing += PendingStudentsForm_FormClosing;
            this.dataGridView1.CellMouseDown += dataGridView1_CellMouseDown;

        }

        private void PendingStudentsForm_Load(object sender, EventArgs e)
        {
            _pendingRegistrationMonitor.PendingCountChanged += PendingRegistrationMonitor_PendingCountChanged;
            LoadPendingStudents();
            UpdateSelectionLabel();
        }

        private void PendingRegistrationMonitor_PendingCountChanged(object sender, int count)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => PendingRegistrationMonitor_PendingCountChanged(sender, count)));
                return;
            }

            if (HasUnsavedChanges())
            {
                return;
            }

            LoadPendingStudents();
        }

        private bool HasUnsavedChanges()
        {
            return dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Any(r => r.DefaultCellStyle.BackColor == Color.LightYellow);
        }

        private void RefreshPendingListAfterLocalChange()
        {
            LoadPendingStudents();
            _pendingRegistrationMonitor.SyncLocalCount();
        }
        private void PendingStudentsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _pendingRegistrationMonitor.PendingCountChanged -= PendingRegistrationMonitor_PendingCountChanged;

            // გადაამოწმოს თუ არის როუები ყვითელი ფერით (შეცვლილი)
            bool hasUnsavedChanges = HasUnsavedChanges();

            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "ცვლილებები არ არის შენახული. გსურთ გასვლა?",
                    "გაფრთხილება",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true; // ბლოკავს დახურვას
                }
            }
        }
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];
            }
        }


        private void btnApprove_Click(object sender, EventArgs e)
        {
            var failedAdd = new List<string>();
            var successAdd = new List<string>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isSelected = Convert.ToBoolean(row.Cells["Select"].Value);
                if (!isSelected) continue;

                // ამოიღე ფასდაკლება იმავე row-დან
                decimal discountAmount = 0;
                if (row.Cells["ApprovalDiscount"].Value != null &&
                    decimal.TryParse(row.Cells["ApprovalDiscount"].Value.ToString(), out decimal parsed))
                {
                    discountAmount = parsed;
                }
                int pendingStudentId = ((PendingStudent)row.DataBoundItem).Id;
                var result = _pendingService.ApproveStudent(pendingStudentId, discountAmount);


                string fullName = $"{row.Cells["FirstName"].Value} {row.Cells["LastName"].Value}";

                if (result.IsSuccess)
                    successAdd.Add(fullName);
                else
                    failedAdd.Add($"{fullName} → {result.Message}");
            }


            // აქ უნდა გამოცხადდეს message ცვლადი 👇
            string message = "";

            if (successAdd.Any())
            {
                message += $"წარმატებით დაემატა: {successAdd.Count}\n";
                message += string.Join("\n", successAdd.Select(name => "> " + name));
                message += "\n\n";
            }

            if (failedAdd.Any())
            {
                message += $"დამატება ვერ მოხერხდა: {failedAdd.Count}\n";
                message += string.Join("\n", failedAdd.Select(reason => "> " + reason));
            }

            if (string.IsNullOrWhiteSpace(message))
                message = "არ არის მონიშნული მოსწავლე.";

            MessageBox.Show(message, "დასტურის შედეგი", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshPendingListAfterLocalChange();
        }
        private void SetupContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var viewDocsItem = new ToolStripMenuItem("📂 დათვალიერება დოკუმენტების");
            viewDocsItem.Click += ViewDocuments_Click;

            var redownloadItem = new ToolStripMenuItem("🔁 გადმოწერა თავიდან");
            redownloadItem.Click += RedownloadDocuments_Click;

            contextMenu.Items.AddRange(new ToolStripItem[] { viewDocsItem, redownloadItem });

            dataGridView1.ContextMenuStrip = contextMenu;
        }

        private void LoadPendingStudents()
        {
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            try
            {
                var list = _pendingService.GetAllPending();
                dataGridView1.DataSource = list;
                EnsureManualColumns();
                ConfigureGridColumns();

                if (list.Count > 0)
                {
                    // შეინახე ორიგინალები Clone-ით
                    _originalStudents = list.ToDictionary(
                        s => s.Id,
                        s => new PendingStudent
                        {
                            Id = s.Id,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            PhoneNumber = s.PhoneNumber,
                            ParentName = s.ParentName,
                            Discount = s.Discount,
                            Age = s.Age,
                            Id_Numb = s.Id_Numb,
                            Address = s.Address,
                            // სხვა ველები
                        });
                    SetupContextMenu();
                }
                else
                {
                    btnApprove.Enabled = false; btnDelete.Enabled = false; btnSaveChanges.Enabled = false; btnDownloadAll.Enabled = false;
                }
            }
            finally
            {
                dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            }
        }

        private void EnsureManualColumns()
        {
            if (!dataGridView1.Columns.Contains("Select"))
            {
                AddCheckboxColumn();
            }

            if (!dataGridView1.Columns.Contains("ApprovalDiscount"))
            {
                AddDiscountColumn();
            }
        }

        private void ConfigureGridColumns()
        {
            var georgianHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["FirstName"] = "სახელი",
                ["LastName"] = "გვარი",
                ["Age"] = "ასაკი",
                ["ParentName"] = "მშობლის სახელი",
                ["PhoneNumber"] = "ტელეფონი",
                ["Id_Numb"] = "პირადი ნომერი",
                ["Address"] = "მისამართი",
                ["CreatedAt"] = "რეგისტრაციის თარიღი"
            };

            var hiddenColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Id",
                "UserId",
                "RegistrationDate",
                "TuitionFee",
                "Discount",
                "Balance",
                "IdCardPath",
                "AdditionalDocsPath"
            };

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (hiddenColumns.Contains(column.Name) || hiddenColumns.Contains(column.DataPropertyName))
                {
                    column.Visible = false;
                    continue;
                }

                if (georgianHeaders.TryGetValue(column.Name, out var header) ||
                    georgianHeaders.TryGetValue(column.DataPropertyName ?? string.Empty, out header))
                {
                    column.HeaderText = header;
                }
            }
        }

        private void AddCheckboxColumn()
        {
            var checkColumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "",
                Name = "Select",
                Width = 30
            };

            var header = new DataGridViewCheckBoxHeaderCell();
            header.OnCheckBoxClicked += (isChecked) =>
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Cells[checkColumn.Index].Value = isChecked;
                }
            };

            checkColumn.HeaderCell = header;
            dataGridView1.Columns.Insert(0, checkColumn);
        }

        private void AddDiscountColumn()
        {
            var discountColumn = new DataGridViewTextBoxColumn
            {
                Name = "ApprovalDiscount",
                HeaderText = "ფასდაკლება",
                ValueType = typeof(decimal),
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            };

            dataGridView1.Columns.Insert(1, discountColumn);
        }
        private void ViewDocuments_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is PendingStudent student)
            {
                _documentService.OpenStudentFolder(student);
            }
        }
        private void RedownloadDocuments_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow?.DataBoundItem is PendingStudent student)
            {
                _documentService.DownloadDocumentsForStudent(student, out List<string> failed);
                _documentService.ShowDownloadSummary(failed, $"📥 დოკუმენტები წარმატებით ჩაიტვირთა სტუდენტისთვის: {student.FirstName} {student.LastName}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                var selected = (PendingStudent)dataGridView1.CurrentRow.DataBoundItem;

                var result = MessageBox.Show("ნამდვილად გსურს წაშლა?", "დასტური", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    _pendingService.Delete(selected.Id);
                    RefreshPendingListAfterLocalChange();
                }
            }
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            int updatedCount = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var student = (PendingStudent)row.DataBoundItem;
                if (!_originalStudents.ContainsKey(student.Id))
                    continue;

                var original = _originalStudents[student.Id];
                var updatedFields = new Dictionary<string, object>();

                void CheckChange(string columnName, object originalValue)
                {
                    var newValue = row.Cells[columnName].Value;
                    if (!Equals(originalValue, newValue))
                    {
                        updatedFields[columnName] = newValue;
                        // მონიშნე წითლად (შეიძლება უკვე მონიშნულია, მაგრამ არაუშავს)
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                }

                CheckChange("FirstName", original.FirstName);
                CheckChange("LastName", original.LastName);
                CheckChange("PhoneNumber", original.PhoneNumber);
                CheckChange("ParentName", original.ParentName);
                CheckChange("Age", original.Age);
                CheckChange("Id_Numb", original.Id_Numb);
                CheckChange("Address", original.Address);
                // დაამატე სხვა ველები რაც გინდა

                if (updatedFields.Count > 0)
                {
                    _pendingService.UpdatePartial(student.Id, updatedFields);
                    updatedCount++;

                    // მარკირება მწვანედ (შენახული წარმატებით)
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }

            if (updatedCount > 0)
            {
                MessageBox.Show($"✅ შენახულია {updatedCount} ჩანაწერის ცვლილება.", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPendingStudents(); // optional: refresh
            }
            else
            {
                MessageBox.Show("ცვლილებები არ დაფიქსირდა.", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                UpdateSelectionLabel();
                return;
            }

            var columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            if (columnName == "ApprovalDiscount" || columnName == "Select")
            {
                return;
            }

            var row = dataGridView1.Rows[e.RowIndex];
            if (row.IsNewRow || row.DataBoundItem is not PendingStudent student)
            {
                return;
            }

            if (!_originalStudents.TryGetValue(student.Id, out var original))
            {
                return;
            }

            bool hasChanges = false;

            void Check(string fieldName, object originalValue)
            {
                if (!dataGridView1.Columns.Contains(fieldName))
                {
                    return;
                }

                var current = row.Cells[fieldName].Value;
                if (!Equals(current, originalValue))
                {
                    hasChanges = true;
                }
            }

            Check("FirstName", original.FirstName);
            Check("LastName", original.LastName);
            Check("PhoneNumber", original.PhoneNumber);
            Check("ParentName", original.ParentName);
            Check("Age", original.Age);
            Check("Id_Numb", original.Id_Numb);
            Check("Address", original.Address);

            row.DefaultCellStyle.BackColor = hasChanges ? Color.LightYellow : Color.White;
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnDownloadAll_Click(object sender, EventArgs e)
        {
            _documentService.LoadConfig();
            if (!DocumentService.IsDownloadFolderConfigured(_documentService.DownloadBaseFolder))
            {
                MessageBox.Show(
                    DocumentService.DownloadFolderNotConfiguredMessage,
                    "შენახვის ადგილი",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!DocumentService.IsFileServerConfigured(_documentService.FileServerBaseUrl))
            {
                MessageBox.Show(
                    DocumentService.FileServerNotConfiguredMessage,
                    "ფაილები სერვერზე",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Task.Run(() =>
            {
                var list = _pendingService.GetAllPending();
                _documentService.DownloadAllPendingDocuments(list, progressBar1, dataGridView1);
            });
        }
        private void UpdateSelectionLabel()
        {
            int totalRows = dataGridView1.Rows.Count;
            int selectedCount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value)) // assuming checkbox is first column
                {
                    selectedCount++;
                }
            }

            lblSelection.Text = $"მონიშნულია {selectedCount} / {totalRows} ჩანაწერიდან";
        }

    }
    class DataGridViewCheckBoxHeaderCell : DataGridViewColumnHeaderCell
    {
        Point checkBoxLocation;
        Size checkBoxSize;
        bool _checked = false;
        Point cellLocation = new Point();
        CheckBoxState state = CheckBoxState.UncheckedNormal;

        public event Action<bool> OnCheckBoxClicked;

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
            int rowIndex, DataGridViewElementStates dataGridViewElementState, object value,
            object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds,
                rowIndex, dataGridViewElementState, value,
                formattedValue, errorText, cellStyle,
                advancedBorderStyle, paintParts);

            Point p = new Point
            {
                X = cellBounds.Location.X + (cellBounds.Width / 2) - 9,
                Y = cellBounds.Location.Y + (cellBounds.Height / 2) - 9
            };

            cellLocation = cellBounds.Location;
            checkBoxLocation = p;
            checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal);
            state = _checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;

            CheckBoxRenderer.DrawCheckBox(graphics, checkBoxLocation, state);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            Point click = new Point(e.X + cellLocation.X, e.Y + cellLocation.Y);

            if (click.X >= checkBoxLocation.X && click.X <= checkBoxLocation.X + checkBoxSize.Width &&
                click.Y >= checkBoxLocation.Y && click.Y <= checkBoxLocation.Y + checkBoxSize.Height)
            {
                _checked = !_checked;
                OnCheckBoxClicked?.Invoke(_checked);
                this.DataGridView.InvalidateCell(this);
            }

            base.OnMouseClick(e);
        }
    }

}

