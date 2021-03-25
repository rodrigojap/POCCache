namespace MHCache.AspNetCore.Filters.MVC.DataModel
{
    public class RouteCachedConfiguration
    {
        public string CachedRouteName { get; set; }

        public int? TimeToLiveSeconds { get; set; }
    }
}