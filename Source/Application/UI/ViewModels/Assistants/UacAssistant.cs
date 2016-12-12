using System;
using System.IO;
using System.Reflection;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
{
    public interface IUacAssistant
    {
        void AddExplorerIntegration();
        void RemoveExplorerIntegration();
        bool AddPrinter(string printerName);
        bool RenamePrinter(string oldPrinterName, string newPrinterName);
        bool DeletePrinter(string printerName);
        bool StoreLicesenForAllUsers();
    }

    public class UacAssistant : IUacAssistant
    {
        private readonly IInteractionInvoker _invoker;
        private readonly IShellExecuteHelper _shellExecuteHelper;
        private readonly IPDFCreatorNameProvider _pdfCreatorNameProvider;
        private readonly ITranslator _translator;

        public UacAssistant(ITranslator translator, IInteractionInvoker invoker, IShellExecuteHelper shellExecuteHelper, IPDFCreatorNameProvider pdfCreatorNameProvider)
        {
            _translator = translator;
            _invoker = invoker;
            _shellExecuteHelper = shellExecuteHelper;
            _pdfCreatorNameProvider = pdfCreatorNameProvider;
        }

        public void AddExplorerIntegration()
        {
            CallSetupHelper("/FileExtensions=Add");
        }

        public void RemoveExplorerIntegration()
        {
            CallSetupHelper("/FileExtensions=Remove");
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
        private void CallSetupHelper(string arguments)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (assemblyPath == null)
                return;

            var setupHelperPath = Path.Combine(assemblyPath, "SetupHelper.exe");

            if (!File.Exists(setupHelperPath))
            {
                var message = _translator.GetFormattedTranslation("Application", "SetupFileMissing",
                    Path.GetFileName(setupHelperPath));
                var caption = _translator.GetTranslation("ApplicationSettingsWindow", "Error");
                ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);
                return;
            }

            if (Environment.OSVersion.Version.Major <= 5)
            {
                var osHelper = new OsHelper();
                if (!osHelper.UserIsAdministrator())
                {
                    var message = _translator.GetTranslation("ApplicationSettingsWindow", "OperationRequiresAdminPrivileges");
                    var caption = _translator.GetTranslation("ApplicationSettingsWindow", "AdminPrivilegesRequired");

                    var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Info);
                    if (response == MessageResponse.No)
                        return;
                }
            }

            CallProgramAsAdmin(setupHelperPath, arguments);
        }

        private bool CallProgramAsAdmin(string path, string arguments)
        {
            var result = _shellExecuteHelper.RunAsAdmin(path, arguments);

            if (result == ShellExecuteResult.RunAsWasDenied)
            {
                var message = _translator.GetTranslation("ApplicationSettingsWindow", "SufficientPermissions");
                var caption = _translator.GetTranslation("ApplicationSettingsWindow", "Error");

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
                var message = _translator.GetFormattedTranslation("Application", "SetupFileMissing",
                    Path.GetFileName(applicationPath));
                var caption = _translator.GetTranslation("ApplicationSettingsWindow", "Error");
                ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);
                return null;
            }

            if (Environment.OSVersion.Version.Major <= 5)
            {
                var osHelper = new OsHelper();
                if (!osHelper.UserIsAdministrator())
                {
                    var message = _translator.GetTranslation("ApplicationSettingsWindow", "OperationRequiresAdminPrivileges");
                    var caption = _translator.GetTranslation("ApplicationSettingsWindow", "AdminPrivilegesRequired");

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

        public bool StoreLicesenForAllUsers()
        {
            return CallPDFCreator("/StoreLicenseForAllUsers");
        }

        private bool CallPDFCreator(string arguments)
        {
            var pdfCreatorName = _pdfCreatorNameProvider.GetName(); 
            var applicationPath = GetApplicationPath(pdfCreatorName);
            if (applicationPath == null)
                return false;

            return CallProgramAsAdmin(applicationPath, arguments);
        }
    }
}