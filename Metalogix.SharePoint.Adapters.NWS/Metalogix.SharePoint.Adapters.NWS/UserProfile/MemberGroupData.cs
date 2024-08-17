using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService")]
    public class MemberGroupData
    {
        private Guid sourceInternalField;

        private string sourceReferenceField;

        public Guid SourceInternal
        {
            get { return this.sourceInternalField; }
            set { this.sourceInternalField = value; }
        }

        public string SourceReference
        {
            get { return this.sourceReferenceField; }
            set { this.sourceReferenceField = value; }
        }

        public MemberGroupData()
        {
        }
    }
}