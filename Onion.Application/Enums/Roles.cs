namespace Onion.Application.Enums
{
    /// <summary>
    /// Application user role classifier.
    /// </summary>
    public enum Roles
    {
        /// <summary>
        /// User allowed to do any action.
        /// </summary>
        SuperAdmin,

        /// <summary>
        /// Application administrator role.
        /// </summary>
        Admin,

        /// <summary>
        /// Content moderator role.
        /// </summary>
        Moderator,

        /// <summary>
        /// Basic user role.
        /// </summary>
        Basic,
    }
}