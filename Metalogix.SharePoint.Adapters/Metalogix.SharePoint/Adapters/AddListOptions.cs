using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddListOptions : AddOptions, IUpdateListOptions
    {
        private bool m_bCopyViews = true;

        private bool m_bEnsureUrlNameMatchesInput = true;

        private bool m_bCopyFields = true;

        private bool m_bUpdateFieldTypes;

        private bool m_bCopyEnableAssignToEmail;

        private bool m_bDeletePreExistingViews;

        [DataMember]
        public bool CopyEnableAssignToEmail
        {
            get { return this.m_bCopyEnableAssignToEmail; }
            set { this.m_bCopyEnableAssignToEmail = value; }
        }

        [DataMember]
        public bool CopyFields
        {
            get { return this.m_bCopyFields; }
            set { this.m_bCopyFields = value; }
        }

        [DataMember]
        public bool CopyViews
        {
            get { return this.m_bCopyViews; }
            set { this.m_bCopyViews = value; }
        }

        [DataMember]
        public bool DeletePreExistingViews
        {
            get { return this.m_bDeletePreExistingViews; }
            set { this.m_bDeletePreExistingViews = value; }
        }

        [DataMember]
        public bool EnsureUrlNameMatchesInput
        {
            get { return this.m_bEnsureUrlNameMatchesInput; }
            set { this.m_bEnsureUrlNameMatchesInput = value; }
        }

        [DataMember]
        public bool UpdateFieldTypes
        {
            get { return this.m_bUpdateFieldTypes; }
            set { this.m_bUpdateFieldTypes = value; }
        }

        public AddListOptions()
        {
        }
    }
}