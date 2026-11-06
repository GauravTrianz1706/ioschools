using System.ComponentModel;

namespace ioschools.Data.User
{
    /// <summary>
    /// Represents the marital status of a staff member or guardian.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum MaritalStatus
    {
        /// <summary>Not married.</summary>
        [Description("Single")]
        SINGLE,

        /// <summary>Currently married.</summary>
        [Description("Married")]
        MARRIED,

        /// <summary>Legally separated but not yet divorced.</summary>
        [Description("Separated")]
        SEPARATED,

        /// <summary>Marriage legally dissolved.</summary>
        [Description("Divorced")]
        DIVORCED,

        /// <summary>Spouse is deceased.</summary>
        [Description("Widowed")]
        WIDOWED,
    }
}
