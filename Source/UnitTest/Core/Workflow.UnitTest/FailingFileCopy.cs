using System.IO;
using System.Security.AccessControl;
using System.Text;
using SystemInterface;
using SystemInterface.IO;
using SystemInterface.Security.AccessControl;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    internal class FailingFileCopy : IFile
    {
        private readonly int _failFirstCount;
        private int _calls;

        public FailingFileCopy(int failFirstCount)
        {
            _failFirstCount = failFirstCount;
        }

        public void AppendAllText(string path, string contents)
        {
            throw new System.NotImplementedException();
        }

        public void AppendAllText(string path, string contents, Encoding encoding)
        {
            throw new System.NotImplementedException();
        }

        public IStreamWriter AppendText(string path)
        {
            throw new System.NotImplementedException();
        }

        public void Copy(string sourceFileName, string destFileName)
        {
            _calls++;
            if (_calls <= _failFirstCount)
                throw new IOException();
        }

        public void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            _calls++;
            if (_calls <= _failFirstCount)
                throw new IOException();
        }

        public IFileStream Create(string path)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream Create(string path, int bufferSize)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream Create(string path, int bufferSize, FileOptions options)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream Create(string path, int bufferSize, FileOptions options, IFileSecurity fileSecurity)
        {
            throw new System.NotImplementedException();
        }

        public IStreamWriter CreateText(string path)
        {
            throw new System.NotImplementedException();
        }

        public void Decrypt(string path)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string path)
        {
            _calls++;
            if (_calls <= _failFirstCount)
                throw new IOException();
        }

        public void Encrypt(string path)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(string path)
        {
            throw new System.NotImplementedException();
        }

        public IFileSecurity GetAccessControl(string path)
        {
            throw new System.NotImplementedException();
        }

        public IFileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new System.NotImplementedException();
        }

        public FileAttributes GetAttributes(string path)
        {
            throw new System.NotImplementedException();
        }

        public IDateTime GetCreationTime(string path)
        {
            throw new System.NotImplementedException();
        }

        public IDateTime GetCreationTimeUtc(string path)
        {
            throw new System.NotImplementedException();
        }

        public IDateTime GetLastAccessTime(string path)
        {
            throw new System.NotImplementedException();
        }

        public IDateTime GetLastAccessTimeUtc(string path)
        {
            throw new System.NotImplementedException();
        }

        public IDateTime GetLastWriteTime(string path)
        {
            throw new System.NotImplementedException();
        }

        public IDateTime GetLastWriteTimeUtc(string path)
        {
            throw new System.NotImplementedException();
        }

        public void Move(string sourceFileName, string destFileName)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream Open(string path, FileMode mode)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream Open(string path, FileMode mode, FileAccess access)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream OpenRead(string path)
        {
            throw new System.NotImplementedException();
        }

        public IStreamReader OpenText(string path)
        {
            throw new System.NotImplementedException();
        }

        public IFileStream OpenWrite(string path)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ReadAllBytes(string path)
        {
            throw new System.NotImplementedException();
        }

        public string[] ReadAllLines(string path)
        {
            throw new System.NotImplementedException();
        }

        public string[] ReadAllLines(string path, Encoding encoding)
        {
            throw new System.NotImplementedException();
        }

        public string ReadAllText(string path)
        {
            throw new System.NotImplementedException();
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            throw new System.NotImplementedException();
        }

        public void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            throw new System.NotImplementedException();
        }

        public void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName,
            bool ignoreMetadataErrors)
        {
            throw new System.NotImplementedException();
        }

        public void SetAccessControl(string path, IFileSecurity fileSecurity)
        {
            throw new System.NotImplementedException();
        }

        public void SetAttributes(string path, FileAttributes fileAttributes)
        {
            throw new System.NotImplementedException();
        }

        public void SetCreationTime(string path, IDateTime creationTime)
        {
            throw new System.NotImplementedException();
        }

        public void SetCreationTimeUtc(string path, IDateTime creationTimeUtc)
        {
            throw new System.NotImplementedException();
        }

        public void SetLastAccessTime(string path, IDateTime lastAccessTime)
        {
            throw new System.NotImplementedException();
        }

        public void SetLastAccessTimeUtc(string path, IDateTime lastAccessTimeUtc)
        {
            throw new System.NotImplementedException();
        }

        public void SetLastWriteTime(string path, IDateTime lastWriteTime)
        {
            throw new System.NotImplementedException();
        }

        public void SetLastWriteTimeUtc(string path, IDateTime lastWriteTimeUtc)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAllLines(string path, string[] contents)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAllText(string path, string contents)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            throw new System.NotImplementedException();
        }
    }
}
