using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddItemOptions : AddOptions
    {
        private bool m_bPreserveID;

        private bool m_bAllowDBWriting;

        [DataMember]
        public bool AllowDBWriting
        {
            get { return this.m_bAllowDBWriting; }
            set { this.m_bAllowDBWriting = value; }
        }

        [DataMember]
        public bool PreserveID
        {
            get { return this.m_bPreserveID; }
            set { this.m_bPreserveID = value; }
        }

        [DataMember]
        public bool ShallowCopyExternalizedData
        {
            get { return false; }
            set { }
        }

        public AddItemOptions()
        {
        }
    }
}