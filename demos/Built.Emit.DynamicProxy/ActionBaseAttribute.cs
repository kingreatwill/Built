using System;

namespace Built.Emit.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ActionBaseAttribute : Attribute
    {
        public virtual void Before(string @method, object[] parameters)
        {
        }

        public virtual object After(string @method, object result)
        {
            return result;
        }
    }
}