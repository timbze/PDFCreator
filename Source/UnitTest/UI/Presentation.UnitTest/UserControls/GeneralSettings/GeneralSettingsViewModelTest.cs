using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace Presentation.UnitTest.UserControls.GeneralSettings
{
    [TestFixture]
    public class GeneralSettingsViewModelTest
    {
        private ITranslationUpdater _translationUpdater;

        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private GeneralSettingsViewModel BuildViewModel()
        {
            return new GeneralSettingsViewModel(_translationUpdater);
        }

        [Test]
        public void RequestProperties_GetProperties()
        {
            var viewModel = BuildViewModel();

            var applicationSettingsWindowTranslation = viewModel.Translation;
            var viewModelIcon = viewModel.Icon;
            var viewModelTitle = viewModel.Title;

            Assert.NotNull(applicationSettingsWindowTranslation);
            Assert.NotNull(viewModelIcon);
            Assert.NotNull(viewModelTitle);
        }

        [Test]
        public void SetTranslationViaTranslationUpdater_TitleFitTranslationInTranslation()
        {
            var viewModel = BuildViewModel();

            var applicationSettingsWindowTranslation = viewModel.Translation;
            var viewModelTitle = viewModel.Title;

            Assert.AreEqual(applicationSettingsWindowTranslation.GeneralTabTitle, viewModelTitle);
        }
    }
}
