using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class DropboxInteraction : IInteraction
    {
        public string AccessToken { get; set; }

        public string AccountId { get; set; }

        public string AccountInfo { get; set; }

        public bool Success { get; set; }
    }

}
