using Microsoft.Practices.ServiceLocation;
using pdfforge.PDFCreator.Editions.EditionBase.Tab;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Architect;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Home;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Printer;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using Prism.Regions.Behaviors;
using SimpleInjector;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector
{
    public class PrismBootstrapper
    {
        private readonly TabRegion _profileSettingsTabs;
        private readonly TabRegion _applicationSettingsTabs;

        public PrismBootstrapper(TabRegion profileSettingsTabs, TabRegion applicationSettingsTabs)
        {
            _profileSettingsTabs = profileSettingsTabs;
            _applicationSettingsTabs = applicationSettingsTabs;
        }

        private ILoggerFacade Logger { get; } = new TextLogger(); // TODO make implementation using NLog

        public void ConfigurePrismDependecies(Container container)
        {
            var locator = new SimpleInjectorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            container.RegisterSingleton<IServiceLocator>(() => locator);

            var whitelistedServiceLocator = new WhitelistedServiceLocator(container);
            RestrictedServiceLocator.Current = whitelistedServiceLocator;
            container.RegisterSingleton<IWhitelistedServiceLocator>(() => whitelistedServiceLocator);

            var regionManager = new RegionManager();

            var shellManager = new ShellManager(whitelistedServiceLocator, regionManager, new WpfTopMostHelper(), _profileSettingsTabs, _applicationSettingsTabs);
            container.RegisterSingleton<IRegionManager>(() => regionManager);
            container.RegisterSingleton<IShellManager>(() => shellManager);

            ConfigureContainer(container);
        }

        public void InitPrismStuff(Container container)
        {
            ConfigureRegionAdapterMappings(container); // this resolves stuff in service locator
            ConfigureDefaultRegionBehaviors(container); // this resolves stuff in service locator
        }

        private void ConfigureContainer(Container container)
        {
            container.RegisterSingleton(() => Logger);
            // container.RegisterSingleton<IModuleCatalog>(() => ModuleCatalog);

            container.RegisterSingleton(() => new RegionAdapterMappings());

            //container.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
            //container.RegisterSingleton<IModuleManager, ModuleManager>();

            container.RegisterSingleton<IEventAggregator, EventAggregator>();
            container.RegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
            container.RegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
            container.Register<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
            container.Register<IRegionNavigationJournal, RegionNavigationJournal>();
            container.Register<IRegionNavigationService, RegionNavigationService>();
            container.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
        }

        private void ConfigureDefaultRegionBehaviors(Container container)
        {
            var defaultRegionBehaviorTypesDictionary = container.GetInstance<IRegionBehaviorFactory>();

            defaultRegionBehaviorTypesDictionary.AddIfMissing(BindRegionContextToDependencyObjectBehavior.BehaviorKey,
                typeof(BindRegionContextToDependencyObjectBehavior));

            defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionActiveAwareBehavior.BehaviorKey,
                typeof(RegionActiveAwareBehavior));

            defaultRegionBehaviorTypesDictionary.AddIfMissing(SyncRegionContextWithHostBehavior.BehaviorKey,
                typeof(SyncRegionContextWithHostBehavior));

            defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionManagerRegistrationBehavior.BehaviorKey,
                typeof(RegionManagerRegistrationBehavior));

            defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionMemberLifetimeBehavior.BehaviorKey,
                typeof(RegionMemberLifetimeBehavior));

            defaultRegionBehaviorTypesDictionary.AddIfMissing(ClearChildViewsRegionBehavior.BehaviorKey,
                typeof(ClearChildViewsRegionBehavior));

            defaultRegionBehaviorTypesDictionary.AddIfMissing(AutoPopulateRegionBehavior.BehaviorKey,
                typeof(AutoPopulateRegionBehavior));
        }

        private void ConfigureRegionAdapterMappings(Container container)
        {
            var mappings = container.GetInstance<RegionAdapterMappings>();

            mappings.RegisterMapping(typeof(Selector), ServiceLocator.Current.GetInstance<SelectorRegionAdapter>());
            mappings.RegisterMapping(typeof(ItemsControl), ServiceLocator.Current.GetInstance<ItemsControlRegionAdapter>());
            mappings.RegisterMapping(typeof(ContentControl), ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>());
        }

        public void RegisterNavigationViews(Container container)
        {
            container.RegisterTypeForNavigation<AboutView>();
            container.RegisterTypeForNavigation<AccountsView>();
            container.RegisterTypeForNavigation<ArchitectView>();
            container.RegisterTypeForNavigation<HomeView>();
            container.RegisterTypeForNavigation<PrinterView>();
            container.RegisterTypeForNavigation<ProfilesView>();
            container.RegisterTypeForNavigation<SettingsView>();
            container.RegisterTypeForNavigation<PrintJobView>();
            container.RegisterTypeForNavigation<PdfPasswordView>();
            container.RegisterTypeForNavigation<QuickActionView>();
            container.RegisterTypeForNavigation<FtpPasswordView>();
            container.RegisterTypeForNavigation<SmtpPasswordView>();
            container.RegisterTypeForNavigation<HttpPasswordView>();
            container.RegisterTypeForNavigation<SignaturePasswordStepView>();
            container.RegisterTypeForNavigation<PlusHintView>();
            container.RegisterTypeForNavigation<ProgressView>();
            container.RegisterTypeForNavigation<DropboxShareLinkStepView>();

            container.RegisterTypeForNavigation<UpdateHintView>();

            _profileSettingsTabs.RegisterNavigationViews(container);
            _applicationSettingsTabs.RegisterNavigationViews(container);
        }
    }
}
