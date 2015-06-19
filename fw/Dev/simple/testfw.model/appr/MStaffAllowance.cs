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
    public sealed class MStaffAllowance : BEntity<MStaffAllowance>
    {
        public MStaffAllowance() {}
        public MStaffAllowance(DbDataReader dr)
            : base(dr) {}

        public int SalaryId { get; set; }
        public int No { get; set; }
        public int AllowanceName { get; set; }
        public int Allowance { get; set; }
        public short AccountingFlag { get; set; }

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }
}
