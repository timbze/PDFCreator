using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NLog;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class DropboxAuthenticationWindowViewModel : InteractionAwareViewModelBase<DropboxInteraction>
    {

        private readonly IDropboxService _dropboxService;
        private readonly DropboxAppData _dropboxData;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public DropboxAuthenticationWindowViewModel(IDropboxService dropboxService, DropboxAppData dropboxAppData)
        {
            _dropboxService = dropboxService;
            _dropboxData = dropboxAppData;

            CancelDialogCommand = new DelegateCommand(CancelInputDialog);
        }

        protected override void HandleInteractionObjectChanged()
        {
        }

        private void CancelInputDialog(object o)
        {
            Interaction.Success = false;
            FinishInteraction();
        }

        public void SetAccessTokenAndUserInfo(Uri eUri)
        {
            // if url doesnt start with redirecturl that means that current url for recieveing token isn't reach yet
            if (eUri.ToString().StartsWith(_dropboxData.RedirectUri, StringComparison.OrdinalIgnoreCase) == false)
                return;

            try
            {
                _dropboxService.ParseAccessToken(eUri);
                var currentUserInfo = _dropboxService.GetDropUserInfo();
                Interaction.AccessToken = currentUserInfo.AccessToken;
                Interaction.AccountInfo = currentUserInfo.AccountInfo;
                Interaction.AccountId = currentUserInfo.AccountId;
                RefreshBrowser = true;
                Interaction.Success = true;
            }
            catch (ArgumentException)
            {
                _logger.Info("Cannot parse access token.");
            }
            FinishInteraction();


        }


        public Uri GetAuthorizeUri()
        {
            return _dropboxService.GetAuthorizeUri( _dropboxData.AppKey, _dropboxData.RedirectUri);
        }

        public DelegateCommand CancelDialogCommand { get; }
        public bool RefreshBrowser { get; private set; }


    }
}
