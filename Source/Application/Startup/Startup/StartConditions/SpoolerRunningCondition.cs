﻿using System;
using System.ComponentModel;
using System.ServiceProcess;
using NLog;
 using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class SpoolerRunningCondition : IStartupCondition
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITranslator _translator;

        public SpoolerRunningCondition(ITranslator translator)
        {
            _translator = translator;
        }

        public StartupConditionResult Check()
        {
            if (SpoolerIsRunning())
                return StartupConditionResult.BuildSuccess();

            return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.SpoolerNotRunning, _translator.GetTranslation("Startup", "SpoolerNotRunning"));
        }

        private bool SpoolerIsRunning()
        {
            try
            {
                var spoolerController = new ServiceController("spooler");
                return spoolerController.Status == ServiceControllerStatus.Running;
            }
            catch (InvalidOperationException ex)
            {
                var win32Exception = ex.InnerException as Win32Exception;
                if (win32Exception?.NativeErrorCode == 5)
                {
                    _logger.Warn(ex, "Could not check spooler status: We do not have sufficient privileges.");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not check spooler status");
                return false;
            }
        }
    }
}