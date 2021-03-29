using System.Collections.Generic;
using MHCache.AspNetCore.Filters.AOP.DataModel;
using MHCache.AspNetCore.Filters.AOP.Extensions;
using MHCache.Tests.Moqs.MHCache.AspNetCore.Filters;
using Xunit;

namespace MHCache.Tests.MHCache.AspNetCore.Filters.AOP.Extensions
{
    public class AspectContextByConfigurationExtensionsTest
    {
        public AspectContextMock AspectContextMock { get; set; }
        
        public AspectContextByConfigurationExtensionsTest()
        {
            AspectContextMock = new AspectContextMock();
        }

        private FilterCachedConfiguration GetFilterCachedConfiguration(string methodName)
            => new FilterCachedConfiguration()
            {
                CachedMethods = new List<MethodCachedConfiguration>()
                {
                    new MethodCachedConfiguration() { CachedMethodName = methodName }
                }
            };

        private FilterCachedConfiguration GetFilterCachedRemoveConfiguration(string methodName)
            => new FilterCachedConfiguration()
            {
                CachedRemoveMethods = new List<MethodCachedRemoveConfiguration>()
                {
                    new MethodCachedRemoveConfiguration() { CachedMethodName = methodName }
                }
            };

        [InlineData("Product.Project.Class", "Method", "method" )]
        [InlineData("Product.Project.Class", "Method", "METHOD")]
        [InlineData("Product.Project.Class", "Method", "Method")]
        [InlineData("Product.Project.Class", "Method", "Class.Method")]
        [InlineData("Product.Project.Class", "Method", "Product.Project.Class.method")]
        [Theory]
        public void When_AnyConfigCachedMethodNameContainsInFullMethodPath_Then_ReturnFirstItemConfig_Test(string fullTypeName, string methodName, string configMethodName) 
        {
            AspectContextMock.Set_PathMethodName(fullTypeName, methodName);

            var cacheConfiguration = AspectContextMock
                        .Object
                        .GetCacheConfigurationByMethodName(
                            GetFilterCachedConfiguration(configMethodName)
                        );

            Assert.NotNull(cacheConfiguration);
            Assert.Equal(configMethodName, cacheConfiguration.CachedMethodName);
        }

        [InlineData("Product.Project.Class", "Method", "Teste")]
        [Theory]
        public void When_AllConfigCachedMethodNameNotContainsInFullMethodPath_Then_ReturnNull_Test(string fullTypeName, string methodName, string configMethodName)
        {
            AspectContextMock.Set_PathMethodName(fullTypeName, methodName);

            var cacheConfiguration = AspectContextMock
                        .Object
                        .GetCacheConfigurationByMethodName(
                            GetFilterCachedConfiguration(configMethodName)
                        );

            Assert.Null(cacheConfiguration);

        }


        [InlineData("Product.Project.Class", "Method", "method")]
        [InlineData("Product.Project.Class", "Method", "METHOD")]
        [InlineData("Product.Project.Class", "Method", "Method")]
        [InlineData("Product.Project.Class", "Method", "Class.Method")]
        [InlineData("Product.Project.Class", "Method", "Product.Project.Class.method")]
        [Theory]
        public void When_AnyConfigCachedRemoveMethodNameContainsInFullMethodPath_Then_ReturnFirstItemConfig_Test(string fullTypeName, string methodName, string configMethodName)
        {
            AspectContextMock.Set_PathMethodName(fullTypeName, methodName);

            var cacheConfiguration = AspectContextMock
                        .Object
                        .GetCacheRemoveConfigurationByMethodName(
                            GetFilterCachedRemoveConfiguration(configMethodName)
                        );

            Assert.NotNull(cacheConfiguration);
            Assert.Equal(configMethodName, cacheConfiguration.CachedMethodName);
        }

        [InlineData("Product.Project.Class", "Method", "Teste")]
        [Theory]
        public void When_AllConfigCachedRemoveMethodNameNotContainsInFullMethodPath_Then_ReturnNull_Test(string fullTypeName, string methodName, string configMethodName)
        {
            AspectContextMock.Set_PathMethodName(fullTypeName, methodName);

            var cacheConfiguration = AspectContextMock
                        .Object
                        .GetCacheRemoveConfigurationByMethodName(
                            GetFilterCachedRemoveConfiguration(configMethodName)
                        );

            Assert.Null(cacheConfiguration);

        }

    }
}
