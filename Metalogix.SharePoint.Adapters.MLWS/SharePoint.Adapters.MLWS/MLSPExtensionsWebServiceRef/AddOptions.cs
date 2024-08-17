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
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddAudienceOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddItemOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddSiteCollectionOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddUserOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddWebOptions))]
    [XmlType(Namespace = "http://www.metalogix.net/")]
    public class AddOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AdapterOptions
    {
        private bool overwriteField;

        public bool Overwrite
        {
            get { return this.overwriteField; }
            set { this.overwriteField = value; }
        }

        public AddOptions()
        {
        }
    }
}