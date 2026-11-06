namespace ioschools.Caching
{
    /// <summary>
    /// Discriminates the type of object stored in the cache.
    /// Add new values here as additional cacheable resource types are introduced.
    /// </summary>
    public enum CacheItemType
    {
        /// <summary>Cached list of application users.</summary>
        USERLIST
    }
}
