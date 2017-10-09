using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IRetypeFileNameQuery
    {
        QueryResult<string> RetypeFileName(string filename, OutputFormat outputFormat);
    }
}
