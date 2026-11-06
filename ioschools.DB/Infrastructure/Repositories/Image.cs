using ioschools.DB.Domain.Entities;

namespace ioschools.DB.Infrastructure.Repositories
{
    /// <summary>
    /// Repository partial class - Image operations.
    /// </summary>
    public partial class Repository
    {
        /// <inheritdoc/>
        public long AddBlogImage(BlogImage img)
        {
            _db.blog_images.Add(img);
            _db.SaveChanges();
            return img.id;
        }

        /// <inheritdoc/>
        public long AddUserImage(UserImage image)
        {
            _db.user_images.Add(image);
            _db.SaveChanges();
            return image.id;
        }

        /// <inheritdoc/>
        public void DeleteUserImage(long id)
        {
            var image = _db.user_images.SingleOrDefault(x => x.id == id);
            if (image != null)
            {
                _db.user_images.Remove(image);
                Save();
            }
        }
    }
}
