using NLog;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    public class GhostScriptPrinter : IJobPrinter
    {
        private readonly IGhostscriptDiscovery _ghostscriptDiscovery;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GhostScriptPrinter(IGhostscriptDiscovery ghostscriptDiscovery)
        {
            _ghostscriptDiscovery = ghostscriptDiscovery;
        }

        public void Print(Job job)
        {
            var gsVersion = _ghostscriptDiscovery.GetGhostscriptInstance();

            if (gsVersion == null)
            {
                _logger.Error("No valid Ghostscript version found.");
                throw new InvalidOperationException("No valid Ghostscript version found.");
            }
            _logger.Debug("Ghostscript Version: " + gsVersion.Version + " loaded from " + gsVersion.ExePath);
            var ghostscript = new GhostScript(gsVersion);

            OutputDevice printingDevice = new PrintingDevice(job);
            ghostscript.Run(printingDevice, job.JobTempFolder);
        }
    }
}
