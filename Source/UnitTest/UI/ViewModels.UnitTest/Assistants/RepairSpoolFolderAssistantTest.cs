using System.IO;
using SystemInterface;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.Assistants
{
    [TestFixture]
    public class RepairSpoolFolderAssistantTest
    {
        [SetUp]
        public void Setup()
        {
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _shellExecuteHelper = Substitute.For<IShellExecuteHelper>();
            _file = Substitute.For<IFile>();
        }

        private IInteractionInvoker _interactionInvoker;
        private IShellExecuteHelper _shellExecuteHelper;
        private IFile _file;

        private const string SpoolFolder = @"X:\Temp\Spool";
        private const string AssemblyFolder = @"X:\MyFolder";
        private const string RepairToolPath = AssemblyFolder + @"\RepairFolderPermissions.exe";
        private const string Username = "MyUser";

        private RepairSpoolFolderAssistant BuildRepairSpoolFolderAssistant()
        {
            var spoolFolderProvider = Substitute.For<ISpoolerProvider>();
            spoolFolderProvider.SpoolFolder.Returns(SpoolFolder);

            var path = Substitute.For<IPath>();
            path.GetFullPath(Arg.Any<string>()).Returns(x => x.Arg<string>());
            path.Combine(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => Path.Combine(x.ArgAt<string>(0), x.ArgAt<string>(1)));

            var environment = Substitute.For<IEnvironment>();
            environment.UserName.Returns(Username);

            var assemblyHelper = Substitute.For<IAssemblyHelper>();
            assemblyHelper.GetPdfforgeAssemblyDirectory().Returns(AssemblyFolder);

            _file.Exists(RepairToolPath).Returns(true);

            return new RepairSpoolFolderAssistant(_interactionInvoker, new SectionNameTranslator(), spoolFolderProvider,
                _shellExecuteHelper, path, _file, environment, assemblyHelper);
        }

        [Test]
        public void DisplayRepairFailedMessage_UsesMessageInteractionWithCorrectMessage()
        {
            MessageInteraction interaction = null;

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x => interaction = x.ArgAt<MessageInteraction>(0));

            var repairSpoolFolderAssistant = BuildRepairSpoolFolderAssistant();

            repairSpoolFolderAssistant.DisplayRepairFailedMessage();

            Assert.NotNull(interaction, "The interaction was not called");
            Assert.AreEqual(interaction.Title, "Application\\SpoolFolderAccessDenied");
            Assert.AreEqual(MessageOptions.OK, interaction.Buttons);
        }

        [Test]
        public void TryRepairSpoolPath_UsesMessageInteractionWithCorrectMessage()
        {
            MessageInteraction interaction = null;

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    interaction = x.ArgAt<MessageInteraction>(0);
                    interaction.Response = MessageResponse.No;
                });

            var repairSpoolFolderAssistant = BuildRepairSpoolFolderAssistant();

            repairSpoolFolderAssistant.TryRepairSpoolPath();

            Assert.NotNull(interaction, "The interaction was not called");
            Assert.AreEqual(interaction.Title, "Application\\SpoolFolderAccessDenied");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons);
        }


        [Test]
        public void TryRepairSpoolPath_WhenRepairToolIsMissing_ShowsErrorMessage()
        {
            MessageInteraction interaction = null;

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    interaction = x.ArgAt<MessageInteraction>(0);
                    interaction.Response = MessageResponse.Yes;
                });

            var repairSpoolFolderAssistant = BuildRepairSpoolFolderAssistant();

            _file.Exists(RepairToolPath).Returns(false);

            repairSpoolFolderAssistant.TryRepairSpoolPath();

            _shellExecuteHelper.DidNotReceive().RunAsAdmin(RepairToolPath, Arg.Any<string>());

            Assert.NotNull(interaction);
            Assert.AreEqual(interaction.Title, "Application\\RepairToolNotFound");
        }

        [Test]
        public void TryRepairSpoolPath_WhenTryingToRepair_ShellExecuteHelperIsCalled()
        {
            MessageInteraction interaction;

            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x =>
                {
                    interaction = x.ArgAt<MessageInteraction>(0);
                    interaction.Response = MessageResponse.Yes;
                });


            var repairSpoolFolderAssistant = BuildRepairSpoolFolderAssistant();

            repairSpoolFolderAssistant.TryRepairSpoolPath();

            _shellExecuteHelper.Received().RunAsAdmin(RepairToolPath, Arg.Any<string>());
        }
    }
}
