using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Onion.Application.Exceptions;
using Xunit;
using FluentAssertions;

namespace Onion.Application.Tests.Exceptions
{
    public partial class AccountExceptionTests
    {
        [Fact, Trait("Category", "Integration")]
        public void AccountException_Constructor_WithSerializableProperties()
        {
            // Arrange
            AccountException ex = new AccountException("Message", "Account");
            AccountException deserializedEx;

            // Assert
            ex.Message.Should().Be("Message");
            ex.Account.Should().Be("Account");

            // Act
            string exception = ex.ToString();

            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                // "Save" object state
                bf.Serialize(ms, ex);

                // Re-use the same stream for de-serialization
                ms.Seek(0, 0);

                // Fill exception with de-serialized one
                deserializedEx = (AccountException) bf.Deserialize(ms);
            }

            // Assert
            deserializedEx.Message.Should().Be("Message");
            deserializedEx.Account.Should().Be("Account");
            ex.ToString().Should().Be(deserializedEx.ToString());
        }
    }
}