using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using Prism.Regions;
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Editions.EditionBase.Tab
{
    public class MultiTab<T> : ISettingsTab
        where T : class, ITabViewModel
    {
        private readonly HelpTopic _helpTopic;
        private readonly string _contentRegionName;
        private readonly IList<Type> _views;

        public MultiTab(string contentRegionName, HelpTopic helpTopic, params Type[] views)
        {
            _helpTopic = helpTopic;
            _contentRegionName = contentRegionName;
            _views = views;
        }

        public void Register(IRegionManager regionManager, IWhitelistedServiceLocator serviceLocator, string regionName)
        {
            regionManager.RegisterMultiContentTab<T>(regionName, _contentRegionName, _helpTopic, serviceLocator);
            foreach (var view in _views)
            {
                regionManager.RegisterViewWithRegion(_contentRegionName, view);
            }
        }

        public void RegisterNavigationViews(Container container)
        {
        }
    }
}
