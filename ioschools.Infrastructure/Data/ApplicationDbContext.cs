using ioschools.Domain.Entities;
using ioschools.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ioschools.Infrastructure.Data
{
    /// <summary>
    /// Application DbContext - replaces legacy ioschoolsDBDataContext (LINQ-to-SQL)
    /// Migrated from: ioschools.DB.ioschoolsDBDataContext
    /// Migration approach: Code-First with EF Core 8
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets - replaces legacy LINQ-to-SQL Table<T> properties
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserImage> UserImages { get; set; } = null!;
        public DbSet<UserChild> UserChildren { get; set; } = null!;
        public DbSet<UserParent> UserParents { get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<CalendarEvent> CalendarEvents { get; set; } = null!;
        public DbSet<SchoolClass> SchoolClasses { get; set; } = null!;
        public DbSet<ClassStudentAllocation> ClassStudentAllocations { get; set; } = null!;
        public DbSet<ClassTeacherAllocation> ClassTeacherAllocations { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<SubjectTeacher> SubjectTeachers { get; set; } = null!;
        public DbSet<Exam> Exams { get; set; } = null!;
        public DbSet<ExamMark> ExamMarks { get; set; } = null!;
        public DbSet<ExamTemplate> ExamTemplates { get; set; } = null!;
        public DbSet<ExamTemplateSubject> ExamTemplateSubjects { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Discipline> Disciplines { get; set; } = null!;
        public DbSet<Conduct> Conducts { get; set; } = null!;
        public DbSet<Homework> Homeworks { get; set; } = null!;
        public DbSet<HomeworkFile> HomeworkFiles { get; set; } = null!;
        public DbSet<HomeworkAnswer> HomeworkAnswers { get; set; } = null!;
        public DbSet<HomeworkAnswerFile> HomeworkAnswerFiles { get; set; } = null!;
        public DbSet<SchoolFee> SchoolFees { get; set; } = null!;
        public DbSet<StaffLeave> StaffLeaves { get; set; } = null!;
        public DbSet<ECA> ECAs { get; set; } = null!;
        public DbSet<ECAStudent> ECAStudents { get; set; } = null!;
        public DbSet<Enrolment> Enrolments { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<EmailQueue> EmailQueues { get; set; } = null!;
        public DbSet<GradingRule> GradingRules { get; set; } = null!;
        public DbSet<SchoolTerm> SchoolTerms { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<Vacancy> Vacancies { get; set; } = null!;
        public DbSet<ChangeLog> ChangeLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(512);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MiddleName).HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.UserGroup).HasConversion<int>();
                entity.Property(e => e.Permissions).HasConversion<long>();
            });

            // UserImage configuration
            modelBuilder.Entity<UserImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.HasOne(e => e.User)
                    .WithOne(u => u.UserImage)
                    .HasForeignKey<UserImage>(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Blog configuration
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CalendarEvent configuration
            modelBuilder.Entity<CalendarEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            });

            // SchoolClass configuration
            modelBuilder.Entity<SchoolClass>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Exam configuration
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ExamMark configuration
            modelBuilder.Entity<ExamMark>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Mark).HasPrecision(5, 2);
                entity.HasOne(e => e.Exam)
                    .WithMany(x => x.Marks)
                    .HasForeignKey(e => e.ExamId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attendance configuration
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Homework configuration
            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.HasOne(e => e.Teacher)
                    .WithMany()
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SchoolFee configuration
            modelBuilder.Entity<SchoolFee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(10, 2);
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EmailQueue configuration
            modelBuilder.Entity<EmailQueue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ToEmail).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            });

            // GradingRule configuration
            modelBuilder.Entity<GradingRule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Grade).IsRequired().HasMaxLength(10);
                entity.Property(e => e.MinMark).HasPrecision(5, 2);
                entity.Property(e => e.MaxMark).HasPrecision(5, 2);
            });

            // Enrolment configuration
            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.IdType).HasConversion<int>();
            });

            // UserChild configuration
            modelBuilder.Entity<UserChild>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Parent)
                    .WithMany(u => u.Children)
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Child)
                    .WithMany()
                    .HasForeignKey(e => e.ChildId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserParent configuration
            modelBuilder.Entity<UserParent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Student)
                    .WithMany(u => u.Parents)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Parent)
                    .WithMany()
                    .HasForeignKey(e => e.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StaffLeave configuration
            modelBuilder.Entity<StaffLeave>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LeaveType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasOne(e => e.Staff)
                    .WithMany()
                    .HasForeignKey(e => e.StaffId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ECA configuration
            modelBuilder.Entity<ECA>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            // Discipline configuration
            modelBuilder.Entity<Discipline>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
