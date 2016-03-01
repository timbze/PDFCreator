using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.ViewModels.UserControls;
using pdfforge.PDFCreator.ViewModels.Wrapper;
using PDFCreator.UnitTest.ViewModels.Helper;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.ViewModels.UserControls
{
    [TestFixture]
    internal class PrinterTabViewModelTest
    {
        private const string ExistingPrinter = "PDFCreator";
        private const string UnknownPrinter = "UnknownPrinter";
        private ApplicationSettings _applicationSettings;
        private ConversionProfile _profile1;
        private ConversionProfile _profile2;
        private TranslationHelper _translationHelper;

        [SetUp]
        public void SetUp()
        {
            _translationHelper = new TranslationHelper();
            _translationHelper.InitEmptyTranslator();
            
            _profile1 = new ConversionProfile
            {
                Name = "Profile1",
                Guid = "Profile1Guid"
            };

            _profile2 = new ConversionProfile
            {
                Name = "Profile1",
                Guid = "Profile1Guid"
            };

            _applicationSettings = new ApplicationSettings();
            _applicationSettings.PrinterMappings = new[] {new PrinterMapping(ExistingPrinter, _profile1.Guid)
                                                        , new PrinterMapping(UnknownPrinter, _profile2.Guid)};
        }

        [Test]
        public void EmptyViewModel_SettingPdfCreatorPrinters_RaisesPdfCreatorPrintersPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new ConversionProfile[] { },
                () => new string[] { }, _translationHelper, null);
            printerTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(printerTabViewModel, "PdfCreatorPrinters");

            printerTabViewModel.PdfCreatorPrinters = new List<string>();

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void RenamePrinterCommand_WithNonexistingPrinter_NotExecutable()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new List<ConversionProfile>(),
                () => new[] {ExistingPrinter, "Something"}, _translationHelper, null);

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToLast();

            Assert.IsFalse(printerTabViewModel.RenamePrinterCommand.CanExecute(null));
        }

        [Test]
        public void RenamePrinterCommand_WithValidPrinter_IsExecutable()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new List<ConversionProfile>(),
                () => new[] {ExistingPrinter, "Something"}, _translationHelper, null);

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToFirst();

            Assert.IsTrue(printerTabViewModel.RenamePrinterCommand.CanExecute(null));
        }

        [Test]
        public void RenamePrinterCommand_Execute_DeletePrinterActionGetsCalled()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new ConversionProfile[] { },
                () => new[] { ExistingPrinter, "Something" }, _translationHelper, null);
            var wasCalled = false;
            printerTabViewModel.RenamePrinterAction = (PrinterMappingWrapper printerMapping) => { wasCalled = true; };

            printerTabViewModel.RenamePrinterCommand.Execute(null);

            Assert.IsTrue(wasCalled, "RenameAction was not called.");
        }

        [Test]
        public void DeletePrinterCommand_WithNonexistingPrinter_NotExecutable()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new List<ConversionProfile>(),
                () => new[] {ExistingPrinter, "Something"}, _translationHelper, null);

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToLast();

            Assert.IsFalse(printerTabViewModel.DeletePrinterCommand.CanExecute(null));
        }

        [Test]
        public void DeletePrinterCommand_WithValidPrinter_IsExecutable()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new List<ConversionProfile>(),
                () => new[] {ExistingPrinter, "Something"}, _translationHelper, null);

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToFirst();

            Assert.IsTrue(printerTabViewModel.DeletePrinterCommand.CanExecute(null));
        }

        [Test]
        public void DeletePrinterCommand_Execute_DeletePrinterActionGetsCalled()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new ConversionProfile[] { },
                () => new[] { ExistingPrinter }, _translationHelper, null);
            var wasCalled = false;
            printerTabViewModel.DeletePrinterAction = (PrinterMappingWrapper printerMapping) => { wasCalled = true; };

            printerTabViewModel.DeletePrinterCommand.Execute(null);

            Assert.IsTrue(wasCalled, "DeleteAction was not called.");
        }

        [Test]
        public void AddPrinterCommand_Execute_PrinterMappingContainsPrinterWithGivenName_DefaultProfileGuidIsAssigned()
        {
            var conversionProfiles = new[]
            {
                new ConversionProfile()
                {
                    Guid = ProfileGuids.DEFAULT_PROFILE_GUID
                }
            };

            var newPrinterName = "New printer for testing";
            var printerTabViewModel = new PrinterTabViewModel(new ApplicationSettings(), conversionProfiles,
                () => new[] { newPrinterName }, _translationHelper, null);
            printerTabViewModel.AddPrinterAction = () => newPrinterName;

            printerTabViewModel.AddPrinterCommand.Execute(null);

            Assert.AreEqual(1, printerTabViewModel.PrinterMappings.Count, "Added more or less than one printer");
            Assert.AreEqual(newPrinterName, printerTabViewModel.PrinterMappings[0].PrinterMapping.PrinterName, "Wrong printername");
            Assert.AreEqual(ProfileGuids.DEFAULT_PROFILE_GUID, printerTabViewModel.PrinterMappings[0].PrinterMapping.ProfileGuid, "Did not assign the default profile GUID.");
        }

        [Test]
        public void UpdatePrimaryPrinter_GivenNameIsPrimaryPrinterInEveryPrinterMappingWrapper()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new ConversionProfile[] { }, () => new string[] { }, _translationHelper, null);
            const string printerName = "Some funny PrinterName for testing";
            
            printerTabViewModel.UpdatePrimaryPrinter(printerName);

            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[0].PrimaryPrinter, "Wrong primary printer in first PrinterMapping");
            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[1].PrimaryPrinter, "Wrong primary printer in second PrinterMapping");
        }

        [Test]
        public void PrimaryPrinter_AppSettingsAreNull_ReturnsNull()
        {
            var printerTabViewModel = new PrinterTabViewModel(null, new ConversionProfile[] { }, () => new string[] { }, _translationHelper, null);

            Assert.IsNull(printerTabViewModel.PrimaryPrinter);
        }

        [Test]
        public void PrimaryPrinter_SetPrimaryPrinter_PrimaryPrinterIsSetInEveryPrinterMappingWrapper()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new ConversionProfile[] { },
                () => new string[] { }, _translationHelper, null);
            var printerName = "some funny printer name (still laughing)";
            printerTabViewModel.PrimaryPrinter = printerName;

            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[0].PrimaryPrinter, "Wrong primary printer in first PrinterMapping");
            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[1].PrimaryPrinter, "Wrong primary printer in second PrinterMapping");
        }

        [Test]
        public void PrimaryPrinter_SetPrimaryPrinter_RaisesPrimaryPrinterPropertyChanged()
        {
            var printerTabViewModel = new PrinterTabViewModel(_applicationSettings, new ConversionProfile[] { },
                () => new string[] { }, _translationHelper, null);
            var propertyListener = new PropertyChangedListenerMock(printerTabViewModel, "PrimaryPrinter");

            printerTabViewModel.PrimaryPrinter = "something";

            Assert.IsTrue(propertyListener.WasCalled);
        }
    }
}