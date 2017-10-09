using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeLicenseSettingsViewModel : LicenseSettingsViewModel
    {
        public DesignTimeLicenseSettingsViewModel()
            : base(null, new DesignTimeLicenseChecker(), new UnlicensedOfflineActivator(), new InteractionRequest(), new DispatcherWrapper(), new DesignTimeTranslationUpdater(), null)
        {
        }
    }

    public class DesignTimeLicenseChecker : ILicenseChecker
    {
        private readonly Activation _activation;

        public DesignTimeLicenseChecker()
        {
            _activation = new Activation(true);
            _activation.SetResult(Result.OK, "OK");
            _activation.TimeOfActivation = DateTime.Today;
            _activation.ActivatedTill = DateTime.Today.AddDays(7);
            _activation.LicenseExpires = DateTime.Today.AddDays(7);
            _activation.Key = "SOME-KEY";
            _activation.Licensee = "pdfforge";
            _activation.MachineId = "my-machine";
        }

        public Option<Activation, LicenseError> GetSavedActivation()
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public Option<Activation, LicenseError> GetActivation()
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public Option<string, LicenseError> GetSavedLicenseKey()
        {
            return _activation.Key.Some<string, LicenseError>();
        }

        public Option<Activation, LicenseError> ActivateWithKey(string key)
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public Option<Activation, LicenseError> ActivateWithoutSaving(string key)
        {
            return _activation.Some<Activation, LicenseError>();
        }

        public void SaveActivation(Activation activation)
        {
        }
    }
}
