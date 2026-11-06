using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Finance/Fee operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public IQueryable<Fee> GetFees()
        {
            return _db.fees;
        }
    }
}
