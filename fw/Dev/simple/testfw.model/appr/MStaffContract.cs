using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.appr
{
    public sealed class MStaffContract : BEntity<MStaffContract>
    {
        public MStaffContract() { }
        public MStaffContract(DbDataReader dr)
            : base(dr) { }

        public int StaffId { get; set; }
        public string ContractNo { get; set; }
        public short ContractType { get; set; }
        public DateTime StartDate { get; set; }

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }
}
