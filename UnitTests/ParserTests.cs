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
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("(2.0) + (17*2-30) * (5)+2 - (8/2)*4 < 10 and 2<= 3 or 3 < 2");
            Assert.AreEqual(true, value);

            value = engine.CalculateValue("(( ((2.0)) + 4))*((5))");
            Assert.AreEqual(30m, value);

        }


        [TestMethod]
        public void TestIf()
        {
            var value = engine.CalculateValue(@"1 + if(true or false,1, 2.0)");
            Assert.AreEqual(2m, value);
        }
    }
}
