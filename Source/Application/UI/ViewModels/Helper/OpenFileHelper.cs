using System;
using System.IO;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public interface IOpenFileInteractionHelper
    {
        string StartOpenFileInteraction(string currentPath, string title, string filter);
    }

    public class OpenFileInteractionHelper : IOpenFileInteractionHelper
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public OpenFileInteractionHelper(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }

        public string StartOpenFileInteraction(string currentPath, string title, string filter)
        {
            var interaction = new OpenFileInteraction();
            interaction.Title = title;
            interaction.Filter = filter;

            if (!string.IsNullOrWhiteSpace(currentPath))
            {
                interaction.FileName = currentPath;
                try
                {
                    interaction.InitialDirectory = Path.GetDirectoryName(currentPath);
                }
                catch (ArgumentException)
                {
                }
            }

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? interaction.FileName : currentPath;
        }
    }
}