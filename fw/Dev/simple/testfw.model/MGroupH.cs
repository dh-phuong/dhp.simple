using System.Data.Common;
using simple.bus.core.attribute;
using simple.bus.core.model;

namespace testfw.model
{
    public sealed class MGroupH : BEntity<MGroupH>
    {
        [PrimaryKey]
        public string GroupCd { get; set; }

        public string GroupName { get; set; }

        public MGroupH()
        {
        }

        public MGroupH(DbDataReader dr)
            : base(dr)
        {
        }
    }
}