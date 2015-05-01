using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using TAlex.License;


namespace TAlex.PowerCalc.Licensing
{
    internal class AppLicenseDataManager : LicFileLicenseDataManager
    {
        #region Fields

        protected override int TextLength { get { return 2000; } }
        protected override int LicenseNameStartIndex { get { return 135; } }
        protected override int LicenseNameMaxLength { get { return 70; } }
        protected override int LicenseKeyStartIndex { get { return 1146; } }
        protected override int LicenseKeyMaxLength { get { return 250; } }

        protected override byte[] SK
        {
            get { return new byte[] { 104, 05, 75, 11, 76, 225, 185, 187 }; }
        }

        protected override byte[] IV
        {
            get {return  new byte[] { 243, 188, 57, 228, 60, 254, 126, 18 }; }
        }

        #endregion
    }
}
