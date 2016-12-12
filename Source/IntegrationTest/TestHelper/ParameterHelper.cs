using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PDFCreator.TestUtilities
{
    /// <summary>
    ///     The class ParameterHelper loads parameters that should not be included in the source code of the tests (i.e.
    ///     passwords and IP addresses for integration tests) from an external text file
    /// </summary>
    public static class ParameterHelper
    {
        private static Dictionary<string, string> _passwordDictionary;

        private static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string GetPassword(string name)
        {
            if (_passwordDictionary == null)
                _passwordDictionary = LoadPasswords();

            var password = _passwordDictionary[name];

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("The password " + name + "was not set!");

            return password;
        }

        private static Dictionary<string, string> LoadPasswords()
        {
            var passwords = new Dictionary<string, string>();

            var passwordFile = FindPasswordFile();

            if (!File.Exists(passwordFile))
                throw new InvalidOperationException("The password file does not exist!");

            var lines = File.ReadAllLines(passwordFile);

            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("#"))
                    continue;

                if (line.Contains("="))
                {
                    var pair = line.Split(new[] {'='}, 2);
                    passwords.Add(pair[0].Trim(), pair[1].Trim());
                }
            }
            return passwords;
        }

        private static string FindPasswordFile()
        {
            var candidates = new[]
            {
                AssemblyDirectory,
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"..")),
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"..\..")),
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"..\..\..")),
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"..\..\..\..")),
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"..\..\..\..\..")),
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"..\..\Source")),
                Path.GetFullPath(Path.Combine(AssemblyDirectory, @"Source")),
                @"D:\",
                @"C:\"
            };

            foreach (var candidate in candidates)
            {
                var passwordFile = Path.Combine(candidate, "passwords.txt");
                if (File.Exists(passwordFile))
                    return passwordFile;
            }

            throw new InvalidOperationException("Could not find the passwords.txt file. Search folders were:\r\n" + string.Join("\r\n", candidates));
        }
    }
}