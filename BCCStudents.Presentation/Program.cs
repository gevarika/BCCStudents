using BCCStudents.Application.Interfaces; // IConnectionStatusService-სთვის
using BCCStudents.Application.Services;
using BCCStudents.Application.Services.AutoFileDetection;
using BCCStudents.Application.Services.Logging;
using BCCStudents.Application.Services.Update;
using BCCStudents.Domain.Entities;
using BCCStudents.Domain.Interfaces;
using BCCStudents.Infrastructure.Data;
using BCCStudents.Infrastructure.Logging;
using BCCStudents.Infrastructure.Repositories;
using BCCStudents.Infrastructure.Services; // ConnectionStatusService-სთვის
using BCCStudents.Presentation.Logging;
using BCCStudents.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
            SerilogBootstrap.Initialize();
            WindowsToastBootstrap.Initialize();

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
                Log.Warning(ex, "Settings migration failed; continuing with defaults");
            }

            var externalConfig = ExternalConfigLoader.Load();
            ExternalConfigLoader.ApplyOverrides(externalConfig);

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

                TaskScheduler.UnobservedTaskException += (_, args) =>
                {
                    // სინქის Timer-ის fire-and-forget Task ან MySql SSL timeout — არა კრიტიკული UI შეცდომა.
                    if (ConnectionErrorHelper.IsLikelyConnectionError(args.Exception))
                    {
                        Log.Warning(args.Exception, "Unobserved task exception (connection/SSL, observed)");
                    }
                    else
                    {
                        Log.Error(args.Exception, "Unobserved task exception");
                    }

                    args.SetObserved();
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

                if (canConnect)
                {
                    ApplicationLogRepository.EnsureTable(serviceProvider.GetRequiredService<IDatabaseConnectionProvider>());
                    var systemConfigService = serviceProvider.GetRequiredService<ISystemConfigurationService>();
                    var logStorageSettings = serviceProvider.GetRequiredService<ILogStorageSettings>();
                    logStorageSettings.Initialize(systemConfigService.GetLogStorageTarget());

                    var writePolicy = serviceProvider.GetRequiredService<ApplicationLogWritePolicy>();
                    SerilogBootstrap.AddDatabaseSink(serviceProvider.GetRequiredService<IServiceScopeFactory>(), writePolicy);

                    var loggerRepository = serviceProvider.GetRequiredService<ILoggerRepository>();
                    loggerRepository.WriteLog(
                        "Program Start",
                        "Success",
                        "პროგრამა გაეშვა",
                        Environment.UserName);
                }

                var applicationLogRetention = serviceProvider.GetRequiredService<IApplicationLogRetentionService>();

                System.Windows.Forms.Application.ApplicationExit += (_, _) =>
                {
                    applicationLogRetention.Stop();
                    AuditLogFactory.CloseAndFlush();
                    SerilogBootstrap.Shutdown();
                };

                var initialForm = ResolveInitialForm(serviceProvider);
                System.Windows.Forms.Application.Run(initialForm);
            }


        }

        private static Form ResolveInitialForm(ServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<LoginForm>();
        }
        private static void Application_ThreadException(object? sender, ThreadExceptionEventArgs e)
        {
            Log.Error(e.Exception, "UI thread exception");
            MessageBox.Show($"⚠️ შეცდომა: {e.Exception.Message}", "შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // MySql.Data-ის შიდა SSL/timeout timer ზოგჯერ აქ მოდის (ConnectionMonitor-ის გარდა, სინქის Open-ებიდან).
            // უკვე ლოგდება sync-ში/Connection-ში — მომხმარებელს არ ვაჩვენებთ კრიტიკულ MessageBox-ს.
            if (e.ExceptionObject is Exception ex && ConnectionErrorHelper.IsLikelyConnectionError(ex))
            {
                Log.Warning(ex, "Background connection error at AppDomain boundary (terminating={IsTerminating})", e.IsTerminating);
                return;
            }

            if (e.ExceptionObject is Exception fatalEx)
                Log.Fatal(fatalEx, "Unhandled domain exception (terminating={IsTerminating})", e.IsTerminating);
            else
                Log.Fatal("Unhandled domain exception: {ExceptionObject} (terminating={IsTerminating})", e.ExceptionObject, e.IsTerminating);

            var message = e.ExceptionObject is Exception unhandled
                ? unhandled.Message
                : e.ExceptionObject?.ToString() ?? "Unknown error";
            MessageBox.Show($"გაუმართავი შეცდომა: {message}", "კრიტიკული შეცდომა", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            services.AddSingleton<ConnectionStatusBarHost>();

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
            services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
            services.AddScoped<IApplicationLogQueryService, ApplicationLogQueryService>();
            services.AddScoped<IApplicationLogDeleteService, ApplicationLogDeleteService>();
            services.AddSingleton<ILogStorageSettings, LogStorageSettingsService>();
            services.AddSingleton<ApplicationLogWritePolicy>();
            services.AddSingleton<IApplicationLogRetentionService, ApplicationLogRetentionService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISubGroupRepository, SubGroupRepository>();
            services.AddScoped<ICleanupRepository, CleanupRepository>();
            services.AddScoped<IPendingStudentRepository, PendingStudentRepository>();
            services.AddScoped<IPendingStudentGroupRepository, PendingStudentGroupRepository>();
            services.AddScoped<IFileTrackingRepository, FileTrackingRepository>();
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
            // განახლების სერვისი ერთ singleton ინსტანციად რეგისტრირდება.
            // მნიშვნელოვანია: LoginForm და AdminPanelForm სერვისს პირდაპირ კონკრეტული ტიპით ითხოვენ
            // (GetService<UpdateService>() / GetRequiredService<UpdateService>()), ამიტომ აუცილებელია
            // თავად კონკრეტული ტიპის რეგისტრაცია — წინააღმდეგ შემთხვევაში GetService null-ს აბრუნებს
            // და გაშვებისას ავტომატური განახლება ჩუმად ჩავარდება.
            // IUpdateService იმავე ინსტანციაზე გადამისამართდება, რომ ორმაგი ობიექტი არ შეიქმნას.
            services.AddSingleton<UpdateService>();
            services.AddSingleton<IUpdateService>(sp => sp.GetRequiredService<UpdateService>());
            services.AddSingleton<PendingRegistrationMonitor>();

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
                if (DocumentService.IsDownloadFolderConfigured(config.DownloadPath))
                    service.DownloadBaseFolder = config.DownloadPath.Trim();
                service.FileServerBaseUrl = config.FileUrl;

                return service;
            });


            // --- 4. Presentation (Forms) ---
            // აქ მხოლოდ UI ელემენტები რეგისტრირდება (Transient).
            //services.AddTransient<SetupWizardForm>();
            services.AddTransient<LoginForm>();
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

