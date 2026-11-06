namespace ioschools.Data
{
    /// <summary>
    /// Classifies a blog / news post as either publicly visible or internal-only.
    /// Migrated from legacy ASP.NET MVC enum — no changes to values required.
    /// </summary>
    public enum BlogType
    {
        /// <summary>Post is visible to the general public (unauthenticated visitors).</summary>
        PUBLIC,

        /// <summary>Post is visible only to authenticated school members.</summary>
        INTERNAL
    }
}
