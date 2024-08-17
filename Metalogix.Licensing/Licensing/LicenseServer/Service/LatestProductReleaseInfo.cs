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
    [XmlType(Namespace = "http://www.metalogix.com/")]
    public class LatestProductReleaseInfo
    {
        private string versionField;

        private string installationFileUrlField;

        private string installationFileNameField;

        private string releaseNotesUrlField;

        private string relaseDateField;

        private string latestVersionField;

        private bool latestVersionIsAllowedByMaintenanceField;

        private string latestVersionReleaseNotesUrlField;

        private string latestReleaseDateField;

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

        public string LatestReleaseDate
        {
            get { return this.latestReleaseDateField; }
            set { this.latestReleaseDateField = value; }
        }

        public string LatestVersion
        {
            get { return this.latestVersionField; }
            set { this.latestVersionField = value; }
        }

        public bool LatestVersionIsAllowedByMaintenance
        {
            get { return this.latestVersionIsAllowedByMaintenanceField; }
            set { this.latestVersionIsAllowedByMaintenanceField = value; }
        }

        public string LatestVersionReleaseNotesUrl
        {
            get { return this.latestVersionReleaseNotesUrlField; }
            set { this.latestVersionReleaseNotesUrlField = value; }
        }

        public string RelaseDate
        {
            get { return this.relaseDateField; }
            set { this.relaseDateField = value; }
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

        public LatestProductReleaseInfo()
        {
        }
    }
}