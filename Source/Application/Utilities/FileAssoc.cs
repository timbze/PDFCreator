using System;
using System.Runtime.InteropServices;

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

            UIntPtr hKey;
            var res = AssocQueryKey(AssocF.Init_IgnoreUnknown, AssocKey.ShellExecClass, assoc, verb, out hKey);

            if (res == 0)
            {
                RegCloseKey(hKey);
                return true;
            }

            return false;
        }

        #region Shell Lightweight API

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint AssocQueryKey(AssocF flags, AssocKey key, string pszAssoc, string pszExtra,
            [Out] out UIntPtr phkeyOut);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int RegCloseKey(UIntPtr hKey);

        [Flags]
        private enum AssocF
        {
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

        private enum AssocKey
        {
            ShellExecClass = 1,
            App,
            Class,
            BaseClass
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
            MAX
        }

        #endregion
    }
}