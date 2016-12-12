using System;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.ConverterInterface
{
    public interface IConverterFactory
    {
        IConverter GetCorrectConverter(JobType jobType);
    }

    public class ConverterFactory : IConverterFactory
    {
        private readonly IPsConverterFactory _psConverterFactory;

        public ConverterFactory(IPsConverterFactory psConverterFactory)
        {
            _psConverterFactory = psConverterFactory;
        }

        public IConverter GetCorrectConverter(JobType jobType)
        {
            if (jobType == JobType.PsJob)
                return _psConverterFactory.BuildPsConverter();
            throw new NotImplementedException("Only JobType PS is supported so far!");
        }
    }
}