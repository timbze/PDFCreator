using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.Utilities.Process;
using System;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class UrlOpenCommandTest
    {
        private UrlOpenCommand _urlOpenCommand;
        private IProcessStarter _processStarter;

        [SetUp]
        public void SetUp()
        {
            _processStarter = Substitute.For<IProcessStarter>();
            _urlOpenCommand = new UrlOpenCommand(_processStarter);
        }

        [Test]
        public void CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_urlOpenCommand.CanExecute(null));
        }

        [Test]
        public void Execute_CommandIsNotInitializedTransmittedParmeterIsNull_DoesNotThrowException()
        {
            _urlOpenCommand.Execute(null);
            _processStarter.When(x => x.Start(Arg.Any<string>())).Do(x => { throw new Exception(); });

            Assert.DoesNotThrow(() => { _urlOpenCommand.Execute(null); });
        }

        [Test]
        public void Execute_CommandIsNotInitializedTransmittedParmeterIsNotNull_OpensProcessStartWithTransmittedParameter()
        {
            _urlOpenCommand.Execute("Transmitted Parameter String");

            _processStarter.Received().Start("Transmitted Parameter String");
        }

        [Test]
        public void Execute_CommandIsInitializedParmeterIsNotNull_OpensProcessStartWithTransmittedParameter()
        {
            _urlOpenCommand.Init("Init Paramter String");

            _urlOpenCommand.Execute("Transmitted Parameter String");

            _processStarter.Received().Start("Transmitted Parameter String");
        }

        [Test]
        public void Execute_CommandIsInitializedTransmittedParmeterIsNull_OpensProcessStartWithInitParameter()
        {
            _urlOpenCommand.Init("Init Paramter String");

            _urlOpenCommand.Execute(null);

            _processStarter.Received().Start("Init Paramter String");
        }
    }
}
