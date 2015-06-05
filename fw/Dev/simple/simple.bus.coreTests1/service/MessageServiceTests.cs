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

namespace simple.bus.core.service.Tests
{
    [TestFixture()]
    public class MessageServiceTests : BService<MMessage>
    {
        [Test()]
        public void Select1Test()
        {
            var test = new MMessage
            {
                Language = 1
            };

            var Items = this.SimpleSelect(new SimpleWhere()
                                                  .Eq("Language", test.Language))
                                        .OrderBy("MessageId asc")
                                        .GroupBy("MessageId")
                                        .Max("MessageStrings")
                                        .Get("MessageId");
            foreach (var item in Items)
            {
                Debug.Write(item["MessageId"]);
                Debug.WriteLine(item["MessageStrings"]);
            }
            Assert.AreEqual(Items.Count(), 110);
        }

        [Test()]
        public void SelectSingleOrDefaultTest()
        {
            var test = new MMessage
            {
                Language = 1,
                MessageId = "E0001"
            };

            var max = this.SimpleSelect(new SimpleWhere()
                                                  .Eq("Language", test.Language)
                                                  .Eq("MessageId", test.MessageId)
                                       )
                                       .SingleOrDefault();
            var pk = max.GetPK();
            Assert.AreEqual(max.MessageId, pk["MessageId"]);
            Assert.AreEqual(max.Language, pk["Language"]);
        }
    }
}
