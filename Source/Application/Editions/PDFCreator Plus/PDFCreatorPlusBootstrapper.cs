using System;
using System.Collections.Generic;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.PDFCreatorPlus
{
    public class PDFCreatorPlusBootstrapper : Bootstrapper
    {
        protected override string EditionName => "PDFCreator Plus";
        protected override bool HideLicensing => false;
        protected override bool ShowWelcomeWindow => true;
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(false, true);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, AutoUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorPlusUpdateInfoUrl, "PDFCreatorPlus"));
        }

        protected override void RegisterActivationHelper(Container container)
        {
            var product = Product.PdfCreator;
            container.RegisterSingleton<ILicenseServerHelper>(() => new LicenseServerHelper(product));

            container.RegisterSingleton<IActivationHelper>(
                () => new ActivationHelper(product, container.GetInstance<ILicenseServerHelper>())
                {
                    AcceptExpiredActivation = true,
                    AcceptExpiredLicense = true
                });
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IUserTokenExtractor, UserTokenExtractorDummy>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));
            defaultConditions.Add(typeof(LicenseCondition));

            return defaultConditions;
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new DefaultSettingsProvider();
        }
    }

    // ReSharper disable once InconsistentNaming
}
