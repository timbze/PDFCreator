using System;
using System.Windows.Input;
using SystemInterface.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings
{
    public class PdfTabViewModel : CurrentProfileViewModel
    {
        private readonly IFile _file;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IPdfProcessor _pdfProcessor;
        private readonly IUserGuideHelper _userGuideHelper;

        public PdfTabViewModel(PdfTabTranslation translation, IInteractionInvoker interactionInvoker, IFile file, 
            IOpenFileInteractionHelper openFileInteractionHelper, EditionHintOptionProvider editionHintOptionProvider, 
            IPdfProcessor pdfProcessor, IUserGuideHelper userGuideHelper)
        {
            _file = file;
            _openFileInteractionHelper = openFileInteractionHelper;
            _pdfProcessor = pdfProcessor;
            Translation = translation;
            _interactionInvoker = interactionInvoker;
            _userGuideHelper = userGuideHelper;
            OnlyForPlusAndBusiness = editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
            SecurityPasswordCommand = new DelegateCommand(SecurityPasswordExecute);
            ChooseCertificateFileCommand = new DelegateCommand(ChooseCertificateFileExecute);
            DefaultTimeServerCommand = new DelegateCommand(DefaultTimeServerExecute);
        }

        public Signature Signature => CurrentProfile.PdfSettings.Signature;
        public PdfTabTranslation Translation { get; }
        public DelegateCommand SecurityPasswordCommand { get; }
        public DelegateCommand ChooseCertificateFileCommand { get; }
        public DelegateCommand DefaultTimeServerCommand { get; }

        public bool OnlyForPlusAndBusiness { get; }

        public bool EncryptionEnabled
        {
            get { return CurrentProfile != null 
                    && CurrentProfile.PdfSettings.Security.Enabled; }
            set
            {
                CurrentProfile.PdfSettings.Security.Enabled = value;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool LowEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool MediumEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool HighEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes256Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;
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

        public bool SignatureEnabled
        {
            get
            {
                return CurrentProfile != null
                       && CurrentProfile.PdfSettings.Signature.Enabled;
            }
            set
            {
                CurrentProfile.PdfSettings.Signature.Enabled = value;
                RaisePropertyChanged(nameof(PdfVersion));
            }
        }

        public string PdfVersion => CurrentProfile == null ? "1.4" : _pdfProcessor.DeterminePdfVersion(CurrentProfile);

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
            var title = Translation.SelectCertFile;
            var filter = Translation.PfxP12Files
                         + @" (*.pfx, *.p12)|*.pfx;*.p12|"
                         + Translation.AllFiles
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
            var caption = Translation.PDFSignature;
            var message = Translation.CertificateDoesNotExist;

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Error);
            _interactionInvoker.Invoke(interaction);
        }

        private void TimeServerPasswordExecute(object obj)
        {
            var title = Translation.TimeServerPasswordTitle;
            var description = Translation.TimeServerPasswordDescription;
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

        public ICommand OpenPdfToolsUserGuideCommand => new DelegateCommand(OpenPdfToolsUserGuide);

        private void OpenPdfToolsUserGuide(object o)
        {
            _userGuideHelper.ShowHelp(HelpTopic.PdfTools);
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
    }
}