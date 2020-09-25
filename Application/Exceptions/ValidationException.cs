using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Onion.Application.Exceptions
{
    /// <summary>
    /// Model validation error details.
    /// </summary>
    /// <seealso cref="Onion.Application.Exceptions.BaseException" />
    public class ValidationException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="failures">The validation failures.</param>
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (ValidationFailure failure in failures)
            {
                Errors.Add(failure.PropertyName, failure.ErrorMessage);
            }
        }

        /// <inheritdoc cref="BaseException(SerializationInfo, StreamingContext)"/>
        public ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (IDictionary<string, string>)info.GetValue(nameof(Errors), typeof(IDictionary<string, string>));
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IDictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        /// <inheritdoc cref="Exception.GetObjectData"/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Errors), Errors, typeof(IDictionary<string, string>));

            base.GetObjectData(info, context);
        }
    }
}