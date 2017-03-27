using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    [TestFixture]
    internal class PrinterTabViewModelTest
    {
        [SetUp]
        public void SetUp()
        {
            _translationHelper = new TranslationHelper(new DefaultSettingsProvider(), new AssemblyHelper(), new TranslationFactory());
            _translationHelper.InitEmptyTranslator();

            _printerActionAssistant = Substitute.For<IPrinterActionsAssistant>();

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

            _applicationSettings = new Conversion.Settings.ApplicationSettings();
            _applicationSettings.PrinterMappings = new[]
            {
                new PrinterMapping(ExistingPrinter, _profile1.Guid)
                , new PrinterMapping(UnknownPrinter, _profile2.Guid)
            }.ToList();

            _printerProvider = Substitute.For<IPrinterProvider>();
        }

        private const string ExistingPrinter = "PDFCreator";
        private const string UnknownPrinter = "UnknownPrinter";
        private Conversion.Settings.ApplicationSettings _applicationSettings;
        private ConversionProfile _profile1;
        private ConversionProfile _profile2;
        private TranslationHelper _translationHelper;
        private IPrinterProvider _printerProvider;
        private IPrinterActionsAssistant _printerActionAssistant;

        private PrinterTabViewModel BuildViewModel(IList<ConversionProfile> profiles = null)
        {
            var settings = new PdfCreatorSettings(null);
            settings.ApplicationSettings = _applicationSettings;
            settings.ConversionProfiles.Add(new ConversionProfile());

            var printerHelper = Substitute.For<IPrinterHelper>();
            printerHelper.GetApplicablePDFCreatorPrinter(Arg.Any<string>(), Arg.Any<string>()).Returns("PDFCreator");

            if (profiles != null)
                settings.ConversionProfiles = profiles;

            var viewModel = new PrinterTabViewModel(_printerProvider, _printerActionAssistant, null, null, printerHelper, new PrinterTabTranslation());

            viewModel.SetSettingsAndRaiseNotifications(settings, null);

            return viewModel;
        }

        [Test]
        public void AddPrinterCommand_Execute_PrinterMappingContainsPrinterWithGivenName_DefaultProfileGuidIsAssigned()
        {
            var conversionProfiles = new[]
            {
                new ConversionProfile
                {
                    Guid = ProfileGuids.DEFAULT_PROFILE_GUID
                }
            };

            _applicationSettings = new Conversion.Settings.ApplicationSettings();

            var newPrinterName = "New printer for testing";
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {newPrinterName});
            var printerTabViewModel = BuildViewModel(conversionProfiles);

            string something;
            _printerActionAssistant.AddPrinter(out something).Returns(x =>
            {
                x[0] = newPrinterName;
                return true;
            });

            printerTabViewModel.AddPrinterCommand.Execute(null);

            Assert.AreEqual(1, printerTabViewModel.PrinterMappings.Count, "Added more or less than one printer");
            Assert.AreEqual(newPrinterName, printerTabViewModel.PrinterMappings[0].PrinterMapping.PrinterName,
                "Wrong printername");
            Assert.AreEqual(ProfileGuids.DEFAULT_PROFILE_GUID,
                printerTabViewModel.PrinterMappings[0].PrinterMapping.ProfileGuid,
                "Did not assign the default profile GUID.");
        }

        [Test]
        public void DeletePrinterCommand_Execute_DeletePrinterActionGetsCalled()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {ExistingPrinter, "Something"});
            var printerTabViewModel = BuildViewModel();

            printerTabViewModel.DeletePrinterCommand.Execute(null);

            _printerActionAssistant.Received().DeletePrinter(ExistingPrinter, 2);
        }

        [Test]
        public void DeletePrinterCommand_WithNonexistingPrinter_NotExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {ExistingPrinter, "Something"});
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToLast();

            Assert.IsFalse(printerTabViewModel.DeletePrinterCommand.CanExecute(null));
        }

        [Test]
        public void DeletePrinterCommand_WithValidPrinter_IsExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {ExistingPrinter, "Something"});
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToFirst();

            Assert.IsTrue(printerTabViewModel.DeletePrinterCommand.CanExecute(null));
        }

        [Test]
        public void EmptyViewModel_SettingPdfCreatorPrinters_RaisesPdfCreatorPrintersPropertyChanged()
        {
            var eventStub = Substitute.For<IEventHandler<PropertyChangedEventArgs>>();
            var printerTabViewModel = BuildViewModel();
            printerTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(printerTabViewModel, "PdfCreatorPrinters");

            printerTabViewModel.PdfCreatorPrinters = new List<string>();

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void PrimaryPrinter_SetPrimaryPrinter_PrimaryPrinterIsSetInEveryPrinterMappingWrapper()
        {
            var printerTabViewModel = BuildViewModel();
            var printerName = "some funny printer name (still laughing)";
            printerTabViewModel.PrimaryPrinter = printerName;

            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[0].PrimaryPrinter,
                "Wrong primary printer in first PrinterMapping");
            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[1].PrimaryPrinter,
                "Wrong primary printer in second PrinterMapping");
        }

        [Test]
        public void PrimaryPrinter_SetPrimaryPrinter_RaisesPrimaryPrinterPropertyChanged()
        {
            var printerTabViewModel = BuildViewModel();

            var propertyListener = new PropertyChangedListenerMock(printerTabViewModel, "PrimaryPrinter");

            printerTabViewModel.PrimaryPrinter = "something";

            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void RenamePrinterCommand_Execute_DeletePrinterActionGetsCalled()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {ExistingPrinter, "Something"});
            var printerTabViewModel = BuildViewModel();

            printerTabViewModel.RenamePrinterCommand.Execute(null);

            string something;
            _printerActionAssistant.Received().RenamePrinter(ExistingPrinter, out something);
        }

        [Test]
        public void RenamePrinterCommand_WithNonexistingPrinter_NotExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {ExistingPrinter, "Something"});
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToLast();

            Assert.IsFalse(printerTabViewModel.RenamePrinterCommand.CanExecute(null));
        }

        [Test]
        public void RenamePrinterCommand_WithValidPrinter_IsExecutable()
        {
            _printerProvider.GetPDFCreatorPrinters().Returns(new[] {ExistingPrinter, "Something"});
            var printerTabViewModel = BuildViewModel();

            var view = CollectionViewSource.GetDefaultView(printerTabViewModel.PrinterMappings);
            view.MoveCurrentToFirst();

            Assert.IsTrue(printerTabViewModel.RenamePrinterCommand.CanExecute(null));
        }

        [Test]
        public void UpdatePrimaryPrinter_GivenNameIsPrimaryPrinterInEveryPrinterMappingWrapper()
        {
            var printerTabViewModel = BuildViewModel();
            const string printerName = "Some funny PrinterName for testing";

            printerTabViewModel.UpdatePrimaryPrinter(printerName);

            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[0].PrimaryPrinter,
                "Wrong primary printer in first PrinterMapping");
            Assert.AreEqual(printerName, printerTabViewModel.PrinterMappings[1].PrimaryPrinter,
                "Wrong primary printer in second PrinterMapping");
        }
    }
}