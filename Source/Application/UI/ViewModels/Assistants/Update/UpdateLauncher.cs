using System;
using System.Diagnostics;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants.Update
{
    public interface IUpdateLauncher
    {
        void LaunchUpdate(ApplicationVersion version);
    }

    public class SimpleUpdateLauncher : IUpdateLauncher
    {
        public void LaunchUpdate(ApplicationVersion version)
        {
            Process.Start(version.DownloadUrl);
        }
    }

    public class AutoUpdateLauncher : IUpdateLauncher
    {
        private readonly ITranslator _translator;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IHashUtil _hashUtil;
        private readonly IThreadManager _threadManager;

        public AutoUpdateLauncher(ITranslator translator, IInteractionInvoker interactionInvoker, IHashUtil hashUtil, IThreadManager threadManager)
        {
            _translator = translator;
            _interactionInvoker = interactionInvoker;
            _hashUtil = hashUtil;
            _threadManager = threadManager;
        }

        public void LaunchUpdate(ApplicationVersion version)
        {
            var caption = _translator.GetTranslation("UpdateManager", "PDFCreatorUpdate");

            try
            {
                var done = false;

                while (!done)
                {
                    var interaction = new UpdateDownloadInteraction(version.DownloadUrl);
                    _interactionInvoker.Invoke(interaction);

                    var result = interaction.Success;

                    if (result != true)
                    {
                        done = true;
                        continue;
                    }

                    if (_hashUtil.VerifyFileMd5(interaction.DownloadedFile, version.FileHash))
                    {
                        done = true;
                        _threadManager.UpdateAfterShutdownAction =
                            () => Process.Start(interaction.DownloadedFile);
                        continue;
                    }

                    var message = _translator.GetTranslation("UpdateManager", "DownloadHashErrorMessage");
                    var res = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Warning);

                    if (res != MessageResponse.No)
                        continue;

                    _threadManager.UpdateAfterShutdownAction =
                        () => Process.Start(interaction.DownloadedFile);
                    done = true;
                }
            }
            catch (Exception)
            {
                var message = _translator.GetTranslation("UpdateManager", "DownloadErrorMessage");
                var res = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.PDFCreator);

                if (res == MessageResponse.Yes)
                {
                    Process.Start(version.DownloadUrl);
                }
            }
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
