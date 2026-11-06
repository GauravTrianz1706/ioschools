using Microsoft.Extensions.Caching.Memory;

namespace ioschools.Caching
{
    /// <summary>
    /// Thread-safe in-memory cache helper with dependency-based invalidation.
    /// Replaces the legacy System.Web.Caching.HttpRuntime.Cache implementation
    /// with Microsoft.Extensions.Caching.Memory.IMemoryCache for .NET 8 compatibility.
    ///
    /// Migration notes:
    ///   - HttpRuntime.Cache.Get(key)    → _cache.TryGetValue(key, out value)
    ///   - HttpRuntime.Cache.Insert(key) → _cache.Set(key, value)
    ///   - HttpRuntime.Cache.Remove(key) → _cache.Remove(key)
    ///   - Singleton pattern preserved; register via AddSingleton&lt;CacheHelper&gt;() in DI.
    /// </summary>
    public sealed class CacheHelper
    {
        // ------------------------------------------------------------------ //
        //  Fields
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Dependency map: key = dependency key, value = set of cache-item keys
        /// that must be evicted when the dependency is invalidated.
        /// </summary>
        private readonly Dictionary<string, HashSet<string>> _depMap;

        /// <summary>
        /// Lock object used to make all cache and dependency-map operations
        /// thread-safe without blocking reads unnecessarily.
        /// </summary>
        private readonly object _lock = new();

        /// <summary>
        /// Underlying IMemoryCache provided by Microsoft.Extensions.Caching.Memory.
        /// Replaces System.Web.Caching.HttpRuntime.Cache.
        /// </summary>
        private readonly IMemoryCache _cache;

        // ------------------------------------------------------------------ //
        //  Singleton (preserved from legacy implementation)
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Default singleton instance backed by a MemoryCache with default options.
        /// Prefer constructor injection (see parameterised constructor) in new code.
        /// </summary>
        public static readonly CacheHelper Instance = new CacheHelper(
            new MemoryCache(new MemoryCacheOptions()));

        // ------------------------------------------------------------------ //
        //  Constructors
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Initialises a new CacheHelper with the supplied IMemoryCache.
        /// Use this constructor when registering via dependency injection:
        /// <code>
        ///   services.AddMemoryCache();
        ///   services.AddSingleton&lt;CacheHelper&gt;();
        /// </code>
        /// </summary>
        /// <param name="cache">The IMemoryCache instance to use for storage.</param>
        public CacheHelper(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _depMap = new Dictionary<string, HashSet<string>>();
        }

        // ------------------------------------------------------------------ //
        //  Private helpers
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Builds the cache-item key from a type discriminator and a user-supplied key.
        /// Format: "{type}_res:{key}"
        /// </summary>
        private static string CreateCacheItemKey(CacheItemType type, string key)
            => $"{type}_res:{key}";

        /// <summary>
        /// Builds the dependency key from a dependency-type discriminator and a user-supplied key.
        /// Format: "{type}_deps:{key}"
        /// </summary>
        private static string CreateDependencyKey(DependencyType type, string key)
            => $"{type}_deps:{key}";

        // ------------------------------------------------------------------ //
        //  Public API
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Attempts to retrieve a cached value.
        /// Replaces: HttpRuntime.Cache.Get(cachekey)
        /// </summary>
        /// <param name="type">The cache-item type discriminator.</param>
        /// <param name="key">The user-supplied key.</param>
        /// <param name="value">
        ///   When this method returns <c>true</c>, contains the cached object;
        ///   otherwise <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the item was found in the cache; otherwise <c>false</c>.</returns>
        public bool TryGetCache(CacheItemType type, string key, out object? value)
        {
            string cacheKey = CreateCacheItemKey(type, key);
            return _cache.TryGetValue(cacheKey, out value);
        }

        /// <summary>
        /// Inserts a value into the cache with no sliding or absolute expiration.
        /// Cache dependencies are managed manually via <see cref="AddDependency"/>.
        /// Replaces: HttpRuntime.Cache.Insert(cacheKey, value)
        /// </summary>
        /// <param name="type">The cache-item type discriminator.</param>
        /// <param name="key">The user-supplied key.</param>
        /// <param name="value">The value to cache. If <c>null</c>, the call is a no-op.</param>
        /// <returns>
        ///   The composite cache key used for storage, or <c>null</c> if <paramref name="value"/>
        ///   was <c>null</c>.
        /// </returns>
        public string? Insert(CacheItemType type, string key, object value)
        {
            if (value == null)
            {
                return null;
            }

            string cacheKey = CreateCacheItemKey(type, key);

            // Store with no expiration - dependency invalidation is handled manually.
            // MemoryCacheEntryOptions with no expiry mirrors the legacy HttpRuntime.Cache.Insert
            // overload that accepted only key and value.
            _cache.Set(cacheKey, value, new MemoryCacheEntryOptions());

            return cacheKey;
        }

        /// <summary>
        /// Registers a dependency between a dependency key and a cache-item key so that
        /// when <see cref="InvalidateDependency"/> is called for the dependency, all
        /// associated cache items are evicted automatically.
        ///
        /// Replaces the manual dependency tracking that was previously coupled to
        /// SqlCacheDependency / HttpRuntime.Cache dependency callbacks.
        /// </summary>
        /// <param name="depType">The dependency type discriminator.</param>
        /// <param name="depId">The dependency identifier (e.g. a user ID).</param>
        /// <param name="itemType">The cache-item type discriminator.</param>
        /// <param name="itemId">The cache-item identifier.</param>
        public void AddDependency(DependencyType depType, string depId, CacheItemType itemType, string itemId)
        {
            string cacheKey = CreateCacheItemKey(itemType, itemId);
            string dependencyKey = CreateDependencyKey(depType, depId);

            lock (_lock)
            {
                if (!_depMap.ContainsKey(dependencyKey))
                {
                    _depMap[dependencyKey] = new HashSet<string>();
                }
                _depMap[dependencyKey].Add(cacheKey);
            }
        }

        /// <summary>
        /// Legacy alias for <see cref="AddDependency"/> preserved for backward compatibility
        /// with call sites that used the original snake_case naming convention.
        /// </summary>
        public void add_dependency(DependencyType depType, string depId, CacheItemType itemType, string itemId)
            => AddDependency(depType, depId, itemType, itemId);

        /// <summary>
        /// Invalidates all cache items that are registered under the specified dependency,
        /// then removes the dependency entry itself.
        /// Replaces: HttpRuntime.Cache.Remove(cachekey) inside the dependency loop.
        /// </summary>
        /// <param name="type">The dependency type discriminator.</param>
        /// <param name="key">The dependency identifier whose associated cache items should be evicted.</param>
        public void InvalidateDependency(DependencyType type, string key)
        {
            string dependencyKey = CreateDependencyKey(type, key);

            HashSet<string>? cacheKeys;

            lock (_lock)
            {
                if (!_depMap.TryGetValue(dependencyKey, out cacheKeys))
                {
                    return;
                }
                // Remove the dependency entry before evicting cache items to prevent
                // re-entrant invalidation issues.
                _depMap.Remove(dependencyKey);
            }

            // Evict all cache items that were registered under this dependency.
            // IMemoryCache.Remove is thread-safe; no lock needed here.
            foreach (string cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
        }

        /// <summary>
        /// Legacy alias for <see cref="InvalidateDependency"/> preserved for backward compatibility
        /// with call sites that used the original snake_case naming convention.
        /// </summary>
        public void invalidate_dependency(DependencyType type, string key)
            => InvalidateDependency(type, key);
    }
}
