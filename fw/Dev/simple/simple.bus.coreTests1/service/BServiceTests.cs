using System;
using System.Data;
using NUnit.Framework;
using System.Linq;

using simple.sql;
using testfw.model;

namespace simple.bus.core.service.Tests
{
    [TestFixture()]
    public class BServiceTests : BService<MUserSp>
    {
        [Test()]
        public void DeleteTest()
        {
            var ret = this.SimpleSelect(new SimpleWhere().Eq("Id", 10)).Delete();
            Assert.AreEqual(1, ret);
        }

        [Test()]
        [ExpectedException(typeof(System.Data.SqlClient.SqlException))]
        public void InsertTest()
        {
            var test = new MUserSp
            {
                UserCd = "0004",
                LoginId = "5dmin",
                Password = "45A8588D978451F8E667996F5430A588",
                UserFullName = "super man",
                UserShortName = "SP",
                GroupCd = "00",
                CustomerCd = "00"
            };
            this.SetUpdateInfo(test);

            var ret = this.SimpleSelect().Insert(test);
            Assert.AreEqual(1, ret);
        }

        [Test()]
        public void Update1Test()
        {
            var test = new MUserSp
            {
                UserCd = "0004",
                LoginId = "5dmin",
                Password = "45A8588D978451F8E667996F5430A588",
                UserFullName = "super man",
                UserShortName = "SP",
                GroupCd = "00",
                CustomerCd = "00"
            };
            this.SetUpdateInfo(test);
            var ret = this.SimpleSelect().Update(test);
            Assert.AreEqual(1, ret);
        }
        [Test()]
        public void Update2Test()
        {
            var test = new MUserSp
            {
                UserCd = "0004",
                LoginId = "5dmin",
                Password = "123456",
                UserFullName = "super man",
                UserShortName = "SP",
                GroupCd = "00",
                CustomerCd = "01"
            };
            this.SetUpdateInfo(test);

            var ret = this.SimpleSelect(new SimpleWhere()
                                        .Eq("LoginId", test.LoginId))
                                        .Update(test);
            Assert.AreEqual(1, ret);
        }

        
    }
}