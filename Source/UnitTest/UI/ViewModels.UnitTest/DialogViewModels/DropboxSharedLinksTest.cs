using System;
using System.Collections.Generic;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class DropboxSharedLinksTest
    {
        [SetUp]
        public void Setup()
        {
            var processStarter = NSubstitute.Substitute.For<IProcessStarter>();
            _viewModel = new DropboxSharedLinksViewModel(new DropboxSharedLinksWindowTranslation(), processStarter);
            _interaction = new DropboxSharedLinksInteraction(new DropboxFileMetaData());
            _viewModel.SetInteraction(_interaction);

        }

        private DropboxSharedLinksViewModel _viewModel;
        private DropboxSharedLinksInteraction _interaction;
        private string redirectUri = "https://local";


        [Test]
        public void OkCommand_WhenExecuted_FinishInteraction()
        {
            var wascalled = false;
            _viewModel.FinishInteraction = () => wascalled = true;
            _viewModel.OkCommand.Execute(null);
            Assert.IsTrue(wascalled);
        }

        [Test]
        [STAThread]
        public void CopyCommand_WhenExecuted_CopySuccessfullTrue()
        {
            _viewModel.CopyCommand.Execute(null);
            Assert.IsTrue(_viewModel.CopySucessfull);
        }

    }
}