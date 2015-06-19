using System;
using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.bus
{
    public sealed class MSysUser : BEntity<MSysUser>
    {
        #region Constructor
        public MSysUser()
        {
        }

        public MSysUser(DbDataReader dr)
            : base(dr)
        {
        } 
        #endregion

        #region Properties
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string FullNm { get; set; }
        public string ShortNm { get; set; }
        public int GrpId { get; set; } 
        #endregion

        #region Override
        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
