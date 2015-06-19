using System;
using System.Data.Common;
using simple.bus.core.attribute;
using simple.bus.core.model;

namespace testfw.model.bus
{
    public sealed class MSysGroup : BEntity<MSysGroup>
    {
        #region Constructor
        public MSysGroup()
        {
        }

        public MSysGroup(DbDataReader dr)
            : base(dr)
        {
        } 
        #endregion

        #region Properties
        public string GrpCd { get; set; }
        public string GrpNm { get; set; }
        #endregion

        #region Override
        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
