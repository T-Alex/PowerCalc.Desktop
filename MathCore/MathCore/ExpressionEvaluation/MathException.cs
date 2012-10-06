using System;

namespace TAlex.MathCore.ExpressionEvaluation
{
    /// <summary>
    /// 
    /// </summary>
    public class MathException : Exception
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public MathException() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public MathException(string message) : base(message) { }

        #endregion
    }
}
