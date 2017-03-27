using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class DropboxSharedLinksViewModel : InteractionAwareViewModelBase<DropboxSharedLinksInteraction>
    {
        private readonly IProcessStarter _processStarter;

        public DropboxSharedLinksViewModel(DropboxSharedLinksWindowTranslation translation, IProcessStarter processStarter)
        {
            _processStarter = processStarter;
            Translation = translation;
            OkCommand = new DelegateCommand(ExecuteOk);
            CopyCommand = new DelegateCommand(ExecuteCopy);
        }

        public DropboxSharedLinksWindowTranslation Translation { get; }
        public DropboxFileMetaData SharedLink { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand VisitWebsiteCommand => new DelegateCommand(VisitWebsiteExecute);
        private void VisitWebsiteExecute(object obj)
        {
            try
            {
                _processStarter.Start(SharedLink.SharedUrl);
            }
            catch
            {
                // ignored
            }
        }


        //Flag to trigger animated "copied successfull" hint
        public bool CopySucessfull { get; set; }

        private void ExecuteCopy(object obj)
        {
            Clipboard.SetText(string.Join(Environment.NewLine, Interaction.SharedLink.SharedUrl));
            CopySucessfull = true;
            RaisePropertyChanged(nameof(CopySucessfull));
        }

        private void ExecuteOk(object obj)
        {
            Interaction.Success = true;
            FinishInteraction();
        }
        protected override void HandleInteractionObjectChanged()
        {
            Interaction.SharedLink = Interaction.SharedLink;
            SharedLink = Interaction != null ? Interaction.SharedLink: new DropboxFileMetaData();
            RaisePropertyChanged(nameof(SharedLink));
        }
    }
}