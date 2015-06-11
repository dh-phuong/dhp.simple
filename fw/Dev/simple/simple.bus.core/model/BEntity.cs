using System;
using System.Data.Common;
using simple.bus.core.attribute;

namespace simple.bus.core.model
{
    public class BEntity<T> : BModel<T>
        where T : class
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BEntity{T}"/> class.
        /// </summary>
        public BEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BEntity{T}"/> class.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public BEntity(DbDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructor

        #region Base

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [AutoColumn()]
        public decimal Id { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether [delete flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delete flag]; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteFlag { get; set; }

        /// <summary>
        /// Gets or sets the create u cd.
        /// </summary>
        /// <value>
        /// The create user cd.
        /// </value>
        public int CreateUId { get; set; }

        /// <summary>
        /// Gets or sets the update u cd.
        /// </summary>
        /// <value>
        /// The update user cd.
        /// </value>
        public int UpdateUId { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        /// <value>
        /// The update date.
        /// </value>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        /// <value>
        /// The version number.
        /// </value>
        [AutoColumn]
        public int VersionNo { get; internal set; }

        #endregion Base
    }
}