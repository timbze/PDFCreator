using System.Collections.Generic;
using System.Media;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    internal class MessageWindowViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _translator = Substitute.For<ITranslator>();
            _translator.GetTranslation(Arg.Any<string>(), Arg.Any<string>()).Returns(info => info.ArgAt<string>(1));
            _soundPlayer = Substitute.For<ISoundPlayer>();
            _viewModel = new MessageWindowViewModel(_translator, _soundPlayer);
        }

        private MessageWindowViewModel _viewModel;
        private ITranslator _translator;
        private ISoundPlayer _soundPlayer;

        [TestCase(MessageOptions.OK, MessageResponse.OK)]
        [TestCase(MessageOptions.MoreInfoCancel, MessageResponse.MoreInfo)]
        [TestCase(MessageOptions.OKCancel, MessageResponse.OK)]
        [TestCase(MessageOptions.RetryCancel, MessageResponse.Retry)]
        [TestCase(MessageOptions.YesNo, MessageResponse.Yes)]
        public void ExecuteButtonLeft_ForDifferentMessageOptions_SetsResponseAndCallsFinish(MessageOptions option, MessageResponse response)
        {
            var finishWasCalled = false;
            _viewModel.FinishInteraction = () => finishWasCalled = true;

            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);
            _viewModel.SetInteraction(interaction);
            _viewModel.ButtonLeftCommand.Execute(null);

            Assert.AreEqual(response, _viewModel.Interaction.Response);
            Assert.IsTrue(finishWasCalled);
        }

        [TestCase(MessageOptions.MoreInfoCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.OKCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.RetryCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.YesNo, MessageResponse.No)]
        public void ExecuteButtonRight_ForDifferentMessageOptions_SetsResponseAndCallsFinish(MessageOptions option, MessageResponse response)
        {
            var finishWasCalled = false;
            _viewModel.FinishInteraction = () => finishWasCalled = true;

            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);
            _viewModel.SetInteraction(interaction);
            _viewModel.ButtonRightCommand.Execute(null);

            Assert.AreEqual(response, _viewModel.Interaction.Response);
            Assert.IsTrue(finishWasCalled);
        }

        [TestCase(MessageOptions.MoreInfoCancel, "MoreInfo", null, "Cancel")]
        [TestCase(MessageOptions.OK, "Ok", null, null)]
        [TestCase(MessageOptions.OKCancel, "Ok", null, "Cancel")]
        [TestCase(MessageOptions.RetryCancel, "Retry", null, "Cancel")]
        [TestCase(MessageOptions.YesNo, "Yes", null, "No")]
        public void ButtonContentsAreSetAccordingToInteraction(MessageOptions option, string left, string middle, string right)
        {
            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(left, _viewModel.LeftButtonContent);
            Assert.AreEqual(middle, _viewModel.MiddleButtonContent);
            Assert.AreEqual(right, _viewModel.RightButtonContent);
        }

        [TestCase(MessageOptions.OK, false, false)]
        [TestCase(MessageOptions.MoreInfoCancel, false, true)]
        [TestCase(MessageOptions.OKCancel, false, true)]
        [TestCase(MessageOptions.RetryCancel, false, true)]
        [TestCase(MessageOptions.YesNo, false, true)]
        public void CorrectButtonsAreActivatedForMessageOptions(MessageOptions option, bool middleActive, bool rightActive)
        {
            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(rightActive, _viewModel.ButtonRightCommand.IsExecutable);
        }

        [TestCase(MessageIcon.PDFCreator)]
        [TestCase(MessageIcon.PDFForge)]
        public void ForLogoIcons_SetsCorrectIconSize(MessageIcon icon)
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, icon);

            _viewModel.SetInteraction(interaction);

            _soundPlayer.DidNotReceive().Play(Arg.Any<SystemSound>());
            Assert.AreEqual(45, _viewModel.IconSize);
        }

        [Test]
        public void ForErrorIcon_PlaysCorrectSound()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Error);

            _viewModel.SetInteraction(interaction);

            _soundPlayer.Received().Play(SystemSounds.Hand);
        }

        [Test]
        public void ForExclamantionIcon_PlaysCorrectSound()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Exclamation);

            _viewModel.SetInteraction(interaction);

            _soundPlayer.Received().Play(SystemSounds.Exclamation);
        }

        [Test]
        public void ForInfoIcon_PlaysCorrectSound()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Info);

            _viewModel.SetInteraction(interaction);

            _soundPlayer.Received().Play(SystemSounds.Asterisk);
        }

        [Test]
        public void ForQuestionIcon_PlaysCorrectSound()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Question);

            _viewModel.SetInteraction(interaction);

            _soundPlayer.Received().Play(SystemSounds.Question);
        }

        [Test]
        public void ForWarningIcon_PlaysCorrectSound()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Warning);

            _viewModel.SetInteraction(interaction);

            _soundPlayer.Received().Play(SystemSounds.Exclamation);
        }

        [Test]
        public void SettingInteractionRaisesPropertyChanged()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Info);

            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);
            _viewModel.SetInteraction(interaction);

            Assert.IsTrue(propertyChangedList.Contains(nameof(_viewModel.LeftButtonContent)));
            Assert.IsTrue(propertyChangedList.Contains(nameof(_viewModel.RightButtonContent)));
            Assert.IsTrue(propertyChangedList.Contains(nameof(_viewModel.MiddleButtonContent)));

            Assert.IsTrue(propertyChangedList.Contains(nameof(_viewModel.Icon)));
            Assert.IsTrue(propertyChangedList.Contains(nameof(_viewModel.IconSize)));
        }
    }
}