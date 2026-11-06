using System.ComponentModel;

namespace ioschools.Data.User
{
    /// <summary>
    /// Bit-flag enumeration of data-quality issues that can be flagged against a user account.
    /// Migrated from legacy ASP.NET MVC UserIssue enum — unnecessary using directives removed.
    /// NOTE: The HTML anchor in TEACHER_NOSUBJECT description is preserved for backward
    /// compatibility with legacy views; update to a Tag Helper or Razor partial in new views.
    /// </summary>
    [Flags]
    public enum UserIssue : int
    {
        NONE                    = 0x0000000,

        /// <summary>Parent/guardian account has no linked child.</summary>
        [Description("Parent/guardian has no child")]
        GUARDIAN_NOCHILD        = 0x00000001,

        /// <summary>User is inactive but has no leaving date recorded.</summary>
        [Description("User inactive but has no leaving date")]
        USER_NOLEAVINGDATE      = 0x00000002,

        /// <summary>User is active but has no admission/start date recorded.</summary>
        [Description("User active but has no admission/start date")]
        USER_NOSTARTDATE        = 0x00000004,

        /// <summary>Teacher has no subject assigned.</summary>
        [Description("Teacher has no subject assigned. <a href='/admin'>Assign subject teachers here</a>")]
        TEACHER_NOSUBJECT       = 0x00000008,

        /// <summary>Staff member has no employment information recorded.</summary>
        [Description("Staff has no employment info")]
        STAFF_NOEMPLOYMENT      = 0x00000010,

        /// <summary>Student has not been assigned to a class.</summary>
        [Description("Student has not being assigned a class")]
        STUDENT_NOCLASS         = 0x00000020,

        /// <summary>Student has no enrolment information recorded.</summary>
        [Description("Student has no enrolment info")]
        STUDENT_NOENROLMENT     = 0x00000040,

        /// <summary>Student has more than one father or mother linked.</summary>
        [Description("Student has more than 1 father/mother")]
        STUDENT_MANYPARENTS     = 0x00000080,

        /// <summary>Student has no date of birth recorded.</summary>
        [Description("Student has no birthday")]
        STUDENT_NOBIRTHDAY      = 0x00000100
    }
}
