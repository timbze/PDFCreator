using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations
{
     public class DropboxAuthenticationWindowTranslation : ITranslatable
     {
          public string CancelAuthenticationButtonContent { get; private set; } = "Cancel";
          public string DropboxAuthenticationTitleText { get; private set; } = "Authenticate with Dropbox";
          public string Title { get; private set; } = "Dropbox";
     }
}
