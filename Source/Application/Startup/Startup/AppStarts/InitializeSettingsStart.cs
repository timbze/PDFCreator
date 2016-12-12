using System;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class InitializeSettingsStart : AppStartBase
    {
        private readonly ISettingsManager _settingsManager;

        public InitializeSettingsStart(ICheckAllStartupConditions checkAllStartupConditions, ISettingsManager settingsManager)
            : base(checkAllStartupConditions)
        {
            _settingsManager = settingsManager;
        }

        public override ExitCode Run()
        {
            try
            {
                _settingsManager.LoadAllSettings();
                _settingsManager.SaveCurrentSettings();
            }
            catch (Exception)
            {
                return ExitCode.Unknown;
            }
                
            return ExitCode.Ok;
        }
    }
}