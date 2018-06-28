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
            _provider.SettingsChanged += (sender, args) =>
            {
                UpdateSettings(true);
            };
        }

        public PdfCreatorSettings Settings
        {
            get
            {
                if (_settings == null)
                    UpdateSettings(false);
                return _settings;
            }
        }

        public ObservableCollection<ConversionProfile> Profiles => Settings?.ConversionProfiles;

        public ConversionProfile SelectedProfile
        {
            get
            {
                if (_selectedProfile == null)
                    UpdateSettings(false);
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
            RaisePropertyChanged(nameof(Settings));
        }

        public event EventHandler SettingsChanged;

        public event PropertyChangedEventHandler SelectedProfileChanged;

        private void UpdateSettings(bool forceUpdate)
        {
            if (_provider?.Settings == null)
                return;

            if (_settings == null || forceUpdate)
            {
                CloneSettings();
                var firstProfile = Profiles.First();
                _selectedProfile = _selectedProfile == null ? firstProfile : Profiles.FirstOrDefault(x => x.Guid == _selectedProfile.Guid) ?? firstProfile;
                _selectedProfile = _selectedProfile ?? Profiles.First();
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CloneSettings()
        {
            _settings = _provider.Settings.CopyAndPreserveApplicationSettings();
        }
    }
}
