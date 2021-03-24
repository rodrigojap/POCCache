namespace MHCache.AspNetCore.Filters.AOP.DataModel
{
    public class MethodCacheRemoveConfiguration
    {
        public string CachedMethodName { get; set; }

        public string PatternMethodCachedName { get; set; }
    }
}
