using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Common;
using ioschools.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ioschools.Web.Controllers
{
    /// <summary>
    /// Base controller for ASP.NET Core MVC
    /// Replaces legacy baseController which used System.Web.Mvc.Controller
    /// 
    /// Key migrations:
    /// - HttpContext.Items["token"] → ClaimsPrincipal (User)
    /// - Request.Cookies["token"] → Cookie Authentication middleware
    /// - Response.Cookies["token"] → SignInAsync with ClaimsPrincipal
    /// - ErrorSignal.FromCurrentContext() → ILogger
    /// - Json(x.ToJsonFail()) → Json(JsonResponse.Fail(x))
    /// - Json(x.ToJsonOKData()) → Json(JsonResponse<T>.Ok(x))
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly ILogger _logger;

        protected long? SessionId => GetSessionId();
        protected UserAuth CurrentUserAuth => GetCurrentUserAuth();
        protected bool IsLoggedIn => User.Identity?.IsAuthenticated ?? false;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        private long? GetSessionId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && long.TryParse(claim.Value, out var id))
                return id;
            return null;
        }

        private UserAuth GetCurrentUserAuth()
        {
            var auth = new UserAuth();
            var groupClaim = User.FindFirst("UserGroup");
            var permClaim = User.FindFirst("Permissions");

            if (groupClaim != null && Enum.TryParse<UserGroup>(groupClaim.Value, out var group))
                auth.Group = group;
            if (permClaim != null && Enum.TryParse<Permission>(permClaim.Value, out var perm))
                auth.Permissions = perm;

            return auth;
        }

        /// <summary>
        /// Set authentication cookie - replaces legacy SetAuthCookie
        /// </summary>
        protected async Task SignInAsync(UserDto user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
                new("UserGroup", user.UserGroup.ToString()),
                new("Permissions", user.Permissions.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe
                    ? DateTimeOffset.UtcNow.AddDays(14)   // replaces COOKIE_LIFETIME = 1209600 seconds
                    : DateTimeOffset.UtcNow.AddHours(1)    // replaces COOKIE_LIFETIME_MIN = 3600 seconds
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
        }

        /// <summary>
        /// Sign out - replaces legacy ClearAuthCookie
        /// </summary>
        protected async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Return not found view - replaces legacy ReturnNotFoundView
        /// </summary>
        protected IActionResult ReturnNotFoundView() =>
            View("~/Views/Error/NotFound.cshtml");

        /// <summary>
        /// Return no permission view - replaces legacy ReturnNoPermissionView
        /// </summary>
        protected IActionResult ReturnNoPermissionView() =>
            View("~/Views/Error/NoPermission.cshtml");

        /// <summary>
        /// Send JSON error response - replaces legacy SendJsonErrorResponse
        /// </summary>
        protected IActionResult SendJsonErrorResponse(Exception ex)
        {
            _logger.LogError(ex, "Error in {Controller}.{Action}", 
                ControllerContext.ActionDescriptor.ControllerName,
                ControllerContext.ActionDescriptor.ActionName);
            return Json(JsonResponse.Fail("Oops, we did something wrong. This will be fixed within 24 hours."));
        }

        protected IActionResult SendJsonErrorResponse(string message)
        {
            _logger.LogError("Error: {Message} by user {UserId}", message, SessionId);
            return Json(JsonResponse.Fail(message));
        }

        /// <summary>
        /// Send JSON no permission response - replaces legacy SendJsonNoPermission
        /// </summary>
        protected IActionResult SendJsonNoPermission(string message = "You are not allowed to do that")
        {
            _logger.LogWarning("Permission denied: {Message} for user {UserId}", message, SessionId);
            return Json(JsonResponse.Fail(message));
        }
    }
}
