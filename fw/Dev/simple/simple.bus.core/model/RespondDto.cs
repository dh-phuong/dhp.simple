using System.Data.SqlClient;

namespace simple.core.model
{
    public sealed class RespondDto<T>
    {
        public SqlException Error { get; set; }
        public string MsgCd { get; set; }
        public object[] MsgParams { get; set; }
        public bool IsOk { get; set; }
        public T Respond { get; set; }

    }
}