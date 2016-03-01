using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.ViewModels.Wrapper;
using pdfforge.PDFCreator.ViewModels.Wrapper;

namespace pdfforge.PDFCreator.ViewModels.UserControls
{
    internal class PrinterTabViewModel : ApplicationSettingsViewModel
    { 
        private readonly ConversionProfile _dummyLastUsedProfile = new ConversionProfile
        {
            Name = "<Last used profile>",
            Guid = ""
        };
        
        private IEnumerable<ConversionProfile> _conversionProfiles;
        private ICollection<string> _pdfCreatorPrinters;
        private SynchronizedCollection<PrinterMappingWrapper> _printerMappings;
        private ICollectionView _printerMappingView;
        private ConversionProfile _defaultProfile;
        private TranslationHelper _translationHelper;

        public PrinterTabViewModel(ApplicationSettings applicationSettings, IEnumerable<ConversionProfile> profiles,
            Func<ICollection<string>> fetchPrintersFunc, TranslationHelper translationHelper, Edition edition)
            : base(edition)
        {
            SettingsChanged += OnSettingsChanged;

            _translationHelper = translationHelper;

            var helper = new Shared.Helper.PrinterHelper();
            GetPrinterListAction = fetchPrintersFunc ?? helper.GetPDFCreatorPrinters;

            ConversionProfiles = profiles;
            ApplicationSettings = applicationSettings;

            AddPrinterCommand = new DelegateCommand(AddPrintercommandExecute);
            RenamePrinterCommand = new DelegateCommand(RenamePrinterCommandExecute, ModifyPrinterCommandCanExecute);
            DeletePrinterCommand = new DelegateCommand(DeletePrinterCommandExecute, ModifyPrinterCommandCanExecute);
        }

        public PrinterTabViewModel()
            : this(null, new List<ConversionProfile>(), null, TranslationHelper.Instance, EditionFactory.Instance.Edition)
        {
        }

        public IEnumerable<ConversionProfile> ConversionProfiles
        {
            get { return _conversionProfiles; }
            set
            {
                _conversionProfiles = value;
                RaisePropertyChanged("ConversionProfiles");

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
                _dummyLastUsedProfile.Name = _translationHelper.TranslatorInstance.GetTranslation("ApplicationSettingsWindow", "LastUsedProfileMapping", "<Last used profile>");
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
                RaisePropertyChanged("PdfCreatorPrinters");
            }
        }

        public ObservableCollection<PrinterMappingWrapper> PrinterMappings
        {
            get
            {
                if (_printerMappings == null)
                    return null;
                return _printerMappings.ObservableCollection;
            }
        }

        public Func<string> AddPrinterAction { private get; set; }
        public Action<PrinterMappingWrapper> RenamePrinterAction { private get; set; }
        public Action<PrinterMappingWrapper> DeletePrinterAction { private get; set; }
        public Func<ICollection<string>> GetPrinterListAction { get; set; }
        public DelegateCommand AddPrinterCommand { get; private set; }
        public DelegateCommand RenamePrinterCommand { get; private set; }
        public DelegateCommand DeletePrinterCommand { get; private set; }

        public string PrimaryPrinter
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
            get
            {
                if (ApplicationSettings == null)
                    return null;
                if (string.IsNullOrEmpty(ApplicationSettings.PrimaryPrinter) ||
                    PrinterMappings.All(o => o.PrinterName != ApplicationSettings.PrimaryPrinter))
                {
                    var printerHelper = new Shared.Helper.PrinterHelper();
                    ApplicationSettings.PrimaryPrinter = printerHelper.GetApplicablePDFCreatorPrinter("PDFCreator", "PDFCreator");
                }     

                return ApplicationSettings.PrimaryPrinter;
            }
            set
            {
                ApplicationSettings.PrimaryPrinter = value;
                UpdatePrimaryPrinter(ApplicationSettings.PrimaryPrinter);
                RaisePropertyChanged("PrimaryPrinter");
            }
        }

        private void OnSettingsChanged(object sender, EventArgs eventArgs)
        {
            if (ApplicationSettings == null)
                return;

            UpdatePrinterList();
            ApplyPrinterMappings();
            UpdatePrinterCollectionViews();
            RaisePropertyChanged("PrimaryPrinter");
        }

        private void PrinterMappings_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ApplicationSettings.PrinterMappings.Clear();

            foreach (var printerMappingWrapper in PrinterMappings)
            {
                ApplicationSettings.PrinterMappings.Add(printerMappingWrapper.PrinterMapping);
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
            RenamePrinterCommand.RaiseCanExecuteChanged();
            DeletePrinterCommand.RaiseCanExecuteChanged();
        }

        private void UpdatePrinterList()
        {
            if (GetPrinterListAction == null)
                return;

            if (_pdfCreatorPrinters == null)
                _pdfCreatorPrinters = new List<string>();

            _pdfCreatorPrinters.Clear();
            foreach (var printer in GetPrinterListAction())
            {
                _pdfCreatorPrinters.Add(printer);
            }

            RaisePropertyChanged("PdfCreatorPrinters");
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
            if (ApplicationSettings != null && ApplicationSettings.PrinterMappings != null)
            {
                var mappingWrappers = new List<PrinterMappingWrapper>();

                foreach (var printerMapping in ApplicationSettings.PrinterMappings)
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

                _printerMappings = new SynchronizedCollection<PrinterMappingWrapper>(mappingWrappers);

                _printerMappings.ObservableCollection.CollectionChanged += PrinterMappings_OnCollectionChanged;
                _printerMappingView = CollectionViewSource.GetDefaultView(_printerMappings.ObservableCollection);
                _printerMappingView.CurrentChanged += PrinterMappingView_OnCurrentChanged;
            }

            RaisePropertyChanged("PrinterMappings");
        }

        private bool ModifyPrinterCommandCanExecute(object o)
        {
            if (_printerMappingView == null)
                return false;

            var currentMapping = _printerMappingView.CurrentItem as PrinterMappingWrapper;

            if (currentMapping == null)
                return false;

            return PdfCreatorPrinters.Contains(currentMapping.PrinterName);
        }

        private void AddPrintercommandExecute(object o)
        {
            var printerName = AddPrinterAction();

            if (!string.IsNullOrWhiteSpace(printerName))
            {
                PrinterMappings.Add(
                    new PrinterMappingWrapper(new PrinterMapping(printerName, ProfileGuids.DEFAULT_PROFILE_GUID), ConversionProfiles));
            }

            UpdatePrinterCollectionViews();
        }

        private void RenamePrinterCommandExecute(object obj)
        {
            var currentMapping = _printerMappingView.CurrentItem as PrinterMappingWrapper;

            if (currentMapping == null)
                return;

            RenamePrinterAction(currentMapping);

            RaisePropertyChanged("PrimaryPrinter");
        }

        private void DeletePrinterCommandExecute(object obj)
        {
            var currentMapping = _printerMappingView.CurrentItem as PrinterMappingWrapper;

            if (currentMapping == null)
                return;

            DeletePrinterAction(currentMapping);
        }

        private void UpdatePrinterCollectionViews()
        {
            UpdatePrinterList();
            CollectionViewSource.GetDefaultView(ApplicationSettings.PrinterMappings).Refresh();
            CollectionViewSource.GetDefaultView(PdfCreatorPrinters).Refresh();
        }

        public void RefreshPrinterMappings()
        {
            // We need to force the UI to read the translated names.
            // This is the hard way to implement "RaisePropertyChanged"
            var tmp = _printerMappings;
            _printerMappings = null;
            RaisePropertyChanged("PrinterMappings");
            _printerMappings = tmp;
            RaisePropertyChanged("PrinterMappings");
        }
    }
}