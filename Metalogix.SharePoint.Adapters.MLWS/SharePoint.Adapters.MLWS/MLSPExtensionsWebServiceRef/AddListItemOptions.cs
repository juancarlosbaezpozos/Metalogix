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
    public class AddListItemOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddItemOptions
    {
        private bool initialVersionField;

        private int? parentIDField;

        private int predictedNextAvailableIDField;

        private bool preserveSharePointDocumentIDsField;

        public bool InitialVersion
        {
            get { return this.initialVersionField; }
            set { this.initialVersionField = value; }
        }

        [XmlElement(IsNullable = true)]
        public int? ParentID
        {
            get { return this.parentIDField; }
            set { this.parentIDField = value; }
        }

        public int PredictedNextAvailableID
        {
            get { return this.predictedNextAvailableIDField; }
            set { this.predictedNextAvailableIDField = value; }
        }

        public bool PreserveSharePointDocumentIDs
        {
            get { return this.preserveSharePointDocumentIDsField; }
            set { this.preserveSharePointDocumentIDsField = value; }
        }

        public AddListItemOptions()
        {
        }
    }
}