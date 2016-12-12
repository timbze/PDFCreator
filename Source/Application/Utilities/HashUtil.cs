using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IHashUtil
    {
        string GetSha1Hash(string text);
        string CalculateFileMd5(string filepath);
        bool VerifyFileMd5(string filepath, string expectedMd5);
    }

    public class HashUtil : IHashUtil
    {
        /// <summary>
        ///     Gets the SHA1 hash.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>hashed text</returns>
        public string GetSha1Hash(string text)
        {
            var SHA1 = new SHA1CryptoServiceProvider();

            string result = null;

            var arrayData = Encoding.ASCII.GetBytes(text);
            var arrayResult = SHA1.ComputeHash(arrayData);

            for (var i = 0; i < arrayResult.Length; i++)
            {
                var temp = Convert.ToString(arrayResult[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                result += temp;
            }

            return result;
        }

        public string CalculateFileMd5(string filepath)
        {
            var fileCheck = File.OpenRead(filepath);

            // calculate MD5-Hash from Byte-Array
            MD5 hashAlgorithm = new MD5CryptoServiceProvider();
            var md5Hash = hashAlgorithm.ComputeHash(fileCheck);
            fileCheck.Close();

            var md5 = BitConverter.ToString(md5Hash).Replace("-", "").ToLowerInvariant();
            return md5;
        }

        public bool VerifyFileMd5(string filepath, string expectedMd5)
        {
            var md5 = CalculateFileMd5(filepath);
            return md5 == expectedMd5.ToLowerInvariant();
        }
    }
}