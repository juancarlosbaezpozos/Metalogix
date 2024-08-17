using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class GetListItemOptions : AdapterOptions
    {
        private bool m_bIncludePermissionsInheritance;

        private bool m_bIncludeExternalizationData;

        [DataMember]
        public bool IncludeExternalizationData
        {
            get { return this.m_bIncludeExternalizationData; }
            set { this.m_bIncludeExternalizationData = value; }
        }

        [DataMember]
        public bool IncludePermissionsInheritance
        {
            get { return this.m_bIncludePermissionsInheritance; }
            set { this.m_bIncludePermissionsInheritance = value; }
        }

        public GetListItemOptions()
        {
        }
    }
}