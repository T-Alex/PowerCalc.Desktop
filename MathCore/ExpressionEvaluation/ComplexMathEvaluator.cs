//---------------------------------------------------------------------------
//
// (c) Copyright T-Alex Software Corporation.
// All rights reserved.
//
// This file is part of the MathCore project.
//
//---------------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

using TAlex.MathCore;
using TAlex.MathCore.Calculus;
using TAlex.MathCore.Calculus.NumericalIntegration;
using TAlex.MathCore.EquationSolvers;
using TAlex.MathCore.Graphing;
using TAlex.MathCore.Interpolation;
using TAlex.MathCore.LinearAlgebra;
using TAlex.MathCore.SpecialFunctions;
using TAlex.MathCore.Statistics;
using TAlex.MathCore.Statistics.Distributions;

namespace TAlex.MathCore.ExpressionEvaluation
{
    /// <summary>
    /// Class that calculates mathematical expressions.
    /// </summary>
    public class ComplexMathEvaluator : IEvaluator<Object>
    {
        #region Fields

        /// <summary>
        /// Contains a mathematical expression.
        /// </summary>
        protected string _expr;

        /// <summary>
        /// 
        /// </summary>
        protected Token[] _tokens;

        /// <summary>
        /// 
        /// </summary>
        protected int _currToken;

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, Object> _vars = new Dictionary<string, Object>();

        /// <summary>
        /// 
        /// </summary>
        protected static Random _rand = new Random();

        /// <summary>
        /// 
        /// </summary>
        protected static readonly Dictionary<string, Complex> _consts = new Dictionary<string, Complex>()
        {
            { "pi", Math.PI },
            { "e", Math.E },
            { "goldrat", ExMath.GoldenRatio },
            { "euler", ExMath.EulersConstant },
            { "catalan", ExMath.CatalansConstant },
            { "sqrt2", ExMath.Sqrt2 },
            { "sqrt3", ExMath.Sqrt3 },
            { "maxval", 1E307},
            { "minval", -1E307},
            { "inf", double.PositiveInfinity }
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets the expression to evaluation.
        /// </summary>
        public string Expression
        {
            get
            {
                return _expr;
            }
        }

        /// <summary>
        /// Gets the collection of variables.
        /// </summary>
        public Dictionary<string, Object> Variables
        {
            get
            {
                return _vars;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public ComplexMathEvaluator(string expression)
        {
            if (expression == null || expression.Length == 0)
                throw new SyntaxException("No expression for evaluation.");

            _expr = expression;
            _tokens = GetTokens(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variables"></param>
        public ComplexMathEvaluator(string expression, Dictionary<string, Object> variables)
        {
            if (expression == null || expression.Length == 0)
                throw new SyntaxException("No expression for evaluation.");

            _expr = expression;

            foreach (KeyValuePair<string, Object> pair in variables)
                _vars[pair.Key] = pair.Value;

            _tokens = GetTokens(expression);
        }

        #endregion

        #region Methods

        #region Statics

        public static Function1Real CreateRealFunction(string expression, string variableName)
        {
            return new FunctionCreator(expression).CreateRealFunction(variableName);
        }

        public static Function1Complex CreateComplexFunction(string expression, string variableName)
        {
            return new FunctionCreator(expression).CreateComplexFunction(variableName);
        }

        public static Function2Real CreateBivariateRealFunction(string expression, string variableName1, string variableName2)
        {
            return new FunctionCreator(expression).CreateBivariateRealFunction(variableName1, variableName2);
        }

        #endregion

        #region Dynamics

        /// <summary>
        /// Returns the result of evaluation expression.
        /// </summary>
        /// <returns>A System.Object containing the result of evaluation.</returns>
        public Object Evaluate()
        {
            _currToken = -1;
            Object result = AddSub();

            if (_currToken != _tokens.Length - 1)
            {
                throw new SyntaxException();
            }

            return result;
        }

        /// <summary>
        /// 1-st step of recursive descent:
        /// Additive operations (addition and subtraction).
        /// (left to right).
        /// </summary>
        /// <returns></returns>
        private Object AddSub()
        {
            Object left = MultDiv();
            Object right;

	        while(true)
            {
		        switch (_tokens[_currToken].Value)
                {
                    case "+":
                        right = MultDiv();

                        if (left is Complex)
                        {
                            if (right is Complex)
                                left = (Complex)left + (Complex)right;
                            else if (right is CMatrix)
                                left = (Complex)left + (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '+'.");
                        }
                        else if (left is CMatrix)
                        {
                            if (right is Complex)
                                left = (CMatrix)left + (Complex)right;
                            else if (right is CMatrix)
                                left = (CMatrix)left + (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '+'.");
                        }
                        else
                        {
                            throw new MathException("Wrong type arguments operation '+'.");
                        }

                        break;

                    case "-":
                        right = MultDiv();

                        if (left is Complex)
                        {
                            if (right is Complex)
                                left = (Complex)left - (Complex)right;
                            else if (right is CMatrix)
                                left = (Complex)left - (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '-'.");
                        }
                        else if (left is CMatrix)
                        {
                            if (right is Complex)
                                left = (CMatrix)left - (Complex)right;
                            else if (right is CMatrix)
                                left = (CMatrix)left - (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '-'.");
                        }
                        else
                        {
                            throw new MathException("Wrong type arguments operation '-'.");
                        }

                        break;

		            default:
                        return left;
		        }
	        }
        }

        /// <summary>
        /// 2-nd step of recursive descent:
        /// Multiplicative operations (multiplication and division).
        /// (left to right).
        /// </summary>
        /// <returns></returns>
        private Object MultDiv()
        {
            Object left = RaiseToPower();
            Object right;

	        while (true) 
	        {
                switch (_tokens[_currToken].Value)
		        {
                    case "*":
                        right = RaiseToPower();

                        if (left is Complex)
                        {
                            if (right is Complex)
                                left = (Complex)left * (Complex)right;
                            else if (right is CMatrix)
                                left = (Complex)left * (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '*'.");
                        }
                        else if (left is CMatrix)
                        {
                            if (right is Complex)
                                left = (CMatrix)left * (Complex)right;
                            else if (right is CMatrix)
                                left = (CMatrix)left * (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '*'.");
                        }
                        else
                        {
                            throw new MathException("Wrong type arguments operation '*'.");
                        }

                        break;

		            case "/":
                        right = RaiseToPower();

                        if (left is Complex)
                        {
                            if (right is Complex)
                                left = (Complex)left / (Complex)right;
                            else if (right is CMatrix)
                                left = (Complex)left / (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '/'.");
                        }
                        else if (left is CMatrix)
                        {
                            if (right is Complex)
                                left = (CMatrix)left / (Complex)right;
                            else if (right is CMatrix)
                                left = (CMatrix)left / (CMatrix)right;
                            else
                                throw new MathException("Wrong type arguments operation '/'.");
                        }
                        else
                        {
                            throw new MathException("Wrong type arguments operation '/'.");
                        }

			            break;

		            default:
                        return left;
		        }
	        }
        }

        /// <summary>
        /// 3-rd step of recursive descent:
        /// Raising to power.
        /// (right to left).
        /// </summary>
        /// <returns></returns>
        private Object RaiseToPower()
        {
            Object left = OtherOps();
            Object right;

            while (true)
            {
                switch (_tokens[_currToken].Value)
                {
                    case "^":
                        right = RaiseToPower();

                        if (left is Complex)
                        {
                            if (right is Complex)
                            {
                                Complex c2 = (Complex)right;

                                if (c2.IsReal)
                                {
                                    if (ExMath.IsInt32(c2.Re))
                                    {
                                        left = Complex.Pow((Complex)left, (int)c2.Re);
                                    }
                                    else
                                    {
                                        left = Complex.Pow((Complex)left, c2.Re);
                                    }
                                }
                                else
                                {
                                    left = Complex.Pow((Complex)left, c2);
                                }
                            }
                            else
                            {
                                throw new MathException("Wrong type arguments operation '^'.");
                            }
                        }
                        else if (left is CMatrix)
                        {
                            if (right is Complex)
                            {
                                if (((CMatrix)left).IsVector)
                                {
                                    left = CMatrix.Pow((CMatrix)left, (Complex)right);
                                }
                                else if (((CMatrix)left).IsSquare)
                                {
                                    left = CMatrix.Pow((CMatrix)left, Convert.ToInt32(right));
                                }
                                else
                                {
                                    throw new ArgumentException("The matrix must be square or column vector.");
                                }
                            }
                            else
                            {
                                throw new MathException("Wrong type arguments operation '^'.");
                            }
                        }
                        else
                        {
                            throw new MathException("Wrong type arguments operation '^'.");
                        }

                        break;

                    default:
                        return left;
                }
            }
        }

        /// <summary>
        /// 4-th step of recursive descent:
        /// Various opperations
        /// </summary>
        /// <returns></returns>
        private Object OtherOps()
        {
            Object value;
            _currToken++;

            switch (_tokens[_currToken].Type)
	        {
                case TokenType.Number:
                {
                    if (_tokens[_currToken].Value[_tokens[_currToken].Value.Length - 1] == 'i')
                        value = Complex.FromRealImaginary(0.0, Double.Parse(_tokens[_currToken].Value.Substring(0, _tokens[_currToken].Value.Length - 1), NumberFormatInfo.InvariantInfo));
                    else
                        value = Complex.FromReal(Convert.ToDouble(_tokens[_currToken].Value));

                    _currToken++;
			        return value;
                }
                
                case TokenType.String:
                {
                    value = _tokens[_currToken].Value;
                    _currToken++;
                    return value;
                }

                case TokenType.Variable:
                {
                    Object o;

                    if (_vars.TryGetValue(_tokens[_currToken].Value, out o))
                    {
                        value = o;
                        _currToken++;
                        return value;
                    }
                    else
                    {
                        throw new SyntaxException(String.Format("Variable \"{0}\" not found.", _tokens[_currToken].Value));
                    }
                }

                case TokenType.Constant:
                {
                    value = _consts[_tokens[_currToken].Value];
                    _currToken++;
                    return value;
                }

                case TokenType.Operator:
                {
                    switch (_tokens[_currToken].Value)
                    {
                        case "-":
                            value = RaiseToPower();

                            if (value is Complex)
                                return Complex.Negate((Complex)value);
                            else if (value is CMatrix)
                                return CMatrix.Negate((CMatrix)value);
                            else
                                throw new MathException("Wrong type argument operation '-'.");

                        case "+":
                            return RaiseToPower();

				        case "(":
						    value = AddSub();

                            if (_tokens[_currToken].Value == ")")
                            {
                                _currToken++;
                                return value;
                            }
                            else if (_tokens[_currToken].Value == ",")
                            {
                                List<Object> agrs = new List<object>();
                                agrs.Add(value);

                                while (_tokens[_currToken].Value != ")" && _tokens[_currToken].Value == ",")
                                {
                                    value = AddSub();
                                    agrs.Add(value);
                                }

                                if (_tokens[_currToken].Value != ")")
                                    throw new SyntaxException("\")\" expected.");

                                _currToken++;
                                return agrs.ToArray();
                            }
                            else
                            {
                                throw new SyntaxException("\")\" expected.");
                            }

                        case "{":
                            value = AddSub();

                            if (_tokens[_currToken].Value == "}")
                            {
                                _currToken++;
                                return new CMatrix( new Complex[]{(Complex)value} );
                            }
                            else if (_tokens[_currToken].Value == "," || _tokens[_currToken].Value == ";")
                            {
                                List<List<Complex>> m = new List<List<Complex>>();
                                m.Add(new List<Complex>());
                                m[m.Count - 1].Add((Complex)value);

                                while (_tokens[_currToken].Value != "}" && (_tokens[_currToken].Value == "," || _tokens[_currToken].Value == ";"))
                                {
                                    if (_tokens[_currToken].Value == ";") m.Add(new List<Complex>());

                                    value = AddSub();
                                    m[m.Count - 1].Add((Complex)value);
                                }

                                if (_tokens[_currToken].Value != "}")
                                    throw new SyntaxException("\"}\" expected.");

                                _currToken++;

                                CMatrix matrix = new CMatrix(m.Count, m[0].Count);

                                for (int i = 0; i < m.Count; i++)
                                {
                                    if (m[i].Count != m[0].Count)
                                        throw new MatrixSizeMismatchException();

                                    for (int j = 0; j < m[0].Count; j++)
                                        matrix[i, j] = m[i][j];
                                }

                                return matrix;
                            }
                            else
                            {
                                throw new SyntaxException("\"}\" expected.");
                            }

                        default:
                            throw new SyntaxException(String.Format("Incorrect operator \"{0}\".", _tokens[_currToken].Value));
			        }
                }

                case TokenType.Function:
		        {
                    string funcName = _tokens[_currToken].Value;
                    value = OtherOps();
                    return FuncEval(funcName, value);
		        }

	            default:
                    throw new SyntaxException("No expression.");
	        }
        }

        /// <summary>
        /// Returns the value of the function on the given name and argument of the function.
        /// </summary>
        /// <param name="funcName">A System.String representing the function name.</param>
        /// <param name="arg">
        /// An object representing argument of the function
        /// or <see cref="System.Collections.Generic.List<object>"/> representing arguments of the function.
        /// </param>
        /// <returns></returns>
        protected virtual Object FuncEval(string funcName, object arg)
        {
            Object[] args = null;

            switch (funcName)
            {
                #region Basics

                case "sign": return (Complex)Complex.Sign((Complex)arg);
                case "abs": return (Complex)Complex.Abs((Complex)arg);

                case "sqr":
                    if (arg is Complex)
                        return Complex.Pow((Complex)arg, 2);
                    else if (arg is CMatrix)
                        return CMatrix.Pow((CMatrix)arg, 2);
                    else
                        throw new ArgumentException();

                case "cube":
                    if (arg is Complex)
                        return Complex.Pow((Complex)arg, 3);
                    else if (arg is CMatrix)
                        return CMatrix.Pow((CMatrix)arg, 3);
                    else
                        throw new ArgumentException();

                case "inv":
                    if (arg is Complex)
                        return Complex.Inverse((Complex)arg);
                    else if (arg is CMatrix)
                        return CMatrix.Inverse((CMatrix)arg);
                    else
                        throw new ArgumentException();

                case "sqrt":
                    if (arg is Complex)
                        return Complex.Sqrt((Complex)arg);
                    else if (arg is CMatrix)
                        return CMatrix.Sqrt((CMatrix)arg);
                    else
                        throw new ArgumentException();

                case "nthroot":
                    args = (Object[])arg;
                    return Complex.Pow((Complex)args[0], 1.0 / (Complex)args[1]);

                case "powten": return Complex.Pow(10.0, (Complex)arg);

                case "mod":
                    args = (Object[])arg;
                    return (Complex)(Convert.ToDouble(args[0]) % Convert.ToDouble(args[1]));

                case "int": return ExMath.IntPart((Complex)arg);
                case "frac": return ExMath.FracPart((Complex)arg);

                case "floor": return Complex.Floor((Complex)arg);
                case "ceil": return Complex.Ceiling((Complex)arg);
                case "trunc": return Complex.Truncate((Complex)arg);

                case "round":
                    if (arg is Complex)
                    {
                        return Complex.Round((Complex)arg);
                    }
                    else if (arg is Object[])
                    {
                        args = (Object[])arg;
                        return Complex.Round((Complex)args[0], Convert.ToInt32(args[1]));
                    }
                    else
                    {
                        throw new ArgumentException();
                    }


                #endregion

                #region Complex numbers

                case "Re": return (Complex)((Complex)arg).Re;
                case "Im": return (Complex)((Complex)arg).Im;
                case "arg": return (Complex)Complex.Arg((Complex)arg);
                case "conj": return Complex.Conjugate((Complex)arg);

                #endregion

                #region Graphing

                case "cart2pol":
                    args = (Object[])arg;
                    return new CMatrix(Mapping.CartesianToPolar(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])));

                case "pol2cart":
                    args = (Object[])arg;
                    return new CMatrix(Mapping.PolarToCartesian(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])));

                case "cart2sph":
                    args = (Object[])arg;
                    return new CMatrix(Mapping.CartesianToSpherical(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]), Convert.ToDouble(args[2])));

                case "sph2cart":
                    args = (Object[])arg;
                    return new CMatrix(Mapping.SphericalToCartesian(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]), Convert.ToDouble(args[2])));

                case "cart2cyl":
                    args = (Object[])arg;
                    return new CMatrix(Mapping.CartesianToCylindrical(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]), Convert.ToDouble(args[2])));

                case "cyl2cart":
                    args = (Object[])arg;
                    return new CMatrix(Mapping.CylindricalToCartesian(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]), Convert.ToDouble(args[2])));

                #endregion

                #region Interpolation

                case "interp":
                    args = (Object[])arg;
                    return (Complex)new NewtonPolynomialInterpolator(Convert.ToDoubleArray(args[0]),
                        Convert.ToDoubleArray(args[1])).Interpolate(Convert.ToDouble(args[2]));

                case "linterp":
                    args = (Object[])arg;
                    return (Complex)new LinearInterpolator(Convert.ToDoubleArray(args[0]),
                        Convert.ToDoubleArray(args[1])).Interpolate(Convert.ToDouble(args[2]));

                #endregion

                #region Statistics

                case "sum": return MathStats.Sum(Convert.ToComplexArray((CMatrix)arg, true));
                case "sumsq": return MathStats.SumOfSquares(Convert.ToComplexArray((CMatrix)arg, true));
                case "prod": return MathStats.Product(Convert.ToComplexArray((CMatrix)arg, true));
                case "mean": return MathStats.Mean(Convert.ToComplexArray((CMatrix)arg, true));
                case "gmean": return (Complex)MathStats.GeometricMean(Convert.ToDoubleArray((CMatrix)arg, true));
                case "hmean": return (Complex)MathStats.HarmonicMean(Convert.ToDoubleArray((CMatrix)arg, true));
                case "median": return (Complex)MathStats.Median(Convert.ToDoubleArray((CMatrix)arg, true));
                case "mode": return MathStats.Mode(Convert.ToComplexArray((CMatrix)arg, true));
                case "pvar": return (Complex)MathStats.PopulationVariance(Convert.ToComplexArray((CMatrix)arg, true));
                case "svar": return (Complex)MathStats.SampleVariance(Convert.ToComplexArray((CMatrix)arg, true));
                case "pstdev": return (Complex)MathStats.PopulationStandardDeviation(Convert.ToComplexArray((CMatrix)arg, true));
                case "sstdev": return (Complex)MathStats.SampleStandardDeviation(Convert.ToComplexArray((CMatrix)arg, true));
                case "pskew": return MathStats.PopulationSkewness(Convert.ToComplexArray((CMatrix)arg, true));
                case "sskew": return MathStats.SampleSkewness(Convert.ToComplexArray((CMatrix)arg, true));
                case "pkurt": return (Complex)MathStats.PopulationKurtosis(Convert.ToComplexArray((CMatrix)arg, true));
                case "skurt": return (Complex)MathStats.SampleKurtosis(Convert.ToComplexArray((CMatrix)arg, true));

                case "moment":
                    args = (Object[])arg;
                    return MathStats.PopulationMoment(Convert.ToComplexArray((CMatrix)args[0], true), Convert.ToInt32(args[1]));

                case "cmoment":
                    args = (Object[])arg;
                    return MathStats.PopulationCentralMoment(Convert.ToComplexArray((CMatrix)args[0], true), Convert.ToInt32(args[1]));

                case "pcov":
                    args = (Object[])arg;
                    return MathStats.PopulationCovariance(Convert.ToComplexArray((CMatrix)args[0], true), Convert.ToComplexArray((CMatrix)args[1], true));

                case "scov":
                    args = (Object[])arg;
                    return MathStats.SampleCovariance(Convert.ToComplexArray((CMatrix)args[0], true), Convert.ToComplexArray((CMatrix)args[1], true));

                case "corr":
                    args = (Object[])arg;
                    return MathStats.Correlation(Convert.ToComplexArray((CMatrix)args[0], true), Convert.ToComplexArray((CMatrix)args[1], true));

                case "hist":
                    if (arg is CMatrix)
                    {
                        return (CMatrix)MathStats.Histogram(Convert.ToDoubleArray((CMatrix)arg, true));
                    }
                    else if (arg is Object[])
                    {
                        args = (Object[])arg;

                        if (args.Length == 2)
                        {
                            if (args[1] is Complex)
                            {
                                return (CMatrix)MathStats.Histogram(Convert.ToDoubleArray((CMatrix)args[0], true), Convert.ToInt32(args[1]));
                            }
                            else if (args[1] is CMatrix)
                            {
                                return (CMatrix)MathStats.Histogram(Convert.ToDoubleArray((CMatrix)args[0], true), Convert.ToDoubleArray((CMatrix)args[1]));
                            }
                            {
                                throw new ArgumentException();
                            }
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }

                #region Distributions

                case "rnd":
                    if ((arg is Complex) && (((Complex)arg).IsReal))
                    {
                        return (Complex)UniformDistribution.GetRandomVariable(_rand, ((Complex)arg).Re);
                    }
                    else if (arg is Object[])
                    {
                        args = (Object[])arg;
                        return (Complex)UniformDistribution.GetRandomVariable(_rand, Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));
                    }
                    else
                        throw new ArgumentException();

                case "unifpdf":
                    args = (Object[])arg;
                    return (Complex)new UniformDistribution(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])).ProbabilityDensityFunction(Convert.ToDouble(args[2]));

                case "unifcdf":
                    args = (Object[])arg;
                    return (Complex)new UniformDistribution(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])).CumulativeDistributionFunction(Convert.ToDouble(args[2]));

                case "normpdf":
                    args = (Object[])arg;
                    return (Complex)new NormalDistribution(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])).ProbabilityDensityFunction(Convert.ToDouble(args[2]));

                case "normcdf":
                    args = (Object[])arg;
                    return (Complex)new NormalDistribution(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])).CumulativeDistributionFunction(Convert.ToDouble(args[2]));

                case "exppdf":
                    args = (Object[])arg;
                    return (Complex)new ExponentialDistribution(Convert.ToDouble(args[0])).ProbabilityDensityFunction(Convert.ToDouble(args[1]));

                case "expcdf":
                    args = (Object[])arg;
                    return (Complex)new ExponentialDistribution(Convert.ToDouble(args[0])).CumulativeDistributionFunction(Convert.ToDouble(args[1]));

                #endregion

                #endregion

                #region Calculus

                case "integ":
                    args = (Object[])arg;
                    return new ComplexAdaptiveIntegrator(
                        CreateComplexFunction((String)args[0], (String)args[3]),
                        Convert.ToDouble(args[1]), Convert.ToDouble(args[2])).Integrate();

                case "derive":
                    args = (Object[])arg;
                    return NumericalDerivation.FirstDerivative(
                        CreateComplexFunction((string)args[0], (string)args[2]),
                        (Complex)args[1]);

                case "derive2":
                    args = (Object[])arg;
                    return NumericalDerivation.SecondDerivative(
                        CreateComplexFunction((string)args[0], (string)args[2]),
                        (Complex)args[1]);

                case "derive3":
                    args = (Object[])arg;
                    return NumericalDerivation.ThirdDerivative(
                        CreateComplexFunction((string)args[0], (string)args[2]),
                        (Complex)args[1]);

                case "derive4":
                    args = (Object[])arg;
                    return NumericalDerivation.FourthDerivative(
                        CreateComplexFunction((string)args[0], (string)args[2]),
                        (Complex)args[1]);

                case "sums":
                    args = (Object[])arg;
                    return (Complex)Sequence.Summation(
                        CreateComplexFunction((string)args[0], (string)args[3]),
                        Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

                case "prods":
                    args = (Object[])arg;
                    return (Complex)Sequence.Product(
                        CreateComplexFunction((string)args[0], (string)args[3]),
                        Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

                #endregion

                #region Polynomials

                case "polyroots":
                    return new CMatrix(CPolynomial.Roots(Convert.ToCPolynomial(arg)));

                case "polyval":
                    args = (Object[])arg;

                    if (args.Length == 2)
                    {
                        CPolynomial p = Convert.ToCPolynomial(args[0]);

                        if (args[1] is Complex)
                            return p.Evaluate(Convert.ToComplex(args[1]));
                        else if (args[1] is CMatrix)
                            return p.Evaluate((CMatrix)args[1]);
                        else
                            throw new ArgumentException();
                    }
                    else
                    {
                        throw new ArgumentException();
                    }

                case "polydr":
                    if (arg is Object[])
                    {
                        args = (Object[])arg;

                        return Convert.ToCMatrix(Convert.ToCPolynomial(args[0]).NthDerivative(Convert.ToInt32(args[1])));
                    }
                    else
                    {
                        return Convert.ToCMatrix(Convert.ToCPolynomial(arg).FirstDerivative());
                    }

                case "polyadr":
                    return Convert.ToCMatrix(Convert.ToCPolynomial(arg).Antiderivative());

                case "polyinterp":
                    args = (Object[])arg;
                    return Convert.ToCMatrix(CPolynomial.InterpolatingPolynomial(Convert.ToDoubleArray(args[0]), Convert.ToDoubleArray(args[1])));

                case "fromroots":
                    return Convert.ToCMatrix(CPolynomial.FromRoots(Convert.ToComplexArray(arg)));

                case "polyadd":
                    args = (Object[])arg;
                    return Convert.ToCMatrix(CPolynomial.Add(Convert.ToCPolynomial(args[0]), Convert.ToCPolynomial(args[1])));

                case "polysub":
                    args = (Object[])arg;
                    return Convert.ToCMatrix(CPolynomial.Subtract(Convert.ToCPolynomial(args[0]), Convert.ToCPolynomial(args[1])));

                case "polymul":
                    args = (Object[])arg;
                    return Convert.ToCMatrix(CPolynomial.Multiply(Convert.ToCPolynomial(args[0]), Convert.ToCPolynomial(args[1])));

                case "polydiv":
                    args = (Object[])arg;
                    return Convert.ToCMatrix(CPolynomial.Divide(Convert.ToCPolynomial(args[0]), Convert.ToCPolynomial(args[1])));

                case "polyrem":
                    args = (Object[])arg;
                    return Convert.ToCMatrix(CPolynomial.Modulus(Convert.ToCPolynomial(args[0]), Convert.ToCPolynomial(args[1])));

                #endregion

                #region Solving

                case "root":
                    args = (Object[])arg;

                    if (args.Length == 3)
                    {
                        return new ComplexMullerEquationSolver(
                            CreateComplexFunction((string)args[0], (string)args[2]),
                            (Complex)args[1]).Solve();
                    }
                    else if (args.Length == 4)
                    {
                        return (Complex)new BrentEquationSolver(
                            CreateRealFunction((string)args[0], (string)args[3]),
                            Convert.ToDouble(args[1]), Convert.ToDouble(args[2])).Solve();
                    }
                    else
                    {
                        throw new ArgumentException();
                    }

                #endregion

                #region Trigonometric

                case "sin": return Complex.Sin((Complex)arg);
                case "cos": return Complex.Cos((Complex)arg);
                case "tan": return Complex.Tan((Complex)arg);
                case "cot": return Complex.Cot((Complex)arg);
                case "sec": return Complex.Sec((Complex)arg);
                case "csc": return Complex.Csc((Complex)arg);

                case "asin": return Complex.Asin((Complex)arg);
                case "acos": return Complex.Acos((Complex)arg);
                case "atan": return Complex.Atan((Complex)arg);
                case "acot": return Complex.Acot((Complex)arg);
                case "asec": return Complex.Asec((Complex)arg);
                case "acsc": return Complex.Acsc((Complex)arg);

                case "atan2":
                    args = (Object[])arg;
                    return (Complex)Math.Atan2(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));

                case "vers": return Complex.Vers((Complex)arg);
                case "cvs": return Complex.Cvs((Complex)arg);
                case "hav": return Complex.Hav((Complex)arg);
                case "exsec": return Complex.Exsec((Complex)arg);
                case "excsc": return Complex.Excsc((Complex)arg);

                case "sinc": return Complex.Sinc((Complex)arg);
                case "tanc": return Complex.Tanc((Complex)arg);

                case "deg": return (Complex)ExMath.ToDegrees(Convert.ToDouble(arg));
                case "rad": return (Complex)ExMath.ToRadians(Convert.ToDouble(arg));

                #endregion

                #region Hyperbolic

                case "sinh": return Complex.Sinh((Complex)arg);
                case "cosh": return Complex.Cosh((Complex)arg);
                case "tanh": return Complex.Tanh((Complex)arg);
                case "coth": return Complex.Coth((Complex)arg);
                case "sech": return Complex.Sech((Complex)arg);
                case "csch": return Complex.Csch((Complex)arg);

                case "asinh": return Complex.Asinh((Complex)arg);
                case "acosh": return Complex.Acosh((Complex)arg);
                case "atanh": return Complex.Atanh((Complex)arg);
                case "acoth": return Complex.Acoth((Complex)arg);
                case "asech": return Complex.Asech((Complex)arg);
                case "acsch": return Complex.Acsch((Complex)arg);

                case "sinhc": return Complex.Sinhc((Complex)arg);
                case "tanhc": return Complex.Tanhc((Complex)arg);

                #endregion

                #region Log and Exponential

                case "ln": return Complex.Log((Complex)arg);

                case "log":
                    if (arg is Complex)
                        return Complex.Log10((Complex)arg);
                    else if (arg is Object[])
                    {
                        args = (Object[])arg;
                        return Complex.Log((Complex)args[0], Convert.ToDouble(args[1]));
                    }
                    else
                        throw new SyntaxException();

                case "exp":
                    if (arg is Complex)
                        return Complex.Exp((Complex)arg);
                    else if (arg is CMatrix)
                        return CMatrix.Exp((CMatrix)arg);
                    else
                        throw new ArgumentException();

                #endregion

                #region Special functions

                case "fact": return (Complex)Combinatorics.Factorial(Convert.ToInt32(arg));

                case "combin":
                    args = (Object[])arg;
                    return (Complex)Combinatorics.Combinations(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));

                case "permut":
                    args = (Object[])arg;
                    return (Complex)Combinatorics.Permutations(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));

                case "fib": return (Complex)Combinatorics.Fibonacci(Convert.ToInt32(arg));

                case "gcd":
                    args = (Object[])arg;
                    return (Complex)NumberTheory.GCD(Convert.ToInt32Array(args));

                case "lcm":
                    args = (Object[])arg;
                    return (Complex)NumberTheory.LCM(Convert.ToInt32Array(args));

                case "erf": return ProbabilityIntegrals.Erf((Complex)arg);
                case "erfc": return ProbabilityIntegrals.Erfc((Complex)arg);

                case "Si": return ExponentialIntegrals.SinIntegral((Complex)arg);
                case "Shi": return ExponentialIntegrals.SinhIntegral((Complex)arg);
                case "Ci": return ExponentialIntegrals.CosIntegral((Complex)arg);
                case "Chi": return ExponentialIntegrals.CoshIntegral((Complex)arg);
                case "Ei": return ExponentialIntegrals.ExpIntegral((Complex)arg);
                case "li": return ExponentialIntegrals.LogIntegral((Complex)arg);

                case "Gamma": return GammaFunctions.Gamma((Complex)arg);

                #endregion

                #region Linear Algebra

                case "det": return CMatrix.Determ((CMatrix)arg);

                case "lsolve":
                    args = (Object[])arg;
                    return CMatrix.Solve((CMatrix)args[0], (CMatrix)args[1]);

                case "pinv": return CMatrix.PseudoInverse((CMatrix)arg);
                case "tran": return ((CMatrix)arg).Transpose;
                case "adjoint": return ((CMatrix)arg).Adjoint;
                case "tr": return CMatrix.Trace((CMatrix)arg);
                case "rank": return (Complex)CMatrix.Rank(((CMatrix)arg));

                case "onenorm": return (Complex)CMatrix.OneNorm((CMatrix)arg);
                case "infnorm": return (Complex)CMatrix.InfinityNorm((CMatrix)arg);
                case "fronorm": return (Complex)CMatrix.FrobeniusNorm((CMatrix)arg);
                case "pnorm":
                    args = (Object[])arg;
                    return CMatrix.PNorm((CMatrix)args[0], Convert.ToDouble(args[1]));

                case "minor":
                    args = (Object[])arg;
                    return CMatrix.Minor((CMatrix)args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

                case "cofactor":
                    args = (Object[])arg;
                    return CMatrix.Cofactor((CMatrix)args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));

                case "matrix":
                    args = (Object[])arg;
                    return new CMatrix(Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));

                case "dot":
                    args = (Object[])arg;
                    return CMatrix.DotProduct((CMatrix)args[0], (CMatrix)args[1]);

                case "cross":
                    args = (Object[])arg;
                    return CMatrix.CrossProduct((CMatrix)args[0], (CMatrix)args[1]);

                case "diag": return CMatrix.Diagonal((CMatrix)arg);
                case "identity": return CMatrix.Identity(Convert.ToInt32(arg));
                case "length": return (Complex)((CMatrix)arg).Length;

                case "last":
                    if (arg is CMatrix)
                    {
                        if (((CMatrix)arg).IsVector)
                            return (Complex)(((CMatrix)arg).Length - 1);
                        else
                            throw new ArgumentException();
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                
                case "max": return ((CMatrix)arg).Max;
                case "min": return ((CMatrix)arg).Min;

                case "row":
                    args = (Object[])arg;
                    return ((CMatrix)args[0]).GetRow(Convert.ToInt32(args[1]));

                case "rows": return (Complex)((CMatrix)arg).RowCount;

                case "col":
                    args = (Object[])arg;
                    return ((CMatrix)args[0]).GetColumn(Convert.ToInt32(args[1]));
                case "cols": return (Complex)((CMatrix)arg).ColumnCount;

                case "submatrix":
                    args = (Object[])arg;
                    return ((CMatrix)args[0]).Submatrix(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]),
                        Convert.ToInt32(args[3]), Convert.ToInt32(args[4]));

                case "stack":
                    args = (Object[])arg;
                    return CMatrix.StackConcat((CMatrix)args[0], (CMatrix)args[1]);

                case "augment":
                    args = (Object[])arg;
                    return CMatrix.AugmentConcat((CMatrix)args[0], (CMatrix)args[1]);

                case "chol": return CMatrix.CholeskyDecomposition((CMatrix)arg);
                case "lu": return CMatrix.AugmentConcat(CMatrix.LUPDecomposition((CMatrix)arg));
                case "qr": return CMatrix.AugmentConcat(CMatrix.QRDecomposition((CMatrix)arg));

                case "svd": return new CSVD((CMatrix)arg).SVD;

                case "eigvals": return CMatrix.Eigenvalues((CMatrix)arg);
                case "eigvecs": return CMatrix.Eigenvectors((CMatrix)arg);
                case "leigvecs": return CMatrix.LeftEigenvectors((CMatrix)arg);

                case "sngvals": return CMatrix.Singularvalues((CMatrix)arg);

                case "char": return Convert.ToCMatrix(CMatrix.CharacteristicPolynomial((CMatrix)arg));

                case "elem":
                    args = (Object[])arg;
                    if (args.Length == 2)
                        return ((CMatrix)args[0])[Convert.ToInt32(args[1])];
                    else if (args.Length == 3)
                        return ((CMatrix)args[0])[Convert.ToInt32(args[1]), Convert.ToInt32(args[2])];
                    else
                        throw new ArgumentException();

                #endregion

                default:
                    throw new SyntaxException(String.Format("Undefined function \"{0}\".", funcName));
            }
        }

        /// <summary>
        /// Parses the expression into tokens and returns the result.
        /// </summary>
        /// <param name="expr">A System.String representing the expression to parse.</param>
        /// <returns></returns>
        protected static Token[] GetTokens(string expr)
        {
            int idx = 0;
            int len = expr.Length;
            List<Token> tokens = new List<Token>();

            while (idx <= len)
            {
                TokenType tokenType = TokenType.End;
                String tokenValue = String.Empty;

                // Skipping white space characters
                while (idx < len && Char.IsWhiteSpace(expr[idx]))
                {
                    idx++;
                }

                if (idx == len)
                {
                    tokenType = TokenType.End;
                    tokenValue = "$";
                    tokens.Add(new Token(tokenValue, tokenType));
                    break;
                }


                if (Char.IsDigit(expr[idx])) // Token type is Number
                {
                    tokenType = TokenType.Number;
                    while (Char.IsLetterOrDigit(expr[idx]) || (expr[idx] == '.'))
                    {
                        tokenValue += expr[idx++];
                        if (idx >= len) break;
                    }
                }
                else if (expr[idx] == '\'') // Token type is String
                {
                    tokenType = TokenType.String;
                    idx++;
                    while (expr[idx] != '\'')
                    {
                        tokenValue += expr[idx++];
                        if (idx >= len) break;
                    }
                    if (expr[idx++] == '\'') tokenValue += "";
                }
                else if ("+-*/^(),{};".IndexOf(expr[idx]) != -1) // Token type is Operator
                {
                    tokenType = TokenType.Operator;
                    tokenValue += expr[idx++];
                }
                else if (Char.IsLetter(expr[idx]) || expr[idx] == '_') // Token type is function or constant or variable
                {
                    while (Char.IsLetterOrDigit(expr[idx]) || expr[idx] == '_')
                    {
                        tokenValue += expr[idx++];
                        if (idx >= len) break;
                    }

                    if (idx < len && expr[idx] == '(')
                    {
                        tokenType = TokenType.Function;
                    }
                    else
                    {
                        if (_consts.ContainsKey(tokenValue))
                            tokenType = TokenType.Constant;
                        else
                            tokenType = TokenType.Variable;
                    }
                }
                else
                {
                    throw new SyntaxException(String.Format("Invalid character '{0}'.", expr[idx]));
                }

                tokens.Add(new Token(tokenValue, tokenType));
            }

            return tokens.ToArray();
        }

        #endregion

        #endregion

        #region Nested types

        protected struct Token
        {
            #region Fields

            public string Value;

            public TokenType Type;

            public Object Tag;

            #endregion

            #region Constructors

            public Token(string value, TokenType type)
            {
                Value = value;
                Type = type;

                Tag = null;
            }

            #endregion

            #region Methods

            public override string ToString()
            {
                return String.Format("Value: '{0}', type: {1}", Value, Type);
            }

            #endregion
        }

        protected enum TokenType
        {
            Constant,
            End,
            Function,
            Number,
            String,
            Operator,
            Variable
        }

        private class FunctionCreator
        {
            #region Fields

            /// <summary>
            /// 
            /// </summary>
            private string[] _variables;

            /// <summary>
            /// 
            /// </summary>
            private ComplexMathEvaluator _evaluator;

            #endregion

            #region Constructors

            public FunctionCreator(string expression)
            {
                _evaluator = new ComplexMathEvaluator(expression);
            }

            #endregion

            #region Methods

            public Function1Real CreateRealFunction(string variableName)
            {
                _variables = new string[1];
                _variables[0] = variableName;

                return RealFunction;
            }

            public Function1Complex CreateComplexFunction(string variableName)
            {
                _variables = new string[1];
                _variables[0] = variableName;

                return ComplexFunction;
            }

            public Function2Real CreateBivariateRealFunction(string variableName1, string variableName2)
            {
                _variables = new string[2];
                _variables[0] = variableName1;
                _variables[1] = variableName2;

                return BivariateRealFunction;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private double RealFunction(double value)
            {
                _evaluator._vars[_variables[0]] = (Complex)value;

                Complex result = (Complex)_evaluator.Evaluate();
                if (!result.IsReal) return double.NaN;
                else return result.Re;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private Complex ComplexFunction(Complex value)
            {
                _evaluator._vars[_variables[0]] = value;
                return (Complex)_evaluator.Evaluate();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value1"></param>
            /// <param name="value2"></param>
            /// <returns></returns>
            private double BivariateRealFunction(double value1, double value2)
            {
                _evaluator._vars[_variables[0]] = (Complex)value1;
                _evaluator._vars[_variables[1]] = (Complex)value2;

                return ((Complex)_evaluator.Evaluate()).Re;
            }

            #endregion
        }

        #endregion
    }
}
