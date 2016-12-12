using System;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Interaction.DialogInteractions;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimeInteractionInvoker : IInteractionInvoker
    {
        public void Invoke<T>(T interaction) where T : IInteraction
        {
            throw new NotImplementedException();
        }

        public void InvokeNonBlocking<T>(T interaction, Action<T> callback) where T : IInteraction
        {
            throw new NotImplementedException();
        }

        public void InvokeNonBlocking<T>(T interaction) where T : IInteraction
        {
            throw new NotImplementedException();
        }

        public void Invoke(SaveFileInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(FolderBrowserInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(ColorInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(FontInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(OpenFileInteraction interaction)
        {
            throw new NotImplementedException();
        }
    }
}
