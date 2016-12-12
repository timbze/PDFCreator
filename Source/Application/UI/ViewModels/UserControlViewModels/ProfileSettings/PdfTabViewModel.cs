using System;
using System.Collections.Generic;
using SystemInterface.IO;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class PdfTabViewModel : CurrentProfileViewModel
    {
        private readonly IFile _file;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public PdfTabViewModel(ITranslator translator, IInteractionInvoker interactionInvoker, IFile file, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            _file = file;
            _openFileInteractionHelper = openFileInteractionHelper;
            Translator = translator;
            _interactionInvoker = interactionInvoker;
            SecurityPasswordCommand = new DelegateCommand(SecurityPasswordExecute);
            ChooseCertificateFileCommand = new DelegateCommand(ChooseCertificateFileExecute);
            DefaultTimeServerCommand = new DelegateCommand(DefaultTimeServerExecute);
        }

        public Signature Signature => CurrentProfile.PdfSettings.Signature;

        public ITranslator Translator { get; }

        public DelegateCommand SecurityPasswordCommand { get; }
        public DelegateCommand ChooseCertificateFileCommand { get; }
        public DelegateCommand DefaultTimeServerCommand { get; }

        public IEnumerable<EnumValue<PageOrientation>> PageOrientationValues => Translator.GetEnumTranslation<PageOrientation>();

        public IEnumerable<EnumValue<ColorModel>> ColorModelValues => Translator.GetEnumTranslation<ColorModel>();

        public IEnumerable<EnumValue<PageView>> PageViewValues => Translator.GetEnumTranslation<PageView>();

        public IEnumerable<EnumValue<DocumentView>> DocumentViewValues => Translator.GetEnumTranslation<DocumentView>();

        public IEnumerable<EnumValue<CompressionColorAndGray>> CompressionColorAndGrayValues => Translator.GetEnumTranslation<CompressionColorAndGray>();

        public IEnumerable<EnumValue<CompressionMonochrome>> CompressionMonochromeValues => Translator.GetEnumTranslation<CompressionMonochrome>();

        public IEnumerable<EnumValue<SignaturePage>> SignaturePageValues => Translator.GetEnumTranslation<SignaturePage>();

        public bool EncryptionEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.PdfSettings.Security.Enabled; }
            set
            {
                CurrentProfile.PdfSettings.Security.Enabled = value;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool LowEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc40Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc40Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool MediumEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool HighEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit; }
            set
            {
                if (value)
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
                return CurrentProfile.PdfSettings.Security.EncryptionLevel != EncryptionLevel.Rc40Bit;
            }
        }

        public bool RestrictLowQualityPrintingEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality && ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = value; }
        }

        public bool AllowFillFormsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowToFillForms || !ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.AllowToFillForms = value; }
        }

        public bool AllowScreenReadersEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowScreenReader || !ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.AllowScreenReader = value; }
        }

        public bool AllowEditingAssemblyEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowToEditAssembly || !ExtendedPermissonsEnabled;
            }

            set { CurrentProfile.PdfSettings.Security.AllowToEditAssembly = value; }
        }

        public string PdfVersion => CurrentProfile == null ? "1.4" : DeterminePdfVersion(CurrentProfile);

        public DelegateCommand TimeServerPasswordCommand => new DelegateCommand(TimeServerPasswordExecute);
        public DelegateCommand SignaturePasswordCommand => new DelegateCommand(SignaturePasswordExecute);
        public Func<HelpTopic> QueryHelpTopicForCurrentTab { get; set; }

        private void DefaultTimeServerExecute(object obj)
        {
            Signature.TimeServerUrl = @"http://timestamp.globalsign.com/scripts/timstamp.dll";
            Signature.TimeServerIsSecured = false;
            RaisePropertyChanged(nameof(Signature));
        }

        private void ChooseCertificateFileExecute(object obj)
        {
            var title = Translator.GetTranslation("ProfileSettingsWindow", "SelectCertFile");
            var filter = Translator.GetTranslation("ProfileSettingsWindow", "PfxP12Files")
                         + @" (*.pfx, *.p12)|*.pfx;*.p12|"
                         + Translator.GetTranslation("ProfileSettingsWindow", "AllFiles")
                         + @" (*.*)|*.*";

            Signature.CertificateFile = _openFileInteractionHelper.StartOpenFileInteraction(Signature.CertificateFile, title, filter);
            RaisePropertyChanged(nameof(Signature));
        }

        private void SecurityPasswordExecute(object obj)
        {
            var askUserPassword = CurrentProfile.PdfSettings.Security.RequireUserPassword;

            var interaction = new EncryptionPasswordInteraction(false, true, askUserPassword);
            interaction.OwnerPassword = CurrentProfile.PdfSettings.Security.OwnerPassword;
            interaction.UserPassword = CurrentProfile.PdfSettings.Security.UserPassword;

            _interactionInvoker.Invoke(interaction);

            switch (interaction.Response)
            {
                case PasswordResult.StorePassword:
                    CurrentProfile.PdfSettings.Security.OwnerPassword = interaction.OwnerPassword;
                    CurrentProfile.PdfSettings.Security.UserPassword = interaction.UserPassword;
                    break;
                case PasswordResult.RemovePassword:
                    CurrentProfile.PdfSettings.Security.UserPassword = "";
                    CurrentProfile.PdfSettings.Security.OwnerPassword = "";
                    break;
            }
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return QueryHelpTopicForCurrentTab?.Invoke() ?? HelpTopic.ProfilePdf;
        }

        protected override void HandleCurrentProfileChanged()
        {
            RaisePropertyChangedForEncryptionProperties();
            RaisePropertyChanged(nameof(Signature));
        }

        private void SignaturePasswordExecute(object obj)
        {
            var certFile = CurrentProfile.PdfSettings.Signature.CertificateFile;

            if (!_file.Exists(certFile))
            {
                ShowCertFileMessage();
                return;
            }

            var signaturePasswordInteraction = new SignaturePasswordInteraction(PasswordMiddleButton.Remove, certFile);
            signaturePasswordInteraction.Password = CurrentProfile.PdfSettings.Signature.SignaturePassword;

            _interactionInvoker.Invoke(signaturePasswordInteraction);

            if (signaturePasswordInteraction.Result == PasswordResult.StorePassword)
            {
                CurrentProfile.PdfSettings.Signature.SignaturePassword = signaturePasswordInteraction.Password;
            }
            else if (signaturePasswordInteraction.Result == PasswordResult.RemovePassword)
            {
                CurrentProfile.PdfSettings.Signature.SignaturePassword = "";
            }
        }

        private void ShowCertFileMessage()
        {
            var message = Translator.GetTranslation("ProfileSettingsWindow", "CertificateDoesNotExist");
            var caption = Translator.GetTranslation("ProfileSettingsWindow", "PDFSignature");

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Error);
            _interactionInvoker.Invoke(interaction);
        }

        private void TimeServerPasswordExecute(object obj)
        {
            var title = Translator.GetTranslation("PdfTab", "TimeServerPasswordTitle");
            var description = Translator.GetTranslation("PdfTab", "TimeServerPasswordDescription" +
                                                                  "");
            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, title, description, false);
            interaction.Password = CurrentProfile.PdfSettings.Signature.TimeServerPassword;

            _interactionInvoker.Invoke(interaction);

            if (interaction.Result == PasswordResult.StorePassword)
            {
                CurrentProfile.PdfSettings.Signature.TimeServerPassword = interaction.Password;
            }
            else if (interaction.Result == PasswordResult.RemovePassword)
            {
                CurrentProfile.PdfSettings.Signature.TimeServerPassword = "";
            }
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private void RaisePropertyChangedForEncryptionProperties()
        {
            RaisePropertyChanged(nameof(EncryptionEnabled));
            RaisePropertyChanged(nameof(LowEncryptionEnabled));
            RaisePropertyChanged(nameof(MediumEncryptionEnabled));
            RaisePropertyChanged(nameof(HighEncryptionEnabled));
            RaisePropertyChanged(nameof(ExtendedPermissonsEnabled));
            RaisePropertyChanged(nameof(RestrictLowQualityPrintingEnabled));
            RaisePropertyChanged(nameof(AllowFillFormsEnabled));
            RaisePropertyChanged(nameof(AllowScreenReadersEnabled));
            RaisePropertyChanged(nameof(AllowEditingAssemblyEnabled));
            RaisePropertyChanged(nameof(PdfVersion));
        }

        private string DeterminePdfVersion(ConversionProfile profile)
        {
            var pdfVersion = "1.4";
            if (profile.PdfSettings.Security.Enabled && (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit))
                pdfVersion = "1.6";
            return pdfVersion;
        }
    }
}