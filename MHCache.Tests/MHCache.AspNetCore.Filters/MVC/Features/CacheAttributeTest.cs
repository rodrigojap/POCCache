using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHCache.AspNetCore.Filters.AOP.Extensions;
using MHCache.Tests.Moqs.MHCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using MHCache.Extensions;
using System.Text.Json;

namespace MHCache.Tests.MHCache.AspNetCore.Filters.MVC.Features
{
    public class CacheAttributeTest
    {
        #region Properties

        private ResponseCacheServiceMock ResponseCacheServiceMock { get; }

        private CachedAttribute CachedAttribute { get; set; }

        public ActionExecutingContext Context { get; set; }
        
        public HttpContext HttpContext { get; set; }

        private Mock<ActionExecutionDelegate> ActionExecutionDelegateMock { get; set; }


        #endregion

        public CacheAttributeTest()
        {
            HttpContext = new DefaultHttpContext();
            Context = new ActionExecutingContext(
                new ActionContext(HttpContext, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<ControllerBase>().Object);

            ResponseCacheServiceMock = new ResponseCacheServiceMock();
            CachedAttribute = new CachedAttribute(ResponseCacheServiceMock.Object);

            ActionExecutionDelegateMock = new Mock<ActionExecutionDelegate>();
            
        }

        private ActionExecutedContext SetActionExecutedContext(object resultObj) 
        {
            var action = new ActionExecutedContext(
                            new ActionContext(HttpContext, new RouteData(), new ActionDescriptor()),
                            new List<IFilterMetadata>(),
                            new Mock<ControllerBase>().Object
                        );

            action.Result = new OkObjectResult(resultObj);
            return action;
        }

        private string BuildValuesFromQueryString(HttpContext httpContext) 
        {
            return string.Join("|", httpContext.Request.Query.OrderBy(x => x.Key).Select(o => $"{o.Key}-{o.Value}"));
        }

        [InlineData("GET", "/Default", "?teste=1")]
        [Theory]
        public async Task When_FilterCalledWithoutCached_Then_SetObjectCached_Test(string method, string path, string queryString ) 
        {
            var expectedValue = new { prop1 = "teste" };
            ActionExecutionDelegateMock
                .Setup(d => d.Invoke())
                .ReturnsAsync(
                    SetActionExecutedContext(expectedValue)
                );

            HttpContext.Request.Method = method;
            HttpContext.Request.Path = path;
            HttpContext.Request.QueryString = new QueryString(queryString);

            //Act
            await CachedAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);
            
            ResponseCacheServiceMock.Verify_GetCachedResponseAsStringAsync( Times.Once());
            ResponseCacheServiceMock.Verify_SetCacheResponseAsync(Times.Once());
            var storedValue = await ResponseCacheServiceMock.Object.GetCachedResponseAsStringAsync($"{path}|{BuildValuesFromQueryString(HttpContext)}");
            Assert.Equal(JsonSerializer.Serialize(expectedValue), storedValue);
        }


        [InlineData("GET", "/Default", "?teste=1")]
        [Theory]
        public async Task When_FilterCalledWithCached_Then_GetObjectCached_Test(string method, string path, string queryString)
        {
            var expectedValue = new { prop1 = "teste" };
            ActionExecutionDelegateMock
                .Setup(d => d.Invoke())
                .ReturnsAsync(
                    SetActionExecutedContext(expectedValue)
                );

            HttpContext.Request.Method = method;
            HttpContext.Request.Path = path;
            HttpContext.Request.QueryString = new QueryString(queryString);

            await ResponseCacheServiceMock.Object.SetCacheResponseAsync($"{path}|{BuildValuesFromQueryString(HttpContext)}", JsonSerializer.Serialize(expectedValue), null);

            //Act
            await CachedAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);

            ResponseCacheServiceMock.Verify_GetCachedResponseAsStringAsync(Times.Once());

            ResponseCacheServiceMock.Verify_SetCacheResponseAsync(Times.Once());
        }



    }
}
