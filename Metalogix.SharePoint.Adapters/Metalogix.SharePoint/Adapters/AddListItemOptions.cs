using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class AddListItemOptions : AddItemOptions, IUpdateListItemOptions
    {
        private bool m_bInitialVersion;

        private int? m_iParentID = new int?(0);

        private bool _preserveSharePointDocumentIDs;

        [DataMember]
        public bool InitialVersion
        {
            get { return this.m_bInitialVersion; }
            set { this.m_bInitialVersion = value; }
        }

        [DataMember]
        public int? ParentID
        {
            get { return this.m_iParentID; }
            set
            {
                if (value.HasValue)
                {
                    this.m_iParentID = value;
                    return;
                }

                this.m_iParentID = new int?(0);
            }
        }

        [DataMember] public int PredictedNextAvailableID { get; set; }

        [DataMember]
        public bool PreserveSharePointDocumentIDs
        {
            get { return this._preserveSharePointDocumentIDs; }
            set { this._preserveSharePointDocumentIDs = value; }
        }

        public AddListItemOptions()
        {
        }
    }
}