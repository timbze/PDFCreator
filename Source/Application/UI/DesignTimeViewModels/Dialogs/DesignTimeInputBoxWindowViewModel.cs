using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeInputBoxWindowViewModel : InputBoxWindowViewModel
    {
        public DesignTimeInputBoxWindowViewModel() : base(new InputBoxWindowTranslation())
        {
            //ConcreteInteraction.QuestionText = "";
            //Interaction = new InputInteraction("Please enter the text here:");
            SetInteraction(new InputInteraction("My Window Title", "Please enter the text here:"));

            Interaction.IsValidInput = s => new InputValidation(false, "This is a testing validation message.");
            Interaction.InputText = "This is my input text";
        }
    }
}