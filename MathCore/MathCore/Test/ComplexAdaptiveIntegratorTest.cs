using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TAlex.MathCore.Calculus.NumericalIntegration;

namespace TAlex.MathCore.Test
{
    /// <summary>
    /// This is a test class for ComplexAdaptiveIntegrator and is intended
    /// to contain all ComplexAdaptiveIntegrator Unit Tests
    ///</summary>
    [TestClass()]
    public class ComplexAdaptiveIntegratorTest
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
        /// A test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_Pi()
        {
            IntegralTester test = new PiFiniteIntegralTester();
            test.Test(new ComplexAdaptiveIntegrator() { MaxIterations = 8000 }, 1E-15);
        }

        /// <summary>
        /// A 2nd test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_LogXX()
        {
            IntegralTester test = new FiniteIntegralTester2();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 3rd test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_X()
        {
            IntegralTester test = new XFiniteIntegralTester();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 4 test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_Square()
        {
            IntegralTester test = new SquareFiniteIntegralTester();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 5 test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_Sqrt()
        {
            IntegralTester test = new SqrtFiniteIntegralTester();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 7 test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_XSin()
        {
            IntegralTester test = new FiniteIntegralTester7();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }


        /// <summary>
        /// A 1st test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_InfinityTest1()
        {
            IntegralTester test = new InfinityIntegralTester1();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 2nd test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_InfinityTest2()
        {
            IntegralTester test = new InfinityIntegralTester2();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 3rd test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_ExpInfinityTest()
        {
            IntegralTester test = new ExpInfinityIntegralTester();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 4 test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_InfinityTest4()
        {
            IntegralTester test = new InfinityIntegralTester4();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 5 test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_InfinityTest5()
        {
            IntegralTester test = new InfinityIntegralTester5();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        /// <summary>
        /// A 6 test for Integrate.
        ///</summary>
        [TestMethod()]
        public void IntegrateTest_XInfinityTest()
        {
            IntegralTester test = new XInfinityIntegralTester();
            test.Test(new ComplexAdaptiveIntegrator(), 1E-9);
        }

        #endregion
    }
}
