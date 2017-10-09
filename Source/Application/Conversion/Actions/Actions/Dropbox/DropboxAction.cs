using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public class DropboxAction : IAction, ICheckable
    {
        private readonly IDropboxService _dropboxService;

        public DropboxAction(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

        public ActionResult ProcessJob(Job job)
        {
            var sharedLink = job.Profile.DropboxSettings.CreateShareLink;
            var shareFolder = job.TokenReplacer.ReplaceTokens(job.Profile.DropboxSettings.SharedFolder);
            shareFolder = shareFolder.Replace("\\", "/");

            var currentDropBoxAccount = job.Accounts.GetDropboxAccount(job.Profile);
            if (sharedLink)
            {
                try
                {
                    var shareLink = _dropboxService.UploadFileWithSharing(
                        currentDropBoxAccount.AccessToken,
                        shareFolder, job.OutputFiles,
                        job.Profile.DropboxSettings.EnsureUniqueFilenames, job.OutputFilenameTemplate);
                    if (shareLink == null)
                    {
                        return new ActionResult(ErrorCode.Dropbox_Upload_And_Share_Error);
                    }

                    job.ShareLinks.DropboxShareUrl = shareLink.ShareUrl;

                    shareLink.Filename = shareLink.Filename.Split('/').Last();
                    job.TokenReplacer.AddToken(new StringToken("DROPBOXFULLLINKS", $"{shareLink.Filename} ( {shareLink.ShareUrl} )"));
                    job.TokenReplacer.AddToken(new StringToken("DROPBOXHTMLLINKS", $"<a href = '{shareLink.ShareUrl}'>{shareLink.Filename}</a>"));
                }
                catch
                {
                    return new ActionResult(ErrorCode.Dropbox_Upload_And_Share_Error);
                }
            }
            else
            {
                var result = _dropboxService.UploadFiles(currentDropBoxAccount.AccessToken, shareFolder, job.OutputFiles, job.Profile.DropboxSettings.EnsureUniqueFilenames, job.OutputFilenameTemplate);
                if (result == false)
                {
                    return new ActionResult(ErrorCode.Dropbox_Upload_Error);
                }
            }

            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.DropboxSettings.Enabled;
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts)
        {
            if (!profile.DropboxSettings.Enabled)
                return new ActionResult();

            var account = accounts.GetDropboxAccount(profile);

            if (account == null)
                return new ActionResult(ErrorCode.Dropbox_AccountNotSpecified);

            var listOfInvalidCharacthers = new[] { ':', '?', '*', '|', '"', '*', '.' };
            if (profile.DropboxSettings.SharedFolder.IndexOfAny(listOfInvalidCharacthers) != -1)
                return new ActionResult(ErrorCode.Dropbox_InvalidFolderName);

            var accessToken = account.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
                return new ActionResult(ErrorCode.Dropbox_AccessTokenNotSpecified);

            return new ActionResult();
        }
    }
}
