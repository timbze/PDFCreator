using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class ActionsTabViewModel : CurrentProfileViewModel
    {
        public ActionsTabViewModel(ActionsTabTranslation translation)
        {
            Translation = translation;
            ActionCollectionView = CollectionViewSource.GetDefaultView(Actions);
        }

        public ActionsTabTranslation Translation { get; }

        private ObservableCollection<ActionViewModel> Actions { get; } = new ObservableCollection<ActionViewModel>();

        public ICollectionView ActionCollectionView { get; }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            var currentViewModel = ActionCollectionView.CurrentItem as ActionViewModel;
            return currentViewModel?.GetContextBasedHelpTopic() ?? HelpTopic.ProfileActions;
        }

        public void SetAccountSettings(Accounts accounts)
        {
            foreach (var item in Actions)
            {
                item.Accounts = accounts;
            }
        }

        protected override void HandleCurrentProfileChanged()
        {
            foreach (var actionViewModel in Actions)
            {
                actionViewModel.CurrentProfile = CurrentProfile;
                actionViewModel.RaiseEnabledChanged();
            }
            SelectFirstEnabledOrFirstAction();
        }

        private void SelectFirstEnabledOrFirstAction()
        {
            if (Actions == null)
                return;

            foreach (var action in Actions)
            {
                if (!action.IsEnabled)
                    continue;

                ActionCollectionView.MoveCurrentTo(action);
                return;
            }

            ActionCollectionView.MoveCurrentToFirst();
        }

        public void AddAction(ActionViewModel actionBundle)
        {
            Actions.Add(actionBundle);
        }
    }
}