using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public class DropboxAction : IAction, ICheckable
    {
        private readonly IDropboxService _dropboxService;
        private readonly IDropboxSharedLinksProvider _dropboxSharedLinksProvider;

        public DropboxAction(IDropboxService dropboxService, IDropboxSharedLinksProvider dropboxSharedLinksProvider)
        {
            _dropboxService = dropboxService;
            _dropboxSharedLinksProvider = dropboxSharedLinksProvider;
        }

        public ActionResult ProcessJob(Job job)
        {

            var sharedLink = job.Profile.DropboxSettings.CreateShareLink;
            var sharedFolder = job.TokenReplacer.ReplaceTokens(job.Profile.DropboxSettings.SharedFolder);

            var accountId = job.Profile.DropboxSettings.AccountId;
            var currentDropBoxAccount = job.Accounts.DropboxAccounts.First(s => s.AccountId.Equals(accountId));
            if (sharedLink)
            {
                try
                {
                    var sharedLinks = _dropboxService.UploadFileWithSharing(
                        currentDropBoxAccount.AccessToken,
                        sharedFolder, job.OutputFiles,
                        job.Profile.DropboxSettings.EnsureUniqueFilenames, job.OutputFilenameTemplate);
                    if (sharedLinks  == null)
                    {
                        return new ActionResult(ErrorCode.Dropbox_Upload_And_Share_Error);
                    }
                    sharedLinks.FilePath = sharedLinks.FilePath.Split('/').Last();
                    job.TokenReplacer.AddToken(new StringToken("DROPBOXFULLLINKS", $"{sharedLinks.FilePath} ( {sharedLinks.SharedUrl} )"));
                    job.TokenReplacer.AddToken(new StringToken("DROPBOXHTMLLINKS", $"<a href = '{sharedLinks.SharedUrl}'>{sharedLinks.FilePath}</a>"));
                    if (job.Profile.AutoSave.Enabled == false)
                    {
                        ShowWindowWithSharedLinks(sharedLinks);
                    }
                }
                catch
                {
                    return new ActionResult(ErrorCode.Dropbox_Upload_And_Share_Error);
                }
            }
            else
            {
                var result = _dropboxService.UploadFiles(currentDropBoxAccount.AccessToken, sharedFolder, job.OutputFiles, job.Profile.DropboxSettings.EnsureUniqueFilenames, job.OutputFilenameTemplate);
                if (result == false)
                {
                    return new ActionResult(ErrorCode.Dropbox_Upload_Error);
                }
            }
           

            return new ActionResult();
        }

        private void ShowWindowWithSharedLinks(DropboxFileMetaData sharedLink)
        {
            _dropboxSharedLinksProvider.ShowSharedLinks(sharedLink);

        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.DropboxSettings.Enabled;
        }

        public bool Init(Job job)
        {
            return true;
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts)
        {
            if (!profile.DropboxSettings.Enabled)
                return new ActionResult();

            var account = accounts.DropboxAccounts.FirstOrDefault(
                s => s.AccountId.Equals(profile.DropboxSettings.AccountId));

            if (account == null)
                return new ActionResult(ErrorCode.Dropbox_AccountNotSpecified);

            var listOfInvalidCharacthers = new[] { '\\', '/', ':', '?', '*', '|', '"', '*', '.' };
            if (profile.DropboxSettings.SharedFolder.IndexOfAny(listOfInvalidCharacthers) != -1)
                return new ActionResult(ErrorCode.Dropbox_InvalidFolderName);

            var accessToken = account.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
                return new ActionResult(ErrorCode.Dropbox_AccessTokenNotSpecified);

            return new ActionResult();
        }
    }
}