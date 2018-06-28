using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services.JobHistory
{
    public interface IJobHistoryManager
    {
        IList<HistoricJob> History { get; set; }

        event EventHandler HistoryChanged;

        Task Load();

        void Save();

        Task Refresh();

        void Clear();

        void Add(Job job);

        HistoricJob TransformToHistoricJob(Job job);

        void Remove(HistoricJob historicJob);
    }
}
