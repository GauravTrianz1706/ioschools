namespace ioschools.Data
{
    /// <summary>
    /// Represents the registration / enrolment application status of a prospective student.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum RegistrationStatus
    {
        /// <summary>Application has been submitted and is awaiting review.</summary>
        PENDING,

        /// <summary>Application has been rejected.</summary>
        REJECTED,

        /// <summary>Application has been accepted.</summary>
        ACCEPTED
    }
}
