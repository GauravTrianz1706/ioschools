namespace ioschools.Domain.Enums
{
    /// <summary>
    /// User group flags - migrated from legacy UserGroup enum
    /// </summary>
    [Flags]
    public enum UserGroup
    {
        NONE = 0,
        ADMIN = 1,
        TEACHER = 2,
        STUDENT = 4,
        PARENT = 8,
        FINANCE = 16,
        PRINCIPAL = 32,
        CLERK = 64
    }

    /// <summary>
    /// Permission flags - migrated from legacy Permission enum
    /// </summary>
    [Flags]
    public enum Permission
    {
        NONE = 0,
        BLOG_CREATE = 1,
        BLOG_ADMIN = 2,
        ATTENDANCE_CREATE = 4,
        ATTENDANCE_ADMIN = 8,
        EXAM_CREATE = 16,
        EXAM_ADMIN = 32,
        HOMEWORK_CREATE = 64,
        HOMEWORK_ADMIN = 128,
        FINANCE_VIEW = 256,
        FINANCE_ADMIN = 512,
        DISCIPLINE_CREATE = 1024,
        DISCIPLINE_ADMIN = 2048,
        ECA_CREATE = 4096,
        ECA_ADMIN = 8192,
        SCHEDULE_CREATE = 16384,
        SCHEDULE_ADMIN = 32768,
        LEAVE_CREATE = 65536,
        LEAVE_ADMIN = 131072,
        USER_ADMIN = 262144,
        SCHOOL_ADMIN = 524288
    }

    /// <summary>
    /// User settings flags
    /// </summary>
    [Flags]
    public enum UserSettings
    {
        NONE = 0,
        INACTIVE = 1,
        PASSWORD_RESET = 2,
        EMAIL_NOTIFICATIONS = 4
    }

    /// <summary>
    /// Attendance type
    /// </summary>
    public enum AttendanceType
    {
        PRESENT = 1,
        ABSENT = 2,
        LATE = 3,
        EXCUSED = 4
    }

    /// <summary>
    /// Discipline type
    /// </summary>
    public enum DisciplineType
    {
        WARNING = 1,
        DETENTION = 2,
        SUSPENSION = 3,
        EXPULSION = 4
    }

    /// <summary>
    /// Admission status
    /// </summary>
    public enum AdmissionStatus
    {
        PENDING = 1,
        APPROVED = 2,
        REJECTED = 3,
        WAITLISTED = 4
    }

    /// <summary>
    /// ID type for enrolment
    /// </summary>
    public enum IdType
    {
        IC = 1,
        PASSPORT = 2,
        BIRTH_CERT = 3
    }

    /// <summary>
    /// Email view type - migrated from legacy EmailViewType
    /// </summary>
    public enum EmailViewType
    {
        PASSWORD_RESET,
        REGISTRATION,
        REGISTRATION_NOTIFICATION,
        REGISTRATION_UPDATE,
        ATTENDANCE,
        NOTIFICATION_TO_PARENT,
        NOTIFICATION_TO_TEACHER,
        FEEDBACK,
        FEEDBACK_VENDOR,
        CIRCULAR,
        LEAVE_APPROVAL,
        LEAVE_UPDATED
    }

    /// <summary>
    /// Uploader type for file uploads
    /// </summary>
    public enum UploaderType
    {
        CALENDAR,
        BLOG,
        PHOTO,
        HOMEWORK,
        REGISTRATION
    }

    /// <summary>
    /// Cache timer type - migrated from legacy CacheTimerType
    /// </summary>
    public enum CacheTimerType
    {
        Minute1,
        Minute5,
        Minute10,
        Minute60
    }
}
