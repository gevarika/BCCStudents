using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Application.Services;
using ClosedXML.Excel;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// PaymentsImportForm-ის დაბრუნების სტატუსი
    /// </summary>
    public enum ImportFormResult
    {
        /// <summary>
        /// ფორმა დაიხურა იმპორტის გარეშე
        /// </summary>
        Cancelled,
        
        /// <summary>
        /// იმპორტი წარმატებით დასრულდა
        /// </summary>
        Success,
        
        /// <summary>
        /// იმპორტი შეცდომით დასრულდა
        /// </summary>
        Failed
    }
    public partial class PaymentsImportForm : Form
    {
        private readonly IExcelPaymentImportService _importService;
        private string _selectedFilePath;
        private DataTable _previewData;
        private readonly string _defaultImportPath;
        private readonly IPaymentDescriptionAnalyzer _descriptionAnalyzer;
        
        /// <summary>
        /// ფორმის დაბრუნების სტატუსი
        /// </summary>
        public ImportFormResult Result { get; private set; } = ImportFormResult.Cancelled;

        public PaymentsImportForm(IExcelPaymentImportService importService, IPaymentDescriptionAnalyzer descriptionAnalyzer)
        {
            InitializeComponent();
            _importService = importService;
            _defaultImportPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Students");
            _descriptionAnalyzer = descriptionAnalyzer;
            
            InitializeForm();
            SetupDataGridView();
            
            // Form closing event handler
            this.FormClosing += PaymentsImportForm_FormClosing;
        }

        /// <summary>
        /// ავტომატური ფაილის აღმოჩენისთვის - ფაილის პათის დაყენება
        /// </summary>
        public void SetSelectedFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("ფაილი არ არსებობს!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _selectedFilePath = filePath;
            txtFilePath.Text = _selectedFilePath;
            btnPreview.Enabled = true;
            btnImport.Enabled = false;
            lblStatus.Text = "ფაილი ავტომატურად არჩეულია. დააჭირეთ 'პრევიუს' ღილაკს";
            lblStatus.ForeColor = Color.Blue;
        }

        private void InitializeForm()
        {
            FormTitleHelper.SetTitle(this, "გადახდების იმპორტი ფაილიდან");
            
            // ფაილის არჩევის ღილაკი
            btnSelectFile.Click += BtnSelectFile_Click;
            
            // იმპორტის ღილაკი
            btnImport.Click += BtnImport_Click;
            
            // პრევიუს ღილაკი
            btnPreview.Click += BtnPreview_Click;
            
            // არჩევის ღილაკები
            btnSelectAll.Click += BtnSelectAll_Click;
            btnDeselectAll.Click += BtnDeselectAll_Click;
            btnSelectValid.Click += BtnSelectValid_Click;
            
            // Add tooltips for selection buttons
            var toolTip = new ToolTip();
            toolTip.SetToolTip(btnSelectAll, "აირჩიეთ ყველა ჩანაწერი იმპორტისთვის");
            toolTip.SetToolTip(btnDeselectAll, "მოახსენეთ ყველა ჩანაწერის არჩევა");
            toolTip.SetToolTip(btnSelectValid, "აირჩიეთ მხოლოდ ის ჩანაწერები, რომლებიც არ საჭიროებენ გადასახედს");
            
            // პროგრეს ბარის ინიციალიზაცია
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Visible = false;
            
            // სტატუს ლეიბლის ინიციალიზაცია
            lblStatus.Text = "მზადაა იმპორტისთვის";
            lblStatus.ForeColor = Color.Gray;
        }

        private void SetupDataGridView()
        {
            dgvPreview.AutoGenerateColumns = false;
            dgvPreview.Columns.Clear();
            
            // Add checkbox column for selection
            var selectColumn = new DataGridViewCheckBoxColumn
            {
                Name = "IsSelected",
                HeaderText = "არჩევა",
                DataPropertyName = "IsSelected",
                Width = 50
            };
            dgvPreview.Columns.Add(selectColumn);
            
            // Add checkbox column for review status
            dgvPreview.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "RequiresReview",
                HeaderText = "გადასახედია",
                DataPropertyName = "RequiresReview",
                Width = 80,
                ReadOnly = true
            });
            
            // Add status column
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "სტატუსი",
                DataPropertyName = "Status",
                Width = 100
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentDate",
                HeaderText = "თარიღი",
                DataPropertyName = "PaymentDate",
                Width = 100
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "თანხა",
                DataPropertyName = "Amount",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PersonalId",
                HeaderText = "პირადი ნომერი",
                DataPropertyName = "PersonalId",
                Width = 120
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PayerName",
                HeaderText = "გადამხდელი",
                DataPropertyName = "PayerName",
                Width = 150
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "აღწერა",
                DataPropertyName = "Description",
                Width = 200
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MatchedStudentName",
                HeaderText = "მოსწავლე",
                DataPropertyName = "MatchedStudentName",
                Width = 150
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MatchedGroupName",
                HeaderText = "ჯგუფი",
                DataPropertyName = "MatchedGroupName",
                Width = 100
            });
            
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AnalysisResult",
                HeaderText = "ანალიზის შედეგი",
                DataPropertyName = "AnalysisResult",
                Width = 200
            });
            
            // Add cell formatting for review status
            dgvPreview.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgvPreview.Columns["RequiresReview"].Index && e.Value != null)
                {
                    var requiresReview = (bool)e.Value;
                    e.CellStyle.BackColor = requiresReview ? Color.LightYellow : Color.White;
                }
            };
            
            // Add cell value changed handler for selection
            dgvPreview.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex == dgvPreview.Columns["IsSelected"].Index)
                {
                    UpdateImportButtonState();
                }
            };
            
            // Add cell click handler for selection (for better UX)
            dgvPreview.CellClick += (s, e) =>
            {
                if (e.ColumnIndex == dgvPreview.Columns["IsSelected"].Index)
                {
                    var row = dgvPreview.Rows[e.RowIndex];
                    var currentValue = (bool)row.Cells["IsSelected"].Value;
                    row.Cells["IsSelected"].Value = !currentValue;
                }
            };
            
            // Add header click handler for select all/none
            dgvPreview.ColumnHeaderMouseClick += (s, e) =>
            {
                if (e.ColumnIndex == dgvPreview.Columns["IsSelected"].Index)
                {
                    bool allSelected = true;
                    foreach (DataGridViewRow row in dgvPreview.Rows)
                    {
                        if (!(bool)row.Cells["IsSelected"].Value)
                        {
                            allSelected = false;
                            break;
                        }
                    }
                    
                    // Toggle selection
                    foreach (DataGridViewRow row in dgvPreview.Rows)
                    {
                        row.Cells["IsSelected"].Value = !allSelected;
                    }
                    
                    UpdateImportButtonState();
                }
            };
        }

        /// <summary>
        /// Form closing event handler - აყენებს Cancelled სტატუსს თუ ფორმა დაიხურა იმპორტის გარეშე
        /// </summary>
        private void PaymentsImportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // თუ Result ჯერ არ არის დაყენებული (ანუ იმპორტი არ მოხდა), დავაყენოთ Cancelled
            if (Result == ImportFormResult.Cancelled)
            {
                // ფორმა დაიხურა იმპორტის გარეშე - სტატუსი უკვე Cancelled-ია
            }
        }

        private void UpdateImportButtonState()
        {
            int selectedCount = 0;
            int totalCount = 0;
            
            if (_previewData != null)
            {
                totalCount = _previewData.Rows.Count;
                foreach (DataRow row in _previewData.Rows)
                {
                    if ((bool)row["IsSelected"])
                    {
                        selectedCount++;
                    }
                }
            }
            
            bool hasSelectedRows = selectedCount > 0;
            
            btnImport.Enabled = hasSelectedRows;
            
            if (totalCount > 0)
            {
                lblStatus.Text = hasSelectedRows ? 
                    $"არჩეულია {selectedCount}/{totalCount} ჩანაწერი იმპორტისთვის" : 
                    $"გთხოვთ, აირჩიოთ ჩანაწერები იმპორტისთვის ({totalCount} ხელმისაწვდომია)";
            }
            else
            {
                lblStatus.Text = hasSelectedRows ? 
                    "არჩეულია იმპორტისთვის" : 
                    "გთხოვთ, აირჩიოთ ჩანაწერები იმპორტისთვის";
            }
            
            lblStatus.ForeColor = hasSelectedRows ? Color.Green : Color.Red;
        }

        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = _defaultImportPath;
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                openFileDialog.Title = "აირჩიეთ გადახდების ფაილი";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedFilePath = openFileDialog.FileName;
                    txtFilePath.Text = _selectedFilePath;
                    btnPreview.Enabled = true;
                    btnImport.Enabled = false;
                    lblStatus.Text = "ფაილი არჩეულია. დააჭირეთ 'პრევიუს' ღილაკს";
                    lblStatus.ForeColor = Color.Blue;
                }
            }
        }

        private async void BtnPreview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
            {
                MessageBox.Show("გთხოვთ, აირჩიოთ სწორი ფაილი!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnPreview.Enabled = false;
                btnImport.Enabled = false;
                btnSelectFile.Enabled = false;
                btnSelectAll.Enabled = false;
                btnDeselectAll.Enabled = false;
                btnSelectValid.Enabled = false;

                _previewData = await LoadExcelPreview(_selectedFilePath);
                dgvPreview.DataSource = _previewData;

                btnImport.Enabled = true;
                btnSelectAll.Enabled = true;
                btnDeselectAll.Enabled = true;
                btnSelectValid.Enabled = true;
                
                // Update the import button state and status
                UpdateImportButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა მონაცემების წინასწარი ნახვის დროს: {ex.Message}", 
                    "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPreview.Enabled = true;
            }
        }

        private async Task<DataTable> LoadExcelPreview(string filePath)
        {
            var dt = new DataTable();
            dt.Columns.Add("RowNumber", typeof(int));
            dt.Columns.Add("IsSelected", typeof(bool));
            dt.Columns.Add("RequiresReview", typeof(bool));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("PaymentDate", typeof(DateTime));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Columns.Add("PersonalId", typeof(long));
            dt.Columns.Add("PayerName", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("MatchedStudentName", typeof(string));
            dt.Columns.Add("MatchedGroupName", typeof(string));
            dt.Columns.Add("AnalysisResult", typeof(string));

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var firstRow = worksheet.FirstRowUsed();
                var lastRow = worksheet.LastRowUsed();

                // დავაყენოთ პროგრეს ბარი
                progressBar.Visible = true;
                progressBar.Value = 0;
                progressBar.Maximum = lastRow.RowNumber() - firstRow.RowNumber();
                lblStatus.Text = "მიმდინარეობს აღწერების ანალიზი...";
                System.Windows.Forms.Application.DoEvents();

                for (int row = firstRow.RowNumber() + 1; row <= lastRow.RowNumber(); row++)
                {
                    try
                    {
                        // სვეტების ინდექსები:
                        // 1 - თარიღი
                        // 3 - შემოსავალი (თანხა)
                        // 5 - მიმღების ს/კ (პირადი ნომერი)
                        // 6 - მიმღების დასახელება
                        // 7 - აღწერა

                        var currentRow = worksheet.Row(row);
                        
                        // დავამატოთ დეტალური ლოგირება თარიღის უჯრისთვის
                        var dateCell = currentRow.Cell(1);
                        
                        DateTime paymentDate;
                        if (dateCell.DataType == XLDataType.DateTime)
                        {
                            paymentDate = dateCell.GetDateTime();
                        }
                        else if (dateCell.DataType == XLDataType.Text)
                        {
                            var dateStr = dateCell.GetString();
                            if (!DateTime.TryParse(dateStr, out paymentDate))
                            {
                                throw new FormatException($"არასწორი თარიღის ფორმატი: {dateStr}");
                            }
                        }
                        else if (dateCell.DataType == XLDataType.Number)
                        {
                            // Excel-ის თარიღის რიცხვითი ფორმატიდან გადაყვანა
                            var excelDate = dateCell.GetDouble();
                            paymentDate = DateTime.FromOADate(excelDate);
                        }
                        else
                        {
                            throw new FormatException($"მოულოდნელი მონაცემის ტიპი თარიღის უჯრაში: {dateCell.DataType}");
                        }

                        // უსაფრთხო კონვერტაცია თანხისთვის
                        decimal amount;
                        var amountStr = currentRow.Cell(3).GetString()?.Trim().Replace("₾", "").Replace(" ", "") ?? "";
                        if (!decimal.TryParse(amountStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amount))
                        {
                            throw new FormatException($"არასწორი ფორმატი თანხისთვის: {amountStr}");
                        }

                        // პირადი ნომრის წაკითხვა
                        long? personalId = null;
                        var personalIdStr = currentRow.Cell(5).GetString()?.Trim() ?? "";
                        if (!string.IsNullOrWhiteSpace(personalIdStr))
                        {
                            if (!long.TryParse(personalIdStr, out long parsedId))
                            {
                                throw new FormatException($"არასწორი ფორმატი პირადი ნომრისთვის: {personalIdStr}");
                            }
                            personalId = parsedId;
                        }

                        var payerName = currentRow.Cell(6).GetString()?.Trim() ?? "";
                        var description = currentRow.Cell(7).GetString()?.Trim() ?? "";

                        var analysis = await _descriptionAnalyzer.AnalyzeDescription(description, personalId);
                        
                        dt.Rows.Add(
                            row, // RowNumber
                            false, // IsSelected
                            analysis.RequiresReview, // RequiresReview - ვიყენებთ ბულეან მნიშვნელობას
                            analysis.RequiresReview ? "გადასახედია" : "მზადაა იმპორტისთვის", // Status
                            paymentDate,
                            amount,
                            personalId,
                            payerName,
                            description,
                            analysis.StudentName,
                            analysis.GroupName,
                            analysis.MatchDetails
                        );

                        // განვაახლოთ პროგრეს ბარი
                        progressBar.Value = row - firstRow.RowNumber();
                        lblStatus.Text = $"მიმდინარეობს აღწერების ანალიზი... {progressBar.Value}/{progressBar.Maximum}";
                        System.Windows.Forms.Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        dt.Rows.Add(
                            row, // RowNumber
                            false, // IsSelected
                            true, // RequiresReview
                            $"შეცდომა: {ex.Message}", // Status
                            DBNull.Value,
                            DBNull.Value,
                            DBNull.Value,
                            DBNull.Value,
                            DBNull.Value,
                            DBNull.Value,
                            DBNull.Value,
                            DBNull.Value
                        );

                        // განვაახლოთ პროგრეს ბარი შეცდომის შემთხვევაშიც
                        progressBar.Value = row - firstRow.RowNumber();
                        lblStatus.Text = $"მიმდინარეობს აღწერების ანალიზი... {progressBar.Value}/{progressBar.Maximum}";
                        System.Windows.Forms.Application.DoEvents();
                    }
                }

                // დავასრულოთ პროგრეს ბარი
                progressBar.Visible = false;
                lblStatus.Text = $"პრევიუს მზადაა. ნაპოვნია {dt.Rows.Count} ჩანაწერი";
            }

            return dt;
        }

        private async void BtnImport_Click(object sender, EventArgs e)
        {
            if (_previewData == null || _previewData.Rows.Count == 0)
            {
                MessageBox.Show("არ არის მონაცემები იმპორტისთვის!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRows = _previewData.AsEnumerable()
                .Where(row => (bool)row["IsSelected"])
                .ToList();

            if (!selectedRows.Any())
            {
                MessageBox.Show("გთხოვთ, აირჩიოთ ჩანაწერები იმპორტისთვის!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnImport.Enabled = false;
                btnPreview.Enabled = false;
                btnSelectFile.Enabled = false;
                progressBar.Visible = true;
                progressBar.Value = 0;
                progressBar.Maximum = selectedRows.Count;

                var importResult = await _importService.ImportSelectedPayments(_selectedFilePath, selectedRows, (current, total) =>
                {
                    progressBar.Value = current;
                    lblStatus.Text = $"იმპორტი მიმდინარეობს... {current}/{total}";
                    System.Windows.Forms.Application.DoEvents();
                });
                
                if (importResult.Success)
                {
                    Result = ImportFormResult.Success;
                    MessageBox.Show($"იმპორტი დასრულდა!\nწარმატებით იმპორტირებული: {importResult.ImportedCount}\nშეცდომებით: {importResult.FailedRows.Count}", 
                        "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    /*if (importResult.FailedRows.Any())
                    {
                        var failedRowsMessage = string.Join("\n", importResult.FailedRows.Select(f => 
                            $"სტრიქონი {f.RowNumber}: {f.Reason}"));
                        MessageBox.Show($"შეცდომებით ჩანაწერები:\n{failedRowsMessage}", 
                            "შეცდომები", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }*/
                }
                else
                {
                    Result = ImportFormResult.Failed;
                    MessageBox.Show($"იმპორტი ვერ მოხერხდა: {importResult.ErrorMessage}", 
                        "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // statusLabel.Text = $"იმპორტის სრული დრო: {sw.Elapsed.TotalSeconds:N1} წამი"; // This line was removed as per the edit hint
            }
            catch (Exception ex)
            {
                Result = ImportFormResult.Failed;
                MessageBox.Show($"შეცდომა იმპორტის დროს: {ex.Message}", 
                    "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnImport.Enabled = true;
                btnPreview.Enabled = true;
                btnSelectFile.Enabled = true;
                progressBar.Visible = false;
                lblStatus.Text = "იმპორტი დასრულდა";
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            if (_previewData != null)
            {
                foreach (DataRow row in _previewData.Rows)
                {
                    row["IsSelected"] = true;
                }
                UpdateImportButtonState();
            }
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            if (_previewData != null)
            {
                foreach (DataRow row in _previewData.Rows)
                {
                    row["IsSelected"] = false;
                }
                UpdateImportButtonState();
            }
        }

        private void BtnSelectValid_Click(object sender, EventArgs e)
        {
            if (_previewData != null)
            {
                foreach (DataRow row in _previewData.Rows)
                {
                    // აირჩიეთ მხოლოდ ის ჩანაწერები, რომლებიც არ საჭიროებენ გადასახედს
                    bool requiresReview = (bool)row["RequiresReview"];
                    row["IsSelected"] = !requiresReview;
                }
                UpdateImportButtonState();
            }
        }
    }
}

