using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using EasyInsur.Views;
using Prism.Ioc;
using Prism.Regions;

namespace EasyInsur
{
    public partial class App
    {
        public App()
        {
            // Setup Global Exception Handlers
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            // Insurance amounts are displayed in Indian Rupees throughout the application.
            var indiaCulture = CultureInfo.GetCultureInfo("en-IN");
            CultureInfo.DefaultThreadCurrentCulture = indiaCulture;
            CultureInfo.DefaultThreadCurrentUICulture = indiaCulture;
            CultureInfo.CurrentCulture = indiaCulture;
            CultureInfo.CurrentUICulture = indiaCulture;

            var syncfusionKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
            if (string.IsNullOrEmpty(syncfusionKey))
            {
                var appSettingsPath = Path.Combine(Services.AppPathWithoutName ?? AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(appSettingsPath))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(appSettingsPath));
                        if (doc.RootElement.TryGetProperty("SyncfusionLicenseKey", out var elem))
                        {
                            syncfusionKey = elem.GetString();
                        }
                    }
                    catch { }
                }
            }
            if (string.IsNullOrEmpty(syncfusionKey))
            {
                syncfusionKey = Services.Settings?.SyncfusionLicenseKey;
            }
            if (!string.IsNullOrEmpty(syncfusionKey))
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionKey);
            }
            RepoDb.SqLiteBootstrap.Initialize();
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                        CultureInfo.CurrentCulture.IetfLanguageTag)));
            
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception, "UI Dispatcher");
            HandyControl.Controls.MessageBox.Error(
                $"An unexpected application error occurred:\n\n{e.Exception.Message}\n\nTechnical details have been recorded in EasyInsur_error.log.",
                "Application Error");
            e.Handled = true;
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException(ex, "AppDomain Unhandled");
            }
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception, "TaskScheduler UnobservedTask");
            e.SetObserved();
        }

        private static readonly object LogLock = new();

        public static void LogException(Exception? ex, string source)
        {
            if (ex == null) return;
            try
            {
                lock (LogLock)
                {
                    var logPath = Path.Combine(Services.AppPathWithoutName ?? AppDomain.CurrentDomain.BaseDirectory, "EasyInsur_error.log");
                    var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{source}] {ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
                    if (ex.InnerException != null)
                    {
                        logEntry += $"  Inner Exception: {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}{Environment.NewLine}{ex.InnerException.StackTrace}{Environment.NewLine}";
                    }
                    logEntry += new string('-', 80) + Environment.NewLine;
                    File.AppendAllText(logPath, logEntry);
                }
            }
            catch
            {
                // Never crash within the error logger
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<Modules.IAppDialogService, Modules.AppDialogService>();
            containerRegistry.RegisterForNavigation<PaymentWindow>();
            containerRegistry.RegisterForNavigation<PersonDetails>();
            containerRegistry.RegisterForNavigation<ViewPaymentWindow>();
            containerRegistry.RegisterForNavigation<ViewPeopleWindow>();
            containerRegistry.RegisterForNavigation<InsuranceWindow>();
            containerRegistry.RegisterForNavigation<Dashboard>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("ContentRegion", typeof(Dashboard));
        }
    }
}
