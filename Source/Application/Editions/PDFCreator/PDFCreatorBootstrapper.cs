using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.PDFCreator
{
    public class PDFCreatorBootstrapper : Bootstrapper
    {
        protected override string EditionName => "PDFCreator";
        protected override bool HideLicensing => true;
        protected override bool ShowWelcomeWindow => true;
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(false, false);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, SimpleUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorUpdateInfoUrl, "PDFCreator"));
        }

        protected override void RegisterActivationHelper(Container container)
        {
            container.RegisterSingleton<ILicenseServerHelper, UnlicensedLicenseServerHelper>();
            container.RegisterSingleton<IActivationHelper, UnlicensedActivationHelper>();
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IUserTokenExtractor, UserTokenExtractorDummy>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));

            return defaultConditions;
        }

        protected override void RegisterPlusHintHelper(Container container)
        {
            container.Register<IPlusHintHelper, PlusHintHelper>();
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new DefaultSettingsProvider();
        }
    }
}
