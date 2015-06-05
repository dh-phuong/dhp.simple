using System.Data.Common;
using simple.bus.core.model;

namespace testfw.model
{
    public class MLabel : BModel<MLabel>
    {
        #region Constructor

        public MLabel()
        {
        }

        public MLabel(DbDataReader dr)
            : base(dr)
        {
        }

        #endregion Constructor

        public string LabelCd { get; set; }

        public string LabelStrings { get; set; }

        public short Language { get; set; }
    }
}