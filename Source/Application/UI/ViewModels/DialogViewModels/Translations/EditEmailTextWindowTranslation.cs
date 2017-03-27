using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations
{
     public class EditEmailTextWindowTranslation : ITranslatable
     {
          public string AddBodyTokenText { get; private set; } = "Add Token:";
          public string AddSubjectTokenText { get; private set; } = "Add Token:";
          public string AttachSignatureText { get; private set; } = "Attach pdfforge signature";
          public string BodyPreviewText { get; private set; } = "Preview:";
          public string BodyTextLabelContent { get; private set; } = "_Text:";
          public string CancelButtonContent { get; private set; } = "_Cancel";
          public string OkButtonContent { get; private set; } = "_OK";
          public string SubjectLabelContent { get; private set; } = "_Subject:";
          public string SubjectPreviewText { get; private set; } = "Preview:";
          public string Title { get; private set; } = "Edit E-Mail Text";
          public string UseHtml { get; private set; } = "Use Html";
     }
}
