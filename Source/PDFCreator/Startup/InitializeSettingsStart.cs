using pdfforge.PDFCreator.Helper;

namespace pdfforge.PDFCreator.Startup
{
    internal class InitializeSettingsStart : IAppStart
    {
        public bool Run()
        {
            SettingsHelper.SaveSettings();

            return true;
        }
    }
}
