using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using simple.bus.core.service;
using simple.sql;
using testfw.model;

namespace simple.bus.coreTests1.service
{
     [TestFixture()]
    public class UserServiceTests : BService<MUserSp>
    {
        [Test()]
         public void SingleTest()
        {
            var test = new MUserSp
            {
                UserCd = "0001"
            };

            var max = this.SimpleSelect(new SimpleWhere()
                                                  .Eq("UserCd", test.UserCd)
                                       ).
                                       Single();
            Debug.WriteLine(" -[RESULT] : " + max.ToString());
            var pk = max.GetPK();
        }
    }
}
