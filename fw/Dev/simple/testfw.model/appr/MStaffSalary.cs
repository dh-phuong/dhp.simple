using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.bus.core.attribute;
using simple.bus.core.model;

namespace testfw.model.appr
{
    public sealed class MStaffSalary : BEntity<MStaffSalary>
    {
        public MStaffSalary() { }
        public MStaffSalary(DbDataReader dr)
            : base(dr) { }

        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Allowance { get; set; }

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }
}
