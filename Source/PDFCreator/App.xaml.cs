using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using NLog;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Assistants;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Helper.Logging;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.Views;
using pdfforge.PDFCreator.Startup;
using pdfforge.PDFCreator.Threading;
using pdfforge.PDFCreator.Utilities.Communication;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace pdfforge.PDFCreator
{
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            InitializeComponent();

            System.Windows.Forms.Application.EnableVisualStyles();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var globalMutex = new GlobalMutex("PDFCreator-137a7751-1070-4db4-a407-83c1625762c7");
            globalMutex.Acquire();
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Thread.CurrentThread.Name = "ProgramThread";

            try
            {
                LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);

                RunApplication(e.Args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "There was an error while starting PDFCreator");

                ErrorAssistant.ShowErrorWindowInNewProcess(ex);
            }
            finally
            {
                globalMutex.Release();
                Logger.Debug("Ending PDFCreator");
                Shutdown();
            }
        }

        private void RunApplication(string[] commandLineArguments)
        {
            CheckSpoolerRunning();

            // Must be done before the other checks to initialize the translator
            SettingsHelper.Init();
            
            // Check translations and Ghostscript. Exit if there is a problem
            CheckInstallation();

            CheckLicensing();

            AppStartFactory appStartFactory = new AppStartFactory();
            var appStart = appStartFactory.CreateApplicationStart(commandLineArguments);

            // PrintFile needs to be started before initializing the JonbInfoQueue
            if (appStart is PrintFileStart)
            {
                appStart.Run();
                return;
            }

            Logger.Debug("Starting PDFCreator");

            if (commandLineArguments.Length > 0)
            {
                Logger.Info("Command Line parameters: \r\n" + String.Join(" ", commandLineArguments));
            }

            if (!InitializeJobInfoQueue())
                return;

            // Start the application
            appStart.Run();

            Logger.Debug("Waiting for all threads to finish");
            ThreadManager.Instance.WaitForThreadsAndShutdown(this);
        }

        private void CheckSpoolerRunning()
        {
            var spoolerController = new ServiceController("spooler");
            if (spoolerController.Status != ServiceControllerStatus.Running)
            {
                Logger.Error("Spooler service is not running. Exiting...");
                var message = "The Windows spooler service is not running. Please start the spooler first.\r\n\r\nProgram exiting now.";
                const string caption = @"PDFCreator";
                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void CheckLicensing()
        {
            RenewActivation();
            TerminalServerCheck();

            var edition = EditionFactory.Instance.Edition;
            if (!edition.IsLicenseValid)
            {
                Logger.Error("Invalid or expired license.");

                var editionWithVersionNumber = edition.Name + " " + edition.VersionHelper.FormatWithThreeDigits();

                if (!SettingsHelper.GpoSettings.HideLicenseTab)
                {
                    var caption = edition.Name;
                    var message = TranslationHelper.Instance.TranslatorInstance.GetFormattedTranslation("Program",
                        "LicenseInvalid",
                        "Your license for \"{0}\" is invalid or has exprired.\r\n\r\nDo you want to check your license now?\r\nElse PDFCreator will shutdown.",
                        editionWithVersionNumber);
                    var result = MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.YesNo,
                        MessageWindowIcon.Exclamation);
                    if (result == MessageWindowResponse.No)
                        Shutdown(3);
                    LicenseWindow.ShowDialogTopMost();
                    //Check latest edition for validity
                    if (EditionFactory.Instance.Edition.IsLicenseValid)
                        return;
                    Shutdown(4);
                }
                else
                {
                    var caption = edition.Name;
                    var message = TranslationHelper.Instance.TranslatorInstance.GetFormattedTranslation("Program",
                        "LicenseInvalidGpoHideLicenseTab",
                        "Your license for \"{0}\" has exprired.\r\nPlease contact your administrator.\r\nPDFCreator will shutdown.",
                        editionWithVersionNumber);
                    MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.OK, MessageWindowIcon.Exclamation);
                    Shutdown(5);
                }
            }
        }

        private void RenewActivation()
        {
            var lastActivation = EditionFactory.Instance.Edition.Activation;

            if (lastActivation == null)
                return;

            var remainingActivationTime = lastActivation.ActivatedTill - DateTime.Now;
            if (remainingActivationTime > TimeSpan.FromDays(4))
                return;

            try
            {
                var licenseServerHelper = new LicenseServerHelper();
                var licenseChecker = licenseServerHelper.BuildLicenseChecker(lastActivation.Product, RegistryHive.CurrentUser);
                licenseChecker.ActivateWithKey(lastActivation.Key);
            }
            finally
            {
                EditionFactory.Instance.ReloadEdition();
            }
        }

        private void TerminalServerCheck()
        {
            var tsd = new TerminalServerDetection();
            if (!tsd.IsTerminalServer())
                return;

            var edition = EditionFactory.Instance.Edition;
            if (!edition.ValidOnTerminalServer)
            {
                Logger.Error("Tryed to run " + edition.Name + " with installed Terminal Services.");
                var caption = edition.Name;
                var message = TranslationHelper.Instance.TranslatorInstance.GetTranslation("Program", "UsePDFCreatorTerminalServer",
                    "Please use \"PDFCreator Terminal Server\" for use on computers with installed Terminal Services.\r\n\r\nPlease visit our website for more information or contact us directly: licensing@pdfforge.org");
                var result = MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.MoreInfoCancel, MessageWindowIcon.Exclamation);
                if (result == MessageWindowResponse.MoreInfo)
                    Process.Start(Urls.PdfCreatorTerminalServerUrl);
                Shutdown(2);
            }
        }

        private bool InitializeJobInfoQueue()
        {
            JobInfoQueue.Init();

            if (!JobInfoQueue.Instance.SpoolFolderIsAccessible())
            {
                var repairSpoolFolderAssistant = new RepairSpoolFolderAssistant();
                return repairSpoolFolderAssistant.TryRepairSpoolPath();
            }

            return true;
        }

        private void CheckInstallation()
        {
            if (TranslationHelper.Instance.TranslationPath == null)
            {
                MessageBox.Show(@"Could not find any translation. Please reinstall PDFCreator.",
                    @"Translations missing");
                Shutdown(1);
            }

            // Verfiy that Ghostscript is installed and exit if not
            EnsureGhoscriptIsInstalled();

            // Verify that PDFCreator printers are installed
            EnsurePrinterIsInstalled();
        }

        private void EnsureGhoscriptIsInstalled()
        {
            if (!HasGhostscriptInstance())
            {
                Logger.Debug("No valid Ghostscript version found. Exiting...");
                var message = TranslationHelper.Instance.TranslatorInstance.GetTranslation("ConversionWorkflow", "NoSupportedGSFound",
                    "Can't find a supported Ghostscript installation.\r\n\r\nProgram exiting now.");
                const string caption = @"PDFCreator";
                MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.OK, MessageWindowIcon.Error);
                Environment.Exit(1);
            }
        }

        private void EnsurePrinterIsInstalled()
        {
            var repairPrinterAssistant = new RepairPrinterAssistant();

            if (repairPrinterAssistant.IsRepairRequired())
            {
                var printers = SettingsHelper.Settings.ApplicationSettings.PrinterMappings.Select(mapping => mapping.PrinterName);
                if (!repairPrinterAssistant.TryRepairPrinter(printers))
                {
                    Environment.Exit(1);
                }
            }
        }

        private bool HasGhostscriptInstance()
        {
            GhostscriptVersion gsVersion = new GhostscriptDiscovery().GetBestGhostscriptInstance();

            return gsVersion != null;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            Logger.Fatal(ex, "Uncaught exception, IsTerminating: {0}", e.IsTerminating);
            ErrorAssistant.ShowErrorWindowInNewProcess(ex);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Uncaught exception in WPF thread");

            e.Handled = true;

            bool terminateRequested;
            ErrorAssistant.ShowErrorWindow(e.Exception, out terminateRequested);

            if (terminateRequested)
            {
                Current.Shutdown(1);
            }
        }
    }
}
