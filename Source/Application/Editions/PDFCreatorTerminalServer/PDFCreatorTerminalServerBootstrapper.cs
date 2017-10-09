using pdfforge.DataStorage;
using pdfforge.LicenseValidator.Bootstrapping;
using pdfforge.LicenseValidator.Data;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.LicenseValidator.Tools.Machine;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.GpoAdapter;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.EditionBase.Tab;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace pdfforge.PDFCreator.Editions.PDFCreatorTerminalServer
{
    public class PDFCreatorTerminalServerBootstrapper : Bootstrapper
    {
        protected override string EditionName => "Terminal Server";
        protected override Color EditionHighlightColor => Color.FromRgb(142, 142, 142);
        protected override bool HideLicensing => false;
        protected override bool ShowWelcomeWindow => false;
        protected override EditionHintOptionProvider ShowOnlyForPlusAndBusinessHint => new EditionHintOptionProvider(false, false);

        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(true, true);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, AutoUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorTerminalServerUpdateInfoUrl, "PDFCreatorTerminalServer"));
        }

        protected override void RegisterJobBuilder(Container container)
        {
            container.Register<IJobBuilder, JobBuilderPlus>();
        }

        protected override void RegisterInteractiveWorkflowManagerFactory(Container container)
        {
            container.Register<IInteractiveWorkflowManagerFactory, InteractiveWorkflowManagerFactory>();
        }

        protected Configuration BuildLicenseValidatorConfig(Product product)
        {
            var versionHelper = new VersionHelper(GetType().Assembly);
            var version = versionHelper.FormatWithThreeDigits();
            var config = new Configuration(product, version, @"SOFTWARE\pdfforge\PDFCreator");
            config.RegistryHive = RegistryHive.LocalMachine;
            config.LoadFromBothRegistryHives = false;
            config.MachineIdGenerator = new MachineIdV2Generator();

            return config;
        }

        protected override void RegisterActivationHelper(Container container)
        {
            var product = Product.PdfCreatorTerminalServer;
            var config = BuildLicenseValidatorConfig(product);

            var licenseChecker = LicenseCheckerFactory.BuildLicenseChecker(config);
            var offlineActivator = LicenseCheckerFactory.BuildOfflineActivator(config);

            container.Register<Configuration>(() => config);

            container.RegisterSingleton<ILicenseChecker>(new TerminalServerLicenseChecker(licenseChecker));
            container.RegisterSingleton(offlineActivator);
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IPsParserFactory, PsParserFactory>();
            container.Register<IUserTokenExtractor, UserTokenExtractor>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            defaultConditions.Add(typeof(LicenseCondition));
            defaultConditions.Add(typeof(PdfToolsLicensingStartUpCondition));

            return defaultConditions;
        }

        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, PdfToolsPdfProcessor>();
            container.Register<IPdfToolsLicensing>(() => new PdfToolsLicensing(Data.Decrypt));
            container.RegisterSingleton<ICertificateManager, CertificateManager>();
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new GpoAwareSettingsProvider(GetGpoSettings());
        }

        protected override IGpoSettings GetGpoSettings()
        {
            var gpoReader = new GpoReader.GpoReader(true);
            return new GpoReaderSettings(gpoReader.ReadGpoSettings());
        }

        protected override void ModifyApplicationSettingsTabs(TabRegion applicationSettingsTabs)
        {
            applicationSettingsTabs.Add(new SimpleTab<LicenseSettingsView, LicenseSettingsViewModel>(RegionNames.LicenseSettingsContentRegion, HelpTopic.AppLicense));
        }
    }
}
