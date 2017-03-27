using System.Windows;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    public class StoreLicenseForAllUsersWindowViewModelTest
    {
        private const string ProductName = "ProductName";
        private StoreLicenseForAllUsersWindowViewModel _storeLicenseForAllUsersWindowViewModel;
        private InteractionHelper<StoreLicenseForAllUsersInteraction> _interactionHelper;
        private IOsHelper _osHelper;
        private IUacAssistant _uacAssistant;
        private IInteractionInvoker _interactionInvoker;


        [SetUp]
        public void Setup()
        {
            _osHelper = Substitute.For<IOsHelper>();
            _uacAssistant = Substitute.For<IUacAssistant>();
            var applicationNameProvider = new ApplicationNameProvider(ProductName);
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _storeLicenseForAllUsersWindowViewModel = new StoreLicenseForAllUsersWindowViewModel(applicationNameProvider, _osHelper, _uacAssistant, _interactionInvoker, new StoreLicenseForAllUsersWindowTranslation());
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
            Assert.AreEqual(ProductName, _storeLicenseForAllUsersWindowViewModel.ProductName);
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
            var title = "";
            var text = "";
            var options = MessageOptions.MoreInfoCancel;
            var icon = MessageIcon.None;
            var translation = new StoreLicenseForAllUsersWindowTranslation();
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(x =>
            {
                var interaction = x[0] as MessageInteraction;
                title = interaction.Title;
                text = interaction.Text;
                options = interaction.Buttons;
                icon = interaction.Icon;
            });

            _storeLicenseForAllUsersWindowViewModel.StoreLicenseInLmCommand.Execute(null);

            Assert.AreEqual(ProductName, title, "Title of message box is not the product name");
            Assert.AreEqual(translation.StoreForAllUsersSuccessful, text, "Wrong text in message box");
            Assert.AreEqual(MessageOptions.OK, options, "Wrong options in message box");
            Assert.AreEqual(icon, MessageIcon.PDFCreator, "Wrong MessageBoxIcon");
        }

        [Test]
        public void StoreLicenseInLmCommandExecute_StoreLicenseForAllUsersFailed_ShowMessageWindow()
        {
            _uacAssistant.StoreLicenseForAllUsers(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            var title = "";
            var text = "";
            var options = MessageOptions.MoreInfoCancel;
            var icon = MessageIcon.None;
            var translation = new StoreLicenseForAllUsersWindowTranslation();
            _interactionInvoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(x =>
            {
                var interaction = x[0] as MessageInteraction;
                title = interaction.Title;
                text = interaction.Text;
                options = interaction.Buttons;
                icon = interaction.Icon;
            });

            _storeLicenseForAllUsersWindowViewModel.StoreLicenseInLmCommand.Execute(null);

            Assert.AreEqual(ProductName, title, "Title of message box is not the product name");
            Assert.AreEqual(translation.StoreForAllUsersFailed, text, "Wrong text in message box");
            Assert.AreEqual(MessageOptions.OK, options, "Wrong options in message box");
            Assert.AreEqual(icon, MessageIcon.Error, "Wrong MessageBoxIcon");
        }
    }
}
