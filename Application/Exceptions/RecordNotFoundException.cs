using System;
using System.Diagnostics.CodeAnalysis;

namespace Onion.Application.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(long id, [AllowNull] Type recordType)
            : base(
                $"Could not find record '{id}'" +
                (recordType == null ? "." : $" of '{recordType.FullName}' type."))
        {
        }

        public RecordNotFoundException(string message)
            : base(message)
        {
        }
    }
}