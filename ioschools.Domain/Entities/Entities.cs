using ioschools.Domain.Enums;

namespace ioschools.Domain.Entities
{
    /// <summary>
    /// Base entity with common audit fields
    /// </summary>
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// User entity - migrated from legacy user LINQ-to-SQL entity
    /// </summary>
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public UserGroup UserGroup { get; set; }
        public Permission Permissions { get; set; }
        public int Settings { get; set; }
        public long? PhotoId { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? Race { get; set; }
        public string? Religion { get; set; }
        public DateTime? LeavingDate { get; set; }
        public int? ClassId { get; set; }

        // Navigation properties
        public UserImage? UserImage { get; set; }
        public ICollection<UserChild> Children { get; set; } = new List<UserChild>();
        public ICollection<UserParent> Parents { get; set; } = new List<UserParent>();

        public string ToName() => string.IsNullOrEmpty(MiddleName)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";

        public bool HasFlag(UserSettings flag) => (Settings & (int)flag) != 0;
        public int SetFlag(UserSettings flag) => Settings | (int)flag;
        public int UnsetFlag(UserSettings flag) => Settings & ~(int)flag;
    }

    /// <summary>
    /// User image entity
    /// </summary>
    public class UserImage : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public long? UserId { get; set; }
        public User? User { get; set; }
    }

    /// <summary>
    /// User child relationship
    /// </summary>
    public class UserChild : BaseEntity
    {
        public long ParentId { get; set; }
        public long ChildId { get; set; }
        public User? Parent { get; set; }
        public User? Child { get; set; }
    }

    /// <summary>
    /// User parent relationship
    /// </summary>
    public class UserParent : BaseEntity
    {
        public long StudentId { get; set; }
        public long ParentId { get; set; }
        public User? Student { get; set; }
        public User? Parent { get; set; }
    }

    /// <summary>
    /// Blog/News entity
    /// </summary>
    public class Blog : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public long CreatorId { get; set; }
        public User? Creator { get; set; }
        public string? ImageUrl { get; set; }
        public string? Tags { get; set; }
    }

    /// <summary>
    /// Calendar event entity
    /// </summary>
    public class CalendarEvent : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublic { get; set; }
        public long? CreatorId { get; set; }
        public User? Creator { get; set; }
    }

    /// <summary>
    /// School class entity
    /// </summary>
    public class SchoolClass : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Level { get; set; }
        public string? Description { get; set; }
        public ICollection<ClassStudentAllocation> StudentAllocations { get; set; } = new List<ClassStudentAllocation>();
        public ICollection<ClassTeacherAllocation> TeacherAllocations { get; set; } = new List<ClassTeacherAllocation>();
    }

    /// <summary>
    /// Class student allocation
    /// </summary>
    public class ClassStudentAllocation : BaseEntity
    {
        public int ClassId { get; set; }
        public long StudentId { get; set; }
        public int Year { get; set; }
        public SchoolClass? Class { get; set; }
        public User? Student { get; set; }
    }

    /// <summary>
    /// Class teacher allocation
    /// </summary>
    public class ClassTeacherAllocation : BaseEntity
    {
        public int ClassId { get; set; }
        public long TeacherId { get; set; }
        public int Year { get; set; }
        public SchoolClass? Class { get; set; }
        public User? Teacher { get; set; }
    }

    /// <summary>
    /// Subject entity
    /// </summary>
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public ICollection<SubjectTeacher> Teachers { get; set; } = new List<SubjectTeacher>();
    }

    /// <summary>
    /// Subject teacher assignment
    /// </summary>
    public class SubjectTeacher : BaseEntity
    {
        public long SubjectId { get; set; }
        public long TeacherId { get; set; }
        public int ClassId { get; set; }
        public int Year { get; set; }
        public Subject? Subject { get; set; }
        public User? Teacher { get; set; }
    }

    /// <summary>
    /// Exam entity
    /// </summary>
    public class Exam : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Term { get; set; }
        public long CreatorId { get; set; }
        public User? Creator { get; set; }
        public ICollection<ExamMark> Marks { get; set; } = new List<ExamMark>();
    }

    /// <summary>
    /// Exam mark entity
    /// </summary>
    public class ExamMark : BaseEntity
    {
        public long ExamId { get; set; }
        public long StudentId { get; set; }
        public long SubjectId { get; set; }
        public decimal? Mark { get; set; }
        public string? Grade { get; set; }
        public string? Remarks { get; set; }
        public Exam? Exam { get; set; }
        public User? Student { get; set; }
        public Subject? Subject { get; set; }
    }

    /// <summary>
    /// Attendance entity
    /// </summary>
    public class Attendance : BaseEntity
    {
        public long StudentId { get; set; }
        public DateTime Date { get; set; }
        public AttendanceType Type { get; set; }
        public string? Remarks { get; set; }
        public long? RecordedById { get; set; }
        public User? Student { get; set; }
        public User? RecordedBy { get; set; }
    }

    /// <summary>
    /// Discipline record entity
    /// </summary>
    public class Discipline : BaseEntity
    {
        public long StudentId { get; set; }
        public DisciplineType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public long? RecordedById { get; set; }
        public User? Student { get; set; }
        public User? RecordedBy { get; set; }
    }

    /// <summary>
    /// Homework entity
    /// </summary>
    public class Homework : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public long TeacherId { get; set; }
        public int ClassId { get; set; }
        public long SubjectId { get; set; }
        public User? Teacher { get; set; }
        public SchoolClass? Class { get; set; }
        public Subject? Subject { get; set; }
        public ICollection<HomeworkFile> Files { get; set; } = new List<HomeworkFile>();
        public ICollection<HomeworkAnswer> Answers { get; set; } = new List<HomeworkAnswer>();
    }

    /// <summary>
    /// Homework file attachment
    /// </summary>
    public class HomeworkFile : BaseEntity
    {
        public long HomeworkId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public Homework? Homework { get; set; }
    }

    /// <summary>
    /// Homework answer entity
    /// </summary>
    public class HomeworkAnswer : BaseEntity
    {
        public long HomeworkId { get; set; }
        public long StudentId { get; set; }
        public string? Content { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Homework? Homework { get; set; }
        public User? Student { get; set; }
        public ICollection<HomeworkAnswerFile> Files { get; set; } = new List<HomeworkAnswerFile>();
    }

    /// <summary>
    /// Homework answer file
    /// </summary>
    public class HomeworkAnswerFile : BaseEntity
    {
        public long AnswerId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public HomeworkAnswer? Answer { get; set; }
    }

    /// <summary>
    /// School fee entity
    /// </summary>
    public class SchoolFee : BaseEntity
    {
        public long StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public int Term { get; set; }
        public User? Student { get; set; }
    }

    /// <summary>
    /// Staff leave entity
    /// </summary>
    public class StaffLeave : BaseEntity
    {
        public long StaffId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending";
        public long? ApprovedById { get; set; }
        public User? Staff { get; set; }
        public User? ApprovedBy { get; set; }
    }

    /// <summary>
    /// ECA (Extra-Curricular Activity) entity
    /// </summary>
    public class ECA : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? TeacherId { get; set; }
        public User? Teacher { get; set; }
        public ICollection<ECAStudent> Students { get; set; } = new List<ECAStudent>();
    }

    /// <summary>
    /// ECA student enrollment
    /// </summary>
    public class ECAStudent : BaseEntity
    {
        public long ECAId { get; set; }
        public long StudentId { get; set; }
        public int Year { get; set; }
        public string? Role { get; set; }
        public ECA? ECAActivity { get; set; }
        public User? Student { get; set; }
    }

    /// <summary>
    /// Enrolment/Admission entity
    /// </summary>
    public class Enrolment : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public AdmissionStatus Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? Notes { get; set; }
        public IdType IdType { get; set; }
        public string? IdNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
    }

    /// <summary>
    /// Schedule entity
    /// </summary>
    public class Schedule : BaseEntity
    {
        public int ClassId { get; set; }
        public long SubjectId { get; set; }
        public long TeacherId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Year { get; set; }
        public int Term { get; set; }
        public SchoolClass? Class { get; set; }
        public Subject? Subject { get; set; }
        public User? Teacher { get; set; }
    }

    /// <summary>
    /// Email queue entity
    /// </summary>
    public class EmailQueue : BaseEntity
    {
        public string ToEmail { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public int RetryCount { get; set; }
        public string? CcList { get; set; }
    }

    /// <summary>
    /// Exam template entity
    /// </summary>
    public class ExamTemplate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Level { get; set; }
        public ICollection<ExamTemplateSubject> Subjects { get; set; } = new List<ExamTemplateSubject>();
    }

    /// <summary>
    /// Exam template subject
    /// </summary>
    public class ExamTemplateSubject : BaseEntity
    {
        public long TemplateId { get; set; }
        public long SubjectId { get; set; }
        public decimal MaxMark { get; set; }
        public decimal PassMark { get; set; }
        public ExamTemplate? Template { get; set; }
        public Subject? Subject { get; set; }
    }

    /// <summary>
    /// Grading rule entity
    /// </summary>
    public class GradingRule : BaseEntity
    {
        public string Grade { get; set; } = string.Empty;
        public decimal MinMark { get; set; }
        public decimal MaxMark { get; set; }
        public string? Description { get; set; }
        public int Level { get; set; }
    }

    /// <summary>
    /// School term entity
    /// </summary>
    public class SchoolTerm : BaseEntity
    {
        public int Year { get; set; }
        public int Term { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Conduct record entity
    /// </summary>
    public class Conduct : BaseEntity
    {
        public long StudentId { get; set; }
        public string Grade { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Term { get; set; }
        public string? Remarks { get; set; }
        public User? Student { get; set; }
    }

    /// <summary>
    /// Feedback entity
    /// </summary>
    public class Feedback : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Category { get; set; }
        public bool IsResolved { get; set; }
    }

    /// <summary>
    /// Vacancy entity
    /// </summary>
    public class Vacancy : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Requirements { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ClosingDate { get; set; }
    }

    /// <summary>
    /// Change log entity
    /// </summary>
    public class ChangeLog : BaseEntity
    {
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
    }
}
