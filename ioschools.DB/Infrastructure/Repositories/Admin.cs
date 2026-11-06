using ioschools.DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Admin/Calendar/Changelog operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public IQueryable<Calendar> GetCalendarEntries(int? year = null)
        {
            if (year.HasValue)
            {
                return _db.calendars.Where(x => x.date.Year == year.Value);
            }
            return _db.calendars;
        }

        /// <inheritdoc/>
        public IQueryable<Changelog> GetChangeLogs()
        {
            return _db.changelogs.AsQueryable();
        }

        /// <inheritdoc/>
        public void AddChangeLog(Changelog change)
        {
            _db.changelogs.Add(change);
        }
    }
}
