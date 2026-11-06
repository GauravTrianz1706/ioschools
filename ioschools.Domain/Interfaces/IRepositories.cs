using ioschools.Domain.Entities;
using ioschools.Domain.Enums;

namespace ioschools.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface - replaces legacy IRepository
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(long id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }

    /// <summary>
    /// User repository interface
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetActiveUserLoginAsync(string email, string passwordHash);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetUsersByGroupAsync(UserGroup group);
        Task<long> GetUserDiskUsageAsync(long userId);
        Task DeleteUserImageAsync(long imageId);
        Task<long> AddUserImageAsync(UserImage image);
    }

    /// <summary>
    /// Blog repository interface
    /// </summary>
    public interface IBlogRepository : IRepository<Blog>
    {
        Task<IEnumerable<Blog>> GetPublicBlogsAsync();
        Task<IEnumerable<Blog>> GetBlogsByCreatorAsync(long creatorId);
    }

    /// <summary>
    /// Exam repository interface
    /// </summary>
    public interface IExamRepository : IRepository<Exam>
    {
        Task<Exam?> GetExamWithMarksAsync(long examId);
        Task<IEnumerable<Exam>> GetExamsByYearAsync(int year);
        Task<IEnumerable<ExamMark>> GetStudentMarksAsync(long studentId, int year);
    }

    /// <summary>
    /// Attendance repository interface
    /// </summary>
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<IEnumerable<Attendance>> GetByStudentAsync(long studentId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date, int? classId = null);
    }

    /// <summary>
    /// Homework repository interface
    /// </summary>
    public interface IHomeworkRepository : IRepository<Homework>
    {
        Task<HomeworkFile?> GetHomeworkFileAsync(long fileId);
        Task AddHomeworkFileAsync(HomeworkFile file);
        Task DeleteHomeworkFileAsync(HomeworkFile file);
        Task<IEnumerable<Homework>> GetByClassAsync(int classId, int year);
        Task<IEnumerable<Homework>> GetByTeacherAsync(long teacherId, int year);
    }

    /// <summary>
    /// School fee repository interface
    /// </summary>
    public interface ISchoolFeeRepository : IRepository<SchoolFee>
    {
        Task<IEnumerable<SchoolFee>> GetByStudentAsync(long studentId);
        Task<IEnumerable<SchoolFee>> GetOverdueFeesAsync();
    }

    /// <summary>
    /// Email queue repository interface
    /// </summary>
    public interface IEmailQueueRepository : IRepository<EmailQueue>
    {
        Task<IEnumerable<EmailQueue>> GetPendingEmailsAsync();
        Task MarkAsSentAsync(long emailId);
    }

    /// <summary>
    /// Unit of work interface - replaces legacy DataContext.SubmitChanges()
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IBlogRepository Blogs { get; }
        IExamRepository Exams { get; }
        IAttendanceRepository Attendance { get; }
        IHomeworkRepository Homework { get; }
        ISchoolFeeRepository SchoolFees { get; }
        IEmailQueueRepository EmailQueue { get; }
        Task<int> SaveChangesAsync();
    }
}
