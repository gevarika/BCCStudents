using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BCCStudents.Infrastructure.Data;
using System.Text;
using System.Linq;

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
            _filePath = filePath; // filePath áƒáƒ áƒ˜áƒ¡ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ’áƒ–áƒ
            InitializeForm();
        }

        private void InitializeForm()
        {
            FormTitleHelper.SetTitle(this, $"áƒ‘áƒ”áƒ¥áƒáƒžáƒ˜áƒ¡ áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜ - {_fileName}");
            
            // áƒ¤áƒáƒ áƒ›áƒ˜áƒ¡ áƒ–áƒáƒ›áƒ˜áƒ¡ áƒ“áƒáƒ§áƒ”áƒœáƒ”áƒ‘áƒ
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            
            // áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ•áƒ”áƒšáƒ˜áƒ¡ áƒ˜áƒœáƒ˜áƒªáƒ˜áƒáƒšáƒ˜áƒ–áƒáƒªáƒ˜áƒ
            txtContent.Font = new Font("Consolas", 10);
            txtContent.BackColor = Color.White;
            txtContent.ForeColor = Color.Black;
            txtContent.ReadOnly = true;
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.WordWrap = false;
            
            // áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ
            LoadContent();
            
            // áƒ¦áƒ˜áƒšáƒáƒ™áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ”áƒ‘áƒ˜
            btnSave.Click += BtnSave_Click;
            btnCopy.Click += BtnCopy_Click;
            btnSearch.Click += BtnSearch_Click;
            btnClose.Click += BtnClose_Click;
            btnTestEncoding.Click += BtnTestEncoding_Click;
            btnDecodeGeorgian.Click += BtnDecodeGeorgian_Click;
            btnDebug.Click += BtnDebug_Click;
            
            // áƒ«áƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ•áƒ”áƒšáƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ”áƒ‘áƒ˜
            txtSearch.KeyDown += TxtSearch_KeyDown;
            
            // áƒ™áƒáƒœáƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ›áƒ”áƒœáƒ˜áƒ£ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ•áƒ”áƒšáƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
            txtContent.ContextMenuStrip = CreateContextMenu();
            
            // áƒ¤áƒáƒ áƒ›áƒ˜áƒ¡ áƒ“áƒáƒ®áƒ£áƒ áƒ•áƒ˜áƒ¡ áƒ˜áƒ•áƒ”áƒœáƒ—áƒ˜
            this.FormClosing += BackupContentViewerForm_FormClosing;
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            
            // áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒáƒ áƒ©áƒ”áƒ•áƒ
            var encodingMenu = new ToolStripMenuItem("áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒáƒ áƒ©áƒ”áƒ•áƒ");
            encodingMenu.DropDownItems.Add("UTF-8", null, (s, e) => ReloadWithEncoding(Encoding.UTF8));
            encodingMenu.DropDownItems.Add("UTF-8 BOM", null, (s, e) => ReloadWithEncoding(new UTF8Encoding(true)));
            encodingMenu.DropDownItems.Add("Windows-1252", null, (s, e) => ReloadWithEncoding(Encoding.GetEncoding(1252)));
            encodingMenu.DropDownItems.Add("ISO-8859-1", null, (s, e) => ReloadWithEncoding(Encoding.GetEncoding("ISO-8859-1")));
            encodingMenu.DropDownItems.Add("ISO-8859-5", null, (s, e) => ReloadWithEncoding(Encoding.GetEncoding("ISO-8859-5")));
            encodingMenu.DropDownItems.Add("System Default", null, (s, e) => ReloadWithEncoding(Encoding.Default));
            
            contextMenu.Items.Add(encodingMenu);
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ“áƒ”áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ
            contextMenu.Items.Add("áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ“áƒ”áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ", null, (s, e) => BtnDecodeGeorgian_Click(s, e));
            contextMenu.Items.Add("áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜", null, (s, e) => BtnTestEncoding_Click(s, e));
            contextMenu.Items.Add(new ToolStripSeparator());
            
            // áƒ¡áƒ®áƒ•áƒ áƒáƒžáƒ”áƒ áƒáƒªáƒ˜áƒ”áƒ‘áƒ˜
            contextMenu.Items.Add("áƒ§áƒ•áƒ”áƒšáƒáƒ¤áƒ áƒ˜áƒ¡ áƒ›áƒáƒœáƒ˜áƒ¨áƒ•áƒœáƒ", null, (s, e) => txtContent.SelectAll());
            contextMenu.Items.Add("áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ", null, (s, e) => txtContent.Copy());
            
            return contextMenu;
        }

        private void ReloadWithEncoding(Encoding encoding)
        {
            try
            {
                var content = File.ReadAllText(_filePath, encoding);
                txtContent.Text = content;
                lblInfo.Text = $"áƒ¤áƒáƒ˜áƒšáƒ˜: {_fileName} | áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ: {encoding.EncodingName} | áƒ–áƒáƒ›áƒ: {FormatFileSize(content.Length)} | áƒ¡áƒ¢áƒ áƒ˜áƒ¥áƒáƒœáƒ”áƒ‘áƒ˜: {content.Split('\n').Length}";
                
                // áƒ•áƒáƒ©áƒ•áƒ”áƒœáƒ”áƒ‘áƒ— áƒ›áƒªáƒ˜áƒ áƒ” áƒ›áƒ”áƒ¡áƒ˜áƒ¯áƒ¡ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ¨áƒ”áƒ¡áƒáƒ®áƒ”áƒ‘
                if (encoding != Encoding.UTF8)
                {
                    MessageBox.Show($"áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ£áƒšáƒ˜áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—: {encoding.EncodingName}", 
                        "áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ— {encoding.EncodingName}: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadContent()
        {
            try
            {
                // áƒ“áƒ”áƒ‘áƒáƒ’áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ
                Console.WriteLine($"Loading file: {_filePath}");
                Console.WriteLine($"File exists: {File.Exists(_filePath)}");
                
                if (File.Exists(_filePath))
                {
                    var fileInfo = new FileInfo(_filePath);
                    Console.WriteLine($"File size: {fileInfo.Length} bytes");
                    
                    // áƒ•áƒ™áƒ˜áƒ—áƒ®áƒ£áƒšáƒáƒ‘áƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒžáƒ˜áƒ áƒ•áƒ”áƒš 100 áƒ‘áƒáƒ˜áƒ¢áƒ¡
                    byte[] firstBytes = File.ReadAllBytes(_filePath).Take(100).ToArray();
                    Console.WriteLine($"First 100 bytes: {BitConverter.ToString(firstBytes)}");
                }
                
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¡áƒ®áƒ•áƒáƒ“áƒáƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¬áƒáƒ¡áƒáƒ™áƒ˜áƒ—áƒ®áƒáƒ“
                string content = ReadFileWithProperEncoding(_filePath);
                txtContent.Text = content;
                lblInfo.Text = $"áƒ¤áƒáƒ˜áƒšáƒ˜: {_fileName} | áƒ–áƒáƒ›áƒ: {FormatFileSize(content.Length)} | áƒ¡áƒ¢áƒ áƒ˜áƒ¥áƒáƒœáƒ”áƒ‘áƒ˜: {content.Split('\n').Length}";
                
                // áƒ•áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ— áƒ™áƒ£áƒ áƒ¡áƒáƒ áƒ¡ áƒ“áƒáƒ¡áƒáƒ¬áƒ§áƒ˜áƒ¡áƒ¨áƒ˜
                txtContent.SelectionStart = 0;
                txtContent.SelectionLength = 0;
                txtContent.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜áƒ¡ áƒ©áƒáƒ¢áƒ•áƒ˜áƒ áƒ—áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ReadFileWithProperEncoding(string filePath)
        {
            try
            {
                // áƒ•áƒ™áƒ˜áƒ—áƒ®áƒ£áƒšáƒáƒ‘áƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ‘áƒáƒ˜áƒ¢áƒ”áƒ‘áƒ¡
                byte[] fileBytes = File.ReadAllBytes(filePath);
                Console.WriteLine($"File bytes length: {fileBytes.Length}");
                
                // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— BOM-áƒ¡
                if (fileBytes.Length >= 3 && fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF)
                {
                    Console.WriteLine("UTF-8 BOM detected");
                    return Encoding.UTF8.GetString(fileBytes, 3, fileBytes.Length - 3);
                }
                
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¡áƒ®áƒ•áƒáƒ“áƒáƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
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
                        
                        // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜
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
                
                // áƒ—áƒ£ áƒáƒ áƒáƒ¤áƒ”áƒ áƒ˜ áƒ›áƒ£áƒ¨áƒáƒáƒ‘áƒ¡, áƒ•áƒáƒ‘áƒ áƒ£áƒœáƒ”áƒ‘áƒ— UTF-8-áƒ˜áƒ—
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
                    Filter = "SQL áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜ (*.sql)|*.sql|áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ£áƒ áƒ˜ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜ (*.txt)|*.txt|áƒ§áƒ•áƒ”áƒšáƒ áƒ¤áƒáƒ˜áƒšáƒ˜ (*.*)|*.*"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, txtContent.Text);
                    MessageBox.Show("áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ¨áƒ”áƒœáƒáƒ®áƒ£áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ¨áƒ”áƒœáƒáƒ®áƒ•áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtContent.SelectionLength > 0)
                {
                    Clipboard.SetText(txtContent.SelectedText);
                    MessageBox.Show("áƒ›áƒáƒœáƒ˜áƒ¨áƒœáƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒ“áƒáƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Clipboard.SetText(txtContent.Text);
                    MessageBox.Show("áƒ›áƒ—áƒ”áƒšáƒ˜ áƒ¨áƒ˜áƒ’áƒ—áƒáƒ•áƒ¡áƒ˜ áƒ“áƒáƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜áƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒ¨áƒ”áƒ˜áƒ§áƒ•áƒáƒœáƒáƒ— áƒ«áƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜!", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // áƒ•áƒžáƒáƒ•áƒáƒ— áƒ¨áƒ”áƒ›áƒ“áƒ”áƒ’áƒ˜ áƒ¨áƒ”áƒ›áƒ—áƒ®áƒ•áƒ”áƒ•áƒ
                int startIndex = txtContent.SelectionStart + txtContent.SelectionLength;
                int index = txtContent.Text.IndexOf(txtSearch.Text, startIndex, StringComparison.OrdinalIgnoreCase);
                
                if (index == -1)
                {
                    // áƒ—áƒ£ áƒ•áƒ”áƒ  áƒ•áƒ˜áƒžáƒáƒ•áƒ”áƒ—, áƒ•áƒ˜áƒ¬áƒ§áƒ”áƒ‘áƒ— áƒ“áƒáƒ¡áƒáƒ¬áƒ§áƒ˜áƒ¡áƒ˜áƒ“áƒáƒœ
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
                    MessageBox.Show("áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ!", "áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ«áƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BackupContentViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // áƒ¤áƒáƒ áƒ›áƒ˜áƒ¡ áƒ“áƒáƒ®áƒ£áƒ áƒ•áƒ˜áƒ¡áƒáƒ¡ áƒáƒ áƒáƒ¤áƒ”áƒ áƒ˜ áƒ’áƒ•áƒ­áƒ˜áƒ áƒ“áƒ”áƒ‘áƒ
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
            // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒœáƒáƒ¬áƒ˜áƒšáƒ¡
            string sampleText = txtContent.Text.Substring(0, Math.Min(2000, txtContent.Text.Length));
            
            // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ¡ (UTF-8 áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ“áƒ˜áƒáƒžáƒáƒ–áƒáƒœáƒ˜)
            bool hasGeorgian = sampleText.Any(c => c >= '\u10A0' && c <= '\u10FF');
            
            // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒáƒ¡áƒ”áƒ•áƒ” UTF-8 áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ˜áƒ¡ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒš áƒ•áƒ”áƒ áƒ¡áƒ˜áƒ”áƒ‘áƒ¡
            bool hasEncodedGeorgian = sampleText.Contains("Ã¡Æ’") || sampleText.Contains("Ã¡Æ’");
            
            if (!hasGeorgian && !hasEncodedGeorgian)
            {
                MessageBox.Show("áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒ•áƒ”áƒ  áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ. áƒ¨áƒ”áƒ˜áƒ«áƒšáƒ”áƒ‘áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒžáƒ áƒáƒ‘áƒšáƒ”áƒ›áƒ áƒ˜áƒ§áƒáƒ¡.", 
                    "áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ’áƒáƒ¤áƒ áƒ—áƒ®áƒ˜áƒšáƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (hasEncodedGeorgian && !hasGeorgian)
            {
                MessageBox.Show("áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒáƒ áƒ˜áƒ¡ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¤áƒáƒ áƒ›áƒ˜áƒ—. áƒ’áƒ—áƒ®áƒáƒ•áƒ—, áƒ¨áƒ”áƒªáƒ•áƒáƒšáƒáƒ— áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ.", 
                    "áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ›áƒáƒ˜áƒ«áƒ”áƒ‘áƒœáƒ!", 
                    "áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string DecodeGeorgianText(string encodedText)
        {
            try
            {
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ’áƒáƒ•áƒ¨áƒ˜áƒ¤áƒ áƒáƒ— UTF-8 áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜
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
                // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒš áƒ¥áƒáƒ áƒ—áƒ£áƒš áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ¡
                if (text.Contains("Ã¡Æ’"))
                {
                    Console.WriteLine("Found encoded Georgian text, attempting to fix...");
                    
                    // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¡áƒ®áƒ•áƒáƒ“áƒáƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
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
                            // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ•áƒ˜áƒžáƒáƒ•áƒáƒ— áƒ¡áƒ¬áƒáƒ áƒ˜ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ
                            string testText = encoding.GetString(originalBytes);
                            
                            // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜
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
                    
                    // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ®áƒ”áƒšáƒ˜áƒ— áƒ’áƒáƒ•áƒáƒ¡áƒ¬áƒáƒ áƒáƒ— áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ
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
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ®áƒ”áƒšáƒ˜áƒ— áƒ’áƒáƒ•áƒáƒ¡áƒ¬áƒáƒ áƒáƒ— áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ˜
                // áƒ”áƒ¡ áƒáƒ áƒ˜áƒ¡ áƒ áƒ—áƒ£áƒšáƒ˜ áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜, áƒ›áƒáƒ’áƒ áƒáƒ› áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ—
                
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¡áƒ®áƒ•áƒáƒ“áƒáƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ™áƒáƒ›áƒ‘áƒ˜áƒœáƒáƒªáƒ˜áƒ”áƒ‘áƒ¡
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— UTF-16-áƒ˜áƒ—
                try
                {
                    string utf16Text = Encoding.Unicode.GetString(bytes);
                    if (utf16Text.Any(c => c >= '\u10A0' && c <= '\u10FF'))
                    {
                        return utf16Text;
                    }
                }
                catch { }
                
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— BigEndianUnicode-áƒ˜áƒ—
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
                // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ’áƒáƒ¨áƒ˜áƒ¤áƒ•áƒ áƒáƒ¡
                string originalText = txtContent.Text;
                string fixedText = FixGeorgianEncoding(originalText);
                
                if (fixedText != originalText)
                {
                    txtContent.Text = fixedText;
                    MessageBox.Show("áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ’áƒáƒ¨áƒ˜áƒ¤áƒ áƒ£áƒšáƒ˜áƒ!", 
                        "áƒ“áƒ”áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // áƒ•áƒªáƒ“áƒ˜áƒšáƒáƒ‘áƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ“áƒáƒœ áƒ®áƒ”áƒšáƒáƒ®áƒšáƒ áƒ¬áƒáƒ¡áƒáƒ™áƒ˜áƒ—áƒ®áƒáƒ“ áƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
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
                            
                            // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒáƒ áƒ˜áƒ¡ áƒ—áƒ£ áƒáƒ áƒ áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜
                            if (testText.Any(c => c >= '\u10A0' && c <= '\u10FF'))
                            {
                                txtContent.Text = testText;
                                MessageBox.Show($"áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ áƒ¬áƒáƒ áƒ›áƒáƒ¢áƒ”áƒ‘áƒ˜áƒ— áƒ’áƒáƒ¨áƒ˜áƒ¤áƒ áƒ£áƒšáƒ˜áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—: {encoding.EncodingName}", 
                                    "áƒ“áƒ”áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    
                    MessageBox.Show("áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ’áƒáƒ¨áƒ˜áƒ¤áƒ•áƒ áƒ. áƒ¨áƒ”áƒ˜áƒ«áƒšáƒ”áƒ‘áƒ áƒ¤áƒáƒ˜áƒšáƒ˜ áƒ£áƒ™áƒ•áƒ” áƒ¡áƒ¬áƒáƒ áƒáƒ“ áƒáƒ áƒ˜áƒ¡ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜.", 
                        "áƒ“áƒ”áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ áƒ“áƒ”áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ áƒáƒ¡: {ex.Message}", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDebug_Click(object sender, EventArgs e)
        {
            try
            {
                var debugInfo = new StringBuilder();
                debugInfo.AppendLine($"áƒ¤áƒáƒ˜áƒšáƒ˜: {_filePath}");
                debugInfo.AppendLine($"áƒ¤áƒáƒ˜áƒšáƒ˜ áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ¡: {File.Exists(_filePath)}");
                
                if (File.Exists(_filePath))
                {
                    var fileInfo = new FileInfo(_filePath);
                    debugInfo.AppendLine($"áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒ–áƒáƒ›áƒ: {fileInfo.Length} áƒ‘áƒáƒ˜áƒ¢áƒ˜");
                    
                    // áƒ•áƒ™áƒ˜áƒ—áƒ®áƒ£áƒšáƒáƒ‘áƒ— áƒ¤áƒáƒ˜áƒšáƒ˜áƒ¡ áƒžáƒ˜áƒ áƒ•áƒ”áƒš 200 áƒ‘áƒáƒ˜áƒ¢áƒ¡
                    byte[] firstBytes = File.ReadAllBytes(_filePath).Take(200).ToArray();
                    debugInfo.AppendLine($"áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜ 200 áƒ‘áƒáƒ˜áƒ¢áƒ˜: {BitConverter.ToString(firstBytes)}");
                    
                    // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— BOM-áƒ¡
                    if (firstBytes.Length >= 3)
                    {
                        debugInfo.AppendLine($"BOM: {firstBytes[0]:X2} {firstBytes[1]:X2} {firstBytes[2]:X2}");
                        if (firstBytes[0] == 0xEF && firstBytes[1] == 0xBB && firstBytes[2] == 0xBF)
                        {
                            debugInfo.AppendLine("UTF-8 BOM áƒœáƒáƒžáƒáƒ•áƒœáƒ˜áƒ");
                        }
                    }
                    
                    // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ¡
                    string currentText = txtContent.Text;
                    debugInfo.AppendLine($"áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ” áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜áƒ¡ áƒ¡áƒ˜áƒ’áƒ áƒ«áƒ”: {currentText.Length}");
                    
                    // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ¡
                    var georgianChars = currentText.Where(c => c >= '\u10A0' && c <= '\u10FF').Take(10).ToArray();
                    debugInfo.AppendLine($"áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ˜ (áƒžáƒ˜áƒ áƒ•áƒ”áƒšáƒ˜ 10): {string.Join(", ", georgianChars)}");
                    
                    // áƒ•áƒ”áƒ«áƒ”áƒ‘áƒ— áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒš áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ¡
                    int encodedCount = currentText.Split(new[] { "Ã¡Æ’" }, StringSplitOptions.None).Length - 1;
                    debugInfo.AppendLine($"áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ£áƒšáƒ˜ áƒ¡áƒ˜áƒ›áƒ‘áƒáƒšáƒáƒ”áƒ‘áƒ˜áƒ¡ áƒ áƒáƒáƒ“áƒ”áƒœáƒáƒ‘áƒ: {encodedCount}");
                    
                    // áƒ•áƒáƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ— áƒ¡áƒ®áƒ•áƒáƒ“áƒáƒ¡áƒ®áƒ•áƒ áƒ™áƒáƒ“áƒ˜áƒ áƒ”áƒ‘áƒ˜áƒ—
                    var encodings = new[] { "UTF-8", "ISO-8859-1", "Windows-1252", "ISO-8859-5" };
                    foreach (var encodingName in encodings)
                    {
                        try
                        {
                            var encoding = Encoding.GetEncoding(encodingName);
                            string testText = encoding.GetString(firstBytes);
                            bool hasGeorgian = testText.Any(c => c >= '\u10A0' && c <= '\u10FF');
                            debugInfo.AppendLine($"{encodingName}: áƒ¥áƒáƒ áƒ—áƒ£áƒšáƒ˜ áƒ¢áƒ”áƒ¥áƒ¡áƒ¢áƒ˜ = {hasGeorgian}");
                        }
                        catch
                        {
                            debugInfo.AppendLine($"{encodingName}: áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ");
                        }
                    }
                }
                
                MessageBox.Show(debugInfo.ToString(), "áƒ“áƒ”áƒ‘áƒáƒ’áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"áƒ“áƒ”áƒ‘áƒáƒ’áƒ˜áƒœáƒ’áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ: {ex.Message}", "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 
