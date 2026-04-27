namespace BCCStudents.Presentation
{
    partial class ImportFormV2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFormV2));
            _btnSelectFile = new Button();
            _lblFileName = new Label();
            pnlFileSelection = new Panel();
            pnlOptions = new Panel();
            _chkEnableMapping = new CheckBox();
            _chkIsActive = new CheckBox();
            _btnSaveMapping = new Button();
            _btnLoadMapping = new Button();
            _cmbSavedMappings = new ComboBox();
            _splitContainerMain = new SplitContainer();
            _pnlMapping = new Panel();
            splitContainerMapping = new SplitContainer();
            pnlSheetMapping = new Panel();
            flowLayoutSheetMapping = new FlowLayoutPanel();
            pnlColumnMapping = new Panel();
            _tabControlSheets = new TabControl();
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel();
            pnlProgress = new Panel();
            _progressBar = new ProgressBar();
            _lblStatus = new Label();
            _btnImport = new Button();
            pnlButton = new Panel();
            pnlFileSelection.SuspendLayout();
            pnlOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_splitContainerMain).BeginInit();
            _splitContainerMain.Panel1.SuspendLayout();
            _splitContainerMain.Panel2.SuspendLayout();
            _splitContainerMain.SuspendLayout();
            _pnlMapping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMapping).BeginInit();
            splitContainerMapping.Panel1.SuspendLayout();
            splitContainerMapping.Panel2.SuspendLayout();
            splitContainerMapping.SuspendLayout();
            pnlSheetMapping.SuspendLayout();
            _statusStrip.SuspendLayout();
            pnlProgress.SuspendLayout();
            pnlButton.SuspendLayout();
            SuspendLayout();
            // 
            // _btnSelectFile
            // 
            _btnSelectFile.Location = new Point(10, 15);
            _btnSelectFile.Name = "_btnSelectFile";
            _btnSelectFile.Size = new Size(150, 30);
            _btnSelectFile.TabIndex = 0;
            _btnSelectFile.Text = "აირჩიეთ Excel ფაილი";
            _btnSelectFile.UseVisualStyleBackColor = true;
            _btnSelectFile.Click += BtnSelectFile_Click;
            // 
            // _lblFileName
            // 
            _lblFileName.AutoSize = true;
            _lblFileName.Location = new Point(170, 20);
            _lblFileName.Name = "_lblFileName";
            _lblFileName.Size = new Size(162, 15);
            _lblFileName.TabIndex = 1;
            _lblFileName.Text = "ფაილი არ არის არჩეული";
            // 
            // pnlFileSelection
            // 
            pnlFileSelection.Controls.Add(_btnSelectFile);
            pnlFileSelection.Controls.Add(_lblFileName);
            pnlFileSelection.Dock = DockStyle.Top;
            pnlFileSelection.Location = new Point(0, 0);
            pnlFileSelection.Name = "pnlFileSelection";
            pnlFileSelection.Padding = new Padding(10);
            pnlFileSelection.Size = new Size(1371, 60);
            pnlFileSelection.TabIndex = 0;
            // 
            // pnlOptions
            // 
            pnlOptions.Controls.Add(_chkEnableMapping);
            pnlOptions.Controls.Add(_chkIsActive);
            pnlOptions.Controls.Add(_btnSaveMapping);
            pnlOptions.Controls.Add(_btnLoadMapping);
            pnlOptions.Controls.Add(_cmbSavedMappings);
            pnlOptions.Dock = DockStyle.Top;
            pnlOptions.Location = new Point(0, 60);
            pnlOptions.Name = "pnlOptions";
            pnlOptions.Padding = new Padding(10);
            pnlOptions.Size = new Size(1371, 50);
            pnlOptions.TabIndex = 1;
            // 
            // _chkEnableMapping
            // 
            _chkEnableMapping.AutoSize = true;
            _chkEnableMapping.Checked = true;
            _chkEnableMapping.CheckState = CheckState.Checked;
            _chkEnableMapping.Location = new Point(10, 15);
            _chkEnableMapping.Name = "_chkEnableMapping";
            _chkEnableMapping.Size = new Size(195, 19);
            _chkEnableMapping.TabIndex = 0;
            _chkEnableMapping.Text = "გამოიყენე სვეტების მეპინგი";
            _chkEnableMapping.UseVisualStyleBackColor = true;
            _chkEnableMapping.CheckedChanged += ChkEnableMapping_CheckedChanged;
            // 
            // _chkIsActive
            // 
            _chkIsActive.AutoSize = true;
            _chkIsActive.Checked = true;
            _chkIsActive.CheckState = CheckState.Checked;
            _chkIsActive.Location = new Point(200, 15);
            _chkIsActive.Name = "_chkIsActive";
            _chkIsActive.Size = new Size(162, 19);
            _chkIsActive.TabIndex = 1;
            _chkIsActive.Text = "სტუდენტები აქტიური";
            _chkIsActive.UseVisualStyleBackColor = true;
            // 
            // _btnSaveMapping
            // 
            _btnSaveMapping.Enabled = false;
            _btnSaveMapping.Location = new Point(350, 12);
            _btnSaveMapping.Name = "_btnSaveMapping";
            _btnSaveMapping.Size = new Size(120, 25);
            _btnSaveMapping.TabIndex = 2;
            _btnSaveMapping.Text = "შეინახე მეპინგი";
            _btnSaveMapping.UseVisualStyleBackColor = true;
            _btnSaveMapping.Click += BtnSaveMapping_Click;
            // 
            // _btnLoadMapping
            // 
            _btnLoadMapping.Location = new Point(480, 12);
            _btnLoadMapping.Name = "_btnLoadMapping";
            _btnLoadMapping.Size = new Size(120, 25);
            _btnLoadMapping.TabIndex = 3;
            _btnLoadMapping.Text = "დატვირთე მეპინგი";
            _btnLoadMapping.UseVisualStyleBackColor = true;
            _btnLoadMapping.Click += BtnLoadMapping_Click;
            // 
            // _cmbSavedMappings
            // 
            _cmbSavedMappings.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbSavedMappings.FormattingEnabled = true;
            _cmbSavedMappings.Location = new Point(610, 14);
            _cmbSavedMappings.Name = "_cmbSavedMappings";
            _cmbSavedMappings.Size = new Size(200, 23);
            _cmbSavedMappings.TabIndex = 4;
            // 
            // _splitContainerMain
            // 
            _splitContainerMain.Dock = DockStyle.Fill;
            _splitContainerMain.Location = new Point(0, 160);
            _splitContainerMain.Name = "_splitContainerMain";
            _splitContainerMain.Orientation = Orientation.Horizontal;
            // 
            // _splitContainerMain.Panel1
            // 
            _splitContainerMain.Panel1.Controls.Add(_pnlMapping);
            _splitContainerMain.Panel1MinSize = 250;
            // 
            // _splitContainerMain.Panel2
            // 
            _splitContainerMain.Panel2.Controls.Add(_tabControlSheets);
            _splitContainerMain.Size = new Size(1371, 490);
            _splitContainerMain.SplitterDistance = 250;
            _splitContainerMain.TabIndex = 2;
            // 
            // _pnlMapping
            // 
            _pnlMapping.Controls.Add(splitContainerMapping);
            _pnlMapping.Dock = DockStyle.Fill;
            _pnlMapping.Location = new Point(0, 0);
            _pnlMapping.Name = "_pnlMapping";
            _pnlMapping.Padding = new Padding(10);
            _pnlMapping.Size = new Size(1371, 250);
            _pnlMapping.TabIndex = 0;
            _pnlMapping.Visible = false;
            // 
            // splitContainerMapping
            // 
            splitContainerMapping.Dock = DockStyle.Fill;
            splitContainerMapping.Location = new Point(10, 10);
            splitContainerMapping.Name = "splitContainerMapping";
            // 
            // splitContainerMapping.Panel1
            // 
            splitContainerMapping.Panel1.Controls.Add(pnlSheetMapping);
            splitContainerMapping.Panel1MinSize = 350;
            // 
            // splitContainerMapping.Panel2
            // 
            splitContainerMapping.Panel2.Controls.Add(pnlColumnMapping);
            splitContainerMapping.Panel2MinSize = 600;
            splitContainerMapping.Size = new Size(1351, 230);
            splitContainerMapping.SplitterDistance = 350;
            splitContainerMapping.TabIndex = 0;
            // 
            // pnlSheetMapping
            // 
            pnlSheetMapping.Controls.Add(flowLayoutSheetMapping);
            pnlSheetMapping.Dock = DockStyle.Fill;
            pnlSheetMapping.Location = new Point(0, 0);
            pnlSheetMapping.Name = "pnlSheetMapping";
            pnlSheetMapping.Size = new Size(350, 230);
            pnlSheetMapping.TabIndex = 0;
            // 
            // flowLayoutSheetMapping
            // 
            flowLayoutSheetMapping.AutoScroll = true;
            flowLayoutSheetMapping.Dock = DockStyle.Fill;
            flowLayoutSheetMapping.FlowDirection = FlowDirection.TopDown;
            flowLayoutSheetMapping.Location = new Point(0, 0);
            flowLayoutSheetMapping.Name = "flowLayoutSheetMapping";
            flowLayoutSheetMapping.Size = new Size(350, 230);
            flowLayoutSheetMapping.TabIndex = 1;
            flowLayoutSheetMapping.WrapContents = false;
            // 
            // pnlColumnMapping
            // 
            pnlColumnMapping.Dock = DockStyle.Fill;
            pnlColumnMapping.Location = new Point(0, 0);
            pnlColumnMapping.Name = "pnlColumnMapping";
            pnlColumnMapping.Padding = new Padding(10);
            pnlColumnMapping.Size = new Size(997, 230);
            pnlColumnMapping.TabIndex = 0;
            // 
            // _tabControlSheets
            // 
            _tabControlSheets.Dock = DockStyle.Fill;
            _tabControlSheets.Location = new Point(0, 0);
            _tabControlSheets.Name = "_tabControlSheets";
            _tabControlSheets.SelectedIndex = 0;
            _tabControlSheets.Size = new Size(1371, 236);
            _tabControlSheets.TabIndex = 0;
            // 
            // _statusStrip
            // 
            _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel });
            _statusStrip.Location = new Point(0, 700);
            _statusStrip.Name = "_statusStrip";
            _statusStrip.Size = new Size(1371, 22);
            _statusStrip.TabIndex = 3;
            _statusStrip.Text = "statusStrip1";
            // 
            // _statusLabel
            // 
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(1356, 17);
            _statusLabel.Spring = true;
            _statusLabel.Text = "მზადაა";
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlProgress
            // 
            pnlProgress.Controls.Add(_progressBar);
            pnlProgress.Controls.Add(_lblStatus);
            pnlProgress.Dock = DockStyle.Top;
            pnlProgress.Location = new Point(0, 110);
            pnlProgress.Name = "pnlProgress";
            pnlProgress.Padding = new Padding(10);
            pnlProgress.Size = new Size(1371, 50);
            pnlProgress.TabIndex = 4;
            pnlProgress.Visible = false;
            // 
            // _progressBar
            // 
            _progressBar.Dock = DockStyle.Fill;
            _progressBar.Location = new Point(10, 30);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(1351, 10);
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.TabIndex = 1;
            // 
            // _lblStatus
            // 
            _lblStatus.Dock = DockStyle.Top;
            _lblStatus.Location = new Point(10, 10);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(1351, 20);
            _lblStatus.TabIndex = 0;
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _btnImport
            // 
            _btnImport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnImport.Enabled = false;
            _btnImport.Location = new Point(1211, 7);
            _btnImport.Name = "_btnImport";
            _btnImport.Size = new Size(150, 35);
            _btnImport.TabIndex = 0;
            _btnImport.Text = "იმპორტის დაწყება";
            _btnImport.UseVisualStyleBackColor = true;
            _btnImport.Click += BtnImport_Click;
            // 
            // pnlButton
            // 
            pnlButton.Controls.Add(_btnImport);
            pnlButton.Dock = DockStyle.Bottom;
            pnlButton.Location = new Point(0, 650);
            pnlButton.Name = "pnlButton";
            pnlButton.Padding = new Padding(10);
            pnlButton.Size = new Size(1371, 50);
            pnlButton.TabIndex = 5;
            // 
            // ImportFormV2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1371, 722);
            Controls.Add(_splitContainerMain);
            Controls.Add(pnlProgress);
            Controls.Add(pnlButton);
            Controls.Add(pnlOptions);
            Controls.Add(pnlFileSelection);
            Controls.Add(_statusStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 600);
            Name = "ImportFormV2";
            StartPosition = FormStartPosition.CenterParent;
            Text = "სტუდენტების იმპორტი Excel-იდან";
            Load += ImportFormV2_Load;
            pnlFileSelection.ResumeLayout(false);
            pnlFileSelection.PerformLayout();
            pnlOptions.ResumeLayout(false);
            pnlOptions.PerformLayout();
            _splitContainerMain.Panel1.ResumeLayout(false);
            _splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_splitContainerMain).EndInit();
            _splitContainerMain.ResumeLayout(false);
            _pnlMapping.ResumeLayout(false);
            splitContainerMapping.Panel1.ResumeLayout(false);
            splitContainerMapping.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMapping).EndInit();
            splitContainerMapping.ResumeLayout(false);
            pnlSheetMapping.ResumeLayout(false);
            _statusStrip.ResumeLayout(false);
            _statusStrip.PerformLayout();
            pnlProgress.ResumeLayout(false);
            pnlButton.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlFileSelection;
        private System.Windows.Forms.Button _btnSelectFile;
        private System.Windows.Forms.Label _lblFileName;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.CheckBox _chkEnableMapping;
        private System.Windows.Forms.CheckBox _chkIsActive;
        private System.Windows.Forms.Button _btnSaveMapping;
        private System.Windows.Forms.Button _btnLoadMapping;
        private System.Windows.Forms.ComboBox _cmbSavedMappings;
        private System.Windows.Forms.SplitContainer _splitContainerMain;
        private System.Windows.Forms.Panel _pnlMapping;
        private System.Windows.Forms.TabControl _tabControlSheets;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Button _btnImport;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.SplitContainer splitContainerMapping;
        private System.Windows.Forms.Panel pnlSheetMapping;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutSheetMapping;
        private System.Windows.Forms.Panel pnlColumnMapping;
    }
}
