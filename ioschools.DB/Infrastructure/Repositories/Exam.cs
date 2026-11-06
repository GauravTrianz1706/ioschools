using ioschools.Data;
using ioschools.Data.User;
using ioschools.DB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Exam operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void AddExam(Exam exam)
        {
            _db.exams.Add(exam);
            Save();
        }

        /// <inheritdoc/>
        public void AddExamMark(ExamMark emark)
        {
            _db.exam_marks.Add(emark);
        }

        /// <inheritdoc/>
        public void AddExamTemplate(ExamTemplate template)
        {
            _db.exam_templates.Add(template);
        }

        /// <inheritdoc/>
        public string CanModifyExamMark(long modifier_userid, long exam_subjectid, long studentid, int year)
        {
            var subject = _db.exam_subjects.Single(x => x.id == exam_subjectid);

            // Allow non-mapped subjects to be modified by anyone
            if (!subject.subjectid.HasValue)
            {
                return string.Empty;
            }

            var studentClasses = _db.classes_students_allocateds
                .Where(x => x.studentid == studentid && x.year == year)
                .Select(x => x.classid)
                .ToArray();

            var deniedString = _db.subject_teachers
                .Include(x => x.subject)
                .Include(x => x.school_class)
                .Where(x => x.teacherid == modifier_userid &&
                            x.subjectid == subject.subjectid.Value &&
                            studentClasses.Contains(x.classid) &&
                            x.year == year)
                .Select(x => $"{x.subject!.name}({x.school_class!.name})")
                .FirstOrDefault();

            return deniedString ?? string.Empty;
        }

        /// <inheritdoc/>
        public void DeleteExamMark(ExamMark emark)
        {
            _db.exam_marks.Remove(emark);
        }

        /// <inheritdoc/>
        public void DeleteExamTemplate(int id)
        {
            // Intentionally left empty - matches legacy behavior
        }

        /// <inheritdoc/>
        public void DeleteExamTemplateSubject(int id)
        {
            var exist = _db.exam_template_subjects.Single(x => x.id == id);
            _db.exam_template_subjects.Remove(exist);
            Save();
        }

        /// <inheritdoc/>
        public Exam? GetExam(long id)
        {
            return _db.exams.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public ExamMark? GetExamMark(long examid, long studentid, long subjectid)
        {
            return _db.exam_marks.SingleOrDefault(x => x.examid == examid && x.studentid == studentid && x.exam_subjectid == subjectid);
        }

        /// <inheritdoc/>
        public ExamTemplate? GetExamTemplate(int id)
        {
            return _db.exam_templates.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IQueryable<ExamTemplate> GetExamTemplates(long viewerid)
        {
            return _db.exam_templates.Where(x => !x.isprivate || (x.isprivate && x.creator.HasValue && x.creator.Value == viewerid));
        }

        /// <inheritdoc/>
        public IQueryable<Exam> GetExams(int? school, int? form, int? year)
        {
            var exams = _db.exams.AsQueryable();

            if (school.HasValue)
            {
                exams = exams.Where(x => x.schoolid == school.Value);
            }

            if (form.HasValue)
            {
                exams = exams
                    .SelectMany(x => x.exam_classes)
                    .Where(y => y.classid == form.Value)
                    .Select(y => y.exam!)
                    .Distinct();
            }

            if (year.HasValue)
            {
                exams = exams.Where(x => x.year == year.Value);
            }

            return exams;
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetExamSubjects(long[] subjectids)
        {
            return _db.exam_subjects.Where(x => subjectids.Contains(x.id)).Select(x => x.name);
        }
    }
}
