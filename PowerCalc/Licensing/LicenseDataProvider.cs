using System;
using System.Collections.Generic;
using System.Text;

namespace TAlex.PowerCalc.Licensing
{
    /// <summary>
    /// Provides the abstract base class for implementing a license data provider. 
    /// </summary>
    internal abstract class LicenseDataProvider
    {
        #region Properties

        public abstract string LicenseName { get; set; }

        public abstract string LicenseKey { get; set; }

        #endregion

        #region Methods

        public abstract void Load();

        public abstract void Save();

        #endregion
    }
}
