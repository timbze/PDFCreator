using System;
using System.Collections.ObjectModel;
using System.Linq;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    partial class PdfCreatorSettings
    {
        public PdfCreatorSettings CopyAndPreserveApplicationSettings()
        {
            var copy = Copy();

            copy.ApplicationProperties = ApplicationProperties;
            copy.ApplicationSettings = ApplicationSettings;

            return copy;
        }

        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given guid.
        /// </summary>
        /// <param name="guid">Guid to look for</param>
        /// <returns>(First) Conversionprofile with the given guid. Returns null, if no profile with given guid exists.</returns>
        public ConversionProfile GetProfileByGuid(string guid)
        {
            if (ConversionProfiles.Count <= 0)
                return null;

            return ConversionProfiles.FirstOrDefault(p => p.Guid == guid);
        }

        /// <summary>
        ///     Function that returns a profile from the inner Conversionprofiles(list) by a given name.
        /// </summary>
        /// <param name="name">Profilename</param>
        /// <returns>(First) Conversionprofile with the given name. Returns null, if no profile with given name exists.</returns>
        public ConversionProfile GetProfileByName(string name)
        {
            if (ConversionProfiles.Count <= 0)
                return null;

            return ConversionProfiles.FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        ///     Function that returns the last used profile, according to the LastUsedProfileGuid of the ApplicationSettings.
        ///     If the Conversionprofiles(list) does not contain a profile with the LastUsedProfileGuid (because it was deleted)
        ///     or the last guid is null the function will null.
        /// </summary>
        /// <returns>Returns last used profile. Returns null if ConversionProfiles is empty or no last profile is known.</returns>
        public ConversionProfile GetLastUsedProfile()
        {
            if (ConversionProfiles.Count <= 0)
                return null;

            if (ApplicationSettings.LastUsedProfileGuid == null)
                return null;

            return GetProfileByGuid(ApplicationSettings.LastUsedProfileGuid);
        }

        /// <summary>
        ///     Function that returns the last used profile, according to the LastUsedProfileGuid of the ApplicationSettings.
        ///     If the Conversionprofiles(list) does not contain a profile with the LastUsedProfileGuid (because it was deleted)
        ///     or the last guid is null the function will return the first profile.
        /// </summary>
        /// <returns>Returns last used or first profile. Returns null if ConversionProfiles is empty.</returns>
        public ConversionProfile GetLastUsedOrFirstProfile()
        {
            if (ConversionProfiles.Count <= 0)
                return null;

            if (ApplicationSettings.LastUsedProfileGuid == null)
                return ConversionProfiles[0];

            var p = GetProfileByGuid(ApplicationSettings.LastUsedProfileGuid);
            if (p == null)
                return ConversionProfiles[0];

            return p;
        }

        /// <summary>
        ///     Sorts the inner list "ConversionProfiles", firstly considering their properties and then the alphabetical order
        ///     temporary > default > other
        /// </summary>
        public void SortConversionProfiles()
        {
            //((List<ConversionProfile>)ConversionProfiles).Sort(CompareTemporaryFirstDefaultSecond);
            var profiles = ConversionProfiles.ToList();
            profiles.Sort(new ProfileSorter().Compare);
            ConversionProfiles = new ObservableCollection<ConversionProfile>(profiles);
        }

        /// <summary>
        ///     Find the first printer mapping for this profile
        /// </summary>
        /// <param name="profile">The profile to look for</param>
        /// <returns>The first printer mapping that is found or null.</returns>
        public PrinterMapping GetPrinterByProfile(ConversionProfile profile)
        {
            foreach (var pm in ApplicationSettings.PrinterMappings)
            {
                if (pm.ProfileGuid.Equals(profile.Guid))
                    return pm;
            }

            return null;
        }

        public IStorage GetStorage()
        {
            return _storage;
        }

        public bool LoadData(IStorage storage, string path, Action<Data> dataValidation)
        {
            try
            {
                data.Clear();
                storage.Data = data;
                storage.ReadData(path);
                dataValidation(data);
                ReadValues(data, "");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
