using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System;
using System.Collections.Generic;
using SystemInterface.IO;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class DeleteHistoricFilesCommandTest
    {
        private IWaitableCommand _deleteHistoricFilesCommand;
        private IFile _file;
        private UnitTestInteractionRequest _unitTestInteractionRequest;
        private DeleteFilesTranslation _translation;
        private IList<HistoricFile> _historicFiles;
        private const string File1 = @"Path\File1.pdf";
        private const string File2 = @"Path\File2.pdf";

        [SetUp]
        public void SetUp()
        {
            _file = Substitute.For<IFile>();
            _file.Exists(Arg.Any<string>()).Returns(true);

            _unitTestInteractionRequest = new UnitTestInteractionRequest();
            var translationUpdater = new DesignTimeTranslationUpdater();

            _deleteHistoricFilesCommand = new DeleteHistoricFilesCommand(_file, _unitTestInteractionRequest, translationUpdater);

            _translation = new DeleteFilesTranslation();

            _historicFiles = new List<HistoricFile> { new HistoricFile(File1, "", "", ""), new HistoricFile(File2, "", "", "") };
        }

        [Test]
        public void CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_deleteHistoricFilesCommand.CanExecute(null));
        }

        [Test]
        public void Execute_ParameterIsNoHistoricFileList_DoesNotThrowException_CallsDoneWithError()
        {
            MacroCommandIsDoneEventArgs calledArgs = null;
            _deleteHistoricFilesCommand.IsDone += (sender, args) => calledArgs = args;

            Assert.DoesNotThrow(() => _deleteHistoricFilesCommand.Execute(null));

            Assert.AreEqual(ResponseStatus.Error, calledArgs.ResponseStatus);
            _unitTestInteractionRequest.AssertWasNotRaised<IInteraction>();
        }

        [Test]
        public void Execute_NotifyUserAboutDeletion()
        {
            _deleteHistoricFilesCommand.Execute(_historicFiles);

            var interaction = _unitTestInteractionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.GetDeleteFilesTitle(_historicFiles.Count), interaction.Title, "Interaction Title");
            var expectedMessage = _translation.GetAreYouSureYouWantToDeleteFilesMessage(_historicFiles.Count)
                                  + "\r\n" + File1 + "\r\n" + File2;
            Assert.AreEqual(expectedMessage, interaction.Text, "Interaction Text");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons);
            Assert.AreEqual(MessageIcon.Question, interaction.Icon);
        }

        [Test]
        public void Execute_UserCancelsDeletion_IsDoneIsCalledWithCancel()
        {
            _unitTestInteractionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            MacroCommandIsDoneEventArgs calledArgs = null;
            _deleteHistoricFilesCommand.IsDone += (sender, args) => calledArgs = args;

            _deleteHistoricFilesCommand.Execute(_historicFiles);

            Assert.AreEqual(ResponseStatus.Cancel, calledArgs.ResponseStatus);
        }

        [Test]
        public void Execute_UserProceedsDeletion_AllFilesGetDeleted_IsDoneIsCalledWithSuccess()
        {
            _unitTestInteractionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            MacroCommandIsDoneEventArgs calledArgs = null;
            _deleteHistoricFilesCommand.IsDone += (sender, args) => calledArgs = args;

            _deleteHistoricFilesCommand.Execute(_historicFiles);

            _file.Received().Exists(File1);
            _file.Received().Delete(File1);
            _file.Received().Exists(File2);
            _file.Received().Delete(File2);
            Assert.AreEqual(ResponseStatus.Success, calledArgs.ResponseStatus);
        }

        [Test]
        public void Execute_UserProceedsDeletion_AllFilesDoNotExist_FileDoNotGeDeleted_NoFurtherUserNofication_IsDoneIsCalledWithSuccess()
        {
            var count = 0;
            _unitTestInteractionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Yes;
                count++;
            });
            MacroCommandIsDoneEventArgs calledArgs = null;
            _deleteHistoricFilesCommand.IsDone += (sender, args) => calledArgs = args;
            _file.Exists(Arg.Any<string>()).Returns(false);

            _deleteHistoricFilesCommand.Execute(_historicFiles);

            Assert.AreEqual(1, count, "InteractionRequest was raised more than once");
            _file.DidNotReceive().Delete(Arg.Any<string>());
            Assert.AreEqual(ResponseStatus.Success, calledArgs.ResponseStatus);
        }

        [Test]
        public void Execute_UserProceedsDelition_FilesCantBeDeleted_NotifyUser_IsDoneIsCalledWithSuccess()
        {
            MessageInteraction secondMessageInteraction = null;
            var count = 0;
            _unitTestInteractionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                count++;
                i.Response = MessageResponse.Yes; //proceed deletion
                if (count == 2)
                    secondMessageInteraction = i;
            });

            MacroCommandIsDoneEventArgs calledArgs = null;
            _deleteHistoricFilesCommand.IsDone += (sender, args) => calledArgs = args;
            _file.When(f => f.Delete(Arg.Any<string>())).Throw<Exception>();

            _deleteHistoricFilesCommand.Execute(_historicFiles);

            Assert.NotNull(secondMessageInteraction, "Request did not raise Second MessageInteraction");

            Assert.AreEqual(_translation.ErrorDuringDeletionTitle, secondMessageInteraction.Title, "Interaction Title");
            var expectedMessage = _translation.GetCouldNotDeleteTheFollowingFilesMessage(_historicFiles.Count)
                                  + "\r\n" + File1 + "\r\n" + File2;
            Assert.AreEqual(expectedMessage, secondMessageInteraction.Text, "Interaction Text");
            Assert.AreEqual(MessageOptions.OK, secondMessageInteraction.Buttons);
            Assert.AreEqual(MessageIcon.Error, secondMessageInteraction.Icon);

            Assert.AreEqual(ResponseStatus.Success, calledArgs.ResponseStatus);
        }
    }
}
