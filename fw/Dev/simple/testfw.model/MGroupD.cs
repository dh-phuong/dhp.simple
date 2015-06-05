using System.Data.Common;
using simple.bus.core.attribute;
using simple.bus.core.model;

namespace testfw.model
{
    public sealed class MGroupD : BModel<MGroupD>
    {
        [PrimaryKey]
        public string GroupCd { get; set; }

        [PrimaryKey]
        public string ViewCd { get; set; }

        [PrimaryKey]
        public string RolesCd { get; set; }

        public bool EnableFlag { get; set; }

        public MGroupD()
        {
        }

        public MGroupD(DbDataReader dr)
        {
        }
    }
}