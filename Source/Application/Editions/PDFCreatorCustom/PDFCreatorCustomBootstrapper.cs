using pdfforge.DataStorage;
using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.GpoAdapter;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.PDFCreatorCustom.Properties;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace pdfforge.PDFCreator.Editions.PDFCreatorCustom
{
    public class PDFCreatorCustomBootstrapper : Bootstrapper
    {
        protected override string EditionName => "Custom";
        protected override Color EditionHighlightColor => Color.FromRgb(150, 76, 224);
        protected override bool HideLicensing => true;
        protected override bool ShowWelcomeWindow => false;
        protected override EditionHintOptionProvider ShowOnlyForPlusAndBusinessHint => new EditionHintOptionProvider(false, false);
        protected override ButtonDisplayOptions ButtonDisplayOptions => new ButtonDisplayOptions(true, true);

        public bool ValidOnTerminalServer => Customization.ApplyCustomization.Equals("true", StringComparison.InvariantCultureIgnoreCase);

        protected override void RegisterUpdateAssistant(Container container)
        {
            container.RegisterSingleton<IUpdateAssistant, DisabledUpdateAssistant>();
        }

        protected override void RegisterJobBuilder(Container container)
        {
            container.Register<IJobBuilder, JobBuilderPlus>();
        }

        protected override void RegisterInteractiveWorkflowManagerFactory(Container container)
        {
            container.Register<IInteractiveWorkflowManagerFactory, InteractiveWorkflowManagerFactory>();
        }

        protected override void RegisterActivationHelper(Container container)
        {
            container.Register<ILicenseChecker, UnlicensedLicenseChecker>();
            container.Register<IOfflineActivator, UnlicensedOfflineActivator>();

            container.Register<PrioritySupportUrlOpenCommand, CustomPrioritySupportUrlOpenCommand>();
        }

        protected override void RegisterUserTokenExtractor(Container container)
        {
            container.Register<IPsParserFactory, PsParserFactory>();
            container.Register<IUserTokenExtractor, UserTokenExtractor>();
        }

        protected override IList<Type> GetStartupConditions(IList<Type> defaultConditions)
        {
            if (!ValidOnTerminalServer)
                defaultConditions.Add(typeof(TerminalServerNotAllowedCondition));

            defaultConditions.Add(typeof(PdfToolsLicensingStartUpCondition));

            return defaultConditions;
        }

        protected override ViewCustomization BuildCustomization()
        {
            if (Customization.ApplyCustomization.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                return new ViewCustomization(Customization.AboutDialog, Customization.MainForm, Customization.PrintJobWindowCaption, Customization.customlogo);
            }

            // TODO maybe throw exeption instead?
            return ViewCustomization.DefaultCustomization;
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
    }
}
