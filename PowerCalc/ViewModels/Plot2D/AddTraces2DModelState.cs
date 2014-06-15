using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class AddTraces2DModelState : ITraces2DModelState
    {
        #region ITraces2DState Members

        public string WindowCaption
        {
            get
            {
                return "New Trace";
            }
        }

        public string SaveButtonCaption
        {
            get
            {
                return "Add";
            }
        }

        public bool ShowTraceList
        {
            get
            {
                return false;
            }
        }

        public void Save(Trace2DCollection source, IEnumerable<Trace2D> newTraces)
        {
            source.Add(newTraces.First());
        }

        #endregion
    }
}
