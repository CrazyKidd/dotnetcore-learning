using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NetNote.Aop
{
    public class LogInterceptor : IInterceptor
    {
        private readonly ILogger<LogInterceptor> _loger;
        public LogInterceptor(ILogger<LogInterceptor> logger)
        {
            _loger = logger;
        }
        public void Intercept(IInvocation invocation)
        {
            //throw new NotImplementedException();
            //调用下一个拦截器直到目标方法
            string logData = $"【执行时间】：{DateTime.Now:yyyy/MM/dd HH:mm:ss}  \r\n" +
                          $"【执行方法】: {invocation.Method.Name}  \r\n" +
                          $"【执行参数】：{string.Join(", ", invocation.Arguments.Select(x => (x ?? "").ToString()).ToArray())} \r\n";
            _loger.LogInformation(logData);
            invocation.Proceed();
            //判断是否异步方法
            if (IsAsyncMethod(invocation.Method){
                var type = invocation.Method.ReturnType;
                var returnProperty = type.GetProperty("Result");
                if (returnProperty == null)
                    return;
                var result = returnProperty.GetValue(type);
                logData += $"【执行完成】：{JsonConvert.SerializeObject(result)}";
                _loger.LogInformation(logData);
            }
            else
            {
                logData += $"【执行完成】：{invocation.ReturnValue}";
                _loger.LogInformation(logData);
            }
        }

        protected bool IsAsyncMethod(MethodInfo method)
        {
            //判断method是不是Task的子类
            bool isAsync = typeof(Task) == method.ReturnType || typeof(Task).IsAssignableFrom(method.ReturnType) ||
                   method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
            return isAsync;
        }
    }
}
