namespace pdfforge.PDFCreator.UI.ViewModels.Wrapper
{
    public class AskSwitchPrinter
    {
        public AskSwitchPrinter(string name, bool value)
        {
            Value = value;
            Name = name;
        }

        public bool Value { get; set; }
        public string Name { get; set; }
    }
}