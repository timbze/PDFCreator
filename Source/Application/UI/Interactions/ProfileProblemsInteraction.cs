using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class ProfileProblemsInteraction : IInteraction
    {
        public ProfileProblemsInteraction(ActionResultDict profileProblems)
        {
            ProfileProblems = profileProblems;
        }

        public ActionResultDict ProfileProblems { get; }

        public bool IgnoreProblems { get; set; }
    }
}