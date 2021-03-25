namespace MHCache.AspNetCore.Filters.AOP.DataModel
{
    public class MethodCachedConfiguration
    {
        public string CachedMethodName { get; set; }

        public int? TimeToLiveSeconds { get; set; }
    }
}