using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using Arg = NSubstitute.Arg;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.Assistants
{
    [TestFixture]
    public class SmtpTestEmailAssistantTest
    {
        private IInteractionInvoker _interactionInvoker;
        private IFile _file;
        private ISmtpMailAction _smtpAction;
        private IPath _path;
        private ConversionProfile _profile;
        private IList<IInteraction> _interactions;

        [SetUp]
        public void Setup()
        {
            _profile = new ConversionProfile();

            _interactions = new List<IInteraction>();
            _interactionInvoker = Substitute.For<IInteractionInvoker>();

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<PasswordInteraction>()))
                .Do(x =>
                {
                    var interaction = x.Arg<PasswordInteraction>();
                    interaction.Result = PasswordResult.StorePassword;
                });

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<IInteraction>()))
                .Do(x =>
                {
                    _interactions.Add(x.Arg<IInteraction>());
                });

            _file = Substitute.For<IFile>();
            _path = Substitute.For<IPath>();
            _smtpAction = Substitute.For<ISmtpMailAction>();
            _smtpAction.Check(_profile, Arg.Any<Accounts>()).Returns(x => new ActionResult());
            _smtpAction.ProcessJob(Arg.Any<Job>()).Returns(x => new ActionResult());
        }

        private SmtpTestEmailAssistant BuildAssistant()
        {
            return new SmtpTestEmailAssistant(new SectionNameTranslator(), _interactionInvoker, _file, _smtpAction, _path);
        }

        [Test]
        public void WhenSuccessful_ShowsSuccessMessage()
        {
            var assistant = BuildAssistant();

            assistant.SendTestMail(_profile, new Accounts());

            _interactionInvoker.Received().Invoke(Arg.Any<MessageInteraction>());

            var messageInteraction =
                _interactions.Where(x => x is MessageInteraction).Cast<MessageInteraction>().First();

            Assert.AreEqual("SmtpEmailActionSettings\\SendTestMail", messageInteraction.Title);
            Assert.AreEqual("SmtpEmailActionSettings\\TestMailSent", messageInteraction.Text);
            Assert.AreEqual(MessageIcon.Info, messageInteraction.Icon);
        }

        [Test]
        public void WhenSmtpPasswordIsSet_UsesPasswordFromProfile()
        {
            var expectedPassword = "My random password string!";
            _profile.EmailSmtpSettings.Password = expectedPassword;
            var assistant = BuildAssistant();
            assistant.SendTestMail(_profile, new Accounts());

            _smtpAction.Received().ProcessJob(Arg.Is<Job>(x => x.Passwords.SmtpPassword == expectedPassword));
        }

        [Test]
        public void WhenCalled_UsesTheGivenProfile()
        {
            var expectedPassword = "My random password string!";
            _profile.EmailSmtpSettings.Password = expectedPassword;
            var assistant = BuildAssistant();
            assistant.SendTestMail(_profile, new Accounts());

            _smtpAction.Received().ProcessJob(Arg.Is<Job>(x => x.Profile.Equals(_profile)));
        }

        [Test]
        public void WhenCalled_WithAutoSave_DisablesAutoSave()
        {
            var expectedPassword = "My random password string!";
            _profile.EmailSmtpSettings.Password = expectedPassword;
            var assistant = BuildAssistant();
            _smtpAction.Check(Arg.Any<ConversionProfile>(), Arg.Any<Accounts>()).Returns(x => new ActionResult());
            _profile.AutoSave.Enabled = true;
            assistant.SendTestMail(_profile, new Accounts());

            _smtpAction.Received().ProcessJob(Arg.Is<Job>(x => x.Profile.AutoSave.Enabled == false));
        }

        [Test]
        public void SendTestMail_CallsSmtpAction()
        {
            var assistant = BuildAssistant();

            assistant.SendTestMail(_profile, new Accounts());

            _smtpAction.Received().ProcessJob(Arg.Any<Job>());
        }

        [Test]
        public void WhenProfileInvalid_DisplaysErrorMessage()
        {
            var expectedError = ErrorCode.Smtp_NoPasswordSpecified;
            _smtpAction.Check(_profile, Arg.Any<Accounts>()).Returns(x => new ActionResult(expectedError));
            var assistant = BuildAssistant();
            assistant.SendTestMail(_profile, new Accounts());

            _interactionInvoker.Received().Invoke(Arg.Any<MessageInteraction>());

            var messageInteraction =
                _interactions.Where(x => x is MessageInteraction).Cast<MessageInteraction>().First();

            var errorCodeInt = (int) expectedError;
            Assert.AreEqual("SmtpEmailActionSettings\\SendTestMail", messageInteraction.Title);
            Assert.AreEqual($"ErrorCodes\\{errorCodeInt}", messageInteraction.Text);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }

        [Test]
        public void WhenPasswordInteractionIsCancelled_DoesNotSendMail()
        {
            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<PasswordInteraction>()))
                .Do(x =>
                {
                    var interaction = x.Arg<PasswordInteraction>();
                    interaction.Result = PasswordResult.Cancel;
                });
            var assistant = BuildAssistant();
            assistant.SendTestMail(_profile, new Accounts());

            _smtpAction.DidNotReceive().ProcessJob(Arg.Any<Job>());
        }

        [Test]
        public void WhenSendingMailFails_ShowsErrorMessage()
        {
            var expectedError = ErrorCode.Smtp_AuthenticationDenied;
            _smtpAction.ProcessJob(Arg.Any<Job>()).Returns(new ActionResult(expectedError));
            var assistant = BuildAssistant();
            assistant.SendTestMail(_profile, new Accounts());

            _interactionInvoker.Received().Invoke(Arg.Any<MessageInteraction>());

            var messageInteraction =
                _interactions.Where(x => x is MessageInteraction).Cast<MessageInteraction>().First();

            var errorCodeInt = (int)expectedError;
            Assert.AreEqual("SmtpEmailActionSettings\\SendTestMail", messageInteraction.Title);
            Assert.AreEqual($"ErrorCodes\\{errorCodeInt}", messageInteraction.Text);
            Assert.AreEqual(MessageIcon.Error, messageInteraction.Icon);
        }
    }
}
