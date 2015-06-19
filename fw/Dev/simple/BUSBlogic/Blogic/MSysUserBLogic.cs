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

namespace BUSBlogic.Blogic
{
    public sealed class MSysUserBlogic : BService<MSysUser>
    {
        public MSysUserBlogic(IBContext ctx)
            : base(ctx) {}


        public override string NextCSeq()
        {
            throw new NotImplementedException();
        }
    }
}
