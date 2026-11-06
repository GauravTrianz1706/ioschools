using FluentAssertions;
using ioschools.Application.DTOs;
using ioschools.Application.Services;
using ioschools.Domain.Entities;
using ioschools.Domain.Enums;
using ioschools.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ioschools.Tests.Application
{
    /// <summary>
    /// Unit tests for AuthService
    /// Tests migrated authentication logic from legacy baseController/loginController
    /// </summary>
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WithEmptyEmail_ReturnsFailure()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "", Password = "password" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("email address");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ReturnsFailure()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetActiveUserLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            var request = new LoginRequestDto { Email = "test@test.com", Password = "wrongpassword" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("incorrect");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                UserGroup = UserGroup.TEACHER,
                Permissions = Permission.BLOG_CREATE
            };

            _userRepositoryMock
                .Setup(r => r.GetActiveUserLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "password",
                RememberMe = false
            };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.User.Should().NotBeNull();
            result.User!.Email.Should().Be("test@test.com");
            result.RedirectUrl.Should().Be("/dashboard");
        }

        [Fact]
        public async Task LoginAsync_WithRedirect_ReturnsRedirectUrl()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock
                .Setup(r => r.GetActiveUserLoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "password",
                Redirect = "/dashboard/special"
            };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.RedirectUrl.Should().Be("/dashboard/special");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithMismatchedPasswords_ReturnsFailure()
        {
            // Arrange
            var dto = new ChangePasswordDto
            {
                NewPassword = "password1",
                ConfirmPassword = "password2"
            };

            // Act
            var result = await _authService.ChangePasswordAsync(1, dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("same");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithUserNotFound_ReturnsFailure()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((User?)null);

            var dto = new ChangePasswordDto
            {
                NewPassword = "password",
                ConfirmPassword = "password"
            };

            // Act
            var result = await _authService.ChangePasswordAsync(999, dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("account");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                Settings = 0
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var dto = new ChangePasswordDto
            {
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = await _authService.ChangePasswordAsync(1, dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ForgotPasswordAsync_WithInvalidEmail_ReturnsFailure()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetActiveUsersAsync())
                .ReturnsAsync(new List<User>());

            var dto = new ForgotPasswordDto { Email = "nonexistent@test.com" };

            // Act
            var result = await _authService.ForgotPasswordAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("invalid");
        }

        [Fact]
        public void GeneratePasswordHash_ReturnsBCryptHash()
        {
            // Arrange
            var email = "test@test.com";
            var password = "testpassword";

            // Act
            var hash = _authService.GeneratePasswordHash(email, password);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().StartWith("$2");  // BCrypt hash prefix
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var email = "test@test.com";
            var password = "testpassword";
            var hash = _authService.GeneratePasswordHash(email, password);

            // Act
            var result = _authService.VerifyPassword(email, password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithWrongPassword_ReturnsFalse()
        {
            // Arrange
            var email = "test@test.com";
            var password = "testpassword";
            var hash = _authService.GeneratePasswordHash(email, password);

            // Act
            var result = _authService.VerifyPassword(email, "wrongpassword", hash);

            // Assert
            result.Should().BeFalse();
        }
    }

    /// <summary>
    /// Unit tests for BlogService
    /// </summary>
    public class BlogServiceTests
    {
        private readonly Mock<IBlogRepository> _blogRepositoryMock;
        private readonly Mock<ILogger<BlogService>> _loggerMock;
        private readonly BlogService _blogService;

        public BlogServiceTests()
        {
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _loggerMock = new Mock<ILogger<BlogService>>();
            _blogService = new BlogService(_blogRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetPublicBlogsAsync_ReturnsOnlyPublicBlogs()
        {
            // Arrange
            var blogs = new List<Blog>
            {
                new() { Id = 1, Title = "Public Blog", IsPublic = true, CreatorId = 1 },
                new() { Id = 2, Title = "Private Blog", IsPublic = false, CreatorId = 1 }
            };

            _blogRepositoryMock
                .Setup(r => r.GetPublicBlogsAsync())
                .ReturnsAsync(blogs.Where(b => b.IsPublic));

            // Act
            var result = await _blogService.GetPublicBlogsAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Public Blog");
        }

        [Fact]
        public async Task CreateBlogAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new BlogDto
            {
                Title = "New Blog",
                Content = "Blog content",
                IsPublic = true
            };

            var createdBlog = new Blog
            {
                Id = 1,
                Title = dto.Title,
                Content = dto.Content,
                IsPublic = dto.IsPublic,
                CreatorId = 1
            };

            _blogRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Blog>()))
                .ReturnsAsync(createdBlog);

            // Act
            var result = await _blogService.CreateBlogAsync(dto, 1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be("New Blog");
        }

        [Fact]
        public async Task DeleteBlogAsync_WithNonExistentBlog_ReturnsFailure()
        {
            // Arrange
            _blogRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((Blog?)null);

            // Act
            var result = await _blogService.DeleteBlogAsync(999, 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }
    }
}
