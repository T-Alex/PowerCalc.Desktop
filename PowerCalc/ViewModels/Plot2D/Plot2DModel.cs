using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.MathCore;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.WPF.Mvvm;
using TAlex.WPF.Mvvm.Commands;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class Plot2DModel : ViewModelBase
    {
        #region Fields

        protected readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        private List<TAlex.PowerCalc.Controls.Plot2D.Trace2D> _traces;
        private bool _closeSignal;

        #endregion

        #region Properties

        public ITraces2DState State
        {
            get;
            private set;
        }

        public List<TAlex.PowerCalc.Controls.Plot2D.Trace2D> Traces
        {
            get
            {
                return _traces;
            }

            set
            {
                Set(() => Traces, ref _traces, value);
            }
        }

        public bool CloseSignal
        {
            get
            {
                return _closeSignal;
            }

            set
            {
                Set(() => CloseSignal, ref _closeSignal, value);
            }
        }

        #endregion

        #region Commands

        public ICommand AcceptCommand { get; set; }

        #endregion

        #region Constructors

        public Plot2DModel(IExpressionTreeBuilder<Object> treeBuilder)
        {
            InitializeCommands();

            ExpressionTreeBuilder = treeBuilder;
            Traces = new List<Controls.Plot2D.Trace2D>();
        }

        #endregion

        #region Methods

        public void SetState(Trace2DMode mode)
        {
            switch (mode)
            {
                case Trace2DMode.Add:
                    State = new AddTraces2DState();
                    break;

                case Trace2DMode.Edit:
                    State = new EditTraces2DState();
                    break;
            }

            RaisePropertyChanged(() => State);
        }

        protected virtual void InitializeCommands()
        {
            AcceptCommand = new RelayCommand(Accept);
        }

        private void Accept()
        {
            foreach (var trace in Traces)
            {
                //if (!String.IsNullOrEmpty(trace.Expression))
                {
                    try
                    {
                        Expression<Object> expression = ExpressionTreeBuilder.BuildTree(trace.Expression);

                        Func<Object, Object> f = ParametricFunctionCreator.CreateOneParametricFunction(expression, "x");
                        Func<double, double> func = (x) =>
                        {
                            Complex result = (Complex)f((Complex)x);
                            return result.IsReal ? result.Re : double.NaN;
                        };
                        func(0.0);
                        trace.Trace = func;
                    }
                    catch (Exception exc)
                    {
                        //MessageBox.Show(this, exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                //else
                //{
                //    trace.Function = null;
                //}
            }

            CloseSignal = true;
        }

        #endregion
    }
}
