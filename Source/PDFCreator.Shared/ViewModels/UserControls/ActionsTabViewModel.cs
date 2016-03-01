using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using pdfforge.PDFCreator.Shared.Views.ActionControls;

namespace pdfforge.PDFCreator.Shared.ViewModels.UserControls
{
    public class ActionsTabViewModel : CurrentProfileViewModel
    {
        private readonly ObservableCollection<ActionBundle> _actions = new ObservableCollection<ActionBundle>();
        private ICollectionView _actionsCollectionView;

        public ActionsTabViewModel()
        {
            ProfileChanged += OnProfileChanged;
            _actionsCollectionView = CollectionViewSource.GetDefaultView(_actions);
        }

        private void OnProfileChanged(object sender, EventArgs eventArgs)
        {
            foreach (var actionBundle in _actions)
            {
                actionBundle.ActionControl.CurrentProfile = CurrentProfile;
                actionBundle.RaiseIsEnabledChanged();
            }
        }

        public ObservableCollection<ActionBundle> Actions
        {
            get { return _actions; }
        }

        public ICollectionView ActionCollectionView
        {
            get {  return _actionsCollectionView; }
            
        }

        public void SelectFirstEnabledOrFirstAction()
        {
            if (Actions == null)
                return;

            foreach (var action in Actions)
            {
                if (action.IsEnabled)
                {
                    _actionsCollectionView.MoveCurrentTo(action);
                    return;
                }
            }

            _actionsCollectionView.MoveCurrentToFirst();
        }

        public void AddAction(ActionControl action)
        {
            var actionBundle = new ActionBundle(action);
            Actions.Add(actionBundle);
        }
    }

}
