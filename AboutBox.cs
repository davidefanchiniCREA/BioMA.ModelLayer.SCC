using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CRA.ModelLayer.SCC
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            //etichetta da assembly
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.lblVersion.Text = String.Format("Version {0} (Assembly)", AssemblyVersion);
            this.lblCopyright.Text = AssemblyCopyright;
            this.lblCompany.Text = AssemblyCompany;
            this.lblProductName.Text = AssemblyProduct;

            //etichetta da manifest ClickOnce
            //if (ApplicationDeployment.IsNetworkDeployed)
            //    this.lblVersionClickOnce.Text =
            //        String.Format("Version {0} (ClickOnce)", 
            //        ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString());
            //else
                this.lblVersionClickOnce.Text =
                    String.Format("Version {0}",
                    "<case not available> (ClickOnce)");
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void btnViewLicense_Click(object sender, EventArgs e)
        {
            MLLicense lic = new MLLicense();
            lic.Show();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, @"Help\\SCC_User_Guide.chm", HelpNavigator.TableOfContents);
        }

      
    }
}
