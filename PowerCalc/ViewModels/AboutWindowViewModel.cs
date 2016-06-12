using System;
using System.Resources;
using TAlex.Common.Models;


namespace TAlex.PowerCalc.ViewModels
{
    public class AboutWindowViewModel
    {
        #region Fields

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

        #endregion

        #region Constructors

        public AboutWindowViewModel(AssemblyInfo assemblyInfo)
        {
            AssemblyInfo = assemblyInfo;
            ResourcesManager = new ResourceManager(typeof(Properties.Resources));
        }

        #endregion
    }
}