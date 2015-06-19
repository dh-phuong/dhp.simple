using System;
using System.Data;
using System.Linq;

using simple.sql;
using testfw.model;
using simple.core.context;
using simple.core.service;
using testfw.model.appr;
using System.Collections.Generic;
using simple.core.model;
using testfw.model.bus;
using BUSBlogic.Rules;

namespace BUSBlogic.Blogic
{
    public sealed class MSysUserBlogic : BService<MSysUser>
    {
        public MSysUserBlogic(IBContext ctx)
            : base(ctx) {}

        readonly MSysUserRules Rules = new MSysUserRules();

        public RespondDto<MSysUser> Login(string loginId, string password)
        {
            var resDto = new RespondDto<MSysUser>();
            Rules.LoginCheck(resDto, loginId, password);
            if (resDto.IsOk)
            {
                var mSysUser = this.SimpleSelect(new SimpleWhere()
                                                                .Eq("LoginId", loginId)
                                                                .Eq("DelFlg", false)).SingleOrDefault();
                resDto.IsOk = false;
                if (mSysUser == null)
                {
                    resDto.MsgCd = "E0005";
                }
                else
                {
                    if (mSysUser.Password.Equals(password))
                    {
                        resDto.IsOk = true;
                        resDto.Respond = mSysUser;
                    }
                    else
                    {
                        resDto.MsgCd = "E0006";
                    }
                }
            }
            
            return resDto;
        }

        public override string NextCSeq()
        {
            throw new NotImplementedException();
        }
    }
}
