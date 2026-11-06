using ioschools.Data;
using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Registration/Enrolment operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void AddRegistration(Registration registration)
        {
            _db.registrations.Add(registration);
        }

        /// <inheritdoc/>
        public void DeleteRegistrationNotificationByUser(long userid)
        {
            var exist = _db.registration_notifications.SingleOrDefault(x => x.userid == userid);
            if (exist != null)
            {
                _db.registration_notifications.Remove(exist);
            }
            Save();
        }

        /// <inheritdoc/>
        public void DeleteRegistraion(Registration registration)
        {
            _db.registrations.Remove(registration);
            Save();
        }

        /// <inheritdoc/>
        public Registration? GetRegistration(long id)
        {
            return _db.registrations.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IQueryable<Registration> GetRegistration(int? school, RegistrationStatus? status, int? year, int? classyear)
        {
            var registrations = _db.registrations.AsQueryable();

            if (school.HasValue)
            {
                registrations = registrations.Where(x => x.schoolid == school.Value);
            }

            if (status.HasValue)
            {
                registrations = registrations.Where(x => x.status == status.ToString());
            }

            if (year.HasValue)
            {
                registrations = registrations.Where(x => x.enrollingYear.HasValue && x.enrollingYear.Value == year.Value);
            }

            if (classyear.HasValue)
            {
                registrations = registrations.Where(x => x.schoolyearid.HasValue && x.schoolyearid.Value == classyear);
            }

            return registrations;
        }

        /// <inheritdoc/>
        public IEnumerable<RegistrationNotification> GetRegistrationNotifications()
        {
            return _db.registration_notifications;
        }
    }
}
