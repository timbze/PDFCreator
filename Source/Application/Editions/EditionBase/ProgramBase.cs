using NLog;
using pdfforge.Communication;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.Utilities;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Application = System.Windows.Forms.Application;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public class ProgramBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static ErrorReportHelper _errorReportHelper;
        private static Container _container;

        public static void Main(string[] args, Func<Bootstrapper> getBootstrapperFunc)
        {
            var globalMutex = AcquireMutex();
            var exitCode = 0;

            try
            {
                exitCode = StartApplication(args, getBootstrapperFunc);
            }
            finally
            {
                globalMutex.Release();
                _container?.Dispose();

                Logger.Debug("Ending PDFCreator");
                Environment.Exit(exitCode);
            }
        }

        private static int StartApplication(string[] args, Func<Bootstrapper> getBootstrapperFunc)
        {
            Application.EnableVisualStyles();

            Thread.CurrentThread.Name = "ProgramThread";

            InitializeLogging();
            Logger.Debug("Starting PDFCreator");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var bootstrapper = getBootstrapperFunc();
            _container = new Container();

            var profileSettingsTabs = bootstrapper.DefineProfileSettingsTabs();
            var applicationSettingsTabs = bootstrapper.DefineApplicationSettingsTabs();

            var prismBootstrapper = new PrismBootstrapper(profileSettingsTabs, applicationSettingsTabs);

            prismBootstrapper.ConfigurePrismDependecies(_container);
            prismBootstrapper.RegisterNavigationViews(_container);

            bootstrapper.ConfigureContainer(_container);
            bootstrapper.RegisterInteractions();

            ViewRegistry.WindowFactory = new DelegateWindowFactory(content =>
            {
                var window = _container.GetInstance<InteractionHostWindow>();
                window.Content = content;

                return window;
            });

            if (Debugger.IsAttached)
                _container.Verify(VerificationOption.VerifyOnly);

            bootstrapper.RegisterEditiondependentRegions(_container.GetInstance<IRegionManager>());
            prismBootstrapper.InitPrismStuff(_container);

            var resolver = new SimpleInjectorAppStartResolver(_container);

            var appStartFactory = new AppStartFactory(resolver);
            var appStart = appStartFactory.CreateApplicationStart(args);

            var helpCommandHandler = _container.GetInstance<HelpCommandHandler>();
            var settingsManager = _container.GetInstance<ISettingsManager>();

            var app = new App(appStart, helpCommandHandler, settingsManager);
            app.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            return app.Run();
        }

        private static void InitializeLogging()
        {
            LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);
            var inMemoryLogger = new InMemoryLogger(100);
            inMemoryLogger.Register();
            var assembly = typeof(ProgramBase).Assembly;
            _errorReportHelper = new ErrorReportHelper(inMemoryLogger, new VersionHelper(assembly), new AssemblyHelper(assembly));
        }

        private static GlobalMutex AcquireMutex()
        {
            var globalMutex = new GlobalMutex("PDFCreator-137a7751-1070-4db4-a407-83c1625762c7");
            globalMutex.Acquire();

            return globalMutex;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.Fatal(ex, "Uncaught exception, IsTerminating: {0}", e.IsTerminating);
            _errorReportHelper.ShowErrorReportInNewProcess(ex);
        }

        private static void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Uncaught exception in WPF thread");

            e.Handled = true;

            _errorReportHelper.ShowErrorReport(e.Exception);

            System.Windows.Application.Current.Shutdown(1);
        }
    }
}
