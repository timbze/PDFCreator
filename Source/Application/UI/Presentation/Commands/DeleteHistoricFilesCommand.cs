using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class DeleteHistoricFilesCommand : TranslatableViewModelBase<DeleteFilesTranslation>, IWaitableCommand
    {
        private readonly IFile _file;
        private readonly IInteractionRequest _interactionRequest;

        public DeleteHistoricFilesCommand(IFile file, IInteractionRequest interactionRequest, ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _file = file;
            _interactionRequest = interactionRequest;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _historicFiles = parameter as IList<HistoricFile>;
            if (_historicFiles == null)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Error));
                return;
            }
            DeleteFilesWithQuery();
        }

        private IList<HistoricFile> _historicFiles;

        private void DeleteFilesWithQuery()
        {
            const int maxDisplayFiles = 5;
            var title = Translation.GetDeleteFilesTitle(_historicFiles.Count);
            var message = Translation.GetAreYouSureYouWantToDeleteFilesMessage(_historicFiles.Count);
            foreach (var historicFile in _historicFiles.Take(maxDisplayFiles))
            {
                message += "\r\n" + historicFile.Path;
            }

            var remainingFiles = _historicFiles.Skip(maxDisplayFiles).Count();
            if (remainingFiles > 0)
            {
                message += "\r\n" + Translation.GetAndXMoreMessage(remainingFiles);
            }

            var interaction = new MessageInteraction(message, title, MessageOptions.YesNo, MessageIcon.Question);

            _interactionRequest.Raise(interaction, DeleteFilesCallback);
        }

        private void DeleteFilesCallback(MessageInteraction interaction)
        {
            if (interaction.Response != MessageResponse.Yes)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
                return;
            }

            var notDeletedFiles = new List<HistoricFile>();
            foreach (var historicFile in _historicFiles)
            {
                try
                {
                    if (_file.Exists(historicFile.Path))
                        _file.Delete(historicFile.Path);
                }
                catch
                {
                    notDeletedFiles.Add(historicFile);
                }
            }

            if (notDeletedFiles.Count > 0)
                NotfiyUserAboutNotDeletedFiles(notDeletedFiles);

            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private void NotfiyUserAboutNotDeletedFiles(List<HistoricFile> notDeletedFiles)
        {
            var title = Translation.ErrorDuringDeletionTitle;
            var message = Translation.GetCouldNotDeleteTheFollowingFilesMessage(notDeletedFiles.Count);
            foreach (var historicFile in notDeletedFiles)
            {
                message += "\r\n" + historicFile.Path;
            }
            var interaction = new MessageInteraction(message, title, MessageOptions.OK, MessageIcon.Error);
            _interactionRequest.Raise(interaction);
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
