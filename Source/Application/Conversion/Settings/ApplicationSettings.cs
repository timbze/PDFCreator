using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// PDFCreator application settings
	/// </summary>
	[ImplementPropertyChanged]
	public partial class ApplicationSettings : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		public Accounts Accounts { get; set; } = new Accounts();
		
		public JobHistory JobHistory { get; set; } = new JobHistory();
		
		public ObservableCollection<PrinterMapping> PrinterMappings { get; set; } = new ObservableCollection<PrinterMapping>();
		public ObservableCollection<TitleReplacement> TitleReplacement { get; set; } = new ObservableCollection<TitleReplacement>();
		public bool AskSwitchDefaultPrinter { get; set; } = true;
		
		public string Language { get; set; } = "";
		
		public string LastLoginVersion { get; set; } = "";
		
		/// <summary>
		/// The last directory the user during interactive job (if no target directory was set in profile)
		/// </summary>
		public string LastSaveDirectory { get; set; } = "";
		
		public string LastUsedProfileGuid { get; set; } = "DefaultGuid";
		
		public LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Error;
		
		public string PrimaryPrinter { get; set; } = "PDFCreator";
		
		public UpdateInterval UpdateInterval { get; set; } = UpdateInterval.Weekly;
		
		
		public void ReadValues(Data data, string path)
		{
			Accounts.ReadValues(data, path + @"Accounts\");
			JobHistory.ReadValues(data, path + @"JobHistory\");
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"PrinterMappings\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					PrinterMapping tmp = new PrinterMapping();
					tmp.ReadValues(data, @"" + path + @"PrinterMappings\" + i + @"\");
					PrinterMappings.Add(tmp);
				}
			} catch {}
			
			
			try
			{
				int numClasses = int.Parse(data.GetValue(@"" + path + @"TitleReplacement\numClasses"));
				for (int i = 0; i < numClasses; i++)
				{
					TitleReplacement tmp = new TitleReplacement();
					tmp.ReadValues(data, @"" + path + @"TitleReplacement\" + i + @"\");
					TitleReplacement.Add(tmp);
				}
			} catch {}
			
			try { AskSwitchDefaultPrinter = bool.Parse(data.GetValue(@"" + path + @"AskSwitchDefaultPrinter")); } catch { AskSwitchDefaultPrinter = true;}
			try { Language = Data.UnescapeString(data.GetValue(@"" + path + @"Language")); } catch { Language = "";}
			try { LastLoginVersion = Data.UnescapeString(data.GetValue(@"" + path + @"LastLoginVersion")); } catch { LastLoginVersion = "";}
			try { LastSaveDirectory = Data.UnescapeString(data.GetValue(@"" + path + @"LastSaveDirectory")); } catch { LastSaveDirectory = "";}
			try { LastUsedProfileGuid = Data.UnescapeString(data.GetValue(@"" + path + @"LastUsedProfileGuid")); } catch { LastUsedProfileGuid = "DefaultGuid";}
			try { LoggingLevel = (LoggingLevel) Enum.Parse(typeof(LoggingLevel), data.GetValue(@"" + path + @"LoggingLevel")); } catch { LoggingLevel = LoggingLevel.Error;}
			try { PrimaryPrinter = Data.UnescapeString(data.GetValue(@"" + path + @"PrimaryPrinter")); } catch { PrimaryPrinter = "PDFCreator";}
			try { UpdateInterval = (UpdateInterval) Enum.Parse(typeof(UpdateInterval), data.GetValue(@"" + path + @"UpdateInterval")); } catch { UpdateInterval = UpdateInterval.Weekly;}
		}
		
		public void StoreValues(Data data, string path)
		{
			Accounts.StoreValues(data, path + @"Accounts\");
			JobHistory.StoreValues(data, path + @"JobHistory\");
			
			for (int i = 0; i < PrinterMappings.Count; i++)
			{
				PrinterMapping tmp = PrinterMappings[i];
				tmp.StoreValues(data, @"" + path + @"PrinterMappings\" + i + @"\");
			}
			data.SetValue(@"" + path + @"PrinterMappings\numClasses", PrinterMappings.Count.ToString());
			
			
			for (int i = 0; i < TitleReplacement.Count; i++)
			{
				TitleReplacement tmp = TitleReplacement[i];
				tmp.StoreValues(data, @"" + path + @"TitleReplacement\" + i + @"\");
			}
			data.SetValue(@"" + path + @"TitleReplacement\numClasses", TitleReplacement.Count.ToString());
			
			data.SetValue(@"" + path + @"AskSwitchDefaultPrinter", AskSwitchDefaultPrinter.ToString());
			data.SetValue(@"" + path + @"Language", Data.EscapeString(Language));
			data.SetValue(@"" + path + @"LastLoginVersion", Data.EscapeString(LastLoginVersion));
			data.SetValue(@"" + path + @"LastSaveDirectory", Data.EscapeString(LastSaveDirectory));
			data.SetValue(@"" + path + @"LastUsedProfileGuid", Data.EscapeString(LastUsedProfileGuid));
			data.SetValue(@"" + path + @"LoggingLevel", LoggingLevel.ToString());
			data.SetValue(@"" + path + @"PrimaryPrinter", Data.EscapeString(PrimaryPrinter));
			data.SetValue(@"" + path + @"UpdateInterval", UpdateInterval.ToString());
		}
		
		public ApplicationSettings Copy()
		{
			ApplicationSettings copy = new ApplicationSettings();
			
			copy.Accounts = Accounts.Copy();
			copy.JobHistory = JobHistory.Copy();
			
			copy.PrinterMappings = new ObservableCollection<PrinterMapping>();
			for (int i = 0; i < PrinterMappings.Count; i++)
			{
				copy.PrinterMappings.Add(PrinterMappings[i].Copy());
			}
			
			
			copy.TitleReplacement = new ObservableCollection<TitleReplacement>();
			for (int i = 0; i < TitleReplacement.Count; i++)
			{
				copy.TitleReplacement.Add(TitleReplacement[i].Copy());
			}
			
			copy.AskSwitchDefaultPrinter = AskSwitchDefaultPrinter;
			copy.Language = Language;
			copy.LastLoginVersion = LastLoginVersion;
			copy.LastSaveDirectory = LastSaveDirectory;
			copy.LastUsedProfileGuid = LastUsedProfileGuid;
			copy.LoggingLevel = LoggingLevel;
			copy.PrimaryPrinter = PrimaryPrinter;
			copy.UpdateInterval = UpdateInterval;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is ApplicationSettings)) return false;
			ApplicationSettings v = o as ApplicationSettings;
			
			if (!Accounts.Equals(v.Accounts)) return false;
			if (!JobHistory.Equals(v.JobHistory)) return false;
			
			if (PrinterMappings.Count != v.PrinterMappings.Count) return false;
			for (int i = 0; i < PrinterMappings.Count; i++)
			{
				if (!PrinterMappings[i].Equals(v.PrinterMappings[i])) return false;
			}
			
			
			if (TitleReplacement.Count != v.TitleReplacement.Count) return false;
			for (int i = 0; i < TitleReplacement.Count; i++)
			{
				if (!TitleReplacement[i].Equals(v.TitleReplacement[i])) return false;
			}
			
			if (!AskSwitchDefaultPrinter.Equals(v.AskSwitchDefaultPrinter)) return false;
			if (!Language.Equals(v.Language)) return false;
			if (!LastLoginVersion.Equals(v.LastLoginVersion)) return false;
			if (!LastSaveDirectory.Equals(v.LastSaveDirectory)) return false;
			if (!LastUsedProfileGuid.Equals(v.LastUsedProfileGuid)) return false;
			if (!LoggingLevel.Equals(v.LoggingLevel)) return false;
			if (!PrimaryPrinter.Equals(v.PrimaryPrinter)) return false;
			if (!UpdateInterval.Equals(v.UpdateInterval)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("[Accounts]");
			sb.AppendLine(Accounts.ToString());
			sb.AppendLine("[JobHistory]");
			sb.AppendLine(JobHistory.ToString());
			
			for (int i = 0; i < PrinterMappings.Count; i++)
			{
				sb.AppendLine(PrinterMappings.ToString());
			}
			
			
			for (int i = 0; i < TitleReplacement.Count; i++)
			{
				sb.AppendLine(TitleReplacement.ToString());
			}
			
			sb.AppendLine("AskSwitchDefaultPrinter=" + AskSwitchDefaultPrinter.ToString());
			sb.AppendLine("Language=" + Language.ToString());
			sb.AppendLine("LastLoginVersion=" + LastLoginVersion.ToString());
			sb.AppendLine("LastSaveDirectory=" + LastSaveDirectory.ToString());
			sb.AppendLine("LastUsedProfileGuid=" + LastUsedProfileGuid.ToString());
			sb.AppendLine("LoggingLevel=" + LoggingLevel.ToString());
			sb.AppendLine("PrimaryPrinter=" + PrimaryPrinter.ToString());
			sb.AppendLine("UpdateInterval=" + UpdateInterval.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
	}
}
