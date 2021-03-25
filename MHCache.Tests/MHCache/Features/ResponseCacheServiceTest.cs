using System;
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

        //When_<Cenario>_Then_<Result>_Test
        //Should_<Result>_When_<Cenario>_Test
        [Fact]
        public async Task When_KeyCreatedAndAskExist_Then_ReturnTrue_Test() 
        {
            //Arrange
            string cachedKey = "chaveTeste";
            await ResponseCacheService.SetCacheResponseAsync(cachedKey, "valorTeste", null);

            //Act
            var result = await ResponseCacheService.ContainsKeyAsync(cachedKey);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task When_AskExistThrow_Then_ReturnThrow_Test()
        {
            //Arrange
            var expectedEx = new Exception("Ocorreu um erro");
            ConnectionMultiplexerMock.DataBaseMock.Throw_KeyExistsAsync(expectedEx);
            string cachedKey = "chaveTeste";
            await ResponseCacheService.SetCacheResponseAsync(cachedKey, "valorTeste", null);

            //Act
            var exResult = await Assert
                            .ThrowsAsync<Exception>(
                                () => ResponseCacheService.ContainsKeyAsync(cachedKey)
                            );
          
            //Assert
            Assert.Equal(expectedEx.Message, exResult.Message);
        }
        
        [Fact]
        public async Task When_SetThrow_Then_ReturnThrow_Test()
        {
            //Arrange
            var expectedEx = new Exception("Ocorreu um erro");
            ConnectionMultiplexerMock.DataBaseMock.Throw_SetCacheResponseAsync(expectedEx);            
            string cachedKey = "chaveTeste";            

            //Act
            var exResult = await Assert
                            .ThrowsAsync<Exception>(
                                () => ResponseCacheService.SetCacheResponseAsync(cachedKey, "valorTeste", null)
                            );

            //Assert
            Assert.Equal(expectedEx.Message, exResult.Message);
        }
        
        [Fact]
        public async Task When_KeyCreated_Then_CompareAndReturnTrue_Test()
        {
            //Arrange            
            string cacheKeyToCreate = "chaveTeste";
            string cacheValueToCreate = "valorTeste";
            await ResponseCacheService.SetCacheResponseAsync(cacheKeyToCreate, cacheValueToCreate, null);

            //Act                        
            string createdValue = await ResponseCacheService.GetCachedResponseAsStringAsync(cacheKeyToCreate);
            var comparisonValue = createdValue.Equals(cacheValueToCreate, StringComparison.InvariantCultureIgnoreCase);

            //Assert            
            Assert.True(comparisonValue);
        }        
    }
}
