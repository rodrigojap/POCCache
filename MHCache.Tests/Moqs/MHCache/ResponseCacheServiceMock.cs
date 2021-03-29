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

        public ResponseCacheServiceMock()
        {
            Set_ContainsKey();
            Set_SetCacheResponseAsync();
            Set_GetCachedResponseAsStringAsync();
            Set_GetKeysByPattern();
        }

        #region Task<bool> ContainsKeyAsync(string cacheKey);

        public void Set_ContainsKey()
        {
            bool result = false;
            Setup(service => 
                service.ContainsKeyAsync(It.IsAny<string>())
            )
            .Callback<string>(key => result = CachedValues.Any(x => x.Item1 == key))
            .ReturnsAsync(() => result);
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

        public void Verify_SetCacheResponseAsync(Times times)
        {
            Verify(service =>
                    service.SetCacheResponseAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), times);
        }

        public void Set_SetCacheResponseAsync()
        {
            Setup(service => 
                service.SetCacheResponseAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>())            
            )
            .Callback<string,string, TimeSpan?>(
                (cacheKey, value, timeLive) 
                    => CachedValues.Add( new Tuple<string, string, TimeSpan?>(cacheKey, value, timeLive))
            )
            .ReturnsAsync(true);
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

        public void Verify_GetCachedResponseAsStringAsync(Times times) 
        {
            Verify(
                service =>
                    service.GetCachedResponseAsStringAsync(It.IsAny<string>()), 
                times
            );
        }

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
            .ReturnsAsync(() => resultValue?.Item2);
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

        public void Verify_GetKeysByPattern(Times times, string pattern) 
        {
            Verify(
                    service =>
                        service
                            .GetKeysByPattern( 
                                It.Is<string>(p => p == pattern),
                                It.IsAny<int>(),
                                It.IsAny<int>()
                            ),
                    times
                );
        }

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

        public void Set_RemoveCachedResponseByNameAsync(Times times)
        {
            Verify(
                service =>
                    service.RemoveCachedResponseByNamesAsync(It.IsAny<string[]>()),
                times
            );
        }

        public void Set_RemoveCachedResponseByNameAsync()
        {
            IEnumerable<string> foundKeys = null;
            long result = 0;

            Setup(service =>
                service.RemoveCachedResponseByNamesAsync(It.IsAny<string[]>())
            )
            .Callback<string[]>((cachedKeys) =>
            {
                foundKeys = CachedValues
                                .Where(o => cachedKeys.Contains(o.Item1))
                                .Select(o => o.Item1)
                                .ToArray();

                result = CachedValues.RemoveAll(o => foundKeys.Contains(o.Item1));
            })
            .ReturnsAsync(() => result);
        }

        public void Throw_RemoveCachedResponseByNameAsync(Exception ex)
        {
            Setup(service =>
                service.GetKeysByPattern(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())
            )
            .Throws(ex);
        }

        #endregion

        
    }
}
