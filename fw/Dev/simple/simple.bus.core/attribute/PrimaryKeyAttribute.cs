using System;

namespace simple.bus.core.attribute
{
    /// <summary>
    /// PrimaryKey Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
        /// </summary>
        public PrimaryKeyAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the named int.
        /// </summary>
        /// <value>
        /// The named int.
        /// </value>
        public int NamedInt { get; set; }
    }
}