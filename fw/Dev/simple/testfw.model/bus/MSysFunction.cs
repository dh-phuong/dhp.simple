using System;
using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.bus
{
    public sealed class MSysFunction : BEntity<MSysFunction>
    {
        #region Constructor
        public MSysFunction()
        {
        }

        public MSysFunction(DbDataReader dr)
            : base(dr)
        {
        } 
        #endregion

        #region Properties
        public string FuncCd { get; set; }
        public string FuncNm { get; set; }
        public string AssemblyDll { get; set; }
        public string ClassNm { get; set; }
        #endregion

        #region Override
        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
