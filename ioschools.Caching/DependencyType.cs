namespace ioschools.Caching
{
    /// <summary>
    /// Discriminates the type of dependency used to group and invalidate related cache entries.
    /// When any entity of a given dependency type changes, all cache items registered
    /// under that dependency are evicted via <see cref="CacheHelper.InvalidateDependency"/>.
    /// </summary>
    public enum DependencyType
    {
        /// <summary>
        /// User-scoped dependency: invalidate all cached entries that depend on user data
        /// whenever a user record is created, updated, or deleted.
        /// </summary>
        USER
    }
}
