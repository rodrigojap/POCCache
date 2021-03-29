using System.Collections.Generic;
using System.Linq;
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
    public class CachedRemoveByConfigurationAttributeTest
    {
        #region Properties

        private ResponseCacheServiceMock ResponseCacheServiceMock { get; }

        private CachedRemoveByConfigurationAttribute CachedRemoveByConfigurationAttribute { get; set; }

        public ActionExecutingContext Context { get; set; }
        
        public HttpContext HttpContext { get; set; }

        private Mock<ActionExecutionDelegate> ActionExecutionDelegateMock { get; set; }


        private Mock<IOptionsMonitor<FilterCachedConfiguration>> Configuration { get; set; }

        #endregion

        public CachedRemoveByConfigurationAttributeTest()
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


        private FilterCachedConfiguration GetConfiguration(IDictionary<string,string> routes)
            => new FilterCachedConfiguration()
               {
                   GeneralTimeToLiveSeconds = 60,
                   CachedRemoveRoutes = routes.Select(route => new RouteCachedRemoveConfiguration { CachedRouteName = route.Key, PatternRouteName = route.Value })
               };

        [InlineData("POST", "/Default", "*Default*")]
        [Theory]
        public async Task When_FilterCalled_Then_RemoveAllCachedPattern_Test(string method, string path, string patternRemove ) 
        {
            Configuration
                .SetupGet(d => d.CurrentValue)
                .Returns(GetConfiguration(new Dictionary<string, string>() { { path, patternRemove } }));

            CachedRemoveByConfigurationAttribute = new CachedRemoveByConfigurationAttribute(
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
            await CachedRemoveByConfigurationAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);
            
            ResponseCacheServiceMock.Verify_GetKeysByPattern( Times.Once(), patternRemove);
            ResponseCacheServiceMock.Set_RemoveCachedResponseByNameAsync(Times.Once());
        }


        [InlineData("GET", "/Default", "/Default", "*Default*")]
        [InlineData("POST", "/Default", "/Teste", "*Default*")]
        [InlineData("PUT", "/Default", "/Teste", "*Default*")]
        [InlineData("DELETE", "/Default", "/Teste", "*Default*")]
        [InlineData("PATCH", "/Default", "/Teste", "*Default*")]
        [Theory]
        public async Task When_FilterCalledWithCached_Then_GetObjectCached_Test(string method, string path, string routeRemove, string patternRemove)
        {
            Configuration
                .SetupGet(d => d.CurrentValue)
                .Returns(GetConfiguration(new Dictionary<string, string>() { { routeRemove, patternRemove } }));


            CachedRemoveByConfigurationAttribute = new CachedRemoveByConfigurationAttribute(
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
            await CachedRemoveByConfigurationAttribute.OnActionExecutionAsync(Context, ActionExecutionDelegateMock.Object);

            ResponseCacheServiceMock.Verify_GetKeysByPattern(Times.Never(), patternRemove);
            ResponseCacheServiceMock.Set_RemoveCachedResponseByNameAsync(Times.Never());
        }



    }
}
