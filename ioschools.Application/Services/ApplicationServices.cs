using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Common;
using ioschools.Domain.Entities;
using ioschools.Domain.Enums;
using ioschools.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ioschools.Application.Services
{
    /// <summary>
    /// Authentication service - replaces legacy cookie-based auth in baseController
    /// Migrated from: baseController.SetAuthCookie, GetAuthCookie, ClearAuthCookie
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IEmailService emailService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return new LoginResponseDto { Success = false, Message = "Please specify an email address" };

            var email = request.Email.Trim().ToLower();
            var passwordHash = GeneratePasswordHash(email, request.Password.Trim());

            var user = await _userRepository.GetActiveUserLoginAsync(email, passwordHash);
            if (user == null)
            {
                _logger.LogWarning("Login failed for {Email}", email);
                return new LoginResponseDto { Success = false, Message = "The email or password that you entered is incorrect" };
            }

            var redirectUrl = !string.IsNullOrEmpty(request.Redirect) ? request.Redirect : "/dashboard";

            return new LoginResponseDto
            {
                Success = true,
                RedirectUrl = redirectUrl,
                User = MapToDto(user)
            };
        }

        public async Task<Result> ChangePasswordAsync(long userId, ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return Result.Failure("Passwords are not the same");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure("Could not locate your account");

            user.PasswordHash = GeneratePasswordHash(user.Email.ToLower(), dto.NewPassword);
            user.Settings = user.UnsetFlag(UserSettings.PASSWORD_RESET);
            await _userRepository.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            var users = await _userRepository.GetActiveUsersAsync();
            var user = users.FirstOrDefault(x => string.Compare(x.Email, email, StringComparison.OrdinalIgnoreCase) == 0);

            if (user == null)
                return Result.Failure("Email address is invalid or does not exist");

            var password = GenerateRandomString();
            user.PasswordHash = GeneratePasswordHash(email, password);
            user.Settings = user.SetFlag(UserSettings.PASSWORD_RESET);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("{Email} forgot password", email);

            await _emailService.SendEmailAsync(email, user.ToName(),
                "Password Reset",
                $"<p>Your new password is: <strong>{password}</strong></p><p>Please change it after logging in.</p>",
                queue: false);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(long userId, long requesterId, UserAuth requesterAuth)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found");

            if (string.IsNullOrEmpty(user.Email))
                return Result.Failure("User does not have an email address");

            var email = user.Email.ToLower();
            var password = GenerateRandomString();
            user.PasswordHash = GeneratePasswordHash(email, password);
            user.Settings = user.SetFlag(UserSettings.PASSWORD_RESET);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("{Email} reset password", email);

            await _emailService.SendEmailAsync(email, user.ToName(),
                "Password Reset",
                $"<p>Your new password is: <strong>{password}</strong></p><p>Please change it after logging in.</p>",
                queue: false);

            return Result.Success();
        }

        public string GeneratePasswordHash(string email, string password)
        {
            // BCrypt hash - replaces legacy BCrypt.HashPassword
            return BCrypt.Net.BCrypt.HashPassword(email + password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string email, string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(email + password, hash);
        }

        private static string GenerateRandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            UserGroup = user.UserGroup,
            Permissions = user.Permissions,
            PhotoUrl = user.UserImage?.Url
        };
    }

    /// <summary>
    /// Blog service implementation
    /// </summary>
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ILogger<BlogService> _logger;

        public BlogService(IBlogRepository blogRepository, ILogger<BlogService> logger)
        {
            _blogRepository = blogRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<BlogDto>> GetPublicBlogsAsync()
        {
            var blogs = await _blogRepository.GetPublicBlogsAsync();
            return blogs.Select(MapToDto);
        }

        public async Task<IEnumerable<BlogDto>> GetAllBlogsAsync()
        {
            var blogs = await _blogRepository.GetAllAsync();
            return blogs.Select(MapToDto);
        }

        public async Task<BlogDto?> GetBlogAsync(long id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            return blog == null ? null : MapToDto(blog);
        }

        public async Task<Result<BlogDto>> CreateBlogAsync(BlogDto dto, long creatorId)
        {
            var blog = new Blog
            {
                Title = dto.Title,
                Content = dto.Content,
                IsPublic = dto.IsPublic,
                CreatorId = creatorId,
                ImageUrl = dto.ImageUrl,
                Tags = dto.Tags
            };
            var created = await _blogRepository.AddAsync(blog);
            return Result<BlogDto>.Success(MapToDto(created));
        }

        public async Task<Result> UpdateBlogAsync(long id, BlogDto dto, long editorId)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null) return Result.Failure("Blog not found");

            blog.Title = dto.Title;
            blog.Content = dto.Content;
            blog.IsPublic = dto.IsPublic;
            blog.ImageUrl = dto.ImageUrl;
            blog.Tags = dto.Tags;
            blog.UpdatedAt = DateTime.UtcNow;

            await _blogRepository.UpdateAsync(blog);
            return Result.Success();
        }

        public async Task<Result> DeleteBlogAsync(long id, long deleterId)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null) return Result.Failure("Blog not found");

            await _blogRepository.DeleteAsync(blog);
            return Result.Success();
        }

        private static BlogDto MapToDto(Blog blog) => new()
        {
            Id = blog.Id,
            Title = blog.Title,
            Content = blog.Content,
            IsPublic = blog.IsPublic,
            CreatorId = blog.CreatorId,
            CreatorName = blog.Creator?.ToName(),
            CreatedAt = blog.CreatedAt,
            ImageUrl = blog.ImageUrl,
            Tags = blog.Tags
        };
    }

    /// <summary>
    /// Dashboard service implementation
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ICalendarService _calendarService;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IUserRepository userRepository,
            IBlogRepository blogRepository,
            ICalendarService calendarService,
            ILogger<DashboardService> logger)
        {
            _userRepository = userRepository;
            _blogRepository = blogRepository;
            _calendarService = calendarService;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(long userId, UserAuth auth)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var events = await _calendarService.GetFutureEventsAsync(7);
            var news = await _blogRepository.GetPublicBlogsAsync();

            return new DashboardDto
            {
                CurrentUser = user == null ? null : new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserGroup = user.UserGroup,
                    Permissions = user.Permissions
                },
                UpcomingEvents = events.ToList(),
                RecentNews = news.OrderByDescending(x => x.CreatedAt).Take(4).Select(b => new BlogDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };
        }
    }
}
