using pdfforge.DataStorage;
using pdfforge.Mail;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Helper;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Interface;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.ConverterInterface;
using pdfforge.PDFCreator.Conversion.Dropbox;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Conversion.Ghostscript.Conversion;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Communication;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.Core.Printing;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using pdfforge.PDFCreator.Core.Startup.AppStarts;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.Editions.EditionBase.Tab;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands.FirstTimeCommands;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Notifications;
using pdfforge.PDFCreator.UI.Presentation.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.UserToken;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Attachment;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Stamp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UI.Presentation.Windows.Startup;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.UI.Presentation.WorkflowQuery;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using pdfforge.PDFCreator.UI.ViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.Views.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.IO;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Registry;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.UserGuide;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;
using SystemWrapper;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using Translatable;
using DefaultViewerView = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings.DefaultViewerView;
using FtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.FtpAccountView;
using InputBoxUserControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Dialogs.InputBoxUserControl;
using LicenseUpdateControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License.LicenseUpdateControl;
using ManagePrintJobsWindow = pdfforge.PDFCreator.UI.Presentation.Windows.ManagePrintJobsWindow;
using OutputFormatTab = pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs.OutputFormatTab;
using PrintJobShell = pdfforge.PDFCreator.UI.Presentation.PrintJobShell;
using SignUserControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign.SignUserControl;
using SmtpAccountView = pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.SmtpAccountView;
using StoreLicenseForAllUsersControl = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License.StoreLicenseForAllUsersControl;
using TitleReplacementsView = pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings.TitleReplacementsView;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    public abstract class Bootstrapper
    {
        private WorkflowFactory _workflowFactory;

        protected abstract string EditionName { get; }
        protected abstract Color EditionHighlightColor { get; }
        protected abstract bool HideLicensing { get; }
        protected abstract bool ShowWelcomeWindow { get; }
        protected abstract EditionHintOptionProvider ShowOnlyForPlusAndBusinessHint { get; }
        protected abstract ButtonDisplayOptions ButtonDisplayOptions { get; }

        public void ConfigureContainer(Container container)
        {
            container.Options.SuppressLifestyleMismatchVerification = true;

            RegisterActivationHelper(container);
            container.RegisterSingleton(() => new ApplicationNameProvider(EditionName));
            container.RegisterSingleton<IHightlightColorRegistration>(() => new HightlightColorRegistration(EditionHighlightColor));
            container.RegisterSingleton(() => new LicenseOptionProvider(HideLicensing));
            container.RegisterSingleton(() => ShowOnlyForPlusAndBusinessHint);
            container.RegisterSingleton(() => ButtonDisplayOptions);
            container.RegisterSingleton(() => new DropboxAppData(Data.Decrypt("r4IH27xLkSb2FWkNUcPfwA=="), "https://www.dropbox.com/1/oauth2/redirect_receiver"));

            if (ShowWelcomeWindow)
                container.Register<WelcomeCommand, ShowWelcomeWindowCommand>();
            else
                container.Register<WelcomeCommand, DisabledWelcomeWindowCommand>();

            var regPath = @"Software\pdfforge\PDFCreator";
            container.RegisterSingleton<IInstallationPathProvider>(() => new InstallationPathProvider(regPath, regPath + @"\Settings", "{0001B4FD-9EA3-4D90-A79E-FD14BA3AB01D}"));

            _workflowFactory = new WorkflowFactory(container);
            container.Register<AutoSaveWorkflow>(); // Workflow is registered so all dependencies can be verified in test
            container.Register<InteractiveWorkflow>(); // Workflow is registered so all dependencies can be verified in test

            container.RegisterSingleton<ICommandLocator>(() => new CommandLocator(container));

            container.Register<IWorkflowFactory>(() => _workflowFactory);

            container.Register<IOutputFileMover, AutosaveOutputFileMover>();

            container.Register<ITargetFileNameComposer, TargetFileNameComposer>();
            container.Register<IErrorNotifier, ErrorNotifierInteractive>();
            container.Register<IInteractiveProfileChecker, InteractiveProfileChecker>();

            container.RegisterSingleton<IRecommendArchitect, RecommendArchitect>();

            container.RegisterSingleton<IInteractionInvoker, InteractionInvoker>();

            RegisterInteractionRequestPerThread(container);

            container.Register<ISoundPlayer, SoundPlayer>();
            container.Register<ISmtpTest, SmtpTestEmailAssistant>();
            container.Register<IClientTestEmail, ClientTestEmail>();
            container.Register<IActionManager, ActionManager>();

            container.Register<IJobPrinter, GhostScriptPrinter>();

            container.Register<IJobDataUpdater, JobDataUpdater>();
            container.Register<IPageNumberCalculator, PageNumberCalculator>();

            container.Register<IJobRunner, JobRunner>();
            container.Register<IConverterFactory, ConverterFactory>();
            container.Register<IPsConverterFactory, GhostscriptConverterFactory>();

            container.Register<IJobCleanUp, JobCleanUp>();

            container.Register<ISystemPrinterProvider, SystemPrinterProvider>();

            container.Register<IDirectConversionProvider, DirectConversionProvider>();
            container.Register<IDirectConversionHelper, DirectConversionHelper>();
            container.RegisterSingleton<IFileConversionAssistant, FileConversionAssistant>();
            container.Register<IPrintFileHelper, PrintFileAssistant>();
            container.Register<IUacAssistant, UacAssistant>();
            container.Register<ITestPageHelper, TestPageHelper>();
            container.RegisterSingleton<IPdfArchitectCheck, PdfArchitectCheck>();
            container.Register<IGhostscriptDiscovery, GhostscriptDiscovery>();
            container.Register<ProfileRemoveCommand>();
            container.Register<ProfileRenameCommand>();

            container.RegisterSingleton<IRegistry, RegistryWrap>();
            container.RegisterSingleton<IFile, FileWrap>();
            container.RegisterSingleton<IDirectory, DirectoryWrap>();
            container.RegisterSingleton<IPath, PathWrap>();
            container.RegisterSingleton<IPathSafe, PathWrapSafe>();
            container.RegisterSingleton<IPathUtil, PathUtil>();
            container.RegisterSingleton<IEnvironment, EnvironmentWrap>();
            container.RegisterSingleton<IDirectoryHelper, DirectoryHelper>();
            container.RegisterSingleton<IReadableFileSizeFormatter, ReadableReadableFileSizeFormatter>();

            container.RegisterSingleton<IProcessStarter, ProcessStarter>();
            container.RegisterSingleton<IDateTimeProvider, DateTimeProvider>();

            container.RegisterSingleton<IAssemblyHelper>(() => new AssemblyHelper(GetType().Assembly));
            container.Register<IOsHelper, OsHelper>();
            container.Register<IFontHelper, FontHelper>();
            container.Register<IOpenFileInteractionHelper, OpenFileInteractionHelper>();
            container.Register<IPDFCreatorNameProvider, PDFCreatorNameProvider>();

            container.RegisterSingleton<IJobInfoQueueManager, JobInfoQueueManager>();
            container.Register<IJobInfoQueue, JobInfoQueue>(Lifestyle.Singleton);
            container.Register<IThreadManager, ThreadManager>(Lifestyle.Singleton);
            container.Register<IPipeServerManager, PipeServerManager>(Lifestyle.Singleton);
            container.RegisterSingleton<ITranslationUpdater, TranslationUpdater>();
            container.RegisterSingleton<IPipeMessageHandler, NewPipeJobHandler>();

            container.RegisterSingleton<IVersionHelper>(() => new VersionHelper(GetType().Assembly));
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
            container.Register<IDefaultViewerCheck, DefaultViewerCheck>();

            container.Register<IAppSettingsChecker, AppSettingsChecker>();
            container.Register<IPrinterActionsAssistant, PrinterActionsAssistant>();
            container.Register<IRepairPrinterAssistant, RepairPrinterAssistant>();
            container.Register<IDispatcher, DispatcherWrapper>();
            container.RegisterSingleton<ISettingsMover, SettingsMover>();
            container.RegisterSingleton<IRegistryUtility, RegistryUtility>();
            container.Register<IMaybePipedApplicationStarter, MaybePipedApplicationStarter>();
            container.Register<ITokenReplacerFactory, TokenReplacerFactory>();

            container.RegisterSingleton<ISettingsLoader, SettingsLoader>();
            container.RegisterSingleton<ISettingsChanged, SettingsChanged>();

            container.RegisterSingleton<IJobHistoryStorage, JobHistoryJsonFileStorage>();
            container.RegisterSingleton<IJobHistoryManager, JobHistoryManager>();

            container.Register<IIniSettingsAssistant, IniSettingsAssistant>();
            container.Register<IIniSettingsLoader, IniSettingsLoader>();
            container.RegisterSingleton<IDataStorageFactory, DataStorageFactory>();
            container.RegisterSingleton<IJobInfoManager, JobInfoManager>();
            container.Register<IStaticPropertiesHack, StaticPropertiesHack>();

            container.Register<IManagePrintJobExceptionHandler, ManagePrintJobExceptionHandler>();
            container.Register<IPdfCreatorCleanUp, PdfCreatorCleanUp>();

            container.Register<ISpooledJobFinder, SpooledJobFinder>();

            container.Register<IFtpConnectionFactory, FtpConnectionFactory>();
            container.Register<IScriptActionHelper, ScriptAction>();

            container.Register<IWorkflowNavigationHelper, WorkflowNavigationHelper>();

            container.Register<IDropboxService, DropboxService>();
            container.Register<IClipboardService, ClipboardService>();
            container.Register<IWinInetHelper, WinInetHelper>();
            container.RegisterSingleton<ITitleReplacerProvider, SettingsTitleReplacerProvider>();

            container.RegisterSingleton<IMainWindowThreadLauncher, MainShellLauncher>();

            container.Register<ILastSaveDirectoryHelper, LastSaveDirectoryHelper>();
            container.Register<ITokenViewModelFactory, TokenViewModelFactory>();

            container.RegisterSingleton<IRegionHelper, RegionHelper>();

            container.RegisterSingleton<IGpoSettings>(GetGpoSettings);

            RegisterCurrentSettingsProvider(container);
            RegisterFolderProvider(container);
            RegisterUserGuideHelper(container);
            RegisterTranslator(container);
            RegisterMailSigantureHelper(container);
            RegisterParameterSettingsManager(container);
            RegisterSettingsHelper(container);
            RegisterStartupConditions(container);
            RegisterActions(container);
            RegisterActionChecks(container);
            RegisterFileNameQuery(container);
            RegisterUpdateAssistant(container);
            RegisterJobBuilder(container);
            RegisterInteractiveWorkflowManagerFactory(container);
            RegisterPdfProcessor(container);
            RegisterUserTokenExtractor(container);
            RegisterPlusHintHelper(container);
            RegisterFirstTimeCommand(container);
            RegisterNotificationService(container);
            container.RegisterSingleton(BuildCustomization);
        }

        private void RegisterInteractionRequestPerThread(Container container)
        {
            var threadMapping = new Dictionary<Thread, IInteractionRequest>();

            container.Register<IInteractionRequest>(() =>
            {
                var thread = Thread.CurrentThread;
                if (threadMapping.ContainsKey(thread))
                    return threadMapping[thread];

                var interactionRequest = new InteractionRequest();
                threadMapping[thread] = interactionRequest;

                return interactionRequest;
                //using (ThreadScopedLifestyle.BeginScope(container))
                //{
                //    return container.GetInstance<IInteractionRequest>();
                //}
            });
        }

        private void RegisterCurrentSettingsProvider(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<CurrentSettingsProvider>(container);
            container.AddRegistration(typeof(ISelectedProfileProvider), registration);
            container.AddRegistration(typeof(ICurrentSettingsProvider), registration);
            container.AddRegistration(typeof(CurrentSettingsProvider), registration);
        }

        protected void RegisterFirstTimeCommand(Container container)
        {
            container.RegisterCollection<IFirstTimeCommand>(new[]
            {
                typeof(ResetShowQuickActionCommand)
            });
        }

        protected abstract void RegisterUpdateAssistant(Container container);

        protected abstract void RegisterJobBuilder(Container container);

        protected abstract void RegisterInteractiveWorkflowManagerFactory(Container container);

        protected abstract void RegisterActivationHelper(Container container);

        protected abstract void RegisterUserTokenExtractor(Container container);

        protected abstract void RegisterPdfProcessor(Container container);

        protected abstract IGpoSettings GetGpoSettings();

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

        private void RegisterActionChecks(Container container)
        {
            container.RegisterCollection<ICheckable>(new[]
            {
                typeof(FtpAction),
                typeof(ScriptAction),
                typeof(DropboxAction),
                typeof(HttpAction),
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
                typeof(HttpAction),
                typeof(ScriptAction),
                typeof(SmtpMailAction)
            });

            container.Register<ISmtpMailAction, SmtpMailAction>();
            container.Register<IEMailClientAction, EMailClientAction>();
            container.Register<IDefaultViewerAction, DefaultViewerAction>();
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
            container.AddRegistration(typeof(ITranslationHelper), registration);

            var translationFactory = new TranslationFactory();
            container.RegisterSingleton(() => translationFactory);
            var cachedTranslationFactory = new CachedTranslationFactory(translationFactory);
            registration = Lifestyle.Singleton.CreateRegistration(() => cachedTranslationFactory, container);
            container.AddRegistration(typeof(CachedTranslationFactory), registration);
            container.AddRegistration(typeof(ITranslationFactory), registration);
        }

        private void RegisterParameterSettingsManager(Container container)
        {
            container.RegisterSingleton<IParametersManager, ParametersManager>();
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

        public void RegisterInteractions()
        {
            ViewRegistry.RegisterInteraction(typeof(WelcomeView), typeof(WelcomeView));
            ViewRegistry.RegisterInteraction(typeof(PrintJobInteraction), typeof(PrintJobShell));
            ViewRegistry.RegisterInteraction(typeof(InputInteraction), typeof(InputBoxUserControl));
            ViewRegistry.RegisterInteraction(typeof(MessageInteraction), typeof(MessageView));
            ViewRegistry.RegisterInteraction(typeof(ManagePrintJobsInteraction), typeof(ManagePrintJobsWindow));
            ViewRegistry.RegisterInteraction(typeof(EncryptionPasswordInteraction), typeof(EncryptionPasswordsUserControl));
            ViewRegistry.RegisterInteraction(typeof(EditEmailTextInteraction), typeof(EditEmailTextUserControl));
            ViewRegistry.RegisterInteraction(typeof(UpdateDownloadInteraction), typeof(UpdateDownloadWindow));
            ViewRegistry.RegisterInteraction(typeof(RecommendPdfArchitectInteraction), typeof(RecommendPdfArchitectView), new WindowOptions { ResizeMode = ResizeMode.NoResize });
            ViewRegistry.RegisterInteraction(typeof(PasswordOverlayInteraction), typeof(PasswordOverlay));
            ViewRegistry.RegisterInteraction(typeof(SignaturePasswordInteraction), typeof(SignaturePasswordOverlayView));
            ViewRegistry.RegisterInteraction(typeof(OfflineActivationInteraction), typeof(OfflineActivationUserControl));
            ViewRegistry.RegisterInteraction(typeof(LicenseInteraction), typeof(LicenseUpdateControl));
            ViewRegistry.RegisterInteraction(typeof(DropboxAccountInteraction), typeof(DropboxAccountWindow));
            ViewRegistry.RegisterInteraction(typeof(StoreLicenseForAllUsersInteraction), typeof(StoreLicenseForAllUsersControl));
            ViewRegistry.RegisterInteraction(typeof(FtpAccountInteraction), typeof(FtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(SmtpAccountInteraction), typeof(SmtpAccountView));
            ViewRegistry.RegisterInteraction(typeof(HttpAccountInteraction), typeof(HttpAccountView));
            ViewRegistry.RegisterInteraction(typeof(TimeServerAccountInteraction), typeof(TimeServerAccountView));
            ViewRegistry.RegisterInteraction(typeof(TitleReplacementEditInteraction), typeof(TitleReplacementEditUserControl));
        }

        public TabRegion DefineProfileSettingsTabs()
        {
            var profileTabs = new TabRegion(RegionNames.ProfileTabContentRegion);

            profileTabs.Add(new SimpleTab<SaveTab, SaveTabViewModel>(RegionNames.SaveTabContentRegion, HelpTopic.ProfileSave));
            // TODO define proper help topic
            profileTabs.Add(new MultiTab<ConvertTabViewModel>(RegionNames.ConvertTabContentRegion, HelpTopic.ProfileSave, typeof(OutputFormatTab), typeof(ConvertPdfView), typeof(ConvertJpgView), typeof(ConvertPngView), typeof(ConvertTiffView), typeof(ConvertTextView)));
            profileTabs.Add(new SimpleTab<MetadataTab, MetadataViewModel>(RegionNames.MetadataTabContentRegion, HelpTopic.ProfileMetadata));

            var modifyTab = new MasterTab<ModifyMasterTabViewModel>(RegionNames.ModifyMasterTabItemsRegion, RegionNames.ModifyMasterTabContentRegion);
            modifyTab.AddSubTab(new SubTab<CoverUserControl, ProfileModifyTranslation>(t => t.Cover, profile => profile.CoverPage));
            modifyTab.AddSubTab(new SubTab<BackgroundUserControl, ProfileModifyTranslation>(t => t.Background, profile => profile.BackgroundPage));
            modifyTab.AddSubTab(new SubTab<AttachmentUserControl, ProfileModifyTranslation>(t => t.Attachment, profile => profile.AttachmentPage));
            modifyTab.AddSubTab(new SubTab<StampUserControl, ProfileModifyTranslation>(t => t.Stamp, profile => profile.Stamping));
            profileTabs.Add(modifyTab);

            var sendTab = new MasterTab<SendMasterTabViewModel>(RegionNames.SendMasterTabItemsRegion, RegionNames.SendMasterTabContentRegion);
            sendTab.AddSubTab(new SubTab<FTPActionUserControl, ProfileSendSubTabTranslation>(t => t.Ftp, profile => profile.Ftp));
            sendTab.AddSubTab(new SubTab<MailClientUserControl, MailClientTabTranslation>(t => t.Email, profile => profile.EmailClientSettings));
            sendTab.AddSubTab(new SubTab<HttpActionUserControl, HttpTranslation>(t => t.HttpSubTabTitle, profile => profile.HttpSettings));
            sendTab.AddSubTab(new SubTab<SmtpActionUserControl, SmtpTranslation>(t => t.SmtpSubTabTitle, profile => profile.EmailSmtpSettings));
            sendTab.AddSubTab(new SubTab<DropboxUserControl, DropboxTranslation>(t => t.Dropbox, profile => profile.DropboxSettings));
            sendTab.AddSubTab(new SubTab<PrintUserControl, PrintTabTranslation>(t => t.Print, profile => profile.Printing));
            profileTabs.Add(sendTab);

            var secureTab = new MasterTab<SecureMasterTabViewModel>(RegionNames.SecureMasterTabItemsRegion, RegionNames.SecureMasterTabContentRegion);
            secureTab.AddSubTab(new SubTab<EncryptUserControl, ProfileSecureTranslation>(t => t.Encryption, profile => profile.PdfSettings.Security));
            secureTab.AddSubTab(new SubTab<SignUserControl, ProfileSecureTranslation>(t => t.Signature, profile => profile.PdfSettings.Signature));
            profileTabs.Add(secureTab);

            var advancedTab = new MasterTab<AdvancedMasterTabViewModel>(RegionNames.AdvancedMasterTabItemsRegion, RegionNames.AdvancedMasterTabContentRegion);
            advancedTab.AddSubTab(new SubTab<ScriptUserControl, ProfileAdvancedTranslation>(t => t.Script, profile => profile.Scripting));
            advancedTab.AddSubTab(new SubTab<UserTokenUserControl, ProfileAdvancedTranslation>(t => t.UserToken, profile => profile.UserTokens));
            profileTabs.Add(advancedTab);

            ModifyProfileSettingsTabs(profileTabs);

            return profileTabs;
        }

        public TabRegion DefineApplicationSettingsTabs()
        {
            var applicationSettingsTabs = new TabRegion(RegionNames.SettingsTabRegion);

            applicationSettingsTabs.Add(new MultiTab<GeneralSettingsViewModel>(RegionNames.GeneralSettingsTabContentRegion, HelpTopic.AppGeneral, typeof(LanguageSelectionSettingsView), typeof(UpdateIntervalSettingsView), typeof(DefaultPrinterSettingsView), typeof(ExplorerIntegrationSettingsView)));
            applicationSettingsTabs.Add(new SimpleTab<TitleReplacementsView, TitleReplacementsViewModel>(RegionNames.TitleReplacementTabContentRegion, HelpTopic.AppTitle));
            applicationSettingsTabs.Add(new SimpleTab<DefaultViewerView, DefaultViewerViewModel>(RegionNames.DefaultViewerTabContentRegion, HelpTopic.AppViewer));
            applicationSettingsTabs.Add(new MultiTab<DebugSettingsViewModel>(RegionNames.DebugSettingsTabContentRegion, HelpTopic.AppDebug, typeof(LoggingSettingView), typeof(TestPageSettingsView), typeof(RestoreSettingsView), typeof(ExportSettingView)));

            ModifyApplicationSettingsTabs(applicationSettingsTabs);

            return applicationSettingsTabs;
        }

        protected virtual void ModifyProfileSettingsTabs(TabRegion profileSettingsTabs)
        {
        }

        protected virtual void ModifyApplicationSettingsTabs(TabRegion applicationSettingsTabs)
        {
        }

        public virtual void RegisterEditiondependentRegions(IRegionManager regionManager)
        {
        }

        public virtual void RegisterNotificationService(Container container)
        {
            container.RegisterSingleton<INotificationService, NotificationService>();
        }
    }
}
