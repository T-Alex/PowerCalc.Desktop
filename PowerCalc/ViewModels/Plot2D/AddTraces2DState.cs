using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public class AddTraces2DState : ITraces2DState
    {
        #region ITraces2DState Members

        public string WindowCaption
        {
            get
            {
                return "New Trace";
            }
        }

        public string AcceptButtonCaption
        {
            get
            {
                return "Add";
            }
        }

        public bool ShowTracesList
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
