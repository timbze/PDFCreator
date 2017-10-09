using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class CheckProfilecommandTest
    {
        private IProfileChecker _profileChecker;
        private UnitTestInteractionRequest _interactionRequest;

        [SetUp]
        public void Setup()
        {
            _profileChecker = Substitute.For<IProfileChecker>();
        }

        private CheckProfileCommand BuildCommand()
        {
            var settings = new PdfCreatorSettings(null);
            _interactionRequest = new UnitTestInteractionRequest();

            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            currentSettingsProvider.Settings.Returns(settings);
            currentSettingsProvider.Profiles.Returns(settings.ConversionProfiles);
            currentSettingsProvider.SelectedProfile.Returns(settings.ConversionProfiles.FirstOrDefault());

            return new CheckProfileCommand(_profileChecker, currentSettingsProvider, _interactionRequest, new ErrorCodeInterpreter(new TranslationFactory()));
        }

        [Test]
        public void CanExecute_AlwaysTrue()
        {
            var command = BuildCommand();
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void Execute_WithNoErrors_CallsIsDoneWithSuccess()
        {
            var command = BuildCommand();
            var commandTester = new WaitableCommandTester<CheckProfileCommand>(command);
            _profileChecker.ProfileCheckDict(null, null).ReturnsForAnyArgs(new ActionResultDict());

            command.Execute(null);

            Assert.IsTrue(commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, commandTester.LastResponseStatus);
        }

        [Test]
        public void Execute_WithErrors_UserChoosesToSave_CallsIsDoneWithSuccess()
        {
            var command = BuildCommand();
            var commandTester = new WaitableCommandTester<CheckProfileCommand>(command);
            var actionResult = new ActionResultDict();
            actionResult.Add("a", new ActionResult(ErrorCode.Processing_GenericError));
            _profileChecker.ProfileCheckDict(null, null).ReturnsForAnyArgs(actionResult);
            _interactionRequest.RegisterInteractionHandler<ProfileProblemsInteraction>(i => i.IgnoreProblems = true);

            command.Execute(null);

            Assert.IsTrue(commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, commandTester.LastResponseStatus);
        }

        [Test]
        public void Execute_WithErrors_UserChoosesToCancel_CallsIsDoneWithCancel()
        {
            var command = BuildCommand();
            var commandTester = new WaitableCommandTester<CheckProfileCommand>(command);
            var actionResult = new ActionResultDict();
            actionResult.Add("a", new ActionResult(ErrorCode.Processing_GenericError));
            _profileChecker.ProfileCheckDict(null, null).ReturnsForAnyArgs(actionResult);
            _interactionRequest.RegisterInteractionHandler<ProfileProblemsInteraction>(i => i.IgnoreProblems = false);

            command.Execute(null);

            Assert.IsTrue(commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Cancel, commandTester.LastResponseStatus);
        }
    }
}
