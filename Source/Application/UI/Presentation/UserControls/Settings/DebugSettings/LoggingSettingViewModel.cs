using System;
using System.Collections.Generic;
using System.Windows.Input;
using SystemInterface.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class LoggingSettingViewModel : ADebugSettingsItemControlModel
    {
        private readonly IFile _fileWrap;
        private readonly IInteractionInvoker _invoker;
        private readonly IProcessStarter _processStarter;

        public LoggingSettingViewModel(IInteractionInvoker invoker, IFile fileWrap, IProcessStarter processStarter, ISettingsManager settingsManager, ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings) : base(settingsManager, translationUpdater, settingsProvider, gpoSettings)
        {
            _fileWrap = fileWrap;
            _processStarter = processStarter;
            _invoker = invoker;
            ShowLogFileCommand = new DelegateCommand(ShowLogFileExecute);
            ClearLogFileCommand = new DelegateCommand(ClearLogFileExecute);
        }

        public ICommand ShowLogFileCommand { get; }
        public ICommand ClearLogFileCommand { get; }

        public IEnumerable<LoggingLevel> LoggingValues => Enum.GetValues(typeof(LoggingLevel)) as LoggingLevel[];

        private void ClearLogFileExecute(object o)
        {
            if (_fileWrap.Exists(LoggingHelper.LogFile))
            {
                _fileWrap.WriteAllText(LoggingHelper.LogFile, "");
            }
        }

        private void ShowLogFileExecute(object o)
        {
            if (_fileWrap.Exists(LoggingHelper.LogFile))
            {
                _processStarter.Start(LoggingHelper.LogFile);
            }
            else
            {
                var caption = Translation.NoLogFile;
                var message = Translation.NoLogFileAvailable;

                var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Warning);
                _invoker.Invoke(interaction);
            }
        }
    }
}
