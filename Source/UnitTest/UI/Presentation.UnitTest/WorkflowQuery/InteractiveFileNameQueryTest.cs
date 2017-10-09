using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.WorkflowQuery;
using pdfforge.PDFCreator.Utilities.IO;

namespace Presentation.UnitTest.WorkflowQuery
{
    [TestFixture]
    public class InteractiveFileNameQueryTest
    {
        private IInteractionInvoker _interactionInvoker;
        private IDirectoryHelper _directoryHelper;
        private string _someDirectory = @"X:\Test\Some Folder";
        private string _someFile = @"test.pdf";
        private string _someFilePath;

        [SetUp]
        public void Setup()
        {
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _directoryHelper = Substitute.For<IDirectoryHelper>();
            _someFilePath = Path.Combine(_someDirectory, _someFile);
        }

        private InteractiveFileNameQuery BuildFileNameQuery()
        {
            return new InteractiveFileNameQuery(new DesignTimeTranslationUpdater(), _interactionInvoker, _directoryHelper);
        }

        private void HandleSaveFileInteraction(Action<SaveFileInteraction> action)
        {
            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<SaveFileInteraction>()))
                .Do(x => action(x.Arg<SaveFileInteraction>()));
        }

        private void HandleMessageInteraction(Action<MessageInteraction> action)
        {
            _interactionInvoker
                .When(x => x.Invoke(Arg.Any<MessageInteraction>()))
                .Do(x => action(x.Arg<MessageInteraction>()));
        }

        [Test]
        public void GetFileName_WhenCancelled_GivesResultFalse()
        {
            HandleSaveFileInteraction(interaction => { interaction.Success = false; });
            var query = BuildFileNameQuery();

            var result = query.GetFileName(_someDirectory, _someFile, OutputFormat.Pdf);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void GetFileName_SaveFileInteraction_HasOneFilterEntryPerOutputFormat()
        {
            SaveFileInteraction interaction = null;
            HandleSaveFileInteraction(i => { interaction = i; });
            var query = BuildFileNameQuery();

            var result = query.GetFileName(_someDirectory, _someFile, OutputFormat.Pdf);

            var filters = interaction.Filter.Split('|');
            var numOutputFormats = Enum.GetValues(typeof(OutputFormat)).Length;
            Assert.AreEqual(numOutputFormats, filters.Length / 2);
        }

        [Test]
        public void GetFileName_WhenInteractionSuccessful_GivesNewFilename()
        {
            var expectedFilename = @"X:\Test\my File.jpg";

            HandleSaveFileInteraction(interaction =>
            {
                interaction.Success = true;
                interaction.FileName = expectedFilename;
                interaction.FilterIndex = 5; // JPEG
            });
            var query = BuildFileNameQuery();

            var result = query.GetFileName(_someDirectory, _someFile, OutputFormat.Pdf);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(expectedFilename, result.Data.Filepath);
            Assert.AreEqual(OutputFormat.Jpeg, result.Data.OutputFormat);
        }

        [Test]
        public void GetFileName_OutputFormatDoesNotMatchExtension_GivesFilenameWithFixedExtension()
        {
            var expectedFilename = @"X:\Test\my File.jpg";
            var parameterFilename = @"X:\Test\my File.pdf";

            HandleSaveFileInteraction(interaction =>
            {
                interaction.Success = true;
                interaction.FileName = parameterFilename;
                interaction.FilterIndex = 5; // JPEG
            });
            var query = BuildFileNameQuery();

            var result = query.GetFileName(_someDirectory, _someFile, OutputFormat.Pdf);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(expectedFilename, result.Data.Filepath);
            Assert.AreEqual(OutputFormat.Jpeg, result.Data.OutputFormat);
        }

        [Test]
        public void GetFileName_WhenInteractionIsCalled_CallsDirectoryHelper()
        {
            var expectedDirectory = Path.GetDirectoryName(_someFilePath);

            HandleSaveFileInteraction(interaction => { interaction.Success = false; });
            var query = BuildFileNameQuery();

            var result = query.GetFileName(_someDirectory, _someFile, OutputFormat.Pdf);

            _directoryHelper.Received().CreateDirectory(expectedDirectory);
        }

        [Test]
        public void GetFileName_WithPathTooLong_NotifiesUser()
        {
            var tooLongDirectory = @"X:\\" + new string('a', 300) + @"";

            SaveFileInteraction interaction = null;
            HandleSaveFileInteraction(i => { interaction = i; });
            var query = BuildFileNameQuery();

            var result = query.GetFileName(tooLongDirectory, _someFile, OutputFormat.Pdf);

            Assert.AreEqual("", interaction.InitialDirectory);
            Assert.AreEqual(_someFile, interaction.FileName);
        }

        [Test]
        public void RetypeFileName_NotifiesUser()
        {
            var translation = new InteractiveWorkflowTranslation();
            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.OK);
            HandleSaveFileInteraction(interaction => interaction.Success = false);
            var query = BuildFileNameQuery();

            query.RetypeFileName(_someFile, OutputFormat.Pdf);

            _interactionInvoker.Received().Invoke(Arg.Is<MessageInteraction>(i => i.Text.Contains(translation.RetypeFilenameMessage)));
            _interactionInvoker.Received().Invoke(Arg.Any<SaveFileInteraction>());
        }

        [Test]
        public void RetypeFileName_WhenCancelled_ReturnsFalse()
        {
            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.OK);
            HandleSaveFileInteraction(interaction => interaction.Success = false);
            var query = BuildFileNameQuery();

            var result = query.RetypeFileName(_someFile, OutputFormat.Pdf);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void RetypeFileName_WhenSuccess_ReturnsFilename()
        {
            HandleMessageInteraction(interaction => interaction.Response = MessageResponse.OK);
            HandleSaveFileInteraction(interaction =>
            {
                interaction.Success = true;
                interaction.FileName = _someFilePath;
            });
            var query = BuildFileNameQuery();

            var result = query.RetypeFileName(_someFile, OutputFormat.Pdf);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(_someFilePath, result.Data);
        }

        [TestCase(OutputFormat.Pdf)]
        [TestCase(OutputFormat.Png)]
        public void RetypeFileName_FileTypeFilter_OnlyHasCurrentFormat(OutputFormat outputFormat)
        {
            SaveFileInteraction saveFileInteraction = null;
            HandleSaveFileInteraction(interaction =>
            {
                saveFileInteraction = interaction;
            });

            var query = BuildFileNameQuery();

            query.RetypeFileName(_someFile, outputFormat);

            // Filter has the Format "Title|filter|Title2|filter2" => we only expect one pipe for a single entry
            Assert.AreEqual(2, saveFileInteraction.Filter.Split('|').Length);
            Assert.IsTrue(saveFileInteraction.Filter.Contains(outputFormat.ToString().ToLower()));
        }
    }
}
