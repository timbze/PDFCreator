using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class DropboxAccountInteraction : AccountInteractionBase
    {
        public DropboxAccount DropboxAccount { get; set; }
        public DropboxAccountInteractionResult Result { get; set; }

        /// <summary>
        /// Do NOT use. Use Result!
        /// </summary>
        public override bool Success
        {
            get { return Result == DropboxAccountInteractionResult.Success; }
            set { Result = value ? DropboxAccountInteractionResult.Success : DropboxAccountInteractionResult.Error; }
        }
    }

    public enum DropboxAccountInteractionResult
    {
        Success,
        UserCanceled,
        AccesTokenParsingError,
        Error
    }
}
