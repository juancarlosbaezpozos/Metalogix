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
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateDocumentOptions))]
    [XmlInclude(typeof(Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateListItemOptions))]
    [XmlType(Namespace = "http://www.metalogix.net/")]
    public class UpdateItemOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AdapterOptions
    {
        private bool shallowCopyExternalizedDataField;

        public bool ShallowCopyExternalizedData
        {
            get { return this.shallowCopyExternalizedDataField; }
            set { this.shallowCopyExternalizedDataField = value; }
        }

        public UpdateItemOptions()
        {
        }
    }
}