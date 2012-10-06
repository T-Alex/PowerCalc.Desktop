using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TAlex.MathCore.SpecialFunctions;


namespace TAlex.MathCore.Test
{
    /// <summary>
    ///This is a test class for NumberTheoryTest and is intended
    ///to contain all NumberTheoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NumberTheoryTest
    {
        #region Fields

        private TestContext testContextInstance;

        private Random _rand = new Random();

        #endregion

        #region Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///A test for GCD
        ///</summary>
        [TestMethod()]
        public void GCDTest()
        {
            int n = 1000000;

            for (int i = 0; i < n; i++)
            {
                long a = _rand.Next(-100000, 100000);
                long b = _rand.Next(-100000, 100000);

                long gcd = NumberTheory.GCD(a, b);

                if (a % gcd != 0L || b % gcd != 0L)
                    Assert.Fail("a: {0} b: {1} gcd: {2}", a, b, gcd);
            }
        }

        #endregion
    }
}
