using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations
{
     public class PasswordWindowTranslation : ITranslatable
     {
          public string CancelButtonContent { get; private set; } = "_Cancel";
          public string OkButtonContent { get; private set; } = "_OK";
          public string PasswordHintText { get; private set; } = "Leave password empty to get a request during the print job (password will not be saved).";
          public string RemoveButtonContent { get; private set; } = "_Remove";
          public string SkipButtonContent { get; private set; } = "_Skip";
     }
}
