using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
     public class TokenPlaceHoldersTranslation : ITranslatable
     {
          public string KeywordsFromSettings { get; private set; } = "keyword 1 keyword 2";
          public string MyFileDocx { get; private set; } = "MyFile.docx";
          public string OutputFilename { get; private set; } = "OutputFilename.jpg";
          public string OutputFilename2 { get; private set; } = "OutputFilename2.jpg";
          public string OutputFilename3 { get; private set; } = "OutputFilename3.jpg";
          public string SubjectFromSettings { get; private set; } = "Subject from Settings";
          public string TitleFromPrintJob { get; private set; } = "Title from Printjob";
          public string TitleFromSettings { get; private set; } = "Title from Settings";
     }
}
