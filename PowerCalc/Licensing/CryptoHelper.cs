using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace TAlex.PowerCalc.Licensing
{
    internal static class CryptoHelper
    {
        #region Fields

        private static Random _rnd = new Random();

        #endregion

        #region Methods

        internal static byte[] Encrypt(string plaintext, SymmetricAlgorithm cipher)
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, cipher.CreateEncryptor(), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(encStream);
            sw.WriteLine(plaintext);

            sw.Close();
            encStream.Close();

            byte[] buffer = ms.ToArray();

            ms.Close();
            return buffer;
        }

        internal static string Decript(byte[] ciphertext, SymmetricAlgorithm cipher)
        {
            MemoryStream ms = new MemoryStream(ciphertext);
            CryptoStream decStream = new CryptoStream(ms, cipher.CreateDecryptor(), CryptoStreamMode.Read);

            StreamReader sr = new StreamReader(decStream);
            string result = sr.ReadToEnd();

            sr.Close();
            decStream.Close();
            ms.Close();

            return result;
        }

        internal static string SHA512Hex(string source)
        {
            byte[] data = Encoding.UTF8.GetBytes(source);
            byte[] hash = new SHA512Managed().ComputeHash(data);

            StringBuilder sb = new StringBuilder(hash.Length * 2);

            for (int i = 0; i < hash.Length; i++)
                sb.AppendFormat("{0:X2}", hash[i]);

            return sb.ToString();
        }

        internal static string SHA512Base64(string source)
        {
            byte[] data = Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(new SHA512Managed().ComputeHash(data));
        }

        internal static string GenerateRandomString(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int mode = _rnd.Next(0, 3);

                if (mode == 0)
                    sb.Append((Char)('A' + _rnd.Next(0, 26)));
                else if (mode == 1)
                    sb.Append((Char)('a' + _rnd.Next(0, 26)));
                else if (mode == 2)
                    sb.Append((Char)('0' + _rnd.Next(0, 10)));
            }

            return sb.ToString();
        }

        #endregion
    }
}
