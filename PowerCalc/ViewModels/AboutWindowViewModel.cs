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

        protected readonly LicenseBase AppLicense;
        internal ResourceManager ResourcesManager;

        #endregion

        #region Properties

        public virtual AssemblyInfo AssemblyInfo { get; set; }

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