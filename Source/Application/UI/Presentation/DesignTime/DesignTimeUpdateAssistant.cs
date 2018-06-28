using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Assistants.Update;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateAssistant : IUpdateAssistant
    {
        public ApplicationVersion OnlineVersion => new ApplicationVersion(new Version(1, 2, 3, 4), "http://download.local", "12345");
        public bool UpdateProcedureIsRunning => false;
        public bool UpdatesEnabled => true;

        public void UpdateProcedure(bool checkNecessity)
        {
        }

        public bool ShowUpdate => true;

        public void InstallNewUpdate()
        {
        }

        public void SkipVersion()
        {
        }

        public void SetNewUpdateTime()
        {
        }

        public bool IsOnlineUpdateAvailable()
        {
            return true;
        }

        public bool IsUpdateAvailable()
        {
            return true;
        }
    }
}
