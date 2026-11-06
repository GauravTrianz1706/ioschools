using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Student operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void DeleteAttendanceForStudent(long studentid)
        {
            var attendances = _db.attendances.Where(x => x.studentid == studentid);
            _db.attendances.RemoveRange(attendances);
        }

        /// <inheritdoc/>
        public IQueryable<User> GetStudentsByPhysicalClass(long classid, int year)
        {
            return _db.classes_students_allocateds
                .Where(x => x.classid == classid && x.year == year)
                .Select(y => y.user!);
        }
    }
}
