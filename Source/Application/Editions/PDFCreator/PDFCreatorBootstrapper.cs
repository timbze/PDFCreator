using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.ITextProcessing;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.UI.ViewModels;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace pdfforge.PDFCreator.Editions.PDFCreator
{
    public class PDFCreatorBootstrapper : Bootstrapper
    {
        protected override string EditionName => "Free";
        protected override Color EditionHighlightColor => Color.FromRgb(215, 40, 40);
        protected override bool HideLicensing => true;
        protected override bool ShowWelcomeWindow => true;
        protected override EditionHintOptionProvider ShowOnlyForPlusAndBusinessHint => new EditionHintOptionProvider(true, true);
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(false, false);

        protected override void RegisterSettingsLoader(Container container)
        {
            container.RegisterSingleton<ISettingsLoader, SettingsLoader>();
        }

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, UpdateAssistant>();
            container.RegisterSingleton<IUpdateLauncher, SimpleUpdateLauncher>();
            container.RegisterSingleton(() => new UpdateInformationProvider(Urls.PdfCreatorUpdateInfoUrl, "PDFCreator"));
        }

        protected override void RegisterInteractiveWorkflowManagerFactory(Container container)
        {
            container.Register<IInteractiveWorkflowManagerFactory, InteractiveWorkflowManagerFactoryWithPlusHintStep>();
        }

        protected override void RegisterJobBuilder(Container container)
        {
            container.Register<IJobBuilder, JobBuilderFree>();
        }

        protected override void RegisterActivationHelper(Container container)
        {
            container.Register<ILicenseChecker, UnlicensedLicenseChecker>();
            container.Register<IOfflineActivator, UnlicensedOfflineActivator>();

            container.Register<PrioritySupportUrlOpenCommand, DisabledPrioritySupportUrlOpenCommand>();
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IUserTokenExtractor, UserTokenExtractorDummy>();
        }

        protected override IGpoSettings GetGpoSettings()
        {
            return new GpoSettingsDefaults();
        }

        protected override void RegisterMailSigantureHelper(Container container)
        {
            container.Register<IMailSignatureHelper, MailSignatureHelperFreeVersion>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));

            return defaultConditions;
        }

        protected override void RegisterPlusHintHelper(Container container)
        {
            container.RegisterSingleton<IPlusHintHelper, PlusHintHelper>();
        }

        protected override SettingsProvider CreateSettingsProvider()
        {
            return new DefaultSettingsProvider();
        }

        protected override void RegisterPdfProcessor(Container container)
        {
            container.Register<IPdfProcessor, ITextPdfProcessor>();
        }

        public override void RegisterEditiondependentRegions(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(RegionNames.StatusBarPlusHintRegion, typeof(PlusHintControl));
        }

        public override void RegisterNotificationService(Container container)
        {
            container.RegisterSingleton<INotificationService, DisabledNotificationService>();
        }
    }
}
