using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    /// <summary>
    ///     The JobInfo class holds all information about the job, like the source files, metadata and such.
    /// </summary>
    public class JobInfo
    {
        public string InfFile { get; set; }

        public IList<SourceFileInfo> SourceFiles { get; set; } = new List<SourceFileInfo>();

        /// <summary>
        ///     Sum of TotalPages of all SourceFiles
        /// </summary>
        public int TotalPages => CalculateTotalPages();

        public Metadata Metadata { get; set; }
        public JobType JobType { get; set; }

        private int CalculateTotalPages()
        {
            var pages = 0;
            if (SourceFiles == null)
                return pages;

            foreach (var sfi in SourceFiles)
                pages += sfi.TotalPages;

            return pages;
        }
    }
}