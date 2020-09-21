using System;
using System.Globalization;

namespace Onion.Application.Exceptions
{
    public class AccountException : Exception
    {
        public AccountException()
            : base()
        {
        }

        public AccountException(string message)
            : base(message)
        {
        }

        public AccountException(string message, string account = null)
            : base(message)
        {
            this.Account = account;
        }

        public AccountException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        public string Account { get; set; }
    }
}