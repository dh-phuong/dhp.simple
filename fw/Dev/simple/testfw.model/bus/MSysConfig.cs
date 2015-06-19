using System;
using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.bus
{
    public sealed class MSysConfig : BEntity<MSysConfig>
    {
        #region Constructor
        public MSysConfig()
        {
        }

        public MSysConfig(DbDataReader dr)
            : base(dr)
        {
        } 
        #endregion

        #region Properties
        public string CfgCd { get; set; }
        public string CfgNm { get; set; }
        public string SubCd { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        #endregion

        #region Override
        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
