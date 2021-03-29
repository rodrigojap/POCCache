using System;
using System.Threading.Tasks;
using MHCache.Extensions;
using MHCache.Features;
using MHCache.Tests.Moqs.MHCache;
using Xunit;

namespace MHCache.Tests.MHCache.Extensions
{
    public class ResponseCacheServiceExtensionsTest
    {
        private ConnectionMultiplexerMock ConnectionMultiplexerMock { get; set; }
        private IResponseCacheService ResponseCacheService { get; set; }

        public ResponseCacheServiceExtensionsTest()
        {
            ConnectionMultiplexerMock = new ConnectionMultiplexerMock();

            ResponseCacheService = new ResponseCacheService(ConnectionMultiplexerMock.Object);
        }

        //When_<Cenario>_Then_<Result>_Test
        //Should_<Result>_When_<Cenario>_Test
        [Fact]
        public async Task When_SetObjectInCacheWithKey_Then_PersistInRedis_Test()
        {
            //Arrange
            string cachedKey = "chaveTeste";
            var expectedValue = new ClasseTeste();
            var resultSet = await ResponseCacheService.SetCacheResponseAsync(cachedKey, expectedValue, null);

            //Act
            var result = await ResponseCacheService.GetCachedResponseAsync<ClasseTeste>(cachedKey);


            //Assert
            Assert.True(resultSet);
            Assert.True(expectedValue.Equals(result));
        }

        [Fact]
        public async Task When_SetNullObjectInCache_Then_NotPersistInRedis_Test()
        {
            //Arrange
            string cachedKey = "chaveTeste";

            //Assert
            Assert.False(await ResponseCacheService.SetCacheResponseAsync<ClasseTeste>(cachedKey, null, null));
            Assert.False(await ResponseCacheService.SetCacheResponseAsync<ClasseTeste>(null, null));
        }

        [Fact]
        public async Task When_GetNotFoundKey_Then_ReturnNull_Test()
        {
            //Assert
            Assert.Null(await ResponseCacheService.GetCachedResponseAsync<ClasseTeste>("teste"));
            Assert.Null(await ResponseCacheService.GetCachedResponseAsync<ClasseTeste>());
            Assert.Null(await ResponseCacheService.GetCachedResponseAsync("teste", typeof(ClasseTeste)));
        }

        [Fact]
        public async Task When_SetObjectInCacheWithoutKey_Then_PersistInRedisWithFullNameClass_Test()
        {
            //Arrange
            var expectedValue = new ClasseTeste();
            await ResponseCacheService.SetCacheResponseAsync(expectedValue, null);

            //Act
            var result = await ResponseCacheService.GetCachedResponseAsync<ClasseTeste>();


            //Assert
            Assert.True(expectedValue.Equals(result));
            Assert.True(await ResponseCacheService.ContainsKeyAsync(typeof(ClasseTeste).FullName));
        }


        [Fact]
        public async Task When_GetObjectInCacheWithType_Then_ReturnTypeAsObject_Test()
        {
            //Arrange
            var expectedValue = new ClasseTeste();
            await ResponseCacheService.SetCacheResponseAsync(expectedValue, null);

            //Act
            var result = await ResponseCacheService.GetCachedResponseAsync(typeof(ClasseTeste).FullName, typeof(ClasseTeste));


            //Assert
            Assert.True(expectedValue.Equals(result));
            Assert.True(await ResponseCacheService.ContainsKeyAsync(typeof(ClasseTeste).FullName));
        }

        [InlineData("key1,key2,key3,_key1,_key2,_key3", 3, "key1,key2,key3")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", 5, "key1,key2,key3,_key1,_key2")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", 10,"key1,key2,key3,_key1,_key2,_key3")]
        [Theory]
        public async Task When_GetAllKeys_Then_ReturnPagedKeys_Test(string keys, int pageSize , string expectedValue)
        {
            //Arrange
            foreach (var item in keys.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                await ResponseCacheService.SetCacheResponseAsync(item, "teste", null);
            }

            //Act
            var result = ResponseCacheService.GetAllKeys(pageSize,0);


            //Assert
            Assert.Equal(expectedValue,string.Join(",",result));
        }


        [InlineData("key1,key2,key3,_key1,_key2,_key3", "key1" , "key2,key3,_key1,_key2,_key3")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "_key*", "key1,key2,key3")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "*key1", "key2,key3,_key2,_key3")]
        [InlineData("key1,key2,key3,_key1,_key2,_key3", "*key*", "")]
        [Theory]
        public async Task When_RemoveKeysWithPattern_Then_ExcludeAllKeysWithPattern_Test(string keys, string pattern, string expectedRemainingKeys)
        {
            //Arrange
            foreach (var item in keys.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                await ResponseCacheService.SetCacheResponseAsync(item, "teste", null);
            }

            //Act
            var countRemoved = await ResponseCacheService.RemoveAllByPatternAsync(pattern);
            var remainingKeys = string.Join(",",ResponseCacheService.GetAllKeys());

            //Assert
            Assert.True(expectedRemainingKeys.Equals(remainingKeys));
        }


    }

    public class ClasseTeste 
    {
        public int Prop1 { get; set; } = 2;
        public string Prop2 { get; set; } = "Teste";

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (ClasseTeste)obj;
            return Prop1 == other.Prop1 || Prop2 == other.Prop2;
        }      
    }
}
