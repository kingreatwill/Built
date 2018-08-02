using System;

namespace Built.Grpc
{
    /// <summary>
    /// 忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GrpcIgnoreAttribute : Attribute
    {
        /// <summary>
        /// GrpcIgnoreAttribute
        /// </summary>
        public GrpcIgnoreAttribute()
        {
        }
    }
}