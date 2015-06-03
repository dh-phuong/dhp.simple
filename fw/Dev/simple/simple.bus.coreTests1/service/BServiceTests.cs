using System;
using System.Data;
using NUnit.Framework;

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
                Id = 10,
                UserCd = "0001",
                LoginId = "4dmin",
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
    }
}