using System;
using System.ComponentModel;
using System.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Ftp;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public class FtpAction : IAction, ICheckable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IFtpConnectionFactory _ftpConnectionFactory;
        private readonly IPathUtil _pathUtil;
        private readonly IFtpPasswordProvider _passwordProvider;

        public FtpAction(IFtpConnectionFactory ftpConnectionFactory, IPathUtil pathUtil, IFtpPasswordProvider passwordProvider)
        {
            _ftpConnectionFactory = ftpConnectionFactory;
            _pathUtil = pathUtil;
            _passwordProvider = passwordProvider;
        }

        /// <summary>
        ///     Upload all output files with ftp
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(Job job)
        {
            Logger.Debug("Launched ftp-Action");
            try
            {
                var result = FtpUpload(job);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while upload file to ftp:\r\n" + ex.Message);
                return new ActionResult(ErrorCode.Ftp_GenericError);
            }
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.Ftp.Enabled;
        }

        public bool Init(Job job)
        {
            return _passwordProvider.SetPassword(job);
        }

        /// <summary>
        ///     Check if the profile is configured properly for this action
        /// </summary>
        /// <param name="profile">The profile to check</param>
        /// <returns>ActionResult with configuration problems</returns>
        public ActionResult Check(ConversionProfile profile, Accounts accounts)
        {
            var actionResult = new ActionResult();
            if (profile.Ftp.Enabled)
            {
                if (string.IsNullOrEmpty(profile.Ftp.Server))
                {
                    Logger.Error("No FTP server specified.");
                    actionResult.Add(ErrorCode.Ftp_NoServer);
                }

                if (string.IsNullOrEmpty(profile.Ftp.UserName))
                {
                    Logger.Error("No FTP username specified.");
                    actionResult.Add(ErrorCode.Ftp_NoUser);
                }

                if (profile.AutoSave.Enabled)
                {
                    if (string.IsNullOrEmpty(profile.Ftp.Password))
                    {
                        Logger.Error("Automatic saving without ftp password.");
                        actionResult.Add(ErrorCode.Ftp_AutoSaveWithoutPassword);
                    }
                }
            }
            return actionResult;
        }

        private ActionResult FtpUpload(Job job)
        {
            var actionResult = Check(job.Profile, job.Accounts);
            if (!actionResult)
            {
                Logger.Error("Canceled FTP upload action.");
                return actionResult;
            }

            if (string.IsNullOrEmpty(job.Passwords.FtpPassword))
            {
                Logger.Error("No ftp password specified in action");
                return new ActionResult(ErrorCode.Ftp_NoPassword);
            }

            Logger.Debug("Creating ftp connection.\r\nServer: " + job.Profile.Ftp.Server + "\r\nUsername: " +
                         job.Profile.Ftp.UserName);

            var ftpConnection = _ftpConnectionFactory.BuilConnection(job.Profile.Ftp.Server, job.Profile.Ftp.UserName, job.Passwords.FtpPassword);

            try
            {
                ftpConnection.Open();
                ftpConnection.Login();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode.Equals(12007))
                {
                    Logger.Error("Can not connect to the internet for login to ftp. Win32Exception Message:\r\n" + ex.Message);
                    ftpConnection.Close();
                    return new ActionResult(ErrorCode.Ftp_ConnectionError);
                }
                else if(ex.NativeErrorCode.Equals(12014))
                {
                    Logger.Error("Can not login to ftp because the password is incorrect. Win32Exception Message:\r\n" + ex.Message);
                    ftpConnection.Close();
                    var retypeResult = _passwordProvider.RetypePassword(job);
                    if (retypeResult)
                    {
                        return FtpUpload(job);
                    }
                    else
                    {
                        return retypeResult;
                    }
                }

                Logger.Error("Win32Exception while login to ftp server:\r\n" + ex.Message);
                ftpConnection.Close();
                return new ActionResult(ErrorCode.Ftp_LoginError);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while login to ftp server:\r\n" + ex.Message);
                ftpConnection.Close();
                return new ActionResult(ErrorCode.Ftp_LoginError);
            }

            var fullDirectory = job.TokenReplacer.ReplaceTokens(job.Profile.Ftp.Directory).Trim();
            if (!IsValidPath(fullDirectory))
            {
                Logger.Warn("Directory contains invalid characters \"" + fullDirectory + "\"");
                fullDirectory = MakeValidPath(fullDirectory);
            }

            Logger.Debug("Directory on ftp server: " + fullDirectory);

            var directories = fullDirectory.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var directory in directories)
                {
                    if (!ftpConnection.DirectoryExists(directory))
                    {
                        Logger.Debug("Create folder: " + directory);
                        ftpConnection.CreateDirectory(directory);
                    }
                    Logger.Debug("Move to: " + directory);
                    ftpConnection.SetCurrentDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while setting directory on ftp server\r\n:" + ex.Message);
                ftpConnection.Close();
                return new ActionResult(ErrorCode.Ftp_DirectoryError);
            }

            foreach (var file in job.OutputFiles)
            {
                var targetFile = Path.GetFileName(file);
                targetFile = MakeValidPath(targetFile);
                if (job.Profile.Ftp.EnsureUniqueFilenames)
                {
                    Logger.Debug("Make unique filename for " + targetFile);
                    try
                    {
                        var uf = new UniqueFilenameForFtp(targetFile, ftpConnection, _pathUtil);
                        targetFile = uf.CreateUniqueFileName();
                        Logger.Debug("-> The unique filename is \"" + targetFile + "\"");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Exception while generating unique filename\r\n:" + ex.Message);
                        ftpConnection.Close();
                        return new ActionResult(ErrorCode.Ftp_DirectoryReadError);
                    }
                }

                try
                {
                    ftpConnection.PutFile(file, targetFile);
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception while uploading the file \"" + file + "\": \r\n" + ex.Message);
                    ftpConnection.Close();
                    return new ActionResult(ErrorCode.Ftp_UploadError);
                }
            }

            ftpConnection.Close();
            return new ActionResult();
        }

        private bool IsValidPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var validName = new ValidName();
            return validName.IsValidFtpPath(path);
        }

        private string MakeValidPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var validName = new ValidName();
            return validName.MakeValidFtpPath(path);
        }
    }
}