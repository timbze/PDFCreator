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
		private IStorage _storage = null;
		
		public ApplicationProperties ApplicationProperties { get; set; } = new ApplicationProperties();
		
		/// <summary>
		/// PDFCreator application settings
		/// </summary>
		public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();
		
		public ObservableCollection<ConversionProfile> ConversionProfiles { get; set; } = new ObservableCollection<ConversionProfile>();
		public PdfCreatorSettings(IStorage storage)
		{
			_storage = storage;
		}
		
		public bool LoadData(IStorage storage, string path)
		{
			try {
				data.Clear();
				storage.Data = data;
				storage.ReadData(path);
				ReadValues(data, "");
				return true;
			} catch { return false; }
			
		}
		
		public bool LoadData(string path)
		{
			return LoadData(_storage, path);
			
		}
		
		public bool SaveData(IStorage storage, string path)
		{
			try {
				data.Clear();
				StoreValues(data, "");
				storage.Data = data;
				storage.WriteData(path);
				return true;
			} catch { return false; }
			
		}
		
		public bool SaveData(string path)
		{
			return SaveData(_storage, path);
			
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
			PdfCreatorSettings copy = new PdfCreatorSettings(_storage);
			
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
		
	}
}
