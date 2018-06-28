using pdfforge.PDFCreator.Conversion.Jobs;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string AuthorLabel { get; private set; } = "_Author:";

        [Context("PrintJobWindowButton")]
        public string CancelButton { get; private set; } = "_Cancel";

        public string VolumeLabelInvalidTitle { get; private set; } = "Invalid path";
        public string VolumeLabelInvalid { get; private set; } = "The path is not valid. Please enter a valid absolute path.";

        public string FolderPathIsNotValid { get; private set; } = "The folder path is not valid or empty. Please enter a valid path. \n" +
                                                                   "The folder path must not contain any of the following characters: \n" +
                                                                   @" \ / : * ? \" + "< >";

        public string FolderPathIsNotValidTitle { get; private set; } = "Folder path is not valid";
        public string FilePathTooLongTitle { get; private set; } = "File path too long";
        private string FilePathTooLongDescription { get; set; } = "The path to the file is longer than the maximum of {0} characters allowed, please choose a shorter file path.";

        public string FormatFilePathTooLongDescription(int maxPathLenght)
        {
            return string.Format(FilePathTooLongDescription, maxPathLenght);
        }

        public string ConfirmSaveAs { get; private set; } = "Confirm Save As";
        public string DefectiveProfile { get; private set; } = "Defective Profile";
        public string EditOrSelectNewProfile { get; private set; } = "Edit the profile or select another one.";
        public string EditProfile { get; private set; } = "Edit";

        private string FileAlreadyExists { get; set; } = "'{0}' already exists.\nDo you want to replace it?";

        public string GetFileAlreadyExists(string filepath)
        {
            return string.Format(FileAlreadyExists, filepath);
        }

        public string KeywordsLabel { get; private set; } = "_Keywords:";

        [Context("PrintJobWindowButton")]
        public string MergeButton { get; private set; } = "_Merge";

        public string ProfileLabel { get; private set; } = "_Profile:";

        [Context("PrintJobWindowButton")]
        public string SaveButton { get; private set; } = "_Save";

        [Context("PrintJobWindowButton")]
        public string EmailButton { get; private set; } = "_E-mail";

        private string[] SelectedProfileIsDefective { get; set; } = { "There is an issue with the profile \'{0}\':", "There are issues with the profile \'{0}\':" };

        public string GetProfileIsDefectiveMessage(string profileName, ActionResult actionResult)
        {
            var pluralMessage = PluralBuilder.GetPlural(actionResult.Count, SelectedProfileIsDefective);
            return string.Format(pluralMessage, profileName);
        }

        public string SubjectLabel { get; private set; } = "S_ubject:";
        public string TitleLabel { get; private set; } = "_Title:";
        public string FilenameText { get; private set; } = "File_name:";
        public string FoldernameText { get; private set; } = "_Folder:";
    }
}
