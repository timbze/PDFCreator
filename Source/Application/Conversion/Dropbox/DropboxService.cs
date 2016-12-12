using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropbox.Api;
using Dropbox.Api.Files;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;

namespace pdfforge.PDFCreator.Conversion.Dropbox
{
    public class DropboxService : IDropboxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public string AccessToken = string.Empty;

        public Uri GetAuthorizeUri(string appKey, string redirectUri)
        {
            return DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, appKey, new Uri(redirectUri), string.Empty);
        }

        public string ParseAccessToken(Uri uri)
        {
            if (IsErrorInUrl(uri))
                throw new ArgumentException(nameof(uri));
            // make handling if bereaer token is not reachable
            var result = DropboxOAuth2Helper.ParseTokenFragment(uri);

            return AccessToken = result.AccessToken;
        }

        public bool UploadFiles(string accessToken, string folder, IEnumerable<string> listOfFilePaths, bool ensureUniqueFilenames)
        {
            try
            {
                var dbxClient = MakeInstanceOfClient(accessToken);
                foreach (var pathOfCurrentItem in listOfFilePaths)
                {
                    var currentFileName = Path.GetFileName(pathOfCurrentItem);

                    // path to upload is just /FileName
                    var pathToUpload = "/" + currentFileName;
                    // if folder to upload is not empty add it to beginning of pathToUpload. 
                    //If it doesnt exists dropbox will create it
                    if (!string.IsNullOrEmpty(folder))
                        pathToUpload = "/" + folder + pathToUpload;
                    using (var mem = GetFileStream(pathOfCurrentItem))
                    {
                        if (ensureUniqueFilenames)
                            dbxClient.Files.UploadAsync(pathToUpload, WriteMode.Add.Instance, true, body: mem).Wait();
                        else
                            dbxClient.Files.UploadAsync(pathToUpload, WriteMode.Overwrite.Instance, true, body: mem).Wait();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // take a look processing exception and make handler here for it
                Logger.Error("Exception while uploading files to DropBox:\r\n" + ex.Message);

                return false;
            }
        }

        public List<DropboxFileMetaData> UploadFileWithSharing(string accessToken, string folder, IEnumerable<string> listOfFilePaths, bool ensureUniqueFilenames)
        {
            try
            {
                var result = new List<DropboxFileMetaData>();
                var dbxClient = MakeInstanceOfClient(accessToken);
                foreach (var pathOfCurrentItem in listOfFilePaths)
                {
                    var currentFileName = Path.GetFileName(pathOfCurrentItem);
                    var pathToUpload = "/" + currentFileName;
                    // if folder to upload is not empty add it to beginning of pathToUpload. 
                    if (!string.IsNullOrEmpty(folder))
                        pathToUpload = "/" + folder + pathToUpload;
                    using (var mem = GetFileStream(pathOfCurrentItem))
                    {
                        FileMetadata uploadFileMethaData;
                        if (ensureUniqueFilenames)
                            uploadFileMethaData = dbxClient.Files.UploadAsync(pathToUpload, WriteMode.Add.Instance, true, body: mem).Result;
                        else
                            uploadFileMethaData = dbxClient.Files.UploadAsync(pathToUpload, WriteMode.Overwrite.Instance, true, body: mem).Result;
                        var sharedLinkExists = dbxClient.Sharing.ListSharedLinksAsync(uploadFileMethaData.PathDisplay).Result;
                        if (sharedLinkExists.Links.Any())
                        {
                            foreach (var item in sharedLinkExists.Links)
                                result.Add(new DropboxFileMetaData {FilePath = uploadFileMethaData.PathDisplay, SharedUrl = item.Url});
                        }
                        else
                        {
                            var sharedLink = dbxClient.Sharing.CreateSharedLinkWithSettingsAsync(uploadFileMethaData.PathDisplay).Result;
                            result.Add(new DropboxFileMetaData {FilePath = uploadFileMethaData.PathDisplay, SharedUrl = sharedLink.Url});
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while uploading and sharing file to DropBox:\r\n" + ex.Message);
                return new List<DropboxFileMetaData>();
            }
        }

        public DropboxUserInfo GetDropUserInfo()
        {
            var currentUser = MakeInstanceOfClient(AccessToken).Users.GetCurrentAccountAsync().Result;
            if (currentUser != null)
                return new DropboxUserInfo
                {
                    AccessToken = AccessToken,
                    AccountId = currentUser.AccountId,
                    AccountInfo = currentUser.Email + " - " + currentUser.Name.DisplayName
                };
            return new DropboxUserInfo();
        }

        private FileStream GetFileStream(string fileUri)
        {
            return new FileStream(fileUri, FileMode.Open, FileAccess.Read);
        }

        private DropboxClient MakeInstanceOfClient(string accessToken)
        {
            return new DropboxClient(accessToken);
        }

        private bool IsErrorInUrl(Uri eUri)
        {
            var fragment = eUri.Fragment;
            if (string.IsNullOrWhiteSpace(fragment))
                throw new ArgumentException("The supplied uri doesn't contain a fragment", "redirectedUri");
            fragment = fragment.Trim('#');

            var errorDescription = string.Empty;
            var error = string.Empty;
            foreach (var pair in fragment.Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                    continue;

                switch (elements[0])
                {
                    case "error_description":
                        errorDescription = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "error":
                        error = Uri.UnescapeDataString(elements[1]);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(errorDescription) && !string.IsNullOrEmpty(error))
            {
                Logger.Error("Dropbox error: Error_description = " + errorDescription + " Error = " + error);
                return true;
            }
            return false;
        }
    }
}
