using System.ComponentModel;
using System.Windows;
using NUnit.Framework;
using pdfforge.GpoReader;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.ViewModels;
using PDFCreator.UnitTest.ViewModels.Helper;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.ViewModels.UserControls
{
    [TestFixture]
    class ApplicationSettingsViewModelTest
    {
        [Test]
        public void EmptyViewModel_SettingApplicationSettings_RaisesPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var asViewModel = new ApplicationSettingsViewModel(null);
            asViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(asViewModel, "ApplicationSettings");

            asViewModel.ApplicationSettings = new ApplicationSettings();

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void ApplicationSettings_OnSet_TriggersOnSettingsChangeEvent()
        {
            var viewModel = new ApplicationSettingsViewModel(null);
            var settingsChanged = false;
            viewModel.SettingsChanged += (sender, args) => settingsChanged = true;

            viewModel.ApplicationSettings = new ApplicationSettings();

            Assert.IsTrue(settingsChanged);
        }

        [Test]
        public void EmptyViewModel_SettingGpoSettings_RaisesPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var asViewModel = new ApplicationSettingsViewModel(null);
            asViewModel.PropertyChanged += eventStub.OnEventRaised;
            var titleTabIsEnabledPropertyListener = new PropertyChangedListenerMock(asViewModel, "TitleTabIsEnabled");
            var debugTabIsEnabledPropertyListener = new PropertyChangedListenerMock(asViewModel, "DebugTabIsEnabled");
            var printerTabIsEnabledPropertyListener = new PropertyChangedListenerMock(asViewModel, "PrinterTabIsEnabled");
            var pdfArchitectVisibiltyPropertyListener = new PropertyChangedListenerMock(asViewModel, "PdfArchitectVisibilty");

            asViewModel.GpoSettings = new GpoSettings();

            Assert.IsTrue(titleTabIsEnabledPropertyListener.WasCalled, "TitleTabIsEnabled PropertyChanged was not called");
            Assert.IsTrue(debugTabIsEnabledPropertyListener.WasCalled, "DebugTabIsEnabled PropertyChanged was not called");
            Assert.IsTrue(printerTabIsEnabledPropertyListener.WasCalled, "PrinterTabIsEnabled PropertyChanged was not called");
            Assert.IsTrue(pdfArchitectVisibiltyPropertyListener.WasCalled, "PdfArchitectVisibilty PropertyChanged was not called");
        }

        [Test]
        public void GpoProperties_CheckInitializations()
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            asViewModel.GpoSettings = null;

            Assert.IsTrue(asViewModel.DebugTabIsEnabled, "DebugTabIsEnabled not initialized with true");
            Assert.IsTrue(asViewModel.PrinterTabIsEnabled, "PrinterTabIsEnabled not initialized with true");
            Assert.IsTrue(asViewModel.TitleTabIsEnabled, "TitleTabIsEnabled not initialized with true");
            Assert.AreEqual(Visibility.Visible, asViewModel.PdfArchitectVisibilty, "PdfArchitectVisibilty not initialized with visible");
        }

        [Test]
        public void SetGpoSettings_DebugTabIsEnabledIsNegationOfDisableDebugTab()
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            var gpoSettings = new GpoSettings();

            gpoSettings.DisableDebugTab = true;
            asViewModel.GpoSettings = gpoSettings;
            Assert.IsFalse(asViewModel.DebugTabIsEnabled, "DebugTabIsEnabled is not the negation of DisableDebugTab");

            gpoSettings.DisableDebugTab = false;
            asViewModel.GpoSettings = gpoSettings;
            Assert.IsTrue(asViewModel.DebugTabIsEnabled, "DebugTabIsEnabled is not the negation of DisableDebugTab");
        }

        [Test]
        public void SetGpoSettings_PrinterTabIsEnabledIsNegationOfDisablePrinterTab()
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            var gpoSettings = new GpoSettings();

            gpoSettings.DisablePrinterTab = true;
            asViewModel.GpoSettings = gpoSettings;
            Assert.IsFalse(asViewModel.PrinterTabIsEnabled, "PrinterTabIsEnabled is not the negation of DisablePrinterTab");

            gpoSettings.DisablePrinterTab = false;
            asViewModel.GpoSettings = gpoSettings;
            Assert.IsTrue(asViewModel.PrinterTabIsEnabled, "PrinterTabIsEnabled is not the negation of DisablePrinterTab");
        }

        [Test]
        public void SetGpoSettings_DebugTabIsEnabledIsNegationOfDisableTitleTab()
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            var gpoSettings = new GpoSettings();

            gpoSettings.DisableTitleTab = true;
            asViewModel.GpoSettings = gpoSettings;
            Assert.IsFalse(asViewModel.TitleTabIsEnabled, "TitleTabIsEnabled is not the negation of DisableTitleTab");

            gpoSettings.DisableTitleTab = false;
            asViewModel.GpoSettings = gpoSettings;
            Assert.IsTrue(asViewModel.TitleTabIsEnabled, "TitleTabIsEnabled is not the negation of DisableTitleTab");
        }

        [Test]
        public void SetGpoSettings_PdfArchitectVisibilityIsCollapsedForHidePdfArchitectInfoElseVisible()
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            var gpoSettings = new GpoSettings();
            gpoSettings.HidePdfArchitectInfo = false;
            asViewModel.GpoSettings = gpoSettings;
            Assert.AreEqual(Visibility.Visible, asViewModel.PdfArchitectVisibilty, "PdfArchitectVisibilty not visible without DisablePdfArchitectInfo");

            asViewModel = new ApplicationSettingsViewModel(null);
            gpoSettings = new GpoSettings();
            gpoSettings.HidePdfArchitectInfo = true;
            asViewModel.GpoSettings = gpoSettings;
            Assert.AreEqual(Visibility.Collapsed, asViewModel.PdfArchitectVisibilty, "PdfArchitectVisibilty not collapsed for DisablePdfArchitectInfo");
        }

        [Test]
        public void LicenseTabVisibility_EditionIsNull_ReturnsCollapsed() 
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            Assert.AreEqual(Visibility.Visible, asViewModel.LicenseTabVisibility);
        }

        [Test]
        public void LicenseTabVisibility_ProductIsNotNull_ReturnsVisible()
        {
            var asViewModel = new ApplicationSettingsViewModel(null);
            Assert.AreEqual(Visibility.Visible, asViewModel.LicenseTabVisibility);
        }
    }
}