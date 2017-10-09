using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class ShowUserGuideCommandTest
    {
        private ShowUserGuideCommand _showUserGuideCommand;
        private IUserGuideHelper _userGuideHelper;
        private HelpTopic _transmittedHelpTopic = HelpTopic.License; //Must not be HelpTopic.General
        private HelpTopic _initHelpTopic = HelpTopic.AppGeneral;  //Must not be HelpTopic.General

        [SetUp]
        public void SetUp()
        {
            _userGuideHelper = Substitute.For<IUserGuideHelper>();
            _showUserGuideCommand = new ShowUserGuideCommand(_userGuideHelper);
        }

        [Test]
        public void CanExecute_ReturnsTrue()
        {
            Assert.IsTrue(_showUserGuideCommand.CanExecute(null));
        }

        [Test]
        public void Execute_CommandIsNotInitializedTransmittedParmeterIsNull_UserGuideShowsGeneralTopic()
        {
            _showUserGuideCommand.Execute(null);

            _userGuideHelper.Received().ShowHelp(HelpTopic.General);
        }

        [Test]
        public void Execute_CommandIsNotInitializedTransmittedParmeterIsNotNull_ShowHelpWithGivenTopic()
        {
            _showUserGuideCommand.Execute(_transmittedHelpTopic);

            _userGuideHelper.Received().ShowHelp(_transmittedHelpTopic);
        }

        [Test]
        public void Execute_CommandIsInitializedTransmittedParmeterIsNotNull_ShowHelpWithTransmittedTopic()
        {
            _showUserGuideCommand.Init(_initHelpTopic);

            _showUserGuideCommand.Execute(_transmittedHelpTopic);

            _userGuideHelper.Received().ShowHelp(_transmittedHelpTopic);
        }

        [Test]
        public void Execute_CommandIsInitializedTransmittedParmeterIsNull_ShowHelpWithInitTopic()
        {
            _showUserGuideCommand.Init(_initHelpTopic);

            _showUserGuideCommand.Execute(null);

            _userGuideHelper.Received().ShowHelp(_initHelpTopic);
        }
    }
}
