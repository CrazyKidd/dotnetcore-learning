using System;
using Castle.DynamicProxy;

namespace NetNote.Aop
{
    public class MyIntercept : StandardInterceptor
    {
        private static NLog.Logger logger;
        //执行前
        protected override void PreProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行前,入参：" + string.Join(",", invocation.Arguments));
        }
        //执行中
        protected override void PerformProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行中");
            
            try
            {
                base.PerformProceed(invocation);
            }
            catch (Exception ex)
            {
                //HandleException(ex);
                throw new NotImplementedException();
                
            }
        }
        //执行后
        protected override void PostProceed(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name + "执行后，返回值：" + invocation.ReturnValue);
        }
    }
}
