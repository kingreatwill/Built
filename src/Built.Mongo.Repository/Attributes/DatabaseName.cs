using System;

namespace Built.Mongo
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DatabaseNameAttribute : Attribute
    {
        public DatabaseNameAttribute(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Empty database name is not allowed", nameof(value));
            Name = value;
        }

        public virtual string Name { get; private set; }
    }
}