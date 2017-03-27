using System;
using System.Collections.Generic;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class TokenHelper
    {
        private readonly TokenPlaceHoldersTranslation _translation;
        private TokenReplacer _tokenReplacer;

        public TokenHelper(TokenPlaceHoldersTranslation translation)
        {
            _translation = translation;
        }

        public TokenReplacer TokenReplacerWithPlaceHolders
        {
            get { return _tokenReplacer ?? (_tokenReplacer = CreateTokenReplacerWithPlaceHolders()); }
        }

        private TokenReplacer CreateTokenReplacerWithPlaceHolders()
        {
            var tr = new TokenReplacer();

            tr.AddToken(new StringToken("Author", Environment.UserName));
            tr.AddToken(new StringToken("PrintJobAuthor", Environment.UserName));
            tr.AddToken(new StringToken("ClientComputer", Environment.MachineName));
            tr.AddToken(new StringToken("ComputerName", Environment.MachineName));
            tr.AddToken(new NumberToken("Counter", 1234));
            tr.AddToken(new DateToken("DateTime", DateTime.Now));
            tr.AddToken(new StringToken("InputFilename", _translation.MyFileDocx));
            tr.AddToken(new StringToken("InputFilePath", @"C:\Temp"));
            tr.AddToken(new NumberToken("JobID", 1));
            tr.AddToken(new NumberToken("NumberOfCopies", 1));
            tr.AddToken(new NumberToken("NumberOfPages", 1));
            tr.AddToken(new ListToken("OutputFilenames",
                new[]
                {
                    _translation.OutputFilename
                    , _translation.OutputFilename2
                    , _translation.OutputFilename3
                }));
            tr.AddToken(new StringToken("OutputFilePath", @"C:\Temp"));
            tr.AddToken(new StringToken("PrinterName", "PDFCreator"));
            tr.AddToken(new NumberToken("SessionID", 0));
            tr.AddToken(new StringToken("Title", _translation.TitleFromSettings));
            tr.AddToken(new StringToken("PrintJobName", _translation.TitleFromPrintJob));
            tr.AddToken(new StringToken("Username", Environment.UserName));
            tr.AddToken(new StringToken("Subject", _translation.SubjectFromSettings));
            tr.AddToken(new StringToken("Keywords", _translation.KeywordsFromSettings));
            tr.AddToken(new StringToken("DropboxHtmlLinks", "<a href=\"https://dropbox.com/link1\">File.pdf</a>"));
            tr.AddToken(new StringToken("DropboxFullLinks", "File.pdf ( https://dropbox.com/link1 )"));
            tr.AddToken(new EnvironmentToken());

            return tr;
        }

        public List<string> GetTokenListWithFormatting()
        {
            var tokenList = new List<string>();
            tokenList.AddRange(TokenReplacerWithPlaceHolders.GetTokenNames());
            tokenList.Sort();
            tokenList.Insert(tokenList.IndexOf("<DateTime>") + 1, "<DateTime:yyyyMMddHHmmss>");
            tokenList.Insert(tokenList.IndexOf("<Environment>") + 1, "<Environment:UserName>");

            return tokenList;
        }

        public List<string> GetTokenListForAuthor()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Remove("<Author>");
            tokenList.Remove("<Title>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<InputFilename>");
            tokenList.Remove("<InputFilePath>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<Subject>");
            tokenList.Remove("<Keywords>");
            tokenList.Remove("<DropboxHtmlLinks>");
            tokenList.Remove("<DropboxFullLinks>");
            return tokenList;
        }

        public List<string> GetTokenListForTitle()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Remove("<Title>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<InputFilePath>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<Subject>");
            tokenList.Remove("<Keywords>");
            tokenList.Remove("<DropboxHtmlLinks>");
            tokenList.Remove("<DropboxFullLinks>");
            return tokenList;
        }

        public List<string> GetTokenListForFilename()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<InputFilePath>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<DropboxHtmlLinks>");
            tokenList.Remove("<DropboxFullLinks>");

            return tokenList;
        }

        public List<string> GetTokenListForDirectory()

        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<DropboxHtmlLinks>");
            tokenList.Remove("<DropboxFullLinks>");

            return tokenList;
        }

        public List<string> GetTokenListForEmail()
        {
            var tokenList = GetTokenListWithFormatting();

            tokenList.Insert(tokenList.IndexOf("<OutputFilePath>") + 1, "<OutputFilenames:, >");
            tokenList.Insert(tokenList.IndexOf("<OutputFilePath>") + 2, "<OutputFilenames:\\r\\n>");

            tokenList.Remove("<OutputFilePath>");
            return tokenList;
        }

        public List<string> GetTokenListForSubjectAndKeyword()
        {
            var tokenList = GetTokenListWithFormatting();
            tokenList.Remove("<Title>");
            tokenList.Remove("<Author>");
            tokenList.Remove("<OutputFilenames>");
            tokenList.Remove("<InputFilename>");
            tokenList.Remove("<InputFilePath>");
            tokenList.Remove("<OutputFilePath>");
            tokenList.Remove("<Subject>");
            tokenList.Remove("<Keywords>");
            tokenList.Remove("<DropboxHtmlLinks>");
            tokenList.Remove("<DropboxFullLinks>");
            return tokenList;
        }

        /// <summary>
        ///     Detection if string contains insecure tokens, like NumberOfPages, InputFilename or InputFilePath
        /// </summary>
        public bool ContainsInsecureTokens(string textWithTokens)
        {
            if (Contains_IgnoreCase(textWithTokens, "<NumberOfPages>"))
                return true;
            if (Contains_IgnoreCase(textWithTokens, "<InputFilename>"))
                return true;
            if (Contains_IgnoreCase(textWithTokens, "<InputFilePath>"))
                return true;

            return false;
        }

        private bool Contains_IgnoreCase(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}