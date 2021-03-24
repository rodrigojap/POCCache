namespace MHCache.AspNetCore.Filters.AOP.DataModel
{
    public class RouteCacheConfiguration
    {
        public string CachedRouteName { get; set; }

        public int? TimeToLiveSeconds { get; set; }
    }
}