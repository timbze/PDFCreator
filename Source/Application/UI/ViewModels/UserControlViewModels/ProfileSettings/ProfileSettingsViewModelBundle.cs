using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class ProfileSettingsViewModelBundle
    {
        public ProfileSettingsViewModelBundle(
            DocumentTabViewModel documentTabViewModel,
            SaveTabViewModel saveTabViewModel,
            AutoSaveTabViewModel autoSaveTabViewModel,
            ActionsTabViewModel actionsTabViewModel,
            ImageFormatsTabViewModel imageFormatsTabViewModel,
            PdfTabViewModel pdfTabViewModel)
        {
            DocumentTabViewModel = documentTabViewModel;
            SaveTabViewModel = saveTabViewModel;
            AutoSaveTabViewModel = autoSaveTabViewModel;
            ActionsTabViewModel = actionsTabViewModel;
            ImageFormatsTabViewModel = imageFormatsTabViewModel;
            PdfTabViewModel = pdfTabViewModel;
        }

        public DocumentTabViewModel DocumentTabViewModel { get; }
        public SaveTabViewModel SaveTabViewModel { get; }
        public AutoSaveTabViewModel AutoSaveTabViewModel { get; }

        public ActionsTabViewModel ActionsTabViewModel { get; }
        public ImageFormatsTabViewModel ImageFormatsTabViewModel { get; }
        public PdfTabViewModel PdfTabViewModel { get; }

        public void UpdateCurrentProfile(ConversionProfile currentProfile)
        {
            DocumentTabViewModel.CurrentProfile = currentProfile;
            SaveTabViewModel.CurrentProfile = currentProfile;
            AutoSaveTabViewModel.CurrentProfile = currentProfile;
            ActionsTabViewModel.CurrentProfile = currentProfile;
            ImageFormatsTabViewModel.CurrentProfile = currentProfile;
            PdfTabViewModel.CurrentProfile = currentProfile;
            
        }

        public void SetAccounts(Accounts accounts)
        {
            ActionsTabViewModel.SetAccountSettings(accounts);
        }
    }
}