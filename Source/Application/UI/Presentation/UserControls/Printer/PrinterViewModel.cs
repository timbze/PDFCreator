using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class PrinterViewModel : TranslatableViewModelBase<PrinterTabTranslation>
    {
        private readonly ConversionProfile _dummyLastUsedProfile = new ConversionProfile
        {
            Name = "<Last used profile>",
            Guid = ""
        };

        private readonly IOsHelper _osHelper;
        private readonly IPrinterActionsAssistant _printerActionsAssistant;
        private readonly IPrinterHelper _printerHelper;
        private readonly IGpoSettings _gpoSettings;

        private readonly IPrinterProvider _printerProvider;

        private ApplicationSettings _applicationSettings;

        private IList<ConversionProfile> _conversionProfiles;
        private ConversionProfile _defaultProfile;
        private ICollection<string> _pdfCreatorPrinters;
        private Helper.SynchronizedCollection<PrinterMappingWrapper> _printerMappings;
        private ICollectionView _printerMappingView;

        public PrinterViewModel(IPrinterProvider printerProvider, IPrinterActionsAssistant printerActionsAssistant, IOsHelper osHelper, TranslationHelper translationHelper, ITranslationUpdater translationUpdater, IPrinterHelper printerHelper, ISettingsProvider settingsProvider, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _osHelper = osHelper;
            _printerHelper = printerHelper;
            _gpoSettings = gpoSettings;
            _printerActionsAssistant = printerActionsAssistant;
            _printerProvider = printerProvider;

            AddPrinterCommand = new DelegateCommand(AddPrintercommandExecute);
            RenamePrinterCommand = new DelegateCommand(RenamePrinterCommandExecute, ModifyPrinterCommandCanExecute);
            DeletePrinterCommand = new DelegateCommand(DeletePrinterCommandExecute, ModifyPrinterCommandCanExecute);
            SetPrimaryPrinterCommand = new DelegateCommand(SetPrimaryPrinter);

            SetSettingsAndRaiseNotifications(settingsProvider.Settings, gpoSettings);
        }

        private void SetPrimaryPrinter(object parameter)
        {
            var selectedPrinter = parameter as PrinterMappingWrapper;

            if (selectedPrinter == null)
                return;

            PrimaryPrinter = selectedPrinter.PrinterName;
        }

        public Visibility RequiresUacVisibility
        {
            get { return _osHelper.UserIsAdministrator() ? Visibility.Collapsed : Visibility.Visible; }
        }

        public IList<ConversionProfile> ConversionProfiles
        {
            get { return _conversionProfiles; }
            set
            {
                _conversionProfiles = value;
                RaisePropertyChanged(nameof(ConversionProfiles));

                _defaultProfile = ConversionProfiles.FirstOrDefault(x => x.Guid == ProfileGuids.DEFAULT_PROFILE_GUID);
                if (_defaultProfile == null)
                    _defaultProfile = _dummyLastUsedProfile;
            }
        }

        public IEnumerable<ConversionProfile> PrinterMappingProfiles
        {
            get
            {
                var profiles = _conversionProfiles.ToList();
                _dummyLastUsedProfile.Name = "<" + Translation.LastUsedProfileMapping + ">";
                profiles.Insert(0, _dummyLastUsedProfile);
                return profiles;
            }
        }

        public ICollection<string> PdfCreatorPrinters
        {
            get { return _pdfCreatorPrinters; }
            set
            {
                _pdfCreatorPrinters = value;
                RaisePropertyChanged(nameof(PdfCreatorPrinters));
            }
        }

        public ObservableCollection<PrinterMappingWrapper> PrinterMappings
        {
            get { return _printerMappings?.ObservableCollection; }
        }

        public ICommand AddPrinterCommand { get; private set; }
        public ICommand RenamePrinterCommand { get; }
        public ICommand DeletePrinterCommand { get; }
        public DelegateCommand SetPrimaryPrinterCommand { get; }

        public string PrimaryPrinter
        {
            get
            {
                if (_applicationSettings == null)
                    return null;
                if (string.IsNullOrEmpty(_applicationSettings.PrimaryPrinter) ||
                    PrinterMappings.All(o => o.PrinterName != _applicationSettings.PrimaryPrinter))
                {
                    _applicationSettings.PrimaryPrinter = _printerHelper.GetApplicablePDFCreatorPrinter("PDFCreator",
                        "PDFCreator");
                }

                return _applicationSettings.PrimaryPrinter;
            }
            set
            {
                _applicationSettings.PrimaryPrinter = value;
                UpdatePrimaryPrinter(_applicationSettings.PrimaryPrinter);
                RaisePropertyChanged(nameof(PrimaryPrinter));
            }
        }

        private void SetSettingsAndRaiseNotifications(PdfCreatorSettings settings, IGpoSettings gpoSettings)
        {
            if (settings == null)
                return;

            _applicationSettings = settings.ApplicationSettings;
            ConversionProfiles = settings.ConversionProfiles;

            RaisePropertyChanged(nameof(_applicationSettings));
            RaisePropertyChanged(nameof(ConversionProfiles));

            UpdatePrinterList();
            ApplyPrinterMappings();
            UpdatePrinterCollectionViews();

            RaisePropertyChanged(nameof(PrimaryPrinter));
            UpdatePrimaryPrinter(PrimaryPrinter);
        }

        private void PrinterMappings_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _applicationSettings.PrinterMappings.Clear();

            foreach (var printerMappingWrapper in PrinterMappings)
            {
                _applicationSettings.PrinterMappings.Add(printerMappingWrapper.PrinterMapping);
                if (printerMappingWrapper.Profile == null)
                    printerMappingWrapper.Profile = _defaultProfile;
            }

            RaisePrinterCommandsCanExecuteChanged();
        }

        private void PrinterMappingView_OnCurrentChanged(object sender, EventArgs eventArgs)
        {
            RaisePrinterCommandsCanExecuteChanged();
        }

        private void RaisePrinterCommandsCanExecuteChanged()
        {
            var delegateCommand = RenamePrinterCommand as DelegateCommand;
            delegateCommand?.RaiseCanExecuteChanged();

            var command = DeletePrinterCommand as DelegateCommand;
            command?.RaiseCanExecuteChanged();
        }

        private void UpdatePrinterList()
        {
            if (_pdfCreatorPrinters == null)
                _pdfCreatorPrinters = new List<string>();

            _pdfCreatorPrinters.Clear();
            foreach (var printer in _printerProvider.GetPDFCreatorPrinters())
            {
                _pdfCreatorPrinters.Add(printer);
            }

            RaisePropertyChanged(nameof(PdfCreatorPrinters));
        }

        public void UpdatePrimaryPrinter(string printerName)
        {
            foreach (var printerMappingWrapper in _printerMappings.ObservableCollection)
            {
                printerMappingWrapper.PrimaryPrinter = printerName;
            }
        }

        private void ApplyPrinterMappings()
        {
            if (_applicationSettings?.PrinterMappings != null)
            {
                var mappingWrappers = new List<PrinterMappingWrapper>();

                foreach (var printerMapping in _applicationSettings.PrinterMappings)
                {
                    var mappingWrapper = new PrinterMappingWrapper(printerMapping, ConversionProfiles);
                    if (printerMapping.ProfileGuid == _dummyLastUsedProfile.Guid)
                    {
                        mappingWrapper.Profile = _dummyLastUsedProfile;
                    }
                    else if (mappingWrapper.Profile == null)
                    {
                        mappingWrapper.Profile = _defaultProfile;
                    }
                    mappingWrappers.Add(mappingWrapper);
                }

                _printerMappings = new Helper.SynchronizedCollection<PrinterMappingWrapper>(mappingWrappers);

                _printerMappings.ObservableCollection.CollectionChanged += PrinterMappings_OnCollectionChanged;
                _printerMappingView = CollectionViewSource.GetDefaultView(_printerMappings.ObservableCollection);
                _printerMappingView.CurrentChanged += PrinterMappingView_OnCurrentChanged;
            }

            RaisePropertyChanged(nameof(PrinterMappings));
        }

        private bool ModifyPrinterCommandCanExecute(object o)
        {
            var currentMapping = _printerMappingView?.CurrentItem as PrinterMappingWrapper;

            if (currentMapping == null)
                return false;

            return PdfCreatorPrinters.Contains(currentMapping.PrinterName);
        }

        private async void AddPrintercommandExecute(object o)
        {
            string printerName = await _printerActionsAssistant.AddPrinter();

            if (string.IsNullOrEmpty(printerName))
                return;

            if (!string.IsNullOrWhiteSpace(printerName))
            {
                PrinterMappings.Add(
                    new PrinterMappingWrapper(new PrinterMapping(printerName, ProfileGuids.DEFAULT_PROFILE_GUID),
                        ConversionProfiles));
            }

            UpdatePrinterCollectionViews();
        }

        private async void RenamePrinterCommandExecute(object obj)
        {
            var currentMapping = _printerMappingView.CurrentItem as PrinterMappingWrapper;

            if (currentMapping == null)
                return;

            var wasPrimaryPrinter = currentMapping.IsPrimaryPrinter;

            string newPrinterName = await _printerActionsAssistant.RenamePrinter(currentMapping.PrinterName);

            if (newPrinterName != null)
            {
                PdfCreatorPrinters = _printerProvider.GetPDFCreatorPrinters();
                currentMapping.PrinterName = newPrinterName;
                if (wasPrimaryPrinter)
                    PrimaryPrinter = newPrinterName;
            }

            RaisePropertyChanged(nameof(PdfCreatorPrinters));
            RaisePropertyChanged(nameof(PrimaryPrinter));
        }

        private async void DeletePrinterCommandExecute(object obj)
        {
            var currentMapping = _printerMappingView.CurrentItem as PrinterMappingWrapper;

            if (currentMapping == null)
                return;

            var success = await _printerActionsAssistant.DeletePrinter(currentMapping.PrinterName, PdfCreatorPrinters.Count);

            if (success)
            {
                PrinterMappings.Remove(currentMapping);
                PdfCreatorPrinters = _printerProvider.GetPDFCreatorPrinters();

                RaisePropertyChanged(nameof(PrimaryPrinter));
                RaisePropertyChanged(nameof(PrinterMappings));
                RaisePropertyChanged(nameof(PdfCreatorPrinters));

                PrimaryPrinter = _printerHelper.GetApplicablePDFCreatorPrinter("");
            }
        }

        public bool PrinterIsDisabled
        {
            get
            {
                if (_applicationSettings == null)
                    return false;

                return _gpoSettings != null && (_gpoSettings.DisablePrinterTab || _gpoSettings.DisableApplicationSettings);
            }
        }

        private void UpdatePrinterCollectionViews()
        {
            UpdatePrinterList();
            CollectionViewSource.GetDefaultView(_applicationSettings.PrinterMappings).Refresh();
            CollectionViewSource.GetDefaultView(PdfCreatorPrinters).Refresh();
        }
    }
}
