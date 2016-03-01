using System;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Licensing
{
    public interface IEditionFactory
    {
        Edition Edition { get; }
        Edition ReloadEdition();
        Edition DetermineEdition(Activation activation);
    }

    public class EditionFactory : IEditionFactory
    {
        private readonly IVersionHelper _versionHelper;
        private readonly string _editionName;
        private readonly bool _validOnTerminalServer;

        private static EditionFactory _instance;
        public static EditionFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EditionFactory(Properties.Edition.Name, Properties.Edition.ValidOnTerminalServer, VersionHelper.Instance, GetSavedActivation);
                }
                return _instance;
            }
        }

        public Edition Edition { get; private set; }

        public Edition ReloadEdition()
        {
            _instance = null;
            return Instance.Edition;
        }

        public EditionFactory(string editionName, string validOnTerminalServer, IVersionHelper versionHelper, Func<Product?, Activation> getSavedActivation)
        {
            _editionName = editionName;

            bool tmpValidOnTs;
            if (bool.TryParse(validOnTerminalServer, out tmpValidOnTs))
            {
                _validOnTerminalServer = tmpValidOnTs;
            }
            _versionHelper = versionHelper;
            Activation activation;
            switch (_editionName.Trim().ToLowerInvariant())
            {
                case "pdfcreator":
                case "pdfcreator custom":
                    activation = null;
                    break;
                case "pdfcreator plus":
                    activation = getSavedActivation(Product.PdfCreator);
                    break;
                case "pdfcreator business":
                    activation = getSavedActivation(Product.PdfCreatorBusiness);
                    break;
                case "pdfcreator terminal server":
                    activation = getSavedActivation(Product.PdfCreatorTerminalServer);
                    break;
                default:
                    throw new NotSupportedException(_editionName + "is no valid PDFCreator Edition.");
            }

            Edition = DetermineEdition(activation);
        }

        public Edition DetermineEdition(Activation activation)
        {
            var edition = new Edition();
            edition.Activation = activation;
            edition.VersionHelper = _versionHelper;

            switch (_editionName.Trim().ToLowerInvariant())
            {
                case "pdfcreator":
                    SetPDFCreatorProperties(edition);
                    edition.Activation = null;
                    break;
                case "pdfcreator plus":
                    SetPdfCreatorPlusProperties(edition);
                    //Product must be reset, because it might become 0 for invalid activations
                    edition.Activation.Product = Product.PdfCreator;
                    break;
                case "pdfcreator business":
                    SetPdfCreatorBusinessProperties(edition);
                    //Product must be reset, because it might become 0 for invalid activations
                    edition.Activation.Product = Product.PdfCreatorBusiness;
                    break;
                case "pdfcreator terminal server":
                    SetPdfCreatorTerminalServerProperties(edition);
                    //Product must be reset, because it might become 0 for invalid activations
                    edition.Activation.Product = Product.PdfCreatorTerminalServer;
                    break;
                case "pdfcreator custom":
                    SetPDFCreatorCustomProperties(edition);
                    edition.Activation = null;
                    break;
                default:
                    throw new NotSupportedException(_editionName + "is no valid PDFCreator Edition.");
            }

            return edition;
        }

        private void SetPDFCreatorProperties(Edition edition)
        {
            edition.Name = "PDFCreator";
            edition.LicenseStatus = LicenseStatus.Valid;
            edition.ValidOnTerminalServer = false;
            edition.UpdateSectionName = "PDFCreator";
            edition.UpdateInfoUrl = Urls.PdfCreatorUpdateInfoUrl;
            edition.ShowPlusHint = true;
            edition.AutomaticUpdate = false;
            edition.ActivateGpo = false;
            edition.HideLicensing = true;
            edition.HideDonateButton = false;
            edition.HideSocialMediaButtons = false;
            edition.HideAndDisableUpdates = false;
            edition.ShowWelcomeWindow = true;
        }

        private void SetPdfCreatorPlusProperties(Edition edition)
        {
            edition.Name = "PDFCreator Plus";
            edition.LicenseStatus = DetermineLicenseStatus(edition.Activation, true, true);
            edition.ValidOnTerminalServer = false;
            edition.UpdateSectionName = "PDFCreatorPlus";
            edition.UpdateInfoUrl = Urls.PdfCreatorPlusUpdateInfoUrl;
            edition.ShowPlusHint = false;
            edition.ShowWelcomeWindow = true;
            edition.AutomaticUpdate = true;
            edition.ActivateGpo = false;
            edition.HideLicensing = false;
            edition.HideDonateButton = true;
            edition.HideSocialMediaButtons = false;
            edition.HideAndDisableUpdates = false;
        }

        private void SetPdfCreatorBusinessProperties(Edition edition)
        {
            edition.Name = "PDFCreator Business";
            edition.LicenseStatus = DetermineLicenseStatus(edition.Activation, false, false);
            edition.ValidOnTerminalServer = false;
            edition.UpdateSectionName = "PDFCreatorBusiness";
            edition.UpdateInfoUrl = Urls.PdfCreatorBusinessUpdateInfoUrl;
            edition.ShowPlusHint = false;
            edition.ShowWelcomeWindow = false;
            edition.AutomaticUpdate = true;
            edition.ActivateGpo = true;
            edition.HideLicensing = false;
            edition.HideDonateButton = true;
            edition.HideSocialMediaButtons = true;
            edition.HideAndDisableUpdates = false;
        }

        private void SetPdfCreatorTerminalServerProperties(Edition edition)
        {
            edition.Name = "PDFCreator Terminal Server";
            edition.LicenseStatus = DetermineLicenseStatus(edition.Activation, false, false);
            edition.ValidOnTerminalServer = true;
            edition.UpdateSectionName = "PDFCreatorTerminalServer";
            edition.UpdateInfoUrl = Urls.PdfCreatorTerminalServerUpdateInfoUrl;
            edition.ShowPlusHint = false;
            edition.ShowWelcomeWindow = false;
            edition.AutomaticUpdate = true;
            edition.ActivateGpo = true;
            edition.HideLicensing = false;
            edition.HideDonateButton = true;
            edition.HideSocialMediaButtons = true;
            edition.HideAndDisableUpdates = false;
        }

        private void SetPDFCreatorCustomProperties(Edition edition)
        {
            edition.Name = "PDFCreator Custom";
            edition.LicenseStatus = LicenseStatus.Valid;
            edition.ValidOnTerminalServer = _validOnTerminalServer;
            edition.UpdateSectionName = "";
            edition.UpdateInfoUrl = "";
            edition.ShowPlusHint = false;
            edition.ShowWelcomeWindow = false;
            edition.AutomaticUpdate = false;
            edition.ActivateGpo = true;
            edition.HideLicensing = true;
            edition.HideDonateButton = true;
            edition.HideSocialMediaButtons = true;
            edition.HideAndDisableUpdates = true;
        }

        private static Activation GetSavedActivation(Product? product)
        {
            if (product == null)
                return null;

            var licenseServerHelper = new LicenseServerHelper();

            var hklmLicenseChecker = licenseServerHelper.BuildLicenseChecker((Product)product, RegistryHive.LocalMachine);
            var localMachineActivation = hklmLicenseChecker.GetSavedActivation();

            var hkcuLicenseChecker = licenseServerHelper.BuildLicenseChecker((Product)product, RegistryHive.CurrentUser);
            var currentUserActivation = hkcuLicenseChecker.GetSavedActivation();

            if ((localMachineActivation.TimeOfActivation == DateTime.MinValue) && (currentUserActivation.TimeOfActivation == DateTime.MinValue))
            {
                if (!string.IsNullOrWhiteSpace(currentUserActivation.Key))
                    return currentUserActivation;

                return localMachineActivation;
            }

            return localMachineActivation.TimeOfActivation >= currentUserActivation.TimeOfActivation
                ? localMachineActivation
                : currentUserActivation;
        }

        private LicenseStatus DetermineLicenseStatus(Activation activation, bool acceptExpiredLicense, bool acceptExpiredActivations)
        {
            if (activation == null)
                return LicenseStatus.NoLicense;

            switch (activation.Result)
            {
                case Result.OK:
                    //Activation must be checked before license
                    if (!acceptExpiredActivations)
                        if (!activation.IsActivationStillValid())
                            return LicenseStatus.ActivationExpired;

                    if (!activation.IsLicenseStillValid())
                        return acceptExpiredLicense ? LicenseStatus.ValidForVersionButLicenseExpired : LicenseStatus.LicenseExpired;

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
