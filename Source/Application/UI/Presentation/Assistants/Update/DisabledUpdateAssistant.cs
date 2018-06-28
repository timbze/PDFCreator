using System;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class DisabledUpdateAssistant : IUpdateAssistant
    {
        public ApplicationVersion OnlineVersion => new ApplicationVersion(new Version(0, 0, 0, 0), "", "");
        public bool UpdateProcedureIsRunning => false;
        public bool UpdatesEnabled => false;

        public void UpdateProcedure(bool checkNecessity)
        {
        }

        public bool ShowUpdate => false;

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
            return false;
        }

        public bool IsUpdateAvailable()
        {
            return false;
        }
    }
}
