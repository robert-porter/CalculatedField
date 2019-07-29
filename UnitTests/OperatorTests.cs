using System;
using System.Collections.Generic;
using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class OperatorTests
    {
        Engine engine = new Engine();


        [TestMethod]
        public void TestIntegerAddition()
        {
            var value = engine.CalculateValue("{a} + 2");
            Assert.AreEqual(4m, value);
        }

        [TestMethod]
        public void TestIntegerSubtraction()
        {
            var value = engine.CalculateValue("2 - 2");
            Assert.AreEqual(0m, value);
        }

        [TestMethod]
        public void TestIntegerMultiplication()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2 * 2");
            Assert.AreEqual(4m, value);
        }

        [TestMethod]
        public void TestIntegerDivision()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("1 / 2");
            Assert.AreEqual(0.5m, value);
        }

        [TestMethod]
        public void TestDecimalAddtion()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2.0 + 2.0");
            Assert.AreEqual(4m, value);
        }

        [TestMethod]
        public void TestDecimalSubtraction()
        {
            var value = engine.CalculateValue("2.0 - 2.0");
            Assert.AreEqual(0m, value);
        }

        [TestMethod]
        public void TestDecimalMultiplication()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("2.0 * 2.0");
            Assert.AreEqual(4m, value);
        }

        [TestMethod]
        public void TestDecimalDivision()
        {
            var engine = new Engine();
            var value = engine.CalculateValue("1.0 / 2.0");
            Assert.AreEqual(0.5m, value);
        }
      
        [TestMethod]
        public void TestStringAddition()
        {
            var value = engine.CalculateValue("\"ab\" + \"cd\"");
            Assert.AreEqual("abcd", value);

            value = engine.CalculateValue("'ab' + 'cd'");
            Assert.AreEqual("abcd", value);       
        }

        [TestMethod]
        public void TestStringLess()
        {
            var value = engine.CalculateValue("\"ab\" < \"cd\"");
            Assert.AreEqual(true, value);

            value = engine.CalculateValue("\"cd\" < \"ab\"");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("\"ab\" < \"ab\"");
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestStringLessOrEqual()
        {
            var value = engine.CalculateValue("\"ab\" <= \"cd\"");
            Assert.AreEqual(true, value);

            value = engine.CalculateValue("\"cd\" <= \"ab\"");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("\"ab\" <= \"ab\"");
            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestStringGreater()
        {
            var value = engine.CalculateValue("\"ab\" > \"cd\"");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("\"ab\" > \"ab\"");
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestStringGreaterOrEqual()
        {
            var value = engine.CalculateValue("\"ab\" >= \"cd\"");
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestStringEqual()
        {
            var value = engine.CalculateValue("\"ab\" = \"cd\"");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("\"ab\" = \"ab\"");
            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestStringNotEqual()
        {
            var value = engine.CalculateValue("\"ab\" <> \"ab\"");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("\"ab\" <> \"cd\"");
            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestDateSubtraction()
        {
            var value = engine.CalculateValue("#1/1/2000# - #11/1/200#");
            var expected = DateTime.Parse("1/1/2000") - DateTime.Parse("11/1/200");            
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void TestDateLess()
        {
            var value = engine.CalculateValue("#1/1/2000# < #11/1/200#");
            var expected = DateTime.Parse("1/1/2000") < DateTime.Parse("11/1/200");
            Assert.AreEqual(expected, value);

            value = engine.CalculateValue("#11/1/200# < #1/1/2000#");
            expected = DateTime.Parse("11/1/200") < DateTime.Parse("1/1/2000");
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void TestDateLessOrEqual()
        {
            var value = engine.CalculateValue("#1/1/2000# <= #11/1/200#");
            var expected = DateTime.Parse("1/1/2000") <= DateTime.Parse("11/1/200");
            Assert.AreEqual(expected, value);

            value = engine.CalculateValue("#11/1/200# <= #1/1/2000#");
            expected = DateTime.Parse("11/1/200") <= DateTime.Parse("1/1/2000");
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void TestDateGreater()
        {
            var value = engine.CalculateValue("#1/1/2000# > #11/1/200#");
            var expected = DateTime.Parse("1/1/2000") > DateTime.Parse("11/1/200");
            Assert.AreEqual(expected, value);

            value = engine.CalculateValue("#11/1/200# > #1/1/2000#");
            expected = DateTime.Parse("11/1/200") > DateTime.Parse("1/1/2000");
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void TestDateTimeSpanAddition()
        {
            var value = engine.CalculateValue("#1/1/200# + timeSpanFromDays(10)");
            var expected =DateTime.Parse("1/1/200") + TimeSpan.FromDays(10);
            Assert.AreEqual(expected, value);

        }

        [TestMethod]
        public void TestTimeSpanAddition()
        {
            var value = engine.CalculateValue("timeSpanFromDays(10) + timeSpanFromDays(10)");
            var expected = TimeSpan.FromDays(20);
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void TestTimeSpanLess()
        {
            var value = engine.CalculateValue("timeSpanFromDays(5) < timeSpanFromDays(10)");
            Assert.AreEqual(true, value);

            value = engine.CalculateValue("timeSpanFromDays(10) < timeSpanFromDays(5)");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("timeSpanFromDays(10) < timeSpanFromDays(10)");
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestDateGreaterOrEqual()
        {
            var value = engine.CalculateValue("#1/1/2000# >= #11/1/200#");
            var expected = DateTime.Parse("1/1/2000") >= DateTime.Parse("11/1/200");
            Assert.AreEqual(expected, value);

            value = engine.CalculateValue("#11/1/200# >= #1/1/2000#");
            expected = DateTime.Parse("11/1/200") >= DateTime.Parse("1/1/2000");
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public void TestBoolOperators()
        {
            var value = engine.CalculateValue("true and true");
            Assert.AreEqual(true, value);
           
            value = engine.CalculateValue("true and false");
            Assert.AreEqual(false, value);
           
            value = engine.CalculateValue("false and true");
            Assert.AreEqual(false, value);

            value = engine.CalculateValue("false and false");
            Assert.AreEqual(false, value);
           
            value = engine.CalculateValue("true or true");
            Assert.AreEqual(true, value);
          
            value = engine.CalculateValue("true or false");
            Assert.AreEqual(true, value);

            value = engine.CalculateValue("false or true");
            Assert.AreEqual(true, value);
           
            value = engine.CalculateValue("false or false");
            Assert.AreEqual(false, value);
          
            value = engine.CalculateValue("not false");
            Assert.AreEqual(true, value);

            value = engine.CalculateValue("not true");
            Assert.AreEqual(false, value);

        }
    }
}
