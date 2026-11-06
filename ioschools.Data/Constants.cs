namespace ioschools.Data
{
    /// <summary>
    /// Application-wide constants for ioSchools.
    /// Migrated from legacy ASP.NET MVC Constants.cs to .NET 8.
    /// NOTE: Session key constants (SESSION_ID, SESSION_USERGROUP, etc.) are preserved
    /// for backward compatibility but session management is now handled via
    /// ASP.NET Core Session middleware configured in Program.cs.
    /// HTTP_HOST is preserved but should be read from IConfiguration / appsettings.json
    /// in production code rather than using compile-time constants.
    /// </summary>
    public static class Constants
    {
        // ---------------------------------------------------------------
        // Search
        // ---------------------------------------------------------------

        /// <summary>Maximum number of results returned by Lucene.Net search.</summary>
        public const int LUCENE_SEARCH_RESULTS_MAX = 50;

        // ---------------------------------------------------------------
        // Duration helpers (in seconds)
        // ---------------------------------------------------------------

        /// <summary>Number of seconds in one day (86 400).</summary>
        public const int DURATION_1DAY_SECS = 86_400;

        /// <summary>Number of seconds in one hour (3 600).</summary>
        public const int DURATION_1HOUR_SECS = 3_600;

        // ---------------------------------------------------------------
        // Date / time format strings
        // ---------------------------------------------------------------

        /// <summary>Long date format used by the date-picker widget (e.g. "Mon, 1 Jan 2024").</summary>
        public const string DATEFORMAT_DATEPICKER = "ddd, d MMM yyyy";

        /// <summary>Short date format used by the date-picker widget (e.g. "1 Jan 2024").</summary>
        public const string DATEFORMAT_DATEPICKER_SHORT = "d MMM yyyy";

        /// <summary>Standard display date (e.g. "01 January 2024").</summary>
        public const string DATETIME_STANDARD = "dd MMMM yyyy";

        /// <summary>Short display date (e.g. "01 Jan 2024").</summary>
        public const string DATETIME_SHORT_DATE = "dd MMM yyyy";

        /// <summary>Standard time display (e.g. "2:30 PM").</summary>
        public const string DATETIME_STANDARD_TIME = "h:mm tt";

        /// <summary>Full date-time display (e.g. "2:30 PM, 01 January 2024").</summary>
        public const string DATETIME_FULL = "h:mm tt, dd MMMM yyyy";

        /// <summary>Short time span format for TimeSpan.ToString (e.g. "14:30").</summary>
        public const string DATETIME_SHORTTIME = "hh\\:mm";

        // ---------------------------------------------------------------
        // File / disk limits
        // ---------------------------------------------------------------

        /// <summary>Maximum total disk usage per user in bytes (100 MB).</summary>
        public const long MAX_DISK_SIZE = 100_000_000L;

        // ---------------------------------------------------------------
        // Google Maps
        // NOTE: In production, read this value from IConfiguration / appsettings.json
        // rather than embedding it as a compile-time constant.
        // ---------------------------------------------------------------

#if DEBUG
        /// <summary>Google Maps API key (development placeholder).</summary>
        public const string GOOGLEMAP_APIKEY = "GOOGLEMAP_APIKEY";
#else
        /// <summary>Google Maps API key (production).</summary>
        public const string GOOGLEMAP_APIKEY = "GOOGLEMAP_APIKEY";
#endif

        // ---------------------------------------------------------------
        // Upload paths (relative to wwwroot)
        // Migrated: legacy paths under ~/Uploads/ are now served from wwwroot/uploads/
        // ---------------------------------------------------------------

        /// <summary>Root upload directory (relative to wwwroot).</summary>
        public const string UPLOAD_PATH = "uploads/";

        /// <summary>Blog image upload directory (relative to wwwroot).</summary>
        public const string UPLOAD_PATH_BLOG = "uploads/blog/";

        /// <summary>Homework file upload directory (relative to wwwroot).</summary>
        public const string UPLOAD_PATH_HOMEWORK = "uploads/homework/";

        /// <summary>Registration document upload directory (relative to wwwroot).</summary>
        public const string UPLOAD_PATH_REGISTRATION = "uploads/registration/";

        // ---------------------------------------------------------------
        // Session keys
        // Migrated: System.Web.SessionState → ASP.NET Core Session middleware.
        // These string keys are used with ISession.SetString / GetString.
        // ---------------------------------------------------------------

        /// <summary>Session key for the authenticated user's ID.</summary>
        public const string SESSION_ID = "id";

        /// <summary>Session key for the authenticated user's group.</summary>
        public const string SESSION_USERGROUP = "usergroup";

        /// <summary>Session key for the authenticated user's school.</summary>
        public const string SESSION_SCHOOL = "school";

        /// <summary>Session key for the authenticated user's display name.</summary>
        public const string SESSION_NAME = "name";

        // ---------------------------------------------------------------
        // Attendance
        // ---------------------------------------------------------------

        /// <summary>Number of consecutive absences that triggers a notification.</summary>
        public const int ATTENDANCE_TRIGGER_LEVEL = 3;

        // ---------------------------------------------------------------
        // HTTP host
        // NOTE: In production, read this value from IConfiguration / appsettings.json
        // (e.g. builder.Configuration["Application:BaseUrl"]) rather than using a
        // compile-time constant.
        // ---------------------------------------------------------------

#if DEBUG
        /// <summary>Base HTTP host URL (development).</summary>
        public const string HTTP_HOST = "http://localhost:33224";
#else
        /// <summary>Base HTTP host URL (production).</summary>
        public const string HTTP_HOST = "http://ioschools.edu.my";
#endif
    }
}
