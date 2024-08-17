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
    public class AddDocumentOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddItemOptions
    {
        private bool correctInvalidNamesField;

        private bool overrideCheckoutsField;

        private bool sideLoadDocumentsToStoragePointField;

        private bool preserveSharePointDocumentIDsField;

        public bool CorrectInvalidNames
        {
            get { return this.correctInvalidNamesField; }
            set { this.correctInvalidNamesField = value; }
        }

        public bool OverrideCheckouts
        {
            get { return this.overrideCheckoutsField; }
            set { this.overrideCheckoutsField = value; }
        }

        public bool PreserveSharePointDocumentIDs
        {
            get { return this.preserveSharePointDocumentIDsField; }
            set { this.preserveSharePointDocumentIDsField = value; }
        }

        public bool SideLoadDocumentsToStoragePoint
        {
            get { return this.sideLoadDocumentsToStoragePointField; }
            set { this.sideLoadDocumentsToStoragePointField = value; }
        }

        public AddDocumentOptions()
        {
        }
    }
}