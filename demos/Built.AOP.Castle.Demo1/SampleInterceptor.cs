using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Built.AOP.Castle.Demo1
{
    internal class SampleInterceptor : IInterceptor
    {
        public SampleInterceptor()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public void Intercept(IInvocation invocation)
        {
            output("开始进入拦截器");

            MethodInfo concreteMethod = invocation.GetConcreteMethod();

            if (!invocation.MethodInvocationTarget.IsAbstract)
            {
                output("开始执行 " + concreteMethod.Name);

                //执行原对象中的方法
                invocation.Proceed();

                output("执行结果 " + invocation.ReturnValue);
            }

            output("执行完毕");
        }

        private void output(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}