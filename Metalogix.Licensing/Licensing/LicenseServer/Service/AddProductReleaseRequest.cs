using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.Licensing.LicenseServer.Service
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.6.1064.2")]
    [Serializable]
    [XmlInclude(typeof(ModifyProductReleaseRequest))]
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public class AddProductReleaseRequest
    {
        private int productCodeField;

        private string versionField;

        private DateTime releaseDateField;

        private VisibilityType visibilityField;

        private string installationFileUrlField;

        private string releaseNotesUrlField;

        private string installationFileNameField;

        private string clcVersionField;

        public string ClcVersion
        {
            get { return this.clcVersionField; }
            set { this.clcVersionField = value; }
        }

        public string InstallationFileName
        {
            get { return this.installationFileNameField; }
            set { this.installationFileNameField = value; }
        }

        public string InstallationFileUrl
        {
            get { return this.installationFileUrlField; }
            set { this.installationFileUrlField = value; }
        }

        public int ProductCode
        {
            get { return this.productCodeField; }
            set { this.productCodeField = value; }
        }

        public DateTime ReleaseDate
        {
            get { return this.releaseDateField; }
            set { this.releaseDateField = value; }
        }

        public string ReleaseNotesUrl
        {
            get { return this.releaseNotesUrlField; }
            set { this.releaseNotesUrlField = value; }
        }

        public string Version
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }

        public VisibilityType Visibility
        {
            get { return this.visibilityField; }
            set { this.visibilityField = value; }
        }

        public AddProductReleaseRequest()
        {
        }
    }
}