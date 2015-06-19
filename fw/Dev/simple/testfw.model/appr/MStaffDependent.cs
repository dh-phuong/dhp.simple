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
    public sealed class MStaffDependent : BEntity<MStaffDependent>
    {
        public MStaffDependent() { }
        public MStaffDependent(DbDataReader dr)
            : base(dr) { }

        public int StaffId { get; set; }
        public DateTime RegistDate { get; set; }
        public int Dependent { get; set; }
        public int Total { get; set; }

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }
}
