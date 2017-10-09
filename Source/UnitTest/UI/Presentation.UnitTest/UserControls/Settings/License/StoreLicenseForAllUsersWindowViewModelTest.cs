using System.Windows;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;

namespace Presentation.UnitTest.UserControls.Settings.License
{
    [TestFixture]
    public class StoreLicenseForAllUsersWindowViewModelTest
    {
        private const string EditionName = "EditionName";
        private const string ApplicationNameWithEdition = "PDFCreator " + EditionName;
        private StoreLicenseForAllUsersWindowViewModel _storeLicenseForAllUsersWindowViewModel;
        private InteractionHelper<StoreLicenseForAllUsersInteraction> _interactionHelper;
        private IOsHelper _osHelper;
        private IUacAssistant _uacAssistant;
        private UnitTestInteractionRequest _interactionRequest;

        [SetUp]
        public void Setup()
        {
            _osHelper = Substitute.For<IOsHelper>();
            _uacAssistant = Substitute.For<IUacAssistant>();
            var applicationNameProvider = new ApplicationNameProvider(EditionName);
            _interactionRequest = new UnitTestInteractionRequest();
            _storeLicenseForAllUsersWindowViewModel = new StoreLicenseForAllUsersWindowViewModel(applicationNameProvider, _osHelper, _uacAssistant, _interactionRequest, new DesignTimeTranslationUpdater());
            var interaction = new StoreLicenseForAllUsersInteraction("", "");
            _interactionHelper = new InteractionHelper<StoreLicenseForAllUsersInteraction>(_storeLicenseForAllUsersWindowViewModel, interaction);
        }

        [Test]
        public void RequiresUacVisibility_UserIsAdmin_ReturnsCollapsed()
        {
            _osHelper.UserIsAdministrator().Returns(true);

            Assert.AreEqual(Visibility.Collapsed, _storeLicenseForAllUsersWindowViewModel.RequiresUacVisibility);
        }

        [Test]
        public void RequiresUacVisibility_UserIsNotAdmin_ReturnsVisible()
        {
            _osHelper.UserIsAdministrator().Returns(false);

            Assert.AreEqual(Visibility.Visible, _storeLicenseForAllUsersWindowViewModel.RequiresUacVisibility);
        }

        [Test]
        public void ProductName_IsApplicationNameProviderProductName()
        {
            Assert.AreEqual(ApplicationNameWithEdition, _storeLicenseForAllUsersWindowViewModel.ProductName);
        }

        [Test]
        public void StoreLicenseInLmCommandExecute_CallsStoreLicesenForAllUsersAndFinishInteraction()
        {
            _storeLicenseForAllUsersWindowViewModel.StoreLicenseInLmCommand.Execute(null);

            _uacAssistant.Received(1).StoreLicenseForAllUsers(Arg.Any<string>(), Arg.Any<string>());
            Assert.IsTrue(_interactionHelper.InteractionIsFinished);
        }

        [Test]
        public void StoreLicenseInLmCommandExecute_StoreLicenseForAllUsersWasSuccessful__ShowMessageWindow()
        {
            _uacAssistant.StoreLicenseForAllUsers(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            var translation = new StoreLicenseForAllUsersWindowTranslation();

            _storeLicenseForAllUsersWindowViewModel.StoreLicenseInLmCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(ApplicationNameWithEdition.ToUpper(), interaction.Title, "Title of message box is not the product name");
            Assert.AreEqual(translation.StoreForAllUsersSuccessful, interaction.Text, "Wrong text in message box");
            Assert.AreEqual(MessageOptions.OK, interaction.Buttons, "Wrong options in message box");
            Assert.AreEqual(MessageIcon.PDFCreator, interaction.Icon, "Wrong MessageBoxIcon");
        }

        [Test]
        public void StoreLicenseInLmCommandExecute_StoreLicenseForAllUsersFailed_ShowMessageWindow()
        {
            _uacAssistant.StoreLicenseForAllUsers(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            var translation = new StoreLicenseForAllUsersWindowTranslation();

            _storeLicenseForAllUsersWindowViewModel.StoreLicenseInLmCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(ApplicationNameWithEdition.ToUpper(), interaction.Title, "Title of message box is not the product name");
            Assert.AreEqual(translation.StoreForAllUsersFailed, interaction.Text, "Wrong text in message box");
            Assert.AreEqual(MessageOptions.OK, interaction.Buttons, "Wrong options in message box");
            Assert.AreEqual(MessageIcon.Error, interaction.Icon, "Wrong MessageBoxIcon");
        }
    }
}
