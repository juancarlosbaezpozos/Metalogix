using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddDocumentOptions : AddItemOptions, IUpdateDocumentOptions
    {
        private bool m_bCorrectInvalidNames;

        private bool m_bOverrideCheckouts = true;

        private bool m_bSideLoadDocumentsToStoragePoint;

        private bool m_bPreserveSharePointDocumentIDs;

        [DataMember]
        public bool CorrectInvalidNames
        {
            get { return this.m_bCorrectInvalidNames; }
            set { this.m_bCorrectInvalidNames = value; }
        }

        [DataMember] public int FileChunkSizeInMB { get; set; }

        [DataMember]
        public bool OverrideCheckouts
        {
            get { return this.m_bOverrideCheckouts; }
            set { this.m_bOverrideCheckouts = value; }
        }

        [DataMember]
        public bool PreserveSharePointDocumentIDs
        {
            get { return this.m_bPreserveSharePointDocumentIDs; }
            set { this.m_bPreserveSharePointDocumentIDs = value; }
        }

        [DataMember]
        public bool SideLoadDocumentsToStoragePoint
        {
            get { return this.m_bSideLoadDocumentsToStoragePoint; }
            set { this.m_bSideLoadDocumentsToStoragePoint = value; }
        }

        public AddDocumentOptions()
        {
        }
    }
}