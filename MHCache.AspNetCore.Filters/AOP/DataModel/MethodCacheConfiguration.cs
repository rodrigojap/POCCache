namespace MHCache.AspNetCore.Filters.AOP.DataModel
{
    public class MethodCacheConfiguration
    {
        public string CachedMethodName { get; set; }

        public int? TimeToLiveSeconds { get; set; }
    }
}