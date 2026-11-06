using System.ComponentModel;

namespace ioschools.Data.Leave
{
    /// <summary>
    /// Identifies which segment of a day a leave period starts or ends on.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum LeaveDaySegment : byte
    {
        /// <summary>Leave begins at the start of the day (morning).</summary>
        [Description("start")]
        START = 1,

        /// <summary>Leave begins in the middle of the day (afternoon).</summary>
        [Description("mid")]
        MIDDLE = 2,

        /// <summary>Leave ends at the end of the day.</summary>
        [Description("end")]
        END = 3
    }
}
