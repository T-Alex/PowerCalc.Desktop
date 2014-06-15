using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class Trace2D : ICloneable
    {
        #region Fields

        private double _lowerBound = -100;

        private bool _isLowerBoundUnlimited = true;

        private double _upperBound = 100;

        private bool _isUpperBoundUnlimited = true;

        private bool _display = true;

        private Func<double, double> _trace;

        #endregion

        #region Properties

        public string Expression { get; set; }

        public string Color { get; set; }

        public double LineThickness { get; set; }

        public double LowerBound
        {
            get
            {
                return _lowerBound;
            }

            set
            {
                _lowerBound = value;
            }
        }

        public bool IsLowerBoundUnlimited
        {
            get { return _isLowerBoundUnlimited; }
            set { _isLowerBoundUnlimited = value; }
        }

        public double UpperBound
        {
            get
            {
                return _upperBound;
            }

            set
            {
                _upperBound = value;
            }
        }

        public bool IsUpperBoundUnlimited
        {
            get { return _isUpperBoundUnlimited; }
            set { _isUpperBoundUnlimited = value; }
        }

        public bool Display
        {
            get
            {
                return _display;
            }

            set
            {
                _display = value;
            }
        }

        public Func<double, double> Trace
        {
            get
            {
                return _trace;
            }

            set
            {
                _trace = value;
            }
        }

        #endregion

        #region Constructors

        public Trace2D()
        {
            Color = "Blue";
            LineThickness = 1;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Expression;
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return new Trace2D
            {
                Expression = Expression,
                Color = Color,
                LineThickness = LineThickness,
                LowerBound = LowerBound,
                IsLowerBoundUnlimited = IsLowerBoundUnlimited,
                UpperBound = UpperBound,
                IsUpperBoundUnlimited = IsUpperBoundUnlimited,
                Display = Display,
                Trace = (Trace != null) ? (Func<double, double>)Trace.Clone() : null
            };
        }

        public Trace2D Clone()
        {
            return (Trace2D)((ICloneable)this).Clone();
        }

        #endregion
    }
}
