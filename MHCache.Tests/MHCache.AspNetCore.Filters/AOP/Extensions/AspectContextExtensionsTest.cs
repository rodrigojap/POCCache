using MHCache.Tests.Moqs.MHCache.AspNetCore.Filters;
using Xunit;
using MHCache.AspNetCore.Filters.AOP.Extensions;
using System.Text.Json;
using System.Collections.Generic;

namespace MHCache.Tests.MHCache.AspNetCore.Filters.AOP.Extensions
{
    public class AspectContextExtensionsTest
    {
        public AspectContextMock AspectContextMock { get; set; }

        public AspectContextExtensionsTest()
        {
            AspectContextMock = new AspectContextMock();
        }

        [InlineData("Product.Project.Class", "Method")]
        [Theory]
        public void When_GetGenerateKeyByMethodNameAndValuesWWithoutParams_Then_GetKeyAsJson_Test(string fullTypeName, string methodName)
        {

            AspectContextMock.Set_PathMethodName(fullTypeName, methodName);

            var generateKey = AspectContextMock.Object.GetGenerateKeyByMethodNameAndValues();

            Assert.Equal($"{{\"Method\":\"{fullTypeName}.{methodName}\",\"Params\":[]}}", generateKey);
        }

       


    }

    
}
