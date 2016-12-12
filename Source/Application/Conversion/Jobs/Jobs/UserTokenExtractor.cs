using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public class UserTokenExtractor : IUserTokenExtractor
    {
        private readonly IPsParserFactory _psParserFactory;

        public UserTokenExtractor(IPsParserFactory psParserFactory)
        {
            _psParserFactory = psParserFactory;
        }

        public UserToken ExtractUserTokenFromPsFile(string psFile)
        {
            var psParser = _psParserFactory.BuildPsParser(psFile);
            psParser.Analyse();
            psParser.RemoveParameterLinesFromPSFile();
            psParser.CloseStream();

            var userToken = new UserToken();
            if (psParser.UserTokens == null)
                return userToken;

            foreach (var ut in psParser.UserTokens)
                userToken.AddKeyValuePair(ut.Key, ut.Value);

            return userToken;
        }
    }
}
