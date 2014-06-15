using System;
using System.Collections.Generic;


namespace TAlex.PowerCalc.ViewModels.Plot2D
{
    public interface ITraces2DModelState
    {
        string WindowCaption { get; }

        string SaveButtonCaption { get; }

        bool ShowTraceList { get; }

        void Save(Trace2DCollection source, IEnumerable<Trace2D> newTraces);
    }
}
