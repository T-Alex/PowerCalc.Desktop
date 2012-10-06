//---------------------------------------------------------------------------
//
// (c) Copyright T-Alex Software Corporation.
// All rights reserved.
// 
// This file is part of the MathCore project.
//
//---------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace TAlex.MathCore.ExpressionEvaluation
{
    /// <summary>
    /// Represents errors that occur when the syntax of the expression is incorrect.
    /// </summary>
    [Serializable]
    public class SyntaxException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SyntaxException class.
        /// </summary>
        public SyntaxException() :
            base("Incorrect syntax expression.") { }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SyntaxException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception,
        /// or a null reference if no inner exception is specified.
        /// </param>
        public SyntaxException(string message, Exception innerException) :
            base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The class name is null or System.Exception.HResult is zero (0).
        /// </exception>
        public SyntaxException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        #endregion
    }
}
