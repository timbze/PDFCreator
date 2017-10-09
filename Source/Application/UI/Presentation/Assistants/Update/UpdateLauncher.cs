using System;
using System.Diagnostics;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public interface IUpdateLauncher
    {
        void LaunchUpdate(ApplicationVersion version);
    }

    public class SimpleUpdateLauncher : IUpdateLauncher
    {
        public void LaunchUpdate(ApplicationVersion version)
        {
            Process.Start(Urls.PDFCreatorDownloadUrl);
        }
    }

    public class AutoUpdateLauncher : IUpdateLauncher
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IHashUtil _hashUtil;
        private readonly IThreadManager _threadManager;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private UpdateManagerTranslation _translation;

        public AutoUpdateLauncher(ITranslationFactory translationFactory, IInteractionInvoker interactionInvoker, IHashUtil hashUtil, IThreadManager threadManager, ApplicationNameProvider applicationNameProvider)
        {
            UpdateTranslation(translationFactory);
            translationFactory.TranslationChanged += (sender, args) => UpdateTranslation(translationFactory);
            _interactionInvoker = interactionInvoker;
            _hashUtil = hashUtil;
            _threadManager = threadManager;
            _applicationNameProvider = applicationNameProvider;
        }

        public void LaunchUpdate(ApplicationVersion version)
        {
            var caption = _translation.GetFormattedTitle(_applicationNameProvider.ApplicationName);

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

                    var message = _translation.DownloadHashErrorMessage;
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
                var message = _translation.DownloadErrorMessage;
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

        private void UpdateTranslation(ITranslationFactory translationFactory)
        {
            _translation = translationFactory.CreateTranslation<UpdateManagerTranslation>();
        }
    }
}
