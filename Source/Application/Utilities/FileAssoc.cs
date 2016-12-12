using System;
using System.Collections.Generic;
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

            var verbs = GetVerbsByExtension(assoc);

            return verbs.Any(s => s.Equals(verb, StringComparison.OrdinalIgnoreCase));
        }

        private string[] GetVerbsByExtension(string extension)
        {
            var win8Version = new Version(6, 2, 9200, 0);

            // On Windows 8 and above, we have to detect the verbs by the associated ProgID
            if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version >= win8Version)
            {
                string progId = AssocQueryString(AssocStr.ASSOCSTR_PROGID, extension);

                if (string.IsNullOrWhiteSpace(progId))
                    return new string[] {};

                return GetVerbsByProgId(progId);
            }

            var psi = new ProcessStartInfo(@"C:\files" + extension);
            return psi.Verbs;
        }

        private string[] GetVerbsByProgId(string progId)
        {
            var verbs = new List<string>();

            if (string.IsNullOrEmpty(progId))
                return verbs.ToArray();

            using (var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(progId + "\\shell"))
            {
                if (key != null)
                {
                    var names = key.GetSubKeyNames();
                    verbs.AddRange(
                        names.Where(
                            name =>
                                string.Compare(
                                    name,
                                    "new",
                                    StringComparison.OrdinalIgnoreCase)
                                != 0));
                }
            }

            return verbs.ToArray();
        }

        private string AssocQueryString(AssocStr association, string extension)
        {
            uint length = 0;
            uint ret = AssocQueryString(
                AssocF.None, association, extension, "printto", null, ref length);
            if (ret != 1) //expected S_FALSE
            {
                throw new Win32Exception();
            }

            var sb = new StringBuilder((int)length);
            ret = AssocQueryString(
                AssocF.None, association, extension, null, sb, ref length);
            if (ret != 0) //expected S_OK
            {
                throw new Win32Exception();
            }

            return sb.ToString();
        }

        #region Shell Lightweight API

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint AssocQueryString(
           AssocF flags,
           AssocStr str,
           string pszAssoc,
           string pszExtra,
           [Out] StringBuilder pszOut,
           ref uint pcchOut);

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

        #endregion
    }
}