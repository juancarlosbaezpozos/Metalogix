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
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddDocumentOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddFolderOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddListItemOptions))]
    [XmlType(Namespace = "http://www.metalogix.net/")]
    public class AddItemOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddOptions
    {
        private bool shallowCopyExternalizedDataField;

        private bool preserveIDField;

        private bool allowDBWritingField;

        public bool AllowDBWriting
        {
            get { return this.allowDBWritingField; }
            set { this.allowDBWritingField = value; }
        }

        public bool PreserveID
        {
            get { return this.preserveIDField; }
            set { this.preserveIDField = value; }
        }

        public bool ShallowCopyExternalizedData
        {
            get { return this.shallowCopyExternalizedDataField; }
            set { this.shallowCopyExternalizedDataField = value; }
        }

        public AddItemOptions()
        {
        }
    }
}