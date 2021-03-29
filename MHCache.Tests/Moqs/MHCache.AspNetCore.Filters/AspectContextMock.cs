using System.Linq;
using System.Reflection;
using AspectCore.DynamicProxy;
using Moq;

namespace MHCache.Tests.Moqs.MHCache.AspNetCore.Filters
{
    public class AspectContextMock : Mock<AspectContext>
    {
        public Mock<MethodInfo> MethodInfoMock { get; set; }

        public AspectContextMock()
        {
            MethodInfoMock = new Mock<MethodInfo>();
        }

        public void Set_PathMethodName(string fullTypeName, string methodName) 
        {
            MethodInfoMock
                .SetupGet(d => d.Name)
                .Returns(methodName);

            MethodInfoMock
                .SetupGet(d => d.DeclaringType.FullName)
                .Returns(fullTypeName);

            SetupGet(d => d.ImplementationMethod)
                .Returns(MethodInfoMock.Object);

            SetupGet(d => d.ProxyMethod)
                .Returns(MethodInfoMock.Object);

        }
                         
        public void Set_Parameters(object parameters) 
        {
            var _parameters = parameters
                    .GetType()
                    .GetProperties()
                    .Select(o =>
                    {
                        var paramMock = new Mock<ParameterInfo>();
                        paramMock.SetupGet(d => d.Name).Returns(o.Name);
                        return paramMock.Object;
                    })
                    .ToArray();

            var values = parameters
                   .GetType()
                   .GetProperties()
                   .Select(o => o.GetValue(parameters))
                   .ToArray();

            MethodInfoMock
                .Setup(d => d.GetParameters())
                .Returns(() => _parameters);

            SetupGet(d => d.Proxy)
                .Returns(MethodInfoMock.Object);

            SetupGet(d => d.Parameters)
                .Returns(() => values);
        }
    }
}
