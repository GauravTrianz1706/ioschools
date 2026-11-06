namespace ioschools.Data
{
    /// <summary>
    /// Classifies the relationship type of a student's guardian.
    /// Values are stored in the database; do not change existing numeric values.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum GuardianType : byte
    {
        /// <summary>Biological or adoptive father.</summary>
        FATHER = 1,

        /// <summary>Biological or adoptive mother.</summary>
        MOTHER = 2,

        /// <summary>Legal guardian (not a parent).</summary>
        GUARDIAN = 4
    }
}
