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
            Func<Dictionary<string, object>, object> calculate;
            object value;

            var fields = new List<Field>()
            {
                new Field
                {
                    Name = "field 1",
                    Type = typeof(decimal),
                    FieldId = Guid.NewGuid().ToString()
                },
                 new Field
                 {
                     Name = "field 2",
                     Type = typeof(long),
                     FieldId = Guid.NewGuid().ToString()
                 }
            };
            var record = new Dictionary<string, object>
            {
                { fields[0].FieldId, 2.0m },
                { fields[1].FieldId, 3L }
            };

            calculate = engine.Compile("{field 2} + 5", fields);
            value = calculate(record);
            Assert.AreEqual(8m, value);

            calculate = engine.Compile("{field 1} + {field 2}", fields);
            value = calculate(record);
            Assert.AreEqual(5m, value);


        }
    }
}
