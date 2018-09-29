using System;
using Microsoft.Extensions.DependencyInjection;

namespace Built.Extensions.DependencyInjection
{
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }

        public static T GetService<T>() where T : class
        {
            return Instance.GetService<T>();
        }
    }
}