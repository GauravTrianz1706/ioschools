namespace ioschools.Data.Attendance
{
    /// <summary>
    /// Represents the attendance status of a student or staff member.
    /// Migrated from legacy ASP.NET MVC enum — no changes to values required.
    /// </summary>
    public enum AttendanceStatus
    {
        /// <summary>The individual arrived after the scheduled start time.</summary>
        LATE,

        /// <summary>The individual was absent without an approved excuse.</summary>
        ABSENT,

        /// <summary>The absence was approved / excused by an administrator.</summary>
        EXCUSED
    }
}
