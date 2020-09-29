using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Onion.Application.Exceptions
{
    /// <summary>
    /// Model validation error details.
    /// </summary>
    /// <seealso cref="Onion.Application.Exceptions.BaseException" />
    [Serializable]
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
                if (Errors.ContainsKey(failure.PropertyName))
                    Errors[failure.PropertyName].Add(failure.ErrorMessage);
                else
                    Errors.Add(failure.PropertyName, new List<string> { failure.ErrorMessage });
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="errors">Collection of the available errors.</param>
        public ValidationException(IDictionary<string, ICollection<string>> errors)
            : this()
        {
            Errors = new Dictionary<string, ICollection<string>>(errors);
        }

        /// <inheritdoc cref="BaseException(SerializationInfo, StreamingContext)"/>
        public ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            object? value = info.GetValue(nameof(Errors), typeof(IDictionary<string, ICollection<string>>));
            if (value is IDictionary<string, ICollection<string>> errors)
                Errors = errors;
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IDictionary<string, ICollection<string>> Errors { get; } = new Dictionary<string, ICollection<string>>();

        /// <summary>
        /// Creates validation error exception with single property and messages in it.
        /// </summary>
        /// <param name="propName">Failed parameter name.</param>
        /// <param name="messages">The failure messages.</param>
        /// <returns>Validation exception with error.</returns>
        public static ValidationException FromPropError(string propName, params string[] messages) =>
            new ValidationException(new Dictionary<string, ICollection<string>>
            {
                { propName, messages },
            });

        /// <inheritdoc cref="Exception.GetObjectData"/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Errors), Errors, typeof(IDictionary<string, ICollection<string>>));

            base.GetObjectData(info, context);
        }
    }
}