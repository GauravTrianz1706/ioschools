namespace ioschools.Data.User
{
    /// <summary>
    /// Carries the authentication context for a logged-in user.
    /// Migrated from legacy ASP.NET MVC UserAuth.cs — no functional changes required.
    /// In ASP.NET Core, this object is typically populated from cookie claims and stored
    /// in the controller's <c>HttpContext.Items</c> or resolved via
    /// <c>IHttpContextAccessor</c> in services.
    /// </summary>
    public class UserAuth
    {
        /// <summary>Gets or sets the permission flags granted to this user.</summary>
        public Permission perms { get; set; }

        /// <summary>Gets or sets the group(s) this user belongs to.</summary>
        public UserGroup group { get; set; }

        /// <summary>Initialises a new <see cref="UserAuth"/> with no permissions and no group.</summary>
        public UserAuth()
        {
            perms = Permission.NONE;
            group = UserGroup.NONE;
        }

        /// <summary>Initialises a new <see cref="UserAuth"/> with the specified permissions and group.</summary>
        /// <param name="p">The permission flags.</param>
        /// <param name="g">The user group.</param>
        public UserAuth(Permission p, UserGroup g)
        {
            perms = p;
            group = g;
        }
    }
}
