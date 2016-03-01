using System;
using System.Collections.Generic;
using System.Windows;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;

namespace pdfforge.PDFCreator.ViewModels.UserControls
{
    internal class GeneralTabViewModel : ApplicationSettingsViewModel
    {
        private ApplicationProperties _applicationProperties;
        private IList<Language> _languages;

        public GeneralTabViewModel(Edition edition) : base(edition)
        {
            Languages = Translator.FindTranslations(TranslationHelper.Instance.TranslationPath);
        }

        public GeneralTabViewModel() : this (EditionFactory.Instance.Edition)
        {   }

        public ApplicationProperties ApplicationProperties
        {
            get { return _applicationProperties; }

            set
            {
                _applicationProperties = value;
                RaisePropertyChanged("ApplicationProperties");
            }
        }

        public IList<Language> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                RaisePropertyChanged("Languages");
            }
        }

        public bool DisplayUpdateWarning
        {
            get
            {
                if (ApplicationSettings == null)
                    return false;
                return ApplicationSettings.UpdateInterval == UpdateInterval.Never;
            }
        }

        public IEnumerable<AskSwitchPrinter> AskSwitchPrinterValues
        {
            get
            {
                return new List<AskSwitchPrinter>
                {
                    new AskSwitchPrinter(
                        TranslationHelper.Instance.TranslatorInstance.GetTranslation("ApplicationSettingsWindow", "Ask", "Ask"), true),
                    new AskSwitchPrinter(
                        TranslationHelper.Instance.TranslatorInstance.GetTranslation("ApplicationSettingsWindow", "Yes", "Yes"), false)
                };
            }
        }

        public IEnumerable<EnumValue<UpdateInterval>> UpdateIntervals
        {
            get { return TranslationHelper.Instance.TranslatorInstance.GetEnumTranslation<UpdateInterval>(); }
        }

        public bool LanguageIsEnabled
        {
            get
            {
                if (ApplicationSettings == null)
                    return true;

                if (GpoSettings == null)
                    return true;
                return GpoSettings.Language == null;
            }
        }

        public string CurrentLanguage
        {
            get
            {
                if (ApplicationSettings == null)
                    return null;

                if ((GpoSettings == null) || (GpoSettings.Language == null))
                    return ApplicationSettings.Language;
                return GpoSettings.Language;
            }
            set { ApplicationSettings.Language = value; }
        }

        public bool UpdateIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return GpoSettings.UpdateInterval == null;
            }
        }

        public UpdateInterval CurrentUpdateInterval
        {
            get
            {
                if (ApplicationSettings == null)
                    return UpdateInterval.Weekly;

                if ((GpoSettings == null) || (GpoSettings.UpdateInterval == null))
                    return ApplicationSettings.UpdateInterval;
                return SettingsHelper.GetUpdateInterval(GpoSettings.UpdateInterval);
            }
            set { ApplicationSettings.UpdateInterval = value; }
        }

        protected override void OnSettingsChanged(EventArgs e)
        {
            base.OnSettingsChanged(e);

            RaisePropertyChanged("Languages");
            RaisePropertyChanged("CurrentLanguage");
            RaisePropertyChanged("LanguageIsEnabled");
            RaisePropertyChanged("CurrentUpdateInterval");
            RaisePropertyChanged("UpdateIsEnabled");
        }

        public void UpdateIntervalChanged()
        {
            RaisePropertyChanged("DisplayUpdateWarning");
        }

        public Visibility UpdateCheckControlVisibility
        {
            get
            {
                if (Edition != null)
                    if (Edition.HideAndDisableUpdates)
                        return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }
    }
}