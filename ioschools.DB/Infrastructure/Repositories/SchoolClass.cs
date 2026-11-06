using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - School class operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void DeleteStudentAllocatedClass(long id)
        {
            var exist = _db.classes_students_allocateds.SingleOrDefault(x => x.id == id);
            if (exist != null)
            {
                _db.classes_students_allocateds.Remove(exist);
                Save();
            }
        }

        /// <inheritdoc/>
        public void DeleteTeacherAllocatedClass(long id)
        {
            var exist = _db.classes_teachers_allocateds.SingleOrDefault(x => x.id == id);
            if (exist != null)
            {
                _db.classes_teachers_allocateds.Remove(exist);
                Save();
            }
        }

        /// <inheritdoc/>
        public ClassesTeachersAllocated? GetAllocatedTeacherClass(long id)
        {
            return _db.classes_teachers_allocateds.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IQueryable<SchoolClass> GetSchoolClasses()
        {
            return _db.school_classes;
        }

        /// <inheritdoc/>
        public IEnumerable<SchoolClass> GetClassesImTeachingThisSubject(long teacherid, long subjectid, int year)
        {
            return _db.classes_teachers_allocateds
                .Where(x => x.year == year &&
                            x.teacherid == teacherid &&
                            x.subjectid.HasValue && x.subjectid.Value == subjectid)
                .Select(x => x.school_class!)
                .Where(sc => sc != null)
                .Distinct();
        }
    }
}
