using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class NavigateMainTabCommand : ICommand
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _aggregator;

        public IInteractionRequest InteractionRequest { get; }

        public NavigateMainTabCommand(IRegionManager regionManager, IInteractionRequest interactionRequest, IEventAggregator aggregator)
        {
            _regionManager = regionManager;
            _aggregator = aggregator;
            InteractionRequest = interactionRequest;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, parameter.ToString(), OnNavigateMainTab);
        }

        private void OnNavigateMainTab(NavigationResult navigationResult)
        {
            var navigateMainTabEvent = _aggregator.GetEvent<NavigateMainTabEvent>();
            navigateMainTabEvent.Publish(navigationResult.Context.Uri.ToString());
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
