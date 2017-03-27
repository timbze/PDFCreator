using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.Translations
{
     public class StartupTranslation : ITranslatable
     {
          public string NoSupportedGSFound { get; private set; } = "Can't find a supported Ghostscript installation.\r\n\r\nProgram exiting now.";
          public string SpoolerNotRunning { get; private set; } = "The Windows spooler service is not running. Please start the spooler first.\r\n\r\nProgram exiting now.";
          public string UnknownError { get; private set; } = "There was an error while starting PDFCreator. Please reinstall the application. If the error should persist, please contact the support.";
     }
}
