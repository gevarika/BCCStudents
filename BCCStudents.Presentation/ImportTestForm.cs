using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Interfaces;
using BCCStudents.TestHelpers;
using System.IO;
using System.Linq; // Added for .First()

namespace BCCStudents.Presentation
{
    public partial class ImportTestForm : Form
    {
        private readonly ImportTestHelper _testHelper;
        private readonly IImportService _importService;
        private readonly IGroupRepository _groupRepository;
        private readonly ISubGroupRepository _subGroupRepository;
        
        public ImportTestForm(IImportService importService, IGroupRepository groupRepository, ISubGroupRepository subGroupRepository)
        {
            _importService = importService;
            _groupRepository = groupRepository;
            _subGroupRepository = subGroupRepository;
            _testHelper = new ImportTestHelper(importService, groupRepository, subGroupRepository);
            
            InitializeComponent();
            SetupControls();
        }
        
        private void SetupControls()
        {
            // Create controls
            var lblFilePath = new Label { Text = "Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒ–áƒ:", Location = new System.Drawing.Point(10, 20), AutoSize = true };
            var txtFilePath = new TextBox { Location = new System.Drawing.Point(10, 45), Width = 400, Name = "txtFilePath" };
            var btnBrowse = new Button { Text = "Browse", Location = new System.Drawing.Point(420, 43), Width = 80 };
            
            var btnTestExcel = new Button { Text = "áƒ¢áƒ”áƒ¡áƒ¢áƒ˜ Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡", Location = new System.Drawing.Point(10, 80), Width = 150 };
            var btnTestDB = new Button { Text = "áƒ¢áƒ”áƒ¡áƒ¢áƒ˜ áƒ‘áƒáƒ–áƒ˜áƒ¡", Location = new System.Drawing.Point(170, 80), Width = 150 };
            var btnTestImport = new Button { Text = "áƒ¢áƒ”áƒ¡áƒ¢áƒ˜ áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡", Location = new System.Drawing.Point(330, 80), Width = 150 };
            
            var txtResults = new TextBox { 
                Location = new System.Drawing.Point(10, 120), 
                Width = 580, 
                Height = 300, 
                Multiline = true, 
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Name = "txtResults"
            };
            
            var btnClear = new Button { Text = "áƒ’áƒáƒ¡áƒ£áƒ¤áƒ—áƒáƒ•áƒ”áƒ‘áƒ", Location = new System.Drawing.Point(10, 430), Width = 100 };
            var btnSave = new Button { Text = "áƒ¨áƒ”áƒœáƒáƒ®áƒ•áƒ", Location = new System.Drawing.Point(120, 430), Width = 100 };
            
            // Add controls to form
            this.Controls.AddRange(new Control[] { 
                lblFilePath, txtFilePath, btnBrowse, 
                btnTestExcel, btnTestDB, btnTestImport, 
                txtResults, btnClear, btnSave 
            });
            
            // Wire up events
            btnBrowse.Click += (s, e) => BrowseFile();
            btnTestExcel.Click += (s, e) => TestExcelFile();
            btnTestDB.Click += (s, e) => TestDatabase();
            btnTestImport.Click += (s, e) => TestImport();
            btnClear.Click += (s, e) => ClearResults();
            btnSave.Click += (s, e) => SaveResults();
            
            // Form properties
            this.Text = "Import Process Test Form";
            this.Size = new System.Drawing.Size(620, 520);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        
        private void BrowseFile()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
                openFileDialog.Title = "áƒáƒ˜áƒ áƒ©áƒ˜áƒ”áƒ— Excel áƒ¤áƒáƒ˜áƒšáƒ˜";
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var txtFilePath = this.Controls["txtFilePath"] as TextBox;
                    txtFilePath.Text = openFileDialog.FileName;
                }
            }
        }
        
        private void TestExcelFile()
        {
            var txtFilePath = this.Controls["txtFilePath"] as TextBox;
            var txtResults = this.Controls["txtResults"] as TextBox;
            
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ— áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— Excel áƒ¤áƒáƒ˜áƒšáƒ˜", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                AppendResult("=== Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ ===");
                AppendResult($"áƒ¤áƒáƒ˜áƒšáƒ˜: {txtFilePath.Text}");
                
                var result = _testHelper.TestExcelFile(txtFilePath.Text);
                
                if (result.Success)
                {
                    AppendResult("Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ!");
                }
                else
                {
                    AppendResult($"Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ: {result.ErrorMessage}");
                }
                
                AppendResult("áƒšáƒáƒ’áƒ˜áƒ¡ áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ”áƒ‘áƒ˜:");
                foreach (var message in result.LogMessages)
                {
                    AppendResult($"  {message}");
                }
                AppendResult("=== Excel áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ ===\n");
            }
            catch (Exception ex)
            {
                AppendResult($"áƒ›áƒáƒ£áƒšáƒáƒ“áƒœáƒ”áƒšáƒ˜ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                AppendResult($"Stack Trace: {ex.StackTrace}");
            }
        }
        
        private void TestDatabase()
        {
            try
            {
                AppendResult("=== áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ ===");
                
                var result = _testHelper.TestDatabaseConnectivity();
                
                if (result.Success)
                {
                    AppendResult("áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ!");
                }
                else
                {
                    AppendResult($"áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ: {result.ErrorMessage}");
                }
                
                AppendResult("áƒšáƒáƒ’áƒ˜áƒ¡ áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ”áƒ‘áƒ˜:");
                foreach (var message in result.LogMessages)
                {
                    AppendResult($"  {message}");
                }
                AppendResult("=== áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ ===\n");
            }
            catch (Exception ex)
            {
                AppendResult($"áƒ›áƒáƒ£áƒšáƒáƒ“áƒœáƒ”áƒšáƒ˜ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                AppendResult($"Stack Trace: {ex.StackTrace}");
            }
        }
        
        private void TestImport()
        {
            var txtFilePath = this.Controls["txtFilePath"] as TextBox;
            
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ— áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— Excel áƒ¤áƒáƒ˜áƒšáƒ˜", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                AppendResult("=== áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ ===");
                AppendResult($"áƒ¤áƒáƒ˜áƒšáƒ˜: {txtFilePath.Text}");
                
                // Get available groups for testing
                var groups = _groupRepository.GetAllGroups();
                if (groups.Count == 0)
                {
                    AppendResult("áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: áƒ¯áƒ’áƒ£áƒ¤áƒ”áƒ‘áƒ˜ áƒáƒ  áƒáƒ áƒ˜áƒ¡ áƒ®áƒ”áƒšáƒ›áƒ˜áƒ¡áƒáƒ¬áƒ•áƒ“áƒáƒ›áƒ˜");
                    return;
                }
                
                // Create a simple sheet to group mapping for testing
                var sheetToGroupIdMap = new Dictionary<string, int>();
                var firstGroup = groups.First();
                sheetToGroupIdMap.Add("Sheet1", firstGroup.Id);
                
                AppendResult($"áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¯áƒ’áƒ£áƒ¤áƒ˜: {firstGroup.Name} (ID: {firstGroup.Id})");
                
                var result = _testHelper.TestImportProcess(txtFilePath.Text, sheetToGroupIdMap);
                
                if (result.Success)
                {
                    AppendResult("áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ!");
                }
                else
                {
                    AppendResult($"áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ: {result.ErrorMessage}");
                }
                
                AppendResult("áƒšáƒáƒ’áƒ˜áƒ¡ áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ”áƒ‘áƒ˜:");
                foreach (var message in result.LogMessages)
                {
                    AppendResult($"  {message}");
                }
                AppendResult("=== áƒ˜áƒ›áƒžáƒáƒ áƒ¢áƒ˜áƒ¡ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ áƒ”áƒ‘áƒ áƒ“áƒáƒ¡áƒ áƒ£áƒšáƒ“áƒ ===\n");
            }
            catch (Exception ex)
            {
                AppendResult($"áƒ›áƒáƒ£áƒšáƒáƒ“áƒœáƒ”áƒšáƒ˜ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}");
                AppendResult($"Stack Trace: {ex.StackTrace}");
            }
        }
        
        private void AppendResult(string message)
        {
            var txtResults = this.Controls["txtResults"] as TextBox;
            if (txtResults.InvokeRequired)
            {
                txtResults.Invoke(new Action(() => AppendResult(message)));
                return;
            }
            
            txtResults.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtResults.SelectionStart = txtResults.Text.Length;
            txtResults.ScrollToCaret();
        }
        
        private void ClearResults()
        {
            var txtResults = this.Controls["txtResults"] as TextBox;
            txtResults.Clear();
        }
        
        private void SaveResults()
        {
            var txtResults = this.Controls["txtResults"] as TextBox;
            
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "áƒ¨áƒ”áƒ˜áƒœáƒáƒ®áƒ”áƒ— áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ¡ áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ”áƒ‘áƒ˜";
                saveFileDialog.FileName = $"ImportTestResults_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, txtResults.Text);
                        MessageBox.Show($"áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ”áƒ‘áƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ¨áƒ”áƒœáƒáƒ®áƒ£áƒšáƒ˜áƒ: {saveFileDialog.FileName}", "áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"áƒ¨áƒ”áƒ“áƒ”áƒ’áƒ”áƒ‘áƒ˜áƒ¡ áƒ¨áƒ”áƒœáƒáƒ®áƒ•áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ: {ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

