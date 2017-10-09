using Optional;
using pdfforge.LicenseValidator.Interface.Data;
using System;

namespace pdfforge.PDFCreator.Core.Services.Licensing
{
    public class ActivationResponseEventArgs : EventArgs
    {
        public ActivationResponseEventArgs(Option<Activation, LicenseError> activation)
        {
            Activation = activation;
            IsLicenseValid = activation.Map(a => a.IsActivationStillValid()).ValueOr(false);
        }

        public Option<Activation, LicenseError> Activation { get; }
        public bool IsLicenseValid { get; }
    }
}
