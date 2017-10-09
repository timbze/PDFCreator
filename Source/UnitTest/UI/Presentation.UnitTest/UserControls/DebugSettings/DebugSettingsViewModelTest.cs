using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace Presentation.UnitTest.UserControls.DebugSettings
{
    [TestFixture]
    public class DebugSettingsViewModelTest
    {
        private ITranslationUpdater _translationUpdater;

        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
        }

        public DebugSettingsViewModel BuildModel()
        {
            return new DebugSettingsViewModel(_translationUpdater, null, null);
        }

        [Test]
        public void Initialization_SetsTranslation()
        {
            var viewModel = BuildModel();

            Assert.NotNull(viewModel.Translation);
            Assert.IsNotNull(viewModel.Icon);
        }

        [Test]
        public void Validate_VTranslatableViewModelBase()
        {
            var viewModel = BuildModel();

            Assert.IsInstanceOf<TranslatableViewModelBase<DebugSettingsTranslation>>(viewModel);
        }
    }
}
