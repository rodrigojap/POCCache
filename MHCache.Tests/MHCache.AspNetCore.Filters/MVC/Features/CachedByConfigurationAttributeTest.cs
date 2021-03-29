using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MHCache.AspNetCore.Filters.MVC.DataModel;
using MHCache.AspNetCore.Filters.MVC.Extensions;
using MHCache.Tests.Moqs.MHCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MHCache.Tests.MHCache.AspNetCore.Filters.MVC.Features
{
    public class CachedByConfigurationAttributeTest
    {
        #region Properties

        private ResponseCacheServiceMock ResponseCacheServiceMock { get; }

        private CachedByConfigurationAttribute CachedByConfigurationAttribute { get; set; }

        public ActionExecutingContext Context { get; set; }
        
        public HttpContext HttpContext { get; set; }

        private Mock<ActionExecutionDelegate> ActionExecutionDelegateMock { get; set; }


        private Mock<IOptionsMonitor<FilterCachedConfiguration>> Configuration { get; set; }

        #endregion

        public CachedByConfigurationAttributeTest()
        {
            Configuration = new Mock<IOptionsMonitor<FilterCachedConfiguration>>();
            
            HttpContext = new DefaultHttpContext();
            Context = new ActionExecutingContext(
                new ActionContext(HttpContext, new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<ControllerBase>().Object);

            ResponseCacheServiceMock = new ResponseCacheServiceMock();
            
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

        private FilterCachedConfiguration GetConfiguration(IEnumerable<string> routesName)
            => new FilterCachedConfiguration()
               {
                   GeneralTimeToLiveSeconds = 60,
                   CachedRoutes = routesName.Select(routeName => new RouteCachedConfiguration { CachedRouteName = routeName, TimeToLiveSeconds = 10 })
               };

        [InlineData("GET", "/Default", "?teste=1")]
        [Theory]
        public async Task When_FilterCalledWithoutCached_Then_SetObjectCached_Test(string method, string path, string queryString ) 
        {
            Configuration
                .SetupGet(d => d.CurrentValue).Returns(GetConfiguration("Default".Split(",")));

            CachedByConfigurationAttribute = new CachedByConfigurationAttribute(
                                                                                    ResponseCacheServiceMock.Object,
                                                                                    Configuration.Object
                                                                               );

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
            await CachedByConfigurationAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);
            
            ResponseCacheServiceMock.Verify_GetCachedResponseAsStringAsync( Times.Once());
            ResponseCacheServiceMock.Verify_SetCacheResponseAsync(Times.Once());
            var storedValue = await ResponseCacheServiceMock.Object.GetCachedResponseAsStringAsync($"{path}|{BuildValuesFromQueryString(HttpContext)}");
            Assert.Equal(JsonSerializer.Serialize(expectedValue), storedValue);
        }

        [InlineData("GET", "/Default", "?teste=1")]
        [Theory]
        public async Task When_FilterCalledWithoutCached2_Then_SetObjectCached_Test(string method, string path, string queryString)
        {
            Configuration
                .SetupGet(d => d.CurrentValue).Returns(GetConfiguration("Default".Split(",")));

            CachedByConfigurationAttribute = new CachedByConfigurationAttribute(
                                                                                    ResponseCacheServiceMock.Object,
                                                                                    Configuration.Object
                                                                               );

            object expectedValue = null;
            ActionExecutionDelegateMock
                .Setup(d => d.Invoke())
                .ReturnsAsync(
                    SetActionExecutedContext(expectedValue)
                );

            HttpContext.Request.Method = method;
            HttpContext.Request.Path = path;
            HttpContext.Request.QueryString = new QueryString(queryString);

            //Act
            await CachedByConfigurationAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);

            ResponseCacheServiceMock.Verify_GetCachedResponseAsStringAsync(Times.Once());
            ResponseCacheServiceMock.Verify_SetCacheResponseAsync(Times.Never());
        }


        [InlineData("GET", "/Default", "?teste=1")]
        [Theory]
        public async Task When_FilterCalledWithCached_Then_GetObjectCached_Test(string method, string path, string queryString)
        {
            Configuration
                .SetupGet(d => d.CurrentValue).Returns(GetConfiguration("Default".Split(",")));

            CachedByConfigurationAttribute = new CachedByConfigurationAttribute(
                                                                                    ResponseCacheServiceMock.Object,
                                                                                    Configuration.Object
                                                                               );
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
            await CachedByConfigurationAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);

            ResponseCacheServiceMock.Verify_GetCachedResponseAsStringAsync(Times.Once());

            ResponseCacheServiceMock.Verify_SetCacheResponseAsync(Times.Once());
        }

        [InlineData("POST", "/Default")]
        [InlineData("PUT", "/Default/1")]
        [InlineData("DELETE", "/Default/1")]
        [InlineData("PATCH", "/Default/1")]
        [InlineData("GET", "/TESTE")]
        [Theory]
        public async Task When_FilterCalledAndNotGetMethod_Then_NotProcess_Test(string method, string path)
        {
            Configuration
                .SetupGet(d => d.CurrentValue).Returns(GetConfiguration("Default".Split(",")));

            CachedByConfigurationAttribute = new CachedByConfigurationAttribute(
                                                                                    ResponseCacheServiceMock.Object,
                                                                                    Configuration.Object
                                                                               );
            var expectedValue = new { prop1 = "teste" };
            ActionExecutionDelegateMock
                .Setup(d => d.Invoke())
                .ReturnsAsync(
                    SetActionExecutedContext(expectedValue)
                );

            HttpContext.Request.Method = method;
            HttpContext.Request.Path = path;

            //Act
            await CachedByConfigurationAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);

            ResponseCacheServiceMock.Verify_GetCachedResponseAsStringAsync(Times.Never());

            ResponseCacheServiceMock.Verify_SetCacheResponseAsync(Times.Never());
        }

    }
}
