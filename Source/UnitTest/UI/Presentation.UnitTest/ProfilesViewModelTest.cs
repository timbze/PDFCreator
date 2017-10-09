using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace Presentation.UnitTest
{
    [TestFixture]
    public class ProfilesViewModelTest
    {
        private ProfilesViewModel _profilesViewModel;
        private ISelectedProfileProvider _selectedProfileProvider;
        private ITranslationUpdater _translationUpdater;
        private ProfileMangementTranslation _translation;

        [SetUp]
        public void SetUp()
        {
            _selectedProfileProvider = Substitute.For<ISelectedProfileProvider>();
            _translationUpdater = Substitute.For<ITranslationUpdater>();

            _profilesViewModel = new ProfilesViewModel(_selectedProfileProvider, _translationUpdater, new DesignTimeCommandLocator(), null, null);

            _translation = new ProfileMangementTranslation();
            _profilesViewModel.Translation = _translation;
        }

        [Test]
        public void TranslationUpdater_RegisterAndSetTranslation_GetsCalled()
        {
            _translationUpdater.Received().RegisterAndSetTranslation(_profilesViewModel);
        }

        [Test]
        public void Translation_SetTranslation_RaisePropertyChangedGetsCalled()
        {
            var wasCalled = false;
            _profilesViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_profilesViewModel.Translation)))
                    wasCalled = true;
            };

            _profilesViewModel.Translation = _translation;

            Assert.IsTrue(wasCalled);
        }
    }
}
