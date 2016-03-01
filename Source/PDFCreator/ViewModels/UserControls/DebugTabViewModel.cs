using System;
using System.Collections.Generic;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.ViewModels.UserControls
{
    internal class DebugTabViewModel : ApplicationSettingsViewModel
    {
        public IEnumerable<EnumValue<LoggingLevel>> LoggingValues
        {
            get { return TranslationHelper.Instance.TranslatorInstance.GetEnumTranslation<LoggingLevel>(); }
        }

        public bool ProfileManagementIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableProfileManagement;
            }
        }

        protected override void OnSettingsChanged(EventArgs e)
        {
            base.OnSettingsChanged(e);

            RaisePropertyChanged("ProfileManagementIsEnabled");
        }
    }
}