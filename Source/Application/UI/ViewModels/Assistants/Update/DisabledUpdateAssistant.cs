namespace pdfforge.PDFCreator.UI.ViewModels.Assistants.Update
{
    public class DisabledUpdateAssistant : IUpdateAssistant
    {
        public bool UpdateProcedureIsRunning => false;
        public bool UpdatesEnabled => false;
        public void UpdateProcedure(bool checkNecessity, bool onlyNotifyOnNewUpdate)
        {
            
        }
    }
}