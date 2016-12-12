using System.Linq;
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

        public DropboxAction(IDropboxService dropboxService)
        {
            _dropboxService = dropboxService;
        }

        public ActionResult ProcessJob(Job job)
        {
            var sharedLink = job.Profile.DropboxSettings.CreateShareLink;
            var sharedFolder = job.TokenReplacer.ReplaceTokens(job.Profile.DropboxSettings.SharedFolder);

            var accountId = job.Profile.DropboxSettings.AccountId;
            var currentDropBoxAccount = job.Accounts.DropboxAccounts.First(s => s.AccountId.Equals(accountId));

            if (sharedLink)
            {
                var sharedLinks = _dropboxService.UploadFileWithSharing(
                    currentDropBoxAccount.AccessToken,
                    sharedFolder, job.OutputFiles,
                    job.Profile.DropboxSettings.EnsureUniqueFilenames);
                if (sharedLinks != null)
                {
                    // first remove full path from filepath
                    foreach (var item in sharedLinks)
                        item.FilePath = item.FilePath.Split('/').Last();

                    var fullLinks = sharedLinks.Select(item => $"{item.FilePath} ( {item.SharedUrl} )").ToList();
                    var htmlLinks = sharedLinks.Select(item => $"<a href = '{item.SharedUrl}'>{item.FilePath}</a>").ToList();
                    job.TokenReplacer.AddToken(new ListToken("DROPBOXFULLLINKS", fullLinks));
                    job.TokenReplacer.AddToken(new ListToken("DROPBOXHTMLLINKS", htmlLinks));
                }
            }
            else
            {
                _dropboxService.UploadFiles(currentDropBoxAccount.AccessToken, sharedFolder, job.OutputFiles, job.Profile.DropboxSettings.EnsureUniqueFilenames);
            }
            
            return new ActionResult();
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

            var listOfInvalidCharacthers = new[] {'\\', '/', ':', '?', '*', '|', '"', '*', '.'};
            if (profile.DropboxSettings.SharedFolder.IndexOfAny(listOfInvalidCharacthers) != -1)
                return new ActionResult(ErrorCode.Dropbox_InvalidFolderName);

            var accessToken = account.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
                return new ActionResult(ErrorCode.Dropbox_AccessTokenNotSpecified);

            return new ActionResult();
        }
    }
}
