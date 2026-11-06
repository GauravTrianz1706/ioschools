namespace ioschools.Data.CMS
{
    /// <summary>
    /// Defines the layout rendering method for a CMS page.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum LayoutMethod
    {
        /// <summary>No special layout; content is rendered inline.</summary>
        NONE,

        /// <summary>Content is rendered in a side-by-side (sidebar) layout.</summary>
        SIDE,

        /// <summary>Content is rendered as a gallery grid.</summary>
        GALLERY
    }
}
