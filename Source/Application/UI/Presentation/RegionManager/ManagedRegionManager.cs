using Prism.Regions;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.RegionManager
{
    public class ManagedRegionManager : IRegionManager
    {
        private readonly Prism.Regions.RegionManager _regionManager;

        public ManagedRegionManager()
        {
            _regionManager = new Prism.Regions.RegionManager();
        }

        public IRegionManager CreateRegionManager()
        {
            return _regionManager.CreateRegionManager();
        }

        public IRegionManager AddToRegion(string regionName, object view)
        {
            return _regionManager.AddToRegion(regionName, view);
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
        {
            return _regionManager.AddToRegion(regionName, viewType);
        }

        public IRegionManager RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            return _regionManager.AddToRegion(regionName, getContentDelegate);
        }

        public void RequestNavigate(string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            _regionManager.RequestNavigate(regionName, source, navigationCallback);
        }

        public void RequestNavigate(string regionName, Uri source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        public void RequestNavigate(string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            _regionManager.RequestNavigate(regionName, source, navigationCallback);
        }

        public void RequestNavigate(string regionName, string source)
        {
            _regionManager.RequestNavigate(regionName, source);
        }

        public void RequestNavigate(string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationCallback, navigationParameters);
        }

        public void RequestNavigate(string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationCallback, navigationParameters);
        }

        public void RequestNavigate(string regionName, Uri target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        public void RequestNavigate(string regionName, string target, NavigationParameters navigationParameters)
        {
            _regionManager.RequestNavigate(regionName, target, navigationParameters);
        }

        public IRegionCollection Regions => _regionManager.Regions;
    }
}
