using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Enums;
using ioschools.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ioschools.Web.Controllers
{
    /// <summary>
    /// Blog controller - migrated from legacy blogController
    /// </summary>
    [Authorize]
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger) : base(logger)
        {
            _blogService = blogService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogService.GetPublicBlogsAsync();
            return View(blogs);
        }

        [PermissionFilter(Permission.BLOG_CREATE)]
        public IActionResult Add() => View();

        [HttpPost]
        [PermissionFilter(Permission.BLOG_CREATE)]
        public async Task<IActionResult> Add(BlogDto dto)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _blogService.CreateBlogAsync(dto, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to create blog");
            return Json(JsonResponse<BlogDto>.Ok(result.Value!));
        }

        [HttpPost]
        [PermissionFilter(Permission.BLOG_ADMIN)]
        public async Task<IActionResult> Delete(long id)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _blogService.DeleteBlogAsync(id, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to delete blog");
            return Json(JsonResponse.Ok("Blog deleted successfully"));
        }
    }

    /// <summary>
    /// News controller - migrated from legacy newsController
    /// </summary>
    public class NewsController : BaseController
    {
        private readonly IBlogService _blogService;

        public NewsController(IBlogService blogService, ILogger<NewsController> logger) : base(logger)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _blogService.GetPublicBlogsAsync();
            return View(news);
        }

        public async Task<IActionResult> Single(long id, string? title)
        {
            var blog = await _blogService.GetBlogAsync(id);
            if (blog == null) return ReturnNotFoundView();
            return View(blog);
        }
    }

    /// <summary>
    /// Calendar controller - migrated from legacy calendarController
    /// </summary>
    public class CalendarController : BaseController
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService, ILogger<CalendarController> logger)
            : base(logger)
        {
            _calendarService = calendarService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _calendarService.GetEventsAsync(DateTime.Now.Year);
            return View(events);
        }

        [Authorize]
        public IActionResult Admin() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(CalendarEventDto dto)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            if (dto.Id == 0)
            {
                var result = await _calendarService.CreateEventAsync(dto, SessionId.Value);
                if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to create event");
                return Json(JsonResponse<CalendarEventDto>.Ok(result.Value!));
            }
            else
            {
                var result = await _calendarService.UpdateEventAsync(dto.Id, dto, SessionId.Value);
                if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to update event");
                return Json(JsonResponse.Ok("Event updated successfully"));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(long id)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _calendarService.DeleteEventAsync(id, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to delete event");
            return Json(JsonResponse.Ok("Event deleted successfully"));
        }
    }

    /// <summary>
    /// Attendance controller - migrated from legacy attendanceController
    /// </summary>
    [Authorize]
    public class AttendanceController : BaseController
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService, ILogger<AttendanceController> logger)
            : base(logger)
        {
            _attendanceService = attendanceService;
        }

        [PermissionFilter(Permission.ATTENDANCE_CREATE)]
        public IActionResult Add() => View();

        [HttpPost]
        [PermissionFilter(Permission.ATTENDANCE_CREATE)]
        public async Task<IActionResult> Add(IEnumerable<AttendanceDto> records)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _attendanceService.RecordAttendanceAsync(records, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to record attendance");
            return Json(JsonResponse.Ok("Attendance recorded successfully"));
        }

        public async Task<IActionResult> Show(long id)
        {
            var records = await _attendanceService.GetAttendanceAsync(id);
            return View(records);
        }

        [PermissionFilter(Permission.ATTENDANCE_ADMIN)]
        public IActionResult Admin() => View();

        [HttpPost]
        [PermissionFilter(Permission.ATTENDANCE_ADMIN)]
        public async Task<IActionResult> Send(long studentId, DateTime date)
        {
            var result = await _attendanceService.SendAttendanceEmailAsync(studentId, date);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to send email");
            return Json(JsonResponse.Ok("Email sent successfully"));
        }
    }

    /// <summary>
    /// Homework controller - migrated from legacy homeworkController
    /// </summary>
    [Authorize]
    public class HomeworkController : BaseController
    {
        private readonly IHomeworkService _homeworkService;

        public HomeworkController(IHomeworkService homeworkService, ILogger<HomeworkController> logger)
            : base(logger)
        {
            _homeworkService = homeworkService;
        }

        [PermissionFilter(Permission.HOMEWORK_CREATE)]
        public IActionResult Index() => View();

        [PermissionFilter(Permission.HOMEWORK_CREATE)]
        public IActionResult Edit(long? id) => View();

        [HttpPost]
        [PermissionFilter(Permission.HOMEWORK_CREATE)]
        public async Task<IActionResult> Edit(HomeworkDto dto)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            if (dto.Id == 0)
            {
                var result = await _homeworkService.CreateHomeworkAsync(dto, SessionId.Value);
                if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to create homework");
                return Json(JsonResponse<HomeworkDto>.Ok(result.Value!));
            }
            else
            {
                var result = await _homeworkService.UpdateHomeworkAsync(dto.Id, dto, SessionId.Value);
                if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to update homework");
                return Json(JsonResponse.Ok("Homework updated successfully"));
            }
        }

        [HttpPost]
        [PermissionFilter(Permission.HOMEWORK_CREATE)]
        public async Task<IActionResult> Delete(long id)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _homeworkService.DeleteHomeworkAsync(id, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to delete homework");
            return Json(JsonResponse.Ok("Homework deleted successfully"));
        }

        public async Task<IActionResult> Show(long id)
        {
            var homework = await _homeworkService.GetHomeworkAsync(id);
            if (homework == null) return ReturnNotFoundView();
            return View(homework);
        }
    }

    /// <summary>
    /// Files controller - migrated from legacy filesController
    /// Replaces: Request.Params["CONTENT_LENGTH"] → Request.ContentLength
    /// Replaces: Request.InputStream → IFormFile
    /// </summary>
    [Authorize]
    public class FilesController : BaseController
    {
        private readonly IHomeworkService _homeworkService;
        private readonly IFileService _fileService;

        public FilesController(IHomeworkService homeworkService, IFileService fileService,
            ILogger<FilesController> logger) : base(logger)
        {
            _homeworkService = homeworkService;
            _fileService = fileService;
        }

        [HttpPost]
        [PermissionFilter(Permission.HOMEWORK_CREATE)]
        public async Task<IActionResult> Delete(long id)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _homeworkService.DeleteFileAsync(id, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to delete file");
            return Json(JsonResponse.Ok("File deleted successfully"));
        }

        /// <summary>
        /// File upload - replaces legacy Upload action using Request.InputStream
        /// Now uses IFormFile for proper ASP.NET Core file handling
        /// </summary>
        [HttpPost]
        [PermissionFilter(Permission.HOMEWORK_CREATE)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(JsonResponse.Fail("No file provided"));

            if (!SessionId.HasValue)
                return SendJsonNoPermission();

            using var stream = file.OpenReadStream();
            var result = await _homeworkService.UploadFileAsync(stream, file.FileName, SessionId.Value);

            if (!result.IsSuccess)
                return Json(JsonResponse.Fail(result.Error ?? "Failed to upload file"));

            return Json(JsonResponse<FileUploadResultDto>.Ok(result.Value!));
        }
    }

    /// <summary>
    /// Photo controller - migrated from legacy photoController
    /// </summary>
    [Authorize]
    public class PhotoController : BaseController
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;

        public PhotoController(IFileService fileService, IUserService userService,
            ILogger<PhotoController> logger) : base(logger)
        {
            _fileService = fileService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            if (SessionId.HasValue)
                await _userService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(long? id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(JsonResponse.Fail("No file provided"));

            using var stream = file.OpenReadStream();
            var result = await _fileService.SaveFileAsync(stream, file.FileName, Domain.Enums.UploaderType.PHOTO);

            if (!result.IsSuccess)
                return Json(JsonResponse.Fail(result.Error ?? "Failed to upload photo"));

            return Json(JsonResponse<FileUploadResultDto>.Ok(result.Value!));
        }
    }

    /// <summary>
    /// Finance controller - migrated from legacy financeController
    /// </summary>
    [Authorize]
    [PermissionFilter(Permission.FINANCE_VIEW)]
    public class FinanceController : BaseController
    {
        private readonly ISchoolFeeService _feeService;

        public FinanceController(ISchoolFeeService feeService, ILogger<FinanceController> logger)
            : base(logger)
        {
            _feeService = feeService;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Fees()
        {
            var fees = await _feeService.GetOverdueFeesAsync();
            return View(fees);
        }

        [PermissionFilter(Permission.FINANCE_ADMIN)]
        public async Task<IActionResult> Alerts()
        {
            var overdue = await _feeService.GetOverdueFeesAsync();
            return View(overdue);
        }
    }

    /// <summary>
    /// School fees controller - migrated from legacy schoolfeesController
    /// </summary>
    [Authorize]
    public class SchoolFeesController : BaseController
    {
        private readonly ISchoolFeeService _feeService;

        public SchoolFeesController(ISchoolFeeService feeService, ILogger<SchoolFeesController> logger)
            : base(logger)
        {
            _feeService = feeService;
        }

        public async Task<IActionResult> ShowStudent(long id)
        {
            var fees = await _feeService.GetStudentFeesAsync(id);
            return View(fees);
        }

        [HttpPost]
        [PermissionFilter(Permission.FINANCE_ADMIN)]
        public async Task<IActionResult> Update(long id, bool isPaid)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _feeService.UpdateFeeStatusAsync(id, isPaid, SessionId.Value);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to update fee");
            return Json(JsonResponse.Ok("Fee status updated"));
        }
    }

    /// <summary>
    /// Feedback controller - migrated from legacy feedbackController
    /// </summary>
    public class FeedbackController : BaseController
    {
        public FeedbackController(ILogger<FeedbackController> logger) : base(logger) { }

        public IActionResult Student() => View();
        public IActionResult Vendor() => View();

        [HttpPost]
        public IActionResult Submit(string name, string email, string message)
        {
            return Json(JsonResponse.Ok("Thank you for your feedback"));
        }
    }

    /// <summary>
    /// Schedule controller - migrated from legacy scheduleController
    /// </summary>
    [Authorize]
    public class ScheduleController : BaseController
    {
        public ScheduleController(ILogger<ScheduleController> logger) : base(logger) { }

        public IActionResult Index() => View();
    }

    /// <summary>
    /// Discipline controller - migrated from legacy disciplineController
    /// </summary>
    [Authorize]
    public class DisciplineController : BaseController
    {
        public DisciplineController(ILogger<DisciplineController> logger) : base(logger) { }

        [PermissionFilter(Permission.DISCIPLINE_CREATE)]
        public IActionResult Add() => View();

        public IActionResult Show(long id) => View();

        [PermissionFilter(Permission.DISCIPLINE_ADMIN)]
        public IActionResult Statistics() => View();
    }

    /// <summary>
    /// Conduct controller - migrated from legacy conductController
    /// </summary>
    [Authorize]
    public class ConductController : BaseController
    {
        public ConductController(ILogger<ConductController> logger) : base(logger) { }

        [PermissionFilter(Permission.DISCIPLINE_ADMIN)]
        public IActionResult Edit(long id) => View();
    }

    /// <summary>
    /// ECA controller - migrated from legacy ecaController
    /// </summary>
    [Authorize]
    public class EcaController : BaseController
    {
        public EcaController(ILogger<EcaController> logger) : base(logger) { }

        [PermissionFilter(Permission.ECA_CREATE)]
        public IActionResult Edit(long? id) => View();

        public IActionResult ShowStudent(long id) => View();

        [PermissionFilter(Permission.ECA_ADMIN)]
        public IActionResult EditStudent(long id) => View();
    }

    /// <summary>
    /// Leave controller - migrated from legacy leaveController
    /// </summary>
    [Authorize]
    public class LeaveController : BaseController
    {
        public LeaveController(ILogger<LeaveController> logger) : base(logger) { }

        [PermissionFilter(Permission.LEAVE_CREATE)]
        public IActionResult Index() => View();

        [PermissionFilter(Permission.LEAVE_CREATE)]
        public IActionResult NewLeave() => View();

        [PermissionFilter(Permission.LEAVE_ADMIN)]
        public IActionResult Review(long id) => View();
    }

    /// <summary>
    /// Staff controller - migrated from legacy staffController
    /// </summary>
    [Authorize]
    public class StaffController : BaseController
    {
        public StaffController(ILogger<StaffController> logger) : base(logger) { }

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Show(long id) => View();
    }

    /// <summary>
    /// Users controller - migrated from legacy usersController
    /// </summary>
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ISearchService _searchService;

        public UsersController(IUserService userService, ISearchService searchService,
            ILogger<UsersController> logger) : base(logger)
        {
            _userService = userService;
            _searchService = searchService;
        }

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Add() => View();

        public async Task<IActionResult> Single(long id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null) return ReturnNotFoundView();
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var results = await _searchService.SearchUsersAsync(term);
            return Json(JsonResponse<IEnumerable<UserDto>>.Ok(results));
        }
    }

    /// <summary>
    /// Guardian controller - migrated from legacy guardianController
    /// </summary>
    [Authorize]
    public class GuardianController : BaseController
    {
        public GuardianController(ILogger<GuardianController> logger) : base(logger) { }

        public IActionResult Index(long id) => View();
    }

    /// <summary>
    /// Admission controller - migrated from legacy admissionController
    /// </summary>
    public class AdmissionController : BaseController
    {
        public AdmissionController(ILogger<AdmissionController> logger) : base(logger) { }

        public IActionResult Index() => View();
        public IActionResult Form() => View();
    }

    /// <summary>
    /// Enrolment controller - migrated from legacy enrolmentController
    /// </summary>
    [Authorize]
    public class EnrolmentController : BaseController
    {
        public EnrolmentController(ILogger<EnrolmentController> logger) : base(logger) { }

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Index() => View();

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Add() => View();

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Edit(long id) => View();
    }

    /// <summary>
    /// Schools controller - migrated from legacy schoolsController
    /// </summary>
    [Authorize]
    public class SchoolsController : BaseController
    {
        public SchoolsController(ILogger<SchoolsController> logger) : base(logger) { }

        [PermissionFilter(Permission.SCHOOL_ADMIN)]
        public IActionResult Index() => View();
    }

    /// <summary>
    /// Classes controller - migrated from legacy classesController
    /// </summary>
    [Authorize]
    public class ClassesController : BaseController
    {
        public ClassesController(ILogger<ClassesController> logger) : base(logger) { }

        [PermissionFilter(Permission.SCHOOL_ADMIN)]
        public IActionResult Edit(long? id) => View();
    }

    /// <summary>
    /// Subjects controller - migrated from legacy subjectsController
    /// </summary>
    [Authorize]
    public class SubjectsController : BaseController
    {
        public SubjectsController(ILogger<SubjectsController> logger) : base(logger) { }

        [PermissionFilter(Permission.SCHOOL_ADMIN)]
        public IActionResult Edit(long? id) => View();
    }

    /// <summary>
    /// Grading controller - migrated from legacy gradingController
    /// </summary>
    [Authorize]
    public class GradingController : BaseController
    {
        public GradingController(ILogger<GradingController> logger) : base(logger) { }

        [PermissionFilter(Permission.SCHOOL_ADMIN)]
        public IActionResult Edit(long? id) => View();
    }

    /// <summary>
    /// Email controller - migrated from legacy emailController
    /// </summary>
    [Authorize]
    public class EmailController : BaseController
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger) : base(logger)
        {
            _emailService = emailService;
        }

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Index() => View();
    }

    /// <summary>
    /// Leaving cert controller - migrated from legacy leavingCertController
    /// </summary>
    [Authorize]
    public class LeavingCertController : BaseController
    {
        public LeavingCertController(ILogger<LeavingCertController> logger) : base(logger) { }

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Index(long id) => View();
    }

    /// <summary>
    /// Testimonial controller - migrated from legacy testimonialController
    /// </summary>
    [Authorize]
    public class TestimonialController : BaseController
    {
        public TestimonialController(ILogger<TestimonialController> logger) : base(logger) { }

        [PermissionFilter(Permission.USER_ADMIN)]
        public IActionResult Index(long id) => View();
    }

    /// <summary>
    /// Fees controller - migrated from legacy feesController
    /// </summary>
    [Authorize]
    public class FeesController : BaseController
    {
        public FeesController(ILogger<FeesController> logger) : base(logger) { }

        [PermissionFilter(Permission.FINANCE_VIEW)]
        public IActionResult Index() => View();
    }

    /// <summary>
    /// Uniforms controller - migrated from legacy uniformsController
    /// </summary>
    public class UniformsController : BaseController
    {
        public UniformsController(ILogger<UniformsController> logger) : base(logger) { }

        public IActionResult Index() => View();
    }

    /// <summary>
    /// About controller - migrated from legacy aboutController
    /// </summary>
    public class AboutController : BaseController
    {
        public AboutController(ILogger<AboutController> logger) : base(logger) { }

        public IActionResult Index() => View();
        public IActionResult Mission() => View();
        public IActionResult Vision() => View();
        public IActionResult History() => View();
        public IActionResult Philosophy() => View();
        public IActionResult People() => View();
        public IActionResult Spirit() => View();
    }

    /// <summary>
    /// Schools sub-controllers - migrated from legacy school-specific controllers
    /// </summary>
    public class KindergartenController : BaseController
    {
        public KindergartenController(ILogger<KindergartenController> logger) : base(logger) { }
        public IActionResult Index() => View();
        public IActionResult AfterSchool() => View();
    }

    public class PrimaryController : BaseController
    {
        public PrimaryController(ILogger<PrimaryController> logger) : base(logger) { }
        public IActionResult Index() => View();
        public IActionResult Eca() => View();
    }

    public class SecondaryController : BaseController
    {
        public SecondaryController(ILogger<SecondaryController> logger) : base(logger) { }
        public IActionResult Index() => View();
        public IActionResult Eca() => View();
    }

    public class InternationalController : BaseController
    {
        public InternationalController(ILogger<InternationalController> logger) : base(logger) { }
        public IActionResult Index() => View();
        public IActionResult Eca() => View();
    }
}
