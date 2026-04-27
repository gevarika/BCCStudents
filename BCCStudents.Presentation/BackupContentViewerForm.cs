using BCCStudents.Presentation.Properties;
using System.Text;

namespace BCCStudents.Presentation
{
    public partial class BackupContentViewerForm : Form
    {
        private string _filePath;
        private string _fileName;

        public BackupContentViewerForm(string fileName, string filePath)
        {
            InitializeComponent();
            _fileName = fileName;
            _filePath = filePath; // filePath არის ფაილის გზა
            InitializeForm();
        }

        private void InitializeForm()
        {
            FormTitleHelper.SetTitle(this, string.Format(Resources.Backup_ViewContent_Title, _fileName));

            // ფორმის ზომის დაყენება
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // ტექსტური ველის ინიციალიზაცია
            txtContent.Font = new Font("Consolas", 10);
            txtContent.BackColor = Color.White;
            txtContent.ForeColor = Color.Black;
            txtContent.ReadOnly = true;
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.WordWrap = false;

            // შიგთავსის ჩატვირთვა
            LoadContent();

            // ღილაკების ივენთები
            btnSave.Click += BtnSave_Click;
            btnCopy.Click += BtnCopy_Click;
            btnSearch.Click += BtnSearch_Click;
            btnClose.Click += BtnClose_Click;
            btnTestEncoding.Click += BtnTestEncoding_Click;
            btnDecodeGeorgian.Click += BtnDecodeGeorgian_Click;
            btnDebug.Click += BtnDebug_Click;

            // ძიების ველის ივენთები
            txtSearch.KeyDown += TxtSearch_KeyDown;

            // კონტექსტური მენიუ ტექსტური ველისთვის
            txtContent.ContextMenuStrip = CreateContextMenu();

            // ფორმის დახურვის ივენთი
            this.FormClosing += BackupContentViewerForm_FormClosing;
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            // კოდირების არჩევა
            var encodingMenu = new ToolStripMenuItem("კოდირების არჩევა");
            encodingMenu.DropDownItems.Add("UTF-8", null, (s, e) => ReloadWithEncoding(Encoding.UTF8));
            encodingMenu.DropDownItems.Add("UTF-8 BOM", null, (s, e) => ReloadWithEncoding(new UTF8Encoding(true)));
            encodingMenu.DropDownItems.Add("Windows-1252", null, (s, e) => ReloadWithEncoding(Encoding.GetEncoding(1252)));
            encodingMenu.DropDownItems.Add("ISO-8859-1", null, (s, e) => ReloadWithEncoding(Encoding.GetEncoding("ISO-8859-1")));
            encodingMenu.DropDownItems.Add("ISO-8859-5", null, (s, e) => ReloadWithEncoding(Encoding.GetEncoding("ISO-8859-5")));
            encodingMenu.DropDownItems.Add("System Default", null, (s, e) => ReloadWithEncoding(Encoding.Default));

            contextMenu.Items.Add(encodingMenu);
            contextMenu.Items.Add(new ToolStripSeparator());

            // ქართული ტექსტის დეკოდირება/ტესტი
            contextMenu.Items.Add("ქართული ტექსტის დეკოდირება", null, (s, e) => BtnDecodeGeorgian_Click(s, e));
            contextMenu.Items.Add("ქართული ტექსტის ტესტი", null, (s, e) => BtnTestEncoding_Click(s, e));
            contextMenu.Items.Add(new ToolStripSeparator());

            // საერთო ოპერაციები
            contextMenu.Items.Add("ყველაფრის მონიშვნა", null, (s, e) => txtContent.SelectAll());
            contextMenu.Items.Add("კოპირება", null, (s, e) => txtContent.Copy());

            return contextMenu;
        }

        private void ReloadWithEncoding(Encoding encoding)
        {
            try
            {
                var content = File.ReadAllText(_filePath, encoding);
                txtContent.Text = content;
                lblInfo.Text = $"ფაილი: {_fileName} | კოდირება: {encoding.EncodingName} | ზომა: {FormatFileSize(content.Length)} | სტრიქონები: {content.Split('\n').Length}";

                // თუ კოდირება არ არის UTF-8, ვაჩვენებთ დამატებით ინფორმაციას
                if (encoding != Encoding.UTF8)
                {
                    MessageBox.Show(
                        $"ფაილი წაიკითხა სხვა კოდირებით: {encoding.EncodingName}",
                        Properties.Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"შეცდომა კოდირების შეცვლისას ({encoding.EncodingName}): {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadContent()
        {
            try
            {
                // დამხმარე ლოგები კონსოლზე
                Console.WriteLine($"Loading file: {_filePath}");
                Console.WriteLine($"File exists: {File.Exists(_filePath)}");

                if (File.Exists(_filePath))
                {
                    var fileInfo = new FileInfo(_filePath);
                    Console.WriteLine($"File size: {fileInfo.Length} bytes");

                    // ვკითხულობთ ფაილის პირველ 100 ბაიტს
                    byte[] firstBytes = File.ReadAllBytes(_filePath).Take(100).ToArray();
                    Console.WriteLine($"First 100 bytes: {BitConverter.ToString(firstBytes)}");
                }

                // ვცდილობთ ფაილის წაკითხვას სწორი კოდირებით
                string content = ReadFileWithProperEncoding(_filePath);
                txtContent.Text = content;
                lblInfo.Text = $"ფაილი: {_fileName} | ზომა: {FormatFileSize(content.Length)} | სტრიქონები: {content.Split('\n').Length}";

                // კურსორს ვაბრუნებთ ტექსტის დასაწყისში
                txtContent.SelectionStart = 0;
                txtContent.SelectionLength = 0;
                txtContent.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"შეცდომა შიგთავსის ჩატვირთვისას: {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string ReadFileWithProperEncoding(string filePath)
        {
            try
            {
                // ვკითხულობთ ფაილის ბაიტებს
                byte[] fileBytes = File.ReadAllBytes(filePath);
                Console.WriteLine($"File bytes length: {fileBytes.Length}");

                // ვამოწმებთ BOM-ს
                if (fileBytes.Length >= 3 && fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF)
                {
                    Console.WriteLine("UTF-8 BOM detected");
                    return Encoding.UTF8.GetString(fileBytes, 3, fileBytes.Length - 3);
                }

                // ვცდილობთ სხვადასხვა კოდირებას
                var encodings = new[]
                {
                    Encoding.UTF8,
                    new UTF8Encoding(false), // UTF-8 without BOM
                    Encoding.GetEncoding("ISO-8859-1"),
                    Encoding.GetEncoding("Windows-1252"),
                    Encoding.GetEncoding("ISO-8859-5"),
                    Encoding.GetEncoding("UTF-7"),
                    Encoding.GetEncoding("UTF-16"),
                    Encoding.GetEncoding("UTF-32"),
                    Encoding.Default
                };

                foreach (var encoding in encodings)
                {
                    try
                    {
                        string testText = encoding.GetString(fileBytes);

                        // ვამოწმებთ არის თუ არა ქართული ტექსტი
                        bool hasGeorgian = testText.Any(c => c >= '\u10A0' && c <= '\u10FF');
                        bool hasEncodedGeorgian = testText.Contains("Ã¡Æ’");

                        Console.WriteLine($"Testing {encoding.EncodingName}: hasGeorgian={hasGeorgian}, hasEncodedGeorgian={hasEncodedGeorgian}");

                        if (hasGeorgian && !hasEncodedGeorgian)
                        {
                            Console.WriteLine($"Found proper Georgian text with {encoding.EncodingName}");
                            return testText;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error with {encoding.EncodingName}: {ex.Message}");
                        continue;
                    }
                }

                // თუ ვერ ვიპოვეთ სწორი კოდირება, ვიყენებთ UTF-8-ს
                Console.WriteLine("No proper encoding found, using UTF-8");
                return Encoding.UTF8.GetString(fileBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReadFileWithProperEncoding: {ex.Message}");
                return File.ReadAllText(filePath);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = _fileName,
                    Filter = "SQL ფაილები (*.sql)|*.sql|ტექსტური ფაილები (*.txt)|*.txt|ყველა ფაილი (*.*)|*.*"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, txtContent.Text);
                    MessageBox.Show(
                        "ფაილი წარმატებით შეინახა!",
                        Properties.Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"შეცდომა ფაილის შენახვისას: {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtContent.SelectionLength > 0)
                {
                    Clipboard.SetText(txtContent.SelectedText);
                    MessageBox.Show(
                        "მონიშნული ტექსტი დაკოპირებულია!",
                        Properties.Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    Clipboard.SetText(txtContent.Text);
                    MessageBox.Show(
                        "მთელი შიგთავსი დაკოპირებულია!",
                        Properties.Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"შეცდომა კოპირების დროს: {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchText();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchText();
                e.Handled = true;
            }
        }

        private void SearchText()
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show(
                    "გთხოვთ, შეიყვანოთ საძიებო ტექსტი!",
                    Properties.Resources.Common_WarningTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ვპოულობთ შემდეგ შესაბამისობას
                int startIndex = txtContent.SelectionStart + txtContent.SelectionLength;
                int index = txtContent.Text.IndexOf(txtSearch.Text, startIndex, StringComparison.OrdinalIgnoreCase);

                if (index == -1)
                {
                    // თუ ვერ ვიპოვეთ, ვეძებთ თავიდან
                    index = txtContent.Text.IndexOf(txtSearch.Text, 0, StringComparison.OrdinalIgnoreCase);
                }

                if (index != -1)
                {
                    txtContent.SelectionStart = index;
                    txtContent.SelectionLength = txtSearch.Text.Length;
                    txtContent.ScrollToCaret();
                    txtContent.Focus();
                }
                else
                {
                    MessageBox.Show(
                        "ტექსტი ვერ მოიძებნა!",
                        Properties.Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"შეცდომა ძებნისას: {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BackupContentViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ფორმის დახურვისას არაფერს ვაკეთებთ
        }

        private string FormatFileSize(int characterCount)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = characterCount;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void BtnTestEncoding_Click(object sender, EventArgs e)
        {
            TestGeorgianText();
        }

        private void TestGeorgianText()
        {
            // ვამოწმებთ ქართული ტექსტის ნიმუშს
            string sampleText = txtContent.Text.Substring(0, Math.Min(2000, txtContent.Text.Length));

            // ვეძებთ სწორად UTF-8-ით კოდირებულ ქართულ სიმბოლოებს
            bool hasGeorgian = sampleText.Any(c => c >= '\u10A0' && c <= '\u10FF');

            // ვამოწმებთ UTF-8-ის არასწორად კოდირებულ ნიშნებს (მაგ. Ã¡Æ’ ...)
            bool hasEncodedGeorgian = sampleText.Contains("Ã¡Æ’") || sampleText.Contains("Ã¡Æ’");

            if (!hasGeorgian && !hasEncodedGeorgian)
            {
                MessageBox.Show(
                    "ქართულ ტექსტი ვერ მოიძებნა. სცადეთ კოდირების პრობლემა იყოს.",
                    Properties.Resources.Common_WarningTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else if (hasEncodedGeorgian && !hasGeorgian)
            {
                MessageBox.Show(
                    "ქართულ ტექსტი ჩანს არასწორად კოდირებული. გთხოვთ, სცადოთ კოდირების შეცვლა.",
                    Properties.Resources.Common_InfoTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "ქართულ ტექსტი სწორად იკითხება!",
                    Properties.Resources.Common_InfoTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private string DecodeGeorgianText(string encodedText)
        {
            try
            {
                // ვცდილობთ UTF-8-ით დეკოდირებას
                byte[] bytes = Encoding.UTF8.GetBytes(encodedText);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return encodedText;
            }
        }

        private string FixGeorgianEncoding(string text)
        {
            try
            {
                // თუ ტექსტი შეიცავს არასწორად კოდირებულ ქართულს, ვცდილობთ გასწორებას
                if (text.Contains("Ã¡Æ’"))
                {
                    Console.WriteLine("Found encoded Georgian text, attempting to fix...");

                    // ვცდილობთ სხვადასხვა კოდირებებს, რომ ვიპოვოთ სწორი ქართული ტექსტი
                    var encodings = new[]
                    {
                        Encoding.GetEncoding("ISO-8859-1"),
                        Encoding.GetEncoding("Windows-1252"),
                        Encoding.GetEncoding("ISO-8859-5"),
                        Encoding.GetEncoding("UTF-7"),
                        Encoding.Default
                    };

                    byte[] originalBytes = Encoding.UTF8.GetBytes(text);

                    foreach (var encoding in encodings)
                    {
                        try
                        {
                            // ვცდილობთ ტექსტის წაკითხვას მოცემული კოდირებით
                            string testText = encoding.GetString(originalBytes);

                            // ვამოწმებთ, შეიცავს თუ არა სწორი ქართული სიმბოლოებს
                            if (testText.Any(c => c >= '\u10A0' && c <= '\u10FF'))
                            {
                                Console.WriteLine($"Fixed Georgian text with {encoding.EncodingName}");
                                return testText;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    // თუ ვერ ვიპოვეთ, გადავდივართ მექანიკურ აღდგენაზე
                    return ManualGeorgianFix(text);
                }

                return text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FixGeorgianEncoding: {ex.Message}");
                return text;
            }
        }

        private string ManualGeorgianFix(string text)
        {
            try
            {
                // ვცდილობთ მექანიკურად აღვადგინოთ ქართული სიმბოლოები
                // ეს არის ბოლო ვარიანტი და ყოველთვის არ იმუშავებს

                // ვცდილობთ სხვადასხვა კოდირებებს
                byte[] bytes = Encoding.UTF8.GetBytes(text);

                // ვცდილობთ UTF-16-ით
                try
                {
                    string utf16Text = Encoding.Unicode.GetString(bytes);
                    if (utf16Text.Any(c => c >= '\u10A0' && c <= '\u10FF'))
                    {
                        return utf16Text;
                    }
                }
                catch { }

                // ვცდილობთ BigEndianUnicode-ით
                try
                {
                    string bigEndianText = Encoding.BigEndianUnicode.GetString(bytes);
                    if (bigEndianText.Any(c => c >= '\u10A0' && c <= '\u10FF'))
                    {
                        return bigEndianText;
                    }
                }
                catch { }

                return text;
            }
            catch
            {
                return text;
            }
        }

        private void BtnDecodeGeorgian_Click(object sender, EventArgs e)
        {
            try
            {
                // ვცდილობთ ქართული ტექსტის გასწორებას
                string originalText = txtContent.Text;
                string fixedText = FixGeorgianEncoding(originalText);

                if (fixedText != originalText)
                {
                    txtContent.Text = fixedText;
                    MessageBox.Show(
                        "ქართულ ტექსტი წარმატებით გასწორდა!",
                        Properties.Resources.Common_InfoTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // თუ ფაილიდანაც ვერ ვიპოვეთ სწორი კოდირებით
                    var encodings = new[]
                    {
                        Encoding.GetEncoding("ISO-8859-1"),
                        Encoding.GetEncoding("Windows-1252"),
                        Encoding.GetEncoding("ISO-8859-5"),
                        Encoding.GetEncoding("UTF-7"),
                        Encoding.Unicode,
                        Encoding.BigEndianUnicode,
                        Encoding.Default
                    };

                    foreach (var encoding in encodings)
                    {
                        try
                        {
                            byte[] bytes = File.ReadAllBytes(_filePath);
                            string testText = encoding.GetString(bytes);

                            // ვამოწმებთ შეიცავს თუ არა ტექსტი სწორ ქართულ სიმბოლოებს
                            if (testText.Any(c => c >= '\u10A0' && c <= '\u10FF'))
                            {
                                txtContent.Text = testText;
                                MessageBox.Show(
                                    $"ქართულ ტექსტი წარმატებით გასწორდა კოდირებით: {encoding.EncodingName}",
                                    Properties.Resources.Common_InfoTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                return;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    MessageBox.Show(
                        "ვერ მოხერხდა ქართული ტექსტის გასწორება. შეამოწმეთ ფაილი სხვა რედაქტორით ან კოდირებით.",
                        Properties.Resources.Common_WarningTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"შეცდომა დეკოდირებისას: {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnDebug_Click(object sender, EventArgs e)
        {
            try
            {
                var debugInfo = new StringBuilder();
                debugInfo.AppendLine($"ფაილი: {_filePath}");
                debugInfo.AppendLine($"ფაილი არსებობს: {File.Exists(_filePath)}");

                if (File.Exists(_filePath))
                {
                    var fileInfo = new FileInfo(_filePath);
                    debugInfo.AppendLine($"ფაილის ზომა: {fileInfo.Length} ბაიტი");

                    // წავისმენთ ფაილის პირველ 200 ბაიტს
                    byte[] firstBytes = File.ReadAllBytes(_filePath).Take(200).ToArray();
                    debugInfo.AppendLine($"პირველი 200 ბაიტი: {BitConverter.ToString(firstBytes)}");

                    // BOM-ის შემოწმება
                    if (firstBytes.Length >= 3)
                    {
                        debugInfo.AppendLine($"BOM: {firstBytes[0]:X2} {firstBytes[1]:X2} {firstBytes[2]:X2}");
                        if (firstBytes[0] == 0xEF && firstBytes[1] == 0xBB && firstBytes[2] == 0xBF)
                        {
                            debugInfo.AppendLine("UTF-8 BOM ნაპოვნია");
                        }
                    }

                    // ვამატებთ მიმდინარე ტექსტის სიგრძეს
                    string currentText = txtContent.Text;
                    debugInfo.AppendLine($"მიმდინარე ტექსტის სიგრძე: {currentText.Length}");

                    // პირველი 10 ქართული სიმბოლო
                    var georgianChars = currentText.Where(c => c >= '\u10A0' && c <= '\u10FF').Take(10).ToArray();
                    debugInfo.AppendLine($"ქართული სიმბოლოები (პირველი 10): {string.Join(", ", georgianChars)}");

                    // არასწორად კოდირებული ნიშნების რაოდენობა
                    int encodedCount = currentText.Split(new[] { "Ã¡Æ’" }, StringSplitOptions.None).Length - 1;
                    debugInfo.AppendLine($"დაშლილი ქართული სიმბოლოების რაოდენობა: {encodedCount}");

                    // სხვადასხვა კოდირების ტესტი
                    var encodings = new[] { "UTF-8", "ISO-8859-1", "Windows-1252", "ISO-8859-5" };
                    foreach (var encodingName in encodings)
                    {
                        try
                        {
                            var encoding = Encoding.GetEncoding(encodingName);
                            string testText = encoding.GetString(firstBytes);
                            bool hasGeorgian = testText.Any(c => c >= '\u10A0' && c <= '\u10FF');
                            debugInfo.AppendLine($"{encodingName}: ქართული ტექსტი = {hasGeorgian}");
                        }
                        catch
                        {
                            debugInfo.AppendLine($"{encodingName}: შეცდომა");
                        }
                    }
                }

                MessageBox.Show(
                    debugInfo.ToString(),
                    "დაბაგინგის ინფორმაცია",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"დაბაგინგის შეცდომა: {ex.Message}",
                    Properties.Resources.Common_ErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
