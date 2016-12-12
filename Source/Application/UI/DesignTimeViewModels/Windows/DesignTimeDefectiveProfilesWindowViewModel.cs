using System.Collections.Generic;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeDefectiveProfilesWindowViewModel : DefectiveProfilesWindowViewModel
    {
        public DesignTimeDefectiveProfilesWindowViewModel() : base(new TranslationProxy())
        {
            DefectiveProfilesText = "Defective Profiles:";

            ProfileErrors = new List<ProfileError>();
            ProfileErrors.Add(new ProfileError("Profile 1", "This is the first error message"));
            ProfileErrors.Add(new ProfileError("Profile 1", "This is the second error message"));
            ProfileErrors.Add(new ProfileError("Profile 2", "This is the first error message for the second profile"));
        }
    }
}