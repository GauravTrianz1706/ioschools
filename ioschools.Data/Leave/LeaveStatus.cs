namespace ioschools.Data.Leave
{
    /// <summary>
    /// Represents the approval workflow status of a staff leave request.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum LeaveStatus
    {
        /// <summary>Leave request has been submitted and is awaiting review.</summary>
        PENDING = 1,

        /// <summary>Leave request has been approved by an administrator.</summary>
        APPROVED = 2,

        /// <summary>Leave request has been rejected by an administrator.</summary>
        REJECTED = 3,

        /// <summary>Leave request was cancelled by the applicant.</summary>
        CANCELLED = 4
    }
}
