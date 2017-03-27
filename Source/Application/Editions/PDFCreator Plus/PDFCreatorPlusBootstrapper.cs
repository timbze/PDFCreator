using System;
using System.Collections.Generic;
using pdfforge.DataStorage;
using pdfforge.LicenseValidator.Bootstrapping;
using pdfforge.LicenseValidator.Data;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.Utilities;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.PDFCreatorPlus
{
    public class PDFCreatorPlusBootstrapper : Bootstrapper
    {
        protected override string EditionName => "PDFCreator Plus";
        protected override bool HideLicensing => false;
        protected override bool ShowWelcomeWindow => true;
        protected override bool ShowOnlyForPlusAndBusinessHint => false;
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(false, true);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, AutoUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorPlusUpdateInfoUrl, "PDFCreatorPlus"));
        }

        protected Configuration BuildLicenseValidatorConfig(Product product)
        {
            var versionHelper = new VersionHelper(new AssemblyHelper());
            var version = versionHelper.FormatWithThreeDigits();
            var config = new Configuration(product, version, @"SOFTWARE\pdfforge\PDFCreator");
            config.RegistryHive = RegistryHive.CurrentUser;
            config.LoadFromBothRegistryHives = true;

            return config;
        }

        protected override void RegisterActivationHelper(Container container)
        {
            var product = Product.PdfCreator;
            var config = BuildLicenseValidatorConfig(product);
            config.AcceptExpiredActivation = true;

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
        }
    }
}
