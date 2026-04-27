namespace BCCStudents.Presentation
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            lblLoginStatus = new Label();
            label2 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsername.Location = new Point(140, 53);
            txtUsername.Margin = new Padding(4, 3, 4, 3);
            txtUsername.MinimumSize = new Size(200, 30);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 30);
            txtUsername.TabIndex = 0;
            txtUsername.Text = "giorgi";
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.Location = new Point(140, 98);
            txtPassword.Margin = new Padding(4, 3, 4, 3);
            txtPassword.MinimumSize = new Size(200, 30);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(200, 30);
            txtPassword.TabIndex = 1;
            txtPassword.Text = "Giorgi1";
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(170, 173);
            btnLogin.Margin = new Padding(4, 3, 4, 3);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(144, 45);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "შესვლა";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // lblLoginStatus
            // 
            lblLoginStatus.AutoSize = true;
            lblLoginStatus.ForeColor = Color.Red;
            lblLoginStatus.Location = new Point(140, 140);
            lblLoginStatus.Margin = new Padding(4, 0, 4, 0);
            lblLoginStatus.Name = "lblLoginStatus";
            lblLoginStatus.Size = new Size(38, 15);
            lblLoginStatus.TabIndex = 2;
            lblLoginStatus.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(46, 57);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 2;
            label2.Text = "სახელი";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(42, 108);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 2;
            label3.Text = "პაროლი";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(414, 242);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(lblLoginStatus);
            Controls.Add(btnLogin);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblLoginStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
