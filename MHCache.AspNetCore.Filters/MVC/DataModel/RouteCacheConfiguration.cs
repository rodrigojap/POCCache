namespace MHCache.AspNetCore.Filters.MVC.DataModel
{
    public class RouteCacheConfiguration
    {
        public string CachedRouteName { get; set; }

        public int? TimeToLiveSeconds { get; set; }
    }
}