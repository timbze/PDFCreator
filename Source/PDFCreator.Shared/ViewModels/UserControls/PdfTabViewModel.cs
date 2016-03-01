using System;
using System.Collections.Generic;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFProcessing;

namespace pdfforge.PDFCreator.Shared.ViewModels.UserControls
{
    public class PdfTabViewModel : CurrentProfileViewModel
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;

        public PdfTabViewModel()
        {
            ProfileChanged += OnProfileChanged;
        }

        private void OnProfileChanged(object sender, EventArgs eventArgs)
        {
            RaisePropertyChangedForEncryptionProperties();
        }

        public static IEnumerable<EnumValue<PageOrientation>> PageOrientationValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<PageOrientation>(); }
        }

        public static IEnumerable<EnumValue<ColorModel>> ColorModelValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<ColorModel>(); }
        }

        public static IEnumerable<EnumValue<PageView>> PageViewValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<PageView>(); }
        }

        public static IEnumerable<EnumValue<DocumentView>> DocumentViewValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<DocumentView>(); }
        }

        public static IEnumerable<EnumValue<CompressionColorAndGray>> CompressionColorAndGrayValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<CompressionColorAndGray>(); }
        }

        public static IEnumerable<EnumValue<CompressionMonochrome>> CompressionMonochromeValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<CompressionMonochrome>(); }
        }

        public static IEnumerable<EnumValue<SignaturePage>> SignaturePageValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<SignaturePage>(); }
        }

        private void RaisePropertyChangedForEncryptionProperties()
        {
            RaisePropertyChanged("EncryptionEnabled");
            RaisePropertyChanged("LowEncryptionEnabled");
            RaisePropertyChanged("MediumEncryptionEnabled");
            RaisePropertyChanged("HighEncryptionEnabled");
            RaisePropertyChanged("ExtendedPermissonsEnabled");
            RaisePropertyChanged("RestrictLowQualityPrintingEnabled");
            RaisePropertyChanged("AllowFillFormsEnabled");
            RaisePropertyChanged("AllowScreenReadersEnabled");
            RaisePropertyChanged("AllowEditingAssemblyEnabled");
            RaisePropertyChanged("PdfVersion");
        }

        public bool EncryptionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.Enabled;
            }
            set
            {
                CurrentProfile.PdfSettings.Security.Enabled = value;
                RaisePropertyChangedForEncryptionProperties();
            }
        } 

        public bool LowEncryptionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit);
            }
            set
            {
                if (value) //== true
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool MediumEncryptionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc128Bit);
            }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool HighEncryptionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit);
            }
            set
            {
                if (value) //== true
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool ExtendedPermissonsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.EncryptionLevel != EncryptionLevel.Rc40Bit);
            }
        }

        public bool RestrictLowQualityPrintingEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality && ExtendedPermissonsEnabled);
            }
            set
            {
                CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = value;
            }
        }

        public bool AllowFillFormsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.AllowToFillForms || !ExtendedPermissonsEnabled);
            }
            set
            {
                CurrentProfile.PdfSettings.Security.AllowToFillForms = value;
            }
        }

        public bool AllowScreenReadersEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.AllowScreenReader || !ExtendedPermissonsEnabled);
            }
            set
            {
                CurrentProfile.PdfSettings.Security.AllowScreenReader = value;
            }
        }

        public bool AllowEditingAssemblyEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return (CurrentProfile.PdfSettings.Security.AllowToEditAssembly || !ExtendedPermissonsEnabled);
            }

            set
            {
                CurrentProfile.PdfSettings.Security.AllowToEditAssembly = value;
            }
        }

        public string PdfVersion
        {
            get
            {
                if (CurrentProfile == null)
                    return "1.4";
                return PDFProcessor.DeterminePdfVersion(CurrentProfile);
            }
        }
    }
}
