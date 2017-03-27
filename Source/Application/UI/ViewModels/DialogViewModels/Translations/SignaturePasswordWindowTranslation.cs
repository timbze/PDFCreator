using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations
{
    public class SignaturePasswordWindowTranslation : ITranslatable
    {
        public string Cancel { get; private set; } = "Cancel";
        public string Remove { get; private set; } = "Remove";
        public string LeavePasswordEmpty { get; private set; } = "Leave password empty to get a request during the print job (password will not be saved).";
        public string OK { get; private set; } = "OK";
        public string SignaturePassword { get; private set; } = "Signature _password:";
        public string Skip { get; private set; } = "Skip";
    }
}