using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants.Translations
{
    public class PrintFilesTranslation : ITranslatable
    {
        private string AndXMore { get; set; } = "and {0} more.";
        public string DirectoriesNotSupported { get; private set; } = "You have tried to convert directories here. This is currently not supported.";
        public string NotPrintableFiles { get; private set; } = "The following files can't be converted:";
        public string ProceedAnyway { get; private set; } = "Do you want to proceed anyway?";
        public string AskSwitchDefaultPrinter { get; private set; } = "PDFCreator needs to temporarily change the default printer to be able to convert the file. Do you want to proceed?";

        public string GetAndxMoreMessage(int countOfFiles)
        {
            return string.Format(AndXMore, countOfFiles);
        }
    }
}
