using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using Prism.Regions;
using SimpleInjector;
using System;
using Translatable;

namespace pdfforge.PDFCreator.Editions.EditionBase.Tab
{
    public interface ISubTab
    {
        void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string masterTabItemsRegion, string masterTabContentRegion, bool isFirstTab);

        void RegisterNavigationViews(Container container);
    }

    public class SubTab<TView, TTranslation> : ISubTab
        where TView : class
        where TTranslation : ITranslatable, new()
    {
        private readonly Func<TTranslation, string> _titleId;
        private readonly Func<ConversionProfile, IProfileSetting> _setting;

        public SubTab(Func<TTranslation, string> titleTranslationFunc, Func<ConversionProfile, IProfileSetting> setting)
        {
            _titleId = titleTranslationFunc;
            _setting = setting;
        }

        public void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string masterTabItemsRegion, string masterTabContentRegion, bool isFirstTab)
        {
            regionManager.RegisterSubTab<TView, ProfileSubTabViewModel, TTranslation>(masterTabItemsRegion, masterTabContentRegion, _titleId, _setting, serviceLocator, isFirstTab);
        }

        public void RegisterNavigationViews(Container container)
        {
            container.RegisterTypeForNavigation<TView>();
        }
    }
}
