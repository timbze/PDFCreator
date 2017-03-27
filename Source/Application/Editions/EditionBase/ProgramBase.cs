using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using NLog;
using pdfforge.Communication;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.UI.Views;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public class ProgramBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static ErrorReportHelper _errorReportHelper;

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
            var container = new Container();

            var windowResolver = new SimpleInjectorWindowResolver(container);
            var windowRegistry = new WindowRegistry(windowResolver);

            bootstrapper.ConfigureContainer(container, windowRegistry);
            bootstrapper.RegisterInteractions(windowRegistry);

            container.Verify(VerificationOption.VerifyOnly);

            var resolver = new SimpleInjectorAppStartResolver(container);

            var appStartFactory = new AppStartFactory(resolver);
            var appStart = appStartFactory.CreateApplicationStart(args);

            var app = new App(appStart);
            app.DispatcherUnhandledException += Application_DispatcherUnhandledException;

            return app.Run();
        }

        private static void InitializeLogging()
        {
            LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);
            var inMemoryLogger = new InMemoryLogger(100);
            inMemoryLogger.Register();
            _errorReportHelper = new ErrorReportHelper(inMemoryLogger);
        }

        private static GlobalMutex AcquireMutex()
        {
            var globalMutex = new GlobalMutex("PDFCreator-137a7751-1070-4db4-a407-83c1625762c7");
            globalMutex.Acquire();

            return globalMutex;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
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
