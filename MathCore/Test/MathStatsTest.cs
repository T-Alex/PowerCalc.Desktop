using System;

using TAlex.MathCore;
using TAlex.MathCore.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TAlex.MathCore.Test
{
    /// <summary>
    ///This is a test class for StatisticsTest and is intended
    ///to contain all MathStatsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MathStatsTest
    {
        #region Fields

        private TestContext testContextInstance;

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
        ///A test for Mode
        ///</summary>
        [TestMethod()]
        public void ModeTest()
        {
            double[] v = new double[] {1, 2, 2, 3, 6, 8, 8, 8, 9};
            double expected = 8.0;
            double actual;
            actual = MathStats.Mode(v);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
