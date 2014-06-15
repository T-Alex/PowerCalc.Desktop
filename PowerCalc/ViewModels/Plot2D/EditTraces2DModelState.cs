using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class EditTraces2DModelState : ITraces2DModelState
    {
        #region ITraces2DState Members

        public string WindowCaption
        {
            get { return "Traces"; }
        }

        public string SaveButtonCaption
        {
            get { return "Save"; }
        }

        public bool ShowTraceList
        {
            get { return true; }
        }

        public void Save(Trace2DCollection source, IEnumerable<Trace2D> newTraces)
        {
            source.Update(newTraces);
        }

        #endregion
    }
}
