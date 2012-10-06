using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TAlex.MathCore;

namespace TAlex.MathCore.Test
{
    /// <summary>
    /// This is a test class for ComplexTest and is intended
    /// to contain all ComplexTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ComplexTest
    {
        #region Fields

        private TestContext testContextInstance;
        private RandomGenerator _rand = new RandomGenerator();

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

        private bool ComplexEquals(Complex c1, Complex c2, double TOL)
        {
            return (Math.Abs(c1.Re - c2.Re) < TOL) && (Math.Abs(c1.Im - c2.Im) < TOL);
        }


        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod()]
        public void AddTest()
        {
            Complex c1 = new Complex(3, 5);
            Complex c2 = new Complex(-6, 16);

            Complex expected = Complex.Add(c1, c2);
            Complex actual = new Complex(-3, 21);

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// A test for Sqrt
        /// </summary>
        [TestMethod()]
        public void SqrtTest_MinusOne()
        {
            Complex expected = Complex.Sqrt(-1);
            Complex actual = Complex.I;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Sqrt
        ///</summary>
        [TestMethod()]
        public void SqrtTest()
        {
            int n = 1000000;
            double TOL = 10E-11;

            for (int idx = 0; idx < n; idx++)
            {
                Complex number = _rand.NextComplex(-100000, 100000, 3);
                Complex sqrt = Complex.Sqrt(number);

                if (!ComplexEquals(sqrt * sqrt, number, TOL))
                {
                    Assert.Fail("Error: {0}", sqrt * sqrt - number);
                }
            }
        }

        /// <summary>
        ///A test for Sin and Cos
        ///</summary>
        [TestMethod()]
        public void SinCosTest()
        {
            int n = 10000;
            double TOL = 10E-8;

            for (int idx = 0; idx < n; idx++)
            {
                Complex number = _rand.NextComplex(-10000, 10000, -10, 10, 3);
                Complex sin = Complex.Sin(number);
                Complex cos = Complex.Cos(number);
                
                if (!ComplexEquals(sin * sin + cos * cos, 1.0, TOL))
                    Assert.Fail("Error: {0}", sin * sin + cos * cos - 1);
            }
        }

        /// <summary>
        ///A test for Sinh and Cosh
        ///</summary>
        [TestMethod()]
        public void SinhCoshTest()
        {
            int n = 1000000;
            double TOL = 10E-9;

            for (int idx = 0; idx < n; idx++)
            {
                Complex number = _rand.NextComplex(-8, 8, -10000, 10000, 3);
                Complex sinh = Complex.Sinh(number);
                Complex cosh = Complex.Cosh(number);

                if (!ComplexEquals(cosh * cosh - sinh * sinh, 1.0, TOL))
                {
                    Assert.Fail("Error: {0}", (cosh * cosh - sinh * sinh) - 1);
                }
            }
        }

        #endregion
    }
}
