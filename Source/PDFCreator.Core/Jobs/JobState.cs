using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pdfforge.PDFCreator.Core.Jobs
{
    public enum JobState
    {
        Pending,
        Cancelled,
        Failed,
        Succeeded
    }
}
