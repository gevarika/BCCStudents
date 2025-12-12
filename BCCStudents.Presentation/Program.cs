using System;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;
using BCCStudents.Infrastructure;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Repositories;
using BCCStudents.Application.Services;
using BCCStudents.Domain.Entities;
using BCCStudents.Application.Services.Update;
using BCCStudents.Application.Services.AutoFileDetection;
using BCCStudents.Application.Services.Sync;
using BCCStudents.Application.Services.Sync.UpStream;
using BCCStudents.Application.Services.Sync.DownStream;
using BCCStudents.Presentation.Services;
using BCCStudents.Infrastructure.Services; // ConnectionStatusService-სთვის
using BCCStudents.Application.Interfaces; // IConnectionStatusService-სთვის

namespace BCCStudents.Presentation
{
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
                // ბაზის ინიციალიზაცია - ახალი ლოგიკა!
                // ვიღებთ ConnectionStatusService-ს DI-თ და ვამოწმებთ კავშირს.
                // MainForm-საც DI-თ გადაეცემა იგივე სერვისი და თავად ნახავს სტატუსს.
                var connectionService = serviceProvider.GetRequiredService<IConnectionStatusService>();
                connectionService.CheckConnection(); // კავშირის საწყისი შემოწმება

                var configService = serviceProvider.GetRequiredService<IConfigurationService>();

                var userService = serviceProvider.GetRequiredService<IUserService>();
                var upStreamManager = serviceProvider.GetRequiredService<IUpStreamSyncManager>();
                var downStreamManager = serviceProvider.GetRequiredService<IDownStreamSyncManager>();
                upStreamManager.Start();
                System.Windows.Forms.Application.ApplicationExit += (sender, args) =>
                {
                    upStreamManager.Stop();
                    downStreamManager.Stop();
                };

                //Form initialForm = userService.IsUserRegistered() ? (Form)serviceProvider.GetRequiredService<LoginForm>() :  (Form)serviceProvider.GetRequiredService<RegisterForm>() && UserSession.FirstStart = true ;
                Form initialForm;

                if (userService.IsUserRegistered())
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

            // Connection Monitor Service (Singleton)
            services.AddSingleton<ConnectionMonitorService>();

            // Backup Manager (Singleton)
            services.AddSingleton<BCCStudents.Infrastructure.Services.BackupManager>();

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


            // --- 3. Application Services (Use Cases) ---
            // ყველა Service უნდა დარეგისტრირდეს ინტერფეისით.

            // User & Group Services
            services.AddScoped<IUserService, UserService>(); // ეს იყო IUserService -> UserService
            services.AddScoped<IGroupService, GroupService>(); // ეს უნდა იყოს IGroupService -> GroupService

            // Import & Payment Services
            services.AddScoped<IImportService, ImportService>();
            services.AddScoped<IExcelPaymentImportService, ExcelPaymentImportService>();
            services.AddScoped<IPendingStudentService, PendingStudentService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISubGroupService, SubGroupService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IPaymentDateService, PaymentDateService>();
            services.AddScoped<IPaymentDescriptionAnalyzer, PaymentDescriptionAnalyzer>();
            services.AddScoped<IStudentCodeGenerator, StudentCodeGenerator>();
            services.AddScoped<ICleanupService, CleanupService>();
            services.AddScoped<IStudentExportService, StudentExportService>();
            services.AddSingleton<IUpdateService, UpdateService>();

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
            services.AddTransient<LoginForm>();
            services.AddTransient<RegisterForm>();
            services.AddTransient<MainForm>();
            services.AddTransient<StudentManagementForm>();
            services.AddTransient<PaymentForm>();
            services.AddTransient<AdminPanelForm>();
            services.AddTransient<GroupManagementForm>();
            services.AddTransient<GroupsEdit>();
            services.AddTransient<ImportForm>();
            services.AddTransient<PendingStudentsForm>();
            services.AddTransient<SetStudyStartDateForm>();
            services.AddTransient<StudentsEditForm>();
            services.AddTransient<PaymentsImportForm>();
            services.AddTransient<StudyStartDateManager>(); // ⚠️ ეს არის Service, მაგრამ რადგან არ აქვს I-ინტერფეისი, დროებით დავტოვოთ აქ.
            services.AddTransient<PaymentImportHistoryForm>();
            services.AddTransient<FinanceManagementForm>();
            services.AddTransient<UnmatchedPaymentsForm>();
            services.AddTransient<LogViewerForm>();
            services.AddTransient<ImportTestForm>();
            services.AddTransient<UpdateProgressForm>();
            services.AddTransient<PaymentTestForm>();
        }
    }
}

