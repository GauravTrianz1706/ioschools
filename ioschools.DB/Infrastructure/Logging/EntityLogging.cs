using ioschools.DB.Domain.Entities;
using ioschools.DB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ioschools.DB.Infrastructure.Logging
{
    /// <summary>
    /// Provides entity change auditing using EF Core change tracking.
    /// Replaces the legacy LINQ-to-SQL <c>EntityLogging</c> class that used
    /// <c>DataContext.GetTable&lt;T&gt;().GetModifiedMembers()</c>.
    /// </summary>
    public static class EntityLogging
    {
        /// <summary>
        /// Audits changes to a specific tracked entity and persists a changelog entry.
        /// </summary>
        /// <typeparam name="T">The entity type being audited.</typeparam>
        /// <param name="dbContext">The EF Core <see cref="IoschoolsDbContext"/>.</param>
        /// <param name="modifiedEntity">The entity whose changes are being audited.</param>
        /// <param name="target">A label describing the target (e.g., entity name or action).</param>
        /// <param name="userid">The ID of the user performing the change.</param>
        public static void LogChanges<T>(IoschoolsDbContext dbContext, T modifiedEntity, string target, long userid)
            where T : class
        {
            if (dbContext == null || modifiedEntity == null)
                return;

            var sb = new StringBuilder();
            sb.AppendFormat("{0}: ", target);

            // Use EF Core change tracker to detect modified properties
            var entry = dbContext.Entry(modifiedEntity);
            if (entry.State == EntityState.Modified)
            {
                foreach (var property in entry.Properties)
                {
                    if (property.IsModified)
                    {
                        sb.AppendFormat("{0}[{1} -> {2}] ",
                            property.Metadata.Name,
                            property.OriginalValue,
                            property.CurrentValue);
                    }
                }
            }

            var change = new Changelog
            {
                changes = sb.ToString(),
                created = DateTime.Now,
                userid = userid
            };

            dbContext.changelogs.Add(change);
            dbContext.SaveChanges();
        }
    }
}
