using BCCStudents.Application.Interfaces;
using BCCStudents.Application.Services.AutoFileDetection;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using Serilog;

namespace BCCStudents.Presentation
{
    /// <summary>
    /// ავტომატური ფაილების აღმოჩენის მენეჯერი
    /// გამოიყენება MainForm-იდან პერიოდულად
    /// შეამოწმებს ახალ ფაილებს და აჩვენებს შეტყობინებებს
    /// </summary>
    public class AutoFileDetectionManager
    {
        private readonly AutoFileDetectionService _detectionService;
        private readonly IUserService _userService;
        private readonly IExcelPaymentImportService _importService;
        private readonly IPaymentDescriptionAnalyzer _descriptionAnalyzer;

        public AutoFileDetectionManager(
            AutoFileDetectionService detectionService,
            IUserService userService,
            IExcelPaymentImportService importService,
            IPaymentDescriptionAnalyzer descriptionAnalyzer)
        {
            _detectionService = detectionService;
            _userService = userService;
            _importService = importService;
            _descriptionAnalyzer = descriptionAnalyzer;
        }

        /// <summary>
        /// ავტომატური შემოწმება MainForm-იდან პერიოდულად
        /// შეამოწმებს ადმინისტრატორის უფლებებს
        /// </summary>
        public async Task CheckForNewFilesAsync()
        {
            // შემოწმება არის თუ არა ადმინისტრატორი
            if (!_userService.IsCurrentUserAdmin())
            {
                return; // მხოლოდ ადმინისტრატორისთვის
            }

            try
            {
                var newFiles = await _detectionService.DetectNewFilesAsync();

                if (newFiles.Count > 0)
                {
                    ShowImportNotificationAsync(newFiles);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "შეცდომა ავტომატური შემოწმებისას");
            }
        }

        /// <summary>
        /// აჩვენებს იმპორტის შეტყობინებას
        /// </summary>
        private void ShowImportNotificationAsync(List<DetectedFile> newFiles)
        {
            if (newFiles.Count == 0) return;

            // გახსენით შეტყობინების ფორმა
            var notificationForm = new AutoImportNotificationForm(newFiles, _detectionService, _importService, _descriptionAnalyzer);

            // ფორმის ჩვენება
            var result = notificationForm.ShowDialog();

            // თუ მომხმარებელმა აირჩია ფაილები და დააჭირა OK
            if (result == DialogResult.OK && notificationForm.SelectedFiles.Count > 0)
            {
                ProcessImportAsync(notificationForm.SelectedFiles);
            }
        }

        /// <summary>
        /// ამუშავებს იმპორტს - გადახდებისთვის PaymentsImportForm-ზე
        /// თუ ერთი ფაილია, გადავიდეთ PaymentsImportForm-ზე
        /// </summary>
        private async void ProcessImportAsync(List<DetectedFile> selectedFiles)
        {
            try
            {
                // თუ ერთი ფაილია, გადავიდეთ PaymentsImportForm-ზე
                if (selectedFiles.Count == 1)
                {
                    var file = selectedFiles[0];

                    // გახსენით იმპორტის ფორმა
                    var importForm = new PaymentsImportForm(_importService, _descriptionAnalyzer);

                    // დააყენეთ არჩეული ფაილი
                    importForm.SetSelectedFile(file.FilePath);

                    // ჩვენება ფორმის
                    var dialogResult = importForm.ShowDialog();

                    // შედეგის შემოწმება
                    if (importForm.Result == ImportFormResult.Success)
                    {
                        // ლოგირება წარმატებული იმპორტის
                        var currentUser = _userService.GetUserById(UserSession.Id);
                        await _detectionService.LogImportedFileAsync(file.FilePath, currentUser?.FullName ?? "Unknown");

                        MessageBox.Show("იმპორტი წარმატებით დასრულდა!", "იმპორტი", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (importForm.Result == ImportFormResult.Failed)
                    {
                        MessageBox.Show("იმპორტი ვერ მოხერხდა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (importForm.Result == ImportFormResult.Cancelled)
                    {
                        MessageBox.Show("იმპორტი გაუქმდა!", "გაუქმება", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (selectedFiles.Count == 0)
                {
                    // თუ ფაილი არ არის არჩეული, ჩვენება შეტყობინება
                    MessageBox.Show("არც ერთი ფაილი არ არის არჩეული იმპორტისთვის.",
                                  "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // ეს არ არის დაპროგრამებული, მხოლოდ ერთი
                    MessageBox.Show($"შეზღუდვა: {selectedFiles.Count} ფაილი აღმოჩენილია. მხოლოდ ერთი ფაილი შეიძლება იმპორტირებული იქნას.",
                                  "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა იმპორტის დამუშავებისას: {ex.Message}",
                              "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
