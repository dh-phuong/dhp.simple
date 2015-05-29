using System;
using NUnit.Framework;

using simple.sql;
using testfw.model;

namespace simple.bus.core.service.Tests
{
    [TestFixture()]
    public class BServiceTests : BService<TestModel>
    {
        [Test()]
        [ExpectedException(typeof(NullReferenceException))]
        public void DeleteTest()
        {
            this.SimpleSelect<TestModel>(new SimpleWhere()
                                           .Eq("FullName", true)
                                           .Eq("CSeq", 1)
                                           .Eq("UpTime", DateTime.Now)
                                           .Neq("ShortName", DBNull.Value)
                                           .Neq("FullName", null)
                                           .In("Value", "1", "2", "3")
                                           .Nin("Value", "4", null, "6")
                                           .Btw("Code", "001", "002")
                                           .Or(new SimpleWhere()
                                               .Eq("FullName", true)
                                               .Eq("CSeq", 1)
                                               .Eq("UpTime", DateTime.Now)
                                               .Neq("ShortName", DBNull.Value)
                                               .Neq("FullName", null)
                                               .In("Value", "1", "2", "3")
                                               .Nin("Value", "4", null, "6")
                                               .Btw("Code", "001", "002")
                                               .StartWith("ShortName", "d")
                                               .EndWith("ShortName", "d")
                                               .Contains("ShortName", "d"))
                                            .Or(new SimpleWhere()
                                                .Eq("UpTime", DateTime.Now))
                                           ) .Single();
            //var model = new TestModel();
            //model.Id = 1;
            //var ret = base.Delete(model);

            //Assert.AreEqual(1, ret);
        }
    }
}