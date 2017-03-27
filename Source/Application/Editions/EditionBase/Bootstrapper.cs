using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;
using SystemWrapper;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using pdfforge.DataStorage;
using pdfforge.Mail;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Dropbox;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.Conversion;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.UI.Views;
using pdfforge.PDFCreator.UI.Views.Dialogs;
using pdfforge.PDFCreator.UI.Views.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Registry;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.UserGuide;
using SimpleInjector;
using Translatable;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public abstract class Bootstrapper
    {
        private WorkflowFactory _workflowFactory;

        protected abstract string EditionName { get; }
        protected abstract bool HideLicensing { get; }
        protected abstract bool ShowWelcomeWindow { get; }
        protected abstract bool ShowOnlyForPlusAndBusinessHint { get; }
        protected abstract ButtonDisplayOptions ButtonDisplayOptions { get; }

        public void ConfigureContainer(Container container, WindowRegistry windowRegistry)
        {
            container.Options.SuppressLifestyleMismatchVerification = true;

            RegisterActivationHelper(container);
            container.RegisterSingleton(() => new ApplicationNameProvider(EditionName));
            container.RegisterSingleton(() => new LicenseOptionProvider(HideLicensing));
            container.RegisterSingleton(() => new EditionHintOptionProvider(ShowOnlyForPlusAndBusinessHint));
            container.RegisterSingleton(() => ButtonDisplayOptions);
            container.RegisterSingleton(() => new DropboxAppData(Data.Decrypt("r4IH27xLkSb2FWkNUcPfwA=="), "https://www.dropbox.com/1/oauth2/redirect_receiver"));

            if (ShowWelcomeWindow)
                container.Register<WelcomeCommand, ShowWelcomeWindowCommand>();
            else
                container.Register<WelcomeCommand, DisabledWelcomeWindowCommand>();

            var regPath = @"Software\pdfforge\PDFCreator";
            container.RegisterSingleton<IInstallationPathProvider>(() => new InstallationPathProvider(regPath, regPath + @"\Settings", "{0001B4FD-9EA3-4D90-A79E-FD14BA3AB01D}"));

            _workflowFactory = new WorkflowFactory(container);
            container.Register<ConversionWorkflow>(); // Workflow is registered so all dependencies can be verified in test

            container.Register<IWorkflowFactory>(() => _workflowFactory);

            // Here come workflow depedent registrations! 

            RegisterWorkflowDependent<IErrorNotifier, ErrorNotifierInteractive, ErrorNotifierAutosave>(container);
            RegisterWorkflowDependent<IProcessingPasswordsProvider, InteractiveProcessingPasswordsProvider, DefaultProcessingPasswordsProvider>(container);
            RegisterWorkflowDependent<IFtpPasswordProvider, InteractiveFtpPasswordProvider, FtpPasswordProvider>(container);
            RegisterWorkflowDependent<ISmtpPasswordProvider, InteractiveSmtpPasswordProvider, SmtpPasswordProvider>(container);
            RegisterWorkflowDependent<ITargetFileNameComposer, InteractiveTargetFileNameComposer, AutosaveTargetFileNameComposer>(container);
            RegisterWorkflowDependent<IRecommendArchitect, InteractiveRecommendArchitect, AutosaveRecommendArchitect>(container);
            RegisterWorkflowDependent<IConversionProgress, InteractiveConversionProgress, AutosaveConversionProgress>(container);
            RegisterWorkflowDependent<IOutputFileMover, InteractiveOutputFileMover, AutosaveOutputFileMover>(container);

            container.RegisterSingleton<IInteractionInvoker>(() => new InteractionInvoker(windowRegistry));
            container.Register<ISoundPlayer, SoundPlayer>();
            container.Register<ISmtpTest, SmtpTestEmailAssistant>();
            container.Register<IClientTestEmail, ClientTestEmail>();
            container.Register<IActionManager, ActionManager>();

            container.Register<IJobPrinter, GhostScriptPrinter>();

            container.Register<IJobDataUpdater, JobDataUpdater>();
            container.Register<IPageNumberCalculator, PageNumberCalculator>();
            container.Register<IOutputFilenameComposer, OutputFilenameComposer>();

            container.Register<IJobRunner, JobRunner>();
            container.Register<IConverterFactory, ConverterFactory>();
            container.Register<IPsConverterFactory, GhostscriptConverterFactory>();

            container.Register<IJobCleanUp, JobCleanUp>();

            container.Register<ISystemPrinterProvider, SystemPrinterProvider>();


            container.Register<IDirectConversionProvider, DirectConversionProvider>();
            container.Register<IDirectConversionHelper, DirectConversionHelper>();
            container.RegisterSingleton<IFileConversionHandler, FileConversionHandler>();
            container.Register<IPrintFileHelper, PrintFileAssistant>();
            container.Register<IUacAssistant, UacAssistant>();
            container.Register<ITestPageHelper, TestPageHelper>();
            container.RegisterSingleton<PdfArchitectCheck>();
            container.RegisterSingleton<IPdfArchitectCheck, CachedPdfArchitectCheck>();
            container.Register<IGhostscriptDiscovery, GhostscriptDiscovery>();

            container.RegisterSingleton<IRegistry, RegistryWrap>();
            container.RegisterSingleton<IFile, FileWrap>();
            container.RegisterSingleton<IDirectory, DirectoryWrap>();
            container.RegisterSingleton<IPath, PathWrap>();
            container.RegisterSingleton<IPathSafe, PathWrapSafe>();
            container.RegisterSingleton<IPathUtil, PathUtil>();
            container.RegisterSingleton<IEnvironment, EnvironmentWrap>();

            container.RegisterSingleton<IProcessStarter, ProcessStarter>();
            container.RegisterSingleton<IDateTimeProvider, DateTimeProvider>();

            container.RegisterSingleton<IAssemblyHelper, AssemblyHelper>();
            container.Register<IOsHelper, OsHelper>();
            container.Register<IFontHelper, FontHelper>();
            container.Register<IOpenFileInteractionHelper, OpenFileInteractionHelper>();
            container.Register<IPDFCreatorNameProvider, PDFCreatorNameProvider>();

            container.RegisterSingleton<IJobInfoQueueManager, JobInfoQueueManager>();
            container.Register<IJobInfoQueue, JobInfoQueue>(Lifestyle.Singleton);
            container.Register<IThreadManager, ThreadManager>(Lifestyle.Singleton);
            container.Register<IPipeServerManager, PipeServerManager>(Lifestyle.Singleton);
            container.RegisterSingleton<IPipeMessageHandler, NewPipeJobHandler>();

            container.Register<IVersionHelper, VersionHelper>();
            container.Register<ITerminalServerDetection, TerminalServerDetection>();

            container.Register<IRepairSpoolFolderAssistant, RepairSpoolFolderAssistant>();
            container.Register<ISpoolFolderAccess, SpoolFolderAccess>();
            container.Register<IShellExecuteHelper, ShellExecuteHelper>();
            container.RegisterSingleton<IPrinterPortReader, PrinterPortReader>();
            RegisterPrinterHelper(container);

            container.Register<IFileAssoc, FileAssoc>();
            container.RegisterSingleton<IHashUtil, HashUtil>();

            container.Register<ISignaturePasswordCheck, SignaturePasswordCheck>();

            container.Register<IWelcomeSettingsHelper, WelcomeSettingsHelper>();

            container.Register<IEmailClientFactory, EmailClientFactory>();
            container.Register<IProfileChecker, ProfileChecker>();
            container.Register<IPrinterActionsAssistant, PrinterActionsAssistant>();
            container.Register<IRepairPrinterAssistant, RepairPrinterAssistant>();
            container.Register<IDispatcher, DispatcherWrapper>();
            container.RegisterSingleton<ISettingsMover, SettingsMover>();
            container.RegisterSingleton<IRegistryUtility, RegistryUtility>();
            container.Register<IMaybePipedApplicationStarter, MaybePipedApplicationStarter>();
            container.Register<ITokenReplacerFactory, TokenReplacerFactory>();

            container.RegisterSingleton<ISettingsLoader, SettingsLoader>();

            container.Register<IIniSettingsAssistant, IniSettingsAssistant>();
            container.Register<IIniSettingsLoader, IniSettingsLoader>();
            container.Register<IDataStorageFactory, DataStorageFactory>();
            container.RegisterSingleton<IJobInfoManager, JobInfoManager>();
            container.Register<IStaticPropertiesHack, StaticPropertiesHack>();

            container.RegisterSingleton<IMainWindowThreadLauncher, MainWindowThreadLauncher>();
            container.Register<IManagePrintJobExceptionHandler, ManagePrintJobExceptionHandler>();
            container.Register<IPdfCreatorCleanUp, PdfCreatorCleanUp>();

            container.Register<ISpooledJobFinder, SpooledJobFinder>();

            container.Register<IFtpConnectionFactory, FtpConnectionFactory>();
            container.Register<IScriptActionHelper, ScriptAction>();

            container.Register<IJobBuilder, JobBuilder>();

            container.Register<IJobFinishedHandler, JobFinishedHandler>();

            container.Register<IDropboxService, DropboxService>();
            container.Register<IDropboxSharedLinksProvider, DropboxSharedLinksProvider>();
            container.Register<IWinInetHelper, WinInetHelper>();
            container.RegisterSingleton<ITitleReplacerProvider, SettingsTitleReplacerProvider>();

            RegisterFolderProvider(container);
            RegisterUserGuideHelper(container);
            RegisterTranslator(container);
            RegisterMailSigantureHelper(container);
            RegisterSettingsHelper(container);
            RegisterStartupConditions(container);
            RegisterActions(container);
            RegisterActionChecks(container);
            RegisterFileNameQuery(container);
            RegisterUpdateAssistant(container);
            RegisterPdfProcessor(container);
            RegisterUserTokenExtractor(container);
            RegisterPlusHintHelper(container);
            
            container.RegisterSingleton(BuildCustomization);
        }

        protected abstract void RegisterUpdateAssistant(Container container);

        protected abstract void RegisterActivationHelper(Container container);

        protected abstract void RegisterUserTokenExtractor(Container container);

        protected abstract void RegisterPdfProcessor(Container container);

        protected virtual void RegisterMailSigantureHelper(Container container)
        {
            container.Register<IMailSignatureHelper, MailSignatureHelperLicensed>();
        }

        protected virtual void RegisterPlusHintHelper(Container container)
        {
            container.Register<IPlusHintHelper, PlusHintHelperDisabled>();
        }

        protected virtual ViewCustomization BuildCustomization()
        {
            return ViewCustomization.DefaultCustomization;
        }

        private void RegisterPrinterHelper(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<Core.Printing.Printer.PrinterHelper>(container);

            container.AddRegistration(typeof(IPrinterHelper), registration);
            container.AddRegistration(typeof(IPrinterProvider), registration);
        }

        private void RegisterWorkflowDependent<TInterface, TInteractive, TAutosave>(Container container) where TInteractive : class, TInterface where TAutosave : class, TInterface where TInterface : class
        {
            container.Register<TInteractive>();
            container.Register<TAutosave>();

            container.Register<TInterface>(delegate
            {
                if (_workflowFactory.WorkflowMode == WorkflowModeEnum.Interactive)
                    return container.GetInstance<TInteractive>();

                return container.GetInstance<TAutosave>();
            });
        }

        private void RegisterActionChecks(Container container)
        {
            container.RegisterCollection<ICheckable>(new[]
            {
                typeof(FtpAction),
                typeof(ScriptAction),
                typeof(DropboxAction),
                typeof(SmtpMailAction)

            });
        }

        private void RegisterFileNameQuery(Container container)
        {
            var registration = Lifestyle.Transient.CreateRegistration<InteractiveFileNameQuery>(container);
            container.AddRegistration(typeof(IFileNameQuery), registration);
            container.AddRegistration(typeof(IRetypeFileNameQuery), registration);
        }

        protected abstract IList<Type> GetStartupConditions(IList<Type> defaultConditions);

        private void RegisterStartupConditions(Container container)
        {
            var defaultConditions = new[]
            {
                typeof(SpoolerRunningCondition),
                typeof(CheckSpoolFolderCondition),
                typeof(GhostscriptCondition),
                typeof(PrinterInstalledCondition)
            }.ToList();

            var conditions = GetStartupConditions(defaultConditions);
            container.RegisterCollection<IStartupCondition>(conditions);

            container.Register<ICheckAllStartupConditions, CheckAllStartupConditions>();
        }

        private void RegisterActions(Container container)
        {
            container.RegisterCollection<IAction>(new[]
            {
                typeof(DefaultViewerAction),
                typeof(DropboxAction),
                typeof(EMailClientAction),
                typeof(FtpAction),
                typeof(PrintingAction),
                typeof(ScriptAction),
                typeof(SmtpMailAction)
            });

            container.Register<ISmtpMailAction, SmtpMailAction>();
        }

        private void RegisterFolderProvider(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<FolderProvider>(container);

            container.AddRegistration(typeof(ITempFolderProvider), registration);
            container.AddRegistration(typeof(ISpoolerProvider), registration);
        }

        private void RegisterUserGuideHelper(Container container)
        {
            container.RegisterSingleton<IUserGuideLauncher, UserGuideLauncher>();
            var registration = Lifestyle.Singleton.CreateRegistration<UserGuideHelper>(container);
            container.AddRegistration(typeof(UserGuideHelper), registration);
            container.AddRegistration(typeof(IUserGuideHelper), registration);
        }

        private void RegisterTranslator(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<TranslationHelper>(container);
            container.AddRegistration(typeof(BaseTranslationHelper), registration);
            container.AddRegistration(typeof(TranslationHelper), registration);
            container.AddRegistration(typeof(ILanguageProvider), registration);

            var translationFactory = new TranslationFactory();
            registration = Lifestyle.Singleton.CreateRegistration(() => translationFactory, container);
            container.AddRegistration(typeof(TranslationFactory), registration);
            container.AddRegistration(typeof(ITranslationFactory), registration);

            var translatables = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(t => typeof(ITranslatable).IsAssignableFrom(t) && !t.IsAbstract).ToList();
            foreach (var t in translatables)
            {
                var reg = Lifestyle.Transient.CreateRegistration(t, () => translationFactory.CreateTranslation(t), container);
                container.AddRegistration(t, reg);
            }
        }

        protected abstract SettingsProvider CreateSettingsProvider();

        private void RegisterSettingsHelper(Container container)
        {
            container.RegisterSingleton<ISettingsManager, SettingsManager>();

            // Register the same SettingsHelper for SettingsHelper and ISettingsProvider
            var registration = Lifestyle.Singleton.CreateRegistration(CreateSettingsProvider, container);
            container.AddRegistration(typeof(SettingsProvider), registration);
            container.AddRegistration(typeof(ISettingsProvider), registration);
            container.AddRegistration(typeof(IApplicationLanguageProvider), registration);
        }

        public void RegisterInteractions(WindowRegistry windowRegistry)
        {
            windowRegistry.RegisterInteraction(typeof(ApplicationSettingsInteraction), typeof(ApplicationSettingsWindow));
            windowRegistry.RegisterInteraction(typeof(InputInteraction), typeof(InputBoxWindow));
            windowRegistry.RegisterInteraction(typeof(MessageInteraction), typeof(MessageWindow));
            windowRegistry.RegisterInteraction(typeof(MainWindowInteraction), typeof(MainWindow));
            windowRegistry.RegisterInteraction(typeof(PlusHintInteraction), typeof(PlusHintWindow));
            windowRegistry.RegisterInteraction(typeof(ManagePrintJobsInteraction), typeof(ManagePrintJobsWindow));
            windowRegistry.RegisterInteraction(typeof(PrintJobInteraction), typeof(PrintJobWindow));
            windowRegistry.RegisterInteraction(typeof(ProfileSettingsInteraction), typeof(ProfileSettingsWindow));
            windowRegistry.RegisterInteraction(typeof(ConversionProgressInteraction), typeof(ConversionProgressWindow));
            windowRegistry.RegisterInteraction(typeof(EncryptionPasswordInteraction), typeof(EncryptionPasswordsWindow));
            windowRegistry.RegisterInteraction(typeof(AboutWindowInteraction), typeof(AboutWindow));
            windowRegistry.RegisterInteraction(typeof(EditEmailTextInteraction), typeof(EditEmailTextWindow));
            windowRegistry.RegisterInteraction(typeof(ProfileProblemsInteraction), typeof(DefectiveProfilesWindow));
            windowRegistry.RegisterInteraction(typeof(UpdateDownloadInteraction), typeof(UpdateDownloadWindow));
            windowRegistry.RegisterInteraction(typeof(RecommendPdfArchitectInteraction), typeof(RecommendPdfArchitectWindow));
            windowRegistry.RegisterInteraction(typeof(SignaturePasswordInteraction), typeof(SignaturePasswordWindow));
            windowRegistry.RegisterInteraction(typeof(PasswordInteraction), typeof(PasswordWindow));
            windowRegistry.RegisterInteraction(typeof(WelcomeInteraction), typeof(WelcomeWindow));
            windowRegistry.RegisterInteraction(typeof(OfflineActivationInteraction), typeof(OfflineActivationWindow));
            windowRegistry.RegisterInteraction(typeof(LicenseInteraction), typeof(LicenseWindow));
            windowRegistry.RegisterInteraction(typeof(DropboxInteraction), typeof(DropboxAuthenticationWindow));
            windowRegistry.RegisterInteraction(typeof(UpdateAvailableInteraction), typeof(UpdateAvailableWindow));
            windowRegistry.RegisterInteraction(typeof(StoreLicenseForAllUsersInteraction), typeof(StoreLicenseForAllUsersWindow));
            windowRegistry.RegisterInteraction(typeof(DropboxSharedLinksInteraction), typeof(DropboxSharedLinksWindow));
        }
    }
}
