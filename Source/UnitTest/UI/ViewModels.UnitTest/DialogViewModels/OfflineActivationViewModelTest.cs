using System.Collections.Generic;
using System.ComponentModel;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class OfflineActivationViewModelTest
    {
        [SetUp]
        public void SetUp()
        {
            _process = Substitute.For<IProcessStarter>();
            _userGuideHelper = Substitute.For<IUserGuideHelper>();
            var product = Product.PdfCreator;
            _activationHelper = Substitute.For<IActivationHelper>();
            _activationHelper.Activation.Returns(new Activation() { Product = product });
            _translator = Substitute.For<ITranslator>();
            _translator.GetTranslation("OfflineActivationViewModel", "InvalidLicenseKeySyntax")
                .Returns(InvalidLicenseKeySyntaxMessage);
            _viewModel = new OfflineActivationViewModel(_process, _userGuideHelper, _activationHelper, _translator);
            _viewModel.FinishInteraction = () => { _finishInteractionWasCalled = true; };
            _propertiesChangedList = new List<string>();

            _interaction = new OfflineActivationInteraction(ValidLicenseKey);
            _viewModel.SetInteraction(_interaction);
        }

        private IProcessStarter _process;
        private IUserGuideHelper _userGuideHelper;
        private ITranslator _translator;
        private OfflineActivationViewModel _viewModel;
        private OfflineActivationInteraction _interaction;
        private List<string> _propertiesChangedList;
        private bool _finishInteractionWasCalled;
        private IActivationHelper _activationHelper;

        private const string ValidLicenseKey = "SOMEL-ICENS-KEYTH-ATFIT-STHEP-ATTER";
        private const string InvalidLicenseKeySyntaxMessage = "Incorrect License Key Syntax Message";

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _propertiesChangedList.Add(propertyChangedEventArgs.PropertyName);
        }

        [Test]
        public void LicenseKey_IsValueFromInteraction()
        {
            Assert.AreEqual(ValidLicenseKey, _viewModel.LicenseKey);
        }

        [Test]
        public void LicenseKey_SetLicenseKeyValueIsStoredInInteraction_LicenseKeyAndOfflineActivationStringRaisePropertyChangedGetCalled()
        {
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;

            _viewModel.LicenseKey = "SomethingElse";

            Assert.AreEqual("SomethingElse", _interaction.LicenseKey);
            Assert.AreEqual(3, _propertiesChangedList.Count);
            Assert.Contains(nameof(_viewModel.LicenseKey), _propertiesChangedList);
            Assert.Contains(nameof(_viewModel.LicenseKeyIsValid), _propertiesChangedList);
            Assert.Contains(nameof(_viewModel.OfflineActivationString), _propertiesChangedList);
        }

        [Test]
        public void LicenseServerAnswer_IsValueFromInteraction()
        {
            _interaction.LicenseServerAnswer = "someLSA";
            _viewModel.SetInteraction(_interaction);

            Assert.AreEqual("someLSA", _viewModel.LicenseServerAnswer);
        }

        [Test]
        public void LicenseServerAnswer_LicenseServerAnswerValueIsStoredInInteraction_RaisePropertyChangedGetCalled()
        {
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;

            _viewModel.LicenseServerAnswer = "anotherLSA";

            Assert.AreEqual("anotherLSA", _interaction.LicenseServerAnswer);
            Assert.AreEqual(1, _propertiesChangedList.Count);
            Assert.Contains(nameof(_viewModel.LicenseServerAnswer), _propertiesChangedList);
        }

        [Test]
        public void OfflineActivationString_InvalidLicenseKey_OfflineActivationStringIsInvalidLicenseKeySyntaxString()
        {
            _interaction.LicenseKey = "InvalidLicenseKeyBecauseOfWrongSyntax";
            _viewModel.SetInteraction(_interaction);

            Assert.AreEqual(InvalidLicenseKeySyntaxMessage, _viewModel.OfflineActivationString);
            _activationHelper.DidNotReceiveWithAnyArgs().GetOfflineActivationString("");
        }

        [Test]
        public void OfflineActivationString_ValidLicenseKey_CallsLicenseCheckerGetOfflineActivationString()
        {
            _activationHelper.GetOfflineActivationString(ValidLicenseKey).Returns("OfflineActivationString");
            Assert.AreEqual("OfflineActivationString", _viewModel.OfflineActivationString);
        }

        [Test]
        public void OfflineActivationString_ValidLicenseKeyWithWhitespaces_CallsLicenseCheckerGetOfflineActivationStringWithTrimedKey()
        {
            _viewModel.LicenseKey = " " + ValidLicenseKey + " ";

            _activationHelper.GetOfflineActivationString(ValidLicenseKey).Returns("OfflineActivationString");
            Assert.AreEqual("OfflineActivationString", _viewModel.OfflineActivationString);
        }

        [Test]
        public void OkCommandCanExecute_IsFalseWhenLicenseServerAnswerIsEmpty()
        {
            _viewModel.LicenseServerAnswer = "";

            Assert.IsFalse(_viewModel.OkCommand.IsExecutable);
        }

        [Test]
        public void OkCommandCanExecute_IsTrueWhenLicenseServerAnswerIsSet()
        {
            _viewModel.LicenseServerAnswer = "someLSA";

            Assert.IsTrue(_viewModel.OkCommand.IsExecutable);
        }

        [Test]
        public void OkCommandExecute_InteractionSuccessIsTrue_FinischInteractionGetsCalled()
        {
            _interaction.Success = false;
            _viewModel.SetInteraction(_interaction);

            _viewModel.OkCommand.Execute(null);

            Assert.IsTrue(_interaction.Success, "Interaction.Success");
            Assert.IsTrue(_finishInteractionWasCalled, "FinishInteraction was not called");
        }

        [Test]
        public void OpenOfflineActivationUrlCommandExecute_IsExecutable_CallsProcessStartWithOfflineActivationUrl()
        {
            Assert.IsTrue(_viewModel.OpenOfflineActivationUrlCommand.IsExecutable);

            _viewModel.OpenOfflineActivationUrlCommand.Execute(null);
            _process.Received().Start(Urls.OfflineActivationUrl);
        }
    }
}