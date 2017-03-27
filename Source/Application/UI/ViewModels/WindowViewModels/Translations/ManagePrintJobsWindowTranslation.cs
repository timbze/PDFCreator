using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
     public class ManagePrintJobsWindowTranslation : ITranslatable
     {
          public string ContinueButtonContent { get; private set; } = "Continue";
          public string DeleteButtonContent { get; private set; } = "Delete";
          public string FilesColumnHeader { get; private set; } = "Files";
          public string MergeAllButtonContent { get; private set; } = "Merge All";
          public string MergeButtonContent { get; private set; } = "Merge";
          public string PagesColumnHeader { get; private set; } = "Pages";
          public string Title { get; private set; } = "Manage Print Jobs";
          public string TitleColumnHeader { get; private set; } = "Title";
     }
}
