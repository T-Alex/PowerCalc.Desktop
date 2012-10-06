using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TAlex.PowerCalc.Licensing
{
    internal class SecretFileTrialPeriodDataProvider : TrialPeriodDataProvider
    {
        #region Fields

        private string _fileName = "vb986b73895v6b67b8xz.dat";

        #endregion

        #region Properties

        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }

        #endregion

        #region Methods

        public override int GetTrialDaysLeft(int trialPeriod)
        {
            int trialDaysLeft = trialPeriod;

            string fullPath = Path.Combine(Environment.SystemDirectory, _fileName);

            if (!File.Exists(fullPath))
            {
                File.Create(fullPath);
                File.SetAttributes(fullPath, FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReadOnly);
            }
            else
            {
                trialDaysLeft = trialPeriod - DateTime.Now.Subtract(File.GetCreationTime(fullPath)).Days;

                if (trialDaysLeft > trialPeriod)
                    trialDaysLeft = -1;

                if (File.ReadAllText(fullPath) == "/%")
                {
                    trialDaysLeft = -1;
                }
                else if (trialDaysLeft < 0)
                {
                    File.SetAttributes(fullPath, FileAttributes.Normal);
                    File.WriteAllText(fullPath, "/%");
                    File.SetAttributes(fullPath, FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReadOnly);
                }
            }

            return trialDaysLeft;
        }

        #endregion
    }
}
