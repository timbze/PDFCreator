using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
    public class PrintJobViewModelTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        private string[] MoreJobsWaiting { get; set; } = { "There is {0} more Job waiting", "There are {0} more Jobs waiting"};
        public string NoJobsWaiting { get; private set; } = "Print more documents to merge or rearrange them";
        public string AuthorLabel { get; private set; } = "_Author";
        [Context("PrintJobWindowButton")]
        public string CancelLabel { get; private set; } = "Cancel";
        [Context("PrintJobWindowButton")]
        public string EmailLabel { get; private set; } = "E-Mail";
        public string KeywordsLabel { get; private set; } = "_Keywords";
        [Context("PrintJobWindowButton")]
        public string MergeJobsLabel { get; private set; } = "Merge";
        public string MetadataExpander { get; private set; } = "_Metadata";
        public string ProfileLabel { get; private set; } = "_Profile";
        [Context("PrintJobWindowButton")]
        public string SaveLabel { get; private set; } = "Save";
        [Context("PrintJobWindowButton")]
        public string SettingsLabel { get; private set; } = "Settings";
        public string SubjectLabel { get; private set; } = "_Subject";
        public string TitleLabel { get; private set; } = "_Title";

        public string FormatMoreJobsWaiting(int numberOfJobs)
        {
            return PluralBuilder.GetFormattedPlural(numberOfJobs, MoreJobsWaiting);
        }
    }
}
