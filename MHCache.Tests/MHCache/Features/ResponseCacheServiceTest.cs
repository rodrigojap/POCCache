using System.Threading.Tasks;
using MHCache.Features;
using MHCache.Tests.Moqs.MHCache;
using Xunit;

namespace MHCache.Tests.MHCache.Features
{
    public class ResponseCacheServiceTest
    { 
        private ConnectionMultiplexerMock ConnectionMultiplexerMock { get; set; }
        private IResponseCacheService ResponseCacheService { get; set; }

        public ResponseCacheServiceTest()
        {
            ConnectionMultiplexerMock = new ConnectionMultiplexerMock();

            ResponseCacheService = new ResponseCacheService(ConnectionMultiplexerMock.Object);
        }

        [Fact]
        public async Task When_KeyCreatedAndAskExist_Then_ReturnTrue_Test() 
        {
            string cachedKey = "chaveTeste";
            await ResponseCacheService.SetCacheResponseAsync(cachedKey, "valorTeste", null);
            Assert.True(await ResponseCacheService.ContainsKeyAsync(cachedKey));
        }


        


    }
}
