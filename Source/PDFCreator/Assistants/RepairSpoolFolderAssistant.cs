using System;
using System.IO;
using NLog;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.Views;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Assistants
{
    internal class RepairSpoolFolderAssistant
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool TryRepairSpoolPath()
        {
            Logger.Error("The spool folder is not accessible due to a permission problem. PDFCreator will not work this way");

            string tempFolder = Path.GetFullPath(Path.Combine(JobInfoQueue.Instance.SpoolFolder, ".."));
            string username = Environment.UserName;

            Logger.Debug("UserName is {0}", username);

            string title = TranslationHelper.Instance.TranslatorInstance.GetTranslation("Application", "SpoolFolderAccessDenied", "Access Denied");
            string message = TranslationHelper.Instance.TranslatorInstance.GetFormattedTranslation("Application", "SpoolFolderAskToRepair", "The temporary path where PDFCreator stores the print jobs can't be accessed. This is a configuration problem on your machine and needs to be fixed. Do you want PDFCreator to attempt repairing it?\r\nYour spool folder is: {0}", tempFolder);

            Logger.Debug("Asking to start repair..");
            if (MessageWindow.ShowTopMost(message, title, MessageWindowButtons.YesNo, MessageWindowIcon.Exclamation) == MessageWindowResponse.Yes)
            {
                string repairToolPath = AppDomain.CurrentDomain.BaseDirectory;
                repairToolPath = Path.Combine(repairToolPath, "RepairFolderPermissions.exe");

                string repairToolParameters = string.Format("\"{0}\" \"{1}\"", username, tempFolder);

                Logger.Debug("RepairTool path is: {0}", repairToolPath);
                Logger.Debug("Parameters: {0}", repairToolParameters);

                if (!File.Exists(repairToolPath))
                {
                    Logger.Error("RepairFolderPermissions.exe does not exist!");
                    title = TranslationHelper.Instance.TranslatorInstance.GetTranslation("Application", "RepairToolNotFound", "RepairTool not found");
                    message = TranslationHelper.Instance.TranslatorInstance.GetFormattedTranslation("Application", "SetupFileMissing",
                        "An important PDFCreator file is missing ('{0}'). Please reinstall PDFCreator!",
                        Path.GetFileName(repairToolPath));

                    MessageWindow.ShowTopMost(message, title, MessageWindowButtons.OK, MessageWindowIcon.Error);
                    return false;
                }

                Logger.Debug("Starting RepairTool...");
                var shellExecuteHelper = new ShellExecuteHelper();
                var result = shellExecuteHelper.RunAsAdmin(repairToolPath, repairToolParameters);
                Logger.Debug("Done: {0}", result.ToString());
            }

            Logger.Debug("Now we'll check again, if the spool folder is not accessible");
            if (!JobInfoQueue.Instance.SpoolFolderIsAccessible())
            {
                Logger.Info("The spool folder could not be repaired.");
                title = TranslationHelper.Instance.TranslatorInstance.GetTranslation("Application", "SpoolFolderAccessDenied", "Access Denied");
                message = TranslationHelper.Instance.TranslatorInstance.GetFormattedTranslation("Application", "SpoolFolderUnableToRepair", "PDFCreator was not able to repair your spool folder. Please contact your administrator or the support to assist you in changing the permissions of the path '{0}'.", tempFolder);

                MessageWindow.ShowTopMost(message, title, MessageWindowButtons.OK, MessageWindowIcon.Exclamation);
                return false;
            }

            Logger.Info("The spool folder was repaired successfully");

            return true;
        }
    }
}
