namespace ioschools.Domain.Common
{
    /// <summary>
    /// Common result wrapper for service operations
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public string? Error { get; private set; }

        private Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(string error) => new(false, default, error);
    }

    /// <summary>
    /// Non-generic result for void operations
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? Error { get; private set; }

        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);
    }

    /// <summary>
    /// Paged result for list operations
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// User authentication info - replaces legacy UserAuth
    /// </summary>
    public class UserAuth
    {
        public Domain.Enums.UserGroup Group { get; set; }
        public Domain.Enums.Permission Permissions { get; set; }
    }

    /// <summary>
    /// User super groups helper - migrated from legacy UserSuperGroups
    /// </summary>
    public static class UserSuperGroups
    {
        public static readonly Domain.Enums.UserGroup STAFF =
            Domain.Enums.UserGroup.ADMIN |
            Domain.Enums.UserGroup.TEACHER |
            Domain.Enums.UserGroup.PRINCIPAL |
            Domain.Enums.UserGroup.CLERK |
            Domain.Enums.UserGroup.FINANCE;
    }
}
