using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TAlex.MathCore;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.Mvvm.Commands;
using TAlex.Mvvm.Services;
using TAlex.Mvvm.ViewModels;
using TAlex.PowerCalc.Properties;


namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class Traces2DModel : ViewModelBase
    {
        #region Fields

        protected readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;
        protected readonly IMessageService MessageService;
        protected Trace2DCollection OriginalTraces;

        private ObservableCollection<Trace2D> _traces;
        private bool _closeSignal;

        #endregion

        #region Properties

        public ITraces2DModelState State
        {
            get;
            private set;
        }

        public ObservableCollection<Trace2D> Traces
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

        public ICommand AddCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand DeleteAllCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructors

        public Traces2DModel(IExpressionTreeBuilder<Object> treeBuilder, IMessageService messageService)
        {
            InitializeCommands();

            ExpressionTreeBuilder = treeBuilder;
            MessageService = messageService;
            Traces = new ObservableCollection<Trace2D>();
        }

        #endregion

        #region Methods

        public void SetState(StateMode mode, Trace2DCollection traces)
        {
            switch (mode)
            {
                case StateMode.Add:
                    State = new AddTraces2DModelState();
                    break;

                case StateMode.Edit:
                    State = new EditTraces2DModelState();
                    break;
            }

            OriginalTraces = traces;
            Traces = new ObservableCollection<Trace2D>((mode == StateMode.Add) ?
                new List<Trace2D> { traces.CreateNew() } :
                traces.Cast<Trace2D>().Select(x => x.Clone())
            );
            
            RaisePropertyChanged(() => State);
        }

        public void UpdateTrace(Trace2D trace)
        {
            Expression<Object> expression = ExpressionTreeBuilder.BuildTree(trace.Expression);

            Func<Object, Object> f = ParametricFunctionCreator.CreateOneParametricFunction(expression, "x");
            Func<double, double> func = (x) =>
            {
                try
                {
                    Complex result = (Complex)f((Complex)x);
                    return result.IsReal ? result.Re : double.NaN;
                }
                catch (Exception)
                {
                    return double.NaN;
                }
            };
            trace.Trace = func;
        }

        private bool UpdateTraces()
        {
            foreach (var trace in Traces)
            {
                try
                {
                    UpdateTrace(trace);
                }
                catch (Exception exc)
                {
                    MessageService.Show(exc.Message, Properties.Resources.MessageBoxCaptionText, MessageButton.OK, MessageImage.Error);
                    trace.Trace = null;
                    return false;
                }
            }

            return true;
        }

        protected virtual void InitializeCommands()
        {
            AddCommand = new RelayCommand(AddNew);
            DeleteCommand = new RelayCommand<Trace2D>(Delete);
            DeleteAllCommand = new RelayCommand(DeleteAll, CanDeleteAll);
            SaveCommand = new RelayCommand(Save);
        }

        private void AddNew()
        {
            if (UpdateTraces())
            {
                Traces.Add(OriginalTraces.CreateNew(Traces.Count));
            }
        }

        private void Delete(Trace2D trace)
        {
            Traces.Remove(trace);
        }

        private bool CanDeleteAll()
        {
            return Traces.Count > 0;
        }

        private void DeleteAll()
        {
            if (MessageService.Show(Resources.DeleteAllTraces2DQuestion, Resources.MessageBoxCaptionText,
                MessageButton.YesNo, MessageImage.Question) == MessageResult.Yes)
            {
                Traces.Clear();
            }
        }

        private void Save()
        {
            if (UpdateTraces())
            {
                State.Save(OriginalTraces, _traces);
                CloseSignal = true;
            }
        }

        #endregion

        #region Nested Types

        public enum StateMode
        {
            Add,
            Edit
        }

        #endregion
    }
}
