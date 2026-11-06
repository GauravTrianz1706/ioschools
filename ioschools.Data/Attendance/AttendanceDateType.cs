namespace ioschools.Data.Attendance
{
    /// <summary>
    /// Specifies whether an attendance record covers a single day or a date range.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum AttendanceDateType
    {
        /// <summary>Attendance record applies to a single calendar day.</summary>
        SINGLEDAY,

        /// <summary>Attendance record spans a range of calendar days.</summary>
        RANGE
    }
}
