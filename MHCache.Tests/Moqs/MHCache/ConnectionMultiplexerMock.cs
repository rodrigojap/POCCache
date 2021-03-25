using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Moq;
using StackExchange.Redis;

namespace MHCache.Tests.Moqs.MHCache
{
    public class ConnectionMultiplexerMock : Mock<IConnectionMultiplexer>
    {
        #region Properties

        private List<Tuple<string, string, TimeSpan?>> CachedValues { get; set; } = new List<Tuple<string, string, TimeSpan?>>();

        public DataBaseMock DataBaseMock { get; set; }

        public ServerMock ServerMock { get; set; }

        #endregion


        public ConnectionMultiplexerMock()
        {
            DataBaseMock = new DataBaseMock(CachedValues);

            ServerMock = new ServerMock(CachedValues);

            Setup(o => o.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(DataBaseMock.Object);

            Setup(o => o.GetServer(It.IsAny<EndPoint>(), It.IsAny<object>()))
                .Returns(ServerMock.Object);

        }
    }

    public class DataBaseMock : Mock<IDatabase> 
    {
        #region Properties

        private List<Tuple<string, string, TimeSpan?>> CachedValues { get; set; }

        #endregion

        public DataBaseMock(List<Tuple<string, string, TimeSpan?>> cachedValues)
        {
            CachedValues = cachedValues;
            Set_KeyExistsAsync();
            Set_SetCacheResponseAsync();
            Set_GetCachedResponseAsStringAsync();
        }

        #region Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags);

        public void Set_KeyExistsAsync()
        {
            var result = false;
            Setup(service =>
                service.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())
            )
            .Callback<RedisKey, CommandFlags>((key, flags) => result = CachedValues.Any(o => o.Item1 == key))
            .ReturnsAsync(() => result);
        }

        public void Throw_KeyExistsAsync(Exception ex)
        {
            Setup(service =>
                service.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())
            )
            .ThrowsAsync(ex);
        }

        #endregion
        
        #region Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None);

        public void Set_SetCacheResponseAsync()
        {
            Setup(service =>
                service.StringSetAsync(
                                        It.IsAny<RedisKey>(), 
                                        It.IsAny<RedisValue>(), 
                                        It.IsAny<TimeSpan?>(),
                                        It.IsAny<When>(),
                                        It.IsAny<CommandFlags>()
                                      )
            )
            .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>(
                (cacheKey, value, timeLive, when, flags)
                    => CachedValues.Add(new Tuple<string, string, TimeSpan?>(cacheKey, value, timeLive))
            )
            .ReturnsAsync(true);
        }

        public void Throw_SetCacheResponseAsync(Exception ex)
        {
            Setup(service =>
                service.StringSetAsync(
                                        It.IsAny<RedisKey>(),
                                        It.IsAny<RedisValue>(),
                                        It.IsAny<TimeSpan?>(),
                                        It.IsAny<When>(),
                                        It.IsAny<CommandFlags>()
                                      )
            )
            .Throws(ex);
        }

        #endregion

        #region Task<bool> StringGetAsync(RedisKey key, CommandFlags flags);

        public void Set_GetCachedResponseAsStringAsync()
        {
            Tuple<string, string, TimeSpan?> resultValue = null;

            Setup(service =>
                service.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())
            )
            .Callback<RedisKey, CommandFlags>((cachedKey, flags) =>
            {
                resultValue = CachedValues.FirstOrDefault(o => o.Item1 == cachedKey);
            })
            .ReturnsAsync(() => resultValue.Item2);
        }

        public void Throw_GetCachedResponseAsStringAsync(Exception ex)
        {
            Setup(service =>
                service.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())
            )
            .Throws(ex);
        }

        #endregion

    }

    public class ServerMock : Mock<IServer> 
    {
        #region Properties

        private List<Tuple<string, string, TimeSpan?>> CachedValues { get; set; }

        #endregion

        public ServerMock(List<Tuple<string, string, TimeSpan?>> cachedValues)
        {
            CachedValues = cachedValues;
        }
        
    }
}
