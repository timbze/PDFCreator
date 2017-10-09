using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class DefectiveProfilesViewModel : OverlayViewModelBase<ProfileProblemsInteraction, DefectiveProfilesViewTranslation>
    {
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;

        public DefectiveProfilesViewModel(ITranslationUpdater translationUpdater, ErrorCodeInterpreter errorCodeInterpreter) : base(translationUpdater)
        {
            _errorCodeInterpreter = errorCodeInterpreter;

            SaveCommand = new DelegateCommand(SaveCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }

        public ActionResultDict ProfileProblems { get; private set; }

        public IList<ProfileError> ProfileErrors { get; protected set; }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public override string Title => Translation.DefectiveProfiles;

        private void SaveCommandExecute(object o)
        {
            Interaction.IgnoreProblems = true;
            FinishInteraction();
        }

        private void CancelCommandExecute(object o)
        {
            Interaction.IgnoreProblems = false;
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            ProfileProblems = Interaction.ProfileProblems;

            ProfileErrors = new List<ProfileError>();

            foreach (var profileNameActionResult in Interaction.ProfileProblems)
            {
                foreach (var error in profileNameActionResult.Value)
                {
                    var errorText = _errorCodeInterpreter.GetErrorText(error, false);
                    ProfileErrors.Add(new ProfileError(profileNameActionResult.Key, errorText));
                }
            }

            RaisePropertyChanged(nameof(ProfileErrors));

            var view = (CollectionView)CollectionViewSource.GetDefaultView(ProfileErrors);
            var groupDescription = new PropertyGroupDescription("Profile");
            view.GroupDescriptions.Add(groupDescription);
        }

        public string ComposeCopyAndPasteText()
        {
            var text = new StringBuilder();
            var previousProfile = "";

            foreach (var profileError in ProfileErrors)
            {
                if (previousProfile != profileError.Profile)
                {
                    text.AppendLine(profileError.Profile);
                    previousProfile = profileError.Profile;
                }

                text.AppendLine("- " + profileError.Error);
            }

            return text.ToString();
        }
    }

    public class ProfileError
    {
        public ProfileError(string profile, string error)
        {
            Profile = profile;
            Error = error;
        }

        public string Error { get; set; }
        public string Profile { get; set; }
    }
}
