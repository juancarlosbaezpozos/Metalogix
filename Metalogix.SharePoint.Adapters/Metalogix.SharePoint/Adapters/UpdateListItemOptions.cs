using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    [DataContract]
    public class UpdateListItemOptions : UpdateItemOptions, IUpdateListItemOptions
    {
        private bool _preserveSharePointDocumentIDs;

        [DataMember]
        public bool PreserveSharePointDocumentIDs
        {
            get { return this._preserveSharePointDocumentIDs; }
            set { this._preserveSharePointDocumentIDs = value; }
        }

        public UpdateListItemOptions()
        {
        }
    }
}