using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class Trace2DCollection : ICollection, IEnumerable
    {
        #region Fields

        protected readonly IPlot2D Plot;

        private List<Trace2D> _traces;

        private List<string> _colorSeries = new List<string>
            {
                "Blue",
                "Green",
                "Orange",
                "Purple",
                "Lime",
                "Magenta",
                "Teal",
                "Gold",
                "DimGray",
                "Cyan",
                "Navy",
                "Maroon",
                "Red",
                "Olive",
                "Silver",
                "LightSalmon",
                "Black"
            };

        #endregion

        #region Constructors

        public Trace2DCollection(IPlot2D plot)
        {
            Plot = plot;
            _traces = new List<Trace2D>();
        }

        public Trace2DCollection(IPlot2D plot, IEnumerable<Trace2D> traces)
            : this(plot)
        {
            _traces = new List<Trace2D>(traces);
        }

        #endregion

        #region Methods

        public Trace2D CreateNew()
        {
            return new Trace2D { Color = _colorSeries[_traces.Count % _colorSeries.Count] };
        }

        public void Add(Trace2D trace)
        {
            _traces.Add(trace);
            Plot.RenderPlot();
        }

        public void Update(IEnumerable<Trace2D> traces)
        {
            _traces = traces.Select(t => (Trace2D)t.Clone()).ToList();
            Plot.RenderPlot();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _traces.GetEnumerator();
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_traces).CopyTo(array, index);
        }

        public int Count
        {
            get { return _traces.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_traces).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_traces).SyncRoot; }
        }

        #endregion
    }
}
