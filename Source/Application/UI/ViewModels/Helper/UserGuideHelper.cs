using System;
using SystemInterface.IO;
using SystemWrapper.IO;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.UserGuide;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public interface IUserGuideHelper
    {
        void ShowHelp(HelpTopic topic);
    }

    public class UserGuideHelper : IUserGuideHelper
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IFile _fileWrap;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();
        private readonly ISettingsProvider _settingsProvider;
        private readonly IUserGuideLauncher _userGuideLauncher;

        public UserGuideHelper(IFile fileWrap, IAssemblyHelper assemblyHelper, IUserGuideLauncher userGuideLauncher, ISettingsProvider settingsProvider)
        {
            _fileWrap = fileWrap;
            _assemblyHelper = assemblyHelper;
            _userGuideLauncher = userGuideLauncher;
            _settingsProvider = settingsProvider;

            if (_settingsProvider.Settings != null)
                SetLanguage(_settingsProvider.Settings.ApplicationSettings.Language);

            _settingsProvider.LanguageChanged += OnLanguageChanged;
        }

        public void ShowHelp(HelpTopic topic)
        {
            _userGuideLauncher.ShowHelpTopic(topic);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            SetLanguage(_settingsProvider.Settings.ApplicationSettings.Language);
        }

        public void SetLanguage(string languageName)
        {
            var applicationDir = _assemblyHelper.GetPdfforgeAssemblyDirectory();

            var candidates = new[]
            {
                _pathSafe.Combine(applicationDir, $"PDFCreator_{languageName}.chm"),
                _pathSafe.Combine(applicationDir, "PDFCreator_english.chm")
            };

            foreach (var candidate in candidates)
            {
                if (!_fileWrap.Exists(candidate))
                    continue;

                _userGuideLauncher.SetUserGuide(candidate);
                break;
            }
        }
    }
}