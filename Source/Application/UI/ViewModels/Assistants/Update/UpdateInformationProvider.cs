namespace pdfforge.PDFCreator.UI.ViewModels.Assistants.Update
{
    public class UpdateInformationProvider
    {
        public UpdateInformationProvider(string updateInfoUrl, string sectionName)
        {
            UpdateInfoUrl = updateInfoUrl;
            SectionName = sectionName;
        }

        public string UpdateInfoUrl { get; }
        public string SectionName { get; }
    }
}