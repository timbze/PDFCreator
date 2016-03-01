using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.Shared.Helper
{
    public class PrinterHelper
    {
        #region Windows Spooler

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumPrinters(PrinterEnumFlags flags, string name, uint level, IntPtr pPrinterEnum, uint cbBuf, ref uint pcbNeeded, ref uint pcReturned);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
// ReSharper disable once InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable MemberCanBePrivate.Local
        private struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pServerName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pShareName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPortName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pComment;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pSepFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDatatype;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }
// ReSharper restore FieldCanBeMadeReadOnly.Local
// ReSharper restore MemberCanBePrivate.Local
        [Flags]
        private enum PrinterEnumFlags
        {
// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
            PRINTER_ENUM_DEFAULT = 0x00000001,
            PRINTER_ENUM_LOCAL = 0x00000002,
            PRINTER_ENUM_CONNECTIONS = 0x00000004,
            PRINTER_ENUM_FAVORITE = 0x00000004,
            PRINTER_ENUM_NAME = 0x00000008,
            PRINTER_ENUM_REMOTE = 0x00000010,
            PRINTER_ENUM_SHARED = 0x00000020,
            PRINTER_ENUM_NETWORK = 0x00000040,
            PRINTER_ENUM_EXPAND = 0x00004000,
            PRINTER_ENUM_CONTAINER = 0x00008000,
            PRINTER_ENUM_ICONMASK = 0x00ff0000,
            PRINTER_ENUM_ICON1 = 0x00010000,
            PRINTER_ENUM_ICON2 = 0x00020000,
            PRINTER_ENUM_ICON3 = 0x00040000,
            PRINTER_ENUM_ICON4 = 0x00080000,
            PRINTER_ENUM_ICON5 = 0x00100000,
            PRINTER_ENUM_ICON6 = 0x00200000,
            PRINTER_ENUM_ICON7 = 0x00400000,
            PRINTER_ENUM_ICON8 = 0x00800000,
            PRINTER_ENUM_HIDE = 0x01000000
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming
        }

        #endregion

        /// <summary>
        /// List all printers that are connected to the pdfcmon port
        /// </summary>
        /// <returns>A Collection of PDFCreator printers</returns>
        public virtual ICollection<string> GetPDFCreatorPrinters()
        {
            IEnumerable<PRINTER_INFO_2> printerInfos = EnumPrinters(PrinterEnumFlags.PRINTER_ENUM_LOCAL);

            var printers = new List<string>();

            foreach (var printer in printerInfos)
            {
                //if (printer.pPortName.Equals("pdfcmon"))
                if (printer.pDriverName.Equals("PDFCreator", StringComparison.OrdinalIgnoreCase))
                    printers.Add(printer.pPrinterName);
            }

            printers.Sort();

            return printers;
        }

        // ReSharper disable once InconsistentNaming
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        private IEnumerable<PRINTER_INFO_2> EnumPrinters(PrinterEnumFlags flags)
        {
            uint cbNeeded = 0;
            uint cReturned = 0;
            if (EnumPrinters(flags, null, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned))
            {
                return null;
            }
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
            {
                IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
                if (EnumPrinters(flags, null, 2, pAddr, cbNeeded, ref cbNeeded, ref cReturned))
                {
                    var printerInfo2 = new PRINTER_INFO_2[cReturned];
                    IntPtr offset = pAddr;
                    Type type = typeof(PRINTER_INFO_2);
                    int increment = Marshal.SizeOf(type);
                    for (int i = 0; i < cReturned; i++)
                    {
                        printerInfo2[i] = (PRINTER_INFO_2)Marshal.PtrToStructure(offset, type);
                        offset += increment;
                    }
                    Marshal.FreeHGlobal(pAddr);
                    return printerInfo2;
                }
                lastWin32Error = Marshal.GetLastWin32Error();
            }
            throw new Win32Exception(lastWin32Error);
        }

        /// <summary>
        /// Prints a windows test page to the printer with the given name
        /// </summary>
        /// <param name="primaryPrinter">Name of the printer to use</param>
        public void PrintWindowsTestPage(string primaryPrinter)
        {
            var settings = new PrinterSettings();
            string defaultPrinter = settings.PrinterName;
            string printer = GetApplicablePDFCreatorPrinter(primaryPrinter, defaultPrinter);
            
            var psi = new ProcessStartInfo("RUNDLL32.exe", "PRINTUI.DLL,PrintUIEntry /k /n \"" + printer + "\"");
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }

        /// <summary>
        /// Prints a windows test page to the preferred printer. It searches all printers connected to the PDFCreator port. If one of them is called "PDFCreator", this one is used. If not, the first one will be used.
        /// </summary>
        public void PrintWindowsTestPage()
        {
            PrintWindowsTestPage("PDFCreator");
        }

        /// <summary>
        /// Function to check if the printer name is already used by another installed printer
        /// </summary>
        /// <param name="printerName">proposed name of printer</param>
        /// <returns>true if printerName is unique or false if the name is already in use</returns>
        public bool IsValidPrinterName(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return false;

            foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
            {
                if (installedPrinter.Equals(printerName, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Function that searches all printers connected to the PDFCreator port and returns the most applicable. This is either the requested printer, 
        /// the default Printer, the one with the name "PDFCreator" or the first one in alphabetical order.
        /// </summary>
        /// <param name="requestedPrinter">Name of the primary PDFCreator printer</param>
        /// <param name="defaultPrinter">Name of the current default printer</param>
        /// <returns>null if no PDFCreator printer is installed, else the name of the most applicable PDFCreator printer</returns>
        public string GetApplicablePDFCreatorPrinter(string requestedPrinter, string defaultPrinter)
        {
            ICollection<string> printers = GetPDFCreatorPrinters();

            if (printers.Count == 0)
                return null;

            // ReSharper disable once InconsistentNaming
            string PDFCreatorPrinter = null;
            bool isDefaultPrinter = false;

            foreach (string printer in printers)
            {
                if (PDFCreatorPrinter == null)
                    PDFCreatorPrinter = printer;

                if (!isDefaultPrinter)
                    if (printer.Equals("PDFCreator", StringComparison.OrdinalIgnoreCase))
                        PDFCreatorPrinter = printer;
 
                if (printer.Equals(defaultPrinter, StringComparison.OrdinalIgnoreCase))
                {
                    PDFCreatorPrinter = printer;
                    isDefaultPrinter = true;
                }

                if (printer.Equals(requestedPrinter, StringComparison.OrdinalIgnoreCase))
                    return printer;
            }

            return PDFCreatorPrinter;
        }

        public string GetApplicablePDFCreatorPrinter(string requestedPrinter)
        {
            return GetApplicablePDFCreatorPrinter(requestedPrinter, GetDefaultPrinter());
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string printerName);

        public string GetDefaultPrinter()
        {
            return (new PrinterSettings()).PrinterName;
        }
    }
}