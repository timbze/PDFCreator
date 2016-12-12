using System;
using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class InputInteraction : IInteraction
    {
        public InputInteraction(string title, string questionText, Func<string, InputValidation> isValidInput = null)
        {
            Title = title;
            QuestionText = questionText;
            IsValidInput = isValidInput;
        }

        public Func<string, InputValidation> IsValidInput { get; set; }

        public string Title { get; set; }
        public string InputText { get; set; }
        public string QuestionText { get; set; }

        public bool Success { get; set; }
    }
}