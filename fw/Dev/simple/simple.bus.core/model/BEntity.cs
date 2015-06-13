using System;
using System.Data.Common;
using simple.bus.core.attribute;

namespace simple.bus.core.model
{
    public abstract class BEntity<T> : BModel<T>
        where T : class
    {
        #region CONST
        public const int C_SEQ_LENGTH = 10;
        #endregion

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
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        public string AppId { get; set; }
        /// <summary>
        /// Gets or sets the c seq.
        /// </summary>
        /// <value>
        /// The c seq.
        /// </value>
        public string CSeq { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [delete flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delete flag]; otherwise, <c>false</c>.
        /// </value>
        public bool DelFlg { get; set; }

        /// <summary>
        /// Gets or sets the create u cd.
        /// </summary>
        /// <value>
        /// The create user id.
        /// </value>
        public int CrtUId { get; set; }

        /// <summary>
        /// Gets or sets the update u cd.
        /// </summary>
        /// <value>
        /// The update user id .
        /// </value>
        public int UpdUId { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime CrtTime { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        /// <value>
        /// The update date.
        /// </value>
        public DateTime UpdTime { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        /// <value>
        /// The version number.
        /// </value>
        [AutoColumn]
        public int VerNo { get; internal set; }

        #endregion Base

        #region abstract

        public abstract string FixCd(string text, char paddingChar = '0'); 
        #endregion
    }
}