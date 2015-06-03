using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.bus.core.model;

namespace testfw.model
{
    public sealed class MUserSp : BEntity<MUserSp>
    {
        public MUserSp()
        {
        }

        public MUserSp(DbDataReader dr)
            : base(dr)
        {
        }

        public string UserCd { get; set; }

        public string LoginId { get; set; }

        public string Password { get; set; }

        public string UserFullName { get; set; }

        public string UserShortName { get; set; }

        public string GroupCd { get; set; }

        public string CustomerCd { get; set; }
    }

}
