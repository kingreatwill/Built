using System;

namespace Built.Mongo
{
    /// <summary>
    /// 未实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class IndexOptionsAttribute : Attribute
    {
        /// <summary>
        /// IndexOptionsAttribute
        /// </summary>
        /// <param name="value">Name of the Index.</param>
        public IndexOptionsAttribute(string indexName = null, bool? unique = null)
        {
            Name = indexName;
            Unique = unique;
        }

        /// <summary>
        /// Name
        /// </summary>
        /// <value>The name of the Index.</value>
        public string Name { get; private set; }

        public virtual bool? Unique { get; set; }
    }
}