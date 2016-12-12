using System.ComponentModel;
using System.Windows;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    internal class ApplicationSettingsViewModelTest
    {
        private ApplicationSettingsViewModel BuildViewModel()
        {
            // TODO extend tests based on Edition
            var activationHelper = Substitute.For<IActivationHelper>();

            var printerHelper = Substitute.For<IPrinterHelper>();
            printerHelper.GetApplicablePDFCreatorPrinter(Arg.Any<string>(), Arg.Any<string>()).Returns("PDFCreator");

            var licenseServerHelper = Substitute.For<ILicenseServerHelper>();
            var licenseChecker = Substitute.For<ILicenseChecker>();
            licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser)
                .ReturnsForAnyArgs(licenseChecker);
            var translator = Substitute.For<ITranslator>();
            var settingsHelper = Substitute.For<ISettingsProvider>();
            var settingsManager = Substitute.For<ISettingsManager>();
            settingsManager.GetSettingsProvider().Returns(settingsHelper);
            var updateAssistant = Substitute.For<IUpdateAssistant>();
            var uacAssistant = Substitute.For<IUacAssistant>();
            var process = Substitute.For<IProcessStarter>();

            var generalTabViewModel = new GeneralTabViewModel(Substitute.For<ILanguageProvider>(), null, translator, updateAssistant, uacAssistant, Substitute.For<IInteractionInvoker>(),
                Substitute.For<IOsHelper>(), Substitute.For<IProcessStarter>());
            var printerTabViewModel = new PrinterTabViewModel(printerHelper, translator, null, null, null, printerHelper);
            var titleTabViewModel = new TitleTabViewModel(translator);
            var debugTabViewModel = new DebugTabViewModel(translator, settingsManager, Substitute.For<ITestPageHelper>(), Substitute.For<IFile>(), Substitute.For<IProcessStarter>(), Substitute.For<IInteractionInvoker>(), printerHelper, Substitute.For<IIniSettingsAssistant>());
            var licenseTabViewModel = new LicenseTabViewModel(process, activationHelper, translator, null, Substitute.For<IDispatcher>());
            var pdfArchitectTabViewModel = new PdfArchitectTabViewModel(translator, Substitute.For<IPdfArchitectCheck>(), Substitute.For<IProcessStarter>());

            var viewModelBundle = new ApplicationSettingsViewModelBundle(generalTabViewModel, printerTabViewModel, titleTabViewModel, debugTabViewModel, licenseTabViewModel, pdfArchitectTabViewModel);

            return new ApplicationSettingsViewModel(viewModelBundle, new TranslationHelper(new TranslationProxy(), new DefaultSettingsProvider(), new AssemblyHelper()), new LicenseOptionProvider(false));
        }

        [Test]
        public void ApplicationSettings_OnSet_TriggersOnSettingsChangeEvent()
        {
            var asViewModel = BuildViewModel();
            var settingsChanged = false;
            asViewModel.SettingsChanged += (sender, args) => settingsChanged = true;

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), new TestGpoSettings());
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(settingsChanged);
        }

        [Test]
        public void DebugTabIsEnabled_WhenDisableDebugTab_IsFalse()
        {
            var asViewModel = BuildViewModel();
            var gpoSettings = new TestGpoSettings
            {
                DisableDebugTab = true
            };

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.IsFalse(asViewModel.DebugTabIsEnabled, "DebugTabIsEnabled is not the negation of DisableDebugTab");
        }

        [Test]
        public void DebugTabIsEnabled_WhenEnaableDebugTab_IsTrue()
        {
            var asViewModel = BuildViewModel();
            var gpoSettings = new TestGpoSettings
            {
                DisableDebugTab = false
            };

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(asViewModel.DebugTabIsEnabled, "DebugTabIsEnabled is not the negation of DisableDebugTab");
        }

        [Test]
        public void EmptyViewModel_SettingApplicationSettings_RaisesPropertyChanged()
        {
            var eventStub = Substitute.For<IEventHandler<PropertyChangedEventArgs>>();
            var asViewModel = BuildViewModel();
            asViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(asViewModel, "ApplicationSettings");

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), new TestGpoSettings());
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void EmptyViewModel_SettingGpoSettings_RaisesPropertyChanged()
        {
            var eventStub = Substitute.For<IEventHandler<PropertyChangedEventArgs>>();
            var asViewModel = BuildViewModel();
            asViewModel.PropertyChanged += eventStub.OnEventRaised;
            var titleTabIsEnabledPropertyListener = new PropertyChangedListenerMock(asViewModel, "TitleTabIsEnabled");
            var debugTabIsEnabledPropertyListener = new PropertyChangedListenerMock(asViewModel, "DebugTabIsEnabled");
            var printerTabIsEnabledPropertyListener = new PropertyChangedListenerMock(asViewModel, "PrinterTabIsEnabled");
            var pdfArchitectVisibiltyPropertyListener = new PropertyChangedListenerMock(asViewModel, "PdfArchitectVisibilty");

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), new TestGpoSettings());
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(titleTabIsEnabledPropertyListener.WasCalled, "TitleTabIsEnabled PropertyChanged was not called");
            Assert.IsTrue(debugTabIsEnabledPropertyListener.WasCalled, "DebugTabIsEnabled PropertyChanged was not called");
            Assert.IsTrue(printerTabIsEnabledPropertyListener.WasCalled, "PrinterTabIsEnabled PropertyChanged was not called");
            Assert.IsTrue(pdfArchitectVisibiltyPropertyListener.WasCalled, "PdfArchitectVisibilty PropertyChanged was not called");
        }

        [Test]
        public void GpoProperties_CheckInitializations()
        {
            var asViewModel = BuildViewModel();

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), null);
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(asViewModel.DebugTabIsEnabled, "DebugTabIsEnabled not initialized with true");
            Assert.IsTrue(asViewModel.PrinterTabIsEnabled, "PrinterTabIsEnabled not initialized with true");
            Assert.IsTrue(asViewModel.TitleTabIsEnabled, "TitleTabIsEnabled not initialized with true");
            Assert.AreEqual(Visibility.Visible, asViewModel.PdfArchitectVisibilty, "PdfArchitectVisibilty not initialized with visible");
        }

        [Test]
        public void LicenseTabVisibility_EditionIsNull_ReturnsCollapsed()
        {
            var asViewModel = BuildViewModel();
            Assert.AreEqual(Visibility.Visible, asViewModel.LicenseTabVisibility);
        }

        [Test]
        public void LicenseTabVisibility_ProductIsNotNull_ReturnsVisible()
        {
            var asViewModel = BuildViewModel();
            Assert.AreEqual(Visibility.Visible, asViewModel.LicenseTabVisibility);
        }

        [Test]
        public void SetGpoSettings_DebugTabIsEnabledIsNegationOfDisableTitleTab()
        {
            var asViewModel = BuildViewModel();
            var gpoSettings = new TestGpoSettings
            {
                DisableTitleTab = true
            };

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.IsFalse(asViewModel.TitleTabIsEnabled, "TitleTabIsEnabled is not the negation of DisableTitleTab");

            gpoSettings.DisableTitleTab = false;

            interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(asViewModel.TitleTabIsEnabled, "TitleTabIsEnabled is not the negation of DisableTitleTab");
        }

        [Test]
        public void SetGpoSettings_PdfArchitectVisibilityIsCollapsedForHidePdfArchitectInfoElseVisible()
        {
            var asViewModel = BuildViewModel();
            var gpoSettings = new TestGpoSettings
            {
                HidePdfArchitectInfo = false
            };

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.AreEqual(Visibility.Visible, asViewModel.PdfArchitectVisibilty, "PdfArchitectVisibilty not visible without DisablePdfArchitectInfo");

            asViewModel = BuildViewModel();
            gpoSettings = new TestGpoSettings
            {
                HidePdfArchitectInfo = true
            };

            interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.AreEqual(Visibility.Collapsed, asViewModel.PdfArchitectVisibilty, "PdfArchitectVisibilty not collapsed for DisablePdfArchitectInfo");
        }

        [Test]
        public void SetGpoSettings_PrinterTabIsEnabledIsNegationOfDisablePrinterTab()
        {
            var asViewModel = BuildViewModel();
            var gpoSettings = new TestGpoSettings
            {
                DisablePrinterTab = true
            };

            var interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.IsFalse(asViewModel.PrinterTabIsEnabled, "PrinterTabIsEnabled is not the negation of DisablePrinterTab");

            gpoSettings.DisablePrinterTab = false;

            interaction = new ApplicationSettingsInteraction(new PdfCreatorSettings(null), gpoSettings);
            asViewModel.SetInteraction(interaction);

            Assert.IsTrue(asViewModel.PrinterTabIsEnabled, "PrinterTabIsEnabled is not the negation of DisablePrinterTab");
        }
    }

    class TestGpoSettings : IGpoSettings
    {
        public bool DisableApplicationSettings { get; set; }
        public bool DisableDebugTab { get; set; }
        public bool DisablePrinterTab { get; set; }
        public bool DisableProfileManagement { get; set; }
        public bool DisableTitleTab { get; set; }
        public bool HideLicenseTab { get; set; }
        public bool HidePdfArchitectInfo { get; set; }
        public string Language { get; set; }
        public string UpdateInterval { get; set; }
    }
}