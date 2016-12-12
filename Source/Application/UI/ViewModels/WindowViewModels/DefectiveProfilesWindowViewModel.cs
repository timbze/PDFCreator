using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class DefectiveProfilesWindowViewModel : InteractionAwareViewModelBase<ProfileProblemsInteraction>
    {
        private readonly ITranslator _translator;

        public DefectiveProfilesWindowViewModel(ITranslator translator)
        {
            _translator = translator;

            IgnoreErrorsCommand = new DelegateCommand(IgnoreErrorsExecute);
        }

        public ActionResultDict ProfileProblems { get; private set; }

        public string DefectiveProfilesText { get; protected set; }

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

            if (Interaction.ProfileProblems.Count > 1)
                DefectiveProfilesText = _translator.GetTranslation(
                    "DefectiveProfilesWindow", "DefectiveProfiles");
            else
                DefectiveProfilesText = _translator.GetTranslation(
                    "DefectiveProfilesWindow", "DefectiveProfile");

            RaisePropertyChanged(nameof(DefectiveProfilesText));

            ProfileErrors = new List<ProfileError>();

            var errorCodeInterpreter = new ErrorCodeInterpreter(_translator);

            foreach (var profileNameActionResult in Interaction.ProfileProblems)
            {
                foreach (var error in profileNameActionResult.Value)
                {
                    var errorText = errorCodeInterpreter.GetErrorText(error, false);
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