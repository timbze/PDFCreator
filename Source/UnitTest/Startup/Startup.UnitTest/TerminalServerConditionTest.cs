using System.Linq;
using SystemInterface.Diagnostics;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.Startup
{


    [TestFixture]
    public class TerminalServerConditionTest
    {
        private ITerminalServerDetection _terminalServerDetection;
        private IInteractionInvoker _interactionInvoker;
        private IProcessStarter _process;

        [SetUp]
        public void Setup()
        {
            _terminalServerDetection = Substitute.For<ITerminalServerDetection>();
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _process = Substitute.For<IProcessStarter>();
        }

        private TerminalServerNotAllowedCondition BuildTerminalServerCondition()
        {
            return new TerminalServerNotAllowedCondition(_terminalServerDetection, new SectionNameTranslator(), _interactionInvoker, _process, new ApplicationNameProvider("PDFCreator"));
        }

        [Test]
        public void WhenNotOnTerminalServer_IsSuccessful()
        {
            var terminalServerCondition = BuildTerminalServerCondition();
            _terminalServerDetection.IsTerminalServer().Returns(false);

            var result = terminalServerCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void WhenOnTerminalServer_FailsWith_NotValidOnTerminalServer()
        {
            var terminalServerCondition = BuildTerminalServerCondition();
            _terminalServerDetection.IsTerminalServer().Returns(true);

            var result = terminalServerCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.NotValidOnTerminalServer, result.ExitCode);
        }

        [Test]
        public void WhenOnTerminalServer_ShowsMoreInfoInteraction()
        {
            var terminalServerCondition = BuildTerminalServerCondition();
            _terminalServerDetection.IsTerminalServer().Returns(true);

            terminalServerCondition.Check();

            _interactionInvoker.Received().Invoke(Arg.Any<MessageInteraction>());
            var interaction = (MessageInteraction)_interactionInvoker.ReceivedCalls().First().GetArguments()[0];
            Assert.AreEqual("Program\\UsePDFCreatorTerminalServer", interaction.Text);
            Assert.AreEqual(MessageOptions.MoreInfoCancel, interaction.Buttons);
        }

        [Test]
        public void WhenOnTerminalServer_MoreInfoButton_TriggersWebpage()
        {
            var terminalServerCondition = BuildTerminalServerCondition();
            _terminalServerDetection.IsTerminalServer().Returns(true);
            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    x.Arg<MessageInteraction>().Response = MessageResponse.MoreInfo;
                });

            terminalServerCondition.Check();

            _process.Received().Start(Urls.PdfCreatorTerminalServerUrl);
        }
    }
}
