using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ioschools.DB.Infrastructure.Caching
{
    /// <summary>
    /// Provides in-memory caching helpers for query results.
    /// Replaces the legacy <c>System.Web.Caching.HttpRuntime.Cache</c> and
    /// <c>SqlCacheDependency</c> approach with <see cref="IMemoryCache"/>.
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// Creates a composite cache key from an array of key parts.
        /// </summary>
        public static string CreateKey(object[] keys)
        {
            return string.Concat(string.Join(":", keys));
        }

        /// <summary>
        /// Retrieves a cached list or executes the query and caches the result.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The LINQ query to execute if cache miss.</param>
        /// <param name="cache">The <see cref="IMemoryCache"/> instance.</param>
        /// <param name="cacheId">The unique cache key.</param>
        /// <param name="absoluteExpiration">Optional absolute expiration. Defaults to 5 minutes.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        /// <returns>A cached or freshly queried list of <typeparamref name="T"/>.</returns>
        public static List<T> GetOrSet<T>(
            IQueryable<T> query,
            IMemoryCache cache,
            string cacheId,
            TimeSpan? absoluteExpiration = null,
            ILogger? logger = null)
        {
            if (cache.TryGetValue(cacheId, out List<T>? cachedResult) && cachedResult != null)
            {
                return cachedResult;
            }

            try
            {
                var result = query.ToList();

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(5)
                };

                cache.Set(cacheId, result, options);
                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error executing query for cache key: {CacheId}", cacheId);
                throw;
            }
        }

        /// <summary>
        /// Invalidates a specific cache entry by key.
        /// </summary>
        public static void Invalidate(IMemoryCache cache, string cacheId)
        {
            cache.Remove(cacheId);
        }
    }
}
