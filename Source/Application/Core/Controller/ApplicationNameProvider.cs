namespace pdfforge.PDFCreator.Core.Controller
{
    public class ApplicationNameProvider
    {
        public ApplicationNameProvider(string applicationName)
        {
            ApplicationName = applicationName;
        }

        public string ApplicationName { get; }
    }
}
