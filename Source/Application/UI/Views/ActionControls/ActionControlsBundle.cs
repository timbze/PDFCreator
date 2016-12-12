using System.Collections.Generic;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    public class ActionControlsBundle
    {
        private readonly List<ActionControl> _list;

        public ActionControlsBundle(
            AttachmentActionControl attachment,
            BackgroundActionControl background,
            CoverActionControl cover,
            EmailClientActionControl emailClient,
            EmailSmtpActionControl emailSmtp,
            FtpActionControl ftp,
            OpenViewerActionControl openViewer,
            PrintActionControl print,
            ScriptActionControl script,
            UserTokenActionControl userTokens,
            DropboxActionControl dropboxActionControl)
        {
            _list = new List<ActionControl>();
            _list.Add(openViewer);
            _list.Add(background);
            _list.Add(cover);
            _list.Add(attachment);
            _list.Add(print);
            _list.Add(dropboxActionControl);
            _list.Add(emailClient);
            _list.Add(emailSmtp);
            _list.Add(script);
            _list.Add(ftp);
            _list.Add(userTokens);
        }

        public IEnumerable<ActionControl> GetActionControls()
        {
            return _list;
        }
    }
}
