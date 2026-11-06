using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Guardian operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public IEnumerable<User> GetStudentsByGuardian(long guardianid)
        {
            return _db.students_guardians
                .Where(x => x.parentid == guardianid)
                .Select(x => x.user!)
                .Where(u => u != null);
        }
    }
}
