using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model
{
    public sealed class MMessage : BModel<MMessage>
    {
        #region Constructor

        public MMessage()
        {
        }

        public MMessage(DbDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructor

        [PrimaryKey]
        public string MessageId { get; set; }

        public string MessageStrings { get; set; }

        [PrimaryKey]
        public short Language { get; set; }
    }
}