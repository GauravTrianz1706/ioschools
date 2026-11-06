using ioschools.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ioschools.Web.Filters
{
    /// <summary>
    /// Permission filter attribute - migrated from legacy PermissionFilterAttribute
    /// 
    /// Legacy: System.Web.Mvc.ActionFilterAttribute
    /// New: Microsoft.AspNetCore.Mvc.Filters.IActionFilter
    /// 
    /// Key changes:
    /// - filterContext.HttpContext.Items["token"] → context.HttpContext.User (ClaimsPrincipal)
    /// - filterContext.IsChildAction → removed (no child actions in ASP.NET Core)
    /// - filterContext.HttpContext.Request.IsAjaxRequest() → check X-Requested-With header
    /// - filterContext.Result = new RedirectResult() → context.Result = new RedirectResult()
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        private readonly Permission _requiredPermission;

        public PermissionFilterAttribute(Permission permission = Permission.NONE)
        {
            _requiredPermission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                // User not logged in
                context.HttpContext.Response.StatusCode = 401;

                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new EmptyResult();
                }
                else
                {
                    var returnUrl = Uri.EscapeDataString(
                        context.HttpContext.Request.Path + context.HttpContext.Request.QueryString);
                    context.Result = new RedirectResult($"/login?redirect={returnUrl}");
                }
                return;
            }

            if (_requiredPermission != Permission.NONE)
            {
                var permClaim = user.FindFirst("Permissions");
                if (permClaim != null && Enum.TryParse<Permission>(permClaim.Value, out var userPerm))
                {
                    if ((userPerm & _requiredPermission) == 0)
                    {
                        context.HttpContext.Response.StatusCode = 403;

                        if (IsAjaxRequest(context.HttpContext.Request))
                        {
                            context.Result = new EmptyResult();
                        }
                        else
                        {
                            context.Result = new ViewResult { ViewName = "~/Views/Error/NoPermission.cshtml" };
                        }

                        var logger = context.HttpContext.RequestServices
                            .GetService<ILogger<PermissionFilterAttribute>>();
                        logger?.LogWarning("Permission denied for user {UserId} accessing {Controller}/{Action}",
                            user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                            context.ActionDescriptor.RouteValues["controller"],
                            context.ActionDescriptor.RouteValues["action"]);
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        private static bool IsAjaxRequest(HttpRequest request) =>
            request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    /// <summary>
    /// Groups filter attribute - migrated from legacy GroupsFilterAttribute
    /// Checks user group membership instead of permissions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class GroupsFilterAttribute : ActionFilterAttribute
    {
        private readonly UserGroup _requiredGroup;

        public GroupsFilterAttribute(UserGroup group)
        {
            _requiredGroup = group;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.HttpContext.Response.StatusCode = 401;
                if (IsAjaxRequest(context.HttpContext.Request))
                    context.Result = new EmptyResult();
                else
                {
                    var returnUrl = Uri.EscapeDataString(
                        context.HttpContext.Request.Path + context.HttpContext.Request.QueryString);
                    context.Result = new RedirectResult($"/login?redirect={returnUrl}");
                }
                return;
            }

            var groupClaim = user.FindFirst("UserGroup");
            if (groupClaim != null && Enum.TryParse<UserGroup>(groupClaim.Value, out var userGroup))
            {
                if (((userGroup & _requiredGroup) == 0) && userGroup != UserGroup.ADMIN)
                {
                    context.HttpContext.Response.StatusCode = 403;
                    if (IsAjaxRequest(context.HttpContext.Request))
                        context.Result = new EmptyResult();
                    else
                        context.Result = new ViewResult { ViewName = "~/Views/Error/NoPermission.cshtml" };
                    return;
                }
            }

            base.OnActionExecuting(context);
        }

        private static bool IsAjaxRequest(HttpRequest request) =>
            request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    /// <summary>
    /// No-cache filter - migrated from legacy NoCache ActionFilterAttribute
    /// 
    /// Legacy: response.AddHeader("Cache-Control", "no-cache;no-store")
    /// New: ResponseCacheAttribute or manual header setting
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Replaces legacy NoCache filter
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "-1";
            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// JSON body filter - migrated from legacy JsonFilterAttribute
    /// 
    /// Legacy: JavaScriptSerializer + Request.InputStream
    /// New: System.Text.Json + [FromBody] attribute (preferred)
    /// This filter is kept for backward compatibility with existing AJAX calls
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class JsonBodyFilterAttribute : ActionFilterAttribute
    {
        public string Param { get; set; } = string.Empty;
        public Type? RootType { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.ContentType?.Contains("application/json") == true &&
                context.HttpContext.Request.Method == "POST" &&
                RootType != null)
            {
                try
                {
                    context.HttpContext.Request.EnableBuffering();
                    using var reader = new StreamReader(context.HttpContext.Request.Body,
                        leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.HttpContext.Request.Body.Position = 0;

                    var obj = System.Text.Json.JsonSerializer.Deserialize(body, RootType,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (!string.IsNullOrEmpty(Param))
                        context.ActionArguments[Param] = obj;
                }
                catch (Exception ex)
                {
                    var logger = context.HttpContext.RequestServices
                        .GetService<ILogger<JsonBodyFilterAttribute>>();
                    logger?.LogError(ex, "Failed to deserialize JSON body for {Type}", RootType?.Name);
                }
            }

            await next();
        }
    }

    /// <summary>
    /// Error logging filter - migrated from legacy ElmahErrorAttribute
    /// 
    /// Legacy: Elmah.HandleErrorAttribute + ErrorSignal.FromContext
    /// New: IExceptionFilter with ILogger (Serilog)
    /// Note: Elmah is not compatible with ASP.NET Core - use Serilog/Application Insights instead
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ErrorLoggingFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetService<ILogger<ErrorLoggingFilterAttribute>>();

            logger?.LogError(context.Exception,
                "Unhandled exception in {Controller}/{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);

            base.OnException(context);
        }
    }
}
