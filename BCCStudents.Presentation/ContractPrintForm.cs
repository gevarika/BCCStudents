using BCCStudents.Application.Services;
using System.Diagnostics;
using System.Drawing.Printing;
using Word = Microsoft.Office.Interop.Word;

namespace BCCStudents.Presentation
{
    public partial class ContractPrintForm : Form
    {
        private readonly DocumentService _documentService;
        private Dictionary<string, string> replacementData;
        public Word.Application wordApp;
        string contractFilePath = "";
        public ContractPrintForm(DocumentService documentService, string phoneNumber, decimal tuitionFee, string parentName, string firstName, string lastName, string address, long idNumber)
        {
            InitializeComponent();
            _documentService = documentService;
            FormTitleHelper.SetTitle(this, "ხელშეკრულების ბეჭდვა");
            replacementData = new Dictionary<string, string>
        {
            { "{FirstName}", firstName },
            { "{LastName}", lastName },
            { "{IdNumber}", idNumber.ToString() },
            { "{ParrentName}", parentName },
            { "{Address}", address },
            { "{Price}", tuitionFee.ToString() },
            { "{PhoneNumber}", phoneNumber }
        };
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(contractFilePath))
                    throw new Exception("გთხოვთ აირჩიოთ ხელშეკრულების ფაილი.");

                _documentService.GenerateAndPrintContract(contractFilePath, replacementData);
                MessageBox.Show("ხელშეკრულება წარმატებით გაიგზავნა ბეჭდვაზე!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა: " + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            /*//string documentPath = @"D:\Contracts\contract_template.docx"; // შენი ხელშეკრულების ფაილი
            string tempPath = Path.Combine(Path.GetTempPath(), "contract_temp.docx"); // დროებითი ფაილი
                                                                                      // Word Interop-ის გაშვება
            
            try
            {
                // კოპირება დროებით ფაილში, რომ არ დავაზიანოთ ორიგინალი
                File.Copy(contractFilePath, tempPath, true);
                Word.Application wordApp = new Word.Application();
                Word.Document doc = wordApp.Documents.Open(tempPath);


                // **მონაცემების ჩანაცვლება**
                ReplaceText(doc, "{FirstName}", FirstName);
                ReplaceText(doc, "{LastName}", LastName);
                ReplaceText(doc, "{IdNumber}", IdNumber);
                ReplaceText(doc, "{ParrentName}", ParrentName);
                ReplaceText(doc, "{Address}", Address);
                ReplaceText(doc, "{Price}", TuitionFee);

                // **პრინტზე გაგზავნა – მხოლოდ ერთ გვერდზე ბეჭდვა (No Duplex)**
                object copies = 1;
                object pages = Type.Missing;
                object range = Word.WdPrintOutRange.wdPrintAllDocument;
                object item = Word.WdPrintOutItem.wdPrintDocumentContent;
                object pageType = Word.WdPrintOutPages.wdPrintAllPages;
                object manualDuplexPrint = false; // გამორთავს ორმხრივ ბეჭდვას

                doc.PrintOut(
                    Background: false,
                    Range: ref range,
                    Item: ref item,
                    Copies: ref copies,
                    Pages: ref pages,
                    PageType: ref pageType,
                    ManualDuplexPrint: ref manualDuplexPrint
                );
                richTextBoxContract.Text = doc.ToString();
                // **პრინტზე გაგზავნა**
                doc.PrintOut(); // ბეჭდვა
                doc.Close(false); // დახურვა ცვლილებების გარეშე

                wordApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა: " + ex.Message);
                CloseWordProcesses();
            }*/

        }
        private void CloseWordProcesses()
        {
            foreach (var process in Process.GetProcessesByName("WINWORD"))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Word-ის პროცესის დახურვა ვერ მოხერხდა: " + ex.Message);
                }
            }
        }
        // **მეთოდი ტექსტის ჩანაცვლებისთვის**
        private void ReplaceText(Word.Document doc, string placeholder, string newValue)
        {
            Word.Find findObject = doc.Content.Find;
            findObject.ClearFormatting();
            findObject.Text = placeholder;

            findObject.Replacement.ClearFormatting();
            findObject.Replacement.Text = newValue;

            object replaceAll = Word.WdReplace.wdReplaceAll;
            findObject.Execute(Replace: ref replaceAll);
        }
        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;
            previewDialog.Document = printDoc;
            previewDialog.ShowDialog();
        }
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (RichTextBox rtb = new RichTextBox())
            {
                rtb.Rtf = richTextBoxContract.Rtf; // ვიღებთ ფორმატირებულ ტექსტს
                e.Graphics.MeasureString(rtb.Text, richTextBoxContract.Font);
                e.Graphics.DrawString(rtb.Text, richTextBoxContract.Font, Brushes.Black, new PointF(50, 50));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                contractFilePath = openFileDialog.FileName;
            }
            /*OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                contractFilePath = openFileDialog.FileName;
                documentPath = contractFilePath;
                //LoadAndReplaceContract();
            }*/
        }

        private void ContractPrintForm_Load(object sender, EventArgs e)
        {
            //LoadAndReplaceContract();
        }
        /*private void LoadAndReplaceContract()
        {
            try
            {
                wordApp = new Word.Application();
                wordDoc = wordApp.Documents.Open(documentPath);
                wordApp.Visible = false;

                foreach (var pair in replacementData)
                {
                    FindAndReplace(pair.Key, pair.Value);
                }

                // დროებითი ფაილი RTF ფორმატისთვის
                string tempRtfPath = Path.Combine(Path.GetTempPath(), "temp_contract.rtf");
                wordDoc.SaveAs2(tempRtfPath, Word.WdSaveFormat.wdFormatRTF);
                wordDoc.Close();
                wordApp.Quit();

                // RTF ფაილის ჩატვირთვა RichTextBox-ში
                richTextBoxContract.LoadFile(tempRtfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა ფაილის დამუშავებისას: " + ex.Message);
            }
        }*/
        /*private void FindAndReplace(string placeholder, string value)
        {
            Word.Find findObject = wordApp.Selection.Find;
            findObject.Text = placeholder;
            findObject.Replacement.Text = value;
            findObject.Execute(Replace: Word.WdReplace.wdReplaceAll);
        }*/
    }
}

