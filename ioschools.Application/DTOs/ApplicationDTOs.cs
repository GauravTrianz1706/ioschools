using ioschools.Domain.Enums;

namespace ioschools.Application.DTOs
{
    /// <summary>
    /// User DTO for API responses - replaces legacy UserViewModel
    /// </summary>
    public class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string FullName => string.IsNullOrEmpty(MiddleName)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";
        public UserGroup UserGroup { get; set; }
        public Permission Permissions { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    /// <summary>
    /// Login request DTO
    /// </summary>
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string? Redirect { get; set; }
    }

    /// <summary>
    /// Login response DTO
    /// </summary>
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? RedirectUrl { get; set; }
        public UserDto? User { get; set; }
    }

    /// <summary>
    /// Blog DTO
    /// </summary>
    public class BlogDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public long CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public string? Tags { get; set; }
    }

    /// <summary>
    /// Calendar event DTO
    /// </summary>
    public class CalendarEventDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsPublic { get; set; }
    }

    /// <summary>
    /// Exam DTO
    /// </summary>
    public class ExamDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Term { get; set; }
        public long CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Exam mark DTO
    /// </summary>
    public class ExamMarkDto
    {
        public long Id { get; set; }
        public long ExamId { get; set; }
        public long StudentId { get; set; }
        public string? StudentName { get; set; }
        public long SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public decimal? Mark { get; set; }
        public string? Grade { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Attendance DTO
    /// </summary>
    public class AttendanceDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string? StudentName { get; set; }
        public DateTime Date { get; set; }
        public AttendanceType Type { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Homework DTO
    /// </summary>
    public class HomeworkDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public long TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public long SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<HomeworkFileDto> Files { get; set; } = new();
    }

    /// <summary>
    /// Homework file DTO
    /// </summary>
    public class HomeworkFileDto
    {
        public long Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
    }

    /// <summary>
    /// School fee DTO
    /// </summary>
    public class SchoolFeeDto
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string? StudentName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? Description { get; set; }
        public int Year { get; set; }
        public int Term { get; set; }
    }

    /// <summary>
    /// JSON response wrapper - replaces legacy ToJsonOKData/ToJsonFail pattern
    /// </summary>
    public class JsonResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static JsonResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static JsonResponse<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }

    /// <summary>
    /// Non-generic JSON response
    /// </summary>
    public class JsonResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static JsonResponse Ok(string? message = null) =>
            new() { Success = true, Message = message };

        public static JsonResponse Fail(string message) =>
            new() { Success = false, Message = message };
    }

    /// <summary>
    /// Id/Name pair DTO - replaces legacy IdName model
    /// </summary>
    public class IdNameDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public IdNameDto() { }
        public IdNameDto(long id, string name) { Id = id; Name = name; }
    }

    /// <summary>
    /// Dashboard view model DTO
    /// </summary>
    public class DashboardDto
    {
        public UserDto? CurrentUser { get; set; }
        public int TotalStudents { get; set; }
        public int TotalStaff { get; set; }
        public int PendingHomework { get; set; }
        public int UpcomingExams { get; set; }
        public List<CalendarEventDto> UpcomingEvents { get; set; } = new();
        public List<BlogDto> RecentNews { get; set; } = new();
    }

    /// <summary>
    /// Password change DTO
    /// </summary>
    public class ChangePasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Forgot password DTO
    /// </summary>
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// File upload result DTO
    /// </summary>
    public class FileUploadResultDto
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
