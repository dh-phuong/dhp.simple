using System;
using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.bus
{
    public sealed class MSysAuthority : BEntity<MSysAuthority>
    {
        #region Constructor
        public MSysAuthority()
        {
        }

        public MSysAuthority(DbDataReader dr)
            : base(dr)
        {
        } 
        #endregion

        #region Properties
        public int GrpId { get; set; }
        public int FuncId { get; set; }
        public bool Authority1 { get; set; }
        public bool Authority2 { get; set; }
        public bool Authority3 { get; set; }
        public bool Authority4 { get; set; }
        public bool Authority5 { get; set; }
        public bool Authority6 { get; set; }
        public bool Authority7 { get; set; }
        public bool Authority8 { get; set; }
        public bool Authority9 { get; set; }
        public bool Authority10 { get; set; }
        public bool Authority11 { get; set; }
        public bool Authority12 { get; set; }
        public bool Authority13 { get; set; }
        public bool Authority14 { get; set; }
        public bool Authority15 { get; set; }
        public bool Authority16 { get; set; }
        public bool Authority17 { get; set; }
        public bool Authority18 { get; set; }
        public bool Authority19 { get; set; }
        public bool Authority20 { get; set; }

        #endregion

        #region Override
        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
