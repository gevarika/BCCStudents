using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BCCStudents.Application.Services;
using BCCStudents.Application.Services.AutoFileDetection;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Interfaces;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// ავტომატური იმპორტის შეტყობინების ფორმა
    /// </summary>
    public partial class AutoImportNotificationForm : Form
    {
        private readonly List<DetectedFile> _newFiles;
        private readonly AutoFileDetectionService _detectionService;
        private readonly IExcelPaymentImportService _importService;
        private readonly IPaymentDescriptionAnalyzer _descriptionAnalyzer;
        
        public List<DetectedFile> SelectedFiles { get; private set; }
        public bool ShouldOpenImportForm { get; private set; } = false;

        public AutoImportNotificationForm(
            List<DetectedFile> newFiles,
            AutoFileDetectionService detectionService,
            IExcelPaymentImportService importService,
            IPaymentDescriptionAnalyzer descriptionAnalyzer)
        {
            _newFiles = newFiles;
            _detectionService = detectionService;
            _importService = importService;
            _descriptionAnalyzer = descriptionAnalyzer;
            SelectedFiles = new List<DetectedFile>();
            
            InitializeComponent();
            SetupForm();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "ახალი გადახდების ფაილები აღმოჩენილია";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void SetupForm()
        {
            // მთავარი ლეიბლი
            var lblTitle = new Label
            {
                Text = $"აღმოჩენილია {_newFiles.Count} ახალი გადახდების ფაილი:",
                Location = new Point(20, 20),
                Size = new Size(550, 30),
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblTitle);

            // ფაილების სია
            var listBox = new CheckedListBox
            {
                Location = new Point(20, 60),
                Size = new Size(550, 200),
                CheckOnClick = true
            };

            foreach (var file in _newFiles)
            {
                var displayText = $"{file.FileName} ({FormatFileSize(file.FileSize)}) - {file.ModifiedAt:dd.MM.yyyy HH:mm}";
                listBox.Items.Add(displayText, true); // ყველა ფაილი არჩეულია ნაგულისხმევად
            }
            this.Controls.Add(listBox);

            // ღილაკები
            var btnSelectAll = new Button
            {
                Text = "ყველა",
                Location = new Point(20, 280),
                Size = new Size(80, 30)
            };
            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                    listBox.SetItemChecked(i, true);
            };
            this.Controls.Add(btnSelectAll);

            var btnDeselectAll = new Button
            {
                Text = "არც ერთი",
                Location = new Point(110, 280),
                Size = new Size(80, 30)
            };
            btnDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                    listBox.SetItemChecked(i, false);
            };
            this.Controls.Add(btnDeselectAll);

            var btnImport = new Button
            {
                Text = "იმპორტი",
                Location = new Point(400, 280),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            btnImport.Click += (s, e) =>
            {
                SelectedFiles.Clear();
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    if (listBox.GetItemChecked(i))
                    {
                        SelectedFiles.Add(_newFiles[i]);
                    }
                }
                
                // თუ ფაილები არჩეულია, გადავიდეთ PaymentsImportForm-ზე
                if (SelectedFiles.Count > 0)
                {
                    ShouldOpenImportForm = true;
                }
            };
            this.Controls.Add(btnImport);

            var btnCancel = new Button
            {
                Text = "გაუქმება",
                Location = new Point(490, 280),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            // ჩეკბოქსი "აღარ აჩვენო"
            var chkDontShowAgain = new CheckBox
            {
                Text = "აღარ აჩვენო ავტომატური შეტყობინებები",
                Location = new Point(20, 320),
                Size = new Size(300, 20)
            };
            this.Controls.Add(chkDontShowAgain);

            // ნაგულისხმევი ღილაკი
            this.AcceptButton = btnImport;
            this.CancelButton = btnCancel;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

