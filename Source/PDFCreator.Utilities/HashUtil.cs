using System;
using System.Security.Cryptography;
using System.Text;

namespace pdfforge.PDFCreator.Utilities
{
    public class HashUtil
    {
        /// <summary>
        /// Gets the SHA1 hash.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>hashed text</returns>
        public static string GetSha1Hash(string text)
        {
            var SHA1 = new SHA1CryptoServiceProvider();

            string result = null;

            byte[] arrayData = Encoding.ASCII.GetBytes(text);
            byte[] arrayResult = SHA1.ComputeHash(arrayData);

            for (int i = 0; i < arrayResult.Length; i++)
            {
                string temp = Convert.ToString(arrayResult[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                result += temp;
            }

            return result;
        }
    }
}
