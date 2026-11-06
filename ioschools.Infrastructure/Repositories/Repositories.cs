using BCrypt.Net;
using ioschools.Domain.Entities;
using ioschools.Domain.Enums;
using ioschools.Domain.Interfaces;
using ioschools.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ioschools.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation - replaces legacy Repository pattern
    /// </summary>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(long id) =>
            await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }

    /// <summary>
    /// User repository implementation
    /// Migrated from: legacy Repository.GetActiveUserLogin, GetUsers, etc.
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users
                .Include(u => u.UserImage)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<User?> GetActiveUserLoginAsync(string email, string passwordHash)
        {
            var user = await _context.Users
                .Include(u => u.UserImage)
                .FirstOrDefaultAsync(u =>
                    u.Email.ToLower() == email.ToLower() &&
                    (u.Settings & (int)UserSettings.INACTIVE) == 0);

            if (user == null) return null;

            // Verify BCrypt hash
            if (!BCrypt.Net.BCrypt.Verify(email + passwordHash, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync() =>
            await _context.Users
                .Where(u => (u.Settings & (int)UserSettings.INACTIVE) == 0)
                .ToListAsync();

        public async Task<IEnumerable<User>> GetUsersByGroupAsync(UserGroup group) =>
            await _context.Users
                .Where(u => (u.UserGroup & group) != 0)
                .ToListAsync();

        public async Task<long> GetUserDiskUsageAsync(long userId)
        {
            var homeworkFiles = await _context.HomeworkFiles
                .Where(f => f.Homework != null && f.Homework.TeacherId == userId)
                .SumAsync(f => f.Size);

            var answerFiles = await _context.HomeworkAnswerFiles
                .Where(f => f.Answer != null && f.Answer.StudentId == userId)
                .SumAsync(f => f.Size);

            return homeworkFiles + answerFiles;
        }

        public async Task DeleteUserImageAsync(long imageId)
        {
            var image = await _context.UserImages.FindAsync(imageId);
            if (image != null)
            {
                _context.UserImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<long> AddUserImageAsync(UserImage image)
        {
            await _context.UserImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return image.Id;
        }
    }

    /// <summary>
    /// Blog repository implementation
    /// </summary>
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Blog>> GetPublicBlogsAsync() =>
            await _context.Blogs
                .Include(b => b.Creator)
                .Where(b => b.IsPublic)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Blog>> GetBlogsByCreatorAsync(long creatorId) =>
            await _context.Blogs
                .Where(b => b.CreatorId == creatorId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
    }

    /// <summary>
    /// Exam repository implementation
    /// </summary>
    public class ExamRepository : Repository<Exam>, IExamRepository
    {
        public ExamRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Exam?> GetExamWithMarksAsync(long examId) =>
            await _context.Exams
                .Include(e => e.Marks)
                    .ThenInclude(m => m.Student)
                .Include(e => e.Marks)
                    .ThenInclude(m => m.Subject)
                .FirstOrDefaultAsync(e => e.Id == examId);

        public async Task<IEnumerable<Exam>> GetExamsByYearAsync(int year) =>
            await _context.Exams
                .Where(e => e.Year == year)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<ExamMark>> GetStudentMarksAsync(long studentId, int year) =>
            await _context.ExamMarks
                .Include(m => m.Exam)
                .Include(m => m.Subject)
                .Where(m => m.StudentId == studentId && m.Exam != null && m.Exam.Year == year)
                .ToListAsync();
    }

    /// <summary>
    /// Attendance repository implementation
    /// </summary>
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Attendance>> GetByStudentAsync(long studentId, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Attendances
                .Where(a => a.StudentId == studentId);

            if (from.HasValue) query = query.Where(a => a.Date >= from.Value);
            if (to.HasValue) query = query.Where(a => a.Date <= to.Value);

            return await query.OrderByDescending(a => a.Date).ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date, int? classId = null)
        {
            var query = _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.Date.Date == date.Date);

            if (classId.HasValue)
                query = query.Where(a => a.Student != null && a.Student.ClassId == classId.Value);

            return await query.ToListAsync();
        }
    }

    /// <summary>
    /// Homework repository implementation
    /// </summary>
    public class HomeworkRepository : Repository<Homework>, IHomeworkRepository
    {
        public HomeworkRepository(ApplicationDbContext context) : base(context) { }

        public async Task<HomeworkFile?> GetHomeworkFileAsync(long fileId) =>
            await _context.HomeworkFiles.FindAsync(fileId);

        public async Task AddHomeworkFileAsync(HomeworkFile file)
        {
            await _context.HomeworkFiles.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHomeworkFileAsync(HomeworkFile file)
        {
            _context.HomeworkFiles.Remove(file);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Homework>> GetByClassAsync(int classId, int year) =>
            await _context.Homeworks
                .Include(h => h.Teacher)
                .Include(h => h.Subject)
                .Include(h => h.Files)
                .Where(h => h.ClassId == classId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Homework>> GetByTeacherAsync(long teacherId, int year) =>
            await _context.Homeworks
                .Include(h => h.Subject)
                .Include(h => h.Files)
                .Where(h => h.TeacherId == teacherId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
    }

    /// <summary>
    /// School fee repository implementation
    /// </summary>
    public class SchoolFeeRepository : Repository<SchoolFee>, ISchoolFeeRepository
    {
        public SchoolFeeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<SchoolFee>> GetByStudentAsync(long studentId) =>
            await _context.SchoolFees
                .Where(f => f.StudentId == studentId)
                .OrderByDescending(f => f.DueDate)
                .ToListAsync();

        public async Task<IEnumerable<SchoolFee>> GetOverdueFeesAsync() =>
            await _context.SchoolFees
                .Include(f => f.Student)
                .Where(f => !f.IsPaid && f.DueDate < DateTime.UtcNow)
                .OrderBy(f => f.DueDate)
                .ToListAsync();
    }

    /// <summary>
    /// Email queue repository implementation
    /// </summary>
    public class EmailQueueRepository : Repository<EmailQueue>, IEmailQueueRepository
    {
        public EmailQueueRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<EmailQueue>> GetPendingEmailsAsync() =>
            await _context.EmailQueues
                .Where(e => !e.IsSent && e.RetryCount < 3)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();

        public async Task MarkAsSentAsync(long emailId)
        {
            var email = await _context.EmailQueues.FindAsync(emailId);
            if (email != null)
            {
                email.IsSent = true;
                email.SentAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Unit of work implementation
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IBlogRepository Blogs { get; }
        public IExamRepository Exams { get; }
        public IAttendanceRepository Attendance { get; }
        public IHomeworkRepository Homework { get; }
        public ISchoolFeeRepository SchoolFees { get; }
        public IEmailQueueRepository EmailQueue { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Blogs = new BlogRepository(context);
            Exams = new ExamRepository(context);
            Attendance = new AttendanceRepository(context);
            Homework = new HomeworkRepository(context);
            SchoolFees = new SchoolFeeRepository(context);
            EmailQueue = new EmailQueueRepository(context);
        }

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
