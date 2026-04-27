using BCCStudents.Application.Interfaces;
using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Services;
using BCCStudents.Presentation.Properties;
using System.Data;
//using Irony;

namespace BCCStudents.Presentation
{
    public partial class BackupManagementForm : Form
    {
        private readonly BackupService _backupManager;
        private readonly IUserContext _userContext;
        private List<BackupInfo> _backups;
        private bool _isLoading = false;

        public BackupManagementForm(BackupService backupManager, IUserContext userContext)
        {
            _backupManager = backupManager ?? throw new ArgumentNullException(nameof(backupManager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            InitializeComponent();
            InitializeForm();
            LoadBackups();

            // Apply security checks after form is loaded
            this.Load += BackupManagementForm_Load;
        }

        private void BackupManagementForm_Load(object sender, EventArgs e)
        {
            ApplySecurityChecks();
        }

        private void ApplySecurityChecks()
        {
            // btnCreateBackup - CanEditSettings permission (backup creation is a settings operation)
            if (btnCreateBackup != null)
            {
                btnCreateBackup.Tag = $"Permission_{Permission.CanEditSettings}";
                btnCreateBackup.Enabled = _userContext.HasPermission(Permission.CanEditSettings);
            }

            // btnRestoreBackup - CanEditSettings permission (backup restore is a settings operation)
            if (btnRestoreBackup != null)
            {
                btnRestoreBackup.Tag = $"Permission_{Permission.CanEditSettings}";
                btnRestoreBackup.Enabled = _userContext.HasPermission(Permission.CanEditSettings);
            }

            // btnDeleteBackup - CanEditSettings permission
            if (btnDeleteBackup != null)
            {
                btnDeleteBackup.Tag = $"Permission_{Permission.CanEditSettings}";
                btnDeleteBackup.Enabled = _userContext.HasPermission(Permission.CanEditSettings);
            }

            // btnEnableAutoBackup - CanEditSettings permission
            if (btnEnableAutoBackup != null)
            {
                btnEnableAutoBackup.Tag = $"Permission_{Permission.CanEditSettings}";
                btnEnableAutoBackup.Enabled = _userContext.HasPermission(Permission.CanEditSettings);
            }

            // btnDisableAutoBackup - CanEditSettings permission
            if (btnDisableAutoBackup != null)
            {
                btnDisableAutoBackup.Tag = $"Permission_{Permission.CanEditSettings}";
                btnDisableAutoBackup.Enabled = _userContext.HasPermission(Permission.CanEditSettings);
            }

            // btnRefresh - CanViewReports or CanEditSettings (viewing backups)
            if (btnRefresh != null)
            {
                btnRefresh.Tag = $"Permission_{Permission.CanViewReports}";
                btnRefresh.Enabled = _userContext.HasPermission(Permission.CanViewReports) || _userContext.HasPermission(Permission.CanEditSettings);
            }

            // btnOpenBackupFolder - CanViewReports or CanEditSettings
            if (btnOpenBackupFolder != null)
            {
                btnOpenBackupFolder.Tag = $"Permission_{Permission.CanViewReports}";
                btnOpenBackupFolder.Enabled = _userContext.HasPermission(Permission.CanViewReports) || _userContext.HasPermission(Permission.CanEditSettings);
            }
        }

        private void InitializeForm()
        {
            FormTitleHelper.SetTitle(this, Resources.BackupTitle);

            // ღილაკების ივენთების მიბმა
            btnCreateBackup.Click += BtnCreateBackup_Click;
            btnRestoreBackup.Click += BtnRestoreBackup_Click;
            btnDeleteBackup.Click += BtnDeleteBackup_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnOpenBackupFolder.Click += BtnOpenBackupFolder_Click;

            // პერიოდული ბექაპის ღილაკების ივენთები
            btnEnableAutoBackup.Click += BtnEnableAutoBackup_Click;
            btnDisableAutoBackup.Click += BtnDisableAutoBackup_Click;

            // ფილტრის ღილაკების ივენთები
            btnApplyFilter.Click += BtnApplyFilter_Click;
            btnClearFilter.Click += BtnClearFilter_Click;

            // ფორმის დახურვის ივენთი
            this.FormClosing += BackupManagementForm_FormClosing;

            // DataGridView-ის ინიციალიზაცია
            SetupDataGridView();

            // პერიოდული ბექაპის სტატუსის ჩვენება
            UpdateAutoBackupStatus();

            // საწყისი თარიღების დაყენება ფილტრისთვის
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
        }

        private void BackupManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // აქ შეგვიძლია გავაჩეროთ პერიოდული ბექაპი დახურვისას (ამჟამად გამორთულია)
            // BackupManager.StopPeriodicBackup();
        }

        private void SetupDataGridView()
        {
            dgvBackups.AutoGenerateColumns = false;
            dgvBackups.Columns.Clear();

            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileName",
                HeaderText = Resources.Backup_Column_FileName,
                DataPropertyName = "FileName",
                Width = 200
            });

            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreationTime",
                HeaderText = Resources.Backup_Column_CreationTime,
                DataPropertyName = "CreationTime",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
            });

            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileSize",
                HeaderText = Resources.Backup_Column_FileSize,
                DataPropertyName = "FileSizeFormatted",
                Width = 100
            });

            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FilePath",
                HeaderText = Resources.Backup_Column_FilePath,
                DataPropertyName = "FilePath",
                Width = 300,
                Visible = false
            });

            // ორმაგი დაჭერა ბექაპის ფაილის გასახსნელად
            dgvBackups.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var backup = dgvBackups.Rows[e.RowIndex].DataBoundItem as BackupInfo;
                    if (backup != null)
                    {
                        OpenBackupFile(backup.FilePath);
                    }
                }
            };
        }

        private async void LoadBackups()
        {
            try
            {
                _isLoading = true;
                btnRefresh.Enabled = false;

                // ვაჩვენებთ, რომ მიმდინარეობს ბექაპების ჩატვირთვა
                lblStatus.Text = Resources.Backup_Status_LoadingBackups;
                System.Windows.Forms.Application.DoEvents();

                _backups = await Task.Run(() => _backupManager.GetBackupList());

                // თარიღის ფილტრის გამოყენება
                ApplyDateFilter();

                lblStatus.Text = string.Format(Resources.Backup_Status_LoadedCount, _backups.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{Resources.Backup_Status_LoadError}: {ex.Message}",
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblStatus.Text = Resources.Backup_Status_LoadError;
            }
            finally
            {
                _isLoading = false;
                btnRefresh.Enabled = true;
            }
        }

        private void ApplyDateFilter()
        {
            if (_backups == null) return;

            var filteredBackups = _backups.Where(b =>
                b.CreationTime.Date >= dtpFrom.Value.Date &&
                b.CreationTime.Date <= dtpTo.Value.Date).ToList();

            dgvBackups.DataSource = filteredBackups;
            lblStatus.Text = string.Format(
                Resources.Backup_Status_FilteredCount,
                filteredBackups.Count,
                dtpFrom.Value.ToString("yyyy-MM-dd"),
                dtpTo.Value.ToString("yyyy-MM-dd"));
        }

        private async void BtnCreateBackup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditSettings))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnCreateBackup.Enabled = false;
                lblStatus.Text = Resources.Backup_Create_Status;
                System.Windows.Forms.Application.DoEvents();

                bool success = _backupManager.CreatePeriodicBackup();

                if (success)
                {
                    MessageBox.Show(
                        Resources.Backup_Create_Success,
                        Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadBackups();
                }
                else
                {
                    MessageBox.Show(
                        Resources.Backup_Create_Fail,
                        Resources.Common_ErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_Create_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnCreateBackup.Enabled = true;
            }
        }

        private async void BtnRestoreBackup_Click(object sender, EventArgs e)
        {
            if (dgvBackups.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    Resources.Backup_Restore_NoSelection,
                    Resources.Common_WarningTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var selectedBackup = dgvBackups.SelectedRows[0].DataBoundItem as BackupInfo;
            if (selectedBackup == null) return;

            var result = MessageBox.Show(
                string.Format(
                    Resources.Backup_Restore_ConfirmText,
                    Environment.NewLine,
                    selectedBackup.FileName,
                    selectedBackup.CreationTime.ToString("yyyy-MM-dd HH:mm")),
                Resources.Backup_Restore_ConfirmTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    btnRestoreBackup.Enabled = false;
                    lblStatus.Text = Resources.Backup_Restore_Status;
                    System.Windows.Forms.Application.DoEvents();

                    bool success = _backupManager.RestoreBackup(selectedBackup.FilePath);

                    if (success)
                    {
                        MessageBox.Show(
                            Resources.Backup_Restore_Success,
                            Resources.Common_InfoTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            Resources.Backup_Restore_Fail,
                            Resources.Common_ErrorTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(Resources.Backup_Restore_Error, ex.Message),
                        Resources.Common_ErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    btnRestoreBackup.Enabled = true;
                }
            }
        }

        private void BtnDeleteBackup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditSettings))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvBackups.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    Resources.Backup_Delete_NoSelection,
                    Resources.Common_WarningTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var selectedBackup = dgvBackups.SelectedRows[0].DataBoundItem as BackupInfo;
            if (selectedBackup == null) return;

            var result = MessageBox.Show(
                string.Format(
                    Resources.Backup_Delete_ConfirmText,
                    Environment.NewLine,
                    selectedBackup.FileName,
                    selectedBackup.CreationTime.ToString("yyyy-MM-dd HH:mm")),
                Resources.Backup_Delete_ConfirmTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(selectedBackup.FilePath);
                    MessageBox.Show(
                        Resources.Backup_Delete_Success,
                        Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadBackups();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(Resources.Backup_Delete_Error, ex.Message),
                        Resources.Common_ErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadBackups();
        }

        private void BtnOpenBackupFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(_backupManager.BackupDirectory))
                {
                    Directory.CreateDirectory(_backupManager.BackupDirectory);
                }

                System.Diagnostics.Process.Start("explorer.exe", _backupManager.BackupDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_OpenFolder_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnEnableAutoBackup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditSettings))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _backupManager.AutoBackupEnabled = true;
                _backupManager.InitializePeriodicBackup();
                UpdateAutoBackupStatus();
                MessageBox.Show(
                    Resources.Backup_Auto_Enable_Success,
                    Resources.Common_InfoTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_Auto_Enable_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnDisableAutoBackup_Click(object sender, EventArgs e)
        {
            // Security check
            if (!_userContext.HasPermission(Permission.CanEditSettings))
            {
                MessageBox.Show("თქვენ არ გაქვთ ამ ოპერაციის გამოყენების უფლება!", "წვდომა უარყოფილია", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _backupManager.AutoBackupEnabled = false;
                _backupManager.StopPeriodicBackup();
                UpdateAutoBackupStatus();
                MessageBox.Show(
                    Resources.Backup_Auto_Disable_Success,
                    Resources.Common_InfoTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_Auto_Disable_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyDateFilter();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
            ApplyDateFilter();
        }

        private void UpdateAutoBackupStatus()
        {
            if (_backupManager.AutoBackupEnabled)
            {
                lblAutoBackupStatus.Text = string.Format(
                    Resources.Backup_Auto_Status_Enabled,
                    _backupManager.BackupIntervalHours);
                lblAutoBackupStatus.ForeColor = Color.Green;
                btnEnableAutoBackup.Enabled = false;
                btnDisableAutoBackup.Enabled = true;
            }
            else
            {
                lblAutoBackupStatus.Text = Resources.Backup_Auto_Status_Disabled;
                lblAutoBackupStatus.ForeColor = Color.Red;
                btnEnableAutoBackup.Enabled = true;
                btnDisableAutoBackup.Enabled = false;
            }

            if (_backupManager.LastBackupTime != DateTime.MinValue)
            {
                lblLastBackup.Text = string.Format(
                    Resources.Backup_LastBackup,
                    _backupManager.LastBackupTime.ToString("yyyy-MM-dd HH:mm"));
            }
            else
            {
                lblLastBackup.Text = Resources.Backup_LastBackup_None;
            }
        }

        private void OpenBackupFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show(
                        Resources.Backup_File_NotFound,
                        Resources.Common_ErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // ვქმნით კონტექსტურ მენიუს არჩეული ბექაპ ფაილისთვის
                var contextMenu = new ContextMenuStrip();

                // ტექსტური რედაქტორით გახსნა
                contextMenu.Items.Add(Resources.Backup_Menu_OpenInEditor, null, (s, e) =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start("notepad.exe", filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(Resources.Backup_File_Open_Error, ex.Message),
                            Resources.Common_ErrorTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                });

                // შიგთავსის ნახვა
                contextMenu.Items.Add(Resources.Backup_Menu_ViewContent, null, (s, e) =>
                {
                    ViewBackupContent(filePath);
                });

                // ფაილის კოპირება
                contextMenu.Items.Add(Resources.Backup_Menu_CopyFile, null, (s, e) =>
                {
                    try
                    {
                        var saveDialog = new SaveFileDialog
                        {
                            FileName = Path.GetFileName(filePath),
                            Filter = "SQL ფაილები (*.sql)|*.sql|ყველა ფაილი (*.*)|*.*"
                        };

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.Copy(filePath, saveDialog.FileName, true);
                            MessageBox.Show(
                                "ფაილი წარმატებით დაკოპირდა!",
                                Resources.Common_InfoTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(Resources.Backup_File_Copy_Error, ex.Message),
                            Resources.Common_ErrorTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                });

                // ფაილის ინფორმაციის ნახვა
                contextMenu.Items.Add(Resources.Backup_Menu_FileInfo, null, (s, e) =>
                {
                    ShowFileInfo(filePath);
                });

                // ვაჩვენებთ კონტექსტურ მენიუს
                var point = dgvBackups.PointToClient(Cursor.Position);
                contextMenu.Show(dgvBackups, point);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_File_Open_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ViewBackupContent(string filePath)
        {
            try
            {
                var viewerForm = new BackupContentViewerForm(Path.GetFileName(filePath), filePath);
                viewerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_File_ViewContent_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string ReadFileWithProperEncoding(string filePath)
        {
            try
            {
                // ვცდილობთ UTF-8 კოდირებით წაკითხვას
                return File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            }
            catch
            {
                try
                {
                    // თუ UTF-8 არ იმუშავებს, ვცდილობთ UTF-8 BOM-ით
                    return File.ReadAllText(filePath, new System.Text.UTF8Encoding(true));
                }
                catch
                {
                    try
                    {
                        // შემდეგ ვცდილობთ Windows-1252 კოდირებით
                        return File.ReadAllText(filePath, System.Text.Encoding.GetEncoding(1252));
                    }
                    catch
                    {
                        try
                        {
                            // ბოლოს ვიყენებთ სისტემის default კოდირებას
                            return File.ReadAllText(filePath, System.Text.Encoding.Default);
                        }
                        catch
                        {
                            // თუ მაინც ვერ წავიკითხეთ, ვაბრუნებთ File.ReadAllText-ის ნაგულისხმევს
                            return File.ReadAllText(filePath);
                        }
                    }
                }
            }
        }

        private void ShowFileInfo(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var info = string.Format(
                    Resources.Backup_File_Info_Text,
                    fileInfo.Name,
                    Environment.NewLine,
                    fileInfo.FullName,
                    FormatFileSize(fileInfo.Length),
                    fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    fileInfo.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss"));

                MessageBox.Show(
                    info,
                    Resources.Backup_File_Info_Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Resources.Backup_File_Info_Error, ex.Message),
                    Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
