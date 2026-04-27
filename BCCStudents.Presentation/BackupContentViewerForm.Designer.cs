namespace BCCStudents.Presentation
{
    partial class BackupContentViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackupContentViewerForm));
            txtContent = new TextBox();
            panel1 = new Panel();
            btnClose = new Button();
            btnSearch = new Button();
            txtSearch = new TextBox();
            label1 = new Label();
            btnCopy = new Button();
            btnSave = new Button();
            lblInfo = new Label();
            btnTestEncoding = new Button();
            btnDecodeGeorgian = new Button();
            btnDebug = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // txtContent
            // 
            txtContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtContent.Location = new Point(14, 92);
            txtContent.Margin = new Padding(4, 3, 4, 3);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.Size = new Size(1496, 666);
            txtContent.TabIndex = 0;
            txtContent.WordWrap = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.LightGray;
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(btnSearch);
            panel1.Controls.Add(txtSearch);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnCopy);
            panel1.Controls.Add(btnSave);
            panel1.Controls.Add(lblInfo);
            panel1.Controls.Add(btnTestEncoding);
            panel1.Controls.Add(btnDecodeGeorgian);
            panel1.Controls.Add(btnDebug);
            panel1.Location = new Point(14, 14);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1497, 72);
            panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Location = new Point(1380, 25);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(88, 27);
            btnClose.TabIndex = 6;
            btnClose.Text = "დახურვა";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(1175, 23);
            btnSearch.Margin = new Padding(4, 3, 4, 3);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(88, 27);
            btnSearch.TabIndex = 5;
            btnSearch.Text = "ძებნა";
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(852, 25);
            txtSearch.Margin = new Padding(4, 3, 4, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(93, 23);
            txtSearch.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(798, 29);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 3;
            label1.Text = "ძებნა:";
            // 
            // btnCopy
            // 
            btnCopy.Location = new Point(583, 25);
            btnCopy.Margin = new Padding(4, 3, 4, 3);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(88, 27);
            btnCopy.TabIndex = 2;
            btnCopy.Text = "კოპირება";
            btnCopy.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(489, 25);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 1;
            btnSave.Text = "შენახვა";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(14, 29);
            lblInfo.Margin = new Padding(4, 0, 4, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(41, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "ინფო";
            // 
            // btnTestEncoding
            // 
            btnTestEncoding.Location = new Point(678, 25);
            btnTestEncoding.Margin = new Padding(4, 3, 4, 3);
            btnTestEncoding.Name = "btnTestEncoding";
            btnTestEncoding.Size = new Size(88, 27);
            btnTestEncoding.TabIndex = 7;
            btnTestEncoding.Text = "ტესტი";
            btnTestEncoding.UseVisualStyleBackColor = true;
            // 
            // btnDecodeGeorgian
            // 
            btnDecodeGeorgian.Location = new Point(986, 23);
            btnDecodeGeorgian.Margin = new Padding(4, 3, 4, 3);
            btnDecodeGeorgian.Name = "btnDecodeGeorgian";
            btnDecodeGeorgian.Size = new Size(88, 27);
            btnDecodeGeorgian.TabIndex = 8;
            btnDecodeGeorgian.Text = "დეკოდირება";
            btnDecodeGeorgian.UseVisualStyleBackColor = true;
            // 
            // btnDebug
            // 
            btnDebug.Location = new Point(1080, 23);
            btnDebug.Margin = new Padding(4, 3, 4, 3);
            btnDebug.Name = "btnDebug";
            btnDebug.Size = new Size(88, 27);
            btnDebug.TabIndex = 9;
            btnDebug.Text = "დებუგი";
            btnDebug.UseVisualStyleBackColor = true;
            // 
            // BackupContentViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1525, 773);
            Controls.Add(panel1);
            Controls.Add(txtContent);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(931, 571);
            Name = "BackupContentViewerForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "ბექაფის შიგთავსი";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnTestEncoding;
        private System.Windows.Forms.Button btnDecodeGeorgian;
        private System.Windows.Forms.Button btnDebug;
    }
} 
