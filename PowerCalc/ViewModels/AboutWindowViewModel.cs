using System;
using System.Reflection;
using System.Resources;
using System.Text;
using TAlex.Common.Environment;


namespace TAlex.PowerCalc.ViewModels
{
    public class AboutWindowViewModel
    {
        #region Fields

        protected readonly ApplicationInfo ApplicationInfo;
        internal ResourceManager ResourcesManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application's title.
        /// </summary>
        public string ProductTitle
        {
            get
            {
                return ApplicationInfo.Title;
            }
        }

        /// <summary>
        /// Gets the application's description.
        /// </summary>
        public string Description
        {
            get
            {
                return ApplicationInfo.Description;
            }
        }

        /// <summary>
        /// Gets the application's company.
        /// </summary>
        public string Company
        {
            get
            {
                return ApplicationInfo.Company;
            }
        }

        /// <summary>
        /// Gets the application's product.
        /// </summary>
        public string Product
        {
            get
            {
                return ApplicationInfo.Product;
            }
        }

        /// <summary>
        /// Gets the application's copyright.
        /// </summary>
        public string Copyright
        {
            get
            {
                return String.Format("{0}. All rights reserved.", ApplicationInfo.Copyright);
            }
        }

        /// <summary>
        /// Gets the application's version.
        /// </summary>
        public Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        /// <summary>
        /// Gets the email support title for this product.
        /// </summary>
        public string EmailTitle
        {
            get
            {
                return EmailAddress.Replace("mailto:", String.Empty);
            }
        }

        /// <summary>
        /// Gets the email support for this product.
        /// </summary>
        public string EmailAddress
        {
            get
            {
                return ResourcesManager.GetString("SupportEmail"); //PowerCalc.Properties.Resources.SupportEmail;
            }
        }

        /// <summary>
        /// Gets the homepage title of this product.
        /// </summary>
        public string HomepageTitle
        {
            get
            {
                return HomepageUrl.Replace("http://", String.Empty);
            }
        }

        /// <summary>
        /// Gets the homepage url of this product.
        /// </summary>
        public string HomepageUrl
        {
            get
            {
                return ResourcesManager.GetString("HomepageUrl"); // PowerCalc.Properties.Resources.HomepageUrl;
            }
        }

        /// <summary>
        /// Gets the license name for this product.
        /// </summary>
        public string LicenseName
        {
            get
            {
                return PowerCalc.Licensing.License.LicenseName;
            }
        }

        public bool LicenseInfoVisibility
        {
            get
            {
                return PowerCalc.Licensing.License.IsLicensed;
            }
        }

        public bool UnregisteredTextVisibility
        {
            get
            {
                return !LicenseInfoVisibility;
            }
        }

        public string WindowTitle
        {
            get
            {
                return "About " + ProductTitle;
            }
        }

        #endregion

        #region Constructors

        public AboutWindowViewModel(ApplicationInfo applicationInfo)
        {
            ApplicationInfo = applicationInfo;
            ResourcesManager = new ResourceManager(typeof(Properties.Resources));
        }

        #endregion
    }
}