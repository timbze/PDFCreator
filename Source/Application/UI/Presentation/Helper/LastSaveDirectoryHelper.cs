using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.Utilities;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface ILastSaveDirectoryHelper
    {
        void Apply(Job job);

        void Save(Job job);
    }

    public class LastSaveDirectoryHelper : ILastSaveDirectoryHelper
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly ISettingsManager _settingsManager;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IPathUtil _pathUtil;
        private readonly PathWrapSafe _pathSafe = new PathWrapSafe();

        public LastSaveDirectoryHelper(ICurrentSettingsProvider currentSettingsProvider, ISettingsManager settingsManager, ITempFolderProvider tempFolderProvider, IPathUtil pathUtil)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _settingsManager = settingsManager;
            _tempFolderProvider = tempFolderProvider;
            _pathUtil = pathUtil;
        }

        private bool IsEnabled(Job job)
        {
            return string.IsNullOrWhiteSpace(job.Profile.TargetDirectory);
        }

        private bool IsTemp(string directory)
        {
            return directory.StartsWith(_tempFolderProvider.TempFolder);
        }

        public void Apply(Job job)
        {
            if (!IsEnabled(job))
                return;

            var lastSaveDirectory = _currentSettingsProvider.Settings.ApplicationSettings.LastSaveDirectory;
            if (string.IsNullOrWhiteSpace(lastSaveDirectory))
                return;

            var filename = _pathSafe.GetFileName(job.OutputFilenameTemplate);
            if (string.IsNullOrWhiteSpace(job.JobInfo.OutputFileParameter))
                job.OutputFilenameTemplate = _pathSafe.Combine(lastSaveDirectory, filename);
        }

        public void Save(Job job)
        {
            if (!IsEnabled(job))
                return;

            var directory = _pathUtil.GetLongDirectoryName(job.OutputFilenameTemplate);

            if (IsTemp(directory))
                return;

            _currentSettingsProvider.Settings.ApplicationSettings.LastSaveDirectory = directory;
            _settingsManager.SaveCurrentSettings();
        }
    }
}
