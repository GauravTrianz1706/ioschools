using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Mail operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public IQueryable<Mail> GetMails()
        {
            return _db.mails;
        }

        /// <inheritdoc/>
        public void DeleteMail(Mail entry)
        {
            _db.mails.Remove(entry);
        }
    }
}
