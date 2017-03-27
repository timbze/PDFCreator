using System.Collections.Generic;
using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.UI.Interactions;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class DropboxSharedLinksProvider : IDropboxSharedLinksProvider
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DropboxSharedLinksProvider(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }
        
        public void ShowSharedLinks(DropboxFileMetaData sharedLinks)
        {
            _logger.Debug("DropboxSharedLinks started");
            var interaction = new DropboxSharedLinksInteraction(sharedLinks);
            _interactionInvoker.Invoke(interaction);
        }
    }
}
