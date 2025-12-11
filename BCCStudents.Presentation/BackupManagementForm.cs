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

namespace BCCStudents.Presentation
{
    public partial class BackupManagementForm : Form
    {
        private List<BackupInfo> _backups;
        private bool _isLoading = false;

        public BackupManagementForm()
        {
            InitializeComponent();
            InitializeForm();
            LoadBackups();
        }

        private void InitializeForm()
        {
            if (!Properties.Settings.Default.IsTestDb)
                FormTitleHelper.SetTitle(this, "áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ›áƒáƒ áƒ—áƒ•áƒ");
            else FormTitleHelper.SetTitle(this, "áƒ‘áƒ”áƒ¥áƒáƒ¤áƒ˜áƒ¡ áƒ›áƒáƒ áƒ—áƒ•áƒ - áƒ¡áƒáƒ¢áƒ”áƒ¡áƒ¢áƒ áƒ áƒ”áƒŸáƒ˜áƒ›áƒ˜");
            
            // áƒ¦áƒ˜áƒšáƒáƒ™áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ”áƒ‘áƒ˜
            btnCreateBackup.Click += BtnCreateBackup_Click;
            btnRestoreBackup.Click += BtnRestoreBackup_Click;
            btnDeleteBackup.Click += BtnDeleteBackup_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnOpenBackupFolder.Click += BtnOpenBackupFolder_Click;
            
            // áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¦áƒ˜áƒšáƒáƒ™áƒ”áƒ‘áƒ˜
            btnEnableAutoBackup.Click += BtnEnableAutoBackup_Click;
            btnDisableAutoBackup.Click += BtnDisableAutoBackup_Click;
            
            // áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜áƒ¡ áƒ¦áƒ˜áƒšáƒáƒ™áƒ”áƒ‘áƒ˜
            btnApplyFilter.Click += BtnApplyFilter_Click;
            btnClearFilter.Click += BtnClearFilter_Click;
            
            // áƒ¤áƒáƒ áƒ›áƒ˜áƒ¡ áƒ“áƒáƒ®áƒ£áƒ áƒ•áƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ˜
            this.FormClosing += BackupManagementForm_FormClosing;
            
            // DataGridView-áƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ
            SetupDataGridView();
            
            // áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¡áƒ¢áƒáƒ¢áƒ£áƒ¡áƒ˜áƒ¡ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
            UpdateAutoBackupStatus();
            
            // áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜áƒ¡ áƒ•áƒ”áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ
            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
        }

        private void BackupManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // áƒáƒ  áƒ•áƒáƒ©áƒ”áƒ áƒ”áƒ‘áƒ— áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒš áƒ‘áƒ”áƒ¥áƒáƒžáƒ¡, áƒ áƒáƒ“áƒ’áƒáƒœ áƒ˜áƒ¡ áƒ£áƒœáƒ“áƒ áƒ’áƒáƒ’áƒ áƒ«áƒ”áƒšáƒ“áƒ”áƒ¡ áƒ›áƒ—áƒáƒ•áƒáƒ  áƒ¤áƒáƒ áƒ›áƒáƒ¨áƒ˜
            // BackupManager.StopPeriodicBackup();
        }

        private void SetupDataGridView()
        {
            dgvBackups.AutoGenerateColumns = false;
            dgvBackups.Columns.Clear();
            
            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileName",
                HeaderText = "áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜",
                DataPropertyName = "FileName",
                Width = 200
            });
            
            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreationTime",
                HeaderText = "áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜",
                DataPropertyName = "CreationTime",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
            });
            
            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileSize",
                HeaderText = "áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ–áƒáƒ›áƒ",
                DataPropertyName = "FileSizeFormatted",
                Width = 100
            });
            
            dgvBackups.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FilePath",
                HeaderText = "áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ›áƒ˜áƒ¡áƒáƒ›áƒáƒ áƒ—áƒ˜",
                DataPropertyName = "FilePath",
                Width = 300,
                Visible = false
            });
            
            // áƒáƒ áƒ›áƒáƒ’áƒ˜ áƒ“áƒáƒ¬áƒ™áƒáƒžáƒ£áƒœáƒ”áƒ‘áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒáƒ¡áƒáƒ®áƒ¡áƒœáƒ”áƒšáƒáƒ“
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
                
                // áƒ•áƒáƒ©áƒ•áƒ”áƒœáƒáƒ— áƒžáƒ áƒáƒ’áƒ áƒ”áƒ¡áƒ˜
                lblStatus.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”áƒáƒ‘áƒ¡ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ...";
                Application.DoEvents();
                
                _backups = await Task.Run(() => BackupManager.GetBackupList());
                
                // áƒ’áƒáƒ•áƒ¤áƒ˜áƒšáƒ¢áƒ áƒáƒ— áƒ—áƒáƒ áƒ˜áƒ¦áƒ”áƒ‘áƒ˜áƒ¡ áƒ›áƒ˜áƒ®áƒ”áƒ“áƒ•áƒ˜áƒ—
                ApplyDateFilter();
                
                lblStatus.Text = $"áƒœáƒáƒžáƒáƒ•áƒœáƒ˜áƒ {_backups.Count} áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡";
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
            lblStatus.Text = $"áƒœáƒáƒžáƒáƒ•áƒœáƒ˜áƒ {filteredBackups.Count} áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ (áƒ¤áƒ˜áƒšáƒ¢áƒ áƒ˜: {dtpFrom.Value:yyyy-MM-dd} - {dtpTo.Value:yyyy-MM-dd})";
        }

        private async void BtnCreateBackup_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreateBackup.Enabled = false;
                lblStatus.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”áƒáƒ‘áƒ¡ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ...";
                Application.DoEvents();
                
                bool success = BackupManager.CreatePeriodicBackup();
                
                if (success)
                {
                    MessageBox.Show("áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ¨áƒ”áƒ˜áƒ¥áƒ›áƒœáƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBackups();
                }
                else
                {
                    MessageBox.Show("áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var selectedBackup = dgvBackups.SelectedRows[0].DataBoundItem as BackupInfo;
            if (selectedBackup == null) return;
            
            var result = MessageBox.Show(
                $"áƒœáƒáƒ›áƒ“áƒ•áƒ˜áƒšáƒáƒ“ áƒ’áƒ¡áƒ£áƒ áƒ— áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ“áƒáƒœ?\n\náƒ¤áƒáƒ˜áƒšáƒ˜: {selectedBackup.FileName}\náƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {selectedBackup.CreationTime:yyyy-MM-dd HH:mm}\n\náƒ§áƒ£áƒ áƒáƒ“áƒ¦áƒ”áƒ‘áƒ: áƒ”áƒ¡ áƒáƒžáƒ”áƒ áƒáƒªáƒ˜áƒ áƒ¨áƒ”áƒªáƒ•áƒšáƒ˜áƒ¡ áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒ›áƒáƒœáƒáƒªáƒ”áƒ›áƒ”áƒ‘áƒ¡!",
                "áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    btnRestoreBackup.Enabled = false;
                    lblStatus.Text = "áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”áƒáƒ‘áƒ¡ áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ...";
                    Application.DoEvents();
                    
                    bool success = BackupManager.RestoreBackup(selectedBackup.FilePath);
                    
                    if (success)
                    {
                        MessageBox.Show("áƒ‘áƒáƒ–áƒ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ˜áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒáƒ–áƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                        "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnRestoreBackup.Enabled = true;
                }
            }
        }

        private void BtnDeleteBackup_Click(object sender, EventArgs e)
        {
            if (dgvBackups.SelectedRows.Count == 0)
            {
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒáƒ˜áƒ áƒ©áƒ˜áƒáƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¬áƒáƒ¡áƒáƒ¨áƒšáƒ”áƒšáƒáƒ“!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var selectedBackup = dgvBackups.SelectedRows[0].DataBoundItem as BackupInfo;
            if (selectedBackup == null) return;
            
            var result = MessageBox.Show(
                $"áƒœáƒáƒ›áƒ“áƒ•áƒ˜áƒšáƒáƒ“ áƒ’áƒ¡áƒ£áƒ áƒ— áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ?\n\náƒ¤áƒáƒ˜áƒšáƒ˜: {selectedBackup.FileName}\náƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {selectedBackup.CreationTime:yyyy-MM-dd HH:mm}",
                "áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(selectedBackup.FilePath);
                    MessageBox.Show("áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ¬áƒáƒ˜áƒ¨áƒáƒšáƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBackups();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¬áƒáƒ¨áƒšáƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                        "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (!Directory.Exists(BackupManager.BackupDirectory))
                {
                    Directory.CreateDirectory(BackupManager.BackupDirectory);
                }
                
                System.Diagnostics.Process.Start("explorer.exe", BackupManager.BackupDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ˜áƒ¡ áƒ’áƒáƒ¡áƒáƒ®áƒ¡áƒœáƒ”áƒšáƒáƒ“: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEnableAutoBackup_Click(object sender, EventArgs e)
        {
            try
            {
                BackupManager.AutoBackupEnabled = true;
                BackupManager.InitializePeriodicBackup();
                UpdateAutoBackupStatus();
                MessageBox.Show("áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ©áƒáƒ áƒ—áƒ£áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ©áƒáƒ áƒ—áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDisableAutoBackup_Click(object sender, EventArgs e)
        {
            try
            {
                BackupManager.AutoBackupEnabled = false;
                BackupManager.StopPeriodicBackup();
                UpdateAutoBackupStatus();
                MessageBox.Show("áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ’áƒáƒ›áƒáƒ áƒ—áƒ£áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ’áƒáƒ›áƒáƒ áƒ—áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (BackupManager.AutoBackupEnabled)
            {
                lblAutoBackupStatus.Text = $"áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ©áƒáƒ áƒ—áƒ£áƒšáƒ˜áƒ (áƒ˜áƒœáƒ¢áƒ”áƒ áƒ•áƒáƒšáƒ˜: {BackupManager.BackupIntervalHours} áƒ¡áƒáƒáƒ—áƒ˜)";
                lblAutoBackupStatus.ForeColor = Color.Green;
                btnEnableAutoBackup.Enabled = false;
                btnDisableAutoBackup.Enabled = true;
            }
            else
            {
                lblAutoBackupStatus.Text = "áƒžáƒ”áƒ áƒ˜áƒáƒ“áƒ£áƒšáƒ˜ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ’áƒáƒ›áƒáƒ áƒ—áƒ£áƒšáƒ˜áƒ";
                lblAutoBackupStatus.ForeColor = Color.Red;
                btnEnableAutoBackup.Enabled = true;
                btnDisableAutoBackup.Enabled = false;
            }
            
            if (BackupManager.LastBackupTime != DateTime.MinValue)
            {
                lblLastBackup.Text = $"áƒ‘áƒáƒšáƒ áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜: {BackupManager.LastBackupTime:yyyy-MM-dd HH:mm}";
            }
            else
            {
                lblLastBackup.Text = "áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜ áƒ¯áƒ”áƒ  áƒáƒ  áƒ’áƒáƒ™áƒ”áƒ—áƒ”áƒ‘áƒ£áƒšáƒ";
            }
        }

        private void OpenBackupFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // áƒ¨áƒ”áƒ•áƒ¥áƒ›áƒœáƒáƒ— áƒ™áƒáƒœáƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ›áƒ”áƒœáƒ˜áƒ£ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒáƒ¡áƒáƒ®áƒ¡áƒœáƒ”áƒšáƒáƒ“
                var contextMenu = new ContextMenuStrip();
                
                // áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ áƒ”áƒ“áƒáƒ¥áƒ¢áƒáƒ áƒ˜áƒ— áƒ’áƒáƒ¡áƒáƒ®áƒ¡áƒœáƒ
                contextMenu.Items.Add("áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ áƒ”áƒ“áƒáƒ¥áƒ¢áƒáƒ áƒ˜áƒ— áƒ’áƒáƒ®áƒ¡áƒœáƒ", null, (s, e) =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start("notepad.exe", filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒáƒ¡áƒáƒ®áƒ¡áƒœáƒ”áƒšáƒáƒ“: {ex.Message}", 
                            "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });

                // áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜áƒ¡ áƒœáƒáƒ®áƒ•áƒ áƒ¤áƒáƒ áƒ›áƒáƒ¨áƒ˜
                contextMenu.Items.Add("áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜áƒ¡ áƒœáƒáƒ®áƒ•áƒ", null, (s, e) =>
                {
                    ViewBackupContent(filePath);
                });

                // áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ
                contextMenu.Items.Add("áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ", null, (s, e) =>
                {
                    try
                    {
                        var saveDialog = new SaveFileDialog
                        {
                            FileName = Path.GetFileName(filePath),
                            Filter = "SQL áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜ (*.sql)|*.sql|áƒ§áƒ•áƒ”áƒšáƒ áƒ¤áƒáƒ˜áƒšáƒ˜ (*.*)|*.*"
                        };

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.Copy(filePath, saveDialog.FileName, true);
                            MessageBox.Show("áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ“áƒáƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                            "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });

                // áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ˜áƒ¡ áƒœáƒáƒ®áƒ•áƒ
                contextMenu.Items.Add("áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", null, (s, e) =>
                {
                    ShowFileInfo(filePath);
                });

                // áƒ•áƒáƒ©áƒ•áƒ”áƒœáƒáƒ— áƒ™áƒáƒœáƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ›áƒ”áƒœáƒ˜áƒ£
                var point = dgvBackups.PointToClient(Cursor.Position);
                contextMenu.Show(dgvBackups, point);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒáƒ¡áƒáƒ®áƒ¡áƒœáƒ”áƒšáƒáƒ“: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜áƒ¡ áƒ¬áƒáƒ¡áƒáƒ™áƒ˜áƒ—áƒ®áƒáƒ“: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ReadFileWithProperEncoding(string filePath)
        {
            try
            {
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— UTF-8 áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
                return File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            }
            catch
            {
                try
                {
                    // áƒ—áƒ£ UTF-8 áƒáƒ  áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ¡, áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— UTF-8 BOM-áƒ˜áƒ—
                    return File.ReadAllText(filePath, new System.Text.UTF8Encoding(true));
                }
                catch
                {
                    try
                    {
                        // áƒ—áƒ£ UTF-8 áƒáƒ  áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ¡, áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— Windows-1252 áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
                        return File.ReadAllText(filePath, System.Text.Encoding.GetEncoding(1252));
                    }
                    catch
                    {
                        try
                        {
                            // áƒ‘áƒáƒšáƒ áƒ•áƒáƒ áƒ˜áƒáƒœáƒ¢áƒ˜ - áƒ¡áƒ˜áƒ¡áƒ¢áƒ”áƒ›áƒ˜áƒ¡ default áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ
                            return File.ReadAllText(filePath, System.Text.Encoding.Default);
                        }
                        catch
                        {
                            // áƒ—áƒ£ áƒáƒ áƒáƒ¤áƒ”áƒ áƒ˜ áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ¡, áƒ•áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ— áƒáƒ áƒ˜áƒ’áƒ˜áƒœáƒáƒšáƒ¡
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
                var info = $"áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¡áƒáƒ®áƒ”áƒšáƒ˜: {fileInfo.Name}\n" +
                          $"áƒ¡áƒ áƒ£áƒšáƒ˜ áƒ›áƒ˜áƒ¡áƒáƒ›áƒáƒ áƒ—áƒ˜: {fileInfo.FullName}\n" +
                          $"áƒ–áƒáƒ›áƒ: {FormatFileSize(fileInfo.Length)}\n" +
                          $"áƒ¨áƒ”áƒ¥áƒ›áƒœáƒ˜áƒ¡ áƒ—áƒáƒ áƒ˜áƒ¦áƒ˜: {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}\n" +
                          $"áƒ‘áƒáƒšáƒ áƒªáƒ•áƒšáƒ˜áƒšáƒ”áƒ‘áƒ: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n" +
                          $"áƒ‘áƒáƒšáƒ áƒ¬áƒ•áƒ“áƒáƒ›áƒ: {fileInfo.LastAccessTime:yyyy-MM-dd HH:mm:ss}";

                MessageBox.Show(info, "áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ›áƒ˜áƒ¦áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
