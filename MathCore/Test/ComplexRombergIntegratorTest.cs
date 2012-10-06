using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAlex.MathCore.Calculus.NumericalIntegration;


namespace TAlex.MathCore.Test
{
    /// <summary>
    /// This is a test class for ComplexRombergIntegrator and is intended
    /// to contain all ComplexRombergIntegrator Unit Tests
    ///</summary>
    [TestClass()]
    public class ComplexRombergIntegratorTest
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
        /// A 2nd test for Integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_LogXX()
        {
            IntegralTester test = new FiniteIntegralTester2();
            test.Test(new ComplexRombergIntegrator(), 1E-6);
        }

        /// <summary>
        /// A 3rd test for Integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_X()
        {
            IntegralTester test = new XFiniteIntegralTester();
            test.Test(new ComplexRombergIntegrator(), 1E-15);
        }

        /// <summary>
        /// A 4 test for Integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_Square()
        {
            IntegralTester test = new SquareFiniteIntegralTester();
            test.Test(new ComplexRombergIntegrator(), 1E-15);
        }

        /// <summary>
        /// A 5 test for Integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_Sqrt()
        {
            IntegralTester test = new SqrtFiniteIntegralTester();
            test.Test(new ComplexRombergIntegrator(), 1E-5);
        }

        /// <summary>
        /// A 7 test for Integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_XSin()
        {
            IntegralTester test = new FiniteIntegralTester7();
            test.Test(new ComplexRombergIntegrator(), 1E-14);
        }

        #endregion
    }
}
