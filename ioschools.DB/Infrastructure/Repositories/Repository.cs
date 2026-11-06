using ioschools.Data;
using ioschools.Data.Attendance;
using ioschools.Data.User;
using ioschools.DB.Application.Interfaces;
using ioschools.DB.Domain.Entities;
using ioschools.DB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IRepository"/>.
    /// Replaces the legacy LINQ-to-SQL partial Repository class.
    /// </summary>
    public partial class Repository : IRepository, IDisposable
    {
        private readonly IoschoolsDbContext _db;
        private readonly ILogger<Repository> _logger;

        public Repository(IoschoolsDbContext db, ILogger<Repository> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Save()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict detected while saving changes.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving changes.");
                throw;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<int> GetOperationalYears()
        {
            var studentYears = _db.classes_students_allocateds.Select(x => x.year).Distinct().ToArray();
            var teacherYears = _db.classes_teachers_allocateds.Select(x => x.year).Distinct().ToArray();
            var years = new HashSet<int>(studentYears);
            years.UnionWith(teacherYears);
            return years;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
