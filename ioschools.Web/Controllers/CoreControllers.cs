using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ioschools.Web.Controllers
{
    /// <summary>
    /// Login controller - migrated from legacy loginController
    /// Replaces: System.Web.Mvc.Controller → Microsoft.AspNetCore.Mvc.Controller
    /// Replaces: FormsAuthentication → Cookie Authentication
    /// Replaces: HttpUtility.UrlDecode → Uri.UnescapeDataString
    /// </summary>
    public class LoginController : BaseController
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService, ILogger<LoginController> logger)
            : base(logger)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index(string? redirect)
        {
            if (IsLoggedIn)
            {
                if (!string.IsNullOrEmpty(redirect))
                    return Redirect(redirect);
                return Redirect("/dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password, bool remember_me, string? redirect)
        {
            if (string.IsNullOrEmpty(email))
                return Json(JsonResponse.Fail("Please specify an email address"));

            var request = new LoginRequestDto
            {
                Email = email,
                Password = password,
                RememberMe = remember_me,
                Redirect = redirect
            };

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return Json(JsonResponse.Fail(result.Message ?? "Login failed"));

            if (result.User != null)
                await SignInAsync(result.User, remember_me);

            var homeUrl = !string.IsNullOrEmpty(redirect)
                ? Uri.UnescapeDataString(redirect)  // replaces HttpUtility.UrlDecode
                : "/dashboard";

            return Json(JsonResponse<string>.Ok(homeUrl));
        }

        [HttpPost]
        public async Task<IActionResult> Forgot(string email)
        {
            var result = await _authService.ForgotPasswordAsync(new ForgotPasswordDto { Email = email });
            if (!result.IsSuccess)
                return SendJsonErrorResponse(result.Error ?? "An error occurred");

            return Json(JsonResponse.Ok("Your new password has been sent to the email account you provided"));
        }

        [HttpGet]
        public IActionResult Password()
        {
            if (!IsLoggedIn)
                return Json(JsonResponse.Fail("Permission denied"));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Password(string newPass, string newPassConfirm)
        {
            if (!SessionId.HasValue)
                return Json(JsonResponse.Fail("Permission denied"));

            var result = await _authService.ChangePasswordAsync(SessionId.Value, new ChangePasswordDto
            {
                NewPassword = newPass,
                ConfirmPassword = newPassConfirm
            });

            if (!result.IsSuccess)
                return Json(JsonResponse.Fail(result.Error ?? "An error occurred"));

            return Json(JsonResponse.Ok("Your password has been changed successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Reset(long id)
        {
            if (!SessionId.HasValue)
                return Json(JsonResponse.Fail("Permission denied"));

            var result = await _authService.ResetPasswordAsync(id, SessionId.Value, CurrentUserAuth);
            if (!result.IsSuccess)
                return Json(JsonResponse.Fail(result.Error ?? "An error occurred"));

            return Json(JsonResponse.Ok("New password emailed successfully"));
        }
    }

    /// <summary>
    /// Logout controller - migrated from legacy logoutController
    /// </summary>
    public class LogoutController : BaseController
    {
        public LogoutController(ILogger<LogoutController> logger) : base(logger) { }

        public async Task<IActionResult> Index()
        {
            await SignOutAsync();
            return Redirect("/");
        }
    }

    /// <summary>
    /// Home controller - migrated from legacy homeController
    /// </summary>
    public class HomeController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly ICalendarService _calendarService;

        public HomeController(IBlogService blogService, ICalendarService calendarService,
            ILogger<HomeController> logger) : base(logger)
        {
            _blogService = blogService;
            _calendarService = calendarService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var events = await _calendarService.GetFutureEventsAsync(7);
            var news = await _blogService.GetPublicBlogsAsync();

            ViewBag.Events = events;
            ViewBag.News = news.OrderByDescending(x => x.CreatedAt).Take(4).ToList();

            return View();
        }
    }

    /// <summary>
    /// Dashboard controller - migrated from legacy dashboardController
    /// Replaces: [PermissionFilter(perm = Permission.NONE)] → [Authorize]
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
            : base(logger)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(long? id)
        {
            if (!SessionId.HasValue)
                return RedirectToAction("Index", "Home");

            var dashboard = await _dashboardService.GetDashboardDataAsync(SessionId.Value, CurrentUserAuth);
            return View(dashboard);
        }
    }

    /// <summary>
    /// Error controller - migrated from legacy errorController
    /// Replaces: Global.asax Application_Error handler
    /// </summary>
    public class ErrorController : BaseController
    {
        public ErrorController(ILogger<ErrorController> logger) : base(logger) { }

        [Route("error")]
        public IActionResult Index()
        {
            Response.StatusCode = 500;
            return View();
        }

        [Route("error/404")]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("error/403")]
        public IActionResult NoPermission()
        {
            Response.StatusCode = 403;
            return View();
        }
    }

    /// <summary>
    /// Admin controller - migrated from legacy adminController
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AdminController : BaseController
    {
        public AdminController(ILogger<AdminController> logger) : base(logger) { }

        public IActionResult Index() => View();
        public IActionResult ChangeLog() => View();
    }

    /// <summary>
    /// Help controller - migrated from legacy helpController
    /// </summary>
    public class HelpController : BaseController
    {
        public HelpController(ILogger<HelpController> logger) : base(logger) { }

        public IActionResult Index() => View();
    }

    /// <summary>
    /// Contact controller - migrated from legacy contactController
    /// </summary>
    public class ContactController : BaseController
    {
        public ContactController(ILogger<ContactController> logger) : base(logger) { }

        public IActionResult Index() => View();
    }

    /// <summary>
    /// Resources controller - migrated from legacy resourcesController
    /// </summary>
    public class ResourcesController : BaseController
    {
        public ResourcesController(ILogger<ResourcesController> logger) : base(logger) { }

        public IActionResult Index() => View();
    }

    /// <summary>
    /// Vacancies controller - migrated from legacy vacanciesController
    /// </summary>
    public class VacanciesController : BaseController
    {
        public VacanciesController(ILogger<VacanciesController> logger) : base(logger) { }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Edit(long? id) => View();

        [HttpPost]
        public IActionResult Edit(long? id, string title, string description) => RedirectToAction("Index");
    }

    /// <summary>
    /// Dummy controller - migrated from legacy dummyController
    /// Used for cache scheduler ping - now replaced by background service
    /// </summary>
    public class DummyController : BaseController
    {
        public DummyController(ILogger<DummyController> logger) : base(logger) { }

        public IActionResult Index() => Content("OK");
    }

    /// <summary>
    /// Test controller - migrated from legacy testController
    /// </summary>
    public class TestController : BaseController
    {
        public TestController(ILogger<TestController> logger) : base(logger) { }

        public IActionResult Index() => View();
    }
}
