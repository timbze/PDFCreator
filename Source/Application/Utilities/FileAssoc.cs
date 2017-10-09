using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IFileAssoc
    {
        bool HasPrint(string assoc);

        bool HasPrintTo(string assoc);

        bool HasOpen(string assoc);
    }

    public class FileAssoc : IFileAssoc
    {
        /// <summary>
        ///     Tests if a file extension has the print verb associated.
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <returns>true, if the association is registered</returns>
        public bool HasPrint(string assoc)
        {
            return FileAssocHasVerb(assoc, "print");
        }

        /// <summary>
        ///     Tests if a file extension has the printto verb associated.
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <returns>true, if the association is registered</returns>
        public bool HasPrintTo(string assoc)
        {
            return FileAssocHasVerb(assoc, "printto");
        }

        /// <summary>
        ///     Tests if a file extension has the open verb associated.
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <returns>true, if the association is registered</returns>
        public bool HasOpen(string assoc)
        {
            return FileAssocHasVerb(assoc, "open");
        }

        private bool FileAssocHasVerb(string assoc, string verb)
        {
            if (string.IsNullOrEmpty(assoc))
            {
                throw new ArgumentNullException(assoc);
            }

            if (!assoc.StartsWith("."))
            {
                assoc = "." + assoc;
            }

            if (assoc.Length < 2)
            {
                throw new ArgumentException("The file extension must at least have a dot and one other character");
            }

            if (assoc.IndexOf(".", 1, StringComparison.Ordinal) > 0)
            {
                throw new ArgumentException(
                    "The file extension must start with a dot (.) and must not contain any dots after the first character");
            }

            var verbs = new string[3];

            if (verb.Equals("open"))
                verbs = GetCoreVerbs(assoc);
            else
                verbs = GetVerbsByExtension(assoc);

            return verbs.Any(s => s.Equals(verb, StringComparison.OrdinalIgnoreCase));
        }

        private string[] GetVerbsByExtension(string extension)
        {
            var win8Version = new Version(6, 2, 9200, 0);

            var verbs = GetCoreVerbs(extension);

            // On Windows 8 and above, we have to filter verbs with no valid command (UWP Apps)
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8Version)
            {
                return verbs.Where(verb => ValidateExtension(extension, verb)).ToArray(); ;
            }

            return verbs;
        }

        /// <summary>
        /// Get verbs from .Net Framework
        /// </summary>
        /// <param name="extension">file extension, i.e. "png"</param>
        /// <returns>An array of shell verbs that are registered for the extension</returns>
        private string[] GetCoreVerbs(string extension)
        {
            var si = new ProcessStartInfo(@"X:\file" + extension);
            return si.Verbs;
        }

        /// <summary>
        /// Shell verbs are only properly registered, if
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        private bool ValidateExtension(string extension, string verb)
        {
            try
            {
                string command = AssocQueryString(AssocStr.Command, @"X:\file" + extension, verb);
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }

        private string AssocQueryString(AssocStr association, string extension, string verb)
        {
            uint length = 0;
            var ret = AssocQueryString(AssocF.None, association, extension, verb, null, ref length);
            if (ret != HRESULT.S_FALSE) //expected S_FALSE to indicate the required amount of memory (ref length)
            {
                return "";
            }

            var sb = new StringBuilder((int)length);
            ret = AssocQueryString(AssocF.None, association, extension, verb, sb, ref length);
            if (ret != HRESULT.S_OK) //expected S_OK
            {
                throw new Win32Exception((int)ret);
            }

            return sb.ToString();
        }

        #region Shell Lightweight API

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern HRESULT AssocQueryString(
           AssocF flags,
           AssocStr str,
           string pszAssoc,
           string pszExtra,
           [Out] StringBuilder pszOut,
           ref uint pcchOut);

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        [Flags]
        private enum AssocF
        {
            None = 0x0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x00000400,
            Init_FixedProgId = 0x00000800,
            IsProtocol = 0x00001000
        }

        private enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            INFOTIP,
            QUICKTIP,
            TILEINFO,
            CONTENTTYPE,
            DEFAULTICON,
            SHELLEXTENSION,
            DROPTARGET,
            DELEGATEEXECUTE,
            SUPPORTED_URI_PROTOCOLS,
            ASSOCSTR_PROGID,
            ASSOCSTR_APPID,
            ASSOCSTR_APPPUBLISHER,
            ASSOCSTR_APPICONREFERENCE,
            ASSOCSTR_MAX
        }

        private enum HRESULT : long
        {
            S_FALSE = 0x0001,
            S_OK = 0x0000,
            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x8007000E,
            E_NO_APPLICATION_ASSOCIATED = 0x80070483
        }

        #endregion Shell Lightweight API
    }
}
