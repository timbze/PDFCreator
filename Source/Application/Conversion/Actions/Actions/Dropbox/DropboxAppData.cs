namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox
{
    public class DropboxAppData
    {
        public readonly  string AppKey;

        public readonly string RedirectUri;

        public DropboxAppData(string appKey, string redirectUri)
        {
            AppKey = appKey;
            RedirectUri = redirectUri;
        }
    }
}