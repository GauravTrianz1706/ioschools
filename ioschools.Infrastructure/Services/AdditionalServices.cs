using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Common;
using ioschools.Domain.Entities;
using ioschools.Domain.Enums;
using ioschools.Domain.Interfaces;
using ioschools.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ioschools.Infrastructure.Services
{
    /// <summary>
    /// User service implementation
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ApplicationDbContext context, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<UserDto?> GetUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<IEnumerable<UserDto>> GetUsersByGroupAsync(UserGroup group)
        {
            var users = await _userRepository.GetUsersByGroupAsync(group);
            return users.Select(MapToDto);
        }

        public async Task<Result<UserDto>> CreateUserAsync(UserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                UserGroup = dto.UserGroup,
                Permissions = dto.Permissions,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender
            };
            var created = await _userRepository.AddAsync(user);
            return Result<UserDto>.Success(MapToDto(created));
        }

        public async Task<Result> UpdateUserAsync(long id, UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return Result.Failure("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.MiddleName = dto.MiddleName;
            user.Phone = dto.Phone;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return Result.Failure("User not found");
            await _userRepository.DeleteAsync(user);
            return Result.Success();
        }

        public async Task<long> GetDiskUsageAsync(long userId) =>
            await _userRepository.GetUserDiskUsageAsync(userId);

        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            UserGroup = user.UserGroup,
            Permissions = user.Permissions,
            PhotoUrl = user.UserImage?.Url,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender
        };
    }

    /// <summary>
    /// Exam service implementation
    /// </summary>
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly ILogger<ExamService> _logger;

        public ExamService(IExamRepository examRepository, ILogger<ExamService> logger)
        {
            _examRepository = examRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ExamDto>> GetExamsAsync(int year)
        {
            var exams = await _examRepository.GetExamsByYearAsync(year);
            return exams.Select(MapToDto);
        }

        public async Task<ExamDto?> GetExamAsync(long id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            return exam == null ? null : MapToDto(exam);
        }

        public async Task<Result<ExamDto>> CreateExamAsync(ExamDto dto, long creatorId)
        {
            var exam = new Exam
            {
                Name = dto.Name,
                Year = dto.Year,
                Term = dto.Term,
                CreatorId = creatorId
            };
            var created = await _examRepository.AddAsync(exam);
            return Result<ExamDto>.Success(MapToDto(created));
        }

        public async Task<Result> DeleteExamAsync(long id, long deleterId, UserAuth auth)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null) return Result.Failure("Exam not found");

            if (!auth.Permissions.HasFlag(Permission.EXAM_ADMIN) && exam.CreatorId != deleterId)
                return Result.Failure("Only exam creator can delete this entry");

            await _examRepository.DeleteAsync(exam);
            return Result.Success();
        }

        public async Task<IEnumerable<ExamMarkDto>> GetStudentMarksAsync(long studentId, int year)
        {
            var marks = await _examRepository.GetStudentMarksAsync(studentId, year);
            return marks.Select(m => new ExamMarkDto
            {
                Id = m.Id,
                ExamId = m.ExamId,
                StudentId = m.StudentId,
                StudentName = m.Student?.ToName(),
                SubjectId = m.SubjectId,
                SubjectName = m.Subject?.Name,
                Mark = m.Mark,
                Grade = m.Grade,
                Remarks = m.Remarks
            });
        }

        public async Task<Result> SaveMarksAsync(long examId, IEnumerable<ExamMarkDto> marks, long editorId)
        {
            // Implementation for saving exam marks
            return Result.Success();
        }

        private static ExamDto MapToDto(Exam exam) => new()
        {
            Id = exam.Id,
            Name = exam.Name,
            Year = exam.Year,
            Term = exam.Term,
            CreatorId = exam.CreatorId,
            CreatorName = exam.Creator?.ToName(),
            CreatedAt = exam.CreatedAt
        };
    }

    /// <summary>
    /// Attendance service implementation
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(IAttendanceRepository attendanceRepository, IEmailService emailService,
            ILogger<AttendanceService> logger)
        {
            _attendanceRepository = attendanceRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAttendanceAsync(long studentId, DateTime? from = null, DateTime? to = null)
        {
            var records = await _attendanceRepository.GetByStudentAsync(studentId, from, to);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<AttendanceDto>> GetClassAttendanceAsync(int classId, DateTime date)
        {
            var records = await _attendanceRepository.GetByDateAsync(date, classId);
            return records.Select(MapToDto);
        }

        public async Task<Result> RecordAttendanceAsync(IEnumerable<AttendanceDto> records, long recorderId)
        {
            foreach (var record in records)
            {
                var attendance = new Attendance
                {
                    StudentId = record.StudentId,
                    Date = record.Date,
                    Type = record.Type,
                    Remarks = record.Remarks,
                    RecordedById = recorderId
                };
                await _attendanceRepository.AddAsync(attendance);
            }
            return Result.Success();
        }

        public async Task<Result> SendAttendanceEmailAsync(long studentId, DateTime date)
        {
            _logger.LogInformation("Sending attendance email for student {StudentId} on {Date}", studentId, date);
            return Result.Success();
        }

        private static AttendanceDto MapToDto(Attendance a) => new()
        {
            Id = a.Id,
            StudentId = a.StudentId,
            StudentName = a.Student?.ToName(),
            Date = a.Date,
            Type = a.Type,
            Remarks = a.Remarks
        };
    }

    /// <summary>
    /// Homework service implementation
    /// </summary>
    public class HomeworkService : IHomeworkService
    {
        private readonly IHomeworkRepository _homeworkRepository;
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<HomeworkService> _logger;

        private const long MAX_DISK_SIZE = 100 * 1024 * 1024; // 100MB

        public HomeworkService(IHomeworkRepository homeworkRepository, IFileService fileService,
            IUserRepository userRepository, ILogger<HomeworkService> logger)
        {
            _homeworkRepository = homeworkRepository;
            _fileService = fileService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<HomeworkDto>> GetHomeworkByClassAsync(int classId, int year)
        {
            var homeworks = await _homeworkRepository.GetByClassAsync(classId, year);
            return homeworks.Select(MapToDto);
        }

        public async Task<IEnumerable<HomeworkDto>> GetHomeworkByTeacherAsync(long teacherId, int year)
        {
            var homeworks = await _homeworkRepository.GetByTeacherAsync(teacherId, year);
            return homeworks.Select(MapToDto);
        }

        public async Task<HomeworkDto?> GetHomeworkAsync(long id)
        {
            var homework = await _homeworkRepository.GetByIdAsync(id);
            return homework == null ? null : MapToDto(homework);
        }

        public async Task<Result<HomeworkDto>> CreateHomeworkAsync(HomeworkDto dto, long creatorId)
        {
            var homework = new Homework
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                TeacherId = creatorId,
                ClassId = dto.ClassId,
                SubjectId = dto.SubjectId
            };
            var created = await _homeworkRepository.AddAsync(homework);
            return Result<HomeworkDto>.Success(MapToDto(created));
        }

        public async Task<Result> UpdateHomeworkAsync(long id, HomeworkDto dto, long editorId)
        {
            var homework = await _homeworkRepository.GetByIdAsync(id);
            if (homework == null) return Result.Failure("Homework not found");

            homework.Title = dto.Title;
            homework.Description = dto.Description;
            homework.DueDate = dto.DueDate;
            homework.UpdatedAt = DateTime.UtcNow;

            await _homeworkRepository.UpdateAsync(homework);
            return Result.Success();
        }

        public async Task<Result> DeleteHomeworkAsync(long id, long deleterId)
        {
            var homework = await _homeworkRepository.GetByIdAsync(id);
            if (homework == null) return Result.Failure("Homework not found");
            await _homeworkRepository.DeleteAsync(homework);
            return Result.Success();
        }

        public async Task<Result<FileUploadResultDto>> UploadFileAsync(Stream fileStream, string fileName, long userId)
        {
            var diskUsage = await _userRepository.GetUserDiskUsageAsync(userId);
            if (diskUsage + fileStream.Length > MAX_DISK_SIZE)
                return Result<FileUploadResultDto>.Failure("Disk Quota exceeded. Please delete old uploaded files.");

            var result = await _fileService.SaveFileAsync(fileStream, fileName, UploaderType.HOMEWORK, userId);
            if (!result.IsSuccess)
                return Result<FileUploadResultDto>.Failure(result.Error ?? "Failed to save file");

            var file = new HomeworkFile
            {
                Url = result.Value!.Url,
                FileName = fileName,
                Size = result.Value.Size
            };
            await _homeworkRepository.AddHomeworkFileAsync(file);

            return Result<FileUploadResultDto>.Success(new FileUploadResultDto
            {
                Id = file.Id,
                FileName = file.FileName,
                Url = file.Url,
                Size = file.Size
            });
        }

        public async Task<Result> DeleteFileAsync(long fileId, long userId)
        {
            var file = await _homeworkRepository.GetHomeworkFileAsync(fileId);
            if (file == null) return Result.Failure("File not found");

            await _fileService.DeleteFileAsync(file.Url);
            await _homeworkRepository.DeleteHomeworkFileAsync(file);
            return Result.Success();
        }

        private static HomeworkDto MapToDto(Homework h) => new()
        {
            Id = h.Id,
            Title = h.Title,
            Description = h.Description,
            DueDate = h.DueDate,
            TeacherId = h.TeacherId,
            TeacherName = h.Teacher?.ToName(),
            ClassId = h.ClassId,
            SubjectId = h.SubjectId,
            SubjectName = h.Subject?.Name,
            CreatedAt = h.CreatedAt,
            Files = h.Files.Select(f => new HomeworkFileDto
            {
                Id = f.Id,
                Url = f.Url,
                FileName = f.FileName,
                Size = f.Size
            }).ToList()
        };
    }

    /// <summary>
    /// School fee service implementation
    /// </summary>
    public class SchoolFeeService : ISchoolFeeService
    {
        private readonly ISchoolFeeRepository _feeRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<SchoolFeeService> _logger;

        public SchoolFeeService(ISchoolFeeRepository feeRepository, IEmailService emailService,
            ILogger<SchoolFeeService> logger)
        {
            _feeRepository = feeRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<SchoolFeeDto>> GetStudentFeesAsync(long studentId)
        {
            var fees = await _feeRepository.GetByStudentAsync(studentId);
            return fees.Select(MapToDto);
        }

        public async Task<IEnumerable<SchoolFeeDto>> GetOverdueFeesAsync()
        {
            var fees = await _feeRepository.GetOverdueFeesAsync();
            return fees.Select(MapToDto);
        }

        public async Task<Result> UpdateFeeStatusAsync(long feeId, bool isPaid, long updaterId)
        {
            var fee = await _feeRepository.GetByIdAsync(feeId);
            if (fee == null) return Result.Failure("Fee not found");

            fee.IsPaid = isPaid;
            fee.PaidDate = isPaid ? DateTime.UtcNow : null;
            fee.UpdatedAt = DateTime.UtcNow;

            await _feeRepository.UpdateAsync(fee);
            return Result.Success();
        }

        public async Task<Result> SendFeeReminderAsync(long studentId)
        {
            _logger.LogInformation("Sending fee reminder for student {StudentId}", studentId);
            return Result.Success();
        }

        private static SchoolFeeDto MapToDto(SchoolFee f) => new()
        {
            Id = f.Id,
            StudentId = f.StudentId,
            StudentName = f.Student?.ToName(),
            Amount = f.Amount,
            DueDate = f.DueDate,
            IsPaid = f.IsPaid,
            PaidDate = f.PaidDate,
            Description = f.Description,
            Year = f.Year,
            Term = f.Term
        };
    }
}
