using FluentAssertions;
using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Enums;
using ioschools.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ioschools.Tests.Web
{
    /// <summary>
    /// Unit tests for LoginController
    /// Tests migrated from legacy loginController
    /// </summary>
    public class LoginControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<LoginController>> _loggerMock;
        private readonly LoginController _controller;

        public LoginControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<LoginController>>();
            _controller = new LoginController(_authServiceMock.Object, _loggerMock.Object);

            // Setup HttpContext for authentication
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new Mock<IServiceProvider>().Object;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void Index_Get_WhenNotLoggedIn_ReturnsView()
        {
            // Arrange - user not authenticated
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity()); // Not authenticated

            // Act
            var result = _controller.Index(null);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Index_Get_WhenLoggedIn_RedirectsToDashboard()
        {
            // Arrange - user authenticated
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Name, "Test User")
            };
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "TestAuth"));

            // Act
            var result = _controller.Index(null);

            // Assert
            result.Should().BeOfType<RedirectResult>();
            ((RedirectResult)result).Url.Should().Be("/dashboard");
        }

        [Fact]
        public async Task Index_Post_WithEmptyEmail_ReturnsJsonFail()
        {
            // Act
            var result = await _controller.Index("", "password", false, null);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var json = (JsonResult)result;
            var response = json.Value as JsonResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }

        [Fact]
        public async Task Index_Post_WithValidCredentials_ReturnsJsonSuccess()
        {
            // Arrange
            var loginResponse = new LoginResponseDto
            {
                Success = true,
                RedirectUrl = "/dashboard",
                User = new UserDto
                {
                    Id = 1,
                    Email = "test@test.com",
                    FirstName = "Test",
                    LastName = "User",
                    UserGroup = UserGroup.TEACHER
                }
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(loginResponse);

            // Setup authentication service
            var authServiceProvider = new Mock<IServiceProvider>();
            var authService = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationService>();
            authService
                .Setup(s => s.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(), It.IsAny<Microsoft.AspNetCore.Authentication.AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            authServiceProvider
                .Setup(sp => sp.GetService(typeof(Microsoft.AspNetCore.Authentication.IAuthenticationService)))
                .Returns(authService.Object);
            _controller.ControllerContext.HttpContext.RequestServices = authServiceProvider.Object;

            // Act
            var result = await _controller.Index("test@test.com", "password", false, null);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var json = (JsonResult)result;
            var response = json.Value as JsonResponse<string>;
            response.Should().NotBeNull();
            response!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Password_Post_WithMismatchedPasswords_ReturnsJsonFail()
        {
            // Arrange - user authenticated
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Name, "Test User")
            };
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "TestAuth"));

            _authServiceMock
                .Setup(s => s.ChangePasswordAsync(It.IsAny<long>(), It.IsAny<ChangePasswordDto>()))
                .ReturnsAsync(Domain.Common.Result.Failure("Passwords are not the same"));

            // Act
            var result = await _controller.Password("password1", "password2");

            // Assert
            result.Should().BeOfType<JsonResult>();
            var json = (JsonResult)result;
            var response = json.Value as JsonResponse;
            response.Should().NotBeNull();
            response!.Success.Should().BeFalse();
        }
    }

    /// <summary>
    /// Unit tests for HomeController
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<IBlogService> _blogServiceMock;
        private readonly Mock<ICalendarService> _calendarServiceMock;
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _blogServiceMock = new Mock<IBlogService>();
            _calendarServiceMock = new Mock<ICalendarService>();
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(
                _blogServiceMock.Object,
                _calendarServiceMock.Object,
                _loggerMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithData()
        {
            // Arrange
            var events = new List<CalendarEventDto>
            {
                new() { Id = 1, Title = "Event 1", StartDate = DateTime.Now.AddDays(1) }
            };
            var news = new List<BlogDto>
            {
                new() { Id = 1, Title = "News 1", IsPublic = true, CreatedAt = DateTime.Now }
            };

            _calendarServiceMock
                .Setup(s => s.GetFutureEventsAsync(7))
                .ReturnsAsync(events);
            _blogServiceMock
                .Setup(s => s.GetPublicBlogsAsync())
                .ReturnsAsync(news);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            viewResult.ViewData["Events"].Should().NotBeNull();
            viewResult.ViewData["News"].Should().NotBeNull();
        }
    }

    /// <summary>
    /// Unit tests for DashboardController
    /// </summary>
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardService> _dashboardServiceMock;
        private readonly Mock<ILogger<DashboardController>> _loggerMock;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _dashboardServiceMock = new Mock<IDashboardService>();
            _loggerMock = new Mock<ILogger<DashboardController>>();
            _controller = new DashboardController(_dashboardServiceMock.Object, _loggerMock.Object);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Name, "Test User"),
                new("UserGroup", UserGroup.TEACHER.ToString()),
                new("Permissions", Permission.BLOG_CREATE.ToString())
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };
        }

        [Fact]
        public async Task Index_WithAuthenticatedUser_ReturnsView()
        {
            // Arrange
            var dashboard = new DashboardDto
            {
                CurrentUser = new UserDto { Id = 1, FirstName = "Test", LastName = "User" },
                TotalStudents = 100,
                TotalStaff = 20
            };

            _dashboardServiceMock
                .Setup(s => s.GetDashboardDataAsync(It.IsAny<long>(), It.IsAny<Domain.Common.UserAuth>()))
                .ReturnsAsync(dashboard);

            // Act
            var result = await _controller.Index(null);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            viewResult.Model.Should().BeOfType<DashboardDto>();
        }
    }
}
