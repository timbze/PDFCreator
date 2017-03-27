using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
     public class RecommendPdfArchitectWindowTranslation : ITranslatable
     {
          public string DownloadButtonContent { get; private set; } = "Download";
          public string ErrorText { get; private set; } = "No application is associated to open PDF files.";
          public string InfoButtonContent { get; private set; } = "More info";
          public string NoButtonContent { get; private set; } = "No, thanks";
          public string RecommendText { get; private set; } = "We recommend PDF Architect, our free PDF Viewer and Editor.\r\nThe free edition contains a viewer and basic editing capabilities. Furthermore, you can buy optional modules, which extend the functionality according to your requirements and even provide OCR.";
     }
}
