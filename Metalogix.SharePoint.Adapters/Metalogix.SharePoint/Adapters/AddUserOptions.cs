using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddUserOptions : AddOptions
    {
        private bool m_bAllowDBWrite;

        private bool m_bAllowDBWriteEnvironment;

        [DataMember]
        public bool AllowDBWrite
        {
            get { return this.m_bAllowDBWrite; }
            set { this.m_bAllowDBWrite = value; }
        }

        [DataMember]
        public bool AllowDBWriteEnvironment
        {
            get { return this.m_bAllowDBWriteEnvironment; }
            set { this.m_bAllowDBWriteEnvironment = value; }
        }

        public AddUserOptions()
        {
        }
    }
}