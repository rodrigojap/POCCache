using MHCache.Services;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RedisCacheTESTS
{
    public class CacheReponseServiceTest
    {


        public CacheReponseServiceTest()
        {            
        }

        [Fact]
        public void ResponseCache_SetCache_MustBeCached()
        {
            var mocker = new AutoMocker();

            //já injeta dependencias
            var serviceMock = mocker.CreateInstance<ResponseCacheService>();

            //mocker.GetMock<IResponseCacheService>().Setup();
        }
    }
}
