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
    public class GetListItemOptions : Metalogix.SharePoint.Adapters.MLWS.MLSPExtensionsWebServiceRef.AdapterOptions
    {
        private bool includePermissionsInheritanceField;

        private bool includeExternalizationDataField;

        public bool IncludeExternalizationData
        {
            get { return this.includeExternalizationDataField; }
            set { this.includeExternalizationDataField = value; }
        }

        public bool IncludePermissionsInheritance
        {
            get { return this.includePermissionsInheritanceField; }
            set { this.includePermissionsInheritanceField = value; }
        }

        public GetListItemOptions()
        {
        }
    }
}