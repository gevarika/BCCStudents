using BCCStudents.Domain.Entities;
using System.Net;
using System.Text.Json;
using Word = Microsoft.Office.Interop.Word;

namespace BCCStudents.Application.Services
{
    public class DocumentService
    {
        //private readonly string _baseUrl = "https://bccenter.ge/";
        //private readonly string _storageRoot;
        public string DownloadBaseFolder { get; set; } // UI-დან მოდის
        public string FileServerBaseUrl { get; set; } // UI-დან მოდის
        private readonly WebClient _webClient = new WebClient();

        public DocumentService()
        {
            DownloadBaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BCCStudents");
            Directory.CreateDirectory(DownloadBaseFolder);
        }
        public void LoadConfig()
        {
            var config = DocumentConfig.Load();
            DownloadBaseFolder = config.DownloadPath;
            FileServerBaseUrl = config.FileUrl;
        }
        public void GenerateAndPrintContract(string templatePath, Dictionary<string, string> replacementData)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "contract_temp.docx");
            File.Copy(templatePath, tempPath, true);

            Word.Application wordApp = new Word.Application();
            Word.Document doc = wordApp.Documents.Open(tempPath);

            try
            {
                foreach (var pair in replacementData)
                {
                    //MessageBox.Show($"Replacing {pair.Key} with {pair.Value}");
                    ReplaceText(doc, pair.Key, pair.Value);
                }

                // --- ახალი ლოგიკა: გამოვიყენოთ PrintDialog ---
                PrintDialog printDialog = new PrintDialog();
                printDialog.AllowSomePages = true;
                printDialog.AllowSelection = false;
                printDialog.UseEXDialog = true;

                DialogResult dialogResult = printDialog.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    // მომხმარებელმა აირჩია პრინტერი
                    string selectedPrinter = printDialog.PrinterSettings.PrinterName;
                    wordApp.ActivePrinter = selectedPrinter;
                    try
                    {
                        object copies = 1;
                        object pages = Type.Missing;
                        object range = Word.WdPrintOutRange.wdPrintAllDocument;
                        object item = Word.WdPrintOutItem.wdPrintDocumentContent;
                        object pageType = Word.WdPrintOutPages.wdPrintAllPages;
                        object manualDuplexPrint = false;

                        doc.PrintOut(
                            Background: false,
                            Range: ref range,
                            Item: ref item,
                            Copies: ref copies,
                            Pages: ref pages,
                            PageType: ref pageType,
                            ManualDuplexPrint: ref manualDuplexPrint
                        );
                    }
                    catch (Exception ex)
                    {
                        // ბეჭდვის შეცდომა: შემოგთავაზე PDF-ად შენახვა
                        MessageBox.Show($"ბეჭდვის შეცდომა: {ex.Message}\nშემოგთავაზებთ PDF-ად შენახვას.", "ბეჭდვის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SaveAsPdf(doc);
                    }
                }
                else
                {
                    // მომხმარებელმა გააუქმა ბეჭდვა: შემოგთავაზე PDF-ად შენახვა
                    DialogResult savePdf = MessageBox.Show("გსურთ ხელშეკრულების PDF-ად შენახვა?", "PDF-ად შენახვა", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (savePdf == DialogResult.Yes)
                    {
                        SaveAsPdf(doc);
                    }
                }

                doc.Close(false);
                wordApp.Quit();
            }
            catch (Exception)
            {
                doc.Close(false);
                wordApp.Quit();
                throw;
            }
        }
        public void GenerateAndPrintContractsBatch(List<string> templatePaths, List<Dictionary<string, string>> replacementDatas)
        {
            // 1. შევქმნათ თითოეული ჯგუფისთვის შევსებული დროებითი docx ფაილები
            List<string> tempDocPaths = new List<string>();
            for (int i = 0; i < templatePaths.Count; i++)
            {
                string templatePath = templatePaths[i];
                var replacementData = replacementDatas[i];
                string tempPath = Path.Combine(Path.GetTempPath(), $"contract_temp_{Guid.NewGuid()}.docx");
                File.Copy(templatePath, tempPath, true);
                var wordApp = new Word.Application();
                var doc = wordApp.Documents.Open(tempPath);
                try
                {
                    foreach (var pair in replacementData)
                    {
                        ReplaceText(doc, pair.Key, pair.Value);
                    }
                    // მარჟინების დაყენება
                    doc.PageSetup.TopMargin = doc.Application.CentimetersToPoints(0.5f); // 0.5 სმ ზემოდან
                    doc.PageSetup.BottomMargin = doc.Application.CentimetersToPoints(1.5f);
                    doc.PageSetup.LeftMargin = doc.Application.CentimetersToPoints(1.5f);
                    doc.PageSetup.RightMargin = doc.Application.CentimetersToPoints(1.5f);
                    doc.Save();
                }
                finally
                {
                    doc.Close(false);
                    wordApp.Quit();
                }
                tempDocPaths.Add(tempPath);
            }

            // 2. გავაერთიანოთ ყველა docx ერთ დოკუმენტად
            string mergedPath = Path.Combine(Path.GetTempPath(), $"contract_merged_{Guid.NewGuid()}.docx");
            var mergeApp = new Word.Application();
            var mergedDoc = mergeApp.Documents.Add();
            try
            {
                foreach (var docPath in tempDocPaths)
                {
                    // ძველი: mergedDoc.Application.Selection.EndKey(Word.WdUnits.wdStory);
                    // ძველი: mergedDoc.Application.Selection.InsertFile(docPath);
                    // ახალი: Range.InsertFile
                    Word.Range endRange = mergedDoc.Content;
                    endRange.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                    endRange.InsertFile(docPath);
                    // Page Break აღარ დავამატოთ, თუ შაბლონი სწორადაა
                }
                mergedDoc.SaveAs2(mergedPath);

                // 3. გამოვიყენოთ PrintDialog და დავბეჭდოთ ერთდროულად
                PrintDialog printDialog = new PrintDialog();
                printDialog.AllowSomePages = true;
                printDialog.AllowSelection = false;
                printDialog.UseEXDialog = true;
                DialogResult dialogResult = printDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string selectedPrinter = printDialog.PrinterSettings.PrinterName;
                    mergeApp.ActivePrinter = selectedPrinter;
                    try
                    {
                        object copies = 1;
                        object pages = Type.Missing;
                        object range = Word.WdPrintOutRange.wdPrintAllDocument;
                        object item = Word.WdPrintOutItem.wdPrintDocumentContent;
                        object pageType = Word.WdPrintOutPages.wdPrintAllPages;
                        object manualDuplexPrint = false; // Simplex (ერთი მხარე)
                        // შენიშვნა: duplex ბეჭდვა შეიძლება მაინც პრინტერის პარამეტრებიდან იყოს ჩართული
                        mergedDoc.PrintOut(
                            Background: false,
                            Range: ref range,
                            Item: ref item,
                            Copies: ref copies,
                            Pages: ref pages,
                            PageType: ref pageType,
                            ManualDuplexPrint: ref manualDuplexPrint
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ბეჭდვის შეცდომა: {ex.Message}", "ბეჭდვის შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("ბეჭდვა გაუქმდა.", "ბეჭდვა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                mergedDoc.Close(false);
                mergeApp.Quit();
                // დროებითი ფაილების წაშლა
                foreach (var path in tempDocPaths)
                {
                    try { File.Delete(path); } catch { }
                }
                try { File.Delete(mergedPath); } catch { }
            }
        }
        private void ReplaceText(Word.Document doc, string placeholder, string newValue)
        {
            Word.Find findObject = doc.Content.Find;
            findObject.ClearFormatting();
            findObject.Text = placeholder;

            findObject.Replacement.ClearFormatting();
            findObject.Replacement.Text = newValue;

            /*object replaceAll = Word.WdReplace.wdReplaceAll;
            findObject.Execute(Replace: ref replaceAll);*/
            object missing = Type.Missing;
            object replaceAll = Word.WdReplace.wdReplaceAll;
            object wrap = Word.WdFindWrap.wdFindContinue;

            findObject.Execute(
                FindText: placeholder,
                MatchCase: false,
                MatchWholeWord: false,
                MatchWildcards: false,
                MatchSoundsLike: false,
                MatchAllWordForms: false,
                Forward: true,
                Wrap: wrap,
                Format: false,
                ReplaceWith: newValue,
                Replace: replaceAll
            );
        }
        // ონლაინ რეგისტრაციის დოკუმენტებთან დაკავშირებული ოპერაციები
        public void ShowDownloadSummary(List<string> failedDownloads, string successMessage = "✅ ყველა დოკუმენტი წარმატებით ჩაიტვირთა!", string errorTitle = "შეცდომები ჩამოტვირთვისას")
        {
            if (failedDownloads != null && failedDownloads.Any())
            {
                string message = "⚠️ ვერ ჩამოიტვირთა შემდეგი ფაილები:\n\n" + string.Join("\n", failedDownloads);
                MessageBox.Show(message, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(successMessage, "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public bool DownloadDocumentsForStudent(PendingStudent student, out List<string> failedFiles)
        {
            failedFiles = new List<string>();
            string folderPath = GetStudentFolder(student);
            Directory.CreateDirectory(folderPath);

            var files = new List<string> { student.IdCardPath };

            // თუ AdditionalDocsPath არის JSON array, გავფართოვოთ ლოგიკა
            if (!string.IsNullOrWhiteSpace(student.AdditionalDocsPath))
            {
                try
                {
                    var additionalDocs = JsonSerializer.Deserialize<List<string>>(student.AdditionalDocsPath);
                    if (additionalDocs != null && additionalDocs.Any())
                        files.AddRange(additionalDocs);
                    else
                        files.Add(student.AdditionalDocsPath); // fallback: add as single path
                }
                catch
                {
                    // fallback if not valid JSON
                    files.Add(student.AdditionalDocsPath);
                }
            }

            bool allSuccess = true;

            foreach (var relPath in files)
            {
                if (string.IsNullOrWhiteSpace(relPath)) continue;

                string fileUrl = FileServerBaseUrl + relPath.Replace("\\", "/");
                string fileName = Path.GetFileName(relPath);
                string originalFileName = Path.GetFileName(relPath);
                string destination = GetUniqueFileName(folderPath, student.FirstName, originalFileName);

                if (File.Exists(destination)) continue;

                try
                {
                    _webClient.DownloadFile(fileUrl, destination);
                }
                catch (Exception ex)
                {
                    allSuccess = false;
                    failedFiles.Add($"{student.FirstName} {student.LastName} - {fileName} ({ex.Message})");

                    // Retry fallback
                    var retry = MessageBox.Show(
                        $"ფაილის გადმოწერა ვერ მოხერხდა:\n{fileUrl}\nშეცდომა: {ex.Message}\n\nგსურთ ხელახლა სცადოთ?",
                        "დოკუმენტის გადმოწერა",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Warning);

                    if (retry == DialogResult.Retry)
                    {
                        try
                        {
                            _webClient.DownloadFile(fileUrl, destination);
                        }
                        catch (Exception retryEx)
                        {
                            failedFiles.Add($"{student.FirstName} {student.LastName} - {fileName} (Retry failed: {retryEx.Message})");
                        }
                    }
                }
            }

            return allSuccess && !failedFiles.Any();
        }
        public void OpenStudentFolder(PendingStudent student)
        {
            string folder = GetStudentFolder(student);
            if (Directory.Exists(folder))
            {
                System.Diagnostics.Process.Start("explorer", folder);
            }
            else
            {
                MessageBox.Show("📁 საქაღალდე ვერ მოიძებნა.");
            }
        }
        public void DownloadAllPendingDocuments(List<PendingStudent> students, ProgressBar progressBar = null, DataGridView gridView = null)
        {
            int total = students.Count;
            int current = 0;

            List<string> failedDownloads = new List<string>();

            foreach (var student in students)
            {
                bool success = DownloadDocumentsForStudent(student, out List<string> failedForStudent);

                if (failedForStudent.Any())
                {
                    failedDownloads.AddRange(failedForStudent);
                }

                // მწვანედ მონიშნე წარმატებული სტუდენტი
                if (success && gridView != null)
                {
                    foreach (DataGridViewRow row in gridView.Rows)
                    {
                        if (row.DataBoundItem is PendingStudent s && s.Id == student.Id)
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                            break;
                        }
                    }
                }

                // ProgressBar განახლება
                current++;
                if (progressBar != null)
                {
                    progressBar.Invoke((MethodInvoker)(() =>
                    {
                        progressBar.Value = (int)((double)current / total * 100);
                    }));
                }
            }

            // შეცდომების გამოჩენა ერთიანად
            ShowDownloadSummary(failedDownloads);

            // progressBar reset
            if (progressBar != null)
            {
                progressBar.Invoke((MethodInvoker)(() => progressBar.Value = 0));
            }
        }
        private string GetUniqueFileName(string folderPath, string firstName, string originalFileName)
        {
            string extension = Path.GetExtension(originalFileName);
            string baseName = $"BCC_{firstName}";
            int index = 1;

            string newFileName;
            do
            {
                newFileName = $"{baseName}_{index}{extension}";
                index++;
            }
            while (File.Exists(Path.Combine(folderPath, newFileName)));

            return Path.Combine(folderPath, newFileName);
        }
        private string GetStudentFolder(PendingStudent student)
        {
            return Path.Combine(DownloadBaseFolder, $"{student.FirstName}_{student.LastName}_{student.Id}");
        }

        private void SaveAsPdf(Word.Document doc)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.Title = "შეინახე ხელშეკრულება PDF-ად";
                saveFileDialog.FileName = "contract.pdf";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pdfPath = saveFileDialog.FileName;
                    doc.ExportAsFixedFormat(pdfPath, Word.WdExportFormat.wdExportFormatPDF);
                    MessageBox.Show($"ფაილი წარმატებით შეინახა: {pdfPath}", "PDF შენახვა", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}


