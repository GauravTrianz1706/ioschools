using ioschools.Application;
using ioschools.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;

// Configure Serilog - replaces legacy clearpixels.Logging.Syslog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/ioschools-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting ioSchools web application");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // ============================================================
    // SERVICE REGISTRATION
    // Replaces: Global.asax Application_Start + App_Start/*.cs
    // ============================================================

    // Add MVC - replaces System.Web.Mvc
    builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        })
        .AddRazorRuntimeCompilation(); // Development only - hot reload views

    // Add Razor Pages support
    builder.Services.AddRazorPages();

    // Cookie Authentication - replaces Forms Authentication from web.config
    // Legacy: <authentication mode="Forms"><forms loginUrl="/login" /></authentication>
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.ExpireTimeSpan = TimeSpan.FromDays(14); // replaces COOKIE_LIFETIME = 1209600 seconds
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true; // replaces <httpCookies httpOnlyCookies="true" />
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.Name = "ioschools_auth"; // replaces legacy "token" cookie
        });

    // Authorization
    builder.Services.AddAuthorization();

    // Session - replaces System.Web.SessionState
    // Legacy: <sessionState mode="Off" timeout="20" />
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Response Compression - replaces CompressFilter action filter
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<GzipCompressionProvider>();
        options.Providers.Add<BrotliCompressionProvider>();
    });

    // Response Caching - replaces OutputCache attribute
    builder.Services.AddResponseCaching();

    // HttpContextAccessor - needed for accessing HttpContext in services
    builder.Services.AddHttpContextAccessor();

    // Infrastructure services (EF Core, Repositories, etc.)
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Application services (Business logic)
    builder.Services.AddApplicationServices();

    // Background service - replaces legacy CacheScheduler using HttpRuntime.Cache
    builder.Services.AddHostedService<ioschools.Web.BackgroundServices.SchedulerBackgroundService>();

    var app = builder.Build();

    // ============================================================
    // MIDDLEWARE PIPELINE
    // Replaces: Global.asax Application_BeginRequest + HTTP Modules
    // ============================================================

    if (!app.Environment.IsDevelopment())
    {
        // Replaces: <customErrors mode="On" defaultRedirect="/Error" />
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    // Status code pages - replaces web.config httpErrors section
    app.UseStatusCodePagesWithReExecute("/error/{0}");

    app.UseHttpsRedirection();

    // Static files - replaces Content/ and Scripts/ folders
    // Files must be in wwwroot/ folder
    app.UseStaticFiles();

    // Response compression - replaces CompressFilter
    app.UseResponseCompression();

    // Response caching - replaces OutputCache
    app.UseResponseCaching();

    // Routing - replaces RouteConfig.RegisterRoutes
    app.UseRouting();

    // Authentication & Authorization
    // Replaces: FormsAuthentication module
    app.UseAuthentication();
    app.UseAuthorization();

    // Session
    app.UseSession();

    // ============================================================
    // ROUTE CONFIGURATION
    // Replaces: App_Start/RouteConfig.cs + Global.asax RegisterRoutes
    // ============================================================

    app.MapControllerRoute(
        name: "Javascripts",
        pattern: "js/{action}.js",
        defaults: new { controller = "Js" });

    app.MapControllerRoute(
        name: "CSS",
        pattern: "css/{action}.css",
        defaults: new { controller = "Css" });

    app.MapControllerRoute(
        name: "News",
        pattern: "news/{id}/{title?}",
        defaults: new { controller = "News", action = "Single" },
        constraints: new { id = @"\d+" });

    app.MapControllerRoute(
        name: "PhotoUpload",
        pattern: "photo/upload/{type}/{id?}",
        defaults: new { controller = "Photo", action = "Upload", type = "none" });

    app.MapControllerRoute(
        name: "SingleUserView",
        pattern: "users/{id}",
        defaults: new { controller = "Users", action = "Single" },
        constraints: new { id = @"\d+" });

    app.MapControllerRoute(
        name: "IndexView",
        pattern: "{controller}/{id}",
        defaults: new { action = "Index" },
        constraints: new { id = @"\d+" });

    // Areas - replaces AreaRegistration.RegisterAllAreas()
    app.MapControllerRoute(
        name: "ExamsSingleView",
        pattern: "exams/{id}",
        defaults: new { controller = "Exams", action = "Single", area = "exams" },
        constraints: new { id = @"\d+" });

    app.MapControllerRoute(
        name: "ExamsTemplates",
        pattern: "exams/templates/{action}/{id?}",
        defaults: new { controller = "Templates", action = "Index", area = "exams" });

    app.MapControllerRoute(
        name: "ExamsDefault",
        pattern: "exams/{action}/{id?}",
        defaults: new { controller = "Exams", action = "Index", area = "exams" });

    // Default route - replaces App_Start/RouteConfig.cs Default route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    Log.Information("ioSchools application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
