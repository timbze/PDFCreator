using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.Views.ActionControls;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class ProfileSettingsWindow : Window
    {
        private readonly IUserGuideHelper _userGuideHelper;

        public ProfileSettingsWindow(ProfileSettingsViewModel profileSettingsViewModel, ActionControlsBundle actionControls, IUserGuideHelper userGuideHelper)
        {
            _userGuideHelper = userGuideHelper;
            DataContext = profileSettingsViewModel;
            InitializeComponent();
            AddActions(actionControls);
        }

        private void AddActions(ActionControlsBundle actionControls)
        {
            foreach (var actionControl in actionControls.GetActionControls())
            {
                // only the viewmodel is added here, as the action control is set as the content property of the viewmodel
                ActionsTabUserControl.ViewModel.AddAction(actionControl.ActionViewModel);
            }
        }

        private void ProfileSettingsWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                ShowConextBasedHelp();
        }

        private void ShowConextBasedHelp()
        {
            var currentTab = TabControl.SelectedItem as TabItem;
            var control = currentTab?.Content as UserControl;
            var dataContext = control?.DataContext as CurrentProfileViewModel;

            var currentTopic = dataContext?.GetContextBasedHelpTopic() ?? HelpTopic.ProfileSettings;

            _userGuideHelper.ShowHelp(currentTopic);
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowConextBasedHelp();
        }
    }
}