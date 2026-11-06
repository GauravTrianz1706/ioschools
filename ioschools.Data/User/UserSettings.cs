namespace ioschools.Data.User
{
    /// <summary>
    /// Bit-flag enumeration of account-level settings / state flags for a user.
    /// Migrated from legacy ASP.NET MVC UserSettings enum — no value changes required.
    /// </summary>
    [Flags]
    public enum UserSettings
    {
        /// <summary>No special settings applied.</summary>
        NONE            = 0,

        /// <summary>User must reset their password on next login.</summary>
        PASSWORD_RESET  = 0x00000001,

        /// <summary>Account is inactive (soft-deleted or suspended).</summary>
        INACTIVE        = 0x00000002,

        /// <summary>Account is pending approval by an administrator.</summary>
        PENDING         = 0x00000004,

        /// <summary>User is in the process of leaving the school.</summary>
        LEAVING         = 0x00000008
    }
}
