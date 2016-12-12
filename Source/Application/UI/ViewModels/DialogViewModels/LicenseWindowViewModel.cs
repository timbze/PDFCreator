using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class LicenseWindowViewModel : InteractionAwareViewModelBase<LicenseInteraction>
    {
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly IActivationHelper _activationHelper;

        public LicenseWindowViewModel(LicenseTabViewModel licenseTabViewModel, ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper, IUserGuideHelper userGuideHelper, IActivationHelper activationHelper)
        {
            LicenseTabViewModel = licenseTabViewModel;
            _userGuideHelper = userGuideHelper;
            _activationHelper = activationHelper;
            Title = applicationNameProvider.ApplicationName + " " + versionHelper.FormatWithThreeDigits();

            ShowHelpCommand = new DelegateCommand<KeyEventArgs>(ShowHelpCommandExecute);

            licenseTabViewModel.CloseLicenseWindowEvent += CloseWhenLicenseValid;
        }

        public LicenseTabViewModel LicenseTabViewModel { get; }

        public string Title { get; private set; }
        public DelegateCommand<KeyEventArgs> ShowHelpCommand { get; }

        private void ShowHelpCommandExecute(KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                _userGuideHelper.ShowHelp(HelpTopic.AppLicense);
        }

        private void CloseWhenLicenseValid(object sender, ActivationResponseEventArgs e)
        {
            if (e.IsLicenseValid)
                FinishInteraction();
        }
    }
}