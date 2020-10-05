namespace Onion.Shared.Extensions
{
    /// <summary>
    /// <see cref="string"/> object extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Uppercase string first character.
        /// </summary>
        /// <param name="input">Initial string value.</param>
        /// <returns>New text where first character is in lowercase.</returns>
        public static string ToLowerFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}