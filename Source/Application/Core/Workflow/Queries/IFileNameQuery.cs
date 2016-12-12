using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IFileNameQuery
    {
        QueryResult<string> GetFileName(Job job);
    }
}