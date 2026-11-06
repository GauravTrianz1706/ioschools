using ioschools.Data;
using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - ECA (Extra-Curricular Activity) operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void AddEca(Eca eca)
        {
            _db.ecas.Add(eca);
        }

        /// <inheritdoc/>
        public void DeleteEca(long id)
        {
            var exist = GetEca(id);
            if (exist != null)
            {
                var studentEcas = _db.eca_students.Where(x => x.ecaid == exist.id);
                _db.eca_students.RemoveRange(studentEcas);
                _db.ecas.Remove(exist);
                Save();
            }
        }

        /// <inheritdoc/>
        public void DeleteStudentEca(long id)
        {
            var exist = _db.eca_students.SingleOrDefault(x => x.id == id);
            if (exist != null)
            {
                var attendances = _db.attendances.Where(x => x.ecaid.HasValue && x.ecaid.Value == id);
                _db.attendances.RemoveRange(attendances);
                _db.eca_students.Remove(exist);
                Save();
            }
        }

        /// <inheritdoc/>
        public Eca? GetEca(long id)
        {
            return _db.ecas.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public EcaStudent? GetStudentEca(long id)
        {
            return _db.eca_students.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IEnumerable<Eca> GetViewableEcas(Schools? userschool)
        {
            if (!userschool.HasValue)
            {
                return _db.ecas;
            }
            switch (userschool)
            {
                case Schools.International:
                    return _db.ecas.Where(x => x.schoolid == (int)Schools.International);
                case Schools.Kindergarten:
                case Schools.Primary:
                case Schools.Secondary:
                    return _db.ecas.Where(x => x.schoolid != (int)Schools.International);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Eca> GetEcas(int? schoolid)
        {
            if (!schoolid.HasValue)
            {
                return _db.ecas;
            }
            return _db.ecas.Where(x => x.schoolid == schoolid);
        }
    }
}
