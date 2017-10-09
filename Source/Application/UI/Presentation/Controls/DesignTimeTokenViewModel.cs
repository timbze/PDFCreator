using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    internal class DesignTimeTokenViewModel : TokenViewModel
    {
        public DesignTimeTokenViewModel() : base(s => { }, () => "Text", new List<string>() { "some", "strings", "test" }, s => s + "_replaced")
        {
        }
    }
}
