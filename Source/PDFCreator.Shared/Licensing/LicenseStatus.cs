using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Licensing
{
    public enum LicenseStatus
    {
        [StringValue("The license is valid.")]
        Valid,

        [StringValue("The license has expired but is valid for this version.")]
        ValidForVersionButLicenseExpired,

        [StringValue("The license has expired. Please renew your license.")]
        LicenseExpired,

        [StringValue("The activation has expired. Please renew your activation.")]
        ActivationExpired,

        [StringValue("The machine or user is blocked. Please contact the support.")]
        Blocked,

        [StringValue("The license is not valid for this version.")]
        VersionNotCoveredByLicense,

        [StringValue("The license has exceeded the allowed number of activated machines.")]
        NumberOfActivationsExceeded,

        [StringValue("The license key is not valid. Please enter a new license key.")]
        InvalidLicenseKey,

        [StringValue("No license key available. Please enter a new license key.")]
        NoLicenseKey,

        [StringValue("No license available. Please enter a new license key.")]
        NoLicense,

        [StringValue("There was a problem with the connection to the license server.")]
        NoServerConnection,

        [StringValue("Error while authenticateting license.")]
        Error
    }
}
 