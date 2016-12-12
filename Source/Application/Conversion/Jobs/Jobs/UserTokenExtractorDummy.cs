using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public class UserTokenExtractorDummy : IUserTokenExtractor
    {
        public UserToken ExtractUserTokenFromPsFile(string psFile)
        {
            return new UserToken();
        }
    }
}
