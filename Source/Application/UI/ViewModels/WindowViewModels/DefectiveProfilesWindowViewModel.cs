using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class DefectiveProfilesWindowViewModel : InteractionAwareViewModelBase<ProfileProblemsInteraction>
    {
        private DefectiveProfilesWindowTranslation _translation;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;

        public DefectiveProfilesWindowViewModel(DefectiveProfilesWindowTranslation translation, ErrorCodeInterpreter errorCodeInterpreter)
        {
            _translation = translation;
            _errorCodeInterpreter = errorCodeInterpreter;

            IgnoreErrorsCommand = new DelegateCommand(IgnoreErrorsExecute);
        }

        public DefectiveProfilesWindowTranslation Translation
        {
            get { return _translation; }
            set { _translation = value; RaisePropertyChanged(nameof(Translation)); }
        }

        public ActionResultDict ProfileProblems { get; private set; }

        public IList<ProfileError> ProfileErrors { get; protected set; }

        public DelegateCommand IgnoreErrorsCommand { get; set; }

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

        private void IgnoreErrorsExecute(object o)
        {
            Interaction.IgnoreProblems = true;
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

            var view = (CollectionView) CollectionViewSource.GetDefaultView(ProfileErrors);
            var groupDescription = new PropertyGroupDescription("Profile");
            view.GroupDescriptions.Add(groupDescription);
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