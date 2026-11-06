using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Homework operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public Homework? GetHomework(long id)
        {
            return _db.homeworks.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IEnumerable<Homework> GetHomeworks(long creator, int year)
        {
            return _db.homeworks.Where(x => x.creator == creator && x.created.Year == year);
        }

        /// <inheritdoc/>
        public HomeworkFile? GetHomeworkFile(long id)
        {
            return _db.homework_files.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IEnumerable<Homework> GetHomeworksByStudent(long studentid, int year)
        {
            var studentClass = _db.classes_students_allocateds
                .SingleOrDefault(x => x.studentid == studentid && x.year == year);

            if (studentClass != null)
            {
                return _db.homework_students
                    .Where(x => x.classid == studentClass.classid && x.studentid == studentid)
                    .Select(x => x.homework!)
                    .Where(h => h != null);
            }
            return Enumerable.Empty<Homework>();
        }

        /// <inheritdoc/>
        public void AddHomework(Homework homework)
        {
            _db.homeworks.Add(homework);
            Save();
        }

        /// <inheritdoc/>
        public void AddHomeworkFile(HomeworkFile file)
        {
            _db.homework_files.Add(file);
            Save();
        }

        /// <inheritdoc/>
        public void DeleteHomework(long id)
        {
            var exist = _db.homeworks.SingleOrDefault(x => x.id == id);
            if (exist != null)
            {
                _db.homework_classes.RemoveRange(exist.homework_classes);
                _db.homework_files.RemoveRange(exist.homework_files);
                _db.homework_answers.RemoveRange(exist.homework_answers);
                _db.homework_students.RemoveRange(exist.homework_students);
                _db.homeworks.Remove(exist);
                Save();
            }
        }

        /// <inheritdoc/>
        public void DeleteHomeworkClasses(long homeworkid)
        {
            var classes = _db.homework_classes.Where(x => x.homeworkid == homeworkid);
            _db.homework_classes.RemoveRange(classes);
            Save();
        }

        /// <inheritdoc/>
        public void DeleteHomeworkFile(HomeworkFile file)
        {
            _db.homework_files.Remove(file);
            Save();
        }
    }
}
