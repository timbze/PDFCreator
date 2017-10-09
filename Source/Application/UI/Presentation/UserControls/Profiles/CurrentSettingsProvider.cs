using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public interface ISelectedProfileProvider
    {
        ConversionProfile SelectedProfile { get; set; }
        ObservableCollection<ConversionProfile> Profiles { get; }

        ConversionProfile GetProfileByName(string name);

        event PropertyChangedEventHandler SelectedProfileChanged;

        event EventHandler SettingsChanged;
    }

    public interface ICurrentSettingsProvider : ISelectedProfileProvider
    {
        PdfCreatorSettings Settings { get; }

        void Reset();
    }

    public class CurrentSettingsProvider : ObservableObject, ICurrentSettingsProvider
    {
        private readonly ISettingsProvider _provider;
        private ConversionProfile _selectedProfile;
        private PdfCreatorSettings _settings;

        public CurrentSettingsProvider(ISettingsProvider provider)
        {
            _provider = provider;
        }

        public PdfCreatorSettings Settings
        {
            get
            {
                if (_settings == null)
                    UpdateSettings();
                return _settings;
            }
        }

        public ObservableCollection<ConversionProfile> Profiles => Settings?.ConversionProfiles;

        public ConversionProfile SelectedProfile
        {
            get
            {
                if (_selectedProfile == null)
                    UpdateSettings();
                return _selectedProfile;
            }
            set
            {
                if (value == null)
                    return;
                _selectedProfile = value;
                RaisePropertyChanged(nameof(SelectedProfile));
                SelectedProfileChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProfile)));
            }
        }

        public ConversionProfile GetProfileByName(string name)
        {
            if (Profiles.Count <= 0)
                return null;

            return Profiles.FirstOrDefault(p => p.Name == name);
        }

        public void Reset()
        {
            CloneSettings();
            SelectedProfile = _settings.GetProfileByName(_selectedProfile.Name);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler SettingsChanged;

        public event PropertyChangedEventHandler SelectedProfileChanged;

        private void UpdateSettings()
        {
            if (_settings == null && _provider.Settings != null)
            {
                CloneSettings();
                _selectedProfile = Profiles.First();
            }
        }

        private void CloneSettings()
        {
            _settings = _provider.Settings.CopyAndPreserveApplicationSettings();
        }
    }
}
