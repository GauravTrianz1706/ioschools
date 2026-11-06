using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Timetable operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public IEnumerable<ClassesTeachersAllocated> GetStudentTimeTable(long userid, DayOfWeek day, int year)
        {
            var sAllocated = _db.classes_students_allocateds
                .SingleOrDefault(x => x.studentid == userid && x.year == year);

            if (sAllocated == null)
            {
                return Enumerable.Empty<ClassesTeachersAllocated>();
            }

            return _db.classes_teachers_allocateds
                .Where(x => x.classid == sAllocated.classid && x.day == (int)day && x.year == year)
                .OrderBy(x => x.time_start);
        }

        /// <inheritdoc/>
        public IEnumerable<ClassesTeachersAllocated> GetTeacherTimeTable(long teacherid, DayOfWeek day, int year)
        {
            return _db.classes_teachers_allocateds
                .Where(x => x.teacherid == teacherid && x.day == (int)day && x.year == year)
                .OrderBy(x => x.time_start);
        }
    }
}
