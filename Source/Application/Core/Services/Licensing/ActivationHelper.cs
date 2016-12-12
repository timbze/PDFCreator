using System;
using pdfforge.LicenseValidator;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public class UnlicensedActivationHelper : IActivationHelper
    {
        public Activation Activation { get; set; }
        public bool IsLicenseValid => true;
        public LicenseStatus LicenseStatus => LicenseStatus.Valid;
        public void LoadActivation()
        {   }
        public void SaveActivation()
        {   }
        public void RenewActivation()
        {   }
        public string GetOfflineActivationString(string licenseKey)
        {   return "";  }
        public Activation ActivateWithoutSavingActivation(string licenseKey)
        { return null; }
        public Activation ActivateOfflineActivationStringFromLicenseServer(string lsa)
        { return null; }
    }

    public class ActivationHelper : IActivationHelper
    {
        private readonly Product _licenseServerProduct;
        private readonly ILicenseChecker _licenseCheckerHkcu;
        private readonly ILicenseChecker _licenseCheckerHklm;

        public ActivationHelper(Product licenseServerProduct, ILicenseServerHelper licenseServerHelper)
        {
            _licenseServerProduct = licenseServerProduct;
            _licenseCheckerHkcu = licenseServerHelper.BuildLicenseChecker(RegistryHive.CurrentUser);
            _licenseCheckerHklm = licenseServerHelper.BuildLicenseChecker(RegistryHive.LocalMachine);
        }

        private Activation _activation;
        public Activation Activation
        {
            get { return _activation; }
            set
            {
                _activation = value ?? new Activation();

                if (_activation.Product == 0)
                    _activation.Product = _licenseServerProduct;

                if (_activation.Key == null)
                    _activation.Key = "";
            }
        }

        public bool AcceptExpiredActivation { private get; set; }

        public bool AcceptExpiredLicense { private get; set; }

        public void LoadActivation()
        {
            Activation = GetSavedActivation();
        }

        public void SaveActivation()
        {
            _licenseCheckerHkcu.SaveActivation(Activation);
        }

        public virtual void RenewActivation()
        { 
            try
            {
                _licenseCheckerHkcu.ActivateWithKey(Activation.Key);
            }
            finally
            {
                LoadActivation();
            }
        }

        public LicenseStatus LicenseStatus => DetermineLicenseStatus(Activation);

        public bool IsLicenseValid
        {
            get
            {
                switch (LicenseStatus)
                {
                    case LicenseStatus.Valid:
                    case LicenseStatus.ValidForVersionButLicenseExpired:
                        return true;
                    default:
                        return false;
                }
            }
        }

        protected virtual Activation GetSavedActivation()
        {
            var localMachineActivation = GetLocalMachineActivation();
            var currentUserActivation = GetCurrentUserActivation();

            if ((localMachineActivation.TimeOfActivation == DateTime.MinValue) &&
                (currentUserActivation.TimeOfActivation == DateTime.MinValue))
            {
                if (!string.IsNullOrWhiteSpace(currentUserActivation.Key))
                    return currentUserActivation;

                return localMachineActivation;
            }

            return currentUserActivation.TimeOfActivation >= localMachineActivation.TimeOfActivation
                ? currentUserActivation : localMachineActivation;
        }

        private Activation GetLocalMachineActivation()
        {
            try
            {
                return _licenseCheckerHklm.GetSavedActivation();
            }
            catch (FormatException)
            {
                return new Activation();
            }
        }

        private Activation GetCurrentUserActivation()
        {
            try
            {
                return _licenseCheckerHkcu.GetSavedActivation();
            }
            catch (FormatException)
            {
                return new Activation();
            }
        }

        public string GetOfflineActivationString(string licenseKey)
        {
            return _licenseCheckerHkcu.GetOfflineActivationString(licenseKey.Trim());
        }

        public Activation ActivateWithoutSavingActivation(string licenseKey)
        {
            return _licenseCheckerHkcu.ActivateWithoutSavingActivation(licenseKey);
        }

        public Activation ActivateOfflineActivationStringFromLicenseServer(string lsa)
        {
            return _licenseCheckerHkcu.ActivateOfflineActivationStringFromLicenseServer(lsa);
        }

        private LicenseStatus DetermineLicenseStatus(Activation activation)
        {
            if (activation == null)
                return LicenseStatus.NoLicense;

            switch (activation.Result)
            {
                case Result.OK:
                    //Activation must be checked before license
                    if (!AcceptExpiredActivation)
                        if (!activation.IsActivationStillValid())
                            return LicenseStatus.ActivationExpired;

                    if (!activation.IsLicenseStillValid())
                        return AcceptExpiredLicense
                            ? LicenseStatus.ValidForVersionButLicenseExpired
                            : LicenseStatus.LicenseExpired;

                    return LicenseStatus.Valid;

                case Result.BLOCKED:
                    return LicenseStatus.Blocked;
                case Result.LICENSE_EXPIRED:
                    return LicenseStatus.VersionNotCoveredByLicense;
                case Result.LICENSE_LIMIT_REACHED:
                    return LicenseStatus.NumberOfActivationsExceeded;
                case Result.INVALID_LICENSE_KEY:
                    return LicenseStatus.InvalidLicenseKey;
                case Result.NO_LICENSE_KEY:
                    return LicenseStatus.NoLicenseKey;
                case Result.NO_SERVER_CONNECTION:
                    return LicenseStatus.NoServerConnection;
                case Result.AUTH_FAILED:
                case Result.UNKNOWN_VERSION:
                default:
                    return LicenseStatus.Error;
            }
        }
    }
}
