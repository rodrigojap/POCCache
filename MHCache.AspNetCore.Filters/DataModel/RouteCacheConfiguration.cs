namespace MHCache.AspNetCore.Filters.DataModel
{
    public class RouteCacheConfiguration
    {
        public string CachedRouteName { get; set; }

        public int? TimeToLiveSeconds { get; set; }
    }
}