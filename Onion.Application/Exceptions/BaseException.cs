using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Onion.Application.Exceptions
{
    /// <summary>
    /// Base application exception with additional constructors.
    /// </summary>
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        protected BaseException()
        {
        }

        /// <inheritdoc cref="Exception(SerializationInfo, StreamingContext)"/>
        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected BaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        /// (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.
        /// </param>
        protected BaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="args">The arguments to substitute provided message.</param>
        protected BaseException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}