using pdfforge.DataStorage.Storage;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System;

// ! This file is generated automatically.
// ! Do not edit it outside the sections for custom code.
// ! These changes will be deleted during the next generation run

namespace pdfforge.PDFCreator.Conversion.Settings
{
	/// <summary>
	/// Digitally sign the PDF document
	/// </summary>
	[ImplementPropertyChanged]
	public partial class Signature : INotifyPropertyChanged {
		#pragma warning disable 67
		public event PropertyChangedEventHandler PropertyChanged;
		#pragma warning restore 67
		
		
		/// <summary>
		/// If true, the PDF file may be signed by additional persons
		/// </summary>
		public bool AllowMultiSigning { get; set; } = false;
		
		/// <summary>
		/// Path to the certificate
		/// </summary>
		public string CertificateFile { get; set; } = "";
		
		/// <summary>
		/// If true, the signature will be displayed in the PDF file
		/// </summary>
		public bool DisplaySignatureInDocument { get; set; } = false;
		
		/// <summary>
		/// If true, the signature will be displayed in the PDF document
		/// </summary>
		public bool Enabled { get; set; } = false;
		
		/// <summary>
		/// Signature location: Top left corner (X part)
		/// </summary>
		public int LeftX { get; set; } = 100;
		
		/// <summary>
		/// Signature location: Top left corner (Y part)
		/// </summary>
		public int LeftY { get; set; } = 100;
		
		/// <summary>
		/// Signature location: Bottom right corner (X part)
		/// </summary>
		public int RightX { get; set; } = 200;
		
		/// <summary>
		/// Signature location: Bottom right corner (Y part)
		/// </summary>
		public int RightY { get; set; } = 200;
		
		/// <summary>
		/// Contact name of the signature
		/// </summary>
		public string SignContact { get; set; } = "";
		
		/// <summary>
		/// Signature location
		/// </summary>
		public string SignLocation { get; set; } = "";
		
		/// <summary>
		/// Reason for the signature
		/// </summary>
		public string SignReason { get; set; } = "";
		
		/// <summary>
		/// If the signature page is set to custom, this property defines the page where the signature will be displayed
		/// </summary>
		public int SignatureCustomPage { get; set; } = 1;
		
		/// <summary>
		/// Defines the page on which the signature will be displayed.
		/// </summary>
		public SignaturePage SignaturePage { get; set; } = SignaturePage.FirstPage;
		
		/// <summary>
		/// Password for the certificate file
		/// </summary>
		private string _signaturePassword = "";
		public string SignaturePassword { get { try { return Data.Decrypt(_signaturePassword); } catch { return ""; } } set { _signaturePassword = Data.Encrypt(value); } }
		
		/// <summary>
		/// ID of the linked account for the timeserver
		/// </summary>
		public string TimeServerAccountId { get; set; } = "";
		
		
		public void ReadValues(Data data, string path)
		{
			try { AllowMultiSigning = bool.Parse(data.GetValue(@"" + path + @"AllowMultiSigning")); } catch { AllowMultiSigning = false;}
			try { CertificateFile = Data.UnescapeString(data.GetValue(@"" + path + @"CertificateFile")); } catch { CertificateFile = "";}
			try { DisplaySignatureInDocument = bool.Parse(data.GetValue(@"" + path + @"DisplaySignatureInDocument")); } catch { DisplaySignatureInDocument = false;}
			try { Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled")); } catch { Enabled = false;}
			try { LeftX = int.Parse(data.GetValue(@"" + path + @"LeftX"), System.Globalization.CultureInfo.InvariantCulture); } catch { LeftX = 100;}
			try { LeftY = int.Parse(data.GetValue(@"" + path + @"LeftY"), System.Globalization.CultureInfo.InvariantCulture); } catch { LeftY = 100;}
			try { RightX = int.Parse(data.GetValue(@"" + path + @"RightX"), System.Globalization.CultureInfo.InvariantCulture); } catch { RightX = 200;}
			try { RightY = int.Parse(data.GetValue(@"" + path + @"RightY"), System.Globalization.CultureInfo.InvariantCulture); } catch { RightY = 200;}
			try { SignContact = Data.UnescapeString(data.GetValue(@"" + path + @"SignContact")); } catch { SignContact = "";}
			try { SignLocation = Data.UnescapeString(data.GetValue(@"" + path + @"SignLocation")); } catch { SignLocation = "";}
			try { SignReason = Data.UnescapeString(data.GetValue(@"" + path + @"SignReason")); } catch { SignReason = "";}
			try { SignatureCustomPage = int.Parse(data.GetValue(@"" + path + @"SignatureCustomPage"), System.Globalization.CultureInfo.InvariantCulture); } catch { SignatureCustomPage = 1;}
			try { SignaturePage = (SignaturePage) Enum.Parse(typeof(SignaturePage), data.GetValue(@"" + path + @"SignaturePage")); } catch { SignaturePage = SignaturePage.FirstPage;}
			_signaturePassword = data.GetValue(@"" + path + @"SignaturePassword");
			try { TimeServerAccountId = Data.UnescapeString(data.GetValue(@"" + path + @"TimeServerAccountId")); } catch { TimeServerAccountId = "";}
		}
		
		public void StoreValues(Data data, string path)
		{
			data.SetValue(@"" + path + @"AllowMultiSigning", AllowMultiSigning.ToString());
			data.SetValue(@"" + path + @"CertificateFile", Data.EscapeString(CertificateFile));
			data.SetValue(@"" + path + @"DisplaySignatureInDocument", DisplaySignatureInDocument.ToString());
			data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
			data.SetValue(@"" + path + @"LeftX", LeftX.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"LeftY", LeftY.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"RightX", RightX.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"RightY", RightY.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"SignContact", Data.EscapeString(SignContact));
			data.SetValue(@"" + path + @"SignLocation", Data.EscapeString(SignLocation));
			data.SetValue(@"" + path + @"SignReason", Data.EscapeString(SignReason));
			data.SetValue(@"" + path + @"SignatureCustomPage", SignatureCustomPage.ToString(System.Globalization.CultureInfo.InvariantCulture));
			data.SetValue(@"" + path + @"SignaturePage", SignaturePage.ToString());
			data.SetValue(@"" + path + @"SignaturePassword", _signaturePassword);
			data.SetValue(@"" + path + @"TimeServerAccountId", Data.EscapeString(TimeServerAccountId));
		}
		
		public Signature Copy()
		{
			Signature copy = new Signature();
			
			copy.AllowMultiSigning = AllowMultiSigning;
			copy.CertificateFile = CertificateFile;
			copy.DisplaySignatureInDocument = DisplaySignatureInDocument;
			copy.Enabled = Enabled;
			copy.LeftX = LeftX;
			copy.LeftY = LeftY;
			copy.RightX = RightX;
			copy.RightY = RightY;
			copy.SignContact = SignContact;
			copy.SignLocation = SignLocation;
			copy.SignReason = SignReason;
			copy.SignatureCustomPage = SignatureCustomPage;
			copy.SignaturePage = SignaturePage;
			copy.SignaturePassword = SignaturePassword;
			copy.TimeServerAccountId = TimeServerAccountId;
			
			return copy;
		}
		
		public override bool Equals(object o)
		{
			if (!(o is Signature)) return false;
			Signature v = o as Signature;
			
			if (!AllowMultiSigning.Equals(v.AllowMultiSigning)) return false;
			if (!CertificateFile.Equals(v.CertificateFile)) return false;
			if (!DisplaySignatureInDocument.Equals(v.DisplaySignatureInDocument)) return false;
			if (!Enabled.Equals(v.Enabled)) return false;
			if (!LeftX.Equals(v.LeftX)) return false;
			if (!LeftY.Equals(v.LeftY)) return false;
			if (!RightX.Equals(v.RightX)) return false;
			if (!RightY.Equals(v.RightY)) return false;
			if (!SignContact.Equals(v.SignContact)) return false;
			if (!SignLocation.Equals(v.SignLocation)) return false;
			if (!SignReason.Equals(v.SignReason)) return false;
			if (!SignatureCustomPage.Equals(v.SignatureCustomPage)) return false;
			if (!SignaturePage.Equals(v.SignaturePage)) return false;
			if (!SignaturePassword.Equals(v.SignaturePassword)) return false;
			if (!TimeServerAccountId.Equals(v.TimeServerAccountId)) return false;
			
			return true;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("AllowMultiSigning=" + AllowMultiSigning.ToString());
			sb.AppendLine("CertificateFile=" + CertificateFile.ToString());
			sb.AppendLine("DisplaySignatureInDocument=" + DisplaySignatureInDocument.ToString());
			sb.AppendLine("Enabled=" + Enabled.ToString());
			sb.AppendLine("LeftX=" + LeftX.ToString());
			sb.AppendLine("LeftY=" + LeftY.ToString());
			sb.AppendLine("RightX=" + RightX.ToString());
			sb.AppendLine("RightY=" + RightY.ToString());
			sb.AppendLine("SignContact=" + SignContact.ToString());
			sb.AppendLine("SignLocation=" + SignLocation.ToString());
			sb.AppendLine("SignReason=" + SignReason.ToString());
			sb.AppendLine("SignatureCustomPage=" + SignatureCustomPage.ToString());
			sb.AppendLine("SignaturePage=" + SignaturePage.ToString());
			sb.AppendLine("SignaturePassword=" + SignaturePassword.ToString());
			sb.AppendLine("TimeServerAccountId=" + TimeServerAccountId.ToString());
			
			return sb.ToString();
		}
		
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
// Custom Code starts here
// START_CUSTOM_SECTION:GENERAL
// END_CUSTOM_SECTION:GENERAL
// Custom Code ends here. Do not edit below
		
// Custom Code starts here
// START_CUSTOM_SECTION:INCLUDES

// END_CUSTOM_SECTION:INCLUDES
// Custom Code ends here. Do not edit below
		
	}
}
