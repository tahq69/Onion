using System.Collections.Generic;
using FluentAssertions;
using Onion.Shared.Extensions;
using Xunit;

namespace Onion.Shared.Tests.Extensions
{
    public class TypeExtensionsTests
    {
        [Fact, Trait("Category", "Integration")]
        public void TypeExtensions_UserFriendlyName_ProperlyCreatesSimpleName()
        {
            // Arrange
            var sut = typeof(Type1);

            // Act
            string result = sut.UserFriendlyName();

            // Assert
            result.Should().Be("Type1");
        }

        [Fact, Trait("Category", "Integration")]
        public void TypeExtensions_UserFriendlyName_ProperlyCreatesComplexName()
        {
            // Arrange
            var sut = typeof(Type2<IEnumerable<string>>);

            // Act
            string result = sut.UserFriendlyName();

            // Assert
            result.Should().Be("Type2<IEnumerable<String>>");
        }


        [Fact, Trait("Category", "Integration")]
        public void TypeExtensions_UserFriendlyName_ProperlyCreatesSimpleFullNamespace()
        {
            // Arrange
            var sut = typeof(Type1);

            // Act
            string result = sut.UserFriendlyName("");

            // Assert
            result.Should().Be("Onion.Shared.Tests.Extensions.Type1");
        }


        [Fact, Trait("Category", "Integration")]
        public void TypeExtensions_UserFriendlyName_ProperlyCreatesSimpleNamespace()
        {
            // Arrange
            var sut = typeof(Type1);

            // Act
            string result = sut.UserFriendlyName("Onion.Shared.Tests");

            // Assert
            result.Should().Be("Extensions.Type1");
        }

        [Fact, Trait("Category", "Integration")]
        public void TypeExtensions_UserFriendlyName_ProperlyCreatesComplexFullNamespace()
        {
            // Arrange
            var sut = typeof(Type2<IEnumerable<string>>);

            // Act
            string result = sut.UserFriendlyName("");

            // Assert
            result.Should().Be("Onion.Shared.Tests.Extensions.Type2<IEnumerable<String>>");
        }

        [Fact, Trait("Category", "Integration")]
        public void TypeExtensions_UserFriendlyName_ProperlyCreatesComplexNamespace()
        {
            // Arrange
            var sut = typeof(Type2<IEnumerable<string>>);

            // Act
            string result = sut.UserFriendlyName("Onion.Shared.Tests");

            // Assert
            result.Should().Be("Extensions.Type2<IEnumerable<String>>");
        }

        class Type1
        {
        }

        class Type2<T>
        {
        }
    }
}