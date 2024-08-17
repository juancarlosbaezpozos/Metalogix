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
    public class AddUserOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AddOptions
    {
        private bool allowDBWriteField;

        private bool allowDBWriteEnvironmentField;

        public bool AllowDBWrite
        {
            get { return this.allowDBWriteField; }
            set { this.allowDBWriteField = value; }
        }

        public bool AllowDBWriteEnvironment
        {
            get { return this.allowDBWriteEnvironmentField; }
            set { this.allowDBWriteEnvironmentField = value; }
        }

        public AddUserOptions()
        {
        }
    }
}