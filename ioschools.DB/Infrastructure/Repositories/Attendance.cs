using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Attendance operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void DeleteAttendance(Attendance att)
        {
            _db.attendances.Remove(att);
        }

        /// <inheritdoc/>
        public Attendance? GetClassAttendance(long studentid, int classid, DateTime date)
        {
            return _db.attendances.SingleOrDefault(x => x.classid == classid && x.date == date && x.studentid == studentid);
        }

        /// <inheritdoc/>
        public Attendance? GetEcaAttendance(long studentid, long ecaid, DateTime date)
        {
            return _db.attendances.SingleOrDefault(x => x.ecaid == ecaid && x.date == date && x.studentid == studentid);
        }

        /// <inheritdoc/>
        public Attendance? GetAttendance(long id)
        {
            return _db.attendances.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public AttendanceTerm? GetAttendanceTerm(int schoolid, int termid, int year)
        {
            return _db.attendance_terms.SingleOrDefault(x => x.schoolid == schoolid && x.termid == termid && x.year == year);
        }
    }
}
