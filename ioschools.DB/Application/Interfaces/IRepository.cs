using ioschools.Data;
using ioschools.Data.Attendance;
using ioschools.Data.User;
using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for the main data repository.
    /// Replaces the legacy LINQ-to-SQL based IRepository interface.
    /// </summary>
    public interface IRepository
    {
        /// <summary>Persists all pending changes to the database.</summary>
        void Save();

        // ─── Attendance ───────────────────────────────────────────────────────────

        void DeleteAttendance(Attendance att);
        Attendance? GetClassAttendance(long studentid, int classid, DateTime date);
        Attendance? GetEcaAttendance(long studentid, long ecaid, DateTime date);
        Attendance? GetAttendance(long id);
        AttendanceTerm? GetAttendanceTerm(int schoolid, int termid, int year);

        // ─── Blog / CMS ───────────────────────────────────────────────────────────

        void AddBlog(Blog blog);
        long AddBlogFile(BlogFile file);
        void DeleteBlog(long id);
        void DeleteBlogFile(long id);
        void DeleteBlogPhoto(long id);
        Blog? GetBlog(long id);
        BlogImage? GetBlogPhoto(long id);
        BlogFile? GetBlogFile(long id);
        IEnumerable<Blog> GetBlogs();
        IEnumerable<Blog> GetBlogs(BlogType? type, int? year);
        void UpdateBlogFiles(long id, string[] files);
        void UpdateBlogImages(long blogid, IEnumerable<string> photoids);

        // ─── Calendar ─────────────────────────────────────────────────────────────

        IQueryable<Calendar> GetCalendarEntries(int? year = null);

        // ─── Change Logs ──────────────────────────────────────────────────────────

        IQueryable<Changelog> GetChangeLogs();
        void AddChangeLog(Changelog change);

        // ─── Classes ──────────────────────────────────────────────────────────────

        void DeleteStudentAllocatedClass(long id);
        void DeleteTeacherAllocatedClass(long id);
        ClassesTeachersAllocated? GetAllocatedTeacherClass(long id);
        IQueryable<SchoolClass> GetSchoolClasses();
        IEnumerable<SchoolClass> GetClassesImTeachingThisSubject(long teacherid, long subjectid, int year);

        // ─── Discipline ───────────────────────────────────────────────────────────

        void DeleteDiscipline(StudentsDiscipline discipline);
        StudentsDiscipline? GetDiscipline(long id);
        IEnumerable<StudentsDiscipline> GetDisciplines();

        // ─── ECA ──────────────────────────────────────────────────────────────────

        void AddEca(Eca eca);
        void DeleteEca(long id);
        IEnumerable<Eca> GetEcas(int? schoolid);
        void DeleteStudentEca(long id);
        Eca? GetEca(long id);
        EcaStudent? GetStudentEca(long id);
        IEnumerable<Eca> GetViewableEcas(Schools? userschool);

        // ─── Employment ───────────────────────────────────────────────────────────

        Employment? GetEmploymentPeriod(long id);

        // ─── Enrolment / Registration ─────────────────────────────────────────────

        void AddRegistration(Registration registration);
        void DeleteRegistrationNotificationByUser(long userid);
        void DeleteRegistraion(Registration registration);
        Registration? GetRegistration(long id);
        IQueryable<Registration> GetRegistration(int? school, RegistrationStatus? status, int? year, int? classyear);
        IEnumerable<RegistrationNotification> GetRegistrationNotifications();

        // ─── Exam ─────────────────────────────────────────────────────────────────

        void AddExam(Exam exam);
        void AddExamMark(ExamMark emark);
        void AddExamTemplate(ExamTemplate template);
        string CanModifyExamMark(long modifier_userid, long subjectid, long studentid, int year);
        void DeleteExamMark(ExamMark emark);
        void DeleteExamTemplate(int id);
        void DeleteExamTemplateSubject(int id);
        Exam? GetExam(long id);
        ExamMark? GetExamMark(long examid, long studentid, long subjectid);
        ExamTemplate? GetExamTemplate(int id);
        IQueryable<ExamTemplate> GetExamTemplates(long viewerid);
        IQueryable<Exam> GetExams(int? school, int? form, int? year);

        // ─── Fees ─────────────────────────────────────────────────────────────────

        IQueryable<Fee> GetFees();

        // ─── Homework ─────────────────────────────────────────────────────────────

        void AddHomework(Homework homework);
        void AddHomeworkFile(HomeworkFile file);
        void DeleteHomework(long id);
        void DeleteHomeworkClasses(long homeworkid);
        void DeleteHomeworkFile(HomeworkFile file);
        Homework? GetHomework(long id);
        IEnumerable<Homework> GetHomeworks(long creator, int year);
        IEnumerable<Homework> GetHomeworksByStudent(long studentid, int year);
        HomeworkFile? GetHomeworkFile(long id);

        // ─── Image ────────────────────────────────────────────────────────────────

        long AddBlogImage(BlogImage img);
        long AddUserImage(UserImage image);
        void DeleteUserImage(long id);

        // ─── Mail ─────────────────────────────────────────────────────────────────

        IQueryable<Mail> GetMails();
        void DeleteMail(Mail entry);

        // ─── Schools ──────────────────────────────────────────────────────────────

        void AddSchoolSubject(Subject subject);
        IQueryable<School> GetSchools();
        IQueryable<School> GetSchools(Schools? myschool);
        IQueryable<SchoolYear> GetSchoolYears();
        IEnumerable<SchoolTerm> GetSchoolTerms();

        // ─── Subjects ─────────────────────────────────────────────────────────────

        void DeleteSchoolSubject(long id);
        Subject? GetSchoolSubject(long id);
        IQueryable<Subject> GetSchoolSubjects(int? schoolid);
        IEnumerable<Subject> GetSubjectsImTeaching(long teacherid, int year);
        IEnumerable<SubjectTeacher> GetSubjectTeachers(long teacherid, int year);

        // ─── User ─────────────────────────────────────────────────────────────────

        User? GetUser(long id);
        IQueryable<User> GetUsers();
        IEnumerable<User> GetActiveUsers();
        long GetUserDiskUsage(long sessionid);
        void AddUser(User u);
        void DeleteStudentOrGuardian(long id);
        IEnumerable<User> GetStudentsByGuardian(long guardianid);
        IQueryable<User> GetUsers(long? viewerid, UserAuth viewer_auth, int? school, int? schoolClass, UserGroup? group,
            string? discipline, AttendanceStatus? attendanceStatus, string? attendanceDate, int year, int? eca,
            bool active = true, bool hasIssues = false);
        IQueryable<User> GetStudentsByPhysicalClass(long classid, int year);
        void DeleteUser(long id, long executer);
        User? GetActiveUserLogin(string email, string password);
        User? GetUserByHash(string hash);
        User? GetUserByNewNRIC(string nric);
        bool IsStudentInMyClass(long myid, long studentid, int year);

        // ─── Time Table ───────────────────────────────────────────────────────────

        ClassesTeachersAllocated? GetClassPeriod(int year, int day, int schoolid, int classid, TimeSpan periodStart, TimeSpan periodEnd);
        IEnumerable<ClassesTeachersAllocated> GetStudentTimeTable(long userid, DayOfWeek day, int year);
        IEnumerable<ClassesTeachersAllocated> GetTeacherTimeTable(long teacherid, DayOfWeek day, int year);

        // ─── Miscellaneous ────────────────────────────────────────────────────────

        IEnumerable<string> GetExamSubjects(long[] subjectids);
        IEnumerable<int> GetOperationalYears();

        Page? GetPage(int typeid);
        void AddPage(Page page);
    }
}
