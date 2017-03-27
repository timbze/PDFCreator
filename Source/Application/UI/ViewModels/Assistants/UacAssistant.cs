using System;
using System.IO;
using System.Reflection;
using System.Text;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
{
    public interface IUacAssistant
    {
        bool AddExplorerIntegration();
        bool RemoveExplorerIntegration();
        bool AddPrinter(string printerName);
        bool RenamePrinter(string oldPrinterName, string newPrinterName);
        bool DeletePrinter(string printerName);
        bool StoreLicenseForAllUsers(string licenseServerCode, string licenseKey);
    }

    public class UacAssistant : IUacAssistant
    {
        private readonly IInteractionInvoker _invoker;
        private readonly IShellExecuteHelper _shellExecuteHelper;
        private readonly IPDFCreatorNameProvider _pdfCreatorNameProvider;
        private readonly ApplicationSettingsWindowTranslation _translation;

        public UacAssistant(ApplicationSettingsWindowTranslation translation, IInteractionInvoker invoker, IShellExecuteHelper shellExecuteHelper, IPDFCreatorNameProvider pdfCreatorNameProvider)
        {
            _translation = translation;
            _invoker = invoker;
            _shellExecuteHelper = shellExecuteHelper;
            _pdfCreatorNameProvider = pdfCreatorNameProvider;
        }

        public bool AddExplorerIntegration()
        {
           return CallSetupHelper("/FileExtensions=Add");
        }

        public bool RemoveExplorerIntegration()
        {
           return CallSetupHelper("/FileExtensions=Remove");
        }

        public bool AddPrinter(string printerName)
        {
            return CallPrinterHelper("/AddPrinter \"" + printerName + "\"");
        }

        public bool RenamePrinter(string oldPrinterName, string newPrinterName)
        {
            return CallPrinterHelper("/RenamePrinter \"" + oldPrinterName + "\" \"" + newPrinterName + "\"");
        }

        public bool DeletePrinter(string printerName)
        {
            return CallPrinterHelper("/DeletePrinter \"" + printerName + "\"");
        }

        /// <summary>
        ///     Call the SetupHelper.exe to add or remove explorer context menu integration
        /// </summary>
        /// <param name="arguments">Command line arguments that will be passed to SetupHelper.exe</param>
        /// <returns>true, if the action was successful</returns>
        private bool CallSetupHelper(string arguments)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (assemblyPath == null)
                return false;

            var setupHelperPath = Path.Combine(assemblyPath, "SetupHelper.exe");

            if (!File.Exists(setupHelperPath))
            {
                var message = _translation.GetFormattedSetupFileMissing(Path.GetFileName(setupHelperPath));
                var caption = _translation.Error;
                ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);
                return false;
            }

            if (Environment.OSVersion.Version.Major <= 5)
            {
                var osHelper = new OsHelper();
                if (!osHelper.UserIsAdministrator())
                {
                    var message = _translation.OperationRequiresAdminPrivileges;
                    var caption = _translation.AdminPrivilegesRequired;

                    var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Info);
                    if (response == MessageResponse.No)
                        return false;
                }
            }

            return CallProgramAsAdmin(setupHelperPath, arguments);
        }

        private bool CallProgramAsAdmin(string path, string arguments)
        {
            var result = _shellExecuteHelper.RunAsAdmin(path, arguments);

            if (result == ShellExecuteResult.RunAsWasDenied)
            {
                var message = _translation.SufficientPermissions;
                var caption = _translation.Error;

                ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);

                return false;
            }

            return result == ShellExecuteResult.Success;
        }

        /// <summary>
        ///     Call the PrinterHelper.exe to add, rename or delete printers
        /// </summary>
        /// <param name="arguments">Command line arguments that will be passed to PrinterHelper.exe</param>
        /// <returns>true, if the action was successful</returns>
        private bool CallPrinterHelper(string arguments)
        {
            var applicationPath = GetApplicationPath("PrinterHelper.exe");
            if (applicationPath == null)
                return false;

            return CallProgramAsAdmin(applicationPath, arguments);
        }

        private string GetApplicationPath(string applicationName)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyPath == null)
                return null;

            var applicationPath = Path.Combine(assemblyPath, applicationName);

            if (!File.Exists(applicationPath))
            {
                var message = _translation.GetFormattedSetupFileMissing(Path.GetFileName(applicationPath));
                var caption = _translation.Error;
                ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);
                return null;
            }

            if (Environment.OSVersion.Version.Major <= 5)
            {
                var osHelper = new OsHelper();
                if (!osHelper.UserIsAdministrator())
                {
                    var message = _translation.OperationRequiresAdminPrivileges;
                    var caption = _translation.AdminPrivilegesRequired;

                    var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Info);
                    if (response == MessageResponse.No)
                        return null;
                }
            }

            return applicationPath;
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _invoker.Invoke(interaction);
            return interaction.Response;
        }

        public bool StoreLicenseForAllUsers(string licenseServerCode, string licenseKey)
        {
            var lsaBase64 = Convert.ToBase64String(Encoding.Default.GetBytes(licenseServerCode));
            return CallPDFCreator($"/StoreLicenseForAllUsers /LicenseServerCode=\"{lsaBase64}\" /LicenseKey=\"{licenseKey}\"");
        }

        private bool CallPDFCreator(string arguments)
        {
            var pdfCreatorName = _pdfCreatorNameProvider.GetExeName(); 
            var applicationPath = GetApplicationPath(pdfCreatorName);
            if (applicationPath == null)
                return false;

            return CallProgramAsAdmin(applicationPath, arguments);
        }
    }
}