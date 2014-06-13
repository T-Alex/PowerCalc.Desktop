using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class EditTraces2DState : ITraces2DState
    {
        #region ITraces2DState Members

        public string WindowCaption
        {
            get { return "Traces"; }
        }

        public string AcceptButtonCaption
        {
            get { return "Save"; }
        }

        public bool ShowTracesList
        {
            get { return true; }
        }

        #endregion
    }
}
