using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using Prism.Regions;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.EditionBase.Tab
{
    public interface ISettingsTab
    {
        void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string regionName);

        void RegisterNavigationViews(Container container);
    }
}
