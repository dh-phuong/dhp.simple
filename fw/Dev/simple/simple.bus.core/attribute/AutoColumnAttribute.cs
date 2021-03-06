﻿using System;

namespace simple.core.attribute
{
    /// <summary>
    /// Auto Column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AutoColumnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoColumnAttribute"/> class.
        /// </summary>
        public AutoColumnAttribute()
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