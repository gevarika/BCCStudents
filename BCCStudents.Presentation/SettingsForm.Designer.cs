namespace BCCStudents.Presentation
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.setStudyStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // setStudyStart
            // 
            this.setStudyStart.Location = new System.Drawing.Point(12, 22);
            this.setStudyStart.Name = "setStudyStart";
            this.setStudyStart.Size = new System.Drawing.Size(180, 23);
            this.setStudyStart.TabIndex = 0;
            this.setStudyStart.Text = "სწავლის დაწყების დაყენება";
            this.setStudyStart.UseVisualStyleBackColor = true;
            this.setStudyStart.Click += new System.EventHandler(this.setStudyStart_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 69);
            this.Controls.Add(this.setStudyStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SettingsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button setStudyStart;
    }
}
