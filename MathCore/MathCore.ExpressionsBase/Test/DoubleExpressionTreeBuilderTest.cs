using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TAlex.MathCore.ExpressionEvaluation.Tokenize;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;


namespace TAlex.MathCore.ExpressionEvaluation.Test
{
    /// <summary>
    /// Summary description for DoubleExpressionTreeBuilder
    /// </summary>
    [TestClass]
    public class DoubleExpressionTreeBuilderTest
    {
        public DoubleExpressionTreeBuilderTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EvaluateTest_NullExpression()
        {
            string expression = null;
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double actual = target.Evaluate();
        }

        [TestMethod]
        public void EvaluateTest_Scalar()
        {
            string expression = "3";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 3;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_UnaryMinus()
        {
            string expression = "-3";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = -3;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_DoubleUnaryMinus()
        {
            string expression = "--3";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 3;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_Add()
        {
            string expression = "3 + 5";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 8;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_Sub()
        {
            string expression = "3 - 5";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = -2;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_Mult()
        {
            string expression = "3*5";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 15;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_Div()
        {
            string expression = "3/5/2";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 3.0 / 5 / 2;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_Pow()
        {
            string expression = "4^2^3";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 65536;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_Brackets()
        {
            string expression = "(2+3)*5";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = 25;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EvaluateTest_AddMult()
        {
            string expression = "3*5+16*-3";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);

            double expected = -33;
            double actual = target.Evaluate();

            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        [ExpectedException(typeof(SyntaxException))]
        public void EvaluateTest_NoExpressionSyntaxException()
        {
            string expression = "3+";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);
            double actual = target.Evaluate();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException))]
        public void EvaluateTest_BracketExpectedSyntaxException()
        {
            string expression = "3*(2+8";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);
            double actual = target.Evaluate();
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxException))]
        public void EvaluateTest_ExpressionPartSyntaxException()
        {
            string expression = "3+5{";
            Expression<double> target = new DoubleExpressionTreeBuilder().BuildTree(expression);
            double actual = target.Evaluate();
        }
    }
}
