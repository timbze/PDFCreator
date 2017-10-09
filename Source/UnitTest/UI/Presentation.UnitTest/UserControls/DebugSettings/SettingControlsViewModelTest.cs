using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.ComponentModel;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class SettingControlsViewModelTest
    {
        private ITranslationUpdater _translationUpdater;

        [SetUp]
        public void Setup()
        {
            _translationUpdater = new DesignTimeTranslationUpdater();
        }

        private SettingControlsViewModel BuildViewModel()
        {
            return new SettingControlsViewModel(_translationUpdater, new DesignTimeCommandLocator());
        }

        [Test]
        public void Check_Properties()
        {
            var viewModel = BuildViewModel();
            Assert.NotNull(viewModel.SaveCommand);
            Assert.NotNull(viewModel.CancelCommand);
            Assert.NotNull(viewModel.Translation);
        }

        [Test]
        public void UpdateTranslation_TestForUpdatedTranslation()
        {
            var viewModel = BuildViewModel();
            var eventStub = Substitute.For<IEventHandler<PropertyChangedEventArgs>>();
            viewModel.PropertyChanged += eventStub.OnEventRaised;
            var listener1 = new PropertyChangedListenerMock(viewModel, nameof(viewModel.Translation));

            viewModel.Translation = new SettingControlsTranslation();

            Assert.IsTrue(listener1.WasCalled, nameof(viewModel.Translation));
        }
    }
}
