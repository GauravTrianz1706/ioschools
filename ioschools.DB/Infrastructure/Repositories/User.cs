using System.Globalization;
using BCrypt.Net;
using ioschools.Data;
using ioschools.Data.Attendance;
using ioschools.Data.User;
using ioschools.DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - User operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public User? GetActiveUserLogin(string email, string password)
        {
            var usr = _db.users.SingleOrDefault(x => x.email == email && (x.settings & (int)UserSettings.INACTIVE) == 0);
            if (usr != null && !string.IsNullOrEmpty(usr.passwordhash) && BCrypt.Net.BCrypt.Verify(email + password, usr.passwordhash))
            {
                return usr;
            }
            return null;
        }

        /// <inheritdoc/>
        public User? GetUser(long id)
        {
            return _db.users.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IQueryable<User> GetUsers()
        {
            return _db.users;
        }

        /// <inheritdoc/>
        public IEnumerable<User> GetActiveUsers()
        {
            return _db.users.Where(x => (x.settings & (int)UserSettings.INACTIVE) == 0);
        }

        /// <inheritdoc/>
        public long GetUserDiskUsage(long sessionid)
        {
            var files = _db.homeworks
                .Where(x => x.creator == sessionid)
                .SelectMany(x => x.homework_files);

            if (!files.Any())
            {
                return 0;
            }
            return files.Sum(y => y.size);
        }

        /// <inheritdoc/>
        public void AddUser(User u)
        {
            _db.users.Add(u);
            // Do not save - many other operations depend on not saving immediately
        }

        /// <inheritdoc/>
        public void DeleteStudentOrGuardian(long id)
        {
            var exist = _db.students_guardians.SingleOrDefault(x => x.id == id);
            if (exist != null)
            {
                _db.students_guardians.Remove(exist);
                Save();
            }
        }

        /// <inheritdoc/>
        public Employment? GetEmploymentPeriod(long id)
        {
            return _db.employments.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IQueryable<User> GetUsers(long? viewerid, UserAuth viewer_auth, int? school, int? schoolClass,
            UserGroup? group, string? discipline, AttendanceStatus? attendanceStatus, string? attendanceDate,
            int year, int? eca, bool active = true, bool hasIssues = false)
        {
            var usrs = _db.users.AsQueryable();

            if (group.HasValue)
            {
                usrs = usrs.Where(x => (x.usergroup & (int)group.Value) != 0);
            }

            if (active)
            {
                usrs = usrs.Where(x => (x.settings & (int)UserSettings.INACTIVE) == 0);
            }
            else
            {
                usrs = usrs.Where(x => (x.settings & (int)UserSettings.INACTIVE) != 0);
            }

            if (schoolClass.HasValue)
            {
                var teachers = usrs
                    .SelectMany(x => x.classes_teachers_allocateds)
                    .Where(y => y.classid == schoolClass.Value && y.year == year)
                    .Select(x => x.user!);
                var students = usrs
                    .SelectMany(x => x.classes_students_allocateds)
                    .Where(y => y.classid == schoolClass.Value && y.year == year)
                    .Select(x => x.user!);
                usrs = teachers.Union(students).Distinct();
            }
            else if (school.HasValue)
            {
                usrs = usrs.Where(x => x.schoolid.HasValue && x.schoolid.Value == school);
            }
            else
            {
                var teachers = usrs
                    .SelectMany(x => x.classes_teachers_allocateds)
                    .Where(y => y.year == year)
                    .Select(x => x.user!);
                var students = usrs
                    .SelectMany(x => x.classes_students_allocateds)
                    .Where(y => y.year == year)
                    .Select(x => x.user!);
                var others = usrs.Where(x => (x.usergroup & (int)(UserGroup.STUDENT | UserGroup.TEACHER)) == 0);
                usrs = teachers.Union(students).Union(others).Distinct();
            }

            if (eca.HasValue)
            {
                usrs = usrs.Where(x =>
                    x.usergroup == (int)UserGroup.STUDENT &&
                    x.eca_students.Count(y => y.ecaid == eca.Value && y.year == year) != 0);
            }

            if (!string.IsNullOrEmpty(discipline))
            {
                try
                {
                    var sorter = new Sorter(discipline);
                    if (sorter.type == SorterType.LESSTHAN)
                    {
                        usrs = usrs.Where(x =>
                            x.students_disciplines.Where(w => w.created.Year == year).Sum(y => y.points) <= sorter.value &&
                            x.students_disciplines.Where(w => w.created.Year == year).Sum(y => y.points) != 0);
                    }
                    else
                    {
                        usrs = usrs.Where(x =>
                            x.students_disciplines.Where(w => w.created.Year == year).Sum(y => y.points) >= sorter.value &&
                            x.students_disciplines.Where(w => w.created.Year == year).Sum(y => y.points) != 0);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing discipline sorter for viewerid: {ViewerId}", viewerid);
                }
            }

            if (!string.IsNullOrEmpty(attendanceDate) && attendanceStatus.HasValue)
            {
                if (DateTime.TryParseExact(attendanceDate, Constants.DATEFORMAT_DATEPICKER,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    usrs = usrs
                        .SelectMany(x => x.attendances)
                        .Where(x => x.date == date && x.status == attendanceStatus.Value.ToString())
                        .Select(x => x.user!);
                }
            }

            // Permission-based filtering
            if (viewer_auth != null && viewerid.HasValue)
            {
                if (UserSuperGroups.STAFF.HasFlag(viewer_auth.group))
                {
                    var student = Enumerable.Empty<User>().AsQueryable();
                    var parents = Enumerable.Empty<User>().AsQueryable();
                    var staff = Enumerable.Empty<User>().AsQueryable();

                    if ((viewer_auth.perms & (Permission.USERS_VIEW_STUDENTS | Permission.USERS_EDIT_STUDENTS)) != 0)
                    {
                        student = usrs.Where(x => x.usergroup == (int)UserGroup.STUDENT);
                    }

                    if ((viewer_auth.perms & (Permission.USERS_VIEW_PARENTS | Permission.USERS_EDIT_PARENTS)) != 0)
                    {
                        parents = usrs.Where(x => x.usergroup == (int)UserGroup.GUARDIAN);
                    }

                    if ((viewer_auth.perms & (Permission.USERS_VIEW_STAFF | Permission.USERS_EDIT_STAFF)) != 0)
                    {
                        staff = usrs.Where(x => (x.usergroup & (int)UserSuperGroups.STAFF) != 0);
                    }

                    usrs = student.Union(parents).Union(staff).AsQueryable();
                }
                else
                {
                    usrs = Enumerable.Empty<User>().AsQueryable();
                }
            }

            // Issue checking is intensive - do it last
            if (hasIssues)
            {
                usrs = usrs.ToArray().Where(x => x.GetIssues(year, true) != UserIssue.NONE).AsQueryable();
            }

            return usrs;
        }

        /// <inheritdoc/>
        public void DeleteUser(long id, long executer)
        {
            var usr = _db.users
                .Include(u => u.blogs)
                .Include(u => u.students_disciplines)
                .Include(u => u.classes_students_allocateds)
                .Include(u => u.students_guardians)
                .Include(u => u.students_guardians1)
                .Include(u => u.attendances)
                .Include(u => u.eca_students)
                .Include(u => u.exam_marks)
                .Include(u => u.siblings)
                .Include(u => u.siblings1)
                .Include(u => u.registrations)
                .Include(u => u.user_files)
                .Include(u => u.students_remarks)
                .Include(u => u.user_parents)
                .Include(u => u.user_staffs)
                .Include(u => u.changelogs)
                .Include(u => u.registrations1)
                .Include(u => u.registrations2)
                .Include(u => u.classes_teachers_allocateds)
                .Include(u => u.employments)
                .Include(u => u.homeworks)
                    .ThenInclude(h => h.homework_files)
                .Include(u => u.homeworks)
                    .ThenInclude(h => h.homework_classes)
                .Include(u => u.exams)
                    .ThenInclude(e => e.exam_classes)
                .Include(u => u.exams)
                    .ThenInclude(e => e.exam_subjects)
                .Include(u => u.exams)
                    .ThenInclude(e => e.exam_marks)
                .SingleOrDefault(x => x.id == id);

            if (usr == null)
            {
                return;
            }

            _db.blogs.RemoveRange(usr.blogs);

            if (usr.usergroup == (int)UserGroup.STUDENT)
            {
                _db.students_disciplines.RemoveRange(usr.students_disciplines);

                var allocatedClasses = _db.classes_students_allocateds.Where(x => x.studentid == usr.id);
                _db.classes_students_allocateds.RemoveRange(allocatedClasses);

                _db.students_guardians.RemoveRange(usr.students_guardians);
                _db.students_guardians.RemoveRange(usr.students_guardians1);

                var attendances = _db.attendances.Where(x => x.studentid == usr.id);
                _db.attendances.RemoveRange(attendances);

                _db.eca_students.RemoveRange(usr.eca_students);
                _db.exam_marks.RemoveRange(usr.exam_marks);
                _db.siblings.RemoveRange(usr.siblings);
                _db.siblings.RemoveRange(usr.siblings1);
                _db.registrations.RemoveRange(usr.registrations);
                _db.user_files.RemoveRange(usr.user_files);
                _db.students_remarks.RemoveRange(usr.students_remarks);
            }
            else
            {
                _db.students_disciplines.RemoveRange(usr.students_disciplines1);

                if (usr.user_parents != null)
                {
                    _db.user_parents.Remove(usr.user_parents);
                }

                _db.students_guardians.RemoveRange(usr.students_guardians1);
                _db.students_guardians.RemoveRange(usr.students_guardians);

                if (usr.user_staffs != null)
                {
                    _db.user_staffs.Remove(usr.user_staffs);
                }

                _db.changelogs.RemoveRange(usr.changelogs);
                _db.registrations.RemoveRange(usr.registrations1.Union(usr.registrations2));

                var allocatedTeachers = _db.classes_teachers_allocateds.Where(x => x.teacherid == usr.id);
                _db.classes_teachers_allocateds.RemoveRange(allocatedTeachers);

                if (usr.employments != null)
                {
                    _db.employments.RemoveRange(usr.employments);
                }

                var homeworkFiles = usr.homeworks.SelectMany(x => x.homework_files);
                var homeworkClasses = usr.homeworks.SelectMany(x => x.homework_classes);

                _db.homework_classes.RemoveRange(homeworkClasses);
                _db.homework_files.RemoveRange(homeworkFiles);
                _db.homeworks.RemoveRange(usr.homeworks);

                _db.exam_classes.RemoveRange(usr.exams.SelectMany(x => x.exam_classes));
                _db.exam_subjects.RemoveRange(usr.exams.SelectMany(x => x.exam_subjects));
                _db.exam_marks.RemoveRange(usr.exams.SelectMany(x => x.exam_marks));
                _db.exams.RemoveRange(usr.exams);
            }

            // Log the deletion
            var change = new Changelog
            {
                changes = $"{usr.name} deleted",
                created = DateTime.Now,
                userid = executer
            };

            _db.changelogs.Add(change);
            _db.users.Remove(usr);
            _db.SaveChanges();
        }

        /// <inheritdoc/>
        public User? GetUserByHash(string hash)
        {
            return _db.users.SingleOrDefault(x => x.passwordhash == hash);
        }

        /// <inheritdoc/>
        public User? GetUserByNewNRIC(string nric)
        {
            nric = nric.Replace("-", "");
            if (string.IsNullOrEmpty(nric))
            {
                return null;
            }
            return _db.users.SingleOrDefault(x => x.nric_new == nric);
        }

        /// <inheritdoc/>
        public bool IsStudentInMyClass(long myid, long studentid, int year)
        {
            var classesITeach = _db.classes_teachers_allocateds
                .Where(x => x.teacherid == myid && x.year == year)
                .Select(x => x.classid)
                .Distinct()
                .ToArray();

            var count = _db.classes_students_allocateds
                .Count(x => x.studentid == studentid && classesITeach.Contains(x.classid) && x.year == year);

            return count > 0;
        }
    }
}
