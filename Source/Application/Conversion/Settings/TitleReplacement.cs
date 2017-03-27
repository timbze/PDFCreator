using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Text.RegularExpressions;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// 
	/// TitleReplacements are used to automatically replace text in the print job name for the final document's title.
	/// i.e. Word prints are named "Document.docx - Microsoft Word", where the replacement can remove the ".docx - Microsoft Word" part.
	/// 
	/// </summary>
	public class TitleReplacement {
		
		public string Replace { get; set; }
		
		public ReplacementType ReplacementType { get; set; }
		
		public string Search { get; set; }
		
		
		private void Init() {
			Replace = "";
			ReplacementType = ReplacementType.Replace;
			Search = "";
		}
		
		public TitleReplacement()
		{
			Init();
		}
		
		public void ReadValues(Data data, string path) {
			try { Replace = Data.UnescapeString(data.GetValue(@"" + path + @"Replace")); } catch { Replace = "";}
			try { ReplacementType = (ReplacementType) Enum.Parse(typeof(ReplacementType), data.GetValue(@"" + path + @"ReplacementType")); } catch { ReplacementType = ReplacementType.Replace;}
			try { Search = Data.UnescapeString(data.GetValue(@"" + path + @"Search")); } catch { Search = "";}
		}
		
		
		public void StoreValues(Data data, string path) {
			data.SetValue(@"" + path + @"Replace", Data.EscapeString(Replace));
			data.SetValue(@"" + path + @"ReplacementType", ReplacementType.ToString());
			data.SetValue(@"" + path + @"Search", Data.EscapeString(Search));
		}
		
		public TitleReplacement Copy()
		{
			TitleReplacement copy = new TitleReplacement();
			
			copy.Replace = Replace;
			copy.ReplacementType = ReplacementType;
			copy.Search = Search;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is TitleReplacement)) return false;
			TitleReplacement v = o as TitleReplacement;
			
			if (!Replace.Equals(v.Replace)) return false;
			if (!ReplacementType.Equals(v.ReplacementType)) return false;
			if (!Search.Equals(v.Search)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("Replace=" + Replace.ToString());
			sb.AppendLine("ReplacementType=" + ReplacementType.ToString());
			sb.AppendLine("Search=" + Search.ToString());
			
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
        ///     Create a TitleReplacement with search and replace fields set
        /// </summary>
        /// <param name="replacementType">Defines how the replacement will be processed</param>
        /// <param name="search">The text to search for</param>
        /// <param name="replace">The text that will be inserted instead</param>
        public TitleReplacement(ReplacementType replacementType, string search, string replace)
        {
            ReplacementType = replacementType;
            Search = search;
            Replace = replace;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Search))
                return false;

            if (ReplacementType != ReplacementType.RegEx)
                return true;
            
            if (string.IsNullOrEmpty(Replace))
                return false;

            try
            {
                Regex.Replace("", Search, Replace);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        // END_CUSTOM_SECTION:GENERAL
// Custom Code ends here. Do not edit below
		
	}
}
