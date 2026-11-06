using ioschools.Data;
using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - CMS/Blog/Page operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public void AddBlog(Blog blog)
        {
            _db.blogs.Add(blog);
        }

        /// <inheritdoc/>
        public long AddBlogFile(BlogFile file)
        {
            _db.blog_files.Add(file);
            Save();
            return file.id;
        }

        /// <inheritdoc/>
        public void DeleteBlog(long id)
        {
            var blog = _db.blogs.SingleOrDefault(x => x.id == id);
            if (blog != null)
            {
                _db.blogs.Remove(blog);
            }
        }

        /// <inheritdoc/>
        public void DeleteBlogFile(long id)
        {
            var exist = _db.blog_files.Single(x => x.id == id);
            _db.blog_files.Remove(exist);
        }

        /// <inheritdoc/>
        public void DeleteBlogPhoto(long id)
        {
            var exist = _db.blog_images.Single(x => x.id == id);
            _db.blog_images.Remove(exist);
        }

        /// <inheritdoc/>
        public Blog? GetBlog(long id)
        {
            return _db.blogs.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public BlogImage? GetBlogPhoto(long id)
        {
            return _db.blog_images.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public BlogFile? GetBlogFile(long id)
        {
            return _db.blog_files.SingleOrDefault(x => x.id == id);
        }

        /// <inheritdoc/>
        public IEnumerable<Blog> GetBlogs()
        {
            return _db.blogs;
        }

        /// <inheritdoc/>
        public IEnumerable<Blog> GetBlogs(BlogType? type, int? year)
        {
            var blogs = _db.blogs.AsQueryable();

            if (type.HasValue)
            {
                switch (type.Value)
                {
                    case BlogType.INTERNAL:
                        blogs = blogs.Where(x => !x.ispublic);
                        break;
                    case BlogType.PUBLIC:
                        blogs = blogs.Where(x => x.ispublic);
                        break;
                }
            }

            if (year.HasValue)
            {
                blogs = blogs.Where(x => x.created.Year == year.Value);
            }

            return blogs;
        }

        /// <inheritdoc/>
        public void UpdateBlogFiles(long id, string[] files)
        {
            var result = _db.blog_files.Where(x => files.Contains(x.id.ToString()));
            foreach (var row in result)
            {
                row.blogid = id;
            }
            Save();
        }

        /// <inheritdoc/>
        public void UpdateBlogImages(long blogid, IEnumerable<string> photoids)
        {
            var result = _db.blog_images.Where(x => photoids.Contains(x.id.ToString()));
            foreach (var row in result)
            {
                row.blogid = blogid;
            }
            Save();
        }

        /// <inheritdoc/>
        public Page? GetPage(int typeid)
        {
            return _db.pages.SingleOrDefault(x => x.type == typeid);
        }

        /// <inheritdoc/>
        public void AddPage(Page page)
        {
            _db.pages.Add(page);
        }
    }
}
