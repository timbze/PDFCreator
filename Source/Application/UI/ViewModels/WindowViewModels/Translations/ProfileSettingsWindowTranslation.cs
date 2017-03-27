using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
     public class ProfileSettingsWindowTranslation : ITranslatable
     {
          public string ActionsTabText { get; private set; } = "Actions";
          public string AutoSaveTabText { get; private set; } = "Auto-Save";
          public string CancelButtonContent { get; private set; } = "Cancel";
          public string DeleteProfile { get; private set; } = "Delete profile";
          public string DeleteProfileWithMappedPrinter { get; private set; } = "The profile '{0}' is assigned to the printer '{1}'. If you delete this profile, the printer will use the default profile instead. Do you want to proceed?";
          public string DocumentTabText { get; private set; } = "Document";
          public string EnterProfileName { get; private set; } = "Please enter profile name:";
          //public string FontFileNotSupported { get; private set; } = "The selected font is not supported. Please select a different font.";
          public string HelpButtonContent { get; private set; } = "Help";
          public string ImageFormatsTabText { get; private set; } = "Image Formats";
          public string InvalidProfileName { get; private set; } = "The entered profile name is invalid or does already exist.";
          public string NewProfile { get; private set; } = "New Profile";
          public string PdfTabText { get; private set; } = "PDF";
          public string ProfileHasPrinterTitle { get; private set; } = "Printer assignment";
          public string ProfileLabelText { get; private set; } = "Profile:";
          public string ProfileName { get; private set; } = "Profile name";
          public string ReallyDeleteProfile { get; private set; } = "Do you really want to the delete '{0}'?";
          public string ReallyWantToCancel { get; private set; } = "Do you really want to cancel the settings and discard the changes?";
          public string SaveButtonContent { get; private set; } = "Save";
          public string SaveTabText { get; private set; } = "Save";
          public string Title { get; private set; } = "PDFCreator Settings";
          public string TitleText { get; private set; } = "Profile Settings";
          public string UnsavedChanges { get; private set; } = "Unsaved changes";

         public string GetReallyDeleteProfileFormattedTranslation(string currentProfileName)
         {
             return string.Format(ReallyDeleteProfile, currentProfileName);
         }

         public string GetDeleteProfileWithMappedPrinterFormattedTranslation(string profileName, string printerName)
         {
            return string.Format(DeleteProfileWithMappedPrinter, profileName, printerName);
        }
    }
}
