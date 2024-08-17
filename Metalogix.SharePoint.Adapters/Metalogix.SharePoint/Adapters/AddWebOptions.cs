using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddWebOptions : AddOptions, IUpdateWebOptions
    {
        private bool m_bCopyCoreMetaData = true;

        private bool m_bCopyFeatures;

        private bool m_bMergeFeatures;

        private bool m_bCopyNavigation;

        private bool m_bApplyTheme;

        private bool m_bApplyMasterPage;

        private bool m_bPreserveUIVersion;

        private bool m_bCopyAccessRequestSettings;

        private bool m_bCopyAssociatedGroupSettings = true;

        [DataMember]
        public bool ApplyMasterPage
        {
            get { return this.m_bApplyMasterPage; }
            set { this.m_bApplyMasterPage = value; }
        }

        [DataMember]
        public bool ApplyTheme
        {
            get { return this.m_bApplyTheme; }
            set { this.m_bApplyTheme = value; }
        }

        [DataMember]
        public bool CopyAccessRequestSettings
        {
            get { return this.m_bCopyAccessRequestSettings; }
            set { this.m_bCopyAccessRequestSettings = value; }
        }

        [DataMember]
        public bool CopyAssociatedGroupSettings
        {
            get { return this.m_bCopyAssociatedGroupSettings; }
            set { this.m_bCopyAssociatedGroupSettings = value; }
        }

        [DataMember]
        public bool CopyCoreMetaData
        {
            get { return this.m_bCopyCoreMetaData; }
            set { this.m_bCopyCoreMetaData = value; }
        }

        [DataMember]
        public bool CopyFeatures
        {
            get { return this.m_bCopyFeatures; }
            set { this.m_bCopyFeatures = value; }
        }

        [DataMember]
        public bool CopyNavigation
        {
            get { return this.m_bCopyNavigation; }
            set { this.m_bCopyNavigation = value; }
        }

        [DataMember]
        public bool MergeFeatures
        {
            get { return this.m_bMergeFeatures; }
            set { this.m_bMergeFeatures = value; }
        }

        [DataMember]
        public bool PreserveUIVersion
        {
            get { return this.m_bPreserveUIVersion; }
            set { this.m_bPreserveUIVersion = value; }
        }

        public AddWebOptions()
        {
        }
    }
}