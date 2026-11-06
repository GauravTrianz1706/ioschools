namespace ioschools.Data
{
    /// <summary>
    /// Identifies the school division within the ioSchools system.
    /// WARNING: These integer values are persisted in the database.
    /// Do NOT change existing values without a corresponding database migration.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum Schools
    {
        /// <summary>Kindergarten / pre-school division.</summary>
        Kindergarten = 1,

        /// <summary>Primary school division.</summary>
        Primary = 2,

        /// <summary>Secondary school division.</summary>
        Secondary = 3,

        /// <summary>International school division.</summary>
        International = 4,
    }
}
