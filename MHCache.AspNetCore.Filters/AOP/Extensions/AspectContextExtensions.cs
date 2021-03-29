using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;

namespace MHCache.AspNetCore.Filters.AOP.Extensions
{
    public static class AspectContextExtensions 
    {
        public static string GetGenerateKeyByMethodNameAndValues(this AspectContext context)
        {            
            var parameters = context.ProxyMethod
                .GetParameters()?
                .Select((o,index) => 
                new
                {
                    o.Name,
                    Value = context.Parameters[index]
                }).ToArray();

            return JsonSerializer
                .Serialize(
                    new
                    {
                        Method = $"{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}",
                        Params = parameters
                    }
                );
        }

        public static void SetReturnType(this AspectContext context, Type methodReturnType, object value)
        {
            if (context.IsAsync())
            {
                var returnType = methodReturnType.GenericTypeArguments.FirstOrDefault();
                if (methodReturnType == typeof(Task<>).MakeGenericType(returnType))
                {
                    context.ReturnValue = typeof(Task).GetMethod(nameof(Task.FromResult)).MakeGenericMethod(returnType).Invoke(null, new[] { value });
                }
                else if (methodReturnType == typeof(ValueTask<>).MakeGenericType(returnType))
                {
                    context.ReturnValue = Activator.CreateInstance(typeof(ValueTask<>).MakeGenericType(returnType), value);
                }
            }
            else
            {
                context.ReturnValue = value;
            }
        }

        public async static Task<object> GetReturnValueAsync(this AspectContext context)
            => context.IsAsync() ?
                  await context.UnwrapAsyncReturnValue() : 
                  context.ReturnValue;

        public static bool HasAttributeType(this AspectContext context, Type attributeType)
            => context.ProxyMethod.GetCustomAttributes(attributeType, true).Length == 0;

    }
}
