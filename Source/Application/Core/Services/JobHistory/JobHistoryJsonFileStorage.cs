using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Services.JobHistory
{
    public class JobHistoryJsonFileStorage : IJobHistoryStorage
    {
        private readonly IFile _file;
        private readonly string _savePath;

        public JobHistoryJsonFileStorage(IFile file)
        {
            _file = file;

            var historyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PDFCreator");
            _savePath = Path.Combine(historyDir, "PDFCreatorHistory.json");
        }

        public List<HistoricJob> Load()
        {
            try
            {
                var value = _file.ReadAllText(_savePath);
                return JsonConvert.DeserializeObject<List<HistoricJob>>(value)
                    .Where(j => j != null)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<HistoricJob>();
            }
        }

        public void Save(IList<HistoricJob> history)
        {
            try
            {
                var serializedHistory = JsonConvert.SerializeObject(history);
                _file.WriteAllText(_savePath, serializedHistory);
            }
            catch
            {
                //todo: log
            }
        }
    }
}
