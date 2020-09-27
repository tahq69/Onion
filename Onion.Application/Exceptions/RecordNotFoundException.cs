using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Onion.Application.Exceptions
{
    /// <summary>
    /// When record not found in DB/IO e.c. this exception will occur.
    /// </summary>
    /// <typeparam name="T">Type of the record identifier.</typeparam>
    /// <seealso cref="Onion.Application.Exceptions.BaseException" />
    [Serializable]
    public class RecordNotFoundException<T> : BaseException
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException{T}"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RecordNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordNotFoundException{T}"/> class.
        /// </summary>
        /// <param name="id">The record search identifier.</param>
        /// <param name="recordType">Type of the record.</param>
        public RecordNotFoundException(T id, [AllowNull] Type recordType)
            : base(
                $"Could not find record '{id}'" +
                (recordType == null ? "." : $" of '{recordType.FullName}' type."))
        {
        }

        /// <inheritdoc cref="BaseException(SerializationInfo, StreamingContext)" />
        protected RecordNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}