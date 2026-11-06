// This file has been superseded by the new CacheHelper in ioschools.Caching project.
// The legacy implementation used System.Web.Caching.HttpRuntime.Cache and SqlCacheDependency
// which are not available in .NET 8.
//
// Migration:
//   - HttpRuntime.Cache.Get(key)    → IMemoryCache.TryGetValue(key, out value)
//   - HttpRuntime.Cache.Insert(key) → IMemoryCache.Set(key, value, options)
//   - SqlCacheDependency            → Time-based expiration via MemoryCacheEntryOptions
//   - DataContext.GetCommand(q)     → IQueryable.ToQueryString() (EF Core 5+)
//   - SqlCacheDependencyAdmin       → Removed (no equivalent in .NET 8)
//
// See: ioschools.Caching/CacheHelper.cs for the .NET 8 implementation.
