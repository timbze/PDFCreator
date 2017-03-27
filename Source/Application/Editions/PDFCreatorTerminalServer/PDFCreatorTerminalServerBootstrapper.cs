using System;
using System.Collections.Generic;
using pdfforge.DataStorage;
using pdfforge.LicenseValidator.Bootstrapping;
using pdfforge.LicenseValidator.Data;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.GpoAdapter;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.Utilities;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.PDFCreatorTerminalServer
{
    public class PDFCreatorTerminalServerBootstrapper : Bootstrapper
    {
        protected override string EditionName => "PDFCreator Terminal Server";
        protected override bool HideLicensing => false;
        protected override bool ShowWelcomeWindow => false;
        protected override bool ShowOnlyForPlusAndBusinessHint => false;

        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(true, true);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, AutoUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorTerminalServerUpdateInfoUrl, "PDFCreatorTerminalServer"));
        }

        protected Configuration BuildLicenseValidatorConfig(Product product)
        {
            var versionHelper = new VersionHelper(new AssemblyHelper());
            var version = versionHelper.FormatWithThreeDigits();
            var config = new Configuration(product, version, @"SOFTWARE\pdfforge\PDFCreator");
            config.RegistryHive = RegistryHive.LocalMachine;
            config.LoadFromBothRegistryHives = false;

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

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new GpoAwareSettingsProvider();
        }

        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, PdfToolsPdfProcessor>();
            container.Register<IPdfToolsLicensing>(() => new PdfToolsLicensing(Data.Decrypt));
        }
    }
}
