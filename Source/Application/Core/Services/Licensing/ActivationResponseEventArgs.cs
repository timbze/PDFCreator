using System;
using pdfforge.LicenseValidator;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public class ActivationResponseEventArgs : EventArgs
    {
        public ActivationResponseEventArgs(Activation activation, bool isLicenseValid)
        {
            Activation = activation;
            IsLicenseValid = isLicenseValid;
        }

        public Activation Activation { get; }
        public bool IsLicenseValid { get; }
    }
}