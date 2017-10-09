using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class CopyToClipboardCommandTest
    {
        private CopyToClipboardCommand _copyToClipboardCommand;
        private IClipboardService _clipboardService;

        [SetUp]
        public void SetUp()
        {
            _clipboardService = Substitute.For<IClipboardService>();
            _copyToClipboardCommand = new CopyToClipboardCommand(_clipboardService);
        }

        [Test]
        public void CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_copyToClipboardCommand.CanExecute(null));
        }

        [Test]
        public void Execute_CallsClipbardServiceSetDataObjectWithParameter()
        {
            var parameter = new object();

            _copyToClipboardCommand.Execute(parameter);

            _clipboardService.Received().SetDataObject(parameter);
        }

        [Test]
        public void Execute_ClipbardServiceSetDataThrowsException_DoNotTrhowException()
        {
            _clipboardService.When(x => x.SetDataObject(Arg.Any<object>())).Throw<Exception>();

            Assert.DoesNotThrow(() => _copyToClipboardCommand.Execute(null));
        }
    }
}
