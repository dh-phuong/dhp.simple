using System;
using System.Data;
using NUnit.Framework;
using System.Linq;

using simple.sql;
using testfw.model;
using simple.bus.core.context;
using simple.bus.coreTests1.dto;

namespace simple.bus.core.service.Tests
{
    [TestFixture()]
    public class BServiceTests : BService<MUserSp>
    {
        public BServiceTests()
            : base(new DBContext())
        {

        }
        [Test()]
        public void DeleteTest()
        {
            var ret = this.SimpleSelect(new SimpleWhere().Eq("Id", 10)).Delete();
            Assert.AreEqual(1, ret);
        }

        [Test()]
        [ExpectedException(typeof(System.Data.SqlClient.SqlException))]
        public void InsertExceptionTest()
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
        public void InsertTest()
        {
            var test = new MUserSp
            {
                UserCd = "0099",
                LoginId = "99dmin",
                Password = "pass@vn",
                UserFullName = "supper women",
                UserShortName = "Sw",
                GroupCd = "00",
                CustomerCd = "99"
            };
            this.SetUpdateInfo(test);
            var ret = this.SimpleSelect().Insert(test);
            Assert.Greater(ret, 0);
            ret = this.SimpleSelect(new SimpleWhere().Eq("Id", ret)).Delete();
            Assert.AreEqual(1, ret);
        }

        [Test()]
        public void Update1Test()
        {
            var test = new MUserSp
            {
                UserCd = "0099",
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

        [Test()]
        public void ExcecuteFromStoredTest()
        {
            this.SimpleSelectFromStored("P_M_User_GetByLoginID", new GetByLoginIDRequestDto
            {
                LoginId = "dh-phuong"
            }).SingleOrDefault();
        }
    }
}