using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Container class for PDFCreator settings and profiles
	/// </summary>
	[ImplementPropertyChanged]
	public partial class PdfCreatorSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		private Data data = Data.CreateDataStorage();
		private IStorage storage = null;
		
		public ApplicationProperties ApplicationProperties { get; set; } = new ApplicationProperties();
		
		/// <summary>
		/// PDFCreator application settings
		/// </summary>
		public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
		
		public ObservableCollection<ConversionProfile> ConversionProfiles { get; set; } = new ObservableCollection<ConversionProfile>();
		public PdfCreatorSettings(IStorage storage)
		{
			this.storage = storage;
			data = Data.CreateDataStorage();
		}
		
		public bool LoadData(IStorage storage, string path)
		{
			try {
				data.Clear();
				storage.SetData(data);
				storage.ReadData(path);
				ReadValues(data, "");
				return true;
			} catch { return false; }
			
		}
		
		public bool LoadData(string path)
		{
			return LoadData(storage, path);
			
		}
		
		public bool SaveData(IStorage storage, string path)
		{
			try {
				data.Clear();
				StoreValues(data, "");
				storage.SetData(data);
				storage.WriteData(path);
				return true;
			} catch { return false; }
			
		}
		
		public bool SaveData(string path)
		{
			return SaveData(storage, path);
			
		}
		
		public void ReadValues(Data data, string path)
		{
			ApplicationProperties.ReadValues(data, path + @"ApplicationProperties\");
			ApplicationSettings.ReadValues(data, path + @"ApplicationSettings\");
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"ConversionProfiles\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					ConversionProfile tmp = new ConversionProfile();
					tmp.ReadValues(data, @"" + path + @"ConversionProfiles\" + i + @"\");
					ConversionProfiles.Add(tmp);
				}
			} catch {}
			
		}
		
		public void StoreValues(Data data, string path)
		{
			ApplicationProperties.StoreValues(data, path + @"ApplicationProperties\");
			ApplicationSettings.StoreValues(data, path + @"ApplicationSettings\");
			
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				ConversionProfile tmp = ConversionProfiles[i];
				tmp.StoreValues(data, @"" + path + @"ConversionProfiles\" + i + @"\");
			}
			data.SetValue(@"" + path + @"ConversionProfiles\numClasses", ConversionProfiles.Count.ToString());
			
		}
		
		public PdfCreatorSettings Copy()
		{
			PdfCreatorSettings copy = new PdfCreatorSettings(storage);
			
			copy.ApplicationProperties = ApplicationProperties.Copy();
			copy.ApplicationSettings = ApplicationSettings.Copy();
			
			copy.ConversionProfiles = new ObservableCollection<ConversionProfile>();
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				copy.ConversionProfiles.Add(ConversionProfiles[i].Copy());
			}
			
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is PdfCreatorSettings)) return false;
			PdfCreatorSettings v = o as PdfCreatorSettings;
			
			if (!ApplicationProperties.Equals(v.ApplicationProperties)) return false;
			if (!ApplicationSettings.Equals(v.ApplicationSettings)) return false;
			
			if (ConversionProfiles.Count != v.ConversionProfiles.Count) return false;
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				if (!ConversionProfiles[i].Equals(v.ConversionProfiles[i])) return false;
			}
			
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("[ApplicationProperties]");
			sb.AppendLine(ApplicationProperties.ToString());
			sb.AppendLine("[ApplicationSettings]");
			sb.AppendLine(ApplicationSettings.ToString());
			
			for (int i = 0; i < ConversionProfiles.Count; i++)
			{
				sb.AppendLine(ConversionProfiles.ToString());
			}
			
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
// Custom Code starts here
// START_CUSTOM_SECTION:GENERAL

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
            return storage;
        }

        public bool LoadData(IStorage storage, string path, Action<Data> dataValidation)
        {
            try
            {
                data.Clear();
                storage.SetData(data);
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

// END_CUSTOM_SECTION:GENERAL
// Custom Code ends here. Do not edit below
		
// Custom Code starts here
// START_CUSTOM_SECTION:INCLUDES

// END_CUSTOM_SECTION:INCLUDES
// Custom Code ends here. Do not edit below
		
	}
}
