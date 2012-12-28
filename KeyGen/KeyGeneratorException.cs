using System;
using System.Collections.Generic;
using System.Text;

namespace TAlex.PowerCalc.KeyGenerator
{
    /// <summary>
    /// Key generator exception class.
    /// </summary>
    public class KeyGeneratorException : Exception
    {
        #region Fields

        public KeyGeneratorReturnCode ERC;

        #endregion

        #region Constructors

        public KeyGeneratorException(string message, KeyGeneratorReturnCode e)
            : base(message)
        {
            ERC = e;
        }

        #endregion
    }
}
