using System;

namespace TAlex.PowerCalc.Licensing
{
    /// <summary>
    /// Provides the abstract base class for implementing a trial data provider. 
    /// </summary>
    internal abstract class TrialPeriodDataProvider
    {
        public abstract int GetTrialDaysLeft(int trialPeriod);
    }
}
