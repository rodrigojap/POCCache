using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;

namespace MHCache.AspNetCore.Filters
{
    public static class AspectContextExtensions 
    {
        public static string GetGenerateKeyByMethodNameAndValues(this AspectContext context)
        {            
            var parameters = context.ProxyMethod.GetParameters().Select((o,index) => {
                return new
                {
                    o.Name,
                    Value = context.Parameters[index]
                };
            });

            return JsonSerializer
                .Serialize(
                    new
                    {
                        Method = $"{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}",
                        Params = parameters
                    }
                );
        }

        public static void SetReturnType(this AspectContext context, Type methodReturnType, Type returnType, object value)
        {
            if (context.IsAsync())
            {
                //Determine whether it is a task or a valuetask
                if (methodReturnType == typeof(Task<>).MakeGenericType(returnType))
                {
                    //Reflection gets the return value of task < > type, which is equivalent to Task.FromResult (value)
                    context.ReturnValue = typeof(Task).GetMethod(nameof(Task.FromResult)).MakeGenericMethod(returnType).Invoke(null, new[] { value });
                }
                else if (methodReturnType == typeof(ValueTask<>).MakeGenericType(returnType))
                {
                    //Reflection builds the return value of valuetask < > type, which is equivalent to new valuetask (value)
                    context.ReturnValue = Activator.CreateInstance(typeof(ValueTask<>).MakeGenericType(returnType), value);
                }
            }
            else
            {
                context.ReturnValue = value;
            }
        }

        public async static Task<object> GetReturnType(this AspectContext context)
            => context.IsAsync() ?
                  await context.UnwrapAsyncReturnValue() : 
                  context.ReturnValue;


    }
}
