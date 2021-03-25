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
        public async Task When_KeyAndValueCreated_Then_CompareValueAndReturnTrue_Test()
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



        [InlineData("key1,key2,key3,_key1,_key2,_key3", "key1", "key1")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "*key2", "key2,_key2")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "key*", "key1,key2,key3")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "*key*", "key1,key2,key3,_key1,_key2,_key3")]
        [Theory]
        public async Task When_KeySearchPattern_Then_ReturnSearchedValues_Test(string keys, string pattern, string expectedFoundValues)
        {
            //Arrange            
            string cacheValueToCreate = "valorTeste";
            foreach (var item in keys.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                await ResponseCacheService.SetCacheResponseAsync(item, cacheValueToCreate, null);
            }            

            //Act                        
            var foundValues = ResponseCacheService.GetKeysByPattern(pattern);

            //Assert            
            Assert.Equal(expectedFoundValues, string.Join(",", foundValues));
        }


        [InlineData("key1,key2,key3,_key1,_key2,_key3", "key1", "key2,key3,_key1,_key2,_key3")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "key2,key3", "key1,_key1,_key2,_key3")]
        [Theory]
        public async Task When_Remove_Then_ReturnSearchedValues_Test(string keys, string strValues, string expectedFoundValues)
        {
            //Arrange            
            string cacheValueToCreate = "valorTeste";
            foreach (var item in keys.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                await ResponseCacheService.SetCacheResponseAsync(item, cacheValueToCreate, null);
            }
            var removeParams = strValues.Split(",", StringSplitOptions.RemoveEmptyEntries);

            //Act                        
            var removedCount = await ResponseCacheService.RemoveCachedResponseByNamesAsync(removeParams);
            var remainedKeys = ResponseCacheService.GetKeysByPattern("*key*");

            //Assert    
            Assert.Equal(removeParams.Length, removedCount);
            Assert.Equal(expectedFoundValues, string.Join(",", remainedKeys));
        }

    }
}
