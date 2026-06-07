using BCCStudents.Application.Services.Update;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Presentation.Properties;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class LoginForm : Form
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly mainFormFactory _mainFormFactory;
        private readonly IApplicationStatus _appStatus;
        private readonly IConfigurationService _configService;
        private readonly ILoggerRepository _loggerRepository;
        private bool _updateRunning;
        //private readonly UserSession userSession;
        //UserSession UserSession = new UserSession();
        public static int LoggedInUserId { get; private set; }

        #region Delegates
        public delegate MainForm mainFormFactory();
        #endregion
        public LoginForm(IServiceProvider serviceProvider,
            IUserService userService,
            mainFormFactory mainFormFactory,
            IApplicationStatus appStatus,
            IConfigurationService configService,
            ILoggerRepository loggerRepository
            )
        {
            InitializeComponent();
            FormTitleHelper.SetTitle(this, "ავტორიზაცია");
            _userService = userService;
            _serviceProvider = serviceProvider;
            _mainFormFactory = mainFormFactory;
            _appStatus = appStatus ?? throw new ArgumentNullException(nameof(appStatus));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _loggerRepository = loggerRepository ?? throw new ArgumentNullException(nameof(loggerRepository));
            this.Shown += LoginForm_Shown;
            this.Load += LoginForm_Load;

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // AutoComplete-ის ინიციალიზაცია Form Load event-ზე, როცა TextBox სრულად არის ინიციალიზებული
            InitializeAutoComplete();
        }

        private void InitializeAutoComplete()
        {
            try
            {
                // ვტვირთავთ შენახულ მომხმარებლების სახელებს
                var usernames = LoadUsedUsernames();

                System.Diagnostics.Debug.WriteLine($"AutoComplete ინიციალიზაცია: ნაპოვნია {usernames.Count} მომხმარებელი");

                if (usernames.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("AutoComplete: მომხმარებლების სია ცარიელია");
                    return;
                }

                // ვქმნით AutoCompleteStringCollection-ს
                var autoCompleteCollection = new AutoCompleteStringCollection();
                foreach (var username in usernames)
                {
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        autoCompleteCollection.Add(username);
                        System.Diagnostics.Debug.WriteLine($"AutoComplete: დამატებულია '{username}'");
                    }
                }

                if (autoCompleteCollection.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("AutoComplete: ცარიელი სია ვალიდური მომხმარებლების სახელების შემდეგ");
                    return;
                }

                // მნიშვნელოვანი: ჯერ ვასუფთავებთ ძველ AutoComplete-ს
                txtUsername.AutoCompleteCustomSource?.Clear();

                // ვაყენებთ AutoComplete-ს txtUsername-ზე
                // Multiline TextBox-ზე AutoComplete როგორც წესი მუშაობს, მაგრამ უნდა დავრწმუნდეთ რომ სწორად არის კონფიგურირებული
                txtUsername.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtUsername.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtUsername.AutoCompleteCustomSource = autoCompleteCollection;

                // დამატებითი ვალიდაცია
                if (txtUsername.AutoCompleteCustomSource == null || txtUsername.AutoCompleteCustomSource.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("AutoComplete: ვერ დაემატა CustomSource");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"AutoComplete: დაინიციალიზდა წარმატებით. სულ {autoCompleteCollection.Count} ელემენტი");
                System.Diagnostics.Debug.WriteLine($"AutoComplete: Mode={txtUsername.AutoCompleteMode}, Source={txtUsername.AutoCompleteSource}, CustomSource.Count={txtUsername.AutoCompleteCustomSource.Count}");

                // Windows Forms-ში AutoComplete შეიძლება არ მუშაობდეს Multiline TextBox-ზე
                // თუ TextBox Multiline=true არის, შეიძლება საჭირო იყოს SingleLine-ზე გადაყვანა
                // ან ComboBox dropdown-style-ით გამოყენება
                // ამ შემთხვევაში TextBox-ი Multiline არის, ამიტომ შეიძლება AutoComplete UI არ გამოჩნდეს
                // მაგრამ მონაცემები უნდა იყოს დაყენებული
            }
            catch (Exception ex)
            {
                // თუ შეცდომა მოხდა, ვაგრძელებთ AutoComplete-ის გარეშე
                System.Diagnostics.Debug.WriteLine($"AutoComplete ინიციალიზაციის შეცდომა: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private List<string> LoadUsedUsernames()
        {
            try
            {
                string json = _configService.UsedUsernames;
                System.Diagnostics.Debug.WriteLine($"LoadUsedUsernames: JSON = '{json}'");

                if (string.IsNullOrWhiteSpace(json))
                {
                    System.Diagnostics.Debug.WriteLine("LoadUsedUsernames: JSON ცარიელია");
                    return new List<string>();
                }

                var usernames = JsonConvert.DeserializeObject<List<string>>(json);
                if (usernames == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadUsedUsernames: DeserializeObject დააბრუნა null");
                    return new List<string>();
                }

                System.Diagnostics.Debug.WriteLine($"LoadUsedUsernames: წარმატებით ჩაიტვირთა {usernames.Count} მომხმარებელი");
                return usernames;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadUsedUsernames შეცდომა: {ex.Message}");
                return new List<string>();
            }
        }

        private void SaveUsedUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return;

                var usernames = LoadUsedUsernames();

                // თუ ეს მომხმარებელი ჯერ არ არის სიაში, ვამატებთ
                if (!usernames.Contains(username, StringComparer.OrdinalIgnoreCase))
                {
                    usernames.Add(username);

                    // ვინახავთ JSON ფორმატში (მაქსიმუმ 20 მომხმარებელი, რომ არ გახდეს ძალიან დიდი)
                    if (usernames.Count > 20)
                    {
                        // ვტოვებთ ბოლო 20 მომხმარებელს
                        usernames = usernames.Skip(usernames.Count - 20).ToList();
                    }

                    string json = JsonConvert.SerializeObject(usernames);
                    _configService.UsedUsernames = json;
                    _configService.Save();

                    // ვაახლებთ AutoComplete-ის სიას
                    var autoCompleteCollection = new AutoCompleteStringCollection();
                    foreach (var u in usernames)
                    {
                        if (!string.IsNullOrWhiteSpace(u))
                        {
                            autoCompleteCollection.Add(u);
                        }
                    }
                    txtUsername.AutoCompleteCustomSource = autoCompleteCollection;

                    System.Diagnostics.Debug.WriteLine($"SaveUsedUsername: დაინახა AutoComplete სია - {autoCompleteCollection.Count} ელემენტი");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"მომხმარებლის სახელის შენახვის შეცდომა: {ex.Message}");
            }
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
                // წარმატებული login-ის შემდეგ ვინახავთ მომხმარებლის სახელს
                SaveUsedUsername(username);

                UserSession.Id = userId;
                UserSession.FullName = _userService.GetFullName(userId);
                //MessageBox.Show($"მოგესალმებით, {UserSession.FullName}!", "წარმატება", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _appStatus.IsAuthenticated = true;

                try
                {
                    _loggerRepository.WriteLog("Authorization", "Success", $"წარმატებული ავტორიზაცია: {username}", username);
                }
                catch { }

                // ანახლებს UserContext-ს მომხმარებლის ინფორმაციით
                try
                {
                    var userContext = _serviceProvider.GetService<BCCStudents.Application.Interfaces.IUserContext>();
                    userContext?.Refresh();
                }
                catch { }

                this.Hide();

                var mainform = _mainFormFactory.Invoke();
                mainform.ShowDialog();
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


