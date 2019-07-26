using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;


namespace UnitTests
{
    [TestClass]
    public class ParserTests
    {
        Engine engine = new Engine();


        [TestMethod]
        public void TestOrderOfOperations()
        {
            var value = engine.CalculateValue("(2.0) + (17*2-30) * (5)+2 - (8/2)*4 < 10 and 2 >= 3");
            Assert.AreEqual(false, value.Value);

            value = engine.CalculateValue("(2.0) + (17*2-30) * (5)+2 - (8/2)*4 < 10 and 2<= 3 or 3 < 2");
            Assert.AreEqual(true, value.Value);

            value = engine.CalculateValue("(( ((2.0)) + 4))*((5))");
            Assert.AreEqual(30m, value.Value);

        }

        [TestMethod]
        public void TestIf()
        {
            var value = engine.CalculateValue("if true or false then 1.0 else 2.0 end");
            Assert.AreEqual(1m, value.Value);

            value = engine.CalculateValue(@"

1 + if true or false then 1.0 else

                2.0

end");
            Assert.AreEqual(2m, value.Value);


            value = engine.CalculateValue(@"
if true or false then 
1.0 
else

2.0 

end");
            Assert.AreEqual(1m, value.Value);

            value = engine.CalculateValue(@"
if true or false then 1.0 
else 2.0 end");
            Assert.AreEqual(1m, value.Value);

            value = engine.CalculateValue(@"
if false then 1.0 
else if true then 3 else 2 end");
            Assert.AreEqual(3m, value.Value);

            value = engine.CalculateValue("if false then null end");
            Assert.AreEqual(null, value.Value, null);
        }
    }
}
