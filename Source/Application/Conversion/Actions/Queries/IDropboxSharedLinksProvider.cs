using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;

namespace pdfforge.PDFCreator.Conversion.Actions.Queries
{
    public interface IDropboxSharedLinksProvider
    {
        void ShowSharedLinks(DropboxFileMetaData sharedLink);
    }
}