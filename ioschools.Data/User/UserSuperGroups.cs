namespace ioschools.Data.User
{
    /// <summary>
    /// Provides composite <see cref="UserGroup"/> constants that represent logical
    /// super-groups used throughout the ioSchools permission system.
    /// Migrated from legacy ASP.NET MVC UserSuperGroups.cs — unnecessary using directives removed.
    /// </summary>
    public static class UserSuperGroups
    {
        /// <summary>
        /// All staff roles: Director, Head, Admin, Teacher, Finance, Clerk, and Support.
        /// Use this constant when a permission check should apply to any staff member.
        /// </summary>
        public const UserGroup STAFF =
            UserGroup.DIRECTOR |
            UserGroup.HEAD     |
            UserGroup.ADMIN    |
            UserGroup.TEACHER  |
            UserGroup.FINANCE  |
            UserGroup.CLERK    |
            UserGroup.SUPPORT;

        /// <summary>
        /// Senior staff roles with elevated administrative access: Director, Head, and Admin.
        /// Use this constant when a permission check should apply only to senior management.
        /// </summary>
        public const UserGroup SUPERSTAFF =
            UserGroup.DIRECTOR |
            UserGroup.HEAD     |
            UserGroup.ADMIN;
    }
}
