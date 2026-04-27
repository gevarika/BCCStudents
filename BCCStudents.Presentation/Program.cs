using BCCStudents.Application.Interfaces; // IConnectionStatusService-სთვის
using BCCStudents.Application.Services;
using BCCStudents.Application.Services.AutoFileDetection;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Application.Services.Sync.DownStream;
using BCCStudents.Application.Services.Sync.UpStream;
using BCCStudents.Application.Services.Update;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Infrastructure.Repositories;
using BCCStudents.Infrastructure.Services; // ConnectionStatusService-სთვის
using BCCStudents.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using static BCCStudents.Presentation.LoginForm;
using static BCCStudents.Presentation.MainForm;
using static BCCStudents.Presentation.StudentManagementForm;

namespace BCCStudents.Presentation
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Settings Migration - ძველი ვერსიის პარამეტრების აღდგენა
            try
            {
                if (Properties.Settings.Default.UpgradeRequired)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                // თუ migration ვერ მოხერხდა, გავაგრძელოთ default settings-ებით
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
                Directory.CreateDirectory(logDir);
                File.AppendAllText(Path.Combine(logDir, "settings-migration.txt"),
                    $"[{DateTime.Now}] Settings migration failed: {ex.Message}\n\n");
            }

            var externalConfig = ExternalConfigLoader.Load();
            ExternalConfigLoader.ApplyOverrides(externalConfig);

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                try
                {
                    var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
                    Directory.CreateDirectory(dir);
                    var path = Path.Combine(dir, "exception-log.txt");
                    var log = $"[{DateTime.Now}] {e.Exception.GetType()}: {e.Exception.Message}\n{e.Exception.StackTrace}\n\n";
                    File.AppendAllText(path, log);
                }
                catch { }
            };


            // DI-ს კონფიგურაცია
            var services = new ServiceCollection();

            ConfigureServices(services);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                // გლობალური შეცდომების დამჭერები
                System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                TaskScheduler.UnobservedTaskException += (sender, args) =>
                {
                    try
                    {
                        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
                        Directory.CreateDirectory(dir);
                        var path = Path.Combine(dir, "task-errors.txt");
                        File.AppendAllText(path, $"[{DateTime.Now}] Unobserved Task Exception: {args.Exception}\n\n");
                    }
                    catch { }
                    args.SetObserved(); // საჭირო რომ პროცესმა არ "ჩაიფერფლოს"
                };

                // --- Emergency Setup Mode: პირველ რიგში შევამოწმოთ MySQL კავშირი ---
                var appStatus = serviceProvider.GetRequiredService<IApplicationStatus>();
                appStatus.IsDatabaseOnline = false;
                appStatus.IsAuthenticated = false;

                var configService = serviceProvider.GetRequiredService<IConfigurationService>();
                var initScriptPath = ExternalConfigLoader.ResolveInitScriptPath(externalConfig);
                if (!DatabaseInitializer.TryEnsureLocalDatabase(configService, initScriptPath, out var initError) &&
                    !string.IsNullOrWhiteSpace(initError))
                {
                    MessageBox.Show(
                        $"ბაზის ინიციალიზაცია ვერ შესრულდა: {initError}",
                        "ინიციალიზაციის შეცდომა",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                var dbHelper = serviceProvider.GetRequiredService<DatabaseHelper>();
                bool canConnect;
                try
                {
                    canConnect = dbHelper.TestConnectionAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    canConnect = false;
                }

                if (!canConnect)
                {
                    MessageBox.Show(
                        "ბაზასთან კავშირი ვერ დამყარდა. პროგრამა ჩაირთვება შეზღუდულ რეჟიმში პარამეტრების გასასწორებლად.",
                        "კავშირი ვერ დამყარდა",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    appStatus.IsDatabaseOnline = false;
                }
                else
                {
                    appStatus.IsDatabaseOnline = true;
                }

                var userService = serviceProvider.GetRequiredService<IUserService>();
                var upStreamManager = serviceProvider.GetRequiredService<IUpStreamSyncManager>();
                var downStreamManager = serviceProvider.GetRequiredService<IDownStreamSyncManager>();

                // UpStreamSyncManager აღარ იწყება აქ - იწყება MainForm_Load-ში ავტორიზაციის შემდეგ
                System.Windows.Forms.Application.ApplicationExit += (sender, args) =>
                {
                    upStreamManager.Stop();
                    downStreamManager.Stop();
                };

                Form initialForm;

                if (!appStatus.IsDatabaseOnline)
                {
                    // პირველი გაშვება / კავშირი ვერ დამყარდა -> გავუშვათ SetupWizardForm
                    initialForm = serviceProvider.GetRequiredService<SetupWizardForm>();
                }
                else if (userService.IsUserRegistered())
                {
                    initialForm = serviceProvider.GetRequiredService<LoginForm>();
                }
                else
                {
                    initialForm = serviceProvider.GetRequiredService<RegisterForm>();
                    UserSession.FirstStart = true;
                }

                System.Windows.Forms.Application.Run(initialForm);
            }


        }
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //LoggerRepository.WriteLog("Exception", "Failed", e.ToString());
            MessageBox.Show($"⚠️ შეცდომა: {e.Exception.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show($"გაუმართავი შეცდომა: {ex.Message + " " + sender}", "კრიტიკული შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // --- 1. Infrastructure Services (Base) ---
            // Configuration Service - საჭიროა DB კავშირის სტრიქონის მისაღებად
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            // Application Status - გლობალური აპლიკაციის მდგომარეობა (DB/ავტორიზაცია)
            services.AddSingleton<IApplicationStatus, ApplicationStatus>();

            // Database Helper - გამოიყენება ConnectionStatusService-ში და DatabaseConnectionChecker-ში კავშირის შესამოწმებლად
            services.AddSingleton<DatabaseHelper>();

            // Database Connection Checker - Clean Architecture-ის დაცვით
            services.AddSingleton<BCCStudents.Application.Interfaces.IDatabaseConnectionChecker, BCCStudents.Infrastructure.Services.DatabaseConnectionChecker>();

            // Database Connection Provider - Clean Architecture-ის დაცვით
            services.AddSingleton<BCCStudents.Application.Interfaces.IDatabaseConnectionProvider, BCCStudents.Infrastructure.Services.DatabaseConnectionProvider>();

            // SMS Service - Clean Architecture-ის დაცვით
            services.AddScoped<BCCStudents.Application.Interfaces.ISmsService, BCCStudents.Infrastructure.Services.SmsService>();

            // Student JSON Service - Clean Architecture-ის დაცვით
            services.AddScoped<BCCStudents.Application.Interfaces.IStudentJsonService, BCCStudents.Infrastructure.Services.StudentJsonService>();

            // Connection Status Service (Singleton - მთელი აპლიკაციისთვის ერთი ინსტანსი)
            services.AddSingleton<IConnectionStatusService, BCCStudents.Infrastructure.Services.ConnectionStatusService>();

            // Connection Monitor Service (Singleton) - Clean Architecture-ის დაცვით
            services.AddSingleton<BCCStudents.Application.Interfaces.IConnectionMonitor, BCCStudents.Infrastructure.Services.ConnectionMonitorService>();

            // Backup Manager (Singleton)
            services.AddSingleton<BCCStudents.Infrastructure.Services.BackupService>();

            // Admin Code Manager (Singleton)
            services.AddSingleton<BCCStudents.Infrastructure.Services.AdminCodeManager>();


            // --- 2. Repositories (Persistence) ---
            // Repositories უნდა დარეგისტრირდეს ინტერფეისით.
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IStudentGroupRepository, StudentGroupRepository>();
            services.AddScoped<IStudentSubGroupRepository, StudentSubGroupRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IBalanceRepository, BalanceRepository>();
            services.AddScoped<ILoggerRepository, LoggerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISubGroupRepository, SubGroupRepository>();
            services.AddScoped<ICleanupRepository, CleanupRepository>();
            services.AddScoped<IPendingStudentRepository, PendingStudentRepository>();
            services.AddScoped<IPendingStudentGroupRepository, PendingStudentGroupRepository>();
            services.AddScoped<IFileTrackingRepository, FileTrackingRepository>();
            services.AddScoped<IUpStreamSyncRepository, UpStreamSyncRepository>();
            services.AddScoped<BCCStudents.Domain.Interfaces.ISystemConfigurationRepository, BCCStudents.Infrastructure.Repositories.SystemConfigurationRepository>();


            // --- 3. Application Services (Use Cases) ---
            // ყველა Service უნდა დარეგისტრირდეს ინტერფეისით.

            // User & Group Services
            services.AddScoped<IUserService, UserService>(); // ეს იყო IUserService -> UserService
            services.AddScoped<IGroupService, GroupService>(); // ეს უნდა იყოს IGroupService -> GroupService

            // User Context - Singleton რომ მიმდინარე მომხმარებლის ინფორმაცია იყოს ერთი სისტემაში
            services.AddSingleton<BCCStudents.Application.Interfaces.IUserContext, BCCStudents.Infrastructure.Services.UserContext>();
            services.AddScoped<BCCStudents.Application.Interfaces.IStudentGroupsService, BCCStudents.Application.Services.StudentGroupsService>();
            services.AddScoped<BCCStudents.Application.Interfaces.IStudentSubGroupsService, BCCStudents.Application.Services.StudentSubGroupsService>();

            // Import & Payment Services
            services.AddScoped<IImportService, ImportServiceV2>();
            services.AddScoped<IExcelPaymentImportService, ExcelPaymentImportService>();
            services.AddScoped<IPendingStudentService, PendingStudentService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISubGroupService, SubGroupService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IPaymentDateService, PaymentDateService>();
            services.AddScoped<IPaymentDescriptionAnalyzer, PaymentDescriptionAnalyzer>();
            services.AddScoped<IStudentCodeGenerator, StudentCodeGenerator>();
            services.AddScoped<ICleanupService, CleanupService>();
            services.AddScoped<IStudentExportService, StudentExportService>();
            services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddScoped<IBackupService, BackupService>();

            // Sync Services
            services.AddSingleton<ISyncLogger, SyncLogger>(); // SyncLogger არის Infrastructure-ში და გამოიყენება Application/Sync-ში
            services.AddScoped<IDownStreamSyncRepository, BCCStudents.Infrastructure.Repositories.DownStreamSyncRepository>();
            services.AddScoped<IDownStreamDataFetcher, DownStreamDataFetcher>();
            services.AddScoped<IDownStreamConflictResolver, DownStreamConflictResolver>();
            services.AddScoped<IDownStreamSyncService, DownStreamSyncService>();
            services.AddSingleton<IDownStreamSyncManager, DownStreamSyncManager>();
            services.AddScoped<IUpStreamSyncService, UpStreamSyncService>();
            services.AddScoped<IUpStreamPayloadBuilder, UpStreamPayloadBuilder>();
            services.AddScoped<IUpStreamChangeTracker, UpStreamChangeTracker>();
            services.AddSingleton<IUpStreamSyncManager, UpStreamSyncManager>();

            // AutoFileDetection კონფიგურაცია
            services.AddSingleton<AutoFileDetectionConfig>(provider =>
            {
                return AutoFileDetectionConfig.LoadFromConfig();
            });

            services.AddScoped<AutoFileDetectionService>(provider =>
            {
                var fileTrackingRepo = provider.GetRequiredService<IFileTrackingRepository>();
                var config = provider.GetRequiredService<AutoFileDetectionConfig>();
                return new AutoFileDetectionService(fileTrackingRepo, config);
            });

            services.AddScoped<AutoFileDetectionManager>();

            // Document Service (Singleton - იყენებს კონფიგურაციას)
            services.AddSingleton<DocumentService>(provider =>
            {
                var service = new DocumentService();

                var config = DocumentConfig.Load();
                service.DownloadBaseFolder = config.DownloadPath;
                service.FileServerBaseUrl = config.FileUrl;

                return service;
            });


            // --- 4. Presentation (Forms) ---
            // აქ მხოლოდ UI ელემენტები რეგისტრირდება (Transient).
            services.AddTransient<SetupWizardForm>();
            services.AddTransient<LoginForm>();
            services.AddTransient<RegisterForm>();
            services.AddTransient<MainForm>();
            services.AddTransient<mainFormFactory>(servicepProvider =>
            {

                return () => servicepProvider.GetRequiredService<MainForm>();
            });
            services.AddTransient<StudentManagementForm>();
            services.AddTransient<StudentManagementFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას StudentManagementForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<StudentManagementForm>();
            });
            services.AddTransient<PaymentForm>();
            services.AddTransient<PaymentFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას PaymentForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<PaymentForm>();
            });
            services.AddTransient<PaymentTestForm>();
            services.AddTransient<PaymentTestFormFactory>(serviceProvider =>
            {
                return () => serviceProvider.GetRequiredService<PaymentTestForm>();
            });
            services.AddTransient<BalanceTransferForm>();
            services.AddTransient<BalanceTransferFormFactory>(serviceProvider =>
            {
                return () => serviceProvider.GetRequiredService<BalanceTransferForm>();
            });
            services.AddTransient<AdminPanelForm>();
            services.AddTransient<AdminPanelFormFactory>(servicepProvider =>
            {
                return () => servicepProvider.GetRequiredService<AdminPanelForm>();
            });
            services.AddTransient<BackupManagementForm>();
            services.AddTransient<BackupManagementFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას BackupManagementForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<BackupManagementForm>();
            });
            services.AddTransient<GroupManagementForm>();
            services.AddTransient<GroupManFormFactory>(serviceProvider =>
            {
                return () => serviceProvider.GetRequiredService<GroupManagementForm>();
            });
            services.AddTransient<GroupsEdit>();
            services.AddTransient<GroupsEditFormFactory>(serviceProvider =>
            {
                return () => serviceProvider.GetRequiredService<GroupsEdit>();
            });
            services.AddTransient<ImportFormV2>();
            services.AddTransient<ImportFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას ImportForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<ImportFormV2>();
            });
            services.AddTransient<PendingStudentsForm>();
            services.AddTransient<PendingStudentsFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას PendingStudentsForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<PendingStudentsForm>();
            });
            services.AddTransient<SetStudyStartDateForm>();
            services.AddTransient<SetStudyStartDateFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას StudentManagementForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<SetStudyStartDateForm>();
            });
            services.AddTransient<StatisticsForm>();
            services.AddTransient<StatisticsFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას StudentManagementForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<StatisticsForm>();
            });
            services.AddTransient<StudentsEditForm>();
            services.AddTransient<StudentsEditFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას StudentsEditForm და გადასცეს მას ყველა დამოკიდებულება (IUserContext-ის ჩათვლით).
                return () => serviceProvider.GetRequiredService<StudentsEditForm>();
            });
            services.AddTransient<PaymentsImportForm>();
            services.AddTransient<PaymentImportHistoryForm>();
            services.AddTransient<FinanceManagementForm>();
            services.AddTransient<FinanceFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას StudentManagementForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<FinanceManagementForm>();
            });
            services.AddTransient<UnmatchedPaymentsForm>();
            services.AddTransient<LogViewerForm>();
            services.AddTransient<LogViewerFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას LogViewerForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<LogViewerForm>();
            });
            services.AddTransient<UpdateProgressForm>();
            services.AddTransient<PaymentTestForm>();
            services.AddTransient<FailedStudentsForm>();
            services.AddTransient<FailedStudentsFormFactory>(serviceProvider =>
            {
                // ეს ლამბდა ფუნქცია (Factory) იყენებს serviceProvider-ს (რომელიც აქ არის დასაშვები)
                // რათა შექმნას LogViewerForm და გადასცეს მას ყველა დამოკიდებულება.
                return () => serviceProvider.GetRequiredService<FailedStudentsForm>();
            });

            // User Management Form
            services.AddTransient<UserManagementForm>();
            services.AddTransient<UserManagementFormFactory>(serviceProvider =>
            {
                return () => serviceProvider.GetRequiredService<UserManagementForm>();
            });

        }
    }
}

