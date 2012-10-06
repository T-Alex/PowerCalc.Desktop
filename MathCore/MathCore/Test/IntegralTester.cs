using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAlex.MathCore.Calculus.NumericalIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;


namespace TAlex.MathCore.Test
{
    public abstract class IntegralTester
    {
        public abstract double LowerBound { get; }

        public abstract double UpperBound { get; }

        public abstract Complex TargetFunction(Complex value);

        public abstract Complex ActualValue { get; }

        public abstract string IntegrandCaption { get; }


        public string GetTestResultInfo(Complex expected)
        {
            return String.Format("Target function: {0}, [{1}, {2}], expected: {3}, actual: {4}",
                IntegrandCaption, LowerBound, UpperBound, expected, ActualValue);
        }

        public void Test(ComplexIntegrator integrator)
        {
            integrator.LowerBound = LowerBound;
            integrator.UpperBound = UpperBound;
            integrator.Integrand = TargetFunction;

            Complex expected = integrator.Integrate();

            Trace.WriteLine(GetTestResultInfo(expected));
            Assert.IsTrue(NumericUtil.FuzzyEquals(expected, ActualValue, 10E-12));
        }

        public void Test(ComplexCompositeIntegrator integrator, double relativeTol)
        {
            integrator.LowerBound = LowerBound;
            integrator.UpperBound = UpperBound;
            integrator.Integrand = TargetFunction;
            integrator.Tolerance = relativeTol;

            Complex expected = integrator.Integrate();

            Trace.WriteLine(GetTestResultInfo(expected));
            Assert.IsTrue(NumericUtil.FuzzyEquals(expected, ActualValue, relativeTol));
        }
    }

    /// <summary>
    /// Testing the integration of the function sqrt(1-x^2) [0, 1].
    /// </summary>
    class PiFiniteIntegralTester : IntegralTester
    {
        public override double LowerBound
        {
            get { return 0.0; }
        }

        public override double UpperBound
        {
            get { return 1.0; }
        }

        public override Complex ActualValue
        {
            get
            {
                return Math.PI / 4.0;
            }
        }

        public override Complex TargetFunction(Complex value)
        {
            return Complex.Sqrt(1.0 - value * value);
        }

        public override string IntegrandCaption
        {
            get { return "sqrt(1-x^2)"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions log(x)*x.
    /// </summary>
    class FiniteIntegralTester2 : IntegralTester
    {
        public override double LowerBound
        {
            get { return -68; }
        }

        public override double UpperBound
        {
            get { return 20; }
        }

        public override Complex ActualValue
        {
            get
            {
                return (2 * UpperBound * UpperBound * Complex.Log(UpperBound) - UpperBound * UpperBound) / 4.0 -
                    (2 * LowerBound * LowerBound * Complex.Log(LowerBound) - LowerBound * LowerBound) / 4.0;
            }
        }

        public override Complex TargetFunction(Complex value)
        {
            return Complex.Log(value) * value;
        }

        public override string IntegrandCaption
        {
            get { return "log(x)*x"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions x.
    /// </summary>
    class XFiniteIntegralTester : IntegralTester
    {
        public override double LowerBound
        {
            get { return 0; }
        }

        public override double UpperBound
        {
            get { return 3000; }
        }

        public override Complex ActualValue
        {
            get { return UpperBound * UpperBound / 2.0 - LowerBound * LowerBound / 2.0; }
        }

        public override Complex TargetFunction(Complex value)
        {
            return value;
        }

        public override string IntegrandCaption
        {
            get { return "x"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions x^2.
    /// </summary>
    class SquareFiniteIntegralTester : IntegralTester
    {
        public override double LowerBound
        {
            get { return 0; }
        }

        public override double UpperBound
        {
            get { return 300; }
        }

        public override Complex ActualValue
        {
            get { return Complex.Pow(UpperBound, 3) / 3.0 - Complex.Pow(LowerBound, 3) / 3.0; }
        }

        public override Complex TargetFunction(Complex value)
        {
            return value * value;
        }

        public override string IntegrandCaption
        {
            get { return "x^2"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions sqrt(x).
    /// </summary>
    class SqrtFiniteIntegralTester : IntegralTester
    {
        public override double LowerBound
        {
            get { return -300; }
        }

        public override double UpperBound
        {
            get { return 405; }
        }

        public override Complex ActualValue
        {
            get { return 2.0 / 3.0 * Complex.Pow(UpperBound, 3.0 / 2.0) - 2.0 / 3.0 * Complex.Pow(LowerBound, 3.0 / 2.0); }
        }

        public override Complex TargetFunction(Complex value)
        {
            return Complex.Sqrt(value);
        }

        public override string IntegrandCaption
        {
            get { return "sqrt(x)"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions x*sin(x).
    /// </summary>
    class FiniteIntegralTester7 : IntegralTester
    {
        public override double LowerBound
        {
            get { return 0; }
        }

        public override double UpperBound
        {
            get { return 1000; }
        }

        public override Complex ActualValue
        {
            get { return (Complex.Sin(UpperBound) - UpperBound * Complex.Cos(UpperBound)) - (Complex.Sin(LowerBound) - LowerBound * Complex.Cos(LowerBound)); }
        }

        public override Complex TargetFunction(Complex value)
        {
            return value * Complex.Sin(value);
        }

        public override string IntegrandCaption
        {
            get { return "x*sin(x)"; }
        }
    }



    /// <summary>
    /// Testing the integration of functions sin(x)^6/x^5.
    /// </summary>
    class InfinityIntegralTester1 : IntegralTester
    {
        public override double LowerBound
        {
            get { return 0; }
        }

        public override double UpperBound
        {
            get { return double.PositiveInfinity; }
        }

        public override Complex ActualValue
        {
            get { return -2.0 * Complex.Log(2) + 27.0 / 16.0 * Complex.Log(3); }
        }

        public override Complex TargetFunction(Complex value)
        {
            return Complex.Pow(Complex.Sin(value), 6) / Complex.Pow(value, 5);
        }

        public override string IntegrandCaption
        {
            get { return "sin(x)^6/x^5"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions 1/(1+x^2).
    /// </summary>
    class InfinityIntegralTester2 : IntegralTester
    {
        public override double LowerBound
        {
            get { return 0; }
        }

        public override double UpperBound
        {
            get { return double.PositiveInfinity; }
        }

        public override Complex ActualValue
        {
            get { return Math.PI / 2.0; }
        }

        public override Complex TargetFunction(Complex value)
        {
            return 1.0 / (1.0 + value * value);
        }

        public override string IntegrandCaption
        {
            get { return "1/(1+x^2)"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions e^x.
    /// </summary>
    class ExpInfinityIntegralTester : IntegralTester
    {
        public override double LowerBound
        {
            get { return double.NegativeInfinity; }
        }

        public override double UpperBound
        {
            get { return 0; }
        }

        public override Complex ActualValue
        {
            get { return 1.0; }
        }

        public override Complex TargetFunction(Complex value)
        {
            return Complex.Exp(value);
        }

        public override string IntegrandCaption
        {
            get { return "e^x"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions 1/x^2.
    /// </summary>
    class InfinityIntegralTester4 : IntegralTester
    {
        public override double LowerBound
        {
            get { return 1; }
        }

        public override double UpperBound
        {
            get { return double.PositiveInfinity; }
        }

        public override Complex ActualValue
        {
            get { return 1.0; }
        }

        public override Complex TargetFunction(Complex value)
        {
            return 1.0 / (value * value);
        }

        public override string IntegrandCaption
        {
            get { return "1/x^2"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions e^(-x^2).
    /// </summary>
    class InfinityIntegralTester5 : IntegralTester
    {
        public override double LowerBound
        {
            get { return double.NegativeInfinity; }
        }

        public override double UpperBound
        {
            get { return double.PositiveInfinity; }
        }

        public override Complex ActualValue
        {
            get { return Math.Sqrt(Math.PI); }
        }

        public override Complex TargetFunction(Complex value)
        {
            return Complex.Exp(-(value * value));
        }

        public override string IntegrandCaption
        {
            get { return "e^(-x^2)"; }
        }
    }

    /// <summary>
    /// Testing the integration of functions x.
    /// </summary>
    class XInfinityIntegralTester : IntegralTester
    {
        public override double LowerBound
        {
            get { return double.NegativeInfinity; }
        }

        public override double UpperBound
        {
            get { return double.PositiveInfinity; }
        }

        public override Complex ActualValue
        {
            get { return 0.0; }
        }

        public override Complex TargetFunction(Complex value)
        {
            return value;
        }

        public override string IntegrandCaption
        {
            get { return "x"; }
        }
    }
}
