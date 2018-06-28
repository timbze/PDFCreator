using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IPathUtil
    {
        // ReSharper disable once InconsistentNaming
        int MAX_PATH { get; }

        string ELLIPSIS { get; }

        string GetFileName(string path);

        string GetFileNameWithoutExtension(string path);

        string GetLongDirectoryName(string givenPath);

        string EllipsisForPath(string filePath, int maxLength);

        string EllipsisForTooLongPath(string filePath);

        bool DirectoryIsEmpty(string path);

        bool CheckWritability(string directory);

        bool IsValidRootedPath(string path);

        PathUtilStatus IsValidRootedPathWithResponse(string path);

        bool IsValidFilename(string fileName);

        string Combine(string path1, string path2);

        bool HasExtension(string path);

        string GetExtension(string path);

        string ChangeExtension(string path, string extension);
    }

    public class PathUtil : IPathUtil
    {
        private readonly IDirectory _directory;
        private readonly IPath _path;

        public PathUtil(IPath path, IDirectory directory)
        {
            _path = path;
            _directory = directory;
        }

        public int MAX_PATH => 259;

        public string ELLIPSIS => "__";

        public string GetLongDirectoryName(string givenPath)
        {
            if (givenPath == null)
                throw new ArgumentNullException(nameof(givenPath));

            var pos = givenPath.LastIndexOf('\\');

            // if pos == 0, the path starts with a backslash and has no further backslashes
            if (pos <= 0)
                return null;

            var folder = givenPath.Substring(0, pos);

            if (folder.Length == 2 && folder[1] == ':')
                folder += "\\";

            return folder;
        }

        public string GetFileName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            if (!path.Contains(@"\"))
                return path;

            var pos = path.LastIndexOf(@"\");

            var fileName = path.Substring(pos + 1);

            return fileName;
        }

        public string GetFileNameWithoutExtension(string path)
        {
            var fileName = GetFileName(path);
            return ChangeExtension(fileName, "");
        }

        /// <summary>
        ///     Adds ellipsis to a path with a length longer than 255.
        /// </summary>
        /// <param name="filePath">full path to file</param>
        /// <param name="maxLength">maximum length of the string. This must be between 10 and MAX_PATH (260)</param>
        /// <returns>file path with ellipsis to ensure length under the max length </returns>
        public string EllipsisForPath(string filePath, int maxLength)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (filePath.EndsWith("\\"))
                throw new ArgumentException("The path has to be a file", nameof(filePath));

            if (maxLength < 10 || maxLength > MAX_PATH)
                throw new ArgumentException($"The desired length must be between 10 and {MAX_PATH}", nameof(maxLength));

            if (filePath.Length > maxLength)
            {
                int minUsefulFileLength = 4;

                var directory = GetLongDirectoryName(filePath) ?? "";
                var file = GetFileNameWithoutExtension(filePath);
                var extension = GetExtension(filePath);

                var remainingLengthForFile = maxLength - directory.Length - extension.Length - ELLIPSIS.Length - 1; // substract -1 to account for the slash between path and filename
                if (remainingLengthForFile < minUsefulFileLength)
                {
                    throw new ArgumentException("The path is too long", nameof(filePath)); //!
                }

                var partLength = remainingLengthForFile / 2;

                file = file.Substring(0, partLength) + ELLIPSIS + file.Substring(file.Length - partLength, partLength);
                filePath = Combine(directory, file + extension);
            }

            return filePath;
        }

        /// <summary>
        ///     Adds ellipsis to a path with a length longer than 255.
        /// </summary>
        /// <param name="filePath">full path to file</param>
        /// <returns>file path with ellipsis to ensure length under 255 </returns>
        public string EllipsisForTooLongPath(string filePath)
        {
            return EllipsisForPath(filePath, MAX_PATH);
        }

        /// <summary>
        ///     Check if directory is writable.
        /// </summary>
        /// <param name="directory">Directory string or full file path</param>
        /// <returns>true if directory is writeable</returns>
        public bool CheckWritability(string directory)
        {
            directory = _path.GetFullPath(directory);

            var permissionSet = new PermissionSet(PermissionState.None);

            var fileIoPermission = new FileIOPermission(FileIOPermissionAccess.Write, directory);

            permissionSet.AddPermission(fileIoPermission);

            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }

        public bool DirectoryIsEmpty(string path)
        {
            return !_directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        ///     Checks if the given path is a (syntactically) valid rooted path, i.e. "C:\Temp\test.txt". This file is not required
        ///     to exist
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>true, if the path is valid</returns>
        public bool IsValidRootedPath(string path)
        {
            return IsValidRootedPathWithResponse(path) == PathUtilStatus.Success;
        }

        public PathUtilStatus IsValidRootedPathWithResponse(string path)
        {
            if (string.IsNullOrEmpty(path))
                return PathUtilStatus.PathWasNullOrEmpty;

            if (path.Length < 3)
                return PathUtilStatus.InvalidRootedPath;

            if (((path.IndexOf(":", StringComparison.Ordinal) != 1) || (path.IndexOf("\\", StringComparison.Ordinal) != 2)) && !path.StartsWith(@"\\"))
                return PathUtilStatus.InvalidRootedPath;

            try
            {
                var fi = new FileInfo(path);
            }
            catch (ArgumentException)
            {
                return PathUtilStatus.ArgumentEx;
            }
            catch (NotSupportedException)
            {
                return PathUtilStatus.NotSupportedEx;
            }
            catch (PathTooLongException)
            {
                return PathUtilStatus.PathTooLongEx;
            }

            if (!path.StartsWith(@"\\"))
            {
                var driveLetter = char.ToUpperInvariant(path[0]);
                if (driveLetter >= 'A' && driveLetter <= 'Z')
                    return PathUtilStatus.Success;

                return PathUtilStatus.InvalidRootedPath;
            }

            return PathUtilStatus.Success;
        }

        //todo: Is this the same as valid rooted path?
        public bool IsValidFilename(string fileName)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");

            if (containsABadCharacter.IsMatch(fileName))
                return false;

            return true;
        }

        public string Combine(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1) && string.IsNullOrWhiteSpace(path2))
                return "";
            if (!string.IsNullOrWhiteSpace(path1) && string.IsNullOrWhiteSpace(path2))
                return path1;
            if (string.IsNullOrWhiteSpace(path1) && !string.IsNullOrWhiteSpace(path2))
                return path2;

            path1 = path1.Trim();
            path2 = path2.Trim();

            while (path1.EndsWith(@"\") && path1.Length > 0)
            {
                path1 = path1.Remove(path1.Length - 1).Trim();
            }

            while (path2.StartsWith(@"\") && path2.Length > 0)
            {
                path2 = path2.Remove(0, 1).Trim();
            }

            return path1 + @"\" + path2;
        }

        public bool HasExtension(string path)
        {
            if (path == null)
                return false;

            path = path.Trim();
            var lastIndexOfPeriod = path.LastIndexOf(".", StringComparison.Ordinal);
            if (lastIndexOfPeriod == path.Length - 1)
                return false;

            var lastIndexOfBackslash = path.LastIndexOf(@"\", StringComparison.Ordinal);

            return lastIndexOfPeriod > lastIndexOfBackslash;
        }

        public string GetExtension(string path)
        {
            if (!HasExtension(path))
                return "";
            if (path == null)
                return "";
            path = path.Trim();
            var spiltByPeriod = path.Split('.');
            return "." + spiltByPeriod.LastOrDefault();
        }

        public string ChangeExtension(string path, string extension)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = "";

            if (string.IsNullOrWhiteSpace(extension))
                extension = "";

            var currentExtension = GetExtension(path);
            if (!string.IsNullOrWhiteSpace(currentExtension))
                path = path.Substring(0, path.LastIndexOf(currentExtension));

            return path + extension;
        }
    }

    public enum PathUtilStatus
    {
        Success,
        PathWasNullOrEmpty,
        InvalidRootedPath,
        ArgumentEx,
        PathTooLongEx,
        NotSupportedEx,
    }
}
