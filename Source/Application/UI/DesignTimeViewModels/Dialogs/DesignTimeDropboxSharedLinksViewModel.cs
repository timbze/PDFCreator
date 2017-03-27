using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeDropboxSharedLinksViewModel : DropboxSharedLinksViewModel
    {
        public DesignTimeDropboxSharedLinksViewModel() : base(new DropboxSharedLinksWindowTranslation(), new ProcessStarter())
        {
            var interaction = new DropboxSharedLinksInteraction(new DropboxFileMetaData());
            SetInteraction(interaction);
            SharedLink = new DropboxFileMetaData();
            var link = new DropboxFileMetaData()
            {
                SharedUrl = "https://www.dropbox.com/test/SharedFile.pdf"
            };
            SharedLink = link;
        }
    }
}