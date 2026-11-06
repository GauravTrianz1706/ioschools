using ioschools.Application.DTOs;
using ioschools.Domain.Common;

namespace ioschools.Application.Interfaces
{
    /// <summary>
    /// Authentication service interface - replaces legacy cookie-based auth in baseController
    /// </summary>
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<Result> ChangePasswordAsync(long userId, ChangePasswordDto dto);
        Task<Result> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<Result> ResetPasswordAsync(long userId, long requesterId, UserAuth requesterAuth);
        string GeneratePasswordHash(string email, string password);
        bool VerifyPassword(string email, string password, string hash);
    }

    /// <summary>
    /// User service interface
    /// </summary>
    public interface IUserService
    {
        Task<UserDto?> GetUserAsync(long id);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersByGroupAsync(Domain.Enums.UserGroup group);
        Task<Result<UserDto>> CreateUserAsync(UserDto dto);
        Task<Result> UpdateUserAsync(long id, UserDto dto);
        Task<Result> DeleteUserAsync(long id);
        Task<long> GetDiskUsageAsync(long userId);
    }

    /// <summary>
    /// Blog service interface
    /// </summary>
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetPublicBlogsAsync();
        Task<IEnumerable<BlogDto>> GetAllBlogsAsync();
        Task<BlogDto?> GetBlogAsync(long id);
        Task<Result<BlogDto>> CreateBlogAsync(BlogDto dto, long creatorId);
        Task<Result> UpdateBlogAsync(long id, BlogDto dto, long editorId);
        Task<Result> DeleteBlogAsync(long id, long deleterId);
    }

    /// <summary>
    /// Exam service interface
    /// </summary>
    public interface IExamService
    {
        Task<IEnumerable<ExamDto>> GetExamsAsync(int year);
        Task<ExamDto?> GetExamAsync(long id);
        Task<Result<ExamDto>> CreateExamAsync(ExamDto dto, long creatorId);
        Task<Result> DeleteExamAsync(long id, long deleterId, UserAuth auth);
        Task<IEnumerable<ExamMarkDto>> GetStudentMarksAsync(long studentId, int year);
        Task<Result> SaveMarksAsync(long examId, IEnumerable<ExamMarkDto> marks, long editorId);
    }

    /// <summary>
    /// Attendance service interface
    /// </summary>
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAttendanceAsync(long studentId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<AttendanceDto>> GetClassAttendanceAsync(int classId, DateTime date);
        Task<Result> RecordAttendanceAsync(IEnumerable<AttendanceDto> records, long recorderId);
        Task<Result> SendAttendanceEmailAsync(long studentId, DateTime date);
    }

    /// <summary>
    /// Homework service interface
    /// </summary>
    public interface IHomeworkService
    {
        Task<IEnumerable<HomeworkDto>> GetHomeworkByClassAsync(int classId, int year);
        Task<IEnumerable<HomeworkDto>> GetHomeworkByTeacherAsync(long teacherId, int year);
        Task<HomeworkDto?> GetHomeworkAsync(long id);
        Task<Result<HomeworkDto>> CreateHomeworkAsync(HomeworkDto dto, long creatorId);
        Task<Result> UpdateHomeworkAsync(long id, HomeworkDto dto, long editorId);
        Task<Result> DeleteHomeworkAsync(long id, long deleterId);
        Task<Result<FileUploadResultDto>> UploadFileAsync(Stream fileStream, string fileName, long userId);
        Task<Result> DeleteFileAsync(long fileId, long userId);
    }

    /// <summary>
    /// School fee service interface
    /// </summary>
    public interface ISchoolFeeService
    {
        Task<IEnumerable<SchoolFeeDto>> GetStudentFeesAsync(long studentId);
        Task<IEnumerable<SchoolFeeDto>> GetOverdueFeesAsync();
        Task<Result> UpdateFeeStatusAsync(long feeId, bool isPaid, long updaterId);
        Task<Result> SendFeeReminderAsync(long studentId);
    }

    /// <summary>
    /// Email service interface - replaces legacy Email.SendMail
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string body,
            string? fromEmail = null, IEnumerable<string>? ccList = null, bool queue = true);
        Task ProcessEmailQueueAsync();
    }

    /// <summary>
    /// File service interface - replaces legacy FileHandler
    /// </summary>
    public interface IFileService
    {
        Task<Result<FileUploadResultDto>> SaveFileAsync(Stream fileStream, string fileName,
            Domain.Enums.UploaderType type, long? ownerId = null);
        Task<Result> DeleteFileAsync(string filePath);
        string GetFilePath(string relativePath);
    }

    /// <summary>
    /// Search service interface - replaces legacy LuceneSearch
    /// </summary>
    public interface ISearchService
    {
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task RebuildIndexAsync();
    }

    /// <summary>
    /// Calendar service interface
    /// </summary>
    public interface ICalendarService
    {
        Task<IEnumerable<CalendarEventDto>> GetFutureEventsAsync(int days);
        Task<IEnumerable<CalendarEventDto>> GetEventsAsync(int year, int? month = null);
        Task<Result<CalendarEventDto>> CreateEventAsync(CalendarEventDto dto, long creatorId);
        Task<Result> UpdateEventAsync(long id, CalendarEventDto dto, long editorId);
        Task<Result> DeleteEventAsync(long id, long deleterId);
    }

    /// <summary>
    /// Dashboard service interface
    /// </summary>
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(long userId, UserAuth auth);
    }

    /// <summary>
    /// Background scheduler service interface - replaces legacy CacheScheduler
    /// </summary>
    public interface ISchedulerService
    {
        Task SendPendingEmailsAsync();
        Task SetStudentInactiveIfLeavingDateSetAsync();
    }
}
