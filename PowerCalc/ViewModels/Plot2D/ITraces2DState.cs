using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public interface ITraces2DState
    {
        string WindowCaption { get; }

        string AcceptButtonCaption { get; }

        bool ShowTracesList { get; }
    }
}
