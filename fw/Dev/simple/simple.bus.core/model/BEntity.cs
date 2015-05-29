using System.Data.Common;

namespace simple.bus.core.model
{
    public abstract class BEntity<T> : BModel<T>
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
        public decimal Id { get; set; }

        #endregion Base
    }
}
