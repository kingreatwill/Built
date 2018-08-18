using System;

namespace Built.Micro.ImageCloud.Mongo
{
    /// <summary>
    /// Attribute used to annotate Enities with to override mongo collection name. By default, when this attribute
    /// is not specified, the classname will be used.
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