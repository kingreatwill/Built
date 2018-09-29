using System;

namespace Built.Emit.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class InterceptorBaseAttribute : Attribute
    {
        public virtual object Invoke(object @object, string @method, object[] parameters)
        {
            return @object.GetType().GetMethod(@method).Invoke(@object, parameters);
        }
    }
}