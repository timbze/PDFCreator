using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using Prism.Regions;
using SimpleInjector;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Editions.EditionBase.Tab
{
    public class TabRegion
    {
        public string RegionName { get; }
        public IList<ISettingsTab> Tabs { get; } = new List<ISettingsTab>();

        public TabRegion(string regionName)
        {
            RegionName = regionName;
        }

        public void RegisterTabs(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator)
        {
            foreach (var pageTab in Tabs)
            {
                pageTab.Register(regionManager, serviceLocator, RegionName);
            }
        }

        public void RegisterNavigationViews(Container container)
        {
            foreach (var pageTab in Tabs)
            {
                pageTab.RegisterNavigationViews(container);
            }
        }

        public void Add(ISettingsTab tab)
        {
            Tabs.Add(tab);
        }
    }
}
