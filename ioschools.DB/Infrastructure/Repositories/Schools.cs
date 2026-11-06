using ioschools.Data;
using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - School operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public IQueryable<School> GetSchools()
        {
            return _db.schools;
        }

        /// <inheritdoc/>
        public IQueryable<School> GetSchools(Schools? myschool)
        {
            if (!myschool.HasValue)
            {
                return _db.schools;
            }
            switch (myschool.Value)
            {
                case Schools.International:
                    return _db.schools.Where(x => x.id == (int)Schools.International);
                case Schools.Primary:
                case Schools.Secondary:
                case Schools.Kindergarten:
                    return _db.schools.Where(x => x.id != (int)Schools.International);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public IQueryable<SchoolYear> GetSchoolYears()
        {
            return _db.school_years;
        }

        /// <inheritdoc/>
        public IEnumerable<SchoolTerm> GetSchoolTerms()
        {
            return _db.school_terms;
        }
    }
}
