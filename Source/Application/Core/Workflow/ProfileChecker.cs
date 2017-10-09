using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IProfileChecker
    {
        ActionResultDict ProfileCheckDict(IList<ConversionProfile> profileList, Accounts accounts);

        ActionResult ProfileCheck(ConversionProfile profile, Accounts accounts);
    }

    public class ProfileChecker : IProfileChecker
    {
        private readonly IEnumerable<ICheckable> _actionChecks;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ProfileChecker(IFile file, IEnumerable<ICheckable> actionChecks)
        {
            _file = file;
            _actionChecks = actionChecks;
        }

        public ActionResultDict ProfileCheckDict(IList<ConversionProfile> profileList, Accounts accounts)
        {
            var nameResultDict = new ActionResultDict();

            foreach (var profile in profileList)
            {
                var result = ProfileCheck(profile, accounts);
                if (!result)
                    nameResultDict.Add(profile.Name, result);
            }

            return nameResultDict;
        }

        public ActionResult ProfileCheck(ConversionProfile profile, Accounts accounts)
        {
            var actionResult = new ActionResult();

            actionResult.AddRange(CheckAutosaveSettings(profile));
            actionResult.AddRange(CheckSaveSettings(profile));
            actionResult.AddRange(CheckCoverPageSettings(profile));
            actionResult.AddRange(CheckAttachmentPageSettings(profile));
            actionResult.AddRange(CheckStampingSettings(profile));
            actionResult.AddRange(CheckEncryptionSettings(profile));
            actionResult.AddRange(CheckBackgroundpageSettings(profile));
            actionResult.AddRange(CheckSignatureSettings(profile, accounts));

            foreach (var actionCheck in _actionChecks)
            {
                actionResult.AddRange(actionCheck.Check(profile, accounts));
            }

            return actionResult;
        }

        private ActionResult CheckSaveSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if (profile.SaveDialog.SetDirectory
                && !profile.AutoSave.Enabled) //Skip if Autosave is enabled.
            {
                if (string.IsNullOrEmpty(profile.TargetDirectory))
                {
                    _logger.Error("Preselected folder for savedialog is empty.");
                    actionResult.Add(ErrorCode.SaveDialog_NoPreselectedFolder);
                }
            }

            return actionResult;
        }

        private ActionResult CheckAutosaveSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if (profile.AutoSave.Enabled)
            {
                if (string.IsNullOrEmpty(profile.TargetDirectory))
                {
                    _logger.Error("Automatic saving without target directory.");
                    actionResult.Add(ErrorCode.AutoSave_NoTargetDirectory);
                }
                if (string.IsNullOrEmpty(profile.FileNameTemplate))
                {
                    _logger.Error("Automatic saving without filename template.");
                    actionResult.Add(ErrorCode.AutoSave_NoFilenameTemplate);
                }
            }

            return actionResult;
        }

        private ActionResult CheckCoverPageSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if (profile.CoverPage.Enabled)
            {
                if (string.IsNullOrEmpty(profile.CoverPage.File))
                {
                    _logger.Error("No cover file is specified.");
                    actionResult.Add(ErrorCode.Cover_NoFileSpecified);
                }
                //Skip check for network path
                else if (!profile.CoverPage.File.StartsWith(@"\\") && !_file.Exists(profile.CoverPage.File))
                {
                    _logger.Error("The cover file \"" + profile.CoverPage.File + "\" does not exist.");
                    actionResult.Add(ErrorCode.Cover_FileDoesNotExist);
                }
                else if (!profile.CoverPage.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.Error("The cover file \"" + profile.CoverPage.File + "\" is no pdf file.");
                    actionResult.Add(ErrorCode.Cover_NoPdf);
                }
            }

            return actionResult;
        }

        private ActionResult CheckAttachmentPageSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if (profile.AttachmentPage.Enabled)
            {
                if (string.IsNullOrEmpty(profile.AttachmentPage.File))
                {
                    _logger.Error("No attachment file is specified.");
                    actionResult.Add(ErrorCode.Attachment_NoFileSpecified);
                }
                //Skip check for network path
                else if (!profile.AttachmentPage.File.StartsWith(@"\\") && !_file.Exists(profile.AttachmentPage.File))
                {
                    _logger.Error("The attachment file \"" + profile.AttachmentPage.File + "\" does not exist.");
                    actionResult.Add(ErrorCode.Attachment_FileDoesNotExist);
                }
                else if (!profile.AttachmentPage.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.Error("The attachment file \"" + profile.CoverPage.File + "\" is no pdf file.");
                    actionResult.Add(ErrorCode.Attachment_NoPdf);
                }
            }
            return actionResult;
        }

        private ActionResult CheckStampingSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if (profile.Stamping.Enabled)
            {
                if (string.IsNullOrEmpty(profile.Stamping.StampText))
                {
                    _logger.Error("No stamp text is specified.");
                    actionResult.Add(ErrorCode.Stamp_NoText);
                }
                if (string.IsNullOrEmpty(profile.Stamping.FontName))
                {
                    _logger.Error("No stamp font is specified.");
                    actionResult.Add(ErrorCode.Stamp_NoFont);
                }
            }
            return actionResult;
        }

        public ActionResult CheckEncryptionSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            var security = profile.PdfSettings.Security;
            if (security.Enabled)
            {
                if (profile.AutoSave.Enabled)
                {
                    if (string.IsNullOrEmpty(security.OwnerPassword))
                    {
                        _logger.Error("No saved owner password for security in automatic saving.");
                        actionResult.Add(ErrorCode.AutoSave_NoOwnerPassword);
                    }

                    if (security.RequireUserPassword)
                    {
                        if (string.IsNullOrEmpty(security.UserPassword))
                        {
                            _logger.Error("No saved user password for security in automatic saving.");
                            actionResult.Add(ErrorCode.AutoSave_NoUserPassword);
                        }
                    }
                }
            }

            return actionResult;
        }

        public ActionResult CheckBackgroundpageSettings(ConversionProfile profile)
        {
            var actionResult = new ActionResult();

            if ((profile.OutputFormat != OutputFormat.Pdf) && (profile.OutputFormat != OutputFormat.PdfA1B)
                && (profile.OutputFormat != OutputFormat.PdfA2B) && (profile.OutputFormat != OutputFormat.PdfX))
                return actionResult;

            if (profile.BackgroundPage.Enabled)
            {
                if (string.IsNullOrEmpty(profile.BackgroundPage.File))
                {
                    _logger.Error("No background file is specified.");
                    actionResult.Add(ErrorCode.Background_NoFileSpecified);
                }
                //Skip check for network path
                else if (!profile.BackgroundPage.File.StartsWith(@"\\") && !_file.Exists(profile.BackgroundPage.File))
                {
                    _logger.Error("The background file \"" + profile.BackgroundPage.File + "\" does not exist.");
                    actionResult.Add(ErrorCode.Background_FileDoesNotExist);
                }
                else if (!profile.BackgroundPage.File.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.Error("The background file \"" + profile.BackgroundPage.File + "\" is no pdf file.");
                    actionResult.Add(ErrorCode.Background_NoPdf);
                }
            }
            return actionResult;
        }

        public ActionResult CheckSignatureSettings(ConversionProfile profile, Accounts accounts)
        {
            var actionResult = new ActionResult();

            var sign = profile.PdfSettings.Signature;
            if (sign.Enabled)
            {
                if (string.IsNullOrEmpty(sign.CertificateFile))
                {
                    _logger.Error("Error in signing. Missing certification file.");
                    actionResult.Add(ErrorCode.ProfileCheck_NoCertificationFileSpecified);
                }
                //Skip check for network path
                else if (!sign.CertificateFile.StartsWith(@"\\") && !_file.Exists(sign.CertificateFile))
                {
                    _logger.Error("Error in signing. The certification file '" + sign.CertificateFile +
                                  "' doesn't exist.");
                    actionResult.Add(ErrorCode.ProfileCheck_CertificateFileDoesNotExist);
                }

                if (profile.AutoSave.Enabled)
                {
                    if (string.IsNullOrEmpty(sign.SignaturePassword))
                    {
                        _logger.Error("Automatic saving without certificate password.");
                        actionResult.Add(ErrorCode.ProfileCheck_AutoSaveWithoutCertificatePassword);
                    }
                }

                var timeServerAccount = accounts.GetTimeServerAccount(profile);
                if (timeServerAccount == null)
                {
                    _logger.Error("The specified time server account for signing is not configured.");
                    actionResult.Add(ErrorCode.Signature_NoTimeServerAccount);
                }
                else
                {
                    if (timeServerAccount.IsSecured)
                    {
                        if (string.IsNullOrEmpty(timeServerAccount.UserName))
                        {
                            _logger.Error("Secured Time Server without Login Name.");
                            actionResult.Add(ErrorCode.ProfileCheck_SecureTimeServerWithoutUsername);
                        }
                        if (string.IsNullOrEmpty(timeServerAccount.Password))
                        {
                            _logger.Error("Secured Time Server without Password.");
                            actionResult.Add(ErrorCode.ProfileCheck_SecureTimeServerWithoutPassword);
                        }
                    }
                }
            }

            return actionResult;
        }
    }
}
