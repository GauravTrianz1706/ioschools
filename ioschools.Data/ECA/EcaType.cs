using System.ComponentModel;

namespace ioschools.Data.ECA
{
    /// <summary>
    /// Classifies an Extra-Curricular Activity (ECA) into a broad category.
    /// Migrated from legacy ASP.NET MVC enum — no value changes required.
    /// </summary>
    public enum EcaType
    {
        /// <summary>Sports, games, and competitive activities.</summary>
        [Description("Sports/Games/Competition")]
        SPORTS,

        /// <summary>Clubs and societies.</summary>
        [Description("Clubs/Society")]
        CLUBS,

        /// <summary>Other school duties and responsibilities.</summary>
        [Description("Other Duties")]
        DUTIES,

        /// <summary>Uniform bodies (e.g. scouts, cadets).</summary>
        [Description("Uniform Bodies")]
        UNIFORM
    }
}
