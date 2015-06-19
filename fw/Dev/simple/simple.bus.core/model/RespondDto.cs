using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simple.core.model
{
    public sealed class RespondDto<T>
    {
        public System.Data.SqlClient.SqlException Error { get; set; }
        public string MsgCd { get; set; }
        public bool IsOk { get; set; }
        public T Respond { get; set; }
    }
}
