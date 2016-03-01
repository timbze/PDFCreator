using System;
using SystemInterface.Microsoft.Win32;
using SystemWrapper.Microsoft.Win32;
using NLog;
using pdfforge.PDFCreator.Core.Printer;
using pdfforge.PDFCreator.Shared.Licensing;

namespace pdfforge.PDFCreator.Helper
{
    internal class PlusHintHelper
    {
        private const string RegistryKeyForHintSettings = @"HKEY_CURRENT_USER\" + SettingsHelper.PDFCREATOR_REG_PATH;
        private const string RegistryKeyForCounter = "LastPlusHintCounter";
        private const string RegistryKeyForDate = "LastPlusHintDate";

        private const int MinNumberOfJobsTillHint = 100;
        private static readonly TimeSpan MinTimeTillHint = TimeSpan.FromDays(14);
        private readonly IPrinterPortReader _portReader;
        private readonly IRegistry _registry;
        private readonly Edition _edition;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PlusHintHelper()
            : this(new PrinterPortReader(), new RegistryWrap(), EditionFactory.Instance.Edition)
        {   }

        public PlusHintHelper(IPrinterPortReader portReader, IRegistry registry, Edition edition)
        {
            _portReader = portReader;
            _registry = registry;
            _edition = edition;
        }

        public int CurrentJobCounter { get; private set; }

        public bool DisplayHint()
        {
            if (!_edition.ShowPlusHint)
                return false;

            var lastJobCounter = GetLastJobCounter();
            var lastDate = GetLastDate();

            CurrentJobCounter = GetCurrentJobCounter();

            var jobDelta = CurrentJobCounter - lastJobCounter;
            var timeDelta = DateTime.Now - lastDate;

            if (jobDelta < MinNumberOfJobsTillHint || timeDelta < MinTimeTillHint)
                return false;

            WriteCurrentDate();
            WriteCounter(CurrentJobCounter);
            return true;
        }

        private DateTime GetLastDate()
        {
            var lastDate = _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForDate, "").ToString();

            if (string.IsNullOrWhiteSpace(lastDate))
            {
                WriteCurrentDate();
                return DateTime.Now;
            }

            DateTime date;
            var success = DateTime.TryParse(lastDate, out date);

            return success ? date : DateTime.Now;
        }

        private int GetLastJobCounter()
        {
            var lastJobCounter = (int) _registry.GetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, 0);

            if (lastJobCounter != 0) return lastJobCounter;

            WriteCounter(lastJobCounter);
            return lastJobCounter;
        }

        private int GetCurrentJobCounter()
        {
            int currentCounter;
            try
            {
                var port = _portReader.ReadPrinterPort("pdfcmon");
                currentCounter = port.JobCounter;
            }
            catch (Exception e)
            {
                currentCounter = 0;
                _logger.Error(e, "Could not read CurrentJobCounter from registry");
            }

            return currentCounter;
        }

        private void WriteCounter(int counter)
        {
            _registry.SetValue(RegistryKeyForHintSettings, RegistryKeyForCounter, counter);
        }

        private void WriteCurrentDate()
        {
            _registry.SetValue(RegistryKeyForHintSettings, RegistryKeyForDate, DateTime.Now);
        }
    }
}