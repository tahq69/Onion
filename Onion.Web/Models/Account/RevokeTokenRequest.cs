namespace Onion.Web.Models.Account
{
    /// <summary>
    /// Revoke application user token request model.
    /// </summary>
    public class RevokeTokenRequest
    {
        /// <summary>
        /// Gets or sets application user refresh token.
        /// </summary>
        public string? Token { get; set; }
    }
}