using System;
using System.Collections.Generic;
using System.Linq;
using MHCache.Features;
using Moq;

namespace MHCache.Tests.Moqs.MHCache
{
    public class ResponseCacheServiceMock : Mock<IResponseCacheService>
    {

        #region Properties

        private List<Tuple<string, string, TimeSpan?>> CachedValues { get; set; } = new List<Tuple<string, string, TimeSpan?>>();
        
        #endregion

        #region Task<bool> ContainsKeyAsync(string cacheKey);

        public void Set_ContainsKey(bool result)
        {
            Setup(service => 
                service.ContainsKeyAsync(It.IsAny<string>())
            )
            .ReturnsAsync(result);
        }
        
        public void Throw_ContainsKey(Exception ex)
        {
            Setup(service => 
                service.ContainsKeyAsync(It.IsAny<string>())
            )
            .Throws(ex);
        }

        #endregion

        #region Task<bool> SetCacheResponseAsync(string cacheKey, string value, TimeSpan? timeLive);

        public void Set_SetCacheResponseAsync(bool result)
        {
            Setup(service => 
                service.SetCacheResponseAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())            
            )
            .Callback<string,string, TimeSpan?>(
                (cacheKey, value, timeLive) 
                    => CachedValues.Add( new Tuple<string, string, TimeSpan?>(cacheKey, value, timeLive))
            )
            .ReturnsAsync(result);
        }

        public void Throw_SetCacheResponseAsync(Exception ex)
        {
            Setup(service => 
                service.SetCacheResponseAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())
            )
            .Throws(ex);
        }

        #endregion

        #region Task<string> GetCachedResponseAsStringAsync(string cacheKey);

        public void Set_GetCachedResponseAsStringAsync()
        {
            Tuple<string, string, TimeSpan?> resultValue = null;

            Setup(service =>
                service.GetCachedResponseAsStringAsync(It.IsAny<string>())
            )
            .Callback<string>((cachedKey) =>
            {

                resultValue = CachedValues.FirstOrDefault(o => o.Item1 == cachedKey);
            })
            .ReturnsAsync(() => resultValue.Item2);
        }

        public void Throw_GetCachedResponseAsStringAsync(Exception ex)
        {
            Setup(service =>
                service.GetCachedResponseAsStringAsync(It.IsAny<string>())
            )
            .Throws(ex);
        }

        #endregion

        #region IEnumerable<string> GetKeysByPattern(string pattern, int pageSize = 250, int pageOffset = 0);

        public void Set_GetKeysByPattern()
        {
            IEnumerable<string> resultValue = null;

            Setup(service =>
                service.GetKeysByPattern(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())
            )
            .Callback<string, int, int>((cachedKey, pageSize, pageOffset) =>
            {
                resultValue = CachedValues
                                .Where(o => o.Item1.Contains(cachedKey))
                                .Select(o => o.Item1);
            })
            .Returns(() => resultValue);
        }

        public void Throw_GetKeysByPattern(Exception ex)
        {
            Setup(service =>
                service.GetKeysByPattern(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())
            )
            .Throws(ex);
        }


        #endregion

        #region Task<bool> RemoveCachedResponseByNameAsync(string cacheKey);

        public void Set_RemoveCachedResponseByNameAsync()
        {
            Tuple<string, string, TimeSpan?> resultValue = null;

            Setup(service =>
                service.RemoveCachedResponseByNameAsync(It.IsAny<string>())
            )
            .Callback<string>((cachedKey) =>
            {
                resultValue = CachedValues
                                .FirstOrDefault(o => o.Item1 == cachedKey);

                CachedValues.Remove(resultValue);
            })
            .ReturnsAsync(() => resultValue != null);
        }

        public void Throw_RemoveCachedResponseByNameAsync(Exception ex)
        {
            Setup(service =>
                service.GetKeysByPattern(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())
            )
            .Throws(ex);
        }

        #endregion

        #region Task<long> RemoveAllByPatternAsync(string pattern);

        public void Set_RemoveAllByPatternAsync()
        {
            long resultValue = 0;

            Setup(service =>
                service.RemoveAllByPatternAsync(It.IsAny<string>())
            )
            .Callback<string>((cachedKey) =>
            {
                resultValue = CachedValues
                                .Count(o => o.Item1.Contains(cachedKey));

                CachedValues.RemoveAll(o => o.Item1.Contains(cachedKey));
            })
            .ReturnsAsync(() => resultValue);
        }

        public void Throw_RemoveAllByPatternAsync(Exception ex)
        {
            Setup(service =>
                service.RemoveAllByPatternAsync(It.IsAny<string>())
            )
            .Throws(ex);
        }


        #endregion
    }
}
