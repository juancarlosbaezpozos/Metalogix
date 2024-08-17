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
    [XmlType(Namespace = "http://www.metalogix.net/")]
    public class
        UpdateListItemOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.UpdateItemOptions
    {
        private bool preserveSharePointDocumentIDsField;

        public bool PreserveSharePointDocumentIDs
        {
            get { return this.preserveSharePointDocumentIDsField; }
            set { this.preserveSharePointDocumentIDsField = value; }
        }

        public UpdateListItemOptions()
        {
        }
    }
}