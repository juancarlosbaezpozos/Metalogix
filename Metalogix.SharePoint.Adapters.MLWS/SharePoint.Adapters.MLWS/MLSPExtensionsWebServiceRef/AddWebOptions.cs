using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.34230")]
    [Serializable]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddSiteCollectionOptions))]
    [XmlType(Namespace = "http://www.metalogix.net/")]
    public class AddWebOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddOptions
    {
        private bool copyCoreMetaDataField;

        private bool copyFeaturesField;

        private bool mergeFeaturesField;

        private bool copyNavigationField;

        private bool applyThemeField;

        private bool applyMasterPageField;

        private bool preserveUIVersionField;

        private bool copyAccessRequestSettingsField;

        private bool copyAssociatedGroupSettingsField;

        public bool ApplyMasterPage
        {
            get { return this.applyMasterPageField; }
            set { this.applyMasterPageField = value; }
        }

        public bool ApplyTheme
        {
            get { return this.applyThemeField; }
            set { this.applyThemeField = value; }
        }

        public bool CopyAccessRequestSettings
        {
            get { return this.copyAccessRequestSettingsField; }
            set { this.copyAccessRequestSettingsField = value; }
        }

        public bool CopyAssociatedGroupSettings
        {
            get { return this.copyAssociatedGroupSettingsField; }
            set { this.copyAssociatedGroupSettingsField = value; }
        }

        public bool CopyCoreMetaData
        {
            get { return this.copyCoreMetaDataField; }
            set { this.copyCoreMetaDataField = value; }
        }

        public bool CopyFeatures
        {
            get { return this.copyFeaturesField; }
            set { this.copyFeaturesField = value; }
        }

        public bool CopyNavigation
        {
            get { return this.copyNavigationField; }
            set { this.copyNavigationField = value; }
        }

        public bool MergeFeatures
        {
            get { return this.mergeFeaturesField; }
            set { this.mergeFeaturesField = value; }
        }

        public bool PreserveUIVersion
        {
            get { return this.preserveUIVersionField; }
            set { this.preserveUIVersionField = value; }
        }

        public AddWebOptions()
        {
        }
    }
}