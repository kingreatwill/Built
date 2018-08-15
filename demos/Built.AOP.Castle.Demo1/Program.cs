using Castle.DynamicProxy;
using System;

namespace Built.AOP.Castle.Demo1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //创建拦截器对象
            SampleInterceptor Interceptor = new SampleInterceptor();

            //给person类生成代理
            ProxyGenerator Generator = new ProxyGenerator();
            IPerson p = Generator.CreateInterfaceProxyWithTarget<IPerson>(new Person(), Interceptor);

            //执行方法看效果
            p.Doing();
            Console.ReadLine();
        }
    }
}