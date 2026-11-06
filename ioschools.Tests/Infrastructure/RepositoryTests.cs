using FluentAssertions;
using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Entities;
using ioschools.Domain.Enums;
using ioschools.Infrastructure.Data;
using ioschools.Infrastructure.Repositories;
using ioschools.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ioschools.Tests.Infrastructure
{
    /// <summary>
    /// Integration tests for repositories using EF Core InMemory database
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task UserRepository_GetByEmail_ReturnsCorrectUser()
        {
            // Arrange
            var user = new User
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hash",
                UserGroup = UserGroup.TEACHER
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var repo = new Repositories.UserRepository(_context);

            // Act
            var result = await repo.GetByEmailAsync("test@test.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@test.com");
        }

        [Fact]
        public async Task UserRepository_GetActiveUsers_ExcludesInactiveUsers()
        {
            // Arrange
            var activeUser = new User
            {
                Email = "active@test.com",
                FirstName = "Active",
                LastName = "User",
                PasswordHash = "hash",
                Settings = 0 // Not inactive
            };
            var inactiveUser = new User
            {
                Email = "inactive@test.com",
                FirstName = "Inactive",
                LastName = "User",
                PasswordHash = "hash",
                Settings = (int)UserSettings.INACTIVE
            };

            await _context.Users.AddRangeAsync(activeUser, inactiveUser);
            await _context.SaveChangesAsync();

            var repo = new Repositories.UserRepository(_context);

            // Act
            var result = await repo.GetActiveUsersAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Email.Should().Be("active@test.com");
        }

        [Fact]
        public async Task BlogRepository_GetPublicBlogs_ReturnsOnlyPublic()
        {
            // Arrange
            var creator = new User
            {
                Email = "creator@test.com",
                FirstName = "Creator",
                LastName = "User",
                PasswordHash = "hash"
            };
            await _context.Users.AddAsync(creator);
            await _context.SaveChangesAsync();

            var publicBlog = new Blog
            {
                Title = "Public",
                Content = "Content",
                IsPublic = true,
                CreatorId = creator.Id
            };
            var privateBlog = new Blog
            {
                Title = "Private",
                Content = "Content",
                IsPublic = false,
                CreatorId = creator.Id
            };

            await _context.Blogs.AddRangeAsync(publicBlog, privateBlog);
            await _context.SaveChangesAsync();

            var repo = new Repositories.BlogRepository(_context);

            // Act
            var result = await repo.GetPublicBlogsAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Public");
        }

        [Fact]
        public async Task ExamRepository_GetExamsByYear_ReturnsCorrectExams()
        {
            // Arrange
            var creator = new User
            {
                Email = "teacher@test.com",
                FirstName = "Teacher",
                LastName = "User",
                PasswordHash = "hash"
            };
            await _context.Users.AddAsync(creator);
            await _context.SaveChangesAsync();

            var exam2024 = new Exam { Name = "Exam 2024", Year = 2024, Term = 1, CreatorId = creator.Id };
            var exam2023 = new Exam { Name = "Exam 2023", Year = 2023, Term = 1, CreatorId = creator.Id };

            await _context.Exams.AddRangeAsync(exam2024, exam2023);
            await _context.SaveChangesAsync();

            var repo = new Repositories.ExamRepository(_context);

            // Act
            var result = await repo.GetExamsByYearAsync(2024);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Exam 2024");
        }

        [Fact]
        public async Task AttendanceRepository_GetByStudent_ReturnsStudentAttendance()
        {
            // Arrange
            var student = new User
            {
                Email = "student@test.com",
                FirstName = "Student",
                LastName = "User",
                PasswordHash = "hash",
                UserGroup = UserGroup.STUDENT
            };
            await _context.Users.AddAsync(student);
            await _context.SaveChangesAsync();

            var attendance = new Attendance
            {
                StudentId = student.Id,
                Date = DateTime.Today,
                Type = AttendanceType.PRESENT
            };
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();

            var repo = new Repositories.AttendanceRepository(_context);

            // Act
            var result = await repo.GetByStudentAsync(student.Id);

            // Assert
            result.Should().HaveCount(1);
            result.First().Type.Should().Be(AttendanceType.PRESENT);
        }

        [Fact]
        public async Task SchoolFeeRepository_GetOverdueFees_ReturnsUnpaidPastDueFees()
        {
            // Arrange
            var student = new User
            {
                Email = "student@test.com",
                FirstName = "Student",
                LastName = "User",
                PasswordHash = "hash"
            };
            await _context.Users.AddAsync(student);
            await _context.SaveChangesAsync();

            var overdueFee = new SchoolFee
            {
                StudentId = student.Id,
                Amount = 100,
                DueDate = DateTime.UtcNow.AddDays(-30),
                IsPaid = false,
                Year = 2024,
                Term = 1
            };
            var paidFee = new SchoolFee
            {
                StudentId = student.Id,
                Amount = 100,
                DueDate = DateTime.UtcNow.AddDays(-30),
                IsPaid = true,
                Year = 2024,
                Term = 2
            };

            await _context.SchoolFees.AddRangeAsync(overdueFee, paidFee);
            await _context.SaveChangesAsync();

            var repo = new Repositories.SchoolFeeRepository(_context);

            // Act
            var result = await repo.GetOverdueFeesAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().IsPaid.Should().BeFalse();
        }

        public void Dispose() => _context.Dispose();
    }

    /// <summary>
    /// Tests for CalendarService
    /// </summary>
    public class CalendarServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CalendarService _calendarService;

        public CalendarServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _calendarService = new CalendarService(_context, Mock.Of<ILogger<CalendarService>>());
        }

        [Fact]
        public async Task GetFutureEventsAsync_ReturnsEventsWithinDays()
        {
            // Arrange
            var futureEvent = new CalendarEvent
            {
                Title = "Future Event",
                StartDate = DateTime.UtcNow.AddDays(3),
                IsPublic = true
            };
            var pastEvent = new CalendarEvent
            {
                Title = "Past Event",
                StartDate = DateTime.UtcNow.AddDays(-3),
                IsPublic = true
            };
            var farFutureEvent = new CalendarEvent
            {
                Title = "Far Future Event",
                StartDate = DateTime.UtcNow.AddDays(30),
                IsPublic = true
            };

            await _context.CalendarEvents.AddRangeAsync(futureEvent, pastEvent, farFutureEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _calendarService.GetFutureEventsAsync(7);

            // Assert
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Future Event");
        }

        [Fact]
        public async Task CreateEventAsync_CreatesEventSuccessfully()
        {
            // Arrange
            var dto = new CalendarEventDto
            {
                Title = "New Event",
                StartDate = DateTime.UtcNow.AddDays(5),
                IsPublic = true
            };

            // Act
            var result = await _calendarService.CreateEventAsync(dto, 1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be("New Event");
        }

        [Fact]
        public async Task DeleteEventAsync_WithNonExistentEvent_ReturnsFailure()
        {
            // Act
            var result = await _calendarService.DeleteEventAsync(999, 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("not found");
        }

        public void Dispose() => _context.Dispose();
    }

    /// <summary>
    /// Tests for SchedulerService
    /// </summary>
    public class SchedulerServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly SchedulerService _schedulerService;

        public SchedulerServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _emailServiceMock = new Mock<IEmailService>();
            _schedulerService = new SchedulerService(
                _context,
                _emailServiceMock.Object,
                Mock.Of<ILogger<SchedulerService>>());
        }

        [Fact]
        public async Task SetStudentInactiveIfLeavingDateSetAsync_DeactivatesStudentsWithPastLeavingDate()
        {
            // Arrange
            var studentToDeactivate = new User
            {
                Email = "leaving@test.com",
                FirstName = "Leaving",
                LastName = "Student",
                PasswordHash = "hash",
                UserGroup = UserGroup.STUDENT,
                LeavingDate = DateTime.UtcNow.AddDays(-1),
                Settings = 0
            };
            var activeStudent = new User
            {
                Email = "active@test.com",
                FirstName = "Active",
                LastName = "Student",
                PasswordHash = "hash",
                UserGroup = UserGroup.STUDENT,
                LeavingDate = DateTime.UtcNow.AddDays(30),
                Settings = 0
            };

            await _context.Users.AddRangeAsync(studentToDeactivate, activeStudent);
            await _context.SaveChangesAsync();

            // Act
            await _schedulerService.SetStudentInactiveIfLeavingDateSetAsync();

            // Assert
            var deactivated = await _context.Users.FindAsync(studentToDeactivate.Id);
            (deactivated!.Settings & (int)UserSettings.INACTIVE).Should().NotBe(0);

            var stillActive = await _context.Users.FindAsync(activeStudent.Id);
            (stillActive!.Settings & (int)UserSettings.INACTIVE).Should().Be(0);
        }

        public void Dispose() => _context.Dispose();
    }
}
