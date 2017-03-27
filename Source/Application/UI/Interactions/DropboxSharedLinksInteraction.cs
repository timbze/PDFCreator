using System.Collections.Generic;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class DropboxSharedLinksInteraction : IInteraction
    {
        public DropboxSharedLinksInteraction(DropboxFileMetaData sharedLink)
        {
            SharedLink = sharedLink;
        }

        public DropboxFileMetaData SharedLink { get; set; }
        public bool Success { get; set; }

        public bool CopySucessfull { get; set; }
    }
    }