﻿using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class NotificationsLevelSettingsViewModel : ProfileUserControlViewModel<NotificationsLevelSettingsTranslation>, IWhitelisted
    {
        public NotificationsLevelSettingsViewModel(ICurrentSettingsProvider settingsProvider,
        IGpoSettings gpoSettings, ITranslationUpdater translationUpdater, EditionHelper editionHelper, IDispatcher dispatcher) :
            base(translationUpdater, settingsProvider, dispatcher)
        {
            AllowNotifications = true;
        }

        public bool AllowNotifications { get; }
    }
}
