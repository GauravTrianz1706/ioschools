using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - School subject operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void AddSchoolSubject(Subject subject)
        {
            _db.subjects.Add(subject);
        }

        /// <inheritdoc/>
        public void DeleteSchoolSubject(long id)
        {
            var subject = GetSchoolSubject(id);
            if (subject != null)
            {
                _db.subjects.Remove(subject);
                Save();
            }
        }

        /// <inheritdoc/>
        public Subject? GetSchoolSubject(long id)
        {
            return _db.subjects.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IQueryable<Subject> GetSchoolSubjects(int? schoolid)
        {
            if (schoolid.HasValue)
            {
                return _db.subjects.Where(x => x.schoolid == schoolid.Value);
            }
            return _db.subjects;
        }

        /// <inheritdoc/>
        public IEnumerable<Subject> GetSubjectsImTeaching(long teacherid, int year)
        {
            return _db.classes_teachers_allocateds
                .Where(x => x.year == year && x.teacherid == teacherid)
                .Select(x => x.subject!)
                .Where(s => s != null)
                .Distinct();
        }

        /// <inheritdoc/>
        public IEnumerable<SubjectTeacher> GetSubjectTeachers(long teacherid, int year)
        {
            return _db.subject_teachers.Where(x => x.teacherid == teacherid && x.year == year);
        }
    }
}
