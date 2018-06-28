using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using Translatable;

namespace Presentation.UnitTest.UserControls.Dialogs
{
    [TestFixture]
    internal class MessageViewModelTest
    {
        private MessageViewModel _viewModel;
        private ISoundPlayer _soundPlayer;
        private ErrorCodeInterpreter _errorCodeInterpreter;
        private IClipboardService _clipboardService;

        [SetUp]
        public void Setup()
        {
            _soundPlayer = Substitute.For<ISoundPlayer>();
            _errorCodeInterpreter = new ErrorCodeInterpreter(new TranslationFactory());
            _clipboardService = Substitute.For<IClipboardService>();
            _viewModel = new MessageViewModel(new DesignTimeTranslationUpdater(), _soundPlayer, _errorCodeInterpreter, _clipboardService);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DesignTimeViewModel_IsNewable(bool withErros)
        {
            var dtvm = new DesignTimeMessageViewModel(withErros);
            Assert.IsNotNull(dtvm);
        }

        [Test]
        public void InitMessageInteraction_KeyIsSetForActionResult()
        {
            var actionResult = new ActionResult(ErrorCode.Attachment_NoPdf);
            var actionResultKey = "ActionResult-Key";

            var interaction = new MessageInteraction("", "", MessageOptions.OK, MessageIcon.Info, actionResultKey, actionResult);

            Assert.AreEqual(actionResultKey, interaction.ActionResultDict.Keys.First());
            Assert.AreEqual(actionResult, interaction.ActionResultDict[actionResultKey]);
        }

        [TestCase(MessageOptions.OK, MessageResponse.OK)]
        [TestCase(MessageOptions.MoreInfoCancel, MessageResponse.MoreInfo)]
        [TestCase(MessageOptions.OKCancel, MessageResponse.OK)]
        [TestCase(MessageOptions.RetryCancel, MessageResponse.Retry)]
        [TestCase(MessageOptions.YesNo, MessageResponse.Yes)]
        [TestCase(MessageOptions.YesNoCancel, MessageResponse.Yes)]
        [TestCase(MessageOptions.YesCancel, MessageResponse.Yes)]
        public void ExecuteButtonLeft_ForDifferentMessageOptions_SetsResponseAndCallsFinish(MessageOptions option, MessageResponse response)
        {
            var finishWasCalled = false;
            _viewModel.FinishInteraction = () => finishWasCalled = true;

            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);
            _viewModel.SetInteraction(interaction);
            _viewModel.LeftButtonCommand.Execute(null);

            Assert.AreEqual(response, _viewModel.Interaction.Response);
            Assert.IsTrue(finishWasCalled);
        }

        [TestCase(MessageOptions.YesNoCancel, MessageResponse.No)]
        public void ExecuteButtonMiddle_ForDifferentMessageOptions_SetsResponseAndCallsFinish(MessageOptions option, MessageResponse response)
        {
            var finishWasCalled = false;
            _viewModel.FinishInteraction = () => finishWasCalled = true;

            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);
            _viewModel.SetInteraction(interaction);
            _viewModel.MiddleButtonCommand.Execute(null);

            Assert.AreEqual(response, _viewModel.Interaction.Response);
            Assert.IsTrue(finishWasCalled);
        }

        [TestCase(MessageOptions.MoreInfoCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.OKCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.RetryCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.YesNo, MessageResponse.No)]
        [TestCase(MessageOptions.YesNoCancel, MessageResponse.Cancel)]
        [TestCase(MessageOptions.YesCancel, MessageResponse.Cancel)]
        public void ExecuteButtonRight_ForDifferentMessageOptions_SetsResponseAndCallsFinish(MessageOptions option, MessageResponse response)
        {
            var finishWasCalled = false;
            _viewModel.FinishInteraction = () => finishWasCalled = true;

            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);
            _viewModel.SetInteraction(interaction);
            _viewModel.RightButtonCommand.Execute(null);

            Assert.AreEqual(response, _viewModel.Interaction.Response);
            Assert.IsTrue(finishWasCalled);
        }

        [TestCase(MessageOptions.YesNoCancel, true)]
        [TestCase(MessageOptions.MoreInfoCancel, false)]
        [TestCase(MessageOptions.OK, false)]
        [TestCase(MessageOptions.OKCancel, false)]
        [TestCase(MessageOptions.RetryCancel, false)]
        [TestCase(MessageOptions.YesNo, false)]
        public void Check_ShowMiddleButton(MessageOptions option, bool isVisible)
        {
            var interaction = new MessageInteraction("message", "title", option, MessageIcon.Info);
            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(isVisible, _viewModel.MiddleButtonCommand.IsExecutable);
        }

        [TestCase(MessageOptions.MoreInfoCancel, "More information", null, "Cancel")]
        [TestCase(MessageOptions.OK, "OK", null, null)]
        [TestCase(MessageOptions.OKCancel, "OK", null, "Cancel")]
        [TestCase(MessageOptions.RetryCancel, "Retry", null, "Cancel")]
        [TestCase(MessageOptions.YesNo, "Yes", null, "No")]
        [TestCase(MessageOptions.YesNoCancel, "Yes", "No", "Cancel")]
        [TestCase(MessageOptions.YesCancel, "Yes", null, "Cancel")]
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

            Assert.AreEqual(rightActive, _viewModel.RightButtonCommand.IsExecutable);
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
        public void SetInteraction_RaisesPropertyChangedAndButtonsCanExecuteChanged()
        {
            var interaction = new MessageInteraction("message", "title", MessageOptions.OK, MessageIcon.Info);

            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);
            var middleButtonCanExecuteChanged = false;
            _viewModel.MiddleButtonCommand.CanExecuteChanged += (s, a) => middleButtonCanExecuteChanged = true;
            var rightButtonCanExecuteChanged = false;
            _viewModel.RightButtonCommand.CanExecuteChanged += (s, a) => rightButtonCanExecuteChanged = true;

            _viewModel.SetInteraction(interaction);

            Assert.IsTrue(middleButtonCanExecuteChanged, "Middle Button CanExecuteChanged");
            Assert.IsTrue(rightButtonCanExecuteChanged, "Right Button CanExecuteChanged");

            Assert.Contains(nameof(_viewModel.LeftButtonContent), propertyChangedList);
            Assert.Contains(nameof(_viewModel.RightButtonContent), propertyChangedList);
            Assert.Contains(nameof(_viewModel.MiddleButtonContent), propertyChangedList);
            Assert.Contains(nameof(_viewModel.IconSize), propertyChangedList);
        }

        [Test]
        public void SetInteractionWithActionResultDict_ErrorListVisibiltyIsVisible_ErrorListIsBuild_RaisesPropertyChanged()
        {
            var actionResultDict = new ActionResultDict();
            var excpetedError = _errorCodeInterpreter.GetErrorText(ErrorCode.Attachment_NoPdf, false);
            var oneErrorActionResult = new ActionResult(ErrorCode.Attachment_NoPdf);
            actionResultDict.Add("RegionWithOneError", oneErrorActionResult);
            var twoErrorsActionResult = new ActionResult(ErrorCode.Attachment_NoPdf);
            twoErrorsActionResult.Add(ErrorCode.Attachment_NoPdf);
            actionResultDict.Add("RegionWithTwoErrors", twoErrorsActionResult);
            var interaction = new MessageInteraction("", "", MessageOptions.OK, MessageIcon.Info, actionResultDict);

            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual("RegionWithOneError", _viewModel.ErrorList[0].Region);
            Assert.AreEqual(excpetedError, _viewModel.ErrorList[0].Error);
            Assert.AreEqual("RegionWithTwoErrors", _viewModel.ErrorList[1].Region);
            Assert.AreEqual(excpetedError, _viewModel.ErrorList[1].Error);
            Assert.AreEqual("RegionWithTwoErrors", _viewModel.ErrorList[2].Region);
            Assert.AreEqual(excpetedError, _viewModel.ErrorList[2].Error);

            Assert.AreEqual(Visibility.Visible, _viewModel.ErrorListVisibility);
            Assert.Contains(nameof(_viewModel.ErrorListVisibility), propertyChangedList);
        }

        [Test]
        public void SetInteractionWithoutActionResultDict_ErrorListVisibiltyIsCollapsed_RaisesPropertyChanged()
        {
            var interaction = new MessageInteraction("", "", MessageOptions.OK, MessageIcon.Info);
            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(Visibility.Collapsed, _viewModel.ErrorListVisibility);
            Assert.Contains(nameof(_viewModel.ErrorListVisibility), propertyChangedList);
        }

        [Test]
        public void SetInteractionWitValidActionResultDict_ErrorListVisibiltyIsCollapsed_RaisesPropertyChanged()
        {
            var validActionResultDict = new ActionResultDict();
            var interaction = new MessageInteraction("", "", MessageOptions.OK, MessageIcon.Info, validActionResultDict);
            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(Visibility.Collapsed, _viewModel.ErrorListVisibility);
            Assert.Contains(nameof(_viewModel.ErrorListVisibility), propertyChangedList);
        }

        [Test]
        public void SetInteractionWithSecondText_SeconTextVisibiltyIsVisible_RaisesPropertyChanged()
        {
            var interaction = new MessageInteraction("", "", MessageOptions.OK, MessageIcon.Info, new ActionResultDict(), "Second Text");
            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(Visibility.Visible, _viewModel.SecondTextVisibility);
            Assert.Contains(nameof(_viewModel.SecondTextVisibility), propertyChangedList);
        }

        [Test]
        public void SetInteractionWithoutSecondText_SecondTextVisibiltyIsCollapsed_RaisesPropertyChanged()
        {
            var interaction = new MessageInteraction("", "", MessageOptions.OK, MessageIcon.Info, new ActionResultDict());
            var propertyChangedList = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => propertyChangedList.Add(args.PropertyName);

            _viewModel.SetInteraction(interaction);

            Assert.AreEqual(Visibility.Collapsed, _viewModel.SecondTextVisibility);
            Assert.Contains(nameof(_viewModel.SecondTextVisibility), propertyChangedList);
        }

        [Test]
        public void CopyToClipBoard_CopyToClipboardisCalledWithCorrectText()
        {
            var actionResultDict = new ActionResultDict();
            var excpetedError = _errorCodeInterpreter.GetErrorText(ErrorCode.Attachment_NoPdf, false);
            var oneErrorActionResult = new ActionResult(ErrorCode.Attachment_NoPdf);
            actionResultDict.Add("RegionWithOneError", oneErrorActionResult);
            var twoErrorsActionResult = new ActionResult(ErrorCode.Attachment_NoPdf);
            twoErrorsActionResult.Add(ErrorCode.Attachment_NoPdf);
            actionResultDict.Add("RegionWithTwoErrors", twoErrorsActionResult);
            var interaction = new MessageInteraction("text", "title", MessageOptions.OK, MessageIcon.Info, actionResultDict, "second text");
            _viewModel.SetInteraction(interaction);

            var receivedText = "";
            _clipboardService.SetDataObject(Arg.Do<object>(o => receivedText = o as string));

            _viewModel.CopyToClipboard_CommandBinding(null, null);

            var expectedText = new StringBuilder()
                .AppendLine("text")
                .AppendLine("RegionWithOneError")
                .AppendLine("- " + excpetedError)
                .AppendLine("RegionWithTwoErrors")
                .AppendLine("- " + excpetedError)
                .AppendLine("- " + excpetedError)
                .AppendLine("second text")
                .ToString();

            Assert.AreEqual(receivedText, expectedText);
        }
    }
}
