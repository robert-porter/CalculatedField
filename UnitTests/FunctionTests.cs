using System;
using CalculatedField;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class FunctionTests
    {
        readonly Engine engine = new Engine();

        [TestMethod]
        public void TestCase()
        {
            object value;

            value = engine.CalculateValue(@"cases(null, 2, null, 4, 4, null, 3)");
            Assert.AreEqual(3m, value);

            value = engine.CalculateValue(@"cases(3, 2, ""two"", 4, ""four"", 5, ""five"", ""default"")");
            Assert.AreEqual("default", value);

            value = engine.CalculateValue(@"cases(3, 2, ""two"", 4, ""four"", 5, ""five"", 10)");
            Assert.AreEqual(null, value);

            value = engine.CalculateValue(@"cases(3, 2, ""two"", 4, ""four"", 3, ""three"")");
            Assert.AreEqual("three", value);
        }

        [TestMethod]
        public void TestIfs()
        {
            object value;
            value = engine.CalculateValue(@"ifs(1 > 2, 2, true, 4, false, 3)");
            Assert.AreEqual(4m, value);

            value = engine.CalculateValue(@"ifs(1 > 2, 2, null, 4, false, 3)");
            Assert.AreEqual(null, value);

            value = engine.CalculateValue(@"ifs(1 > 2, 2, 1 > 3, 4, false, 3, 10)");
            Assert.AreEqual(10m, value);

            value = engine.CalculateValue(@"ifs(1 > 2, 2, 1 > 3, 4, true, 3, 10)");
            Assert.AreEqual(3m, value);
        }
    }
}
