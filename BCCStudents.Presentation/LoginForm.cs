using BCCStudents.Domain.Entities;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using BCCStudents.Application.Services.Update;
using BCCStudents.Presentation.Properties;


namespace BCCStudents.Presentation
{
    public partial class LoginForm : Form
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private bool _updateRunning;
        //private readonly UserSession userSession;
        //UserSession UserSession = new UserSession();
        public static int LoggedInUserId { get; private set; }

        public LoginForm(IServiceProvider serviceProvider, IUserService userService)
        {
            InitializeComponent();
            FormTitleHelper.SetTitle(this, "ავტორიზაცია");
            _userService = userService;
            _serviceProvider = serviceProvider;
            this.Shown += LoginForm_Shown;
        }
        private async void LoginForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // ავტომატური განახლების შემოწმება მხოლოდ თუ ჩართულია
                if (Settings.Default.AutoUpdateEnabled)
                {
                    _updateRunning = true;
                    ToggleLoginControls(false);
                    var updater = _serviceProvider.GetService<UpdateService>();
                    if (updater == null) return;
                    var manifest = await updater.GetManifestAsync(System.Threading.CancellationToken.None);
                    if (manifest == null) return;
                var current = updater.GetCurrentVersion();
                var latest = new Version(manifest.latestVersion);
                if (!updater.IsNewer(latest, current)) return;

                // შეტყობინების ჩვენება
                var isRequired = updater.IsUpdateRequired(latest, current);
                var message = $"გამოვიდა პროგრამის ახალი ვერსია.\n\n" +
                             $"მიმდინარე ვერსია: {current}\n" +
                             $"ახალი ვერსია: {latest}\n\n" +
                             $"{(isRequired ? "განახლება აუცილებელია!" : "გსურთ განახლება?")}";

                var result = MessageBox.Show(message, "განახლება", 
                    isRequired ? MessageBoxButtons.OK : MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Information);

                    if (isRequired || result == DialogResult.Yes)
                    {
                        using (var dlg = new UpdateProgressForm())
                        {
                            dlg.Show(this);
                            string zip = null;
                            try
                            {
                                var progress = new Progress<(long current, long total)>(p => dlg.Report(p.current, p.total));
                                zip = await updater.DownloadAsync(manifest, progress, System.Threading.CancellationToken.None);
                            }
                            finally { dlg.Close(); }
                            if (!string.IsNullOrWhiteSpace(zip))
                            {
                                await updater.ScheduleApplyAndRestartAsync(zip, this);
                                return; // აპი დაიხურება და რესტარტი მოხდება
                            }
                        }
                    }
                    else
                    {
                        // მომხმარებელმა არ აირჩია განახლება
                        MessageBox.Show("განახლება გადაიდო. შეგიძლიათ მოგვიანებით განახლოთ Admin Panel-იდან.",
                            "განახლება გადაიდო", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), ex.Source); }
            finally
            {
                _updateRunning = false;
                ToggleLoginControls(true);
            }
        }
        private void ToggleLoginControls(bool enabled)
        {
            try
            {
                btnLogin.Enabled = enabled;
                txtUsername.Enabled = enabled;
                txtPassword.Enabled = enabled;
            }
            catch { }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (_updateRunning)
            {
                MessageBox.Show("მიმდინარეობს განახლების პროცესი. გთხოვთ დაელოდოთ.");
                return;
            }
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (_userService.Login(username, password, out int userId))
            {
                UserSession.Id = userId;
                UserSession.FullName = _userService.GetFullName(userId);
                //MessageBox.Show($"მოგესალმებით, {UserSession.FullName}!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();

                var form = _serviceProvider.GetRequiredService<MainForm>();
                form.ShowDialog();
                this.Close();
            }
            else
            {
                lblLoginStatus.Text = "მომხმარებელი ან პაროლი არასწორია!";
                //MessageBox.Show("მომხმარებელი ან პაროლი არასწორია!", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


