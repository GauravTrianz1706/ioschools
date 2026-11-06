using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Student discipline operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void DeleteDiscipline(StudentsDiscipline discipline)
        {
            _db.students_disciplines.Remove(discipline);
        }

        /// <inheritdoc/>
        public StudentsDiscipline? GetDiscipline(long id)
        {
            return _db.students_disciplines.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IEnumerable<StudentsDiscipline> GetDisciplines()
        {
            return _db.students_disciplines;
        }
    }
}
