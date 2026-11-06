using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - School class period/timetable operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public ClassesTeachersAllocated? GetClassPeriod(int year, int day, int schoolid, int classid, TimeSpan periodStart, TimeSpan periodEnd)
        {
            return _db.classes_teachers_allocateds.Where(x =>
                x.year == year &&
                x.day == day &&
                x.classid == classid &&
                x.schoolid == schoolid &&
                ((x.time_start == periodStart && x.time_end == periodEnd) ||
                 (periodStart < x.time_end && periodStart >= x.time_start) ||
                 (periodEnd > x.time_start && periodEnd <= x.time_end) ||
                 (periodStart <= x.time_start && periodEnd >= x.time_end)
                )).FirstOrDefault();
        }
    }
}
