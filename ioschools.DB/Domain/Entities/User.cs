using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ioschools.Data;
using ioschools.Data.User;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a user (student, teacher, guardian, admin, etc.) in the system.
    /// </summary>
    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? email { get; set; }

        public string? passwordhash { get; set; }

        public int usergroup { get; set; }

        public int settings { get; set; }

        public int? schoolid { get; set; }

        [MaxLength(50)]
        public string? designation { get; set; }

        [MaxLength(100)]
        public string? nric_new { get; set; }

        public DateTime? dob { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        public virtual UserImage? user_image { get; set; }

        public virtual UserParent? user_parents { get; set; }

        public virtual UserStaff? user_staffs { get; set; }

        public virtual ICollection<Blog> blogs { get; set; } = new List<Blog>();

        public virtual ICollection<Changelog> changelogs { get; set; } = new List<Changelog>();

        public virtual ICollection<ClassesStudentsAllocated> classes_students_allocateds { get; set; } = new List<ClassesStudentsAllocated>();

        public virtual ICollection<ClassesTeachersAllocated> classes_teachers_allocateds { get; set; } = new List<ClassesTeachersAllocated>();

        public virtual ICollection<EcaStudent> eca_students { get; set; } = new List<EcaStudent>();

        public virtual ICollection<Employment> employments { get; set; } = new List<Employment>();

        public virtual ICollection<ExamMark> exam_marks { get; set; } = new List<ExamMark>();

        public virtual ICollection<Exam> exams { get; set; } = new List<Exam>();

        public virtual ICollection<Homework> homeworks { get; set; } = new List<Homework>();

        public virtual ICollection<Registration> registrations { get; set; } = new List<Registration>();

        // registrations where user is reviewer
        public virtual ICollection<Registration> registrations1 { get; set; } = new List<Registration>();

        // registrations where user is applicant
        public virtual ICollection<Registration> registrations2 { get; set; } = new List<Registration>();

        public virtual ICollection<Sibling> siblings { get; set; } = new List<Sibling>();

        public virtual ICollection<Sibling> siblings1 { get; set; } = new List<Sibling>();

        public virtual ICollection<StudentsDiscipline> students_disciplines { get; set; } = new List<StudentsDiscipline>();

        // discipline records where user is the teacher
        public virtual ICollection<StudentsDiscipline> students_disciplines1 { get; set; } = new List<StudentsDiscipline>();

        public virtual ICollection<StudentsGuardian> students_guardians { get; set; } = new List<StudentsGuardian>();

        public virtual ICollection<StudentsGuardian> students_guardians1 { get; set; } = new List<StudentsGuardian>();

        public virtual ICollection<StudentsRemark> students_remarks { get; set; } = new List<StudentsRemark>();

        public virtual ICollection<SubjectTeacher> subject_teachers { get; set; } = new List<SubjectTeacher>();

        public virtual ICollection<UserFile> user_files { get; set; } = new List<UserFile>();

        public virtual ICollection<Attendance> attendances { get; set; } = new List<Attendance>();

        /// <summary>
        /// Determines whether the viewer can edit this user record.
        /// </summary>
        public bool GetCanEdit(long viewerid, UserAuth auth)
        {
            var canedit = false;
            var rowGroup = (UserGroup)usergroup;

            if (UserSuperGroups.STAFF.HasFlag(auth.group))
            {
                if ((rowGroup == UserGroup.STUDENT && auth.perms.HasFlag(Permission.USERS_EDIT_STUDENTS)) ||
                    (rowGroup == UserGroup.GUARDIAN && auth.perms.HasFlag(Permission.USERS_EDIT_PARENTS)) ||
                    (UserSuperGroups.STAFF.HasFlag(rowGroup) && auth.perms.HasFlag(Permission.USERS_EDIT_STAFF)) ||
                    (id == viewerid && auth.perms.HasFlag(Permission.USERS_EDIT_OWN)))
                {
                    canedit = true;
                }
            }

            return canedit;
        }

        /// <summary>
        /// Determines whether the viewer can view this user record.
        /// </summary>
        public bool GetCanView(long viewerid, UserAuth auth)
        {
            var rowGroup = (UserGroup)usergroup;
            var canview = false;

            if (UserSuperGroups.STAFF.HasFlag(auth.group))
            {
                if ((rowGroup == UserGroup.STUDENT && auth.perms.HasFlag(Permission.USERS_VIEW_STUDENTS)) ||
                    (rowGroup == UserGroup.GUARDIAN && auth.perms.HasFlag(Permission.USERS_VIEW_PARENTS)) ||
                    (UserSuperGroups.STAFF.HasFlag(rowGroup) && auth.perms.HasFlag(Permission.USERS_VIEW_STAFF)) ||
                    (id == viewerid))
                {
                    canview = true;
                }
            }
            else
            {
                switch (auth.group)
                {
                    case UserGroup.STUDENT:
                        if (id == viewerid)
                        {
                            canview = true;
                        }
                        break;
                    case UserGroup.GUARDIAN:
                        if (rowGroup == UserGroup.STUDENT)
                        {
                            if (students_guardians.Count(x => x.parentid == viewerid) != 0)
                            {
                                canview = true;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return canview;
        }

        /// <summary>
        /// Detects issues with this user record for the given academic year.
        /// </summary>
        public UserIssue GetIssues(int year, bool returnOnFirstIssueDetected = true)
        {
            var issue = UserIssue.NONE;
            var _ugroup = (UserGroup)usergroup;
            var isActive = !((UserSettings)settings).HasFlag(UserSettings.INACTIVE);

            if (UserSuperGroups.STAFF.HasFlag(_ugroup))
            {
                if (!employments.Any())
                {
                    issue |= UserIssue.STAFF_NOEMPLOYMENT;
                    if (returnOnFirstIssueDetected) return issue;
                }
                else
                {
                    if (isActive && !employments.Any(x => x.start_date.HasValue))
                    {
                        issue |= UserIssue.USER_NOSTARTDATE;
                        if (returnOnFirstIssueDetected) return issue;
                    }

                    if (!isActive)
                    {
                        var activeRow = employments.Where(x => x.start_date.HasValue)
                                            .OrderBy(x => x.start_date).LastOrDefault();
                        if (activeRow != null && !activeRow.end_date.HasValue && DateTime.Now > activeRow.start_date!.Value)
                        {
                            issue |= UserIssue.USER_NOLEAVINGDATE;
                            if (returnOnFirstIssueDetected) return issue;
                        }
                    }
                }
            }

            switch (_ugroup)
            {
                case UserGroup.NONE:
                case UserGroup.DIRECTOR:
                case UserGroup.HEAD:
                case UserGroup.ADMIN:
                case UserGroup.FINANCE:
                case UserGroup.CLERK:
                case UserGroup.SUPPORT:
                    break;
                case UserGroup.TEACHER:
                    if (isActive && !subject_teachers.Any(x => x.year == year))
                    {
                        if (schoolid.HasValue && schoolid.Value == (int)Schools.Kindergarten)
                        {
                            // ignore for kindergarten teachers
                        }
                        else
                        {
                            issue |= UserIssue.TEACHER_NOSUBJECT;
                            if (returnOnFirstIssueDetected) return issue;
                        }
                    }
                    break;
                case UserGroup.STUDENT:
                    if (isActive && !classes_students_allocateds.Any(x => x.year == year))
                    {
                        issue |= UserIssue.STUDENT_NOCLASS;
                        if (returnOnFirstIssueDetected) return issue;
                    }

                    if (!dob.HasValue)
                    {
                        issue |= UserIssue.STUDENT_NOBIRTHDAY;
                        if (returnOnFirstIssueDetected) return issue;
                    }

                    var registration = registrations.LastOrDefault();
                    if (registration == null)
                    {
                        issue |= UserIssue.STUDENT_NOENROLMENT;
                        if (returnOnFirstIssueDetected) return issue;
                    }
                    else
                    {
                        if (registration.status == RegistrationStatus.ACCEPTED.ToString())
                        {
                            if (isActive && !registrations.Any(x => x.admissionDate.HasValue))
                            {
                                issue |= UserIssue.USER_NOSTARTDATE;
                                if (returnOnFirstIssueDetected) return issue;
                            }

                            if (!isActive)
                            {
                                var activeRow = registrations.Where(x => x.admissionDate.HasValue)
                                                    .OrderBy(x => x.admissionDate).LastOrDefault();
                                if (activeRow != null && !activeRow.leftDate.HasValue && DateTime.Now > activeRow.admissionDate!.Value)
                                {
                                    issue |= UserIssue.USER_NOLEAVINGDATE;
                                    if (returnOnFirstIssueDetected) return issue;
                                }
                            }
                        }
                    }

                    if (students_guardians.Count(y => y.type.HasValue && y.type.Value == (byte)GuardianType.FATHER) > 1 ||
                        students_guardians.Count(y => y.type.HasValue && y.type.Value == (byte)GuardianType.MOTHER) > 1)
                    {
                        issue |= UserIssue.STUDENT_MANYPARENTS;
                        if (returnOnFirstIssueDetected) return issue;
                    }
                    break;
                case UserGroup.GUARDIAN:
                    if (!students_guardians1.Any())
                    {
                        issue |= UserIssue.GUARDIAN_NOCHILD;
                        if (returnOnFirstIssueDetected) return issue;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return issue;
        }

        /// <summary>
        /// Returns the final year of the user (based on last enrolment left date).
        /// </summary>
        [NotMapped]
        public int? FinalYear
        {
            get
            {
                var latestEnrolled = registrations.OrderByDescending(x => x.id).FirstOrDefault();
                if (latestEnrolled != null && latestEnrolled.leftDate.HasValue)
                {
                    return latestEnrolled.leftDate.Value.Year;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the school ID based on the user's current class or subject assignment.
        /// </summary>
        public int? GetNewSchoolID()
        {
            if (usergroup == (int)UserGroup.STUDENT)
            {
                var classname = classes_students_allocateds.FirstOrDefault(x => x.year == DateTime.Now.Year);
                if (classname != null)
                {
                    return classname.school_class?.school?.id;
                }
            }
            else if (usergroup == (int)UserGroup.TEACHER)
            {
                var subject = subject_teachers.OrderByDescending(x => x.year).FirstOrDefault();
                if (subject != null)
                {
                    return subject.school_class?.schoolid;
                }

                var classname = classes_teachers_allocateds.OrderByDescending(x => x.year).FirstOrDefault();
                if (classname != null)
                {
                    return classname.school?.id;
                }
            }
            return schoolid;
        }

        /// <summary>
        /// Returns the formatted display name of the user.
        /// </summary>
        public string ToName(bool includeDesignation = true)
        {
            if (string.IsNullOrEmpty(designation) || !includeDesignation)
            {
                return name;
            }
            return $"{designation} {name}";
        }
    }
}
