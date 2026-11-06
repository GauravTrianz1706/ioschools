namespace ioschools.Data.User
{
    /// <summary>
    /// Represents the lifecycle status of a user account.
    /// Migrated from legacy ASP.NET MVC UserStatus enum — unnecessary using directives removed.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>Account has been created but not yet approved.</summary>
        PENDING = 1,

        /// <summary>Account has been accepted / approved but not yet activated.</summary>
        ACCEPTED = 2,

        /// <summary>Account is active and the user can log in.</summary>
        ACTIVE = 3,

        /// <summary>User has left the school; account is read-only.</summary>
        LEFT = 4,

        /// <summary>Account has been disabled by an administrator.</summary>
        DISABLED = 5
    }
}
