using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace TAlex.PowerCalc.Licensing
{
    internal class LicFileLicenseDataProvider : LicenseDataProvider
    {
        #region Fields

        private const string DefaultLicenseFileName = "License.dat";

        private const string LicenseNameSeparator = "\n";
        private const string LicenseKeySeparator = "$";

        private const int TextLength = 2000;
        private const int LicenseNameStartIndex = 135;
        private const int LicenseNameMaxLength = 70;
        private const int LicenseKeyStartIndex = 1146;
        private const int LicenseKeyMaxLength = 250;

        private static Random _rnd = new Random();

        private static byte[] SK = new byte[]
        {
            104, 05, 75, 11, 76, 225, 185, 187
        };

        private static byte[] IV = new byte[]
        {
            243, 188, 57, 228, 60, 254, 126, 18
        };

        private SymmetricAlgorithm _cipher;

        private string _licenseFileName = DefaultLicenseFileName;

        private string _lin;

        private string _lik;

        #endregion

        #region Properties

        public string LicenseFileName
        {
            get
            {
                return _licenseFileName;
            }

            set
            {
                _licenseFileName = value;
            }
        }

        public override string LicenseName
        {
            get
            {
                return _lin;
            }

            set
            {
                _lin = value;
            }
        }

        public override string LicenseKey
        {
            get
            {
                return _lik;
            }

            set
            {
                _lik = value;
            }
        }

        #endregion

        #region Constructors

        public LicFileLicenseDataProvider()
        {
            _cipher = new DESCryptoServiceProvider();
            _cipher.IV = IV;
            _cipher.Key = SK;
        }

        #endregion

        #region Methods
        
        public override void Load()
        {
            _lin = String.Empty;
            _lik = String.Empty;

            FileStream fs = null;

            try
            {
                fs = new FileStream(_licenseFileName, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);

            string text = String.Empty;
            try
            {
                text = CryptoHelper.Decript(data, _cipher);
            }
            catch (CryptographicException)
            {
                return;
            }

            if (text.Length != TextLength + 2)
                return;

            // Load license data
            try
            {
                _lin = text.Substring(LicenseNameStartIndex, LicenseNameMaxLength + 1);
                _lin = _lin.Substring(0, _lin.IndexOf(LicenseNameSeparator));

                _lik = text.Substring(LicenseKeyStartIndex, LicenseKeyMaxLength + 1);
                _lik = _lik.Substring(0, _lik.IndexOf(LicenseKeySeparator));
            }
            catch (ArgumentOutOfRangeException)
            {
                _lin = String.Empty;
                _lik = String.Empty;
            }
        }

        public override void Save()
        {
            FileStream fs = new FileStream(_licenseFileName, FileMode.Create);
            byte[] data = CryptoHelper.Encrypt(GenerateText(_lin, _lik), _cipher);

            fs.Write(data, 0, data.Length);
            fs.Close();
        }

        private static string GenerateText(string lin, string lik)
        {
            StringBuilder sb = new StringBuilder(TextLength);

            sb.Append(CryptoHelper.GenerateRandomString(LicenseNameStartIndex));
            sb.Append(lin.Substring(0, Math.Min(lin.Length, LicenseNameMaxLength)));
            sb.Append(LicenseNameSeparator);
            sb.Append(CryptoHelper.GenerateRandomString(LicenseKeyStartIndex - sb.Length));
            sb.Append(lik.Substring(0, Math.Min(lik.Length, LicenseKeyMaxLength)));
            sb.Append(LicenseKeySeparator);
            sb.Append(CryptoHelper.GenerateRandomString(TextLength - sb.Length));
            return sb.ToString();
        }

        #endregion
    }
}
