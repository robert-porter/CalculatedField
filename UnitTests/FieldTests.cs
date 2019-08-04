using System;
using System.Collections.Generic;
using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class FieldTests
    {
        readonly Engine engine = new Engine();

        [TestMethod]
        public void TestFields()
        {
            var fields = new List<Field>()
            {
                new Field
                {
                    Name = "field 1",
                    Type = typeof(decimal),
                    FieldId = Guid.NewGuid()
                },
                 new Field
                 {
                     Name = "field 2",
                     Type = typeof(long),
                     FieldId = Guid.NewGuid()
                 }
            };
            var record = new Dictionary<Guid, object>
            {
                { fields[0].FieldId, 2.0m },
                { fields[1].FieldId, 3.0m }
            };
            var calculate = engine.Compile("{field 1} + {field 2}", fields);
            var value = calculate(record);
            Assert.AreEqual(5m, value);
        }
    }
}
