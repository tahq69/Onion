using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Onion.Application.Exceptions
{
    /// <summary>
    /// Account operation exception.
    /// </summary>
    /// <seealso cref="Onion.Application.Exceptions.BaseException" />
    [Serializable]
    public class AccountException : BaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountException"/> class.
        /// </summary>
        public AccountException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AccountException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="args">The arguments to substitute provided message.</param>
        public AccountException(string message, params object[] args)
            : base(message, args)
        {
        }

        public AccountException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            Account = info.GetString(nameof(Account));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="account">With error associated account email address.</param>
        public AccountException(string message, string account = null)
            : base(message)
        {
            Account = account;
        }

        /// <summary>
        /// Gets or sets the account email address.
        /// </summary>
        public string Account { get; protected set; }

        /// <inheritdoc/>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Account), Account);

            base.GetObjectData(info, context);
        }
    }
}