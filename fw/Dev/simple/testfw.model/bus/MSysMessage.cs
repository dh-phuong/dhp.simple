using System;
using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.bus
{
    public sealed class MSysMessage : BEntity<MSysMessage>
    {
        #region Constructor
        public MSysMessage()
        {
        }

        public MSysMessage(DbDataReader dr)
            : base(dr)
        {
        } 
        #endregion

        #region Properties
        public string MsgCd { get; set; }
        public string MsgText { get; set; }
        public string MsgHelp { get; set; }
        public short MsgType { get; set; }
        #endregion

        #region Override
        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
