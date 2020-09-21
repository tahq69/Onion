using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace Onion.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
        }

        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (ValidationFailure failure in failures)
            {
                Errors.Add(failure.PropertyName, failure.ErrorMessage);
            }
        }
    }
}