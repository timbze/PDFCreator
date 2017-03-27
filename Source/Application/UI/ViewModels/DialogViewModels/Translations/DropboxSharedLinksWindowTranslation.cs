using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations
{
     public class DropboxSharedLinksWindowTranslation : ITranslatable
     {
          public string CopyButtonContent { get; private set; } = "Copy";
          public string DropboxLinksToCliboardText { get; private set; } = "Links copied successfully";
          public string DropboxSharedLinksNoteText { get; private set; } = "Please note: Everyone who recieves this link will be able to access the file";
          public string DropboxShareLinksSubTitleText { get; private set; } = "The following links have been created for your document:";
          public string OkButtonContent { get; private set; } = "_OK";
          public string Title { get; private set; } = "Dropbox";
     }
}
