using System;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
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
        private readonly IUserGuideLauncher _userGuideLauncher;
        private readonly IApplicationLanguageProvider _applicationLanguageProvider;
        private readonly ILanguageProvider _languageProvider;

        public UserGuideHelper(IFile fileWrap, IAssemblyHelper assemblyHelper, IUserGuideLauncher userGuideLauncher, IApplicationLanguageProvider applicationLanguageProvider, ILanguageProvider languageProvider)
        {
            _fileWrap = fileWrap;
            _assemblyHelper = assemblyHelper;
            _userGuideLauncher = userGuideLauncher;
            _applicationLanguageProvider = applicationLanguageProvider;
            _languageProvider = languageProvider;

            UpdateLanguage();

            _applicationLanguageProvider.LanguageChanged += OnLanguageChanged;
        }

        private Language GetLanguage()
        {
            var englishLanguage = _languageProvider.FindBestLanguage("en");
            var languageIso = _applicationLanguageProvider.GetApplicationLanguage();
            var language = _languageProvider.GetAvailableLanguages().FirstOrDefault(lang => lang.Iso2 == languageIso);

            return language ?? englishLanguage;
        }

        public void ShowHelp(HelpTopic topic)
        {
            _userGuideLauncher.ShowHelpTopic(topic);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            UpdateLanguage();
        }

        public void UpdateLanguage()
        {
            var language = GetLanguage();

            var applicationDir = _assemblyHelper.GetPdfforgeAssemblyDirectory();

            var candidates = new[]
            {
                _pathSafe.Combine(applicationDir, $"PDFCreator_{language.CommonName}.chm"),
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