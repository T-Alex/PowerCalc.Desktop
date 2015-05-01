using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using TAlex.License;


namespace TAlex.PowerCalc.Licensing
{
    internal class AppLicense : LicenseBase
    {
        #region Fields

        private static readonly byte[] _SK = new byte[]
        {
            18, 52, 75, 11, 76, 14, 195, 87
        };

        private static readonly byte[] _IV = new byte[]
        {
            246, 198, 57, 228, 138, 4, 126, 118
        };

        private static List<string> _SKH = new List<string>()
        {
            "90znMZdYMIfh6hUCFkq6MGTTdCWmw6No+N9xRcyW6L/KEg6ILvhyqW8s0ExosYROzB9IUO0JAC+PloOqxcXJkw==",
	        "pWO8Ehuax0Em6mOtXUTfYEbr9gfQGYTKRE3fYjKunRzfty13eO5mRYiEbidukUleiT2RMZCbtYFuxp/g4c8XEg==",
	        "0Slc8kXkPg1pwxHNuOWdqx20+RojQ7kMFGc0xtzPdt7Jk53dci3J2qug3aMi5K4UN/IU9mM/jFjxZR+OKHitmg==",
	        "sccoARamiTI23tQNAdOmGg7vN2KrI/3eeZWN6oTt+CmnIcyhNMokvXw5hYKnIn3ERzCivRSESzArZrEbz2W4mw==",
	        "rp/pn7P9zcvVa3luiJaJqR1T3ACEH0yq2PN7oN/Hvgu2Lcx2dmBdudaKoDiW2C3OYaIC/bfGccom1f7jkbmmDw==",
	        "vJCNu/JS8SZ4+SSaLT7Vh5/MIEwbHjwaZDScrTQ+YvL8P8JBSr/Fvaossgx12p/GCwYnSURW9A5qULQpdr+F0g==",
	        "IHf2o79ONXI162iq0OrVXD3GYJDmyfAuiT+pI1/GdR9h+A90ueJc7dddvfj44IiyrO9+oKKlPGA3uxNmVxJ3hA==",
	        "6WLuNIohEzNszaSum3k5QIDbWDPoIa2+qWPd+ciKSgLk5xTz5eOu4YPy9c6aYG7qfx1BbK8IjQQx7tzXzXFyZg==",
	        "78AyoV4KutQyhkgjqAJj0eS742GRRF6fOHvmvCp/lpLV7I1/r2pj7O/Qs4UDVEpXVs0gMV4cQwnqlKJTvuP4+w==",
	        "oSd4725vd7JSY0frAlllF0TkSXAMP3Ce7ht6mLeRlL8BoRtMt3QxUSHQoAxezuAWvcabji4UW5zhWFtSEuOjvw==",
	        "2uWxEaDQ8oUZ/02OfU/OlMJ1TW6g7C120ljAqddXAAfGYNXBqFiOu3sN1JWJE71Gtbov73NADL86dQ3QpsKm7g==",
	        "tl0uQ33KGTOG4YBh+eTJBcu177G0vt10cgzpo/uzERUslF3Yqm1zIie33Srg/FOHmAne0M8EP2genp2yYQ+OxQ==",
	        "Luy7pJPSDri8ClTxgOBRkJDkkoLkQtuul31FU9Pf/nCoW6JvfZ8yGvqiQkZOXO7osU9kC4paINBFevANwI+4Bg==",
	        "O7AnPvhQGqZFFwx49Bay3rjkuxgDkOqwlk37YBLF6aahbXG2VbBJFlIMH+ClbwlUi/Uy0nwo35QlGL4MwHHbqQ==",
	        "7NkwRCc7hdq7HjIC6Ao+vR/pYq9Zj8/py9ECBwqB958txyUpnYT6b1o1SPB13ZtH0lQhHPYCQY3+Dosx8tcMmQ==",
	        "T/Hbepk5pCMWvMKcIv1/Dnl8DT9dwIrdp3z++mnRXQki7UNXlUF9HqMfWV1lfEutILYrOb/MKsyxdbzVHM5uWw==",
	        "R37SRlS9cam0m2Vf/OV1nStA0r4x3OOb8wpc2th8rLsqeF3CX4t0xZczhvl90h8VievGaEJ7OCsLyNsyMARBSg==",
	        "xDZ2y1eGCXktc0Yt/n6rX9soOrDfeeKfttfADBQe3jzyqgNUxrZ83hEZhHSrqU2zxP6jVxRGlxMPWnbhYJjOXA==",
	        "Nm5Pu43M2ToB+vZlKDpo5GYoSkHdFyksdVqXTnpEHeChB20AvPVXXV5vSMFt/PVvLi6cv7nP4k3KlQmXwwQh2g==",
	        "By4owzxduT9wcyc3JC2GREYygQVG+st8lU+sDFDi2MPvG+KHP197UOnknx21DNcS+1wGRdfcwIE+Miwlv+Kb0g=="
        };

        #endregion

        #region Properties

        public override int TrialPeriod
        {
            get
            {
                return 30;
            }
        }


        protected override byte[] IV
        {
            get { return _IV; }
        }

        protected override byte[] SK
        {
            get { return _SK; }
        }

        protected override List<string> SKH
        {
            get { return _SKH; }
        }

        #endregion

        #region Constructors

        public AppLicense(ILicenseDataManager licenseDataManager, ITrialPeriodDataProvider trialPeriodDataProvider)
            : base(licenseDataManager, trialPeriodDataProvider)
        {
        }

        #endregion
    }
}
