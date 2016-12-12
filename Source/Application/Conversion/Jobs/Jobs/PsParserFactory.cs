using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pdfforge.PsParser;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public interface IPsParserFactory
    {
        IPsParser BuildPsParser(string psFile);
    }

    public class PsParserFactory : IPsParserFactory
    {
        public IPsParser BuildPsParser(string psFile)
        {
            return new PsParser.PsParser(psFile);
        }
    }
}
