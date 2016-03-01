using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Licensing
{
    public class Edition
    {
        public string Name { get; set; }
        public IVersionHelper VersionHelper { get; set; }
        public Activation Activation { get; set; } 
        public bool ValidOnTerminalServer { get; set; }
        public string UpdateSectionName { get; set; }
        public string UpdateInfoUrl { get; set; }
        public bool ActivateGpo { get; set; }
        public bool AutomaticUpdate { get; set; }
        public bool ShowPlusHint { get; set; }
        public bool ShowWelcomeWindow { get; set; }
        public bool HideLicensing { get; set; }
        public bool HideAndDisableUpdates { get; set; }
        public bool HideDonateButton { get; set; }
        public bool HideSocialMediaButtons { get; set; }
        public LicenseStatus LicenseStatus { get; set; }

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


    }
}