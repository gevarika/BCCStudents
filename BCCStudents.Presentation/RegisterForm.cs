using BCCStudents.Application.Services;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Entities;

namespace BCCStudents.Presentation
{
    public partial class RegisterForm : Form
    {
        private readonly UserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public RegisterForm(IServiceProvider serviceProvider, UserService userService)
        {
            InitializeComponent();
            FormTitleHelper.SetTitle(this, "მომხმარებლის რეგისტრაცია");
            _userService = userService;
            _serviceProvider = serviceProvider;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string fullname = txtFullName.Text.Trim();
            string role = cmbUserRole.Text;
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (password != confirmPassword)
            {
                MessageBox.Show("პაროლები არ ემთხვევა!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _userService.RegisterUser(username, fullname, email, password, role);

                MessageBox.Show("რეგისტრაცია წარმატებით დასრულდა!", "ინფორმაცია", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                if (UserSession.FirstStart)
                {
                    var loginForm = _serviceProvider.GetRequiredService<LoginForm>();  // ან საჭირო სერვისებით
                    loginForm.ShowDialog();
                }
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "მომხმარებელი უკვე არსებობს", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("შეცდომა რეგისტრაციისას: " + ex.Message, "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

