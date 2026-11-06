using ioschools.DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ioschools.DB.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for the ioschools database.
    /// Replaces the legacy LINQ-to-SQL <c>ioschoolsDBDataContext</c>.
    /// </summary>
    public class IoschoolsDbContext : DbContext
    {
        public IoschoolsDbContext(DbContextOptions<IoschoolsDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Attendance> attendances { get; set; } = null!;
        public DbSet<AttendanceTerm> attendance_terms { get; set; } = null!;
        public DbSet<Blog> blogs { get; set; } = null!;
        public DbSet<BlogFile> blog_files { get; set; } = null!;
        public DbSet<BlogImage> blog_images { get; set; } = null!;
        public DbSet<Calendar> calendars { get; set; } = null!;
        public DbSet<Changelog> changelogs { get; set; } = null!;
        public DbSet<ClassesStudentsAllocated> classes_students_allocateds { get; set; } = null!;
        public DbSet<ClassesTeachersAllocated> classes_teachers_allocateds { get; set; } = null!;
        public DbSet<Conduct> conducts { get; set; } = null!;
        public DbSet<Eca> ecas { get; set; } = null!;
        public DbSet<EcaStudent> eca_students { get; set; } = null!;
        public DbSet<Employment> employments { get; set; } = null!;
        public DbSet<Exam> exams { get; set; } = null!;
        public DbSet<ExamClass> exam_classes { get; set; } = null!;
        public DbSet<ExamMark> exam_marks { get; set; } = null!;
        public DbSet<ExamSubject> exam_subjects { get; set; } = null!;
        public DbSet<ExamTemplate> exam_templates { get; set; } = null!;
        public DbSet<ExamTemplateSubject> exam_template_subjects { get; set; } = null!;
        public DbSet<Fee> fees { get; set; } = null!;
        public DbSet<FeesReminder> fees_reminders { get; set; } = null!;
        public DbSet<FeesTemplate> fees_templates { get; set; } = null!;
        public DbSet<FeesType> fees_types { get; set; } = null!;
        public DbSet<GradesMethod> grades_methods { get; set; } = null!;
        public DbSet<GradesRule> grades_rules { get; set; } = null!;
        public DbSet<Homework> homeworks { get; set; } = null!;
        public DbSet<HomeworkAnswer> homework_answers { get; set; } = null!;
        public DbSet<HomeworkClass> homework_classes { get; set; } = null!;
        public DbSet<HomeworkFile> homework_files { get; set; } = null!;
        public DbSet<HomeworkStudent> homework_students { get; set; } = null!;
        public DbSet<Leave> leaves { get; set; } = null!;
        public DbSet<LeavesAllocated> leaves_allocateds { get; set; } = null!;
        public DbSet<LeavesTaken> leaves_takens { get; set; } = null!;
        public DbSet<Mail> mails { get; set; } = null!;
        public DbSet<Page> pages { get; set; } = null!;
        public DbSet<Registration> registrations { get; set; } = null!;
        public DbSet<RegistrationNotification> registration_notifications { get; set; } = null!;
        public DbSet<School> schools { get; set; } = null!;
        public DbSet<SchoolClass> school_classes { get; set; } = null!;
        public DbSet<SchoolTerm> school_terms { get; set; } = null!;
        public DbSet<SchoolYear> school_years { get; set; } = null!;
        public DbSet<Sibling> siblings { get; set; } = null!;
        public DbSet<StudentsDiscipline> students_disciplines { get; set; } = null!;
        public DbSet<StudentsGuardian> students_guardians { get; set; } = null!;
        public DbSet<StudentsRemark> students_remarks { get; set; } = null!;
        public DbSet<Subject> subjects { get; set; } = null!;
        public DbSet<SubjectTeacher> subject_teachers { get; set; } = null!;
        public DbSet<User> users { get; set; } = null!;
        public DbSet<UserFile> user_files { get; set; } = null!;
        public DbSet<UserImage> user_images { get; set; } = null!;
        public DbSet<UserParent> user_parents { get; set; } = null!;
        public DbSet<UserStaff> user_staffs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User self-referencing relationships for siblings
            modelBuilder.Entity<Sibling>()
                .HasOne(s => s.user)
                .WithMany(u => u.siblings)
                .HasForeignKey(s => s.siblingid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sibling>()
                .HasOne(s => s.user1)
                .WithMany(u => u.siblings1)
                .HasForeignKey(s => s.otherid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User self-referencing relationships for guardian-student
            modelBuilder.Entity<StudentsGuardian>()
                .HasOne(sg => sg.user)
                .WithMany(u => u.students_guardians)
                .HasForeignKey(sg => sg.studentid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentsGuardian>()
                .HasOne(sg => sg.user1)
                .WithMany(u => u.students_guardians1)
                .HasForeignKey(sg => sg.parentid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StudentsDiscipline self-referencing relationships
            modelBuilder.Entity<StudentsDiscipline>()
                .HasOne(sd => sd.user)
                .WithMany(u => u.students_disciplines)
                .HasForeignKey(sd => sd.studentid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentsDiscipline>()
                .HasOne(sd => sd.user1)
                .WithMany(u => u.students_disciplines1)
                .HasForeignKey(sd => sd.teacherid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Registration multi-FK relationships
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.user)
                .WithMany(u => u.registrations)
                .HasForeignKey(r => r.studentid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User → School relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.school)
                .WithMany(s => s.users)
                .HasForeignKey(u => u.schoolid)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure UserImage one-to-one
            modelBuilder.Entity<UserImage>()
                .HasOne(ui => ui.user)
                .WithOne(u => u.user_image)
                .HasForeignKey<UserImage>(ui => ui.userid)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserParent one-to-one
            modelBuilder.Entity<UserParent>()
                .HasOne(up => up.user)
                .WithOne(u => u.user_parents)
                .HasForeignKey<UserParent>(up => up.userid)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserStaff one-to-one
            modelBuilder.Entity<UserStaff>()
                .HasOne(us => us.user)
                .WithOne(u => u.user_staffs)
                .HasForeignKey<UserStaff>(us => us.userid)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Exam → User (creator) relationship
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.user)
                .WithMany(u => u.exams)
                .HasForeignKey(e => e.creator)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Blog → User (creator) relationship
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.user)
                .WithMany(u => u.blogs)
                .HasForeignKey(b => b.creator)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Homework → User (creator) relationship
            modelBuilder.Entity<Homework>()
                .HasOne(h => h.user)
                .WithMany(u => u.homeworks)
                .HasForeignKey(h => h.creator)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Changelog → User relationship
            modelBuilder.Entity<Changelog>()
                .HasOne(c => c.user)
                .WithMany(u => u.changelogs)
                .HasForeignKey(c => c.userid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ExamTemplate → User (creator) relationship
            modelBuilder.Entity<ExamTemplate>()
                .HasOne(et => et.user)
                .WithMany()
                .HasForeignKey(et => et.creator)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure ClassesTeachersAllocated → User (teacher) relationship
            modelBuilder.Entity<ClassesTeachersAllocated>()
                .HasOne(cta => cta.user)
                .WithMany(u => u.classes_teachers_allocateds)
                .HasForeignKey(cta => cta.teacherid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure SubjectTeacher → User (teacher) relationship
            modelBuilder.Entity<SubjectTeacher>()
                .HasOne(st => st.user)
                .WithMany(u => u.subject_teachers)
                .HasForeignKey(st => st.teacherid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ExamMark → User (student) relationship
            modelBuilder.Entity<ExamMark>()
                .HasOne(em => em.user)
                .WithMany(u => u.exam_marks)
                .HasForeignKey(em => em.studentid)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Attendance → User (student) relationship
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.user)
                .WithMany(u => u.attendances)
                .HasForeignKey(a => a.studentid)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
