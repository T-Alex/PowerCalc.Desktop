using System;
using System.Reflection;
using System.Resources;
using System.Text;
using TAlex.Common.Models;
using TAlex.License;


namespace TAlex.PowerCalc.ViewModels
{
    public class AboutWindowViewModel
    {
        #region Fields

        protected readonly AssemblyInfo AssemblyInfo;
        protected readonly LicenseBase AppLicense;
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
                return AssemblyInfo.Title;
            }
        }

        /// <summary>
        /// Gets the application's description.
        /// </summary>
        public string Description
        {
            get
            {
                return AssemblyInfo.Description;
            }
        }

        /// <summary>
        /// Gets the application's company.
        /// </summary>
        public string Company
        {
            get
            {
                return AssemblyInfo.Company;
            }
        }

        /// <summary>
        /// Gets the application's product.
        /// </summary>
        public string Product
        {
            get
            {
                return AssemblyInfo.Product;
            }
        }

        /// <summary>
        /// Gets the application's copyright.
        /// </summary>
        public string Copyright
        {
            get
            {
                return String.Format("{0}. All rights reserved.", AssemblyInfo.Copyright);
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
                return ResourcesManager.GetString("SupportEmail");
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
                return ResourcesManager.GetString("HomepageUrl");
            }
        }

        /// <summary>
        /// Gets the license name for this product.
        /// </summary>
        public string LicenseName
        {
            get
            {
                return AppLicense.LicenseName;
            }
        }

        public bool LicenseInfoVisibility
        {
            get
            {
                return AppLicense.IsLicensed;
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

        public AboutWindowViewModel(AssemblyInfo assemblyInfo, LicenseBase appLicense)
        {
            AssemblyInfo = assemblyInfo;
            AppLicense = appLicense;
            ResourcesManager = new ResourceManager(typeof(Properties.Resources));
        }

        #endregion
    }
}