using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class QuickActionViewModelTest
    {
        private ICommandLocator _commandLocator;
        private IReadableFileSizeFormatter _fileSizeFormater;
        private ICommand _testCommand;
        private Job _job;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private ConversionProfile _profile;
        private ObservableCollection<ConversionProfile> _profiles;

        [SetUp]
        public void Setup()
        {
            _commandLocator = Substitute.For<ICommandLocator>();
            _testCommand = Substitute.For<ICommand>();
            _commandLocator.GetCommand<JobQuickActionOpenWithPdfArchitectCommand>().Returns(_testCommand);
            _fileSizeFormater = Substitute.For<IReadableFileSizeFormatter>();
            _job = new Job(null, null, null, null);
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _profile = new ConversionProfile();
            _profiles = new ObservableCollection<ConversionProfile>();
            _profiles.Add(_profile);
            _currentSettingsProvider.Profiles.Returns(_profiles);
            _job.Profile = _profile;
            _job.OutputFiles = new List<string> { "FirstFile.pdf" };
        }

        public QuickActionViewModel build()
        {
            var quickActionViewModel = new QuickActionViewModel(new DesignTimeTranslationUpdater(), _commandLocator, _fileSizeFormater, _currentSettingsProvider);

            quickActionViewModel.ExecuteWorkflowStep(_job);
            return quickActionViewModel;
        }

        [Test]
        public void Create_CheckVariables()
        {
            var model = build();
            _commandLocator.Received(1).GetCommand<JobQuickActionOpenWithPdfArchitectCommand>();
            _commandLocator.Received(1).GetCommand<JobQuickActionOpenWithDefaultCommand>();
            _commandLocator.Received(1).GetCommand<JobQuickActionOpenExplorerLocationCommand>();
            _commandLocator.Received(1).GetCommand<JobQuickActionSendEmailCommand>();
            Assert.AreEqual(OutputFormat.Pdf, model.OutputFormat);
            Assert.IsFalse(model.IsActive);
        }

        [Test]
        public void SetIsActive_ChangesInJob()
        {
            var model = build();
            model.IsActive = true;
            Assert.False(_job.Profile.ShowQuickActions);
            model.IsActive = false;
            Assert.True(_job.Profile.ShowQuickActions);
            // _commandLocator.Received(2).GetCommand<SaveApplicationSettingsChangesCommand>();
        }

        [Test]
        public void StartQuickAction_VoIsNull_DoNothing()
        {
            var model = build();
            model.StartQuickActionCommand.Execute(null);
            _testCommand.DidNotReceive().Execute(_job);
        }

        [Test]
        public void StartQuickAction_VoIsValid_DoNothing()
        {
            var model = build();
            var vo = new QuickActionListItemVo("testAction", _testCommand, model.StartQuickActionCommand);
            model.StartQuickActionCommand.Execute(vo);
            _testCommand.Received().Execute(_job);
        }
    }
}
