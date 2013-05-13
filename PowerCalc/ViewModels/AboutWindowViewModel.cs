using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TAlex.Common.Environment;


namespace TAlex.PowerCalc.ViewModels
{
    public class AboutWindowViewModel
    {
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
                return PowerCalc.Properties.Resources.SupportEmail;
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
                return PowerCalc.Properties.Resources.HomepageUrl;
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

        public Visibility LicenseInfoVisibility
        {
            get
            {
                if (PowerCalc.Licensing.License.IsLicensed)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public Visibility UnregisteredTextVisibility
        {
            get
            {
                if (LicenseInfoVisibility == Visibility.Visible)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
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
    }
}