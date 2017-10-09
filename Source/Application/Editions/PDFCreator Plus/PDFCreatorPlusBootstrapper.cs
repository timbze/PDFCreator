using pdfforge.DataStorage;
using pdfforge.LicenseValidator.Bootstrapping;
using pdfforge.LicenseValidator.Data;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.LicenseValidator.Tools.Machine;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
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

namespace pdfforge.PDFCreator.Editions.PDFCreatorPlus
{
    public class PDFCreatorPlusBootstrapper : Bootstrapper
    {
        protected override string EditionName => "Plus";
        protected override Color EditionHighlightColor => Color.FromRgb(87, 175, 87);
        protected override bool HideLicensing => false;
        protected override bool ShowWelcomeWindow => true;
        protected override EditionHintOptionProvider ShowOnlyForPlusAndBusinessHint => new EditionHintOptionProvider(false, true);
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(false, true);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, AutoUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorPlusUpdateInfoUrl, "PDFCreatorPlus"));
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
            config.RegistryHive = RegistryHive.CurrentUser;
            config.LoadFromBothRegistryHives = true;
            config.MachineIdGenerator = new MachineIdV2Generator();

            return config;
        }

        protected override void RegisterActivationHelper(Container container)
        {
            var product = Product.PdfCreator;
            var config = BuildLicenseValidatorConfig(product);
            config.AcceptExpiredActivation = false;

            var licenseChecker = LicenseCheckerFactory.BuildLicenseChecker(config);
            var offlineActivator = LicenseCheckerFactory.BuildOfflineActivator(config);

            container.Register<Configuration>(() => config);

            container.RegisterSingleton(licenseChecker);
            container.RegisterSingleton(offlineActivator);
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IUserTokenExtractor, UserTokenExtractorDummy>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));
            defaultConditions.Add(typeof(LicenseCondition));
            defaultConditions.Add(typeof(PdfToolsLicensingStartUpCondition));

            return defaultConditions;
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new DefaultSettingsProvider();
        }

        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, PdfToolsPdfProcessor>();
            container.Register<IPdfToolsLicensing>(() => new PdfToolsLicensing(Data.Decrypt));
            container.RegisterSingleton<ICertificateManager, CertificateManager>();
        }

        protected override IGpoSettings GetGpoSettings()
        {
            return new GpoSettingsDefaults();
        }

        protected override void ModifyApplicationSettingsTabs(TabRegion applicationSettingsTabs)
        {
            applicationSettingsTabs.Add(new SimpleTab<LicenseSettingsView, LicenseSettingsViewModel>(RegionNames.LicenseSettingsContentRegion, HelpTopic.AppLicense));
        }
    }
}
