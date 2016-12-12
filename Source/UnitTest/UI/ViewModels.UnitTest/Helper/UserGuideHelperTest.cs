using System.IO;
using System.Linq;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.UserGuide;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.Helper
{
    [TestFixture]
    public class UserGuideHelperTest
    {
        [SetUp]
        public void Setup()
        {
            _fileWrap = Substitute.For<IFile>();
            _userGuideLauncher = Substitute.For<IUserGuideLauncher>();
            _settingsProvider = Substitute.For<ISettingsProvider>();
            _settingsProvider.Settings.Returns(new PdfCreatorSettings(Substitute.For<IStorage>()));
            _assemblyHelper = Substitute.For<IAssemblyHelper>();
            _assemblyHelper.GetPdfforgeAssemblyDirectory().Returns(AssemblyPath);
        }

        private IFile _fileWrap;
        private IUserGuideLauncher _userGuideLauncher;
        private IAssemblyHelper _assemblyHelper;
        private ISettingsProvider _settingsProvider;

        private const string AssemblyPath = @"X:\MyPath";

        private UserGuideHelper BuildUserGuideHelper()
        {
            return new UserGuideHelper(_fileWrap, _assemblyHelper, _userGuideLauncher, _settingsProvider);
        }

        [Test]
        public void SetLanguage_WithExistingLanguageAndEnglishExists_SetsLanguage()
        {
            var userGuideHelper = BuildUserGuideHelper();
            var englishPath = Path.Combine(AssemblyPath, "PDFCreator_english.chm");
            var expectedPath = Path.Combine(AssemblyPath, "PDFCreator_german.chm");
            _fileWrap.Exists(englishPath).Returns(true);
            _fileWrap.Exists(expectedPath).Returns(true);

            userGuideHelper.SetLanguage("german");

            Assert.AreEqual(1, _userGuideLauncher.ReceivedCalls().Count());
            _userGuideLauncher.Received().SetUserGuide(expectedPath);
        }

        [Test]
        public void SetLanguage_WithNonexistentLanguage_DoesNotUpdateLauncher()
        {
            var userGuideHelper = BuildUserGuideHelper();

            userGuideHelper.SetLanguage("UnknownLanguage");

            _userGuideLauncher.DidNotReceiveWithAnyArgs().SetUserGuide(Arg.Any<string>());
        }

        [Test]
        public void SetLanguage_WithNonexistentLanguageAndEnglishExists_SetsEnglish()
        {
            var userGuideHelper = BuildUserGuideHelper();
            var englishPath = Path.Combine(AssemblyPath, "PDFCreator_english.chm");
            _fileWrap.Exists(englishPath).Returns(true);

            userGuideHelper.SetLanguage("UnknownLanguage");

            Assert.AreEqual(1, _userGuideLauncher.ReceivedCalls().Count());
            _userGuideLauncher.Received().SetUserGuide(englishPath);
        }

        [Test]
        public void ShowHelp_WithAnnotatedTopic_CallsShowHelp()
        {
            var userGuideHelper = BuildUserGuideHelper();

            userGuideHelper.ShowHelp(HelpTopic.General);

            Assert.AreEqual(1, _userGuideLauncher.ReceivedCalls().Count());
            _userGuideLauncher.Received().ShowHelpTopic(HelpTopic.General);
        }
    }
}